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
        public PromotionDto PromotionApplied { get; set; }
        public double DiscountAmount { get; set; }

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

                    // Lógica de promoción y descuento
                    PromotionApplied = null;
                    DiscountAmount = 0;
                    if (Transaction.MerchantPromotionID.HasValue)
                    {
                        var promoManager = new MerchantPromotionManager();
                        var promo = promoManager.RetrievePromotionById(Transaction.MerchantPromotionID.Value);
                        if (promo != null)
                        {
                            PromotionApplied = new PromotionDto
                            {
                                Name = promo.MerchantPromotionName,
                                DiscountPercentage = promo.DiscountPercentage,
                                TypeText = "Promoción del Comercio"
                            };
                            DiscountAmount = Transaction.GrossAmount * (promo.DiscountPercentage / 100.0);
                        }
                    }
                    else if (Transaction.FinancialPromotionID.HasValue)
                    {
                        var promoManager = new FinancialPromotionManager();
                        var promo = promoManager.RetrievePromotionById(Transaction.FinancialPromotionID.Value);
                        if (promo != null)
                        {
                            PromotionApplied = new PromotionDto
                            {
                                Name = promo.FinancialPromotionName,
                                DiscountPercentage = promo.DiscountPercentage,
                                TypeText = "Promoción del Banco"
                            };
                            DiscountAmount = Transaction.GrossAmount * (promo.DiscountPercentage / 100.0);
                        }
                    }
                }
            }
            return Page();
        }
    }

    public class PromotionDto
    {
        public string Name { get; set; }
        public double DiscountPercentage { get; set; }
        public string TypeText { get; set; }
    }
}
