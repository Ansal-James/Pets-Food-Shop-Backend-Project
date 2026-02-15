using Microsoft.EntityFrameworkCore;
using PFS.Application.DTOs.User;
using PFS.Application.Execeptions;
using PFS.Application.Interface;
using PFS.Domain.Enums;
using PFS.Infrastructure.Data;

namespace PFS.Infrastructure.Services
{
    public class AdminUserService : IAdminUserService
    {
        private readonly AppDbContext _context;

        public AdminUserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AdminUserListDto>> GetAllUsersAsync()
        {
            return await _context.Users
                .Where(u => u.Role == UserRole.User)
                .Select(u => new AdminUserListDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    IsBlocked = u.IsBlocked
                })
                .ToListAsync();
        }

        public async Task<AdminUserDetailsDto> GetUserByIdAsync(Guid userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId && u.Role == UserRole.User);

            if (user == null)
                throw new NotFoundException("User not found");

            return new AdminUserDetailsDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                IsBlocked = user.IsBlocked,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task BlockUserAsync(Guid userId)
        {
            var user = await GetUserEntity(userId);
            user.IsBlocked = true;
            await _context.SaveChangesAsync();
        }

        public async Task UnblockUserAsync(Guid userId)
        {
            var user = await GetUserEntity(userId);
            user.IsBlocked = false;
            await _context.SaveChangesAsync();
        }

        private async Task<Domain.Entites.User> GetUserEntity(Guid userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId && u.Role == UserRole.User);

            if (user == null)
                throw new NotFoundException("User not found");

            return user;
        }
    }
}
