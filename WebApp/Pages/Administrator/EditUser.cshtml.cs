using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using CoreApp;
using DTOs;

namespace WebApp.Pages.Administrator
{
    [Authorize]
    public class EditUserModel : PageModel
    {
        [BindProperty]
        public DTOs.User UserToEdit { get; set; } = new();

        [BindProperty]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }

        public IActionResult OnGet(int userId)
        {
            // Verificar que el usuario actual sea admin
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

            // Cargar el usuario a editar
            UserToEdit = userManager.RetrieveUserById(userId);
            if (UserToEdit == null)
            {
                TempData["ErrorMessage"] = "Usuario no encontrado.";
                return RedirectToPage("AdministratorPanelUsers");
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            try
            {
                var userManager = new UserManager();
                
                // Cargar el usuario completo desde la base de datos para preservar todos los campos
                var existingUser = userManager.RetrieveUserById(UserToEdit.ID);
                if (existingUser == null)
                {
                    Message = "Usuario no encontrado.";
                    return Page();
                }

                // Actualizar solo los campos editables del formulario
                existingUser.FirstName = UserToEdit.FirstName;
                existingUser.LastName = UserToEdit.LastName;
                existingUser.Email = UserToEdit.Email;
                existingUser.MobilePhone = UserToEdit.MobilePhone;
                existingUser.IDNumber = UserToEdit.IDNumber;
                existingUser.BirthDate = UserToEdit.BirthDate;
                existingUser.ValidationStatus = UserToEdit.ValidationStatus;
                existingUser.Role = UserToEdit.Role;

                // Si se proporcionó una nueva contraseña
                if (!string.IsNullOrEmpty(UserToEdit.Password))
                {
                    if (UserToEdit.Password != ConfirmPassword)
                    {
                        Message = "Las contraseñas no coinciden.";
                        UserToEdit = existingUser; // Restaurar datos para mostrar en el formulario
                        return Page();
                    }
                    existingUser.Password = UserToEdit.Password;
                }
                // Si no se proporciona nueva contraseña, mantener la actual (ya está en existingUser)

                // La validación de campos se hace ahora centralmente en UserManager.UpdateUser
                userManager.UpdateUser(existingUser);
                TempData["SuccessMessage"] = $"Usuario {existingUser.FirstName} {existingUser.LastName} actualizado exitosamente.";
                return RedirectToPage("AdministratorPanelUsers");
            }
            catch (Exception ex)
            {
                Message = $"Error al actualizar usuario: {ex.Message}";
                return Page();
            }
        }
    }
}