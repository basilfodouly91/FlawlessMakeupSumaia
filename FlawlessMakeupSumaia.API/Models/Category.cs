using System.ComponentModel.DataAnnotations;

namespace FlawlessMakeupSumaia.API.Models
{
    public class Category
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        public string ImageUrl { get; set; } = string.Empty;
        
        public bool IsActive { get; set; } = true;
        
        public int DisplayOrder { get; set; } = 0;
        
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
