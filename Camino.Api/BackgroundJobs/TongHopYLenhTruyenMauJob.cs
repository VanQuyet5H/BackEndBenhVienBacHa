using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.DieuTriNoiTru;

namespace Camino.Api.BackgroundJobs
{
    [ScopedDependency(ServiceType = typeof(ITongHopYLenhTruyenMauJob))]
    public class TongHopYLenhTruyenMauJob : ITongHopYLenhTruyenMauJob
    {
        private readonly IDieuTriNoiTruService _dieuTriNoiTruService;
        public TongHopYLenhTruyenMauJob(IDieuTriNoiTruService dieuTriNoiTruService)
        {
            _dieuTriNoiTruService = dieuTriNoiTruService;
        }

        public void Run()
        {
            _dieuTriNoiTruService.XuLyTongHopYLenhTruyenMau();
        }
    }
}
