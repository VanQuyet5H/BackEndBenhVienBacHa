using Camino.Api.BackgroundJobs.Hangfire;
using Hangfire;

namespace Camino.Api.BackgroundJobs
{
    public interface ISendEmailJob
    {
        [SkipWhenPreviousJobIsRunning(Order = 1)]
        [DisableConcurrentExecution(60, Order = 2)]
        [AutomaticRetry(Attempts = 0)]
        void Run();
    }
}
