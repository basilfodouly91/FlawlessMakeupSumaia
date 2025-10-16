namespace FlawlessMakeupSumaia.API.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? SalePrice { get; set; }
        public int StockQuantity { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public List<string> ImageUrls { get; set; } = new List<string>();
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsOnSale { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public string? Brand { get; set; }
        public string? Shade { get; set; }
        public string? Size { get; set; }
        public string? Ingredients { get; set; }
        public string? SkinType { get; set; }
        public List<ProductShadeDto> ProductShades { get; set; } = new List<ProductShadeDto>();
        public decimal CurrentPrice => SalePrice ?? Price;
        public bool HasDiscount => SalePrice.HasValue && SalePrice < Price;
        public decimal DiscountPercentage => HasDiscount ? Math.Round((1 - (SalePrice!.Value / Price)) * 100, 0) : 0;
    }

    public class CreateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? SalePrice { get; set; }
        public int StockQuantity { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public List<string> ImageUrls { get; set; } = new List<string>();
        public int CategoryId { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsOnSale { get; set; }
        public string? Brand { get; set; }
        public string? Shade { get; set; }
        public string? Size { get; set; }
        public string? Ingredients { get; set; }
        public string? SkinType { get; set; }
        public List<CreateProductShadeDto>? ProductShades { get; set; }
    }

    public class UpdateProductDto : CreateProductDto
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
    }
}
