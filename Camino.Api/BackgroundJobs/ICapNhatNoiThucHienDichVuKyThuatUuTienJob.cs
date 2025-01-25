using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.BackgroundJobs.Hangfire;
using Hangfire;

namespace Camino.Api.BackgroundJobs
{
    public interface ICapNhatNoiThucHienDichVuKyThuatUuTienJob
    {
        [SkipWhenPreviousJobIsRunning(Order = 1)]
        [DisableConcurrentExecution(60, Order = 2)]
        [AutomaticRetry(Attempts = 0)]
        void Run();
    }
}
