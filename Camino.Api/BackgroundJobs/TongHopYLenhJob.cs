using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.DieuTriNoiTru;

namespace Camino.Api.BackgroundJobs
{
    [ScopedDependency(ServiceType = typeof(ITongHopYLenhJob))]
    public class TongHopYLenhJob : ITongHopYLenhJob
    {
        private readonly IDieuTriNoiTruService _dieuTriNoiTruService;
        public TongHopYLenhJob(IDieuTriNoiTruService dieuTriNoiTruService)
        {
            _dieuTriNoiTruService = dieuTriNoiTruService;
        }

        public void Run()
        {
            _dieuTriNoiTruService.XuLyTongHopYLenh();
        }
    }
}
