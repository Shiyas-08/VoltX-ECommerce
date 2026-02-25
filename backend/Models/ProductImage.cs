namespace ECommerce.Models
{
    public class ProductImage:BaseEntity
    {
        public string ImageUrl { get; set; } = null!;

        // FK
        public int ProductId { get; set; }
        public Product? Product { get; set; } 
    }
}
