namespace ECommerce.Models
{
    public class Cart
    {
        public int Id { get; set; }

        public string UserId { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
