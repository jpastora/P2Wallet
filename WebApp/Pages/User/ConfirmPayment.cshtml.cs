using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using CoreApp;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Linq;
using MerchantDto = DTOs.Merchant;
using FinancialEntityDto = DTOs.FinancialEntity;
using BankAccountDto = DTOs.BankAccount;
using TransactionDto = DTOs.Transaction;
using UserDto = DTOs.User;

namespace WebApp.Pages.User
{
    [Authorize]
    public class ConfirmPaymentModel : PageModel
    {
        [BindProperty(SupportsGet = true)] public string PaymentCode { get; set; } = string.Empty;
        [BindProperty(SupportsGet = true)] public int BankAccountId { get; set; }
        [BindProperty(SupportsGet = true)] public int? PromotionId { get; set; }
        [BindProperty(SupportsGet = true)] public string? PromotionType { get; set; }

        public TransactionDto? PaymentRequest { get; set; }
        public MerchantDto? MerchantInfo { get; set; }
        public BankAccountDto? BankAccount { get; set; }
        public FinancialEntityDto? FinancialEntity { get; set; }

        public double GrossAmount => PaymentRequest?.GrossAmount ?? 0;
        public double SalesTaxAmount => PaymentRequest?.SalesTaxAmount ?? 0;
        public double OriginalNetAmount => PaymentRequest != null ? PaymentRequest.GrossAmount - PaymentRequest.SalesTaxAmount : 0;
        public double DiscountPercentage { get; set; }
        public double DiscountAmount { get; set; }
        public double NetAfterDiscount { get; set; }
        public double FinalPayable => NetAfterDiscount + SalesTaxAmount; // Impuesto no se descuenta

        public string Message { get; set; } = string.Empty;

        private readonly IHttpClientFactory _httpClientFactory;
        public ConfirmPaymentModel(IHttpClientFactory httpClientFactory) { _httpClientFactory = httpClientFactory; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrWhiteSpace(PaymentCode) || BankAccountId <= 0)
            {
                return RedirectToPage("/User/ScanPayment", new { code = PaymentCode });
            }
            await LoadBaseData();
            if (PaymentRequest == null) return RedirectToPage("/User/ScanPayment", new { code = PaymentCode });
            CalculateDiscount();
            return Page();
        }

