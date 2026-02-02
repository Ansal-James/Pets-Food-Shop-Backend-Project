using PFS.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFS.Application.Interface
{
    public interface ITokenService
    {
        string CreateToken(User user);
        string CreateRefreshToken();
    }
}
