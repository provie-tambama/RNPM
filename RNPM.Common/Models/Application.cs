using System.Collections.Generic;

namespace RNPM.Common.Models;

public class Application : BaseEntity
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string UserId { get; set; }
    public string UniqueAccessCode { get; set; }
    public ApplicationUser User { get; set; }
    public virtual ICollection<ScreenComponent> ScreenComponents { get; set; }
    public virtual ICollection<Navigation> Navigations { get; set; }
    public virtual ICollection<NetworkRequest> NetworkRequests { get; set; }
}