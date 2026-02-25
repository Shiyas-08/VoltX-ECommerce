namespace ECommerce.Models
{
    public class User: BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }

        public string PasswordHash { get; set; } = null!;


        public int RoleId { get; set; }
        public Role Role { get; set; }

        public bool IsBlocked { get; set; } = false;

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; } 
    }
}
