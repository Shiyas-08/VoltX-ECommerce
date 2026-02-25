using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs
{
    public class CreateProductDto
    {

        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        [RegularExpression(@"^(?![.\-_]+$).+", ErrorMessage = "Invalid product name")]
        public string Name { get; set; } = null!;

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Price must be greater than 0")]

        public decimal Price { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(500)]
        [RegularExpression(@"^(?![.\-_]+$).+", ErrorMessage = "Invalid description")]
        public string Description { get; set; } = null!;
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid category")]
        public int CategoryId { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int Stock { get; set; }
        public bool IsFeatured { get; set; }


        public List<IFormFile> Images { get; set; }

    }
}
    