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
    public class AdministratorPanelUsersModel : PageModel
    {
        public List<DTOs.User> AllUsers { get; set; } = new();
        
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; } = string.Empty;
        
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public int TotalUsers { get; set; }
        public bool IsAdmin { get; set; }

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
            LoadUsers();
            return Page();
        }

        private void LoadUsers()
        {
            try
            {
                var userManager = new UserManager();
                var allUsers = userManager.RetrieveAllUsers();
                
                // Filtro de búsqueda simple
                if (!string.IsNullOrEmpty(SearchTerm))
                {
                    allUsers = allUsers.Where(u => 
                        u.FirstName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                        u.LastName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                        u.Email.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                TotalUsers = allUsers.Count;
                TotalPages = (int)Math.Ceiling(TotalUsers / (double)PageSize);
                CurrentPage = Math.Max(1, Math.Min(CurrentPage, TotalPages));

                // Paginación simple
                AllUsers = allUsers
                    .Skip((CurrentPage - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar usuarios: {ex.Message}";
                AllUsers = new List<DTOs.User>();
            }
        }

        public IActionResult OnPostToggleStatus(int userId)
        {
            try
            {
                var userManager = new UserManager();
                var user = userManager.RetrieveUserById(userId);
                
                if (user != null)
                {
                    // Solo alternar entre Active/Inactive
                    var newStatus = user.ValidationStatus == "Active" ? "Inactive" : "Active";
                    user.ValidationStatus = newStatus;
                    
                    // La validación de campos se hace ahora centralmente en UserManager.UpdateUser
                    userManager.UpdateUser(user);
                    
                    TempData["SuccessMessage"] = $"Usuario {user.FirstName} {user.LastName} {(newStatus == "Active" ? "activado" : "desactivado")} exitosamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Usuario no encontrado.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al actualizar estado: {ex.Message}";
            }

            return RedirectToPage("AdministratorPanelUsers", new { 
                searchTerm = SearchTerm, 
                currentPage = CurrentPage 
            });
        }

        // Métodos para paginación simple
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
        public int PreviousPage => CurrentPage - 1;
        public int NextPage => CurrentPage + 1;
    }
}
