namespace FlawlessMakeupSumaia.API.DTOs
{
    public class CartDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public List<CartItemDto> CartItems { get; set; } = new List<CartItemDto>();
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public decimal TotalAmount { get; set; }
        public int TotalItems { get; set; }
    }

    public class CartItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImageUrl { get; set; } = string.Empty;
        public string ProductBrand { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int? ProductShadeId { get; set; }
        public string? ProductShadeName { get; set; }
        public decimal TotalPrice => Quantity * Price;
        public DateTime DateAdded { get; set; }
        public bool IsInStock { get; set; }
        public int StockQuantity { get; set; }
    }

    public class AddToCartDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
        public int? ProductShadeId { get; set; }
    }

    public class UpdateCartItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
