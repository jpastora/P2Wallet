using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using DTOs;
using CoreApp;

namespace WebApp.Pages.User
{
    [Authorize]
    public class UserTransactionInvoiceModel : PageModel
    {
        public Transaction Transaction { get; set; }
        public DTOs.User CurrentUser { get; set; }
        public string MerchantName { get; set; }
        public string IBAN { get; set; }
        public string BankName { get; set; }

        public IActionResult OnGet(int id)
        {
            var email = User.Identity?.Name;
            if (!string.IsNullOrEmpty(email))
            {
                var userManager = new UserManager();
                CurrentUser = userManager.RetrieveUserByEmail(new DTOs.User { Email = email });
                if (CurrentUser != null)
                {
                    var transactionManager = new TransactionManager();
                    Transaction = transactionManager.RetrieveTransactionById(id);
                    if (Transaction == null || Transaction.UserID != CurrentUser.ID)
                    {
                        return NotFound();
                    }
                    // Obtener datos relacionados
                    var merchantManager = new MerchantManager();
                    var merchant = merchantManager.RetrieveMerchantById(Transaction.MerchantID);
                    MerchantName = merchant?.MerchantName ?? "-";

                    var accountManager = new BankAccountManager();
                    var account = accountManager.RetrieveBankAccountById(Transaction.BankAccountID);
                    IBAN = account?.IBAN ?? "-";

                    var bankName = "-";
                    if (account != null)
                    {
                        var entityManager = new FinancialEntityManager();
                        var entity = entityManager.RetrieveFinancialEntityById(account.FinancialEntityID);
                        bankName = entity?.EntityName ?? "-";
                    }
                    BankName = bankName;
                }
            }
            return Page();
        }
    }
}
