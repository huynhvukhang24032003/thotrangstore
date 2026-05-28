using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopWebApp.Models;

namespace ShopWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private const int PageSize = 15;

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Admin/User
        public async Task<IActionResult> Index(string? search, string? role, int page = 1)
        {
            var users = _userManager.Users.AsQueryable();

            // Tìm kiếm không phân biệt hoa thường
            if (!string.IsNullOrWhiteSpace(search))
            {
                var kw = search.Trim().ToLower();
                users = users.Where(u =>
                    u.FullName.ToLower().Contains(kw) ||
                    u.Email!.ToLower().Contains(kw) ||
                    (u.Phone != null && u.Phone.Contains(kw)));
            }

            var totalUsers = await users.CountAsync();
            var pagedUsers = await users
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            // Lấy role cho từng user
            var userRoles = new Dictionary<string, IList<string>>();
            foreach (var u in pagedUsers)
                userRoles[u.Id] = await _userManager.GetRolesAsync(u);

            // Lọc theo role sau khi lấy danh sách
            if (!string.IsNullOrWhiteSpace(role))
            {
                pagedUsers = pagedUsers
                    .Where(u => userRoles.ContainsKey(u.Id) && userRoles[u.Id].Contains(role))
                    .ToList();
            }

            ViewBag.Search = search;
            ViewBag.Role = role;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalUsers / (double)PageSize);
            ViewBag.UserRoles = userRoles;
            ViewBag.AllRoles = _roleManager.Roles.Select(r => r.Name).ToList();

            return View(pagedUsers);
        }

        // GET: Admin/User/Details/id
        public async Task<IActionResult> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            ViewBag.Roles = roles;
            ViewBag.OrderCount = 0; // Có thể mở rộng để đếm đơn hàng
            ViewBag.AllRoles = _roleManager.Roles.Select(r => r.Name).ToList();
            return View(user);
        }

        // POST: Admin/User/ChangeRole
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            // Không cho thay đổi role của chính mình
            if (user.Email == User.Identity?.Name)
            {
                TempData["Error"] = "Không thể thay đổi role của chính mình!";
                return RedirectToAction(nameof(Details), new { id = userId });
            }

            // Xóa tất cả role cũ rồi gán role mới
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, role);

            TempData["Success"] = $"Đã đổi role của '{user.FullName}' thành '{role}'!";
            return RedirectToAction(nameof(Details), new { id = userId });
        }

        // POST: Admin/User/ToggleLock
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLock(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            if (user.Email == User.Identity?.Name)
            {
                TempData["Error"] = "Không thể khóa tài khoản của chính mình!";
                return RedirectToAction(nameof(Index));
            }

            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow)
            {
                // Đang bị khóa → mở khóa
                await _userManager.SetLockoutEndDateAsync(user, null);
                TempData["Success"] = $"Đã mở khóa tài khoản '{user.FullName}'!";
            }
            else
            {
                // Chưa bị khóa → khóa 100 năm
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
                TempData["Success"] = $"Đã khóa tài khoản '{user.FullName}'!";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/User/Delete
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            if (user.Email == User.Identity?.Name)
            {
                TempData["Error"] = "Không thể xóa tài khoản của chính mình!";
                return RedirectToAction(nameof(Index));
            }

            await _userManager.DeleteAsync(user);
            TempData["Success"] = $"Đã xóa tài khoản '{user.FullName}'!";
            return RedirectToAction(nameof(Index));
        }
    }
}
