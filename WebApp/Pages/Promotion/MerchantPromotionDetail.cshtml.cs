using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CoreApp;
using DTOs;

namespace WebApp.Pages.Promotion
{
    public class MerchantPromotionDetailModel : PageModel
    {
        public DTOs.MerchantPromotion PromotionInfo { get; set; } = new();
        public DTOs.Merchant MerchantInfo { get; set; } = new();
        public string Message { get; set; } = string.Empty;
        public bool PromotionFound { get; set; }

        public IActionResult OnGet(int promotionId)
        {
            if (promotionId <= 0)
            {
                return NotFound();
            }

            LoadPromotionInfo(promotionId);
            if (PromotionFound && PromotionInfo.MerchantID > 0)
            {
                LoadMerchantInfo(PromotionInfo.MerchantID);
            }

            return Page();
        }

        private void LoadPromotionInfo(int promotionId)
        {
            try
            {
                var promotionManager = new MerchantPromotionManager();
                PromotionInfo = promotionManager.RetrievePromotionById(promotionId);

                if (PromotionInfo != null)
                {
                    PromotionFound = true;
                }
                else
                {
                    PromotionFound = false;
                    Message = "Promoción no encontrada.";
                }
            }
            catch (Exception ex)
            {
                PromotionFound = false;
                Message = $"Error al cargar información de la promoción: {ex.Message}";
            }
        }

        private void LoadMerchantInfo(int merchantId)
        {
            try
            {
                var merchantManager = new MerchantManager();
                MerchantInfo = merchantManager.RetrieveMerchantById(merchantId);
                
                if (MerchantInfo == null)
                {
                    MerchantInfo = new DTOs.Merchant { MerchantName = "Comercio no encontrado" };
                }
            }
            catch (Exception ex)
            {
                MerchantInfo = new DTOs.Merchant { MerchantName = "Error al cargar comercio" };
            }
        }

        public string GetPromotionStatusText()
        {
            var now = DateTime.Now;
            
            if (PromotionInfo.EndDate < now)
                return "Expirada";
            
            if (PromotionInfo.ValidationStatus != "Active")
                return "Inactiva";
            
            if (PromotionInfo.AvailableQuantity <= 0)
                return "Agotada";
            
            if (PromotionInfo.StartDate <= now && PromotionInfo.EndDate >= now)
                return "Vigente";
            
            if (PromotionInfo.StartDate > now)
                return "Próxima";
            
            return "Inactiva";
        }

        public string GetPromotionStatusBadgeClass()
        {
            var now = DateTime.Now;
            
            if (PromotionInfo.EndDate < now)
                return "bg-danger";
            
            if (PromotionInfo.ValidationStatus != "Active")
                return "bg-secondary";
            
            if (PromotionInfo.AvailableQuantity <= 0)
                return "bg-warning";
            
            if (PromotionInfo.StartDate <= now && PromotionInfo.EndDate >= now)
                return "bg-success";
            
            if (PromotionInfo.StartDate > now)
                return "bg-info";
            
            return "bg-secondary";
        }

        public string GetPromotionTypeIcon()
        {
            return PromotionInfo.PromotionType?.ToLower() switch
            {
                "descuento" => "bi-percent",
                "cashback" => "bi-cash-coin",
                "regalo" => "bi-gift",
                "envio gratis" => "bi-truck",
                _ => "bi-tag"
            };
        }

        public bool IsPromotionActive()
        {
            var now = DateTime.Now;
            return PromotionInfo.ValidationStatus == "Active" && 
                   PromotionInfo.StartDate <= now && 
                   PromotionInfo.EndDate >= now &&
                   PromotionInfo.AvailableQuantity > 0;
        }

        public int GetDaysRemaining()
        {
            var now = DateTime.Now;
            if (PromotionInfo.EndDate > now)
            {
                return (int)(PromotionInfo.EndDate - now).TotalDays;
            }
            return 0;
        }
    }
}