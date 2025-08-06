using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using DTOs;
using CoreApp;

namespace WebApp.Pages.User
{
    [Authorize]
    public class UserPanelModel : PageModel
    {
        public DTOs.User CurrentUser { get; set; }
        public List<BankAccount> UserAccounts { get; set; } = new();
        public List<Transaction> LastTransactions { get; set; } = new();
        public Dictionary<int, string> MerchantNames { get; set; } = new();
        public Dictionary<int, string> AccountIBANs { get; set; } = new();
        public Dictionary<int, string> BankNames { get; set; } = new();
        public Dictionary<int, DTOs.FinancialEntity> FinancialEntities { get; set; } = new();
        
        // Propiedades para funcionalidades de pago/cobro
        public bool HasMerchantAccess { get; set; } = false;
        public List<DTOs.Merchant> UserMerchants { get; set; } = new();
        public DTOs.Merchant? PrimaryMerchant { get; set; }
        
        // Propiedad para el saldo total
        public decimal TotalBalance { get; set; } = 0;

        public void OnGet()
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email)) return;

            var userManager = new UserManager();
            CurrentUser = userManager.RetrieveUserByEmail(new DTOs.User { Email = email });
            if (CurrentUser == null) return;

            LoadUserAccounts();
            LoadRecentTransactions();
            LoadMerchants();
            LoadFinancialEntities();
            LoadUserMerchants();
            CalculateTotalBalance();
        }

        private void LoadUserAccounts()
        {
            var accountManager = new BankAccountManager();
            var allAccounts = accountManager.RetrieveAllBankAccounts();
            UserAccounts = allAccounts.FindAll(a => a.UserID == CurrentUser.ID);
            AccountIBANs = allAccounts.ToDictionary(a => a.ID, a => a.IBAN);
        }

        private void LoadRecentTransactions()
        {
            var transactionManager = new TransactionManager();
            var allTransactions = transactionManager.RetrieveAllTransactions();
            LastTransactions = allTransactions
                .Where(t => t.UserID == CurrentUser.ID)
                .OrderByDescending(t => t.ID)
                .Take(5)
                .ToList();
        }

        private void LoadMerchants()
        {
            var merchantManager = new MerchantManager();
            var allMerchants = merchantManager.RetrieveAllMerchants();
            MerchantNames = allMerchants.ToDictionary(m => m.ID, m => m.MerchantName);
        }

        private void LoadFinancialEntities()
        {
            var entityManager = new FinancialEntityManager();
            var allEntities = entityManager.RetrieveAllFinancialEntities();
            BankNames = allEntities.ToDictionary(e => e.ID, e => e.EntityName);
            FinancialEntities = allEntities.ToDictionary(e => e.ID, e => e);
        }

        private void LoadUserMerchants()
        {
            if (CurrentUser.Role == "Admin")
            {
                HasMerchantAccess = true;
                var merchantManager = new MerchantManager();
                UserMerchants = merchantManager.RetrieveAllMerchants()
                    .Where(m => m.ValidationStatus == "Active")
                    .Take(5)
                    .ToList();
            }
            else
            {
                var userMerchantManager = new UserMerchantManager();
                var userMerchantRelations = userMerchantManager.RetrieveAllUserMerchants()
                    .Where(um => um.UserID == CurrentUser.ID)
                    .ToList();

                if (userMerchantRelations.Any())
                {
                    HasMerchantAccess = true;
                    var merchantManager = new MerchantManager();
                    
                    foreach (var relation in userMerchantRelations)
                    {
                        var merchant = merchantManager.RetrieveMerchantById(relation.MerchantID);
                        if (merchant != null && merchant.ValidationStatus == "Active")
                        {
                            UserMerchants.Add(merchant);
                        }
                    }
                }
            }

            PrimaryMerchant = UserMerchants.FirstOrDefault();
        }
        
        private void CalculateTotalBalance()
        {
            TotalBalance = (decimal)UserAccounts
                .Where(account => account.ValidationStatus == "Active")
                .Sum(account => account.Balance);
        }

        // Funciones auxiliares para el estado de las transacciones
        public static string GetStatusBadgeClass(string status)
        {
            return status switch
            {
                "Completed" => "success",
                "PendingUserApproval" => "warning",
                "Pending" => "warning",
                "Processing" => "info",
                "Failed" => "danger",
                "Cancelled" => "secondary",
                "Rejected" => "danger",
                _ => "secondary"
            };
        }

        public static string GetStatusText(string status)
        {
            return status switch
            {
                "Completed" => "Completado",
                "PendingUserApproval" => "Pendiente",
                "Pending" => "Pendiente",
                "Processing" => "Procesando",
                "Failed" => "Fallido",
                "Cancelled" => "Cancelado",
                "Rejected" => "Rechazado",
                _ => status ?? "Desconocido"
            };
        }
    }
}
