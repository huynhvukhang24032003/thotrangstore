namespace ShopWebApp.ViewModels
{
    // ========== Account ViewModels ==========
    public class RegisterViewModel
    {
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [System.ComponentModel.DataAnnotations.Display(Name = "Họ và tên")]
        public string FullName { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Vui lòng nhập email")]
        [System.ComponentModel.DataAnnotations.EmailAddress]
        [System.ComponentModel.DataAnnotations.Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Display(Name = "Số điện thoại")]
        public string? Phone { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [System.ComponentModel.DataAnnotations.StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu ít nhất 6 ký tự")]
        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [System.ComponentModel.DataAnnotations.Display(Name = "Mật khẩu")]
        public string Password { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        [System.ComponentModel.DataAnnotations.Display(Name = "Xác nhận mật khẩu")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class LoginViewModel
    {
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Vui lòng nhập email")]
        [System.ComponentModel.DataAnnotations.EmailAddress]
        [System.ComponentModel.DataAnnotations.Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [System.ComponentModel.DataAnnotations.Display(Name = "Mật khẩu")]
        public string Password { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Display(Name = "Ghi nhớ đăng nhập")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }

    // ========== Checkout ViewModels ==========
    public class CheckoutViewModel
    {
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [System.ComponentModel.DataAnnotations.Display(Name = "Họ tên")]
        public string GuestName { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [System.ComponentModel.DataAnnotations.Display(Name = "Số điện thoại")]
        public string GuestPhone { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Display(Name = "Email")]
        public string? GuestEmail { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
        [System.ComponentModel.DataAnnotations.Display(Name = "Địa chỉ giao hàng")]
        public string ShippingAddress { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Display(Name = "Ghi chú")]
        public string? Note { get; set; }

        [System.ComponentModel.DataAnnotations.Display(Name = "Phương thức thanh toán")]
        public Models.PaymentMethod PaymentMethod { get; set; } = Models.PaymentMethod.COD;

        public List<Models.CartItem> CartItems { get; set; } = new();
        public decimal TotalAmount => CartItems.Sum(ci => (ci.Product?.Price ?? 0) * ci.Quantity);
    }

    // ========== Product ViewModels ==========
    public class ProductListViewModel
    {
        public List<Models.Product> Products { get; set; } = new();
        public List<Models.Category> Categories { get; set; } = new();
        public int? CategoryId { get; set; }
        public string? SearchQuery { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
    }
}
