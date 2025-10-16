using System.ComponentModel.DataAnnotations;

namespace FlawlessMakeupSumaia.API.Models
{
    public class ProductShade
    {
        public int Id { get; set; }
        
        [Required]
        public int ProductId { get; set; }
        
        public virtual Product Product { get; set; } = null!;
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public int DisplayOrder { get; set; }
        
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        
        public DateTime DateUpdated { get; set; } = DateTime.UtcNow;
    }
}











