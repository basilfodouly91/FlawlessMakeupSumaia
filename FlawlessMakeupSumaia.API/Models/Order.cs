using System.ComponentModel.DataAnnotations;

namespace FlawlessMakeupSumaia.API.Models
{
    public class Order
    {
        public int Id { get; set; }
        
        // Nullable for guest orders
        public string? UserId { get; set; }
        
        public virtual ApplicationUser? User { get; set; }
        
        // Guest order information
        public string? GuestEmail { get; set; }
        public string? GuestName { get; set; }
        
        public string OrderNumber { get; set; } = string.Empty;
        
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        
        public decimal SubTotal { get; set; }
        
        public decimal Tax { get; set; }
        
        public decimal ShippingCost { get; set; }
        
        public decimal TotalAmount { get; set; }
        
        // Shipping Address
        [Required]
        public string ShippingFirstName { get; set; } = string.Empty;
        
        [Required]
        public string ShippingLastName { get; set; } = string.Empty;
        
        [Required]
        public string ShippingAddress { get; set; } = string.Empty;
        
        public string ShippingAddress2 { get; set; } = string.Empty;
        
        [Required]
        public string ShippingCity { get; set; } = string.Empty;
        
        [Required]
        public string ShippingState { get; set; } = string.Empty;
        
        [Required]
        public string ShippingZipCode { get; set; } = string.Empty;
        
        [Required]
        public string ShippingCountry { get; set; } = string.Empty;
        
        public string ShippingPhone { get; set; } = string.Empty;
        
        // Payment Information
        public string PaymentMethod { get; set; } = string.Empty;
        
        public string PaymentTransactionId { get; set; } = string.Empty;
        
        public string? PaymentProofImageUrl { get; set; }
        
        public DateTime? PaymentDate { get; set; }
        
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        
        public string Notes { get; set; } = string.Empty;
    }
    
    public class OrderItem
    {
        public int Id { get; set; }
        
        public int OrderId { get; set; }
        
        public virtual Order Order { get; set; } = null!;
        
        public int ProductId { get; set; }
        
        public virtual Product Product { get; set; } = null!;
        
        public int? ProductShadeId { get; set; }
        
        public virtual ProductShade? ProductShade { get; set; }
        
        public int Quantity { get; set; }
        
        public decimal UnitPrice { get; set; }
        
        public decimal TotalPrice => Quantity * UnitPrice;
        
        public string ProductName { get; set; } = string.Empty;
        
        public string ProductImageUrl { get; set; } = string.Empty;
        
        public string? ProductShadeName { get; set; }
    }
    
    public enum OrderStatus
    {
        Pending = 0,
        Confirmed = 1,
        Completed = 2,
        Cancelled = 3
    }
}
