using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DTOs;
using CoreApp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Linq;

namespace WebApp.Pages.User
{
    public class UserPaymentStatusModel : PageModel
    {
        [BindProperty]
        public int MerchantID { get; set; }

        // Propiedades para mostrar datos del comercio
        public string MerchantName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsAuthorized { get; set; } = false;
        public DTOs.User CurrentUser { get; set; } = new();

        // Propiedades para las solicitudes de pago
        public List<PaymentRequestInfo> PaymentRequests { get; set; } = new();
        public List<PaymentRequestInfo> ActiveRequests { get; set; } = new();
        public List<PaymentRequestInfo> CompletedRequests { get; set; } = new();
        public List<PaymentRequestInfo> ExpiredRequests { get; set; } = new();
        public List<PaymentRequestInfo> CancelledRequests { get; set; } = new();

        // Estadísticas
        public int TotalRequests { get; set; }
        public int ActiveCount { get; set; }
        public int CompletedCount { get; set; }
        public int ExpiredCount { get; set; }
        public int CancelledCount { get; set; }
        public decimal TotalSales { get; set; }

        private readonly IHttpClientFactory _httpClientFactory;

        public UserPaymentStatusModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> OnGetAsync([FromQuery] int merchantId)
        {
            if (merchantId <= 0)
                return NotFound();

            MerchantID = merchantId;

            // Verificar autorización del usuario
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
                return RedirectToPage("/Login");

            var userManager = new UserManager();
            CurrentUser = userManager.RetrieveUserByEmail(new DTOs.User { Email = email });
            
            if (CurrentUser == null)
                return RedirectToPage("/Login");

            // Verificar que el usuario sea admin o esté asignado al comercio
            if (CurrentUser.Role == "Admin")
            {
                IsAuthorized = true;
            }
            else
            {
                var userMerchantManager = new UserMerchantManager();
                var userMerchants = userMerchantManager.RetrieveAllUserMerchants();
                IsAuthorized = userMerchants.Any(um => um.UserID == CurrentUser.ID && um.MerchantID == merchantId);
            }

            if (!IsAuthorized)
            {
                TempData["ErrorMessage"] = "No tienes autorización para acceder a este comercio.";
                return RedirectToPage("/User/UserBusinessManager");
            }

            // Obtener información del comercio
            var merchantManager = new MerchantManager();
            var merchant = merchantManager.RetrieveMerchantById(merchantId);
            if (merchant == null)
                return NotFound();

            MerchantName = merchant.MerchantName;

            // Cargar solicitudes de pago
            await LoadPaymentRequests();

            // Transferir mensajes de TempData si existen
            if (TempData.ContainsKey("SuccessMessage"))
            {
                Message = TempData["SuccessMessage"]?.ToString() ?? "";
            }
            else if (TempData.ContainsKey("ErrorMessage"))
            {
                Message = TempData["ErrorMessage"]?.ToString() ?? "";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostCancelRequestAsync(string paymentCode)
        {
            try
            {
                await ReloadMerchantData();

                if (!IsAuthorized)
                {
                    TempData["ErrorMessage"] = "No tienes autorización para realizar esta acción.";
                    return RedirectToPage("/User/UserBusinessManager");
                }

                if (string.IsNullOrEmpty(paymentCode))
                {
                    TempData["ErrorMessage"] = "Código de solicitud inválido.";
                    return RedirectToPage("/User/UserPaymentStatus", new { merchantId = MerchantID });
                }

                // Obtener el MerchantID desde la transacción si es necesario
                if (MerchantID == 0)
                {
                    var transactionManager = new TransactionManager();
                    var transaction = transactionManager.RetrieveAllTransactions()
                        .FirstOrDefault(t => t.PaymentRequestCode == paymentCode);
                    if (transaction != null)
                    {
                        MerchantID = transaction.MerchantID;
                    }
                }

                // Cancelar vía API
                var httpClient = _httpClientFactory.CreateClient();
                var apiUrl = GetApiBaseUrl() + $"/api/PaymentRequest/Cancel/{paymentCode}";
                var response = await httpClient.PostAsync(apiUrl, null);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "La solicitud de pago ha sido cancelada correctamente.";
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        TempData["ErrorMessage"] = "No se puede cancelar esta solicitud. Puede que ya esté procesada o haya expirado.";
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        TempData["ErrorMessage"] = "La solicitud de pago no fue encontrada.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = $"Error al cancelar: {errorContent}";
                    }
                }

                // Redirigir con el MerchantID correcto
                return RedirectToPage("/User/UserPaymentStatus", new { merchantId = MerchantID });
            }
            catch (HttpRequestException httpEx)
            {
                TempData["ErrorMessage"] = "Error de conexión con el servidor. Verifica tu conexión a internet.";
                return RedirectToPage("/User/UserPaymentStatus", new { merchantId = MerchantID });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error inesperado: {ex.Message}";
                return RedirectToPage("/User/UserPaymentStatus", new { merchantId = MerchantID });
            }
        }

