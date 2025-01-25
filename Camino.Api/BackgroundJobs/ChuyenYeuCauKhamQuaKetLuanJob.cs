using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Data;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.BackgroundJobs
{
    [ScopedDependency(ServiceType = typeof(IChuyenYeuCauKhamQuaKetLuanJob))]
    public class ChuyenYeuCauKhamQuaKetLuanJob : IChuyenYeuCauKhamQuaKetLuanJob
    {
        private readonly IRepository<YeuCauKhamBenh> _yeuCauKhamBenhRepository;

        public ChuyenYeuCauKhamQuaKetLuanJob(IRepository<YeuCauKhamBenh> yeuCauKhamBenhRepository)
        {
            _yeuCauKhamBenhRepository = yeuCauKhamBenhRepository;
        }

        public void Run()
        {
          var ycKhams = _yeuCauKhamBenhRepository.Table.Include(o=>o.PhongBenhVienHangDois).Where(o =>
                o.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DangLamChiDinh &&
                o.YeuCauDichVuKyThuats.Any(kt =>
                    kt.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.Khac &&
                    kt.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.TheoYeuCau)

                && o.YeuCauDichVuKyThuats.All(yc =>
                    yc.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.Khac ||
                    yc.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.TheoYeuCau ||
                    yc.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien ||
                    yc.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
            ).ToList();
            if (ycKhams.Count > 0)
            {
                foreach (var ycKham in ycKhams)
                {
                    ycKham.TrangThai = Enums.EnumTrangThaiYeuCauKhamBenh.DangDoiKetLuan;
                    ycKham.YeuCauKhamBenhLichSuTrangThais.Add(new YeuCauKhamBenhLichSuTrangThai
                    {
                        TrangThaiYeuCauKhamBenh = ycKham.TrangThai
                    });
                }
                _yeuCauKhamBenhRepository.Context.SaveChanges();
            }
        }
    }
}
