namespace RNPM.CodeOptimizer.Requests;

public class CodeOptimizationRequest
{
    public string ComponentId { get; set; }
    public string ComponentName { get; set; }

    public string Code { get; set; }

    public string ComponentType { get; set; }
}