        private async Task LoadPaymentRequests()
        {
            try
            {
                // Obtener TODAS las transacciones del comercio (sin filtrar por PaymentRequestCode)
                var transactionManager = new TransactionManager();
                var allTransactions = transactionManager.RetrieveAllTransactions()
                    .Where(t => t.MerchantID == MerchantID)
                    .OrderByDescending(t => t.Timestamp)
                    .ToList();

                PaymentRequests = allTransactions.Select(t => new PaymentRequestInfo
                {
                    PaymentRequestCode = t.PaymentRequestCode ?? "",
                    Description = t.Description,
                    GrossAmount = (decimal)t.GrossAmount,
                    NetAmount = (decimal)t.NetAmount,
                    SalesTaxAmount = (decimal)t.SalesTaxAmount,
                    Status = t.TransactionStatus,
                    CreatedAt = t.Timestamp,
                    ExpiresAt = t.ExpiresAt,
                    QRCodeUrl = !string.IsNullOrEmpty(t.PaymentRequestCode) ? 
                        QRCodeHelper.GeneratePaymentQR(t.PaymentRequestCode, 150) : "",
                    IsExpired = t.ExpiresAt.HasValue && t.ExpiresAt.Value < DateTime.Now,
                    IsActive = t.TransactionStatus == "PendingUserApproval",
                    IsCompleted = t.TransactionStatus == "Completed"
                }).ToList();

                // Categorizar solicitudes por estado específico
                ActiveRequests = PaymentRequests
                    .Where(p => p.Status == "PendingUserApproval" && !p.IsExpired)
                    .ToList();

                CompletedRequests = PaymentRequests
                    .Where(p => p.Status == "Completed")
                    .ToList();

                // Separar expiradas de canceladas
                ExpiredRequests = PaymentRequests
                    .Where(p => p.Status == "PendingUserApproval" && p.IsExpired)
                    .ToList();

                CancelledRequests = PaymentRequests
                    .Where(p => p.Status == "Canceled" || p.Status == "Failed")
                    .ToList();

                // Calcular estadísticas
                TotalRequests = PaymentRequests.Count;
                ActiveCount = ActiveRequests.Count;
                CompletedCount = CompletedRequests.Count;
                ExpiredCount = ExpiredRequests.Count;
                CancelledCount = CancelledRequests.Count;

                TotalSales = CompletedRequests.Sum(p => p.NetAmount);
            }
            catch (Exception ex)
            {
                Message = $"Error al cargar solicitudes: {ex.Message}";
                PaymentRequests = new List<PaymentRequestInfo>();
                
                // Inicializar listas vacías para evitar errores en la vista
                ActiveRequests = new List<PaymentRequestInfo>();
                CompletedRequests = new List<PaymentRequestInfo>();
                ExpiredRequests = new List<PaymentRequestInfo>();
                CancelledRequests = new List<PaymentRequestInfo>();
                
                TotalRequests = 0;
                ActiveCount = 0;
                CompletedCount = 0;
                ExpiredCount = 0;
                CancelledCount = 0;
                TotalSales = 0;
            }
        }

        private async Task ReloadMerchantData()
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
                return;

            var userManager = new UserManager();
            CurrentUser = userManager.RetrieveUserByEmail(new DTOs.User { Email = email });
            
            if (CurrentUser == null)
                return;

            // Verificar autorización
            if (CurrentUser.Role == "Admin")
            {
                IsAuthorized = true;
            }
            else
            {
                var userMerchantManager = new UserMerchantManager();
                var userMerchants = userMerchantManager.RetrieveAllUserMerchants();
                IsAuthorized = userMerchants.Any(um => um.UserID == CurrentUser.ID && um.MerchantID == MerchantID);
            }

            // Cargar datos del comercio
            var merchantManager = new MerchantManager();
            var merchant = merchantManager.RetrieveMerchantById(MerchantID);
            if (merchant != null)
            {
                MerchantName = merchant.MerchantName;
            }
        }

        private string GetApiBaseUrl()
        {
            return "https://p2wallet-api-eyefddgeeda9c2fk.eastus-01.azurewebsites.net";
        }

        // Clase auxiliar para información de solicitudes de pago
        public class PaymentRequestInfo
        {
            public string PaymentRequestCode { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public decimal GrossAmount { get; set; }
            public decimal NetAmount { get; set; }
            public decimal SalesTaxAmount { get; set; }
            public string Status { get; set; } = string.Empty;
            public DateTime CreatedAt { get; set; }
            public DateTime? ExpiresAt { get; set; }
            public string QRCodeUrl { get; set; } = string.Empty;
            public bool IsExpired { get; set; }
            public bool IsActive { get; set; }
            public bool IsCompleted { get; set; }

            public string StatusBadgeClass => Status switch
            {
                "Completed" => "bg-success",
                "PendingUserApproval" => IsExpired ? "bg-danger" : "bg-warning text-dark",
                "Failed" => "bg-danger",
                "Cancelled" => "bg-secondary",
                "Canceled" => "bg-secondary",
                _ => "bg-secondary"
            };

            public string StatusText => Status switch
            {
                "Completed" => "Completado",
                "PendingUserApproval" => IsExpired ? "Expirado" : "Pendiente",
                "Failed" => "Fallido",
                "Cancelled" => "Cancelado",
                "Canceled" => "Cancelado",
                _ => Status
            };

            public string TimeRemaining
            {
                get
                {
                    // Si no tiene fecha de expiración, está expirado o completado, no mostrar tiempo
                    if (!ExpiresAt.HasValue || IsExpired || IsCompleted)
                        return "";

                    // Usar DateTime.Now para zona horaria local (consistente con IsExpired)
                    var timeLeft = ExpiresAt.Value - DateTime.Now;
                    
                    // Si el tiempo es negativo o muy pequeño, considerar expirado
                    if (timeLeft.TotalSeconds <= 0)
                        return "";

                    // Formatear el tiempo restante
                    if (timeLeft.TotalDays >= 1)
                    {
                        return $"{(int)timeLeft.TotalDays}d {timeLeft.Hours}h";
                    }
                    else if (timeLeft.TotalHours >= 1)
                    {
                        return $"{timeLeft.Hours}h {timeLeft.Minutes}m";
                    }
                    else if (timeLeft.TotalMinutes >= 1)
                    {
                        return $"{timeLeft.Minutes}m {timeLeft.Seconds}s";
                    }
                    else
                    {
                        return $"{timeLeft.Seconds}s";
                    }
                }
            }
        }
    }
}