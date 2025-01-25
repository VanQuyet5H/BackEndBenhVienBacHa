using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject.DichVuKyThuat;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Data;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.BackgroundJobs
{
    [ScopedDependency(ServiceType = typeof(ICapNhatNoiThucHienDichVuKyThuatUuTienJob))]
    public class CapNhatNoiThucHienDichVuKyThuatUuTienJob : ICapNhatNoiThucHienDichVuKyThuatUuTienJob
    {
        private IRepository<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuatRepository;
        private IRepository<DichVuKyThuatBenhVienNoiThucHienUuTien> _dichVuKyThuatBenhVienNoiThucHienUuTienRepository;

        public CapNhatNoiThucHienDichVuKyThuatUuTienJob(IRepository<YeuCauDichVuKyThuat> yeuCauDichVuKyThuatRepository,
            IRepository<DichVuKyThuatBenhVienNoiThucHienUuTien> dichVuKyThuatBenhVienNoiThucHienUuTienRepository)
        {
            _yeuCauDichVuKyThuatRepository = yeuCauDichVuKyThuatRepository;
            _dichVuKyThuatBenhVienNoiThucHienUuTienRepository = dichVuKyThuatBenhVienNoiThucHienUuTienRepository;
        }

        public void Run()
        {
            // clear all data nơi thực hiện ưu tiên hiện tại với loại hệ thống
            _dichVuKyThuatBenhVienNoiThucHienUuTienRepository.Context.Database.ExecuteSqlCommand("EXEC [dbo].[sp_update_noi_thuc_hien_uu_tien_dvkt]");

            //// get all yêu cầu dvkt, group lại và get nơi thực hiện được chọn nhiều nhất
            //var lstDichVuKyThuatBenhVienNoiThucHienUuTien = _yeuCauDichVuKyThuatRepository.TableNoTracking
            //    .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy 
            //                && x.NoiThucHienId != null
            //                && x.NoiThucHien != null
            //                && x.NoiThucHien.HopDongKhamSucKhoeId == null)
            //    .GroupBy(x => x.DichVuKyThuatBenhVienId)
            //    .Select(x => new DichVuKyThuatBenhVienNoiThucHienUuTien
            //    {
            //        DichVuKyThuatBenhVienId = x.First().DichVuKyThuatBenhVienId,
            //        PhongBenhVienId = x.GroupBy(a => a.NoiThucHienId).OrderByDescending(g => g.Count()).First().Key ?? 0,
            //        LoaiNoiThucHienUuTien = Enums.LoaiNoiThucHienUuTien.HeThong
            //    })
            //    .ToList();
            //_dichVuKyThuatBenhVienNoiThucHienUuTienRepository.AddRange(lstDichVuKyThuatBenhVienNoiThucHienUuTien);
            //_dichVuKyThuatBenhVienNoiThucHienUuTienRepository.Context.SaveChanges();
            //// lưu lại thông tin nơi thực hiện ưu tiên
        }
    }
}
