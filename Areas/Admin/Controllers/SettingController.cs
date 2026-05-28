using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopWebApp.Data;
using ShopWebApp.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ShopWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SettingController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SettingController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var setting = await _context.SystemSettings.FirstOrDefaultAsync();
            if (setting == null)
            {
                setting = new SystemSetting(); // Create a default empty one if none exists in the DB
            }
            return View(setting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(SystemSetting model, IFormFile? LogoFile)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            var existingSetting = await _context.SystemSettings.FirstOrDefaultAsync();
            
            if (LogoFile != null && LogoFile.Length > 0)
            {
                string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadDir))
                {
                    Directory.CreateDirectory(uploadDir);
                }

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(LogoFile.FileName);
                string filePath = Path.Combine(uploadDir, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await LogoFile.CopyToAsync(fileStream);
                }

                model.LogoUrl = "/uploads/" + fileName;

                // Optionally, delete the old logo if it's there
                if (existingSetting != null && !string.IsNullOrEmpty(existingSetting.LogoUrl))
                {
                    var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, existingSetting.LogoUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
            }
            else if (existingSetting != null)
            {
                // Preserve old logo if right now no new uploaded logo
                model.LogoUrl = existingSetting.LogoUrl;
            }

            if (existingSetting != null)
            {
                // Update existing setting
                existingSetting.WebsiteName = model.WebsiteName;
                existingSetting.LogoUrl = model.LogoUrl;
                existingSetting.EnableRegistration = model.EnableRegistration;
                existingSetting.EnableOrdering = model.EnableOrdering;
                existingSetting.SeoDescription = model.SeoDescription;
                existingSetting.SeoKeywords = model.SeoKeywords;
                existingSetting.Hotline = model.Hotline;
                existingSetting.ContactEmail = model.ContactEmail;
                existingSetting.Address = model.Address;
                existingSetting.Slogan = model.Slogan;
                existingSetting.RequireLoginToPurchase = model.RequireLoginToPurchase;
                
                _context.Update(existingSetting);
            }
            else
            {
                // Insert a new setting row
                _context.Add(model);
            }

            await _context.SaveChangesAsync();

            TempData["SettingSuccess"] = "Cập nhật cài đặt thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
