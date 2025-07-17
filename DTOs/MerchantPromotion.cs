using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public class MerchantPromotion : BaseDTO
    {
        public int MerchantID { get; set; }
        public string MerchantPromotionName { get; set; }
        public string MerchantPromotionDescription { get; set; }
        public string MerchantPromotionTerms { get; set; }
        public string MerchantPromotionImage { get; set; }
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
