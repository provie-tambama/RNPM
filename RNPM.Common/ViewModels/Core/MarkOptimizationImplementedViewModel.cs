namespace RNPM.Common.ViewModels.Core;

public class MarkOptimizationImplementedViewModel
{
    public required string OptimizationSuggestionId { get; set; }
    public bool ResetMeasurements { get; set; }
    public bool ClearOldMeasurements { get; set; }
    public decimal? NewThreshold { get; set; }
}