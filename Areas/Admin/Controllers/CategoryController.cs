using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopWebApp.Data;
using ShopWebApp.Models;

namespace ShopWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _db;

        public CategoryController(AppDbContext db)
        {
            _db = db;
        }

        // GET: Admin/Category
        public async Task<IActionResult> Index()
        {
            var categories = await _db.Categories
                .Include(c => c.Products)
                .OrderBy(c => c.Name)
                .ToListAsync();
            return View(categories);
        }

        // GET: Admin/Category/Create
        public IActionResult Create() => View(new Category());

        // POST: Admin/Category/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category model)
        {
            if (!ModelState.IsValid) return View(model);

            // Tự động tạo slug
            model.Slug = GenerateSlug(model.Name);

            // Kiểm tra slug trùng
            if (await _db.Categories.AnyAsync(c => c.Slug == model.Slug))
                model.Slug = model.Slug + "-" + Guid.NewGuid().ToString("N")[..6];

            _db.Categories.Add(model);
            await _db.SaveChangesAsync();

            TempData["Success"] = $"Đã thêm danh mục '{model.Name}' thành công!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Category/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var cat = await _db.Categories.FindAsync(id);
            if (cat == null) return NotFound();
            return View(cat);
        }

        // POST: Admin/Category/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            _db.Update(model);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Cập nhật danh mục thành công!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Category/Delete/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var cat = await _db.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id);
            if (cat == null) return NotFound();

            if (cat.Products.Any())
            {
                TempData["Error"] = "Không thể xóa danh mục đang có sản phẩm!";
                return RedirectToAction(nameof(Index));
            }

            _db.Categories.Remove(cat);
            await _db.SaveChangesAsync();

            TempData["Success"] = $"Đã xóa danh mục '{cat.Name}'!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Category/ToggleActive/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var cat = await _db.Categories.FindAsync(id);
            if (cat == null) return NotFound();

            cat.IsActive = !cat.IsActive;
            await _db.SaveChangesAsync();

            TempData["Success"] = $"Danh mục '{cat.Name}' đã được {(cat.IsActive ? "kích hoạt" : "vô hiệu hóa")}!";
            return RedirectToAction(nameof(Index));
        }

        private static string GenerateSlug(string name)
        {
            var slug = name.ToLower().Trim();
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[àáạảãâầấậẩẫăằắặẳẵ]", "a");
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[èéẹẻẽêềếệểễ]", "e");
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[ìíịỉĩ]", "i");
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[òóọỏõôồốộổỗơờớợởỡ]", "o");
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[ùúụủũưừứựửữ]", "u");
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[ỳýỵỷỹ]", "y");
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[đ]", "d");
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", "-");
            return slug;
        }
    }
}
