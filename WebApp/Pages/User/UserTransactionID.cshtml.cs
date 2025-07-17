using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using DTOs;
using CoreApp;
using System.Collections.Generic;
using System.Linq;

namespace WebApp.Pages.User
{
    [Authorize]
    public class UserTransactionIDModel : PageModel
    {
        public List<Transaction> UserTransactions { get; set; } = new();
        public DTOs.User CurrentUser { get; set; }
        public Dictionary<int, string> MerchantNames { get; set; } = new();
        public Dictionary<int, string> AccountIBANs { get; set; } = new();
        public Dictionary<int, string> BankNames { get; set; } = new();

        public void OnGet()
        {
            var email = User.Identity?.Name;
            if (!string.IsNullOrEmpty(email))
            {
                var userManager = new UserManager();
                CurrentUser = userManager.RetrieveUserByEmail(new DTOs.User { Email = email });
                if (CurrentUser != null)
                {
                    var transactionManager = new TransactionManager();
                    var allTransactions = transactionManager.RetrieveAllTransactions();
                    UserTransactions = allTransactions.Where(t => t.UserID == CurrentUser.ID).ToList();

                    // Obtener datos relacionados
                    var merchantManager = new MerchantManager();
                    var allMerchants = merchantManager.RetrieveAllMerchants();
                    MerchantNames = allMerchants.ToDictionary(m => m.ID, m => m.MerchantName);

                    var accountManager = new BankAccountManager();
                    var allAccounts = accountManager.RetrieveAllBankAccounts();
                    AccountIBANs = allAccounts.ToDictionary(a => a.ID, a => a.IBAN);

                    var entityManager = new FinancialEntityManager();
                    var allEntities = entityManager.RetrieveAllFinancialEntities();
                    BankNames = allEntities.ToDictionary(e => e.ID, e => e.EntityName);
                }
            }
        }
    }
}
