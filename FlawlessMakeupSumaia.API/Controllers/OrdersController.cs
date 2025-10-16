using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FlawlessMakeupSumaia.API.Services;
using FlawlessMakeupSumaia.API.DTOs;
using FlawlessMakeupSumaia.API.Models;

namespace FlawlessMakeupSumaia.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IImageService _imageService;

        public OrdersController(IOrderService orderService, IImageService imageService)
        {
            _orderService = orderService;
            _imageService = imageService;
        }

        private string? GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        private bool IsAuthenticated()
        {
            return User.Identity?.IsAuthenticated ?? false;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
                
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(orders.Select(o => o.ToDto()));
        }

        [HttpGet("admin/all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders.Select(o => o.ToDto()));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            // Ensure user can only access their own orders (unless admin)
            var userId = GetUserId();
            if (order.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            return Ok(order.ToDto());
        }

        [HttpGet("by-number/{orderNumber}")]
        public async Task<ActionResult<OrderDto>> GetOrderByNumber(string orderNumber)
        {
            var order = await _orderService.GetOrderByOrderNumberAsync(orderNumber);
            if (order == null)
                return NotFound();

            // For authenticated users, ensure they can only access their own orders
            if (IsAuthenticated())
            {
                var userId = GetUserId();
                if (order.UserId != userId && !User.IsInRole("Admin"))
                    return Forbid();
            }
            else
            {
                // For guest orders, allow access if it's a guest order
                if (!string.IsNullOrEmpty(order.UserId))
                    return Forbid();
            }

            return Ok(order.ToDto());
        }

        [HttpPost("create-from-cart")]
        public async Task<ActionResult<OrderDto>> CreateOrderFromCart(CreateOrderDto dto)
        {
            var userId = GetUserId();
            
            // Validate: for guest checkout, email and name are required
            if (string.IsNullOrEmpty(userId))
            {
                if (string.IsNullOrEmpty(dto.GuestEmail) || string.IsNullOrEmpty(dto.GuestName))
                {
                    return BadRequest("Guest email and name are required for guest checkout");
                }
            }
            
            try
            {
                var orderModel = dto.ToModel();
                
                // Process payment proof image if provided
                if (!string.IsNullOrEmpty(dto.PaymentProofImageUrl))
                {
                    Console.WriteLine($"=== PAYMENT PROOF RECEIVED ===");
                    Console.WriteLine($"Length: {dto.PaymentProofImageUrl.Length}");
                    Console.WriteLine($"Preview: {dto.PaymentProofImageUrl.Substring(0, Math.Min(100, dto.PaymentProofImageUrl.Length))}...");
                    
                    orderModel.PaymentProofImageUrl = _imageService.ProcessImage(dto.PaymentProofImageUrl);
                    
                    Console.WriteLine($"Processed URL: {orderModel.PaymentProofImageUrl.Substring(0, Math.Min(100, orderModel.PaymentProofImageUrl.Length))}...");
                }
                else
                {
                    Console.WriteLine("=== NO PAYMENT PROOF PROVIDED ===");
                }
                
                var order = await _orderService.CreateOrderFromCartAsync(userId, orderModel);
                
                Console.WriteLine($"=== ORDER CREATED ===");
                Console.WriteLine($"Order ID: {order.Id}");
                Console.WriteLine($"Has Payment Proof: {!string.IsNullOrEmpty(order.PaymentProofImageUrl)}");
                
                return CreatedAtAction(nameof(GetOrderByNumber), new { orderNumber = order.OrderNumber }, order.ToDto());
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<OrderDto>> UpdateOrderStatus(int id, UpdateOrderStatusDto dto)
        {
            try
            {
                var order = await _orderService.UpdateOrderStatusAsync(id, dto.Status);
                return Ok(order.ToDto());
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
