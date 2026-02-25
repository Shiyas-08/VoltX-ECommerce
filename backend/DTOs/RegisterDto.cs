using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Name is required")]
        [RegularExpression(@"^[A-Za-z ]+$",
        ErrorMessage = "Name can contain only letters and spaces")]
        public string Name { get; set; } = null!;


        [Required]
        [RegularExpression(@"^[a-zA-Z0-9][a-zA-Z0-9._%+-]*@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = null!;


        [Phone(ErrorMessage = "Invalid phone number")]
        [MinLength(10, ErrorMessage = "phone number must be at least 10 characters")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).+$",
        ErrorMessage = "Password must contain uppercase, lowercase, number and special character")]

        public string Password { get; set; } = null!;
    }
}
