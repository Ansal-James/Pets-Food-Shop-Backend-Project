using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PFS.Application.DTOs.Auth;
using PFS.Application.Execeptions;
using PFS.Application.Interface;
using PFS.Domain.Entites;
using PFS.Domain.Enums;
using PFS.Infrastructure.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PFS.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext context, ITokenService tokenService, IConfiguration configuration)
        {
            _context = context;
            _tokenService = tokenService;
            _config = configuration;
        }

        public async Task RegisterAsync(SignUpDto dto)
        {
            var val = validation(dto);
            var exisiting = await _context.Users.FirstOrDefaultAsync(s => s.Username == val.Username);
            if (exisiting != null)
                throw new AlreadyExisitException("This UserName is Already Exist!");

            

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = val.Username,
                Email = val.Email,
                PhoneNo = val.PhoneNo,
                PasswordHash = HashPassword(val.Password),
                Role = UserRole.User
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Username == dto.Username);

            if (user == null || !VerifyPassword(dto.Password, user.PasswordHash))
                throw new NotFoundException("Invalid Credentials");

            if (user.IsBlocked)
                throw new UnAuthorizedException("User is Blocked");

            var refreshToken = _tokenService.CreateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _context.SaveChangesAsync();

            return new LoginResponseDto
            {
                Username = user.Username,
                Role = user.Role.ToString(),
                Token = _tokenService.CreateToken(user),
                RefreshToken = refreshToken
            };
        }
        public async Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenDto dto)
        {
            var principal = GetPrincipalFromExpiredToken(dto.Token);

            var username = principal.Identity!.Name;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user.IsBlocked)
                throw new UnAuthorizedException("Sorry! User is Blocked");

            if (user == null || user.RefreshToken != dto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                throw new UnAuthorizedException("Invalid refresh token");

            var newAccessToken = _tokenService.CreateToken(user);
            var newRefreshToken = _tokenService.CreateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _context.SaveChangesAsync();

            return new LoginResponseDto
            {
                Username = user.Username,
                Role = user.Role.ToString(),
                Token = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var key = _config["Jwt:Key"]
                ?? throw new Exception("JWT Key not found in configuration");

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(key)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private SignUpDto validation(SignUpDto dto)
        {
            return new SignUpDto
            {
                Username = dto.Username?.Trim() ?? string.Empty,
                Password = dto.Password?.Trim() ?? string.Empty,
                Email = dto.Email?.Trim().ToLower() ?? string.Empty,
                PhoneNo = dto.PhoneNo?.Trim() ?? string.Empty
            };
        }

        private bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }
    }
}
