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
                TempData["ErrorMessage"] = "? ID de transacción inválido.";
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

            // Manejar mensajes de TempData
            if (TempData.ContainsKey("SuccessMessage"))
            {
                Message = TempData["SuccessMessage"]?.ToString() ?? "";
            }
            else if (TempData.ContainsKey("ErrorMessage"))
            {
                Message = TempData["ErrorMessage"]?.ToString() ?? "";
            }

            try
            {
                // Obtener la transacción
                var transactionManager = new TransactionManager();
                Transaction = transactionManager.RetrieveTransactionById(transactionId);

                if (Transaction == null)
                {
                    TempData["ErrorMessage"] = "? Transacción no encontrada.";
                    return RedirectToPage("/User/UserPanel");
                }

                // Verificar que la transacción pertenezca al usuario actual
                if (Transaction.UserID != CurrentUser.ID)
                {
                    TempData["ErrorMessage"] = "? No tienes autorización para ver esta transacción.";
                    return RedirectToPage("/User/UserPanel");
                }

                // Verificar que la transacción esté completada
                if (Transaction.TransactionStatus != "Completed")
                {
                    TempData["ErrorMessage"] = "? Esta transacción aún no ha sido completada.";
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

                // Si no hay mensaje de éxito de TempData, usar mensaje por defecto
                if (string.IsNullOrEmpty(Message))
                {
                    Message = "? Pago procesado exitosamente.";
                }
            }
            catch (Exception ex)
            {
                Message = $"? Error al cargar información del pago: {ex.Message}";
                TempData["ErrorMessage"] = Message;
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
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading merchant info: {ex.Message}");
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
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading account info: {ex.Message}");
                    AccountInfo = "Cuenta Desconocida";
                }
            }
        }

        private void CalculateDiscount()
        {
            if (Transaction != null)
            {
                try
                {
                    // CORREGIDO: Lógica comercial correcta
                    // En la nueva lógica:
                    // - NetAmount contiene el monto de venta FINAL (después del descuento)
                    // - GrossAmount contiene el total FINAL que pagó el cliente
                    // - SalesTaxAmount contiene el impuesto FINAL (calculado sobre NetAmount final)
                    
                    // Si hay promoción aplicada, calcular el descuento
                    double discountPercentage = 0;
                    
                    if (Transaction.MerchantPromotionID.HasValue)
                    {
                        var merchantPromotionManager = new MerchantPromotionManager();
                        var promotion = merchantPromotionManager.RetrievePromotionById(Transaction.MerchantPromotionID.Value);
                        if (promotion != null)
                        {
                            discountPercentage = promotion.DiscountPercentage;
                        }
                    }
                    else if (Transaction.FinancialPromotionID.HasValue)
                    {
                        var financialPromotionManager = new FinancialPromotionManager();
                        var promotion = financialPromotionManager.RetrievePromotionById(Transaction.FinancialPromotionID.Value);
                        if (promotion != null)
                        {
                            discountPercentage = promotion.DiscountPercentage;
                        }
                    }
                    
                    if (discountPercentage > 0)
                    {
                        // Calcular el monto de venta original antes del descuento
                        // FinalNetAmount = OriginalNetAmount * (1 - discountPercentage/100)
                        // Por lo tanto: OriginalNetAmount = FinalNetAmount / (1 - discountPercentage/100)
                        var discountFactor = 1 - (discountPercentage / 100.0);
                        var originalNetAmount = Transaction.NetAmount / discountFactor;
                        
                        // El descuento aplicado es la diferencia
                        DiscountApplied = originalNetAmount - Transaction.NetAmount;
                    }
                    else
                    {
                        // No hay descuento aplicado
                        DiscountApplied = 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error calculating discount: {ex.Message}");
                    DiscountApplied = 0;
                }
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
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading promotion info: {ex.Message}");
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