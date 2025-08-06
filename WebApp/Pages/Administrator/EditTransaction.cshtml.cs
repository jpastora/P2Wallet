using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using CoreApp;
using DTOs;

namespace WebApp.Pages.Administrator
{
    [Authorize]
    public class EditTransactionModel : PageModel
    {
        [BindProperty]
        public DTOs.Transaction TransactionToEdit { get; set; } = new();

        public string Message { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }

        // Make dictionaries public properties so they persist across requests
        public Dictionary<int, string> UserNames { get; set; } = new();
        public Dictionary<int, string> MerchantNames { get; set; } = new();
        public Dictionary<int, string> BankAccountInfo { get; set; } = new();

        // Traducción de estados
        public static string EstadoEspanol(string status) => status switch
        {
            "Completed" => "Completada",
            "Pending" => "Pendiente",
            "PendingUserApproval" => "Pendiente de Aprobación",
            "Failed" => "Fallida",
            "Cancelled" => "Cancelada",
            "Processing" => "Procesando",
            "Rejected" => "Rechazada",
            _ => status ?? "Desconocido"
        };

        public IActionResult OnGet(int transactionId)
        {
            // Verificar que el usuario actual sea admin
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToPage("/Login");
            }

            var userManager = new UserManager();
            var currentUser = userManager.RetrieveUserByEmail(new DTOs.User { Email = email });
            
            if (currentUser == null || currentUser.Role != "Admin")
            {
                return RedirectToPage("/User/UserPanel");
            }

            IsAdmin = true;

            // Cargar la transacción a editar
            var transactionManager = new TransactionManager();
            TransactionToEdit = transactionManager.RetrieveTransactionById(transactionId);
            if (TransactionToEdit == null)
            {
                TempData["ErrorMessage"] = "Transacción no encontrada.";
                return RedirectToPage("AdministratorPanelTransactions");
            }

            LoadReferenceData();
            return Page();
        }

        public IActionResult OnPost()
        {
            try
            {
                // Always reload reference data first
                LoadReferenceData();

                var transactionManager = new TransactionManager();
                
                // Cargar la transacción completa desde la base de datos para preservar todos los campos
                var existingTransaction = transactionManager.RetrieveTransactionById(TransactionToEdit.ID);
                if (existingTransaction == null)
                {
                    Message = "Transacción no encontrada.";
                    return Page();
                }

                // Actualizar los campos editables del formulario
                existingTransaction.TransactionStatus = TransactionToEdit.TransactionStatus;
                existingTransaction.GrossAmount = TransactionToEdit.GrossAmount;
                existingTransaction.NetAmount = TransactionToEdit.NetAmount;
                existingTransaction.CommissionApplied = TransactionToEdit.CommissionApplied;
                existingTransaction.SalesTaxAmount = TransactionToEdit.SalesTaxAmount;
                existingTransaction.TaxRateApplied = TransactionToEdit.TaxRateApplied;
                existingTransaction.Timestamp = TransactionToEdit.Timestamp;
                existingTransaction.MerchantPromotionID = TransactionToEdit.MerchantPromotionID;
                existingTransaction.FinancialPromotionID = TransactionToEdit.FinancialPromotionID;

                // Los campos UserID, MerchantID y BankAccountID no se actualizan por seguridad

                transactionManager.UpdateTransaction(existingTransaction);
                TempData["SuccessMessage"] = $"Transacción #{existingTransaction.ID} actualizada exitosamente.";
                return RedirectToPage("AdministratorPanelTransactions");
            }
            catch (Exception ex)
            {
                Message = $"Error al actualizar transacción: {ex.Message}";
                return Page();
            }
        }

        private void LoadReferenceData()
        {
            try
            {
                var userManager = new UserManager();
                var merchantManager = new MerchantManager();
                var bankAccountManager = new BankAccountManager();
                var financialEntityManager = new FinancialEntityManager();

                // Cargar nombres de usuarios
                var allUsers = userManager.RetrieveAllUsers();
                UserNames = allUsers.ToDictionary(u => u.ID, u => $"{u.FirstName} {u.LastName}");

                // Cargar nombres de comercios
                var allMerchants = merchantManager.RetrieveAllMerchants();
                MerchantNames = allMerchants.ToDictionary(m => m.ID, m => m.MerchantName ?? "Sin nombre");

                // Cargar información de cuentas bancarias
                var allBankAccounts = bankAccountManager.RetrieveAllBankAccounts();
                var allFinancialEntities = financialEntityManager.RetrieveAllFinancialEntities();
                var entityNames = allFinancialEntities.ToDictionary(e => e.ID, e => e.EntityName ?? "Sin nombre");

                BankAccountInfo = allBankAccounts.ToDictionary(
                    ba => ba.ID, 
                    ba => {
                        var entityName = entityNames.ContainsKey(ba.FinancialEntityID) ? entityNames[ba.FinancialEntityID] : "Entidad Desconocida";
                        return $"{ba.IBAN} ({entityName})";
                    });
            }
            catch (Exception ex)
            {
                Message = $"Error al cargar datos de referencia: {ex.Message}";
            }
        }

        public string GetUserName(int userId)
        {
            return UserNames.ContainsKey(userId) ? UserNames[userId] : $"Usuario #{userId}";
        }

        public string GetMerchantName(int merchantId)
        {
            return MerchantNames.ContainsKey(merchantId) ? MerchantNames[merchantId] : $"Comercio #{merchantId}";
        }

        public string GetBankAccountInfo(int bankAccountId)
        {
            return BankAccountInfo.ContainsKey(bankAccountId) ? BankAccountInfo[bankAccountId] : $"Cuenta #{bankAccountId}";
        }
    }
}