using System.ComponentModel.DataAnnotations;

namespace FlawlessMakeupSumaia.API.Models
{
    public class Order
    {
        public int Id { get; set; }
        
        public string UserId { get; set; } = string.Empty;
        
        public virtual ApplicationUser User { get; set; } = null!;
        
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
        
        public int Quantity { get; set; }
        
        public decimal UnitPrice { get; set; }
        
        public decimal TotalPrice => Quantity * UnitPrice;
        
        public string ProductName { get; set; } = string.Empty;
        
        public string ProductImageUrl { get; set; } = string.Empty;
    }
    
    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled,
        Refunded
    }
}
