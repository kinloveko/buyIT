using AutoMapper;
using BuyIT.API.Coupon.Models;
using BuyIT.API.Coupon.Models.Dto;

namespace BuyIT.API.Coupon.Profiles
{
    public class CouponProfile : Profile
    {
        public CouponProfile()
        {
           //create a map from Coupons to CreateCouponDto that will generate the CoupodId
            CreateMap<Coupons, CreateCouponDto>().ReverseMap();
        
            //create a map from Coupons to CouponDto
            CreateMap<Coupons, CouponDto>().ReverseMap();
        }
    }
}
