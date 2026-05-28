namespace ShopWebApp.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; } = 1;

        // For Guest checkout
        public string? SessionId { get; set; }

        // Foreign keys
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
