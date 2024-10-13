using AutoMapper;
using BuyIT.API.Coupon.Models.Dto;
using BuyIT.API.Coupon.Services;
using BuyIT.API.Coupon.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BuyIT.API.Coupon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CouponApiController : ControllerBase
    {
        private readonly ICouponService _couponService;
        private ResponseDto response;

        public CouponApiController(ICouponService couponService)
        {
            _couponService = couponService;
            response = new ResponseDto();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCoupons(string searchTerm = null)
        {
            try
            {
                var coupons = await _couponService.GetAllCouponsAsync(searchTerm);
                if (coupons.Count == 0)
                {
                    response.Message = "No coupons found";
                    response.IsSuccess = false;
                    return NotFound(response);
                }

                response.Result = coupons;
                response.Message = string.IsNullOrEmpty(searchTerm)
                    ? "Coupons retrieved successfully"
                    : $"Coupons matching '{searchTerm}' retrieved successfully";

                return Ok(response);
            }
            catch (Exception error)
            {
                response.Message = $"An error occurred: {error.Message}";
                response.IsSuccess = false;
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpGet("{CouponCode}")]
        public async Task<IActionResult> GetCouponByCode(string CouponCode)
        {
            try
            {
                var coupon = await _couponService.GetCouponByCodeAsync(CouponCode);
                if (coupon == null)
                {
                    response.Message = "Coupon not found";
                    response.IsSuccess = false;
                    return NotFound(response);
                }

                response.Result = coupon;
                return Ok(response);
            }
            catch (Exception error)
            {
                response.Message = error.Message;
                response.IsSuccess = false;
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCoupon([FromBody] CreateCouponDto createCouponDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var coupon = await _couponService.CreateCouponAsync(createCouponDto);
                    if (coupon == null)
                    {
                        response.Message = "Coupon code already exists";
                        response.IsSuccess = false;
                        return Conflict(response);
                    }

                    response.Result = coupon;
                    response.IsSuccess = true;
                    return CreatedAtAction("GetCouponByCode", new { CouponCode = coupon.CouponCode }, response);
                }
                else
                {
                    response.Message = "Invalid model";
                    response.IsSuccess = false;
                    return BadRequest(response);
                }
            }
            catch (Exception error)
            {
                response.Message = error.Message;
                response.IsSuccess = false;
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCoupon([FromBody] CouponDto couponDto)
        {
            try
            {
                var coupon = await _couponService.UpdateCouponAsync(couponDto);
                if (coupon == null)
                {
                    response.Message = "Coupon not found";
                    response.IsSuccess = false;
                    return NotFound(response);
                }

                response.Message = "Coupon updated successfully";
                response.IsSuccess = true;
                return Ok(response);
            }
            catch (Exception error)
            {
                response.Message = error.Message;
                response.IsSuccess = false;
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
            try
            {
                var success = await _couponService.DeleteCouponAsync(id);
                if (!success)
                {
                    response.Message = "Coupon not found";
                    response.IsSuccess = false;
                    return NotFound(response);
                }

                response.Message = "Coupon deleted successfully";
                response.IsSuccess = true;
                return Ok(response);
            }
            catch (Exception error)
            {
                response.Message = error.Message;
                response.IsSuccess = false;
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
    }
}
