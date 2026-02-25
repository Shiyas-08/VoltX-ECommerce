using ECommerce.DTOs;
using ECommerce.Models;

namespace ECommerce.Services
{
    public interface IAuthService
    {
        void Register(RegisterDto dto);
        User Login(LoginDto dto);
        Task<ApiResponse<string>> ForgotPasswordAsync(string email);
        Task<ApiResponse<string>> VerifyOtpAsync(string email, string otp);
        Task<ApiResponse<string>> ResetPasswordAsync(string email, string newPassword);
        Task<ApiResponse<string>> UpdateProfileAsync(string userId, UpdateProfileDto dto);

    }
}
