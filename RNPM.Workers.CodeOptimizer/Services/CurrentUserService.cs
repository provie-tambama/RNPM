using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using RNPM.Common.Interfaces;

namespace RNPM.Workers.CodeOptimizer.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService(IConfiguration config)
        {
            UserId = config["Users:SystemAdmin"];
        }

        public string UserId { get; }
    }
}
