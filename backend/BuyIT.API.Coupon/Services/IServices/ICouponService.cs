using BuyIT.API.Coupon.Models.Dto;

namespace BuyIT.API.Coupon.Services.IServices
{
    public interface ICouponService
    {
        Task<List<CouponDto>> GetAllCouponsAsync(string searchTerm = null);
        Task<CouponDto> GetCouponByCodeAsync(string couponCode);
        Task<CouponDto> CreateCouponAsync(CreateCouponDto createCouponDto);
        Task<CouponDto> UpdateCouponAsync(CouponDto couponDto);
        Task<bool> DeleteCouponAsync(int id);
    }
}
