using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.DuocPhamBenhVien;

namespace Camino.Api.BackgroundJobs
{
    [ScopedDependency(ServiceType = typeof(ICapNhatMaDuocPhamBenhVienJob))]
    public class CapNhatMaDuocPhamBenhVienJob : ICapNhatMaDuocPhamBenhVienJob
    {
        private readonly IDuocPhamBenhVienService _duocPhamBenhVienService;
        public CapNhatMaDuocPhamBenhVienJob(IDuocPhamBenhVienService duocPhamBenhVienService)
        {
            _duocPhamBenhVienService = duocPhamBenhVienService;
        }
        public void Run()
        {
            _duocPhamBenhVienService.XuLyCapNhatMaDuocPhamBenhVien();
        }
    }
}
