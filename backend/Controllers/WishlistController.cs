using ECommerce.DTOs;
using ECommerce.Models;
using ECommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Controllers
{
    [Route("api/wishlist")]
    [ApiController]
    [Authorize(Policy = "UserOnly")]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _service;

        public WishlistController(IWishlistService service)
        {
            _service = service;
        }

        private string UserId =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)!;

       
        [HttpPost]
        public async Task<IActionResult> Add(AddWishlistDto dto)
        {
            try
            {
                await _service.AddToWishlistAsync(UserId, dto);

                return Ok(ApiResponse<string>.Ok(
                    null,
                    "Added to wishlist",
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

        [HttpDelete("{productId}")]
        public async Task<IActionResult> Remove(int productId)
        {
            try
            {
                await _service.RemoveFromWishlistAsync(UserId, productId);

                return Ok(ApiResponse<string>.Ok(
                    null,
                    "Removed from wishlist",
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

        [HttpGet]
        public async Task<IActionResult> MyWishlist()
        {
            var result = await _service.GetMyWishlistAsync(UserId);

            return Ok(ApiResponse<object>.Ok(
                result,
                "Wishlist fetched successfully",
                StatusCodes.Status200OK
            ));
        }
    }
}
