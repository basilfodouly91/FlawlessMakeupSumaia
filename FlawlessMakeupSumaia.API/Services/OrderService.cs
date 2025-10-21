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
                // For guest orders, convert guest cart items to CartItem objects
                if (orderDetails.OrderItems == null || !orderDetails.OrderItems.Any())
                    throw new ArgumentException("Guest checkout requires cart items to be provided");
                
                // Create temporary CartItem objects from OrderItems for guest checkout
                cartItems = new List<CartItem>();
                foreach (var item in orderDetails.OrderItems)
                {
                    var product = await _context.Products
                        .Include(p => p.ProductShades)
                        .FirstOrDefaultAsync(p => p.Id == item.ProductId);
                    
                    if (product == null)
                        continue;
                    
                    var cartItem = new CartItem
                    {
                        ProductId = item.ProductId,
                        Product = product,
                        ProductShadeId = item.ProductShadeId,
                        Quantity = item.Quantity,
                        Price = item.UnitPrice
                    };
                    
                    if (item.ProductShadeId.HasValue)
                    {
                        cartItem.ProductShade = product.ProductShades.FirstOrDefault(s => s.Id == item.ProductShadeId.Value);
                    }
                    
                    cartItems.Add(cartItem);
                }
                
                if (!cartItems.Any())
                    throw new ArgumentException("No valid products found in cart");
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

            // Create order items from cart items and update stock
            foreach (var cartItem in cartItems)
            {
                // Load the product with shades to update stock
                var product = await _context.Products
                    .Include(p => p.ProductShades)
                    .FirstOrDefaultAsync(p => p.Id == cartItem.ProductId);

                if (product == null)
                    throw new ArgumentException($"Product {cartItem.ProductId} not found");

                // Check stock and update
                if (cartItem.ProductShadeId.HasValue)
                {
                    // Product with shade - check and update shade stock
                    var shade = product.ProductShades.FirstOrDefault(s => s.Id == cartItem.ProductShadeId.Value);
                    if (shade == null)
                        throw new ArgumentException($"Product shade {cartItem.ProductShadeId.Value} not found");

                    if (shade.StockQuantity < cartItem.Quantity)
                        throw new ArgumentException($"Insufficient stock for {product.Name} - {shade.Name}. Available: {shade.StockQuantity}, Requested: {cartItem.Quantity}");

                    shade.StockQuantity -= cartItem.Quantity;
                }
                else
                {
                    // Product without shade - check and update product stock
                    if (product.StockQuantity < cartItem.Quantity)
                        throw new ArgumentException($"Insufficient stock for {product.Name}. Available: {product.StockQuantity}, Requested: {cartItem.Quantity}");

                    product.StockQuantity -= cartItem.Quantity;
                }

                var orderItem = new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    ProductShadeId = cartItem.ProductShadeId,
                    ProductShadeName = cartItem.ProductShade?.Name,
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
