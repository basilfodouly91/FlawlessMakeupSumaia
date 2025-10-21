using FlawlessMakeupSumaia.API.Models;

namespace FlawlessMakeupSumaia.API.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        
        // Guest order information
        public string? GuestEmail { get; set; }
        public string? GuestName { get; set; }
        
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal TotalAmount { get; set; }
        
        // Shipping Address
        public string ShippingFirstName { get; set; } = string.Empty;
        public string ShippingLastName { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string ShippingAddress2 { get; set; } = string.Empty;
        public string ShippingCity { get; set; } = string.Empty;
        public string ShippingState { get; set; } = string.Empty;
        public string ShippingZipCode { get; set; } = string.Empty;
        public string ShippingCountry { get; set; } = string.Empty;
        public string ShippingPhone { get; set; } = string.Empty;
        
        // Payment Information
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentTransactionId { get; set; } = string.Empty;
        public string? PaymentProofImageUrl { get; set; }
        public DateTime? PaymentDate { get; set; }
        
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
        public string Notes { get; set; } = string.Empty;
        
        public string FullShippingAddress => $"{ShippingAddress}, {ShippingCity}, {ShippingState} {ShippingZipCode}, {ShippingCountry}";
        public string CustomerName => $"{ShippingFirstName} {ShippingLastName}";
    }

    public class OrderItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int? ProductShadeId { get; set; }
        public string? ProductShadeName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImageUrl { get; set; } = string.Empty;
    }

    public class CreateOrderDto
    {
        // Guest order information (optional)
        public string? GuestEmail { get; set; }
        public string? GuestName { get; set; }
        
        // Guest cart items (for guest checkout)
        public List<GuestCartItemDto>? GuestCartItems { get; set; }
        
        // Shipping Address
        public string ShippingFirstName { get; set; } = string.Empty;
        public string ShippingLastName { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string ShippingAddress2 { get; set; } = string.Empty;
        public string ShippingCity { get; set; } = string.Empty;
        public string ShippingState { get; set; } = string.Empty;
        public string ShippingZipCode { get; set; } = string.Empty;
        public string ShippingCountry { get; set; } = string.Empty;
        public string ShippingPhone { get; set; } = string.Empty;
        
        // Payment Information
        public string PaymentMethod { get; set; } = string.Empty;
        public string? PaymentProofImageUrl { get; set; }
        public string Notes { get; set; } = string.Empty;
    }

    public class GuestCartItemDto
    {
        public int ProductId { get; set; }
        public int? ProductShadeId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class UpdateOrderStatusDto
    {
        public OrderStatus Status { get; set; }
    }
}
