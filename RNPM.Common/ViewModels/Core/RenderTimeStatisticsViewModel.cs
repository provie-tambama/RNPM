using RNPM.Common.Data.Enums;

namespace RNPM.Common.ViewModels.Core;

public class RenderTimeStatisticsViewModel
{
    public string Name { get; set; }
    public decimal Last100Average { get; set; }
    public decimal WeeklyAverage { get; set; }
    public decimal MonthlyAverage { get; set; }
    public decimal DailyAverage { get; set; }
    public decimal Average { get; set; }
}