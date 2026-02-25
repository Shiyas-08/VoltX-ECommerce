using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs
{
    public class UpdateProfileDto
    {
        [Required(ErrorMessage = "Name is required")]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters")]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        [RegularExpression(@"^[A-Za-z][A-Za-z\s-]*$", ErrorMessage = "Invalid name format")]
        public string Name { get; set; }

        public string? Phone { get; set; }
    }
}
