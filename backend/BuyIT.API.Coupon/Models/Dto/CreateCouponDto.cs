namespace BuyIT.API.Coupon.Models.Dto
{
    public class CreateCouponDto
    {
        public string CouponCode { get; set; }
        public double DiscountAmount { get; set; }
        public double MinAmount { get; set; }
    }
}
