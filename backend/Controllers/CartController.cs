using ECommerce.DTOs;
using ECommerce.Models;
using ECommerce.Services;
using ECommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Controllers
{
    [ApiController]
    [Route("api/cart")]
    [Authorize(Policy = "UserOnly")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _service;

        public CartController(ICartService service)
        {
            _service = service;
        }

        private string UserId =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // add cart
        [HttpPost]
        public async Task<IActionResult> Add(AddToCartDto dto)
        {
            try
            {
                await _service.AddToCartAsync(UserId, dto);

                return Ok(ApiResponse<string>.Ok(
                    null,
                    "Added to cart",
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

        //remove from cart
        [HttpDelete("{productId}")]
        public async Task<IActionResult> Remove(int productId)
        {
            try
            {
                await _service.RemoveFromCartAsync(UserId, productId);

                return Ok(ApiResponse<string>.Ok(
                    null,
                    "Removed from cart",
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

        // get my cart
        [HttpGet]
        public async Task<IActionResult> MyCart()
        {
            var cart = await _service.GetMyCartAsync(UserId);

            return Ok(ApiResponse<object>.Ok(
                cart,
                "Cart fetched successfully",
                StatusCodes.Status200OK
            ));
        }
      
        
        [HttpPut("update")]
        public async Task<IActionResult> UpdateCart(UpdateCartDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var (success, message) = await _service.UpdateCartAsync(userId, dto);

            if (!success)
            {
                return BadRequest(ApiResponse<string>.Fail(
                    message,
                    StatusCodes.Status400BadRequest
                ));
            }

            return Ok(ApiResponse<string>.Ok(
                null,
                message,
                StatusCodes.Status200OK
            ));
        }

    }
}
