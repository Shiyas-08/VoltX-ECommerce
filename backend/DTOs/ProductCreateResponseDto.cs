using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs
{
    public class ProductCreateResponseDto
    {

        public int Id { get; set;}
        
        public string Name { get; set; } = null!;

        public decimal Price { get; set; }
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } 

        public int Stock { get; set; }
        public bool IsFeatured { get; set; }

        public List<string>? ImageUrls { get; set; }
    }
}
