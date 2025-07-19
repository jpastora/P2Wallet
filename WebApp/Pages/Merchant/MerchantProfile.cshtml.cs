using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DTOs;
using CoreApp;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace WebApp.Pages.Merchant
{
    public class MerchantProfileModel : PageModel
    {
        [BindProperty]
        public int MerchantID { get; set; }
        [BindProperty]
        public string MerchantName { get; set; }
        [BindProperty]
        public string TaxID { get; set; }
        [BindProperty]
        public string LogoImage { get; set; }
        [BindProperty]
        public double Latitude { get; set; }
        [BindProperty]
        public double Longitude { get; set; }
        [BindProperty]
        public string ContactPhone { get; set; }
        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public double CommissionPercentage { get; set; }
        [BindProperty]
        public string ValidationStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Message { get; set; }
        public bool IsAdmin { get; set; }
        public string Address { get; set; }
        private readonly IConfiguration _configuration;
        public MerchantProfileModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> OnGet([FromQuery] int merchantId)
        {
            if (merchantId <= 0)
                return NotFound();

            var merchantManager = new MerchantManager();
            var merchant = merchantManager.RetrieveMerchantById(merchantId);
            if (merchant == null)
                return NotFound();

            MerchantID = merchant.ID;
            MerchantName = merchant.MerchantName;
            TaxID = merchant.TaxID;
            LogoImage = merchant.LogoImage;
            Latitude = merchant.Latitude;
            Longitude = merchant.Longitude;
            ContactPhone = merchant.ContactPhone;
            Email = merchant.Email;
            CommissionPercentage = merchant.CommissionPercentage;
            ValidationStatus = merchant.ValidationStatus;

            // Obtener el rol del usuario actual
            var email = User.Identity?.Name;
            if (!string.IsNullOrEmpty(email))
            {
                var userManager = new UserManager();
                var currentUser = userManager.RetrieveUserByEmail(new DTOs.User { Email = email });
                IsAdmin = currentUser != null && currentUser.Role == "Admin";
            }
            else
            {
                IsAdmin = false;
            }

            Address = await GetAddressFromCoordinatesAsync(Latitude, Longitude);
            return Page();
        }

        private async Task<string> GetAddressFromCoordinatesAsync(double latitude, double longitude)
        {
            if (latitude == 0 && longitude == 0)
                return string.Empty;
            var apiKey = _configuration["GoogleMaps:ApiKey"];
            var url = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={latitude},{longitude}&key={apiKey}";
            using var httpClient = new HttpClient();
            var response = await httpClient.GetFromJsonAsync<GoogleGeocodeResponse>(url);
            if (response != null && response.status == "OK" && response.results.Length > 0)
                return response.results[0].formatted_address;
            return $"Lat: {latitude}, Lng: {longitude}";
        }

        private class GoogleGeocodeResponse
        {
            public string status { get; set; }
            public GoogleGeocodeResult[] results { get; set; }
        }
        private class GoogleGeocodeResult
        {
            public string formatted_address { get; set; }
        }

        public async Task<IActionResult> OnPostSaveProfile()
        {
            try
            {
                var merchantManager = new MerchantManager();
                var merchant = merchantManager.RetrieveMerchantById(MerchantID);
                if (merchant != null)
                {
                    merchant.MerchantName = MerchantName;
                    merchant.TaxID = TaxID;
                    merchant.LogoImage = LogoImage;
                    merchant.Latitude = Latitude;
                    merchant.Longitude = Longitude;
                    merchant.ContactPhone = ContactPhone;
                    merchant.Email = Email;
                    merchant.CommissionPercentage = CommissionPercentage;
                    merchant.ValidationStatus = ValidationStatus;
                    merchantManager.UpdateMerchant(merchant);
                    Message = "Datos del comercio actualizados correctamente.";
                }
                else
                {
                    Message = "No se encontró el comercio para actualizar.";
                }
                // Recargar datos actualizados
                var updatedMerchant = merchantManager.RetrieveMerchantById(MerchantID);
                if (updatedMerchant != null)
                {
                    MerchantID = updatedMerchant.ID;
                    MerchantName = updatedMerchant.MerchantName;
                    TaxID = updatedMerchant.TaxID;
                    LogoImage = updatedMerchant.LogoImage;
                    Latitude = updatedMerchant.Latitude;
                    Longitude = updatedMerchant.Longitude;
                    ContactPhone = updatedMerchant.ContactPhone;
                    Email = updatedMerchant.Email;
                    CommissionPercentage = updatedMerchant.CommissionPercentage;
                    ValidationStatus = updatedMerchant.ValidationStatus;
                }
                // Recalcular el rol del usuario actual
                var email = User.Identity?.Name;
                if (!string.IsNullOrEmpty(email))
                {
                    var userManager = new UserManager();
                    var currentUser = userManager.RetrieveUserByEmail(new DTOs.User { Email = email });
                    IsAdmin = currentUser != null && currentUser.Role == "Admin";
                }
                else
                {
                    IsAdmin = false;
                }
                Address = await GetAddressFromCoordinatesAsync(Latitude, Longitude);
            }
            catch (Exception ex)
            {
                Message = $"Error al guardar: {ex.Message}";
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAssignUser(string userEmail)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userEmail))
                {
                    Message = "El email del usuario es obligatorio.";
                    await ReloadMerchantData();
                    return Page();
                }

                // Validar que el MerchantID sea válido
                if (MerchantID <= 0)
                {
                    Message = "Error: ID de comercio inválido.";
                    await ReloadMerchantData();
                    return Page();
                }

                var userManager = new UserManager();
                var userMerchantManager = new UserMerchantManager();
                var merchantManager = new MerchantManager();

                // Verificar que el comercio existe
                var merchant = merchantManager.RetrieveMerchantById(MerchantID);
                if (merchant == null)
                {
                    Message = "Error: El comercio no existe.";
                    await ReloadMerchantData();
                    return Page();
                }

                // Buscar usuario por email
                var user = userManager.RetrieveUserByEmail(new DTOs.User { Email = userEmail.Trim() });
                if (user == null)
                {
                    Message = $"No existe un usuario con el email: {userEmail}";
                    await ReloadMerchantData();
                    return Page();
                }

                // Verificar si ya existe la asignación
                var existingAssignments = userMerchantManager.RetrieveAllUserMerchants();
                var existingAssignment = existingAssignments.FirstOrDefault(um => um.UserID == user.ID && um.MerchantID == MerchantID);

                if (existingAssignment != null)
                {
                    Message = $"El usuario {user.FirstName} {user.LastName} ya está asignado a este comercio.";
                    await ReloadMerchantData();
                    return Page();
                }

                // Crear nueva asignación
                var userMerchant = new UserMerchant
                {
                    UserID = user.ID,
                    MerchantID = MerchantID
                };

                userMerchantManager.CreateUserMerchant(userMerchant);
                Message = $"Usuario {user.FirstName} {user.LastName} asignado exitosamente al comercio.";
            }
            catch (Exception ex)
            {
                Message = $"Error al asignar usuario: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Error detallado: MerchantID={MerchantID}, UserEmail={userEmail}, Exception={ex}");
            }

            await ReloadMerchantData();
            return Page();
        }

        private async Task ReloadMerchantData()
        {
            var merchantManager = new MerchantManager();
            var merchant = merchantManager.RetrieveMerchantById(MerchantID);
            if (merchant != null)
            {
                MerchantName = merchant.MerchantName;
                TaxID = merchant.TaxID;
                LogoImage = merchant.LogoImage;
                Latitude = merchant.Latitude;
                Longitude = merchant.Longitude;
                ContactPhone = merchant.ContactPhone;
                Email = merchant.Email;
                CommissionPercentage = merchant.CommissionPercentage;
                ValidationStatus = merchant.ValidationStatus;

                // Recalcular el rol del usuario actual
                var email = User.Identity?.Name;
                if (!string.IsNullOrEmpty(email))
                {
                    var userManager = new UserManager();
                    var currentUser = userManager.RetrieveUserByEmail(new DTOs.User { Email = email });
                    IsAdmin = currentUser != null && currentUser.Role == "Admin";
                }

                Address = await GetAddressFromCoordinatesAsync(Latitude, Longitude);
            }
        }
    }
}
