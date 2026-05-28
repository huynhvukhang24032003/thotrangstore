using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopWebApp.Models;
using ShopWebApp.Services;
using ShopWebApp.ViewModels;

namespace ShopWebApp.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly CartService _cartService;
        private readonly OrderService _orderService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ShopWebApp.Data.AppDbContext _context;

        public CheckoutController(CartService cartService, OrderService orderService,
            UserManager<ApplicationUser> userManager, ShopWebApp.Data.AppDbContext context)
        {
            _cartService = cartService;
            _orderService = orderService;
            _userManager = userManager;
            _context = context;
        }

        private (string? userId, string? sessionId) GetCartIdentifiers()
        {
            if (User.Identity?.IsAuthenticated == true)
                return (_userManager.GetUserId(User), null);

            var sessionId = HttpContext.Request.Cookies["CartSessionId"];
            if (string.IsNullOrEmpty(sessionId))
            {
                sessionId = Guid.NewGuid().ToString();
                HttpContext.Response.Cookies.Append("CartSessionId", sessionId, new CookieOptions 
                { 
                    Expires = DateTime.Now.AddDays(30),
                    IsEssential = true 
                });
            }
            return (null, sessionId);
        }

        // GET: /Checkout
        public async Task<IActionResult> Index()
        {
            var setting = _context.SystemSettings.FirstOrDefault();
            if (setting?.RequireLoginToPurchase == true && User.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Login", "Account", new { ReturnUrl = "/Checkout" });
            }

            var ids = GetCartIdentifiers();
            var cartItems = await _cartService.GetCartAsync(ids.userId, ids.sessionId);

            if (!cartItems.Any())
            {
                TempData["Error"] = "Giỏ hàng trống. Vui lòng thêm sản phẩm trước khi đặt hàng.";
                return RedirectToAction("Index", "Cart");
            }

            ApplicationUser? user = null;
            if (ids.userId != null)
                user = await _userManager.FindByIdAsync(ids.userId);

            var vm = new CheckoutViewModel
            {
                CartItems = cartItems,
                GuestName = user?.FullName ?? "",
                GuestPhone = user?.PhoneNumber ?? "",
                GuestEmail = user?.Email ?? "",
                ShippingAddress = user?.Address ?? ""
            };

            return View(vm);
        }

        // POST: /Checkout/PlaceOrder
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            var setting = _context.SystemSettings.FirstOrDefault();
            if (setting?.RequireLoginToPurchase == true && User.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Login", "Account", new { ReturnUrl = "/Checkout" });
            }

            var ids = GetCartIdentifiers();

            if (!ModelState.IsValid)
            {
                model.CartItems = await _cartService.GetCartAsync(ids.userId, ids.sessionId);
                return View("Index", model);
            }

            try
            {
                var order = await _orderService.CreateOrderFromCartAsync(
                    ids.userId, ids.sessionId, model);

                if (order == null)
                {
                    TempData["Error"] = "Không thể tạo đơn hàng. Giỏ hàng trống.";
                    return RedirectToAction("Index", "Cart");
                }

                TempData["Success"] = $"Đặt hàng thành công! Mã đơn hàng: #{order.Id}";
                return RedirectToAction("Confirmation", new { id = order.Id });
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Cart");
            }
        }

        // GET: /Checkout/Confirmation/5
        public async Task<IActionResult> Confirmation(int id)
        {
            var ids = GetCartIdentifiers();
            var order = await _orderService.GetOrderDetailAsync(id, ids.userId, ids.sessionId);
            if (order == null) return NotFound();
            return View(order);
        }
    }
}
