namespace ECommerce.DTOs
{
    public class OrderAddressResponseDto
    {
        public string FullName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string AddressLine1 { get; set; } = "";
        public string AddressLine2 { get; set; } = "";
        public string City { get; set; } = "";
        public string State { get; set; } = "";
        public string Pincode { get; set; } = "";
    }
}
