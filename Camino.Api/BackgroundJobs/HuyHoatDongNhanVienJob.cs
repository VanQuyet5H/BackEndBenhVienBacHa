using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Data;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.BackgroundJobs
{
    [ScopedDependency(ServiceType = typeof(IHuyHoatDongNhanVienJob))]
    public class HuyHoatDongNhanVienJob : IHuyHoatDongNhanVienJob
    {
        private readonly IRepository<NhanVien> _nhanVienRepository;

        public HuyHoatDongNhanVienJob(IRepository<NhanVien> nhanVienRepository)
        {
            _nhanVienRepository = nhanVienRepository;
        }

        public void Run()
        {
            var nhanViens = _nhanVienRepository.Table.Include(x => x.HoatDongNhanViens).Include(x => x.LichSuHoatDongNhanViens).Where(o=>o.HoatDongNhanViens.Any()).ToList();
            foreach (var nhanVien in nhanViens)
            {
                foreach (var nhanVienHoatDongNhanVien in nhanVien.HoatDongNhanViens)
                {
                    if (!nhanVien.LichSuHoatDongNhanViens.Any(o =>o.PhongBenhVienId == nhanVienHoatDongNhanVien.PhongBenhVienId && (DateTime.Now - o.ThoiDiemBatDau).TotalHours < 12))
                    {
                        nhanVienHoatDongNhanVien.WillDelete = true;
                    }
                }
            }
            _nhanVienRepository.Context.SaveChanges();
        }
    }
}
