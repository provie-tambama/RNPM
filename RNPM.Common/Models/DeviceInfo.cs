namespace RNPM.Common.Models;

public class DeviceInfo : BaseEntity
{
    public string Id { get; set; }

    // Basic device identifiers
    public string Brand { get; set; }
    public string ModelName { get; set; }
    public string Manufacturer { get; set; }
    public string DeviceName { get; set; }

    // OS information
    public string Os { get; set; }
    public string OsVersion { get; set; }
    public string OsName { get; set; }
    public string OsBuildId { get; set; }

    // Device type and characteristics
    public string DeviceType { get; set; }
    public int DeviceYearClass { get; set; }
    public bool IsDevice { get; set; }

    // Hardware specifications
    public string SupportedCpuArchitectures { get; set; }
    public long TotalMemory { get; set; }

    // Relationships
    public virtual ICollection<ScreenComponentRender> ScreenComponentRenders { get; set; }
    public virtual ICollection<NavigationInstance> NavigationInstances { get; set; }
    public virtual ICollection<HttpRequestInstance> HttpRequestInstances { get; set; }
}