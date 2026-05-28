using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopWebApp.Data;

namespace ShopWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;

        public HomeController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy sản phẩm nổi bật (8 sản phẩm mới nhất)
            var featuredProducts = await _db.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive && p.Stock > 0)
                .OrderByDescending(p => p.CreatedAt)
                .Take(8)
                .ToListAsync();

            var categories = await _db.Categories
                .Where(c => c.IsActive)
                .ToListAsync();

            ViewBag.Categories = categories;
            return View(featuredProducts);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
