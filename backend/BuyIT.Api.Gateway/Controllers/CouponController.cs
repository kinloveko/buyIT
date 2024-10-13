using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Grpc.Net.Client;
using Grpc.Core;
using System.Threading.Tasks;

namespace BuyIT.Api.Gateway.Controllers
{
    [Route("api/coupons")]
    [ApiController]
    [Authorize]
    [Authorize]
    public class CouponController : ControllerBase
    {
        private readonly CouponService.CouponServiceClient _couponServiceClient;

        public CouponController(CouponService.CouponServiceClient couponServiceClient)
        {
            _couponServiceClient = couponServiceClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCoupons([FromQuery] string searchTerm = null)
        {
            try
            {
                var request = new CouponRequest { SearchTerm = searchTerm ?? string.Empty };
                var response = await _couponServiceClient.GetAllCouponsAsync(request);
                if (!response.Success)
                {
                    return NotFound(response);
                }
                return Ok(response);
            }
            catch (RpcException ex) when ((int)ex.StatusCode == 404)
            {
                return NotFound(new { Success = false, Message = ex.Status.Detail });
            }
            catch (RpcException ex)
            {
                return StatusCode((int)ex.StatusCode, new { Success = false, Message = ex.Status.Detail });
            }
        }

        [HttpGet("{couponCode}")]
        public async Task<IActionResult> GetCouponByCode(string couponCode)
        {
            try
            {
                var request = new CouponCodeRequest { CouponCode = couponCode };
                var response = await _couponServiceClient.GetCouponByCodeAsync(request);
                if (!response.Success)
                {
                    return NotFound(response);
                }
                return Ok(response);
            }
            catch (RpcException ex) when ((int)ex.StatusCode == 5)
            {
                return NotFound(new { Success = false, Message = ex.Status.Detail });
            }
            catch (RpcException ex)
            {
                return StatusCode((int)ex.StatusCode, new { Success = false, Message = ex.Status.Detail });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCoupon([FromBody] CreateCouponRequest request)
        {
            try
            {
                var response = await _couponServiceClient.CreateCouponAsync(request);
                return Ok(response);
            }
            catch (RpcException ex) when ((int)ex.StatusCode == 3)
            {
                return StatusCode(StatusCodes.Status400BadRequest,new { Success = false, Message = ex.Status.Detail });
            }
            catch (RpcException ex) when ((int)ex.StatusCode == 6)
            {
                return StatusCode(StatusCodes.Status409Conflict, new { Success = false, Message = ex.Status.Detail });
            }
            catch (RpcException ex)
            {
                return StatusCode((int)ex.StatusCode, new { Success = false, Message = ex.Status.Detail });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCoupon([FromBody] UpdateCouponRequest request)
        {
            try
            {
                var response = await _couponServiceClient.UpdateCouponAsync(request);
                return Ok(response);
            }
            catch (RpcException ex) when ((int)ex.StatusCode == 5)
            {
                return NotFound(new { Success = false, Message = ex.Status.Detail });
            }
            catch (RpcException ex)
            {
                return StatusCode((int)ex.StatusCode, new { Success = false, Message = ex.Status.Detail });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
            
            try
            {
                var request = new DeleteCouponRequest { CouponId = id };
                var response = await _couponServiceClient.DeleteCouponAsync(request);
                return Ok(response);
            }
            catch (RpcException ex) when ((int)ex.StatusCode == 5)
            {
                return NotFound(new { Success = false, Message = ex.Status.Detail });
            }
            catch (RpcException ex)
            {
                return StatusCode((int)ex.StatusCode, new { Success = false, Message = ex.Status.Detail });
            }
        }
    }
}