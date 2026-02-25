namespace ECommerce.Models
{
    public class Product:BaseEntity
    {
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }

        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public bool IsFeatured { get; set; }
        public bool IsActive { get; set; } = true;
        public int Stock { get; set; }
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();

    }
}
