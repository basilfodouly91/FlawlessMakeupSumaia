using Microsoft.EntityFrameworkCore;
using FlawlessMakeupSumaia.API.Data;
using FlawlessMakeupSumaia.API.Models;

namespace FlawlessMakeupSumaia.API.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICartService _cartService;

        public OrderService(ApplicationDbContext context, ICartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<Order?> GetOrderByOrderNumberAsync(string orderNumber)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            order.OrderNumber = await GenerateOrderNumberAsync();
            order.OrderDate = DateTime.UtcNow;
            
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order> UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                throw new ArgumentException("Order not found");

            order.Status = status;
            
            if (status == OrderStatus.Processing && order.PaymentDate == null)
            {
                order.PaymentDate = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order> CreateOrderFromCartAsync(string userId, Order orderDetails)
        {
            var cart = await _cartService.GetCartByUserIdAsync(userId);
            if (cart == null || !cart.CartItems.Any())
                throw new ArgumentException("Cart is empty");

            var order = new Order
            {
                UserId = userId,
                OrderNumber = await GenerateOrderNumberAsync(),
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                
                // Copy shipping details
                ShippingFirstName = orderDetails.ShippingFirstName,
                ShippingLastName = orderDetails.ShippingLastName,
                ShippingAddress = orderDetails.ShippingAddress,
                ShippingAddress2 = orderDetails.ShippingAddress2,
                ShippingCity = orderDetails.ShippingCity,
                ShippingState = orderDetails.ShippingState,
                ShippingZipCode = orderDetails.ShippingZipCode,
                ShippingCountry = orderDetails.ShippingCountry,
                ShippingPhone = orderDetails.ShippingPhone,
                
                // Payment details
                PaymentMethod = orderDetails.PaymentMethod,
                Notes = orderDetails.Notes
            };

            // Create order items from cart items
            foreach (var cartItem in cart.CartItems)
            {
                var orderItem = new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Price,
                    ProductName = cartItem.Product.Name,
                    ProductImageUrl = cartItem.Product.ImageUrl
                };
                order.OrderItems.Add(orderItem);
            }

            // Calculate totals
            order.SubTotal = order.OrderItems.Sum(oi => oi.TotalPrice);
            order.Tax = order.SubTotal * 0.16m; // 16% tax
            order.ShippingCost = order.SubTotal > 50 ? 0 : 5; // Free shipping over 50 JOD
            order.TotalAmount = order.SubTotal + order.Tax + order.ShippingCost;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Clear the cart
            await _cartService.ClearCartAsync(userId);

            return await GetOrderByIdAsync(order.Id) ?? order;
        }

        private async Task<string> GenerateOrderNumberAsync()
        {
            var today = DateTime.UtcNow.ToString("yyyyMMdd");
            var lastOrder = await _context.Orders
                .Where(o => o.OrderNumber.StartsWith(today))
                .OrderByDescending(o => o.OrderNumber)
                .FirstOrDefaultAsync();

            int sequence = 1;
            if (lastOrder != null)
            {
                var lastSequence = lastOrder.OrderNumber.Substring(8);
                if (int.TryParse(lastSequence, out int parsed))
                {
                    sequence = parsed + 1;
                }
            }

            return $"{today}{sequence:D4}";
        }
    }
}
