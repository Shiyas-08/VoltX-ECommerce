using ECommerce.DTOs;

namespace ECommerce.Services.Interfaces
{
    public interface ICartService
    {
        Task AddToCartAsync(string userId, AddToCartDto dto);
        Task RemoveFromCartAsync(string userId, int productId);
        Task<List<CartItemResponseDto>> GetMyCartAsync(string userId);
        Task<(bool Success, string Message)> UpdateCartAsync(string userId, UpdateCartDto dto);

    }
}
