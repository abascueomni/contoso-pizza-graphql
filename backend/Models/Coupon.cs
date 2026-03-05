namespace ContosoPizza.Models;

public class Coupon
{
    public int Id { get; set; }                       // Primary key
    public string CouponCode { get; set; } = "";
    public double DiscountPercent { get; set; }
}
