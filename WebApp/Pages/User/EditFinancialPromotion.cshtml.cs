using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DTOs;
using CoreApp;
using System;
using System.Linq;

namespace WebApp.Pages.User
{
    public class EditFinancialPromotionModel : PageModel
    {
        [BindProperty]
        public FinancialPromotion PromotionToEdit { get; set; } = new();

        public string FinancialEntityName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsAuthorized { get; set; } = false;
        public DTOs.User CurrentUser { get; set; } = new();

        public IActionResult OnGet([FromQuery] int promotionId)
        {
            if (promotionId <= 0)
                return NotFound();

            // Verificar autorización del usuario
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
                return RedirectToPage("/Login");

            var userManager = new UserManager();
            CurrentUser = userManager.RetrieveUserByEmail(new DTOs.User { Email = email });
            
            if (CurrentUser == null)
                return RedirectToPage("/Login");

            // Cargar la promoción
            var promotionManager = new FinancialPromotionManager();
            PromotionToEdit = promotionManager.RetrievePromotionById(promotionId);
            
            if (PromotionToEdit == null)
            {
                TempData["ErrorMessage"] = "Promoción no encontrada.";
                return RedirectToPage("/User/UserBusinessManager");
            }

            // Verificar autorización para la entidad financiera de esta promoción
            if (CurrentUser.Role == "Admin")
            {
                IsAuthorized = true;
            }
            else
            {
                var userEntityManager = new UserEntityManager();
                var userEntities = userEntityManager.RetrieveAllUserEntities();
                IsAuthorized = userEntities.Any(ue => ue.UserID == CurrentUser.ID && ue.FinancialEntityID == PromotionToEdit.FinancialEntityID);
            }

            if (!IsAuthorized)
            {
                TempData["ErrorMessage"] = "No tienes permisos para editar esta promoción.";
                return RedirectToPage("/User/UserBusinessManager");
            }

            LoadFinancialEntityInfo();
            return Page();
        }

        public IActionResult OnPost()
        {
            try
            {
                // Verificar autorización nuevamente
                var email = User.Identity?.Name;
                if (string.IsNullOrEmpty(email))
                    return RedirectToPage("/Login");

                var userManager = new UserManager();
                CurrentUser = userManager.RetrieveUserByEmail(new DTOs.User { Email = email });
                
                if (CurrentUser == null)
                    return RedirectToPage("/Login");

                // Verificar autorización para la entidad financiera
                if (CurrentUser.Role != "Admin")
                {
                    var userEntityManager = new UserEntityManager();
                    var userEntities = userEntityManager.RetrieveAllUserEntities();
                    IsAuthorized = userEntities.Any(ue => ue.UserID == CurrentUser.ID && ue.FinancialEntityID == PromotionToEdit.FinancialEntityID);
                    
                    if (!IsAuthorized)
                    {
                        TempData["ErrorMessage"] = "No tienes permisos para editar esta promoción.";
                        return RedirectToPage("/User/UserBusinessManager");
                    }
                }

                // Validaciones básicas
                if (PromotionToEdit.EndDate <= PromotionToEdit.StartDate)
                {
                    Message = "La fecha de fin debe ser posterior a la fecha de inicio.";
                    LoadFinancialEntityInfo();
                    return Page();
                }

                if (PromotionToEdit.DiscountPercentage < 0 || PromotionToEdit.DiscountPercentage > 100)
                {
                    Message = "El porcentaje de descuento debe estar entre 0 y 100.";
                    LoadFinancialEntityInfo();
                    return Page();
                }

                if (PromotionToEdit.MaxRefund < 0)
                {
                    Message = "El reembolso máximo no puede ser negativo.";
                    LoadFinancialEntityInfo();
                    return Page();
                }

                if (PromotionToEdit.AvailableQuantity < 0)
                {
                    Message = "La cantidad disponible no puede ser negativa.";
                    LoadFinancialEntityInfo();
                    return Page();
                }

                // Cargar la promoción existente para preservar campos no editables
                var promotionManager = new FinancialPromotionManager();
                var existingPromotion = promotionManager.RetrievePromotionById(PromotionToEdit.ID);
                
                if (existingPromotion == null)
                {
                    Message = "Promoción no encontrada.";
                    LoadFinancialEntityInfo();
                    return Page();
                }

                // Actualizar solo los campos editables
                existingPromotion.FinancialPromotionName = PromotionToEdit.FinancialPromotionName;
                existingPromotion.FinancialPromotionDescription = PromotionToEdit.FinancialPromotionDescription;
                existingPromotion.FinancialPromotionTerms = PromotionToEdit.FinancialPromotionTerms;
                existingPromotion.PromotionType = PromotionToEdit.PromotionType;
                existingPromotion.DiscountPercentage = PromotionToEdit.DiscountPercentage;
                existingPromotion.MaxRefund = PromotionToEdit.MaxRefund;
                existingPromotion.StartDate = PromotionToEdit.StartDate;
                existingPromotion.EndDate = PromotionToEdit.EndDate;
                existingPromotion.AvailableQuantity = PromotionToEdit.AvailableQuantity;
                existingPromotion.ValidationStatus = PromotionToEdit.ValidationStatus;
                
                // Actualizar imagen si se proporcionó una nueva
                if (!string.IsNullOrEmpty(PromotionToEdit.FinancialPromotionImage))
                {
                    existingPromotion.FinancialPromotionImage = PromotionToEdit.FinancialPromotionImage;
                }

                promotionManager.UpdatePromotion(existingPromotion);
                
                TempData["SuccessMessage"] = $"Promoción '{existingPromotion.FinancialPromotionName}' actualizada exitosamente.";
                return RedirectToPage("/User/FinancialPromotions", new { financialEntityId = existingPromotion.FinancialEntityID });
            }
            catch (Exception ex)
            {
                Message = $"Error al actualizar la promoción: {ex.Message}";
                LoadFinancialEntityInfo();
                return Page();
            }
        }

        private void LoadFinancialEntityInfo()
        {
            var financialEntityManager = new FinancialEntityManager();
            var entity = financialEntityManager.RetrieveFinancialEntityById(PromotionToEdit.FinancialEntityID);
            FinancialEntityName = entity?.EntityName ?? "Entidad Financiera Desconocida";
        }

        // Métodos auxiliares para la vista
        public string GetPromotionStatusText()
        {
            var now = DateTime.Now;
            
            if (PromotionToEdit.EndDate < now)
                return "Expirada";
            
            if (PromotionToEdit.ValidationStatus == "Inactive")
                return "Inactiva";
            
            if (PromotionToEdit.AvailableQuantity <= 0)
                return "Agotada";
            
            if (PromotionToEdit.ValidationStatus == "Active" && PromotionToEdit.StartDate <= now && PromotionToEdit.EndDate >= now)
                return "Activa";
            
            if (PromotionToEdit.StartDate > now)
                return "Programada";
            
            return "Inactiva";
        }

        public string GetPromotionStatusBadgeClass()
        {
            var now = DateTime.Now;
            
            if (PromotionToEdit.EndDate < now)
                return "bg-danger";
            
            if (PromotionToEdit.ValidationStatus == "Inactive")
                return "bg-secondary";
            
            if (PromotionToEdit.AvailableQuantity <= 0)
                return "bg-warning";
            
            if (PromotionToEdit.ValidationStatus == "Active" && PromotionToEdit.StartDate <= now && PromotionToEdit.EndDate >= now)
                return "bg-success";
            
            return "bg-secondary";
        }
    }
}