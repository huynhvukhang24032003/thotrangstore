using Microsoft.EntityFrameworkCore;
using ShopWebApp.Data;
using ShopWebApp.Models;

namespace ShopWebApp.Services
{
    public class CartService
    {
        private readonly AppDbContext _db;

        public CartService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<CartItem>> GetCartAsync(string? userId, string? sessionId)
        {
            var query = _db.CartItems.Include(ci => ci.Product).AsQueryable();
            if (!string.IsNullOrEmpty(userId))
                query = query.Where(ci => ci.UserId == userId);
            else if (!string.IsNullOrEmpty(sessionId))
                query = query.Where(ci => ci.SessionId == sessionId);
            else return new List<CartItem>();

            return await query.ToListAsync();
        }

        public async Task<int> GetCartCountAsync(string? userId, string? sessionId)
        {
            var query = _db.CartItems.AsQueryable();
            if (!string.IsNullOrEmpty(userId))
                query = query.Where(ci => ci.UserId == userId);
            else if (!string.IsNullOrEmpty(sessionId))
                query = query.Where(ci => ci.SessionId == sessionId);
            else return 0;

            return await query.SumAsync(ci => ci.Quantity);
        }

        public async Task AddToCartAsync(string? userId, string? sessionId, int productId, int quantity = 1)
        {
            var query = _db.CartItems.AsQueryable();
            if (!string.IsNullOrEmpty(userId))
                query = query.Where(ci => ci.UserId == userId && ci.ProductId == productId);
            else
                query = query.Where(ci => ci.SessionId == sessionId && ci.ProductId == productId);

            var existing = await query.FirstOrDefaultAsync();

            if (existing != null)
            {
                existing.Quantity += quantity;
            }
            else
            {
                _db.CartItems.Add(new CartItem
                {
                    UserId = string.IsNullOrEmpty(userId) ? null : userId,
                    SessionId = string.IsNullOrEmpty(userId) ? sessionId : null,
                    ProductId = productId,
                    Quantity = quantity
                });
            }
            await _db.SaveChangesAsync();
        }

        public async Task UpdateQuantityAsync(string? userId, string? sessionId, int cartItemId, int quantity)
        {
            var item = await _db.CartItems.FindAsync(cartItemId);
            if (item == null) return;
            if (!string.IsNullOrEmpty(userId) && item.UserId != userId) return;
            if (!string.IsNullOrEmpty(sessionId) && item.SessionId != sessionId) return;

            if (quantity <= 0)
                _db.CartItems.Remove(item);
            else
                item.Quantity = quantity;

            await _db.SaveChangesAsync();
        }

        public async Task RemoveItemAsync(string? userId, string? sessionId, int cartItemId)
        {
            var item = await _db.CartItems.FindAsync(cartItemId);
            if (item != null)
            {
                if (!string.IsNullOrEmpty(userId) && item.UserId != userId) return;
                if (!string.IsNullOrEmpty(sessionId) && item.SessionId != sessionId) return;

                _db.CartItems.Remove(item);
                await _db.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(string? userId, string? sessionId)
        {
            var query = _db.CartItems.AsQueryable();
            if (!string.IsNullOrEmpty(userId))
                query = query.Where(ci => ci.UserId == userId);
            else if (!string.IsNullOrEmpty(sessionId))
                query = query.Where(ci => ci.SessionId == sessionId);
            else return;

            _db.CartItems.RemoveRange(query);
            await _db.SaveChangesAsync();
        }
    }
}
