namespace RNPM.Common.Models;

public class ScreenComponent : BaseEntity
{
    public string Id { get; set; }
    
    public string ApplicationId { get; set; }
    
    public string Name { get; set; }
    public string SourceCode { get; set; }
    public string OptimizationSuggestion { get; set; }
    public virtual Application Application { get; set; }
    
    public virtual ICollection<ScreenComponentRender> ScreenComponentRenders { get; set; }
    public virtual ICollection<OptimizationSuggestion> OptimizationSuggestions { get; set; }
}