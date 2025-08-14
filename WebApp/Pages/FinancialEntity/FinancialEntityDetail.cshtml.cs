using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CoreApp;
using DTOs;

namespace WebApp.Pages.FinancialEntity
{
    public class FinancialEntityDetailModel : PageModel
    {
        public DTOs.FinancialEntity EntityInfo { get; set; } = new();
        public List<FinancialPromotionInfo> ActivePromotions { get; set; } = new();
        public string Message { get; set; } = string.Empty;
        public bool EntityFound { get; set; }

        public IActionResult OnGet(int entityId)
        {
            if (entityId <= 0)
            {
                return NotFound();
            }

            LoadEntityInfo(entityId);
            LoadActivePromotions(entityId);

            return Page();
        }

        private void LoadEntityInfo(int entityId)
        {
            try
            {
                var entityManager = new FinancialEntityManager();
                EntityInfo = entityManager.RetrieveFinancialEntityById(entityId);

                if (EntityInfo != null)
                {
                    EntityFound = true;
                }
                else
                {
                    EntityFound = false;
                    Message = "Entidad financiera no encontrada.";
                }
            }
            catch (Exception ex)
            {
                EntityFound = false;
                Message = $"Error al cargar información de la entidad: {ex.Message}";
            }
        }

        private void LoadActivePromotions(int entityId)
        {
            try
            {
                var promotionManager = new FinancialPromotionManager();
                var allPromotions = promotionManager.RetrieveAllPromotions();
                var currentDate = DateTime.Now;

                var entityPromotions = allPromotions
                    .Where(p => p.FinancialEntityID == entityId && 
                               p.ValidationStatus == "Active" &&
                               p.StartDate <= currentDate &&
                               p.EndDate >= currentDate &&
                               p.AvailableQuantity > 0)
                    .OrderBy(p => p.EndDate)
                    .Take(6)
                    .ToList();

                ActivePromotions = entityPromotions.Select(p => new FinancialPromotionInfo
                {
                    ID = p.ID,
                    Name = p.FinancialPromotionName ?? "Promoción sin nombre",
                    Description = p.FinancialPromotionDescription ?? "Sin descripción",
                    DiscountPercentage = (decimal)p.DiscountPercentage,
                    MaxRefund = (decimal)p.MaxRefund,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    AvailableQuantity = p.AvailableQuantity,
                    ImageUrl = !string.IsNullOrEmpty(p.FinancialPromotionImage) ? p.FinancialPromotionImage : GetDefaultPromotionImage(),
                    PromotionType = p.PromotionType ?? "Descuento",
                    Terms = p.FinancialPromotionTerms ?? "Términos y condiciones aplican"
                }).ToList();
            }
            catch (Exception ex)
            {
                ActivePromotions = new List<FinancialPromotionInfo>();
                if (string.IsNullOrEmpty(Message))
                {
                    Message = $"Error al cargar promociones: {ex.Message}";
                }
            }
        }

        private string GetDefaultPromotionImage()
        {
            return "https://picsum.photos/seed/financial-promo/400/200";
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

        public string GetStatusBadgeClass()
        {
            if (EntityInfo?.ValidationStatus == "Active")
                return "bg-success";
            else
                return "bg-secondary";
        }

        public string GetStatusText()
        {
            return EntityInfo?.ValidationStatus switch
            {
                "Active" => "Activo",
                "Inactive" => "Inactivo",
                _ => "Sin Estado"
            };
        }
    }

    public class FinancialPromotionInfo
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal DiscountPercentage { get; set; }
        public decimal MaxRefund { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int AvailableQuantity { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string PromotionType { get; set; } = string.Empty;
        public string Terms { get; set; } = string.Empty;
    }
}