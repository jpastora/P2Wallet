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

        // Zona horaria de Costa Rica (UTC-6)
        private static readonly TimeZoneInfo CostaRicaTimeZone = 
            TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time");

        public ScanPaymentModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Método para obtener hora actual de Costa Rica
        private static DateTime GetCostaRicaTime()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, CostaRicaTimeZone);
        }

        // Método para verificar si una solicitud está expirada
        private bool IsPaymentRequestExpired(DateTime? expiresAt)
        {
            return expiresAt.HasValue && expiresAt.Value < GetCostaRicaTime();
        }

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
            try
            {
                string cleanCode = PaymentCode.Trim();
                var httpClient = _httpClientFactory.CreateClient();
                var apiUrl = GetApiBaseUrl() + $"/api/PaymentRequest/GetByCode/{cleanCode}";
                var response = await httpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var paymentResponse = JsonConvert.DeserializeObject<PaymentRequestResponse>(await response.Content.ReadAsStringAsync());
                    if (paymentResponse != null)
                    {
                        // Verificar si la solicitud está expirada usando hora de Costa Rica
                        if (IsPaymentRequestExpired(paymentResponse.ExpiresAt))
                        {
                            Message = "La solicitud de pago ha expirado. No se puede procesar el pago.";
                            return Page();
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
                        await LoadAvailablePromotions(cleanCode);
                        await LoadMerchantInfo();
                    }
                    else
                    {
                        Message = "Error: Respuesta inválida del servidor.";
                    }
                }
                else
                {
                    Message = "Solicitud no encontrada o expirada.";
                }
            }
            catch
            {
                Message = "Error al procesar el código.";
            }
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
                await OnPostScanAsync();
                return Page();
            }
            
            // Verificar nuevamente si no ha expirado antes de ejecutar
            if (PaymentRequest?.ExpiresAt != null && IsPaymentRequestExpired(PaymentRequest.ExpiresAt))
            {
                Message = "La solicitud de pago ha expirado mientras procesabas el pago.";
                PaymentRequestFound = false;
                return Page();
            }

            try
            {
                var selectedAccount = UserAccountsInfo.FirstOrDefault(a => a.ID == SelectedBankAccountId);
                if (selectedAccount != null && PaymentRequest != null && !selectedAccount.HasSufficientFunds(PaymentRequest.GrossAmount))
                {
                    Message = "Fondos insuficientes en la cuenta seleccionada.";
                    await OnPostScanAsync();
                    return Page();
                }
                var httpClient = _httpClientFactory.CreateClient();
                var apiUrl = GetApiBaseUrl() + "/api/PaymentRequest/Execute";
                var executeRequest = new
                {
                    PaymentRequestCode = PaymentCode,
                    UserId = CurrentUser.ID,
                    BankAccountId = SelectedBankAccountId,
                    PromotionId = SelectedPromotionId,
                    PromotionType = SelectedPromotionType
                };
                var jsonContent = new StringContent(JsonConvert.SerializeObject(executeRequest), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(apiUrl, jsonContent);
                if (response.IsSuccessStatusCode)
                {
                    ExecutionResult = JsonConvert.DeserializeObject<PaymentExecutionResponse>(await response.Content.ReadAsStringAsync());
                    if (ExecutionResult?.Success == true)
                    {
                        PaymentExecuted = true;
                        Message = "¡Pago realizado exitosamente!";
                        return RedirectToPage("/User/PaymentSuccess", new { transactionId = ExecutionResult.TransactionId });
                    }
                    else
                    {
                        Message = "Error al procesar el pago.";
                        await OnPostScanAsync();
                    }
                }
                else
                {
                    Message = "Error al ejecutar pago.";
                    await OnPostScanAsync();
                }
            }
            catch
            {
                Message = "Error inesperado al procesar el pago.";
                await OnPostScanAsync();
            }
            return Page();
        }

        // Método auxiliar para calcular tiempo restante
        public string GetTimeRemaining()
        {
            if (PaymentRequest?.ExpiresAt == null)
                return "";

            var timeLeft = PaymentRequest.ExpiresAt.Value - GetCostaRicaTime();
            
            if (timeLeft.TotalSeconds <= 0)
                return "Expirado";

            if (timeLeft.TotalMinutes < 1)
                return $"{(int)timeLeft.TotalSeconds} segundos";
            else if (timeLeft.TotalHours < 1)
                return $"{(int)timeLeft.TotalMinutes} minutos";
            else
                return $"{(int)timeLeft.TotalHours}h {timeLeft.Minutes}m";
        }

        private async Task LoadUserData()
        {
            var email = User.Identity?.Name;
            if (!string.IsNullOrEmpty(email))
            {
                var userManager = new UserManager();
                CurrentUser = userManager.RetrieveUserByEmail(new DTOs.User { Email = email });
                if (CurrentUser != null)
                {
                    var accountManager = new BankAccountManager();
                    var allAccounts = accountManager.RetrieveAllBankAccounts();
                    var entityManager = new FinancialEntityManager();
                    var allEntities = entityManager.RetrieveAllFinancialEntities();
                    UserAccountsInfo = allAccounts
                        .Where(a => a.UserID == CurrentUser.ID && a.ValidationStatus == "Active")
                        .Select(account => {
                            var entity = allEntities.FirstOrDefault(e => e.ID == account.FinancialEntityID);
                            return new BankAccountInfo
                            {
                                ID = account.ID,
                                IBAN = account.IBAN,
                                BankName = entity?.EntityName ?? "Banco Desconocido",
                                Balance = account.Balance,
                                LogoUrl = entity?.LogoImage ?? "https://picsum.photos/seed/bank/40/40"
                            };
                        }).ToList();
                }
            }
        }

        private async Task LoadAvailablePromotions(string paymentCode)
        {
            try
            {
                int? financialEntityId = null;
                if (SelectedBankAccountId > 0 && UserAccountsInfo != null)
                {
                    var selectedAccount = UserAccountsInfo.FirstOrDefault(a => a.ID == SelectedBankAccountId);
                    if (selectedAccount != null)
                    {
                        var accountManager = new BankAccountManager();
                        var account = accountManager.RetrieveBankAccountById(selectedAccount.ID);
                        if (account != null)
                        {
                            financialEntityId = account.FinancialEntityID;
                        }
                    }
                }
                var httpClient = _httpClientFactory.CreateClient();
                var apiUrl = GetApiBaseUrl() + $"/api/PaymentRequest/GetPromotions/{paymentCode}";
                if (financialEntityId.HasValue)
                {
                    apiUrl += $"?financialEntityId={financialEntityId.Value}";
                }
                var response = await httpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var promotions = JsonConvert.DeserializeObject<List<object>>(await response.Content.ReadAsStringAsync());
                    ConvertPromotionsToInfo(promotions ?? new List<object>());
                }
                else
                {
                    AvailablePromotions = new List<PromotionInfo>();
                }
            }
            catch
            {
                AvailablePromotions = new List<PromotionInfo>();
            }
        }

        private void ConvertPromotionsToInfo(List<object> promotions)
        {
            AvailablePromotions = promotions.Select(p => {
                var type = p.GetType();
                return new PromotionInfo
                {
                    Id = type.GetProperty("ID")?.GetValue(p)?.ToString() ?? "0",
                    Type = type.Name.Contains("Merchant") ? "Merchant" : "Financial",
                    Name = type.GetProperty("Name")?.GetValue(p)?.ToString() ?? 
                           type.GetProperty("MerchantPromotionName")?.GetValue(p)?.ToString() ??
                           type.GetProperty("FinancialPromotionName")?.GetValue(p)?.ToString() ?? "Promoción",
                    Description = type.GetProperty("Description")?.GetValue(p)?.ToString() ?? 
                                  type.GetProperty("MerchantPromotionDescription")?.GetValue(p)?.ToString() ??
                                  type.GetProperty("FinancialPromotionDescription")?.GetValue(p)?.ToString() ?? "",
                    DiscountPercentage = Convert.ToDouble(type.GetProperty("DiscountPercentage")?.GetValue(p) ?? 0),
                    MaxRefund = Convert.ToDouble(type.GetProperty("MaxRefund")?.GetValue(p) ?? 0)
                };
            }).ToList();
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
                catch
                {
                    MerchantInfo = null;
                }
            }
        }

        private int GetMerchantIdFromTransaction(int transactionId)
        {
            try
            {
                var transactionManager = new TransactionManager();
                var transaction = transactionManager.RetrieveTransactionById(transactionId);
                return transaction?.MerchantID ?? 0;
            }
            catch
            {
                return 0;
            }
        }

        private string GetApiBaseUrl() => "https://p2wallet-api-eyefddgeeda9c2fk.eastus-01.azurewebsites.net";

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
            public string DiscountText => DiscountPercentage > 0 ? $"{DiscountPercentage}% desc." : "";
        }
        public class BankAccountInfo
        {
            public int ID { get; set; }
            public string IBAN { get; set; } = string.Empty;
            public string BankName { get; set; } = string.Empty;
            public double Balance { get; set; }
            public string LogoUrl { get; set; } = string.Empty;
            public string DisplayText => $"{IBAN} - {BankName}";
            public string BalanceText => $"₡{Balance:N2}";
            public bool HasSufficientFunds(double amount) => Balance >= amount;
        }
    }
}