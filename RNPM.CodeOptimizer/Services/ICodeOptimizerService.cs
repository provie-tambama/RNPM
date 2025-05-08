using System.Threading.Tasks;
using RNPM.CodeOptimizer.Requests;
using RNPM.CodeOptimizer.Responses;

namespace RNPM.CodeOptimizer.Services;

public interface ICodeOptimizerService
{
    Task<CodeOptimizationResponse> OptimizeCodeAsync(CodeOptimizationRequest request);
    
    Task<bool> IsServiceAvailableAsync();
}