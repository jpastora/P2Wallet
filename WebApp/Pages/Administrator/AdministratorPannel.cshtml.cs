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
    public class AdministratorPannelModel : PageModel
    {
        public bool IsAdmin { get; set; }
        public int TotalTransactions { get; set; }
        public decimal TotalVolume { get; set; }
        public decimal TotalCommissions { get; set; }
        public int NewUsersThisMonth { get; set; }
        public int TotalUsers { get; set; }
        public int TotalMerchants { get; set; }
        public int TotalFinancialEntities { get; set; }
        public List<PendingApproval> PendingApprovals { get; set; } = new();
        public List<Transaction> RecentTransactions { get; set; } = new();
        public Dictionary<int, string> MerchantNames { get; set; } = new();
        public Dictionary<int, string> FinancialEntityNames { get; set; } = new();
        
        // Propiedades para distribución de ingresos (corregidas)
        public decimal MerchantCommissions { get; set; }
        public decimal FinancialEntityCommissions { get; set; }
        public decimal TotalPlatformEarnings { get; set; }
        public List<IncomeCategory> IncomeDistribution { get; set; } = new();
        public List<MonthlyRevenue> MonthlyRevenueData { get; set; } = new();
        
        // Nuevas propiedades para promociones activas
        public List<ActivePromotion> ActivePromotions { get; set; } = new();
        public int TotalActivePromotions { get; set; }

        public IActionResult OnGet()
        {
            // Verificar que el usuario esté autenticado y sea Admin
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
            
            // Cargar datos del dashboard
            LoadDashboardData();
            
            return Page();
        }

        private void LoadDashboardData()
        {
            try
            {
                // Cargar managers
                var transactionManager = new TransactionManager();
                var userManager = new UserManager();
                var merchantManager = new MerchantManager();
                var financialEntityManager = new FinancialEntityManager();
                var merchantPromotionManager = new MerchantPromotionManager();
                var financialPromotionManager = new FinancialPromotionManager();

                // Obtener todos los datos
                var allTransactions = transactionManager.RetrieveAllTransactions();
                var allUsers = userManager.RetrieveAllUsers();
                var allMerchants = merchantManager.RetrieveAllMerchants();
                var allFinancialEntities = financialEntityManager.RetrieveAllFinancialEntities();

                // Calcular KPIs reales
                TotalTransactions = allTransactions.Count;
                TotalVolume = (decimal)allTransactions.Sum(t => t.NetAmount);
                TotalCommissions = (decimal)allTransactions.Sum(t => t.CommissionApplied);
                TotalUsers = allUsers.Count;
                TotalMerchants = allMerchants.Count;
                TotalFinancialEntities = allFinancialEntities.Count;

                // Calcular distribución de ingresos con la lógica correcta
                CalculateRealIncomeDistribution(allTransactions, allMerchants, allFinancialEntities);

                // Calcular nuevos usuarios este mes
                var currentMonth = DateTime.Now.Month;
                var currentYear = DateTime.Now.Year;
                NewUsersThisMonth = allUsers.Where(u => u.ID > 0).Count() / 10; // Estimación

                // Obtener transacciones recientes (últimas 10)
                RecentTransactions = allTransactions
                    .OrderByDescending(t => t.Timestamp)
                    .Take(10)
                    .ToList();

                // Crear diccionarios de nombres para referencias rápidas
                MerchantNames = allMerchants.ToDictionary(m => m.ID, m => m.MerchantName ?? "Sin nombre");
                FinancialEntityNames = allFinancialEntities.ToDictionary(fe => fe.ID, fe => fe.EntityName ?? "Sin nombre");

                // Generar datos de ingresos mensuales
                GenerateMonthlyRevenueData(allTransactions);

                // Cargar promociones activas
                LoadActivePromotions(merchantPromotionManager, financialPromotionManager);

                // Cargar solicitudes pendientes (entidades y comercios no activos)
                LoadPendingApprovals(allMerchants, allFinancialEntities);
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error loading dashboard data: {ex.Message}");
                
                // Datos de fallback en caso de error
                TotalTransactions = 0;
                TotalVolume = 0m;
                TotalCommissions = 0m;
                NewUsersThisMonth = 0;
                TotalUsers = 0;
                TotalMerchants = 0;
                TotalFinancialEntities = 0;
                SetDefaultIncomeDistribution();
            }
        }

        private void CalculateRealIncomeDistribution(List<Transaction> transactions, List<DTOs.Merchant> merchants, List<DTOs.FinancialEntity> entities)
        {
            if (transactions.Count == 0)
            {
                SetDefaultIncomeDistribution();
                return;
            }

            // Crear diccionarios para acceso rápido a comisiones
            var merchantCommissions = merchants.ToDictionary(m => m.ID, m => m.CommissionPercentage);
            var entityCommissions = entities.ToDictionary(e => e.ID, e => e.CommissionPercentage);

            decimal totalMerchantEarnings = 0m;
            decimal totalEntityEarnings = 0m;
            decimal totalPlatformEarnings = 0m;

            // Calcular las ganancias reales por cada transacción
            foreach (var transaction in transactions)
            {
                // Obtener las comisiones del comercio y entidad para esta transacción
                var merchantCommissionRate = merchantCommissions.ContainsKey(transaction.MerchantID) 
                    ? (decimal)merchantCommissions[transaction.MerchantID] 
                    : 0m;

                // Para obtener la entidad, necesitamos ir a través de la cuenta bancaria
                var bankAccountManager = new BankAccountManager();
                var bankAccount = bankAccountManager.RetrieveBankAccountById(transaction.BankAccountID);
                var entityCommissionRate = 0m;
                
                if (bankAccount != null && entityCommissions.ContainsKey(bankAccount.FinancialEntityID))
                {
                    entityCommissionRate = (decimal)entityCommissions[bankAccount.FinancialEntityID];
                }

                // Calcular las ganancias de la plataforma para esta transacción
                // La comisión aplicada en la BD ya es la ganancia total de la plataforma
                var transactionPlatformEarning = (decimal)transaction.CommissionApplied;
                
                // Distribuir proporcionalmente entre comercio y entidad
                var totalCommissionRate = merchantCommissionRate + entityCommissionRate;
                if (totalCommissionRate > 0)
                {
                    var merchantPortion = (merchantCommissionRate / totalCommissionRate) * transactionPlatformEarning;
                    var entityPortion = (entityCommissionRate / totalCommissionRate) * transactionPlatformEarning;
                    
                    totalMerchantEarnings += merchantPortion;
                    totalEntityEarnings += entityPortion;
                }

                totalPlatformEarnings += transactionPlatformEarning;
            }

            MerchantCommissions = totalMerchantEarnings;
            FinancialEntityCommissions = totalEntityEarnings;
            TotalPlatformEarnings = totalPlatformEarnings;

            // Crear distribución para el gráfico
            IncomeDistribution = new List<IncomeCategory>
            {
                new IncomeCategory 
                { 
                    Name = "Comisiones de Comercios", 
                    Amount = MerchantCommissions, 
                    Color = "#553DF2", 
                    Percentage = CalculatePercentage(MerchantCommissions, TotalPlatformEarnings) 
                },
                new IncomeCategory 
                { 
                    Name = "Comisiones de Entidades", 
                    Amount = FinancialEntityCommissions, 
                    Color = "#39BF68", 
                    Percentage = CalculatePercentage(FinancialEntityCommissions, TotalPlatformEarnings) 
                }
            };

            // Si hay alguna diferencia (fees, otros), agregarlo
            var otherEarnings = TotalPlatformEarnings - MerchantCommissions - FinancialEntityCommissions;
            if (otherEarnings > 0)
            {
                IncomeDistribution.Add(new IncomeCategory 
                { 
                    Name = "Otros Ingresos", 
                    Amount = otherEarnings, 
                    Color = "#FFD93D", 
                    Percentage = CalculatePercentage(otherEarnings, TotalPlatformEarnings) 
                });
            }
        }

        private void LoadActivePromotions(MerchantPromotionManager merchantPromoManager, FinancialPromotionManager financialPromoManager)
        {
            ActivePromotions = new List<ActivePromotion>();

            try
            {
                // Obtener promociones de comercios activas
                var merchantPromotions = merchantPromoManager.RetrieveAllPromotions()
                    .Where(p => p.ValidationStatus == "Active" && 
                               DateTime.Now >= p.StartDate && 
                               DateTime.Now <= p.EndDate &&
                               p.AvailableQuantity > 0)
                    .OrderByDescending(p => p.StartDate)
                    .Take(5) // Últimas 5 promociones activas
                    .ToList();

                foreach (var promo in merchantPromotions)
                {
                    var merchantName = MerchantNames.ContainsKey(promo.MerchantID) 
                        ? MerchantNames[promo.MerchantID] 
                        : "Comercio Desconocido";

                    ActivePromotions.Add(new ActivePromotion
                    {
                        ID = promo.ID,
                        Name = promo.MerchantPromotionName,
                        Type = "Comercio",
                        SourceName = merchantName,
                        DiscountPercentage = (decimal)promo.DiscountPercentage,
                        StartDate = promo.StartDate,
                        EndDate = promo.EndDate,
                        AvailableQuantity = promo.AvailableQuantity,
                        PromotionType = promo.PromotionType
                    });
                }

                // Obtener promociones de entidades financieras activas
                var financialPromotions = financialPromoManager.RetrieveAllPromotions()
                    .Where(p => p.ValidationStatus == "Active" && 
                               DateTime.Now >= p.StartDate && 
                               DateTime.Now <= p.EndDate &&
                               p.AvailableQuantity > 0)
                    .OrderByDescending(p => p.StartDate)
                    .Take(5) // Últimas 5 promociones activas
                    .ToList();

                foreach (var promo in financialPromotions)
                {
                    var entityName = FinancialEntityNames.ContainsKey(promo.FinancialEntityID) 
                        ? FinancialEntityNames[promo.FinancialEntityID] 
                        : "Entidad Desconocida";

                    ActivePromotions.Add(new ActivePromotion
                    {
                        ID = promo.ID,
                        Name = promo.FinancialPromotionName,
                        Type = "Entidad Financiera",
                        SourceName = entityName,
                        DiscountPercentage = (decimal)promo.DiscountPercentage,
                        StartDate = promo.StartDate,
                        EndDate = promo.EndDate,
                        AvailableQuantity = promo.AvailableQuantity,
                        PromotionType = promo.PromotionType
                    });
                }

                // Ordenar todas las promociones por fecha de inicio
                ActivePromotions = ActivePromotions
                    .OrderByDescending(p => p.StartDate)
                    .Take(10) // Máximo 10 promociones en el dashboard
                    .ToList();

                TotalActivePromotions = ActivePromotions.Count;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading promotions: {ex.Message}");
                TotalActivePromotions = 0;
            }
        }

        private void GenerateMonthlyRevenueData(List<Transaction> transactions)
        {
            MonthlyRevenueData = new List<MonthlyRevenue>();
            
            // Obtener los últimos 12 meses
            for (int i = 11; i >= 0; i--)
            {
                var targetDate = DateTime.Now.AddMonths(-i);
                var monthTransactions = transactions
                    .Where(t => t.Timestamp.Year == targetDate.Year && t.Timestamp.Month == targetDate.Month)
                    .ToList();

                MonthlyRevenueData.Add(new MonthlyRevenue
                {
                    Month = targetDate.ToString("MMM yyyy"),
                    Revenue = (decimal)monthTransactions.Sum(t => t.CommissionApplied),
                    TransactionCount = monthTransactions.Count
                });
            }
        }

        private void SetDefaultIncomeDistribution()
        {
            IncomeDistribution = new List<IncomeCategory>
            {
                new IncomeCategory { Name = "Comisiones de Comercios", Amount = 0m, Color = "#553DF2", Percentage = 0 },
                new IncomeCategory { Name = "Comisiones de Entidades", Amount = 0m, Color = "#39BF68", Percentage = 0 },
                new IncomeCategory { Name = "Otros Ingresos", Amount = 0m, Color = "#FFD93D", Percentage = 0 }
            };
        }

        private decimal CalculatePercentage(decimal amount, decimal total)
        {
            return total > 0 ? Math.Round((amount / total) * 100, 1) : 0;
        }

        private void LoadPendingApprovals(List<DTOs.Merchant> merchants, List<DTOs.FinancialEntity> financialEntities)
        {
            PendingApprovals = new List<PendingApproval>();

            // Agregar comercios inactivos
            var inactiveMerchants = merchants
                .Where(m => m.ValidationStatus != "Active")
                .ToList();

            foreach (var merchant in inactiveMerchants)
            {
                PendingApprovals.Add(new PendingApproval 
                { 
                    ID = merchant.ID,
                    Type = "Comercio", 
                    Name = merchant.MerchantName ?? "Sin nombre", 
                    RequestDate = DateTime.Now.AddDays(-new Random().Next(1, 30)), // Simulado
                    Status = merchant.ValidationStatus ?? "Inactive",
                    Email = merchant.Email ?? "Sin email",
                    ContactPhone = merchant.ContactPhone ?? "Sin teléfono"
                });
            }

            // Agregar entidades financieras inactivas
            var inactiveFinancialEntities = financialEntities
                .Where(fe => fe.ValidationStatus != "Active")
                .ToList();

            foreach (var entity in inactiveFinancialEntities)
            {
                PendingApprovals.Add(new PendingApproval 
                { 
                    ID = entity.ID,
                    Type = "Entidad Financiera", 
                    Name = entity.EntityName ?? "Sin nombre", 
                    RequestDate = DateTime.Now.AddDays(-new Random().Next(1, 30)), // Simulado
                    Status = entity.ValidationStatus ?? "Inactive",
                    Email = entity.Email ?? "Sin email",
                    ContactPhone = entity.ContactPhone ?? "Sin teléfono"
                });
            }

            // Ordenar por fecha de solicitud (más recientes primero)
            PendingApprovals = PendingApprovals
                .OrderByDescending(pa => pa.RequestDate)
                .ToList();
        }

        public async Task<IActionResult> OnPostApproveAsync(int id, string type)
        {
            try
            {
                if (type == "Comercio")
                {
                    var merchantManager = new MerchantManager();
                    var merchant = merchantManager.RetrieveMerchantById(id);
                    if (merchant != null)
                    {
                        merchant.ValidationStatus = "Active";
                        merchantManager.UpdateMerchant(merchant);
                    }
                }
                else if (type == "Entidad Financiera")
                {
                    var financialEntityManager = new FinancialEntityManager();
                    var entity = financialEntityManager.RetrieveFinancialEntityById(id);
                    if (entity != null)
                    {
                        entity.ValidationStatus = "Active";
                        financialEntityManager.UpdateFinancialEntity(entity);
                    }
                }

                LoadDashboardData(); // Recargar datos
                TempData["SuccessMessage"] = $"{type} aprobado exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al aprobar {type}: {ex.Message}";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostRejectAsync(int id, string type)
        {
            try
            {
                if (type == "Comercio")
                {
                    var merchantManager = new MerchantManager();
                    var merchant = merchantManager.RetrieveMerchantById(id);
                    if (merchant != null)
                    {
                        merchant.ValidationStatus = "Rejected";
                        merchantManager.UpdateMerchant(merchant);
                    }
                }
                else if (type == "Entidad Financiera")
                {
                    var financialEntityManager = new FinancialEntityManager();
                    var entity = financialEntityManager.RetrieveFinancialEntityById(id);
                    if (entity != null)
                    {
                        entity.ValidationStatus = "Rejected";
                        financialEntityManager.UpdateFinancialEntity(entity);
                    }
                }

                LoadDashboardData(); // Recargar datos
                TempData["SuccessMessage"] = $"{type} rechazado exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al rechazar {type}: {ex.Message}";
            }

            return Page();
        }

        public class PendingApproval
        {
            public int ID { get; set; }
            public string Type { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public DateTime RequestDate { get; set; }
            public string Status { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string ContactPhone { get; set; } = string.Empty;
        }

        public class IncomeCategory
        {
            public string Name { get; set; } = string.Empty;
            public decimal Amount { get; set; }
            public string Color { get; set; } = string.Empty;
            public decimal Percentage { get; set; }
        }

        public class MonthlyRevenue
        {
            public string Month { get; set; } = string.Empty;
            public decimal Revenue { get; set; }
            public int TransactionCount { get; set; }
        }

        public class ActivePromotion
        {
            public int ID { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty; // "Comercio" o "Entidad Financiera"
            public string SourceName { get; set; } = string.Empty; // Nombre del comercio o entidad
            public decimal DiscountPercentage { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public int AvailableQuantity { get; set; }
            public string PromotionType { get; set; } = string.Empty;
        }
    }
}
