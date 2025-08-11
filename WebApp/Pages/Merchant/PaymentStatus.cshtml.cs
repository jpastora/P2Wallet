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

namespace WebApp.Pages.Merchant
{
    public class PaymentStatusModel : PageModel
    {
        [BindProperty]
        public int MerchantID { get; set; }

        // Propiedades para mostrar datos del comercio
        public string MerchantName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsAuthorized { get; set; } = false;

        // Propiedades para las solicitudes de pago
        public List<PaymentRequestInfo> PaymentRequests { get; set; } = new();
        public List<PaymentRequestInfo> ActiveRequests { get; set; } = new();
        public List<PaymentRequestInfo> CompletedRequests { get; set; } = new();
        public List<PaymentRequestInfo> ExpiredRequests { get; set; } = new();

        // Estadísticas
        public int TotalRequests { get; set; }
        public int ActiveCount { get; set; }
        public int CompletedCount { get; set; }
        public int ExpiredCount { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TodaySales { get; set; }

        private readonly IHttpClientFactory _httpClientFactory;

        public PaymentStatusModel(IHttpClientFactory httpClientFactory)
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
            var currentUser = userManager.RetrieveUserByEmail(new DTOs.User { Email = email });
            
            if (currentUser == null)
                return RedirectToPage("/Login");

            // Verificar que el usuario sea admin o esté asignado al comercio
            if (currentUser.Role == "Admin")
            {
                IsAuthorized = true;
            }
            else
            {
                var userMerchantManager = new UserMerchantManager();
                var userMerchants = userMerchantManager.RetrieveAllUserMerchants();
                IsAuthorized = userMerchants.Any(um => um.UserID == currentUser.ID && um.MerchantID == merchantId);
            }

            if (!IsAuthorized)
                return RedirectToPage("/User/UserProfile");

            // Obtener información del comercio
            var merchantManager = new MerchantManager();
            var merchant = merchantManager.RetrieveMerchantById(merchantId);
            if (merchant == null)
                return NotFound();

            MerchantName = merchant.MerchantName;

            // Cargar solicitudes de pago
            await LoadPaymentRequests();

            return Page();
        }

        public async Task<IActionResult> OnPostCancelRequestAsync(string paymentCode)
        {
            try
            {
                await ReloadMerchantData();

                if (!IsAuthorized)
                    return RedirectToPage("/User/UserProfile");

                if (string.IsNullOrEmpty(paymentCode))
                {
                    Message = "Código de solicitud inválido.";
                    await LoadPaymentRequests();
                    return Page();
                }

                // Cancelar vía API
                var httpClient = _httpClientFactory.CreateClient();
                var apiUrl = GetApiBaseUrl() + $"/api/PaymentRequest/Cancel/{paymentCode}";
                
                var response = await httpClient.PostAsync(apiUrl, null);

                if (response.IsSuccessStatusCode)
                {
                    Message = "Solicitud cancelada exitosamente.";
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Message = $"Error al cancelar: {errorContent}";
                }

                await LoadPaymentRequests();
            }
            catch (Exception ex)
            {
                Message = $"Error al cancelar solicitud: {ex.Message}";
            }

            return Page();
        }

        private async Task LoadPaymentRequests()
        {
            try
            {
                // Obtener todas las transacciones del comercio
                var transactionManager = new TransactionManager();
                var allTransactions = transactionManager.RetrieveAllTransactions()
                    .Where(t => t.MerchantID == MerchantID && !string.IsNullOrEmpty(t.PaymentRequestCode))
                    .OrderByDescending(t => t.Timestamp)
                    .ToList();

                PaymentRequests = allTransactions.Select(t => new PaymentRequestInfo
                {
                    PaymentRequestCode = t.PaymentRequestCode,
                    Description = t.Description,
                    GrossAmount = (decimal)t.GrossAmount,
                    NetAmount = (decimal)t.NetAmount,
                    SalesTaxAmount = (decimal)t.SalesTaxAmount,
                    Status = t.TransactionStatus,
                    CreatedAt = t.Timestamp,
                    ExpiresAt = t.ExpiresAt,
                    QRCodeUrl = QRCodeHelper.GeneratePaymentQR(t.PaymentRequestCode, 150),
                    IsExpired = t.ExpiresAt.HasValue && t.ExpiresAt.Value < DateTime.Now,
                    IsActive = t.TransactionStatus == "PendingUserApproval" && (!t.ExpiresAt.HasValue || t.ExpiresAt.Value > DateTime.Now),
                    IsCompleted = t.TransactionStatus == "Completed"
                }).ToList();

                // Categorizar solicitudes
                ActiveRequests = PaymentRequests.Where(p => p.IsActive).ToList();
                CompletedRequests = PaymentRequests.Where(p => p.IsCompleted).ToList();
                ExpiredRequests = PaymentRequests.Where(p => p.IsExpired).ToList();

                // Calcular estadísticas
                TotalRequests = PaymentRequests.Count;
                ActiveCount = ActiveRequests.Count;
                CompletedCount = CompletedRequests.Count;
                ExpiredCount = ExpiredRequests.Count;

                TotalSales = CompletedRequests.Sum(p => p.NetAmount);
                TodaySales = CompletedRequests
                    .Where(p => p.CreatedAt.Date == DateTime.Today)
                    .Sum(p => p.NetAmount);
            }
            catch (Exception ex)
            {
                Message = $"Error al cargar solicitudes: {ex.Message}";
                PaymentRequests = new List<PaymentRequestInfo>();
            }
        }

        private async Task ReloadMerchantData()
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
                return;

            var userManager = new UserManager();
            var currentUser = userManager.RetrieveUserByEmail(new DTOs.User { Email = email });
            
            if (currentUser == null)
                return;

            // Verificar autorización
            if (currentUser.Role == "Admin")
            {
                IsAuthorized = true;
            }
            else
            {
                var userMerchantManager = new UserMerchantManager();
                var userMerchants = userMerchantManager.RetrieveAllUserMerchants();
                IsAuthorized = userMerchants.Any(um => um.UserID == currentUser.ID && um.MerchantID == MerchantID);
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
                _ => "bg-secondary"
            };

            public string StatusText => Status switch
            {
                "Completed" => "Completado",
                "PendingUserApproval" => IsExpired ? "Expirado" : "Pendiente",
                "Failed" => "Fallido",
                "Cancelled" => "Cancelado",
                _ => Status
            };

            public string TimeRemaining
            {
                get
                {
                    // Si no tiene fecha de expiración, está expirado o completado, no mostrar tiempo
                    if (!ExpiresAt.HasValue || IsExpired || IsCompleted)
                        return "";

                    // Usar DateTime.Now directamente para simplicidad
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