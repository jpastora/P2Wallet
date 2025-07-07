using System;

namespace DTOs
{
    public class FinancialEntityPromotions : BaseDTO
    {
        public int PromotionID { get; set; }
        public int FinancialEntityID { get; set; }
        public string PromotionType { get; set; } 
        public decimal DiscountPercentage { get; set; }
        public decimal MaxRefund { get; set; } 
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? AvailableQuantity { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
