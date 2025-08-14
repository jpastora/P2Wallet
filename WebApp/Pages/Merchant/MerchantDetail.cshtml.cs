using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CoreApp;
using DTOs;

namespace WebApp.Pages.Merchant
{
    public class MerchantDetailModel : PageModel
    {
        public DTOs.Merchant MerchantInfo { get; set; } = new();
        public List<MerchantPromotionInfo> ActivePromotions { get; set; } = new();
        public string Message { get; set; } = string.Empty;
        public bool MerchantFound { get; set; }

        public IActionResult OnGet(int merchantId)
        {
            if (merchantId <= 0)
            {
                return NotFound();
            }

            LoadMerchantInfo(merchantId);
            LoadActivePromotions(merchantId);

            return Page();
        }

        private void LoadMerchantInfo(int merchantId)
        {
            try
            {
                var merchantManager = new MerchantManager();
                MerchantInfo = merchantManager.RetrieveMerchantById(merchantId);

                if (MerchantInfo != null)
                {
                    MerchantFound = true;
                }
                else
                {
                    MerchantFound = false;
                    Message = "Comercio no encontrado.";
                }
            }
            catch (Exception ex)
            {
                MerchantFound = false;
                Message = $"Error al cargar información del comercio: {ex.Message}";
            }
        }

        private void LoadActivePromotions(int merchantId)
        {
            try
            {
                var promotionManager = new MerchantPromotionManager();
                var allPromotions = promotionManager.RetrieveAllPromotions();
                var currentDate = DateTime.Now;

                var merchantPromotions = allPromotions
                    .Where(p => p.MerchantID == merchantId && 
                               p.ValidationStatus == "Active" &&
                               p.StartDate <= currentDate &&
                               p.EndDate >= currentDate &&
                               p.AvailableQuantity > 0)
                    .OrderBy(p => p.EndDate)
                    .Take(6)
                    .ToList();

                ActivePromotions = merchantPromotions.Select(p => new MerchantPromotionInfo
                {
                    ID = p.ID,
                    Name = p.MerchantPromotionName ?? "Promoción sin nombre",
                    Description = p.MerchantPromotionDescription ?? "Sin descripción",
                    DiscountPercentage = (decimal)p.DiscountPercentage,
                    MaxRefund = (decimal)p.MaxRefund,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    AvailableQuantity = p.AvailableQuantity,
                    ImageUrl = !string.IsNullOrEmpty(p.MerchantPromotionImage) ? p.MerchantPromotionImage : GetDefaultPromotionImage(),
                    PromotionType = p.PromotionType ?? "Descuento",
                    Terms = p.MerchantPromotionTerms ?? "Términos y condiciones aplican"
                }).ToList();
            }
            catch (Exception ex)
            {
                ActivePromotions = new List<MerchantPromotionInfo>();
                if (string.IsNullOrEmpty(Message))
                {
                    Message = $"Error al cargar promociones: {ex.Message}";
                }
            }
        }

        private string GetDefaultPromotionImage()
        {
            return "https://picsum.photos/seed/promo-default/400/200";
        }

        public string GetMerchantCategory()
        {
            if (MerchantInfo == null || string.IsNullOrEmpty(MerchantInfo.MerchantName))
                return "General";

            var name = MerchantInfo.MerchantName.ToLower();
            
            if (name.Contains("super") || name.Contains("market") || name.Contains("tienda"))
                return "Supermercado";
            else if (name.Contains("café") || name.Contains("coffee") || name.Contains("cafetería"))
                return "Cafetería";
            else if (name.Contains("restaurante") || name.Contains("comida") || name.Contains("food"))
                return "Restaurante";
            else if (name.Contains("farmacia") || name.Contains("salud") || name.Contains("medicina"))
                return "Farmacia";
            else if (name.Contains("ropa") || name.Contains("moda") || name.Contains("fashion"))
                return "Moda";
            else if (name.Contains("tecnología") || name.Contains("electrónica") || name.Contains("tech"))
                return "Tecnología";
            else
                return "Comercio";
        }

        public string GetStatusBadgeClass()
        {
            if (MerchantInfo?.ValidationStatus == "Active")
                return "bg-success";
            else
                return "bg-secondary";
        }

        public string GetStatusText()
        {
            return MerchantInfo?.ValidationStatus switch
            {
                "Active" => "Activo",
                "Inactive" => "Inactivo",
                _ => "Sin Estado"
            };
        }
    }

    public class MerchantPromotionInfo
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