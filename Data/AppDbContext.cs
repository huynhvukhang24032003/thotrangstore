using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShopWebApp.Models;

namespace ShopWebApp.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Category
            builder.Entity<Category>(e =>
            {
                e.HasIndex(c => c.Slug).IsUnique();
                e.Property(c => c.Name).IsRequired().HasMaxLength(100);
            });

            // Product
            builder.Entity<Product>(e =>
            {
                e.Property(p => p.Name).IsRequired().HasMaxLength(200);
                e.Property(p => p.Price).HasColumnType("decimal(18,2)");
                e.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Order
            builder.Entity<Order>(e =>
            {
                e.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");
                e.HasOne(o => o.User)
                    .WithMany(u => u.Orders)
                    .HasForeignKey(o => o.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // OrderItem
            builder.Entity<OrderItem>(e =>
            {
                e.Property(oi => oi.UnitPrice).HasColumnType("decimal(18,2)");
                e.HasOne(oi => oi.Order)
                    .WithMany(o => o.OrderItems)
                    .HasForeignKey(oi => oi.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(oi => oi.Product)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(oi => oi.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // CartItem
            builder.Entity<CartItem>(e =>
            {
                e.HasOne(ci => ci.User)
                    .WithMany(u => u.CartItems)
                    .HasForeignKey(ci => ci.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired(false);
                e.HasOne(ci => ci.Product)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(ci => ci.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
                // Một user hoặc session chỉ có 1 CartItem cho mỗi Product
                e.HasIndex(ci => new { ci.UserId, ci.SessionId, ci.ProductId }).IsUnique();
            });
        }
    }
}
