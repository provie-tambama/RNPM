using RNPM.Common.Models;

namespace RNPM.Common.ViewModels.Core;

public class NavigationViewModel
{
    public string? Id { get; set; }
    public string? FromScreen { get; set; }
    public string? ToScreen { get; set; }
    public string? ApplicationId { get; set; }
    public NavigationStatisticsViewModel? Statistics { get; set; }
    public virtual ICollection<NavigationInstance>? NavigationInstances { get; set; }
}