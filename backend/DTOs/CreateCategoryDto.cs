using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs
{
    public class CreateCategoryDto
    {
        [Required]
        public string Name { get; set; } = null!;
    }
}
