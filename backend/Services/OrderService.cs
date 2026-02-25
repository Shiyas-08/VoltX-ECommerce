using ECommerce.Data;
using ECommerce.DTOs;
using ECommerce.Models;
using ECommerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text.Json;






namespace ECommerce.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;


        public OrderService(AppDbContext context,IConfiguration confiq)
        {
            _context = context;
            _config = confiq;
        }


        public async Task<int> CreateOrderAsync(string userId, CreateOrderDto dto)
        {
            await EnsureUserNotBlocked(userId);

            var cart = await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Category)
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Images)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.Items.Any())
                throw new ApplicationException("Cart is empty");

            // ✅ FIX 1 — FILTER ONLY VALID ITEMS (VERY IMPORTANT)
            var validItems = cart.Items
                .Where(i => i.Product.IsActive &&
                            i.Product.Category.IsActive)
                .ToList();

            if (!validItems.Any())
                throw new ApplicationException("No valid items in cart");

            // ✅ FIX 2 — VALIDATE ONLY VALID ITEMS
            foreach (var item in validItems)
            {
                if (item.Quantity > item.Product.Stock)
                    throw new ApplicationException($"{item.Product.Name} is out of stock");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var address = new OrderAddress
                {
                    FullName = dto.FullName,
                    Phone = dto.Phone,
                    AddressLine1 = dto.AddressLine1,
                    AddressLine2 = dto.AddressLine2,
                    City = dto.City,
                    State = dto.State,
                    Pincode = dto.Pincode
                };

                // ✅ FIX 3 — USE validItems for total
                var order = new Order
                {
                    UserId = userId,
                    Status = OrderStatus.Pending,
                    TotalAmount = validItems.Sum(i => i.Price * i.Quantity),
                    Address = address,
                    Items = new List<OrderItem>()
                };

                foreach (var item in validItems)
                {
                    order.Items.Add(new OrderItem
                    {
                        ProductId = item.ProductId,
                        ProductName = item.Product.Name,
                        CategoryName = item.Product.Category.Name,
                        ImageUrl = item.Product.Images
                            .Select(i => i.ImageUrl)
                            .FirstOrDefault() ?? "",
                        Quantity = item.Quantity,
                        Price = item.Price
                    });

                    // reduce stock
                    item.Product.Stock -= item.Quantity;
                }

                //_context.Orders.Add(order);

                //// (your current approach — acceptable)
                //_context.Carts.Remove(cart);

                //await _context.SaveChangesAsync();
                //await transaction.CommitAsync();

                //return order.Id;
                _context.Orders.Add(order);

                // ⭐ NEW — SAVE ADDRESS BOOK
                if (dto.SaveAddress)
                {
                    var exists = await _context.UserAddresses.AnyAsync(x =>
                        x.UserId == userId &&
                        x.AddressLine1 == dto.AddressLine1 &&
                        x.Pincode == dto.Pincode);

                    if (!exists)
                    {
                        _context.UserAddresses.Add(new UserAddress
                        {
                            UserId = userId,
                            FullName = dto.FullName,
                            Phone = dto.Phone,
                            AddressLine1 = dto.AddressLine1,
                            AddressLine2 = dto.AddressLine2,
                            City = dto.City,
                            State = dto.State,
                            Pincode = dto.Pincode
                        });
                    }
                }

                // remove cart
                _context.Carts.Remove(cart);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return order.Id;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }




        // user by now

        public async Task<int> BuyNowAsync(string userId, BuyNowOrderDto dto)
        {
            await EnsureUserNotBlocked(userId);

            var product = await _context.Products
                .Include(p => p.Category)   
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p =>
                    p.Id == dto.ProductId &&
                    p.IsActive &&
                    p.Category.IsActive);   

            if (product == null)
                throw new ApplicationException("Product not found or inactive");

            if (dto.Quantity > product.Stock)
                throw new ApplicationException("Insufficient stock");

            var address = new OrderAddress
            {
                FullName = dto.FullName,
                Phone = dto.Phone,
                AddressLine1 = dto.AddressLine1,
                AddressLine2 = dto.AddressLine2,
                City = dto.City,
                State = dto.State,
                Pincode = dto.Pincode
            };

            var order = new Order
            {
                UserId = userId,
                Status = OrderStatus.Pending,
                Address = address,
                TotalAmount = product.Price * dto.Quantity,
                Items = new List<OrderItem>()
            };

            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                CategoryName = product.Category.Name,  
                    ImageUrl = product.Images
             .Select(i => i.ImageUrl)
             .FirstOrDefault() ?? "",
                Quantity = dto.Quantity,
                Price = product.Price
            });


            //product.Stock -= dto.Quantity;

            //_context.Orders.Add(order);
            //await _context.SaveChangesAsync();

            //return order.Id;
            product.Stock -= dto.Quantity;

            _context.Orders.Add(order);

            // ⭐ NEW — SAVE ADDRESS BOOK
            if (dto.SaveAddress)
            {
                var exists = await _context.UserAddresses.AnyAsync(x =>
                    x.UserId == userId &&
                    x.AddressLine1 == dto.AddressLine1 &&
                    x.Pincode == dto.Pincode);

                if (!exists)
                {
                    _context.UserAddresses.Add(new UserAddress
                    {
                        UserId = userId,
                        FullName = dto.FullName,
                        Phone = dto.Phone,
                        AddressLine1 = dto.AddressLine1,
                        AddressLine2 = dto.AddressLine2,
                        City = dto.City,
                        State = dto.State,
                        Pincode = dto.Pincode
                    });
                }
            }

            await _context.SaveChangesAsync();

            return order.Id;
        }

        // USER get my order
        public async Task<List<OrderResponseDto>> GetMyOrdersAsync(string userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.Address)
                .Include(o => o.Items)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => MapOrder(o))
                .ToListAsync();
        }

        // user get single order
        public async Task<OrderResponseDto?> GetMyOrderAsync(string userId, int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Address)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            return order == null ? null : MapOrder(order);
        }

        // user can cancel order
        public async Task CancelOrderAsync(string userId, int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
                throw new ApplicationException("Order not found");

            // Already cancelled
            if (order.Status == OrderStatus.Cancelled)
                throw new ApplicationException("Order already cancelled");

            // Paid or after shipped cannot cancel
            if (order.Status != OrderStatus.Pending)
                throw new ApplicationException("Cannot cancel order after payment");

            // Restore stock
            foreach (var item in order.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.Stock += item.Quantity;
                }
            }

            order.Status = OrderStatus.Cancelled;
            await _context.SaveChangesAsync();
        }



        // get all order admin
        public async Task<List<OrderResponseDto>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Address)
                .Include(o => o.Items)
                 .ThenInclude(i => i.Product)
                .ThenInclude(p => p.Category)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => MapOrder(o))
                .ToListAsync();
        }

        //admin can update status
        public async Task UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);

            if (order == null)
                throw new ApplicationException("Order not found");

            // Final states cannot change
            if (order.Status == OrderStatus.Delivered)
                throw new ApplicationException("Delivered order cannot be updated");

            if (order.Status == OrderStatus.Cancelled)
                throw new ApplicationException("Cancelled order cannot be updated");

            // Paid cannot go back to Pending
            if (order.Status == OrderStatus.Paid && newStatus == OrderStatus.Pending)
                throw new ApplicationException("Cannot revert Paid order to Pending");

            // Only forward status allowed
            if ((int)newStatus < (int)order.Status)
                throw new ApplicationException("Order status cannot move backward");
            // Cancel only allowed from Pending or Paid
            if (newStatus == OrderStatus.Cancelled &&
                order.Status != OrderStatus.Pending &&
                order.Status != OrderStatus.Paid)
            {
                throw new ApplicationException(
                    "Only Pending or Paid orders can be cancelled"
                );
            }


            order.Status = newStatus;
            await _context.SaveChangesAsync();
        }

        public async Task<object> CreateRazorpayOrderAsync(string userId, int orderId)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
                throw new ApplicationException("Order not found");

            if (order.Status != OrderStatus.Pending)
                throw new ApplicationException("Order is not payable");

            // prevent duplicate payment creation
            if (!string.IsNullOrEmpty(order.RazorpayOrderId))
                throw new ApplicationException("Payment already initiated");

            var key = _config["Razorpay:Key"];
            var secret = _config["Razorpay:Secret"];

            var authToken = Convert.ToBase64String(
                System.Text.Encoding.ASCII.GetBytes($"{key}:{secret}")
            );

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", authToken);

            var payload = new
            {
                amount = (int)(order.TotalAmount * 100), // INR 
                currency = "INR",
                receipt = $"order_{order.Id}"
            };

            var response = await client.PostAsJsonAsync(
                "https://api.razorpay.com/v1/orders",
                payload
            );

            if (!response.IsSuccessStatusCode)
                throw new ApplicationException("Failed to create Razorpay order");

            var razorpayResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
            var razorpayOrderId = razorpayResponse.GetProperty("id").GetString();

            // save razor pay id
            order.RazorpayOrderId = razorpayOrderId;
            await _context.SaveChangesAsync();

            return new
            {
                razorpayOrderId,
                amount = order.TotalAmount,
                currency = "INR",
                key
            };
        }




        // payment
        public async Task VerifyPaymentAsync(string userId, VerifyPaymentDto dto)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == dto.OrderId && o.UserId == userId);

            if (order == null)
                throw new ApplicationException("Order not found");

            if (order.Status == OrderStatus.Paid)
                throw new ApplicationException("Order already paid");

            // Razorpay order binding check
            if (order.RazorpayOrderId != dto.RazorpayOrderId)
                throw new ApplicationException("Razorpay order mismatch");

            var secret = _config["Razorpay:Secret"];

            // Razorpay signature verification
            var payload = $"{dto.RazorpayOrderId}|{dto.RazorpayPaymentId}";

            using var hmac = new System.Security.Cryptography.HMACSHA256(
                System.Text.Encoding.ASCII.GetBytes(secret)
            );

            var hashBytes = hmac.ComputeHash(
                System.Text.Encoding.ASCII.GetBytes(payload)
            );

            var generatedSignature = BitConverter
                .ToString(hashBytes)
                .Replace("-", "")
                .ToLower();

            if (generatedSignature != dto.RazorpaySignature)
                throw new ApplicationException("Invalid payment signature");

            
            order.Status = OrderStatus.Paid;
            await _context.SaveChangesAsync();
        }



        // mapper

        private static OrderResponseDto MapOrder(Order o) =>new()
{
    Id = o.Id,
    Total = o.TotalAmount,
    Date = o.CreatedAt,
    Status = o.Status.ToString(),

    Address = o.Address == null ? null : new OrderAddressResponseDto
    {
        FullName = o.Address.FullName,
        Phone = o.Address.Phone,
        AddressLine1 = o.Address.AddressLine1,
        AddressLine2 = o.Address.AddressLine2,
        City = o.Address.City,
        State = o.Address.State,
        Pincode = o.Address.Pincode
    },

    Items = o.Items.Select(i => new OrderItemResponseDto
    {
        ProductId = i.ProductId,
        Name = i.ProductName,
        Image = i.ImageUrl,
        Quantity = i.Quantity,
        Price = i.Price,
        Category = i.CategoryName
    }).ToList()

   };

        private async Task EnsureUserNotBlocked(string userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id.ToString() == userId);

            if (user == null)
                throw new UnauthorizedAccessException("USER_NOT_FOUND");

            if (user.IsBlocked)
                throw new UnauthorizedAccessException("USER_BLOCKED");
        }
        public async Task<List<UserAddress>> GetMyAddressesAsync(string userId)
        {
            return await _context.UserAddresses
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.IsDefault)
                .ThenByDescending(x => x.CreatedAt)
                .ToListAsync();
        }
    }

}
