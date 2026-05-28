using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopWebApp.Models;
using ShopWebApp.Services;
using ShopWebApp.ViewModels;

namespace ShopWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly OAuthProviderConfig _oauthConfig;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            OAuthProviderConfig oauthConfig)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _oauthConfig = oauthConfig;
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Home");
            SetOAuthViewBag();
            return View();
        }

        // POST: /Account/Register
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                Phone = model.Phone
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Customer");
                await _signInManager.SignInAsync(user, isPersistent: false);
                TempData["Success"] = "Đăng ký thành công! Chào mừng bạn đến với ShopWeb 🎉";
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Home");
            SetOAuthViewBag();
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        // POST: /Account/Login
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password,
                model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                TempData["Success"] = "Đăng nhập thành công!";
                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    return Redirect(model.ReturnUrl);
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["Success"] = "Đã đăng xuất thành công.";
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/AccessDenied
        public IActionResult AccessDenied() => View();

        // GET: /Account/ExternalLogin (Google/Facebook)
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string? returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        // GET: /Account/ExternalLoginCallback
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null) return RedirectToAction(nameof(Login));

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (result.Succeeded)
            {
                TempData["Success"] = $"Đăng nhập bằng {info.LoginProvider} thành công!";
                return RedirectToAction("Index", "Home");
            }

            // Tạo tài khoản mới từ OAuth
            var email = info.Principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? "";
            var name = info.Principal.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? email;

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = name,
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(user);
            if (createResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Customer");
                await _userManager.AddLoginAsync(user, info);
                await _signInManager.SignInAsync(user, isPersistent: false);
                TempData["Success"] = $"Đăng nhập bằng {info.LoginProvider} thành công!";
                return RedirectToAction("Index", "Home");
            }

            TempData["Error"] = "Đăng nhập bằng tài khoản bên ngoài thất bại.";
            return RedirectToAction(nameof(Login));
        }

        // GET: /Account/Profile
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction(nameof(Login));
            ViewBag.Roles = await _userManager.GetRolesAsync(user);
            return View(user);
        }

        // POST: /Account/Profile (cập nhật thông tin)
        [HttpPost, ValidateAntiForgeryToken, Authorize]
        public async Task<IActionResult> Profile(string fullName, string? phone, string? address)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction(nameof(Login));

            if (string.IsNullOrWhiteSpace(fullName))
            {
                TempData["Error"] = "Họ và tên không được để trống!";
                ViewBag.Roles = await _userManager.GetRolesAsync(user);
                return View(user);
            }

            user.FullName = fullName.Trim();
            user.Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
            user.Address = string.IsNullOrWhiteSpace(address) ? null : address.Trim();

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                TempData["Success"] = "Cập nhật thông tin thành công!";
            else
                TempData["Error"] = "Có lỗi xảy ra: " + string.Join(", ", result.Errors.Select(e => e.Description));

            ViewBag.Roles = await _userManager.GetRolesAsync(user);
            return View(user);
        }

        // POST: /Account/ChangePassword
        [HttpPost, ValidateAntiForgeryToken, Authorize]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction(nameof(Login));

            if (newPassword != confirmPassword)
            {
                TempData["Error"] = "Mật khẩu xác nhận không khớp!";
                return RedirectToAction(nameof(Profile));
            }

            if (newPassword.Length < 6)
            {
                TempData["Error"] = "Mật khẩu mới phải có ít nhất 6 ký tự!";
                return RedirectToAction(nameof(Profile));
            }

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["Success"] = "Đổi mật khẩu thành công!";
            }
            else
            {
                TempData["Error"] = result.Errors.FirstOrDefault()?.Description ?? "Đổi mật khẩu thất bại!";
            }

            return RedirectToAction(nameof(Profile));
        }

        // ========== Helper ==========
        private void SetOAuthViewBag()
        {
            ViewBag.GoogleEnabled = _oauthConfig.GoogleEnabled;
            ViewBag.FacebookEnabled = _oauthConfig.FacebookEnabled;
            ViewBag.AnyOAuthEnabled = _oauthConfig.AnyEnabled;
        }
    }
}
