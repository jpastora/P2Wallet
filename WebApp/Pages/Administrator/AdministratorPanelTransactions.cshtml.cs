using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using CoreApp;
using DTOs;
using System.Collections.Generic;
using System.Linq;

namespace WebApp.Pages.Administrator
{
    [Authorize]
    public class AdministratorPanelTransactionsModel : PageModel
    {
        public List<DTOs.Transaction> AllTransactions { get; set; } = new();
        public Dictionary<int, string> UserNames { get; set; } = new();
        public Dictionary<int, string> MerchantNames { get; set; } = new();
        
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; } = string.Empty;
        
        [BindProperty(SupportsGet = true)]
        public string StatusFilter { get; set; } = string.Empty;
        
        [BindProperty(SupportsGet = true)]
        public DateTime? DateFilter { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        
        public int PageSize { get; set; } = 15;
        public int TotalPages { get; set; }
        public int TotalTransactionsFound { get; set; }
        public bool IsAdmin { get; set; }

        // Estadísticas
        public int TotalTransactions { get; set; }
        public decimal TotalVolume { get; set; }
        public decimal TotalCommissions { get; set; }
        public decimal AverageTransaction { get; set; }

        // Propiedades para paginación
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
        public int PreviousPage => CurrentPage - 1;
        public int NextPage => CurrentPage + 1;

        // Traducción de estados
        public static string EstadoEspanol(string status) => status switch
        {
            "Completed" => "Completada",
            "Pending" => "Pendiente",
            "Failed" => "Fallida",
            "Cancelled" => "Cancelada",
            _ => status ?? "Desconocido"
        };

        public IActionResult OnGet()
        {
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
            LoadTransactions();
            return Page();
        }

        private void LoadTransactions()
        {
            try
            {
                var transactionManager = new TransactionManager();
                var userManager = new UserManager();
                var merchantManager = new MerchantManager();

                // Cargar datos de referencia
                var allUsers = userManager.RetrieveAllUsers();
                var allMerchants = merchantManager.RetrieveAllMerchants();
                
                UserNames = allUsers.ToDictionary(u => u.ID, u => $"{u.FirstName} {u.LastName}");
                MerchantNames = allMerchants.ToDictionary(m => m.ID, m => m.MerchantName ?? "Sin nombre");

                // Obtener todas las transacciones
                var allTransactions = transactionManager.RetrieveAllTransactions();
                
                // Calcular estadísticas generales
                TotalTransactions = allTransactions.Count;
                TotalVolume = (decimal)allTransactions.Sum(t => t.NetAmount);
                TotalCommissions = (decimal)allTransactions.Sum(t => t.CommissionApplied);
                AverageTransaction = TotalTransactions > 0 ? TotalVolume / TotalTransactions : 0;

                // Aplicar filtros
                var filteredTransactions = allTransactions.AsQueryable();

                // Filtro de búsqueda
                if (!string.IsNullOrEmpty(SearchTerm))
                {
                    if (int.TryParse(SearchTerm, out int searchId))
                    {
                        filteredTransactions = filteredTransactions.Where(t => t.ID == searchId);
                    }
                    else
                    {
                        filteredTransactions = filteredTransactions.Where(t => 
                            MerchantNames.ContainsKey(t.MerchantID) && 
                            MerchantNames[t.MerchantID].Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));
                    }
                }

                // Filtro por estado
                if (!string.IsNullOrEmpty(StatusFilter))
                {
                    filteredTransactions = filteredTransactions.Where(t => t.TransactionStatus == StatusFilter);
                }

                // Filtro por fecha
                if (DateFilter.HasValue)
                {
                    var filterDate = DateFilter.Value.Date;
                    filteredTransactions = filteredTransactions.Where(t => t.Timestamp.Date == filterDate);
                }

                // Ordenar por fecha (más recientes primero)
                var orderedTransactions = filteredTransactions.OrderByDescending(t => t.Timestamp).ToList();

                TotalTransactionsFound = orderedTransactions.Count;
                TotalPages = (int)Math.Ceiling(TotalTransactionsFound / (double)PageSize);
                CurrentPage = Math.Max(1, Math.Min(CurrentPage, TotalPages));

                // Paginación
                AllTransactions = orderedTransactions
                    .Skip((CurrentPage - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar transacciones: {ex.Message}";
                AllTransactions = new List<DTOs.Transaction>();
            }
        }

        public IActionResult OnPostChangeStatus(int transactionId, string newStatus)
        {
            try
            {
                var transactionManager = new TransactionManager();
                var transaction = transactionManager.RetrieveTransactionById(transactionId);
                
                if (transaction != null)
                {
                    var oldStatus = transaction.TransactionStatus;
                    transaction.TransactionStatus = newStatus;
                    transactionManager.UpdateTransaction(transaction);
                    
                    var statusText = EstadoEspanol(newStatus);
                    TempData["SuccessMessage"] = $"Transacción #{transactionId} cambiada de {EstadoEspanol(oldStatus)} a {statusText} exitosamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Transacción no encontrada.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cambiar estado de la transacción: {ex.Message}";
            }

            return RedirectToPage();
        }

        // Métodos auxiliares para obtener nombres
        public string GetUserName(int userId)
        {
            return UserNames.ContainsKey(userId) ? UserNames[userId] : $"Usuario #{userId}";
        }

        public string GetMerchantName(int merchantId)
        {
            return MerchantNames.ContainsKey(merchantId) ? MerchantNames[merchantId] : $"Comercio #{merchantId}";
        }
    }
}