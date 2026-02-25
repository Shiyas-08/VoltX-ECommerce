using ECommerce.DTOs;

namespace ECommerce.Services.Interfaces
{
    public interface IWishlistService
    {
        Task AddToWishlistAsync(string userId, AddWishlistDto dto);
        Task RemoveFromWishlistAsync(string userId, int productId);
        Task<List<WishlistResponseDto>> GetMyWishlistAsync(string userId);
    }
}
