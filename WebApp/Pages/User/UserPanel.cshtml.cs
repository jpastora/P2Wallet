using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using DTOs;
using CoreApp;

namespace WebApp.Pages.User
{
    [Authorize]
    public class UserPanelModel : PageModel
    {
        public DTOs.User CurrentUser { get; set; }
        public List<BankAccount> UserAccounts { get; set; } = new();

        public void OnGet()
        {
            var email = User.Identity?.Name;
            if (!string.IsNullOrEmpty(email))
            {
                var userManager = new UserManager();
                CurrentUser = userManager.RetrieveUserByEmail(new DTOs.User { Email = email });
                if (CurrentUser != null)
                {
                    var accountManager = new BankAccountManager();
                    var allAccounts = accountManager.RetrieveAllBankAccounts();
                    UserAccounts = allAccounts.FindAll(a => a.UserID == CurrentUser.ID);
                }
            }
        }
    }
}
