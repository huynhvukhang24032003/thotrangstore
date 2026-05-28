using System.ComponentModel.DataAnnotations;

namespace ShopWebApp.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        [Display(Name = "Tên danh mục")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? Slug { get; set; }

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Display(Name = "Kích hoạt")]
        public bool IsActive { get; set; } = true;

        // Navigation
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
