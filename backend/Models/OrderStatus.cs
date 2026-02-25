namespace ECommerce.Models
{
    public enum OrderStatus
    {
        Pending = 1,
        Paid = 2,
        Packed = 3,
        Shipped = 4,
        OutForDelivery = 5,
        Delivered = 6,
        Cancelled = 99
    }

}
