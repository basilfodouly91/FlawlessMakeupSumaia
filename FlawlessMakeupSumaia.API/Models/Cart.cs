namespace FlawlessMakeupSumaia.API.Models
{
    public class Cart
    {
        public int Id { get; set; }
        
        public string UserId { get; set; } = string.Empty;
        
        public virtual ApplicationUser User { get; set; } = null!;
        
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        
        public DateTime DateUpdated { get; set; } = DateTime.UtcNow;
        
        public decimal TotalAmount => CartItems.Sum(item => item.Quantity * item.Price);
        
        public int TotalItems => CartItems.Sum(item => item.Quantity);
    }
    
    public class CartItem
    {
        public int Id { get; set; }
        
        public int CartId { get; set; }
        
        public virtual Cart Cart { get; set; } = null!;
        
        public int ProductId { get; set; }
        
        public virtual Product Product { get; set; } = null!;
        
        public int Quantity { get; set; }
        
        public decimal Price { get; set; }
        
        public int? ProductShadeId { get; set; }
        
        public virtual ProductShade? ProductShade { get; set; }
        
        public DateTime DateAdded { get; set; } = DateTime.UtcNow;
    }
}
