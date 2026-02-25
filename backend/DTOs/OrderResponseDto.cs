namespace ECommerce.DTOs
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public decimal Total { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; } = "";    
        public List<OrderItemResponseDto> Items { get; set; } = new();
        public OrderAddressResponseDto? Address { get; set; }
    }
}
