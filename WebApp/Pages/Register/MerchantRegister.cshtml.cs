using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Register
{
    public class MerchantRegisterModel : PageModel
    {
        public void OnGet()
        {
            var userIdString = User.FindFirst("UserId")?.Value;
            UserId = int.Parse(userIdString);
        }
    }
}
