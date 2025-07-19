using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using CoreApp;
using DTOs;

namespace WebApp.Pages.Administrator
{
    [Authorize]
    public class EditFinancialEntityModel : PageModel
    {
        [BindProperty]
        public DTOs.FinancialEntity EntityToEdit { get; set; } = new();

        public string Message { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }

        public IActionResult OnGet(int entityId)
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

            // Cargar la entidad financiera a editar
            var financialEntityManager = new FinancialEntityManager();
            EntityToEdit = financialEntityManager.RetrieveFinancialEntityById(entityId);
            if (EntityToEdit == null)
            {
                TempData["ErrorMessage"] = "Entidad financiera no encontrada.";
                return RedirectToPage("AdministratorPanelFinancialEntities");
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            try
            {
                var financialEntityManager = new FinancialEntityManager();
                
                // Cargar la entidad completa desde la base de datos para preservar todos los campos
                var existingEntity = financialEntityManager.RetrieveFinancialEntityById(EntityToEdit.ID);
                if (existingEntity == null)
                {
                    Message = "Entidad financiera no encontrada.";
                    return Page();
                }

                // Actualizar los campos editables del formulario
                existingEntity.EntityName = EntityToEdit.EntityName;
                existingEntity.TaxID = EntityToEdit.TaxID;
                existingEntity.Email = EntityToEdit.Email;
                existingEntity.ContactPhone = EntityToEdit.ContactPhone;
                existingEntity.CommissionPercentage = EntityToEdit.CommissionPercentage;
                existingEntity.ValidationStatus = EntityToEdit.ValidationStatus;
                existingEntity.Latitude = EntityToEdit.Latitude;
                existingEntity.Longitude = EntityToEdit.Longitude;
                existingEntity.LogoImage = EntityToEdit.LogoImage;

                financialEntityManager.UpdateFinancialEntity(existingEntity);
                TempData["SuccessMessage"] = $"Entidad financiera {existingEntity.EntityName} actualizada exitosamente.";
                return RedirectToPage("AdministratorPanelFinancialEntities");
            }
            catch (Exception ex)
            {
                Message = $"Error al actualizar entidad financiera: {ex.Message}";
                return Page();
            }
        }
    }
}