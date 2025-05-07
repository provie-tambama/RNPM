namespace RNPM.Common.ViewModels.Core;

public class RenderInstanceViewModel
{
    public string? Id { get; set; }
    
    public string? ComponentId { get; set; }
    
    public DateTime Date { get; set; }
    
    public Decimal RenderTime { get; set; }
}