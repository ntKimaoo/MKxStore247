using MKxStore247.Models;

namespace MKxStore247.Services.Interface
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(string userId, int? addressId, int paymentMethodId, int? couponId, string note);
        Task<Order> GetOrderByIdAsync(int orderId, string userId);
        Task<List<Order>> GetUserOrdersAsync(string userId, int page = 1, int pageSize = 10);
        Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status);
        Task<bool> CancelOrderAsync(int orderId, string userId);
        Task<decimal> CalculateShippingFeeAsync(int? addressId);
        //Task<decimal> ApplyDiscountAsync(int? couponId, decimal amount, string userId);
        Task<bool> ValidateStockAsync(string userId);
    }
}
