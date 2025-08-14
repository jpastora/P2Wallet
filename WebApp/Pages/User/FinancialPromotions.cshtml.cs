using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DTOs;
using CoreApp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApp.Pages.User
{
    public class FinancialPromotionsModel : PageModel
    {
        [BindProperty]
        public int FinancialEntityID { get; set; }

        // Propiedades para mostrar datos de la entidad financiera
        public string FinancialEntityName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsAuthorized { get; set; } = false;
        public DTOs.User CurrentUser { get; set; } = new();

        // Propiedades para las promociones
        public List<FinancialPromotion> AllPromotions { get; set; } = new();
        public List<FinancialPromotion> ActivePromotions { get; set; } = new();
        public List<FinancialPromotion> InactivePromotions { get; set; } = new();
        public List<FinancialPromotion> ExpiredPromotions { get; set; } = new();

        // Estadísticas
        public int TotalPromotions { get; set; }
        public int ActiveCount { get; set; }
        public int InactiveCount { get; set; }
        public int ExpiredCount { get; set; }

        public IActionResult OnGet([FromQuery] int financialEntityId)
        {
            if (financialEntityId <= 0)
                return NotFound();

            FinancialEntityID = financialEntityId;

            // Verificar autorización del usuario
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
                return RedirectToPage("/Login");

            var userManager = new UserManager();
            CurrentUser = userManager.RetrieveUserByEmail(new DTOs.User { Email = email });
            
            if (CurrentUser == null)
                return RedirectToPage("/Login");

            // Verificar que el usuario sea admin o esté asignado a la entidad financiera
            if (CurrentUser.Role == "Admin")
            {
                IsAuthorized = true;
            }
            else
            {
                var userEntityManager = new UserEntityManager();
                var userEntities = userEntityManager.RetrieveAllUserEntities();
                IsAuthorized = userEntities.Any(ue => ue.UserID == CurrentUser.ID && ue.FinancialEntityID == financialEntityId);
            }

            if (!IsAuthorized)
            {
                TempData["ErrorMessage"] = "No tienes permisos para ver las promociones de esta entidad financiera.";
                return RedirectToPage("/User/UserBusinessManager");
            }

            LoadFinancialEntityInfo();
            LoadPromotions();
            CalculateStatistics();

            return Page();
        }

        public IActionResult OnPostToggleStatus(int promotionId)
        {
            try
            {
                var promotionManager = new FinancialPromotionManager();
                var promotion = promotionManager.RetrievePromotionById(promotionId);
                
                if (promotion == null)
                {
                    TempData["ErrorMessage"] = "Promoción no encontrada.";
                    return RedirectToPage(new { financialEntityId = FinancialEntityID });
                }

                // Cambiar estado
                promotion.ValidationStatus = promotion.ValidationStatus == "Active" ? "Inactive" : "Active";
                promotionManager.UpdatePromotion(promotion);

                var action = promotion.ValidationStatus == "Active" ? "activada" : "desactivada";
                TempData["SuccessMessage"] = $"Promoción '{promotion.FinancialPromotionName}' {action} exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cambiar estado de la promoción: {ex.Message}";
            }

            return RedirectToPage(new { financialEntityId = FinancialEntityID });
        }

        public IActionResult OnPostDelete(int promotionId)
        {
            try
            {
                var promotionManager = new FinancialPromotionManager();
                var promotion = promotionManager.RetrievePromotionById(promotionId);
                
                if (promotion == null)
                {
                    TempData["ErrorMessage"] = "Promoción no encontrada.";
                    return RedirectToPage(new { financialEntityId = FinancialEntityID });
                }

                promotionManager.DeletePromotion(promotion);
                TempData["SuccessMessage"] = $"Promoción '{promotion.FinancialPromotionName}' eliminada exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al eliminar la promoción: {ex.Message}";
            }

            return RedirectToPage(new { financialEntityId = FinancialEntityID });
        }

        private void LoadFinancialEntityInfo()
        {
            var financialEntityManager = new FinancialEntityManager();
            var entity = financialEntityManager.RetrieveFinancialEntityById(FinancialEntityID);
            FinancialEntityName = entity?.EntityName ?? "Entidad Financiera Desconocida";
        }

        private void LoadPromotions()
        {
            var promotionManager = new FinancialPromotionManager();
            var allPromotions = promotionManager.RetrieveAllPromotions();
            
            AllPromotions = allPromotions.Where(p => p.FinancialEntityID == FinancialEntityID).ToList();

            var now = DateTime.Now;

            ActivePromotions = AllPromotions.Where(p => 
                p.ValidationStatus == "Active" && 
                p.StartDate <= now && 
                p.EndDate >= now
            ).ToList();

            InactivePromotions = AllPromotions.Where(p => 
                p.ValidationStatus == "Inactive"
            ).ToList();

            ExpiredPromotions = AllPromotions.Where(p => 
                p.EndDate < now || 
                (p.ValidationStatus == "Active" && p.AvailableQuantity <= 0)
            ).ToList();
        }

        private void CalculateStatistics()
        {
            TotalPromotions = AllPromotions.Count;
            ActiveCount = ActivePromotions.Count;
            InactiveCount = InactivePromotions.Count;
            ExpiredCount = ExpiredPromotions.Count;
        }

        // Métodos auxiliares para la vista
        public string GetPromotionStatusBadgeClass(FinancialPromotion promotion)
        {
            var now = DateTime.Now;
            
            if (promotion.EndDate < now)
                return "bg-danger";
            
            if (promotion.ValidationStatus == "Inactive")
                return "bg-secondary";
            
            if (promotion.AvailableQuantity <= 0)
                return "bg-warning";
            
            if (promotion.ValidationStatus == "Active" && promotion.StartDate <= now && promotion.EndDate >= now)
                return "bg-success";
            
            return "bg-secondary";
        }

        public string GetPromotionStatusText(FinancialPromotion promotion)
        {
            var now = DateTime.Now;
            
            if (promotion.EndDate < now)
                return "Expirada";
            
            if (promotion.ValidationStatus == "Inactive")
                return "Inactiva";
            
            if (promotion.AvailableQuantity <= 0)
                return "Agotada";
            
            if (promotion.ValidationStatus == "Active" && promotion.StartDate <= now && promotion.EndDate >= now)
                return "Activa";
            
            if (promotion.StartDate > now)
                return "Programada";
            
            return "Inactiva";
        }

        public string GetPromotionTypeText(string type)
        {
            return type switch
            {
                "Percentage" => "Porcentaje",
                "FixedAmount" => "Monto Fijo",
                "BOGO" => "2x1",
                "Cashback" => "Cashback",
                _ => type
            };
        }
    }
}