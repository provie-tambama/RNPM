namespace RNPM.Common.Models;

public class OptimizationSuggestion : BaseEntity
{
    public string Id { get; set; }
    public string ComponentId { get; set; }
    public string Prompt { get; set; }
    public string Suggestion { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
    
    public virtual ScreenComponent Component { get; set; }
}