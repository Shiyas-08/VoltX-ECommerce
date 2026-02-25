using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs
{
    public class BuyNowOrderDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 1000, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; } = 1;

        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        [RegularExpression(@"^(?![.\-_ ]+$)[a-zA-Z ]+$", ErrorMessage = "Invalid full name")]
        public string FullName { get; set; } = null!;

        [Required]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Invalid phone number")]
        public string Phone { get; set; } = null!;

        [Required]
        [MinLength(10)]
        [MaxLength(300)]
        [RegularExpression(@"^(?![.\-_ ]+$).+", ErrorMessage = "Invalid address")]
        public string AddressLine1 { get; set; } = null!;

        public string? AddressLine2 { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        [RegularExpression(@"^(?![.\-_ ]+$)[a-zA-Z ]+$", ErrorMessage = "Invalid city")]
        public string City { get; set; } = null!;

        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        [RegularExpression(@"^(?![.\-_ ]+$)[a-zA-Z ]+$", ErrorMessage = "Invalid state")]
        public string State { get; set; } = null!;

        [Required]
        [RegularExpression(@"^[1-9][0-9]{5}$", ErrorMessage = "Invalid pincode")]
        public string Pincode { get; set; } = null!;
        public bool SaveAddress { get; set; }
    }
}
