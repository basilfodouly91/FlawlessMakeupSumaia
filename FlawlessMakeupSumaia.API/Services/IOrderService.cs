using FlawlessMakeupSumaia.API.Models;

namespace FlawlessMakeupSumaia.API.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task<Order?> GetOrderByOrderNumberAsync(string orderNumber);
        Task<Order> CreateOrderAsync(Order order);
        Task<Order> UpdateOrderStatusAsync(int orderId, OrderStatus status);
        Task<Order> CreateOrderFromCartAsync(string? userId, Order orderDetails);
    }
}
