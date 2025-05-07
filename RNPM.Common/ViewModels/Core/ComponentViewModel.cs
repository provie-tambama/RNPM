using RNPM.Common.Models;

namespace RNPM.Common.ViewModels.Core;

public class ComponentViewModel
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? ApplicationId { get; set; }
    public RenderTimeStatisticsViewModel? Statistics { get; set; }
    public virtual ICollection<ScreenComponentRender>? ScreenComponentRenders { get; set; }
}