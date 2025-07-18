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
        public string Email { get; set; }
        public string Message { get; set; }
        public string ProfilePhotoUrl { get; set; }
        public string Role { get; set; }
        public List<Merchant> AdminMerchants { get; set; } = new();
        public List<FinancialEntity> AdminEntities { get; set; } = new();

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
                    ProfilePhotoUrl = currentUser.ProfilePhotoUrl;
                    Role = currentUser.Role;
                    Latitude = currentUser.Latitude;
                    Longitude = currentUser.Longitude;
                    EmailNotification = currentUser.EmailNotification;
                    PushNotification = currentUser.PushNotification;
                    SMSNotification = currentUser.SMSNotification;

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
                    currentUser.FirstName = FirstName;
                    currentUser.LastName = LastName;
                    currentUser.MobilePhone = MobilePhone;
                    currentUser.Latitude = Latitude;
                    currentUser.Longitude = Longitude;
                    userManager.UpdateUser(currentUser);
                    Message = "Datos personales actualizados correctamente.";
                }
            }
            OnGet();
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
            OnGet();
            return Page();
        }

        public IActionResult OnPostChangePassword()
        {
            if (Password != ConfirmPassword)
            {
                Message = "Las contraseñas no coinciden.";
                OnGet();
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
            OnGet();
            return Page();
        }
    }
}
