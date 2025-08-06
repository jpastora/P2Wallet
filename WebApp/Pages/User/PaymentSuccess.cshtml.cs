using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using DTOs;
using CoreApp;

namespace WebApp.Pages.User
{
    [Authorize]
    public class PaymentSuccessModel : PageModel
    {
        public Transaction? Transaction { get; set; }
        public string Message { get; set; } = string.Empty;
        public string MerchantName { get; set; } = string.Empty;
        public string AccountInfo { get; set; } = string.Empty;
        public double DiscountApplied { get; set; } = 0;
        public PromotionInfo? PromotionApplied { get; set; }
        public DTOs.User? CurrentUser { get; set; }

        public async Task<IActionResult> OnGetAsync([FromQuery] int transactionId)
        {
            if (transactionId <= 0)
            {
                return RedirectToPage("/User/UserPanel");
            }

            // Verificar que el usuario esté autenticado
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToPage("/Login");
            }

            var userManager = new UserManager();
            CurrentUser = userManager.RetrieveUserByEmail(new DTOs.User { Email = email });
            
            if (CurrentUser == null)
            {
                return RedirectToPage("/Login");
            }

            try
            {
                // Obtener la transacción
                var transactionManager = new TransactionManager();
                Transaction = transactionManager.RetrieveTransactionById(transactionId);

                if (Transaction == null)
                {
                    Message = "Transacción no encontrada.";
                    return RedirectToPage("/User/UserPanel");
                }

                // Verificar que la transacción pertenezca al usuario actual
                if (Transaction.UserID != CurrentUser.ID)
                {
                    Message = "No tienes autorización para ver esta transacción.";
                    return RedirectToPage("/User/UserPanel");
                }

                // Verificar que la transacción esté completada
                if (Transaction.TransactionStatus != "Completed")
                {
                    Message = "Esta transacción aún no ha sido completada.";
                    return RedirectToPage("/User/ScanPayment");
                }

                // Cargar información del comercio
                await LoadMerchantInfo();

                // Cargar información de la cuenta bancaria utilizada
                await LoadAccountInfo();

                // Calcular descuento aplicado
                CalculateDiscount();

                // Cargar información de promoción aplicada
                await LoadPromotionInfo();

                Message = "Pago procesado exitosamente.";
            }
            catch (Exception ex)
            {
                Message = $"Error al cargar información del pago: {ex.Message}";
                return RedirectToPage("/User/UserPanel");
            }

            return Page();
        }

        private async Task LoadMerchantInfo()
        {
            if (Transaction?.MerchantID > 0)
            {
                try
                {
                    var merchantManager = new MerchantManager();
                    var merchant = merchantManager.RetrieveMerchantById(Transaction.MerchantID);
                    MerchantName = merchant?.MerchantName ?? "Comercio Desconocido";
                }
                catch
                {
                    MerchantName = "Comercio Desconocido";
                }
            }
        }

        private async Task LoadAccountInfo()
        {
            if (Transaction?.BankAccountID > 0)
            {
                try
                {
                    var accountManager = new BankAccountManager();
                    var account = accountManager.RetrieveBankAccountById(Transaction.BankAccountID);
                    
                    if (account != null)
                    {
                        var entityManager = new FinancialEntityManager();
                        var entity = entityManager.RetrieveFinancialEntityById(account.FinancialEntityID);
                        
                        var bankName = entity?.EntityName ?? "Banco Desconocido";
                        AccountInfo = $"{account.IBAN} - {bankName}";
                    }
                    else
                    {
                        AccountInfo = "Cuenta Desconocida";
                    }
                }
                catch
                {
                    AccountInfo = "Cuenta Desconocida";
                }
            }
        }

        private void CalculateDiscount()
        {
            if (Transaction != null)
            {
                // El descuento es la diferencia entre el monto bruto y neto
                // (sin contar impuestos ya que esos no son descuentos)
                var grossAmount = Transaction.GrossAmount;
                var netAmount = Transaction.NetAmount;
                var taxAmount = Transaction.SalesTaxAmount;

                // Si hay diferencia entre bruto y neto (más allá de los impuestos), es descuento
                var expectedNetWithoutDiscount = grossAmount - taxAmount;
                DiscountApplied = Math.Max(0, expectedNetWithoutDiscount - netAmount);
            }
        }

        private async Task LoadPromotionInfo()
        {
            try
            {
                // Verificar si se aplicó una promoción de comercio
                if (Transaction?.MerchantPromotionID.HasValue == true)
                {
                    var merchantPromotionManager = new MerchantPromotionManager();
                    var promotion = merchantPromotionManager.RetrievePromotionById(Transaction.MerchantPromotionID.Value);
                    
                    if (promotion != null)
                    {
                        PromotionApplied = new PromotionInfo
                        {
                            Id = promotion.ID.ToString(),
                            Type = "Merchant",
                            Name = promotion.MerchantPromotionName,
                            Description = promotion.MerchantPromotionDescription,
                            DiscountPercentage = promotion.DiscountPercentage
                        };
                        return;
                    }
                }

                // Verificar si se aplicó una promoción financiera
                if (Transaction?.FinancialPromotionID.HasValue == true)
                {
                    var financialPromotionManager = new FinancialPromotionManager();
                    var promotion = financialPromotionManager.RetrievePromotionById(Transaction.FinancialPromotionID.Value);
                    
                    if (promotion != null)
                    {
                        PromotionApplied = new PromotionInfo
                        {
                            Id = promotion.ID.ToString(),
                            Type = "Financial",
                            Name = promotion.FinancialPromotionName,
                            Description = promotion.FinancialPromotionDescription,
                            DiscountPercentage = promotion.DiscountPercentage
                        };
                    }
                }
            }
            catch
            {
                // En caso de error, no mostrar promoción
                PromotionApplied = null;
            }
        }

        // Clase auxiliar para información de promociones
        public class PromotionInfo
        {
            public string Id { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public double DiscountPercentage { get; set; }
            
            public string TypeBadgeClass => Type == "Merchant" ? "bg-success" : "bg-primary";
            public string TypeText => Type == "Merchant" ? "Promoción del Comercio" : "Promoción del Banco";
        }
    }
}