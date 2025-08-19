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
                using var client = new HttpClient();
                client.BaseAddress = new Uri("https://p2wallet-api-eyefddgeeda9c2fk.eastus-01.azurewebsites.net/");
                var loginData = new { email = Email, password = Password };
                var content = new StringContent(JsonSerializer.Serialize(loginData), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/User/Login", content);

                if (response.IsSuccessStatusCode)
                {
                    // Leer el contenido de la respuesta
                    var responseString = await response.Content.ReadAsStringAsync();

                    using var doc = JsonDocument.Parse(responseString);
                    var root = doc.RootElement;

                    // Acceder al objeto "data"
                    var data = root.GetProperty("data");

                    // Obtener valores dentro de "data"
                    var biometricStatus = data.GetProperty("biometricVerified").GetString();
                    var userId = data.GetProperty("id").GetInt32();

                if (biometricStatus == "Inactive")
                    {
                        // Si no tiene la parte biometrica verificada hace redirect a la pagina y le enviar el user ID para oder verificar la biometrica
                        
                        return RedirectToPage("/Security/UserBiometricVerifications", new { id = userId });
                    }
                    else
                    {
                        // Login exitoso y biometrica validado, crear cookie de autenticación
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, Email),
                            new Claim("UserId", userId.ToString())
                        };
                        var claimsIdentity = new ClaimsIdentity(claims, "MyCookieAuth");
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = RememberMe
                        };
                        await HttpContext.SignInAsync("MyCookieAuth", new ClaimsPrincipal(claimsIdentity), authProperties);
                        return RedirectToPage("/User/UserPanel");
                    }

                    
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
