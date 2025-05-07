using System.Collections.Generic;
using RNPM.Common.Models;

namespace RNPM.Common.Constants;

public static class ApplicationRoles
{
    public static List<ApplicationRole> GetApplicationRoles()
    {
        return new()
        {
            new ApplicationRole {
                Name = ApiRoles.SystemAdministrator,
                Description = "System Administrator",
                RoleClass = RoleClass.System,
                RoleClaims = new List<ApplicationRoleClaim>
                {
                }
            },
            new ApplicationRole {
                Name = ApiRoles.Administrator,
                Description = "Administrator",
                RoleClass = RoleClass.System,
                RoleClaims = new List<ApplicationRoleClaim>
                {
                }
            },
            new ApplicationRole
            {
                Name = ApiRoles.Deliverer,
                Description = "Deliverer",
                RoleClass = RoleClass.General,
                RoleClaims = new List<ApplicationRoleClaim>
                {
                }
            },
            
        };
    }
}
