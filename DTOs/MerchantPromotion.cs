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
        public string PromotionType { get; set; }
        public double DiscountPercentage { get; set; }
        public double MaxRefund { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int AvailableQuantity { get; set; }
        public String ValidationStatus { get; set; }

    }
}
