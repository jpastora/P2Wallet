using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using CoreApp;
using DTOs;

namespace WebApp.Pages.Administrator
{
    [Authorize]
    public class EditMerchantModel : PageModel
    {
        [BindProperty]
        public DTOs.Merchant MerchantToEdit { get; set; } = new();

        public string Message { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }

        public IActionResult OnGet(int merchantId)
        {
            // Verificar que el usuario actual sea admin
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToPage("/Login");
            }

            var userManager = new UserManager();
            var currentUser = userManager.RetrieveUserByEmail(new DTOs.User { Email = email });
            
            if (currentUser == null || currentUser.Role != "Admin")
            {
                return RedirectToPage("/User/UserPanel");
            }

            IsAdmin = true;

            // Cargar el comercio a editar
            var merchantManager = new MerchantManager();
            MerchantToEdit = merchantManager.RetrieveMerchantById(merchantId);
            if (MerchantToEdit == null)
            {
                TempData["ErrorMessage"] = "Comercio no encontrado.";
                return RedirectToPage("AdministratorPanelMerchants");
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            try
            {
                var merchantManager = new MerchantManager();
                
                // Cargar el comercio completo desde la base de datos para preservar todos los campos
                var existingMerchant = merchantManager.RetrieveMerchantById(MerchantToEdit.ID);
                if (existingMerchant == null)
                {
                    Message = "Comercio no encontrado.";
                    return Page();
                }

                // Actualizar los campos editables del formulario
                existingMerchant.MerchantName = MerchantToEdit.MerchantName;
                existingMerchant.TaxID = MerchantToEdit.TaxID;
                existingMerchant.Email = MerchantToEdit.Email;
                existingMerchant.ContactPhone = MerchantToEdit.ContactPhone;
                existingMerchant.CommissionPercentage = MerchantToEdit.CommissionPercentage;
                existingMerchant.ValidationStatus = MerchantToEdit.ValidationStatus;
                existingMerchant.Latitude = MerchantToEdit.Latitude;
                existingMerchant.Longitude = MerchantToEdit.Longitude;
                existingMerchant.LogoImage = MerchantToEdit.LogoImage;

                merchantManager.UpdateMerchant(existingMerchant);
                TempData["SuccessMessage"] = $"Comercio {existingMerchant.MerchantName} actualizado exitosamente.";
                return RedirectToPage("AdministratorPanelMerchants");
            }
            catch (Exception ex)
            {
                Message = $"Error al actualizar comercio: {ex.Message}";
                return Page();
            }
        }
    }
}