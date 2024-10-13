using AutoMapper;
using BuyIT.API.Coupon.Data;
using BuyIT.API.Coupon.Models;
using BuyIT.API.Coupon.Models.Dto;
using BuyIT.API.Coupon.Services.IServices;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuyIT.API.Coupon.Services
{
    public class CouponService : ICouponService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CouponService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<CouponDto>> GetAllCouponsAsync(string searchTerm = null)
        {
            IQueryable<Coupons> query = _context.Coupons;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(c => c.CouponCode.ToLower().Contains(searchTerm) || c.DiscountAmount.ToString().Contains(searchTerm));
            }

            List<Coupons> coupons = await query.ToListAsync();
            return _mapper.Map<List<CouponDto>>(coupons);
        }

        public async Task<CouponDto> GetCouponByCodeAsync(string couponCode)
        {
            Coupons coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.CouponCode.ToLower() == couponCode.ToLower());
            return _mapper.Map<CouponDto>(coupon);
        }

        public async Task<CouponDto> CreateCouponAsync(CreateCouponDto createCouponDto)
        {
            var existingCoupon = await _context.Coupons.FirstOrDefaultAsync(c => c.CouponCode == createCouponDto.CouponCode);
            if (existingCoupon != null)
            {
                return null; // Or throw an exception
            }

            var coupon = _mapper.Map<Coupons>(createCouponDto);
            await _context.Coupons.AddAsync(coupon);
            await _context.SaveChangesAsync();

            return _mapper.Map<CouponDto>(coupon);
        }

        public async Task<CouponDto> UpdateCouponAsync(CouponDto couponDto)
        {
            var existingCoupon = await _context.Coupons.FindAsync(couponDto.CouponId);
            if (existingCoupon == null)
            {
                return null; // Or throw an exception
            }

            _mapper.Map(couponDto, existingCoupon);
            _context.Entry(existingCoupon).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return _mapper.Map<CouponDto>(existingCoupon);
        }

        public async Task<bool> DeleteCouponAsync(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null)
            {
                return false;
            }

            _context.Coupons.Remove(coupon);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
