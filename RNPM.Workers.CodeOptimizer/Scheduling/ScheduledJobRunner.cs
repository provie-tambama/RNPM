using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Threading.Tasks;

namespace RNPM.Workers.CodeOptimizer.Scheduling
{
    public class ScheduledJobRunner : IJob
    {
        private readonly IServiceProvider _serviceProvider;
        public ScheduledJobRunner(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using var scope = _serviceProvider.CreateScope();
            var jobType = context.JobDetail.JobType;
            var job = scope.ServiceProvider.GetRequiredService(jobType) as IJob;

            await job.Execute(context);
        }
    }
}
