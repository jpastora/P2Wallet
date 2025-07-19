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
    public class AdministratorPanelMerchantsModel : PageModel
    {
        public List<DTOs.Merchant> AllMerchants { get; set; } = new();
        
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; } = string.Empty;
        
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public int TotalMerchants { get; set; }
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
            LoadMerchants();
            return Page();
        }

        private void LoadMerchants()
        {
            try
            {
                var merchantManager = new MerchantManager();
                var allMerchants = merchantManager.RetrieveAllMerchants();
                
                // Filtro de búsqueda simple
                if (!string.IsNullOrEmpty(SearchTerm))
                {
                    allMerchants = allMerchants.Where(m => 
                        m.MerchantName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                        m.Email.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                        m.TaxID.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                TotalMerchants = allMerchants.Count;
                TotalPages = (int)Math.Ceiling(TotalMerchants / (double)PageSize);
                CurrentPage = Math.Max(1, Math.Min(CurrentPage, TotalPages));

                // Paginación simple
                AllMerchants = allMerchants
                    .Skip((CurrentPage - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar comercios: {ex.Message}";
                AllMerchants = new List<DTOs.Merchant>();
            }
        }

        public IActionResult OnPostToggleStatus(int merchantId)
        {
            try
            {
                var merchantManager = new MerchantManager();
                var merchant = merchantManager.RetrieveMerchantById(merchantId);
                
                if (merchant != null)
                {
                    merchant.ValidationStatus = merchant.ValidationStatus == "Active" ? "Inactive" : "Active";
                    merchantManager.UpdateMerchant(merchant);
                    
                    var action = merchant.ValidationStatus == "Active" ? "activado" : "desactivado";
                    TempData["SuccessMessage"] = $"Comercio {merchant.MerchantName} {action} exitosamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Comercio no encontrado.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cambiar estado del comercio: {ex.Message}";
            }

            return RedirectToPage();
        }
    }
}