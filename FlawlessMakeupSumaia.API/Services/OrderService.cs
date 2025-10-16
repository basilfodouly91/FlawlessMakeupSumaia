using Microsoft.EntityFrameworkCore;
using FlawlessMakeupSumaia.API.Data;
using FlawlessMakeupSumaia.API.Models;

namespace FlawlessMakeupSumaia.API.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICartService _cartService;
        private readonly IEmailService _emailService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            ApplicationDbContext context, 
            ICartService cartService,
            IEmailService emailService,
            ILogger<OrderService> logger)
        {
            _context = context;
            _cartService = cartService;
            _emailService = emailService;
            _logger = logger;
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

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.User)
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
            
            if (status == OrderStatus.Confirmed && order.PaymentDate == null)
            {
                order.PaymentDate = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order> CreateOrderFromCartAsync(string? userId, Order orderDetails)
        {
            List<CartItem> cartItems;

            // Get cart items based on whether user is authenticated or guest
            if (!string.IsNullOrEmpty(userId))
            {
                var cart = await _cartService.GetCartByUserIdAsync(userId);
                if (cart == null || !cart.CartItems.Any())
                    throw new ArgumentException("Cart is empty");
                cartItems = cart.CartItems.ToList();
            }
            else
            {
                // For guest orders, we expect cart items to be passed through the guest cart service
                // This is a simplified approach - in production, you might want to handle this differently
                throw new ArgumentException("Guest checkout requires cart items to be provided");
            }

            var order = new Order
            {
                UserId = userId,
                GuestEmail = orderDetails.GuestEmail,
                GuestName = orderDetails.GuestName,
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
                PaymentProofImageUrl = orderDetails.PaymentProofImageUrl,
                Notes = orderDetails.Notes
            };

            // Create order items from cart items
            foreach (var cartItem in cartItems)
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
            order.Tax = 0; // No tax
            order.ShippingCost = 3; // Fixed 3 JOD shipping
            order.TotalAmount = order.SubTotal + order.Tax + order.ShippingCost;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Clear the cart
            if (!string.IsNullOrEmpty(userId))
            {
                await _cartService.ClearCartAsync(userId);
            }

            // Send email notification to admin
            try
            {
                await _emailService.SendOrderNotificationToAdminAsync(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email notification for order {order.OrderNumber}");
                // Don't fail the order creation if email fails
            }

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
