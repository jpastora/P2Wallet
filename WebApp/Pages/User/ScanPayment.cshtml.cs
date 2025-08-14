using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using DTOs;
using CoreApp;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Linq;

namespace WebApp.Pages.User
{
    [Authorize]
    public class ScanPaymentModel : PageModel
    {
        [BindProperty]
        public string PaymentCode { get; set; } = string.Empty;
        [BindProperty]
        public int SelectedBankAccountId { get; set; }
        [BindProperty]
        public int? SelectedPromotionId { get; set; }
        [BindProperty]
        public string SelectedPromotionType { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
        public bool PaymentRequestFound { get; set; } = false;
        public bool PaymentExecuted { get; set; } = false;

        public DTOs.User CurrentUser { get; set; } = new();
        public List<BankAccountInfo> UserAccountsInfo { get; set; } = new();
        public Transaction? PaymentRequest { get; set; }
        public List<PromotionInfo> AvailablePromotions { get; set; } = new();
        public DTOs.Merchant? MerchantInfo { get; set; }
        public PaymentExecutionResponse? ExecutionResult { get; set; }

        private readonly IHttpClientFactory _httpClientFactory;

        public ScanPaymentModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private bool IsPaymentRequestExpired(DateTime? expiresAt) => expiresAt.HasValue && expiresAt.Value < DateTime.Now;

        public async Task<IActionResult> OnGetAsync([FromQuery] string? code = null)
        {
            await LoadUserData();
            if (CurrentUser == null) return RedirectToPage("/Login");
            if (!string.IsNullOrEmpty(code))
            {
                PaymentCode = code;
                return await OnPostScanAsync();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostScanAsync()
        {
            await LoadUserData();
            if (CurrentUser == null) return RedirectToPage("/Login");
            if (string.IsNullOrWhiteSpace(PaymentCode))
            {
                Message = "Ingresa un código de pago válido.";
                return Page();
            }
            await FetchAndPopulatePaymentRequest(PaymentCode.Trim());
            return Page();
        }

        public async Task<IActionResult> OnPostExecuteAsync()
        {
            await LoadUserData();
            if (CurrentUser == null) return RedirectToPage("/Login");

            if (string.IsNullOrWhiteSpace(PaymentCode))
            {
                Message = "Código de pago requerido.";
                return Page();
            }
            if (SelectedBankAccountId <= 0)
            {
                Message = "Selecciona una cuenta bancaria.";
                await FetchAndPopulatePaymentRequest(PaymentCode.Trim());
                return Page();
            }

            // Re-validar solicitud fresca desde API antes de ejecutar
            await FetchAndPopulatePaymentRequest(PaymentCode.Trim());
            if (!PaymentRequestFound || PaymentRequest == null)
            {
                Message = "No se pudo encontrar la solicitud de pago.";
                return Page();
            }
            if (IsPaymentRequestExpired(PaymentRequest.ExpiresAt))
            {
                Message = "La solicitud de pago ha expirado.";
                return Page();
            }

            try
            {
                var selectedAccount = UserAccountsInfo.FirstOrDefault(a => a.ID == SelectedBankAccountId);
                if (selectedAccount == null)
                {
                    Message = "Cuenta bancaria inválida.";
                    return Page();
                }
                if (!selectedAccount.HasSufficientFunds(PaymentRequest.GrossAmount))
                {
                    Message = "Fondos insuficientes en la cuenta seleccionada.";
                    return Page();
                }

                var httpClient = _httpClientFactory.CreateClient();
                var apiUrl = GetApiBaseUrl() + "/api/PaymentRequest/Execute";
                var executeRequest = new
                {
                    PaymentRequestCode = PaymentCode.Trim(),
                    UserId = CurrentUser.ID,
                    BankAccountId = SelectedBankAccountId,
                    PromotionId = SelectedPromotionId,
                    PromotionType = NormalizePromotionType(SelectedPromotionType)
                };
                var jsonContent = new StringContent(JsonConvert.SerializeObject(executeRequest), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(apiUrl, jsonContent);
                if (response.IsSuccessStatusCode)
                {
                    ExecutionResult = JsonConvert.DeserializeObject<PaymentExecutionResponse>(await response.Content.ReadAsStringAsync());
                    if (ExecutionResult?.Success == true)
                    {
                        PaymentExecuted = true;
                        TempData["SuccessMessage"] = "¡Pago realizado exitosamente!";
                        return RedirectToPage("/User/PaymentSuccess", new { transactionId = ExecutionResult.TransactionId });
                    }
                    Message = ExecutionResult?.Message ?? "Error al procesar el pago.";
                }
                else
                {
                    Message = await BuildErrorMessage(response);
                }
            }
            catch (Exception ex)
            {
                Message = $"Error inesperado al procesar el pago: {ex.Message}";
            }

            // Re-cargar datos para mostrar nuevamente formulario y promociones actualizadas
            await FetchAndPopulatePaymentRequest(PaymentCode.Trim());
            return Page();
        }

        public async Task<IActionResult> OnPostPreviewAsync()
        {
            await LoadUserData();
            if (CurrentUser == null) return RedirectToPage("/Login");
            if (string.IsNullOrWhiteSpace(PaymentCode))
            {
                Message = "Código de pago requerido.";
                return Page();
            }
            if (SelectedBankAccountId <= 0)
            {
                Message = "Selecciona una cuenta bancaria.";
                await FetchAndPopulatePaymentRequest(PaymentCode.Trim());
                return Page();
            }
            // Normalizar tipo de promo (puede venir vacio)
            var promoTypeNorm = NormalizePromotionType(SelectedPromotionType);
            return RedirectToPage("/User/ConfirmPayment", new { paymentCode = PaymentCode.Trim(), bankAccountId = SelectedBankAccountId, promotionId = SelectedPromotionId, promotionType = promoTypeNorm });
        }

        // =====================
        // Helper principal de carga de solicitud
        // =====================
        private async Task FetchAndPopulatePaymentRequest(string code)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var apiUrl = GetApiBaseUrl() + $"/api/PaymentRequest/GetByCode/{code}";
                var response = await httpClient.GetAsync(apiUrl);
                if (!response.IsSuccessStatusCode)
                {
                    PaymentRequestFound = false;
                    Message = "Solicitud no encontrada o expirada.";
                    return;
                }
                var paymentResponse = JsonConvert.DeserializeObject<PaymentRequestResponse>(await response.Content.ReadAsStringAsync());
                if (paymentResponse == null)
                {
                    PaymentRequestFound = false;
                    Message = "Respuesta inválida del servidor.";
                    return;
                }
                if (IsPaymentRequestExpired(paymentResponse.ExpiresAt))
                {
                    PaymentRequestFound = false;
                    Message = "La solicitud de pago ha expirado.";
                    return;
                }
                PaymentRequest = new Transaction
                {
                    ID = paymentResponse.TransactionId,
                    PaymentRequestCode = paymentResponse.PaymentRequestCode,
                    Description = paymentResponse.Description,
                    GrossAmount = (double)paymentResponse.GrossAmount,
                    NetAmount = (double)paymentResponse.NetAmount,
                    SalesTaxAmount = (double)paymentResponse.SalesTaxAmount,
                    ExpiresAt = paymentResponse.ExpiresAt,
                    TransactionStatus = paymentResponse.Status,
                    MerchantID = GetMerchantIdFromTransaction(paymentResponse.TransactionId)
                };
                PaymentRequestFound = true;
                Message = "Solicitud de pago encontrada.";
                await LoadMerchantInfo();

                // Si ya hay una cuenta seleccionada, incluir promos financieras desde el inicio
                int? finId = null;
                if (SelectedBankAccountId > 0)
                {
                    var selected = UserAccountsInfo.FirstOrDefault(a => a.ID == SelectedBankAccountId);
                    if (selected != null) finId = selected.FinancialEntityID;
                }
                await LoadAvailablePromotions(code, finId); // promociones comercio + (opcional) financieras
            }
            catch
            {
                PaymentRequestFound = false;
                Message = "Error al cargar la solicitud de pago.";
            }
        }

        private string NormalizePromotionType(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            var t = input.Trim();
            if (t.Equals("Merchant", StringComparison.OrdinalIgnoreCase)) return "Merchant";
            if (t.Equals("Financial", StringComparison.OrdinalIgnoreCase) || t.Equals("FinancialEntity", StringComparison.OrdinalIgnoreCase)) return "Financial";
            return string.Empty;
        }

        private async Task<string> BuildErrorMessage(HttpResponseMessage response)
        {
            try
            {
                var content = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(content)) return content.Length > 300 ? content.Substring(0, 300) + "..." : content;
            }
            catch { }
            return $"Error HTTP {(int)response.StatusCode}";
        }

        public string GetTimeRemaining()
        {
            if (PaymentRequest?.ExpiresAt == null) return string.Empty;
            var timeLeft = PaymentRequest.ExpiresAt.Value - DateTime.Now;
            if (timeLeft.TotalSeconds <= 0) return "Expirado";
            if (timeLeft.TotalMinutes < 1) return $"{(int)timeLeft.TotalSeconds} segundos";
            if (timeLeft.TotalHours < 1) return $"{(int)timeLeft.TotalMinutes} minutos";
            return $"{(int)timeLeft.TotalHours}h {timeLeft.Minutes}m";
        }

        private async Task LoadUserData()
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email)) return;
            var userManager = new UserManager();
            CurrentUser = userManager.RetrieveUserByEmail(new DTOs.User { Email = email });
            if (CurrentUser == null) return;
            try
            {
                var accountManager = new BankAccountManager();
                var allAccounts = accountManager.RetrieveAllBankAccounts();
                var entityManager = new FinancialEntityManager();
                var allEntities = entityManager.RetrieveAllFinancialEntities();
                UserAccountsInfo = allAccounts
                    .Where(a => a.UserID == CurrentUser.ID && a.ValidationStatus == "Active")
                    .Select(a => {
                        var entity = allEntities.FirstOrDefault(e => e.ID == a.FinancialEntityID);
                        return new BankAccountInfo
                        {
                            ID = a.ID,
                            IBAN = a.IBAN,
                            BankName = entity?.EntityName ?? "Banco Desconocido",
                            Balance = a.Balance,
                            LogoUrl = entity?.LogoImage ?? "https://picsum.photos/seed/bank/40/40",
                            FinancialEntityID = a.FinancialEntityID
                        };
                    }).ToList();
            }
            catch { UserAccountsInfo = new(); }
        }

        private async Task LoadAvailablePromotions(string paymentCode, int? financialEntityId = null)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var url = GetApiBaseUrl() + $"/api/PaymentRequest/GetPromotions/{paymentCode}";
                if (financialEntityId.HasValue && financialEntityId.Value > 0)
                {
                    url += $"?financialEntityId={financialEntityId.Value}";
                }
                var response = await httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    AvailablePromotions = new();
                    return;
                }
                var content = await response.Content.ReadAsStringAsync();
                var promotions = JsonConvert.DeserializeObject<List<object>>(content) ?? new List<object>();
                ConvertPromotionsToInfo(promotions);
            }
            catch { AvailablePromotions = new(); }
        }

        // =============================
        // FIX: Permitir pasar directamente financialEntityId desde el cliente si la cuenta aún no se pudo resolver en servidor.
        // =============================
        public async Task<IActionResult> OnGetPromotionsAsync(string paymentCode, int accountId, int? financialEntityId = null)
        {
            try
            {
                if (string.IsNullOrEmpty(paymentCode))
                    return new JsonResult(new List<object>());

                int? finId = null;

                // Priorizar id financiero enviado explícitamente (nuevo)
                if (financialEntityId.HasValue && financialEntityId.Value > 0)
                {
                    finId = financialEntityId.Value;
                }
                else if (accountId > 0)
                {
                    // Fallback: resolver por cuenta (lógica anterior)
                    var accountManager = new BankAccountManager();
                    var account = accountManager.RetrieveBankAccountById(accountId);
                    if (account != null)
                    {
                        finId = account.FinancialEntityID;
                    }
                }

                // Construir URL con/ sin entidad financiera
                var httpClient = _httpClientFactory.CreateClient();
                var url = GetApiBaseUrl() + $"/api/PaymentRequest/GetPromotions/{paymentCode}" + (finId.HasValue ? $"?financialEntityId={finId.Value}" : string.Empty);
                var response = await httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    return new JsonResult(new List<object>());

                var promotions = JsonConvert.DeserializeObject<List<object>>(await response.Content.ReadAsStringAsync()) ?? new List<object>();
                var converted = ConvertPromotionsToInfo(promotions);
                return new JsonResult(converted);
            }
            catch { return new JsonResult(new List<object>()); }
        }

        private List<PromotionInfo> ConvertPromotionsToInfo(List<object> promotions)
        {
            var list = new List<PromotionInfo>();
            foreach (var p in promotions)
            {
                try
                {
                    // Intento 1: serialización a diccionario
                    Dictionary<string, object>? dict = null;
                    try
                    {
                        var json = JsonConvert.SerializeObject(p);
                        dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                    }
                    catch { /* ignore */ }

                    string id = string.Empty;
                    string type = string.Empty;
                    string name = string.Empty;
                    string desc = string.Empty;
                    double discount = 0d;
                    double maxRefund = 0d;

                    string[] idKeys = { "PromotionID", "ID", "Id", "MerchantPromotionID", "FinancialPromotionID" };
                    string[] typeKeys = { "PromotionType", "Type" };
                    string[] nameKeys = { "Name", "PromotionName", "MerchantPromotionName", "FinancialPromotionName" };
                    string[] descKeys = { "Description", "MerchantPromotionDescription", "FinancialPromotionDescription" };
                    string[] discountKeys = { "DiscountPercentage", "Percentage", "Discount", "Percent", "DiscountValue" };
                    string[] maxRefundKeys = { "MaxRefund", "MaxReturn" };

                    if (dict != null)
                    {
                        id = GetValueSafely(dict, idKeys);
                        type = GetValueSafely(dict, typeKeys);
                        name = GetValueSafely(dict, nameKeys, "Promoción");
                        desc = GetValueSafely(dict, descKeys);
                        discount = GetNumericSafely(dict, discountKeys);
                        maxRefund = GetNumericSafely(dict, maxRefundKeys);
                    }

                    // Fallback con reflexión si faltan datos clave
                    var pType = p.GetType();
                    if (string.IsNullOrWhiteSpace(id))
                    {
                        id = TryReflection(pType, p, idKeys);
                    }
                    if (string.IsNullOrWhiteSpace(type))
                    {
                        type = TryReflection(pType, p, typeKeys);
                    }
                    if (string.IsNullOrWhiteSpace(name) || name == "Promoción")
                    {
                        name = TryReflection(pType, p, nameKeys) ?? name;
                        // Si aún vacío, tomar primera propiedad que contenga "Name"
                        if (string.IsNullOrWhiteSpace(name))
                        {
                            var anyNameProp = pType.GetProperties().FirstOrDefault(pr => pr.Name.Contains("Name", StringComparison.OrdinalIgnoreCase));
                            if (anyNameProp != null)
                            {
                                name = anyNameProp.GetValue(p)?.ToString() ?? name;
                            }
                        }
                    }
                    if (string.IsNullOrWhiteSpace(desc))
                    {
                        desc = TryReflection(pType, p, descKeys) ?? desc;
                        if (string.IsNullOrWhiteSpace(desc))
                        {
                            var anyDescProp = pType.GetProperties().FirstOrDefault(pr => pr.Name.Contains("Desc", StringComparison.OrdinalIgnoreCase));
                            if (anyDescProp != null)
                            {
                                desc = anyDescProp.GetValue(p)?.ToString() ?? desc;
                            }
                        }
                    }
                    if (discount == 0d)
                    {
                        var discountStr = TryReflection(pType, p, discountKeys);
                        discount = ParsePercentage(discountStr, discount);
                    }
                    if (maxRefund == 0d)
                    {
                        var maxRefStr = TryReflection(pType, p, maxRefundKeys);
                        maxRefund = ParseDouble(maxRefStr, maxRefund);
                    }

                    // Normalizar tipo
                    type = NormalizePromotionType(type);
                    if (string.IsNullOrWhiteSpace(type)) type = "Merchant";

                    list.Add(new PromotionInfo
                    {
                        Id = id,
                        Type = type,
                        Name = string.IsNullOrWhiteSpace(name) ? "Promoción" : name,
                        Description = desc ?? string.Empty,
                        DiscountPercentage = discount,
                        MaxRefund = maxRefund
                    });
                }
                catch { /* skip */ }
            }
            AvailablePromotions = list;
            return list;
        }

        // Helpers específicos para extracción dentro de ConvertPromotionsToInfo (sobrecargas locales)
        private string GetValueSafely(Dictionary<string, object> data, string[] keys, string fallback = "")
        {
            if (data == null || data.Count == 0) return fallback;
            foreach (var k in keys)
            {
                // Búsqueda case-insensitive
                var match = data.FirstOrDefault(d => string.Equals(d.Key, k, StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrEmpty(match.Key) && match.Value != null)
                {
                    var v = match.Value.ToString();
                    if (!string.IsNullOrWhiteSpace(v)) return v;
                }
            }
            return fallback;
        }

        private double GetNumericSafely(Dictionary<string, object> data, string[] keys)
        {
            if (data == null || data.Count == 0) return 0d;
            foreach (var k in keys)
            {
                var match = data.FirstOrDefault(d => string.Equals(d.Key, k, StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrEmpty(match.Key) && match.Value != null)
                {
                    var valStr = match.Value.ToString();
                    var parsed = ParsePercentage(valStr, double.NaN);
                    if (!double.IsNaN(parsed)) return parsed;
                }
            }
            return 0d;
        }

        private string TryReflection(Type t, object instance, string[] keys)
        {
            foreach (var key in keys)
            {
                var prop = t.GetProperty(key);
                if (prop != null && prop.CanRead)
                {
                    var v = prop.GetValue(instance)?.ToString();
                    if (!string.IsNullOrWhiteSpace(v)) return v;
                }
            }
            return string.Empty;
        }

        private double ParsePercentage(string? raw, double fallback)
        {
            if (string.IsNullOrWhiteSpace(raw)) return fallback;
            raw = raw.Trim();
            if (raw.EndsWith("%")) raw = raw[..^1];
            if (double.TryParse(raw, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var d)) return d;
            if (double.TryParse(raw, out d)) return d;
            return fallback;
        }

        private double ParseDouble(string? raw, double fallback)
        {
            if (string.IsNullOrWhiteSpace(raw)) return fallback;
            if (double.TryParse(raw, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var d)) return d;
            if (double.TryParse(raw, out d)) return d;
            return fallback;
        }

        private async Task LoadMerchantInfo()
        {
            if (PaymentRequest?.MerchantID > 0)
            {
                try
                {
                    var merchantManager = new MerchantManager();
                    MerchantInfo = merchantManager.RetrieveMerchantById(PaymentRequest.MerchantID);
                }
                catch { MerchantInfo = null; }
            }
        }

        private int GetMerchantIdFromTransaction(int transactionId)
        {
            try
            {
                var tm = new TransactionManager();
                var tx = tm.RetrieveTransactionById(transactionId);
                return tx?.MerchantID ?? 0;
            }
            catch { return 0; }
        }

        private string GetApiBaseUrl() => "https://p2wallet-api-eyefddgeeda9c2fk.eastus-01.azurewebsites.net";

        // DTOs internos API
        public class PaymentRequestResponse
        {
            public int TransactionId { get; set; }
            public string PaymentRequestCode { get; set; } = string.Empty;
            public string QRCodeUrl { get; set; } = string.Empty;
            public string QRContent { get; set; } = string.Empty;
            public decimal GrossAmount { get; set; }
            public decimal NetAmount { get; set; }
            public decimal SalesTaxAmount { get; set; }
            public DateTime ExpiresAt { get; set; }
            public string Status { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
        }
        public class PaymentExecutionResponse
        {
            public bool Success { get; set; }
            public int TransactionId { get; set; }
            public decimal FinalAmount { get; set; }
            public decimal DiscountApplied { get; set; }
            public string Message { get; set; } = string.Empty;
        }
        public class PromotionInfo
        {
            public string Id { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public double DiscountPercentage { get; set; }
            public double MaxRefund { get; set; }
            public string TypeBadgeClass => Type == "Merchant" ? "bg-success" : "bg-primary";
            public string TypeText => Type == "Merchant" ? "Comercio" : "Banco";
            public string DiscountText => DiscountPercentage > 0 ? $"{DiscountPercentage}% desc." : string.Empty;
        }
        public class BankAccountInfo
        {
            public int ID { get; set; }
            public string IBAN { get; set; } = string.Empty;
            public string BankName { get; set; } = string.Empty;
            public double Balance { get; set; }
            public string LogoUrl { get; set; } = string.Empty;
            public int FinancialEntityID { get; set; }
            public string DisplayText => $"{IBAN} - {BankName}";
            public string BalanceText => $"₡{Balance:N2}";
            public bool HasSufficientFunds(double amount) => Balance >= amount;
        }
    }
}