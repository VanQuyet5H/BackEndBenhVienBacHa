using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Configuration;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.DonVatTus;
using Camino.Core.Domain.Entities.Messages;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Infrastructure;
using Camino.Data;
using DocumentFormat.OpenXml.Drawing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.BackgroundJobs
{
    [ScopedDependency(ServiceType = typeof(IQuyetToanGoiDichVuDuocBaoLanh))]
    public class QuyetToanGoiDichVuDuocBaoLanh : IQuyetToanGoiDichVuDuocBaoLanh
    {
        private readonly IRepository<YeuCauTiepNhan> _yeuCauTiepNhanRepository;
        private readonly IRepository<YeuCauGoiDichVu> _yeuCauGoiDichVuRepository;
        private readonly ILoggerManager _logger;

        public QuyetToanGoiDichVuDuocBaoLanh(IRepository<YeuCauTiepNhan> yeuCauTiepNhanRepository, IRepository<YeuCauGoiDichVu> yeuCauGoiDichVuRepository, ILoggerManager logger)
        {
            _yeuCauTiepNhanRepository = yeuCauTiepNhanRepository;
            _yeuCauGoiDichVuRepository = yeuCauGoiDichVuRepository;
            _logger = logger;
        }

        public void Run()
        {
            var ycGoiDichVuCanHoanThanhs = _yeuCauGoiDichVuRepository.Table.Where(o =>
                o.DaQuyetToan != true &&
                o.ChuongTrinhGoiDichVu.CongTyBaoHiemTuNhanId != null
                && (o.YeuCauKhamBenhs.Any(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan) || o.YeuCauDichVuKyThuats.Any(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
                && o.YeuCauKhamBenhs.All(yc => (yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan || yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.HuyThanhToan) && yc.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat)
                && o.YeuCauDichVuKyThuats.All(yc => (yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan || yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.HuyThanhToan) && yc.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat)
                && o.YeuCauDichVuGiuongBenhVienChiPhiBenhViens.All(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan || yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.HuyThanhToan))
                .ToList();

            var saveChange = false;
            foreach (var ycGoiDichVuCanHoanThanh in ycGoiDichVuCanHoanThanhs)
            {
                ycGoiDichVuCanHoanThanh.DaQuyetToan = true;
                ycGoiDichVuCanHoanThanh.ThoiDiemQuyetToan = DateTime.Now;
                ycGoiDichVuCanHoanThanh.NhanVienQuyetToanId = (long)NhanVienHeThong.NhanVienThanhToanTuDong;
                ycGoiDichVuCanHoanThanh.NoiQuyetToanId = (long)PhongHeThong.PhongThanhToanTuDong;
                ycGoiDichVuCanHoanThanh.ThoiDiemHuyQuyetToan = null;
                ycGoiDichVuCanHoanThanh.NhanVienHuyQuyetToanId = null;
                saveChange = true;
                //if ((ycGoiDichVuCanHoanThanh.YeuCauKhamBenhs.Any(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan) || 
                //    ycGoiDichVuCanHoanThanh.YeuCauDichVuKyThuats.Any(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
                //    && ycGoiDichVuCanHoanThanh.YeuCauKhamBenhs.All(yc=>yc.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat)
                //    && ycGoiDichVuCanHoanThanh.YeuCauDichVuKyThuats.All(yc => yc.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat))
                //{
                //    ycGoiDichVuCanHoanThanh.DaQuyetToan = true;
                //    ycGoiDichVuCanHoanThanh.ThoiDiemQuyetToan = DateTime.Now;
                //    ycGoiDichVuCanHoanThanh.NhanVienQuyetToanId = (long)NhanVienHeThong.NhanVienThanhToanTuDong;
                //    ycGoiDichVuCanHoanThanh.NoiQuyetToanId = (long)PhongHeThong.PhongThanhToanTuDong;
                //    ycGoiDichVuCanHoanThanh.ThoiDiemHuyQuyetToan = null;
                //    ycGoiDichVuCanHoanThanh.NhanVienHuyQuyetToanId = null;
                //    saveChange = true;
                //}
            }
            if(saveChange)
            {
                _yeuCauTiepNhanRepository.Context.SaveChanges();
            }
        }
    }
}
