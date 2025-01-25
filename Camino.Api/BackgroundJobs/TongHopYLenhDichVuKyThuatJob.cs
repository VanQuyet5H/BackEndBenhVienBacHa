using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.DieuTriNoiTru;

namespace Camino.Api.BackgroundJobs
{
    [ScopedDependency(ServiceType = typeof(ITongHopYLenhDichVuKyThuatJob))]
    public class TongHopYLenhDichVuKyThuatJob : ITongHopYLenhDichVuKyThuatJob
    {
        private readonly IDieuTriNoiTruService _dieuTriNoiTruService;
        public TongHopYLenhDichVuKyThuatJob(IDieuTriNoiTruService dieuTriNoiTruService)
        {
            _dieuTriNoiTruService = dieuTriNoiTruService;
        }

        public void Run()
        {
            _dieuTriNoiTruService.XuLyTongHopYLenhDichVuKyThuat();
        }
    }
}
