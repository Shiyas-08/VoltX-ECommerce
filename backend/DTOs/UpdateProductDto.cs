using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs
{
    public class UpdateProductDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        [RegularExpression(@"^(?![\s._-]+$)(?!.*[\s]{2,}).*[A-Za-z0-9].*",
        ErrorMessage = "Invalid product name")]
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        [Required]
        [MinLength(5)]
        [MaxLength(500)]
        [RegularExpression(@"^(?![\s._-]+$).*[A-Za-z0-9].*",
        ErrorMessage = "Invalid description")]
        public string Description { get; set; } = null!;
        public int CategoryId { get; set; }
        public int Stock { get; set; }
        public bool IsFeatured { get; set; }
    }
}
