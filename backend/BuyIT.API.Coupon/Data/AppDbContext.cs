using BuyIT.API.Coupon.Models;
using Microsoft.EntityFrameworkCore;

namespace BuyIT.API.Coupon.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Coupons> Coupons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Coupons>().HasData(
                new Coupons
                {
                    CouponId = 1,
                    CouponCode = "10OFF",
                    DiscountAmount = 10,
                    MinAmount = 100
                },
                new Coupons
                {
                    CouponId = 2,
                    CouponCode = "20OFF",
                    DiscountAmount = 20,
                    MinAmount = 200
                },
                new Coupons
                {
                    CouponId = 3,
                    CouponCode = "30OFF",
                    DiscountAmount = 30,
                    MinAmount = 300
                }
            );
        }
    }
}
