using ECommerce.Data;
using ECommerce.DTOs;
using ECommerce.Helpers;
using ECommerce.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
namespace ECommerce.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly EmailService _emailService;

        public AuthService(AppDbContext context, IConfiguration config,EmailService emailService)
        {
            _context = context;
            _config = config;
            _emailService = emailService;
        }

        //register
        public void Register(RegisterDto dto)
        {
            if (_context.Users.Any(u => u.Email == dto.Email))
                throw new Exception("Email already exists");

            if (!PasswordValidator.IsStrong(dto.Password))
                throw new ApplicationException(
                    "Password must contain uppercase, lowercase, number and special character"
                );

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                RoleId = 2,              
                IsBlocked = false
            };

            _context.Users.Add(user);
            _context.SaveChanges();
        }

        // login

        public User Login(LoginDto dto)
        {
            var user = _context.Users.SingleOrDefault(u => u.Email == dto.Email);

            if (user == null)
                throw new Exception("Invalid email");

            if (user.IsBlocked)
                throw new Exception("User is blocked");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new Exception("Invalid password");

            // Generate refresh token
            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            _context.SaveChanges();

            return user;
        }


        //access token 
        public string GenerateJwt(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("roleId",user.RoleId.ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"])
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // refresh token create 
        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        public async Task<ApiResponse<string>> ForgotPasswordAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return ApiResponse<string>.Fail("User not found", 404);

            var otp = new Random().Next(100000, 999999).ToString();

            var otpEntity = new PasswordResetOtp
            {
                Email = email,
                Otp = otp,
                ExpiryTime = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false
            };

            _context.PasswordResetOtps.Add(otpEntity);
            await _context.SaveChangesAsync();

            // SEND EMAIL
            await _emailService.SendOtpEmailAsync(email,otp);

            return ApiResponse<string>.Ok(null, "OTP sent to email", 200);
        }

        public async Task<ApiResponse<string>> VerifyOtpAsync(string email, string otp)
        {
            var record = await _context.PasswordResetOtps
                .FirstOrDefaultAsync(x => x.Email == email && x.Otp == otp && !x.IsUsed);

            if (record == null)
                return ApiResponse<string>.Fail("Invalid OTP", 400);

            if (record.ExpiryTime < DateTime.UtcNow)
                return ApiResponse<string>.Fail("OTP expired", 400);

            record.IsUsed = true;
            await _context.SaveChangesAsync();

            return ApiResponse<string>.Ok(null, "OTP verified", 200);
        }

        public async Task<ApiResponse<string>> ResetPasswordAsync(string email, string newPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return ApiResponse<string>.Fail("User not found", 404);

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _context.SaveChangesAsync();

            return ApiResponse<string>.Ok(null, "Password updated successfully", 200);
        }

        //update profile
        public async Task<ApiResponse<string>> UpdateProfileAsync(string userId, UpdateProfileDto dto)
        {
            int id = int.Parse(userId);
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return ApiResponse<string>.Fail("User not found", 404);

            
            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                if (!Regex.IsMatch(dto.Name, @"^[a-zA-Z\s]{2,50}$"))
                    return ApiResponse<string>.Fail("Invalid name format", 400);

                user.Name = dto.Name.Trim();
            }

           
            if (!string.IsNullOrWhiteSpace(dto.Phone) && dto.Phone != "string")
            {
                var phone = dto.Phone.Trim();

                if (!Regex.IsMatch(phone, @"^[6-9]\d{9}$"))
                    return ApiResponse<string>.Fail("Invalid phone number", 400);

                user.Phone = phone;
            }

            await _context.SaveChangesAsync();

            return ApiResponse<string>.Ok(null, "Profile updated successfully", 200);
        }



    }
}
