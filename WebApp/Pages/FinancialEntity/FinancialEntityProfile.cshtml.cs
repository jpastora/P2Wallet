using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DTOs;
using CoreApp;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace WebApp.Pages.FinancialEntity
{
    [Authorize]
    public class FinancialEntityProfileModel : PageModel
    {
        [BindProperty]
        public int FinancialEntityID { get; set; }
        [BindProperty]
        public string FinancialEntityName { get; set; }
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
        [BindProperty]
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Message { get; set; }
        public bool IsAdmin { get; set; }
        
        private readonly IConfiguration _configuration;
        
        public FinancialEntityProfileModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> OnGet([FromQuery] int FinancialEntityId)
        {
            if (FinancialEntityId <= 0)
                return NotFound();

            var financialEntityManager = new FinancialEntityManager();
            var financialEntity = financialEntityManager.RetrieveFinancialEntityById(FinancialEntityId);
            if (financialEntity == null)
                return NotFound();

            // Mapear del DTO (que usa EntityName) al modelo (que usa FinancialEntityName)
            FinancialEntityID = financialEntity.ID;
            FinancialEntityName = financialEntity.EntityName; 
            TaxID = financialEntity.TaxID;
            LogoImage = financialEntity.LogoImage;
            Latitude = financialEntity.Latitude;
            Longitude = financialEntity.Longitude;
            ContactPhone = financialEntity.ContactPhone;
            Email = financialEntity.Email;
            CommissionPercentage = financialEntity.CommissionPercentage;
            ValidationStatus = financialEntity.ValidationStatus;

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
            
            try
            {
                var apiKey = _configuration["GoogleMaps:ApiKey"];
                var url = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={latitude},{longitude}&key={apiKey}";
                using var httpClient = new HttpClient();
                var response = await httpClient.GetFromJsonAsync<GoogleGeocodeResponse>(url);
                if (response != null && response.status == "OK" && response.results.Length > 0)
                    return response.results[0].formatted_address;
            }
            catch (Exception)
            {
                // Si falla la geocodificación, continuar sin error
            }
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
                // Validaciones básicas
                if (string.IsNullOrWhiteSpace(FinancialEntityName))
                {
                    Message = "El nombre de la entidad financiera es obligatorio.";
                    await ReloadData();
                    return Page();
                }

                if (string.IsNullOrWhiteSpace(TaxID))
                {
                    Message = "La cédula jurídica es obligatoria.";
                    await ReloadData();
                    return Page();
                }

                // Validación de coordenadas
                if (Latitude < -90 || Latitude > 90 || Longitude < -180 || Longitude > 180)
                {
                    Message = "Ubicación inválida.";
                    await ReloadData();
                    return Page();
                }

                var financialEntityManager = new FinancialEntityManager();
                var financialEntity = financialEntityManager.RetrieveFinancialEntityById(FinancialEntityID);
                if (financialEntity != null)
                {
                    // Mapear del modelo al DTO (FinancialEntityName -> EntityName)
                    financialEntity.EntityName = FinancialEntityName;
                    financialEntity.TaxID = TaxID;
                    financialEntity.Latitude = Latitude;
                    financialEntity.Longitude = Longitude;
                    financialEntity.ContactPhone = ContactPhone;
                    financialEntity.Email = Email;
                    financialEntity.CommissionPercentage = CommissionPercentage;
                    financialEntity.ValidationStatus = ValidationStatus;
                    
                    // Solo actualizar LogoImage si se proporcionó un valor nuevo
                    if (!string.IsNullOrWhiteSpace(LogoImage))
                    {
                        financialEntity.LogoImage = LogoImage;
                    }
                    
                    financialEntityManager.UpdateFinancialEntity(financialEntity);
                    Message = "Datos de la entidad financiera actualizados correctamente.";
                }
                else
                {
                    Message = "No se encontró la entidad financiera para actualizar.";
                }
                
                // Recargar todos los datos después de la actualización
                await ReloadData();
            }
            catch (Exception ex)
            {
                Message = $"Error al guardar: {ex.Message}";
                await ReloadData();
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
                    await ReloadData();
                    return Page();
                }

                // Validar que el FinancialEntityID sea válido
                if (FinancialEntityID <= 0)
                {
                    Message = "Error: ID de entidad financiera inválido.";
                    await ReloadData();
                    return Page();
                }

                var userManager = new UserManager();
                var userEntityManager = new UserEntityManager();
                var financialEntityManager = new FinancialEntityManager();

                // Verificar que la entidad financiera existe
                var entity = financialEntityManager.RetrieveFinancialEntityById(FinancialEntityID);
                if (entity == null)
                {
                    Message = "Error: La entidad financiera no existe.";
                    await ReloadData();
                    return Page();
                }

                // Buscar usuario por email
                var user = userManager.RetrieveUserByEmail(new DTOs.User { Email = userEmail.Trim() });
                if (user == null)
                {
                    Message = $"No existe un usuario con el email: {userEmail}";
                    await ReloadData();
                    return Page();
                }

                // Verificar si ya existe la asignación
                var existingAssignments = userEntityManager.RetrieveAllUserEntities();
                var existingAssignment = existingAssignments.FirstOrDefault(ue => ue.UserID == user.ID && ue.FinancialEntityID == FinancialEntityID);

                if (existingAssignment != null)
                {
                    Message = $"El usuario {user.FirstName} {user.LastName} ya está asignado a esta entidad financiera.";
                    await ReloadData();
                    return Page();
                }

                // Crear nueva asignación
                var userEntity = new UserEntity
                {
                    UserID = user.ID,
                    FinancialEntityID = FinancialEntityID
                };

                userEntityManager.CreateUserEntity(userEntity);
                Message = $"Usuario {user.FirstName} {user.LastName} asignado exitosamente a la entidad financiera.";
            }
            catch (Exception ex)
            {
                Message = $"Error al asignar usuario: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Error detallado: FinancialEntityID={FinancialEntityID}, UserEmail={userEmail}, Exception={ex}");
            }

            await ReloadData();
            return Page();
        }

        private async Task ReloadData()
        {
            if (FinancialEntityID > 0)
            {
                var financialEntityManager = new FinancialEntityManager();
                var updatedFinancialEntity = financialEntityManager.RetrieveFinancialEntityById(FinancialEntityID);
                if (updatedFinancialEntity != null)
                {
                    FinancialEntityID = updatedFinancialEntity.ID;
                    FinancialEntityName = updatedFinancialEntity.EntityName;
                    TaxID = updatedFinancialEntity.TaxID;
                    LogoImage = updatedFinancialEntity.LogoImage;
                    Latitude = updatedFinancialEntity.Latitude;
                    Longitude = updatedFinancialEntity.Longitude;
                    ContactPhone = updatedFinancialEntity.ContactPhone;
                    Email = updatedFinancialEntity.Email;
                    CommissionPercentage = updatedFinancialEntity.CommissionPercentage;
                    ValidationStatus = updatedFinancialEntity.ValidationStatus;
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
        }
    }
}
