using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using RNPM.Common.Interfaces;

namespace RNPM.API.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            UserId = httpContextAccessor.HttpContext?.User?.FindFirst(c => c.Type == ClaimTypes.PrimarySid)?.Value ?? string.Empty;
        }
        public string UserId { get; }
    }
}
