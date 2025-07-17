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
        public string MobilePhone { get; set; }
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
                    FirstName = currentUser.FirstName + (string.IsNullOrWhiteSpace(currentUser.LastName) ? "" : " " + currentUser.LastName);
                    MobilePhone = currentUser.MobilePhone;
                    Email = currentUser.Email;
                    ProfilePhotoUrl = currentUser.ProfilePhotoUrl;
                    Role = currentUser.Role;

                    if (Role == "admin")
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
                    // Separar nombre y apellido si es posible
                    var names = FirstName?.Split(' ');
                    currentUser.FirstName = names != null && names.Length > 0 ? names[0] : FirstName;
                    currentUser.LastName = names != null && names.Length > 1 ? string.Join(" ", names, 1, names.Length - 1) : "";
                    currentUser.MobilePhone = MobilePhone;
                    userManager.UpdateUser(currentUser);
                    Message = "Datos personales actualizados correctamente.";
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
