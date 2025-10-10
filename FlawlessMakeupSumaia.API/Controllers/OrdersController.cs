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
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            var userId = GetUserId();
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(orders.Select(o => o.ToDto()));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            // Ensure user can only access their own orders
            var userId = GetUserId();
            if (order.UserId != userId)
                return Forbid();

            return Ok(order.ToDto());
        }

        [HttpGet("by-number/{orderNumber}")]
        public async Task<ActionResult<OrderDto>> GetOrderByNumber(string orderNumber)
        {
            var order = await _orderService.GetOrderByOrderNumberAsync(orderNumber);
            if (order == null)
                return NotFound();

            // Ensure user can only access their own orders
            var userId = GetUserId();
            if (order.UserId != userId)
                return Forbid();

            return Ok(order.ToDto());
        }

        [HttpPost("create-from-cart")]
        public async Task<ActionResult<OrderDto>> CreateOrderFromCart(CreateOrderDto dto)
        {
            var userId = GetUserId();
            
            try
            {
                var orderModel = dto.ToModel();
                var order = await _orderService.CreateOrderFromCartAsync(userId, orderModel);
                return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order.ToDto());
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/status")]
        [Authorize] // In a real app, this would be admin-only
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
