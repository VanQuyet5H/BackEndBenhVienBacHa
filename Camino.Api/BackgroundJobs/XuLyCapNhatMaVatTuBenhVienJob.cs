using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.VatTuBenhViens;

namespace Camino.Api.BackgroundJobs
{
    [ScopedDependency(ServiceType = typeof(IXuLyCapNhatMaVatTuBenhVienJob))]
    public class XuLyCapNhatMaVatTuBenhVienJob : IXuLyCapNhatMaVatTuBenhVienJob
    {
        private readonly IVatTuBenhVienService _vatTuBenhVienService;
        public XuLyCapNhatMaVatTuBenhVienJob(IVatTuBenhVienService vatTuBenhVienService)
        {
            _vatTuBenhVienService = vatTuBenhVienService;
        }

        public void Run()
        {
            _vatTuBenhVienService.XuLyCapNhatMaVatTuBenhVien();
        }
    }
}
