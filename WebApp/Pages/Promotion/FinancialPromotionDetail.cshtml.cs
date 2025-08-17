using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CoreApp;
using DTOs;

namespace WebApp.Pages.Promotion
{
    public class FinancialPromotionDetailModel : PageModel
    {
        public DTOs.FinancialPromotion PromotionInfo { get; set; } = new();
        public DTOs.FinancialEntity EntityInfo { get; set; } = new();
        public string Message { get; set; } = string.Empty;
        public bool PromotionFound { get; set; }

        public IActionResult OnGet(int promotionId)
        {
            if (promotionId <= 0)
            {
                return NotFound();
            }

            LoadPromotionInfo(promotionId);
            if (PromotionFound && PromotionInfo.FinancialEntityID > 0)
            {
                LoadEntityInfo(PromotionInfo.FinancialEntityID);
            }

            return Page();
        }

        private void LoadPromotionInfo(int promotionId)
        {
            try
            {
                var promotionManager = new FinancialPromotionManager();
                PromotionInfo = promotionManager.RetrievePromotionById(promotionId);

                if (PromotionInfo != null)
                {
                    PromotionFound = true;
                }
                else
                {
                    PromotionFound = false;
                    Message = "Promoción financiera no encontrada.";
                }
            }
            catch (Exception ex)
            {
                PromotionFound = false;
                Message = $"Error al cargar información de la promoción: {ex.Message}";
            }
        }

        private void LoadEntityInfo(int entityId)
        {
            try
            {
                var entityManager = new FinancialEntityManager();
                EntityInfo = entityManager.RetrieveFinancialEntityById(entityId);
                
                if (EntityInfo == null)
                {
                    EntityInfo = new DTOs.FinancialEntity { EntityName = "Entidad no encontrada" };
                }
            }
            catch (Exception ex)
            {
                EntityInfo = new DTOs.FinancialEntity { EntityName = "Error al cargar entidad" };
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
                "interes preferencial" => "bi-graph-up",
                "comision reducida" => "bi-arrow-down-circle",
                _ => "bi-bank"
            };
        }

        public string GetEntityType()
        {
            if (EntityInfo == null || string.IsNullOrEmpty(EntityInfo.EntityName))
                return "Entidad Financiera";

            var name = EntityInfo.EntityName.ToLower();
            
            if (name.Contains("banco") || name.Contains("bank"))
                return "Banco";
            else if (name.Contains("cooperativa") || name.Contains("coop"))
                return "Cooperativa";
            else if (name.Contains("mutualista") || name.Contains("mutual"))
                return "Mutualista";
            else if (name.Contains("financiera") || name.Contains("crédito"))
                return "Financiera";
            else if (name.Contains("popular") || name.Contains("ahorro"))
                return "Banco Popular";
            else
                return "Entidad Financiera";
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