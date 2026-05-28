using Microsoft.AspNetCore.Identity;
using ShopWebApp.Models;

namespace ShopWebApp.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var context = services.GetRequiredService<AppDbContext>();

            // Tạo roles
            string[] roles = { "Admin", "Customer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Tạo tài khoản Admin mặc định
            const string adminEmail = "admin@shopweb.com";
            const string adminPassword = "Admin@123";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Administrator",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }

            // Seed danh mục - đảm bảo tất cả slug cần thiết tồn tại
            var requiredCategories = new[]
            {
                new { Name = "Điện thoại",         Slug = "dien-thoai",         Desc = "Điện thoại thông minh các thương hiệu" },
                new { Name = "Laptop",              Slug = "laptop",              Desc = "Máy tính xách tay cao cấp" },
                new { Name = "Phụ kiện",            Slug = "phu-kien",            Desc = "Phụ kiện điện tử đa dạng" },
                new { Name = "Máy tính bảng",       Slug = "may-tinh-bang",       Desc = "Tablet Android và iPad" },
                new { Name = "Đồng hồ thông minh",  Slug = "dong-ho-thong-minh",  Desc = "Smartwatch và wearable" }
            };

            foreach (var cat in requiredCategories)
            {
                if (!context.Categories.Any(c => c.Slug == cat.Slug))
                {
                    context.Categories.Add(new Category
                    {
                        Name = cat.Name,
                        Slug = cat.Slug,
                        Description = cat.Desc,
                        IsActive = true
                    });
                }
            }
            await context.SaveChangesAsync();

            // Seed sản phẩm mẫu nếu chưa có
            if (!context.Products.Any())
            {
                var cats = context.Categories.ToList();
                int dtId  = cats.First(c => c.Slug == "dien-thoai").Id;
                int lapId = cats.First(c => c.Slug == "laptop").Id;
                int pkId  = cats.First(c => c.Slug == "phu-kien").Id;
                int tabId = cats.First(c => c.Slug == "may-tinh-bang").Id;
                int dhId  = cats.First(c => c.Slug == "dong-ho-thong-minh").Id;

                var products = new List<Product>
                {
                    // ===== Điện thoại =====
                    new Product
                    {
                        Name = "iPhone 15 Pro Max 256GB",
                        Description = "iPhone 15 Pro Max với chip A17 Pro mạnh mẽ, camera 48MP, màn hình Super Retina XDR 6.7 inch. Thiết kế titan sang trọng, pin cả ngày.",
                        Price = 34_990_000,
                        Stock = 25,
                        CategoryId = dtId,
                        IsActive = true,
                        ImageUrl = "https://images.unsplash.com/photo-1695048133142-1a20484d2569?w=400&h=400&fit=crop",
                        CreatedAt = DateTime.UtcNow.AddDays(-30)
                    },
                    new Product
                    {
                        Name = "Samsung Galaxy S24 Ultra 512GB",
                        Description = "Galaxy S24 Ultra với bút S Pen tích hợp, camera 200MP, màn hình Dynamic AMOLED 6.8 inch 120Hz. Hiệu năng Galaxy AI vượt trội.",
                        Price = 31_990_000,
                        Stock = 18,
                        CategoryId = dtId,
                        IsActive = true,
                        ImageUrl = "https://images.unsplash.com/photo-1707818871924-30d0ce30dfa8?w=400&h=400&fit=crop",
                        CreatedAt = DateTime.UtcNow.AddDays(-25)
                    },
                    new Product
                    {
                        Name = "Xiaomi 14 Ultra 512GB",
                        Description = "Xiaomi 14 Ultra tích hợp camera Leica, chip Snapdragon 8 Gen 3, sạc nhanh 90W, màn hình AMOLED 6.73 inch 120Hz.",
                        Price = 22_990_000,
                        Stock = 30,
                        CategoryId = dtId,
                        IsActive = true,
                        ImageUrl = "https://images.unsplash.com/photo-1598327105666-5b89351aff97?w=400&h=400&fit=crop",
                        CreatedAt = DateTime.UtcNow.AddDays(-20)
                    },
                    new Product
                    {
                        Name = "OPPO Find X7 Ultra 256GB",
                        Description = "OPPO Find X7 Ultra với camera Hasselblad tiên tiến, chip Dimensity 9300, pin 5000mAh sạc nhanh 100W.",
                        Price = 19_990_000,
                        Stock = 15,
                        CategoryId = dtId,
                        IsActive = true,
                        ImageUrl = "https://images.unsplash.com/photo-1610945265064-0e34e5519bbf?w=400&h=400&fit=crop",
                        CreatedAt = DateTime.UtcNow.AddDays(-18)
                    },
                    new Product
                    {
                        Name = "Google Pixel 8 Pro 256GB",
                        Description = "Google Pixel 8 Pro với AI thông minh, camera Magic Eraser, chip Tensor G3, Android thuần túy, cập nhật 7 năm.",
                        Price = 24_990_000,
                        Stock = 12,
                        CategoryId = dtId,
                        IsActive = true,
                        ImageUrl = "https://images.unsplash.com/photo-1648737963503-1a26da876aca?w=400&h=400&fit=crop",
                        CreatedAt = DateTime.UtcNow.AddDays(-15)
                    },
                    new Product
                    {
                        Name = "Vivo X100 Pro 256GB",
                        Description = "Vivo X100 Pro với camera ZEISS hợp tác, chip Dimensity 9300, màn hình AMOLED 6.78 inch 120Hz, pin 5400mAh.",
                        Price = 17_990_000,
                        Stock = 20,
                        CategoryId = dtId,
                        IsActive = true,
                        ImageUrl = "https://images.unsplash.com/photo-1573075175787-94e36a0e3e4e?w=400&h=400&fit=crop",
                        CreatedAt = DateTime.UtcNow.AddDays(-10)
                    },

                    // ===== Laptop =====
                    new Product
                    {
                        Name = "MacBook Pro M3 Pro 14 inch",
                        Description = "MacBook Pro với chip M3 Pro 12 nhân, màn hình Liquid Retina XDR 14.2 inch, 18GB RAM, 512GB SSD, pin 18 giờ.",
                        Price = 52_990_000,
                        Stock = 10,
                        CategoryId = lapId,
                        IsActive = true,
                        ImageUrl = "https://images.unsplash.com/photo-1517336714731-489689fd1ca8?w=400&h=400&fit=crop",
                        CreatedAt = DateTime.UtcNow.AddDays(-28)
                    },
                    new Product
                    {
                        Name = "Dell XPS 15 9530 Core i9",
                        Description = "Dell XPS 15 với Core i9-13900H, NVIDIA RTX 4070 8GB, màn hình OLED 15.6 inch 3.5K, 32GB RAM DDR5, 1TB NVMe SSD.",
                        Price = 45_990_000,
                        Stock = 8,
                        CategoryId = lapId,
                        IsActive = true,
                        ImageUrl = "https://images.unsplash.com/photo-1593642632559-0c6d3fc62b89?w=400&h=400&fit=crop",
                        CreatedAt = DateTime.UtcNow.AddDays(-22)
                    },
                    new Product
                    {
                        Name = "ASUS ROG Zephyrus G16 RTX 4080",
                        Description = "Laptop gaming cao cấp với RTX 4080 12GB, Core i9-14900HX, màn hình QHD 240Hz, 32GB DDR5, 2TB SSD.",
                        Price = 62_990_000,
                        Stock = 5,
                        CategoryId = lapId,
                        IsActive = true,
                        ImageUrl = "https://images.unsplash.com/photo-1603302576837-37561b2e2302?w=400&h=400&fit=crop",
                        CreatedAt = DateTime.UtcNow.AddDays(-16)
                    },
                    new Product
                    {
                        Name = "Lenovo ThinkPad X1 Carbon Gen 12",
                        Description = "ThinkPad X1 Carbon doanh nhân siêu mỏng, Core Ultra 7, 32GB LPDDR5x, 1TB SSD, màn hình 14 inch IPS, bàn phím backlit.",
                        Price = 38_990_000,
                        Stock = 7,
                        CategoryId = lapId,
                        IsActive = true,
                        ImageUrl = "https://images.unsplash.com/photo-1525547719571-a2d4ac8945e2?w=400&h=400&fit=crop",
                        CreatedAt = DateTime.UtcNow.AddDays(-12)
                    },
                    new Product
                    {
                        Name = "HP Spectre x360 14 OLED",
                        Description = "HP Spectre x360 2-in-1 cao cấp với Core Ultra 7, màn hình OLED 14 inch cảm ứng, 32GB RAM, 1TB SSD.",
                        Price = 41_990_000,
                        Stock = 6,
                        CategoryId = lapId,
                        IsActive = true,
                        ImageUrl = "https://images.unsplash.com/photo-1488590528505-98d2b5aba04b?w=400&h=400&fit=crop",
                        CreatedAt = DateTime.UtcNow.AddDays(-8)
                    },

                    // ===== Phụ kiện =====
                    new Product
                    {
                        Name = "AirPods Pro (2nd Generation)",
                        Description = "AirPods Pro thế hệ 2 với chip H2, chống ồn chủ động ANC nâng cao, âm thanh không gian thích ứng, chống nước IPX4.",
                        Price = 6_490_000,
                        Stock = 50,
                        CategoryId = pkId,
                        IsActive = true,
                        ImageUrl = "https://images.unsplash.com/photo-1606841837239-c5a1a4a07af7?w=400&h=400&fit=crop",
                        CreatedAt = DateTime.UtcNow.AddDays(-35)
                    },
                    new Product
                    {
                        Name = "Sony WH-1000XM5 Tai nghe ANC",
                        Description = "Sony WH-1000XM5 với chống ồn tốt nhất thế giới, pin 30 giờ, sạc nhanh 3 phút dùng 3 giờ, âm thanh LDAC Hi-Res.",
                        Price = 8_990_000,
                        Stock = 35,
                        CategoryId = pkId,
                        IsActive = true,
                        ImageUrl = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=400&h=400&fit=crop",
                        CreatedAt = DateTime.UtcNow.AddDays(-32)
                    },
                    new Product
                    {
                        Name = "Logitech MX Master 3S",
                        Description = "Chuột Logitech MX Master 3S với cảm biến 8000DPI, cuộn siêu êm MagSpeed, kết nối đa thiết bị Bluetooth/USB, sạc USB-C.",
                        Price = 2_290_000,
                        Stock = 80,
                        CategoryId = pkId,
                        IsActive = true,
                        ImageUrl = "https://images.unsplash.com/photo-1527864550417-7fd91fc51a46?w=400&h=400&fit=crop",
                        CreatedAt = DateTime.UtcNow.AddDays(-40)
                    },
                    new Product
                    {
                        Name = "Sạc nhanh Anker 140W GaN",
                        Description = "Củ sạc Anker 140W GaN 3 cổng (2 USB-C + 1 USB-A), sạc MacBook Pro, iPhone và laptop đồng thời.",
                        Price = 1_590_000,
                        Stock = 100,
                        CategoryId = pkId,
                        IsActive = true,
                        ImageUrl = "https://images.unsplash.com/photo-1585771724684-38269d6639fd?w=400&h=400&fit=crop",
                        CreatedAt = DateTime.UtcNow.AddDays(-45)
                    },
                    new Product
                    {
                        Name = "Bàn phím cơ Keychron Q3 Pro",
                        Description = "Bàn phím cơ Keychron Q3 Pro TKL không dây, switch Gateron G Pro, gasket mount, nhôm nguyên khối, Bluetooth 5.1.",
                        Price = 4_190_000,
                        Stock = 40,
                        CategoryId = pkId,
                        IsActive = true,
                        ImageUrl = "https://images.unsplash.com/photo-1587829741301-dc798b83add3?w=400&h=400&fit=crop",
                        CreatedAt = DateTime.UtcNow.AddDays(-14)
                    },

                    // ===== Máy tính bảng =====
                    new Product
                    {
                        Name = "iPad Pro M4 13 inch WiFi 256GB",
                        Description = "iPad Pro M4 mỏng nhất từ trước đến nay, chip M4 mạnh mẽ, màn hình OLED Ultra Retina XDR 13 inch, hỗ trợ Apple Pencil Pro.",
                        Price = 32_990_000,
                        Stock = 12,
                        CategoryId = tabId,
                        IsActive = true,
                        ImageUrl = "https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?w=400&h=400&fit=crop",
                        CreatedAt = DateTime.UtcNow.AddDays(-7)
                    },
                    new Product
                    {
                        Name = "Samsung Galaxy Tab S9 Ultra 512GB",
                        Description = "Galaxy Tab S9 Ultra màn hình 14.6 inch Dynamic AMOLED 2X, bút S Pen tích hợp, Snapdragon 8 Gen 2, IP68 chống nước.",
                        Price = 27_990_000,
                        Stock = 9,
                        CategoryId = tabId,
                        IsActive = true,
                        ImageUrl = "https://images.unsplash.com/photo-1548036328-c9fa89d128fa?w=400&h=400&fit=crop",
                        CreatedAt = DateTime.UtcNow.AddDays(-11)
                    },

                    // ===== Đồng hồ thông minh =====
                    new Product
                    {
                        Name = "Apple Watch Ultra 2 49mm",
                        Description = "Apple Watch Ultra 2 với vỏ titan 49mm, pin 60 giờ, chịu nước 100m, GPS đa hệ chính xác, chip S9 SiP mới nhất.",
                        Price = 22_990_000,
                        Stock = 20,
                        CategoryId = dhId,
                        IsActive = true,
                        ImageUrl = "https://images.unsplash.com/photo-1546868871-7041f2a55e12?w=400&h=400&fit=crop",
                        CreatedAt = DateTime.UtcNow.AddDays(-9)
                    },
                    new Product
                    {
                        Name = "Samsung Galaxy Watch 7 Pro 47mm",
                        Description = "Galaxy Watch 7 Pro titan cao cấp, theo dõi sức khỏe AI, pin 5 ngày, vỏ titan grade 4, mặt kính sapphire.",
                        Price = 12_990_000,
                        Stock = 25,
                        CategoryId = dhId,
                        IsActive = true,
                        ImageUrl = "https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=400&h=400&fit=crop",
                        CreatedAt = DateTime.UtcNow.AddDays(-6)
                    },
                    new Product
                    {
                        Name = "Garmin Fenix 7X Pro Solar",
                        Description = "Garmin Fenix 7X Pro Solar với sạc năng lượng mặt trời, pin 28 ngày, GPS đa hệ, đo SpO2/nhịp tim/nhiệt độ, chống nước 10ATM.",
                        Price = 19_990_000,
                        Stock = 15,
                        CategoryId = dhId,
                        IsActive = true,
                        ImageUrl = "https://images.unsplash.com/photo-1508685096489-7aacd43bd3b1?w=400&h=400&fit=crop",
                        CreatedAt = DateTime.UtcNow.AddDays(-3)
                    }
                };

                context.Products.AddRange(products);
                await context.SaveChangesAsync();
            }
        }
    }
}
