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
            try
            {
            Log.Error("Here we go {0}", _dateTimeService.Now);
            var components = await _context.ScreenComponents.Where(c => c.IsActive && !c.IsDeleted).ToListAsync();

            foreach (var c in components)
            {
                var screenRenderInstances = await 
                    _context.ScreenComponentRenders.Where(r => r.ComponentId == c.Id && c.IsActive && !c.IsArchived).ToListAsync();
                if (screenRenderInstances.Count > 0)
                {
                    var sumRenderTime = screenRenderInstances.Sum(r => r.RenderTime);
                    var avgRenderTime = sumRenderTime / screenRenderInstances.Count;

                    if (avgRenderTime > c.Threshold && c.Threshold > 0)
                    {
                        if (string.IsNullOrWhiteSpace(c.SourceCode))
                        {
                            Log.Error("Code not available for component {0} - {1}",c.Name, _dateTimeService.Now);
                        }
                        else
                        {
                            var activeOptimisationSuggestions =
                                await _context.OptimizationSuggestions.Where(a =>
                                    a.IsActive && !a.IsDeleted && a.IsImplemented == false).ToListAsync();
                            if (activeOptimisationSuggestions.Count != 0) continue;
                            var request = new CodeOptimizationRequest
                            {
                                ComponentName = c.Name,
                                Code = c.SourceCode,
                                ComponentId = c.Id
                            };
                            var suggestionResponse = await _codeOptimizerService.OptimizeCodeSubAsync(request);

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
            catch (Exception e)
            {
                Log.Error("Error in Code Optimizer Job: {0}", e.Message);
            }

        }
    }
}
