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
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Infrastructure;
using Camino.Data;
using Camino.Services.CauHinh;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Api.BackgroundJobs
{
    [ScopedDependency(ServiceType = typeof(IHoanThanhYeuCauTiepNhanJob))]
    public class HoanThanhYeuCauTiepNhanJob : IHoanThanhYeuCauTiepNhanJob
    {
        private readonly IRepository<YeuCauTiepNhan> _yeuCauTiepNhanRepository;
        private readonly IRepository<DonThuocThanhToan> _donThuocThanhToanRepository;
        private readonly IRepository<DonVTYTThanhToan> _donVTYTThanhToanRepository;
        private readonly ILoggerManager _logger;
        private readonly ICauHinhService _cauHinhService;

        public HoanThanhYeuCauTiepNhanJob(IRepository<YeuCauTiepNhan> yeuCauTiepNhanRepository, IRepository<DonThuocThanhToan> donThuocThanhToanRepository, IRepository<DonVTYTThanhToan> donVTYTThanhToanRepository, ILoggerManager logger, ICauHinhService cauHinhService)
        {
            _yeuCauTiepNhanRepository = yeuCauTiepNhanRepository;
            _donThuocThanhToanRepository = donThuocThanhToanRepository;
            _donVTYTThanhToanRepository = donVTYTThanhToanRepository;
            _logger = logger;
            _cauHinhService = cauHinhService;
        }

        public void Run()
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var ngayHuy = DateTime.Now.AddHours(cauHinhChung.ThoiGianGiuDonThuoc * (-1));
            //huy don thuoc
            try
            {
                var dsDonThuocCanHuys = _donThuocThanhToanRepository.Table
                .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.CongTyBaoHiemTuNhanCongNos)
                .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.MienGiamChiPhis)
                .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.TaiKhoanBenhNhanChis)
                .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.XuatKhoDuocPhamChiTietViTri).ThenInclude(o => o.NhapKhoDuocPhamChiTiet)
                .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.XuatKhoDuocPhamChiTietViTri).ThenInclude(o => o.XuatKhoDuocPhamChiTiet)
                .Where(o => o.TrangThaiThanhToan != Enums.TrangThaiThanhToan.DaThanhToan && o.TrangThaiThanhToan != Enums.TrangThaiThanhToan.HuyThanhToan && o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocKhongBHYT
                && o.CreatedOn != null && o.CreatedOn.Value < ngayHuy && (o.YeuCauKhamBenhId == null || o.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham)).ToList();

                foreach (var donthuocCanHuy in dsDonThuocCanHuys)
                {
                    foreach (var donThuocThanhToanChiTiet in donthuocCanHuy.DonThuocThanhToanChiTiets)
                    {
                        foreach (var congNo in donThuocThanhToanChiTiet.CongTyBaoHiemTuNhanCongNos)
                        {
                            congNo.WillDelete = true;
                        }
                        foreach (var mienGiam in donThuocThanhToanChiTiet.MienGiamChiPhis)
                        {
                            mienGiam.WillDelete = true;
                        }
                        foreach (var taiKhoanBenhNhanChi in donThuocThanhToanChiTiet.TaiKhoanBenhNhanChis)
                        {
                            taiKhoanBenhNhanChi.DonThuocThanhToanChiTietId = null;
                        }
                        donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat = Math.Round(donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat - donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.SoLuongXuat, 2);
                        donThuocThanhToanChiTiet.WillDelete = true;
                        donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.WillDelete = true;
                        donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.WillDelete = true;
                    }
                    donthuocCanHuy.WillDelete = true;
                }
                if (dsDonThuocCanHuys.Any())
                {
                    _donThuocThanhToanRepository.Context.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                _logger.LogError($"Huy don thuoc: {ex}");
            }
            try
            {
                //huy don vat tu
                var dsDonVTYTCanHuys = _donVTYTThanhToanRepository.Table
                    .Include(o => o.DonVTYTThanhToanChiTiets).ThenInclude(o => o.CongTyBaoHiemTuNhanCongNos)
                    .Include(o => o.DonVTYTThanhToanChiTiets).ThenInclude(o => o.MienGiamChiPhis)
                    .Include(o => o.DonVTYTThanhToanChiTiets).ThenInclude(o => o.TaiKhoanBenhNhanChis)
                    .Include(o => o.DonVTYTThanhToanChiTiets).ThenInclude(o => o.XuatKhoVatTuChiTietViTri).ThenInclude(o => o.NhapKhoVatTuChiTiet)
                    .Include(o => o.DonVTYTThanhToanChiTiets).ThenInclude(o => o.XuatKhoVatTuChiTietViTri).ThenInclude(o => o.XuatKhoVatTuChiTiet)
                    .Where(o => o.TrangThaiThanhToan != Enums.TrangThaiThanhToan.DaThanhToan && o.TrangThaiThanhToan != Enums.TrangThaiThanhToan.HuyThanhToan
                && o.CreatedOn != null && o.CreatedOn.Value < ngayHuy && (o.YeuCauKhamBenhId == null || o.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham)).ToList();

                foreach (var donVTYTCanHuy in dsDonVTYTCanHuys)
                {
                    foreach (var donVTYTThanhToanChiTiet in donVTYTCanHuy.DonVTYTThanhToanChiTiets)
                    {
                        foreach (var congNo in donVTYTThanhToanChiTiet.CongTyBaoHiemTuNhanCongNos)
                        {
                            congNo.WillDelete = true;
                        }
                        foreach (var mienGiam in donVTYTThanhToanChiTiet.MienGiamChiPhis)
                        {
                            mienGiam.WillDelete = true;
                        }
                        foreach (var taiKhoanBenhNhanChi in donVTYTThanhToanChiTiet.TaiKhoanBenhNhanChis)
                        {
                            taiKhoanBenhNhanChi.DonVTYTThanhToanChiTietId = null;
                        }
                        donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat = Math.Round(donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat - donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.SoLuongXuat, 2);
                        donVTYTThanhToanChiTiet.WillDelete = true;
                        donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.WillDelete = true;
                        donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.WillDelete = true;
                    }
                    donVTYTCanHuy.WillDelete = true;
                }
                if (dsDonVTYTCanHuys.Any())
                {
                    _donVTYTThanhToanRepository.Context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Huy don vat tu: {ex}");
            }

            var yctnCanHoanThanhs = _yeuCauTiepNhanRepository.Table.Where(o =>
                o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru &&
                o.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien &&
                o.ThoiDiemTiepNhan.DayOfYear != DateTime.Now.DayOfYear

                && o.YeuCauKhamBenhs.All(yc =>
                    (yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan || yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.HuyThanhToan) &&
                    (yc.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham || yc.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                    && !yc.YeuCauNhapViens.Any(nv=> !nv.YeuCauTiepNhans.Any() || nv.YeuCauTiepNhans.Any(tnnt=>tnnt.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy)))
                && o.YeuCauDichVuKyThuats.All(yc =>
                    (yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan || yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.HuyThanhToan) &&
                    (yc.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.Khac || yc.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.TheoYeuCau || yc.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien || yc.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
                //&& o.YeuCauDichVuGiuongBenhViens.All(yc => (yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan || yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.HuyThanhToan) && (yc.TrangThai == Enums.EnumTrangThaiGiuongBenh.DaThucHien || yc.TrangThai == Enums.EnumTrangThaiGiuongBenh.DaHuy))
                && o.YeuCauDuocPhamBenhViens.All(yc =>
                    (yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan || yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.HuyThanhToan) &&
                    (yc.TrangThai == Enums.EnumYeuCauDuocPhamBenhVien.DaThucHien || yc.TrangThai == Enums.EnumYeuCauDuocPhamBenhVien.DaHuy))
                && o.YeuCauVatTuBenhViens.All(yc =>
                    (yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan || yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.HuyThanhToan) &&
                    (yc.TrangThai == Enums.EnumYeuCauVatTuBenhVien.DaThucHien || yc.TrangThai == Enums.EnumYeuCauVatTuBenhVien.DaHuy))
                && o.DonThuocThanhToans.All(yc =>
                    (yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan ||
                     yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.HuyThanhToan) &&
                    (yc.TrangThai == Enums.TrangThaiDonThuocThanhToan.DaXuatThuoc ||
                     yc.TrangThai == Enums.TrangThaiDonThuocThanhToan.DaHuy))
                && o.DonVTYTThanhToans.All(yc =>
                    (yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan ||
                     yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.HuyThanhToan) &&
                    (yc.TrangThai == Enums.TrangThaiDonVTYTThanhToan.DaXuatVTYT ||
                     yc.TrangThai == Enums.TrangThaiDonVTYTThanhToan.DaHuy))).ToList();
            var yctnGoiMarketingCanHoanThanhs = _yeuCauTiepNhanRepository.Table.Where(o =>
                o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.DangKyGoiMarketing &&
                o.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien &&
                o.ThoiDiemTiepNhan.DayOfYear != DateTime.Now.DayOfYear
                && o.BenhNhan.YeuCauGoiDichVus.All(yc =>
                yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan ||
                yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.HuyThanhToan ||
                yc.TrangThai == Enums.EnumTrangThaiYeuCauGoiDichVu.DaHuy)).ToList();
            if (yctnCanHoanThanhs.Count > 0 || yctnGoiMarketingCanHoanThanhs.Count > 0)
            {
                //var maYeuCauTiepNhanNoiTrus = new List<string>();
                //if (yctnCanHoanThanhs.Count > 0)
                //{
                //    var maYeuCauTiepNhanNgoaiTrus = yctnCanHoanThanhs.Select(o => o.MaYeuCauTiepNhan).ToList();
                //    maYeuCauTiepNhanNoiTrus = _yeuCauTiepNhanRepository.TableNoTracking
                //        .Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru
                //                && o.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien
                //                && maYeuCauTiepNhanNgoaiTrus.Contains(o.MaYeuCauTiepNhan))
                //        .Select(o => o.MaYeuCauTiepNhan).ToList();
                //}

                foreach (var yctnCanHoanThanh in yctnCanHoanThanhs)
                {
                    //if (!maYeuCauTiepNhanNoiTrus.Contains(yctnCanHoanThanh.MaYeuCauTiepNhan))
                    //{
                    //    yctnCanHoanThanh.TrangThaiYeuCauTiepNhan = Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat;
                    //}
                    yctnCanHoanThanh.TrangThaiYeuCauTiepNhan = Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat;
                }
                foreach (var yctnGoiMarketingCanHoanThanh in yctnGoiMarketingCanHoanThanhs)
                {
                    yctnGoiMarketingCanHoanThanh.TrangThaiYeuCauTiepNhan = Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat;
                }
                _yeuCauTiepNhanRepository.Context.SaveChanges();
            }
        }
    }
}
