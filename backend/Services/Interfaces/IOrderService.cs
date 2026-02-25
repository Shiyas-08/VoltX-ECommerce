using ECommerce.DTOs;
using ECommerce.Models;

namespace ECommerce.Services.Interfaces
{
    public interface IOrderService
    {
        Task<int> CreateOrderAsync(string userId, CreateOrderDto dto);
        Task<OrderResponseDto?> GetMyOrderAsync(string userId, int orderId);
        Task CancelOrderAsync(string userId, int orderId);
        Task<List<OrderResponseDto>> GetMyOrdersAsync(string userId);

        Task<List<OrderResponseDto>> GetAllOrdersAsync();
        Task UpdateOrderStatusAsync(int orderId, OrderStatus status);

        Task VerifyPaymentAsync(string userId, VerifyPaymentDto dto);
        Task<int> BuyNowAsync(string userId, BuyNowOrderDto dto);
        Task<object> CreateRazorpayOrderAsync(string userId, int orderId);
        Task<List<UserAddress>> GetMyAddressesAsync(string userId);



    }
}
