using ECommerce.Data;
using ECommerce.DTOs;
using ECommerce.Models;
using ECommerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly AppDbContext _context;

        public WishlistService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddToWishlistAsync(string userId, AddWishlistDto dto)
        {
            var productExists = await _context.Products
                .AnyAsync(p => p.Id == dto.ProductId && p.IsActive);

            if (!productExists)
                throw new ApplicationException("Product not found or inactive");

            var exists = await _context.Wishlists
                .AnyAsync(w => w.UserId == userId && w.ProductId == dto.ProductId);

            if (exists)
                throw new ApplicationException("Product already in wishlist");

            _context.Wishlists.Add(new Wishlist
            {
                UserId = userId,
                ProductId = dto.ProductId
            });

            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromWishlistAsync(string userId, int productId)
        {
            var wishlist = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

            if (wishlist == null)
                throw new ApplicationException("Wishlist item not found");

            _context.Wishlists.Remove(wishlist);
            await _context.SaveChangesAsync();
        }

        public async Task<List<WishlistResponseDto>> GetMyWishlistAsync(string userId)
        {
            return await _context.Wishlists
                .Where(w => w.UserId == userId
                            && w.Product.IsActive
                            && w.Product.Category.IsActive)
                .Include(w => w.Product)
                    .ThenInclude(p => p.Images)
                .Select(w => new WishlistResponseDto
                {
                    ProductId = w.Product.Id,
                    ProductName = w.Product.Name,
                    Price = w.Product.Price,
                    ImageUrl = w.Product.Images
                        .Where(i => i.IsActive)
                        .Select(i => i.ImageUrl)
                        .FirstOrDefault()
                })
                .ToListAsync();
        }

    }

}

