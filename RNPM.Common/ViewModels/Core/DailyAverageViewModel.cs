namespace RNPM.Common.ViewModels.Core;

public class DailyAverageViewModel
{
    public DateTime Date { get; set; }
    public Decimal Average { get; set; }
    public int Count { get; set; }
    public List<DeviceAverageViewModel> DeviceBreakdown { get; set; }
}