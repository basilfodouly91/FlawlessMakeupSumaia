using FlawlessMakeupSumaia.API.Models;
using FlawlessMakeupSumaia.API.DTOs;

namespace FlawlessMakeupSumaia.API.Services
{
    public static class MappingService
    {
        public static ProductDto ToDto(this Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                SalePrice = product.SalePrice,
                StockQuantity = product.StockQuantity,
                ImageUrl = product.ImageUrl,
                ImageUrls = product.ImageUrls,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.NameEn ?? string.Empty,
                IsActive = product.IsActive,
                IsFeatured = product.IsFeatured,
                IsOnSale = product.IsOnSale,
                DateCreated = product.DateCreated,
                DateUpdated = product.DateUpdated,
                Brand = product.Brand,
                Shade = product.Shade,
                Size = product.Size,
                Ingredients = product.Ingredients,
                SkinType = product.SkinType,
                ProductShades = product.ProductShades.Select(ps => ps.ToDto()).ToList()
            };
        }

        public static Product ToModel(this CreateProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                SalePrice = dto.SalePrice,
                StockQuantity = dto.StockQuantity,
                ImageUrl = dto.ImageUrl,
                ImageUrls = dto.ImageUrls,
                CategoryId = dto.CategoryId,
                IsFeatured = dto.IsFeatured,
                IsOnSale = dto.IsOnSale,
                Brand = dto.Brand,
                Shade = dto.Shade,
                Size = dto.Size,
                Ingredients = dto.Ingredients,
                SkinType = dto.SkinType
            };

            // Map ProductShades if provided
            if (dto.ProductShades != null && dto.ProductShades.Count > 0)
            {
                product.ProductShades = dto.ProductShades
                    .Select(shadeDto => new ProductShade
                    {
                        Name = shadeDto.Name,
                        StockQuantity = shadeDto.StockQuantity,
                        IsActive = shadeDto.IsActive,
                        DisplayOrder = shadeDto.DisplayOrder,
                        DateCreated = DateTime.UtcNow,
                        DateUpdated = DateTime.UtcNow
                    })
                    .ToList();
            }

