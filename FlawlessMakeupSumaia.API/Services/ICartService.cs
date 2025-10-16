using FlawlessMakeupSumaia.API.Models;

namespace FlawlessMakeupSumaia.API.Services
{
    public interface ICartService
    {
        Task<Cart?> GetCartByUserIdAsync(string userId);
        Task<Cart> AddToCartAsync(string userId, int productId, int quantity, int? productShadeId = null);
        Task<Cart> UpdateCartItemAsync(string userId, int productId, int quantity);
        Task<Cart> RemoveFromCartAsync(string userId, int productId);
        Task<bool> ClearCartAsync(string userId);
        Task<int> GetCartItemCountAsync(string userId);
    }
}
