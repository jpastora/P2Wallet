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

        // Zona horaria de Costa Rica (UTC-6)
        private static readonly TimeZoneInfo CostaRicaTimeZone = 
            TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time");

        public CreatePaymentRequestModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Método para obtener hora actual de Costa Rica
        private static DateTime GetCostaRicaTime()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, CostaRicaTimeZone);
        }

        // Método para convertir fecha a zona horaria de Costa Rica para mostrar
        private static DateTime ConvertToCostaRicaTime(DateTime utcDateTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, CostaRicaTimeZone);
        }

        // Método para calcular tiempo restante hasta expiración
        public string GetTimeUntilExpiration()
        {
            if (!ExpiresAt.HasValue)
                return "";

            var timeLeft = ExpiresAt.Value - GetCostaRicaTime();
            
            if (timeLeft.TotalSeconds <= 0)
                return "Expirado";

            if (timeLeft.TotalMinutes < 1)
                return $"{(int)timeLeft.TotalSeconds} segundos";
            else if (timeLeft.TotalHours < 1)
                return $"{(int)timeLeft.TotalMinutes} minutos";
            else
                return $"{(int)timeLeft.TotalHours}h {timeLeft.Minutes}m";
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
                        
                        // Convertir la fecha de expiración a zona horaria de Costa Rica para mostrar
                        ExpiresAt = paymentResponse.ExpiresAt;
                        
                        PaymentRequestCreated = true;
                        
                        var expirationTime = GetTimeUntilExpiration();
                        Message = $"¡Solicitud de pago creada exitosamente! Expira en {expirationTime}. Muestra el código QR al cliente.";
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
            catch (Exception ex)
            {
                Message = $"Error al crear solicitud de pago: {ex.Message}";
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
            return "https://p2wallet-api-eyefddgeeda9c2fk.eastus-01.azurewebsites.net";
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