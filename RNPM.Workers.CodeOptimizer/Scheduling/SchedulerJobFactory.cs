using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;
using System;

namespace RNPM.Workers.CodeOptimizer.Scheduling
{
    public class SchedulerJobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public SchedulerJobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return _serviceProvider.GetRequiredService<ScheduledJobRunner>();
        }

        public void ReturnJob(IJob job)
        {
        }
    }
}
