using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using CoreApp;
using DTOs;
using DataAccess.ServicesAccess;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;

namespace WebApp.Pages.User
{
    public class UserPasswordRecoveryModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Correo inválido")]
        public string Email { get; set; }

        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        private readonly IMemoryCache _cache;
        private readonly EmailService _emailService;

        public UserPasswordRecoveryModel(IMemoryCache cache, EmailService emailService)
        {
            _cache = cache;
            _emailService = emailService;
        }

        public void OnGet() { }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Por favor, introduce un correo válido.";
                return Page();
            }

            var user = new UserManager().RetrieveUserByEmail(new DTOs.User { Email = Email });
            if (user == null)
            {
                ErrorMessage = "No se encontró una cuenta con ese correo.";
                return Page();
            }

            // Generar token seguro
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));
            _cache.Set($"PWD_RESET_{Email}", token, TimeSpan.FromHours(1));

            // Construir enlace de recuperación
            var resetUrl = Url.Page(
                "/User/UserPasswordReset",
                pageHandler: null,
                values: new { email = Email, token = token },
                protocol: Request.Scheme
            );

            var html = $@"<h3>Recuperación de contraseña</h3>
<p>Haz clic en el siguiente enlace para restablecer tu contraseña:</p>
<p><a href='{resetUrl}'>Restablecer contraseña</a></p>
<p>Si no solicitaste este cambio, ignora este mensaje.</p>";
            var enviado = _emailService.EnviarCorreoAsync(Email, "Recuperación de contraseña Yavi", html).GetAwaiter().GetResult();
            if (enviado)
            {
                SuccessMessage = "Si el correo está registrado, recibirás un enlace para restablecer tu contraseña.";
            }
            else
            {
                ErrorMessage = "No se pudo enviar el correo de recuperación. Intenta más tarde.";
            }
            return Page();
        }
    }
}
