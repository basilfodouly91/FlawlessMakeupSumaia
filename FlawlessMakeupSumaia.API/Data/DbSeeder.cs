using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using FlawlessMakeupSumaia.API.Models;

namespace FlawlessMakeupSumaia.API.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Check if admin user already exists
            var adminUser = await userManager.FindByEmailAsync("admin@flawlessmakeup.com");
            if (adminUser == null)
            {
                // Create admin user
                adminUser = new ApplicationUser
                {
                    UserName = "admin@flawlessmakeup.com",
                    Email = "admin@flawlessmakeup.com",
                    FirstName = "Admin",
                    LastName = "User",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    // Add admin role
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Check if categories already exist (but still allow user creation)
            var categoriesExist = await context.Categories.AnyAsync();

            // Seed Categories (matching the reference site)
            var categories = new List<Category>
            {
                new Category
                {
                    NameEn = "MakeUp",
                    NameAr = "مكياج",
                    Description = "Complete makeup collection including foundations, lipsticks, eyeshadows and more",
                    ImageUrl = "https://images.unsplash.com/photo-1512496015851-a90fb38ba796?w=400",
                    DisplayOrder = 1,
                    IsActive = true
                },
                new Category
                {
                    NameEn = "Skin Care",
                    NameAr = "العناية بالبشرة",
                    Description = "Premium skincare products for all skin types",
                    ImageUrl = "https://images.unsplash.com/photo-1556228720-195a672e8a03?w=400",
                    DisplayOrder = 2,
                    IsActive = true
                },
                new Category
                {
                    NameEn = "Fragrance",
                    NameAr = "العطور",
                    Description = "Luxury fragrances and perfumes",
                    ImageUrl = "https://images.unsplash.com/photo-1541643600914-78b084683601?w=400",
                    DisplayOrder = 3,
                    IsActive = true
                },
                new Category
                {
                    NameEn = "Hair Care",
                    NameAr = "العناية بالشعر",
                    Description = "Professional hair care products",
                    ImageUrl = "https://images.unsplash.com/photo-1522338242992-e1a54906a8da?w=400",
                    DisplayOrder = 4,
                    IsActive = true
                },
                new Category
                {
                    NameEn = "Body Care",
                    NameAr = "العناية بالجسم",
                    Description = "Nourishing body care essentials",
                    ImageUrl = "https://images.unsplash.com/photo-1608248597279-f99d160bfcbc?w=400",
                    DisplayOrder = 5,
                    IsActive = true
                },
                new Category
                {
                    NameEn = "Lash/Brow Care",
                    NameAr = "العناية بالرموش والحواجب",
                    Description = "Enhance your lashes and brows",
                    ImageUrl = "https://images.unsplash.com/photo-1631214524020-7e18db9a8f92?w=400",
                    DisplayOrder = 6,
                    IsActive = true
                },
                new Category
                {
                    NameEn = "Teeth care",
                    NameAr = "العناية بالأسنان",
                    Description = "Oral care and teeth whitening products",
                    ImageUrl = "https://images.unsplash.com/photo-1606811971618-4486d14f3f99?w=400",
                    DisplayOrder = 7,
                    IsActive = true
                },
                new Category
                {
                    NameEn = "Trendy Original products",
                    NameAr = "منتجات أصلية رائجة",
                    Description = "Latest trending beauty products",
                    ImageUrl = "https://images.unsplash.com/photo-1596462502278-27bfdc403348?w=400",
                    DisplayOrder = 8,
                    IsActive = true
                }
            };

            if (!categoriesExist)
            {
                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }

            // Seed Products (based on the reference site)
            var products = new List<Product>
            {
                // MakeUp Category Products
                new Product
                {
                    Name = "Anastasia Brow Gel",
                    Description = "Long-lasting, waterproof brow gel for perfectly defined eyebrows",
                    Price = 21.000m,
                    StockQuantity = 50,
                    ImageUrl = "https://images.unsplash.com/photo-1631214540242-6b1e5b3c8e9b?ixlib=rb-4.0.3&auto=format&fit=crop&w=400&h=400&q=80",
                    CategoryId = 1, // MakeUp
                    Brand = "Anastasia Beverly Hills",
                    IsFeatured = true,
                    IsActive = true
                },
                new Product
                {
                    Name = "Benefit BADgal BANG! Volumizing Mascara",
                    Description = "Volumizing mascara for dramatic lashes",
                    Price = 13.000m,
                    StockQuantity = 75,
                    ImageUrl = "https://images.unsplash.com/photo-1586495777744-4413f21062fa?ixlib=rb-4.0.3&auto=format&fit=crop&w=400&h=400&q=80",
                    CategoryId = 1, // MakeUp
                    Brand = "Benefit",
                    IsFeatured = true,
                    IsActive = true
                },
                new Product
                {
                    Name = "Huda Beauty Easy Bake Blurring Loose Powder",
                    Description = "Blurring loose powder for a flawless finish",
                    Price = 20.000m,
                    SalePrice = 25.000m,
                    StockQuantity = 30,
                    ImageUrl = "https://images.unsplash.com/photo-1522335789203-aabd1fc54bc9?ixlib=rb-4.0.3&auto=format&fit=crop&w=400&h=400&q=80",
                    CategoryId = 1, // MakeUp
                    Brand = "Huda Beauty",
                    IsOnSale = true,
                    IsActive = true
                },
                new Product
                {
                    Name = "Elf Camo Liquid Blush",
                    Description = "Highly pigmented liquid blush for a natural flush",
                    Price = 7.000m,
                    SalePrice = 10.000m,
                    StockQuantity = 60,
                    ImageUrl = "https://images.unsplash.com/photo-1583241800698-9c2e0c2e9e0a?ixlib=rb-4.0.3&auto=format&fit=crop&w=400&h=400&q=80",
                    CategoryId = 1, // MakeUp
                    Brand = "e.l.f.",
                    IsOnSale = true,
                    IsActive = true
                },
                new Product
                {
                    Name = "NYX Buttermelt Pressed Powder Natural Finish Bronzer",
                    Description = "Natural finish bronzer for a sun-kissed glow",
                    Price = 8.000m,
                    SalePrice = 13.000m,
                    StockQuantity = 40,
                    ImageUrl = "https://images.unsplash.com/photo-1512496015851-a90fb38ba796?ixlib=rb-4.0.3&auto=format&fit=crop&w=400&h=400&q=80",
                    CategoryId = 1, // MakeUp
                    Brand = "NYX Professional Makeup",
                    IsOnSale = true,
                    IsActive = true
                },
                new Product
                {
                    Name = "Nars Powder Blush",
                    Description = "Iconic powder blush for a natural flush of color",
                    Price = 20.000m,
                    SalePrice = 28.000m,
                    StockQuantity = 25,
                    ImageUrl = "https://images.unsplash.com/photo-1571019613454-1cb2f99b2d8b?ixlib=rb-4.0.3&auto=format&fit=crop&w=400&h=400&q=80",
                    CategoryId = 1, // MakeUp
                    Brand = "NARS",
                    IsOnSale = true,
                    IsActive = true
                },
                new Product
                {
                    Name = "Charlotte Tilbury Beautiful Skin Medium Coverage Foundation",
                    Description = "Medium coverage foundation for beautiful, natural-looking skin",
                    Price = 30.000m,
                    SalePrice = 39.000m,
                    StockQuantity = 35,
                    ImageUrl = "https://images.unsplash.com/photo-1596462502278-27bfdc403348?ixlib=rb-4.0.3&auto=format&fit=crop&w=400&h=400&q=80",
                    CategoryId = 1, // MakeUp
                    Brand = "Charlotte Tilbury",
                    IsOnSale = true,
                    IsActive = true
                },
                new Product
                {
                    Name = "Too Faced Born This Way Mini Eyeshadow Palettes",
                    Description = "Mini eyeshadow palette with versatile shades",
                    Price = 15.000m,
                    SalePrice = 21.000m,
                    StockQuantity = 45,
                    ImageUrl = "https://images.unsplash.com/photo-1487412947147-5cebf100ffc2?ixlib=rb-4.0.3&auto=format&fit=crop&w=400&h=400&q=80",
                    CategoryId = 1, // MakeUp
                    Brand = "Too Faced",
                    IsOnSale = true,
                    IsActive = true
                },
                new Product
                {
                    Name = "NYX Jumbo Lash! False Eyelashes",
                    Description = "Dramatic false eyelashes for bold eye looks",
                    Price = 8.000m,
                    SalePrice = 13.000m,
                    StockQuantity = 80,
                    ImageUrl = "https://images.unsplash.com/photo-1583241800698-9c2e0c2e9e0a?ixlib=rb-4.0.3&auto=format&fit=crop&w=400&h=400&q=80",
                    CategoryId = 1, // MakeUp
                    Brand = "NYX Professional Makeup",
                    IsOnSale = true,
                    IsActive = true
                },

                // Skin Care Category Products
                new Product
                {
                    Name = "Anua Niacinamide 10 + TXA 4 Serum",
                    Description = "Brightening serum with niacinamide and tranexamic acid",
                    Price = 24.000m,
                    StockQuantity = 40,
                    ImageUrl = "https://images.unsplash.com/photo-1620916566398-39f1143ab7be?ixlib=rb-4.0.3&auto=format&fit=crop&w=400&h=400&q=80",
                    CategoryId = 2, // Skin Care
                    Brand = "Anua",
                    IsFeatured = true,
                    IsActive = true
                },
                new Product
                {
                    Name = "Beauty of Joseon Glow Serum, Propolis + Niacinamide",
                    Description = "Glow serum with propolis and niacinamide for radiant skin",
                    Price = 16.000m,
                    StockQuantity = 0, // Sold out
                    ImageUrl = "https://images.unsplash.com/photo-1556228578-8c89e6adf883?ixlib=rb-4.0.3&auto=format&fit=crop&w=400&h=400&q=80",
                    CategoryId = 2, // Skin Care
                    Brand = "Beauty of Joseon",
                    IsFeatured = true,
                    IsActive = true
                },
                new Product
                {
                    Name = "Beauty of Joseon Matte Sun Stick",
                    Description = "Convenient matte sun stick for on-the-go sun protection",
                    Price = 15.000m,
                    StockQuantity = 55,
                    ImageUrl = "https://images.unsplash.com/photo-1571019613454-1cb2f99b2d8b?ixlib=rb-4.0.3&auto=format&fit=crop&w=400&h=400&q=80",
                    CategoryId = 2, // Skin Care
                    Brand = "Beauty of Joseon",
                    IsFeatured = true,
                    IsActive = true
                },
                new Product
                {
                    Name = "Beauty of Joseon Relief Sun: Rice + Probiotics",
                    Description = "Gentle sunscreen with rice and probiotics",
                    Price = 16.000m,
                    StockQuantity = 65,
                    ImageUrl = "https://images.unsplash.com/photo-1598300042247-d088f8ab3a91?ixlib=rb-4.0.3&auto=format&fit=crop&w=400&h=400&q=80",
                    CategoryId = 2, // Skin Care
                    Brand = "Beauty of Joseon",
                    IsFeatured = true,
                    IsActive = true
                },
                new Product
                {
                    Name = "Beauty of Joseon Revive Eye Serum",
                    Description = "Revitalizing eye serum for the delicate eye area",
                    Price = 14.000m,
                    StockQuantity = 30,
                    ImageUrl = "https://images.unsplash.com/photo-1620916566398-39f1143ab7be?ixlib=rb-4.0.3&auto=format&fit=crop&w=400&h=400&q=80",
                    CategoryId = 2, // Skin Care
                    Brand = "Beauty of Joseon",
                    IsFeatured = true,
                    IsActive = true
                },

                // Lash/Brow Care Category Products
                new Product
                {
                    Name = "Babe Essential Lash Serum",
                    Description = "Nourishing lash serum for longer, stronger lashes",
                    Price = 30.000m,
                    StockQuantity = 20,
                    ImageUrl = "https://images.unsplash.com/photo-1631214540242-6b1e5b3c8e9b?ixlib=rb-4.0.3&auto=format&fit=crop&w=400&h=400&q=80",
                    CategoryId = 6, // Lash/Brow Care
                    Brand = "Babe",
                    IsFeatured = true,
                    IsActive = true
                },

                // Body Care Category Products
                new Product
                {
                    Name = "Summer Fridays Lip Butter Balm",
                    Description = "Nourishing lip balm for soft, hydrated lips",
                    Price = 12.000m,
                    SalePrice = 23.000m,
                    StockQuantity = 70,
                    ImageUrl = "https://images.unsplash.com/photo-1570194065650-d99fb4bedf0a?ixlib=rb-4.0.3&auto=format&fit=crop&w=400&h=400&q=80",
                    CategoryId = 5, // Body Care
                    Brand = "Summer Fridays",
                    IsOnSale = true,
                    IsActive = true
                }
            };

            // Check if products already exist
            var productsExist = await context.Products.AnyAsync();
            
            if (!productsExist)
            {
                await context.Products.AddRangeAsync(products);
                await context.SaveChangesAsync();
            }
        }
    }
}
