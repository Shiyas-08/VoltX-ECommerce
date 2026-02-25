using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs
{
    public class UpdateCategoryDto
    {
        [Required]
        public string Name { get; set; } = null!;

        public bool IsActive { get; set; }
    }
}
