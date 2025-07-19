using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using DTOs;
using CoreApp;
using System.Collections.Generic;
using System.Linq;

namespace WebApp.Pages.User
{
    [Authorize]
    public class UserProfileModel : PageModel
    {
        [BindProperty]
        public string FirstName { get; set; }
        [BindProperty]
        public string LastName { get; set; }
        [BindProperty]
        public string MobilePhone { get; set; }
        [BindProperty]
        public double Latitude { get; set; }
        [BindProperty]
        public double Longitude { get; set; }
        [BindProperty]
        public string EmailNotification { get; set; }
        [BindProperty]
        public string PushNotification { get; set; }
        [BindProperty]
        public string SMSNotification { get; set; }
        [BindProperty]
        public string Password { get; set; }
        [BindProperty]
        public string ConfirmPassword { get; set; }
        [BindProperty]
        public string ProfilePhotoUrl { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public string Role { get; set; }
        public List<DTOs.Merchant> AdminMerchants { get; set; } = new();
        public List<DTOs.FinancialEntity> AdminEntities { get; set; } = new();

        public void OnGet()
        {
            var email = User.Identity?.Name;
            if (!string.IsNullOrEmpty(email))
            {
                var userManager = new UserManager();
                var currentUser = userManager.RetrieveUserByEmail(new DTOs.User { Email = email });
                if (currentUser != null)
                {
                    FirstName = currentUser.FirstName;
                    LastName = currentUser.LastName;
                    MobilePhone = currentUser.MobilePhone;
                    Email = currentUser.Email;
                    Role = currentUser.Role;
                    Latitude = currentUser.Latitude;
                    Longitude = currentUser.Longitude;
                    EmailNotification = currentUser.EmailNotification;
                    PushNotification = currentUser.PushNotification;
                    SMSNotification = currentUser.SMSNotification;
                    ProfilePhotoUrl = currentUser.ProfilePhotoUrl;

                    if (Role == "Admin")
                    {
                        AdminMerchants = userManager.GetMerchantsForUser(currentUser.ID);
                        AdminEntities = userManager.GetEntitiesForUser(currentUser.ID);
                    }
                }
            }
        }

        public IActionResult OnPostSaveProfile()
        {
            var email = User.Identity?.Name;
            if (!string.IsNullOrEmpty(email))
            {
                var userManager = new UserManager();
                var currentUser = userManager.RetrieveUserByEmail(new DTOs.User { Email = email });
                if (currentUser != null)
                {
                    // Validación básica de teléfono
                    if (!string.IsNullOrWhiteSpace(MobilePhone) && !MobilePhone.StartsWith("+"))
                    {
                        MobilePhone = "+506" + MobilePhone;
                    }
                    if (!string.IsNullOrWhiteSpace(MobilePhone) && !System.Text.RegularExpressions.Regex.IsMatch(MobilePhone, @"^\+\d{1,3}\d{8,}$"))
                    {
                        Message = "El número telefónico debe incluir el código de país, por ejemplo: +506XXXXXXXX";
                        OnGet(); // Recargar datos completos
                        return Page();
                    }
                    // Validación de latitud/longitud
                    if (Latitude < -90 || Latitude > 90 || Longitude < -180 || Longitude > 180)
                    {
                        Message = "Ubicación inválida.";
                        OnGet(); // Recargar datos completos
                        return Page();
                    }
                    
                    // Actualizar los datos del perfil
                    currentUser.FirstName = FirstName;
                    currentUser.LastName = LastName;
                    currentUser.MobilePhone = MobilePhone;
                    currentUser.Latitude = Latitude;
                    currentUser.Longitude = Longitude;
                    
                    // Solo actualizar ProfilePhotoUrl si se proporcionó un valor nuevo
                    if (!string.IsNullOrWhiteSpace(ProfilePhotoUrl))
                    {
                        currentUser.ProfilePhotoUrl = ProfilePhotoUrl;
                    }
                    
                    // Log de los datos enviados al SP
                    Console.WriteLine($"UpdateUser: ID={currentUser.ID}, FirstName={currentUser.FirstName}, LastName={currentUser.LastName}, MobilePhone={currentUser.MobilePhone}, Latitude={currentUser.Latitude}, Longitude={currentUser.Longitude}, Email={currentUser.Email}, ProfilePhotoUrl={currentUser.ProfilePhotoUrl}, Password={currentUser.Password}, EmailVerified={currentUser.EmailVerified}, MobileVerified={currentUser.MobileVerified}, BiometricVerified={currentUser.BiometricVerified}, ValidationStatus={currentUser.ValidationStatus}, SMSNotification={currentUser.SMSNotification}, EmailNotification={currentUser.EmailNotification}, PushNotification={currentUser.PushNotification}, Role={currentUser.Role}");
                    
                    userManager.UpdateUser(currentUser);
                    Message = "Datos personales actualizados correctamente.";
                    
                    // Recargar todos los datos del usuario después de la actualización
                    OnGet();
                }
            }
            return Page();
        }

        public IActionResult OnPostSaveNotifications()
        {
            var email = User.Identity?.Name;
            if (!string.IsNullOrEmpty(email))
            {
                var userManager = new UserManager();
                var currentUser = userManager.RetrieveUserByEmail(new DTOs.User { Email = email });
                if (currentUser != null)
                {
                    currentUser.EmailNotification = Request.Form["EmailNotification"] == "on" ? "Active" : "Inactive";
                    currentUser.PushNotification = Request.Form["PushNotification"] == "on" ? "Active" : "Inactive";
                    currentUser.SMSNotification = Request.Form["SMSNotification"] == "on" ? "Active" : "Inactive";
                    userManager.UpdateUser(currentUser);
                    Message = "Preferencias de notificación actualizadas.";
                }
            }
            OnGet(); // Recargar todos los datos
            return Page();
        }

        public IActionResult OnPostChangePassword()
        {
            if (Password != ConfirmPassword)
            {
                Message = "Las contraseñas no coinciden.";
                OnGet(); // Recargar todos los datos
                return Page();
            }
            var email = User.Identity?.Name;
            if (!string.IsNullOrEmpty(email))
            {
                var userManager = new UserManager();
                var currentUser = userManager.RetrieveUserByEmail(new DTOs.User { Email = email });
                if (currentUser != null)
                {
                    currentUser.Password = Password;
                    userManager.UpdateUser(currentUser);
                    Message = "Contraseña actualizada correctamente.";
                }
            }
            OnGet(); // Recargar todos los datos
            return Page();
        }
    }
}
