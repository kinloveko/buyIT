using BuyIT.Api.Coupon;
using BuyIT.API.Coupon.Models.Dto;
using BuyIT.API.Coupon.Services.IServices;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

public class CouponServiceImpl : CouponService.CouponServiceBase
{
    private readonly ICouponService _couponService;

    public CouponServiceImpl(ICouponService couponService)
    {
        _couponService = couponService;
    }

    public override async Task<CouponResponse> GetAllCoupons(CouponRequest request, ServerCallContext context)
    {
        var coupons = await _couponService.GetAllCouponsAsync(request.SearchTerm);
        if (coupons == null || !coupons.Any())
        {
            context.Status = new Status(StatusCode.NotFound, "No coupons found");
            return new CouponResponse
            {
                Success = false,
                Message = "No coupons found"
            };
        }

        return new CouponResponse
        {
            Coupons = { coupons.Select(c => new Coupon
            {
                CouponId = c.CouponId,
                CouponCode = c.CouponCode,
                DiscountAmount = c.DiscountAmount,
                 MinAmount = c.MinAmount
            }) },
            Success = true,
            Message = "Coupons retrieved successfully"
        };
    }

    public override async Task<CouponResponse> GetCouponByCode(CouponCodeRequest request, ServerCallContext context)
    {
        var coupon = await _couponService.GetCouponByCodeAsync(request.CouponCode);
        if (coupon == null)
        {
            context.Status = new Status(StatusCode.NotFound, "Coupon not found");
            return new CouponResponse
            {
                Success = false,
                Message = "Coupon not found"
            };
        }

        return new CouponResponse
        {
            Coupons = { new Coupon
            {
                CouponId = coupon.CouponId,
                CouponCode = coupon.CouponCode,
                DiscountAmount = coupon.DiscountAmount,
                 MinAmount = coupon.MinAmount
            } },
            Success = true,
            Message = "Coupon retrieved successfully"
        };
    }

    public override async Task<CouponResponse> CreateCoupon(CreateCouponRequest request, ServerCallContext context)
    {
        if(string.IsNullOrEmpty(request.CouponCode))
        {
            context.Status = new Status(StatusCode.InvalidArgument, "Coupon code is required");
            return new CouponResponse
            {
                Success = false,
                Message = "Coupon code is  required"
            };
        }
        else if(request.DiscountAmount <= 0 || request.MinAmount <= 0)
        {
            context.Status = new Status(StatusCode.InvalidArgument, "Discount amount and minimum amount must be greater than 0");
            return new CouponResponse
            {
                Success = false,
                Message = "Discount amount and minimum amount must be greater than 0"
            };
        }
        
        // Check if CouponCode already exists
        var existingCoupon = await _couponService.GetCouponByCodeAsync(request.CouponCode);
        if (existingCoupon != null)
        {
            context.Status = new Status(StatusCode.AlreadyExists, "Coupon code already exists");
            return new CouponResponse
            {
                Success = false,
                Message = "Coupon code already exists"
            };
        }

        var coupon = await _couponService.CreateCouponAsync(new CreateCouponDto
        {
            CouponCode = request.CouponCode,
            DiscountAmount = request.DiscountAmount,
            MinAmount = request.MinAmount
        });

        
        return new CouponResponse
        {
            Coupons = { new Coupon
            {
                CouponId = coupon.CouponId,
                CouponCode = coupon.CouponCode,
                DiscountAmount = coupon.DiscountAmount,
                MinAmount = coupon.MinAmount
            } },
            Success = true,
            Message = "Coupon created successfully"
        };
    }

    public override async Task<CouponResponse> UpdateCoupon(UpdateCouponRequest request, ServerCallContext context)
    {
        var coupon = await _couponService.UpdateCouponAsync(new CouponDto
        {
            CouponId = request.CouponId,
            CouponCode = request.CouponCode,
            DiscountAmount = request.DiscountAmount,
            MinAmount = request.MinAmount,
        });

        if (coupon == null)
        {
            context.Status = new Status(StatusCode.NotFound, "Coupon not found or failed to update");
            return new CouponResponse
            {
                Success = false,
                Message = "Coupon not found or failed to update"
            };
        }

        return new CouponResponse
        {
            Coupons = { new Coupon
            {
                CouponId = coupon.CouponId,
                CouponCode = coupon.CouponCode,
                DiscountAmount = coupon.DiscountAmount,
                 MinAmount = coupon.MinAmount
            } },
            Success = true,
            Message = "Coupon updated successfully"
        };
    }

    public override async Task<DeleteCouponResponse> DeleteCoupon(DeleteCouponRequest request, ServerCallContext context)
    {
        var success = await _couponService.DeleteCouponAsync(request.CouponId);
        if (!success)
        {
            context.Status = new Status(StatusCode.NotFound, "Coupon not found");
        }

        return new DeleteCouponResponse
        {
            Success = success,
            Message = success ? "Coupon deleted successfully" : "Coupon not found"
        };
    }
}