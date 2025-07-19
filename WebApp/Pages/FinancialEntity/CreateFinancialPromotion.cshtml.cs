using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using DTOs;
using CoreApp;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace WebApp.Pages.FinancialEntity
{
    [Authorize]
    public class CreateFinancialPromotionModel : PageModel
    {
        [BindProperty]
        public int FinancialEntityID { get; set; }
        [BindProperty]
        public string FinancialPromotionName { get; set; }
        [BindProperty]
        public string FinancialPromotionDescription { get; set; }
        [BindProperty]
        public string FinancialPromotionTerms { get; set; }
        [BindProperty]
        public string FinancialPromotionImage { get; set; }
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

        public string FinancialEntityName { get; set; }
        public string Message { get; set; }
        public bool IsAuthorized { get; set; }

        public async Task<IActionResult> OnGetAsync(int financialEntityId)
        {
            if (financialEntityId <= 0)
                return NotFound();

            FinancialEntityID = financialEntityId;

            // Verificar autorización del usuario
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
                return RedirectToPage("/Login");

            var userManager = new UserManager();
            var currentUser = userManager.RetrieveUserByEmail(new DTOs.User { Email = email });
            if (currentUser == null)
                return RedirectToPage("/Login");

            // Verificar que el usuario sea admin o esté asignado a la entidad financiera
            if (currentUser.Role == "Admin")
            {
                IsAuthorized = true;
            }
            else
            {
                var userEntityManager = new UserEntityManager();
                var userEntities = userEntityManager.RetrieveAllUserEntities();
                IsAuthorized = userEntities.Any(ue => ue.UserID == currentUser.ID && ue.FinancialEntityID == financialEntityId);
            }

            if (!IsAuthorized)
                return RedirectToPage("/User/UserProfile");

            // Obtener información de la entidad financiera
            var financialEntityManager = new FinancialEntityManager();
            var entity = financialEntityManager.RetrieveFinancialEntityById(financialEntityId);
            if (entity == null)
                return NotFound();

            FinancialEntityName = entity.EntityName;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // Validaciones básicas
                if (string.IsNullOrWhiteSpace(FinancialPromotionName))
                {
                    Message = "El nombre de la promoción es obligatorio.";
                    await ReloadEntityData();
                    return Page();
                }

                if (string.IsNullOrWhiteSpace(FinancialPromotionDescription))
                {
                    Message = "La descripción de la promoción es obligatoria.";
                    await ReloadEntityData();
                    return Page();
                }

                if (EndDate <= StartDate)
                {
                    Message = "La fecha de fin debe ser posterior a la fecha de inicio.";
                    await ReloadEntityData();
                    return Page();
                }

                if (DiscountPercentage <= 0 || DiscountPercentage > 100)
                {
                    Message = "El porcentaje de descuento debe estar entre 1 y 100.";
                    await ReloadEntityData();
                    return Page();
                }

                if (AvailableQuantity <= 0)
                {
                    Message = "La cantidad disponible debe ser mayor a 0.";
                    await ReloadEntityData();
                    return Page();
                }

                // Crear la promoción
                var promotion = new FinancialPromotion
                {
                    FinancialEntityID = FinancialEntityID,
                    FinancialPromotionName = FinancialPromotionName,
                    FinancialPromotionDescription = FinancialPromotionDescription,
                    FinancialPromotionTerms = FinancialPromotionTerms ?? "",
                    FinancialPromotionImage = FinancialPromotionImage ?? "",
                    PromotionType = PromotionType,
                    DiscountPercentage = DiscountPercentage,
                    MaxRefund = MaxRefund,
                    StartDate = StartDate,
                    EndDate = EndDate,
                    AvailableQuantity = AvailableQuantity,
                    ValidationStatus = "Active",
                    CreatedAt = DateTime.Now
                };

                var financialPromotionManager = new FinancialPromotionManager();
                financialPromotionManager.CreatePromotion(promotion);

                return RedirectToPage("/User/UserProfile", new { message = "Promoción financiera creada exitosamente." });
            }
            catch (Exception ex)
            {
                Message = $"Error al crear la promoción: {ex.Message}";
                await ReloadEntityData();
                return Page();
            }
        }

        private async Task ReloadEntityData()
        {
            var financialEntityManager = new FinancialEntityManager();
            var entity = financialEntityManager.RetrieveFinancialEntityById(FinancialEntityID);
            if (entity != null)
            {
                FinancialEntityName = entity.EntityName;
            }
        }
    }
}