using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public class Transaction : BaseDTO
    {
        public int UserID { get; set; }
        public int MerchantID { get; set; }
        public int BankAccountID { get; set; }
        public double GrossAmount { get; set; }
        public double NetAmount { get; set; }
        public double CommissionApplied { get; set; }
        public double SalesTaxAmount { get; set; }
        public double TaxRateApplied { get; set; }
        public DateTime Timestamp { get; set; }
        public string TransactionStatus { get; set; }
        public int? FinancialPromotionID { get; set; }
        public int? MerchantPromotionID { get; set; }
    }
}