            return product;
        }

        public static CategoryDto ToDto(this Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                NameEn = category.NameEn,
                NameAr = category.NameAr,
                Description = category.Description,
                ImageUrl = category.ImageUrl,
                IsActive = category.IsActive,
                DisplayOrder = category.DisplayOrder,
                DateCreated = category.DateCreated,
                ProductCount = category.Products?.Count ?? 0
            };
        }

        public static Category ToModel(this CreateCategoryDto dto)
        {
            return new Category
            {
                NameEn = dto.NameEn,
                NameAr = dto.NameAr,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                DisplayOrder = dto.DisplayOrder,
                IsActive = dto.IsActive
            };
        }

        public static CartDto ToDto(this Cart cart)
        {
            return new CartDto
            {
                Id = cart.Id,
                UserId = cart.UserId,
                CartItems = cart.CartItems.Select(ci => ci.ToDto()).ToList(),
                DateCreated = cart.DateCreated,
                DateUpdated = cart.DateUpdated,
                TotalAmount = cart.TotalAmount,
                TotalItems = cart.TotalItems
            };
        }

        public static CartItemDto ToDto(this CartItem cartItem)
        {
            return new CartItemDto
            {
                Id = cartItem.Id,
                ProductId = cartItem.ProductId,
                ProductName = cartItem.Product?.Name ?? string.Empty,
                ProductImageUrl = cartItem.Product?.ImageUrl ?? string.Empty,
                ProductBrand = cartItem.Product?.Brand ?? string.Empty,
                Quantity = cartItem.Quantity,
                Price = cartItem.Price,
                ProductShadeId = cartItem.ProductShadeId,
                ProductShadeName = cartItem.ProductShade?.Name,
                DateAdded = cartItem.DateAdded,
                IsInStock = cartItem.ProductShadeId.HasValue 
                    ? (cartItem.ProductShade?.StockQuantity ?? 0) >= cartItem.Quantity
                    : (cartItem.Product?.StockQuantity ?? 0) >= cartItem.Quantity,
                StockQuantity = cartItem.ProductShadeId.HasValue
                    ? (cartItem.ProductShade?.StockQuantity ?? 0)
                    : (cartItem.Product?.StockQuantity ?? 0)
            };
        }

        public static ProductShadeDto ToDto(this ProductShade shade)
        {
            return new ProductShadeDto
            {
                Id = shade.Id,
                ProductId = shade.ProductId,
                Name = shade.Name,
                StockQuantity = shade.StockQuantity,
                IsActive = shade.IsActive,
                DisplayOrder = shade.DisplayOrder,
                DateCreated = shade.DateCreated,
                DateUpdated = shade.DateUpdated
            };
        }

        public static ProductShade ToModel(this CreateProductShadeDto dto, int productId)
        {
            return new ProductShade
            {
                ProductId = productId,
                Name = dto.Name,
                StockQuantity = dto.StockQuantity,
                IsActive = dto.IsActive,
                DisplayOrder = dto.DisplayOrder
            };
        }

        public static OrderDto ToDto(this Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                GuestEmail = order.GuestEmail,
                GuestName = order.GuestName,
                OrderNumber = order.OrderNumber,
                OrderDate = order.OrderDate,
                Status = order.Status,
                SubTotal = order.SubTotal,
                Tax = order.Tax,
                ShippingCost = order.ShippingCost,
                TotalAmount = order.TotalAmount,
                ShippingFirstName = order.ShippingFirstName,
                ShippingLastName = order.ShippingLastName,
                ShippingAddress = order.ShippingAddress,
                ShippingAddress2 = order.ShippingAddress2,
                ShippingCity = order.ShippingCity,
                ShippingState = order.ShippingState,
                ShippingZipCode = order.ShippingZipCode,
                ShippingCountry = order.ShippingCountry,
                ShippingPhone = order.ShippingPhone,
                PaymentMethod = order.PaymentMethod,
                PaymentTransactionId = order.PaymentTransactionId,
                PaymentProofImageUrl = order.PaymentProofImageUrl,
                PaymentDate = order.PaymentDate,
                OrderItems = order.OrderItems.Select(oi => oi.ToDto()).ToList(),
                Notes = order.Notes
            };
        }

        public static OrderItemDto ToDto(this OrderItem orderItem)
        {
            return new OrderItemDto
            {
                Id = orderItem.Id,
                ProductId = orderItem.ProductId,
                ProductShadeId = orderItem.ProductShadeId,
                ProductShadeName = orderItem.ProductShadeName,
                Quantity = orderItem.Quantity,
                UnitPrice = orderItem.UnitPrice,
                TotalPrice = orderItem.TotalPrice,
                ProductName = orderItem.ProductName,
                ProductImageUrl = orderItem.ProductImageUrl
            };
        }

        public static Order ToModel(this CreateOrderDto dto)
        {
            return new Order
            {
                GuestEmail = dto.GuestEmail,
                GuestName = dto.GuestName,
                ShippingFirstName = dto.ShippingFirstName,
                ShippingLastName = dto.ShippingLastName,
                ShippingAddress = dto.ShippingAddress,
                ShippingAddress2 = dto.ShippingAddress2,
                ShippingCity = dto.ShippingCity,
                ShippingState = dto.ShippingState,
                ShippingZipCode = dto.ShippingZipCode,
                ShippingCountry = dto.ShippingCountry,
                ShippingPhone = dto.ShippingPhone,
                PaymentMethod = dto.PaymentMethod,
                PaymentProofImageUrl = dto.PaymentProofImageUrl,
                Notes = dto.Notes
            };
        }

        public static UserDto ToDto(this ApplicationUser user, List<string>? roles = null)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateCreated = user.DateCreated,
                Roles = roles ?? new List<string>()
            };
        }
    }
}
