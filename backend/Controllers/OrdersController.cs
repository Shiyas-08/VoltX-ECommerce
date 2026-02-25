using ECommerce.DTOs;
using ECommerce.Models;
using ECommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _service;

        public OrdersController(IOrderService service)
        {
            _service = service;
        }

        private string UserId =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // create order
        [Authorize(Policy = "UserOnly")]
        [HttpPost("add-cart")]
        public async Task<IActionResult> Create([FromForm] CreateOrderDto dto)
        {
            try
            {
                var orderId = await _service.CreateOrderAsync(UserId, dto);

                return Ok(ApiResponse<int>.Ok(
                    orderId,
                    "Order placed successfully",
                    StatusCodes.Status200OK
                ));
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ApiResponse<string>.Fail(
                    ex.Message,
                    StatusCodes.Status400BadRequest
                ));
            }
        }

        // by now 
        [Authorize(Policy = "UserOnly")]
        [HttpPost("buy-now")]
        public async Task<IActionResult> BuyNow([FromForm] BuyNowOrderDto dto)
        {
            try
            {
                var orderId = await _service.BuyNowAsync(UserId, dto);

                return Ok(ApiResponse<int>.Ok(
                    orderId,
                    "Buy now order placed successfully",
                    StatusCodes.Status200OK
                ));
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ApiResponse<string>.Fail(
                    ex.Message,
                    StatusCodes.Status400BadRequest
                ));
            }
        }

        // my orders
        [Authorize(Policy = "UserOnly")]
        [HttpGet("my")]
        public async Task<IActionResult> MyOrders()
        {
            var orders = await _service.GetMyOrdersAsync(UserId);

            return Ok(ApiResponse<List<OrderResponseDto>>.Ok(
                orders,
                "Orders fetched successfully",
                StatusCodes.Status200OK
            ));

        }

        // get single order
        [Authorize(Policy = "UserOnly")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var order = await _service.GetMyOrderAsync(UserId, id);

            if (order == null)
            {
                return NotFound(ApiResponse<string>.Fail(
                    "Order not found",
                    StatusCodes.Status404NotFound
                ));
            }

            return Ok(ApiResponse<OrderResponseDto>.Ok(
      order,
      "Order fetched successfully",
      StatusCodes.Status200OK
  ));

        }
            //cancel order
            [Authorize(Policy = "UserOnly")]
        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                await _service.CancelOrderAsync(UserId, id);

                return Ok(ApiResponse<string>.Ok(
                    null,
                    "Order cancelled successfully",
                    StatusCodes.Status200OK
                ));
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ApiResponse<string>.Fail(
                    ex.Message,
                    StatusCodes.Status400BadRequest
                ));
            }
        }



        // get all orders
        [Authorize(Policy = "AdminOnly")]
        [HttpGet("admin/all")]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _service.GetAllOrdersAsync();

            return Ok(ApiResponse<List<OrderResponseDto>>.Ok(
       orders,
       "All orders fetched successfully",
       StatusCodes.Status200OK
   ));

        }

        // admin can change status
        [Authorize(Policy = "AdminOnly")]
        [HttpPatch("{id}/admin/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusDto dto)
        {
            try
            {
                await _service.UpdateOrderStatusAsync(id, dto.Status);

                return Ok(ApiResponse<string>.Ok(
                    null,
                    "Order status updated",
                    StatusCodes.Status200OK
                ));
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ApiResponse<string>.Fail(
                    ex.Message,
                    StatusCodes.Status400BadRequest
                ));
            }
        }

        // create razorpay order
        // create razorpay order
        [Authorize(Policy = "UserOnly")]
        [HttpPost("{orderId}/create-payment")]
        public async Task<IActionResult> CreatePayment(int orderId)
        {
            var result = await _service.CreateRazorpayOrderAsync(UserId, orderId);

            return Ok(ApiResponse<object>.Ok(
                result,
                "Razorpay order created",
                StatusCodes.Status200OK
            ));
        }

        // verify payment
        [Authorize(Policy = "UserOnly")]
        [HttpPost("verify-payment")]
        public async Task<IActionResult> VerifyPayment([FromBody] VerifyPaymentDto dto)
        {
            try
            {
                await _service.VerifyPaymentAsync(UserId, dto);
                return Ok(ApiResponse<string>.Ok(
                    null,
                    "Payment verified successfully",
                    StatusCodes.Status200OK
                ));
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ApiResponse<string>.Fail(
                    ex.Message,
                    StatusCodes.Status400BadRequest
                ));
            }
        }
        [Authorize(Policy = "UserOnly")]
        [HttpGet("my-addresses")]
        public async Task<IActionResult> GetMyAddresses()
        {
            var data = await _service.GetMyAddressesAsync(UserId);

            return Ok(ApiResponse<List<UserAddress>>.Ok(
                data,
                "Addresses fetched successfully",
                StatusCodes.Status200OK
            ));
        }

    }
}
