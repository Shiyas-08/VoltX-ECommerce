
using ECommerce.Data;
using ECommerce.DTOs;
using ECommerce.Models;
using ECommerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IAuthService _authService;
        private readonly IConfiguration _config;

        public AuthController(
            AppDbContext context,
            IAuthService authService,
            IConfiguration config)
        {
            _context = context;
            _authService = authService;
            _config = config;
        }

        //register

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<string>.Fail(
                    ModelState.Values.First().Errors.First().ErrorMessage,
                    StatusCodes.Status400BadRequest
                ));
            }

            try
            {
                _authService.Register(dto);

                return Ok(ApiResponse<string>.Ok(
                    "OK",
                    "User registered successfully",
                    StatusCodes.Status200OK
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Fail(
                    ex.Message,
                    StatusCodes.Status400BadRequest
                ));
            }
        }

       //login

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            try
            {
                var user = _authService.Login(dto);

                var accessToken = GenerateAccessToken(user);
                var refreshToken = user.RefreshToken;

                Response.Cookies.Append(
                    "access_token",
                    accessToken,
                    GetAuthCookieOptions(DateTime.UtcNow.AddMinutes(15))
                );

                Response.Cookies.Append(
                    "refresh_token",
                    refreshToken!,
                    GetAuthCookieOptions(DateTime.UtcNow.AddDays(7))
                );

                return Ok(new AuthResponseDto
                {
                    Message = "Login successful",
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //refresh

        [AllowAnonymous]
        [HttpPost("refresh")]
        public IActionResult Refresh()
        {
            var refreshToken = Request.Cookies["refresh_token"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(ApiResponse<string>.Fail(
                    "Refresh token missing",
                    StatusCodes.Status401Unauthorized
                ));
            }

            var user = _context.Users.SingleOrDefault(u =>
                u.RefreshToken == refreshToken &&
                u.RefreshTokenExpiry > DateTime.UtcNow &&
                !u.IsBlocked
            );

            if (user == null)
            {
                return Unauthorized(ApiResponse<string>.Fail(
                    "Session expired or user blocked",
                    StatusCodes.Status401Unauthorized
                ));
            }

            var newAccessToken = GenerateAccessToken(user);

            Response.Cookies.Append(
                "access_token",
                newAccessToken,
                GetAuthCookieOptions(DateTime.UtcNow.AddMinutes(15))
            );

            return Ok(ApiResponse<string>.Ok(
                "OK",
                "Access token refreshed",
                StatusCodes.Status200OK
            ));
        }

        //profile

        [Authorize]
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var user = _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new
                {
                    id = u.Id,
                    name = u.Name,
                    email = u.Email,
                    phone = u.Phone,
                    roleId = u.RoleId,
                    isBlocked = u.IsBlocked
                })
                .FirstOrDefault();

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        //logout

        [AllowAnonymous]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,               
                SameSite = SameSiteMode.None,
                Path = "/"
            };

            Response.Cookies.Delete("access_token", cookieOptions);
            Response.Cookies.Delete("refresh_token", cookieOptions);

            return Ok(new { message = "Logged out successfully" });
        }

        //jwt

        private string GenerateAccessToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("roleId", user.RoleId.ToString())
            };

            var key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
            );

            var creds = new Microsoft.IdentityModel.Tokens.SigningCredentials(
                key,
                Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256
            );

            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: creds
            );

            return new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler()
                .WriteToken(token);
        }

        

        private CookieOptions GetAuthCookieOptions(DateTime expires)
        {
            return new CookieOptions
            {
                HttpOnly = true,
                Secure = true,                
                SameSite = SameSiteMode.None, 
                Path = "/",
                Expires = expires
            };
        }

        // forgout password

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            var result = await _authService.ForgotPasswordAsync(dto.Email);
            return StatusCode(result.StatusCode, result);
        }

        [AllowAnonymous]
        // verify otp
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(VerifyOtpDto dto)
        {
            var result = await _authService.VerifyOtpAsync(dto.Email, dto.Otp);
            return StatusCode(result.StatusCode, result);
        }

        [AllowAnonymous]
        //reset password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            var result = await _authService.ResetPasswordAsync(dto.Email, dto.NewPassword);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            var result = await _authService.UpdateProfileAsync(userId, dto);
            return StatusCode(result.StatusCode, result);
        }
    }
}
