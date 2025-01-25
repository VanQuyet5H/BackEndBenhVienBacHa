using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Services.ExportImport.Help;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using Camino.Services.Helpers;
using Camino.Core.Data;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVus;
using Camino.Core.Domain.ValueObject.CauHinh;
using Microsoft.EntityFrameworkCore.Internal;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        private string GetMergeCellTextHeader(DateTime dateTime)
        {
            var dayOfWeek = dateTime.DayOfWeek;
            var thuTrongTuan = string.Empty;
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday:
                    thuTrongTuan = "Thứ hai";
                    break;
                case DayOfWeek.Tuesday:
                    thuTrongTuan = "Thứ ba";
                    break;
                case DayOfWeek.Wednesday:
                    thuTrongTuan = "Thứ tư";
                    break;
                case DayOfWeek.Thursday:
                    thuTrongTuan = "Thứ năm";
                    break;
                case DayOfWeek.Friday:
                    thuTrongTuan = "Thứ sáu";
                    break;
                case DayOfWeek.Saturday:
                    thuTrongTuan = "Thứ bảy";
                    break;
                case DayOfWeek.Sunday:
                    thuTrongTuan = "Chủ nhật";
                    break;
            }

            return $"{thuTrongTuan} ngày {dateTime.ApplyFormatDate()}";
        }

        private decimal? LamTronHienThi(decimal? d)
        {
            return d != null ? Math.Round(d.Value, MidpointRounding.AwayFromZero) : (decimal?)null;
        }

        public async Task<BaoCaoKetQuaKhamChuaBenhKTVo> GetDataBaoCaoKetQuaKhamChuaBenhKTForGrid(BaoCaoKetQuaKhamChuaBenhKTQueryInfoVo queryInfo)
        {
            var cauHinhBaoCao = _cauHinhService.LoadSetting<CauHinhBaoCao>();

            //var tuNgay = queryInfo.TuNgay.Date;
            var tuNgayKhungGio = queryInfo.TuNgay.Date.AddSeconds(cauHinhBaoCao.KhungGioBaoCaoKetQuaKhamChuaBenh).AddMilliseconds(1);

            var tuNgayBaoCao = queryInfo.TuNgay.Date;

            //var denNgay = queryInfo.DenNgay.Date.AddDays(1).AddMilliseconds(-1);
            var denNgayKhungGio = queryInfo.DenNgay.Date.AddDays(1).AddSeconds(cauHinhBaoCao.KhungGioBaoCaoKetQuaKhamChuaBenh);
            var denNgayBaoCao = queryInfo.DenNgay.Date.AddDays(1).AddMilliseconds(-1);

            var khungGioMilliseconds = (tuNgayKhungGio - tuNgayBaoCao).TotalMilliseconds;

            if (tuNgayKhungGio > denNgayKhungGio)
            {
                throw new Exception("Ngày không hợp lệ");
            }
            if ((denNgayBaoCao.Date - tuNgayBaoCao.Date).TotalDays > 7)
            {
                throw new Exception("Báo cáo không thể quá 7 ngày");
            }
            var baoCaoKetQuaKhamChuaBenhKTVo = new BaoCaoKetQuaKhamChuaBenhKTVo();
            List<BaoCaoKetQuaColumnHeaderVo> baoCaoKetQuaColumnHeader = new List<BaoCaoKetQuaColumnHeaderVo>();
            baoCaoKetQuaColumnHeader.Add(new BaoCaoKetQuaColumnHeaderVo
            {
                Index = 0,
                MergeCellText = "Kết quả",
                Cell1Text = "Số dư đầu kỳ tháng",
                Cell2Text = "Số dư đầu kỳ tuần"
            });
            for (int i = 0; tuNgayBaoCao.AddDays(i) < denNgayBaoCao; i++)
            {
                baoCaoKetQuaColumnHeader.Add(new BaoCaoKetQuaColumnHeaderVo
                {
                    Index = i + 1,
                    MergeCellText = GetMergeCellTextHeader(tuNgayBaoCao.AddDays(i)),
                    Cell1Text = "Số người",
                    Cell2Text = "Doanh thu trong ngày"
                });
            }

            var lastIndex = baoCaoKetQuaColumnHeader.Last().Index + 1;
            baoCaoKetQuaColumnHeader.Add(new BaoCaoKetQuaColumnHeaderVo
            {
                Index = lastIndex,
                MergeCellText = "Cộng",
                Cell1Text = "Số người",
                Cell2Text = "Doanh thu"
            });

            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaColumnHeader = baoCaoKetQuaColumnHeader;

            var ngayDauThangTruocBaoCao = new DateTime(tuNgayBaoCao.Month == 1 ? (tuNgayBaoCao.Year - 1) : tuNgayBaoCao.Year, tuNgayBaoCao.Month == 1 ? 12 : (tuNgayBaoCao.Month - 1), 1, 0, 0, 0);
            var ngayCuoiThangTruocBaoCao = new DateTime(ngayDauThangTruocBaoCao.Year, ngayDauThangTruocBaoCao.Month, DateTime.DaysInMonth(ngayDauThangTruocBaoCao.Year, ngayDauThangTruocBaoCao.Month), 0, 0, 0);
            var ngayDauThangTruocKhungGio = ngayDauThangTruocBaoCao.AddMilliseconds(khungGioMilliseconds);
            var ngayCuoiThangTruocKhungGio = ngayCuoiThangTruocBaoCao.AddDays(1).AddMilliseconds(khungGioMilliseconds);

            var ngayDauTuanNayBaoCao = tuNgayBaoCao.AddDays((int)DayOfWeek.Monday - (int)tuNgayBaoCao.DayOfWeek);
            var ngayDauTuanTruocBaoCao = ngayDauTuanNayBaoCao.AddDays(-7);
            var ngayCuoiTuanTruocBaoCao = ngayDauTuanNayBaoCao.AddDays(-1);
            var ngayDauTuanNayKhungGio = ngayDauTuanNayBaoCao.AddMilliseconds(khungGioMilliseconds);
            var ngayDauTuanTruocKhungGio = ngayDauTuanTruocBaoCao.AddMilliseconds(khungGioMilliseconds);
            var ngayCuoiTuanTruocKhungGio = ngayCuoiTuanTruocBaoCao.AddDays(1).AddMilliseconds(khungGioMilliseconds);

            string[] nhomDichVuBenhVienRangHamMatIds = cauHinhBaoCao.NhomDichVuBenhVienRangHamMatIds != null ? cauHinhBaoCao.NhomDichVuBenhVienRangHamMatIds?.Split(";") : new string[0];
            string[] dichVuKhamRangHamMatIds = cauHinhBaoCao.DichVuKhamRangHamMatIds != null ? cauHinhBaoCao.DichVuKhamRangHamMatIds?.Split(";") : new string[0];
            //so nguoi KSK chi tinh tu ngay - den ngay
            var ycKSKs = _yeuCauKhamBenhRepository.TableNoTracking
                .Where(o => o.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham &&
                            (o.GoiKhamSucKhoeId != null || o.GoiKhamSucKhoeDichVuPhatSinhId != null) &&
                            o.ThoiDiemThucHien != null && o.ThoiDiemThucHien >= tuNgayKhungGio && o.ThoiDiemThucHien <= denNgayKhungGio)
                .Select(o => new { o.Id, o.YeuCauTiepNhanId, o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId }).ToList();
            var yctnKSKIds = ycKSKs.Select(o => o.YeuCauTiepNhanId).Distinct().ToList();
            var hopDongKhamSucKhoeNhanVienIds = ycKSKs.Where(o => o.HopDongKhamSucKhoeNhanVienId != null).Select(o => o.HopDongKhamSucKhoeNhanVienId.Value).Distinct().ToList();
            var tiepNhanKSKDataVos = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(o => yctnKSKIds.Contains(o.Id))
                .Select(o => new BaoCaoKetQuaTiepNhanKSKDataVo
                {
                    YeuCauTiepNhanId = o.Id,
                    HopDongKhamSucKhoeNhanVienId = o.HopDongKhamSucKhoeNhanVienId,
                    BaoCaoKetQuaYeuCauKhamKSKDataVos = o.YeuCauKhamBenhs
                        .Where(k => k.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham &&
                                    (k.GoiKhamSucKhoeId != null || k.GoiKhamSucKhoeDichVuPhatSinhId != null) &&
                                    k.ThoiDiemThucHien != null)
                        .Select(k => new BaoCaoKetQuaYeuCauKhamKSKDataVo
                        { Id = k.Id, ThoiDiemThucHien = k.ThoiDiemThucHien }).ToList()
                }).ToList();

            var hopDongKhamSucKhoeNhanVienDatas = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking
                .Where(o => hopDongKhamSucKhoeNhanVienIds.Contains(o.Id))
                .Select(o => new { o.Id, o.HopDongKhamSucKhoeId, o.HopDongKhamSucKhoe.CongTyKhamSucKhoeId }).ToList();
            var congTyKhamSucKhoeIds = hopDongKhamSucKhoeNhanVienDatas.Select(o => o.CongTyKhamSucKhoeId).Distinct().ToList();
            var congTyKhamSucKhoeDatas = _congTyKhamSucKhoeRepository.TableNoTracking
                .Where(o => congTyKhamSucKhoeIds.Contains(o.Id))
                .Select(o => new { o.Id, o.Ten }).ToList();

            foreach (var tiepNhanKSKDataVo in tiepNhanKSKDataVos)
            {
                if (tiepNhanKSKDataVo.HopDongKhamSucKhoeNhanVienId != null)
                {
                    var hopDongKhamSucKhoeNhanVienData = hopDongKhamSucKhoeNhanVienDatas.First(o => o.Id == tiepNhanKSKDataVo.HopDongKhamSucKhoeNhanVienId);
                    tiepNhanKSKDataVo.CongTyKhamSucKhoeId = hopDongKhamSucKhoeNhanVienData.CongTyKhamSucKhoeId;
                    tiepNhanKSKDataVo.CongTyKhamSucKhoe = congTyKhamSucKhoeDatas
                        .First(o => o.Id == hopDongKhamSucKhoeNhanVienData.CongTyKhamSucKhoeId).Ten;
                }
            }

            var tiepNhanKSKDataVoDuocTinhTheoCongtyGroups = tiepNhanKSKDataVos.Where(o =>
                o.ThoiDiemThucHienChuyenKhoaDauTien != null && o.ThoiDiemThucHienChuyenKhoaDauTien >= tuNgayKhungGio &&
                o.ThoiDiemThucHienChuyenKhoaDauTien <= denNgayKhungGio).GroupBy(o => o.CongTyKhamSucKhoeId).ToList();

            List<BaoCaoKetQuaKhamSucKhoeDoan> baoCaoKetQuaKhamSucKhoeDoans = new List<BaoCaoKetQuaKhamSucKhoeDoan>();
            foreach (var tiepNhanKSKDataVoDuocTinhTheoCongty in tiepNhanKSKDataVoDuocTinhTheoCongtyGroups)
            {
                var baoCaoKetQuaKhamSucKhoeDoan = new BaoCaoKetQuaKhamSucKhoeDoan();
                baoCaoKetQuaKhamSucKhoeDoan.BaoCaoKetQuaColumnKhamSucKhoeDoanVos = new List<BaoCaoKetQuaColumnKhamSucKhoeDoanVo>();
                baoCaoKetQuaKhamSucKhoeDoan.BaoCaoKetQuaColumnKhamSucKhoeDoanVos.Add(new BaoCaoKetQuaColumnKhamSucKhoeDoanVo { Index = 0, Cell1Value = null, Cell2Value = null });
                int i = 0;
                while (tuNgayKhungGio.AddDays(i) < denNgayKhungGio)
                {
                    var soNguoiKSK = tiepNhanKSKDataVoDuocTinhTheoCongty
                        .Where(o => o.ThoiDiemThucHienChuyenKhoaDauTien >= tuNgayKhungGio.AddDays(i) &&
                                    o.ThoiDiemThucHienChuyenKhoaDauTien < tuNgayKhungGio.AddDays(i + 1))
                        .GroupBy(o => o.HopDongKhamSucKhoeNhanVienId).Count();
                    baoCaoKetQuaKhamSucKhoeDoan.BaoCaoKetQuaColumnKhamSucKhoeDoanVos.Add(new BaoCaoKetQuaColumnKhamSucKhoeDoanVo { Index = i + 1, Cell1Value = soNguoiKSK, Cell2Value = tiepNhanKSKDataVoDuocTinhTheoCongty.First().CongTyKhamSucKhoe });
                    i++;
                }

                var tongSoNguoiKSKTheoCty = baoCaoKetQuaKhamSucKhoeDoan.BaoCaoKetQuaColumnKhamSucKhoeDoanVos.Sum(o => o.Cell1Value.GetValueOrDefault());
                baoCaoKetQuaKhamSucKhoeDoan.BaoCaoKetQuaColumnKhamSucKhoeDoanVos.Add(new BaoCaoKetQuaColumnKhamSucKhoeDoanVo { Index = i + 1, Cell1Value = tongSoNguoiKSKTheoCty, Cell2Value = tiepNhanKSKDataVoDuocTinhTheoCongty.First().CongTyKhamSucKhoe });
                baoCaoKetQuaKhamSucKhoeDoans.Add(baoCaoKetQuaKhamSucKhoeDoan);
            }
            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaKhamSucKhoeDoans = baoCaoKetQuaKhamSucKhoeDoans;

            List<BaoCaoKetQuaColumnKhamSucKhoeDoanVo> baoCaoKetQuaColumnTongKhamSucKhoeDoan = new List<BaoCaoKetQuaColumnKhamSucKhoeDoanVo>();
            for (int i = 0; i <= lastIndex; i++)
            {
                if (i == 0)
                {
                    baoCaoKetQuaColumnTongKhamSucKhoeDoan.Add(new BaoCaoKetQuaColumnKhamSucKhoeDoanVo { Index = i, Cell1Value = null, Cell2Value = null });
                }
                else
                {
                    var tong = baoCaoKetQuaKhamSucKhoeDoans.Select(o => o.BaoCaoKetQuaColumnKhamSucKhoeDoanVos.FirstOrDefault(x => x.Index == i)?.Cell1Value).DefaultIfEmpty().Sum();
                    baoCaoKetQuaColumnTongKhamSucKhoeDoan.Add(new BaoCaoKetQuaColumnKhamSucKhoeDoanVo { Index = i, Cell1Value = tong, Cell2Value = null });
                }
            }

            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaColumnTongKhamSucKhoeDoan = baoCaoKetQuaColumnTongKhamSucKhoeDoan;

            //doanh thu
            var phieuThuData = _taiKhoanBenhNhanThuRepository.TableNoTracking
                .Where(o => (ngayDauThangTruocKhungGio <= o.NgayThu && o.NgayThu <= denNgayKhungGio) || (o.NgayHuy != null && ngayDauThangTruocKhungGio <= o.NgayHuy && o.NgayHuy <= denNgayKhungGio))
                .Select(o => new BaoCaoKetQuaPhieuThuDataVo
                {
                    Id = o.Id,
                    LoaiThuTienBenhNhan = o.LoaiThuTienBenhNhan,
                    LoaiNoiThu = o.LoaiNoiThu,
                    DaHuy = o.DaHuy,
                    //MaBenhNhan = o.YeuCauTiepNhan.BenhNhan.MaBN,
                    NgayThu = o.NgayThu,
                    NgayHuy = o.NgayHuy,//new
                    YeuCauTiepNhanId = o.YeuCauTiepNhanId,//new
                    MaYeuCauTiepNhan = o.YeuCauTiepNhan.MaYeuCauTiepNhan,//new
                    LoaiYeuCauTiepNhan = o.YeuCauTiepNhan.LoaiYeuCauTiepNhan,
                    QuyetToanTheoNoiTru = o.YeuCauTiepNhan.QuyetToanTheoNoiTru,
                    YeuCauTiepNhanNgoaiTruCanQuyetToanId = o.YeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId,
                    //TenBenhNhan = o.YeuCauTiepNhan.HoTen,
                    //NamSinh = o.YeuCauTiepNhan.NamSinh != null ? o.YeuCauTiepNhan.NamSinh.ToString() : string.Empty,
                    //NguoiGioiThieu = o.YeuCauTiepNhan.NoiGioiThieu != null ? o.YeuCauTiepNhan.NoiGioiThieu.Ten : string.Empty,
                    //SoBenhAn = o.YeuCauTiepNhan.NoiTruBenhAn != null ? o.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn : string.Empty,
                    //BenhAnSoSinh = o.YeuCauTiepNhan.YeuCauNhapVien != null ? (o.YeuCauTiepNhan.YeuCauNhapVien.YeuCauTiepNhanMeId != null) : false,
                    GoiDichVu = o.ThuTienGoiDichVu != null && o.ThuTienGoiDichVu == true,
                    //SoBLHD = o.SoPhieuHienThi,
                    //TongChiPhiBNTT = o.TaiKhoanBenhNhanChis.Where(chi => chi.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi).Select(chi => chi.TienChiPhi.GetValueOrDefault()).DefaultIfEmpty().Sum(),
                    CongNoTuNhan = o.CongTyBaoHiemTuNhanCongNos.Select(cn => cn.SoTien).DefaultIfEmpty().Sum(),
                    CongNoCaNhan = o.CongNo.GetValueOrDefault(),
                    SoTienThuTienMat = o.TienMat.GetValueOrDefault(),
                    SoTienThuChuyenKhoan = o.ChuyenKhoan.GetValueOrDefault(),
                    SoTienThuPos = o.POS.GetValueOrDefault(),
                    SoTienThuTamUng = o.TamUng.GetValueOrDefault(),
                    //SoPhieuThuGhiNo = o.ThuNoPhieuThu != null ? o.ThuNoPhieuThu.SoPhieuHienThi : string.Empty,
                    //NguoiThu = o.NhanVienThucHien.User.HoTen,
                    //ChiTietCongNoTuNhans = o.CongTyBaoHiemTuNhanCongNos.Select(cntn => cntn.CongTyBaoHiemTuNhan.Ten).ToList(),
                    BaoCaoKetQuaPhieuChiDataVos = o.TaiKhoanBenhNhanChis.Select(c => new BaoCaoKetQuaPhieuChiDataVo
                    {
                        Id = c.Id,
                        LoaiChiTienBenhNhan = c.LoaiChiTienBenhNhan,
                        TienChiPhi = c.TienChiPhi,
                        TienMat = c.TienMat,
                        YeuCauKhamBenhId = c.YeuCauKhamBenhId,
                        DichVuKhamBenhBenhVienId = c.YeuCauKhamBenhId != null ? c.YeuCauKhamBenh.DichVuKhamBenhBenhVienId : 0,
                        YeuCauDichVuKyThuatId = c.YeuCauDichVuKyThuatId,
                        NhomDichVuBenhVienId = c.YeuCauDichVuKyThuatId != null ? c.YeuCauDichVuKyThuat.NhomDichVuBenhVienId : 0,
                        YeuCauDuocPhamBenhVienId = c.YeuCauDuocPhamBenhVienId,
                        YeuCauVatTuBenhVienId = c.YeuCauVatTuBenhVienId,
                        YeuCauDichVuGiuongBenhVienId = c.YeuCauDichVuGiuongBenhVienId,
                        YeuCauGoiDichVuId = c.YeuCauGoiDichVuId,
                        DonThuocThanhToanChiTietId = c.DonThuocThanhToanChiTietId,
                        DonVTYTThanhToanChiTietId = c.DonVTYTThanhToanChiTietId,
                        YeuCauDichVuGiuongBenhVienChiPhiBenhVienId = c.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId,
                        YeuCauTruyenMauId = c.YeuCauTruyenMauId,
                        Gia = c.Gia,
                        SoLuong = c.SoLuong,
                        DonGiaBaoHiem = c.DonGiaBaoHiem,
                        MucHuongBaoHiem = c.MucHuongBaoHiem,
                        TiLeBaoHiemThanhToan = c.TiLeBaoHiemThanhToan,
                        SoTienBaoHiemTuNhanChiTra = c.SoTienBaoHiemTuNhanChiTra,
                        SoTienMienGiam = c.SoTienMienGiam,
                        DaHuy = c.DaHuy,
                    }).ToList()
                }).ToList();

            var phieuChiData = _taiKhoanBenhNhanChiRepository.TableNoTracking.Where(o =>
            (o.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanUng || o.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanThu) &&
            ((ngayDauThangTruocKhungGio <= o.NgayChi && o.NgayChi <= denNgayKhungGio) || (o.NgayHuy != null && ngayDauThangTruocKhungGio <= o.NgayHuy && o.NgayHuy <= denNgayKhungGio)))
                .Select(o => new BaoCaoKetQuaPhieuThuDataVo
                {
                    Id = o.Id,
                    LoaiChiTienBenhNhan = o.LoaiChiTienBenhNhan,
                    DaHuy = o.DaHuy,
                    NgayThu = o.NgayChi,
                    NgayHuy = o.NgayHuy,//new
                    YeuCauTiepNhanId = o.YeuCauTiepNhanId.GetValueOrDefault(),//new
                    MaYeuCauTiepNhan = o.YeuCauTiepNhan != null ? o.YeuCauTiepNhan.MaYeuCauTiepNhan : "",//new
                    LoaiYeuCauTiepNhan = o.YeuCauTiepNhan != null ? o.YeuCauTiepNhan.LoaiYeuCauTiepNhan : EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru,
                    QuyetToanTheoNoiTru = o.YeuCauTiepNhan != null ? o.YeuCauTiepNhan.QuyetToanTheoNoiTru : false,
                    YeuCauTiepNhanNgoaiTruCanQuyetToanId = o.YeuCauTiepNhan != null ? o.YeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId : null,
                    //MaBenhNhan = o.YeuCauTiepNhanId.BenhNhan.MaBN,
                    //NgayThu = o.NgayChi,
                    //TenBenhNhan = o.YeuCauTiepNhan.HoTen,
                    //NamSinh = o.YeuCauTiepNhan.NamSinh != null ? o.YeuCauTiepNhan.NamSinh.ToString() : string.Empty,
                    //NguoiGioiThieu = o.YeuCauTiepNhan.NoiGioiThieu != null ? o.YeuCauTiepNhan.NoiGioiThieu.Ten : string.Empty,
                    //MaYTe = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    //SoBenhAn = o.YeuCauTiepNhan.NoiTruBenhAn != null ? o.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn : string.Empty,
                    //BenhAnSoSinh = o.YeuCauTiepNhan.YeuCauNhapVien != null ? (o.YeuCauTiepNhan.YeuCauNhapVien.YeuCauTiepNhanMeId != null) : false,
                    GoiDichVu = o.YeuCauGoiDichVuId != null,
                    SoBLHD = o.SoPhieuHienThi,
                    SoTienThuTienMat = o.TienMat.GetValueOrDefault(),
                    //NguoiThu = o.NhanVienThucHien != null ? o.NhanVienThucHien.User.HoTen : string.Empty,
                    BaoCaoKetQuaPhieuChiDataVos = o.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanThu ?
                        new List<BaoCaoKetQuaPhieuChiDataVo>
                        { new BaoCaoKetQuaPhieuChiDataVo
                            {
                                Id = o.Id,
                                LoaiChiTienBenhNhan = o.LoaiChiTienBenhNhan,
                                TienChiPhi = o.TienChiPhi,
                                TienMat = o.TienMat,
                                YeuCauKhamBenhId = o.YeuCauKhamBenhId,
                                DichVuKhamBenhBenhVienId = o.YeuCauKhamBenhId != null ? o.YeuCauKhamBenh.DichVuKhamBenhBenhVienId : 0,
                                YeuCauDichVuKyThuatId = o.YeuCauDichVuKyThuatId,
                                NhomDichVuBenhVienId = o.YeuCauDichVuKyThuatId != null ? o.YeuCauDichVuKyThuat.NhomDichVuBenhVienId : 0,
                                YeuCauDuocPhamBenhVienId = o.YeuCauDuocPhamBenhVienId,
                                YeuCauVatTuBenhVienId = o.YeuCauVatTuBenhVienId,
                                YeuCauDichVuGiuongBenhVienId = o.YeuCauDichVuGiuongBenhVienId,
                                YeuCauGoiDichVuId = o.YeuCauGoiDichVuId,
                                DonThuocThanhToanChiTietId = o.DonThuocThanhToanChiTietId,
                                DonVTYTThanhToanChiTietId = o.DonVTYTThanhToanChiTietId,
                                YeuCauDichVuGiuongBenhVienChiPhiBenhVienId = o.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId,
                                YeuCauTruyenMauId = o.YeuCauTruyenMauId,
                                Gia = o.Gia,
                                SoLuong = o.SoLuong,
                                DonGiaBaoHiem = o.DonGiaBaoHiem,
                                MucHuongBaoHiem = o.MucHuongBaoHiem,
                                TiLeBaoHiemThanhToan = o.TiLeBaoHiemThanhToan,
                                SoTienBaoHiemTuNhanChiTra = o.SoTienBaoHiemTuNhanChiTra,
                                SoTienMienGiam = o.SoTienMienGiam,
                                DaHuy = o.DaHuy,
                            }
                        } :
                        new List<BaoCaoKetQuaPhieuChiDataVo>()
                }).ToList();


            //thai san
            var yeuCauGoiDichVuIds = phieuThuData.Where(o => o.GoiDichVu)
                .SelectMany(o => o.BaoCaoKetQuaPhieuChiDataVos.Where(c => c.LoaiChiTienBenhNhan == LoaiChiTienBenhNhan.HoanUng && c.YeuCauGoiDichVuId != null))
                .Select(c => c.YeuCauGoiDichVuId.GetValueOrDefault()).ToList();

            var yeuCauGoiDichVuData = _yeuCauGoiDichVuRepository.TableNoTracking
                .Where(o => yeuCauGoiDichVuIds.Contains(o.Id))
                .Select(o => new
                {
                    o.Id,
                    o.ChuongTrinhGoiDichVuId,
                    o.ChuongTrinhGoiDichVu.LoaiGoiDichVuId
                }).ToList();

            var dsMaYeuCauTiepNhanDungGoiThaiSans = new List<string>();
            foreach (var phieuThu in phieuThuData.Where(o => o.GoiDichVu))
            {
                var yeuCauGoiDichVuIdPhieuThus = phieuThu.BaoCaoKetQuaPhieuChiDataVos
                    .Where(c => c.LoaiChiTienBenhNhan == LoaiChiTienBenhNhan.HoanUng && c.YeuCauGoiDichVuId != null)
                    .Select(c => c.YeuCauGoiDichVuId.GetValueOrDefault()).ToList();
                if (yeuCauGoiDichVuData.Any(o => yeuCauGoiDichVuIdPhieuThus.Contains(o.Id) &&
                                                 o.LoaiGoiDichVuId == (long)EnumLoaiGoiDichVuMarketing.GoiSanPhu))
                {
                    dsMaYeuCauTiepNhanDungGoiThaiSans.Add(phieuThu.MaYeuCauTiepNhan);
                }
            }

            var yeuCauTiepNhanNoiTruIds = phieuThuData.Where(o => o.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru)
                .Select(c => c.YeuCauTiepNhanId).ToList();
            //So sinh
            var yeuCauTiepNhanSoSinhIds = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(o => yeuCauTiepNhanNoiTruIds.Contains(o.Id) && o.YeuCauNhapVien != null && o.YeuCauNhapVien.YeuCauTiepNhanMeId != null)
                .Select(o => o.Id).ToList();

            //theo hinh thuc den
            var yeuCauTiepNhanNgoaiTruTheoHinhThucDenIds = phieuThuData.Where(o => o.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru && !dsMaYeuCauTiepNhanDungGoiThaiSans.Contains(o.MaYeuCauTiepNhan))
                .Select(c => c.YeuCauTiepNhanId).ToList();
            var yeuCauTiepNhanNoiTruTheoHinhThucDenIds = phieuThuData.Where(o => o.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && !yeuCauTiepNhanSoSinhIds.Contains(o.YeuCauTiepNhanId)
                                                                                 && !dsMaYeuCauTiepNhanDungGoiThaiSans.Contains(o.MaYeuCauTiepNhan))
                .Select(c => new { c.YeuCauTiepNhanId, c.YeuCauTiepNhanNgoaiTruCanQuyetToanId }).ToList();

            var yeuCauTiepNhanNgoaiTruTheoHinhThucDenChiIds = phieuChiData.Where(o => o.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru && !dsMaYeuCauTiepNhanDungGoiThaiSans.Contains(o.MaYeuCauTiepNhan))
                .Select(c => c.YeuCauTiepNhanId).ToList();
            var yeuCauTiepNhanNoiTruTheoHinhThucDenChiIds = phieuChiData.Where(o => o.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && !yeuCauTiepNhanSoSinhIds.Contains(o.YeuCauTiepNhanId)
                                                                                 && !dsMaYeuCauTiepNhanDungGoiThaiSans.Contains(o.MaYeuCauTiepNhan))
                .Select(c => new { c.YeuCauTiepNhanId, c.YeuCauTiepNhanNgoaiTruCanQuyetToanId }).ToList();

            var yeuCauTiepNhanIds = yeuCauTiepNhanNgoaiTruTheoHinhThucDenIds
                .Concat(yeuCauTiepNhanNoiTruTheoHinhThucDenIds.Select(o => o.YeuCauTiepNhanId))
                .Concat(yeuCauTiepNhanNoiTruTheoHinhThucDenIds.Select(o => o.YeuCauTiepNhanNgoaiTruCanQuyetToanId.GetValueOrDefault()))
                .Concat(yeuCauTiepNhanNgoaiTruTheoHinhThucDenChiIds)
                .Concat(yeuCauTiepNhanNoiTruTheoHinhThucDenChiIds.Select(o => o.YeuCauTiepNhanId))
                .Concat(yeuCauTiepNhanNoiTruTheoHinhThucDenChiIds.Select(o => o.YeuCauTiepNhanNgoaiTruCanQuyetToanId.GetValueOrDefault()))
                .Distinct().ToList();

            var yeuCauTiepNhanData = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(o => yeuCauTiepNhanIds.Contains(o.Id))
                .Select(o => new BaoCaoKetQuaYeuCauTiepNhanDataVo
                {
                    Id = o.Id,
                    MaYeuCauTiepNhan = o.MaYeuCauTiepNhan,
                    LoaiYeuCauTiepNhan = o.LoaiYeuCauTiepNhan,
                    BenhNhanId = o.BenhNhanId,
                    HinhThucDenId = o.HinhThucDenId,
                    NoiGioiThieuId = o.NoiGioiThieuId,
                    ThoiDiemTiepNhan = o.ThoiDiemTiepNhan,
                    NgaySinh = o.NgaySinh,
                    ThangSinh = o.ThangSinh,
                    NamSinh = o.NamSinh,
                    QuyetToanTheoNoiTru = o.QuyetToanTheoNoiTru
                }).ToList();

            var benhNhanIds = yeuCauTiepNhanData.Select(o => o.BenhNhanId.GetValueOrDefault()).Distinct().ToList();

            var benhNhanDuocGioiThieuData = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(o => o.BenhNhanId != null && benhNhanIds.Contains(o.BenhNhanId.Value) && o.NoiGioiThieuId != null)
                .Select(o => new
                {
                    o.Id,
                    o.MaYeuCauTiepNhan,
                    o.LoaiYeuCauTiepNhan,
                    o.BenhNhanId,
                    o.NoiGioiThieuId,
                }).ToList();
            //cap nhat noi gioi thieu
            foreach (var yeuCauTiepNhan in yeuCauTiepNhanData.Where(o => o.NoiGioiThieuId == null))
            {
                var yctnDuocGioiThieuTruocs = benhNhanDuocGioiThieuData
                    .Where(o => o.BenhNhanId == yeuCauTiepNhan.BenhNhanId && o.Id < yeuCauTiepNhan.Id).ToList();
                if (yctnDuocGioiThieuTruocs.Any())
                {
                    yeuCauTiepNhan.HinhThucDenId = cauHinhBaoCao.HinhThucDenGioiThieuId;
                    yeuCauTiepNhan.NoiGioiThieuId = yctnDuocGioiThieuTruocs.OrderBy(o => o.Id).Last().NoiGioiThieuId;
                }
            }
            var noiGioiThieus = _noiGioiThieuRepository.TableNoTracking.Include(o => o.DonViMau).ToList();


            var yeuCauTiepNhanNhiNgoaiTruIds = yeuCauTiepNhanData
                .Where(o => o.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru && o.QuyetToanTheoNoiTru != true && o.BenhNhanNhi)
                .Select(o => o.Id).ToList();

            var yeuCauTiepNhanNhiNgoaiTruData = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(o => yeuCauTiepNhanNhiNgoaiTruIds.Contains(o.Id))
                .Select(o => new BaoCaoKetQuaYeuCauTiepNhanNhiNgoaiTruVo
                {
                    Id = o.Id,
                    CountYeuCauKhamBenh = o.YeuCauKhamBenhs.Where(k => k.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham).Count(),
                    //CountYeuCauDichVuKyThuatKhamSangLocTiemChung = o.YeuCauDichVuKyThuats.Where(k => k.YeuCauDichVuKyThuatKhamSangLocTiemChungId != null && k.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).Count(),
                    BaoCaoKetQuaYeuCauTiepNhanNhiDVKTNgoaiTruVos = o.YeuCauDichVuKyThuats
                        .Where(k => k.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                        .Select(k => new BaoCaoKetQuaYeuCauTiepNhanNhiDVKTNgoaiTruVo
                        {
                            Id = k.Id,
                            KhamSangLocTiemChungId = k.KhamSangLocTiemChung != null ? k.KhamSangLocTiemChung.Id : (long?)null,
                            TiemChungId = k.TiemChung != null ? k.TiemChung.Id : (long?)null
                        }).ToList()
                    //CountYeuCauDichVuKyNgoaiTiemChung = o.YeuCauDichVuKyThuats.Where(k => k.YeuCauDichVuKyThuatKhamSangLocTiemChungId == null && k.TiemChung == null && k.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).Count()
                }).ToList();

            var baoCaoKetQuaDoanhThuTheoNgays = new List<BaoCaoKetQuaDoanhThuTheoNgay>();
            var baoCaoKetQuaNhaThuocTheoNgays = new List<BaoCaoKetQuaNhaThuocTheoNgay>();
            var baoCaoKetQuaThucThuTheoNgays = new List<BaoCaoKetQuaThucThuTheoNgay>();

            for (int i = 0; ngayDauThangTruocBaoCao.AddDays(i) < denNgayBaoCao; i++)
            {
                baoCaoKetQuaDoanhThuTheoNgays.Add(new BaoCaoKetQuaDoanhThuTheoNgay
                {
                    Ngay = ngayDauThangTruocBaoCao.AddDays(i).Date
                });
                baoCaoKetQuaNhaThuocTheoNgays.Add(new BaoCaoKetQuaNhaThuocTheoNgay
                {
                    Ngay = ngayDauThangTruocBaoCao.AddDays(i).Date
                });
                baoCaoKetQuaThucThuTheoNgays.Add(new BaoCaoKetQuaThucThuTheoNgay
                {
                    Ngay = ngayDauThangTruocBaoCao.AddDays(i).Date
                });
            }
            var phieuThuChiPhiTaiThuNganData = phieuThuData.Where(o => o.LoaiNoiThu == LoaiNoiThu.ThuNgan && o.LoaiThuTienBenhNhan == LoaiThuTienBenhNhan.ThuTheoChiPhi);
            foreach (var phieuThu in phieuThuChiPhiTaiThuNganData)
            {
                var ngayThuBaoCao = phieuThu.NgayThu.AddMilliseconds(khungGioMilliseconds * (-1));
                var baoCaoKetQuaDoanhThuTheoNgay = baoCaoKetQuaDoanhThuTheoNgays.FirstOrDefault(o => o.Ngay == ngayThuBaoCao.Date);
                if (baoCaoKetQuaDoanhThuTheoNgay != null)
                {
                    decimal doanhThuCong = phieuThu.BaoCaoKetQuaPhieuChiDataVos
                        .Where(o => o.LoaiChiTienBenhNhan == LoaiChiTienBenhNhan.ThanhToanChiPhi)
                        .Select(o => o.TienChiPhi.GetValueOrDefault() + o.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault()).DefaultIfEmpty().Sum();
                    decimal doanhThuCong1 = phieuThu.SoTienThuTienMat.GetValueOrDefault() + phieuThu.SoTienThuChuyenKhoan.GetValueOrDefault() +
                    phieuThu.SoTienThuPos.GetValueOrDefault() + phieuThu.SoTienThuTamUng.GetValueOrDefault() + phieuThu.CongNoTuNhan.GetValueOrDefault() + phieuThu.CongNoCaNhan.GetValueOrDefault();
                    if (dsMaYeuCauTiepNhanDungGoiThaiSans.Contains(phieuThu.MaYeuCauTiepNhan))
                    {
                        if (phieuThu.GoiDichVu)
                        {
                            baoCaoKetQuaDoanhThuTheoNgay.DoanhThuThaiSanTrongGoi += doanhThuCong;
                        }
                        else
                        {
                            baoCaoKetQuaDoanhThuTheoNgay.DoanhThuThaiSanNgoaiGoi += doanhThuCong;
                        }
                    }
                    else if (phieuThu.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamSucKhoe)
                    {
                        baoCaoKetQuaDoanhThuTheoNgay.DoanhThuPhatSinhNgoaiGoiKSKDoan += doanhThuCong;
                    }
                    else if (yeuCauTiepNhanSoSinhIds.Contains(phieuThu.YeuCauTiepNhanId))
                    {
                        baoCaoKetQuaDoanhThuTheoNgay.DoanhThuSoSinh += doanhThuCong;
                    }
                    else
                    {
                        var yeuCauTiepNhanDataChiTiet = yeuCauTiepNhanData.FirstOrDefault(o => o.Id == phieuThu.YeuCauTiepNhanId);
                        if (yeuCauTiepNhanDataChiTiet != null)
                        {
                            if (yeuCauTiepNhanDataChiTiet.HinhThucDenId == cauHinhBaoCao.HinhThucDenCBNVId)
                            {
                                baoCaoKetQuaDoanhThuTheoNgay.DoanhThuCBNV += doanhThuCong;
                            }
                            else if (yeuCauTiepNhanDataChiTiet.HinhThucDenId == cauHinhBaoCao.HinhThucDenGioiThieuId)
                            {
                                var tenDonVi = string.Empty;
                                if (yeuCauTiepNhanDataChiTiet.NoiGioiThieuId != null)
                                {
                                    tenDonVi = noiGioiThieus.FirstOrDefault(o => o.Id == yeuCauTiepNhanDataChiTiet.NoiGioiThieuId.Value)?.DonViMau?.Ten ?? string.Empty;
                                }
                                if (tenDonVi.ToLower().Contains(cauHinhBaoCao.NoiGioiThieuThamMy))
                                {
                                    if (tenDonVi.ToLower().Contains(cauHinhBaoCao.NoiGioiThieuThamMyBsTu))
                                    {
                                        //Chỉ tính doanh thu từ các dịch vụ kỹ thuật trừ suất ăn (tính cả ngoại trú và nội trú), Doanh thu tính theo công thức: Thành tiền BV - Miễn giảm nếu có
                                        var doanhThuBsTu = phieuThu.BaoCaoKetQuaPhieuChiDataVos?
                                            .Where(o => o.YeuCauDichVuKyThuatId != null && o.NhomDichVuBenhVienId != cauHinhBaoCao.NhomDichVuBenhVienSuatAnId)
                                            .Select(o => o.Gia.GetValueOrDefault() * (decimal)o.SoLuong.GetValueOrDefault() - o.SoTienMienGiam.GetValueOrDefault()).DefaultIfEmpty().Sum();
                                        baoCaoKetQuaDoanhThuTheoNgay.DoanhThuThamMyBSTu += doanhThuBsTu.GetValueOrDefault();
                                    }
                                    else
                                    {
                                        //Doanh thu tính theo công thức: Thành tiền BV - Miễn giảm nếu có
                                        var doanhThuThamMy = phieuThu.BaoCaoKetQuaPhieuChiDataVos?.Select(o => o.Gia.GetValueOrDefault() * (decimal)o.SoLuong.GetValueOrDefault() - o.SoTienMienGiam.GetValueOrDefault()).DefaultIfEmpty().Sum();
                                        baoCaoKetQuaDoanhThuTheoNgay.DoanhThuThamMyKhac += doanhThuThamMy.GetValueOrDefault();
                                    }
                                }
                                else if (tenDonVi.ToLower().Contains(cauHinhBaoCao.NoiGioiThieuVietDuc))
                                {
                                    baoCaoKetQuaDoanhThuTheoNgay.DoanhThuVietDuc += doanhThuCong;
                                }
                                else
                                {
                                    baoCaoKetQuaDoanhThuTheoNgay.DoanhThuCTV += doanhThuCong;
                                }
                            }
                            //tu den
                            else
                            {
                                if (yeuCauTiepNhanDataChiTiet.BenhNhanNhi)
                                {
                                    if (yeuCauTiepNhanDataChiTiet.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru || yeuCauTiepNhanDataChiTiet.QuyetToanTheoNoiTru == true)
                                    {
                                        baoCaoKetQuaDoanhThuTheoNgay.DoanhThuBenhNhanTuDenNhiNoiTRu += doanhThuCong;
                                    }
                                    else
                                    {
                                        var yeuCauTiepNhanNhiNgoaiTru = yeuCauTiepNhanNhiNgoaiTruData.FirstOrDefault(o => o.Id == yeuCauTiepNhanDataChiTiet.Id);
                                        if (yeuCauTiepNhanNhiNgoaiTru != null)
                                        {
                                            if (yeuCauTiepNhanNhiNgoaiTru.CountYeuCauDichVuKyThuatKhamSangLocTiemChung > 0)
                                            {
                                                if (yeuCauTiepNhanNhiNgoaiTru.CountYeuCauKhamBenh == 0 && yeuCauTiepNhanNhiNgoaiTru.CountYeuCauDichVuKyNgoaiTiemChung == 0)
                                                {
                                                    baoCaoKetQuaDoanhThuTheoNgay.DoanhThuBenhNhanTuDenNhiTiemChung += doanhThuCong;
                                                }
                                                else
                                                {
                                                    baoCaoKetQuaDoanhThuTheoNgay.DoanhThuBenhNhanTuDenNhiKcbTiemChung += doanhThuCong;
                                                }
                                            }
                                            else
                                            {
                                                baoCaoKetQuaDoanhThuTheoNgay.DoanhThuBenhNhanTuDenNhiKcb += doanhThuCong;
                                            }
                                        }
                                        else
                                        {
                                            baoCaoKetQuaDoanhThuTheoNgay.DoanhThuBenhNhanTuDenNhiKcb += doanhThuCong;
                                        }
                                    }
                                }
                                else
                                {
                                    if (yeuCauTiepNhanDataChiTiet.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru || yeuCauTiepNhanDataChiTiet.QuyetToanTheoNoiTru == true)
                                    {
                                        baoCaoKetQuaDoanhThuTheoNgay.DoanhThuBenhNhanTuDenNoiTru += doanhThuCong;
                                    }
                                    else
                                    {
                                        baoCaoKetQuaDoanhThuTheoNgay.DoanhThuBenhNhanTuDenKhachLe += doanhThuCong;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (phieuThu.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru || phieuThu.QuyetToanTheoNoiTru == true)
                            {
                                baoCaoKetQuaDoanhThuTheoNgay.DoanhThuBenhNhanTuDenNoiTru += doanhThuCong;
                            }
                            else
                            {
                                baoCaoKetQuaDoanhThuTheoNgay.DoanhThuBenhNhanTuDenKhachLe += doanhThuCong;
                            }
                        }
                    }
                    //doanh thu RHM
                    var doanhThuRHM = phieuThu.BaoCaoKetQuaPhieuChiDataVos?
                        .Where(o => o.LoaiChiTienBenhNhan == LoaiChiTienBenhNhan.ThanhToanChiPhi &&
                                    ((o.YeuCauKhamBenhId != null && o.DichVuKhamBenhBenhVienId != null && dichVuKhamRangHamMatIds.Contains(o.DichVuKhamBenhBenhVienId.ToString())) ||
                                     (o.YeuCauDichVuKyThuatId != null && o.NhomDichVuBenhVienId != null && nhomDichVuBenhVienRangHamMatIds.Contains(o.NhomDichVuBenhVienId.ToString()))))
                        .Select(o => o.TienChiPhi.GetValueOrDefault() + o.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault()).DefaultIfEmpty().Sum();

                    baoCaoKetQuaDoanhThuTheoNgay.DoanhThuRangHamMat += doanhThuRHM.GetValueOrDefault();
                }
                //Doanh Thu am
                if (phieuThu.DaHuy == true && phieuThu.NgayHuy != null)
                {
                    var ngayHuyBaoCao = phieuThu.NgayHuy.Value.AddMilliseconds(khungGioMilliseconds * (-1));
                    var baoCaoKetQuaDoanhThuTheoNgayHuy = baoCaoKetQuaDoanhThuTheoNgays.FirstOrDefault(o => o.Ngay == ngayHuyBaoCao.Date);
                    if (baoCaoKetQuaDoanhThuTheoNgayHuy != null)
                    {
                        decimal doanhThuTru = phieuThu.BaoCaoKetQuaPhieuChiDataVos
                            .Where(o => o.LoaiChiTienBenhNhan == LoaiChiTienBenhNhan.ThanhToanChiPhi)
                            .Select(o => o.TienChiPhi.GetValueOrDefault() + o.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault()).DefaultIfEmpty().Sum();
                        decimal doanhThuTru1 = phieuThu.SoTienThuTienMat.GetValueOrDefault() + phieuThu.SoTienThuChuyenKhoan.GetValueOrDefault() +
                            phieuThu.SoTienThuPos.GetValueOrDefault() + phieuThu.SoTienThuTamUng.GetValueOrDefault() + phieuThu.CongNoTuNhan.GetValueOrDefault() + phieuThu.CongNoCaNhan.GetValueOrDefault();
                        if (dsMaYeuCauTiepNhanDungGoiThaiSans.Contains(phieuThu.MaYeuCauTiepNhan))
                        {
                            if (phieuThu.GoiDichVu)
                            {
                                baoCaoKetQuaDoanhThuTheoNgayHuy.DoanhThuThaiSanTrongGoi -= doanhThuTru;
                            }
                            else
                            {
                                baoCaoKetQuaDoanhThuTheoNgayHuy.DoanhThuThaiSanNgoaiGoi -= doanhThuTru;
                            }
                        }
                        else if (phieuThu.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamSucKhoe)
                        {
                            baoCaoKetQuaDoanhThuTheoNgayHuy.DoanhThuPhatSinhNgoaiGoiKSKDoan -= doanhThuTru;
                        }
                        else if (yeuCauTiepNhanSoSinhIds.Contains(phieuThu.YeuCauTiepNhanId))
                        {
                            baoCaoKetQuaDoanhThuTheoNgayHuy.DoanhThuSoSinh -= doanhThuTru;
                        }
                        else
                        {
                            var yeuCauTiepNhanDataChiTiet = yeuCauTiepNhanData.FirstOrDefault(o => o.Id == phieuThu.YeuCauTiepNhanId);
                            if (yeuCauTiepNhanDataChiTiet != null)
                            {
                                if (yeuCauTiepNhanDataChiTiet.HinhThucDenId == cauHinhBaoCao.HinhThucDenCBNVId)
                                {
                                    baoCaoKetQuaDoanhThuTheoNgayHuy.DoanhThuCBNV -= doanhThuTru;
                                }
                                else if (yeuCauTiepNhanDataChiTiet.HinhThucDenId == cauHinhBaoCao.HinhThucDenGioiThieuId)
                                {
                                    var tenDonVi = string.Empty;
                                    if (yeuCauTiepNhanDataChiTiet.NoiGioiThieuId != null)
                                    {
                                        tenDonVi = noiGioiThieus.FirstOrDefault(o => o.Id == yeuCauTiepNhanDataChiTiet.NoiGioiThieuId.Value)?.DonViMau?.Ten ?? string.Empty;
                                    }
                                    if (tenDonVi.ToLower().Contains(cauHinhBaoCao.NoiGioiThieuThamMy))
                                    {
                                        if (tenDonVi.ToLower().Contains(cauHinhBaoCao.NoiGioiThieuThamMyBsTu))
                                        {
                                            //Chỉ tính doanh thu từ các dịch vụ kỹ thuật trừ suất ăn (tính cả ngoại trú và nội trú), Doanh thu tính theo công thức: Thành tiền BV - Miễn giảm nếu có
                                            var doanhThuBsTu = phieuThu.BaoCaoKetQuaPhieuChiDataVos?
                                                .Where(o => o.YeuCauDichVuKyThuatId != null && o.NhomDichVuBenhVienId != cauHinhBaoCao.NhomDichVuBenhVienSuatAnId)
                                                .Select(o => o.Gia.GetValueOrDefault() * (decimal)o.SoLuong.GetValueOrDefault() - o.SoTienMienGiam.GetValueOrDefault()).DefaultIfEmpty().Sum();
                                            baoCaoKetQuaDoanhThuTheoNgayHuy.DoanhThuThamMyBSTu -= doanhThuBsTu.GetValueOrDefault();
                                        }
                                        else
                                        {
                                            //Doanh thu tính theo công thức: Thành tiền BV - Miễn giảm nếu có
                                            var doanhThuThamMy = phieuThu.BaoCaoKetQuaPhieuChiDataVos?.Select(o => o.Gia.GetValueOrDefault() * (decimal)o.SoLuong.GetValueOrDefault() - o.SoTienMienGiam.GetValueOrDefault()).DefaultIfEmpty().Sum();
                                            baoCaoKetQuaDoanhThuTheoNgayHuy.DoanhThuThamMyKhac -= doanhThuThamMy.GetValueOrDefault();
                                        }
                                    }
                                    else if (tenDonVi.ToLower().Contains(cauHinhBaoCao.NoiGioiThieuVietDuc))
                                    {
                                        baoCaoKetQuaDoanhThuTheoNgayHuy.DoanhThuVietDuc -= doanhThuTru;
                                    }
                                    else
                                    {
                                        baoCaoKetQuaDoanhThuTheoNgayHuy.DoanhThuCTV -= doanhThuTru;
                                    }
                                }
                                //tu den
                                else
                                {
                                    if (yeuCauTiepNhanDataChiTiet.BenhNhanNhi)
                                    {
                                        if (yeuCauTiepNhanDataChiTiet.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru || yeuCauTiepNhanDataChiTiet.QuyetToanTheoNoiTru == true)
                                        {
                                            baoCaoKetQuaDoanhThuTheoNgayHuy.DoanhThuBenhNhanTuDenNhiNoiTRu -= doanhThuTru;
                                        }
                                        else
                                        {
                                            var yeuCauTiepNhanNhiNgoaiTru = yeuCauTiepNhanNhiNgoaiTruData.FirstOrDefault(o => o.Id == yeuCauTiepNhanDataChiTiet.Id);
                                            if (yeuCauTiepNhanNhiNgoaiTru != null)
                                            {
                                                if (yeuCauTiepNhanNhiNgoaiTru.CountYeuCauDichVuKyThuatKhamSangLocTiemChung > 0)
                                                {
                                                    if (yeuCauTiepNhanNhiNgoaiTru.CountYeuCauKhamBenh == 0 && yeuCauTiepNhanNhiNgoaiTru.CountYeuCauDichVuKyNgoaiTiemChung == 0)
                                                    {
                                                        baoCaoKetQuaDoanhThuTheoNgayHuy.DoanhThuBenhNhanTuDenNhiTiemChung -= doanhThuTru;
                                                    }
                                                    else
                                                    {
                                                        baoCaoKetQuaDoanhThuTheoNgayHuy.DoanhThuBenhNhanTuDenNhiKcbTiemChung -= doanhThuTru;
                                                    }
                                                }
                                                else
                                                {
                                                    baoCaoKetQuaDoanhThuTheoNgayHuy.DoanhThuBenhNhanTuDenNhiKcb -= doanhThuTru;
                                                }
                                            }
                                            else
                                            {
                                                baoCaoKetQuaDoanhThuTheoNgayHuy.DoanhThuBenhNhanTuDenNhiKcb -= doanhThuTru;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (yeuCauTiepNhanDataChiTiet.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru || yeuCauTiepNhanDataChiTiet.QuyetToanTheoNoiTru == true)
                                        {
                                            baoCaoKetQuaDoanhThuTheoNgayHuy.DoanhThuBenhNhanTuDenNoiTru -= doanhThuTru;
                                        }
                                        else
                                        {
                                            baoCaoKetQuaDoanhThuTheoNgayHuy.DoanhThuBenhNhanTuDenKhachLe -= doanhThuTru;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (phieuThu.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru || phieuThu.QuyetToanTheoNoiTru == true)
                                {
                                    baoCaoKetQuaDoanhThuTheoNgayHuy.DoanhThuBenhNhanTuDenNoiTru -= doanhThuTru;
                                }
                                else
                                {
                                    baoCaoKetQuaDoanhThuTheoNgayHuy.DoanhThuBenhNhanTuDenKhachLe -= doanhThuTru;
                                }
                            }
                        }
                        //doanh thu RHM
                        var doanhThuRHMTru = phieuThu.BaoCaoKetQuaPhieuChiDataVos?
                            .Where(o => o.LoaiChiTienBenhNhan == LoaiChiTienBenhNhan.ThanhToanChiPhi &&
                                        ((o.YeuCauKhamBenhId != null && o.DichVuKhamBenhBenhVienId != null && dichVuKhamRangHamMatIds.Contains(o.DichVuKhamBenhBenhVienId.ToString())) ||
                                         (o.YeuCauDichVuKyThuatId != null && o.NhomDichVuBenhVienId != null && nhomDichVuBenhVienRangHamMatIds.Contains(o.NhomDichVuBenhVienId.ToString()))))
                            .Select(o => o.TienChiPhi.GetValueOrDefault() + o.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault()).DefaultIfEmpty().Sum();

                        baoCaoKetQuaDoanhThuTheoNgayHuy.DoanhThuRangHamMat -= doanhThuRHMTru.GetValueOrDefault();
                    }
                }
            }

            foreach (var phieuThu in phieuChiData.Where(o => o.LoaiChiTienBenhNhan == LoaiChiTienBenhNhan.HoanThu))
            {
                var ngayThuBaoCao = phieuThu.NgayThu.AddMilliseconds(khungGioMilliseconds * (-1));
                var baoCaoKetQuaDoanhThuTheoNgayHoan = baoCaoKetQuaDoanhThuTheoNgays.FirstOrDefault(o => o.Ngay == ngayThuBaoCao.Date);
                if (baoCaoKetQuaDoanhThuTheoNgayHoan != null)
                {
                    decimal doanhThuTru = phieuThu.SoTienThuTienMat.GetValueOrDefault();
                    if (dsMaYeuCauTiepNhanDungGoiThaiSans.Contains(phieuThu.MaYeuCauTiepNhan))
                    {
                        if (phieuThu.GoiDichVu)
                        {
                            baoCaoKetQuaDoanhThuTheoNgayHoan.DoanhThuThaiSanTrongGoi -= doanhThuTru;
                        }
                        else
                        {
                            baoCaoKetQuaDoanhThuTheoNgayHoan.DoanhThuThaiSanNgoaiGoi -= doanhThuTru;
                        }
                    }
                    else if (phieuThu.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamSucKhoe)
                    {
                        baoCaoKetQuaDoanhThuTheoNgayHoan.DoanhThuPhatSinhNgoaiGoiKSKDoan -= doanhThuTru;
                    }
                    else if (yeuCauTiepNhanSoSinhIds.Contains(phieuThu.YeuCauTiepNhanId))
                    {
                        baoCaoKetQuaDoanhThuTheoNgayHoan.DoanhThuSoSinh -= doanhThuTru;
                    }
                    else
                    {
                        var yeuCauTiepNhanDataChiTiet = yeuCauTiepNhanData.FirstOrDefault(o => o.Id == phieuThu.YeuCauTiepNhanId);
                        if (yeuCauTiepNhanDataChiTiet != null)
                        {
                            if (yeuCauTiepNhanDataChiTiet.HinhThucDenId == cauHinhBaoCao.HinhThucDenCBNVId)
                            {
                                baoCaoKetQuaDoanhThuTheoNgayHoan.DoanhThuCBNV -= doanhThuTru;
                            }
                            else if (yeuCauTiepNhanDataChiTiet.HinhThucDenId == cauHinhBaoCao.HinhThucDenGioiThieuId)
                            {
                                var tenDonVi = string.Empty;
                                if (yeuCauTiepNhanDataChiTiet.NoiGioiThieuId != null)
                                {
                                    tenDonVi = noiGioiThieus.FirstOrDefault(o => o.Id == yeuCauTiepNhanDataChiTiet.NoiGioiThieuId.Value)?.DonViMau?.Ten ?? string.Empty;
                                }
                                if (tenDonVi.ToLower().Contains(cauHinhBaoCao.NoiGioiThieuThamMy))
                                {
                                    if (tenDonVi.ToLower().Contains(cauHinhBaoCao.NoiGioiThieuThamMyBsTu))
                                    {
                                        //Chỉ tính doanh thu từ các dịch vụ kỹ thuật trừ suất ăn (tính cả ngoại trú và nội trú), Doanh thu tính theo công thức: Thành tiền BV - Miễn giảm nếu có
                                        var doanhThuBsTu = phieuThu.BaoCaoKetQuaPhieuChiDataVos?
                                            .Where(o => o.YeuCauDichVuKyThuatId != null && o.NhomDichVuBenhVienId != cauHinhBaoCao.NhomDichVuBenhVienSuatAnId)
                                            .Select(o => o.Gia.GetValueOrDefault() * (decimal)o.SoLuong.GetValueOrDefault() - o.SoTienMienGiam.GetValueOrDefault()).DefaultIfEmpty().Sum();
                                        baoCaoKetQuaDoanhThuTheoNgayHoan.DoanhThuThamMyBSTu -= doanhThuBsTu.GetValueOrDefault();
                                    }
                                    else
                                    {
                                        //Doanh thu tính theo công thức: Thành tiền BV - Miễn giảm nếu có
                                        var doanhThuThamMy = phieuThu.BaoCaoKetQuaPhieuChiDataVos?.Select(o => o.Gia.GetValueOrDefault() * (decimal)o.SoLuong.GetValueOrDefault() - o.SoTienMienGiam.GetValueOrDefault()).DefaultIfEmpty().Sum();
                                        baoCaoKetQuaDoanhThuTheoNgayHoan.DoanhThuThamMyKhac -= doanhThuThamMy.GetValueOrDefault();
                                    }
                                }
                                else if (tenDonVi.ToLower().Contains(cauHinhBaoCao.NoiGioiThieuVietDuc))
                                {
                                    baoCaoKetQuaDoanhThuTheoNgayHoan.DoanhThuVietDuc -= doanhThuTru;
                                }
                                else
                                {
                                    baoCaoKetQuaDoanhThuTheoNgayHoan.DoanhThuCTV -= doanhThuTru;
                                }
                            }
                            //tu den
                            else
                            {
                                if (yeuCauTiepNhanDataChiTiet.BenhNhanNhi)
                                {
                                    if (yeuCauTiepNhanDataChiTiet.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru || yeuCauTiepNhanDataChiTiet.QuyetToanTheoNoiTru == true)
                                    {
                                        baoCaoKetQuaDoanhThuTheoNgayHoan.DoanhThuBenhNhanTuDenNhiNoiTRu -= doanhThuTru;
                                    }
                                    else
                                    {
                                        var yeuCauTiepNhanNhiNgoaiTru = yeuCauTiepNhanNhiNgoaiTruData.FirstOrDefault(o => o.Id == yeuCauTiepNhanDataChiTiet.Id);
                                        if (yeuCauTiepNhanNhiNgoaiTru != null)
                                        {
                                            if (yeuCauTiepNhanNhiNgoaiTru.CountYeuCauDichVuKyThuatKhamSangLocTiemChung > 0)
                                            {
                                                if (yeuCauTiepNhanNhiNgoaiTru.CountYeuCauKhamBenh == 0 && yeuCauTiepNhanNhiNgoaiTru.CountYeuCauDichVuKyNgoaiTiemChung == 0)
                                                {
                                                    baoCaoKetQuaDoanhThuTheoNgayHoan.DoanhThuBenhNhanTuDenNhiTiemChung -= doanhThuTru;
                                                }
                                                else
                                                {
                                                    baoCaoKetQuaDoanhThuTheoNgayHoan.DoanhThuBenhNhanTuDenNhiKcbTiemChung -= doanhThuTru;
                                                }
                                            }
                                            else
                                            {
                                                baoCaoKetQuaDoanhThuTheoNgayHoan.DoanhThuBenhNhanTuDenNhiKcb -= doanhThuTru;
                                            }
                                        }
                                        else
                                        {
                                            baoCaoKetQuaDoanhThuTheoNgayHoan.DoanhThuBenhNhanTuDenNhiKcb -= doanhThuTru;
                                        }
                                    }
                                }
                                else
                                {
                                    if (yeuCauTiepNhanDataChiTiet.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru || yeuCauTiepNhanDataChiTiet.QuyetToanTheoNoiTru == true)
                                    {
                                        baoCaoKetQuaDoanhThuTheoNgayHoan.DoanhThuBenhNhanTuDenNoiTru -= doanhThuTru;
                                    }
                                    else
                                    {
                                        baoCaoKetQuaDoanhThuTheoNgayHoan.DoanhThuBenhNhanTuDenKhachLe -= doanhThuTru;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (phieuThu.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru || phieuThu.QuyetToanTheoNoiTru == true)
                            {
                                baoCaoKetQuaDoanhThuTheoNgayHoan.DoanhThuBenhNhanTuDenNoiTru -= doanhThuTru;
                            }
                            else
                            {
                                baoCaoKetQuaDoanhThuTheoNgayHoan.DoanhThuBenhNhanTuDenKhachLe -= doanhThuTru;
                            }
                        }
                    }

                    //doanh thu RHM
                    var doanhThuRHMTru = phieuThu.BaoCaoKetQuaPhieuChiDataVos?
                        .Where(o => (o.YeuCauKhamBenhId != null && o.DichVuKhamBenhBenhVienId != null && dichVuKhamRangHamMatIds.Contains(o.DichVuKhamBenhBenhVienId.ToString())) ||
                                     (o.YeuCauDichVuKyThuatId != null && o.NhomDichVuBenhVienId != null && nhomDichVuBenhVienRangHamMatIds.Contains(o.NhomDichVuBenhVienId.ToString())))
                        .Select(o => o.TienChiPhi.GetValueOrDefault() + o.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault()).DefaultIfEmpty().Sum();

                    baoCaoKetQuaDoanhThuTheoNgayHoan.DoanhThuRangHamMat -= doanhThuRHMTru.GetValueOrDefault();
                }
            }

            //dem so BN
            var phieuThuDataTheoTiepNhanGroup = phieuThuChiPhiTaiThuNganData.GroupBy(o => o.MaYeuCauTiepNhan).ToList();
            foreach (var phieuThuDataTheoTiepNhan in phieuThuDataTheoTiepNhanGroup)
            {
                var phieuThu = phieuThuDataTheoTiepNhan.OrderBy(o => o.NgayThu).First();
                var ngayThuBaoCao = phieuThu.NgayThu.AddMilliseconds(khungGioMilliseconds * (-1));
                var baoCaoKetQuaDoanhThuTheoNgay = baoCaoKetQuaDoanhThuTheoNgays.FirstOrDefault(o => o.Ngay == ngayThuBaoCao.Date);
                if (baoCaoKetQuaDoanhThuTheoNgay != null)
                {
                    if (dsMaYeuCauTiepNhanDungGoiThaiSans.Contains(phieuThu.MaYeuCauTiepNhan))
                    {
                        baoCaoKetQuaDoanhThuTheoNgay.SoNguoiThaiSanTrongGoi += 1;
                    }
                    else if (phieuThu.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamSucKhoe)
                    {
                        baoCaoKetQuaDoanhThuTheoNgay.SoNguoiPhatSinhNgoaiGoiKSKDoan += 1;
                    }
                    else if (yeuCauTiepNhanSoSinhIds.Contains(phieuThu.YeuCauTiepNhanId))
                    {
                        baoCaoKetQuaDoanhThuTheoNgay.SoNguoiSoSinh += 1;
                    }
                    else
                    {
                        var yeuCauTiepNhanDataChiTiet = yeuCauTiepNhanData.FirstOrDefault(o => o.Id == phieuThu.YeuCauTiepNhanId);
                        if (yeuCauTiepNhanDataChiTiet != null)
                        {
                            if (yeuCauTiepNhanDataChiTiet.HinhThucDenId == cauHinhBaoCao.HinhThucDenCBNVId)
                            {
                                baoCaoKetQuaDoanhThuTheoNgay.SoNguoiCBNV += 1;
                            }
                            else if (yeuCauTiepNhanDataChiTiet.HinhThucDenId == cauHinhBaoCao.HinhThucDenGioiThieuId)
                            {
                                var tenDonVi = string.Empty;
                                if (yeuCauTiepNhanDataChiTiet.NoiGioiThieuId != null)
                                {
                                    tenDonVi = noiGioiThieus.FirstOrDefault(o => o.Id == yeuCauTiepNhanDataChiTiet.NoiGioiThieuId.Value)?.DonViMau?.Ten ?? string.Empty;
                                }
                                if (tenDonVi.ToLower().Contains(cauHinhBaoCao.NoiGioiThieuThamMy))
                                {
                                    if (tenDonVi.ToLower().Contains(cauHinhBaoCao.NoiGioiThieuThamMyBsTu))
                                    {
                                        baoCaoKetQuaDoanhThuTheoNgay.SoNguoiThamMyBSTu += 1;
                                    }
                                    else
                                    {
                                        baoCaoKetQuaDoanhThuTheoNgay.SoNguoiThamMyKhac += 1;
                                    }
                                }
                                else if (tenDonVi.ToLower().Contains(cauHinhBaoCao.NoiGioiThieuVietDuc))
                                {
                                    baoCaoKetQuaDoanhThuTheoNgay.SoNguoiVietDuc += 1;
                                }
                                else
                                {
                                    baoCaoKetQuaDoanhThuTheoNgay.SoNguoiCTV += 1;
                                }
                            }
                            //tu den
                            else
                            {
                                if (yeuCauTiepNhanDataChiTiet.BenhNhanNhi)
                                {
                                    if (yeuCauTiepNhanDataChiTiet.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru || yeuCauTiepNhanDataChiTiet.QuyetToanTheoNoiTru == true)
                                    {
                                        baoCaoKetQuaDoanhThuTheoNgay.SoNguoiBenhNhanTuDenNhiNoiTRu += 1;
                                    }
                                    else
                                    {
                                        var yeuCauTiepNhanNhiNgoaiTru = yeuCauTiepNhanNhiNgoaiTruData.FirstOrDefault(o => o.Id == yeuCauTiepNhanDataChiTiet.Id);
                                        if (yeuCauTiepNhanNhiNgoaiTru != null)
                                        {
                                            if (yeuCauTiepNhanNhiNgoaiTru.CountYeuCauDichVuKyThuatKhamSangLocTiemChung > 0)
                                            {
                                                if (yeuCauTiepNhanNhiNgoaiTru.CountYeuCauKhamBenh == 0 && yeuCauTiepNhanNhiNgoaiTru.CountYeuCauDichVuKyNgoaiTiemChung == 0)
                                                {
                                                    baoCaoKetQuaDoanhThuTheoNgay.SoNguoiBenhNhanTuDenNhiTiemChung += 1;
                                                }
                                                else
                                                {
                                                    baoCaoKetQuaDoanhThuTheoNgay.SoNguoiBenhNhanTuDenNhiKcbTiemChung += 1;
                                                }
                                            }
                                            else
                                            {
                                                baoCaoKetQuaDoanhThuTheoNgay.SoNguoiBenhNhanTuDenNhiKcb += 1;
                                            }
                                        }
                                        else
                                        {
                                            baoCaoKetQuaDoanhThuTheoNgay.SoNguoiBenhNhanTuDenNhiKcb += 1;
                                        }
                                    }
                                }
                                else
                                {
                                    if (yeuCauTiepNhanDataChiTiet.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru || yeuCauTiepNhanDataChiTiet.QuyetToanTheoNoiTru == true)
                                    {
                                        baoCaoKetQuaDoanhThuTheoNgay.SoNguoiBenhNhanTuDenNoiTru += 1;
                                    }
                                    else
                                    {
                                        baoCaoKetQuaDoanhThuTheoNgay.SoNguoiBenhNhanTuDenKhachLe += 1;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (phieuThu.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru || phieuThu.QuyetToanTheoNoiTru == true)
                            {
                                baoCaoKetQuaDoanhThuTheoNgay.SoNguoiBenhNhanTuDenNoiTru += 1;
                            }
                            else
                            {
                                baoCaoKetQuaDoanhThuTheoNgay.SoNguoiBenhNhanTuDenKhachLe += 1;
                            }
                        }
                    }
                }
            }

            //dem so BN RHM
            var phieuThuDataTheoTiepNhanRHMGroup = phieuThuChiPhiTaiThuNganData
                .Where(t => t.BaoCaoKetQuaPhieuChiDataVos
                    .Any(o => o.LoaiChiTienBenhNhan == LoaiChiTienBenhNhan.ThanhToanChiPhi &&
                              ((o.YeuCauKhamBenhId != null && o.DichVuKhamBenhBenhVienId != null && dichVuKhamRangHamMatIds.Contains(o.DichVuKhamBenhBenhVienId.ToString())) ||
                               (o.YeuCauDichVuKyThuatId != null && o.NhomDichVuBenhVienId != null && nhomDichVuBenhVienRangHamMatIds.Contains(o.NhomDichVuBenhVienId.ToString())))))
                .GroupBy(o => o.MaYeuCauTiepNhan).ToList();
            foreach (var phieuThuDataTheoTiepNhan in phieuThuDataTheoTiepNhanRHMGroup)
            {
                var phieuThu = phieuThuDataTheoTiepNhan.OrderBy(o => o.NgayThu).First();
                var ngayThuBaoCao = phieuThu.NgayThu.AddMilliseconds(khungGioMilliseconds * (-1));
                var baoCaoKetQuaDoanhThuTheoNgay = baoCaoKetQuaDoanhThuTheoNgays.FirstOrDefault(o => o.Ngay == ngayThuBaoCao.Date);
                if (baoCaoKetQuaDoanhThuTheoNgay != null)
                {
                    baoCaoKetQuaDoanhThuTheoNgay.SoNguoiRangHamMat += 1;
                }
            }

            //tinh doanh thu vao grid
            //--tu den
            decimal donViDoanhThu = 1000000;
            //----KhachLe
            List<BaoCaoKetQuaColumnDoanhThuVo> baoCaoKetQuaBenhNhanTuDenKhachLe = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            baoCaoKetQuaBenhNhanTuDenKhachLe.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = 0,
                Cell1Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.Where(o => ngayDauThangTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiThangTruocBaoCao).Select(o => o.DoanhThuBenhNhanTuDenKhachLe).DefaultIfEmpty().Sum()),
                Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.Where(o => ngayDauTuanTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiTuanTruocBaoCao).Select(o => o.DoanhThuBenhNhanTuDenKhachLe).DefaultIfEmpty().Sum())
            });
            //----NoiTru
            List<BaoCaoKetQuaColumnDoanhThuVo> baoCaoKetQuaBenhNhanTuDenNoiTru = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            baoCaoKetQuaBenhNhanTuDenNoiTru.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = 0,
                Cell1Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.Where(o => ngayDauThangTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiThangTruocBaoCao).Select(o => o.DoanhThuBenhNhanTuDenNoiTru).DefaultIfEmpty().Sum()),
                Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.Where(o => ngayDauTuanTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiTuanTruocBaoCao).Select(o => o.DoanhThuBenhNhanTuDenNoiTru).DefaultIfEmpty().Sum())
            });
            //----NhiKcb
            List<BaoCaoKetQuaColumnDoanhThuVo> baoCaoKetQuaBenhNhanTuDenNhiKcb = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            baoCaoKetQuaBenhNhanTuDenNhiKcb.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = 0,
                Cell1Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.Where(o => ngayDauThangTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiThangTruocBaoCao).Select(o => o.DoanhThuBenhNhanTuDenNhiKcb).DefaultIfEmpty().Sum()),
                Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.Where(o => ngayDauTuanTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiTuanTruocBaoCao).Select(o => o.DoanhThuBenhNhanTuDenNhiKcb).DefaultIfEmpty().Sum())
            });
            //----NhiKcbTiemChung
            List<BaoCaoKetQuaColumnDoanhThuVo> baoCaoKetQuaBenhNhanTuDenNhiKcbTiemChung = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            baoCaoKetQuaBenhNhanTuDenNhiKcbTiemChung.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = 0,
                Cell1Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.Where(o => ngayDauThangTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiThangTruocBaoCao).Select(o => o.DoanhThuBenhNhanTuDenNhiKcbTiemChung).DefaultIfEmpty().Sum()),
                Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.Where(o => ngayDauTuanTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiTuanTruocBaoCao).Select(o => o.DoanhThuBenhNhanTuDenNhiKcbTiemChung).DefaultIfEmpty().Sum())
            });
            //----NhiTiemChung
            List<BaoCaoKetQuaColumnDoanhThuVo> baoCaoKetQuaBenhNhanTuDenNhiTiemChung = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            baoCaoKetQuaBenhNhanTuDenNhiTiemChung.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = 0,
                Cell1Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.Where(o => ngayDauThangTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiThangTruocBaoCao).Select(o => o.DoanhThuBenhNhanTuDenNhiTiemChung).DefaultIfEmpty().Sum()),
                Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.Where(o => ngayDauTuanTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiTuanTruocBaoCao).Select(o => o.DoanhThuBenhNhanTuDenNhiTiemChung).DefaultIfEmpty().Sum())
            });
            //----NhiNoiTRu
            List<BaoCaoKetQuaColumnDoanhThuVo> baoCaoKetQuaBenhNhanTuDenNhiNoiTRu = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            baoCaoKetQuaBenhNhanTuDenNhiNoiTRu.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = 0,
                Cell1Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.Where(o => ngayDauThangTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiThangTruocBaoCao).Select(o => o.DoanhThuBenhNhanTuDenNhiNoiTRu).DefaultIfEmpty().Sum()),
                Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.Where(o => ngayDauTuanTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiTuanTruocBaoCao).Select(o => o.DoanhThuBenhNhanTuDenNhiNoiTRu).DefaultIfEmpty().Sum())
            });

            //--SoSinh
            List<BaoCaoKetQuaColumnDoanhThuVo> baoCaoKetQuaSoSinh = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            baoCaoKetQuaSoSinh.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = 0,
                Cell1Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.Where(o => ngayDauThangTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiThangTruocBaoCao).Select(o => o.DoanhThuSoSinh).DefaultIfEmpty().Sum()),
                Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.Where(o => ngayDauTuanTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiTuanTruocBaoCao).Select(o => o.DoanhThuSoSinh).DefaultIfEmpty().Sum())
            });
            //--PhatSinhNgoaiGoiKSKDoan
            List<BaoCaoKetQuaColumnDoanhThuVo> baoCaoKetQuaPhatSinhNgoaiGoiKSKDoan = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            baoCaoKetQuaPhatSinhNgoaiGoiKSKDoan.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = 0,
                Cell1Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.Where(o => ngayDauThangTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiThangTruocBaoCao).Select(o => o.DoanhThuPhatSinhNgoaiGoiKSKDoan).DefaultIfEmpty().Sum()),
                Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.Where(o => ngayDauTuanTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiTuanTruocBaoCao).Select(o => o.DoanhThuPhatSinhNgoaiGoiKSKDoan).DefaultIfEmpty().Sum())
            });
            //--CBNV
            List<BaoCaoKetQuaColumnDoanhThuVo> baoCaoKetQuaCBNV = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            baoCaoKetQuaCBNV.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = 0,
                Cell1Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.Where(o => ngayDauThangTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiThangTruocBaoCao).Select(o => o.DoanhThuCBNV).DefaultIfEmpty().Sum()),
                Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.Where(o => ngayDauTuanTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiTuanTruocBaoCao).Select(o => o.DoanhThuCBNV).DefaultIfEmpty().Sum())
            });
            //--CTV
            List<BaoCaoKetQuaColumnDoanhThuVo> baoCaoKetQuaCTV = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            baoCaoKetQuaCTV.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = 0,
                Cell1Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.Where(o => ngayDauThangTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiThangTruocBaoCao).Select(o => o.DoanhThuCTV).DefaultIfEmpty().Sum()),
                Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.Where(o => ngayDauTuanTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiTuanTruocBaoCao).Select(o => o.DoanhThuCTV).DefaultIfEmpty().Sum())
            });
            //--VietDuc
            List<BaoCaoKetQuaColumnDoanhThuVo> baoCaoKetQuaVietDuc = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            baoCaoKetQuaVietDuc.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = 0,
                Cell1Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.Where(o => ngayDauThangTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiThangTruocBaoCao).Select(o => o.DoanhThuVietDuc).DefaultIfEmpty().Sum()),
                Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.Where(o => ngayDauTuanTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiTuanTruocBaoCao).Select(o => o.DoanhThuVietDuc).DefaultIfEmpty().Sum())
            });
            //--ThamMyBSTu
            List<BaoCaoKetQuaColumnDoanhThuVo> baoCaoKetQuaThamMyBSTu = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            baoCaoKetQuaThamMyBSTu.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = 0,
                Cell1Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.Where(o => ngayDauThangTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiThangTruocBaoCao).Select(o => o.DoanhThuThamMyBSTu).DefaultIfEmpty().Sum()),
                Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.Where(o => ngayDauTuanTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiTuanTruocBaoCao).Select(o => o.DoanhThuThamMyBSTu).DefaultIfEmpty().Sum())
            });
            //--ThamMyKhac
            List<BaoCaoKetQuaColumnDoanhThuVo> baoCaoKetQuaThamMyKhac = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            baoCaoKetQuaThamMyKhac.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = 0,
                Cell1Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.Where(o => ngayDauThangTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiThangTruocBaoCao).Select(o => o.DoanhThuThamMyKhac).DefaultIfEmpty().Sum()),
                Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.Where(o => ngayDauTuanTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiTuanTruocBaoCao).Select(o => o.DoanhThuThamMyKhac).DefaultIfEmpty().Sum())
            });
            //--ThaiSanTrongGoi
            List<BaoCaoKetQuaColumnDoanhThuVo> baoCaoKetQuaThaiSanTrongGoi = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            baoCaoKetQuaThaiSanTrongGoi.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = 0,
                Cell1Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.Where(o => ngayDauThangTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiThangTruocBaoCao).Select(o => o.DoanhThuThaiSanTrongGoi).DefaultIfEmpty().Sum()),
                Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.Where(o => ngayDauTuanTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiTuanTruocBaoCao).Select(o => o.DoanhThuThaiSanTrongGoi).DefaultIfEmpty().Sum())
            });
            //--ThaiSanNgoaiGoi
            List<BaoCaoKetQuaColumnDoanhThuVo> baoCaoKetQuaThaiSanNgoaiGoi = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            baoCaoKetQuaThaiSanNgoaiGoi.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = 0,
                Cell1Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.Where(o => ngayDauThangTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiThangTruocBaoCao).Select(o => o.DoanhThuThaiSanNgoaiGoi).DefaultIfEmpty().Sum()),
                Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.Where(o => ngayDauTuanTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiTuanTruocBaoCao).Select(o => o.DoanhThuThaiSanNgoaiGoi).DefaultIfEmpty().Sum())
            });
            //--RangHamMat
            List<BaoCaoKetQuaColumnDoanhThuVo> baoCaoKetQuaRangHamMat = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            baoCaoKetQuaRangHamMat.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = 0,
                Cell1Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.Where(o => ngayDauThangTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiThangTruocBaoCao).Select(o => o.DoanhThuRangHamMat).DefaultIfEmpty().Sum()),
                Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.Where(o => ngayDauTuanTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiTuanTruocBaoCao).Select(o => o.DoanhThuRangHamMat).DefaultIfEmpty().Sum())
            });

            int iDoanhThu = 0;
            while (tuNgayBaoCao.AddDays(iDoanhThu) < denNgayBaoCao)
            {
                //----KhachLe
                baoCaoKetQuaBenhNhanTuDenKhachLe.Add(new BaoCaoKetQuaColumnDoanhThuVo
                {
                    Index = iDoanhThu + 1,
                    Cell1Value = baoCaoKetQuaDoanhThuTheoNgays.FirstOrDefault(o => tuNgayBaoCao.AddDays(iDoanhThu) == o.Ngay)?.SoNguoiBenhNhanTuDenKhachLe,
                    Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.FirstOrDefault(o => tuNgayBaoCao.AddDays(iDoanhThu) == o.Ngay)?.DoanhThuBenhNhanTuDenKhachLe),
                });
                //----NoiTru
                baoCaoKetQuaBenhNhanTuDenNoiTru.Add(new BaoCaoKetQuaColumnDoanhThuVo
                {
                    Index = iDoanhThu + 1,
                    Cell1Value = baoCaoKetQuaDoanhThuTheoNgays.FirstOrDefault(o => tuNgayBaoCao.AddDays(iDoanhThu) == o.Ngay)?.SoNguoiBenhNhanTuDenNoiTru,
                    Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.FirstOrDefault(o => tuNgayBaoCao.AddDays(iDoanhThu) == o.Ngay)?.DoanhThuBenhNhanTuDenNoiTru),
                });
                //----NhiKcb
                baoCaoKetQuaBenhNhanTuDenNhiKcb.Add(new BaoCaoKetQuaColumnDoanhThuVo
                {
                    Index = iDoanhThu + 1,
                    Cell1Value = baoCaoKetQuaDoanhThuTheoNgays.FirstOrDefault(o => tuNgayBaoCao.AddDays(iDoanhThu) == o.Ngay)?.SoNguoiBenhNhanTuDenNhiKcb,
                    Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.FirstOrDefault(o => tuNgayBaoCao.AddDays(iDoanhThu) == o.Ngay)?.DoanhThuBenhNhanTuDenNhiKcb),
                });
                //----NhiKcbTiemChung
                baoCaoKetQuaBenhNhanTuDenNhiKcbTiemChung.Add(new BaoCaoKetQuaColumnDoanhThuVo
                {
                    Index = iDoanhThu + 1,
                    Cell1Value = baoCaoKetQuaDoanhThuTheoNgays.FirstOrDefault(o => tuNgayBaoCao.AddDays(iDoanhThu) == o.Ngay)?.SoNguoiBenhNhanTuDenNhiKcbTiemChung,
                    Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.FirstOrDefault(o => tuNgayBaoCao.AddDays(iDoanhThu) == o.Ngay)?.DoanhThuBenhNhanTuDenNhiKcbTiemChung),
                });
                //----NhiTiemChung
                baoCaoKetQuaBenhNhanTuDenNhiTiemChung.Add(new BaoCaoKetQuaColumnDoanhThuVo
                {
                    Index = iDoanhThu + 1,
                    Cell1Value = baoCaoKetQuaDoanhThuTheoNgays.FirstOrDefault(o => tuNgayBaoCao.AddDays(iDoanhThu) == o.Ngay)?.SoNguoiBenhNhanTuDenNhiTiemChung,
                    Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.FirstOrDefault(o => tuNgayBaoCao.AddDays(iDoanhThu) == o.Ngay)?.DoanhThuBenhNhanTuDenNhiTiemChung),
                });
                //----NhiNoiTRu
                baoCaoKetQuaBenhNhanTuDenNhiNoiTRu.Add(new BaoCaoKetQuaColumnDoanhThuVo
                {
                    Index = iDoanhThu + 1,
                    Cell1Value = baoCaoKetQuaDoanhThuTheoNgays.FirstOrDefault(o => tuNgayBaoCao.AddDays(iDoanhThu) == o.Ngay)?.SoNguoiBenhNhanTuDenNhiNoiTRu,
                    Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.FirstOrDefault(o => tuNgayBaoCao.AddDays(iDoanhThu) == o.Ngay)?.DoanhThuBenhNhanTuDenNhiNoiTRu),
                });

                //--SoSinh
                baoCaoKetQuaSoSinh.Add(new BaoCaoKetQuaColumnDoanhThuVo
                {
                    Index = iDoanhThu + 1,
                    Cell1Value = baoCaoKetQuaDoanhThuTheoNgays.FirstOrDefault(o => tuNgayBaoCao.AddDays(iDoanhThu) == o.Ngay)?.SoNguoiSoSinh,
                    Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.FirstOrDefault(o => tuNgayBaoCao.AddDays(iDoanhThu) == o.Ngay)?.DoanhThuSoSinh),
                });
                //--PhatSinhNgoaiGoiKSKDoan
                baoCaoKetQuaPhatSinhNgoaiGoiKSKDoan.Add(new BaoCaoKetQuaColumnDoanhThuVo
                {
                    Index = iDoanhThu + 1,
                    Cell1Value = baoCaoKetQuaDoanhThuTheoNgays.FirstOrDefault(o => tuNgayBaoCao.AddDays(iDoanhThu) == o.Ngay)?.SoNguoiPhatSinhNgoaiGoiKSKDoan,
                    Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.FirstOrDefault(o => tuNgayBaoCao.AddDays(iDoanhThu) == o.Ngay)?.DoanhThuPhatSinhNgoaiGoiKSKDoan),
                });
                //--CBNV
                baoCaoKetQuaCBNV.Add(new BaoCaoKetQuaColumnDoanhThuVo
                {
                    Index = iDoanhThu + 1,
                    Cell1Value = baoCaoKetQuaDoanhThuTheoNgays.FirstOrDefault(o => tuNgayBaoCao.AddDays(iDoanhThu) == o.Ngay)?.SoNguoiCBNV,
                    Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.FirstOrDefault(o => tuNgayBaoCao.AddDays(iDoanhThu) == o.Ngay)?.DoanhThuCBNV),
                });
                //--CTV
                baoCaoKetQuaCTV.Add(new BaoCaoKetQuaColumnDoanhThuVo
                {
                    Index = iDoanhThu + 1,
                    Cell1Value = baoCaoKetQuaDoanhThuTheoNgays.FirstOrDefault(o => tuNgayBaoCao.AddDays(iDoanhThu) == o.Ngay)?.SoNguoiCTV,
                    Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.FirstOrDefault(o => tuNgayBaoCao.AddDays(iDoanhThu) == o.Ngay)?.DoanhThuCTV),
                });
                //--VietDuc
                baoCaoKetQuaVietDuc.Add(new BaoCaoKetQuaColumnDoanhThuVo
                {
                    Index = iDoanhThu + 1,
                    Cell1Value = baoCaoKetQuaDoanhThuTheoNgays.FirstOrDefault(o => tuNgayBaoCao.AddDays(iDoanhThu) == o.Ngay)?.SoNguoiVietDuc,
                    Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.FirstOrDefault(o => tuNgayBaoCao.AddDays(iDoanhThu) == o.Ngay)?.DoanhThuVietDuc),
                });
                //--ThamMyBSTu
                baoCaoKetQuaThamMyBSTu.Add(new BaoCaoKetQuaColumnDoanhThuVo
                {
                    Index = iDoanhThu + 1,
                    Cell1Value = baoCaoKetQuaDoanhThuTheoNgays.FirstOrDefault(o => tuNgayBaoCao.AddDays(iDoanhThu) == o.Ngay)?.SoNguoiThamMyBSTu,
                    Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.FirstOrDefault(o => tuNgayBaoCao.AddDays(iDoanhThu) == o.Ngay)?.DoanhThuThamMyBSTu),
                });
                //--ThamMyKhac
                baoCaoKetQuaThamMyKhac.Add(new BaoCaoKetQuaColumnDoanhThuVo
                {
                    Index = iDoanhThu + 1,
                    Cell1Value = baoCaoKetQuaDoanhThuTheoNgays.FirstOrDefault(o => tuNgayBaoCao.AddDays(iDoanhThu) == o.Ngay)?.SoNguoiThamMyKhac,
                    Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.FirstOrDefault(o => tuNgayBaoCao.AddDays(iDoanhThu) == o.Ngay)?.DoanhThuThamMyKhac),
                });
                //--ThaiSanTrongGoi
                baoCaoKetQuaThaiSanTrongGoi.Add(new BaoCaoKetQuaColumnDoanhThuVo
                {
                    Index = iDoanhThu + 1,
                    Cell1Value = baoCaoKetQuaDoanhThuTheoNgays.FirstOrDefault(o => tuNgayBaoCao.AddDays(iDoanhThu) == o.Ngay)?.SoNguoiThaiSanTrongGoi,
                    Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.FirstOrDefault(o => tuNgayBaoCao.AddDays(iDoanhThu) == o.Ngay)?.DoanhThuThaiSanTrongGoi),
                });
                //--ThaiSanNgoaiGoi
                baoCaoKetQuaThaiSanNgoaiGoi.Add(new BaoCaoKetQuaColumnDoanhThuVo
                {
                    Index = iDoanhThu + 1,
                    Cell1Value = baoCaoKetQuaDoanhThuTheoNgays.FirstOrDefault(o => tuNgayBaoCao.AddDays(iDoanhThu) == o.Ngay)?.SoNguoiThaiSanNgoaiGoi,
                    Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.FirstOrDefault(o => tuNgayBaoCao.AddDays(iDoanhThu) == o.Ngay)?.DoanhThuThaiSanNgoaiGoi),
                });
                //--RangHamMat
                baoCaoKetQuaRangHamMat.Add(new BaoCaoKetQuaColumnDoanhThuVo
                {
                    Index = iDoanhThu + 1,
                    Cell1Value = baoCaoKetQuaDoanhThuTheoNgays.FirstOrDefault(o => tuNgayBaoCao.AddDays(iDoanhThu) == o.Ngay)?.SoNguoiRangHamMat,
                    Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaDoanhThuTheoNgays.FirstOrDefault(o => tuNgayBaoCao.AddDays(iDoanhThu) == o.Ngay)?.DoanhThuRangHamMat),
                });

                iDoanhThu++;
            }

            //----KhachLe
            var tongSoNguoiBenhNhanTuDenKhachLe = baoCaoKetQuaBenhNhanTuDenKhachLe.Where(o => o.Index > 0).Sum(o => o.Cell1Value.GetValueOrDefault());
            var tongDoanhThuBenhNhanTuDenKhachLe = baoCaoKetQuaBenhNhanTuDenKhachLe.Where(o => o.Index > 0).Sum(o => o.Cell2Value.GetValueOrDefault());
            baoCaoKetQuaBenhNhanTuDenKhachLe.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = iDoanhThu + 1,
                Cell1Value = tongSoNguoiBenhNhanTuDenKhachLe,
                Cell2Value = tongDoanhThuBenhNhanTuDenKhachLe
            });
            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaBenhNhanTuDenKhachLe = baoCaoKetQuaBenhNhanTuDenKhachLe;
            //----NoiTru
            var tongSoNguoiBenhNhanTuDenNoiTru = baoCaoKetQuaBenhNhanTuDenNoiTru.Where(o => o.Index > 0).Sum(o => o.Cell1Value.GetValueOrDefault());
            var tongDoanhThuBenhNhanTuDenNoiTru = baoCaoKetQuaBenhNhanTuDenNoiTru.Where(o => o.Index > 0).Sum(o => o.Cell2Value.GetValueOrDefault());
            baoCaoKetQuaBenhNhanTuDenNoiTru.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = iDoanhThu + 1,
                Cell1Value = tongSoNguoiBenhNhanTuDenNoiTru,
                Cell2Value = tongDoanhThuBenhNhanTuDenNoiTru
            });
            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaBenhNhanTuDenNoiTru = baoCaoKetQuaBenhNhanTuDenNoiTru;
            //----NhiKcb
            var tongSoNguoiBenhNhanTuDenNhiKcb = baoCaoKetQuaBenhNhanTuDenNhiKcb.Where(o => o.Index > 0).Sum(o => o.Cell1Value.GetValueOrDefault());
            var tongDoanhThuBenhNhanTuDenNhiKcb = baoCaoKetQuaBenhNhanTuDenNhiKcb.Where(o => o.Index > 0).Sum(o => o.Cell2Value.GetValueOrDefault());
            baoCaoKetQuaBenhNhanTuDenNhiKcb.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = iDoanhThu + 1,
                Cell1Value = tongSoNguoiBenhNhanTuDenNhiKcb,
                Cell2Value = tongDoanhThuBenhNhanTuDenNhiKcb
            });
            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaBenhNhanTuDenNhiKcb = baoCaoKetQuaBenhNhanTuDenNhiKcb;
            //----NhiKcbTiemChung
            var tongSoNguoiBenhNhanTuDenNhiKcbTiemChung = baoCaoKetQuaBenhNhanTuDenNhiKcbTiemChung.Where(o => o.Index > 0).Sum(o => o.Cell1Value.GetValueOrDefault());
            var tongDoanhThuBenhNhanTuDenNhiKcbTiemChung = baoCaoKetQuaBenhNhanTuDenNhiKcbTiemChung.Where(o => o.Index > 0).Sum(o => o.Cell2Value.GetValueOrDefault());
            baoCaoKetQuaBenhNhanTuDenNhiKcbTiemChung.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = iDoanhThu + 1,
                Cell1Value = tongSoNguoiBenhNhanTuDenNhiKcbTiemChung,
                Cell2Value = tongDoanhThuBenhNhanTuDenNhiKcbTiemChung
            });
            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaBenhNhanTuDenNhiKcbTiemChung = baoCaoKetQuaBenhNhanTuDenNhiKcbTiemChung;
            //----NhiTiemChung
            var tongSoNguoiBenhNhanTuDenNhiTiemChung = baoCaoKetQuaBenhNhanTuDenNhiTiemChung.Where(o => o.Index > 0).Sum(o => o.Cell1Value.GetValueOrDefault());
            var tongDoanhThuBenhNhanTuDenNhiTiemChung = baoCaoKetQuaBenhNhanTuDenNhiTiemChung.Where(o => o.Index > 0).Sum(o => o.Cell2Value.GetValueOrDefault());
            baoCaoKetQuaBenhNhanTuDenNhiTiemChung.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = iDoanhThu + 1,
                Cell1Value = tongSoNguoiBenhNhanTuDenNhiTiemChung,
                Cell2Value = tongDoanhThuBenhNhanTuDenNhiTiemChung
            });
            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaBenhNhanTuDenNhiTiemChung = baoCaoKetQuaBenhNhanTuDenNhiTiemChung;
            //----NhiNoiTRu
            var tongSoNguoiBenhNhanTuDenNhiNoiTRu = baoCaoKetQuaBenhNhanTuDenNhiNoiTRu.Where(o => o.Index > 0).Sum(o => o.Cell1Value.GetValueOrDefault());
            var tongDoanhThuBenhNhanTuDenNhiNoiTRu = baoCaoKetQuaBenhNhanTuDenNhiNoiTRu.Where(o => o.Index > 0).Sum(o => o.Cell2Value.GetValueOrDefault());
            baoCaoKetQuaBenhNhanTuDenNhiNoiTRu.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = iDoanhThu + 1,
                Cell1Value = tongSoNguoiBenhNhanTuDenNhiNoiTRu,
                Cell2Value = tongDoanhThuBenhNhanTuDenNhiNoiTRu
            });
            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaBenhNhanTuDenNhiNoiTRu = baoCaoKetQuaBenhNhanTuDenNhiNoiTRu;


            //--SoSinh
            var tongSoNguoiSoSinh = baoCaoKetQuaSoSinh.Where(o => o.Index > 0).Sum(o => o.Cell1Value.GetValueOrDefault());
            var tongDoanhThuSoSinh = baoCaoKetQuaSoSinh.Where(o => o.Index > 0).Sum(o => o.Cell2Value.GetValueOrDefault());
            baoCaoKetQuaSoSinh.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = iDoanhThu + 1,
                Cell1Value = tongSoNguoiSoSinh,
                Cell2Value = tongDoanhThuSoSinh
            });
            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaSoSinh = baoCaoKetQuaSoSinh;
            //--PhatSinhNgoaiGoiKSKDoan
            var tongSoNguoiPhatSinhNgoaiGoiKSKDoan = baoCaoKetQuaPhatSinhNgoaiGoiKSKDoan.Where(o => o.Index > 0).Sum(o => o.Cell1Value.GetValueOrDefault());
            var tongDoanhThuPhatSinhNgoaiGoiKSKDoan = baoCaoKetQuaPhatSinhNgoaiGoiKSKDoan.Where(o => o.Index > 0).Sum(o => o.Cell2Value.GetValueOrDefault());
            baoCaoKetQuaPhatSinhNgoaiGoiKSKDoan.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = iDoanhThu + 1,
                Cell1Value = tongSoNguoiPhatSinhNgoaiGoiKSKDoan,
                Cell2Value = tongDoanhThuPhatSinhNgoaiGoiKSKDoan
            });
            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaPhatSinhNgoaiGoiKSKDoan = baoCaoKetQuaPhatSinhNgoaiGoiKSKDoan;
            //--CBNV
            var tongSoNguoiCBNV = baoCaoKetQuaCBNV.Where(o => o.Index > 0).Sum(o => o.Cell1Value.GetValueOrDefault());
            var tongDoanhThuCBNV = baoCaoKetQuaCBNV.Where(o => o.Index > 0).Sum(o => o.Cell2Value.GetValueOrDefault());
            baoCaoKetQuaCBNV.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = iDoanhThu + 1,
                Cell1Value = tongSoNguoiCBNV,
                Cell2Value = tongDoanhThuCBNV
            });
            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaCBNV = baoCaoKetQuaCBNV;
            //--CTV
            var tongSoNguoiCTV = baoCaoKetQuaCTV.Where(o => o.Index > 0).Sum(o => o.Cell1Value.GetValueOrDefault());
            var tongDoanhThuCTV = baoCaoKetQuaCTV.Where(o => o.Index > 0).Sum(o => o.Cell2Value.GetValueOrDefault());
            baoCaoKetQuaCTV.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = iDoanhThu + 1,
                Cell1Value = tongSoNguoiCTV,
                Cell2Value = tongDoanhThuCTV
            });
            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaCTV = baoCaoKetQuaCTV;
            //--VietDuc
            var tongSoNguoiVietDuc = baoCaoKetQuaVietDuc.Where(o => o.Index > 0).Sum(o => o.Cell1Value.GetValueOrDefault());
            var tongDoanhThuVietDuc = baoCaoKetQuaVietDuc.Where(o => o.Index > 0).Sum(o => o.Cell2Value.GetValueOrDefault());
            baoCaoKetQuaVietDuc.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = iDoanhThu + 1,
                Cell1Value = tongSoNguoiVietDuc,
                Cell2Value = tongDoanhThuVietDuc
            });
            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaVietDuc = baoCaoKetQuaVietDuc;
            //--ThamMyBSTu
            var tongSoNguoiThamMyBSTu = baoCaoKetQuaThamMyBSTu.Where(o => o.Index > 0).Sum(o => o.Cell1Value.GetValueOrDefault());
            var tongDoanhThuThamMyBSTu = baoCaoKetQuaThamMyBSTu.Where(o => o.Index > 0).Sum(o => o.Cell2Value.GetValueOrDefault());
            baoCaoKetQuaThamMyBSTu.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = iDoanhThu + 1,
                Cell1Value = tongSoNguoiThamMyBSTu,
                Cell2Value = tongDoanhThuThamMyBSTu
            });
            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaThamMyBSTu = baoCaoKetQuaThamMyBSTu;
            //--ThamMyKhac
            var tongSoNguoiThamMyKhac = baoCaoKetQuaThamMyKhac.Where(o => o.Index > 0).Sum(o => o.Cell1Value.GetValueOrDefault());
            var tongDoanhThuThamMyKhac = baoCaoKetQuaThamMyKhac.Where(o => o.Index > 0).Sum(o => o.Cell2Value.GetValueOrDefault());
            baoCaoKetQuaThamMyKhac.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = iDoanhThu + 1,
                Cell1Value = tongSoNguoiThamMyKhac,
                Cell2Value = tongDoanhThuThamMyKhac
            });
            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaThamMyKhac = baoCaoKetQuaThamMyKhac;
            //--ThaiSanTrongGoi
            var tongSoNguoiThaiSanTrongGoi = baoCaoKetQuaThaiSanTrongGoi.Where(o => o.Index > 0).Sum(o => o.Cell1Value.GetValueOrDefault());
            var tongDoanhThuThaiSanTrongGoi = baoCaoKetQuaThaiSanTrongGoi.Where(o => o.Index > 0).Sum(o => o.Cell2Value.GetValueOrDefault());
            baoCaoKetQuaThaiSanTrongGoi.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = iDoanhThu + 1,
                Cell1Value = tongSoNguoiThaiSanTrongGoi,
                Cell2Value = tongDoanhThuThaiSanTrongGoi
            });
            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaThaiSanTrongGoi = baoCaoKetQuaThaiSanTrongGoi;
            //--ThaiSanNgoaiGoi
            var tongSoNguoiThaiSanNgoaiGoi = baoCaoKetQuaThaiSanNgoaiGoi.Where(o => o.Index > 0).Sum(o => o.Cell1Value.GetValueOrDefault());
            var tongDoanhThuThaiSanNgoaiGoi = baoCaoKetQuaThaiSanNgoaiGoi.Where(o => o.Index > 0).Sum(o => o.Cell2Value.GetValueOrDefault());
            baoCaoKetQuaThaiSanNgoaiGoi.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = iDoanhThu + 1,
                Cell1Value = tongSoNguoiThaiSanNgoaiGoi,
                Cell2Value = tongDoanhThuThaiSanNgoaiGoi
            });
            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaThaiSanNgoaiGoi = baoCaoKetQuaThaiSanNgoaiGoi;
            //--RangHamMat
            var tongSoNguoiRangHamMat = baoCaoKetQuaRangHamMat.Where(o => o.Index > 0).Sum(o => o.Cell1Value.GetValueOrDefault());
            var tongDoanhThuRangHamMat = baoCaoKetQuaRangHamMat.Where(o => o.Index > 0).Sum(o => o.Cell2Value.GetValueOrDefault());
            baoCaoKetQuaRangHamMat.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = iDoanhThu + 1,
                Cell1Value = tongSoNguoiRangHamMat,
                Cell2Value = tongDoanhThuRangHamMat
            });
            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaRangHamMat = baoCaoKetQuaRangHamMat;

            //tong doanh thu

            List<BaoCaoKetQuaColumnDoanhThuVo> baoCaoKetQuaKhamChuaBenhVaDoanhThuNgay = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            List<BaoCaoKetQuaColumnDoanhThuVo> baoCaoKetQuaBenhNhanTuDenTong = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            for (int i = 0; i <= lastIndex; i++)
            {
                var cell1ValueBenhNhanTuDenTong = baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaBenhNhanTuDenKhachLe.FirstOrDefault(x => x.Index == i)?.Cell1Value.GetValueOrDefault() +
                                                    baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaBenhNhanTuDenNoiTru.FirstOrDefault(x => x.Index == i)?.Cell1Value.GetValueOrDefault() +
                                                    baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaBenhNhanTuDenNhiKcb.FirstOrDefault(x => x.Index == i)?.Cell1Value.GetValueOrDefault() +
                                                    baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaBenhNhanTuDenNhiKcbTiemChung.FirstOrDefault(x => x.Index == i)?.Cell1Value.GetValueOrDefault() +
                                                    baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaBenhNhanTuDenNhiTiemChung.FirstOrDefault(x => x.Index == i)?.Cell1Value.GetValueOrDefault() +
                                                    baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaBenhNhanTuDenNhiNoiTRu.FirstOrDefault(x => x.Index == i)?.Cell1Value.GetValueOrDefault();

                var cell2ValueBenhNhanTuDenTong = baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaBenhNhanTuDenKhachLe.FirstOrDefault(x => x.Index == i)?.Cell2Value.GetValueOrDefault() +
                                                    baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaBenhNhanTuDenNoiTru.FirstOrDefault(x => x.Index == i)?.Cell2Value.GetValueOrDefault() +
                                                    baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaBenhNhanTuDenNhiKcb.FirstOrDefault(x => x.Index == i)?.Cell2Value.GetValueOrDefault() +
                                                    baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaBenhNhanTuDenNhiKcbTiemChung.FirstOrDefault(x => x.Index == i)?.Cell2Value.GetValueOrDefault() +
                                                    baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaBenhNhanTuDenNhiTiemChung.FirstOrDefault(x => x.Index == i)?.Cell2Value.GetValueOrDefault() +
                                                    baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaBenhNhanTuDenNhiNoiTRu.FirstOrDefault(x => x.Index == i)?.Cell2Value.GetValueOrDefault();
                baoCaoKetQuaBenhNhanTuDenTong.Add(new BaoCaoKetQuaColumnDoanhThuVo
                {
                    Index = i,
                    Cell1Value = cell1ValueBenhNhanTuDenTong,
                    Cell2Value = cell2ValueBenhNhanTuDenTong,
                });

                var cell1ValueDoanhThuNgay = cell1ValueBenhNhanTuDenTong.GetValueOrDefault() +
                                            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaSoSinh.FirstOrDefault(x => x.Index == i)?.Cell1Value.GetValueOrDefault() +
                                            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaPhatSinhNgoaiGoiKSKDoan.FirstOrDefault(x => x.Index == i)?.Cell1Value.GetValueOrDefault() +
                                            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaCBNV.FirstOrDefault(x => x.Index == i)?.Cell1Value.GetValueOrDefault() +
                                            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaCTV.FirstOrDefault(x => x.Index == i)?.Cell1Value.GetValueOrDefault() +
                                            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaVietDuc.FirstOrDefault(x => x.Index == i)?.Cell1Value.GetValueOrDefault() +
                                            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaThamMyBSTu.FirstOrDefault(x => x.Index == i)?.Cell1Value.GetValueOrDefault() +
                                            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaThamMyKhac.FirstOrDefault(x => x.Index == i)?.Cell1Value.GetValueOrDefault() +
                                            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaThaiSanTrongGoi.FirstOrDefault(x => x.Index == i)?.Cell1Value.GetValueOrDefault() +
                                            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaThaiSanNgoaiGoi.FirstOrDefault(x => x.Index == i)?.Cell1Value.GetValueOrDefault();


                var cell2ValueDoanhThuNgay = cell2ValueBenhNhanTuDenTong.GetValueOrDefault() +
                                            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaSoSinh.FirstOrDefault(x => x.Index == i)?.Cell2Value.GetValueOrDefault() +
                                            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaPhatSinhNgoaiGoiKSKDoan.FirstOrDefault(x => x.Index == i)?.Cell2Value.GetValueOrDefault() +
                                            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaCBNV.FirstOrDefault(x => x.Index == i)?.Cell2Value.GetValueOrDefault() +
                                            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaCTV.FirstOrDefault(x => x.Index == i)?.Cell2Value.GetValueOrDefault() +
                                            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaVietDuc.FirstOrDefault(x => x.Index == i)?.Cell2Value.GetValueOrDefault() +
                                            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaThamMyBSTu.FirstOrDefault(x => x.Index == i)?.Cell2Value.GetValueOrDefault() +
                                            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaThamMyKhac.FirstOrDefault(x => x.Index == i)?.Cell2Value.GetValueOrDefault() +
                                            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaThaiSanTrongGoi.FirstOrDefault(x => x.Index == i)?.Cell2Value.GetValueOrDefault() +
                                            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaThaiSanNgoaiGoi.FirstOrDefault(x => x.Index == i)?.Cell2Value.GetValueOrDefault();

                baoCaoKetQuaKhamChuaBenhVaDoanhThuNgay.Add(new BaoCaoKetQuaColumnDoanhThuVo
                {
                    Index = i,
                    Cell1Value = cell1ValueDoanhThuNgay,
                    Cell2Value = cell2ValueDoanhThuNgay,
                });
            }

            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaKhamChuaBenhVaDoanhThuNgay = baoCaoKetQuaKhamChuaBenhVaDoanhThuNgay;
            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaBenhNhanTuDenTong = baoCaoKetQuaBenhNhanTuDenTong;

            //doanh thu ban thuoc
            var phieuThuChiPhiTaiNhaThuocData = phieuThuData.Where(o => o.LoaiNoiThu == LoaiNoiThu.NhaThuoc && o.LoaiThuTienBenhNhan == LoaiThuTienBenhNhan.ThuTheoChiPhi);
            foreach (var phieuThu in phieuThuChiPhiTaiNhaThuocData)
            {
                var ngayThuBaoCao = phieuThu.NgayThu.AddMilliseconds(khungGioMilliseconds * (-1));
                var baoCaoKetQuaNhaThuocTheoNgay = baoCaoKetQuaNhaThuocTheoNgays.FirstOrDefault(o => o.Ngay == ngayThuBaoCao.Date);
                if (baoCaoKetQuaNhaThuocTheoNgay != null)
                {
                    decimal doanhThuCong = phieuThu.SoTienThuTienMat.GetValueOrDefault() + phieuThu.SoTienThuChuyenKhoan.GetValueOrDefault() +
                    phieuThu.SoTienThuPos.GetValueOrDefault() + phieuThu.CongNoTuNhan.GetValueOrDefault() + phieuThu.CongNoCaNhan.GetValueOrDefault();
                    baoCaoKetQuaNhaThuocTheoNgay.DoanhThu += doanhThuCong;
                }
                //Doanh Thu am
                if (phieuThu.DaHuy == true && phieuThu.NgayHuy != null)
                {
                    var ngayHuyBaoCao = phieuThu.NgayHuy.Value.AddMilliseconds(khungGioMilliseconds * (-1));
                    var baoCaoKetQuaNhaThuocTheoNgayHuy = baoCaoKetQuaNhaThuocTheoNgays.FirstOrDefault(o => o.Ngay == ngayHuyBaoCao.Date);
                    if (baoCaoKetQuaNhaThuocTheoNgayHuy != null)
                    {
                        decimal doanhThuTru = phieuThu.SoTienThuTienMat.GetValueOrDefault() + phieuThu.SoTienThuChuyenKhoan.GetValueOrDefault() +
                            phieuThu.SoTienThuPos.GetValueOrDefault() + phieuThu.CongNoTuNhan.GetValueOrDefault() + phieuThu.CongNoCaNhan.GetValueOrDefault();
                        baoCaoKetQuaNhaThuocTheoNgayHuy.DoanhThu -= doanhThuTru;
                    }
                }
            }

            //dem so BN mua thuoc
            var phieuThuNhaThuocDataTheoTiepNhanGroup = phieuThuChiPhiTaiNhaThuocData.GroupBy(o => o.MaYeuCauTiepNhan).ToList();
            foreach (var phieuThuDataTheoTiepNhan in phieuThuNhaThuocDataTheoTiepNhanGroup)
            {
                var phieuThu = phieuThuDataTheoTiepNhan.OrderBy(o => o.NgayThu).First();
                var ngayThuBaoCao = phieuThu.NgayThu.AddMilliseconds(khungGioMilliseconds * (-1));
                var baoCaoKetQuaNhaThuocTheoNgay = baoCaoKetQuaNhaThuocTheoNgays.FirstOrDefault(o => o.Ngay == ngayThuBaoCao.Date);
                if (baoCaoKetQuaNhaThuocTheoNgay != null)
                {
                    baoCaoKetQuaNhaThuocTheoNgay.SoNguoi += 1;
                }
            }

            //--BanThuoc
            List<BaoCaoKetQuaColumnDoanhThuVo> baoCaoKetQuaDoanhThuBanThuoc = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            baoCaoKetQuaDoanhThuBanThuoc.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = 0,
                Cell1Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaNhaThuocTheoNgays.Where(o => ngayDauThangTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiThangTruocBaoCao).Select(o => o.DoanhThu).DefaultIfEmpty().Sum()),
                Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaNhaThuocTheoNgays.Where(o => ngayDauTuanTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiTuanTruocBaoCao).Select(o => o.DoanhThu).DefaultIfEmpty().Sum())
            });
            int iBanThuoc = 0;
            while (tuNgayBaoCao.AddDays(iBanThuoc) < denNgayBaoCao)
            {
                //----BanThuoc
                baoCaoKetQuaDoanhThuBanThuoc.Add(new BaoCaoKetQuaColumnDoanhThuVo
                {
                    Index = iBanThuoc + 1,
                    Cell1Value = baoCaoKetQuaNhaThuocTheoNgays.FirstOrDefault(o => tuNgayBaoCao.AddDays(iBanThuoc) == o.Ngay)?.SoNguoi,
                    Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaNhaThuocTheoNgays.FirstOrDefault(o => tuNgayBaoCao.AddDays(iBanThuoc) == o.Ngay)?.DoanhThu),
                });
                iBanThuoc++;
            }

            //----BanThuoc
            var tongSoNguoiBanThuoc = baoCaoKetQuaDoanhThuBanThuoc.Where(o => o.Index > 0).Sum(o => o.Cell1Value.GetValueOrDefault());
            var tongDoanhThuBanThuoc = baoCaoKetQuaDoanhThuBanThuoc.Where(o => o.Index > 0).Sum(o => o.Cell2Value.GetValueOrDefault());
            baoCaoKetQuaDoanhThuBanThuoc.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = iBanThuoc + 1,
                Cell1Value = tongSoNguoiBanThuoc,
                Cell2Value = tongDoanhThuBanThuoc
            });
            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaDoanhThuBanThuoc = baoCaoKetQuaDoanhThuBanThuoc;

            //Thuc thu
            var phieuThuChiPhiThucThuData = phieuThuData.Where(o => o.LoaiNoiThu == LoaiNoiThu.ThuNgan &&
            (o.LoaiThuTienBenhNhan == LoaiThuTienBenhNhan.ThuTheoChiPhi || o.LoaiThuTienBenhNhan == LoaiThuTienBenhNhan.ThuTamUng || o.LoaiThuTienBenhNhan == LoaiThuTienBenhNhan.ThuNo));
            foreach (var phieuThu in phieuThuChiPhiThucThuData)
            {
                var ngayThuBaoCao = phieuThu.NgayThu.AddMilliseconds(khungGioMilliseconds * (-1));
                var baoCaoKetQuaThucThuTheoNgay = baoCaoKetQuaThucThuTheoNgays.FirstOrDefault(o => o.Ngay == ngayThuBaoCao.Date);
                if (baoCaoKetQuaThucThuTheoNgay != null)
                {
                    baoCaoKetQuaThucThuTheoNgay.TienMat += phieuThu.TienMat.GetValueOrDefault() * (phieuThu.DaHuy == true ? -1 : 1);
                    baoCaoKetQuaThucThuTheoNgay.ChuyenKhoan += phieuThu.ChuyenKhoan.GetValueOrDefault() * (phieuThu.DaHuy == true ? -1 : 1);
                    baoCaoKetQuaThucThuTheoNgay.QuetThe += phieuThu.Pos.GetValueOrDefault() * (phieuThu.DaHuy == true ? -1 : 1);
                    baoCaoKetQuaThucThuTheoNgay.CongNoThuDuoc += (phieuThu.ThuNoTienMat.GetValueOrDefault() + phieuThu.ThuNoChuyenKhoan.GetValueOrDefault() + phieuThu.ThuNoPos.GetValueOrDefault()) * (phieuThu.DaHuy == true ? -1 : 1);
                }

                if (phieuThu.DaHuy == true && phieuThu.NgayHuy != null)
                {
                    var ngayHuyBaoCao = phieuThu.NgayHuy.Value.AddMilliseconds(khungGioMilliseconds * (-1));
                    var baoCaoKetQuaThucThuTheoNgayHuy = baoCaoKetQuaThucThuTheoNgays.FirstOrDefault(o => o.Ngay == ngayHuyBaoCao.Date);
                    if (baoCaoKetQuaThucThuTheoNgayHuy != null)
                    {
                        baoCaoKetQuaThucThuTheoNgayHuy.TienMat += phieuThu.TienMat.GetValueOrDefault();
                        baoCaoKetQuaThucThuTheoNgayHuy.ChuyenKhoan += phieuThu.ChuyenKhoan.GetValueOrDefault();
                        baoCaoKetQuaThucThuTheoNgayHuy.QuetThe += phieuThu.Pos.GetValueOrDefault();
                        baoCaoKetQuaThucThuTheoNgayHuy.CongNoThuDuoc += (phieuThu.ThuNoTienMat.GetValueOrDefault() + phieuThu.ThuNoChuyenKhoan.GetValueOrDefault() + phieuThu.ThuNoPos.GetValueOrDefault());
                    }
                }
            }
            foreach (var phieuThu in phieuChiData)
            {
                var ngayThuBaoCao = phieuThu.NgayThu.AddMilliseconds(khungGioMilliseconds * (-1));
                var baoCaoKetQuaThucThuTheoNgay = baoCaoKetQuaThucThuTheoNgays.FirstOrDefault(o => o.Ngay == ngayThuBaoCao.Date);
                if (baoCaoKetQuaThucThuTheoNgay != null)
                {
                    baoCaoKetQuaThucThuTheoNgay.TienMat += phieuThu.TienMat.GetValueOrDefault() * (phieuThu.DaHuy == true ? -1 : 1);
                    baoCaoKetQuaThucThuTheoNgay.ChuyenKhoan += phieuThu.ChuyenKhoan.GetValueOrDefault() * (phieuThu.DaHuy == true ? -1 : 1);
                    baoCaoKetQuaThucThuTheoNgay.QuetThe += phieuThu.Pos.GetValueOrDefault() * (phieuThu.DaHuy == true ? -1 : 1);
                    baoCaoKetQuaThucThuTheoNgay.CongNoThuDuoc += (phieuThu.ThuNoTienMat.GetValueOrDefault() + phieuThu.ThuNoChuyenKhoan.GetValueOrDefault() + phieuThu.ThuNoPos.GetValueOrDefault()) * (phieuThu.DaHuy == true ? -1 : 1);
                }

                if (phieuThu.DaHuy == true && phieuThu.NgayHuy != null)
                {
                    var ngayHuyBaoCao = phieuThu.NgayHuy.Value.AddMilliseconds(khungGioMilliseconds * (-1));
                    var baoCaoKetQuaThucThuTheoNgayHuy = baoCaoKetQuaThucThuTheoNgays.FirstOrDefault(o => o.Ngay == ngayHuyBaoCao.Date);
                    if (baoCaoKetQuaThucThuTheoNgayHuy != null)
                    {
                        baoCaoKetQuaThucThuTheoNgayHuy.TienMat += phieuThu.TienMat.GetValueOrDefault();
                        baoCaoKetQuaThucThuTheoNgayHuy.ChuyenKhoan += phieuThu.ChuyenKhoan.GetValueOrDefault();
                        baoCaoKetQuaThucThuTheoNgayHuy.QuetThe += phieuThu.Pos.GetValueOrDefault();
                        baoCaoKetQuaThucThuTheoNgayHuy.CongNoThuDuoc += (phieuThu.ThuNoTienMat.GetValueOrDefault() + phieuThu.ThuNoChuyenKhoan.GetValueOrDefault() + phieuThu.ThuNoPos.GetValueOrDefault());
                    }
                }
            }

            //--ThucThuTienMat
            List<BaoCaoKetQuaColumnDoanhThuVo> baoCaoKetQuaThucThuTienMat = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            baoCaoKetQuaThucThuTienMat.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = 0,
                Cell1Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaThucThuTheoNgays.Where(o => ngayDauThangTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiThangTruocBaoCao).Select(o => o.TienMat).DefaultIfEmpty().Sum()),
                Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaThucThuTheoNgays.Where(o => ngayDauTuanTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiTuanTruocBaoCao).Select(o => o.TienMat).DefaultIfEmpty().Sum())
            });
            //ThucThuChuyenKhoan
            List<BaoCaoKetQuaColumnDoanhThuVo> baoCaoKetQuaThucThuChuyenKhoan = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            baoCaoKetQuaThucThuChuyenKhoan.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = 0,
                Cell1Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaThucThuTheoNgays.Where(o => ngayDauThangTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiThangTruocBaoCao).Select(o => o.ChuyenKhoan).DefaultIfEmpty().Sum()),
                Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaThucThuTheoNgays.Where(o => ngayDauTuanTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiTuanTruocBaoCao).Select(o => o.ChuyenKhoan).DefaultIfEmpty().Sum())
            });
            //ThucThuQuetThe
            List<BaoCaoKetQuaColumnDoanhThuVo> baoCaoKetQuaThucThuQuetThe = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            baoCaoKetQuaThucThuQuetThe.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = 0,
                Cell1Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaThucThuTheoNgays.Where(o => ngayDauThangTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiThangTruocBaoCao).Select(o => o.QuetThe).DefaultIfEmpty().Sum()),
                Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaThucThuTheoNgays.Where(o => ngayDauTuanTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiTuanTruocBaoCao).Select(o => o.QuetThe).DefaultIfEmpty().Sum())
            });
            //CongNoThuDuoc
            List<BaoCaoKetQuaColumnDoanhThuVo> baoCaoKetQuaCongNoThuDuoc = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            baoCaoKetQuaCongNoThuDuoc.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = 0,
                Cell1Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaThucThuTheoNgays.Where(o => ngayDauThangTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiThangTruocBaoCao).Select(o => o.CongNoThuDuoc).DefaultIfEmpty().Sum()),
                Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaThucThuTheoNgays.Where(o => ngayDauTuanTruocBaoCao <= o.Ngay && o.Ngay <= ngayCuoiTuanTruocBaoCao).Select(o => o.CongNoThuDuoc).DefaultIfEmpty().Sum())
            });

            int iThucThu = 0;
            while (tuNgayBaoCao.AddDays(iThucThu) < denNgayBaoCao)
            {
                //----ThucThuTienMat
                baoCaoKetQuaThucThuTienMat.Add(new BaoCaoKetQuaColumnDoanhThuVo
                {
                    Index = iThucThu + 1,
                    Cell1Value = null,
                    Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaThucThuTheoNgays.FirstOrDefault(o => tuNgayBaoCao.AddDays(iThucThu) == o.Ngay)?.TienMat),
                });
                //ThucThuChuyenKhoan
                baoCaoKetQuaThucThuChuyenKhoan.Add(new BaoCaoKetQuaColumnDoanhThuVo
                {
                    Index = iThucThu + 1,
                    Cell1Value = null,
                    Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaThucThuTheoNgays.FirstOrDefault(o => tuNgayBaoCao.AddDays(iThucThu) == o.Ngay)?.ChuyenKhoan),
                });
                //ThucThuQuetThe
                baoCaoKetQuaThucThuQuetThe.Add(new BaoCaoKetQuaColumnDoanhThuVo
                {
                    Index = iThucThu + 1,
                    Cell1Value = null,
                    Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaThucThuTheoNgays.FirstOrDefault(o => tuNgayBaoCao.AddDays(iThucThu) == o.Ngay)?.QuetThe),
                });
                //CongNoThuDuoc
                baoCaoKetQuaCongNoThuDuoc.Add(new BaoCaoKetQuaColumnDoanhThuVo
                {
                    Index = iThucThu + 1,
                    Cell1Value = null,
                    Cell2Value = LamTronHienThi(1 / donViDoanhThu * baoCaoKetQuaThucThuTheoNgays.FirstOrDefault(o => tuNgayBaoCao.AddDays(iThucThu) == o.Ngay)?.CongNoThuDuoc),
                });

                iThucThu++;
            }

            //----ThucThuTienMat
            var tongThucThuTienMat = baoCaoKetQuaThucThuTienMat.Where(o => o.Index > 0).Sum(o => o.Cell2Value.GetValueOrDefault());
            baoCaoKetQuaThucThuTienMat.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = iThucThu + 1,
                Cell1Value = null,
                Cell2Value = tongThucThuTienMat
            });
            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaThucThuTienMat = baoCaoKetQuaThucThuTienMat;
            //ThucThuChuyenKhoan
            var tongThucThuChuyenKhoan = baoCaoKetQuaThucThuChuyenKhoan.Where(o => o.Index > 0).Sum(o => o.Cell2Value.GetValueOrDefault());
            baoCaoKetQuaThucThuChuyenKhoan.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = iThucThu + 1,
                Cell1Value = null,
                Cell2Value = tongThucThuChuyenKhoan
            });
            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaThucThuChuyenKhoan = baoCaoKetQuaThucThuChuyenKhoan;
            //ThucThuQuetThe
            var tongThucThuQuetThe = baoCaoKetQuaThucThuQuetThe.Where(o => o.Index > 0).Sum(o => o.Cell2Value.GetValueOrDefault());
            baoCaoKetQuaThucThuQuetThe.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = iThucThu + 1,
                Cell1Value = null,
                Cell2Value = tongThucThuQuetThe
            });
            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaThucThuQuetThe = baoCaoKetQuaThucThuQuetThe;
            //CongNoThuDuoc
            var tongCongNoThuDuoc = baoCaoKetQuaCongNoThuDuoc.Where(o => o.Index > 0).Sum(o => o.Cell2Value.GetValueOrDefault());
            baoCaoKetQuaCongNoThuDuoc.Add(new BaoCaoKetQuaColumnDoanhThuVo
            {
                Index = iThucThu + 1,
                Cell1Value = null,
                Cell2Value = tongCongNoThuDuoc
            });
            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaCongNoThuDuoc = baoCaoKetQuaCongNoThuDuoc;

            //tong Thuc Thu
            List<BaoCaoKetQuaColumnDoanhThuVo> baoCaoKetQuaThucThuTienMatVaThe = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            for (int i = 0; i <= lastIndex; i++)
            {
                decimal? cell1TongThucThu = null;
                if (i == 0)
                {
                    cell1TongThucThu = baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaThucThuTienMat.FirstOrDefault(x => x.Index == i)?.Cell1Value.GetValueOrDefault() +
                                                    baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaThucThuChuyenKhoan.FirstOrDefault(x => x.Index == i)?.Cell1Value.GetValueOrDefault() +
                                                    baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaThucThuQuetThe.FirstOrDefault(x => x.Index == i)?.Cell1Value.GetValueOrDefault() +
                                                    baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaCongNoThuDuoc.FirstOrDefault(x => x.Index == i)?.Cell1Value.GetValueOrDefault();
                }

                var cell2TongThucThu = baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaThucThuTienMat.FirstOrDefault(x => x.Index == i)?.Cell2Value.GetValueOrDefault() +
                                                    baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaThucThuChuyenKhoan.FirstOrDefault(x => x.Index == i)?.Cell2Value.GetValueOrDefault() +
                                                    baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaThucThuQuetThe.FirstOrDefault(x => x.Index == i)?.Cell2Value.GetValueOrDefault() +
                                                    baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaCongNoThuDuoc.FirstOrDefault(x => x.Index == i)?.Cell2Value.GetValueOrDefault();
                baoCaoKetQuaThucThuTienMatVaThe.Add(new BaoCaoKetQuaColumnDoanhThuVo
                {
                    Index = i,
                    Cell1Value = cell1TongThucThu,
                    Cell2Value = cell2TongThucThu,
                });
            }

            baoCaoKetQuaKhamChuaBenhKTVo.BaoCaoKetQuaThucThuTienMatVaThe = baoCaoKetQuaThucThuTienMatVaThe;

            return baoCaoKetQuaKhamChuaBenhKTVo;
        }

        public virtual byte[] ExportBaoCaoKetQuaKhamChuaBenhKT(BaoCaoKetQuaKhamChuaBenhKTVo datas, BaoCaoKetQuaKhamChuaBenhKTQueryInfoVo query)
        {
            int ind = 1;
            var baoCaoKetQuaColumnHeaderVoCounts = (datas.BaoCaoKetQuaColumnHeader.Count() * 2) + 4;

            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoKetQuaKhamChuaBenhKTVo>("TT", p => ind++)
            };

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("[KT] BÁO CÁO KẾT QUẢ KHÁM CHỮA BỆNH ");
                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 6;
                    for (int i = 2; i < baoCaoKetQuaColumnHeaderVoCounts; i++)
                    {
                        worksheet.Column(i).Width = 25;
                    }

                    worksheet.DefaultColWidth = 7;
                    worksheet.Row(8).Height = 24;

                    using (var range = worksheet.Cells["A1:B1"])
                    {
                        range.Worksheet.Cells["A1:B1"].Merge = true;
                        range.Worksheet.Cells["A1:B1"].Value = "BỆNH VIỆN ĐA KHOA QUỐC TẾ BẮC HÀ 2021 / TCKT";
                        range.Worksheet.Cells["A1:B1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:B1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:B1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:B1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:B1"].Style.Font.Bold = true;

                    }

                    using (var range = worksheet.Cells["A2:V2"])
                    {
                        range.Worksheet.Cells["A2:V2"].Merge = true;
                        range.Worksheet.Cells["A2:V2"].Value = "BÁO CÁO KẾT QUẢ KHÁM CHỮA BỆNH";
                        range.Worksheet.Cells["A2:V2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A2:V2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A2:V2"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A2:V2"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A2:V2"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A3:V3"])
                    {
                        range.Worksheet.Cells["A3:V3"].Merge = true;
                        range.Worksheet.Cells["A3:V3"].Value = "Từ ngày: " + query.TuNgay.FormatNgayTimKiemTrenBaoCao()
                                                          + " - đến ngày: " + query.DenNgay.FormatNgayTimKiemTrenBaoCao();

                        range.Worksheet.Cells["A3:V3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:V3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:V3"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A3:V3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:V3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["R4:S4"])
                    {
                        range.Worksheet.Cells["R4:S4"].Merge = true;
                        range.Worksheet.Cells["R4:S4"].Value = "ĐVT: Triệu đồng";
                        range.Worksheet.Cells["R4:S4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["R4:S4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["R4:S4"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["R4:S4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["R4:S4"].Style.Font.Bold = true;
                    }



                    string[] SetColumnItems = { "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

                    var setTitleColumnItems = new List<string>();
                    var setTitleColumnCoupleItems = new Dictionary<int, (string, string)>()
                    {
                        { 0, ("D", "E") },
                        { 1, ("F", "G") },
                        { 2, ("H", "I") },
                        { 3, ("J", "K") },
                        { 4, ("L", "M") },
                        { 5, ("N", "O") },
                        { 6, ("P", "Q") },
                        { 7, ("R", "S") },
                        { 8, ("T", "U") },
                        { 9, ("V", "W") },
                        { 10, ("X", "Y") },
                        { 11, ("Z", "AA") }
                    };

                    for (int i = 0; i < SetColumnItems.Length; i++)
                    {
                        if (i <= baoCaoKetQuaColumnHeaderVoCounts)
                        {
                            setTitleColumnItems.Add(SetColumnItems[i]);
                        }
                    }

                    var symbolFirst = setTitleColumnItems.FirstOrDefault();
                    var symbolLast = setTitleColumnItems.LastOrDefault();

                    int indexHeader6 = 6;
                    int indexHeader7 = 7;

                    using (var range = worksheet.Cells[symbolFirst + indexHeader6 + ":" + symbolLast + indexHeader6])
                    {
                        range.Worksheet.Cells[symbolFirst + indexHeader6 + ":" + symbolLast + indexHeader7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[symbolFirst + indexHeader6 + ":" + symbolLast + indexHeader7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[symbolFirst + indexHeader6 + ":" + symbolLast + indexHeader7].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells[symbolFirst + indexHeader6 + ":" + symbolLast + indexHeader7].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[symbolFirst + indexHeader6 + ":" + symbolLast + indexHeader7].Style.Font.Bold = true;

                        range.Worksheet.Cells["A" + indexHeader6 + ":A" + indexHeader7].Merge = true;
                        range.Worksheet.Cells["A" + indexHeader6 + ":A" + indexHeader7].Value = "TT";
                        range.Worksheet.Cells["A" + indexHeader6 + ":A" + indexHeader7].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + indexHeader6 + ":A" + indexHeader7].Style.Font.Bold = true;

                        range.Worksheet.Cells["A" + indexHeader6 + ":A" + indexHeader7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A" + indexHeader6 + ":A" + indexHeader7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A" + indexHeader6 + ":A" + indexHeader7].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["B" + indexHeader6 + ":C" + indexHeader7].Merge = true;
                        range.Worksheet.Cells["B" + indexHeader6 + ":C" + indexHeader7].Value = "Nội dung";
                        range.Worksheet.Cells["B" + indexHeader6 + ":C" + indexHeader7].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B" + indexHeader6 + ":C" + indexHeader7].Style.Font.Bold = true;
                        range.Worksheet.Cells["B" + indexHeader6 + ":C" + indexHeader7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["B" + indexHeader6 + ":C" + indexHeader7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["B" + indexHeader6 + ":C" + indexHeader7].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        foreach (var columnHeader in datas.BaoCaoKetQuaColumnHeader)
                        {
                            foreach (var setTitleColumn in setTitleColumnCoupleItems)
                            {
                                if (columnHeader.Index == setTitleColumn.Key)
                                {

                                    range.Worksheet.Cells[setTitleColumn.Value.Item1 + indexHeader6 + ":" + setTitleColumn.Value.Item2 + indexHeader6].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                    range.Worksheet.Cells[setTitleColumn.Value.Item1 + indexHeader6 + ":" + setTitleColumn.Value.Item2 + indexHeader6].Style.Font.Bold = true;
                                    range.Worksheet.Cells[setTitleColumn.Value.Item1 + indexHeader6 + ":" + setTitleColumn.Value.Item2 + indexHeader6].Merge = true;
                                    range.Worksheet.Cells[setTitleColumn.Value.Item1 + indexHeader6 + ":" + setTitleColumn.Value.Item2 + indexHeader6].Value = columnHeader.MergeCellText;
                                    range.Worksheet.Cells[setTitleColumn.Value.Item1 + indexHeader6 + ":" + setTitleColumn.Value.Item2 + indexHeader6].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                    range.Worksheet.Cells[setTitleColumn.Value.Item1 + indexHeader6 + ":" + setTitleColumn.Value.Item2 + indexHeader6].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                    range.Worksheet.Cells[setTitleColumn.Value.Item1 + indexHeader7 + ":" + setTitleColumn.Value.Item1 + indexHeader7].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                    range.Worksheet.Cells[setTitleColumn.Value.Item1 + indexHeader7 + ":" + setTitleColumn.Value.Item1 + indexHeader7].Merge = true;
                                    range.Worksheet.Cells[setTitleColumn.Value.Item1 + indexHeader7 + ":" + setTitleColumn.Value.Item1 + indexHeader7].Value = columnHeader.Cell1Text;
                                    range.Worksheet.Cells[setTitleColumn.Value.Item1 + indexHeader7 + ":" + setTitleColumn.Value.Item1 + indexHeader7].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                    range.Worksheet.Cells[setTitleColumn.Value.Item1 + indexHeader7 + ":" + setTitleColumn.Value.Item1 + indexHeader7].Style.Numberformat.Format = "#,##";
                                    range.Worksheet.Cells[setTitleColumn.Value.Item1 + indexHeader7 + ":" + setTitleColumn.Value.Item1 + indexHeader7].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                    range.Worksheet.Cells[setTitleColumn.Value.Item2 + indexHeader7 + ":" + setTitleColumn.Value.Item2 + indexHeader7].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                    range.Worksheet.Cells[setTitleColumn.Value.Item2 + indexHeader7 + ":" + setTitleColumn.Value.Item2 + indexHeader7].Merge = true;
                                    range.Worksheet.Cells[setTitleColumn.Value.Item2 + indexHeader7 + ":" + setTitleColumn.Value.Item2 + indexHeader7].Value = columnHeader.Cell2Text;
                                    range.Worksheet.Cells[setTitleColumn.Value.Item2 + indexHeader7 + ":" + setTitleColumn.Value.Item2 + indexHeader7].Style.Numberformat.Format = "#,##";
                                    range.Worksheet.Cells[setTitleColumn.Value.Item2 + indexHeader7 + ":" + setTitleColumn.Value.Item2 + indexHeader7].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                    range.Worksheet.Cells[setTitleColumn.Value.Item2 + indexHeader7 + ":" + setTitleColumn.Value.Item2 + indexHeader7].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                }
                            }
                        }
                    }

                    var manager = new PropertyManager<BaoCaoKetQuaKhamChuaBenhKTVo>(requestProperties);

                    int index = 8;

                    //I.Kết quả KCB và số tiền doanh thu theo ngày
                    using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                    {
                        range.Worksheet.Cells["A" + index + ":A" + index].Merge = true;
                        range.Worksheet.Cells["A" + index + ":A" + index].Value = "I";
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Font.Bold = true;

                        range.Worksheet.Cells["B" + index + ":C" + index].Merge = true;
                        range.Worksheet.Cells["B" + index + ":C" + index].Value = "Kết quả KCB và số tiền doanh thu theo ngày";
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Font.Bold = true;
                    }
                    if (datas.BaoCaoKetQuaColumnTongKhamSucKhoeDoan.Any())
                    {
                        foreach (var baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem in datas.BaoCaoKetQuaColumnTongKhamSucKhoeDoan)
                        {
                            foreach (var columnHeader in datas.BaoCaoKetQuaColumnHeader)
                            {
                                foreach (var setTitleColumn in setTitleColumnCoupleItems)
                                {
                                    if (baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem.Index == setTitleColumn.Key)
                                    {
                                        using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                                        {
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Value = string.Empty;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Font.Bold = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Value = string.Empty;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Font.Bold = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    index++;

                    //Khám sức khỏe đoàn
                    using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                    {
                        range.Worksheet.Cells["A" + index + ":A" + index].Value = "1";
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Font.Bold = true;

                        range.Worksheet.Cells["B" + index + ":C" + index].Merge = true;
                        range.Worksheet.Cells["B" + index + ":C" + index].Value = "Khám sức khỏe đoàn";
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Font.Bold = true;
                    }
                    if (datas.BaoCaoKetQuaColumnTongKhamSucKhoeDoan.Any())
                    {
                        foreach (var item in datas.BaoCaoKetQuaColumnTongKhamSucKhoeDoan)
                        {
                            foreach (var columnHeader in datas.BaoCaoKetQuaColumnHeader)
                            {
                                foreach (var setTitleColumn in setTitleColumnCoupleItems)
                                {
                                    if (item.Index == setTitleColumn.Key)
                                    {
                                        using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                                        {
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Value = item.Cell1Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Font.Bold = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Value = item.Cell2Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Font.Bold = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        }
                                    }
                                }
                            }

                        }
                    }
                    index++;

                    int columnTongKhamSucKhoeDoan = 0;
                    //Phần doanh thu sẽ tổng kết khi có quyết toán đoàn
                    var baoCaoKetQuaColumnTongKhamSucKhoeDoan = datas.BaoCaoKetQuaKhamSucKhoeDoans.Count();

                    if (baoCaoKetQuaColumnTongKhamSucKhoeDoan != 0) { columnTongKhamSucKhoeDoan = index + baoCaoKetQuaColumnTongKhamSucKhoeDoan - 1; } else { columnTongKhamSucKhoeDoan = index; };
                    ;
                    using (var range = worksheet.Cells["B" + index + ":C" + columnTongKhamSucKhoeDoan])
                    {
                        range.Worksheet.Cells["B" + index + ":C" + columnTongKhamSucKhoeDoan].Merge = true;
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B" + index + ":C" + columnTongKhamSucKhoeDoan].Value = "Phần doanh thu sẽ tổng kết khi có quyết toán đoàn";
                        range.Worksheet.Cells["B" + index + ":C" + columnTongKhamSucKhoeDoan].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["B" + index + ":C" + columnTongKhamSucKhoeDoan].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }

                    using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                    {
                        range.Worksheet.Cells["A" + index + ":A" + index].Merge = true;
                        range.Worksheet.Cells["A" + index + ":A" + index].Value = "";
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Font.Bold = true;
                    }

                    if (datas.BaoCaoKetQuaKhamSucKhoeDoans.Any())
                    {
                        foreach (var item in datas.BaoCaoKetQuaKhamSucKhoeDoans)
                        {
                            foreach (var columnKhamSucKhoeDoanVo in item.BaoCaoKetQuaColumnKhamSucKhoeDoanVos)
                            {
                                foreach (var setTitleColumn in setTitleColumnCoupleItems)
                                {
                                    if (columnKhamSucKhoeDoanVo.Index == setTitleColumn.Key)
                                    {
                                        using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                                        {
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Value = columnKhamSucKhoeDoanVo.Cell1Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Value = columnKhamSucKhoeDoanVo.Cell1Value != 0 ? columnKhamSucKhoeDoanVo.Cell2Value : string.Empty;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        }
                                    }
                                }
                            }
                            index++;
                        }
                    }


                    index = columnTongKhamSucKhoeDoan != 0 ? columnTongKhamSucKhoeDoan + 1 : index++;

                    //2.Kết quả khám chữa bệnh và doanh thu ngày
                    using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                    {
                        range.Worksheet.Cells["A" + index + ":A" + index].Merge = true;
                        range.Worksheet.Cells["A" + index + ":A" + index].Value = "2";
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Font.Bold = true;

                        range.Worksheet.Cells["B" + index + ":C" + index].Merge = true;
                        range.Worksheet.Cells["B" + index + ":C" + index].Value = "Kết quả khám chữa bệnh và doanh thu ngày";
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Font.Bold = true;
                    }
                    if (datas.BaoCaoKetQuaKhamChuaBenhVaDoanhThuNgay.Any())
                    {
                        foreach (var baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem in datas.BaoCaoKetQuaKhamChuaBenhVaDoanhThuNgay)
                        {
                            foreach (var columnHeader in datas.BaoCaoKetQuaColumnHeader)
                            {
                                foreach (var setTitleColumn in setTitleColumnCoupleItems)
                                {
                                    if (baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem.Index == setTitleColumn.Key)
                                    {
                                        using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                                        {
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Value = baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem.Cell1Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Font.Bold = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Value = baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem.Cell2Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Font.Bold = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        }
                                    }
                                }
                            }

                        }
                    }
                    index++;

                    //Trong đó
                    using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                    {
                        range.Worksheet.Cells["A" + index + ":A" + index].Merge = true;
                        range.Worksheet.Cells["A" + index + ":A" + index].Value = "";
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["B" + index + ":C" + index].Merge = true;
                        range.Worksheet.Cells["B" + index + ":C" + index].Value = "Trong đó:";
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }
                    if (datas.BaoCaoKetQuaKhamChuaBenhVaDoanhThuNgay.Any())
                    {
                        foreach (var baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem in datas.BaoCaoKetQuaKhamChuaBenhVaDoanhThuNgay)
                        {
                            foreach (var columnHeader in datas.BaoCaoKetQuaColumnHeader)
                            {
                                foreach (var setTitleColumn in setTitleColumnCoupleItems)
                                {
                                    if (baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem.Index == setTitleColumn.Key)
                                    {
                                        using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                                        {

                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Value = string.Empty;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Value = string.Empty;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        }
                                    }
                                }
                            }

                        }
                    }

                    index++;

                    //Bênh Nhân tự đến
                    int benhNhanTuDenItem7 = index + 6;
                    using (var range = worksheet.Cells["B" + index + ":B" + benhNhanTuDenItem7])
                    {
                        range.Worksheet.Cells["B" + index + ":B" + benhNhanTuDenItem7].Merge = true;
                        range.Worksheet.Cells["B" + index + ":B" + benhNhanTuDenItem7].Value = "+ Bệnh nhân tự đến";
                        range.Worksheet.Cells["B" + index + ":B" + benhNhanTuDenItem7].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B" + index + ":B" + benhNhanTuDenItem7].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["B" + index + ":B" + benhNhanTuDenItem7].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }

                    //Bênh Nhân tự đến=>Khách lẻ
                    using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                    {
                        range.Worksheet.Cells["A" + index + ":A" + index].Merge = true;
                        range.Worksheet.Cells["A" + index + ":A" + index].Value = "";
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["C" + index + ":C" + index].Merge = true;
                        range.Worksheet.Cells["C" + index + ":C" + index].Value = "Khách lẻ";
                        range.Worksheet.Cells["C" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["C" + index + ":C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["C" + index + ":C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }
                    if (datas.BaoCaoKetQuaBenhNhanTuDenKhachLe.Any())
                    {
                        foreach (var baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem in datas.BaoCaoKetQuaBenhNhanTuDenKhachLe)
                        {
                            foreach (var columnHeader in datas.BaoCaoKetQuaColumnHeader)
                            {
                                foreach (var setTitleColumn in setTitleColumnCoupleItems)
                                {
                                    if (baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem.Index == setTitleColumn.Key)
                                    {
                                        using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                                        {

                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Value = baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem.Cell1Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Value = baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem.Cell2Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        }
                                    }
                                }
                            }

                        }
                    }

                    index++;
                    //Bênh Nhân tự đến=>Nội Trú
                    using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                    {
                        range.Worksheet.Cells["A" + index + ":A" + index].Merge = true;
                        range.Worksheet.Cells["A" + index + ":A" + index].Value = "";
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["C" + index + ":C" + index].Merge = true;
                        range.Worksheet.Cells["C" + index + ":C" + index].Value = "Nội trú";
                        range.Worksheet.Cells["C" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["C" + index + ":C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["C" + index + ":C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }
                    if (datas.BaoCaoKetQuaBenhNhanTuDenNoiTru.Any())
                    {
                        foreach (var baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem in datas.BaoCaoKetQuaBenhNhanTuDenNoiTru)
                        {
                            foreach (var columnHeader in datas.BaoCaoKetQuaColumnHeader)
                            {
                                foreach (var setTitleColumn in setTitleColumnCoupleItems)
                                {
                                    if (baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem.Index == setTitleColumn.Key)
                                    {
                                        using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                                        {

                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Value = baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem.Cell1Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Value = baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem.Cell2Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        }
                                    }
                                }
                            }

                        }
                    }

                    index++;
                    //Bênh Nhân tự đến=> Nhi KCB
                    using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                    {
                        range.Worksheet.Cells["A" + index + ":A" + index].Merge = true;
                        range.Worksheet.Cells["A" + index + ":A" + index].Value = "";
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["C" + index + ":C" + index].Merge = true;
                        range.Worksheet.Cells["C" + index + ":C" + index].Value = "Nhi KCB";
                        range.Worksheet.Cells["C" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["C" + index + ":C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["C" + index + ":C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }
                    if (datas.BaoCaoKetQuaBenhNhanTuDenNhiKcb.Any())
                    {
                        foreach (var baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem in datas.BaoCaoKetQuaBenhNhanTuDenNhiKcb)
                        {
                            foreach (var columnHeader in datas.BaoCaoKetQuaColumnHeader)
                            {
                                foreach (var setTitleColumn in setTitleColumnCoupleItems)
                                {
                                    if (baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem.Index == setTitleColumn.Key)
                                    {
                                        using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                                        {

                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Value = baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem.Cell1Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Value = baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem.Cell2Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        }
                                    }
                                }
                            }

                        }
                    }

                    index++;
                    //Bênh Nhân tự đến=> Nhi KCB + tiêm chủng
                    using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                    {
                        range.Worksheet.Cells["A" + index + ":A" + index].Merge = true;
                        range.Worksheet.Cells["A" + index + ":A" + index].Value = "";
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["C" + index + ":C" + index].Merge = true;
                        range.Worksheet.Cells["C" + index + ":C" + index].Value = "Nhi KCB + Tiêm chủng";
                        range.Worksheet.Cells["C" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["C" + index + ":C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["C" + index + ":C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }
                    if (datas.BaoCaoKetQuaBenhNhanTuDenNhiKcbTiemChung.Any())
                    {
                        foreach (var baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem in datas.BaoCaoKetQuaBenhNhanTuDenNhiKcbTiemChung)
                        {
                            foreach (var columnHeader in datas.BaoCaoKetQuaColumnHeader)
                            {
                                foreach (var setTitleColumn in setTitleColumnCoupleItems)
                                {
                                    if (baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem.Index == setTitleColumn.Key)
                                    {
                                        using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                                        {

                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Value = baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem.Cell1Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Value = baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem.Cell2Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        }
                                    }
                                }
                            }

                        }
                    }

                    index++;
                    //Bênh Nhân tự đến=> Nhi KCB + Nhi tiêm chủng
                    using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                    {
                        range.Worksheet.Cells["A" + index + ":A" + index].Merge = true;
                        range.Worksheet.Cells["A" + index + ":A" + index].Value = "";
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["C" + index + ":C" + index].Merge = true;
                        range.Worksheet.Cells["C" + index + ":C" + index].Value = "Nhi tiêm chủng";
                        range.Worksheet.Cells["C" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["C" + index + ":C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["C" + index + ":C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }
                    if (datas.BaoCaoKetQuaBenhNhanTuDenNhiTiemChung.Any())
                    {
                        foreach (var baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem in datas.BaoCaoKetQuaBenhNhanTuDenNhiTiemChung)
                        {
                            foreach (var columnHeader in datas.BaoCaoKetQuaColumnHeader)
                            {
                                foreach (var setTitleColumn in setTitleColumnCoupleItems)
                                {
                                    if (baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem.Index == setTitleColumn.Key)
                                    {
                                        using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                                        {

                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Value = baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem.Cell1Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Value = baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem.Cell2Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        }
                                    }
                                }
                            }

                        }
                    }

                    index++;
                    //Bênh Nhân tự đến=> Nhi KCB + Nhi nội trú
                    using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                    {
                        range.Worksheet.Cells["A" + index + ":A" + index].Merge = true;
                        range.Worksheet.Cells["A" + index + ":A" + index].Value = "";
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["C" + index + ":C" + index].Merge = true;
                        range.Worksheet.Cells["C" + index + ":C" + index].Value = "Nhi nội trú";
                        range.Worksheet.Cells["C" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["C" + index + ":C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["C" + index + ":C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }
                    if (datas.BaoCaoKetQuaBenhNhanTuDenNhiNoiTRu.Any())
                    {
                        foreach (var baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem in datas.BaoCaoKetQuaBenhNhanTuDenNhiNoiTRu)
                        {
                            foreach (var columnHeader in datas.BaoCaoKetQuaColumnHeader)
                            {
                                foreach (var setTitleColumn in setTitleColumnCoupleItems)
                                {
                                    if (baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem.Index == setTitleColumn.Key)
                                    {
                                        using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                                        {

                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Value = baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem.Cell1Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Value = baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem.Cell2Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        }
                                    }
                                }
                            }

                        }
                    }

                    index++;
                    //Bênh Nhân tự đến=> Nhi KCB + Tổng tự đến
                    using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                    {
                        range.Worksheet.Cells["A" + index + ":A" + index].Merge = true;
                        range.Worksheet.Cells["A" + index + ":A" + index].Value = "";
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["C" + index + ":C" + index].Merge = true;
                        range.Worksheet.Cells["C" + index + ":C" + index].Value = "Tổng tự đến";
                        range.Worksheet.Cells["C" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["C" + index + ":C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["C" + index + ":C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C" + index + ":C" + index].Style.Font.Bold = true;
                    }
                    if (datas.BaoCaoKetQuaBenhNhanTuDenTong.Any())
                    {
                        foreach (var baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem in datas.BaoCaoKetQuaBenhNhanTuDenTong)
                        {
                            foreach (var columnHeader in datas.BaoCaoKetQuaColumnHeader)
                            {
                                foreach (var setTitleColumn in setTitleColumnCoupleItems)
                                {
                                    if (baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem.Index == setTitleColumn.Key)
                                    {
                                        using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                                        {

                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Value = baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem.Cell1Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Font.Bold = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Value = baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem.Cell2Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Font.Bold = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        }
                                    }
                                }
                            }

                        }
                    }

                    index++;
                    //+ Sơ sinh
                    using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                    {
                        range.Worksheet.Cells["A" + index + ":A" + index].Merge = true;
                        range.Worksheet.Cells["A" + index + ":A" + index].Value = "";
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["B" + index + ":C" + index].Merge = true;
                        range.Worksheet.Cells["B" + index + ":C" + index].Value = "+ Sơ sinh";
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }
                    if (datas.BaoCaoKetQuaSoSinh.Any())
                    {
                        foreach (var baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem in datas.BaoCaoKetQuaSoSinh)
                        {
                            foreach (var columnHeader in datas.BaoCaoKetQuaColumnHeader)
                            {
                                foreach (var setTitleColumn in setTitleColumnCoupleItems)
                                {
                                    if (baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem.Index == setTitleColumn.Key)
                                    {
                                        using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                                        {

                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Value = baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem.Cell1Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Value = baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem.Cell2Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        }
                                    }
                                }
                            }

                        }
                    }

                    index++;
                    //+ Phát sinh ngoài gói KSK đoàn
                    using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                    {
                        range.Worksheet.Cells["A" + index + ":A" + index].Merge = true;
                        range.Worksheet.Cells["A" + index + ":A" + index].Value = "";
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["B" + index + ":C" + index].Merge = true;
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B" + index + ":C" + index].Value = "+ Phát sinh ngoài gói KSK đoàn";
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }
                    if (datas.BaoCaoKetQuaPhatSinhNgoaiGoiKSKDoan.Any())
                    {
                        foreach (var baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem in datas.BaoCaoKetQuaPhatSinhNgoaiGoiKSKDoan)
                        {
                            foreach (var columnHeader in datas.BaoCaoKetQuaColumnHeader)
                            {
                                foreach (var setTitleColumn in setTitleColumnCoupleItems)
                                {
                                    if (baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem.Index == setTitleColumn.Key)
                                    {
                                        using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                                        {

                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Value = baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem.Cell1Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Value = baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem.Cell2Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        }
                                    }
                                }
                            }

                        }
                    }

                    index++;
                    //+ CBNV
                    using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                    {
                        range.Worksheet.Cells["A" + index + ":A" + index].Merge = true;
                        range.Worksheet.Cells["A" + index + ":A" + index].Value = "";
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["B" + index + ":C" + index].Merge = true;
                        range.Worksheet.Cells["B" + index + ":C" + index].Value = "+ CBNV";
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }
                    if (datas.BaoCaoKetQuaCBNV.Any())
                    {
                        foreach (var baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem in datas.BaoCaoKetQuaCBNV)
                        {
                            foreach (var columnHeader in datas.BaoCaoKetQuaColumnHeader)
                            {
                                foreach (var setTitleColumn in setTitleColumnCoupleItems)
                                {
                                    if (baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem.Index == setTitleColumn.Key)
                                    {
                                        using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                                        {

                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Value = baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem.Cell1Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Value = baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem.Cell2Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        }
                                    }
                                }
                            }

                        }
                    }

                    index++;
                    //+ CTV
                    using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                    {
                        range.Worksheet.Cells["A" + index + ":A" + index].Merge = true;
                        range.Worksheet.Cells["A" + index + ":A" + index].Value = "";
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["B" + index + ":C" + index].Merge = true;
                        range.Worksheet.Cells["B" + index + ":C" + index].Value = "+ CTV";
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }
                    if (datas.BaoCaoKetQuaCTV.Any())
                    {
                        foreach (var baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem in datas.BaoCaoKetQuaCTV)
                        {
                            foreach (var columnHeader in datas.BaoCaoKetQuaColumnHeader)
                            {
                                foreach (var setTitleColumn in setTitleColumnCoupleItems)
                                {
                                    if (baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem.Index == setTitleColumn.Key)
                                    {
                                        using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                                        {

                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Value = baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem.Cell1Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Value = baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem.Cell2Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        }
                                    }
                                }
                            }

                        }
                    }

                    index++;
                    //+ Việt Đức
                    using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                    {
                        range.Worksheet.Cells["A" + index + ":A" + index].Merge = true;
                        range.Worksheet.Cells["A" + index + ":A" + index].Value = "";
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["B" + index + ":C" + index].Merge = true;
                        range.Worksheet.Cells["B" + index + ":C" + index].Value = "+ Việt Đức";
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }
                    if (datas.BaoCaoKetQuaVietDuc.Any())
                    {
                        foreach (var item in datas.BaoCaoKetQuaVietDuc)
                        {
                            foreach (var columnHeader in datas.BaoCaoKetQuaColumnHeader)
                            {
                                foreach (var setTitleColumn in setTitleColumnCoupleItems)
                                {
                                    if (item.Index == setTitleColumn.Key)
                                    {
                                        using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                                        {

                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Value = item.Cell1Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Value = item.Cell2Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        }
                                    }
                                }
                            }

                        }
                    }

                    index++;
                    //+ Thẩm mỹ BS Tú (chỉ có doanh thu CLS)
                    using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                    {
                        range.Worksheet.Cells["A" + index + ":A" + index].Merge = true;
                        range.Worksheet.Cells["A" + index + ":A" + index].Value = "";
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["B" + index + ":C" + index].Merge = true;
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B" + index + ":C" + index].Value = "+ Thẩm mỹ BS Tú (chỉ có doanh thu CLS)";
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }
                    if (datas.BaoCaoKetQuaThamMyBSTu.Any())
                    {
                        foreach (var item in datas.BaoCaoKetQuaThamMyBSTu)
                        {
                            foreach (var columnHeader in datas.BaoCaoKetQuaColumnHeader)
                            {
                                foreach (var setTitleColumn in setTitleColumnCoupleItems)
                                {
                                    if (item.Index == setTitleColumn.Key)
                                    {
                                        using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                                        {

                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Value = item.Cell1Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Value = item.Cell2Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        }
                                    }
                                }
                            }

                        }
                    }

                    index++;
                    //+ Thẩm mỹ khác (BS Đoàn, Huân, Dương, Trực,…)
                    using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                    {
                        range.Worksheet.Cells["A" + index + ":A" + index].Merge = true;
                        range.Worksheet.Cells["A" + index + ":A" + index].Value = "";
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["B" + index + ":C" + index].Merge = true;
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B" + index + ":C" + index].Value = "+ Thẩm mỹ khác (BS Đoàn, Huân, Dương, Trực,…)";
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }
                    if (datas.BaoCaoKetQuaThamMyKhac.Any())
                    {
                        foreach (var item in datas.BaoCaoKetQuaThamMyKhac)
                        {
                            foreach (var columnHeader in datas.BaoCaoKetQuaColumnHeader)
                            {
                                foreach (var setTitleColumn in setTitleColumnCoupleItems)
                                {
                                    if (item.Index == setTitleColumn.Key)
                                    {
                                        using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                                        {

                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Value = item.Cell1Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Value = item.Cell2Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        }
                                    }
                                }
                            }

                        }
                    }


                    //Thai sản
                    index++;
                    int thaiSanItem = index + 1;
                    using (var range = worksheet.Cells["B" + index + ":B" + thaiSanItem])
                    {
                        range.Worksheet.Cells["B" + index + ":B" + thaiSanItem].Merge = true;
                        range.Worksheet.Cells["B" + index + ":B" + thaiSanItem].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B" + index + ":B" + thaiSanItem].Value = "+ Thai sản";
                        range.Worksheet.Cells["B" + index + ":B" + thaiSanItem].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["B" + index + ":B" + thaiSanItem].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }

                    //Thai sản=>Trong gói
                    using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                    {
                        range.Worksheet.Cells["A" + index + ":A" + index].Merge = true;
                        range.Worksheet.Cells["A" + index + ":A" + index].Value = "";
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["C" + index + ":C" + index].Merge = true;
                        range.Worksheet.Cells["C" + index + ":C" + index].Value = "Trong gói";
                        range.Worksheet.Cells["C" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["C" + index + ":C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["C" + index + ":C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }
                    if (datas.BaoCaoKetQuaThaiSanTrongGoi.Any())
                    {
                        foreach (var item in datas.BaoCaoKetQuaThaiSanTrongGoi)
                        {
                            foreach (var columnHeader in datas.BaoCaoKetQuaColumnHeader)
                            {
                                foreach (var setTitleColumn in setTitleColumnCoupleItems)
                                {
                                    if (item.Index == setTitleColumn.Key)
                                    {
                                        using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                                        {

                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Value = item.Cell1Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Value = item.Cell2Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        }
                                    }
                                }
                            }

                        }
                    }

                    index++;
                    //Thai sản=>Ngoài gói
                    using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                    {
                        range.Worksheet.Cells["A" + index + ":A" + index].Merge = true;
                        range.Worksheet.Cells["A" + index + ":A" + index].Value = "";
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["C" + index + ":C" + index].Merge = true;
                        range.Worksheet.Cells["C" + index + ":C" + index].Value = "Ngoài gói";
                        range.Worksheet.Cells["C" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["C" + index + ":C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["C" + index + ":C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }
                    if (datas.BaoCaoKetQuaThaiSanNgoaiGoi.Any())
                    {
                        foreach (var item in datas.BaoCaoKetQuaThaiSanNgoaiGoi)
                        {
                            foreach (var columnHeader in datas.BaoCaoKetQuaColumnHeader)
                            {
                                foreach (var setTitleColumn in setTitleColumnCoupleItems)
                                {
                                    if (item.Index == setTitleColumn.Key)
                                    {
                                        using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                                        {

                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Value = item.Cell1Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Value = item.Cell2Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        }
                                    }
                                }
                            }

                        }
                    }

                    index++;
                    //Răng hàm mặt
                    using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                    {
                        range.Worksheet.Cells["A" + index + ":A" + index].Merge = true;
                        range.Worksheet.Cells["A" + index + ":A" + index].Value = "";
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["B" + index + ":C" + index].Merge = true;
                        range.Worksheet.Cells["B" + index + ":C" + index].Value = "*Răng hàm mặt";
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }
                    if (datas.BaoCaoKetQuaRangHamMat.Any())
                    {
                        foreach (var item in datas.BaoCaoKetQuaRangHamMat)
                        {
                            foreach (var columnHeader in datas.BaoCaoKetQuaColumnHeader)
                            {
                                foreach (var setTitleColumn in setTitleColumnCoupleItems)
                                {
                                    if (item.Index == setTitleColumn.Key)
                                    {
                                        using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                                        {

                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Value = item.Cell1Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Value = item.Cell2Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        }
                                    }
                                }
                            }

                        }
                    }

                    index++;
                    //Doanh thu bán thuốc theo ngày
                    using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                    {
                        range.Worksheet.Cells["A" + index + ":A" + index].Merge = true;
                        range.Worksheet.Cells["A" + index + ":A" + index].Value = "3";
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Font.Bold = true;

                        range.Worksheet.Cells["B" + index + ":C" + index].Merge = true;
                        range.Worksheet.Cells["B" + index + ":C" + index].Value = "Doanh thu bán thuốc theo ngày";
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Font.Bold = true;
                    }
                    if (datas.BaoCaoKetQuaDoanhThuBanThuoc.Any())
                    {
                        foreach (var item in datas.BaoCaoKetQuaDoanhThuBanThuoc)
                        {
                            foreach (var columnHeader in datas.BaoCaoKetQuaColumnHeader)
                            {
                                foreach (var setTitleColumn in setTitleColumnCoupleItems)
                                {
                                    if (item.Index == setTitleColumn.Key)
                                    {
                                        using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                                        {

                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Value = item.Cell1Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Font.Bold = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Value = item.Cell2Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Font.Bold = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        }
                                    }
                                }
                            }

                        }
                    }

                    index++;
                    //Trong đó thực thu tiền mặt và thẻ:
                    using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                    {
                        range.Worksheet.Cells["A" + index + ":A" + index].Merge = true;
                        range.Worksheet.Cells["A" + index + ":A" + index].Value = "II";
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Font.Bold = true;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["B" + index + ":C" + index].Merge = true;
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B" + index + ":C" + index].Value = "Trong đó thực thu tiền mặt và thẻ:";
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Font.Bold = true;
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }
                    if (datas.BaoCaoKetQuaThucThuTienMatVaThe.Any())
                    {
                        foreach (var item in datas.BaoCaoKetQuaThucThuTienMatVaThe)
                        {
                            foreach (var columnHeader in datas.BaoCaoKetQuaColumnHeader)
                            {
                                foreach (var setTitleColumn in setTitleColumnCoupleItems)
                                {
                                    if (item.Index == setTitleColumn.Key)
                                    {
                                        using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                                        {

                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Value = item.Cell1Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Font.Bold = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Value = item.Cell2Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Font.Bold = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        }
                                    }
                                }
                            }

                        }
                    }


                    index++;
                    //Thực thu tiền mặt (đã trừ số tiền hoàn trả cho khách)
                    using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                    {
                        range.Worksheet.Cells["A" + index + ":A" + index].Merge = true;
                        range.Worksheet.Cells["A" + index + ":A" + index].Value = "1";
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["B" + index + ":C" + index].Merge = true;
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B" + index + ":C" + index].Value = "Thực thu tiền mặt (đã trừ số tiền hoàn trả cho khách)";
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }
                    if (datas.BaoCaoKetQuaThucThuTienMat.Any())
                    {
                        foreach (var item in datas.BaoCaoKetQuaThucThuTienMat)
                        {
                            foreach (var columnHeader in datas.BaoCaoKetQuaColumnHeader)
                            {
                                foreach (var setTitleColumn in setTitleColumnCoupleItems)
                                {
                                    if (item.Index == setTitleColumn.Key)
                                    {
                                        using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                                        {

                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Value = item.Cell1Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Value = item.Cell2Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        }
                                    }
                                }
                            }

                        }
                    }

                    index++;
                    //Thực thu Chuyển khoản
                    using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                    {
                        range.Worksheet.Cells["A" + index + ":A" + index].Merge = true;
                        range.Worksheet.Cells["A" + index + ":A" + index].Value = "2";
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["B" + index + ":C" + index].Merge = true;
                        range.Worksheet.Cells["B" + index + ":C" + index].Value = "Thực thu Chuyển khoản";
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }
                    if (datas.BaoCaoKetQuaThucThuChuyenKhoan.Any())
                    {
                        foreach (var item in datas.BaoCaoKetQuaThucThuChuyenKhoan)
                        {
                            foreach (var columnHeader in datas.BaoCaoKetQuaColumnHeader)
                            {
                                foreach (var setTitleColumn in setTitleColumnCoupleItems)
                                {
                                    if (item.Index == setTitleColumn.Key)
                                    {
                                        using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                                        {

                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Value = item.Cell1Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Value = item.Cell2Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        }
                                    }
                                }
                            }

                        }
                    }


                    index++;
                    //Thực thu quẹt thẻ
                    using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                    {
                        range.Worksheet.Cells["A" + index + ":A" + index].Merge = true;
                        range.Worksheet.Cells["A" + index + ":A" + index].Value = "3";
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["B" + index + ":C" + index].Merge = true;
                        range.Worksheet.Cells["B" + index + ":C" + index].Value = "Thực thu quẹt thẻ";
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }
                    if (datas.BaoCaoKetQuaThucThuQuetThe.Any())
                    {
                        foreach (var item in datas.BaoCaoKetQuaThucThuQuetThe)
                        {
                            foreach (var columnHeader in datas.BaoCaoKetQuaColumnHeader)
                            {
                                foreach (var setTitleColumn in setTitleColumnCoupleItems)
                                {
                                    if (item.Index == setTitleColumn.Key)
                                    {
                                        using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                                        {

                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Value = item.Cell1Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Value = item.Cell2Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        }
                                    }
                                }
                            }

                        }
                    }

                    index++;
                    //Công nợ thu được trong ngày
                    using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                    {
                        range.Worksheet.Cells["A" + index + ":A" + index].Merge = true;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":A" + index].Value = "4";
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A" + index + ":A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["B" + index + ":C" + index].Merge = true;
                        range.Worksheet.Cells["B" + index + ":C" + index].Value = "Công nợ thu được trong ngày";
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["B" + index + ":C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }
                    if (datas.BaoCaoKetQuaCongNoThuDuoc.Any())
                    {
                        foreach (var item in datas.BaoCaoKetQuaCongNoThuDuoc)
                        {
                            foreach (var columnHeader in datas.BaoCaoKetQuaColumnHeader)
                            {
                                foreach (var setTitleColumn in setTitleColumnCoupleItems)
                                {
                                    if (item.Index == setTitleColumn.Key)
                                    {
                                        using (var range = worksheet.Cells[symbolFirst + index + ":" + symbolLast + index])
                                        {

                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Value = item.Cell1Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item1 + index + ":" + setTitleColumn.Value.Item1 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Merge = true;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Numberformat.Format = "#,##";
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Value = item.Cell2Value;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            range.Worksheet.Cells[setTitleColumn.Value.Item2 + index + ":" + setTitleColumn.Value.Item2 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        }
                                    }
                                }
                            }

                        }
                    }

                    xlPackage.Save();
                }
                return stream.ToArray();

            }
        }

        public string GetHTMLBaoCaoKetQuaKhamChuaBenhKT(BaoCaoKetQuaKhamChuaBenhKTVo datas, BaoCaoKetQuaKhamChuaBenhKTQueryInfoVo query)
        {

            var table = $"<table  class='boder-table-main'>" +
                 "<tr>" +
                 "<th rowspan='2' class='boder-table '>TT</th>" +
                 "<th colspan ='2' rowspan='2' class='boder-table '>Nội dung</th>";

            var thuNgay = string.Empty;
            var setTitleColumnCoupleItems = new Dictionary<int, (string, string)>()
                    {
                        { 0, ("D", "E") },
                        { 1, ("F", "G") },
                        { 2, ("H", "I") },
                        { 3, ("J", "K") },
                        { 4, ("L", "M") },
                        { 5, ("N", "O") },
                        { 6, ("P", "Q") },
                        { 7, ("R", "S") },
                        { 8, ("T", "U") },
                        { 9, ("V", "W") },
                        { 10, ("X", "Y") },
                        { 11, ("Z", "AA") }
                    };

            foreach (var columnHeader in datas.BaoCaoKetQuaColumnHeader)
            {
                foreach (var setTitleColumn in setTitleColumnCoupleItems)
                {
                    if (columnHeader.Index == setTitleColumn.Key)
                    {

                        thuNgay += $"<th colspan='2' class='boder-table'>{columnHeader.MergeCellText}</th>";

                    }
                }
            }
            table = table + thuNgay;

            var tableRow2 = "<tr>";
            var valueRow2 = string.Empty;

            foreach (var columnHeader in datas.BaoCaoKetQuaColumnHeader)
            {
                foreach (var setTitleColumn in setTitleColumnCoupleItems)
                {
                    if (columnHeader.Index == setTitleColumn.Key)
                    {

                        valueRow2 += $"<th class='boder-table'>{columnHeader.Cell1Text}</th>";
                        valueRow2 += $"<th class='boder-table'>{columnHeader.Cell2Text}</th>";
                    }
                }
            }

            tableRow2 += valueRow2;
            table += tableRow2;

            //I.Kết quả KCB và số tiền doanh thu theo ngày
            var kqKCBVaDoanhThuTheoNgay = string.Empty;
            kqKCBVaDoanhThuTheoNgay = "<tr><td class='boder-table-left'>I</td><td  class='boder-table-left' colspan='2'>Kết quả KCB và số tiền doanh thu theo ngày</td>";
            var valueKQKCBVaDoanhThuTheoNgayRow = string.Empty;
            if (datas.BaoCaoKetQuaColumnTongKhamSucKhoeDoan.Any())
            {
                foreach (var baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem in datas.BaoCaoKetQuaColumnTongKhamSucKhoeDoan)
                {

                    foreach (var setTitleColumn in setTitleColumnCoupleItems)
                    {
                        if (baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem.Index == setTitleColumn.Key)
                        {

                            valueKQKCBVaDoanhThuTheoNgayRow += $"<td class='boder-table'></td>";
                            valueKQKCBVaDoanhThuTheoNgayRow += $"<td class='boder-table'></td>";
                        }
                    }
                }
            }
            kqKCBVaDoanhThuTheoNgay += valueKQKCBVaDoanhThuTheoNgayRow + "</tr>";

            //Khám sức khỏe đoàn
            var khamSucKhoeDoan = string.Empty;
            khamSucKhoeDoan = "<tr><td class='boder-table-left'>1</td><td  class='boder-table-left' colspan='2'>Khám sức khỏe đoàn</td>";
            var valueKhamSucKhoeDoanRow = string.Empty;
            if (datas.BaoCaoKetQuaColumnTongKhamSucKhoeDoan.Any())
            {
                foreach (var khamSucKhoeItem in datas.BaoCaoKetQuaColumnTongKhamSucKhoeDoan)
                {

                    foreach (var setTitleColumn in setTitleColumnCoupleItems)
                    {
                        if (khamSucKhoeItem.Index == setTitleColumn.Key)
                        {
                            var itemCell1 = khamSucKhoeItem.Cell1Value != 0 ? khamSucKhoeItem.Cell1Value : (decimal?)null;
                            var itemCell2 = khamSucKhoeItem.Cell1Value != 0 ? khamSucKhoeItem.Cell2Value : string.Empty;

                            valueKhamSucKhoeDoanRow += $"<td class='boder-table'>{itemCell1}</td>";
                            valueKhamSucKhoeDoanRow += $"<td class='boder-table'>{itemCell2}</td>";
                        }
                    }
                }
            }
            khamSucKhoeDoan += valueKhamSucKhoeDoanRow + "</tr>";


            //Phần doanh thu sẽ tổng kết khi có quyết toán đoàn
            var baoCaoKetQuaColumnTongKhamSucKhoeDoan = datas.BaoCaoKetQuaKhamSucKhoeDoans.Count();
            var phanDoanhThuTongKetKhiCoQTDoan = string.Empty;
         
            if (baoCaoKetQuaColumnTongKhamSucKhoeDoan > 1)
            {
                phanDoanhThuTongKetKhiCoQTDoan = $"<tr><td rowspan='{baoCaoKetQuaColumnTongKhamSucKhoeDoan + 1 }' class='boder-table-left'></td><td colspan='2' rowspan='{baoCaoKetQuaColumnTongKhamSucKhoeDoan + 1}'  class='boder-table-left'>Phần doanh thu sẽ tổng kết khi có quyết toán đoàn</td>";

                var valuephanDoanhThuTongKetKhiCoQTDoanRow = string.Empty;
                if (datas.BaoCaoKetQuaKhamSucKhoeDoans.Any())
                {
                    foreach (var item in datas.BaoCaoKetQuaKhamSucKhoeDoans)
                    {
                        valuephanDoanhThuTongKetKhiCoQTDoanRow = $"<tr>";

                        foreach (var columnKhamSucKhoeDoanVo in item.BaoCaoKetQuaColumnKhamSucKhoeDoanVos)
                        {
                            foreach (var setTitleColumn in setTitleColumnCoupleItems)
                            {
                                if (columnKhamSucKhoeDoanVo.Index == setTitleColumn.Key)
                                {
                                    var itemCell1 = columnKhamSucKhoeDoanVo.Cell1Value != 0 ? columnKhamSucKhoeDoanVo.Cell1Value : (decimal?)null;
                                    var itemCell2 = columnKhamSucKhoeDoanVo.Cell1Value != 0 ? columnKhamSucKhoeDoanVo.Cell2Value : string.Empty;

                                    valuephanDoanhThuTongKetKhiCoQTDoanRow += $"<td class='boder-table-center'>{itemCell1}</td>";
                                    valuephanDoanhThuTongKetKhiCoQTDoanRow += $"<td class='boder-table-center'>{itemCell2}</td>";

                                }
                            }
                        }
                        valuephanDoanhThuTongKetKhiCoQTDoanRow += "</tr>";
                        phanDoanhThuTongKetKhiCoQTDoan += valuephanDoanhThuTongKetKhiCoQTDoanRow;
                    }
                }
            }
            else
            {
                phanDoanhThuTongKetKhiCoQTDoan = $"<tr><td rowspan='{baoCaoKetQuaColumnTongKhamSucKhoeDoan}' class='boder-table-left'></td><td colspan='2' rowspan='{baoCaoKetQuaColumnTongKhamSucKhoeDoan}'  class='boder-table-left'>Phần doanh thu sẽ tổng kết khi có quyết toán đoàn</td>";

                var valuephanDoanhThuTongKetKhiCoQTDoanRow = string.Empty;
                if (datas.BaoCaoKetQuaKhamSucKhoeDoans.Any())
                {
                    foreach (var item in datas.BaoCaoKetQuaKhamSucKhoeDoans)
                    {
                        foreach (var columnKhamSucKhoeDoanVo in item.BaoCaoKetQuaColumnKhamSucKhoeDoanVos)
                        {
                            foreach (var setTitleColumn in setTitleColumnCoupleItems)
                            {
                                if (columnKhamSucKhoeDoanVo.Index == setTitleColumn.Key)
                                {
                                    var itemCell1 = columnKhamSucKhoeDoanVo.Cell1Value != 0 ? columnKhamSucKhoeDoanVo.Cell1Value : (decimal?)null;
                                    var itemCell2 = columnKhamSucKhoeDoanVo.Cell1Value != 0 ? columnKhamSucKhoeDoanVo.Cell2Value : string.Empty;

                                    valuephanDoanhThuTongKetKhiCoQTDoanRow += $"<td class='boder-table-center'>{itemCell1}</td>";
                                    valuephanDoanhThuTongKetKhiCoQTDoanRow += $"<td class='boder-table-center'>{itemCell2}</td>";

                                }
                            }
                        }
                    }
                    phanDoanhThuTongKetKhiCoQTDoan += valuephanDoanhThuTongKetKhiCoQTDoanRow + "</tr>";
                }

            }

            //2.Kết quả khám chữa bệnh và doanh thu ngày
            var ketQuaKhamChuaBenhVaDoanhThu = string.Empty;
            ketQuaKhamChuaBenhVaDoanhThu = "<tr><td class='boder-table-left'>2</td><td class='boder-table-left' colspan='2'>Kết quả khám chữa bệnh và doanh thu ngày</td>";
            var valueKetQuaKhamChuaBenhVaDoanhThuRow = string.Empty;

            if (datas.BaoCaoKetQuaKhamChuaBenhVaDoanhThuNgay.Any())
            {
                foreach (var item in datas.BaoCaoKetQuaKhamChuaBenhVaDoanhThuNgay)
                {
                    foreach (var setTitleColumn in setTitleColumnCoupleItems)
                    {
                        if (item.Index == setTitleColumn.Key)
                        {
                            var itemCell1 = item.Cell1Value != 0 ? item.Cell1Value : (decimal?)null;
                            var itemCell2 = item.Cell2Value != 0 ? item.Cell2Value : (decimal?)null;

                            valueKetQuaKhamChuaBenhVaDoanhThuRow += $"<td class='boder-table'>{itemCell1}</td>";
                            valueKetQuaKhamChuaBenhVaDoanhThuRow += $"<td class='boder-table'>{itemCell2}</td>";
                        }
                    }

                }
            }
            ketQuaKhamChuaBenhVaDoanhThu += valueKetQuaKhamChuaBenhVaDoanhThuRow + "</tr>";

            //Trong đó
            var trongDo = "<tr><td class='boder-table-left'></td><td  class='boder-table-left' colspan='2'>Trong đó:</td>";
            var valueTrongDoRow = string.Empty;

            if (datas.BaoCaoKetQuaKhamChuaBenhVaDoanhThuNgay.Any())
            {
                foreach (var baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem in datas.BaoCaoKetQuaKhamChuaBenhVaDoanhThuNgay)
                {
                    foreach (var setTitleColumn in setTitleColumnCoupleItems)
                    {
                        if (baoCaoKetQuaKhamChuaBenhVaDoanhThuNgayItem.Index == setTitleColumn.Key)
                        {
                          

                            valueTrongDoRow += $"<td class='boder-table'></td>";
                            valueTrongDoRow += $"<td class='boder-table'></td>";
                        }
                    }

                }
            }
            trongDo += valueTrongDoRow + "</tr>";

            //Bệnh nhân tự đến => Khách lẻ
            var benhNhanTuDenKhachLe = "<tr><td class='boder-table-left' rowspan='7''></td><td rowspan='7' class='boder-table-left' colspan='1'>+ Bệnh nhân tự đến</td><td class='boder-table-left' colspan='1'>Khách lẻ</td>";
            var valueBenhNhanTuDenKhachLeRow = string.Empty;
            if (datas.BaoCaoKetQuaBenhNhanTuDenKhachLe.Any())
            {
                foreach (var item in datas.BaoCaoKetQuaBenhNhanTuDenKhachLe)
                {
                    foreach (var setTitleColumn in setTitleColumnCoupleItems)
                    {
                        if (item.Index == setTitleColumn.Key)
                        {
                            var itemCell1 = item.Cell1Value != 0 ? item.Cell1Value : (decimal?)null;
                            var itemCell2 = item.Cell2Value != 0 ? item.Cell2Value : (decimal?)null;

                            valueBenhNhanTuDenKhachLeRow += $"<td class='boder-table-center'>{itemCell1}</td>";
                            valueBenhNhanTuDenKhachLeRow += $"<td class='boder-table-center'>{itemCell2}</td>";
                        }
                    }

                }
            }
            benhNhanTuDenKhachLe += valueBenhNhanTuDenKhachLeRow + "</tr>";


            //Bệnh nhân tự đến => Nội trú
            var benhNhanTuDenNoiTru = "<tr><td class='boder-table-left'>Nội trú</td>";
            var valueBenhNhanTuDenNoiTruRow = string.Empty;
            if (datas.BaoCaoKetQuaBenhNhanTuDenNoiTru.Any())
            {
                foreach (var item in datas.BaoCaoKetQuaBenhNhanTuDenNoiTru)
                {
                    foreach (var setTitleColumn in setTitleColumnCoupleItems)
                    {
                        if (item.Index == setTitleColumn.Key)
                        {
                            var itemCell1 = item.Cell1Value != 0 ? item.Cell1Value : (decimal?)null;
                            var itemCell2 = item.Cell2Value != 0 ? item.Cell2Value : (decimal?)null;

                            valueBenhNhanTuDenNoiTruRow += $"<td class='boder-table-center'>{itemCell1}</td>";
                            valueBenhNhanTuDenNoiTruRow += $"<td class='boder-table-center'>{itemCell2}</td>";
                        }
                    }

                }
            }
            benhNhanTuDenNoiTru += valueBenhNhanTuDenNoiTruRow + "</tr>";

            //Bệnh nhân tự đến => Nhi KCB
            var benhNhanTuDenNhiKCB = "<tr><td class='boder-table-left'>Nhi KCB</td>";
            var valueBenhNhanTuDenNhiKCBRow = string.Empty;
            if (datas.BaoCaoKetQuaBenhNhanTuDenNhiKcb.Any())
            {
                foreach (var item in datas.BaoCaoKetQuaBenhNhanTuDenNhiKcb)
                {
                    foreach (var setTitleColumn in setTitleColumnCoupleItems)
                    {
                        if (item.Index == setTitleColumn.Key)
                        {
                            var itemCell1 = item.Cell1Value != 0 ? item.Cell1Value : (decimal?)null;
                            var itemCell2 = item.Cell2Value != 0 ? item.Cell2Value : (decimal?)null;

                            valueBenhNhanTuDenNhiKCBRow += $"<td class='boder-table-center'>{itemCell1}</td>";
                            valueBenhNhanTuDenNhiKCBRow += $"<td class='boder-table-center'>{itemCell2}</td>";
                        }
                    }

                }
            }
            benhNhanTuDenNhiKCB += valueBenhNhanTuDenNhiKCBRow + "</tr>";

            //Bệnh nhân tự đến => Nhi KCB + Tiêm chủng
            var benhNhanTuDenNhiKCBTiemChung = "<tr><td class='boder-table-left'>Nhi KCB + Tiêm chủng</td>";
            var valueBenhNhanTuDenNhiKCBTiemChungRow = string.Empty;
            if (datas.BaoCaoKetQuaBenhNhanTuDenNhiKcbTiemChung.Any())
            {
                foreach (var item in datas.BaoCaoKetQuaBenhNhanTuDenNhiKcbTiemChung)
                {
                    foreach (var setTitleColumn in setTitleColumnCoupleItems)
                    {
                        if (item.Index == setTitleColumn.Key)
                        {

                            var itemCell1 = item.Cell1Value != 0 ? item.Cell1Value : (decimal?)null;
                            var itemCell2 = item.Cell2Value != 0 ? item.Cell2Value : (decimal?)null;

                            valueBenhNhanTuDenNhiKCBTiemChungRow += $"<td class='boder-table-center'>{itemCell1}</td>";
                            valueBenhNhanTuDenNhiKCBTiemChungRow += $"<td class='boder-table-center'>{itemCell2}</td>";
                        }
                    }

                }
            }
            benhNhanTuDenNhiKCBTiemChung += valueBenhNhanTuDenNhiKCBTiemChungRow + "</tr>";

            //Bệnh nhân tự đến => Nhi tiêm chủng
            var benhNhanTuDenNhiTiemChung = "<tr><td class='boder-table-left'>Nhi tiêm chủng</td>";
            var valueBenhNhanTuDenNhiTiemChungRow = string.Empty;
            if (datas.BaoCaoKetQuaBenhNhanTuDenNhiTiemChung.Any())
            {
                foreach (var item in datas.BaoCaoKetQuaBenhNhanTuDenNhiTiemChung)
                {
                    foreach (var setTitleColumn in setTitleColumnCoupleItems)
                    {
                        if (item.Index == setTitleColumn.Key)
                        {
                            var itemCell1 = item.Cell1Value != 0 ? item.Cell1Value : (decimal?)null;
                            var itemCell2 = item.Cell2Value != 0 ? item.Cell2Value : (decimal?)null;

                            valueBenhNhanTuDenNhiTiemChungRow += $"<td class='boder-table-center'>{itemCell1}</td>";
                            valueBenhNhanTuDenNhiTiemChungRow += $"<td class='boder-table-center'>{itemCell2}</td>";
                        }
                    }

                }
            }
            benhNhanTuDenNhiTiemChung += valueBenhNhanTuDenNhiTiemChungRow + "</tr>";

            //Bệnh nhân tự đến => Nhi nội trú
            var benhNhanTuDenNhiNoiTru = "<tr><td class='boder-table-left'>Nhi nội trú</td>";
            var valueBenhNhanTuDenNhiNoiTruRow = string.Empty;
            if (datas.BaoCaoKetQuaBenhNhanTuDenNhiNoiTRu.Any())
            {
                foreach (var item in datas.BaoCaoKetQuaBenhNhanTuDenNhiNoiTRu)
                {
                    foreach (var setTitleColumn in setTitleColumnCoupleItems)
                    {
                        if (item.Index == setTitleColumn.Key)
                        {
                            var itemCell1 = item.Cell1Value != 0 ? item.Cell1Value : (decimal?)null;
                            var itemCell2 = item.Cell2Value != 0 ? item.Cell2Value : (decimal?)null;

                            valueBenhNhanTuDenNhiNoiTruRow += $"<td class='boder-table-center'>{itemCell1}</td>";
                            valueBenhNhanTuDenNhiNoiTruRow += $"<td class='boder-table-center'>{itemCell2}</td>";
                        }
                    }

                }
            }
            benhNhanTuDenNhiNoiTru += valueBenhNhanTuDenNhiNoiTruRow + "</tr>";


            //Bệnh nhân tự đến => Tổng tự đến
            var benhNhanTuDenTongTuDen = "<tr><td class='boder-table-left'>Tổng tự đến</td>";
            var valueBenhNhanTuDenTongTuDenRow = string.Empty;
            if (datas.BaoCaoKetQuaBenhNhanTuDenTong.Any())
            {
                foreach (var item in datas.BaoCaoKetQuaBenhNhanTuDenTong)
                {
                    foreach (var setTitleColumn in setTitleColumnCoupleItems)
                    {
                        if (item.Index == setTitleColumn.Key)
                        {
                            var itemCell1 = item.Cell1Value != 0 ? item.Cell1Value : (decimal?)null;
                            var itemCell2 = item.Cell2Value != 0 ? item.Cell2Value : (decimal?)null;
                            valueBenhNhanTuDenTongTuDenRow += $"<td class='boder-table'>{itemCell1}</td>";
                            valueBenhNhanTuDenTongTuDenRow += $"<td class='boder-table'>{itemCell2}</td>";
                        }
                    }

                }
            }
            benhNhanTuDenTongTuDen += valueBenhNhanTuDenTongTuDenRow + "</tr>";


            //+ Sơ sinh
            var soSinh = "<tr><td class='boder-table-left'></td><td class='boder-table-left' colspan='2'>+ Sơ sinh</td>";
            var valueSoSinhRow = string.Empty;

            if (datas.BaoCaoKetQuaSoSinh.Any())
            {
                foreach (var item in datas.BaoCaoKetQuaSoSinh)
                {
                    foreach (var setTitleColumn in setTitleColumnCoupleItems)
                    {
                        if (item.Index == setTitleColumn.Key)
                        {
                            var itemCell1 = item.Cell1Value != 0 ? item.Cell1Value : (decimal?)null;
                            var itemCell2 = item.Cell2Value != 0 ? item.Cell2Value : (decimal?)null;

                            valueSoSinhRow += $"<td class='boder-table-center'>{itemCell1}</td>";
                            valueSoSinhRow += $"<td class='boder-table-center'>{itemCell2}</td>";
                        }
                    }

                }
            }
            soSinh += valueSoSinhRow + "</tr>";

            //+ Phát sinh ngoài gói KSK đoàn
            var phatSinhNgoaiGoiKSKDoan = "<tr><td class='boder-table-left'></td><td class='boder-table-left' colspan='2'>+ Phát sinh ngoài gói KSK đoàn</td>";
            var valuePhatSinhNgoaiGoiKSKDoanRow = string.Empty;

            if (datas.BaoCaoKetQuaPhatSinhNgoaiGoiKSKDoan.Any())
            {
                foreach (var item in datas.BaoCaoKetQuaPhatSinhNgoaiGoiKSKDoan)
                {
                    foreach (var setTitleColumn in setTitleColumnCoupleItems)
                    {
                        if (item.Index == setTitleColumn.Key)
                        {
                            var itemCell1 = item.Cell1Value != 0 ? item.Cell1Value : (decimal?)null;
                            var itemCell2 = item.Cell2Value != 0 ? item.Cell2Value : (decimal?)null;

                            valuePhatSinhNgoaiGoiKSKDoanRow += $"<td class='boder-table-center'>{itemCell1}</td>";
                            valuePhatSinhNgoaiGoiKSKDoanRow += $"<td class='boder-table-center'>{itemCell2}</td>";
                        }
                    }

                }
            }
            phatSinhNgoaiGoiKSKDoan += valuePhatSinhNgoaiGoiKSKDoanRow + "</tr>";

            //+ CBNV
            var cBNV = "<tr><td class='boder-table-left'></td><td class='boder-table-left' colspan='2'>+ CBNV</td>";
            var valueCBNVRow = string.Empty;

            if (datas.BaoCaoKetQuaCBNV.Any())
            {
                foreach (var item in datas.BaoCaoKetQuaCBNV)
                {
                    foreach (var setTitleColumn in setTitleColumnCoupleItems)
                    {
                        if (item.Index == setTitleColumn.Key)
                        {
                            var itemCell1 = item.Cell1Value != 0 ? item.Cell1Value : (decimal?)null;
                            var itemCell2 = item.Cell2Value != 0 ? item.Cell2Value : (decimal?)null;

                            valueCBNVRow += $"<td class='boder-table-center'>{itemCell1}</td>";
                            valueCBNVRow += $"<td class='boder-table-center'>{itemCell2}</td>";
                        }
                    }

                }
            }
            cBNV += valueCBNVRow + "</tr>";


            //+ CTV
            var ctv = "<tr><td class='boder-table-left'></td><td class='boder-table-left' colspan='2'>+ CTV</td>";
            var valueCtvRow = string.Empty;

            if (datas.BaoCaoKetQuaCTV.Any())
            {
                foreach (var item in datas.BaoCaoKetQuaCTV)
                {
                    foreach (var setTitleColumn in setTitleColumnCoupleItems)
                    {
                        if (item.Index == setTitleColumn.Key)
                        {
                            var itemCell1 = item.Cell1Value != 0 ? item.Cell1Value : (decimal?)null;
                            var itemCell2 = item.Cell2Value != 0 ? item.Cell2Value : (decimal?)null;

                            valueCtvRow += $"<td class='boder-table-center'>{itemCell1}</td>";
                            valueCtvRow += $"<td class='boder-table-center'>{itemCell2}</td>";
                        }
                    }

                }
            }
            ctv += valueCtvRow + "</tr>";

            //+ Việt Đức
            var vietDuc = "<tr><td class='boder-table-left'></td><td class='boder-table-left' colspan='2'>+ Việt Đức</td>";
            var valueVietDucRow = string.Empty;

            if (datas.BaoCaoKetQuaVietDuc.Any())
            {
                foreach (var item in datas.BaoCaoKetQuaVietDuc)
                {
                    foreach (var setTitleColumn in setTitleColumnCoupleItems)
                    {
                        if (item.Index == setTitleColumn.Key)
                        {
                            var itemCell1 = item.Cell1Value != 0 ? item.Cell1Value : (decimal?)null;
                            var itemCell2 = item.Cell2Value != 0 ? item.Cell2Value : (decimal?)null;

                            valueVietDucRow += $"<td class='boder-table-center'>{itemCell1}</td>";
                            valueVietDucRow += $"<td class='boder-table-center'>{itemCell2}</td>";
                        }
                    }

                }
            }
            vietDuc += valueVietDucRow + "</tr>";


            //+ Thẩm mỹ BS Tú (chỉ có doanh thu CLS)
            var thamMyBSTu = "<tr><td class='boder-table-left'></td><td class='boder-table-left' colspan='2'>+ Thẩm mỹ BS Tú (chỉ có doanh thu CLS)</td>";
            var thamMyBSTuRow = string.Empty;

            if (datas.BaoCaoKetQuaThamMyBSTu.Any())
            {
                foreach (var item in datas.BaoCaoKetQuaThamMyBSTu)
                {
                    foreach (var setTitleColumn in setTitleColumnCoupleItems)
                    {
                        if (item.Index == setTitleColumn.Key)
                        {
                            var itemCell1 = item.Cell1Value != 0 ? item.Cell1Value : (decimal?)null;
                            var itemCell2 = item.Cell2Value != 0 ? item.Cell2Value : (decimal?)null;

                            thamMyBSTuRow += $"<td class='boder-table-center'>{itemCell1}</td>";
                            thamMyBSTuRow += $"<td class='boder-table-center'>{itemCell2}</td>";
                        }
                    }

                }
            }
            thamMyBSTu += thamMyBSTuRow + "</tr>";

            //+ Thẩm mỹ khác (BS Đoàn, Huân, Dương, Trực,…)
            var thamMyKhac = "<tr><td class='boder-table-left'></td><td class='boder-table-left' colspan='2'>+ Thẩm mỹ khác (BS Đoàn, Huân, Dương, Trực,…)</td>";
            var thamMyKhacRow = string.Empty;

            if (datas.BaoCaoKetQuaThamMyBSTu.Any())
            {
                foreach (var item in datas.BaoCaoKetQuaThamMyKhac)
                {
                    foreach (var setTitleColumn in setTitleColumnCoupleItems)
                    {
                        if (item.Index == setTitleColumn.Key)
                        {
                            var itemCell1 = item.Cell1Value != 0 ? item.Cell1Value : (decimal?)null;
                            var itemCell2 = item.Cell2Value != 0 ? item.Cell2Value : (decimal?)null;

                            thamMyKhacRow += $"<td class='boder-table-center'>{itemCell1}</td>";
                            thamMyKhacRow += $"<td class='boder-table-center'>{itemCell2}</td>";
                        }
                    }

                }
            }
            thamMyKhac += thamMyKhacRow + "</tr>";


            //+ Thai sản => trong gói
            var thaiSanTrongGoi = "<tr><td class='boder-table-left' rowspan='2''></td><td rowspan='2' class='boder-table-left' colspan='1'>+ Thai sản</td><td class='boder-table-left' colspan='1'>Trong gói</td>";
            var valueThaiSanTrongGoiRow = string.Empty;
            if (datas.BaoCaoKetQuaThaiSanTrongGoi.Any())
            {
                foreach (var item in datas.BaoCaoKetQuaThaiSanTrongGoi)
                {
                    foreach (var setTitleColumn in setTitleColumnCoupleItems)
                    {
                        if (item.Index == setTitleColumn.Key)
                        {
                            var itemCell1 = item.Cell1Value != 0 ? item.Cell1Value : (decimal?)null;
                            var itemCell2 = item.Cell2Value != 0 ? item.Cell2Value : (decimal?)null;

                            valueThaiSanTrongGoiRow += $"<td class='boder-table-center'>{itemCell1}</td>";
                            valueThaiSanTrongGoiRow += $"<td class='boder-table-center'>{itemCell2}</td>";
                        }
                    }

                }
            }
            thaiSanTrongGoi += valueThaiSanTrongGoiRow + "</tr>";


            //+ Thai sản => ngoài gói
            var thaiSanNgoaiGoi = "<tr><td class='boder-table-left' colspan='1'>Ngoài gói</td>";
            var valueThaiSanNgoaiGoiRow = string.Empty;
            if (datas.BaoCaoKetQuaThaiSanNgoaiGoi.Any())
            {
                foreach (var item in datas.BaoCaoKetQuaThaiSanNgoaiGoi)
                {
                    foreach (var setTitleColumn in setTitleColumnCoupleItems)
                    {
                        if (item.Index == setTitleColumn.Key)
                        {
                            var itemCell1 = item.Cell1Value != 0 ? item.Cell1Value : (decimal?)null;
                            var itemCell2 = item.Cell2Value != 0 ? item.Cell2Value : (decimal?)null;

                            valueThaiSanNgoaiGoiRow += $"<td class='boder-table-center'>{itemCell1}</td>";
                            valueThaiSanNgoaiGoiRow += $"<td class='boder-table-center'>{itemCell2}</td>";
                        }
                    }

                }
            }
            thaiSanNgoaiGoi += valueThaiSanNgoaiGoiRow + "</tr>";

            //+	* Răng hàm mặt
            var rangHamMat = "<tr><td class='boder-table-left'></td><td class='boder-table-left' colspan='2'>* Răng hàm mặt</td>";
            var rangHamMatRow = string.Empty;

            if (datas.BaoCaoKetQuaRangHamMat.Any())
            {
                foreach (var item in datas.BaoCaoKetQuaRangHamMat)
                {
                    foreach (var setTitleColumn in setTitleColumnCoupleItems)
                    {
                        if (item.Index == setTitleColumn.Key)
                        {
                            var itemCell1 = item.Cell1Value != 0 ? item.Cell1Value : (decimal?)null;
                            var itemCell2 = item.Cell2Value != 0 ? item.Cell2Value : (decimal?)null;

                            rangHamMatRow += $"<td class='boder-table-center'>{itemCell1}</td>";
                            rangHamMatRow += $"<td class='boder-table-center'>{itemCell2}</td>";
                        }
                    }

                }
            }
            rangHamMat += rangHamMatRow + "</tr>";

            //3.Doanh thu bán thuốc theo ngày
            var ketQuaKhamChuaBenhVaDoanhThuBanThuoc = "<tr><td class='boder-table-left'>3</td><td class='boder-table-left' colspan='2'>Doanh thu bán thuốc theo ngày</td>";
            var ketQuaKhamChuaBenhVaDoanhThuBanThuocRow = string.Empty;

            if (datas.BaoCaoKetQuaDoanhThuBanThuoc.Any())
            {
                foreach (var item in datas.BaoCaoKetQuaDoanhThuBanThuoc)
                {
                    foreach (var setTitleColumn in setTitleColumnCoupleItems)
                    {
                        if (item.Index == setTitleColumn.Key)
                        {
                            var itemCell1 = item.Cell1Value != 0 ? item.Cell1Value : (decimal?)null;
                            var itemCell2 = item.Cell2Value != 0 ? item.Cell2Value : (decimal?)null;

                            ketQuaKhamChuaBenhVaDoanhThuBanThuocRow += $"<td class='boder-table'>{itemCell1}</td>";
                            ketQuaKhamChuaBenhVaDoanhThuBanThuocRow += $"<td class='boder-table'>{itemCell2}</td>";
                        }
                    }

                }
            }
            ketQuaKhamChuaBenhVaDoanhThuBanThuoc += ketQuaKhamChuaBenhVaDoanhThuBanThuocRow + "</tr>";

            //II.Trong đó thực thu tiền mặt và thẻ:
            var thuTienMatVathe = "<tr><td class='boder-table-left'>II</td><td class='boder-table-left' colspan='2'>Trong đó thực thu tiền mặt và thẻ:</td>";
            var thuTienMatVatheRow = string.Empty;

            if (datas.BaoCaoKetQuaThucThuTienMatVaThe.Any())
            {
                foreach (var item in datas.BaoCaoKetQuaThucThuTienMatVaThe)
                {
                    foreach (var setTitleColumn in setTitleColumnCoupleItems)
                    {
                        if (item.Index == setTitleColumn.Key)
                        {
                            var itemCell1 = item.Cell1Value != 0 ? item.Cell1Value : (decimal?)null;
                            var itemCell2 = item.Cell2Value != 0 ? item.Cell2Value : (decimal?)null;
                            thuTienMatVatheRow += $"<td class='boder-table'>{itemCell1}</td>";
                            thuTienMatVatheRow += $"<td class='boder-table'>{itemCell2}</td>";
                        }
                    }

                }
            }
            thuTienMatVathe += thuTienMatVatheRow + "</tr>";

            //Thực thu tiền mặt (đã trừ số tiền hoàn trả cho khách):
            var thucThuTienMat = "<tr><td class='boder-table-left'>1</td><td class='boder-table-left' colspan='2'>Thực thu tiền mặt (đã trừ số tiền hoàn trả cho khách)</td>";
            var thucThuTienMatRow = string.Empty;

            if (datas.BaoCaoKetQuaThucThuTienMat.Any())
            {
                foreach (var item in datas.BaoCaoKetQuaThucThuTienMat)
                {
                    foreach (var setTitleColumn in setTitleColumnCoupleItems)
                    {
                        if (item.Index == setTitleColumn.Key)
                        {
                            var itemCell1 = item.Cell1Value != 0 ? item.Cell1Value : (decimal?)null;
                            var itemCell2 = item.Cell2Value != 0 ? item.Cell2Value : (decimal?)null;
                            thucThuTienMatRow += $"<td class='boder-table-center'>{itemCell1}</td>";
                            thucThuTienMatRow += $"<td class='boder-table-center'>{itemCell2}</td>";
                        }
                    }

                }
            }
            thucThuTienMat += thucThuTienMatRow + "</tr>";

            //Thực thu Chuyển khoản
            var thucThuChuyenKhoan = "<tr><td class='boder-table-left'>2</td><td class='boder-table-left' colspan='2'>Thực thu Chuyển khoản</td>";
            var thuThuChuyenKhoanRow = string.Empty;

            if (datas.BaoCaoKetQuaThucThuChuyenKhoan.Any())
            {
                foreach (var item in datas.BaoCaoKetQuaThucThuChuyenKhoan)
                {
                    foreach (var setTitleColumn in setTitleColumnCoupleItems)
                    {
                        if (item.Index == setTitleColumn.Key)
                        {
                            var itemCell1 = item.Cell1Value != 0 ? item.Cell1Value : (decimal?)null;
                            var itemCell2 = item.Cell2Value != 0 ? item.Cell2Value : (decimal?)null;
                            thuThuChuyenKhoanRow += $"<td class='boder-table-center'>{itemCell1}</td>";
                            thuThuChuyenKhoanRow += $"<td class='boder-table-center'>{itemCell2}</td>";
                        }
                    }

                }
            }
            thucThuChuyenKhoan += thuThuChuyenKhoanRow + "</tr>";


            //Thực thu quẹt thẻ
            var thuVaQuetThe = "<tr><td class='boder-table-left'>3</td><td class='boder-table-left' colspan='2'>Thực thu quẹt thẻ</td>";
            var thuVaQuetTheRow = string.Empty;

            if (datas.BaoCaoKetQuaThucThuQuetThe.Any())
            {
                foreach (var item in datas.BaoCaoKetQuaThucThuQuetThe)
                {
                    foreach (var setTitleColumn in setTitleColumnCoupleItems)
                    {
                        if (item.Index == setTitleColumn.Key)
                        {
                            var itemCell1 = item.Cell1Value != 0 ? item.Cell1Value : (decimal?)null;
                            var itemCell2 = item.Cell2Value != 0 ? item.Cell2Value : (decimal?)null;
                            thuVaQuetTheRow += $"<td class='boder-table-center'>{itemCell1}</td>";
                            thuVaQuetTheRow += $"<td class='boder-table-center'>{itemCell2}</td>";
                        }
                    }

                }
            }
            thuVaQuetThe += thuVaQuetTheRow + "</tr>";

            //Công nợ thu được trong ngày
            var congNoThuTrongNgay = "<tr><td class='boder-table-left'>4</td><td class='boder-table-left' colspan='2'>Công nợ thu được trong ngày</td>";
            var congNoThuTrongNgayRow = string.Empty;

            if (datas.BaoCaoKetQuaCongNoThuDuoc.Any())
            {
                foreach (var item in datas.BaoCaoKetQuaCongNoThuDuoc)
                {
                    foreach (var setTitleColumn in setTitleColumnCoupleItems)
                    {
                        if (item.Index == setTitleColumn.Key)
                        {
                            var itemCell1 = item.Cell1Value != 0 ? item.Cell1Value : (decimal?)null;
                            var itemCell2 = item.Cell2Value != 0 ? item.Cell2Value : (decimal?)null;

                            congNoThuTrongNgayRow += $"<td class='boder-table-center'>{itemCell1}</td>";
                            congNoThuTrongNgayRow += $"<td class='boder-table-center'>{itemCell2}</td>";
                        }
                    }

                }
            }
            congNoThuTrongNgay += congNoThuTrongNgayRow + "</tr>";

            table += kqKCBVaDoanhThuTheoNgay + khamSucKhoeDoan + phanDoanhThuTongKetKhiCoQTDoan + ketQuaKhamChuaBenhVaDoanhThu + trongDo + benhNhanTuDenKhachLe + benhNhanTuDenNoiTru
                + benhNhanTuDenNhiKCB + benhNhanTuDenNhiKCBTiemChung + benhNhanTuDenNhiTiemChung + benhNhanTuDenNhiNoiTru + benhNhanTuDenTongTuDen
                + soSinh + phatSinhNgoaiGoiKSKDoan + cBNV + ctv + vietDuc + thamMyBSTu + thamMyKhac + thaiSanTrongGoi + thaiSanNgoaiGoi + rangHamMat
                + ketQuaKhamChuaBenhVaDoanhThuBanThuoc + thuTienMatVathe + thucThuTienMat + thucThuChuyenKhoan + thuVaQuetThe + congNoThuTrongNgay + "<table>";
            return table;
        }

    }
}