        public async Task<IActionResult> OnPostConfirmAsync()
        {
            await LoadBaseData();
            if (PaymentRequest == null) return RedirectToPage("/User/ScanPayment", new { code = PaymentCode });
            CalculateDiscount();

            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var apiUrl = GetApiBaseUrl() + "/api/PaymentRequest/Execute";
                var currentUserEmail = User.Identity?.Name;
                if (string.IsNullOrEmpty(currentUserEmail)) return RedirectToPage("/Login");
                var um = new UserManager();
                var user = um.RetrieveUserByEmail(new DTOs.User { Email = currentUserEmail });
                if (user == null) return RedirectToPage("/Login");

                var executeRequest = new
                {
                    PaymentRequestCode = PaymentCode.Trim(),
                    UserId = user.ID,
                    BankAccountId = BankAccountId,
                    PromotionId = PromotionId,
                    PromotionType = PromotionType
                };
                var jsonContent = new StringContent(JsonConvert.SerializeObject(executeRequest), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(apiUrl, jsonContent);
                if (!response.IsSuccessStatusCode)
                {
                    Message = $"Error HTTP {(int)response.StatusCode}";
                    return Page();
                }
                var exec = JsonConvert.DeserializeObject<ExecuteResponse>(await response.Content.ReadAsStringAsync());
                if (exec != null && exec.Success)
                {
                    TempData["SuccessMessage"] = exec.Message ?? "Pago procesado";
                    return RedirectToPage("/User/PaymentSuccess", new { transactionId = exec.TransactionId });
                }
                Message = exec?.Message ?? "No se pudo ejecutar el pago.";
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
            return Page();
        }

        private async Task LoadBaseData()
        {
            try
            {
                var tm = new TransactionManager();
                // Usamos API para coherencia con ScanPayment
                var httpClient = _httpClientFactory.CreateClient();
                var apiUrl = GetApiBaseUrl() + $"/api/PaymentRequest/GetByCode/{PaymentCode}";
                var resp = await httpClient.GetAsync(apiUrl);
                if (!resp.IsSuccessStatusCode) return;
                var pr = JsonConvert.DeserializeObject<PaymentRequestAPI>(await resp.Content.ReadAsStringAsync());
                if (pr == null) return;
                PaymentRequest = new TransactionDto
                {
                    ID = pr.TransactionId,
                    PaymentRequestCode = pr.PaymentRequestCode,
                    Description = pr.Description,
                    GrossAmount = (double)pr.GrossAmount,
                    NetAmount = (double)pr.NetAmount,
                    SalesTaxAmount = (double)pr.SalesTaxAmount,
                    ExpiresAt = pr.ExpiresAt,
                    TransactionStatus = pr.Status,
                    MerchantID = GetMerchantIdFromTransaction(pr.TransactionId)
                };
                var merchantManager = new MerchantManager();
                MerchantInfo = merchantManager.RetrieveMerchantById(PaymentRequest.MerchantID);
                var bam = new BankAccountManager();
                BankAccount = bam.RetrieveBankAccountById(BankAccountId);
                if (BankAccount != null)
                {
                    var fem = new FinancialEntityManager();
                    FinancialEntity = fem.RetrieveFinancialEntityById(BankAccount.FinancialEntityID);
                }

                // Cargar porcentaje de promoción seleccionada si existe
                if (PromotionId.HasValue && !string.IsNullOrWhiteSpace(PromotionType))
                {
                    if (PromotionType.Equals("Merchant", StringComparison.OrdinalIgnoreCase))
                    {
                        var mpm = new MerchantPromotionManager();
                        var promo = mpm.RetrievePromotionById(PromotionId.Value);
                        if (promo != null) DiscountPercentage = promo.DiscountPercentage;
                    }
                    else if (PromotionType.StartsWith("Finan", StringComparison.OrdinalIgnoreCase))
                    {
                        var fpm = new FinancialPromotionManager();
                        var promo = fpm.RetrievePromotionById(PromotionId.Value);
                        if (promo != null) DiscountPercentage = promo.DiscountPercentage;
                    }
                }
            }
            catch { }
        }

        private void CalculateDiscount()
        {
            // Calcular descuento sobre el monto bruto (GrossAmount), no sobre la base imponible
            DiscountAmount = 0;
            if (DiscountPercentage > 0)
            {
                DiscountAmount = GrossAmount * (DiscountPercentage / 100.0);
            }
            
            // El monto después del descuento debe ser calculado correctamente
            // NetAfterDiscount es el GrossAmount menos el descuento menos el impuesto
            var netAmountAfterDiscount = GrossAmount - DiscountAmount - SalesTaxAmount;
            NetAfterDiscount = Math.Max(0, netAmountAfterDiscount);
        }

        private int GetMerchantIdFromTransaction(int transactionId)
        {
            try
            {
                var tm = new TransactionManager();
                var tx = tm.RetrieveTransactionById(transactionId);
                return tx?.MerchantID ?? 0;
            }
            catch { return 0; }
        }

        private string GetApiBaseUrl() => "https://p2wallet-api-eyefddgeeda9c2fk.eastus-01.azurewebsites.net";

        public class PaymentRequestAPI
        {
            public int TransactionId { get; set; }
            public string PaymentRequestCode { get; set; } = string.Empty;
            public decimal GrossAmount { get; set; }
            public decimal NetAmount { get; set; }
            public decimal SalesTaxAmount { get; set; }
            public DateTime ExpiresAt { get; set; }
            public string Status { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
        }

        public class ExecuteResponse
        {
            public bool Success { get; set; }
            public int TransactionId { get; set; }
            public string Message { get; set; } = string.Empty;
        }
    }
}
