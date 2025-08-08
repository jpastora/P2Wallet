using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using DTOs;
using CoreApp;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

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

        // Estado de la página
        public string Message { get; set; } = string.Empty;
        public bool PaymentRequestFound { get; set; } = false;
        public bool PaymentExecuted { get; set; } = false;

        // Datos del usuario y cuentas
        public DTOs.User CurrentUser { get; set; } = new();
        public List<BankAccount> UserAccounts { get; set; } = new();
        public List<BankAccountInfo> UserAccountsInfo { get; set; } = new();

        // Datos de la solicitud de pago
        public Transaction? PaymentRequest { get; set; }
        public List<PromotionInfo> AvailablePromotions { get; set; } = new();

        // Para navegación y estado
        public DTOs.Merchant? MerchantInfo { get; set; }
        public PaymentExecutionResponse? ExecutionResult { get; set; }

        private readonly IHttpClientFactory _httpClientFactory;

        public ScanPaymentModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> OnGetAsync([FromQuery] string? code = null)
        {
            // Cargar datos del usuario
            await LoadUserData();
            
            if (CurrentUser == null)
                return RedirectToPage("/Login");

            // Si viene un código en la URL, procesarlo automáticamente
            if (!string.IsNullOrEmpty(code))
            {
                PaymentCode = code;
                return await OnPostScanAsync();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostScanAsync()
        {
            // Cargar datos del usuario
            await LoadUserData();
            
            if (CurrentUser == null)
                return RedirectToPage("/Login");

            if (string.IsNullOrWhiteSpace(PaymentCode))
            {
                Message = "Ingresa un código de pago válido.";
                return Page();
            }

            try
            {
                // Limpiar y extraer código del QR si es necesario
                string cleanCode = QRCodeHelper.ExtractPaymentCodeFromQR(PaymentCode) ?? PaymentCode.Trim();

                // Obtener solicitud de pago vía API
                var httpClient = _httpClientFactory.CreateClient();
                var apiUrl = GetApiBaseUrl() + $"/api/PaymentRequest/GetByCode/{cleanCode}";
                
                var response = await httpClient.GetAsync(apiUrl);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var paymentResponse = JsonConvert.DeserializeObject<PaymentRequestResponse>(responseContent);
                    
                    if (paymentResponse != null)
                    {
                        // Convertir response a Transaction para la vista
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
                        Message = "Solicitud de pago encontrada. Revisa los detalles y confirma el pago.";
                        
                        // Cargar promociones disponibles
                        await LoadAvailablePromotions(cleanCode);
                        
                        // Cargar información del comercio
                        await LoadMerchantInfo();
                    }
                    else
                    {
                        Message = "Error: Respuesta inválida del servidor.";
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Message = $"Solicitud no encontrada o expirada: {errorContent}";
                }
            }
            catch (Exception ex)
            {
                Message = $"Error al procesar el código: {ex.Message}";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostExecuteAsync()
        {
            // Cargar datos del usuario
            await LoadUserData();
            
            if (CurrentUser == null)
                return RedirectToPage("/Login");

            if (string.IsNullOrWhiteSpace(PaymentCode))
            {
                Message = "Código de pago requerido.";
                return Page();
            }

            if (SelectedBankAccountId <= 0)
            {
                Message = "Selecciona una cuenta bancaria para el pago.";
                await OnPostScanAsync(); // Recargar datos de la solicitud
                return Page();
            }

            try
            {
                // Ejecutar pago vía API
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

                var jsonContent = new StringContent(
                    JsonConvert.SerializeObject(executeRequest),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await httpClient.PostAsync(apiUrl, jsonContent);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    ExecutionResult = JsonConvert.DeserializeObject<PaymentExecutionResponse>(responseContent);
                    
                    if (ExecutionResult?.Success == true)
                    {
                        PaymentExecuted = true;
                        Message = "¡Pago realizado exitosamente!";
                        
                        // Redirigir a página de confirmación después de un delay
                        return RedirectToPage("/User/PaymentSuccess", new { transactionId = ExecutionResult.TransactionId });
                    }
                    else
                    {
                        Message = "Error al procesar el pago. Intenta nuevamente.";
                        await OnPostScanAsync(); // Recargar datos
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Message = $"Error al ejecutar pago: {errorContent}";
                    await OnPostScanAsync(); // Recargar datos
                }
            }
            catch (Exception ex)
            {
                Message = $"Error al procesar el pago: {ex.Message}";
                await OnPostScanAsync(); // Recargar datos
            }

            return Page();
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
                    // Cargar cuentas bancarias del usuario
                    var accountManager = new BankAccountManager();
                    var allAccounts = accountManager.RetrieveAllBankAccounts();
                    UserAccounts = allAccounts.Where(a => a.UserID == CurrentUser.ID && a.ValidationStatus == "Active").ToList();
                    
                    // Crear información detallada de cuentas
                    var entityManager = new FinancialEntityManager();
                    var allEntities = entityManager.RetrieveAllFinancialEntities();
                    
                    UserAccountsInfo = UserAccounts.Select(account => {
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
                var httpClient = _httpClientFactory.CreateClient();
                var apiUrl = GetApiBaseUrl() + $"/api/PaymentRequest/GetPromotions/{paymentCode}";
                
                var response = await httpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var promotions = JsonConvert.DeserializeObject<List<object>>(responseContent);
                    
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
                // Usar reflexión para extraer propiedades dinámicamente
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

        private string GetApiBaseUrl()
        {
            return "https://p2wallet-api-eyefddgeeda9c2fk.eastus-01.azurewebsites.net";
        }

        // Clases auxiliares para deserialización y UI
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