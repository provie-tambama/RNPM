namespace RNPM.Common.Models;

public class Navigation : BaseEntity
{
    public string? Id { get; set; }
    
    public string? ApplicationId { get; set; }
    
    public string FromScreen { get; set; }
    
    public string ToScreen { get; set; }
    
    public virtual Application? Application { get; set; }
    
    public virtual ICollection<NavigationInstance> NavigationInstances { get; set; }
}