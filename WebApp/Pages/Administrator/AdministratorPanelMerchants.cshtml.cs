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
        public List<DTOs.User> AllUsers { get; set; } = new();
        
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
            LoadUsers();
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

        private void LoadUsers()
        {
            try
            {
                var userManager = new UserManager();
                AllUsers = userManager.RetrieveAllUsers()
                    .Where(u => u.ValidationStatus == "Active")
                    .OrderBy(u => u.FirstName)
                    .ThenBy(u => u.LastName)
                    .ToList();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar usuarios: {ex.Message}";
                AllUsers = new List<DTOs.User>();
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

        public IActionResult OnPostAssignMerchant(int merchantId, int userId)
        {
            try
            {
                var userMerchantManager = new UserMerchantManager();
                var merchantManager = new MerchantManager();
                var userManager = new UserManager();

                var merchant = merchantManager.RetrieveMerchantById(merchantId);
                var user = userManager.RetrieveUserById(userId);

                if (merchant == null)
                {
                    TempData["ErrorMessage"] = "Comercio no encontrado.";
                    return RedirectToPage();
                }

                if (user == null)
                {
                    TempData["ErrorMessage"] = "Usuario no encontrado.";
                    return RedirectToPage();
                }

                // Verificar si ya existe la asignación
                var existingAssignments = userMerchantManager.RetrieveAllUserMerchants();
                var existingAssignment = existingAssignments.FirstOrDefault(um => um.UserID == userId && um.MerchantID == merchantId);

                if (existingAssignment != null)
                {
                    TempData["ErrorMessage"] = $"El usuario {user.FirstName} {user.LastName} ya está asignado al comercio {merchant.MerchantName}.";
                    return RedirectToPage();
                }

                // Crear nueva asignación
                var userMerchant = new UserMerchant
                {
                    UserID = userId,
                    MerchantID = merchantId
                };

                userMerchantManager.CreateUserMerchant(userMerchant);
                TempData["SuccessMessage"] = $"Comercio {merchant.MerchantName} asignado exitosamente a {user.FirstName} {user.LastName}.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al asignar comercio: {ex.Message}";
            }

            return RedirectToPage();
        }
    }
}