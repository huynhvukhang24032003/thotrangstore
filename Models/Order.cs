using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopWebApp.Models
{
    public enum OrderStatus
    {
        Pending,        // Chờ xử lý
        Processing,     // Đang xử lý
        Shipping,       // Đang giao hàng
        Completed,      // Hoàn thành
        Cancelled       // Đã hủy
    }

    public enum PaymentMethod
    {
        COD,            // Thanh toán khi nhận hàng
        SePay,          // SePay
        PayOS           // PayOS
    }

    public class Order
    {
        public int Id { get; set; }

        [Display(Name = "Địa chỉ giao hàng")]
        [Required, MaxLength(500)]
        public string ShippingAddress { get; set; } = string.Empty;

        [Display(Name = "Ghi chú")]
        public string? Note { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Tổng tiền")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Trạng thái")]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [Display(Name = "Phương thức thanh toán")]
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.COD;

        [Display(Name = "Đã thanh toán")]
        public bool IsPaid { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Foreign key
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        [Display(Name = "Họ tên")]
        [MaxLength(100)]
        public string? GuestName { get; set; }

        [Display(Name = "Số điện thoại")]
        [MaxLength(20)]
        public string? GuestPhone { get; set; }

        [Display(Name = "Email")]
        [MaxLength(255)]
        public string? GuestEmail { get; set; }

        // Navigation
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
