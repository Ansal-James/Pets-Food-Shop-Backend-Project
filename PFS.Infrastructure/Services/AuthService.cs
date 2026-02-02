using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PFS.Application.DTOs.Auth;
using PFS.Application.Execeptions;
using PFS.Application.Interface;
using PFS.Domain.Entites;
using PFS.Domain.Enums;
using PFS.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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
            var exisiting = await _context.Users.FirstOrDefaultAsync(s => s.Username == dto.Username);
            if (exisiting != null)
                throw new AlreadyExisitException("This UserName is Already Exist !");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = dto.Username,
                Email = dto.Email,
                PhoneNo = dto.PhoneNo,
                PasswordHash = HashPassword(dto.Password),
                Role = UserRole.User
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
        {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == dto.Username && x.Role == UserRole.User);
                if (user == null || !VerifyPassword(dto.Password, user.PasswordHash))
                    throw new NotFoundException("Invalid Credintials");

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
        public async Task<LoginResponseDto> AdminLoginAsync(LoginDto dto)
        {
            var admin = await _context.Users
                .FirstOrDefaultAsync(x => x.Username == dto.Username && x.Role == UserRole.Admin);

            if (admin == null || !VerifyPassword(dto.Password, admin.PasswordHash))
                throw new NotFoundException("Invalid Credentials");

            var refreshToken = _tokenService.CreateRefreshToken();

            admin.RefreshToken = refreshToken;
            admin.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _context.SaveChangesAsync();

            return new LoginResponseDto
            {
                Username = admin.Username,
                Role = admin.Role.ToString(),
                Token = _tokenService.CreateToken(admin),
                RefreshToken = refreshToken
            };
        }

        public async Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenDto dto)
        {
            var principal = GetPrincipalFromExpiredToken(dto.Token);

            var username = principal.Identity!.Name;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null || user.RefreshToken != dto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                throw new Exception("Invalid refresh token");

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
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken)
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
        private bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }
    }
}
