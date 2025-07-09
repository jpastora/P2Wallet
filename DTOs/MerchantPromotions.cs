using DTOs;

public class MerchantPromotions : BaseDTO
{
    public int PromotionID { get; set; }
    public int MerchantID { get; set; }
    public string PromotionType { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal? MaxRefund { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int AvailableQuantity { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
