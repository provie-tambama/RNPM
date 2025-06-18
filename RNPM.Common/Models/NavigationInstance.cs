namespace RNPM.Common.Models;

public class NavigationInstance : BaseEntity
{
    public string Id { get; set; }
    public string NavigationId { get; set; }
    public DateTime Date { get; set; }
    public Decimal NavigationCompletionTime { get; set; }
    public string DeviceInfoId { get; set; }
    public virtual DeviceInfo DeviceInfo { get; set; }
    public virtual Navigation Navigation { get; set; }
}