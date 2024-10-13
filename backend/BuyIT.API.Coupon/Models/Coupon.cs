using System.ComponentModel.DataAnnotations;

namespace BuyIT.API.Coupon.Models
{
    public class Coupons
    {
        [Key]
        public int CouponId { get; set; }
        [Required]
        public string CouponCode { get; set; }
        [Required]
        public double DiscountAmount { get; set; }
        public int MinAmount { get; set; }
    }
}
