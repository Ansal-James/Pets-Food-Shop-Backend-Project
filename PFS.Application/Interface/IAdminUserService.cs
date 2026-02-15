using PFS.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFS.Application.Interface
{
    public interface IAdminUserService
    {
        Task<List<AdminUserListDto>> GetAllUsersAsync();
        Task<AdminUserDetailsDto> GetUserByIdAsync(Guid userId);
        Task BlockUserAsync(Guid userId);
        Task UnblockUserAsync(Guid userId);
    }
}
