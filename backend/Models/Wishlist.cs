namespace ECommerce.Models
{
    public class Wishlist
    {
        public int Id { get; set; }

        public string UserId { get; set; } = null!;

        public int ProductId { get; set; }
        public Product? Product { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
