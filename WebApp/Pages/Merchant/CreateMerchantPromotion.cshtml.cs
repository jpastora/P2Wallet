using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using DTOs;
using CoreApp;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace WebApp.Pages.Merchant
{
    [Authorize]
    public class CreateMerchantPromotionModel : PageModel
    {
        [BindProperty]
        public int MerchantID { get; set; }
        [BindProperty]
        public string MerchantPromotionName { get; set; }
        [BindProperty]
        public string MerchantPromotionDescription { get; set; }
        [BindProperty]
        public string MerchantPromotionTerms { get; set; }
        [BindProperty]
        public string MerchantPromotionImage { get; set; }
        [BindProperty]
        public string PromotionType { get; set; }
        [BindProperty]
        public double DiscountPercentage { get; set; }
        [BindProperty]
        public double MaxRefund { get; set; }
        [BindProperty]
        public DateTime StartDate { get; set; } = DateTime.Now;
        [BindProperty]
        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(30);
        [BindProperty]
        public int AvailableQuantity { get; set; } = 100;

        public string MerchantName { get; set; }
        public string Message { get; set; }
        public bool IsAuthorized { get; set; }

        public async Task<IActionResult> OnGetAsync(int merchantId)
        {
            if (merchantId <= 0)
                return NotFound();

            MerchantID = merchantId;

            // Verificar autorización del usuario
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
                return RedirectToPage("/Login");

            var userManager = new UserManager();
            var currentUser = userManager.RetrieveUserByEmail(new DTOs.User { Email = email });
            if (currentUser == null)
                return RedirectToPage("/Login");

            // Verificar que el usuario sea admin o esté asignado al comercio
            if (currentUser.Role == "Admin")
            {
                IsAuthorized = true;
            }
            else
            {
                var userMerchantManager = new UserMerchantManager();
                var userMerchants = userMerchantManager.RetrieveAllUserMerchants();
                IsAuthorized = userMerchants.Any(um => um.UserID == currentUser.ID && um.MerchantID == merchantId);
            }

            if (!IsAuthorized)
                return RedirectToPage("/User/UserProfile");

            // Obtener información del comercio
            var merchantManager = new MerchantManager();
            var merchant = merchantManager.RetrieveMerchantById(merchantId);
            if (merchant == null)
                return NotFound();

            MerchantName = merchant.MerchantName;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // Validaciones básicas
                if (string.IsNullOrWhiteSpace(MerchantPromotionName))
                {
                    Message = "El nombre de la promoción es obligatorio.";
                    await ReloadMerchantData();
                    return Page();
                }

                if (string.IsNullOrWhiteSpace(MerchantPromotionDescription))
                {
                    Message = "La descripción de la promoción es obligatoria.";
                    await ReloadMerchantData();
                    return Page();
                }

                if (EndDate <= StartDate)
                {
                    Message = "La fecha de fin debe ser posterior a la fecha de inicio.";
                    await ReloadMerchantData();
                    return Page();
                }

                if (DiscountPercentage <= 0 || DiscountPercentage > 100)
                {
                    Message = "El porcentaje de descuento debe estar entre 1 y 100.";
                    await ReloadMerchantData();
                    return Page();
                }

                if (AvailableQuantity <= 0)
                {
                    Message = "La cantidad disponible debe ser mayor a 0.";
                    await ReloadMerchantData();
                    return Page();
                }

                // Crear la promoción
                var promotion = new MerchantPromotion
                {
                    MerchantID = MerchantID,
                    MerchantPromotionName = MerchantPromotionName,
                    MerchantPromotionDescription = MerchantPromotionDescription,
                    MerchantPromotionTerms = MerchantPromotionTerms ?? "",
                    MerchantPromotionImage = MerchantPromotionImage ?? "",
                    PromotionType = PromotionType,
                    DiscountPercentage = DiscountPercentage,
                    MaxRefund = MaxRefund,
                    StartDate = StartDate,
                    EndDate = EndDate,
                    AvailableQuantity = AvailableQuantity,
                    ValidationStatus = "Active",
                    CreatedAt = DateTime.Now
                };

                var merchantPromotionManager = new MerchantPromotionManager();
                merchantPromotionManager.CreatePromotion(promotion);

                return RedirectToPage("/User/UserProfile", new { message = "Promoción creada exitosamente." });
            }
            catch (Exception ex)
            {
                Message = $"Error al crear la promoción: {ex.Message}";
                await ReloadMerchantData();
                return Page();
            }
        }

        private async Task ReloadMerchantData()
        {
            var merchantManager = new MerchantManager();
            var merchant = merchantManager.RetrieveMerchantById(MerchantID);
            if (merchant != null)
            {
                MerchantName = merchant.MerchantName;
            }
        }
    }
}