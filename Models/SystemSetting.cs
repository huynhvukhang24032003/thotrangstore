using System.ComponentModel.DataAnnotations;

namespace ShopWebApp.Models
{
    public class SystemSetting
    {
        [Key]
        public int Id { get; set; }
        
        [Display(Name = "Tên website")]
        [StringLength(255)]
        public string? WebsiteName { get; set; }

        [Display(Name = "Logo")]
        [StringLength(500)]
        public string? LogoUrl { get; set; }

        [Display(Name = "Cho phép đăng ký thành viên")]
        public bool EnableRegistration { get; set; } = true;

        [Display(Name = "Cho phép đặt hàng")]
        public bool EnableOrdering { get; set; } = true;

        [Display(Name = "Mô tả SEO trang chủ")]
        public string? SeoDescription { get; set; }

        [Display(Name = "Từ khóa SEO")]
        public string? SeoKeywords { get; set; }

        [Display(Name = "Số điện thoại Hotline")]
        [StringLength(50)]
        public string? Hotline { get; set; }

        [Display(Name = "Email liên hệ")]
        [StringLength(255)]
        [EmailAddress]
        public string? ContactEmail { get; set; }

        [Display(Name = "Địa chỉ cửa hàng")]
        public string? Address { get; set; }

        [Display(Name = "Slogan")]
        [StringLength(255)]
        public string? Slogan { get; set; }

        [Display(Name = "Bắt buộc đăng nhập để mua hàng")]
        public bool RequireLoginToPurchase { get; set; } = true;
    }
}
