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

        // Propiedades para el ordenamiento
        [BindProperty(SupportsGet = true)]
        public string SortBy { get; set; } = "ID";

        [BindProperty(SupportsGet = true)]
        public string SortDirection { get; set; } = "desc";

        // Propiedades auxiliares para la vista
        public string CurrentSort { get; set; }
        public string IdSort { get; set; }
        public string DateSort { get; set; }
        public string MerchantSort { get; set; }
        public string AmountSort { get; set; }
        public string StatusSort { get; set; }

        public void OnGet()
        {
            var email = User.Identity?.Name;
            if (!string.IsNullOrEmpty(email))
            {
                var userManager = new UserManager();
                CurrentUser = userManager.RetrieveUserByEmail(new DTOs.User { Email = email });
                if (CurrentUser != null)
                {
                    LoadTransactions();
                    LoadReferenceData();
                    SetSortProperties();
                }
            }
        }

        private void LoadTransactions()
        {
            var transactionManager = new TransactionManager();
            var allTransactions = transactionManager.RetrieveAllTransactions();
            var userTransactions = allTransactions.Where(t => t.UserID == CurrentUser.ID);

            // Aplicar ordenamiento
            userTransactions = SortBy.ToLower() switch
            {
                "id" => SortDirection == "asc" 
                    ? userTransactions.OrderBy(t => t.ID) 
                    : userTransactions.OrderByDescending(t => t.ID),
                "date" or "fecha" => SortDirection == "asc" 
                    ? userTransactions.OrderBy(t => t.Timestamp) 
                    : userTransactions.OrderByDescending(t => t.Timestamp),
                "merchant" or "comercio" => SortDirection == "asc" 
                    ? userTransactions.OrderBy(t => t.MerchantID) 
                    : userTransactions.OrderByDescending(t => t.MerchantID),
                "amount" or "monto" => SortDirection == "asc" 
                    ? userTransactions.OrderBy(t => t.NetAmount) 
                    : userTransactions.OrderByDescending(t => t.NetAmount),
                "status" or "estado" => SortDirection == "asc" 
                    ? userTransactions.OrderBy(t => t.TransactionStatus) 
                    : userTransactions.OrderByDescending(t => t.TransactionStatus),
                _ => userTransactions.OrderByDescending(t => t.ID) // Por defecto ID descendente
            };

            UserTransactions = userTransactions.ToList();
        }

        private void LoadReferenceData()
        {
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

        private void SetSortProperties()
        {
            CurrentSort = SortBy;

            // Configurar las propiedades de ordenamiento para cada columna
            IdSort = SortBy == "ID" ? (SortDirection == "asc" ? "desc" : "asc") : "asc";
            DateSort = SortBy == "Date" ? (SortDirection == "asc" ? "desc" : "asc") : "asc";
            MerchantSort = SortBy == "Merchant" ? (SortDirection == "asc" ? "desc" : "asc") : "asc";
            AmountSort = SortBy == "Amount" ? (SortDirection == "asc" ? "desc" : "asc") : "asc";
            StatusSort = SortBy == "Status" ? (SortDirection == "asc" ? "desc" : "asc") : "asc";
        }

        // Método auxiliar para obtener el ícono de ordenamiento
        public string GetSortIcon(string columnName)
        {
            if (SortBy.Equals(columnName, StringComparison.OrdinalIgnoreCase))
            {
                return SortDirection == "asc" ? "bi-arrow-up" : "bi-arrow-down";
            }
            return "bi-arrow-up-down";
        }

        // Método auxiliar para obtener la clase CSS del encabezado
        public string GetSortHeaderClass(string columnName)
        {
            if (SortBy.Equals(columnName, StringComparison.OrdinalIgnoreCase))
            {
                return "table-active";
            }
            return "";
        }
    }
}
