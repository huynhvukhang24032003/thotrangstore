using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopWebApp.Data;
using ShopWebApp.ViewModels;

namespace ShopWebApp.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _db;
        private const int PageSize = 12;

        public ShopController(AppDbContext db)
        {
            _db = db;
        }

        // GET: /Shop
        public async Task<IActionResult> Index(string? search, int? categoryId, string? sort, int page = 1)
        {
            var query = _db.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .AsQueryable();

            // Tìm kiếm không phân biệt hoa/thường (SQLite: LIKE mặc định case-insensitive với ASCII)
            if (!string.IsNullOrWhiteSpace(search))
            {
                var keyword = $"%{search.Trim()}%";
                query = query.Where(p =>
                    EF.Functions.Like(p.Name, keyword) ||
                    (p.Description != null && EF.Functions.Like(p.Description, keyword)));
            }

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            // Sắp xếp - normalize giá trị sort để tránh lỗi null/unknown
            sort = sort?.ToLower() switch
            {
                "price-asc"  => "price-asc",
                "price-desc" => "price-desc",
                "newest"     => "newest",
                _            => "newest"
            };

            query = sort switch
            {
                "price-asc"  => query.OrderBy(p => (double)p.Price),
                "price-desc" => query.OrderByDescending(p => (double)p.Price),
                _            => query.OrderByDescending(p => p.CreatedAt)
            };

            var total = await query.CountAsync();
            var products = await query
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            var vm = new ProductListViewModel
            {
                Products   = products,
                Categories = await _db.Categories.Where(c => c.IsActive).ToListAsync(),
                CategoryId  = categoryId,
                SearchQuery = search?.Trim(),
                CurrentPage = page,
                TotalPages  = (int)Math.Ceiling(total / (double)PageSize)
            };

            ViewBag.Sort = sort;
            return View(vm);
        }

        // GET: /Shop/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var product = await _db.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            if (product == null) return NotFound();

            // Sản phẩm liên quan
            var related = await _db.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == product.CategoryId && p.Id != id && p.IsActive)
                .Take(4)
                .ToListAsync();

            ViewBag.RelatedProducts = related;
            return View(product);
        }
    }
}
