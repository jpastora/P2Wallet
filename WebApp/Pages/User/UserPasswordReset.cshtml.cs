using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using CoreApp;
using DTOs;
using DataAccess.ServicesAccess;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace WebApp.Pages.User
{
    public class UserPasswordResetModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Email { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Token { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
        public string NewPassword { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Confirma la contraseña")]
        [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; }

        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        private readonly IMemoryCache _cache;
        private readonly ILogger<UserPasswordResetModel> _logger;

        public UserPasswordResetModel(IMemoryCache cache, ILogger<UserPasswordResetModel> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public void OnGet() { }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Por favor, corrige los errores.";
                return Page();
            }
            if (!_cache.TryGetValue($"PWD_RESET_{Email}", out string tokenGuardado) || tokenGuardado != Token)
            {
                ErrorMessage = "El enlace de recuperación es inválido o ha expirado.";
                return Page();
            }
            var userManager = new UserManager();
            // Recuperar el usuario completo de la base de datos
            var user = userManager.RetrieveUserByEmail(new DTOs.User { Email = Email });
            if (user == null)
            {
                ErrorMessage = "No se encontró el usuario.";
                return Page();
            }
            // Solo actualiza la contraseña, pero mantiene el resto de propiedades
            user.Password = NewPassword;
            userManager.UpdateUser(user);
            _cache.Remove($"PWD_RESET_{Email}");
            SuccessMessage = "¡Contraseña restablecida exitosamente! Ya puedes iniciar sesión.";
            return Page();
        }
    }
}
