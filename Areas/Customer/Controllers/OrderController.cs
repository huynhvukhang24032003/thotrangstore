using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopWebApp.Models;
using ShopWebApp.Services;

namespace ShopWebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly OrderService _orderService;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(OrderService orderService, UserManager<ApplicationUser> userManager)
        {
            _orderService = orderService;
            _userManager = userManager;
        }

        // GET: Customer/Order
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User)!;
            var orders = await _orderService.GetUserOrdersAsync(userId);
            return View(orders);
        }

        // GET: Customer/Order/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var order = await _orderService.GetOrderDetailAsync(id, userId);
            if (order == null) return NotFound();
            return View(order);
        }
    }
}
