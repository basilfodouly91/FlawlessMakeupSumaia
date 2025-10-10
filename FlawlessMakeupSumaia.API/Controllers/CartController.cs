using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FlawlessMakeupSumaia.API.Services;
using FlawlessMakeupSumaia.API.DTOs;

namespace FlawlessMakeupSumaia.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException();
        }

        [HttpGet]
        public async Task<ActionResult<CartDto>> GetCart()
        {
            var userId = GetUserId();
            var cart = await _cartService.GetCartByUserIdAsync(userId);
            
            if (cart == null)
            {
                return Ok(new CartDto { UserId = userId });
            }

            return Ok(cart.ToDto());
        }

        [HttpPost("add")]
        public async Task<ActionResult<CartDto>> AddToCart(AddToCartDto dto)
        {
            var userId = GetUserId();
            
            try
            {
                var cart = await _cartService.AddToCartAsync(userId, dto.ProductId, dto.Quantity);
                return Ok(cart.ToDto());
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<ActionResult<CartDto>> UpdateCartItem(UpdateCartItemDto dto)
        {
            var userId = GetUserId();
            
            try
            {
                var cart = await _cartService.UpdateCartItemAsync(userId, dto.ProductId, dto.Quantity);
                return Ok(cart.ToDto());
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("remove/{productId}")]
        public async Task<ActionResult<CartDto>> RemoveFromCart(int productId)
        {
            var userId = GetUserId();
            
            try
            {
                var cart = await _cartService.RemoveFromCartAsync(userId, productId);
                return Ok(cart.ToDto());
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserId();
            var result = await _cartService.ClearCartAsync(userId);
            
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> GetCartItemCount()
        {
            var userId = GetUserId();
            var count = await _cartService.GetCartItemCountAsync(userId);
            return Ok(count);
        }
    }
}
