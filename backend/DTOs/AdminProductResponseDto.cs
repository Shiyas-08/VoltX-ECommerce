using ECommerce.DTOs;

public class AdminProductResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public string? Description { get; set; }

    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;

    public int Stock { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsActive { get; set; }


    public List<ProductImageDto> Images { get; set; } = [];
}
