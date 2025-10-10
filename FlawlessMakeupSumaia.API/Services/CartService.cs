using Microsoft.EntityFrameworkCore;
using FlawlessMakeupSumaia.API.Data;
using FlawlessMakeupSumaia.API.Models;

namespace FlawlessMakeupSumaia.API.Services
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;

        public CartService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Cart?> GetCartByUserIdAsync(string userId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Cart> AddToCartAsync(string userId, int productId, int quantity)
        {
            var cart = await GetCartByUserIdAsync(userId);
            
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                throw new ArgumentException("Product not found");

            var existingCartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            
            if (existingCartItem != null)
            {
                existingCartItem.Quantity += quantity;
                existingCartItem.Price = product.SalePrice ?? product.Price;
            }
            else
            {
                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = quantity,
                    Price = product.SalePrice ?? product.Price,
                    DateAdded = DateTime.UtcNow
                };
                cart.CartItems.Add(cartItem);
            }

            cart.DateUpdated = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            
            return await GetCartByUserIdAsync(userId) ?? cart;
        }

        public async Task<Cart> UpdateCartItemAsync(string userId, int productId, int quantity)
        {
            var cart = await GetCartByUserIdAsync(userId);
            if (cart == null)
                throw new ArgumentException("Cart not found");

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (cartItem == null)
                throw new ArgumentException("Cart item not found");

            if (quantity <= 0)
            {
                cart.CartItems.Remove(cartItem);
            }
            else
            {
                cartItem.Quantity = quantity;
            }

            cart.DateUpdated = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            
            return await GetCartByUserIdAsync(userId) ?? cart;
        }

        public async Task<Cart> RemoveFromCartAsync(string userId, int productId)
        {
            var cart = await GetCartByUserIdAsync(userId);
            if (cart == null)
                throw new ArgumentException("Cart not found");

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (cartItem != null)
            {
                cart.CartItems.Remove(cartItem);
                cart.DateUpdated = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            return await GetCartByUserIdAsync(userId) ?? cart;
        }

        public async Task<bool> ClearCartAsync(string userId)
        {
            var cart = await GetCartByUserIdAsync(userId);
            if (cart == null) return false;

            cart.CartItems.Clear();
            cart.DateUpdated = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            
            return true;
        }

        public async Task<int> GetCartItemCountAsync(string userId)
        {
            var cart = await GetCartByUserIdAsync(userId);
            return cart?.TotalItems ?? 0;
        }
    }
}
