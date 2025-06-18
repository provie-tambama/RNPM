namespace RNPM.Common.ViewModels.Core;

public class CreateRenderComponentViewModel
{
    public required string UniqueAccessCode { get; set; }
    public required string Name { get; set; }
    
    
    public required Decimal RenderTime { get; set; }
    public DeviceInfoViewModel DeviceInfo { get; set; }
    
}