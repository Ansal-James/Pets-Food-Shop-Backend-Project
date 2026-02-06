using PFS.Application.Execeptions;
using System.Security.Claims;

namespace PFS.API.Helpers
{
    public class ClaimsHelper
    {
        public static Guid GetUserId(ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                throw new UnAuthorizedException("User ID claim is missing");

            return Guid.Parse(userId);
        }
    }
}
