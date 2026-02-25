namespace ECommerce.DTOs
{
    public class OrderItemResponseDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = "";
        public string Image { get; set; } = "";
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public string Category { get; set; }
    }
}
