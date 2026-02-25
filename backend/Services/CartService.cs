using ECommerce.Data;
using ECommerce.DTOs;
using ECommerce.Models;
using ECommerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace ECommerce.Services
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;

        public CartService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddToCartAsync(string userId, AddToCartDto dto)
        {
            await EnsureUserNotBlocked(userId);

            if (dto.Quantity <= 0)
                throw new ApplicationException("Quantity must be greater than zero");

            var product = await _context.Products
                 .Include(p => p.Category)
                 .FirstOrDefaultAsync(p =>
                 p.Id == dto.ProductId &&
                 p.IsActive &&
                 p.Category.IsActive);

            if (product == null)
                throw new ApplicationException("Product not found");

            if (dto.Quantity > product.Stock)
                throw new ApplicationException("Insufficient stock");

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
            }

            var existingItem = cart.Items
                .FirstOrDefault(i => i.ProductId == dto.ProductId);

            if (existingItem != null)
            {
                if (existingItem.Quantity + dto.Quantity > product.Stock)
                    throw new ApplicationException("Insufficient stock");

                existingItem.Quantity += dto.Quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    Price = product.Price
                });
            }

            await _context.SaveChangesAsync();
        }


        public async Task RemoveFromCartAsync(string userId, int productId)
        {
            await EnsureUserNotBlocked(userId);

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
                throw new ApplicationException("Cart not found");

            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (item == null)
                throw new ApplicationException("Item not found in cart");

            cart.Items.Remove(item);
            await _context.SaveChangesAsync();
        }


        public async Task<List<CartItemResponseDto>> GetMyCartAsync(string userId)
        {
            await EnsureUserNotBlocked(userId);

            // ✅ STEP 1 — find invalid items (NEW)
            var invalidItems = await _context.CartItems
                .Where(ci => ci.Cart.UserId == userId &&
                    (!ci.Product.IsActive ||
                     !ci.Product.Category.IsActive ||
                     ci.Product.Stock == 0))
                .ToListAsync();

            // ✅ STEP 2 — remove them (NEW)
            if (invalidItems.Any())
            {
                _context.CartItems.RemoveRange(invalidItems);
                await _context.SaveChangesAsync();
            }

            // ✅ STEP 3 — return clean cart (your existing logic)
            return await _context.CartItems
                .Where(ci => ci.Cart.UserId == userId
                    && ci.Product.IsActive
                    && ci.Product.Category.IsActive)
                .Include(ci => ci.Product)
                    .ThenInclude(p => p.Images)
                .Include(ci => ci.Product)
                    .ThenInclude(p => p.Category)
                .Select(ci => new CartItemResponseDto
                {
                    Id = ci.Id,
                    ProductId = ci.ProductId,
                    Name = ci.Product.Name,
                    Price = ci.Price,
                    Quantity = ci.Quantity,
                    Stock = ci.Product.Stock,

                    Image = ci.Product.Images
                        .Select(i => i.ImageUrl)
                        .FirstOrDefault(),

                    Category = ci.Product.Category.Name
                })
                .ToListAsync();
        }


        public async Task<(bool Success, string Message)> UpdateCartAsync(string userId, UpdateCartDto dto)
        {
            await EnsureUserNotBlocked(userId);

            if (dto.Quantity < 0)
                return (false, "Quantity cannot be negative");

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
                return (false, "Cart not found");

            var item = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);
            if (item == null)
                return (false, "Product not in cart");

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p =>
                    p.Id == dto.ProductId &&
                    p.IsActive &&
                    p.Category.IsActive);

            if (product == null)
                return (false, "Product not found");

            if (dto.Quantity > product.Stock)
                return (false, "Insufficient stock");

            // Quantity = 0 → Remove item
            if (dto.Quantity == 0)
            {
                cart.Items.Remove(item);
                await _context.SaveChangesAsync();
                return (true, "Item removed from cart");
            }

            // Update quantity
            item.Quantity = dto.Quantity;
            await _context.SaveChangesAsync();

            return (true, "Cart updated successfully");
        }
        private async Task EnsureUserNotBlocked(string userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id.ToString() == userId);

            if (user == null)
                throw new UnauthorizedAccessException("USER_NOT_FOUND");

            if (user.IsBlocked)
                throw new UnauthorizedAccessException("USER_BLOCKED");
        }

    }
}
