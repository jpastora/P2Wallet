using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public class FinancialPromotion : BaseDTO
    {
        public int FinancialEntityID { get; set; }
        public string FinancialPromotionName { get; set; }
        public string FinancialPromotionDescription { get; set; }
        public string FinancialPromotionTerms { get; set; }
        public string FinancialPromotionImage { get; set; }
        public string PromotionType { get; set; }
        public double DiscountPercentage { get; set; }
        public double MaxRefund { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int AvailableQuantity { get; set; }
        public string ValidationStatus { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
