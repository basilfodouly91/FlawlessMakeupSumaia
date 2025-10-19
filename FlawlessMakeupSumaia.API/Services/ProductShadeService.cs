using Microsoft.EntityFrameworkCore;
using FlawlessMakeupSumaia.API.Data;
using FlawlessMakeupSumaia.API.Models;

namespace FlawlessMakeupSumaia.API.Services
{
    public class ProductShadeService : IProductShadeService
    {
        private readonly ApplicationDbContext _context;

        public ProductShadeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductShade>> GetShadesByProductIdAsync(int productId)
        {
            return await _context.ProductShades
                .Where(ps => ps.ProductId == productId)
                .OrderBy(ps => ps.DisplayOrder)
                .ThenBy(ps => ps.Name)
                .ToListAsync();
        }

        public async Task<ProductShade?> GetShadeByIdAsync(int id)
        {
            return await _context.ProductShades.FindAsync(id);
        }

        public async Task<ProductShade> CreateShadeAsync(int productId, ProductShade shade)
        {
            shade.ProductId = productId;
            shade.DateCreated = DateTime.UtcNow;
            shade.DateUpdated = DateTime.UtcNow;

            _context.ProductShades.Add(shade);
            await _context.SaveChangesAsync();

            return shade;
        }

        public async Task<ProductShade> UpdateShadeAsync(ProductShade shade)
        {
            var existingShade = await _context.ProductShades.FindAsync(shade.Id);
            if (existingShade == null)
                throw new ArgumentException("Shade not found");

            existingShade.Name = shade.Name;
            existingShade.StockQuantity = shade.StockQuantity;
            existingShade.IsActive = shade.IsActive;
            existingShade.DisplayOrder = shade.DisplayOrder;
            existingShade.DateUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingShade;
        }

        public async Task<bool> DeleteShadeAsync(int id)
        {
            var shade = await _context.ProductShades.FindAsync(id);
            if (shade == null)
                return false;

            _context.ProductShades.Remove(shade);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}












