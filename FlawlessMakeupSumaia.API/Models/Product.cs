using System.ComponentModel.DataAnnotations;

namespace FlawlessMakeupSumaia.API.Models
{
    public class Product
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
        
        public decimal? SalePrice { get; set; }
        
        [Required]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }
        
        public string ImageUrl { get; set; } = string.Empty;
        
        public List<string> ImageUrls { get; set; } = new List<string>();
        
        [Required]
        public int CategoryId { get; set; }
        
        public virtual Category Category { get; set; } = null!;
        
        public bool IsActive { get; set; } = true;
        
        public bool IsFeatured { get; set; } = false;
        
        public bool IsOnSale { get; set; } = false;
        
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        
        public DateTime DateUpdated { get; set; } = DateTime.UtcNow;
        
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        
        public virtual ICollection<ProductShade> ProductShades { get; set; } = new List<ProductShade>();
        
        // Beauty-specific properties
        public string? Brand { get; set; }
        public string? Shade { get; set; }
        public string? Size { get; set; }
        public string? Ingredients { get; set; }
        public string? SkinType { get; set; }
    }
}
