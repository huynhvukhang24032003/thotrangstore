using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopWebApp.Data;
using ShopWebApp.Models;
using ShopWebApp.Services;

namespace ShopWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ImageUploadService _imageService;
        private const int PageSize = 10;

        public ProductController(AppDbContext db, ImageUploadService imageService)
        {
            _db = db;
            _imageService = imageService;
        }

        // GET: Admin/Product
        public async Task<IActionResult> Index(string? search, int? categoryId, int page = 1)
        {
            var query = _db.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var keyword = $"%{search.Trim()}%";
                query = query.Where(p =>
                    EF.Functions.Like(p.Name, keyword) ||
                    (p.Description != null && EF.Functions.Like(p.Description, keyword)));
            }

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            var total = await query.CountAsync();
            var products = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.CategoryId = categoryId;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)PageSize);
            ViewBag.Categories = await _db.Categories.Where(c => c.IsActive).ToListAsync();

            return View(products);
        }

        // GET: Admin/Product/Create
        public async Task<IActionResult> Create()
        {
            await LoadCategoriesAsync();
            return View(new Product());
        }

        // POST: Admin/Product/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product model, IFormFile? imageFile)
        {
            ModelState.Remove("ImageUrl");
            ModelState.Remove("Category");
            if (!ModelState.IsValid) { await LoadCategoriesAsync(); return View(model); }

            model.ImageUrl = await _imageService.UploadAsync(imageFile);
            model.CreatedAt = DateTime.UtcNow;

            _db.Products.Add(model);
            await _db.SaveChangesAsync();

            TempData["Success"] = $"Đã thêm sản phẩm '{model.Name}'!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Product/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();
            await LoadCategoriesAsync(product.CategoryId);
            return View(product);
        }

        // POST: Admin/Product/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product model, IFormFile? imageFile)
        {
            if (id != model.Id) return BadRequest();
            ModelState.Remove("ImageUrl");
            ModelState.Remove("Category");
            if (!ModelState.IsValid) { await LoadCategoriesAsync(model.CategoryId); return View(model); }

            var existing = await _db.Products.FindAsync(id);
            if (existing == null) return NotFound();

            // Cập nhật ảnh nếu có file mới
            if (imageFile != null && imageFile.Length > 0)
            {
                _imageService.Delete(existing.ImageUrl);
                existing.ImageUrl = await _imageService.UploadAsync(imageFile);
            }

            existing.Name = model.Name;
            existing.Description = model.Description;
            existing.Price = model.Price;
            existing.Stock = model.Stock;
            existing.CategoryId = model.CategoryId;
            existing.IsActive = model.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            TempData["Success"] = "Cập nhật sản phẩm thành công!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Product/Delete/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();

            _imageService.Delete(product.ImageUrl);
            _db.Products.Remove(product);
            await _db.SaveChangesAsync();

            TempData["Success"] = $"Đã xóa sản phẩm '{product.Name}'!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Product/ToggleActive/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.IsActive = !product.IsActive;
            await _db.SaveChangesAsync();

            TempData["Success"] = $"Sản phẩm '{product.Name}' đã {(product.IsActive ? "kích hoạt" : "vô hiệu hóa")}!";
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadCategoriesAsync(int? selectedId = null)
        {
            var categories = await _db.Categories.Where(c => c.IsActive).ToListAsync();
            ViewBag.CategoryId = new SelectList(categories, "Id", "Name", selectedId);
        }
    }
}
