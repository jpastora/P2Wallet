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
    public class AdministratorPanelFinancialEntitiesModel : PageModel
    {
        public List<DTOs.FinancialEntity> AllFinancialEntities { get; set; } = new();
        public List<DTOs.User> AllUsers { get; set; } = new();
        
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; } = string.Empty;
        
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public int TotalFinancialEntities { get; set; }
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
            LoadFinancialEntities();
            LoadUsers();
            return Page();
        }

        private void LoadFinancialEntities()
        {
            try
            {
                var financialEntityManager = new FinancialEntityManager();
                var allEntities = financialEntityManager.RetrieveAllFinancialEntities();
                
                // Filtro de búsqueda simple
                if (!string.IsNullOrEmpty(SearchTerm))
                {
                    allEntities = allEntities.Where(e => 
                        e.EntityName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                        e.Email.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                        e.TaxID.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                TotalFinancialEntities = allEntities.Count;
                TotalPages = (int)Math.Ceiling(TotalFinancialEntities / (double)PageSize);
                CurrentPage = Math.Max(1, Math.Min(CurrentPage, TotalPages));

                // Paginación simple
                AllFinancialEntities = allEntities
                    .Skip((CurrentPage - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar entidades financieras: {ex.Message}";
                AllFinancialEntities = new List<DTOs.FinancialEntity>();
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

        public IActionResult OnPostToggleStatus(int entityId)
        {
            try
            {
                var financialEntityManager = new FinancialEntityManager();
                var entity = financialEntityManager.RetrieveFinancialEntityById(entityId);
                
                if (entity != null)
                {
                    entity.ValidationStatus = entity.ValidationStatus == "Active" ? "Inactive" : "Active";
                    financialEntityManager.UpdateFinancialEntity(entity);
                    
                    var action = entity.ValidationStatus == "Active" ? "activada" : "desactivada";
                    TempData["SuccessMessage"] = $"Entidad financiera {entity.EntityName} {action} exitosamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Entidad financiera no encontrada.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cambiar estado de la entidad financiera: {ex.Message}";
            }

            return RedirectToPage();
        }

        public IActionResult OnPostAssignEntity(int entityId, int userId)
        {
            try
            {
                var userEntityManager = new UserEntityManager();
                var financialEntityManager = new FinancialEntityManager();
                var userManager = new UserManager();

                var entity = financialEntityManager.RetrieveFinancialEntityById(entityId);
                var user = userManager.RetrieveUserById(userId);

                if (entity == null)
                {
                    TempData["ErrorMessage"] = "Entidad financiera no encontrada.";
                    return RedirectToPage();
                }

                if (user == null)
                {
                    TempData["ErrorMessage"] = "Usuario no encontrado.";
                    return RedirectToPage();
                }

                // Verificar si ya existe la asignación
                var existingAssignments = userEntityManager.RetrieveAllUserEntities();
                var existingAssignment = existingAssignments.FirstOrDefault(ue => ue.UserID == userId && ue.FinancialEntityID == entityId);

                if (existingAssignment != null)
                {
                    TempData["ErrorMessage"] = $"El usuario {user.FirstName} {user.LastName} ya está asignado a la entidad {entity.EntityName}.";
                    return RedirectToPage();
                }

                // Crear nueva asignación
                var userEntity = new UserEntity
                {
                    UserID = userId,
                    FinancialEntityID = entityId
                };

                userEntityManager.CreateUserEntity(userEntity);
                TempData["SuccessMessage"] = $"Entidad financiera {entity.EntityName} asignada exitosamente a {user.FirstName} {user.LastName}.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al asignar entidad financiera: {ex.Message}";
            }

            return RedirectToPage();
        }
    }
}