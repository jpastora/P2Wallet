using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DTOs;
using CoreApp;
using System;
using System.Linq;

namespace WebApp.Pages.User
{
    public class EditMerchantPromotionModel : PageModel
    {
        [BindProperty]
        public MerchantPromotion PromotionToEdit { get; set; } = new();

        public string MerchantName { get; set; } = string.Empty;
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
            var promotionManager = new MerchantPromotionManager();
            PromotionToEdit = promotionManager.RetrievePromotionById(promotionId);
            
            if (PromotionToEdit == null)
            {
                TempData["ErrorMessage"] = "Promoción no encontrada.";
                return RedirectToPage("/User/UserBusinessManager");
            }

            // Verificar autorización para el comercio de esta promoción
            if (CurrentUser.Role == "Admin")
            {
                IsAuthorized = true;
            }
            else
            {
                var userMerchantManager = new UserMerchantManager();
                var userMerchants = userMerchantManager.RetrieveAllUserMerchants();
                IsAuthorized = userMerchants.Any(um => um.UserID == CurrentUser.ID && um.MerchantID == PromotionToEdit.MerchantID);
            }

            if (!IsAuthorized)
            {
                TempData["ErrorMessage"] = "No tienes permisos para editar esta promoción.";
                return RedirectToPage("/User/UserBusinessManager");
            }

            LoadMerchantInfo();
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

                // Verificar autorización para el comercio
                if (CurrentUser.Role != "Admin")
                {
                    var userMerchantManager = new UserMerchantManager();
                    var userMerchants = userMerchantManager.RetrieveAllUserMerchants();
                    IsAuthorized = userMerchants.Any(um => um.UserID == CurrentUser.ID && um.MerchantID == PromotionToEdit.MerchantID);
                    
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
                    LoadMerchantInfo();
                    return Page();
                }

                if (PromotionToEdit.DiscountPercentage < 0 || PromotionToEdit.DiscountPercentage > 100)
                {
                    Message = "El porcentaje de descuento debe estar entre 0 y 100.";
                    LoadMerchantInfo();
                    return Page();
                }

                if (PromotionToEdit.MaxRefund < 0)
                {
                    Message = "El reembolso máximo no puede ser negativo.";
                    LoadMerchantInfo();
                    return Page();
                }

                if (PromotionToEdit.AvailableQuantity < 0)
                {
                    Message = "La cantidad disponible no puede ser negativa.";
                    LoadMerchantInfo();
                    return Page();
                }

                // Cargar la promoción existente para preservar campos no editables
                var promotionManager = new MerchantPromotionManager();
                var existingPromotion = promotionManager.RetrievePromotionById(PromotionToEdit.ID);
                
                if (existingPromotion == null)
                {
                    Message = "Promoción no encontrada.";
                    LoadMerchantInfo();
                    return Page();
                }

                // Actualizar solo los campos editables
                existingPromotion.MerchantPromotionName = PromotionToEdit.MerchantPromotionName;
                existingPromotion.MerchantPromotionDescription = PromotionToEdit.MerchantPromotionDescription;
                existingPromotion.MerchantPromotionTerms = PromotionToEdit.MerchantPromotionTerms;
                existingPromotion.PromotionType = PromotionToEdit.PromotionType;
                existingPromotion.DiscountPercentage = PromotionToEdit.DiscountPercentage;
                existingPromotion.MaxRefund = PromotionToEdit.MaxRefund;
                existingPromotion.StartDate = PromotionToEdit.StartDate;
                existingPromotion.EndDate = PromotionToEdit.EndDate;
                existingPromotion.AvailableQuantity = PromotionToEdit.AvailableQuantity;
                existingPromotion.ValidationStatus = PromotionToEdit.ValidationStatus;
                
                // Actualizar imagen si se proporcionó una nueva
                if (!string.IsNullOrEmpty(PromotionToEdit.MerchantPromotionImage))
                {
                    existingPromotion.MerchantPromotionImage = PromotionToEdit.MerchantPromotionImage;
                }

                promotionManager.UpdatePromotion(existingPromotion);
                
                TempData["SuccessMessage"] = $"Promoción '{existingPromotion.MerchantPromotionName}' actualizada exitosamente.";
                return RedirectToPage("/User/MerchantPromotions", new { merchantId = existingPromotion.MerchantID });
            }
            catch (Exception ex)
            {
                Message = $"Error al actualizar la promoción: {ex.Message}";
                LoadMerchantInfo();
                return Page();
            }
        }

        private void LoadMerchantInfo()
        {
            var merchantManager = new MerchantManager();
            var merchant = merchantManager.RetrieveMerchantById(PromotionToEdit.MerchantID);
            MerchantName = merchant?.MerchantName ?? "Comercio Desconocido";
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