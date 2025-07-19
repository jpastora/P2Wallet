using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using DTOs;
using CoreApp;
using System.Collections.Generic;
using DataAccess.CRUD;

namespace WebApp.Pages.User
{
    [Authorize]
    public class UserBankAccountModel : PageModel
    {
        public DTOs.User CurrentUser { get; set; }
        public List<BankAccount> UserAccounts { get; set; } = new();
        public List<DTOs.FinancialEntity> FinancialEntities { get; set; } = new();

        [BindProperty]
        public BankAccount NewAccount { get; set; }

        public string Message { get; set; }

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
                    // Mostrar solo cuentas activas
                    UserAccounts = allAccounts.FindAll(a => a.UserID == CurrentUser.ID && a.ValidationStatus == "Active");

                    var entityManager = new FinancialEntityCrudFactory();
                    FinancialEntities = entityManager.RetrieveAll<DTOs.FinancialEntity>();
                }
            }
        }

        public IActionResult OnPostAdd()
        {
            var email = User.Identity?.Name;
            if (!string.IsNullOrEmpty(email))
            {
                var userManager = new UserManager();
                CurrentUser = userManager.RetrieveUserByEmail(new DTOs.User { Email = email });
                if (CurrentUser != null && NewAccount != null)
                {
                    NewAccount.UserID = CurrentUser.ID;
                    // Guardar en inglés para la base de datos
                    NewAccount.ValidationStatus = "Inactive";
                    var accountManager = new BankAccountManager();
                    try
                    {
                        accountManager.CreateBankAccount(NewAccount);
                        TempData["SuccessMessage"] = "Cuenta agregada correctamente.";
                        return RedirectToPage();
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("UQ_BankAccounts_IBAN") || ex.Message.Contains("ya existe"))
                        {
                            TempData["ErrorMessage"] = "No se puede agregar la cuenta: ya fue registrada anteriormente.";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Ocurrió un error al agregar la cuenta: " + ex.Message;
                        }
                        return RedirectToPage();
                    }
                }
            }
            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            var accountManager = new BankAccountManager();
            var account = accountManager.RetrieveBankAccountById(id);
            if (account != null)
            {
                try
                {
                    // Desactivar la cuenta en vez de eliminar
                    account.ValidationStatus = "Inactive";
                    accountManager.UpdateBankAccount(account);
                    TempData["SuccessMessage"] = "Cuenta desactivada correctamente.";
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("REFERENCE constraint") || ex.Message.Contains("FK__Transacti__BankA"))
                    {
                        TempData["ErrorMessage"] = "No se puede desactivar la cuenta porque tiene transacciones asociadas.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Ocurrió un error al desactivar la cuenta: " + ex.Message;
                    }
                }
            }
            return RedirectToPage();
        }

        public IActionResult OnPostToggleStatus(int id)
        {
            var accountManager = new BankAccountManager();
            var account = accountManager.RetrieveBankAccountById(id);
            if (account != null)
            {
                // Desactivar la cuenta (no alternar, solo desactivar)
                account.ValidationStatus = "Inactive";
                accountManager.UpdateBankAccount(account);
                TempData["SuccessMessage"] = "Cuenta desactivada correctamente.";
            }
            return RedirectToPage();
        }
    }
}
