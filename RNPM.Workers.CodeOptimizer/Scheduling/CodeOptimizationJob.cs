using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RNPM.CodeOptimizer.Requests;
using RNPM.CodeOptimizer.Services;
using RNPM.Common.Data;
using RNPM.Common.Interfaces;
using RNPM.Common.Models;
using Serilog;

namespace RNPM.Workers.CodeOptimizer.Scheduling
{
    [DisallowConcurrentExecution]
    public class CodeOptimizationJob : IJob
    {
        private readonly RnpmDbContext _context;
        private readonly IDateTimeService _dateTimeService;
        private ICodeOptimizerService _codeOptimizerService;

        public CodeOptimizationJob(
            RnpmDbContext context,
            IDateTimeService dateTimeService, ICodeOptimizerService codeOptimizerService)
        {
            _context = context;
            _dateTimeService = dateTimeService;
            _codeOptimizerService = codeOptimizerService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var components = await _context.ScreenComponents.Where(c => c.IsActive && !c.IsDeleted).ToListAsync();

            foreach (var c in components)
            {
                var screenRenderInstances = await 
                    _context.ScreenComponentRenders.Where(r => r.ComponentId == c.Id && c.IsActive && !c.IsArchived).ToListAsync();
                if (screenRenderInstances.Count > 0)
                {
                    var sumRenderTime = screenRenderInstances.Sum(r => r.RenderTime);
                    var avgRenderTime = sumRenderTime / screenRenderInstances.Count;

                    if (avgRenderTime > c.Threshold)
                    {
                        var request = new CodeOptimizationRequest
                        {
                            ComponentName = c.Name,
                            Code = c.SourceCode,
                            ComponentId = c.Id
                        };
                        var suggestionResponse = await _codeOptimizerService.OptimizeCodeAsync(request);

                        if (suggestionResponse.Success)
                        {
                            c.OptimizationSuggestion = suggestionResponse.OptimizationSuggestion;
                            _context.Update(c);

                            var optimizationSuggestion = new OptimizationSuggestion
                            {
                                ComponentId = c.Id,
                                Prompt = c.SourceCode,
                                Suggestion = suggestionResponse.OptimizationSuggestion,
                                Success = true,
                                Message = suggestionResponse.Message,
                            };

                            await _context.AddAsync(optimizationSuggestion);
                            await _context.SaveChangesAsync().ConfigureAwait(false);
                        }
                        else
                        {
                            Log.Error("Suggestion unsuccessful: {}",suggestionResponse.Message);
                        }
                        
                    }
                }
            }
        }
    }
}
