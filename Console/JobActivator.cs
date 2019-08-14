using System;

namespace Console
{
    public class JobActivator : Hangfire.JobActivator
    {
        private readonly IServiceProvider _serviceProvider;

        public JobActivator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override object ActivateJob(Type type)
        {
            return _serviceProvider.GetService(type);
        }
    }
}