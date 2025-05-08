namespace RNPM.CodeOptimizer.Responses;

public class CodeOptimizationResponse
{
    public bool Success { get; set; }

    public string Message { get; set; }

    public string OriginalCode { get; set; }

    public string OptimizationSuggestion { get; set; }

    public List<string> OptimizationCategories { get; set; } = new List<string>();
}