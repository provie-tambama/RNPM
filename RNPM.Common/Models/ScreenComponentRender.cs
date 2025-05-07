namespace RNPM.Common.Models;

public class ScreenComponentRender : BaseEntity
{
    public string Id { get; set; }
    
    public string ComponentId { get; set; }
    
    public DateTime Date { get; set; }
    
    public Decimal RenderTime { get; set; }
    public virtual ScreenComponent ScreenComponent { get; set; }
}