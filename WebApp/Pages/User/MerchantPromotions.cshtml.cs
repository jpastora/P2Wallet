using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DTOs;
using CoreApp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApp.Pages.User
{
    public class MerchantPromotionsModel : PageModel
    {
        [BindProperty]
        public int MerchantID { get; set; }

        // Propiedades para mostrar datos del comercio
        public string MerchantName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsAuthorized { get; set; } = false;
        public DTOs.User CurrentUser { get; set; } = new();

        // Propiedades para las promociones
        public List<MerchantPromotion> AllPromotions { get; set; } = new();
        public List<MerchantPromotion> ActivePromotions { get; set; } = new();
        public List<MerchantPromotion> InactivePromotions { get; set; } = new();
        public List<MerchantPromotion> ExpiredPromotions { get; set; } = new();

        // Estadísticas
        public int TotalPromotions { get; set; }
        public int ActiveCount { get; set; }
        public int InactiveCount { get; set; }
        public int ExpiredCount { get; set; }

        public IActionResult OnGet([FromQuery] int merchantId)
        {
            if (merchantId <= 0)
                return NotFound();

            MerchantID = merchantId;

            // Verificar autorización del usuario
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
                return RedirectToPage("/Login");

            var userManager = new UserManager();
            CurrentUser = userManager.RetrieveUserByEmail(new DTOs.User { Email = email });
            
            if (CurrentUser == null)
                return RedirectToPage("/Login");

            // Verificar que el usuario sea admin o esté asignado al comercio
            if (CurrentUser.Role == "Admin")
            {
                IsAuthorized = true;
            }
            else
            {
                var userMerchantManager = new UserMerchantManager();
                var userMerchants = userMerchantManager.RetrieveAllUserMerchants();
                IsAuthorized = userMerchants.Any(um => um.UserID == CurrentUser.ID && um.MerchantID == merchantId);
            }

            if (!IsAuthorized)
            {
                TempData["ErrorMessage"] = "No tienes permisos para ver las promociones de este comercio.";
                return RedirectToPage("/User/UserBusinessManager");
            }

            LoadMerchantInfo();
            LoadPromotions();
            CalculateStatistics();

            return Page();
        }

        public IActionResult OnPostToggleStatus(int promotionId)
        {
            try
            {
                var promotionManager = new MerchantPromotionManager();
                var promotion = promotionManager.RetrievePromotionById(promotionId);
                
                if (promotion == null)
                {
                    TempData["ErrorMessage"] = "Promoción no encontrada.";
                    return RedirectToPage(new { merchantId = MerchantID });
                }

                // Cambiar estado
                promotion.ValidationStatus = promotion.ValidationStatus == "Active" ? "Inactive" : "Active";
                promotionManager.UpdatePromotion(promotion);

                var action = promotion.ValidationStatus == "Active" ? "activada" : "desactivada";
                TempData["SuccessMessage"] = $"Promoción '{promotion.MerchantPromotionName}' {action} exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cambiar estado de la promoción: {ex.Message}";
            }

            return RedirectToPage(new { merchantId = MerchantID });
        }

        public IActionResult OnPostDelete(int promotionId)
        {
            try
            {
                var promotionManager = new MerchantPromotionManager();
                var promotion = promotionManager.RetrievePromotionById(promotionId);
                
                if (promotion == null)
                {
                    TempData["ErrorMessage"] = "Promoción no encontrada.";
                    return RedirectToPage(new { merchantId = MerchantID });
                }

                promotionManager.DeletePromotion(promotion);
                TempData["SuccessMessage"] = $"Promoción '{promotion.MerchantPromotionName}' eliminada exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al eliminar la promoción: {ex.Message}";
            }

            return RedirectToPage(new { merchantId = MerchantID });
        }

        private void LoadMerchantInfo()
        {
            var merchantManager = new MerchantManager();
            var merchant = merchantManager.RetrieveMerchantById(MerchantID);
            MerchantName = merchant?.MerchantName ?? "Comercio Desconocido";
        }

        private void LoadPromotions()
        {
            var promotionManager = new MerchantPromotionManager();
            var allPromotions = promotionManager.RetrieveAllPromotions();
            
            AllPromotions = allPromotions.Where(p => p.MerchantID == MerchantID).ToList();

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
        public string GetPromotionStatusBadgeClass(MerchantPromotion promotion)
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

        public string GetPromotionStatusText(MerchantPromotion promotion)
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
                _ => type
            };
        }
    }
}