using PFS.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFS.Application.Interface
{
    public interface IAuthService
    {
        Task RegisterAsync(SignUpDto dto);
        Task<LoginResponseDto> LoginAsync(LoginDto dto);
        Task<LoginResponseDto> AdminLoginAsync(LoginDto dto);
        Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenDto dto);
    }
}
