using RNPM.Common.Data.Enums;

namespace RNPM.Common.ViewModels.Core;

public class NavigationStatisticsViewModel
{
    public decimal? Average { get; set; }
    public string? Insight { get; set; }
    public MetricStatus Status { get; set; }
    public string? Comment { get; set; }
}