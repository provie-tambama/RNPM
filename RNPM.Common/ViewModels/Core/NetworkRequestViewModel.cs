using RNPM.Common.Models;

namespace RNPM.Common.ViewModels.Core;

public class NetworkRequestViewModel
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? ApplicationId { get; set; }
    public NetworkRequestStatisticsViewModel? Statistics { get; set; }
    public virtual ICollection<HttpRequestInstance>? HttpRequestInstances { get; set; }
}