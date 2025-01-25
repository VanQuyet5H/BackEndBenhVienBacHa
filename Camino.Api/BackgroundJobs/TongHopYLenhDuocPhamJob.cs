using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.DieuTriNoiTru;

namespace Camino.Api.BackgroundJobs
{
    [ScopedDependency(ServiceType = typeof(ITongHopYLenhDuocPhamJob))]
    public class TongHopYLenhDuocPhamJob : ITongHopYLenhDuocPhamJob
    {
        private readonly IDieuTriNoiTruService _dieuTriNoiTruService;
        public TongHopYLenhDuocPhamJob(IDieuTriNoiTruService dieuTriNoiTruService)
        {
            _dieuTriNoiTruService = dieuTriNoiTruService;
        }

        public void Run()
        {
            _dieuTriNoiTruService.XuLyTongHopYLenhDuocPham();
        }
    }
}
