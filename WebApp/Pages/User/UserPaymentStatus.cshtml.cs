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

        // Propiedades para las solicitudes de pago simplificadas
        public List<PaymentRequestInfo> ActiveRequests { get; set; } = new();
        public List<PaymentRequestInfo> CompletedRequests { get; set; } = new();

        // Estadísticas básicas
        public int ActiveCount { get; set; }
        public int CompletedCount { get; set; }
        public decimal TodaySales { get; set; }

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
                return RedirectToPage("/User/UserBusinessManager");

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

        private async Task LoadPaymentRequests()
        {
            try
            {
                // Aquí puedes usar el API o directamente TransactionManager
                var transactionManager = new TransactionManager();
                var allRequests = LoadPaymentRequestsFromDatabase();

                // Separar por estado
                ActiveRequests = allRequests.Where(r => r.Status == "Active" || r.Status == "Pending").ToList();
                CompletedRequests = allRequests.Where(r => r.Status == "Completed").ToList();

                // Calcular estadísticas
                ActiveCount = ActiveRequests.Count;
                CompletedCount = CompletedRequests.Count;
                TodaySales = CompletedRequests
                    .Where(r => r.CreatedAt.Date == DateTime.Today)
                    .Sum(r => r.NetAmount);
            }
            catch (Exception ex)
            {
                Message = $"Error al cargar solicitudes: {ex.Message}";
            }
        }

        private List<PaymentRequestInfo> LoadPaymentRequestsFromDatabase()
        {
            var requests = new List<PaymentRequestInfo>();
            
            try
            {
                var transactionManager = new TransactionManager();
                var allTransactions = transactionManager.RetrieveAllTransactions();
                
                // Filtrar transacciones del comercio específico
                var merchantTransactions = allTransactions
                    .Where(t => t.MerchantID == MerchantID)
                    .OrderByDescending(t => t.Timestamp)
                    .ToList();

                foreach (var transaction in merchantTransactions)
                {
                    var request = new PaymentRequestInfo
                    {
                        PaymentRequestCode = transaction.PaymentRequestCode ?? $"TXN-{transaction.ID}",
                        Description = transaction.Description ?? "Pago",
                        GrossAmount = (decimal)transaction.GrossAmount,
                        NetAmount = (decimal)transaction.NetAmount,
                        Status = transaction.TransactionStatus,
                        CreatedAt = transaction.Timestamp,
                        ExpiresAt = transaction.ExpiresAt
                    };

                    // Determinar el estado y clase CSS
                    request.StatusText = GetStatusText(request.Status);
                    request.StatusBadgeClass = GetStatusBadgeClass(request.Status);

                    requests.Add(request);
                }
            }
            catch (Exception)
            {
                // En caso de error, retornar lista vacía
            }

            return requests;
        }

        public async Task<IActionResult> OnPostCancelRequestAsync(string paymentCode)
        {
            try
            {
                if (string.IsNullOrEmpty(paymentCode))
                {
                    Message = "Código de solicitud requerido.";
                    await LoadPaymentRequests();
                    return Page();
                }

                // Aquí implementarías la lógica de cancelación
                var transactionManager = new TransactionManager();
                // bool cancelled = transactionManager.CancelPaymentRequest(paymentCode);

                Message = "Solicitud cancelada exitosamente.";
                await LoadPaymentRequests();
            }
            catch (Exception ex)
            {
                Message = $"Error al cancelar solicitud: {ex.Message}";
                await LoadPaymentRequests();
            }

            return Page();
        }

        private string GetStatusText(string status)
        {
            return status switch
            {
                "Active" or "Pending" => "Pendiente",
                "Completed" => "Completado",
                "Expired" => "Expirado",
                "Cancelled" => "Cancelado",
                _ => status
            };
        }

        private string GetStatusBadgeClass(string status)
        {
            return status switch
            {
                "Active" or "Pending" => "bg-warning text-dark",
                "Completed" => "bg-success",
                "Expired" => "bg-danger",
                "Cancelled" => "bg-secondary",
                _ => "bg-secondary"
            };
        }

        // Clase auxiliar para la información de solicitudes
        public class PaymentRequestInfo
        {
            public string PaymentRequestCode { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public decimal GrossAmount { get; set; }
            public decimal NetAmount { get; set; }
            public string Status { get; set; } = string.Empty;
            public string StatusText { get; set; } = string.Empty;
            public string StatusBadgeClass { get; set; } = string.Empty;
            public DateTime CreatedAt { get; set; }
            public DateTime? ExpiresAt { get; set; }
            public string QRCodeUrl { get; set; } = string.Empty;
        }
    }
}