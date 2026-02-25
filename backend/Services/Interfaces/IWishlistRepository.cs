using ECommerce.DTOs;

namespace ECommerce.Services.Interfaces
{
    public interface IWishlistRepository
    {
        Task<bool> ExistsAsync(string userId, int productId);
        Task AddAsync(string userId, int productId);
        Task RemoveAsync(string userId, int productId);
        Task<List<WishlistResponseDto>> GetUserWishlistAsync(string userId);
    }
}
