using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Services.ExportImport.Help;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public List<BaoCaoTongHopDoanhThuTheoKhoaPhongGridVo> GetDataForBaoCaoTongHopDoanhThuTheoKhoaPhong(
            DateTimeFilterVo dateTimeFilter)
        {
            var baoCaoTongHopDoanhThuTheoKhoaPhongGrid = new List<BaoCaoTongHopDoanhThuTheoKhoaPhongGridVo>();

            var baoCaoTongHopDoanhThuTheoKhoaPhong = new BaoCaoTongHopDoanhThuTheoKhoaPhongGridVo
            {
                Id = 1,
                KhoaPhong = "Khoa khám bệnh",
                DoanhThuTheoThang = 1790000,
                MienGiamTheoThang = 100000,
                ChiPhiKhacTheoThang = 247000,
                BhytTheoThang = 69000,
                DoanhThuTheoKySoSanh = 2790000,
                ChiPhiKhacTheoKySoSanh = 247000,
                BhytTheoKySoSanh = 69000,
                MienGiamTheoKySoSanh = 100000
            };

            baoCaoTongHopDoanhThuTheoKhoaPhongGrid.Add(baoCaoTongHopDoanhThuTheoKhoaPhong);

            var baoCaoTongHopDoanhThuTheoKhoaPhong1 = new BaoCaoTongHopDoanhThuTheoKhoaPhongGridVo
            {
                Id = 2,
                KhoaPhong = "Khoa sản",
                DoanhThuTheoThang = 1820000,
                MienGiamTheoThang = 120000,
                ChiPhiKhacTheoThang = 350000,
                BhytTheoThang = 70000,
                DoanhThuTheoKySoSanh = 3790000,
                ChiPhiKhacTheoKySoSanh = 237000,
                BhytTheoKySoSanh = 100000,
                MienGiamTheoKySoSanh = 130000
            };

            baoCaoTongHopDoanhThuTheoKhoaPhongGrid.Add(baoCaoTongHopDoanhThuTheoKhoaPhong1);

            return baoCaoTongHopDoanhThuTheoKhoaPhongGrid;
        }

        public async Task<GridDataSource> GetBaoCaoTongHopDoanhThuTheoKhoaPhongForGridAsync(BaoCaoTongHopDoanhThuTheoKhoaPhongQueryInfo queryInfo)
        {
            var dsPhong = await _phongBenhVienRepository.TableNoTracking.Include(o => o.KhoaPhong).ToListAsync();

            var khoaPhongData = dsPhong.GroupBy(o => o.KhoaPhong.Id, o => o).Select(g => new BaoCaoTongHopDoanhThuTheoKhoaPhongGridVo
            {
                Id = g.Key,
                KhoaPhong = g.First().KhoaPhong.Ten,
                DoanhThuTheoThang = 0,
                MienGiamTheoThang = 0,
                ChiPhiKhacTheoThang = 0,
                BhytTheoThang = 0,
                DoanhThuTheoKySoSanh = 0,
                ChiPhiKhacTheoKySoSanh = 0,
                BhytTheoKySoSanh = 0,
                MienGiamTheoKySoSanh = 0
            }).ToList();
            var quayThuocData = new BaoCaoTongHopDoanhThuTheoKhoaPhongGridVo
            {
                Id = 0,
                KhoaPhong = "Quầy thuốc",
                DoanhThuTheoThang = 0,
                MienGiamTheoThang = 0,
                ChiPhiKhacTheoThang = 0,
                BhytTheoThang = 0,
                DoanhThuTheoKySoSanh = 0,
                ChiPhiKhacTheoKySoSanh = 0,
                BhytTheoKySoSanh = 0,
                MienGiamTheoKySoSanh = 0
            };

            var tuNgay = queryInfo.TuNgay ?? DateTime.MinValue;
            var denNgay = queryInfo.DenNgay ?? DateTime.Now;

            var ycTiepNhanQuery = _yeuCauTiepNhanRepository.TableNoTracking.Where(o =>
                o.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                (queryInfo.TuNgay == null || tuNgay <= o.ThoiDiemTiepNhan) &&
                (queryInfo.DenNgay == null || o.ThoiDiemTiepNhan < denNgay));

            var ycTiepNhanData = await ycTiepNhanQuery
                .Include(o => o.YeuCauKhamBenhs).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.YeuCauDichVuGiuongBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.YeuCauDuocPhamBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.YeuCauVatTuBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                //TODO: need update goi dv
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.MienGiamChiPhis)
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauKhamBenhs).ThenInclude(yc => yc.MienGiamChiPhis)
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauDichVuKyThuats).ThenInclude(yc => yc.MienGiamChiPhis)
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauDichVuGiuongBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauDuocPhamBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauVatTuBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.DonThuocThanhToans).ThenInclude(yc => yc.DonThuocThanhToanChiTiets).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.DonVTYTThanhToans).ThenInclude(yc => yc.DonVTYTThanhToanChiTiets).ThenInclude(yc => yc.MienGiamChiPhis)
                .ToListAsync();

            foreach (var yeuCauTiepNhan in ycTiepNhanData)
            {
                foreach (var yeuCauKhamBenh in yeuCauTiepNhan.YeuCauKhamBenhs.Where(yc => yc.NoiThucHienId != null && yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                {
                    var khoaPhongItem = khoaPhongData.FirstOrDefault(o => dsPhong.First(p => p.Id == yeuCauKhamBenh.NoiThucHienId).KhoaPhongId == o.Id);
                    if (khoaPhongItem != null)
                    {
                        khoaPhongItem.DoanhThuTheoThang = khoaPhongItem.DoanhThuTheoThang.GetValueOrDefault() + yeuCauKhamBenh.Gia;
                        khoaPhongItem.MienGiamTheoThang = khoaPhongItem.MienGiamTheoThang.GetValueOrDefault() + yeuCauKhamBenh.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        khoaPhongItem.ChiPhiKhacTheoThang = khoaPhongItem.ChiPhiKhacTheoThang.GetValueOrDefault() + yeuCauKhamBenh.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        khoaPhongItem.BhytTheoThang = khoaPhongItem.BhytTheoThang.GetValueOrDefault() + yeuCauKhamBenh.DonGiaBaoHiem.GetValueOrDefault() * yeuCauKhamBenh.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauKhamBenh.MucHuongBaoHiem.GetValueOrDefault() / 100;
                    }
                }
                foreach (var yeuCauDichVuKyThuat in yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(yc => yc.NoiThucHienId != null && yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null))
                {
                    var khoaPhongItem = khoaPhongData.FirstOrDefault(o => dsPhong.First(p => p.Id == yeuCauDichVuKyThuat.NoiThucHienId).KhoaPhongId == o.Id);
                    if (khoaPhongItem != null)
                    {
                        khoaPhongItem.DoanhThuTheoThang = khoaPhongItem.DoanhThuTheoThang.GetValueOrDefault() + yeuCauDichVuKyThuat.Gia * yeuCauDichVuKyThuat.SoLan;
                        khoaPhongItem.MienGiamTheoThang = khoaPhongItem.MienGiamTheoThang.GetValueOrDefault() + yeuCauDichVuKyThuat.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        khoaPhongItem.ChiPhiKhacTheoThang = khoaPhongItem.ChiPhiKhacTheoThang.GetValueOrDefault() + yeuCauDichVuKyThuat.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        khoaPhongItem.BhytTheoThang = khoaPhongItem.BhytTheoThang.GetValueOrDefault() + yeuCauDichVuKyThuat.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDichVuKyThuat.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDichVuKyThuat.MucHuongBaoHiem.GetValueOrDefault() / 100 * yeuCauDichVuKyThuat.SoLan;
                    }
                }
                foreach (var yeuCauDichVuGiuong in yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(yc => yc.NoiThucHienId != null && yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                {
                    var khoaPhongItem = khoaPhongData.FirstOrDefault(o => dsPhong.First(p => p.Id == yeuCauDichVuGiuong.NoiThucHienId).KhoaPhongId == o.Id);
                    if (khoaPhongItem != null)
                    {
                        khoaPhongItem.DoanhThuTheoThang = khoaPhongItem.DoanhThuTheoThang.GetValueOrDefault() + yeuCauDichVuGiuong.Gia;
                        khoaPhongItem.MienGiamTheoThang = khoaPhongItem.MienGiamTheoThang.GetValueOrDefault() + yeuCauDichVuGiuong.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        khoaPhongItem.ChiPhiKhacTheoThang = khoaPhongItem.ChiPhiKhacTheoThang.GetValueOrDefault() + yeuCauDichVuGiuong.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        khoaPhongItem.BhytTheoThang = khoaPhongItem.BhytTheoThang.GetValueOrDefault() + yeuCauDichVuGiuong.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDichVuGiuong.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDichVuGiuong.MucHuongBaoHiem.GetValueOrDefault() / 100;
                    }
                }

                foreach (var yeuCauDuocPham in yeuCauTiepNhan.YeuCauDuocPhamBenhViens.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                {
                    quayThuocData.DoanhThuTheoThang = quayThuocData.DoanhThuTheoThang.GetValueOrDefault() + yeuCauDuocPham.DonGiaBan * (decimal)yeuCauDuocPham.SoLuong;
                    quayThuocData.MienGiamTheoThang = quayThuocData.MienGiamTheoThang.GetValueOrDefault() + yeuCauDuocPham.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    quayThuocData.ChiPhiKhacTheoThang = quayThuocData.ChiPhiKhacTheoThang.GetValueOrDefault() + yeuCauDuocPham.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    quayThuocData.BhytTheoThang = quayThuocData.BhytTheoThang.GetValueOrDefault() + yeuCauDuocPham.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDuocPham.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDuocPham.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)yeuCauDuocPham.SoLuong;
                }

                foreach (var yeuCauVatTu in yeuCauTiepNhan.YeuCauVatTuBenhViens.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                {
                    quayThuocData.DoanhThuTheoThang = quayThuocData.DoanhThuTheoThang.GetValueOrDefault() + yeuCauVatTu.DonGiaBan * (decimal)yeuCauVatTu.SoLuong;
                    quayThuocData.MienGiamTheoThang = quayThuocData.MienGiamTheoThang.GetValueOrDefault() + yeuCauVatTu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    quayThuocData.ChiPhiKhacTheoThang = quayThuocData.ChiPhiKhacTheoThang.GetValueOrDefault() + yeuCauVatTu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    quayThuocData.BhytTheoThang = quayThuocData.BhytTheoThang.GetValueOrDefault() + yeuCauVatTu.DonGiaBaoHiem.GetValueOrDefault() * yeuCauVatTu.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauVatTu.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)yeuCauVatTu.SoLuong;
                }

                foreach (var donThuocThanhToanChiTiet in yeuCauTiepNhan.DonThuocThanhToans.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan).SelectMany(o => o.DonThuocThanhToanChiTiets))
                {
                    quayThuocData.DoanhThuTheoThang = quayThuocData.DoanhThuTheoThang.GetValueOrDefault() + donThuocThanhToanChiTiet.DonGiaBan * (decimal)donThuocThanhToanChiTiet.SoLuong;
                    quayThuocData.MienGiamTheoThang = quayThuocData.MienGiamTheoThang.GetValueOrDefault() + donThuocThanhToanChiTiet.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    quayThuocData.ChiPhiKhacTheoThang = quayThuocData.ChiPhiKhacTheoThang.GetValueOrDefault() + donThuocThanhToanChiTiet.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    quayThuocData.BhytTheoThang = quayThuocData.BhytTheoThang.GetValueOrDefault() + donThuocThanhToanChiTiet.DonGiaBaoHiem.GetValueOrDefault() * donThuocThanhToanChiTiet.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * donThuocThanhToanChiTiet.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)donThuocThanhToanChiTiet.SoLuong;
                }
                foreach (var donVTYTThanhToanChiTiet in yeuCauTiepNhan.DonVTYTThanhToans.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan).SelectMany(o => o.DonVTYTThanhToanChiTiets))
                {
                    quayThuocData.DoanhThuTheoThang = quayThuocData.DoanhThuTheoThang.GetValueOrDefault() + donVTYTThanhToanChiTiet.DonGiaBan * (decimal)donVTYTThanhToanChiTiet.SoLuong;
                    quayThuocData.MienGiamTheoThang = quayThuocData.MienGiamTheoThang.GetValueOrDefault() + donVTYTThanhToanChiTiet.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    quayThuocData.ChiPhiKhacTheoThang = quayThuocData.ChiPhiKhacTheoThang.GetValueOrDefault() + donVTYTThanhToanChiTiet.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                }
                //TODO: need update goi dv
                //foreach (var yeuCauGoiDichVu in yeuCauTiepNhan.YeuCauGoiDichVus.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
                //{
                //    var sotienMienGiam = yeuCauGoiDichVu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                //    var sotienGiamTruKhac = yeuCauGoiDichVu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                //    var tiLeMienGiam = sotienMienGiam.AlmostEqual(0) ? 0 : (sotienMienGiam / yeuCauGoiDichVu.ChiPhiGoiDichVu);
                //    var tiLeGiamTruKhac = sotienGiamTruKhac.AlmostEqual(0) ? 0 : (sotienGiamTruKhac / yeuCauGoiDichVu.ChiPhiGoiDichVu);

                //    foreach (var yeuCauKhamBenh in yeuCauGoiDichVu.YeuCauKhamBenhs.Where(o=>o.NoiThucHienId!=null))
                //    {
                //        var khoaPhongItem = khoaPhongData.FirstOrDefault(o => dsPhong.First(p => p.Id == yeuCauKhamBenh.NoiThucHienId).KhoaPhongId == o.Id);
                //        if (khoaPhongItem != null)
                //        {
                //            var doanhThuDv = yeuCauKhamBenh.Gia - (yeuCauKhamBenh.Gia * (decimal)yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100);
                //            khoaPhongItem.DoanhThuTheoThang = khoaPhongItem.DoanhThuTheoThang.GetValueOrDefault() + doanhThuDv;
                //            khoaPhongItem.MienGiamTheoThang = khoaPhongItem.MienGiamTheoThang.GetValueOrDefault() + tiLeMienGiam * doanhThuDv;
                //            khoaPhongItem.ChiPhiKhacTheoThang = khoaPhongItem.ChiPhiKhacTheoThang.GetValueOrDefault() + tiLeGiamTruKhac * doanhThuDv;
                //        }
                //    }
                //    foreach (var yeuCauDichVuKyThuat in yeuCauGoiDichVu.YeuCauDichVuKyThuats.Where(o => o.NoiThucHienId != null))
                //    {
                //        var khoaPhongItem = khoaPhongData.FirstOrDefault(o => dsPhong.First(p => p.Id == yeuCauDichVuKyThuat.NoiThucHienId).KhoaPhongId == o.Id);
                //        if (khoaPhongItem != null)
                //        {
                //            var doanhThuDv = (yeuCauDichVuKyThuat.Gia - (yeuCauDichVuKyThuat.Gia * (decimal)yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100)) * yeuCauDichVuKyThuat.SoLan;
                //            khoaPhongItem.DoanhThuTheoThang = khoaPhongItem.DoanhThuTheoThang.GetValueOrDefault() + doanhThuDv;
                //            khoaPhongItem.MienGiamTheoThang = khoaPhongItem.MienGiamTheoThang.GetValueOrDefault() + tiLeMienGiam * doanhThuDv;
                //            khoaPhongItem.ChiPhiKhacTheoThang = khoaPhongItem.ChiPhiKhacTheoThang.GetValueOrDefault() + tiLeGiamTruKhac * doanhThuDv;
                //        }
                //    }
                //    foreach (var yeuCauDichVuGiuong in yeuCauGoiDichVu.YeuCauDichVuGiuongBenhViens.Where(o => o.NoiThucHienId != null))
                //    {
                //        var khoaPhongItem = khoaPhongData.FirstOrDefault(o => dsPhong.First(p => p.Id == yeuCauDichVuGiuong.NoiThucHienId).KhoaPhongId == o.Id);
                //        if (khoaPhongItem != null)
                //        {
                //            var doanhThuDv = yeuCauDichVuGiuong.Gia - (yeuCauDichVuGiuong.Gia * (decimal)yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100);
                //            khoaPhongItem.DoanhThuTheoThang = khoaPhongItem.DoanhThuTheoThang.GetValueOrDefault() + doanhThuDv;
                //            khoaPhongItem.MienGiamTheoThang = khoaPhongItem.MienGiamTheoThang.GetValueOrDefault() + tiLeMienGiam * doanhThuDv;
                //            khoaPhongItem.ChiPhiKhacTheoThang = khoaPhongItem.ChiPhiKhacTheoThang.GetValueOrDefault() + tiLeGiamTruKhac * doanhThuDv;
                //        }
                //    }
                //}
            }

            if (queryInfo.KySoSanhTuNgay != null && queryInfo.KySoSanhDenNgay != null)
            {
                var ycTiepNhanKySoSanhQuery = _yeuCauTiepNhanRepository.TableNoTracking.Where(o =>
                    o.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                    queryInfo.KySoSanhTuNgay.Value <= o.ThoiDiemTiepNhan && o.ThoiDiemTiepNhan < queryInfo.KySoSanhDenNgay.Value);

                var ycTiepNhanKySoSanhData = await ycTiepNhanKySoSanhQuery
                    .Include(o => o.YeuCauKhamBenhs).ThenInclude(yc => yc.MienGiamChiPhis)
                    .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(yc => yc.MienGiamChiPhis)
                    .Include(o => o.YeuCauDichVuGiuongBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                    .Include(o => o.YeuCauDuocPhamBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                    .Include(o => o.YeuCauVatTuBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                    //TODO: need update goi dv
                    //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.MienGiamChiPhis)
                    //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauKhamBenhs).ThenInclude(yc => yc.MienGiamChiPhis)
                    //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauDichVuKyThuats).ThenInclude(yc => yc.MienGiamChiPhis)
                    //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauDichVuGiuongBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                    //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauDuocPhamBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                    //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauVatTuBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                    .Include(o => o.DonThuocThanhToans).ThenInclude(yc => yc.DonThuocThanhToanChiTiets).ThenInclude(yc => yc.MienGiamChiPhis)
                    .Include(o => o.DonVTYTThanhToans).ThenInclude(yc => yc.DonVTYTThanhToanChiTiets).ThenInclude(yc => yc.MienGiamChiPhis)
                    .ToListAsync();

                foreach (var yeuCauTiepNhan in ycTiepNhanKySoSanhData)
                {
                    foreach (var yeuCauKhamBenh in yeuCauTiepNhan.YeuCauKhamBenhs.Where(yc => yc.NoiThucHienId != null && yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                    {
                        var khoaPhongItem = khoaPhongData.FirstOrDefault(o => dsPhong.First(p => p.Id == yeuCauKhamBenh.NoiThucHienId).KhoaPhongId == o.Id);
                        if (khoaPhongItem != null)
                        {
                            khoaPhongItem.DoanhThuTheoKySoSanh = khoaPhongItem.DoanhThuTheoKySoSanh.GetValueOrDefault() + yeuCauKhamBenh.Gia;
                            khoaPhongItem.MienGiamTheoKySoSanh = khoaPhongItem.MienGiamTheoKySoSanh.GetValueOrDefault() + yeuCauKhamBenh.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                            khoaPhongItem.ChiPhiKhacTheoKySoSanh = khoaPhongItem.ChiPhiKhacTheoKySoSanh.GetValueOrDefault() + yeuCauKhamBenh.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                            khoaPhongItem.BhytTheoKySoSanh = khoaPhongItem.BhytTheoKySoSanh.GetValueOrDefault() + yeuCauKhamBenh.DonGiaBaoHiem.GetValueOrDefault() * yeuCauKhamBenh.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauKhamBenh.MucHuongBaoHiem.GetValueOrDefault() / 100;
                        }
                    }
                    foreach (var yeuCauDichVuKyThuat in yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(yc => yc.NoiThucHienId != null && yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null))
                    {
                        var khoaPhongItem = khoaPhongData.FirstOrDefault(o => dsPhong.First(p => p.Id == yeuCauDichVuKyThuat.NoiThucHienId).KhoaPhongId == o.Id);
                        if (khoaPhongItem != null)
                        {
                            khoaPhongItem.DoanhThuTheoKySoSanh = khoaPhongItem.DoanhThuTheoKySoSanh.GetValueOrDefault() + yeuCauDichVuKyThuat.Gia * yeuCauDichVuKyThuat.SoLan;
                            khoaPhongItem.MienGiamTheoKySoSanh = khoaPhongItem.MienGiamTheoKySoSanh.GetValueOrDefault() + yeuCauDichVuKyThuat.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                            khoaPhongItem.ChiPhiKhacTheoKySoSanh = khoaPhongItem.ChiPhiKhacTheoKySoSanh.GetValueOrDefault() + yeuCauDichVuKyThuat.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                            khoaPhongItem.BhytTheoKySoSanh = khoaPhongItem.BhytTheoKySoSanh.GetValueOrDefault() + yeuCauDichVuKyThuat.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDichVuKyThuat.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDichVuKyThuat.MucHuongBaoHiem.GetValueOrDefault() / 100 * yeuCauDichVuKyThuat.SoLan;
                        }
                    }
                    foreach (var yeuCauDichVuGiuong in yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(yc => yc.NoiThucHienId != null && yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                    {
                        var khoaPhongItem = khoaPhongData.FirstOrDefault(o => dsPhong.First(p => p.Id == yeuCauDichVuGiuong.NoiThucHienId).KhoaPhongId == o.Id);
                        if (khoaPhongItem != null)
                        {
                            khoaPhongItem.DoanhThuTheoKySoSanh = khoaPhongItem.DoanhThuTheoKySoSanh.GetValueOrDefault() + yeuCauDichVuGiuong.Gia;
                            khoaPhongItem.MienGiamTheoKySoSanh = khoaPhongItem.MienGiamTheoKySoSanh.GetValueOrDefault() + yeuCauDichVuGiuong.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                            khoaPhongItem.ChiPhiKhacTheoKySoSanh = khoaPhongItem.ChiPhiKhacTheoKySoSanh.GetValueOrDefault() + yeuCauDichVuGiuong.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                            khoaPhongItem.BhytTheoKySoSanh = khoaPhongItem.BhytTheoKySoSanh.GetValueOrDefault() + yeuCauDichVuGiuong.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDichVuGiuong.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDichVuGiuong.MucHuongBaoHiem.GetValueOrDefault() / 100;
                        }
                    }

                    foreach (var yeuCauDuocPham in yeuCauTiepNhan.YeuCauDuocPhamBenhViens.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                    {
                        quayThuocData.DoanhThuTheoKySoSanh = quayThuocData.DoanhThuTheoKySoSanh.GetValueOrDefault() + yeuCauDuocPham.DonGiaBan * (decimal)yeuCauDuocPham.SoLuong;
                        quayThuocData.MienGiamTheoKySoSanh = quayThuocData.MienGiamTheoKySoSanh.GetValueOrDefault() + yeuCauDuocPham.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        quayThuocData.ChiPhiKhacTheoKySoSanh = quayThuocData.ChiPhiKhacTheoKySoSanh.GetValueOrDefault() + yeuCauDuocPham.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        quayThuocData.BhytTheoKySoSanh = quayThuocData.BhytTheoKySoSanh.GetValueOrDefault() + yeuCauDuocPham.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDuocPham.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDuocPham.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)yeuCauDuocPham.SoLuong;
                    }

                    foreach (var yeuCauVatTu in yeuCauTiepNhan.YeuCauVatTuBenhViens.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                    {
                        quayThuocData.DoanhThuTheoKySoSanh = quayThuocData.DoanhThuTheoKySoSanh.GetValueOrDefault() + yeuCauVatTu.DonGiaBan * (decimal)yeuCauVatTu.SoLuong;
                        quayThuocData.MienGiamTheoKySoSanh = quayThuocData.MienGiamTheoKySoSanh.GetValueOrDefault() + yeuCauVatTu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        quayThuocData.ChiPhiKhacTheoKySoSanh = quayThuocData.ChiPhiKhacTheoKySoSanh.GetValueOrDefault() + yeuCauVatTu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        quayThuocData.BhytTheoKySoSanh = quayThuocData.BhytTheoKySoSanh.GetValueOrDefault() + yeuCauVatTu.DonGiaBaoHiem.GetValueOrDefault() * yeuCauVatTu.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauVatTu.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)yeuCauVatTu.SoLuong;
                    }

                    foreach (var donThuocThanhToanChiTiet in yeuCauTiepNhan.DonThuocThanhToans.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan).SelectMany(o => o.DonThuocThanhToanChiTiets))
                    {
                        quayThuocData.DoanhThuTheoKySoSanh = quayThuocData.DoanhThuTheoKySoSanh.GetValueOrDefault() + donThuocThanhToanChiTiet.DonGiaBan * (decimal)donThuocThanhToanChiTiet.SoLuong;
                        quayThuocData.MienGiamTheoKySoSanh = quayThuocData.MienGiamTheoKySoSanh.GetValueOrDefault() + donThuocThanhToanChiTiet.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        quayThuocData.ChiPhiKhacTheoKySoSanh = quayThuocData.ChiPhiKhacTheoKySoSanh.GetValueOrDefault() + donThuocThanhToanChiTiet.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        quayThuocData.BhytTheoKySoSanh = quayThuocData.BhytTheoKySoSanh.GetValueOrDefault() + donThuocThanhToanChiTiet.DonGiaBaoHiem.GetValueOrDefault() * donThuocThanhToanChiTiet.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * donThuocThanhToanChiTiet.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)donThuocThanhToanChiTiet.SoLuong;
                    }

                    foreach (var donVTYTThanhToanChiTiet in yeuCauTiepNhan.DonVTYTThanhToans.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan).SelectMany(o => o.DonVTYTThanhToanChiTiets))
                    {
                        quayThuocData.DoanhThuTheoKySoSanh = quayThuocData.DoanhThuTheoKySoSanh.GetValueOrDefault() + donVTYTThanhToanChiTiet.DonGiaBan * (decimal)donVTYTThanhToanChiTiet.SoLuong;
                        quayThuocData.MienGiamTheoKySoSanh = quayThuocData.MienGiamTheoKySoSanh.GetValueOrDefault() + donVTYTThanhToanChiTiet.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        quayThuocData.ChiPhiKhacTheoKySoSanh = quayThuocData.ChiPhiKhacTheoKySoSanh.GetValueOrDefault() + donVTYTThanhToanChiTiet.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    }
                    //TODO: need update goi dv
                    //foreach (var yeuCauGoiDichVu in yeuCauTiepNhan.YeuCauGoiDichVus.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
                    //{
                    //    var sotienMienGiam = yeuCauGoiDichVu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    //    var sotienGiamTruKhac = yeuCauGoiDichVu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    //    var tiLeMienGiam = sotienMienGiam.AlmostEqual(0) ? 0 : (sotienMienGiam / yeuCauGoiDichVu.ChiPhiGoiDichVu);
                    //    var tiLeGiamTruKhac = sotienGiamTruKhac.AlmostEqual(0) ? 0 : (sotienGiamTruKhac / yeuCauGoiDichVu.ChiPhiGoiDichVu);

                    //    foreach (var yeuCauKhamBenh in yeuCauGoiDichVu.YeuCauKhamBenhs.Where(o=>o.NoiThucHienId!=null))
                    //    {
                    //        var khoaPhongItem = khoaPhongData.FirstOrDefault(o => dsPhong.First(p => p.Id == yeuCauKhamBenh.NoiThucHienId).KhoaPhongId == o.Id);
                    //        if (khoaPhongItem != null)
                    //        {
                    //            var doanhThuDv = yeuCauKhamBenh.Gia - (yeuCauKhamBenh.Gia * (decimal)yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100);
                    //            khoaPhongItem.DoanhThuTheoKySoSanh = khoaPhongItem.DoanhThuTheoKySoSanh.GetValueOrDefault() + doanhThuDv;
                    //            khoaPhongItem.MienGiamTheoKySoSanh = khoaPhongItem.MienGiamTheoKySoSanh.GetValueOrDefault() + tiLeMienGiam * doanhThuDv;
                    //            khoaPhongItem.ChiPhiKhacTheoKySoSanh = khoaPhongItem.ChiPhiKhacTheoKySoSanh.GetValueOrDefault() + tiLeGiamTruKhac * doanhThuDv;
                    //        }
                    //    }
                    //    foreach (var yeuCauDichVuKyThuat in yeuCauGoiDichVu.YeuCauDichVuKyThuats.Where(o => o.NoiThucHienId != null))
                    //    {
                    //        var khoaPhongItem = khoaPhongData.FirstOrDefault(o => dsPhong.First(p => p.Id == yeuCauDichVuKyThuat.NoiThucHienId).KhoaPhongId == o.Id);
                    //        if (khoaPhongItem != null)
                    //        {
                    //            var doanhThuDv = (yeuCauDichVuKyThuat.Gia - (yeuCauDichVuKyThuat.Gia * (decimal)yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100)) * yeuCauDichVuKyThuat.SoLan;
                    //            khoaPhongItem.DoanhThuTheoKySoSanh = khoaPhongItem.DoanhThuTheoKySoSanh.GetValueOrDefault() + doanhThuDv;
                    //            khoaPhongItem.MienGiamTheoKySoSanh = khoaPhongItem.MienGiamTheoKySoSanh.GetValueOrDefault() + tiLeMienGiam * doanhThuDv;
                    //            khoaPhongItem.ChiPhiKhacTheoKySoSanh = khoaPhongItem.ChiPhiKhacTheoKySoSanh.GetValueOrDefault() + tiLeGiamTruKhac * doanhThuDv;
                    //        }
                    //    }
                    //    foreach (var yeuCauDichVuGiuong in yeuCauGoiDichVu.YeuCauDichVuGiuongBenhViens.Where(o => o.NoiThucHienId != null))
                    //    {
                    //        var khoaPhongItem = khoaPhongData.FirstOrDefault(o => dsPhong.First(p => p.Id == yeuCauDichVuGiuong.NoiThucHienId).KhoaPhongId == o.Id);
                    //        if (khoaPhongItem != null)
                    //        {
                    //            var doanhThuDv = yeuCauDichVuGiuong.Gia - (yeuCauDichVuGiuong.Gia * (decimal)yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100);
                    //            khoaPhongItem.DoanhThuTheoKySoSanh = khoaPhongItem.DoanhThuTheoKySoSanh.GetValueOrDefault() + doanhThuDv;
                    //            khoaPhongItem.MienGiamTheoKySoSanh = khoaPhongItem.MienGiamTheoKySoSanh.GetValueOrDefault() + tiLeMienGiam * doanhThuDv;
                    //            khoaPhongItem.ChiPhiKhacTheoKySoSanh = khoaPhongItem.ChiPhiKhacTheoKySoSanh.GetValueOrDefault() + tiLeGiamTruKhac * doanhThuDv;
                    //        }
                    //    }
                    //}
                }
            }

            khoaPhongData.Add(quayThuocData);
            var gridData = khoaPhongData
                .Where(o => !o.DoanhThuTheoThang.GetValueOrDefault().AlmostEqual(0) ||
                            !o.DoanhThuTheoKySoSanh.GetValueOrDefault().AlmostEqual(0))
                .Select((o, i) =>
                {
                    o.Stt = i + 1;
                    return o;
                }).ToList();
            return new GridDataSource { Data = gridData.Skip(queryInfo.LayTatCa ? 0 : queryInfo.Skip).Take(queryInfo.LayTatCa ? int.MaxValue : queryInfo.Take).ToArray(), TotalRowCount = gridData.Count };
        }

        public async Task<GridItem> GetTotalBaoCaoTongHopDoanhThuTheoKhoaPhongForGridAsync(BaoCaoTongHopDoanhThuTheoKhoaPhongQueryInfo queryInfo)
        {
            var itemData = new BaoCaoTongHopDoanhThuTheoKhoaPhongGridVo
            {
                Id = 0,
                DoanhThuTheoThang = 0,
                MienGiamTheoThang = 0,
                ChiPhiKhacTheoThang = 0,
                BhytTheoThang = 0,
                DoanhThuTheoKySoSanh = 0,
                ChiPhiKhacTheoKySoSanh = 0,
                BhytTheoKySoSanh = 0,
                MienGiamTheoKySoSanh = 0
            };

            var tuNgay = queryInfo.TuNgay ?? DateTime.MinValue;
            var denNgay = queryInfo.DenNgay ?? DateTime.Now;

            var ycTiepNhanQuery = _yeuCauTiepNhanRepository.TableNoTracking.Where(o =>
                o.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                (queryInfo.TuNgay == null || tuNgay <= o.ThoiDiemTiepNhan) &&
                (queryInfo.DenNgay == null || o.ThoiDiemTiepNhan < denNgay));

            var ycTiepNhanData = await ycTiepNhanQuery
                .Include(o => o.YeuCauKhamBenhs).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.YeuCauDichVuGiuongBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.YeuCauDuocPhamBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.YeuCauVatTuBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                //TODO: need update goi dv
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.MienGiamChiPhis)
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauKhamBenhs).ThenInclude(yc => yc.MienGiamChiPhis)
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauDichVuKyThuats).ThenInclude(yc => yc.MienGiamChiPhis)
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauDichVuGiuongBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauDuocPhamBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauVatTuBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.DonThuocThanhToans).ThenInclude(yc => yc.DonThuocThanhToanChiTiets).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.DonVTYTThanhToans).ThenInclude(yc => yc.DonVTYTThanhToanChiTiets).ThenInclude(yc => yc.MienGiamChiPhis)
                .ToListAsync();

            foreach (var yeuCauTiepNhan in ycTiepNhanData)
            {
                foreach (var yeuCauKhamBenh in yeuCauTiepNhan.YeuCauKhamBenhs.Where(yc => yc.NoiThucHienId != null && yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                {
                    itemData.DoanhThuTheoThang = itemData.DoanhThuTheoThang.GetValueOrDefault() + yeuCauKhamBenh.Gia;
                    itemData.MienGiamTheoThang = itemData.MienGiamTheoThang.GetValueOrDefault() + yeuCauKhamBenh.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    itemData.ChiPhiKhacTheoThang = itemData.ChiPhiKhacTheoThang.GetValueOrDefault() + yeuCauKhamBenh.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    itemData.BhytTheoThang = itemData.BhytTheoThang.GetValueOrDefault() + yeuCauKhamBenh.DonGiaBaoHiem.GetValueOrDefault() * yeuCauKhamBenh.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauKhamBenh.MucHuongBaoHiem.GetValueOrDefault() / 100;
                }
                foreach (var yeuCauDichVuKyThuat in yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(yc => yc.NoiThucHienId != null && yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null))
                {
                    itemData.DoanhThuTheoThang = itemData.DoanhThuTheoThang.GetValueOrDefault() + yeuCauDichVuKyThuat.Gia * yeuCauDichVuKyThuat.SoLan;
                    itemData.MienGiamTheoThang = itemData.MienGiamTheoThang.GetValueOrDefault() + yeuCauDichVuKyThuat.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    itemData.ChiPhiKhacTheoThang = itemData.ChiPhiKhacTheoThang.GetValueOrDefault() + yeuCauDichVuKyThuat.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    itemData.BhytTheoThang = itemData.BhytTheoThang.GetValueOrDefault() + yeuCauDichVuKyThuat.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDichVuKyThuat.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDichVuKyThuat.MucHuongBaoHiem.GetValueOrDefault() / 100 * yeuCauDichVuKyThuat.SoLan;
                }
                foreach (var yeuCauDichVuGiuong in yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(yc => yc.NoiThucHienId != null && yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                {
                    itemData.DoanhThuTheoThang = itemData.DoanhThuTheoThang.GetValueOrDefault() + yeuCauDichVuGiuong.Gia;
                    itemData.MienGiamTheoThang = itemData.MienGiamTheoThang.GetValueOrDefault() + yeuCauDichVuGiuong.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    itemData.ChiPhiKhacTheoThang = itemData.ChiPhiKhacTheoThang.GetValueOrDefault() + yeuCauDichVuGiuong.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    itemData.BhytTheoThang = itemData.BhytTheoThang.GetValueOrDefault() + yeuCauDichVuGiuong.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDichVuGiuong.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDichVuGiuong.MucHuongBaoHiem.GetValueOrDefault() / 100;
                }

                foreach (var yeuCauDuocPham in yeuCauTiepNhan.YeuCauDuocPhamBenhViens.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                {
                    itemData.DoanhThuTheoThang = itemData.DoanhThuTheoThang.GetValueOrDefault() + yeuCauDuocPham.DonGiaBan * (decimal)yeuCauDuocPham.SoLuong;
                    itemData.MienGiamTheoThang = itemData.MienGiamTheoThang.GetValueOrDefault() + yeuCauDuocPham.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    itemData.ChiPhiKhacTheoThang = itemData.ChiPhiKhacTheoThang.GetValueOrDefault() + yeuCauDuocPham.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    itemData.BhytTheoThang = itemData.BhytTheoThang.GetValueOrDefault() + yeuCauDuocPham.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDuocPham.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDuocPham.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)yeuCauDuocPham.SoLuong;
                }

                foreach (var yeuCauVatTu in yeuCauTiepNhan.YeuCauVatTuBenhViens.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                {
                    itemData.DoanhThuTheoThang = itemData.DoanhThuTheoThang.GetValueOrDefault() + yeuCauVatTu.DonGiaBan * (decimal)yeuCauVatTu.SoLuong;
                    itemData.MienGiamTheoThang = itemData.MienGiamTheoThang.GetValueOrDefault() + yeuCauVatTu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    itemData.ChiPhiKhacTheoThang = itemData.ChiPhiKhacTheoThang.GetValueOrDefault() + yeuCauVatTu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    itemData.BhytTheoThang = itemData.BhytTheoThang.GetValueOrDefault() + yeuCauVatTu.DonGiaBaoHiem.GetValueOrDefault() * yeuCauVatTu.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauVatTu.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)yeuCauVatTu.SoLuong;
                }

                foreach (var donThuocThanhToanChiTiet in yeuCauTiepNhan.DonThuocThanhToans.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan).SelectMany(o => o.DonThuocThanhToanChiTiets))
                {
                    itemData.DoanhThuTheoThang = itemData.DoanhThuTheoThang.GetValueOrDefault() + donThuocThanhToanChiTiet.DonGiaBan * (decimal)donThuocThanhToanChiTiet.SoLuong;
                    itemData.MienGiamTheoThang = itemData.MienGiamTheoThang.GetValueOrDefault() + donThuocThanhToanChiTiet.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    itemData.ChiPhiKhacTheoThang = itemData.ChiPhiKhacTheoThang.GetValueOrDefault() + donThuocThanhToanChiTiet.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    itemData.BhytTheoThang = itemData.BhytTheoThang.GetValueOrDefault() + donThuocThanhToanChiTiet.DonGiaBaoHiem.GetValueOrDefault() * donThuocThanhToanChiTiet.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * donThuocThanhToanChiTiet.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)donThuocThanhToanChiTiet.SoLuong;
                }

                foreach (var donVTYTThanhToanChiTiet in yeuCauTiepNhan.DonVTYTThanhToans.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan).SelectMany(o => o.DonVTYTThanhToanChiTiets))
                {
                    itemData.DoanhThuTheoThang = itemData.DoanhThuTheoThang.GetValueOrDefault() + donVTYTThanhToanChiTiet.DonGiaBan * (decimal)donVTYTThanhToanChiTiet.SoLuong;
                    itemData.MienGiamTheoThang = itemData.MienGiamTheoThang.GetValueOrDefault() + donVTYTThanhToanChiTiet.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    itemData.ChiPhiKhacTheoThang = itemData.ChiPhiKhacTheoThang.GetValueOrDefault() + donVTYTThanhToanChiTiet.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                }
                //TODO: need update goi dv
                //foreach (var yeuCauGoiDichVu in yeuCauTiepNhan.YeuCauGoiDichVus.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
                //{
                //    var sotienMienGiam = yeuCauGoiDichVu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                //    var sotienGiamTruKhac = yeuCauGoiDichVu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                //    var tiLeMienGiam = sotienMienGiam.AlmostEqual(0) ? 0 : (sotienMienGiam / yeuCauGoiDichVu.ChiPhiGoiDichVu);
                //    var tiLeGiamTruKhac = sotienGiamTruKhac.AlmostEqual(0) ? 0 : (sotienGiamTruKhac / yeuCauGoiDichVu.ChiPhiGoiDichVu);

                //    foreach (var yeuCauKhamBenh in yeuCauGoiDichVu.YeuCauKhamBenhs.Where(x=>x.NoiThucHienId != null))
                //    {
                //        var doanhThuDv = yeuCauKhamBenh.Gia - (yeuCauKhamBenh.Gia * (decimal)yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100);
                //        itemData.DoanhThuTheoThang = itemData.DoanhThuTheoThang.GetValueOrDefault() + doanhThuDv;
                //        itemData.MienGiamTheoThang = itemData.MienGiamTheoThang.GetValueOrDefault() + tiLeMienGiam * doanhThuDv;
                //        itemData.ChiPhiKhacTheoThang = itemData.ChiPhiKhacTheoThang.GetValueOrDefault() + tiLeGiamTruKhac * doanhThuDv;
                //    }
                //    foreach (var yeuCauDichVuKyThuat in yeuCauGoiDichVu.YeuCauDichVuKyThuats.Where(x => x.NoiThucHienId != null))
                //    {
                //        var doanhThuDv = (yeuCauDichVuKyThuat.Gia - (yeuCauDichVuKyThuat.Gia * (decimal)yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100)) * yeuCauDichVuKyThuat.SoLan;
                //        itemData.DoanhThuTheoThang = itemData.DoanhThuTheoThang.GetValueOrDefault() + doanhThuDv;
                //        itemData.MienGiamTheoThang = itemData.MienGiamTheoThang.GetValueOrDefault() + tiLeMienGiam * doanhThuDv;
                //        itemData.ChiPhiKhacTheoThang = itemData.ChiPhiKhacTheoThang.GetValueOrDefault() + tiLeGiamTruKhac * doanhThuDv;
                //    }
                //    foreach (var yeuCauDichVuGiuong in yeuCauGoiDichVu.YeuCauDichVuGiuongBenhViens.Where(x => x.NoiThucHienId != null))
                //    {
                //        var doanhThuDv = yeuCauDichVuGiuong.Gia - (yeuCauDichVuGiuong.Gia * (decimal)yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100);
                //        itemData.DoanhThuTheoThang = itemData.DoanhThuTheoThang.GetValueOrDefault() + doanhThuDv;
                //        itemData.MienGiamTheoThang = itemData.MienGiamTheoThang.GetValueOrDefault() + tiLeMienGiam * doanhThuDv;
                //        itemData.ChiPhiKhacTheoThang = itemData.ChiPhiKhacTheoThang.GetValueOrDefault() + tiLeGiamTruKhac * doanhThuDv;
                //    }
                //}
            }

            if (queryInfo.KySoSanhTuNgay != null && queryInfo.KySoSanhDenNgay != null)
            {
                var ycTiepNhanKySoSanhQuery = _yeuCauTiepNhanRepository.TableNoTracking.Where(o =>
                    o.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                    queryInfo.KySoSanhTuNgay.Value <= o.ThoiDiemTiepNhan && o.ThoiDiemTiepNhan < queryInfo.KySoSanhDenNgay.Value);

                var ycTiepNhanKySoSanhData = await ycTiepNhanKySoSanhQuery
                    .Include(o => o.YeuCauKhamBenhs).ThenInclude(yc => yc.MienGiamChiPhis)
                    .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(yc => yc.MienGiamChiPhis)
                    .Include(o => o.YeuCauDichVuGiuongBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                    .Include(o => o.YeuCauDuocPhamBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                    .Include(o => o.YeuCauVatTuBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                    //TODO: need update goi dv
                    //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.MienGiamChiPhis)
                    //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauKhamBenhs).ThenInclude(yc => yc.MienGiamChiPhis)
                    //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauDichVuKyThuats).ThenInclude(yc => yc.MienGiamChiPhis)
                    //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauDichVuGiuongBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                    //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauDuocPhamBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                    //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauVatTuBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                    .Include(o => o.DonThuocThanhToans).ThenInclude(yc => yc.DonThuocThanhToanChiTiets).ThenInclude(yc => yc.MienGiamChiPhis)
                    .Include(o => o.DonVTYTThanhToans).ThenInclude(yc => yc.DonVTYTThanhToanChiTiets).ThenInclude(yc => yc.MienGiamChiPhis)
                    .ToListAsync();

                foreach (var yeuCauTiepNhan in ycTiepNhanKySoSanhData)
                {
                    foreach (var yeuCauKhamBenh in yeuCauTiepNhan.YeuCauKhamBenhs.Where(yc => yc.NoiThucHienId != null && yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                    {
                        itemData.DoanhThuTheoKySoSanh = itemData.DoanhThuTheoKySoSanh.GetValueOrDefault() + yeuCauKhamBenh.Gia;
                        itemData.MienGiamTheoKySoSanh = itemData.MienGiamTheoKySoSanh.GetValueOrDefault() + yeuCauKhamBenh.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        itemData.ChiPhiKhacTheoKySoSanh = itemData.ChiPhiKhacTheoKySoSanh.GetValueOrDefault() + yeuCauKhamBenh.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        itemData.BhytTheoKySoSanh = itemData.BhytTheoKySoSanh.GetValueOrDefault() + yeuCauKhamBenh.DonGiaBaoHiem.GetValueOrDefault() * yeuCauKhamBenh.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauKhamBenh.MucHuongBaoHiem.GetValueOrDefault() / 100;
                    }
                    foreach (var yeuCauDichVuKyThuat in yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(yc => yc.NoiThucHienId != null && yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null))
                    {
                        itemData.DoanhThuTheoKySoSanh = itemData.DoanhThuTheoKySoSanh.GetValueOrDefault() + yeuCauDichVuKyThuat.Gia * yeuCauDichVuKyThuat.SoLan;
                        itemData.MienGiamTheoKySoSanh = itemData.MienGiamTheoKySoSanh.GetValueOrDefault() + yeuCauDichVuKyThuat.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        itemData.ChiPhiKhacTheoKySoSanh = itemData.ChiPhiKhacTheoKySoSanh.GetValueOrDefault() + yeuCauDichVuKyThuat.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        itemData.BhytTheoKySoSanh = itemData.BhytTheoKySoSanh.GetValueOrDefault() + yeuCauDichVuKyThuat.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDichVuKyThuat.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDichVuKyThuat.MucHuongBaoHiem.GetValueOrDefault() / 100 * yeuCauDichVuKyThuat.SoLan;
                    }
                    foreach (var yeuCauDichVuGiuong in yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(yc => yc.NoiThucHienId != null && yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                    {
                        itemData.DoanhThuTheoKySoSanh = itemData.DoanhThuTheoKySoSanh.GetValueOrDefault() + yeuCauDichVuGiuong.Gia;
                        itemData.MienGiamTheoKySoSanh = itemData.MienGiamTheoKySoSanh.GetValueOrDefault() + yeuCauDichVuGiuong.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        itemData.ChiPhiKhacTheoKySoSanh = itemData.ChiPhiKhacTheoKySoSanh.GetValueOrDefault() + yeuCauDichVuGiuong.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        itemData.BhytTheoKySoSanh = itemData.BhytTheoKySoSanh.GetValueOrDefault() + yeuCauDichVuGiuong.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDichVuGiuong.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDichVuGiuong.MucHuongBaoHiem.GetValueOrDefault() / 100;
                    }

                    foreach (var yeuCauDuocPham in yeuCauTiepNhan.YeuCauDuocPhamBenhViens.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                    {
                        itemData.DoanhThuTheoKySoSanh = itemData.DoanhThuTheoKySoSanh.GetValueOrDefault() + yeuCauDuocPham.DonGiaBan * (decimal)yeuCauDuocPham.SoLuong;
                        itemData.MienGiamTheoKySoSanh = itemData.MienGiamTheoKySoSanh.GetValueOrDefault() + yeuCauDuocPham.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        itemData.ChiPhiKhacTheoKySoSanh = itemData.ChiPhiKhacTheoKySoSanh.GetValueOrDefault() + yeuCauDuocPham.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        itemData.BhytTheoKySoSanh = itemData.BhytTheoKySoSanh.GetValueOrDefault() + yeuCauDuocPham.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDuocPham.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDuocPham.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)yeuCauDuocPham.SoLuong;
                    }
                    foreach (var yeuCauVatTu in yeuCauTiepNhan.YeuCauVatTuBenhViens.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                    {
                        itemData.DoanhThuTheoKySoSanh = itemData.DoanhThuTheoKySoSanh.GetValueOrDefault() + yeuCauVatTu.DonGiaBan * (decimal)yeuCauVatTu.SoLuong;
                        itemData.MienGiamTheoKySoSanh = itemData.MienGiamTheoKySoSanh.GetValueOrDefault() + yeuCauVatTu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        itemData.ChiPhiKhacTheoKySoSanh = itemData.ChiPhiKhacTheoKySoSanh.GetValueOrDefault() + yeuCauVatTu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        itemData.BhytTheoKySoSanh = itemData.BhytTheoKySoSanh.GetValueOrDefault() + yeuCauVatTu.DonGiaBaoHiem.GetValueOrDefault() * yeuCauVatTu.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauVatTu.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)yeuCauVatTu.SoLuong;
                    }

                    foreach (var donThuocThanhToanChiTiet in yeuCauTiepNhan.DonThuocThanhToans.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan).SelectMany(o => o.DonThuocThanhToanChiTiets))
                    {
                        itemData.DoanhThuTheoKySoSanh = itemData.DoanhThuTheoKySoSanh.GetValueOrDefault() + donThuocThanhToanChiTiet.DonGiaBan * (decimal)donThuocThanhToanChiTiet.SoLuong;
                        itemData.MienGiamTheoKySoSanh = itemData.MienGiamTheoKySoSanh.GetValueOrDefault() + donThuocThanhToanChiTiet.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        itemData.ChiPhiKhacTheoKySoSanh = itemData.ChiPhiKhacTheoKySoSanh.GetValueOrDefault() + donThuocThanhToanChiTiet.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        itemData.BhytTheoKySoSanh = itemData.BhytTheoKySoSanh.GetValueOrDefault() + donThuocThanhToanChiTiet.DonGiaBaoHiem.GetValueOrDefault() * donThuocThanhToanChiTiet.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * donThuocThanhToanChiTiet.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)donThuocThanhToanChiTiet.SoLuong;
                    }

                    foreach (var donVTYTThanhToanChiTiet in yeuCauTiepNhan.DonVTYTThanhToans.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan).SelectMany(o => o.DonVTYTThanhToanChiTiets))
                    {
                        itemData.DoanhThuTheoKySoSanh = itemData.DoanhThuTheoKySoSanh.GetValueOrDefault() + donVTYTThanhToanChiTiet.DonGiaBan * (decimal)donVTYTThanhToanChiTiet.SoLuong;
                        itemData.MienGiamTheoKySoSanh = itemData.MienGiamTheoKySoSanh.GetValueOrDefault() + donVTYTThanhToanChiTiet.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        itemData.ChiPhiKhacTheoKySoSanh = itemData.ChiPhiKhacTheoKySoSanh.GetValueOrDefault() + donVTYTThanhToanChiTiet.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    }
                    //TODO: need update goi dv
                    //foreach (var yeuCauGoiDichVu in yeuCauTiepNhan.YeuCauGoiDichVus.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
                    //{
                    //    var sotienMienGiam = yeuCauGoiDichVu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    //    var sotienGiamTruKhac = yeuCauGoiDichVu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    //    var tiLeMienGiam = sotienMienGiam.AlmostEqual(0) ? 0 : (sotienMienGiam / yeuCauGoiDichVu.ChiPhiGoiDichVu);
                    //    var tiLeGiamTruKhac = sotienGiamTruKhac.AlmostEqual(0) ? 0 : (sotienGiamTruKhac / yeuCauGoiDichVu.ChiPhiGoiDichVu);

                    //    foreach (var yeuCauKhamBenh in yeuCauGoiDichVu.YeuCauKhamBenhs.Where(x=>x.NoiThucHienId != null))
                    //    {
                    //        var doanhThuDv = yeuCauKhamBenh.Gia - (yeuCauKhamBenh.Gia * (decimal)yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100);
                    //        itemData.DoanhThuTheoKySoSanh = itemData.DoanhThuTheoKySoSanh.GetValueOrDefault() + doanhThuDv;
                    //        itemData.MienGiamTheoKySoSanh = itemData.MienGiamTheoKySoSanh.GetValueOrDefault() + tiLeMienGiam * doanhThuDv;
                    //        itemData.ChiPhiKhacTheoKySoSanh = itemData.ChiPhiKhacTheoKySoSanh.GetValueOrDefault() + tiLeGiamTruKhac * doanhThuDv;
                    //    }
                    //    foreach (var yeuCauDichVuKyThuat in yeuCauGoiDichVu.YeuCauDichVuKyThuats.Where(x => x.NoiThucHienId != null))
                    //    {
                    //        var doanhThuDv = (yeuCauDichVuKyThuat.Gia - (yeuCauDichVuKyThuat.Gia * (decimal)yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100)) * yeuCauDichVuKyThuat.SoLan;
                    //        itemData.DoanhThuTheoKySoSanh = itemData.DoanhThuTheoKySoSanh.GetValueOrDefault() + doanhThuDv;
                    //        itemData.MienGiamTheoKySoSanh = itemData.MienGiamTheoKySoSanh.GetValueOrDefault() + tiLeMienGiam * doanhThuDv;
                    //        itemData.ChiPhiKhacTheoKySoSanh = itemData.ChiPhiKhacTheoKySoSanh.GetValueOrDefault() + tiLeGiamTruKhac * doanhThuDv;
                    //    }
                    //    foreach (var yeuCauDichVuGiuong in yeuCauGoiDichVu.YeuCauDichVuGiuongBenhViens.Where(x => x.NoiThucHienId != null))
                    //    {
                    //        var doanhThuDv = yeuCauDichVuGiuong.Gia - (yeuCauDichVuGiuong.Gia * (decimal)yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100);
                    //        itemData.DoanhThuTheoKySoSanh = itemData.DoanhThuTheoKySoSanh.GetValueOrDefault() + doanhThuDv;
                    //        itemData.MienGiamTheoKySoSanh = itemData.MienGiamTheoKySoSanh.GetValueOrDefault() + tiLeMienGiam * doanhThuDv;
                    //        itemData.ChiPhiKhacTheoKySoSanh = itemData.ChiPhiKhacTheoKySoSanh.GetValueOrDefault() + tiLeGiamTruKhac * doanhThuDv;
                    //    }
                    //}
                }
            }
            return itemData;
        }

        #region Báo cáo xuất nhập tồn

        public async Task<GridDataSource> GetDataBaoCaoXuatNhapTonForGridAsync(BaoCaoXuatNhapTonQueryInfo queryInfo)
        {
            IQueryable<NhapKhoDuocPhamChiTiet> allDataNhapQuery = null;
            if (queryInfo.KhoId != (long)EnumKhoDuocPham.KhoTongDuocPham)
            {
                allDataNhapQuery = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(o =>
                        o.NhapKhoDuocPhams.KhoId == queryInfo.KhoId && o.NgayNhap <= queryInfo.ToDate);
            }
            else
            {
                allDataNhapQuery = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(o =>
                        o.NhapKhoDuocPhams.KhoId == queryInfo.KhoId && o.KhoNhapSauKhiDuyetId == null && o.NgayNhap <= queryInfo.ToDate);                
            }

            var allDataNhap = allDataNhapQuery
                    .Select(o => new BaoCaoChiTietXuatNhapTonGridVo()
                    {
                        Id = o.Id,
                        DuocPhamBenhVienId = o.DuocPhamBenhVienId,
                        Ma = o.DuocPhamBenhViens.Ma,
                        Ten = o.DuocPhamBenhViens.DuocPham.Ten,
                        HamLuong = o.DuocPhamBenhViens.DuocPham.HamLuong,
                        DVT = o.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                        SoLo = o.Solo,
                        //tinh don gia ton kho
                        DonGiaNhap = o.DonGiaTonKho,
                        VAT = 0,
                        NgayNhapXuat = o.NgayNhap,
                        LaDuocPhamBHYT = o.LaDuocPhamBHYT,
                        DuocPhamBenhVienPhanNhomId = o.DuocPhamBenhViens.DuocPhamBenhVienPhanNhomId,
                        //Nhom = o.DuocPhamBenhViens.DuocPhamBenhVienPhanNhomId != null ? o.DuocPhamBenhViens.DuocPhamBenhVienPhanNhom.Ten : "Các thuốc khác",
                        SLNhap = o.SoLuongNhap,
                        SLXuat = 0
                    }).ToList();

            IQueryable<XuatKhoDuocPhamChiTietViTri> allDataXuatQuery = null;
            if (queryInfo.KhoId != (long)EnumKhoDuocPham.KhoTongDuocPham)
            {
                allDataXuatQuery = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking
                .Where(o => o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null &&
                            o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoXuatId == queryInfo.KhoId
                            && ((o.NgayXuat != null && o.NgayXuat <= queryInfo.ToDate) ||
                                (o.NgayXuat == null && o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat <= queryInfo.ToDate)));
            }
            else
            {
                allDataXuatQuery = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking
                .Where(o => o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null &&
                            o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoXuatId == queryInfo.KhoId && o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.LyDoXuatKho != Constants.StringXuatNhapKho.LyDoXuatVeKhoSauKhiDuyet
                            && ((o.NgayXuat != null && o.NgayXuat <= queryInfo.ToDate) ||
                                (o.NgayXuat == null && o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat <= queryInfo.ToDate)));                
            }

            var allDataXuat = allDataXuatQuery
                .Select(o => new BaoCaoChiTietXuatNhapTonGridVo
                {
                    Id = o.Id,
                    DuocPhamBenhVienId = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienId,
                    Ma = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.Ma,
                    Ten = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.Ten,
                    HamLuong = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.HamLuong,
                    DVT = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                    SoLo = o.NhapKhoDuocPhamChiTiet.Solo,
                    //tinh don gia ton kho
                    DonGiaNhap = o.NhapKhoDuocPhamChiTiet.DonGiaTonKho,
                    VAT = 0,
                    NgayNhapXuat = o.NgayXuat != null
                        ? o.NgayXuat.Value
                        : o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat,
                    LaDuocPhamBHYT = o.NhapKhoDuocPhamChiTiet.LaDuocPhamBHYT,
                    DuocPhamBenhVienPhanNhomId= o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPhamBenhVienPhanNhomId,
                    //Nhom = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPhamBenhVienPhanNhom != null ? o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPhamBenhVienPhanNhom.Ten : "Các thuốc khác",
                    SLNhap = o.SoLuongXuat < 0 ? o.SoLuongXuat * (-1) : 0,
                    SLXuat = o.SoLuongXuat > 0 ? o.SoLuongXuat : 0,
                }).ToList();

            var allDataNhapXuat = allDataNhap.Concat(allDataXuat).OrderBy(o => o.Ten).ToList();

            var allDataGroup = allDataNhapXuat.GroupBy(o => new { o.LaDuocPhamBHYT, o.DuocPhamBenhVienId, o.SoLo, o.DonGiaNhapSauVAT });
            var dataReturn = new List<BaoCaoXuatNhapTonGridVo>();
            var duocPhamBenhVienPhanNhoms = _duocPhamBenhVienPhanNhomRepository.TableNoTracking.ToList();
            foreach (var xuatNhapDuocPham in allDataGroup)
            {
                var tonDau = xuatNhapDuocPham.Where(o => o.NgayNhapXuat < queryInfo.FromDate)
                    .Select(o => o.SLNhap.GetValueOrDefault() - o.SLXuat.GetValueOrDefault()).DefaultIfEmpty(0).Sum().MathRoundNumber(2);
                var allDataNhapXuatTuNgay = xuatNhapDuocPham.Where(o => o.NgayNhapXuat >= queryInfo.FromDate).ToList();
                var nhapTrongKy = allDataNhapXuatTuNgay.Select(o => o.SLNhap.GetValueOrDefault()).DefaultIfEmpty().Sum().MathRoundNumber(2);
                var xuatTrongKy = allDataNhapXuatTuNgay.Select(o => o.SLXuat.GetValueOrDefault()).DefaultIfEmpty().Sum().MathRoundNumber(2);
                var tonCuoi = (tonDau + nhapTrongKy - xuatTrongKy).MathRoundNumber(2);

                if (!tonDau.AlmostEqual(0) || !nhapTrongKy.AlmostEqual(0) || !xuatTrongKy.AlmostEqual(0) || !tonCuoi.AlmostEqual(0))
                {
                    dataReturn.Add(new BaoCaoXuatNhapTonGridVo
                    {
                        Ten = xuatNhapDuocPham.First().Ten,
                        HamLuong = xuatNhapDuocPham.First().HamLuong,
                        DVT = xuatNhapDuocPham.First().DVT,
                        SoLo = xuatNhapDuocPham.Key.SoLo,
                        SLTonDauKy = tonDau,
                        DonGiaTonDauKy = tonDau > 0 ? xuatNhapDuocPham.Key.DonGiaNhapSauVAT : (decimal?)null,
                        SLNhapTrongKy = nhapTrongKy,
                        DonGiaNhapTrongKy = nhapTrongKy > 0 ? xuatNhapDuocPham.Key.DonGiaNhapSauVAT : (decimal?)null,
                        SLXuatTrongKy = xuatTrongKy,
                        DonGiaXuatTrongKy = xuatTrongKy > 0 ? xuatNhapDuocPham.Key.DonGiaNhapSauVAT : (decimal?)null,
                        SLTonCuoiKy = tonCuoi,
                        DonGiaTonCuoiKy = tonCuoi > 0 ? xuatNhapDuocPham.Key.DonGiaNhapSauVAT : (decimal?)null,
                        Loai = xuatNhapDuocPham.Key.LaDuocPhamBHYT ? "Thuốc BHYT" : "Viện phí",
                        DuocPhamBenhVienPhanNhomId = xuatNhapDuocPham.First().DuocPhamBenhVienPhanNhomId,
                        Nhom=duocPhamBenhVienPhanNhoms.FirstOrDefault(o=>o.Id== xuatNhapDuocPham.First().DuocPhamBenhVienPhanNhomId)?.Ten??"Các thuốc khác"
                    });
                }
            }
            return new GridDataSource { Data = dataReturn.OrderBy(s => s.Loai).ThenBy(s => s.Nhom).ToArray(), TotalRowCount = dataReturn.Count };
        }

        public async Task<GridDataSource> GetTotalBaoCaoXuatNhapTonForGridAsync(BaoCaoXuatNhapTonQueryInfo queryInfo)
        {
            return null;
        }

        public async Task<GridDataSource> GetDataBaoCaoXuatNhapTonForGridAsyncChild(BaoCaoXuatNhapTonQueryInfo queryInfo, bool exportExcel)
        {
            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var queryObj = queryInfo.AdditionalSearchString.Split(";");
            var khoId = long.Parse(queryObj[0]);
            var NhomDuocPhamVatTuId = long.Parse(queryObj[1]);
            //var tuNgay = queryObj[2];
            //var denNgay = queryObj[3];
            var listGridVos = new List<BaoCaoXuatNhapTonGridVo>();
            return new GridDataSource { Data = listGridVos.Take(queryInfo.Take).ToArray(), TotalRowCount = listGridVos.Count };
        }

        public async Task<GridDataSource> GetTotalBaoCaoXuatNhapTonForGridAsyncChild(BaoCaoXuatNhapTonQueryInfo queryInfo)
        {
            return null;
        }

        public string InBaoCaoXuatNhapTon(InBaoCaoXuatNhapTonVo inBaoCaoXuatNhapTon)
        {
            var content = string.Empty;
            var template = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("BaoCaoXuatNhapTon")).First();
            var datas = new List<BaoCaoXuatNhapTonGridVo>()
            {
                new BaoCaoXuatNhapTonGridVo
                {
                    Id = 1,
                    Ten = "Băng chun 3 móc QM 10.2cm x 5.5m XXXXX",
                    HamLuong = "ABC",
                    SoLo = "123",
                    DVT = "Cuộn",
                    SLTonDauKy = 21,
                    SLNhapTrongKy = 0,
                    SLXuatTrongKy = 1,
                    SLTonCuoiKy = 20,
                    DonGiaTonCuoiKy = 14000,
                    Loai = "Viện phí",
                    Nhom = "A1",
                    DonGiaTonDauKy = 2000,
                    DonGiaNhapTrongKy = 3000,
                    DonGiaXuatTrongKy = 4000

                },
                new BaoCaoXuatNhapTonGridVo
                {
                    Id = 2,
                    Ten = "Bông viên 10g (Bảo Thạch, Việt Nam)",
                    HamLuong = "ABC",
                    SoLo = "123",
                    DVT = "Gói",
                    SLTonDauKy = 20,
                    SLNhapTrongKy = 0,
                    SLXuatTrongKy = 4,
                    SLTonCuoiKy = 16,
                    DonGiaTonCuoiKy = 2700,
                    Loai = "Viện phí",
                    Nhom = "A1",
                    DonGiaTonDauKy = 2000,
                    DonGiaNhapTrongKy = 3000,
                    DonGiaXuatTrongKy = 4000

                },
            new BaoCaoXuatNhapTonGridVo
            {
                Id = 3,
                Ten = "Gạc miếng 10x10cm x 12 lớp",
                HamLuong = "ABC",
                SoLo = "123",
                DVT = "Túi",
                SLTonDauKy = 103,
                SLNhapTrongKy = 0,
                SLXuatTrongKy = 38,
                SLTonCuoiKy = 65,
                DonGiaTonCuoiKy = 7970,
                Loai = "Viện phí",
                Nhom = "A1",
                DonGiaTonDauKy = 2000,
                DonGiaNhapTrongKy = 3000,
                DonGiaXuatTrongKy = 4000

            }, new BaoCaoXuatNhapTonGridVo
            {
                Id = 4,
                Ten = "Tưa lưỡi trẻ em hộp (Đông Pha, Việt Nam)",
                HamLuong = "ABC",
                SoLo = "123",
                DVT = "Hộp",
                SLTonDauKy = 100,
                SLNhapTrongKy = 0,
                SLXuatTrongKy = 9,
                SLTonCuoiKy = 91,
                DonGiaTonCuoiKy = 1600,
                Loai = "Viện phí",
                Nhom = "A1",
                DonGiaTonDauKy = 2000,
                DonGiaNhapTrongKy = 3000,
                DonGiaXuatTrongKy = 4000

            }, new BaoCaoXuatNhapTonGridVo
            {
                Id = 5,
                Ten = "Aerius 0,5mg/ml 60ml",
                HamLuong = "ABC",
                SoLo = "123",
                DVT = "Chai",
                SLTonDauKy = 15,
                SLNhapTrongKy = 0,
                SLXuatTrongKy = 15,
                SLTonCuoiKy = 0,
                DonGiaTonCuoiKy = 0,
                Loai = "BHYT",
                Nhom = "A2",
                DonGiaTonDauKy = 2000,
                DonGiaNhapTrongKy = 3000,
                DonGiaXuatTrongKy = 4000


            }, new BaoCaoXuatNhapTonGridVo
            {
                Id = 6,
                Ten = "Rocuronium-BFS",
                HamLuong = "ABC",
                SoLo = "123",
                DVT = "Gói",
                SLTonDauKy = 16,
                SLNhapTrongKy = 0,
                SLXuatTrongKy = 2,
                SLTonCuoiKy = 4,
                DonGiaTonCuoiKy = 38000,
                Loai = "Viện phí",
                Nhom = "A1",
                DonGiaTonDauKy = 2000,
                DonGiaNhapTrongKy = 3000,
                DonGiaXuatTrongKy = 4000


            },new BaoCaoXuatNhapTonGridVo
            {
                Id = 7,
                Ten = "Rhomatic Gel a",
                HamLuong = "ABC",
                SoLo = "123",
                DVT = "Lít",
                SLTonDauKy = 46,
                SLNhapTrongKy = 0,
                SLXuatTrongKy = 2,
                SLTonCuoiKy = 4,
                DonGiaTonCuoiKy = 38000,
                Loai = "Viện phí",
                Nhom = "A1",
                DonGiaTonDauKy = 2000,
                DonGiaNhapTrongKy = 3000,
                DonGiaXuatTrongKy = 4000

            }, new BaoCaoXuatNhapTonGridVo
            {
                Id = 8,
                Ten = "Flucinar 15g",
                HamLuong = "ABC",
                SoLo = "123",
                DVT = "Tuýp",
                SLTonDauKy = 6,
                SLNhapTrongKy = 0,
                SLXuatTrongKy = 2,
                SLTonCuoiKy = 4,
                DonGiaTonCuoiKy = 38000,
                Loai = "Viện phí",
                Nhom = "A1",
                DonGiaTonDauKy = 2000,
                DonGiaNhapTrongKy = 3000,
                DonGiaXuatTrongKy = 4000

            }, new BaoCaoXuatNhapTonGridVo
            {
                Id = 11,
                Ten = "Băng chun 16 móc QM 10.2cm x 5.5m 1235435435",
                HamLuong = "ABC",
                SoLo = "123",
                DVT = "Cuộn",
                SLTonDauKy = 25,
                SLNhapTrongKy = 0,
                SLXuatTrongKy = 1,
                SLTonCuoiKy = 20,
                DonGiaTonCuoiKy = 14000,
                Loai = "BHYT",
                Nhom = "A2",
                DonGiaTonDauKy = 2000,
                DonGiaNhapTrongKy = 3000,
                DonGiaXuatTrongKy = 4000

            },new BaoCaoXuatNhapTonGridVo
            {
                Id = 12,
                Ten = "Băng chun 13 móc QM 10.2cm x 5.5m ABCSTTS",
                HamLuong = "ABC",
                SoLo = "123",
                DVT = "Cuộn",
                SLTonDauKy = 11,
                SLNhapTrongKy = 0,
                SLXuatTrongKy = 1,
                SLTonCuoiKy = 20,
                DonGiaTonCuoiKy = 14000,
                Loai = "BHYT",
                Nhom = "A2",
                DonGiaTonDauKy = 2000,
                DonGiaNhapTrongKy = 3000,
                DonGiaXuatTrongKy = 4000

            }
            };

            var lstLoai = datas.GroupBy(x => new { x.Loai })
                .Select(item => new LoaiGroupVo
                {
                    Loai = item.First().Loai

                }).OrderBy(p => p.Loai).ToList();
            var lstNhom = datas.GroupBy(x => new { x.Loai })
                .Select(item => new NhomGroupVo
                {
                    Loai = item.First().Loai,
                    Nhom = item.First().Nhom

                }).OrderBy(p => p.Nhom).ToList();
            var stt = 1;
            var html = "";
            if (lstLoai.Any())
            {
                foreach (var loai in lstLoai)
                {
                    var listNhomTheoLoai = lstNhom.Where(o => o.Loai == loai.Loai).ToList();
                    if (listNhomTheoLoai.Any())
                    {
                        html += "<tr><td colspan='5'>" + loai.Loai + "</td><td></td><td></td><td></td><td></td><td></td></tr>";
                        foreach (var nhom in listNhomTheoLoai)
                        {
                            var listDuocPhamTheoNhom =
                                datas.Where(o => o.Loai == loai.Loai && o.Nhom == nhom.Nhom).ToList();
                            if (listDuocPhamTheoNhom.Any())
                            {
                                html += "<tr><td></td><td colspan='11'>" + nhom.Loai + "</td></tr>";
                                foreach (var duocPham in listDuocPhamTheoNhom)
                                {
                                    stt++;
                                }
                            }
                        }
                    }
                }
            }

            var data = new InBaoCaoXuatNhapTonData
            {
                LogoUrl = inBaoCaoXuatNhapTon.HostingName + "/assets/img/logo-bacha-full.png",
                TenKho = "Tất cả",
                ThoiGian = inBaoCaoXuatNhapTon.FromDate.Replace("AM", "SA").Replace("PM", "CH") + " đến ngày " + inBaoCaoXuatNhapTon.ToDate.Replace("AM", "SA").Replace("PM", "CH"),
                BaoCaoXuatNhapTon = html
            };
            content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
            return content;
        }

        public virtual byte[] ExportBaoCaoXuatNhapTon(GridDataSource gridDataSource, BaoCaoXuatNhapTonQueryInfo query)
        {
            var datas = (ICollection<BaoCaoXuatNhapTonGridVo>)gridDataSource.Data;
            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoXuatNhapTonGridVo>("STT", p => ind++)
            };
            var lstLoai = datas.GroupBy(x => new { x.Loai })
                .Select(item => new LoaiGroupVo
                {
                    Loai = item.First().Loai

                }).OrderBy(p => p.Loai).ToList();
            var lstNhom = datas.GroupBy(x => new { x.Nhom, x.Loai })
               .Select(item => new NhomGroupVo
               {
                   Loai = item.First().Loai,
                   Nhom = item.First().Nhom

               }).OrderBy(p => p.Nhom).ToList();
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO XUẤT NHẬP TỒN");

                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 40;
                    worksheet.Column(3).Width = 20;
                    worksheet.Column(4).Width = 15;
                    worksheet.Column(5).Width = 10;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 15;
                    worksheet.Column(8).Width = 15;
                    worksheet.Column(9).Width = 15;
                    worksheet.Column(10).Width = 15;
                    worksheet.Column(11).Width = 15;
                    worksheet.Column(12).Width = 15;
                    worksheet.Column(13).Width = 15;
                    worksheet.Column(14).Width = 15;
                    worksheet.Column(15).Width = 15;
                    worksheet.Column(16).Width = 15;
                    worksheet.Column(17).Width = 15;
                    worksheet.DefaultColWidth = 7;

                    //SET img 
                    using (var range = worksheet.Cells["A1:Q1"])
                    {
                        //                        var url = hostingName + "/assets/img/logo-bacha-full.png";
                        //                        WebClient wc = new WebClient();
                        //                        byte[] bytes = wc.DownloadData(url); // download file từ server
                        //                        MemoryStream ms = new MemoryStream(bytes); //
                        //                        Image img = Image.FromStream(ms); // chuyển đổi thành img
                        //                        ExcelPicture pic = range.Worksheet.Drawings.AddPicture("Logo", img);
                        //                        pic.SetPosition(0, 0, 0, 0);
                        //                        var height = 120; // chiều cao từ A1 đến A6
                        //                        var width = 510; // chiều rộng từ A1 đến D1
                        //                        pic.SetSize(width, height);
                        //                        range.Worksheet.Protection.IsProtected = false;
                        //                        range.Worksheet.Protection.AllowSelectLockedCells = false;
                        range.Worksheet.Cells["A1:G1"].Merge = true;
                        range.Worksheet.Cells["A1:G1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:G1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:G1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:G1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:G1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:G1"].Style.Font.Bold = true;
                    }

                    // SET title head cho bảng excel
                    using (var range = worksheet.Cells["A3:Q3"])
                    {
                        range.Worksheet.Cells["A3:Q3"].Merge = true;
                        range.Worksheet.Cells["A3:Q3"].Value = "BÁO CÁO XUẤT NHẬP TỒN";
                        range.Worksheet.Cells["A3:Q3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:Q3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:Q3"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["A3:Q3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:Q3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A4:Q4"])
                    {
                        range.Worksheet.Cells["A4:Q4"].Merge = true;
                        range.Worksheet.Cells["A4:Q4"].Value = "Từ ngày: " + query.FromDate.FormatNgayGioTimKiemTrenBaoCao()
                                                          + " - đến ngày: " + query.ToDate.FormatNgayGioTimKiemTrenBaoCao();
                        range.Worksheet.Cells["A4:Q4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:Q4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:Q4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A4:Q4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:Q4"].Style.Font.Bold = true;
                    }

                    var tenKho = string.Empty;
                    if (query.KhoId == 0)
                    {
                        tenKho = "Tất cả";
                    }
                    else
                    {
                        tenKho = _khoRepository.TableNoTracking.Where(p => p.Id == query.KhoId).Select(p => p.Ten).FirstOrDefault();
                    }
                    using (var range = worksheet.Cells["A5:Q5"])
                    {
                        range.Worksheet.Cells["A5:Q5"].Merge = true;
                        range.Worksheet.Cells["A5:Q5"].Value = "Kho: " + tenKho;
                        range.Worksheet.Cells["A5:Q5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A5:Q5"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A5:Q5"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A5:Q5"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A5:Q5"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A7:Q7"])
                    {
                        range.Worksheet.Cells["A7:Q7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A7:Q7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A7:Q7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A7:Q7"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A7:Q7"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A7:Q7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A8:Q8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A8:Q8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A8:Q8"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A8:Q8"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A8:Q8"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A8:Q8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["A7:A8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A7:A8"].Merge = true;
                        range.Worksheet.Cells["A7:A8"].Value = "STT";

                        range.Worksheet.Cells["B7:B8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B7:B8"].Merge = true;
                        range.Worksheet.Cells["B7:B8"].Value = "Tên thuốc, Vật tư, Hoá chất";

                        range.Worksheet.Cells["C7:C8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C7:C8"].Merge = true;
                        range.Worksheet.Cells["C7:C8"].Value = "Hàm lượng";

                        range.Worksheet.Cells["D7:D8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D7:D8"].Merge = true;
                        range.Worksheet.Cells["D7:D8"].Value = "Số lô";

                        range.Worksheet.Cells["E7:E8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E7:E8"].Merge = true;
                        range.Worksheet.Cells["E7:E8"].Value = "ĐVT";

                        range.Worksheet.Cells["F7:H7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F7:H7"].Merge = true;
                        range.Worksheet.Cells["F7:H7"].Value = "Tồn đầu kỳ";
                        range.Worksheet.Cells["F8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["F8"].Value = "Số lượng";
                        range.Worksheet.Cells["G8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["G8"].Value = "Giá";
                        range.Worksheet.Cells["H8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["H8"].Value = "Thành tiền";

                        range.Worksheet.Cells["I7:K7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I7:K7"].Merge = true;
                        range.Worksheet.Cells["I7:K7"].Value = "Nhập trong kỳ";
                        range.Worksheet.Cells["I8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["I8"].Value = "Số lượng";
                        range.Worksheet.Cells["J8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["J8"].Value = "Giá";
                        range.Worksheet.Cells["K8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["K8"].Value = "Thành tiền";

                        range.Worksheet.Cells["L7:N7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["L7:N7"].Merge = true;
                        range.Worksheet.Cells["L7:N7"].Value = "Xuất trong kỳ";
                        range.Worksheet.Cells["L8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["L8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["L8"].Value = "Số lượng";
                        range.Worksheet.Cells["M8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["M8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["M8"].Value = "Giá";
                        range.Worksheet.Cells["N8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["N8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["N8"].Value = "Thành tiền";

                        range.Worksheet.Cells["O7:Q7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["O7:Q7"].Merge = true;
                        range.Worksheet.Cells["O7:Q7"].Value = "Tồn cuối kỳ";
                        range.Worksheet.Cells["O8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["O8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["O8"].Value = "Số lượng";
                        range.Worksheet.Cells["P8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["P8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["P8"].Value = "Giá";
                        range.Worksheet.Cells["Q8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["Q8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["Q8"].Value = "Thành tiền";
                    }

                    var manager = new PropertyManager<BaoCaoXuatNhapTonGridVo>(requestProperties);
                    int index = 9; // bắt đầu đổ data từ dòng 13

                    ///////Đổ data vào bảng excel
                    ///
                    var stt = 1;
                    if (lstLoai.Any())
                    {
                        foreach (var loai in lstLoai)
                        {
                            var listNhomTheoLoai = lstNhom.Where(o => o.Loai == loai.Loai).ToList();
                            if (listNhomTheoLoai.Any())
                            {
                                using (var range = worksheet.Cells["A" + index + ":Q" + index])
                                {
                                    range.Worksheet.Cells["A" + index + ":Q" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                    range.Worksheet.Cells["A" + index + ":Q" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                    range.Worksheet.Cells["A" + index + ":Q" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["A" + index + ":Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["A" + index + ":Q" + index].Style.Font.Bold = true;

                                    range.Worksheet.Cells["A" + index + ":E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["A" + index + ":E" + index].Value = loai.Loai;
                                    range.Worksheet.Cells["A" + index + ":E" + index].Merge = true;

                                    worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                    worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["H" + index].Style.Font.Bold = true;
                                    worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["H" + index].Value = datas.Where(o => o.Loai == loai.Loai).Sum(p => p.ThanhTienTonDauKy);

                                    worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                    worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["K" + index].Style.Font.Bold = true;
                                    worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["K" + index].Value = datas.Where(o => o.Loai == loai.Loai).Sum(p => p.ThanhTienNhapTrongKy);

                                    worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                    worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["N" + index].Style.Font.Bold = true;
                                    worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["N" + index].Value = datas.Where(o => o.Loai == loai.Loai).Sum(p => p.ThanhTienXuatTrongKy);

                                    worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                    worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["Q" + index].Style.Font.Bold = true;
                                    worksheet.Cells["Q" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["Q" + index].Value = datas.Where(o => o.Loai == loai.Loai).Sum(p => p.ThanhTienTonCuoiKy);
                                }
                                index++;
                                foreach (var nhom in listNhomTheoLoai)
                                {
                                    using (var range = worksheet.Cells["B" + index + ":Q" + index])
                                    {
                                        range.Worksheet.Cells["B" + index + ":Q" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                        range.Worksheet.Cells["B" + index + ":Q" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                        range.Worksheet.Cells["B" + index + ":Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["B" + index + ":Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["B" + index + ":Q" + index].Style.Font.Color.SetColor(Color.Black);
                                        range.Worksheet.Cells["B" + index + ":Q" + index].Style.Font.Bold = true;

                                        range.Worksheet.Cells["B" + index + ":E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["B" + index + ":E" + index].Value = nhom.Nhom;
                                        range.Worksheet.Cells["B" + index + ":E" + index].Merge = true;

                                        range.Worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                        worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        worksheet.Cells["H" + index].Style.Font.Bold = true;
                                        worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                                        worksheet.Cells["H" + index].Value = datas.Where(o => o.Loai == loai.Loai && o.Nhom == nhom.Nhom).Sum(p => p.ThanhTienTonDauKy);

                                        range.Worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                        worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        worksheet.Cells["KI" + index].Style.Font.Bold = true;
                                        worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                                        worksheet.Cells["K" + index].Value = datas.Where(o => o.Loai == loai.Loai && o.Nhom == nhom.Nhom).Sum(p => p.ThanhTienNhapTrongKy);

                                        range.Worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                        worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        worksheet.Cells["N" + index].Style.Font.Bold = true;
                                        worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                                        worksheet.Cells["N" + index].Value = datas.Where(o => o.Loai == loai.Loai && o.Nhom == nhom.Nhom).Sum(p => p.ThanhTienXuatTrongKy);

                                        range.Worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                        worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        worksheet.Cells["Q" + index].Style.Font.Bold = true;
                                        worksheet.Cells["Q" + index].Style.Numberformat.Format = "#,##0.00";
                                        worksheet.Cells["Q" + index].Value = datas.Where(o => o.Loai == loai.Loai && o.Nhom == nhom.Nhom).Sum(p => p.ThanhTienTonCuoiKy);
                                    }
                                    index++;

                                    var listDuocPhamTheoNhom = datas.Where(o => o.Loai == loai.Loai && o.Nhom == nhom.Nhom).ToList();
                                    if (listDuocPhamTheoNhom.Any())
                                    {
                                        foreach (var duocPham in listDuocPhamTheoNhom)
                                        {
                                            manager.CurrentObject = duocPham;
                                            //// format border, font chữ,....
                                            worksheet.Cells["A" + index + ":Q" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                            worksheet.Cells["A" + index + ":Q" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            worksheet.Cells["A" + index + ":Q" + index].Style.Font.Color.SetColor(Color.Black);
                                            //worksheet.Cells["A" + index + ":B" + index].Style.Font.Bold = true;
                                            worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                            worksheet.Row(index).Height = 20.5;
                                            manager.WriteToXlsx(worksheet, index);
                                            // Đổ data
                                            worksheet.Cells["A" + index + ":Q" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                            worksheet.Cells["A" + index + ":Q" + index].Style.Font.Color.SetColor(Color.Black);
                                            worksheet.Cells["A" + index + ":Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["A" + index + ":Q" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                                            worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["A" + index].Value = stt;

                                            worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["B" + index].Value = duocPham.Ten;

                                            worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["C" + index].Value = duocPham.HamLuong;

                                            worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["D" + index].Value = duocPham.SoLo;

                                            worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["E" + index].Value = duocPham.DVT;

                                            worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["F" + index].Value = duocPham.SLTonDauKy;

                                            worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["G" + index].Value = duocPham.DonGiaTonDauKy;

                                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["H" + index].Value = duocPham.ThanhTienTonDauKy;

                                            worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["I" + index].Value = duocPham.SLNhapTrongKy;

                                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["J" + index].Value = duocPham.DonGiaNhapTrongKy;

                                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["K" + index].Value = duocPham.ThanhTienNhapTrongKy;

                                            worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["L" + index].Value = duocPham.SLXuatTrongKy;

                                            worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["M" + index].Value = duocPham.DonGiaXuatTrongKy;

                                            worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["N" + index].Value = duocPham.ThanhTienXuatTrongKy;

                                            worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["O" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["O" + index].Value = duocPham.SLTonCuoiKy;

                                            worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["P" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["P" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["P" + index].Value = duocPham.DonGiaTonCuoiKy;

                                            worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["Q" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["Q" + index].Value = duocPham.ThanhTienTonCuoiKy;
                                            index++;
                                            stt++;
                                        }
                                    }
                                }
                            }
                        }

                        //footer tính tổng số tiền
                        //set font size, merge,...
                        worksheet.Cells["A" + index + ":Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black); //set border
                        worksheet.Cells["A" + index + ":Q" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        worksheet.Cells["A" + index + ":Q" + index].Style.Font.Bold = true;
                        worksheet.Cells["A" + index + ":E" + index].Merge = true;
                        //value
                        worksheet.Cells["A" + index].Value = "Tổng cộng";
                        worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["H" + index].Value = datas.Sum(p => p.ThanhTienTonDauKy);


                        worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["K" + index].Value = datas.Sum(p => p.ThanhTienNhapTrongKy);

                        worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["N" + index].Value = datas.Sum(p => p.ThanhTienXuatTrongKy);

                        worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells["Q" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["Q" + index].Value = datas.Sum(p => p.ThanhTienTonCuoiKy);
                        index++;
                    }

                    index = index + 3;

                    worksheet.Cells["A" + index + ":Q" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["A" + index + ":Q" + index].Style.Font.Bold = true;
                    //value
                    worksheet.Cells["A" + index + ":E" + index].Value = "Người lập";
                    worksheet.Cells["A" + index + ":E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A" + index + ":E" + index].Merge = true;

                    worksheet.Cells["F" + index + ":H" + index].Value = "Thủ kho";
                    worksheet.Cells["F" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["F" + index + ":H" + index].Merge = true;

                    worksheet.Cells["I" + index + ":K" + index].Value = "Kế toán";
                    worksheet.Cells["I" + index + ":K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["I" + index + ":K" + index].Merge = true;

                    worksheet.Cells["L" + index + ":N" + index].Value = "Trưởng khoa dược/VTYT";
                    worksheet.Cells["L" + index + ":N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["L" + index + ":N" + index].Merge = true;

                    //Ngày tháng  ký
                    var rowNgay = index - 2;
                    var ngayHienTai = DateTime.Now;
                    worksheet.Cells["O" + rowNgay + ":Q" + rowNgay].Value = "Ngày " + ngayHienTai.Day + " Tháng " + ngayHienTai.Month + " Năm " + ngayHienTai.Year;
                    worksheet.Cells["O" + rowNgay + ":Q" + rowNgay].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["O" + rowNgay + ":Q" + rowNgay].Merge = true;
                   

                    worksheet.Cells["O" + index + ":Q" + index].Value = "Trường bộ phận";
                    worksheet.Cells["O" + index + ":Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["O" + index + ":Q" + index].Merge = true;

                    index++;



                    worksheet.Cells["A" + index + ":O" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["A" + index + ":O" + index].Style.Font.Italic = true;
                    //value
                    worksheet.Cells["A" + index + ":E" + index].Value = "(ký, ghi rõ họ tên)";
                    worksheet.Cells["A" + index + ":E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A" + index + ":E" + index].Merge = true;

                    worksheet.Cells["F" + index + ":H" + index].Value = "(ký, ghi rõ họ tên)";
                    worksheet.Cells["F" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["F" + index + ":H" + index].Merge = true;

                    worksheet.Cells["I" + index + ":K" + index].Value = "(ký, ghi rõ họ tên)";
                    worksheet.Cells["I" + index + ":K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["I" + index + ":K" + index].Merge = true;

                    worksheet.Cells["L" + index + ":N" + index].Value = "(ký, ghi rõ họ tên)";
                    worksheet.Cells["L" + index + ":N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["L" + index + ":N" + index].Merge = true;

                    worksheet.Cells["O" + index + ":Q" + index].Value = "(ký, ghi rõ họ tên)";
                    worksheet.Cells["O" + index + ":Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["O" + index + ":Q" + index].Merge = true;
                    index++;


                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }
        #endregion



        #region Báo cáo tiếp nhận người bệnh khám

        public async Task<GridDataSource> GetDataBaoCaoTiepNhanBenhNhanKhamForGridAsync(QueryInfo queryInfo)
        {
            var yeuCauTiepNhans = new List<BaoCaoTNBenhNhanKhamGridVo>();

            var timKiemNangCaoObj = new BaoCaoTiepNhanNguoiBenhKhamQueryInfo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoTiepNhanNguoiBenhKhamQueryInfo>(queryInfo.AdditionalSearchString);
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.FromDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.FromDate)) //) || !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.FromDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (timKiemNangCaoObj.ToDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.ToDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.ToDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }

            if (tuNgay != null && denNgay != null)
            {
                var allYeuCauTiepNhans = _yeuCauTiepNhanRepository.TableNoTracking
                    .Where(x => x.LoaiYeuCauTiepNhan != Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru
                                && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                                && x.ThoiDiemTiepNhan >= tuNgay
                                && x.ThoiDiemTiepNhan <= denNgay)
                                //&& (x.YeuCauKhamBenhs.Any(a => a.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                                //    || x.YeuCauDichVuKyThuats.Any(a => a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)))
                    .Select(item => new BaoCaoTNBenhNhanKhamGridVo
                    {
                        Id = item.Id,
                        MaTN = item.MaYeuCauTiepNhan,
                        ThoiGianTiepNhan = item.ThoiDiemTiepNhan,
                        HoTenBN = item.HoTen,
                        NgaySinh = item.NgaySinh,
                        ThangSinh = item.ThangSinh,
                        NamSinh = item.NamSinh,
                        GioiTinhDisplay = item.GioiTinh.GetDescription(),
                        BHTYMaSoThe = item.BHYTMaSoThe,
                        //TrangThaiDisplay = item.TrangThaiYeuCauTiepNhan.GetDescription(),
                        HinhThucDen = item.HinhThucDen != null ? item.HinhThucDen.Ten : "",
                        NoiGioiThieu = item.NoiGioiThieu != null ? item.NoiGioiThieu.Ten : "",
                        BaoCaoTNBenhNhanKhamThongTinYeuCauKhamBenhVos = item.YeuCauKhamBenhs.Select(o=>new BaoCaoTNBenhNhanKhamThongTinYeuCauKhamBenhVo{Id = o.Id, TrangThai = o.TrangThai}).ToList(),
                        BaoCaoTNBenhNhanKhamThongTinYeuCauDichVuKyThuatVos = item.YeuCauDichVuKyThuats.Select(o => new BaoCaoTNBenhNhanKhamThongTinYeuCauDichVuKyThuatVo { Id = o.Id, TrangThai = o.TrangThai }).ToList(),
                        //Nhom = item.YeuCauKhamBenhs.Any(a => a.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                        //    ? Enums.EnumLoaiTiepNhanNguoiBenh.NguoiBenhCoPhieuKham
                        //    : Enums.EnumLoaiTiepNhanNguoiBenh.NguoiBenhChiLamDichVu
                    })
                    //.OrderByDescending(x => x.Nhom == Enums.EnumLoaiTiepNhanNguoiBenh.NguoiBenhCoPhieuKham)
                    //.ThenByDescending(x => x.Nhom == Enums.EnumLoaiTiepNhanNguoiBenh.NguoiBenhChiLamDichVu)
                    //.ThenBy(x => x.ThoiGianTiepNhan)
                    //.Skip(queryInfo.Skip).Take(queryInfo.Take)
                    .ToList();

                yeuCauTiepNhans = allYeuCauTiepNhans
                    .Where(x => x.BaoCaoTNBenhNhanKhamThongTinYeuCauKhamBenhVos.Any(a => a.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham) || x.BaoCaoTNBenhNhanKhamThongTinYeuCauDichVuKyThuatVos.Any(a => a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
                    .OrderByDescending(x => x.Nhom == Enums.EnumLoaiTiepNhanNguoiBenh.NguoiBenhCoPhieuKham)
                    .ThenByDescending(x => x.Nhom == Enums.EnumLoaiTiepNhanNguoiBenh.NguoiBenhChiLamDichVu)
                    .ThenBy(x => x.ThoiGianTiepNhan)
                    .Skip(queryInfo.Skip).Take(queryInfo.Take)
                    .ToList();

                var lstYeuCauTiepNhanIdCoPhieuKham = yeuCauTiepNhans
                    .Where(x => x.Nhom == Enums.EnumLoaiTiepNhanNguoiBenh.NguoiBenhCoPhieuKham)
                    .Select(x => x.Id).Distinct()
                    .ToList();
                var lstYeuCauTiepNhanIdChiLamDichVu = yeuCauTiepNhans
                    //.Where(x => x.Nhom == Enums.EnumLoaiTiepNhanNguoiBenh.NguoiBenhChiLamDichVu)
                    .Select(x => x.Id).Distinct()
                    .ToList();

                var phieuKhamTheoTiepNhans = _yeuCauKhamBenhRepository.TableNoTracking
                    .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham
                                && lstYeuCauTiepNhanIdCoPhieuKham.Contains(x.YeuCauTiepNhanId))
                    .Select(item => new BaoCaoTNBenhNhanKhamThongTinDichVuVo()
                    {
                        YeuCauTiepNhanId = item.YeuCauTiepNhanId,
                        TenDichVu = item.TenDichVu
                    })
                    .Distinct()
                    .ToList();

                var dichVuTiepNhans = _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && lstYeuCauTiepNhanIdChiLamDichVu.Contains(x.YeuCauTiepNhanId))
                    .Select(item => new BaoCaoTNBenhNhanKhamThongTinDichVuVo()
                    {
                        YeuCauTiepNhanId = item.YeuCauTiepNhanId,
                        TenDichVu = item.TenDichVu
                    })
                    .Distinct()
                    .ToList();

                foreach (var yeuCauTiepNhan in yeuCauTiepNhans)
                {
                    //if (yeuCauTiepNhan.Nhom == Enums.EnumLoaiTiepNhanNguoiBenh.NguoiBenhCoPhieuKham)
                    //{
                    //    var lstDchVu = phieuKhamTheoTiepNhans.Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhan.Id)
                    //        .Select(x => x.TenDichVu).ToList();
                    //    yeuCauTiepNhan.DichVu = string.Join(", ", lstDchVu);
                    //}
                    //else
                    //{
                    //    var lstDchVu = dichVuTiepNhans.Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhan.Id)
                    //        .Select(x => x.TenDichVu).ToList();
                    //    yeuCauTiepNhan.DichVu = string.Join(", ", lstDchVu);
                    //}

                    //dịch vụ khám 
                    var lstDchVu = phieuKhamTheoTiepNhans.Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhan.Id)
                        .Select(x => x.TenDichVu).ToList();

                    //dịch vụ kỹ thuật
                    lstDchVu.AddRange(dichVuTiepNhans.Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhan.Id)
                        .Select(x => x.TenDichVu).ToList());

                    yeuCauTiepNhan.DichVu = string.Join(", ", lstDchVu);
                }
            }

            return new GridDataSource
            {
                Data = yeuCauTiepNhans.ToArray(),
                TotalRowCount = yeuCauTiepNhans.Count()
            };
        }

        public async Task<GridDataSource> GetTotalBaoCaoTiepNhanBenhNhanKhamForGridAsync(QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new BaoCaoTiepNhanNguoiBenhKhamQueryInfo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoTiepNhanNguoiBenhKhamQueryInfo>(queryInfo.AdditionalSearchString);
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.FromDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.FromDate)) //) || !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.FromDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (timKiemNangCaoObj.ToDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.ToDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.ToDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }

            if (tuNgay != null && denNgay != null)
            {
                var yeuCauTiepNhans = _yeuCauTiepNhanRepository.TableNoTracking
                    .Where(x => x.LoaiYeuCauTiepNhan != Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru
                                && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                                && x.ThoiDiemTiepNhan >= tuNgay
                                && x.ThoiDiemTiepNhan <= denNgay)
                    .Select(item => new BaoCaoTNBenhNhanKhamGridVo
                    {
                        Id = item.Id,
                        BaoCaoTNBenhNhanKhamThongTinYeuCauKhamBenhVos = item.YeuCauKhamBenhs.Select(o => new BaoCaoTNBenhNhanKhamThongTinYeuCauKhamBenhVo { Id = o.Id, TrangThai = o.TrangThai }).ToList(),
                        BaoCaoTNBenhNhanKhamThongTinYeuCauDichVuKyThuatVos = item.YeuCauDichVuKyThuats.Select(o => new BaoCaoTNBenhNhanKhamThongTinYeuCauDichVuKyThuatVo { Id = o.Id, TrangThai = o.TrangThai }).ToList()
                    })
                    .ToList();
                //&& (x.YeuCauKhamBenhs.Any(a => a.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                //    || x.YeuCauDichVuKyThuats.Any(a =>
                //        a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)));

                var countTask = yeuCauTiepNhans
                    .Count(x => x.BaoCaoTNBenhNhanKhamThongTinYeuCauKhamBenhVos.Any(a => a.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham) || x.BaoCaoTNBenhNhanKhamThongTinYeuCauDichVuKyThuatVos.Any(a => a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy));
                return new GridDataSource { TotalRowCount = countTask };
            }
            else
            {
                return new GridDataSource { TotalRowCount = 0 };
            }
        }

        public string InBaoCaoTiepNhanBenhNhanKham(InBaoCaoTNBenhNhanKhamVo inBaoCaoTiepNhanBenhNhanKham)
        {
            var content = string.Empty;
            var template = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("BaoCaoTiepNhanBenhNhanKham")).First();
            var dataBenhNhan = string.Empty;
            var listGridVos = new List<BaoCaoTNBenhNhanKhamGridVo>();
            //var baoCaoTNBenhNhanKhamGridVo1 = new BaoCaoTNBenhNhanKhamGridVo
            //{
            //    Id = 1,
            //    NhomId = 1,
            //    Nhom = "BN có phiếu khám",
            //    ThoiGianTiepNhan = DateTime.Now,
            //    MaTN = "TN00858",
            //    HoTenBN = "Nguyễn Thị Mai",
            //    NgaySinhDisplay = "09/09/2009",
            //    GioiTinhDisplay = "Nữ",
            //    BHTYMaSoThe = "HS9257829467789345",
            //    PhieuKham = "Khám nội",
            //    TenPhongKham = "Phòng khám nội",
            //    TrangThaiDisplay = "Đang thực hiện",
            //    NgoaiGioHanhChinh = "x",
            //    HinhThucDen = "Tự đến",
            //    NoiGioiThieu = ""
            //};
            //var baoCaoTNBenhNhanKhamGridVo2 = new BaoCaoTNBenhNhanKhamGridVo
            //{
            //    Id = 2,
            //    NhomId = 2,
            //    Nhom = "BN chỉ làm DV",
            //    ThoiGianTiepNhan = DateTime.Now,
            //    MaTN = "TN0579747",
            //    HoTenBN = "Trần Mai Anh",
            //    NgaySinhDisplay = "10/10/1996",
            //    GioiTinhDisplay = "Nữ",
            //    BHTYMaSoThe = "x",
            //    PhieuKham = "Xét nghiệm máu, X quang ngực thẳng",
            //    TenPhongKham = "x",
            //    TrangThaiDisplay = "",
            //    NgoaiGioHanhChinh = "",
            //    HinhThucDen = "Tự đến",
            //    NoiGioiThieu = ""
            //};

            //var baoCaoTNBenhNhanKhamGridVo3 = new BaoCaoTNBenhNhanKhamGridVo
            //{
            //    Id = 3,
            //    NhomId = 2,
            //    Nhom = "BN chỉ làm DV",
            //    ThoiGianTiepNhan = DateTime.Now,
            //    MaTN = "TN0577346",
            //    HoTenBN = "Hoàng Tiến Minh",
            //    NgaySinhDisplay = "10/10/1996",
            //    GioiTinhDisplay = "Nam",
            //    BHTYMaSoThe = "x",
            //    PhieuKham = "X quang ngực thẳng, Siêu âm tim",
            //    TenPhongKham = "x",
            //    TrangThaiDisplay = "",
            //    NgoaiGioHanhChinh = "",
            //    HinhThucDen = "Tự đến",
            //    NoiGioiThieu = ""
            //};

            //listGridVos.Add(baoCaoTNBenhNhanKhamGridVo1);
            //listGridVos.Add(baoCaoTNBenhNhanKhamGridVo2);
            //listGridVos.Add(baoCaoTNBenhNhanKhamGridVo3);
            listGridVos = listGridVos.OrderBy(p => p.MaTN).ToList();
            var groupPhieuKham = "BN có phiếu khám";
            var groupDichVu = "BN chỉ làm DV";

            var headerPhieuKham = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                        + "<td style='border: 1px solid #020000;text-align: left;' colspan='13'><b>" + groupPhieuKham.ToUpper()
                                        + "</b></tr>";
            var headerDichVu = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                        + "<td style='border: 1px solid #020000;text-align: left;' colspan='13'><b>" + groupDichVu.ToUpper()
                                        + "</b></tr>";
            var STT = 1;
            if (listGridVos.Any())
            {
                if (listGridVos.Any(p => p.Nhom == Enums.EnumLoaiTiepNhanNguoiBenh.NguoiBenhCoPhieuKham))
                {
                    dataBenhNhan += headerPhieuKham;
                    foreach (var item in listGridVos.Where(p => p.Nhom == Enums.EnumLoaiTiepNhanNguoiBenh.NguoiBenhCoPhieuKham))
                    {
                        dataBenhNhan += "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                                + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + STT
                                                + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.ThoiGianTiepNhanDisplay
                                                + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.MaTN
                                                + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.HoTenBN
                                                + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.NgaySinhDisplay
                                                + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.GioiTinhDisplay
                                                + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.BHTYMaSoThe
                                                + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.DichVu
                                                + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.TenPhongKham
                                                + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.TrangThaiDisplay
                                                + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.NgoaiGioHanhChinh
                                                + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.HinhThucDen
                                                + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.NoiGioiThieu
                                                + "</tr>";
                        STT++;
                    }
                }

                if (listGridVos.Any(p => p.Nhom == Enums.EnumLoaiTiepNhanNguoiBenh.NguoiBenhChiLamDichVu))
                {
                    dataBenhNhan += headerDichVu;
                    foreach (var item in listGridVos.Where(p => p.Nhom == Enums.EnumLoaiTiepNhanNguoiBenh.NguoiBenhChiLamDichVu))
                    {
                        dataBenhNhan += "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                                + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + STT
                                                + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.ThoiGianTiepNhanDisplay
                                                + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.MaTN
                                                + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.HoTenBN
                                                + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.NgaySinhDisplay
                                                + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.GioiTinhDisplay
                                                + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.BHTYMaSoThe
                                                + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.DichVu
                                                + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.TenPhongKham
                                                + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.TrangThaiDisplay
                                                + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.NgoaiGioHanhChinh
                                                + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.HinhThucDen
                                                + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.NoiGioiThieu
                                                + "</tr>";
                        STT++;
                    }
                }
            }
            var data = new InBaoCaoTNBenhNhanKhamData
            {
                LogoUrl = inBaoCaoTiepNhanBenhNhanKham.HostingName + "/assets/img/logo-bacha-full.png",
                ThoiGianBaoCao = inBaoCaoTiepNhanBenhNhanKham.FromDate.Replace("AM", "SA").Replace("PM", "CH") + " đến ngày " + inBaoCaoTiepNhanBenhNhanKham.ToDate.Replace("AM", "SA").Replace("PM", "CH"),
                DataBenhNhan = dataBenhNhan
            };
            content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
            return content;
        }

        public virtual byte[] ExportBaoCaoTiepNhanBenhNhanKham(ICollection<BaoCaoTNBenhNhanKhamGridVo> datas, QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new BaoCaoTiepNhanNguoiBenhKhamQueryInfo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoTiepNhanNguoiBenhKhamQueryInfo>(queryInfo.AdditionalSearchString);
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.FromDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.FromDate)) //) || !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.FromDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (timKiemNangCaoObj.ToDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.ToDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.ToDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }

            int indexSTT = 1;
            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoTNBenhNhanKhamGridVo>("STT", p => indexSTT++)
            };
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO TIẾP NHẬN NGƯỜI BỆNH");

                    // set row
                    worksheet.Row(9).Height = 24.5;
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 20;
                    worksheet.Column(3).Width = 20;
                    worksheet.Column(4).Width = 30;
                    worksheet.Column(5).Width = 15;
                    worksheet.Column(6).Width = 10;
                    worksheet.Column(7).Width = 20;
                    worksheet.Column(8).Width = 40;
                    worksheet.Column(9).Width = 20;
                    worksheet.Column(10).Width = 20;
                    worksheet.Column(11).Width = 30;
                    //worksheet.Column(12).Width = 20;
                    //worksheet.Column(13).Width = 30;
                    worksheet.DefaultColWidth = 7;

                    //SET img 
                    //using (var range = worksheet.Cells["A1:C1"])
                    //{
                    //    var url = hostingName + "/assets/img/logo-bacha-full.png";
                    //    WebClient wc = new WebClient();
                    //    byte[] bytes = wc.DownloadData(url); // download file từ server
                    //    MemoryStream ms = new MemoryStream(bytes); //
                    //    Image img = Image.FromStream(ms); // chuyển đổi thành img
                    //    ExcelPicture pic = range.Worksheet.Drawings.AddPicture("Logo", img);
                    //    pic.SetPosition(0, 0, 0, 0);
                    //    var height = 120; // chiều cao từ A1 đến A6
                    //    var width = 510; // chiều rộng từ A1 đến D1
                    //    pic.SetSize(width, height);
                    //    range.Worksheet.Protection.IsProtected = false;
                    //    range.Worksheet.Protection.AllowSelectLockedCells = false;
                    //}

                    using (var range = worksheet.Cells["A1:E1"])
                    {
                        range.Worksheet.Cells["A1:E1"].Merge = true;
                        range.Worksheet.Cells["A1:E1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A1:E1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:E1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:E1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:E1"].Style.Font.Bold = true;
                    }

                    // SET title head cho bảng excel
                    using (var range = worksheet.Cells["A3:K3"])
                    {
                        range.Worksheet.Cells["A3:K3"].Merge = true;
                        range.Worksheet.Cells["A3:K3"].Value = "BÁO CÁO TIẾP NHẬN NGƯỜI BỆNH";
                        range.Worksheet.Cells["A3:K3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:K3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:K3"].Style.Font.SetFromFont(new Font("Times New Roman", 17));
                        range.Worksheet.Cells["A3:K3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:K3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A4:K4"])
                    {
                        range.Worksheet.Cells["A4:K4"].Merge = true;
                        range.Worksheet.Cells["A4:K4"].Value = "Từ ngày: " + tuNgay?.FormatNgayGioTimKiemTrenBaoCao()
                                                       + " - đến ngày: " + denNgay?.FormatNgayGioTimKiemTrenBaoCao();
                        range.Worksheet.Cells["A4:K4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:K4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:K4"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A4:K4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:K4"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A6:K6"])
                    {
                        range.Worksheet.Cells["A6:K6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A6:K6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A6:K6"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A6:K6"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A6:K6"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A6:K6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A6"].Value = "STT";

                        range.Worksheet.Cells["B6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B6"].Value = "Thời gian tiếp nhận";

                        range.Worksheet.Cells["C6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C6"].Value = "Mã TN";

                        range.Worksheet.Cells["D6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D6"].Value = "Họ tên NB";

                        range.Worksheet.Cells["E6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E6"].Value = "Ngày sinh";

                        range.Worksheet.Cells["F6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F6"].Value = "Giới tính";

                        range.Worksheet.Cells["G6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G6"].Value = "Thẻ BHYT";

                        range.Worksheet.Cells["H6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H6"].Value = "Nội dung";

                        //range.Worksheet.Cells["I6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //range.Worksheet.Cells["I6"].Value = "Phòng khám";

                        //range.Worksheet.Cells["J6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //range.Worksheet.Cells["J6"].Value = "Trạng thái";

                        range.Worksheet.Cells["I6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I6"].Value = "Ngoài giờ hành chính";

                        range.Worksheet.Cells["J6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J6"].Value = "Hình thức đến";

                        range.Worksheet.Cells["K6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K6"].Value = "Nơi giới thiệu";
                    }

                    var manager = new PropertyManager<BaoCaoTNBenhNhanKhamGridVo>(requestProperties);
                    int index = 7; // bắt đầu đổ data từ dòng 7
                    ///////Đổ data vào bảng excel
                    ///
                    if (datas.Any(x => x.Nhom == Enums.EnumLoaiTiepNhanNguoiBenh.NguoiBenhCoPhieuKham))
                    {
                        using (var range = worksheet.Cells["B" + index + ":K" + index])
                        {
                            range.Worksheet.Cells["B" + index + ":K" + index].Merge = true;
                            range.Worksheet.Cells["B" + index + ":K" + index].Value = Enums.EnumLoaiTiepNhanNguoiBenh.NguoiBenhCoPhieuKham.GetDescription();
                            range.Worksheet.Cells["B" + index + ":K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                            range.Worksheet.Cells["B" + index + ":K" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells["B" + index + ":K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells["B" + index + ":K" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["B" + index + ":K" + index].Style.Font.Bold = true;
                        }
                        index++;
                        var queryPhieuKham = datas.Where(x => x.Nhom == Enums.EnumLoaiTiepNhanNguoiBenh.NguoiBenhCoPhieuKham).OrderBy(p => p.ThoiGianTiepNhan).ThenBy(p => p.TenPhongKham).ToList();
                        foreach (var data in queryPhieuKham)
                        {
                            manager.CurrentObject = data;
                            //// format border, font chữ,....
                            worksheet.Cells["A" + index + ":K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                            worksheet.Cells["A" + index + ":K" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            worksheet.Cells["A" + index + ":B" + index].Style.Font.Color.SetColor(Color.Black);
                            worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                            worksheet.Row(index).Height = 20.5;
                            manager.WriteToXlsx(worksheet, index);
                            // Đổ data
                            worksheet.Cells["A" + index + ":K" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            worksheet.Cells["A" + index + ":K" + index].Style.Font.Color.SetColor(Color.Black);
                            worksheet.Cells["A" + index + ":K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                            worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["B" + index].Value = data.ThoiGianTiepNhanDisplay;
                            worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["C" + index].Value = data.MaTN;
                            worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["D" + index].Value = data.HoTenBN;
                            worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["E" + index].Value = data.NgaySinhDisplay;
                            worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["F" + index].Value = data.GioiTinhDisplay;
                            worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["G" + index].Value = data.BHTYMaSoThe;
                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["H" + index].Value = data.DichVu;
                            //worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["I" + index].Value = data.TenPhongKham;
                            //worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["J" + index].Value = data.TrangThaiDisplay;
                            worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["I" + index].Value = data.NgoaiGioHanhChinh ? "X" : string.Empty;
                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["J" + index].Value = data.HinhThucDen;
                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["K" + index].Value = data.NoiGioiThieu;
                            index++;
                        }

                    }

                    if (datas.Any(x => x.Nhom == Enums.EnumLoaiTiepNhanNguoiBenh.NguoiBenhChiLamDichVu))
                    {
                        using (var range = worksheet.Cells["B" + index + ":K" + index])
                        {
                            range.Worksheet.Cells["B" + index + ":K" + index].Merge = true;
                            range.Worksheet.Cells["B" + index + ":K" + index].Value = Enums.EnumLoaiTiepNhanNguoiBenh.NguoiBenhChiLamDichVu.GetDescription();
                            range.Worksheet.Cells["B" + index + ":K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                            range.Worksheet.Cells["B" + index + ":K" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells["B" + index + ":K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells["B" + index + ":K" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["B" + index + ":K" + index].Style.Font.Bold = true;
                        }
                        index++;
                        var queryPhieuKham = datas.Where(x => x.Nhom == Enums.EnumLoaiTiepNhanNguoiBenh.NguoiBenhChiLamDichVu).OrderBy(p => p.ThoiGianTiepNhan).ThenBy(p => p.TenPhongKham).ToList();
                        foreach (var data in queryPhieuKham)
                        {
                            manager.CurrentObject = data;
                            //// format border, font chữ,....
                            worksheet.Cells["A" + index + ":K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                            worksheet.Cells["A" + index + ":K" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            worksheet.Cells["A" + index + ":B" + index].Style.Font.Color.SetColor(Color.Black);
                            worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                            worksheet.Row(index).Height = 20.5;
                            manager.WriteToXlsx(worksheet, index);
                            // Đổ data
                            worksheet.Cells["A" + index + ":K" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            worksheet.Cells["A" + index + ":K" + index].Style.Font.Color.SetColor(Color.Black);
                            worksheet.Cells["A" + index + ":K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                            worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["B" + index].Value = data.ThoiGianTiepNhanDisplay;
                            worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["C" + index].Value = data.MaTN;
                            worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["D" + index].Value = data.HoTenBN;
                            worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["E" + index].Value = data.NgaySinhDisplay;
                            worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["F" + index].Value = data.GioiTinhDisplay;
                            worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["G" + index].Value = data.BHTYMaSoThe;
                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["H" + index].Value = data.DichVu;
                            //worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["I" + index].Value = data.TenPhongKham;
                            //worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            //worksheet.Cells["J" + index].Value = data.TrangThaiDisplay;
                            worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["I" + index].Value = data.NgoaiGioHanhChinh ? "X" : string.Empty;
                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["J" + index].Value = data.HinhThucDen;
                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["K" + index].Value = data.NoiGioiThieu;
                            index++;
                        }
                    }
                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }

        #endregion
        #region Bao cao ton kho

        public async Task<GridDataSource> GetDataBaoCaoTonKhoForGridAsync(BaoCaoTonKhoQueryInfo queryInfo)
        {
            var allDataNhap = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(o =>
                        o.NhapKhoDuocPhams.KhoId == queryInfo.KhoId && o.NgayNhap <= queryInfo.ToDate)
                    .Select(o => new BaoCaoChiTietTonKhoGridVo
                    {
                        Id = o.Id,
                        DuocPhamBenhVienId = o.DuocPhamBenhVienId,
                        Ma = o.DuocPhamBenhViens.Ma,
                        Ten = o.DuocPhamBenhViens.DuocPham.Ten,
                        HamLuong = o.DuocPhamBenhViens.DuocPham.HamLuong,
                        DVT = o.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                        SoLo = o.Solo,
                        HanSuDungDateTime = o.HanSuDung,
                        NgayNhapXuat = o.NgayNhap,
                        LaDuocPhamBHYT = o.LaDuocPhamBHYT,
                        DuocPhamBenhVienPhanNhomId = o.DuocPhamBenhViens.DuocPhamBenhVienPhanNhomId,
                        //Nhom = o.DuocPhamBenhViens.DuocPhamBenhVienPhanNhomId != null ? o.DuocPhamBenhViens.DuocPhamBenhVienPhanNhom.Ten : "Các thuốc khác",
                        SLNhap = o.SoLuongNhap,
                        SLXuat = 0
                    }).ToList();

            var allDataXuat = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking
                .Where(o => o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null &&
                            o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoXuatId == queryInfo.KhoId
                            && ((o.NgayXuat != null && o.NgayXuat <= queryInfo.ToDate) ||
                                (o.NgayXuat == null && o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat <= queryInfo.ToDate)))
                .Select(o => new BaoCaoChiTietTonKhoGridVo
                {
                    Id = o.Id,
                    DuocPhamBenhVienId = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienId,
                    Ma = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.Ma,
                    Ten = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.Ten,
                    HamLuong = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.HamLuong,
                    DVT = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                    SoLo = o.NhapKhoDuocPhamChiTiet.Solo,
                    HanSuDungDateTime = o.NhapKhoDuocPhamChiTiet.HanSuDung,
                    NgayNhapXuat = o.NgayXuat != null
                        ? o.NgayXuat.Value
                        : o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat,
                    LaDuocPhamBHYT = o.NhapKhoDuocPhamChiTiet.LaDuocPhamBHYT,
                    DuocPhamBenhVienPhanNhomId = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPhamBenhVienPhanNhomId,
                    //Nhom = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPhamBenhVienPhanNhomId != null ? o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPhamBenhVienPhanNhom.Ten : "Các thuốc khác",
                    SLNhap = o.SoLuongXuat < 0 ? o.SoLuongXuat * (-1) : 0,
                    SLXuat = o.SoLuongXuat > 0 ? o.SoLuongXuat : 0,
                }).ToList();

            var allDataNhapXuat = allDataNhap.Concat(allDataXuat).OrderBy(o => o.Ten).ThenBy(o => o.HanSuDungDateTime).ToList();

            var allDataGroup = allDataNhapXuat.GroupBy(o => new { o.LaDuocPhamBHYT, o.DuocPhamBenhVienId, o.SoLo, HanSuDung = o.HanSuDungDateTime.Date });
            var duocPhamBenhVienPhanNhoms = _duocPhamBenhVienPhanNhomRepository.TableNoTracking.ToList();
            var dataReturn = new List<BaoCaoTonKhoGridVo>();
            foreach (var xuatNhapDuocPham in allDataGroup)
            {
                var tonDau = xuatNhapDuocPham.Where(o => o.NgayNhapXuat < queryInfo.FromDate)
                    .Select(o => o.SLNhap.GetValueOrDefault() - o.SLXuat.GetValueOrDefault()).DefaultIfEmpty(0).Sum().MathRoundNumber(2);
                var allDataNhapXuatTuNgay = xuatNhapDuocPham.Where(o => o.NgayNhapXuat >= queryInfo.FromDate).ToList();
                var baoCaoTonKhoGridVo = new BaoCaoTonKhoGridVo
                {
                    Ma = xuatNhapDuocPham.First().Ma,
                    Ten = xuatNhapDuocPham.First().Ten,
                    HamLuong = xuatNhapDuocPham.First().HamLuong,
                    DVT = xuatNhapDuocPham.First().DVT,
                    SoLo = xuatNhapDuocPham.Key.SoLo,
                    HanSuDung = xuatNhapDuocPham.Key.HanSuDung.ApplyFormatDate(),
                    TonDau = tonDau,
                    Nhap = allDataNhapXuatTuNgay.Select(o => o.SLNhap.GetValueOrDefault()).DefaultIfEmpty().Sum().MathRoundNumber(2),
                    Xuat = allDataNhapXuatTuNgay.Select(o => o.SLXuat.GetValueOrDefault()).DefaultIfEmpty().Sum().MathRoundNumber(2),
                    Loai = xuatNhapDuocPham.Key.LaDuocPhamBHYT ? "Thuốc BHYT" : "Viện phí",
                    Nhom = duocPhamBenhVienPhanNhoms.FirstOrDefault(o => o.Id == xuatNhapDuocPham.First().DuocPhamBenhVienPhanNhomId)?.Ten ?? "Các thuốc khác"
                    //Nhom = xuatNhapDuocPham.First().Nhom
                };
                if (baoCaoTonKhoGridVo.TonCuoi != null && !baoCaoTonKhoGridVo.TonCuoi.Value.AlmostEqual(0))
                {
                    dataReturn.Add(baoCaoTonKhoGridVo);
                }
            }
            return new GridDataSource { Data = dataReturn.OrderBy(s => s.Loai).ThenBy(s => s.Nhom).ToArray(), TotalRowCount = dataReturn.Count };
        }
        public virtual byte[] ExportBaoCaoTonKho(GridDataSource gridDataSource, BaoCaoTonKhoQueryInfo query)
        {
            var datas = (ICollection<BaoCaoTonKhoGridVo>)gridDataSource.Data;
            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoTonKhoGridVo>("STT", p => ind++)
            };
            var lstLoai = datas.GroupBy(x => new { x.Loai })
                .Select(item => new LoaiGroupVo
                {
                    Loai = item.First().Loai

                }).OrderBy(p => p.Loai).ToList();
            var lstNhom = datas.GroupBy(x => new { x.Loai, x.Nhom })
               .Select(item => new NhomGroupVo
               {
                   Loai = item.First().Loai,
                   Nhom = item.First().Nhom

               }).OrderBy(p => p.Nhom).ToList();
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO XUẤT NHẬP TỒN");

                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 10;
                    worksheet.Column(3).Width = 40;
                    worksheet.Column(4).Width = 10;
                    worksheet.Column(5).Width = 15;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 15;
                    worksheet.Column(8).Width = 15;
                    worksheet.Column(9).Width = 15;
                    worksheet.Column(10).Width = 15;
                    worksheet.Column(11).Width = 15;
                    worksheet.Column(12).Width = 15;
                    worksheet.DefaultColWidth = 7;

                    //SET img 
                    using (var range = worksheet.Cells["A1:L1"])
                    {
                        //                        var url = hostingName + "/assets/img/logo-bacha-full.png";
                        //                        WebClient wc = new WebClient();
                        //                        byte[] bytes = wc.DownloadData(url); // download file từ server
                        //                        MemoryStream ms = new MemoryStream(bytes); //
                        //                        Image img = Image.FromStream(ms); // chuyển đổi thành img
                        //                        ExcelPicture pic = range.Worksheet.Drawings.AddPicture("Logo", img);
                        //                        pic.SetPosition(0, 0, 0, 0);
                        //                        var height = 120; // chiều cao từ A1 đến A6
                        //                        var width = 510; // chiều rộng từ A1 đến D1
                        //                        pic.SetSize(width, height);
                        //                        range.Worksheet.Protection.IsProtected = false;
                        //                        range.Worksheet.Protection.AllowSelectLockedCells = false;
                        range.Worksheet.Cells["A1:G1"].Merge = true;
                        range.Worksheet.Cells["A1:G1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:G1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:G1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:G1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:G1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:G1"].Style.Font.Bold = true;
                    }

                    // SET title head cho bảng excel
                    using (var range = worksheet.Cells["A3:L3"])
                    {
                        range.Worksheet.Cells["A3:L3"].Merge = true;
                        range.Worksheet.Cells["A3:L3"].Value = "BÁO CÁO TỒN KHO";
                        range.Worksheet.Cells["A3:L3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:L3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:L3"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["A3:L3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:L3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A4:L4"])
                    {
                        range.Worksheet.Cells["A4:L4"].Merge = true;
                        range.Worksheet.Cells["A4:L4"].Value = "Từ ngày: " + query.FromDate.FormatNgayGioTimKiemTrenBaoCao()
                                                          + " - đến ngày: " + query.ToDate.FormatNgayGioTimKiemTrenBaoCao();
                        range.Worksheet.Cells["A4:L4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:L4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:L4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A4:L4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:L4"].Style.Font.Bold = true;
                    }

                    var tenKho = string.Empty;
                    if (query.KhoId == 0)
                    {
                        tenKho = "Tất cả";
                    }
                    else
                    {
                        tenKho = _khoRepository.TableNoTracking.Where(p => p.Id == query.KhoId).Select(p => p.Ten).FirstOrDefault();
                    }
                    using (var range = worksheet.Cells["A5:L5"])
                    {
                        range.Worksheet.Cells["A5:L5"].Merge = true;
                        range.Worksheet.Cells["A5:L5"].Value = "Kho: " + tenKho;
                        range.Worksheet.Cells["A5:L5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A5:L5"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A5:L5"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A5:L5"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A5:L5"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A7:L7"])
                    {
                        range.Worksheet.Cells["A7:L7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A7:L7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A7:L7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A7:L7"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A7:L7"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A7:L7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["A7"].Value = "STT";
                        range.Worksheet.Cells["A7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B7"].Value = "Mã dược";
                        range.Worksheet.Cells["B7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C7"].Value = "Tên thuốc, Vật tư, Hoá chất";
                        range.Worksheet.Cells["C7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D7"].Value = "ĐVT";
                        range.Worksheet.Cells["D7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E7"].Value = "Hàm lượng";
                        range.Worksheet.Cells["E7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F7"].Value = "Số lô";
                        range.Worksheet.Cells["F7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G7"].Value = "HSD";
                        range.Worksheet.Cells["G7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H7"].Value = "Tồn đầu";
                        range.Worksheet.Cells["H7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I7"].Value = "Nhập";
                        range.Worksheet.Cells["I7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J7"].Value = "Tổng số";
                        range.Worksheet.Cells["J7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K7"].Value = "Xuất";
                        range.Worksheet.Cells["K7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["L7"].Value = "Tồn cuối";
                        range.Worksheet.Cells["L7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }

                    var manager = new PropertyManager<BaoCaoTonKhoGridVo>(requestProperties);
                    int index = 8; // bắt đầu đổ data từ dòng 13

                    ///////Đổ data vào bảng excel
                    ///
                    var stt = 1;
                    if (lstLoai.Any())
                    {
                        foreach (var loai in lstLoai)
                        {
                            using (var range = worksheet.Cells["A" + index + ":L" + index])
                            {
                                range.Worksheet.Cells["A" + index + ":L" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                range.Worksheet.Cells["A" + index + ":L" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                range.Worksheet.Cells["A" + index + ":L" + index].Style.Font.Color.SetColor(Color.Red);
                                range.Worksheet.Cells["A" + index + ":L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["A" + index + ":L" + index].Style.Font.Bold = true;

                                range.Worksheet.Cells["A" + index + ":L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["A" + index + ":L" + index].Value = loai.Loai;
                                range.Worksheet.Cells["A" + index + ":L" + index].Merge = true;
                            }
                            index++;


                            var listNhomTheoLoai = lstNhom.Where(o => o.Loai == loai.Loai).ToList();
                            if (listNhomTheoLoai.Any())
                            {
                                foreach (var nhom in listNhomTheoLoai)
                                {
                                    using (var range = worksheet.Cells["A" + index + ":L" + index])
                                    {
                                        range.Worksheet.Cells["A" + index + ":L" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                        range.Worksheet.Cells["A" + index + ":L" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                        range.Worksheet.Cells["A" + index + ":L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["A" + index + ":L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["A" + index + ":L" + index].Style.Font.Color.SetColor(Color.Black);
                                        range.Worksheet.Cells["A" + index + ":L" + index].Style.Font.Bold = true;
                                        range.Worksheet.Cells["A" + index + ":L" + index].Style.Font.Italic = true;


                                        range.Worksheet.Cells["A" + index + ":L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["A" + index + ":L" + index].Value = nhom.Nhom;
                                        range.Worksheet.Cells["A" + index + ":L" + index].Merge = true;
                                    }
                                    index++;

                                    var listDuocPhamTheoNhom = datas.Where(o => o.Loai == loai.Loai && o.Nhom == nhom.Nhom).ToList();
                                    if (listDuocPhamTheoNhom.Any())
                                    {
                                        foreach (var duocPham in listDuocPhamTheoNhom)
                                        {
                                            manager.CurrentObject = duocPham;
                                            //// format border, font chữ,....
                                            worksheet.Cells["A" + index + ":L" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                            worksheet.Cells["A" + index + ":L" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            worksheet.Cells["A" + index + ":L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["A" + index + ":L" + index].Style.Font.Color.SetColor(Color.Black);

                                            worksheet.Row(index).Height = 20.5;
                                            manager.WriteToXlsx(worksheet, index);
                                            // Đổ data
                                            worksheet.Cells["A" + index + ":L" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                            worksheet.Cells["A" + index + ":L" + index].Style.Font.Color.SetColor(Color.Black);
                                            worksheet.Cells["A" + index + ":L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["A" + index + ":L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["A" + index + ":L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["A" + index + ":L" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                                            worksheet.Cells["A" + index].Value = stt;
                                            worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["B" + index].Value = duocPham.Ma;
                                            worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["C" + index].Value = duocPham.Ten;
                                            worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["D" + index].Value = duocPham.DVT;
                                            worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["E" + index].Value = duocPham.HamLuong;
                                            worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["F" + index].Value = duocPham.SoLo;
                                            worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["G" + index].Value = duocPham.HanSuDung;
                                            worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["H" + index].Value = duocPham.TonDau;
                                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["I" + index].Value = duocPham.Nhap;
                                            worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["J" + index].Value = duocPham.TongSo;
                                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["K" + index].Value = duocPham.Xuat;
                                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["L" + index].Value = duocPham.TonCuoi;
                                            worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            index++;
                                            stt++;
                                        }
                                    }
                                }
                            }
                        }
                    }


                    worksheet.Cells["A" + index + ":L" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["A" + index + ":L" + index].Style.Font.Italic = true;
                    //value
                    var now = DateTime.Now;
                    worksheet.Cells["I" + index + ":L" + index].Value = "Ngày " + now.Day + " tháng " + now.Month + " năm " + now.Year;
                    worksheet.Cells["I" + index + ":L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["I" + index + ":L" + index].Merge = true;
                    index++;


                    worksheet.Cells["A" + index + ":L" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["A" + index + ":L" + index].Style.Font.Bold = true;
                    //value
                    worksheet.Cells["A" + index + ":D" + index].Value = "Trưởng khoa";
                    worksheet.Cells["A" + index + ":D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A" + index + ":D" + index].Merge = true;

                    worksheet.Cells["E" + index + ":H" + index].Value = "Thủ kho";
                    worksheet.Cells["E" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["E" + index + ":H" + index].Merge = true;

                    worksheet.Cells["I" + index + ":L" + index].Value = "Người lập";
                    worksheet.Cells["I" + index + ":L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["I" + index + ":L" + index].Merge = true;
                    index++;



                    worksheet.Cells["A" + index + ":L" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["A" + index + ":L" + index].Style.Font.Italic = true;
                    //value
                    worksheet.Cells["A" + index + ":D" + index].Value = "(ký, ghi rõ họ tên)";
                    worksheet.Cells["A" + index + ":D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A" + index + ":D" + index].Merge = true;

                    worksheet.Cells["E" + index + ":H" + index].Value = "(ký, ghi rõ họ tên)";
                    worksheet.Cells["E" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["E" + index + ":H" + index].Merge = true;

                    worksheet.Cells["I" + index + ":L" + index].Value = "(ký, ghi rõ họ tên)";
                    worksheet.Cells["I" + index + ":L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["I" + index + ":L" + index].Merge = true;
                    index++;


                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }
        #endregion
    }
}
