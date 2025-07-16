using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebApp.Pages
{
    public class loginModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public string Password { get; set; }
        [BindProperty]
        public bool RememberMe { get; set; }
        public string ErrorMessage { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // Llamar al API de login
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://p2wallet-api-eyefddgeeda9c2fk.eastus-01.azurewebsites.net/");
            var loginData = new { email = Email, password = Password };
            var content = new StringContent(JsonSerializer.Serialize(loginData), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/User/Login", content);

            if (response.IsSuccessStatusCode)
            {
                // Login exitoso, crear cookie de autenticación
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, Email)
                };
                var claimsIdentity = new ClaimsIdentity(claims, "MyCookieAuth");
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = RememberMe
                };
                await HttpContext.SignInAsync("MyCookieAuth", new ClaimsPrincipal(claimsIdentity), authProperties);
                return RedirectToPage("/User/UserPanel");
            }
            else
            {
                // Mostrar error del API
                var apiError = await response.Content.ReadAsStringAsync();
                try
                {
                    var errorObj = JsonSerializer.Deserialize<JsonElement>(apiError);
                    if (errorObj.TryGetProperty("message", out var msg))
                        ErrorMessage = msg.GetString();
                    else
                        ErrorMessage = "Usuario o contraseña incorrectos.";
                }
                catch
                {
                    ErrorMessage = "Usuario o contraseña incorrectos.";
                }
                return Page();
            }
        }
    }
}
