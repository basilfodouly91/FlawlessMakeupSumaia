using FlawlessMakeupSumaia.API.Models;

namespace FlawlessMakeupSumaia.API.Services
{
    public interface IProductShadeService
    {
        Task<IEnumerable<ProductShade>> GetShadesByProductIdAsync(int productId);
        Task<ProductShade?> GetShadeByIdAsync(int id);
        Task<ProductShade> CreateShadeAsync(int productId, ProductShade shade);
        Task<ProductShade> UpdateShadeAsync(ProductShade shade);
        Task<bool> DeleteShadeAsync(int id);
    }
}












