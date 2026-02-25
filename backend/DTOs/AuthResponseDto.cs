namespace ECommerce.DTOs
{
    public class AuthResponseDto
    {
        public string Message { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
