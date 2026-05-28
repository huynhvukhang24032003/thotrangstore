using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopWebApp.Models;
using ShopWebApp.Services;

namespace ShopWebApp.Controllers
{
    public class CartController : Controller
    {
        private readonly CartService _cartService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ShopWebApp.Data.AppDbContext _context;

        public CartController(CartService cartService, UserManager<ApplicationUser> userManager, ShopWebApp.Data.AppDbContext context)
        {
            _cartService = cartService;
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

        // GET: /Cart
        public async Task<IActionResult> Index()
        {
            var ids = GetCartIdentifiers();
            var items = await _cartService.GetCartAsync(ids.userId, ids.sessionId);
            return View(items);
        }

        // GET: /Cart/Add
        [HttpGet]
        public IActionResult Add()
        {
            // Redirect GET to Index to prevent methods not allowed errors
            return RedirectToAction("Index", "Shop");
        }

        // POST: /Cart/Add
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int productId, int quantity = 1)
        {
            var setting = _context.SystemSettings.FirstOrDefault();
            if (setting?.RequireLoginToPurchase == true && User.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Login", "Account", new { ReturnUrl = $"/Shop/Details/{productId}" });
            }

            var ids = GetCartIdentifiers();
            await _cartService.AddToCartAsync(ids.userId, ids.sessionId, productId, quantity);
            TempData["Success"] = "Đã thêm vào giỏ hàng!";
            return RedirectToAction("Index");
        }

        // POST: /Cart/Update
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int cartItemId, int quantity)
        {
            var ids = GetCartIdentifiers();
            await _cartService.UpdateQuantityAsync(ids.userId, ids.sessionId, cartItemId, quantity);
            return RedirectToAction("Index");
        }

        // POST: /Cart/Remove
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int cartItemId)
        {
            var ids = GetCartIdentifiers();
            await _cartService.RemoveItemAsync(ids.userId, ids.sessionId, cartItemId);
            TempData["Success"] = "Đã xóa sản phẩm khỏi giỏ hàng!";
            return RedirectToAction("Index");
        }
    }
}
