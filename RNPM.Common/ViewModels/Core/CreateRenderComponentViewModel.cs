namespace RNPM.API.ViewModels.Core;

public class CreateRenderComponentViewModel
{
    public required string UniqueAccessCode { get; set; }
    public required string Name { get; set; }
    
    
    public required Decimal RenderTime { get; set; }
    
}