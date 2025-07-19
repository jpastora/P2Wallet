using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using CoreApp;
using DTOs;
using System.Collections.Generic;
using System.Linq;

namespace WebApp.Pages.Administrator
{
    [Authorize]
    public class AdministratorPanelPromotionsModel : PageModel
    {
        public List<PromotionViewModel> AllPromotions { get; set; } = new();
        
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; } = string.Empty;
        
        [BindProperty(SupportsGet = true)]
        public string PromotionTypeFilter { get; set; } = string.Empty;
        
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public int TotalPromotions { get; set; }
        public bool IsAdmin { get; set; }

        // Propiedades para paginación
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
        public int PreviousPage => CurrentPage - 1;
        public int NextPage => CurrentPage + 1;

        // Solo dos estados principales
        public static string EstadoEspanol(string status) => status switch
        {
            "Active" => "Activo",
            "Inactive" => "Inactivo",
            _ => "Inactivo"
        };

        public IActionResult OnGet()
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToPage("/Login");
            }

            var userManager = new UserManager();
            var currentUser = userManager.RetrieveUserByEmail(new DTOs.User { Email = email });
            
            if (currentUser == null || currentUser.Role != "Admin")
            {
                return RedirectToPage("/User/UserPanel");
            }

            IsAdmin = true;
            LoadPromotions();
            return Page();
        }

        private void LoadPromotions()
        {
            try
            {
                var merchantPromotionManager = new MerchantPromotionManager();
                var financialPromotionManager = new FinancialPromotionManager();
                var merchantManager = new MerchantManager();
                var financialEntityManager = new FinancialEntityManager();

                var allPromotions = new List<PromotionViewModel>();

                // Obtener todas las promociones de comercios
                if (string.IsNullOrEmpty(PromotionTypeFilter) || PromotionTypeFilter == "Merchant")
                {
                    var merchantPromotions = merchantPromotionManager.RetrieveAllPromotions();
                    var merchants = merchantManager.RetrieveAllMerchants().ToDictionary(m => m.ID, m => m.MerchantName);

                    foreach (var promo in merchantPromotions)
                    {
                        allPromotions.Add(new PromotionViewModel
                        {
                            ID = promo.ID,
                            Name = promo.MerchantPromotionName,
                            Description = promo.MerchantPromotionDescription,
                            Type = "Comercio",
                            SourceName = merchants.ContainsKey(promo.MerchantID) ? merchants[promo.MerchantID] : "Comercio Desconocido",
                            DiscountPercentage = (decimal)promo.DiscountPercentage,
                            StartDate = promo.StartDate,
                            EndDate = promo.EndDate,
                            AvailableQuantity = promo.AvailableQuantity,
                            ValidationStatus = promo.ValidationStatus,
                            PromotionType = promo.PromotionType,
                            MaxRefund = (decimal)promo.MaxRefund
                        });
                    }
                }

                // Obtener todas las promociones de entidades financieras
                if (string.IsNullOrEmpty(PromotionTypeFilter) || PromotionTypeFilter == "Financial")
                {
                    var financialPromotions = financialPromotionManager.RetrieveAllPromotions();
                    var entities = financialEntityManager.RetrieveAllFinancialEntities().ToDictionary(e => e.ID, e => e.EntityName);

                    foreach (var promo in financialPromotions)
                    {
                        allPromotions.Add(new PromotionViewModel
                        {
                            ID = promo.ID,
                            Name = promo.FinancialPromotionName,
                            Description = promo.FinancialPromotionDescription,
                            Type = "Entidad Financiera",
                            SourceName = entities.ContainsKey(promo.FinancialEntityID) ? entities[promo.FinancialEntityID] : "Entidad Desconocida",
                            DiscountPercentage = (decimal)promo.DiscountPercentage,
                            StartDate = promo.StartDate,
                            EndDate = promo.EndDate,
                            AvailableQuantity = promo.AvailableQuantity,
                            ValidationStatus = promo.ValidationStatus,
                            PromotionType = promo.PromotionType,
                            MaxRefund = (decimal)promo.MaxRefund
                        });
                    }
                }

                // Filtro de búsqueda simple
                if (!string.IsNullOrEmpty(SearchTerm))
                {
                    allPromotions = allPromotions.Where(p => 
                        p.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                        p.SourceName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                        p.Description.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                // Ordenar por fecha de creación (más recientes primero)
                allPromotions = allPromotions.OrderByDescending(p => p.StartDate).ToList();

                TotalPromotions = allPromotions.Count;
                TotalPages = (int)Math.Ceiling(TotalPromotions / (double)PageSize);
                CurrentPage = Math.Max(1, Math.Min(CurrentPage, TotalPages));

                // Paginación simple
                AllPromotions = allPromotions
                    .Skip((CurrentPage - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar promociones: {ex.Message}";
                AllPromotions = new List<PromotionViewModel>();
            }
        }

        public IActionResult OnPostToggleStatus(int promotionId, string promotionType)
        {
            try
            {
                if (promotionType == "Comercio")
                {
                    var merchantPromotionManager = new MerchantPromotionManager();
                    var promotion = merchantPromotionManager.RetrievePromotionById(promotionId);
                    
                    if (promotion != null)
                    {
                        promotion.ValidationStatus = promotion.ValidationStatus == "Active" ? "Inactive" : "Active";
                        merchantPromotionManager.UpdatePromotion(promotion);
                        
                        var action = promotion.ValidationStatus == "Active" ? "activada" : "desactivada";
                        TempData["SuccessMessage"] = $"Promoción de comercio {promotion.MerchantPromotionName} {action} exitosamente.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Promoción de comercio no encontrada.";
                    }
                }
                else if (promotionType == "Entidad Financiera")
                {
                    var financialPromotionManager = new FinancialPromotionManager();
                    var promotion = financialPromotionManager.RetrievePromotionById(promotionId);
                    
                    if (promotion != null)
                    {
                        promotion.ValidationStatus = promotion.ValidationStatus == "Active" ? "Inactive" : "Active";
                        financialPromotionManager.UpdatePromotion(promotion);
                        
                        var action = promotion.ValidationStatus == "Active" ? "activada" : "desactivada";
                        TempData["SuccessMessage"] = $"Promoción financiera {promotion.FinancialPromotionName} {action} exitosamente.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Promoción financiera no encontrada.";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cambiar estado de la promoción: {ex.Message}";
            }

            return RedirectToPage();
        }

        public class PromotionViewModel
        {
            public int ID { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty; // "Comercio" o "Entidad Financiera"
            public string SourceName { get; set; } = string.Empty;
            public decimal DiscountPercentage { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public int AvailableQuantity { get; set; }
            public string ValidationStatus { get; set; } = string.Empty;
            public string PromotionType { get; set; } = string.Empty;
            public decimal MaxRefund { get; set; }
        }
    }
}