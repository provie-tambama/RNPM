
using RNPM.Common.Models;

namespace RNPM.Common.ViewModels.UserManagement;

public class RoleViewModel
{
    public string? Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public List<IdentityClaim>? Permissions { get; set; }
}

public class FilterRolesViewModel
{
    public List<RoleClass> RoleClasses { get; set; }
}