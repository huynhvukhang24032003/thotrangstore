using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopWebApp.Data;
using ShopWebApp.Models;
using ShopWebApp.Services;

namespace ShopWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly AppDbContext _db;
        private readonly OrderService _orderService;
        private const int PageSize = 15;

        public OrderController(AppDbContext db, OrderService orderService)
        {
            _db = db;
            _orderService = orderService;
        }

        // GET: Admin/Order
        public async Task<IActionResult> Index(string? search, OrderStatus? status, int page = 1)
        {
            var query = _db.Orders.Include(o => o.User).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                var lowerSearch = search.ToLower();
                query = query.Where(o => (o.User != null && (o.User.FullName.ToLower().Contains(lowerSearch) || o.User.Email!.ToLower().Contains(lowerSearch))) ||
                                         (o.GuestName != null && o.GuestName.ToLower().Contains(lowerSearch)) ||
                                         (o.GuestEmail != null && o.GuestEmail.ToLower().Contains(lowerSearch)));
            }

            if (status.HasValue)
                query = query.Where(o => o.Status == status.Value);

            var total = await query.CountAsync();
            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.Status = status;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)PageSize);

            return View(orders);
        }

        // GET: Admin/Order/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderDetailAsync(id);
            if (order == null) return NotFound();
            return View(order);
        }

        // POST: Admin/Order/UpdateStatus/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
        {
            var success = await _orderService.UpdateStatusAsync(id, status);
            if (!success) return NotFound();

            TempData["Success"] = "Đã cập nhật trạng thái đơn hàng!";
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
