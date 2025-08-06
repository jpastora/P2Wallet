using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DTOs;
using CoreApp;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Linq;

namespace WebApp.Pages.Merchant
{
    public class CreatePaymentRequestModel : PageModel
    {
        [BindProperty]
        public int MerchantID { get; set; }
        
        [BindProperty]
        public decimal SaleAmount { get; set; }
        
        [BindProperty]
        public string Description { get; set; } = string.Empty;
        
        [BindProperty]
        public int ExpirationMinutes { get; set; } = 30;

        // Propiedades para mostrar datos del comercio
        public string MerchantName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsAuthorized { get; set; } = false;

        // Propiedades para la respuesta del pago creado
        public string PaymentRequestCode { get; set; } = string.Empty;
        public string QRCodeUrl { get; set; } = string.Empty;
        public string QRContent { get; set; } = string.Empty;
        public decimal GrossAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal SalesTaxAmount { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool PaymentRequestCreated { get; set; } = false;

        private readonly IHttpClientFactory _httpClientFactory;

        public CreatePaymentRequestModel(IHttpClientFactory httpClientFactory)
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

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // Recargar datos del comercio
                await ReloadMerchantData();

                if (!IsAuthorized)
                    return RedirectToPage("/User/UserProfile");

                // Validaciones básicas
                if (SaleAmount <= 0)
                {
                    Message = "El monto de venta debe ser mayor a cero.";
                    return Page();
                }

                if (ExpirationMinutes < 5 || ExpirationMinutes > 120)
                {
                    Message = "La expiración debe estar entre 5 y 120 minutos.";
                    return Page();
                }

                if (string.IsNullOrWhiteSpace(Description))
                {
                    Description = $"Solicitud de pago - {MerchantName}";
                }

                // Llamar al API para crear la solicitud de pago
                var httpClient = _httpClientFactory.CreateClient();
                
                var requestDto = new
                {
                    MerchantId = MerchantID,
                    SaleAmount = SaleAmount,
                    Description = Description,
                    ExpirationMinutes = ExpirationMinutes
                };

                var jsonContent = new StringContent(
                    JsonConvert.SerializeObject(requestDto),
                    Encoding.UTF8,
                    "application/json"
                );

                // Construir URL del API (ajustar según tu configuración)
                var apiUrl = GetApiBaseUrl() + "/api/PaymentRequest/Create";
                var response = await httpClient.PostAsync(apiUrl, jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var paymentResponse = JsonConvert.DeserializeObject<PaymentRequestResponse>(responseContent);

                    if (paymentResponse != null)
                    {
                        // Mostrar los datos de la solicitud creada
                        PaymentRequestCode = paymentResponse.PaymentRequestCode;
                        QRCodeUrl = paymentResponse.QRCodeUrl;
                        QRContent = paymentResponse.QRContent;
                        GrossAmount = paymentResponse.GrossAmount;
                        NetAmount = paymentResponse.NetAmount;
                        SalesTaxAmount = paymentResponse.SalesTaxAmount;
                        ExpiresAt = paymentResponse.ExpiresAt;
                        PaymentRequestCreated = true;
                        
                        Message = "¡Solicitud de pago creada exitosamente! Muestra el código QR al cliente.";
                    }
                    else
                    {
                        Message = "Error: Respuesta inválida del servidor.";
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Message = $"Error al crear solicitud: {errorContent}";
                }
            }
            catch (HttpRequestException)
            {
                // Si no hay conexión al API, usar TransactionManager directamente
                try
                {
                    var transactionManager = new TransactionManager();
                    var paymentRequest = transactionManager.CreatePaymentRequest(MerchantID, SaleAmount, Description);

                    if (paymentRequest != null)
                    {
                        PaymentRequestCode = paymentRequest.PaymentRequestCode;
                        QRCodeUrl = QRCodeHelper.GeneratePaymentQR(paymentRequest.PaymentRequestCode);
                        QRContent = QRCodeHelper.GeneratePaymentQRContent(paymentRequest.PaymentRequestCode);
                        GrossAmount = (decimal)paymentRequest.GrossAmount;
                        NetAmount = (decimal)paymentRequest.NetAmount;
                        SalesTaxAmount = (decimal)paymentRequest.SalesTaxAmount;
                        ExpiresAt = paymentRequest.ExpiresAt ?? DateTime.Now.AddMinutes(ExpirationMinutes);
                        PaymentRequestCreated = true;
                        
                        Message = "¡Solicitud de pago creada exitosamente! (Modo offline)";
                    }
                    else
                    {
                        Message = "Error: No se pudo crear la solicitud de pago.";
                    }
                }
                catch (Exception ex)
                {
                    Message = $"Error al crear solicitud: {ex.Message}";
                }
            }
            catch (Exception ex)
            {
                Message = $"Error inesperado: {ex.Message}";
            }

            return Page();
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
            // Ajustar según tu configuración - puede ser desde appsettings.json
            return "https://localhost:7071"; // Puerto del WebAPI
        }

        // Clase auxiliar para deserializar la respuesta del API
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
    }
}