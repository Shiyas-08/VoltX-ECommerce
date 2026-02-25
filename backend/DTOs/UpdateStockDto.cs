using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs
{
    public class UpdateStockDto
    {
      
            [Required]
            public int Quantity { get; set; } 
        
    }
}
