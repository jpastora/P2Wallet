using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using CoreApp;
using DTOs;

namespace WebApp.Pages.Administrator
{
    [Authorize]
    public class EditMerchantPromotionModel : PageModel
    {
        [BindProperty]
        public DTOs.MerchantPromotion PromotionToEdit { get; set; } = new();

        public string Message { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }

        public IActionResult OnGet(int promotionId)
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

            // Cargar la promoción a editar
            var merchantPromotionManager = new MerchantPromotionManager();
            PromotionToEdit = merchantPromotionManager.RetrievePromotionById(promotionId);
            if (PromotionToEdit == null)
            {
                TempData["ErrorMessage"] = "Promoción de comercio no encontrada.";
                return RedirectToPage("AdministratorPanelPromotions");
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            try
            {
                var merchantPromotionManager = new MerchantPromotionManager();
                
                // Cargar la promoción completa desde la base de datos para preservar todos los campos
                var existingPromotion = merchantPromotionManager.RetrievePromotionById(PromotionToEdit.ID);
                if (existingPromotion == null)
                {
                    Message = "Promoción de comercio no encontrada.";
                    return Page();
                }

                // Actualizar los campos editables del formulario
                existingPromotion.MerchantPromotionName = PromotionToEdit.MerchantPromotionName;
                existingPromotion.MerchantPromotionDescription = PromotionToEdit.MerchantPromotionDescription;
                existingPromotion.MerchantPromotionTerms = PromotionToEdit.MerchantPromotionTerms;
                existingPromotion.MerchantPromotionImage = PromotionToEdit.MerchantPromotionImage;
                existingPromotion.PromotionType = PromotionToEdit.PromotionType;
                existingPromotion.DiscountPercentage = PromotionToEdit.DiscountPercentage;
                existingPromotion.MaxRefund = PromotionToEdit.MaxRefund;
                existingPromotion.StartDate = PromotionToEdit.StartDate;
                existingPromotion.EndDate = PromotionToEdit.EndDate;
                existingPromotion.AvailableQuantity = PromotionToEdit.AvailableQuantity;
                existingPromotion.ValidationStatus = PromotionToEdit.ValidationStatus;

                merchantPromotionManager.UpdatePromotion(existingPromotion);
                TempData["SuccessMessage"] = $"Promoción de comercio {existingPromotion.MerchantPromotionName} actualizada exitosamente.";
                return RedirectToPage("AdministratorPanelPromotions");
            }
            catch (Exception ex)
            {
                Message = $"Error al actualizar promoción de comercio: {ex.Message}";
                return Page();
            }
        }
    }
}