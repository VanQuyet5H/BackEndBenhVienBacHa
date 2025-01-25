using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Services.ExportImport.Help;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.PhieuDeNghiTestTruocKhiDungThuoc;
using Newtonsoft.Json;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public async Task<GridDataSource> GetDataBaoCaoChiTietMienPhiTronVienForGridAsync(QueryInfo queryInfo)
        {
            var phieuThus = new List<BaoCaoChiTietMienPhiTronVienGridVo>();
            var timKiemNangCaoObj = new BaoCaoChiTietMienPhiTronVienQueryInfo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoChiTietMienPhiTronVienQueryInfo>(queryInfo.AdditionalSearchString);
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
                var lyDoKhongTinhPhiDuocPhamVatTu = "Miễn phí thuốc/VTYT của dịch vụ {0}";
                var khoaDuocKhoaPhong = _KhoaPhongRepository.TableNoTracking
                    .FirstOrDefault(x => x.Ma.Contains("KD"));
                var khoaDuoc = khoaDuocKhoaPhong.Ten ?? "Khoa dược";

                #region BVHD-3918: Cập nhật bổ sung lý do -> trường hợp có nơi giới thiệu
                var hinhThucDenGioiThieu = _cauHinhService.GetSetting("CauHinhBaoCao.HinhThucDenGioiThieu");
                long.TryParse(hinhThucDenGioiThieu?.Value, out long hinhThucDenGioiThieuId);
                #endregion

                var taiKhoanBenhNhanChis = _taiKhoanBenhNhanChiRepository.TableNoTracking
                    .Where(o => o.DaHuy != true && o.NgayChi >= tuNgay && o.NgayChi <= denNgay && o.SoTienMienGiam != null && o.SoTienMienGiam > 0 && o.TaiKhoanBenhNhanThuId != null)
                    .Select(o => new
                    {
                        o.Id,
                        o.TaiKhoanBenhNhanThu.YeuCauTiepNhanId,
                        o.YeuCauKhamBenhId,
                        o.YeuCauDichVuKyThuatId,
                        o.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId,
                        o.YeuCauDuocPhamBenhVienId,
                        o.YeuCauVatTuBenhVienId,
                        o.DonThuocThanhToanChiTietId,
                        o.DonVTYTThanhToanChiTietId,
                        o.YeuCauTruyenMauId,
                        o.Gia,
                        o.SoLuong,
                        o.SoTienMienGiam
                    }).ToList();

                var yeuCauKhamBenhIds = taiKhoanBenhNhanChis.Where(o => o.YeuCauKhamBenhId != null).Select(o => o.YeuCauKhamBenhId.Value).ToList();
                var yeuCauDichVuKyThuatIds = taiKhoanBenhNhanChis.Where(o => o.YeuCauDichVuKyThuatId != null).Select(o => o.YeuCauDichVuKyThuatId.Value).ToList();
                var yeuCauDichVuGiuongBenhVienChiPhiBenhVienIds = taiKhoanBenhNhanChis.Where(o => o.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId != null).Select(o => o.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId.Value).ToList();
                var yeuCauDuocPhamBenhVienIds = taiKhoanBenhNhanChis.Where(o => o.YeuCauDuocPhamBenhVienId != null).Select(o => o.YeuCauDuocPhamBenhVienId.Value).ToList();
                var yeuCauVatTuBenhVienIds = taiKhoanBenhNhanChis.Where(o => o.YeuCauVatTuBenhVienId != null).Select(o => o.YeuCauVatTuBenhVienId.Value).ToList();
                var donThuocThanhToanChiTietIds = taiKhoanBenhNhanChis.Where(o => o.DonThuocThanhToanChiTietId != null).Select(o => o.DonThuocThanhToanChiTietId.Value).ToList();
                var donVTYTThanhToanChiTietIds = taiKhoanBenhNhanChis.Where(o => o.DonVTYTThanhToanChiTietId != null).Select(o => o.DonVTYTThanhToanChiTietId.Value).ToList();
                var yeuCauTruyenMauIds = taiKhoanBenhNhanChis.Where(o => o.YeuCauTruyenMauId != null).Select(o => o.YeuCauTruyenMauId.Value).ToList();

                var phieuThuYeuCauKhamBenhs = _yeuCauKhamBenhRepository.TableNoTracking
                        .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham
                                    && (yeuCauKhamBenhIds.Contains(x.Id) || (x.KhongTinhPhi != null && x.KhongTinhPhi == true && x.ThoiDiemChiDinh >= tuNgay && x.ThoiDiemChiDinh <= denNgay)))
                        .Select(item => new 
                        {
                            item.Id,
                            item.YeuCauTiepNhanId,
                            item.NoiChiDinhId,
                            item.KhongTinhPhi,
                            item.YeuCauGoiDichVuId,
                            item.Gia,
                            item.DonGiaSauChietKhau,
                            SoLuong = 1,
                            item.SoTienMienGiam,
                            item.GhiChuMienGiamThem,
                            NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKhamBenh
                        }).ToList();

                var phieuThuYeuCauDichVuKyThuats = _yeuCauDichVuKyThuatRepository.TableNoTracking
                        .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                    && (yeuCauDichVuKyThuatIds.Contains(x.Id) || (x.KhongTinhPhi != null && x.KhongTinhPhi == true && x.ThoiDiemChiDinh >= tuNgay && x.ThoiDiemChiDinh <= denNgay)))
                        .Select(item => new
                        {
                            item.Id,
                            item.YeuCauTiepNhanId,
                            item.NoiChiDinhId,
                            item.KhongTinhPhi,
                            item.YeuCauGoiDichVuId,
                            item.Gia,
                            item.DonGiaSauChietKhau,
                            SoLuong = item.SoLan,
                            item.SoTienMienGiam,
                            item.GhiChuMienGiamThem,
                            NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKyThuat
                        }).ToList();

                var phieuThuDuocPhamBenhViens = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                        .Where(x => x.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                                    && (yeuCauDuocPhamBenhVienIds.Contains(x.Id) || (x.KhongTinhPhi != null && x.KhongTinhPhi == true && x.ThoiDiemChiDinh >= tuNgay && x.ThoiDiemChiDinh <= denNgay)))
                        .Select(item => new
                        {
                            item.Id,
                            item.YeuCauTiepNhanId,
                            item.NoiChiDinhId,
                            item.KhongTinhPhi,
                            item.YeuCauGoiDichVuId,                            
                            SoLuong = item.SoLuong,
                            item.SoTienMienGiam,
                            item.GhiChuMienGiamThem,
                            TenDichVuKyThuat = (item.KhongTinhPhi != null && item.KhongTinhPhi == true && item.YeuCauDichVuKyThuatId != null) ? item.YeuCauDichVuKyThuat.TenDichVu : "",
                            //NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKyThuat
                            XuatKhoChiTietId = item.XuatKhoDuocPhamChiTietId,
                            DonGiaNhap = item.DonGiaNhap,
                            VAT = item.VAT,
                            TiLeTheoThapGia = item.TiLeTheoThapGia,
                            PhuongPhapTinhGiaTriTonKhos = item.XuatKhoDuocPhamChiTietId != null
                                        ? item.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Select(a => a.NhapKhoDuocPhamChiTiet.PhuongPhapTinhGiaTriTonKho).ToList()
                                        : new List<Enums.PhuongPhapTinhGiaTriTonKho>()

                        }).ToList();

                var phieuThuVatTuBenhViens = _yeuCauVatTuBenhVienRepository.TableNoTracking
                        .Where(x => x.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy
                                    && (yeuCauVatTuBenhVienIds.Contains(x.Id) || (x.KhongTinhPhi != null && x.KhongTinhPhi == true && x.ThoiDiemChiDinh >= tuNgay && x.ThoiDiemChiDinh <= denNgay)))
                        .Select(item => new
                        {
                            item.Id,
                            item.YeuCauTiepNhanId,
                            item.NoiChiDinhId,
                            item.KhongTinhPhi,
                            item.YeuCauGoiDichVuId,
                            item.GiaBan,
                            item.SoTienMienGiam,
                            item.GhiChuMienGiamThem,
                            TenDichVuKyThuat = (item.KhongTinhPhi != null && item.KhongTinhPhi == true && item.YeuCauDichVuKyThuatId != null) ? item.YeuCauDichVuKyThuat.TenDichVu : "",
                            //NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKyThuat
                        }).ToList();

                var phieuThuDichVuGiuongs = _yeuCauDichVuGiuongBenhVienChiPhiBenhVienRepository.TableNoTracking
                        .Where(x => yeuCauDichVuGiuongBenhVienChiPhiBenhVienIds.Contains(x.Id))
                        .Select(item => new
                        {
                            item.Id,
                            item.YeuCauTiepNhanId,
                            TenKhoaPhong = item.KhoaPhong.Ten,
                            item.YeuCauGoiDichVuId,
                            item.Gia,
                            SoLuong = item.SoLuong,
                            item.DonGiaSauChietKhau,
                            item.SoTienMienGiam,
                            item.GhiChuMienGiamThem,
                            NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuGiuongBenh
                        }).ToList();

                var phieuThuDonThuocs = _donThuocThanhToanChiTietRepository.TableNoTracking
                        .Where(x => donThuocThanhToanChiTietIds.Contains(x.Id))
                        .Select(item => new
                        {
                            item.Id,
                            item.DonThuocThanhToan.YeuCauTiepNhanId,
                            NoiKeDonId = item.YeuCauKhamBenhDonThuocChiTietId != null ? item.YeuCauKhamBenhDonThuocChiTiet.YeuCauKhamBenhDonThuoc.NoiKeDonId : 0,
                            item.GiaBan,
                            item.SoTienMienGiam,
                            item.GhiChuMienGiamThem,
                            //NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKyThuat
                        }).ToList();

                var phieuThuDonVTYTs = _donVTYTThanhToanChiTietRepository.TableNoTracking
                        .Where(x => donVTYTThanhToanChiTietIds.Contains(x.Id))
                        .Select(item => new
                        {
                            item.Id,
                            item.DonVTYTThanhToan.YeuCauTiepNhanId,
                            NoiKeDonId = item.YeuCauKhamBenhDonVTYTChiTietId != null ? item.YeuCauKhamBenhDonVTYTChiTiet.YeuCauKhamBenhDonVTYT.NoiKeDonId : 0,
                            item.GiaBan,
                            item.SoTienMienGiam,
                            item.GhiChuMienGiamThem,
                            //NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKyThuat
                        }).ToList();

                var phieuThuTruyenMaus = _yeuCauTruyenMauRepository.TableNoTracking
                        .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauTruyenMau.DaHuy && yeuCauTruyenMauIds.Contains(x.Id))
                        .Select(item => new
                        {
                            item.Id,
                            item.YeuCauTiepNhanId,
                            item.NoiChiDinhId,
                            item.DonGiaBan,
                            SoLuong = 1,
                            item.SoTienMienGiam,
                            item.GhiChuMienGiamThem,
                            //NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKyThuat
                        }).ToList();
                var yeuCauTiepNhanIds = phieuThuYeuCauKhamBenhs.Select(o => o.YeuCauTiepNhanId)
                                            .Concat(phieuThuYeuCauDichVuKyThuats.Select(o => o.YeuCauTiepNhanId))
                                            .Concat(phieuThuDuocPhamBenhViens.Select(o => o.YeuCauTiepNhanId))
                                            .Concat(phieuThuVatTuBenhViens.Select(o => o.YeuCauTiepNhanId))
                                            .Concat(phieuThuDichVuGiuongs.Select(o => o.YeuCauTiepNhanId))
                                            .Concat(phieuThuDonThuocs.Select(o => o.YeuCauTiepNhanId.GetValueOrDefault()))
                                            .Concat(phieuThuDonVTYTs.Select(o => o.YeuCauTiepNhanId.GetValueOrDefault()))
                                            .Concat(phieuThuTruyenMaus.Select(o => o.YeuCauTiepNhanId))
                                            .ToList();

                var dataYeuCauTiepNhans = _yeuCauTiepNhanRepository.TableNoTracking
                    .Where(o => yeuCauTiepNhanIds.Contains(o.Id))
                    .Select(item => new
                    {
                        item.Id,
                        MaNB = item.BenhNhan.MaBN,
                        MaTN = item.MaYeuCauTiepNhan,
                        TenBN = item.HoTen,
                        NgaySinh = item.NgaySinh,
                        ThangSinh = item.ThangSinh,
                        NamSinh = item.NamSinh,
                        SoDienThoai = item.SoDienThoaiDisplay,
                        DiaChi = item.DiaChiDayDu,
                        item.HinhThucDenId
                    }).ToList();

                var phongBenhViens = _phongBenhVienRepository.TableNoTracking.Select(o => new
                {
                    o.Id,
                    TenKhoaPhong = o.KhoaPhong.Ten
                }).ToList();

                foreach(var item in phieuThuYeuCauKhamBenhs)
                {
                    var tenKhoaPhong = phongBenhViens.FirstOrDefault(o => o.Id == item.NoiChiDinhId)?.TenKhoaPhong;
                    var dataYeuCauTiepNhan = dataYeuCauTiepNhans.First(o => o.Id == item.YeuCauTiepNhanId);

                    phieuThus.Add(new BaoCaoChiTietMienPhiTronVienGridVo
                    {
                        YeuCauTiepNhanId = item.YeuCauTiepNhanId,
                        TenKhoaPhong = tenKhoaPhong,
                        MaNB = dataYeuCauTiepNhan.MaNB,
                        MaTN = dataYeuCauTiepNhan.MaTN,
                        TenBN = dataYeuCauTiepNhan.TenBN,
                        NgaySinh = dataYeuCauTiepNhan.NgaySinh,
                        ThangSinh = dataYeuCauTiepNhan.ThangSinh,
                        NamSinh = dataYeuCauTiepNhan.NamSinh,
                        SoDienThoai = dataYeuCauTiepNhan.SoDienThoai,
                        DiaChi = dataYeuCauTiepNhan.DiaChi,
                        LaHinhThucDenGioiThieu = dataYeuCauTiepNhan.HinhThucDenId == hinhThucDenGioiThieuId,
                        ThanhTien = item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia,
                        GiamPhi = item.KhongTinhPhi != true
                                ? (item.SoTienMienGiam < (item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia) ? item.SoTienMienGiam : (decimal?)null)
                                : (decimal?)null,
                        MienPhi = item.KhongTinhPhi != true
                                ? (item.SoTienMienGiam >= (item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia) ? item.SoTienMienGiam : (decimal?)null)
                                : (item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia),
                        SoTienMienGiam = item.SoTienMienGiam,
                        KhongTinhPhi = item.KhongTinhPhi != null && item.KhongTinhPhi == true,
                        LyDo = item.GhiChuMienGiamThem,
                        YeuCauDichVuId = item.Id,
                        NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKhamBenh,
                    });
                }
                foreach (var item in phieuThuYeuCauDichVuKyThuats)
                {
                    var tenKhoaPhong = phongBenhViens.FirstOrDefault(o => o.Id == item.NoiChiDinhId)?.TenKhoaPhong;
                    var dataYeuCauTiepNhan = dataYeuCauTiepNhans.First(o => o.Id == item.YeuCauTiepNhanId);

                    phieuThus.Add(new BaoCaoChiTietMienPhiTronVienGridVo
                    {
                        YeuCauTiepNhanId = item.YeuCauTiepNhanId,
                        TenKhoaPhong = tenKhoaPhong,
                        MaNB = dataYeuCauTiepNhan.MaNB,
                        MaTN = dataYeuCauTiepNhan.MaTN,
                        TenBN = dataYeuCauTiepNhan.TenBN,
                        NgaySinh = dataYeuCauTiepNhan.NgaySinh,
                        ThangSinh = dataYeuCauTiepNhan.ThangSinh,
                        NamSinh = dataYeuCauTiepNhan.NamSinh,
                        SoDienThoai = dataYeuCauTiepNhan.SoDienThoai,
                        DiaChi = dataYeuCauTiepNhan.DiaChi,
                        LaHinhThucDenGioiThieu = dataYeuCauTiepNhan.HinhThucDenId == hinhThucDenGioiThieuId,
                        ThanhTien = (item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia) * item.SoLuong,
                        GiamPhi = item.KhongTinhPhi != true
                                        ? (item.SoTienMienGiam < ((item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia) * item.SoLuong) ? item.SoTienMienGiam : (decimal?)null)
                                        : (decimal?)null,
                        MienPhi = item.KhongTinhPhi != true
                                        ? (item.SoTienMienGiam >= ((item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia) * item.SoLuong) ? item.SoTienMienGiam : (decimal?)null)
                                        : ((item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia) * item.SoLuong),
                        SoTienMienGiam = item.SoTienMienGiam,
                        KhongTinhPhi = item.KhongTinhPhi != null && item.KhongTinhPhi == true,
                        LyDo = item.GhiChuMienGiamThem,
                        YeuCauDichVuId = item.Id,
                        NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKyThuat,
                    });
                }
                foreach (var item in phieuThuDuocPhamBenhViens)
                {
                    var tenKhoaPhong = phongBenhViens.FirstOrDefault(o => o.Id == item.NoiChiDinhId)?.TenKhoaPhong;
                    var dataYeuCauTiepNhan = dataYeuCauTiepNhans.First(o => o.Id == item.YeuCauTiepNhanId);

                    var phuongPhapTinhGiaTriTonKho = item.PhuongPhapTinhGiaTriTonKhos.Any() ? item.PhuongPhapTinhGiaTriTonKhos.First() : Enums.PhuongPhapTinhGiaTriTonKho.ApVAT;
                    var giaTonKho = item.DonGiaNhap + (item.DonGiaNhap * (phuongPhapTinhGiaTriTonKho == Enums.PhuongPhapTinhGiaTriTonKho.ApVAT ? item.VAT : 0) / 100);
                    var donGiaBan = Math.Round(giaTonKho + (giaTonKho * item.TiLeTheoThapGia / 100), 2, MidpointRounding.AwayFromZero);
                    var giaBan = Math.Round((decimal)item.SoLuong * donGiaBan, 2, MidpointRounding.AwayFromZero);

                    phieuThus.Add(new BaoCaoChiTietMienPhiTronVienGridVo
                    {
                        YeuCauTiepNhanId = item.YeuCauTiepNhanId,
                        TenKhoaPhong = tenKhoaPhong,
                        MaNB = dataYeuCauTiepNhan.MaNB,
                        MaTN = dataYeuCauTiepNhan.MaTN,
                        TenBN = dataYeuCauTiepNhan.TenBN,
                        NgaySinh = dataYeuCauTiepNhan.NgaySinh,
                        ThangSinh = dataYeuCauTiepNhan.ThangSinh,
                        NamSinh = dataYeuCauTiepNhan.NamSinh,
                        SoDienThoai = dataYeuCauTiepNhan.SoDienThoai,
                        DiaChi = dataYeuCauTiepNhan.DiaChi,
                        LaHinhThucDenGioiThieu = dataYeuCauTiepNhan.HinhThucDenId == hinhThucDenGioiThieuId,
                        ThanhTien = giaBan,
                        GiamPhi = item.KhongTinhPhi != true
                                                    ? (item.SoTienMienGiam < giaBan ? item.SoTienMienGiam : (decimal?)null)
                                                    : (decimal?)null,
                        MienPhi = item.KhongTinhPhi != true
                                                    ? (item.SoTienMienGiam >= giaBan ? item.SoTienMienGiam : (decimal?)null)
                                                    : giaBan,
                        SoTienMienGiam = item.SoTienMienGiam,
                        KhongTinhPhi = item.KhongTinhPhi != null && item.KhongTinhPhi == true,
                        LyDo = (item.KhongTinhPhi != null && item.KhongTinhPhi == true && !string.IsNullOrEmpty(item.TenDichVuKyThuat))
                                                    ? string.Format(lyDoKhongTinhPhiDuocPhamVatTu, item.TenDichVuKyThuat) : item.GhiChuMienGiamThem,
                    });
                }
                foreach (var item in phieuThuVatTuBenhViens)
                {
                    var tenKhoaPhong = phongBenhViens.FirstOrDefault(o => o.Id == item.NoiChiDinhId)?.TenKhoaPhong;
                    var dataYeuCauTiepNhan = dataYeuCauTiepNhans.First(o => o.Id == item.YeuCauTiepNhanId);

                    phieuThus.Add(new BaoCaoChiTietMienPhiTronVienGridVo
                    {
                        YeuCauTiepNhanId = item.YeuCauTiepNhanId,
                        TenKhoaPhong = tenKhoaPhong,
                        MaNB = dataYeuCauTiepNhan.MaNB,
                        MaTN = dataYeuCauTiepNhan.MaTN,
                        TenBN = dataYeuCauTiepNhan.TenBN,
                        NgaySinh = dataYeuCauTiepNhan.NgaySinh,
                        ThangSinh = dataYeuCauTiepNhan.ThangSinh,
                        NamSinh = dataYeuCauTiepNhan.NamSinh,
                        SoDienThoai = dataYeuCauTiepNhan.SoDienThoai,
                        DiaChi = dataYeuCauTiepNhan.DiaChi,
                        LaHinhThucDenGioiThieu = dataYeuCauTiepNhan.HinhThucDenId == hinhThucDenGioiThieuId,
                        ThanhTien = item.GiaBan,
                        GiamPhi = item.KhongTinhPhi != true
                                        ? (item.SoTienMienGiam < item.GiaBan ? item.SoTienMienGiam : (decimal?)null)
                                        : (decimal?)null,
                        MienPhi = item.KhongTinhPhi != true
                                        ? (item.SoTienMienGiam >= item.GiaBan ? item.SoTienMienGiam : (decimal?)null)
                                        : item.GiaBan,
                        SoTienMienGiam = item.SoTienMienGiam,
                        KhongTinhPhi = item.KhongTinhPhi != null && item.KhongTinhPhi == true,
                        LyDo = (item.KhongTinhPhi != null && item.KhongTinhPhi == true && !string.IsNullOrEmpty(item.TenDichVuKyThuat))
                                        ? string.Format(lyDoKhongTinhPhiDuocPhamVatTu, item.TenDichVuKyThuat) : item.GhiChuMienGiamThem,
                    });
                }
                foreach (var item in phieuThuDichVuGiuongs)
                {
                    var dataYeuCauTiepNhan = dataYeuCauTiepNhans.First(o => o.Id == item.YeuCauTiepNhanId);

                    phieuThus.Add(new BaoCaoChiTietMienPhiTronVienGridVo
                    {
                        YeuCauTiepNhanId = item.YeuCauTiepNhanId,
                        TenKhoaPhong = item.TenKhoaPhong,
                        MaNB = dataYeuCauTiepNhan.MaNB,
                        MaTN = dataYeuCauTiepNhan.MaTN,
                        TenBN = dataYeuCauTiepNhan.TenBN,
                        NgaySinh = dataYeuCauTiepNhan.NgaySinh,
                        ThangSinh = dataYeuCauTiepNhan.ThangSinh,
                        NamSinh = dataYeuCauTiepNhan.NamSinh,
                        SoDienThoai = dataYeuCauTiepNhan.SoDienThoai,
                        DiaChi = dataYeuCauTiepNhan.DiaChi,
                        LaHinhThucDenGioiThieu = dataYeuCauTiepNhan.HinhThucDenId == hinhThucDenGioiThieuId,

                        ThanhTien = Convert.ToDecimal(item.SoLuong * (double)(item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau ?? 0 : item.Gia)),
                        GiamPhi = item.SoTienMienGiam < (Convert.ToDecimal((double)((item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia) ?? 0) * item.SoLuong)) ? item.SoTienMienGiam : (decimal?)null,
                        MienPhi = item.SoTienMienGiam >= (Convert.ToDecimal((double)((item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia) ?? 0) * item.SoLuong)) ? item.SoTienMienGiam : (decimal?)null,
                        SoTienMienGiam = item.SoTienMienGiam,
                        LyDo = item.GhiChuMienGiamThem,
                        YeuCauDichVuId = item.Id,
                        NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuGiuongBenh
                    });
                }
                foreach (var item in phieuThuDonThuocs)
                {
                    var tenKhoaPhong = phongBenhViens.FirstOrDefault(o => o.Id == item.NoiKeDonId)?.TenKhoaPhong ?? khoaDuoc;
                    var dataYeuCauTiepNhan = dataYeuCauTiepNhans.First(o => o.Id == item.YeuCauTiepNhanId);

                    phieuThus.Add(new BaoCaoChiTietMienPhiTronVienGridVo
                    {
                        YeuCauTiepNhanId = item.YeuCauTiepNhanId.Value,
                        TenKhoaPhong = tenKhoaPhong,
                        MaNB = dataYeuCauTiepNhan.MaNB,
                        MaTN = dataYeuCauTiepNhan.MaTN,
                        TenBN = dataYeuCauTiepNhan.TenBN,
                        NgaySinh = dataYeuCauTiepNhan.NgaySinh,
                        ThangSinh = dataYeuCauTiepNhan.ThangSinh,
                        NamSinh = dataYeuCauTiepNhan.NamSinh,
                        SoDienThoai = dataYeuCauTiepNhan.SoDienThoai,
                        DiaChi = dataYeuCauTiepNhan.DiaChi,
                        LaHinhThucDenGioiThieu = dataYeuCauTiepNhan.HinhThucDenId == hinhThucDenGioiThieuId,
                        ThanhTien = item.GiaBan,
                        GiamPhi = item.SoTienMienGiam < item.GiaBan ? item.SoTienMienGiam : (decimal?)null,
                        MienPhi = item.SoTienMienGiam >= item.GiaBan ? item.SoTienMienGiam : (decimal?)null,
                        SoTienMienGiam = item.SoTienMienGiam,
                        LyDo = item.GhiChuMienGiamThem
                    });
                }
                foreach (var item in phieuThuDonVTYTs)
                {
                    var tenKhoaPhong = phongBenhViens.FirstOrDefault(o => o.Id == item.NoiKeDonId)?.TenKhoaPhong ?? khoaDuoc;
                    var dataYeuCauTiepNhan = dataYeuCauTiepNhans.First(o => o.Id == item.YeuCauTiepNhanId);

                    phieuThus.Add(new BaoCaoChiTietMienPhiTronVienGridVo
                    {
                        YeuCauTiepNhanId = item.YeuCauTiepNhanId.Value,
                        TenKhoaPhong = tenKhoaPhong,
                        MaNB = dataYeuCauTiepNhan.MaNB,
                        MaTN = dataYeuCauTiepNhan.MaTN,
                        TenBN = dataYeuCauTiepNhan.TenBN,
                        NgaySinh = dataYeuCauTiepNhan.NgaySinh,
                        ThangSinh = dataYeuCauTiepNhan.ThangSinh,
                        NamSinh = dataYeuCauTiepNhan.NamSinh,
                        SoDienThoai = dataYeuCauTiepNhan.SoDienThoai,
                        DiaChi = dataYeuCauTiepNhan.DiaChi,
                        LaHinhThucDenGioiThieu = dataYeuCauTiepNhan.HinhThucDenId == hinhThucDenGioiThieuId,
                        ThanhTien = item.GiaBan,
                        GiamPhi = item.SoTienMienGiam < item.GiaBan ? item.SoTienMienGiam : (decimal?)null,
                        MienPhi = item.SoTienMienGiam >= item.GiaBan ? item.SoTienMienGiam : (decimal?)null,
                        SoTienMienGiam = item.SoTienMienGiam,
                        LyDo = item.GhiChuMienGiamThem
                    });
                }
                foreach (var item in phieuThuTruyenMaus)
                {
                    var tenKhoaPhong = phongBenhViens.FirstOrDefault(o => o.Id == item.NoiChiDinhId)?.TenKhoaPhong ?? khoaDuoc;
                    var dataYeuCauTiepNhan = dataYeuCauTiepNhans.First(o => o.Id == item.YeuCauTiepNhanId);

                    phieuThus.Add(new BaoCaoChiTietMienPhiTronVienGridVo
                    {
                        YeuCauTiepNhanId = item.YeuCauTiepNhanId,
                        TenKhoaPhong = tenKhoaPhong,
                        MaNB = dataYeuCauTiepNhan.MaNB,
                        MaTN = dataYeuCauTiepNhan.MaTN,
                        TenBN = dataYeuCauTiepNhan.TenBN,
                        NgaySinh = dataYeuCauTiepNhan.NgaySinh,
                        ThangSinh = dataYeuCauTiepNhan.ThangSinh,
                        NamSinh = dataYeuCauTiepNhan.NamSinh,
                        SoDienThoai = dataYeuCauTiepNhan.SoDienThoai,
                        DiaChi = dataYeuCauTiepNhan.DiaChi,
                        LaHinhThucDenGioiThieu = dataYeuCauTiepNhan.HinhThucDenId == hinhThucDenGioiThieuId,
                        ThanhTien = item.DonGiaBan,
                        GiamPhi = item.SoTienMienGiam < item.DonGiaBan ? item.SoTienMienGiam : (decimal?)null,
                        MienPhi = item.SoTienMienGiam >= item.DonGiaBan ? item.SoTienMienGiam : (decimal?)null,
                        SoTienMienGiam = item.SoTienMienGiam,
                        LyDo = item.GhiChuMienGiamThem,
                    });
                }

                phieuThus = phieuThus.OrderBy(x => x.TenKhoaPhong).ThenBy(x => x.MaTN)
                    .GroupBy(x => new { x.MaTN, x.TenKhoaPhong })
                    .Select(item => new BaoCaoChiTietMienPhiTronVienGridVo()
                    {
                        YeuCauTiepNhanId = item.First().YeuCauTiepNhanId,
                        TenKhoaPhong = item.Key.TenKhoaPhong,
                        MaNB = item.First().MaNB,
                        MaTN = item.Key.MaTN,
                        TenBN = item.First().TenBN,
                        NgaySinh = item.First().NgaySinh,
                        ThangSinh = item.First().ThangSinh,
                        NamSinh = item.First().NamSinh,
                        SoDienThoai = item.First().SoDienThoai,
                        DiaChi = item.First().DiaChi,
                        ThanhTien = item.Sum(a => a.ThanhTien),
                        GiamPhi = item.Any(a => a.GiamPhi != null && a.GiamPhi != 0)
                            ? item.Sum(a => a.GiamPhi)
                            : (decimal?)null,
                        MienPhi = item.Any(a => a.MienPhi != null && a.MienPhi != 0)
                            ? item.Sum(a => a.MienPhi)
                            : (decimal?)null,
                        TongGiamPhiMienPhi = item.Any(a => a.TongGiamPhiMienPhiItem != null && a.TongGiamPhiMienPhiItem != 0)
                            ? item.Sum(a => a.TongGiamPhiMienPhiItem)
                            : (decimal?)null,
                        LyDo = string.Join("; ", item.Where(a => !string.IsNullOrEmpty(a.LyDo)).Select(a => a.LyDo).Distinct().ToList()),

                            //BVHD-3918
                            LaHinhThucDenGioiThieu = item.Any(a => a.LaHinhThucDenGioiThieu != null && a.LaHinhThucDenGioiThieu == true),
                        ThongTinYeuCaus = item
                            .Where(a => a.YeuCauDichVuId != null
                                        && a.NhomDichVu != null)
                            .Select(a => new ThongTinYeuCauDichVuVo()
                            {
                                YeuCauTiepNhanId = a.YeuCauTiepNhanId,
                                YeuCauDichVuId = a.YeuCauDichVuId,
                                NhomDichVu = a.NhomDichVu
                            }).ToList()
                    })
                    .Skip(queryInfo.Skip).Take(queryInfo.Take)
                    .ToList();

                #region BVHD-3918: Cập nhật bổ sung lý do: theo nơi giới thiệu, gói khuyến mãi

                #region Nơi giới thiệu
                var lstYeuCauTiepNhanId = phieuThus.Where(x => x.LaHinhThucDenGioiThieu != null
                                                               && x.LaHinhThucDenGioiThieu == true)
                    .Select(x => x.YeuCauTiepNhanId).Distinct().ToList();

                var noiGioiThieus = _yeuCauTiepNhanRepository.TableNoTracking
                    .Where(x => lstYeuCauTiepNhanId.Contains(x.Id)
                                && x.HinhThucDenId == hinhThucDenGioiThieuId
                                && x.NoiGioiThieuId != null)
                    .Select(x => new LookupItemVo()
                    {
                        KeyId = x.Id,
                        DisplayName = x.NoiGioiThieu.Ten
                    }).Distinct().ToList();
                #endregion

                #region Gói khuyến mãi

                var lstThongTinKhuyenMai = new List<ThongTinYeuCauDichVuVo>();

                var lstYeuCauKhamBenhId = phieuThus.SelectMany(x => x.ThongTinYeuCaus)
                    .Where(x => x.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKhamBenh)
                    .Select(x => x.YeuCauDichVuId.Value).Distinct().ToList();
                if (lstYeuCauKhamBenhId.Any())
                {
                    lstThongTinKhuyenMai.AddRange(_mienGiamChiPhiRepository.TableNoTracking
                        .Where(x => x.YeuCauKhamBenhId != null
                                    && x.DaHuy != true
                                    && x.TaiKhoanBenhNhanThu.DaHuy != true
                                    //Cập nhật 06/06/2022: hiển thị thêm trường hợp có voucher
                                    && (x.YeuCauGoiDichVuId != null || !string.IsNullOrEmpty(x.MaTheVoucher))
                                    && lstYeuCauKhamBenhId.Contains(x.YeuCauKhamBenhId.Value))
                        .Select(x => new ThongTinYeuCauDichVuVo()
                        {
                            YeuCauTiepNhanId = x.YeuCauTiepNhanId,
                            NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKhamBenh,
                            YeuCauDichVuId = x.YeuCauKhamBenhId.Value,
                            TenGoiKhuyenMai = x.YeuCauGoiDichVu.ChuongTrinhGoiDichVu.Ten,

                            //Cập nhật 06/06/2022: hiển thị thêm trường hợp có voucher
                            MaVoucher = x.MaTheVoucher
                        }).ToList());
                }

                var lstYeuCauKyThuatId = phieuThus.SelectMany(x => x.ThongTinYeuCaus)
                    .Where(x => x.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat)
                    .Select(x => x.YeuCauDichVuId.Value).Distinct().ToList();
                if (lstYeuCauKyThuatId.Any())
                {
                    lstThongTinKhuyenMai.AddRange(_mienGiamChiPhiRepository.TableNoTracking
                        .Where(x => x.YeuCauDichVuKyThuatId != null
                                    && x.DaHuy != true
                                    && x.TaiKhoanBenhNhanThu.DaHuy != true
                                    //Cập nhật 06/06/2022: hiển thị thêm trường hợp có voucher
                                    && (x.YeuCauGoiDichVuId != null || !string.IsNullOrEmpty(x.MaTheVoucher))
                                    && lstYeuCauKyThuatId.Contains(x.YeuCauDichVuKyThuatId.Value))
                        .Select(x => new ThongTinYeuCauDichVuVo()
                        {
                            YeuCauTiepNhanId = x.YeuCauTiepNhanId,
                            NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKyThuat,
                            YeuCauDichVuId = x.YeuCauDichVuKyThuatId.Value,
                            TenGoiKhuyenMai = x.YeuCauGoiDichVu.ChuongTrinhGoiDichVu.Ten,

                            //Cập nhật 06/06/2022: hiển thị thêm trường hợp có voucher
                            MaVoucher = x.MaTheVoucher
                        }).ToList());
                }

                var lstYeuCauGuongId = phieuThus.SelectMany(x => x.ThongTinYeuCaus)
                    .Where(x => x.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuGiuongBenh)
                    .Select(x => x.YeuCauDichVuId.Value).Distinct().ToList();
                if (lstYeuCauGuongId.Any())
                {
                    lstThongTinKhuyenMai.AddRange(_mienGiamChiPhiRepository.TableNoTracking
                        .Where(x => x.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId != null
                                    && x.DaHuy != true
                                    && x.TaiKhoanBenhNhanThu.DaHuy != true
                                    //Cập nhật 06/06/2022: hiển thị thêm trường hợp có voucher
                                    && (x.YeuCauGoiDichVuId != null || !string.IsNullOrEmpty(x.MaTheVoucher))
                                    && lstYeuCauGuongId.Contains(x.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId.Value))
                        .Select(x => new ThongTinYeuCauDichVuVo()
                        {
                            YeuCauTiepNhanId = x.YeuCauTiepNhanId,
                            NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuGiuongBenh,
                            YeuCauDichVuId = x.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId.Value,
                            TenGoiKhuyenMai = x.YeuCauGoiDichVu.ChuongTrinhGoiDichVu.Ten,

                            //Cập nhật 06/06/2022: hiển thị thêm trường hợp có voucher
                            MaVoucher = x.MaTheVoucher
                        }).ToList());
                }

                #endregion

                //Cập nhật 06/06/2022: hiển thị thêm trường hợp có voucher
                //if (noiGioiThieus.Any())
                //{
                foreach (var phieuThu in phieuThus)
                {
                    if (phieuThu.LaHinhThucDenGioiThieu != null && phieuThu.LaHinhThucDenGioiThieu == true)
                    {
                        var thongTinGioiThieu = noiGioiThieus.FirstOrDefault(x => x.KeyId == phieuThu.YeuCauTiepNhanId);
                        if (thongTinGioiThieu != null)
                        {
                            phieuThu.LyDo = $"Dịch vụ khuyến mại của BS hợp tác - {thongTinGioiThieu.DisplayName}" + (!string.IsNullOrEmpty(phieuThu.LyDo) ? "; " : "") + phieuThu.LyDo;
                        }
                    }

                    if (phieuThu.ThongTinYeuCaus.Any())
                    {
                        var lstYeuCauKhamBenhIdTheoPhieuThu = phieuThu.ThongTinYeuCaus
                            .Where(x => x.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKhamBenh)
                            .Select(x => x.YeuCauDichVuId.Value).Distinct().ToList();
                        var lstYeuCauKyThuatIdTheoPhieuThu = phieuThu.ThongTinYeuCaus
                            .Where(x => x.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat)
                            .Select(x => x.YeuCauDichVuId.Value).Distinct().ToList();
                        var lstYeuCauGuongIdTheoPhieuThu = phieuThu.ThongTinYeuCaus
                            .Where(x => x.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuGiuongBenh)
                            .Select(x => x.YeuCauDichVuId.Value).Distinct().ToList();

                        var thongTinKhuyenMaiTheoPhieuThus = lstThongTinKhuyenMai
                            .Where(x =>
                                        //Cập nhật 06/06/2022: hiển thị thêm trường hợp có voucher
                                        ((x.YeuCauDichVuId != null && !string.IsNullOrEmpty(x.TenGoiKhuyenMai))
                                         || !string.IsNullOrEmpty(x.MaVoucher))

                                        && ((x.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKhamBenh && lstYeuCauKhamBenhIdTheoPhieuThu.Contains(x.YeuCauDichVuId.Value))
                                            || (x.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat && lstYeuCauKyThuatIdTheoPhieuThu.Contains(x.YeuCauDichVuId.Value))
                                            || (x.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuGiuongBenh && lstYeuCauGuongIdTheoPhieuThu.Contains(x.YeuCauDichVuId.Value))))
                            //Cập nhật 06/06/2022: hiển thị thêm trường hợp có voucher
                            //.Select(x => x.TenGoiKhuyenMai)
                            //.Distinct()
                            .ToList();

                        if (thongTinKhuyenMaiTheoPhieuThus.Any())
                        {
                            //Cập nhật 06/06/2022: hiển thị thêm trường hợp có voucher
                            //phieuThu.LyDo += (!string.IsNullOrEmpty(phieuThu.LyDo) ? "; " : "") + $"Dịch vụ khuyến mại của gói {string.Join(", ", thongTinKhuyenMaiTheoPhieuThus)}";

                            var lstThongTinKhuyenMaiGoi = thongTinKhuyenMaiTheoPhieuThus.Where(x => !string.IsNullOrEmpty(x.TenGoiKhuyenMai)).Select(x => x.TenGoiKhuyenMai).Distinct().ToList();
                            var lstThongTinKhuyenMaiVoucher = thongTinKhuyenMaiTheoPhieuThus.Where(x => !string.IsNullOrEmpty(x.MaVoucher)).Select(x => x.MaVoucher).Distinct().ToList();

                            if (lstThongTinKhuyenMaiGoi.Any())
                            {
                                phieuThu.LyDo += (!string.IsNullOrEmpty(phieuThu.LyDo) ? "; " : "") + $"Dịch vụ khuyến mại của gói {string.Join(", ", lstThongTinKhuyenMaiGoi)}";
                            }

                            if (lstThongTinKhuyenMaiVoucher.Any())
                            {
                                phieuThu.LyDo += (!string.IsNullOrEmpty(phieuThu.LyDo) ? "; " : "") + $"Dịch vụ khuyến mại của voucher {string.Join(", ", lstThongTinKhuyenMaiVoucher)}";
                            }
                        }
                    }
                }
                //}
                #endregion
            }

            return new GridDataSource
            {
                Data = phieuThus.ToArray(),
                TotalRowCount = phieuThus.Count()
            };
        }
        public async Task<GridDataSource> GetDataBaoCaoChiTietMienPhiTronVienForGridAsyncOld(QueryInfo queryInfo)
        {
            var phieuThus = new List<BaoCaoChiTietMienPhiTronVienGridVo>();
            var timKiemNangCaoObj = new BaoCaoChiTietMienPhiTronVienQueryInfo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoChiTietMienPhiTronVienQueryInfo>(queryInfo.AdditionalSearchString);
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
                var lyDoKhongTinhPhiDuocPhamVatTu = "Miễn phí thuốc/VTYT của dịch vụ {0}";
                var khoaDuocKhoaPhong = _KhoaPhongRepository.TableNoTracking
                    .FirstOrDefault(x => x.Ma.Contains("KD"));
                var khoaDuoc = khoaDuocKhoaPhong.Ten ?? "Khoa dược";

                #region BVHD-3918: Cập nhật bổ sung lý do -> trường hợp có nơi giới thiệu
                var hinhThucDenGioiThieu = _cauHinhService.GetSetting("CauHinhBaoCao.HinhThucDenGioiThieu");
                long.TryParse(hinhThucDenGioiThieu?.Value, out long hinhThucDenGioiThieuId);
                #endregion

                #region Cập nhật 07/06/2022: tách sub query để hạn chế truy vấn
                var benhNhanChis = _taiKhoanBenhNhanChiRepository.TableNoTracking
                    .Where(x => x.DaHuy != true
                                && (
                                    x.YeuCauKhamBenhId != null
                                    || x.YeuCauDichVuKyThuatId != null
                                    || x.YeuCauDichVuGiuongBenhVienId != null
                                    || x.YeuCauDuocPhamBenhVienId != null
                                    || x.YeuCauVatTuBenhVienId != null
                                    || x.DonThuocThanhToanChiTietId != null
                                    || x.DonVTYTThanhToanChiTietId != null
                                    || x.YeuCauTruyenMauId != null
                                )
                                && x.NgayChi >= tuNgay)
                                //&& x.NgayChi <= denNgay)
                    .ToList();

                var lstBenhNhanChiTimKiems = benhNhanChis.Where(x => x.NgayChi <= denNgay).ToList();
                var lstBenhNhanChiNamNgoaiTimKiems = benhNhanChis.Where(x => x.NgayChi > denNgay).ToList();

                #region lst YCDV ID tìm kiếm
                var lstYCKhamId = lstBenhNhanChiTimKiems.Where(x => x.YeuCauKhamBenhId != null).Select(x => x.YeuCauKhamBenhId.Value).Distinct().ToList();
                var lstYCKyThuatId = lstBenhNhanChiTimKiems.Where(x => x.YeuCauDichVuKyThuatId != null).Select(x => x.YeuCauDichVuKyThuatId.Value).Distinct().ToList();
                var lstYCGiuongId = lstBenhNhanChiTimKiems.Where(x => x.YeuCauDichVuGiuongBenhVienId != null).Select(x => x.YeuCauDichVuGiuongBenhVienId.Value).Distinct().ToList();
                var lstYCDuocPhamId = lstBenhNhanChiTimKiems.Where(x => x.YeuCauDuocPhamBenhVienId != null).Select(x => x.YeuCauDuocPhamBenhVienId.Value).Distinct().ToList();
                var lstYCVatTuId = lstBenhNhanChiTimKiems.Where(x => x.YeuCauVatTuBenhVienId != null).Select(x => x.YeuCauVatTuBenhVienId.Value).Distinct().ToList();
                var lstDonThuocId = lstBenhNhanChiTimKiems.Where(x => x.DonThuocThanhToanChiTietId != null).Select(x => x.DonThuocThanhToanChiTietId.Value).Distinct().ToList();
                var lstDonVTId = lstBenhNhanChiTimKiems.Where(x => x.DonVTYTThanhToanChiTietId != null).Select(x => x.DonVTYTThanhToanChiTietId.Value).Distinct().ToList();
                var lstYCTruyenMauId = lstBenhNhanChiTimKiems.Where(x => x.YeuCauTruyenMauId != null).Select(x => x.YeuCauTruyenMauId.Value).Distinct().ToList();
                #endregion

                #region lst YCDV ID nằm ngoài tìm kiếm
                var lstYCKhamNgoaiTimKiemId = lstBenhNhanChiNamNgoaiTimKiems.Where(x => x.YeuCauKhamBenhId != null).Select(x => x.YeuCauKhamBenhId.Value).Distinct().ToList();
                var lstYCKyThuatNgoaiTimKiemId = lstBenhNhanChiNamNgoaiTimKiems.Where(x => x.YeuCauDichVuKyThuatId != null).Select(x => x.YeuCauDichVuKyThuatId.Value).Distinct().ToList();
                var lstYCGiuongNgoaiTimKiemId = lstBenhNhanChiNamNgoaiTimKiems.Where(x => x.YeuCauDichVuGiuongBenhVienId != null).Select(x => x.YeuCauDichVuGiuongBenhVienId.Value).Distinct().ToList();
                var lstYCDuocPhamNgoaiTimKiemId = lstBenhNhanChiNamNgoaiTimKiems.Where(x => x.YeuCauDuocPhamBenhVienId != null).Select(x => x.YeuCauDuocPhamBenhVienId.Value).Distinct().ToList();
                var lstYCVatTuNgoaiTimKiemId = lstBenhNhanChiNamNgoaiTimKiems.Where(x => x.YeuCauVatTuBenhVienId != null).Select(x => x.YeuCauVatTuBenhVienId.Value).Distinct().ToList();
                var lstDonThuocNgoaiTimKiemId = lstBenhNhanChiNamNgoaiTimKiems.Where(x => x.DonThuocThanhToanChiTietId != null).Select(x => x.DonThuocThanhToanChiTietId.Value).Distinct().ToList();
                var lstDonVTNgoaiTimKiemId = lstBenhNhanChiNamNgoaiTimKiems.Where(x => x.DonVTYTThanhToanChiTietId != null).Select(x => x.DonVTYTThanhToanChiTietId.Value).Distinct().ToList();
                var lstYCTruyenMauNgoaiTimKiemId = lstBenhNhanChiNamNgoaiTimKiems.Where(x => x.YeuCauTruyenMauId != null).Select(x => x.YeuCauTruyenMauId.Value).Distinct().ToList();
                #endregion

                #endregion
                /*
                var a1 = _yeuCauKhamBenhRepository.TableNoTracking
                        .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham

                                    //15/12/2021: bổ sung thêm dv ko tính phí
                                    && ((x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                        && x.SoTienMienGiam != null
                                        && x.SoTienMienGiam > 0

                                        && x.TaiKhoanBenhNhanChis.Any(a => a.DaHuy != true && a.NgayChi >= tuNgay && a.NgayChi <= denNgay)
                                        //&& x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi >= tuNgay && a.NgayChi <= denNgay)
                                        //&& !x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi > denNgay))
                                        //&& lstYCKhamId.Contains(x.Id)
                                        //&& !lstYCKhamNgoaiTimKiemId.Contains(x.Id)
                                        )
                                    || (x.KhongTinhPhi != null && x.KhongTinhPhi == true && x.ThoiDiemChiDinh >= tuNgay && x.ThoiDiemChiDinh <= denNgay))
                                    )
                        .Select(item => new BaoCaoChiTietMienPhiTronVienGridVo()
                        {
                            YeuCauTiepNhanId = item.YeuCauTiepNhanId,
                            TenKhoaPhong = item.NoiChiDinh.KhoaPhong.Ten,
                            MaNB = item.YeuCauTiepNhan.BenhNhan.MaBN,
                            MaTN = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                            TenBN = item.YeuCauTiepNhan.HoTen,
                            NgaySinh = item.YeuCauTiepNhan.NgaySinh,
                            ThangSinh = item.YeuCauTiepNhan.ThangSinh,
                            NamSinh = item.YeuCauTiepNhan.NamSinh,
                            SoDienThoai = item.YeuCauTiepNhan.SoDienThoaiDisplay,
                            DiaChi = item.YeuCauTiepNhan.DiaChiDayDu,
                            ThanhTien = item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia,
                            GiamPhi = item.KhongTinhPhi != true
                                ? (item.SoTienMienGiam < (item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia) ? item.SoTienMienGiam : (decimal?)null)
                                : (decimal?)null,
                            MienPhi = item.KhongTinhPhi != true
                                ? (item.SoTienMienGiam >= (item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia) ? item.SoTienMienGiam : (decimal?)null)
                                : (item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia),
                            SoTienMienGiam = item.SoTienMienGiam,
                            KhongTinhPhi = item.KhongTinhPhi != null && item.KhongTinhPhi == true,
                            //TongGiamPhiMienPhi = item.SoTienMienGiam,
                            LyDo = item.GhiChuMienGiamThem,

                            //BVHD-3918
                            YeuCauDichVuId = item.Id,
                            NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKhamBenh,
                            LaHinhThucDenGioiThieu = item.YeuCauTiepNhan.HinhThucDenId == hinhThucDenGioiThieuId
                        }).ToList();
                var b1 = _yeuCauDichVuKyThuatRepository.TableNoTracking
                                .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy

                                            //15/12/2021: bổ sung thêm dv ko tính phí
                                            && ((x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                                && x.SoTienMienGiam != null
                                                && x.SoTienMienGiam > 0

                                                && x.TaiKhoanBenhNhanChis.Any(a => a.DaHuy != true && a.NgayChi >= tuNgay && a.NgayChi <= denNgay)
                                                //&& x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi >= tuNgay && a.NgayChi <= denNgay)
                                                //&& !x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi > denNgay))
                                                //&& lstYCKyThuatId.Contains(x.Id)
                                                //&& !lstYCKyThuatNgoaiTimKiemId.Contains(x.Id)
                                                )
                                            || (x.KhongTinhPhi != null && x.KhongTinhPhi == true && x.ThoiDiemChiDinh >= tuNgay && x.ThoiDiemChiDinh <= denNgay))
                                    )
                                .Select(item => new BaoCaoChiTietMienPhiTronVienGridVo()
                                {
                                    YeuCauTiepNhanId = item.YeuCauTiepNhanId,
                                    TenKhoaPhong = item.NoiChiDinh.KhoaPhong.Ten,
                                    MaNB = item.YeuCauTiepNhan.BenhNhan.MaBN,
                                    MaTN = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                    TenBN = item.YeuCauTiepNhan.HoTen,
                                    NgaySinh = item.YeuCauTiepNhan.NgaySinh,
                                    ThangSinh = item.YeuCauTiepNhan.ThangSinh,
                                    NamSinh = item.YeuCauTiepNhan.NamSinh,
                                    SoDienThoai = item.YeuCauTiepNhan.SoDienThoaiDisplay,
                                    DiaChi = item.YeuCauTiepNhan.DiaChiDayDu,
                                    ThanhTien = (item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia) * item.SoLan,
                                    GiamPhi = item.KhongTinhPhi != true
                                        ? (item.SoTienMienGiam < ((item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia) * item.SoLan) ? item.SoTienMienGiam : (decimal?)null)
                                        : (decimal?)null,
                                    MienPhi = item.KhongTinhPhi != true
                                        ? (item.SoTienMienGiam >= ((item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia) * item.SoLan) ? item.SoTienMienGiam : (decimal?)null)
                                        : ((item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia) * item.SoLan),
                                    SoTienMienGiam = item.SoTienMienGiam,
                                    KhongTinhPhi = item.KhongTinhPhi != null && item.KhongTinhPhi == true,
                                    //TongGiamPhiMienPhi = item.SoTienMienGiam,
                                    LyDo = item.GhiChuMienGiamThem,

                                    //BVHD-3918
                                    YeuCauDichVuId = item.Id,
                                    NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKyThuat,
                                    LaHinhThucDenGioiThieu = item.YeuCauTiepNhan.HinhThucDenId == hinhThucDenGioiThieuId
                                }).ToList();
                var c1 = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                            .Where(x => x.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy

                                        //15/12/2021: bổ sung thêm dv ko tính phí
                                        && ((x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                                && x.SoTienMienGiam != null
                                                && x.SoTienMienGiam > 0

                                                && x.TaiKhoanBenhNhanChis.Any(a => a.DaHuy != true && a.NgayChi >= tuNgay && a.NgayChi <= denNgay)
                                                //&& x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi >= tuNgay && a.NgayChi <= denNgay)
                                                //&& !x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi > denNgay))
                                                //&& lstYCDuocPhamId.Contains(x.Id)
                                                //&& !lstYCDuocPhamNgoaiTimKiemId.Contains(x.Id)
                                                )

                                            || (x.KhongTinhPhi != null && x.KhongTinhPhi == true && x.ThoiDiemChiDinh >= tuNgay && x.ThoiDiemChiDinh <= denNgay))
                                        )
                                .Select(item => new BaoCaoChiTietMienPhiTronVienGridVo()
                                {
                                    YeuCauTiepNhanId = item.YeuCauTiepNhanId,
                                    TenKhoaPhong = item.NoiChiDinh.KhoaPhong.Ten,
                                    MaNB = item.YeuCauTiepNhan.BenhNhan.MaBN,
                                    MaTN = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                    TenBN = item.YeuCauTiepNhan.HoTen,
                                    NgaySinh = item.YeuCauTiepNhan.NgaySinh,
                                    ThangSinh = item.YeuCauTiepNhan.ThangSinh,
                                    NamSinh = item.YeuCauTiepNhan.NamSinh,
                                    SoDienThoai = item.YeuCauTiepNhan.SoDienThoaiDisplay,
                                    DiaChi = item.YeuCauTiepNhan.DiaChiDayDu,
                                    ThanhTien = item.GiaBan,
                                    GiamPhi = item.KhongTinhPhi != true
                                        ? (item.SoTienMienGiam < item.GiaBan ? item.SoTienMienGiam : (decimal?)null)
                                        : (decimal?)null,
                                    MienPhi = item.KhongTinhPhi != true
                                        ? (item.SoTienMienGiam >= item.GiaBan ? item.SoTienMienGiam : (decimal?)null)
                                        : item.GiaBan,
                                    SoTienMienGiam = item.SoTienMienGiam,
                                    KhongTinhPhi = item.KhongTinhPhi != null && item.KhongTinhPhi == true,
                                    //TongGiamPhiMienPhi = item.SoTienMienGiam,
                                    LyDo = (item.KhongTinhPhi != null && item.KhongTinhPhi == true && item.YeuCauDichVuKyThuatId != null)
                                        ? string.Format(lyDoKhongTinhPhiDuocPhamVatTu, item.YeuCauDichVuKyThuat.TenDichVu) : item.GhiChuMienGiamThem,

                                    //BVHD-3918
                                    LaHinhThucDenGioiThieu = item.YeuCauTiepNhan.HinhThucDenId == hinhThucDenGioiThieuId
                                }).ToList();
                var d1 = _yeuCauVatTuBenhVienRepository.TableNoTracking
                                .Where(x => x.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy

                                            //15/12/2021: bổ sung thêm dv ko tính phí
                                            && ((x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                                && x.SoTienMienGiam != null
                                                && x.SoTienMienGiam > 0

                                                && x.TaiKhoanBenhNhanChis.Any(a => a.DaHuy != true && a.NgayChi >= tuNgay && a.NgayChi <= denNgay)
                                                //&& x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi >= tuNgay && a.NgayChi <= denNgay)
                                                //&& !x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi > denNgay))
                                                //&& lstYCVatTuId.Contains(x.Id)
                                                //&& !lstYCVatTuNgoaiTimKiemId.Contains(x.Id)
                                                )

                                            || (x.KhongTinhPhi != null && x.KhongTinhPhi == true && x.ThoiDiemChiDinh >= tuNgay && x.ThoiDiemChiDinh <= denNgay))
                                            )
                                .Select(item => new BaoCaoChiTietMienPhiTronVienGridVo()
                                {
                                    YeuCauTiepNhanId = item.YeuCauTiepNhanId,
                                    TenKhoaPhong = item.NoiChiDinh.KhoaPhong.Ten,
                                    MaNB = item.YeuCauTiepNhan.BenhNhan.MaBN,
                                    MaTN = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                    TenBN = item.YeuCauTiepNhan.HoTen,
                                    NgaySinh = item.YeuCauTiepNhan.NgaySinh,
                                    ThangSinh = item.YeuCauTiepNhan.ThangSinh,
                                    NamSinh = item.YeuCauTiepNhan.NamSinh,
                                    SoDienThoai = item.YeuCauTiepNhan.SoDienThoaiDisplay,
                                    DiaChi = item.YeuCauTiepNhan.DiaChiDayDu,
                                    ThanhTien = item.GiaBan,
                                    GiamPhi = item.KhongTinhPhi != true
                                        ? (item.SoTienMienGiam < item.GiaBan ? item.SoTienMienGiam : (decimal?)null)
                                        : (decimal?)null,
                                    MienPhi = item.KhongTinhPhi != true
                                        ? (item.SoTienMienGiam >= item.GiaBan ? item.SoTienMienGiam : (decimal?)null)
                                        : item.GiaBan,
                                    SoTienMienGiam = item.SoTienMienGiam,
                                    KhongTinhPhi = item.KhongTinhPhi != null && item.KhongTinhPhi == true,
                                    //TongGiamPhiMienPhi = item.SoTienMienGiam,
                                    LyDo = (item.KhongTinhPhi != null && item.KhongTinhPhi == true && item.YeuCauDichVuKyThuatId != null)
                                        ? string.Format(lyDoKhongTinhPhiDuocPhamVatTu, item.YeuCauDichVuKyThuat.TenDichVu) : item.GhiChuMienGiamThem,

                                    //BVHD-3918
                                    LaHinhThucDenGioiThieu = item.YeuCauTiepNhan.HinhThucDenId == hinhThucDenGioiThieuId
                                }).ToList();
                var e1 = _yeuCauDichVuGiuongBenhVienChiPhiBenhVienRepository.TableNoTracking
                                .Where(x => x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                            && x.SoTienMienGiam != null
                                            && x.SoTienMienGiam > 0

                                            && x.TaiKhoanBenhNhanChis.Any(a => a.DaHuy != true && a.NgayChi >= tuNgay && a.NgayChi <= denNgay)
                                            //&& x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi >= tuNgay && a.NgayChi <= denNgay)
                                            //&& !x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi > denNgay))
                                            //&& lstYCGiuongId.Contains(x.Id)
                                            //&& !lstYCGiuongNgoaiTimKiemId.Contains(x.Id)
                                            )

                                .Select(item => new BaoCaoChiTietMienPhiTronVienGridVo()
                                {
                                    YeuCauTiepNhanId = item.YeuCauTiepNhanId,
                                    TenKhoaPhong = item.KhoaPhong.Ten,
                                    MaNB = item.YeuCauTiepNhan.BenhNhan.MaBN,
                                    MaTN = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                    TenBN = item.YeuCauTiepNhan.HoTen,
                                    NgaySinh = item.YeuCauTiepNhan.NgaySinh,
                                    ThangSinh = item.YeuCauTiepNhan.ThangSinh,
                                    NamSinh = item.YeuCauTiepNhan.NamSinh,
                                    SoDienThoai = item.YeuCauTiepNhan.SoDienThoaiDisplay,
                                    DiaChi = item.YeuCauTiepNhan.DiaChiDayDu,
                                    ThanhTien = Convert.ToDecimal(item.SoLuong * (double)(item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau ?? 0 : item.Gia)),
                                    GiamPhi = item.SoTienMienGiam < (Convert.ToDecimal((double)((item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia) ?? 0) * item.SoLuong)) ? item.SoTienMienGiam : (decimal?)null,
                                    MienPhi = item.SoTienMienGiam >= (Convert.ToDecimal((double)((item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia) ?? 0) * item.SoLuong)) ? item.SoTienMienGiam : (decimal?)null,
                                    SoTienMienGiam = item.SoTienMienGiam,
                                    //TongGiamPhiMienPhi = item.SoTienMienGiam,
                                    LyDo = item.GhiChuMienGiamThem,

                                    //BVHD-3918
                                    YeuCauDichVuId = item.Id,
                                    NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuGiuongBenh,
                                    LaHinhThucDenGioiThieu = item.YeuCauTiepNhan.HinhThucDenId == hinhThucDenGioiThieuId
                                }).ToList();

                var f1 = _donThuocThanhToanChiTietRepository.TableNoTracking
                                .Where(x => x.DonThuocThanhToan.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                            && x.SoTienMienGiam != null
                                            && x.SoTienMienGiam > 0
                                            && x.TaiKhoanBenhNhanChis.Any(a => a.DaHuy != true && a.NgayChi >= tuNgay && a.NgayChi <= denNgay)
                                            //&& x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi >= tuNgay && a.NgayChi <= denNgay)
                                            //&& !x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi > denNgay))
                                            //&& lstDonThuocId.Contains(x.Id)
                                            //&& !lstDonThuocNgoaiTimKiemId.Contains(x.Id)
                                            )

                                .Select(item => new BaoCaoChiTietMienPhiTronVienGridVo()
                                {
                                    YeuCauTiepNhanId = item.DonThuocThanhToan.YeuCauTiepNhanId ?? 0,
                                    TenKhoaPhong = item.YeuCauKhamBenhDonThuocChiTiet.YeuCauKhamBenhDonThuoc.NoiKeDon.KhoaPhong.Ten ?? item.NoiTruDonThuocChiTiet.NoiTruDonThuoc.NoiKeDon.KhoaPhong.Ten ?? khoaDuoc,
                                    MaNB = item.DonThuocThanhToan.YeuCauTiepNhan.BenhNhan.MaBN,
                                    MaTN = item.DonThuocThanhToan.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                    TenBN = item.DonThuocThanhToan.YeuCauTiepNhan.HoTen,
                                    NgaySinh = item.DonThuocThanhToan.YeuCauTiepNhan.NgaySinh,
                                    ThangSinh = item.DonThuocThanhToan.YeuCauTiepNhan.ThangSinh,
                                    NamSinh = item.DonThuocThanhToan.YeuCauTiepNhan.NamSinh,
                                    SoDienThoai = item.DonThuocThanhToan.YeuCauTiepNhan.SoDienThoaiDisplay,
                                    DiaChi = item.DonThuocThanhToan.YeuCauTiepNhan.DiaChiDayDu,
                                    ThanhTien = item.GiaBan,
                                    GiamPhi = item.SoTienMienGiam < item.GiaBan ? item.SoTienMienGiam : (decimal?)null,
                                    MienPhi = item.SoTienMienGiam >= item.GiaBan ? item.SoTienMienGiam : (decimal?)null,
                                    SoTienMienGiam = item.SoTienMienGiam,
                                    //TongGiamPhiMienPhi = item.SoTienMienGiam,
                                    LyDo = item.GhiChuMienGiamThem,

                                    //BVHD-3918
                                    LaHinhThucDenGioiThieu = item.DonThuocThanhToan.YeuCauTiepNhan.HinhThucDenId == hinhThucDenGioiThieuId
                                }).ToList();

                var g1 = _donVTYTThanhToanChiTietRepository.TableNoTracking
                                .Where(x => x.DonVTYTThanhToan.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                            && x.SoTienMienGiam != null
                                            && x.SoTienMienGiam > 0

                                            && x.TaiKhoanBenhNhanChis.Any(a => a.DaHuy != true && a.NgayChi >= tuNgay && a.NgayChi <= denNgay)
                                            //&& x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi >= tuNgay && a.NgayChi <= denNgay)
                                            //&& !x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi > denNgay))
                                            //&& lstDonVTId.Contains(x.Id)
                                            //&& !lstDonVTNgoaiTimKiemId.Contains(x.Id)
                                            )

                                .Select(item => new BaoCaoChiTietMienPhiTronVienGridVo()
                                {
                                    YeuCauTiepNhanId = item.DonVTYTThanhToan.YeuCauTiepNhanId ?? 0,
                                    TenKhoaPhong = item.YeuCauKhamBenhDonVTYTChiTiet.YeuCauKhamBenhDonVTYT.NoiKeDon.KhoaPhong.Ten ?? khoaDuoc,
                                    MaNB = item.DonVTYTThanhToan.YeuCauTiepNhan.BenhNhan.MaBN,
                                    MaTN = item.DonVTYTThanhToan.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                    TenBN = item.DonVTYTThanhToan.YeuCauTiepNhan.HoTen,
                                    NgaySinh = item.DonVTYTThanhToan.YeuCauTiepNhan.NgaySinh,
                                    ThangSinh = item.DonVTYTThanhToan.YeuCauTiepNhan.ThangSinh,
                                    NamSinh = item.DonVTYTThanhToan.YeuCauTiepNhan.NamSinh,
                                    SoDienThoai = item.DonVTYTThanhToan.YeuCauTiepNhan.SoDienThoaiDisplay,
                                    DiaChi = item.DonVTYTThanhToan.YeuCauTiepNhan.DiaChiDayDu,
                                    ThanhTien = item.GiaBan,
                                    GiamPhi = item.SoTienMienGiam < item.GiaBan ? item.SoTienMienGiam : (decimal?)null,
                                    MienPhi = item.SoTienMienGiam >= item.GiaBan ? item.SoTienMienGiam : (decimal?)null,
                                    SoTienMienGiam = item.SoTienMienGiam,
                                    //TongGiamPhiMienPhi = item.SoTienMienGiam,
                                    LyDo = item.GhiChuMienGiamThem,

                                    //BVHD-3918
                                    LaHinhThucDenGioiThieu = item.DonVTYTThanhToan.YeuCauTiepNhan.HinhThucDenId == hinhThucDenGioiThieuId
                                }).ToList();
                var h1 = _yeuCauTruyenMauRepository.TableNoTracking
                                .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauTruyenMau.DaHuy
                                            && x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                            && x.SoTienMienGiam != null
                                            && x.SoTienMienGiam > 0
                                            && x.TaiKhoanBenhNhanChis.Any(a => a.DaHuy != true && a.NgayChi >= tuNgay && a.NgayChi <= denNgay)

                                            //&& x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi >= tuNgay && a.NgayChi <= denNgay)
                                            //&& !x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi > denNgay))
                                            //&& lstYCTruyenMauId.Contains(x.Id)
                                            //&& lstYCTruyenMauNgoaiTimKiemId.Contains(x.Id)
                                            )

                                .Select(item => new BaoCaoChiTietMienPhiTronVienGridVo()
                                {
                                    YeuCauTiepNhanId = item.YeuCauTiepNhanId,
                                    TenKhoaPhong = item.NoiChiDinh.KhoaPhong.Ten,
                                    MaNB = item.YeuCauTiepNhan.BenhNhan.MaBN,
                                    MaTN = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                    TenBN = item.YeuCauTiepNhan.HoTen,
                                    NgaySinh = item.YeuCauTiepNhan.NgaySinh,
                                    ThangSinh = item.YeuCauTiepNhan.ThangSinh,
                                    NamSinh = item.YeuCauTiepNhan.NamSinh,
                                    SoDienThoai = item.YeuCauTiepNhan.SoDienThoaiDisplay,
                                    DiaChi = item.YeuCauTiepNhan.DiaChiDayDu,
                                    ThanhTien = item.DonGiaBan,
                                    GiamPhi = item.SoTienMienGiam < item.DonGiaBan ? item.SoTienMienGiam : (decimal?)null,
                                    MienPhi = item.SoTienMienGiam >= item.DonGiaBan ? item.SoTienMienGiam : (decimal?)null,
                                    SoTienMienGiam = item.SoTienMienGiam,
                                    //TongGiamPhiMienPhi = item.SoTienMienGiam,
                                    LyDo = item.GhiChuMienGiamThem,

                                    //BVHD-3918
                                    LaHinhThucDenGioiThieu = item.YeuCauTiepNhan.HinhThucDenId == hinhThucDenGioiThieuId
                                }).ToList();
                */




                phieuThus = _yeuCauKhamBenhRepository.TableNoTracking
                        .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham

                                    //15/12/2021: bổ sung thêm dv ko tính phí
                                    && ((x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                        && x.SoTienMienGiam != null
                                        && x.SoTienMienGiam > 0

                                        && x.TaiKhoanBenhNhanChis.Any(a => a.DaHuy != true && a.NgayChi >= tuNgay && a.NgayChi <= denNgay)
                                        //&& x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi >= tuNgay && a.NgayChi <= denNgay)
                                        //&& !x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi > denNgay))
                                        //&& lstYCKhamId.Contains(x.Id)
                                        //&& !lstYCKhamNgoaiTimKiemId.Contains(x.Id)
                                        )

                                    || (x.KhongTinhPhi != null && x.KhongTinhPhi == true && x.ThoiDiemChiDinh >= tuNgay && x.ThoiDiemChiDinh <= denNgay))
                                    )
                        .Select(item => new BaoCaoChiTietMienPhiTronVienGridVo()
                        {
                            YeuCauTiepNhanId = item.YeuCauTiepNhanId,
                            TenKhoaPhong = item.NoiChiDinh.KhoaPhong.Ten,
                            MaNB = item.YeuCauTiepNhan.BenhNhan.MaBN,
                            MaTN = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                            TenBN = item.YeuCauTiepNhan.HoTen,
                            NgaySinh = item.YeuCauTiepNhan.NgaySinh,
                            ThangSinh = item.YeuCauTiepNhan.ThangSinh,
                            NamSinh = item.YeuCauTiepNhan.NamSinh,
                            SoDienThoai = item.YeuCauTiepNhan.SoDienThoaiDisplay,
                            DiaChi = item.YeuCauTiepNhan.DiaChiDayDu,
                            ThanhTien = item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia,
                            GiamPhi = item.KhongTinhPhi != true 
                                ? (item.SoTienMienGiam < (item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia) ? item.SoTienMienGiam : (decimal?)null) 
                                : (decimal?)null,
                            MienPhi = item.KhongTinhPhi != true 
                                ? (item.SoTienMienGiam >= (item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia) ? item.SoTienMienGiam : (decimal?)null) 
                                : (item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia),
                            SoTienMienGiam = item.SoTienMienGiam,
                            KhongTinhPhi = item.KhongTinhPhi != null && item.KhongTinhPhi == true,
                            //TongGiamPhiMienPhi = item.SoTienMienGiam,
                            LyDo = item.GhiChuMienGiamThem,

                            //BVHD-3918
                            YeuCauDichVuId = item.Id,
                            NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKhamBenh,
                            LaHinhThucDenGioiThieu = item.YeuCauTiepNhan.HinhThucDenId == hinhThucDenGioiThieuId
                        })
                        .Union(
                            _yeuCauDichVuKyThuatRepository.TableNoTracking
                                .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy

                                            //15/12/2021: bổ sung thêm dv ko tính phí
                                            && ((x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                                && x.SoTienMienGiam != null
                                                && x.SoTienMienGiam > 0

                                                && x.TaiKhoanBenhNhanChis.Any(a => a.DaHuy != true && a.NgayChi >= tuNgay && a.NgayChi <= denNgay)
                                                //&& x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi >= tuNgay && a.NgayChi <= denNgay)
                                                //&& !x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi > denNgay))
                                                //&& lstYCKyThuatId.Contains(x.Id)
                                                //&& !lstYCKyThuatNgoaiTimKiemId.Contains(x.Id)
                                                )

                                            || (x.KhongTinhPhi != null && x.KhongTinhPhi == true && x.ThoiDiemChiDinh >= tuNgay && x.ThoiDiemChiDinh <= denNgay))
                                    )
                                .Select(item => new BaoCaoChiTietMienPhiTronVienGridVo()
                                {
                                    YeuCauTiepNhanId = item.YeuCauTiepNhanId,
                                    TenKhoaPhong = item.NoiChiDinh.KhoaPhong.Ten,
                                    MaNB = item.YeuCauTiepNhan.BenhNhan.MaBN,
                                    MaTN = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                    TenBN = item.YeuCauTiepNhan.HoTen,
                                    NgaySinh = item.YeuCauTiepNhan.NgaySinh,
                                    ThangSinh = item.YeuCauTiepNhan.ThangSinh,
                                    NamSinh = item.YeuCauTiepNhan.NamSinh,
                                    SoDienThoai = item.YeuCauTiepNhan.SoDienThoaiDisplay,
                                    DiaChi = item.YeuCauTiepNhan.DiaChiDayDu,
                                    ThanhTien = (item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia) * item.SoLan,
                                    GiamPhi = item.KhongTinhPhi != true
                                        ? (item.SoTienMienGiam < ((item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia) * item.SoLan) ? item.SoTienMienGiam : (decimal?)null) 
                                        : (decimal?)null,
                                    MienPhi = item.KhongTinhPhi != true
                                        ? (item.SoTienMienGiam >= ((item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia) * item.SoLan) ? item.SoTienMienGiam : (decimal?)null)
                                        : ((item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia) * item.SoLan),
                                    SoTienMienGiam = item.SoTienMienGiam,
                                    KhongTinhPhi = item.KhongTinhPhi != null && item.KhongTinhPhi == true,
                                    //TongGiamPhiMienPhi = item.SoTienMienGiam,
                                    LyDo = item.GhiChuMienGiamThem,

                                    //BVHD-3918
                                    YeuCauDichVuId = item.Id,
                                    NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKyThuat,
                                    LaHinhThucDenGioiThieu = item.YeuCauTiepNhan.HinhThucDenId == hinhThucDenGioiThieuId
                                })
                            )
                        .Union(
                            _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                            .Where(x => x.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy

                                        //15/12/2021: bổ sung thêm dv ko tính phí
                                        && ((x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                                && x.SoTienMienGiam != null
                                                && x.SoTienMienGiam > 0

                                                && x.TaiKhoanBenhNhanChis.Any(a => a.DaHuy != true && a.NgayChi >= tuNgay && a.NgayChi <= denNgay)
                                                //&& x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi >= tuNgay && a.NgayChi <= denNgay)
                                                //&& !x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi > denNgay))
                                                //&& lstYCDuocPhamId.Contains(x.Id)
                                                //&& !lstYCDuocPhamNgoaiTimKiemId.Contains(x.Id)
                                                )

                                            || (x.KhongTinhPhi != null && x.KhongTinhPhi == true && x.ThoiDiemChiDinh >= tuNgay && x.ThoiDiemChiDinh <= denNgay))
                                        )
                                .Select(item => new BaoCaoChiTietMienPhiTronVienGridVo()
                                {
                                    YeuCauTiepNhanId = item.YeuCauTiepNhanId,
                                    TenKhoaPhong = item.NoiChiDinh.KhoaPhong.Ten,
                                    MaNB = item.YeuCauTiepNhan.BenhNhan.MaBN,
                                    MaTN = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                    TenBN = item.YeuCauTiepNhan.HoTen,
                                    NgaySinh = item.YeuCauTiepNhan.NgaySinh,
                                    ThangSinh = item.YeuCauTiepNhan.ThangSinh,
                                    NamSinh = item.YeuCauTiepNhan.NamSinh,
                                    SoDienThoai = item.YeuCauTiepNhan.SoDienThoaiDisplay,
                                    DiaChi = item.YeuCauTiepNhan.DiaChiDayDu,
                                    ThanhTien = item.GiaBan,
                                    GiamPhi = item.KhongTinhPhi != true
                                        ? (item.SoTienMienGiam < item.GiaBan ? item.SoTienMienGiam : (decimal?)null) 
                                        : (decimal?)null,
                                    MienPhi = item.KhongTinhPhi != true
                                        ? (item.SoTienMienGiam >= item.GiaBan ? item.SoTienMienGiam : (decimal?)null) 
                                        : item.GiaBan,
                                    SoTienMienGiam = item.SoTienMienGiam,
                                    KhongTinhPhi = item.KhongTinhPhi != null && item.KhongTinhPhi == true,
                                    //TongGiamPhiMienPhi = item.SoTienMienGiam,
                                    LyDo = (item.KhongTinhPhi != null && item.KhongTinhPhi == true && item.YeuCauDichVuKyThuatId != null) 
                                        ? string.Format(lyDoKhongTinhPhiDuocPhamVatTu, item.YeuCauDichVuKyThuat.TenDichVu) : item.GhiChuMienGiamThem,

                                    //BVHD-3918
                                    LaHinhThucDenGioiThieu = item.YeuCauTiepNhan.HinhThucDenId == hinhThucDenGioiThieuId
                                })
                            )
                        .Union(
                            _yeuCauVatTuBenhVienRepository.TableNoTracking
                                .Where(x => x.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy

                                            //15/12/2021: bổ sung thêm dv ko tính phí
                                            && ((x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                                && x.SoTienMienGiam != null
                                                && x.SoTienMienGiam > 0

                                                && x.TaiKhoanBenhNhanChis.Any(a => a.DaHuy != true && a.NgayChi >= tuNgay && a.NgayChi <= denNgay)
                                                //&& x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi >= tuNgay && a.NgayChi <= denNgay)
                                                //&& !x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi > denNgay))
                                                //&& lstYCVatTuId.Contains(x.Id)
                                                //&& !lstYCVatTuNgoaiTimKiemId.Contains(x.Id)
                                                )

                                            || (x.KhongTinhPhi != null && x.KhongTinhPhi == true && x.ThoiDiemChiDinh >= tuNgay && x.ThoiDiemChiDinh <= denNgay))
                                            )
                                .Select(item => new BaoCaoChiTietMienPhiTronVienGridVo()
                                {
                                    YeuCauTiepNhanId = item.YeuCauTiepNhanId,
                                    TenKhoaPhong = item.NoiChiDinh.KhoaPhong.Ten,
                                    MaNB = item.YeuCauTiepNhan.BenhNhan.MaBN,
                                    MaTN = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                    TenBN = item.YeuCauTiepNhan.HoTen,
                                    NgaySinh = item.YeuCauTiepNhan.NgaySinh,
                                    ThangSinh = item.YeuCauTiepNhan.ThangSinh,
                                    NamSinh = item.YeuCauTiepNhan.NamSinh,
                                    SoDienThoai = item.YeuCauTiepNhan.SoDienThoaiDisplay,
                                    DiaChi = item.YeuCauTiepNhan.DiaChiDayDu,
                                    ThanhTien = item.GiaBan,
                                    GiamPhi = item.KhongTinhPhi != true
                                        ? (item.SoTienMienGiam < item.GiaBan ? item.SoTienMienGiam : (decimal?)null) 
                                        : (decimal?)null,
                                    MienPhi = item.KhongTinhPhi != true
                                        ? (item.SoTienMienGiam >= item.GiaBan ? item.SoTienMienGiam : (decimal?)null)
                                        : item.GiaBan,
                                    SoTienMienGiam = item.SoTienMienGiam,
                                    KhongTinhPhi = item.KhongTinhPhi != null && item.KhongTinhPhi == true,
                                    //TongGiamPhiMienPhi = item.SoTienMienGiam,
                                    LyDo = (item.KhongTinhPhi != null && item.KhongTinhPhi == true && item.YeuCauDichVuKyThuatId != null)
                                        ? string.Format(lyDoKhongTinhPhiDuocPhamVatTu, item.YeuCauDichVuKyThuat.TenDichVu) : item.GhiChuMienGiamThem,

                                    //BVHD-3918
                                    LaHinhThucDenGioiThieu = item.YeuCauTiepNhan.HinhThucDenId == hinhThucDenGioiThieuId
                                })
                            )
                        .Union(
                            _yeuCauDichVuGiuongBenhVienChiPhiBenhVienRepository.TableNoTracking
                                .Where(x => x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                            && x.SoTienMienGiam != null
                                            && x.SoTienMienGiam > 0

                                            && x.TaiKhoanBenhNhanChis.Any(a => a.DaHuy != true && a.NgayChi >= tuNgay && a.NgayChi <= denNgay)
                                            //&& x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi >= tuNgay && a.NgayChi <= denNgay)
                                            //&& !x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi > denNgay))
                                            //&& lstYCGiuongId.Contains(x.Id)
                                            //&& !lstYCGiuongNgoaiTimKiemId.Contains(x.Id)
                                            )

                                .Select(item => new BaoCaoChiTietMienPhiTronVienGridVo()
                                {
                                    YeuCauTiepNhanId = item.YeuCauTiepNhanId,
                                    TenKhoaPhong = item.KhoaPhong.Ten,
                                    MaNB = item.YeuCauTiepNhan.BenhNhan.MaBN,
                                    MaTN = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                    TenBN = item.YeuCauTiepNhan.HoTen,
                                    NgaySinh = item.YeuCauTiepNhan.NgaySinh,
                                    ThangSinh = item.YeuCauTiepNhan.ThangSinh,
                                    NamSinh = item.YeuCauTiepNhan.NamSinh,
                                    SoDienThoai = item.YeuCauTiepNhan.SoDienThoaiDisplay,
                                    DiaChi = item.YeuCauTiepNhan.DiaChiDayDu,
                                    ThanhTien = Convert.ToDecimal(item.SoLuong * (double)(item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau ?? 0 : item.Gia)),
                                    GiamPhi = item.SoTienMienGiam < (Convert.ToDecimal((double)((item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia) ?? 0) * item.SoLuong)) ? item.SoTienMienGiam : (decimal?)null,
                                    MienPhi = item.SoTienMienGiam >= (Convert.ToDecimal((double)((item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia) ?? 0) * item.SoLuong)) ? item.SoTienMienGiam : (decimal?)null,
                                    SoTienMienGiam = item.SoTienMienGiam,
                                    //TongGiamPhiMienPhi = item.SoTienMienGiam,
                                    LyDo = item.GhiChuMienGiamThem,

                                    //BVHD-3918
                                    YeuCauDichVuId = item.Id,
                                    NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuGiuongBenh,
                                    LaHinhThucDenGioiThieu = item.YeuCauTiepNhan.HinhThucDenId == hinhThucDenGioiThieuId
                                })
                            )
                        .Union(
                            _donThuocThanhToanChiTietRepository.TableNoTracking
                                .Where(x => x.DonThuocThanhToan.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                            && x.SoTienMienGiam != null
                                            && x.SoTienMienGiam > 0
                                            && x.TaiKhoanBenhNhanChis.Any(a => a.DaHuy != true && a.NgayChi >= tuNgay && a.NgayChi <= denNgay)
                                            //&& x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi >= tuNgay && a.NgayChi <= denNgay)
                                            //&& !x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi > denNgay))
                                            //&& lstDonThuocId.Contains(x.Id)
                                            //&& !lstDonThuocNgoaiTimKiemId.Contains(x.Id)
                                            )

                                .Select(item => new BaoCaoChiTietMienPhiTronVienGridVo()
                                {
                                    YeuCauTiepNhanId = item.DonThuocThanhToan.YeuCauTiepNhanId ?? 0,
                                    TenKhoaPhong = item.YeuCauKhamBenhDonThuocChiTiet.YeuCauKhamBenhDonThuoc.NoiKeDon.KhoaPhong.Ten ?? item.NoiTruDonThuocChiTiet.NoiTruDonThuoc.NoiKeDon.KhoaPhong.Ten ?? khoaDuoc,
                                    MaNB = item.DonThuocThanhToan.YeuCauTiepNhan.BenhNhan.MaBN,
                                    MaTN = item.DonThuocThanhToan.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                    TenBN = item.DonThuocThanhToan.YeuCauTiepNhan.HoTen,
                                    NgaySinh = item.DonThuocThanhToan.YeuCauTiepNhan.NgaySinh,
                                    ThangSinh = item.DonThuocThanhToan.YeuCauTiepNhan.ThangSinh,
                                    NamSinh = item.DonThuocThanhToan.YeuCauTiepNhan.NamSinh,
                                    SoDienThoai = item.DonThuocThanhToan.YeuCauTiepNhan.SoDienThoaiDisplay,
                                    DiaChi = item.DonThuocThanhToan.YeuCauTiepNhan.DiaChiDayDu,
                                    ThanhTien = item.GiaBan,
                                    GiamPhi = item.SoTienMienGiam < item.GiaBan ? item.SoTienMienGiam : (decimal?)null,
                                    MienPhi = item.SoTienMienGiam >= item.GiaBan ? item.SoTienMienGiam : (decimal?)null,
                                    SoTienMienGiam = item.SoTienMienGiam,
                                    //TongGiamPhiMienPhi = item.SoTienMienGiam,
                                    LyDo = item.GhiChuMienGiamThem,

                                    //BVHD-3918
                                    LaHinhThucDenGioiThieu = item.DonThuocThanhToan.YeuCauTiepNhan.HinhThucDenId == hinhThucDenGioiThieuId
                                })
                            )
                        .Union(
                            _donVTYTThanhToanChiTietRepository.TableNoTracking
                                .Where(x => x.DonVTYTThanhToan.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                            && x.SoTienMienGiam != null
                                            && x.SoTienMienGiam > 0

                                            && x.TaiKhoanBenhNhanChis.Any(a => a.DaHuy != true && a.NgayChi >= tuNgay && a.NgayChi <= denNgay)
                                            //&& x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi >= tuNgay && a.NgayChi <= denNgay)
                                            //&& !x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi > denNgay))
                                            //&& lstDonVTId.Contains(x.Id)
                                            //&& !lstDonVTNgoaiTimKiemId.Contains(x.Id)
                                            )

                                .Select(item => new BaoCaoChiTietMienPhiTronVienGridVo()
                                {
                                    YeuCauTiepNhanId = item.DonVTYTThanhToan.YeuCauTiepNhanId ?? 0,
                                    TenKhoaPhong = item.YeuCauKhamBenhDonVTYTChiTiet.YeuCauKhamBenhDonVTYT.NoiKeDon.KhoaPhong.Ten ?? khoaDuoc,
                                    MaNB = item.DonVTYTThanhToan.YeuCauTiepNhan.BenhNhan.MaBN,
                                    MaTN = item.DonVTYTThanhToan.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                    TenBN = item.DonVTYTThanhToan.YeuCauTiepNhan.HoTen,
                                    NgaySinh = item.DonVTYTThanhToan.YeuCauTiepNhan.NgaySinh,
                                    ThangSinh = item.DonVTYTThanhToan.YeuCauTiepNhan.ThangSinh,
                                    NamSinh = item.DonVTYTThanhToan.YeuCauTiepNhan.NamSinh,
                                    SoDienThoai = item.DonVTYTThanhToan.YeuCauTiepNhan.SoDienThoaiDisplay,
                                    DiaChi = item.DonVTYTThanhToan.YeuCauTiepNhan.DiaChiDayDu,
                                    ThanhTien = item.GiaBan,
                                    GiamPhi = item.SoTienMienGiam < item.GiaBan ? item.SoTienMienGiam : (decimal?)null,
                                    MienPhi = item.SoTienMienGiam >= item.GiaBan ? item.SoTienMienGiam : (decimal?)null,
                                    SoTienMienGiam = item.SoTienMienGiam,
                                    //TongGiamPhiMienPhi = item.SoTienMienGiam,
                                    LyDo = item.GhiChuMienGiamThem,

                                    //BVHD-3918
                                    LaHinhThucDenGioiThieu = item.DonVTYTThanhToan.YeuCauTiepNhan.HinhThucDenId == hinhThucDenGioiThieuId
                                })
                            )
                        .Union(
                            _yeuCauTruyenMauRepository.TableNoTracking
                                .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauTruyenMau.DaHuy
                                            && x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                            && x.SoTienMienGiam != null
                                            && x.SoTienMienGiam > 0

                                            && x.TaiKhoanBenhNhanChis.Any(a => a.DaHuy != true && a.NgayChi >= tuNgay && a.NgayChi <= denNgay)
                                            //&& x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi >= tuNgay && a.NgayChi <= denNgay)
                                            //&& !x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi > denNgay))
                                            //&& lstYCTruyenMauId.Contains(x.Id)
                                            //&& lstYCTruyenMauNgoaiTimKiemId.Contains(x.Id)
                                            )

                                .Select(item => new BaoCaoChiTietMienPhiTronVienGridVo()
                                {
                                    YeuCauTiepNhanId = item.YeuCauTiepNhanId,
                                    TenKhoaPhong = item.NoiChiDinh.KhoaPhong.Ten,
                                    MaNB = item.YeuCauTiepNhan.BenhNhan.MaBN,
                                    MaTN = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                    TenBN = item.YeuCauTiepNhan.HoTen,
                                    NgaySinh = item.YeuCauTiepNhan.NgaySinh,
                                    ThangSinh = item.YeuCauTiepNhan.ThangSinh,
                                    NamSinh = item.YeuCauTiepNhan.NamSinh,
                                    SoDienThoai = item.YeuCauTiepNhan.SoDienThoaiDisplay,
                                    DiaChi = item.YeuCauTiepNhan.DiaChiDayDu,
                                    ThanhTien = item.DonGiaBan,
                                    GiamPhi = item.SoTienMienGiam < item.DonGiaBan ? item.SoTienMienGiam : (decimal?)null,
                                    MienPhi = item.SoTienMienGiam >= item.DonGiaBan ? item.SoTienMienGiam : (decimal?)null,
                                    SoTienMienGiam = item.SoTienMienGiam,
                                    //TongGiamPhiMienPhi = item.SoTienMienGiam,
                                    LyDo = item.GhiChuMienGiamThem,

                                    //BVHD-3918
                                    LaHinhThucDenGioiThieu = item.YeuCauTiepNhan.HinhThucDenId == hinhThucDenGioiThieuId
                                })
                            )
                        .OrderBy(x => x.TenKhoaPhong).ThenBy(x => x.MaTN)
                        .ToList();

                    phieuThus = phieuThus
                        .GroupBy(x => new {x.MaTN, x.TenKhoaPhong})
                        .Select(item => new BaoCaoChiTietMienPhiTronVienGridVo()
                        {
                            YeuCauTiepNhanId = item.First().YeuCauTiepNhanId,
                            TenKhoaPhong = item.Key.TenKhoaPhong,
                            MaNB = item.First().MaNB,
                            MaTN = item.Key.MaTN,
                            TenBN = item.First().TenBN,
                            NgaySinh = item.First().NgaySinh,
                            ThangSinh = item.First().ThangSinh,
                            NamSinh = item.First().NamSinh,
                            SoDienThoai = item.First().SoDienThoai,
                            DiaChi = item.First().DiaChi,
                            ThanhTien = item.Sum(a => a.ThanhTien),
                            GiamPhi = item.Any(a => a.GiamPhi != null && a.GiamPhi != 0)
                                ? item.Sum(a => a.GiamPhi)
                                : (decimal?) null,
                            MienPhi = item.Any(a => a.MienPhi != null && a.MienPhi != 0)
                                ? item.Sum(a => a.MienPhi)
                                : (decimal?) null,
                            TongGiamPhiMienPhi = item.Any(a => a.TongGiamPhiMienPhiItem != null && a.TongGiamPhiMienPhiItem != 0)
                                ? item.Sum(a => a.TongGiamPhiMienPhiItem)
                                : (decimal?)null,
                            LyDo = string.Join("; ", item.Where(a => !string.IsNullOrEmpty(a.LyDo)).Select(a => a.LyDo).Distinct().ToList()),

                            //BVHD-3918
                            LaHinhThucDenGioiThieu = item.Any(a => a.LaHinhThucDenGioiThieu != null && a.LaHinhThucDenGioiThieu == true),
                            ThongTinYeuCaus = item
                                .Where(a => a.YeuCauDichVuId != null 
                                            && a.NhomDichVu != null)
                                .Select(a => new ThongTinYeuCauDichVuVo()
                                {
                                    YeuCauTiepNhanId = a.YeuCauTiepNhanId,
                                    YeuCauDichVuId = a.YeuCauDichVuId,
                                    NhomDichVu = a.NhomDichVu
                                }).ToList()
                        })
                        .Skip(queryInfo.Skip).Take(queryInfo.Take)
                        .ToList();

                #region BVHD-3918: Cập nhật bổ sung lý do: theo nơi giới thiệu, gói khuyến mãi

                #region Nơi giới thiệu
                var lstYeuCauTiepNhanId = phieuThus.Where(x => x.LaHinhThucDenGioiThieu != null
                                                               && x.LaHinhThucDenGioiThieu == true)
                    .Select(x => x.YeuCauTiepNhanId).Distinct().ToList();

                var noiGioiThieus = _yeuCauTiepNhanRepository.TableNoTracking
                    .Where(x => lstYeuCauTiepNhanId.Contains(x.Id)
                                && x.HinhThucDenId == hinhThucDenGioiThieuId
                                && x.NoiGioiThieuId != null)
                    .Select(x => new LookupItemVo()
                    {
                        KeyId = x.Id,
                        DisplayName = x.NoiGioiThieu.Ten
                    }).Distinct().ToList();
                #endregion

                #region Gói khuyến mãi

                var lstThongTinKhuyenMai = new List<ThongTinYeuCauDichVuVo>();

                var lstYeuCauKhamBenhId = phieuThus.SelectMany(x => x.ThongTinYeuCaus)
                    .Where(x => x.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKhamBenh)
                    .Select(x => x.YeuCauDichVuId.Value).Distinct().ToList();
                if (lstYeuCauKhamBenhId.Any())
                {
                    lstThongTinKhuyenMai.AddRange(_mienGiamChiPhiRepository.TableNoTracking
                        .Where(x => x.YeuCauKhamBenhId != null
                                    && x.DaHuy != true
                                    && x.TaiKhoanBenhNhanThu.DaHuy != true
                                    //Cập nhật 06/06/2022: hiển thị thêm trường hợp có voucher
                                    && (x.YeuCauGoiDichVuId != null || !string.IsNullOrEmpty(x.MaTheVoucher))
                                    && lstYeuCauKhamBenhId.Contains(x.YeuCauKhamBenhId.Value))
                        .Select(x => new ThongTinYeuCauDichVuVo()
                        {
                            YeuCauTiepNhanId = x.YeuCauTiepNhanId,
                            NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKhamBenh,
                            YeuCauDichVuId = x.YeuCauKhamBenhId.Value,
                            TenGoiKhuyenMai = x.YeuCauGoiDichVu.ChuongTrinhGoiDichVu.Ten,

                            //Cập nhật 06/06/2022: hiển thị thêm trường hợp có voucher
                            MaVoucher = x.MaTheVoucher
                        }).ToList());
                }

                var lstYeuCauKyThuatId = phieuThus.SelectMany(x => x.ThongTinYeuCaus)
                    .Where(x => x.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat)
                    .Select(x => x.YeuCauDichVuId.Value).Distinct().ToList();
                if (lstYeuCauKyThuatId.Any())
                {
                    lstThongTinKhuyenMai.AddRange(_mienGiamChiPhiRepository.TableNoTracking
                        .Where(x => x.YeuCauDichVuKyThuatId != null
                                    && x.DaHuy != true
                                    && x.TaiKhoanBenhNhanThu.DaHuy != true
                                    //Cập nhật 06/06/2022: hiển thị thêm trường hợp có voucher
                                    && (x.YeuCauGoiDichVuId != null || !string.IsNullOrEmpty(x.MaTheVoucher))
                                    && lstYeuCauKyThuatId.Contains(x.YeuCauDichVuKyThuatId.Value))
                        .Select(x => new ThongTinYeuCauDichVuVo()
                        {
                            YeuCauTiepNhanId = x.YeuCauTiepNhanId,
                            NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKyThuat,
                            YeuCauDichVuId = x.YeuCauDichVuKyThuatId.Value,
                            TenGoiKhuyenMai = x.YeuCauGoiDichVu.ChuongTrinhGoiDichVu.Ten,

                            //Cập nhật 06/06/2022: hiển thị thêm trường hợp có voucher
                            MaVoucher = x.MaTheVoucher
                        }).ToList());
                }

                var lstYeuCauGuongId = phieuThus.SelectMany(x => x.ThongTinYeuCaus)
                    .Where(x => x.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuGiuongBenh)
                    .Select(x => x.YeuCauDichVuId.Value).Distinct().ToList();
                if (lstYeuCauGuongId.Any())
                {
                    lstThongTinKhuyenMai.AddRange(_mienGiamChiPhiRepository.TableNoTracking
                        .Where(x => x.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId != null
                                    && x.DaHuy != true
                                    && x.TaiKhoanBenhNhanThu.DaHuy != true
                                    //Cập nhật 06/06/2022: hiển thị thêm trường hợp có voucher
                                    && (x.YeuCauGoiDichVuId != null || !string.IsNullOrEmpty(x.MaTheVoucher))
                                    && lstYeuCauGuongId.Contains(x.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId.Value))
                        .Select(x => new ThongTinYeuCauDichVuVo()
                        {
                            YeuCauTiepNhanId = x.YeuCauTiepNhanId,
                            NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuGiuongBenh,
                            YeuCauDichVuId = x.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId.Value,
                            TenGoiKhuyenMai = x.YeuCauGoiDichVu.ChuongTrinhGoiDichVu.Ten,

                            //Cập nhật 06/06/2022: hiển thị thêm trường hợp có voucher
                            MaVoucher = x.MaTheVoucher
                        }).ToList());
                }

                #endregion

                //Cập nhật 06/06/2022: hiển thị thêm trường hợp có voucher
                //if (noiGioiThieus.Any())
                //{
                foreach (var phieuThu in phieuThus)
                {
                    if (phieuThu.LaHinhThucDenGioiThieu != null && phieuThu.LaHinhThucDenGioiThieu == true)
                    {
                        var thongTinGioiThieu = noiGioiThieus.FirstOrDefault(x => x.KeyId == phieuThu.YeuCauTiepNhanId);
                        if (thongTinGioiThieu != null)
                        {
                            phieuThu.LyDo = $"Dịch vụ khuyến mại của BS hợp tác - {thongTinGioiThieu.DisplayName}" + (!string.IsNullOrEmpty(phieuThu.LyDo) ? "; " : "") + phieuThu.LyDo;
                        }
                    }

                    if (phieuThu.ThongTinYeuCaus.Any())
                    {
                        var lstYeuCauKhamBenhIdTheoPhieuThu = phieuThu.ThongTinYeuCaus
                            .Where(x => x.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKhamBenh)
                            .Select(x => x.YeuCauDichVuId.Value).Distinct().ToList();
                        var lstYeuCauKyThuatIdTheoPhieuThu = phieuThu.ThongTinYeuCaus
                            .Where(x => x.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat)
                            .Select(x => x.YeuCauDichVuId.Value).Distinct().ToList();
                        var lstYeuCauGuongIdTheoPhieuThu = phieuThu.ThongTinYeuCaus
                            .Where(x => x.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuGiuongBenh)
                            .Select(x => x.YeuCauDichVuId.Value).Distinct().ToList();

                        var thongTinKhuyenMaiTheoPhieuThus = lstThongTinKhuyenMai
                            .Where(x =>
                                        //Cập nhật 06/06/2022: hiển thị thêm trường hợp có voucher
                                        ((x.YeuCauDichVuId != null && !string.IsNullOrEmpty(x.TenGoiKhuyenMai))
                                         || !string.IsNullOrEmpty(x.MaVoucher))

                                        && ((x.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKhamBenh && lstYeuCauKhamBenhIdTheoPhieuThu.Contains(x.YeuCauDichVuId.Value))
                                            || (x.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat && lstYeuCauKyThuatIdTheoPhieuThu.Contains(x.YeuCauDichVuId.Value))
                                            || (x.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuGiuongBenh && lstYeuCauGuongIdTheoPhieuThu.Contains(x.YeuCauDichVuId.Value))))
                            //Cập nhật 06/06/2022: hiển thị thêm trường hợp có voucher
                            //.Select(x => x.TenGoiKhuyenMai)
                            //.Distinct()
                            .ToList();

                        if (thongTinKhuyenMaiTheoPhieuThus.Any())
                        {
                            //Cập nhật 06/06/2022: hiển thị thêm trường hợp có voucher
                            //phieuThu.LyDo += (!string.IsNullOrEmpty(phieuThu.LyDo) ? "; " : "") + $"Dịch vụ khuyến mại của gói {string.Join(", ", thongTinKhuyenMaiTheoPhieuThus)}";

                            var lstThongTinKhuyenMaiGoi = thongTinKhuyenMaiTheoPhieuThus.Where(x => !string.IsNullOrEmpty(x.TenGoiKhuyenMai)).Select(x => x.TenGoiKhuyenMai).Distinct().ToList();
                            var lstThongTinKhuyenMaiVoucher = thongTinKhuyenMaiTheoPhieuThus.Where(x => !string.IsNullOrEmpty(x.MaVoucher)).Select(x => x.MaVoucher).Distinct().ToList();

                            if (lstThongTinKhuyenMaiGoi.Any())
                            {
                                phieuThu.LyDo += (!string.IsNullOrEmpty(phieuThu.LyDo) ? "; " : "") + $"Dịch vụ khuyến mại của gói {string.Join(", ", lstThongTinKhuyenMaiGoi)}";
                            }

                            if (lstThongTinKhuyenMaiVoucher.Any())
                            {
                                phieuThu.LyDo += (!string.IsNullOrEmpty(phieuThu.LyDo) ? "; " : "") + $"Dịch vụ khuyến mại của voucher {string.Join(", ", lstThongTinKhuyenMaiVoucher)}";
                            }
                        }
                    }
                }
                //}
                #endregion
            }

            return new GridDataSource
            {
                Data = phieuThus.ToArray(),
                TotalRowCount = phieuThus.Count()
            };
        }

        public async Task<GridDataSource> GetTotalBaoCaoChiTietMienPhiTronVienForGridAsync(QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new BaoCaoChiTietMienPhiTronVienQueryInfo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoChiTietMienPhiTronVienQueryInfo>(queryInfo.AdditionalSearchString);
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
                var khoaDuocKhoaPhong = _KhoaPhongRepository.TableNoTracking
                    .FirstOrDefault(x => x.Ma.Contains("KD"));
                var khoaDuoc = khoaDuocKhoaPhong.Ten ?? "Khoa dược";

                #region Cập nhật 07/06/2022: tách sub query để hạn chế truy vấn
                var benhNhanChis = _taiKhoanBenhNhanChiRepository.TableNoTracking
                    .Where(x => x.DaHuy != true
                                && (
                                    x.YeuCauKhamBenhId != null
                                    || x.YeuCauDichVuKyThuatId != null
                                    || x.YeuCauDichVuGiuongBenhVienId != null
                                    || x.YeuCauDuocPhamBenhVienId != null
                                    || x.YeuCauVatTuBenhVienId != null
                                    || x.DonThuocThanhToanChiTietId != null
                                    || x.DonVTYTThanhToanChiTietId != null
                                    || x.YeuCauTruyenMauId != null
                                )
                                && x.NgayChi >= tuNgay)
                    //&& x.NgayChi <= denNgay)
                    .ToList();

                var lstBenhNhanChiTimKiems = benhNhanChis.Where(x => x.NgayChi <= denNgay).ToList();
                var lstBenhNhanChiNamNgoaiTimKiems = benhNhanChis.Where(x => x.NgayChi > denNgay).ToList();

                #region lst YCDV ID tìm kiếm
                var lstYCKhamId = lstBenhNhanChiTimKiems.Where(x => x.YeuCauKhamBenhId != null).Select(x => x.YeuCauKhamBenhId.Value).Distinct().ToList();
                var lstYCKyThuatId = lstBenhNhanChiTimKiems.Where(x => x.YeuCauDichVuKyThuatId != null).Select(x => x.YeuCauDichVuKyThuatId.Value).Distinct().ToList();
                var lstYCGiuongId = lstBenhNhanChiTimKiems.Where(x => x.YeuCauDichVuGiuongBenhVienId != null).Select(x => x.YeuCauDichVuGiuongBenhVienId.Value).Distinct().ToList();
                var lstYCDuocPhamId = lstBenhNhanChiTimKiems.Where(x => x.YeuCauDuocPhamBenhVienId != null).Select(x => x.YeuCauDuocPhamBenhVienId.Value).Distinct().ToList();
                var lstYCVatTuId = lstBenhNhanChiTimKiems.Where(x => x.YeuCauVatTuBenhVienId != null).Select(x => x.YeuCauVatTuBenhVienId.Value).Distinct().ToList();
                var lstDonThuocId = lstBenhNhanChiTimKiems.Where(x => x.DonThuocThanhToanChiTietId != null).Select(x => x.DonThuocThanhToanChiTietId.Value).Distinct().ToList();
                var lstDonVTId = lstBenhNhanChiTimKiems.Where(x => x.DonVTYTThanhToanChiTietId != null).Select(x => x.DonVTYTThanhToanChiTietId.Value).Distinct().ToList();
                var lstYCTruyenMauId = lstBenhNhanChiTimKiems.Where(x => x.YeuCauTruyenMauId != null).Select(x => x.YeuCauTruyenMauId.Value).Distinct().ToList();
                #endregion

                #region lst YCDV ID nằm ngoài tìm kiếm
                var lstYCKhamNgoaiTimKiemId = lstBenhNhanChiNamNgoaiTimKiems.Where(x => x.YeuCauKhamBenhId != null).Select(x => x.YeuCauKhamBenhId.Value).Distinct().ToList();
                var lstYCKyThuatNgoaiTimKiemId = lstBenhNhanChiNamNgoaiTimKiems.Where(x => x.YeuCauDichVuKyThuatId != null).Select(x => x.YeuCauDichVuKyThuatId.Value).Distinct().ToList();
                var lstYCGiuongNgoaiTimKiemId = lstBenhNhanChiNamNgoaiTimKiems.Where(x => x.YeuCauDichVuGiuongBenhVienId != null).Select(x => x.YeuCauDichVuGiuongBenhVienId.Value).Distinct().ToList();
                var lstYCDuocPhamNgoaiTimKiemId = lstBenhNhanChiNamNgoaiTimKiems.Where(x => x.YeuCauDuocPhamBenhVienId != null).Select(x => x.YeuCauDuocPhamBenhVienId.Value).Distinct().ToList();
                var lstYCVatTuNgoaiTimKiemId = lstBenhNhanChiNamNgoaiTimKiems.Where(x => x.YeuCauVatTuBenhVienId != null).Select(x => x.YeuCauVatTuBenhVienId.Value).Distinct().ToList();
                var lstDonThuocNgoaiTimKiemId = lstBenhNhanChiNamNgoaiTimKiems.Where(x => x.DonThuocThanhToanChiTietId != null).Select(x => x.DonThuocThanhToanChiTietId.Value).Distinct().ToList();
                var lstDonVTNgoaiTimKiemId = lstBenhNhanChiNamNgoaiTimKiems.Where(x => x.DonVTYTThanhToanChiTietId != null).Select(x => x.DonVTYTThanhToanChiTietId.Value).Distinct().ToList();
                var lstYCTruyenMauNgoaiTimKiemId = lstBenhNhanChiNamNgoaiTimKiems.Where(x => x.YeuCauTruyenMauId != null).Select(x => x.YeuCauTruyenMauId.Value).Distinct().ToList();
                #endregion

                #endregion

                var phieuThus = _yeuCauKhamBenhRepository.TableNoTracking
                        .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham

                                    //15/12/2021: bổ sung thêm dv ko tính phí
                                    && ((x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                        && x.SoTienMienGiam != null
                                        && x.SoTienMienGiam > 0

                                        //&& x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi >= tuNgay && a.NgayChi <= denNgay)
                                        //&& !x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi > denNgay))
                                        && lstYCKhamId.Contains(x.Id)
                                        && !lstYCKhamNgoaiTimKiemId.Contains(x.Id))

                                    || (x.KhongTinhPhi != null && x.KhongTinhPhi == true && x.ThoiDiemChiDinh >= tuNgay && x.ThoiDiemChiDinh <= denNgay))
                                    )
                        .Select(item => new BaoCaoChiTietMienPhiTronVienGridVo()
                        {
                            YeuCauTiepNhanId = item.YeuCauTiepNhanId,
                            TenKhoaPhong = item.NoiChiDinh.KhoaPhong.Ten,
                            MaTN = item.YeuCauTiepNhan.MaYeuCauTiepNhan
                        })
                        .Union(
                            _yeuCauDichVuKyThuatRepository.TableNoTracking
                                .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy

                                            //15/12/2021: bổ sung thêm dv ko tính phí
                                            && ((x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                                && x.SoTienMienGiam != null
                                                && x.SoTienMienGiam > 0

                                                //&& x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi >= tuNgay && a.NgayChi <= denNgay)
                                                //&& !x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi > denNgay))
                                                && lstYCKyThuatId.Contains(x.Id)
                                                && !lstYCKyThuatNgoaiTimKiemId.Contains(x.Id))

                                            || (x.KhongTinhPhi != null && x.KhongTinhPhi == true && x.ThoiDiemChiDinh >= tuNgay && x.ThoiDiemChiDinh <= denNgay))
                                            )
                                .Select(item => new BaoCaoChiTietMienPhiTronVienGridVo()
                                {
                                    YeuCauTiepNhanId = item.YeuCauTiepNhanId,
                                    TenKhoaPhong = item.NoiChiDinh.KhoaPhong.Ten,
                                    MaTN = item.YeuCauTiepNhan.MaYeuCauTiepNhan
                                })
                            )
                        .Union(
                            _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                            .Where(x => x.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy

                                        //15/12/2021: bổ sung thêm dv ko tính phí
                                        && ((x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                            && x.SoTienMienGiam != null
                                            && x.SoTienMienGiam > 0

                                            //&& x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi >= tuNgay && a.NgayChi <= denNgay)
                                            //&& !x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi > denNgay))
                                            && lstYCDuocPhamId.Contains(x.Id)
                                            && !lstYCDuocPhamNgoaiTimKiemId.Contains(x.Id))

                                        || (x.KhongTinhPhi != null && x.KhongTinhPhi == true && x.ThoiDiemChiDinh >= tuNgay && x.ThoiDiemChiDinh <= denNgay))
                                        )
                                .Select(item => new BaoCaoChiTietMienPhiTronVienGridVo()
                                {
                                    YeuCauTiepNhanId = item.YeuCauTiepNhanId,
                                    TenKhoaPhong = item.NoiChiDinh.KhoaPhong.Ten,
                                    MaTN = item.YeuCauTiepNhan.MaYeuCauTiepNhan
                                })
                            )
                        .Union(
                            _yeuCauVatTuBenhVienRepository.TableNoTracking
                                .Where(x => x.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy

                                            //15/12/2021: bổ sung thêm dv ko tính phí
                                            && ((x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                                && x.SoTienMienGiam != null
                                                && x.SoTienMienGiam > 0

                                                //&& x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi >= tuNgay && a.NgayChi <= denNgay)
                                                //&& !x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi > denNgay))
                                                && lstYCVatTuId.Contains(x.Id)
                                                && !lstYCVatTuNgoaiTimKiemId.Contains(x.Id))

                                            || (x.KhongTinhPhi != null && x.KhongTinhPhi == true && x.ThoiDiemChiDinh >= tuNgay && x.ThoiDiemChiDinh <= denNgay))
                                            )
                                .Select(item => new BaoCaoChiTietMienPhiTronVienGridVo()
                                {
                                    YeuCauTiepNhanId = item.YeuCauTiepNhanId,
                                    TenKhoaPhong = item.NoiChiDinh.KhoaPhong.Ten,
                                    MaTN = item.YeuCauTiepNhan.MaYeuCauTiepNhan
                                })
                            )
                        .Union(
                            _yeuCauDichVuGiuongBenhVienChiPhiBenhVienRepository.TableNoTracking
                                .Where(x => x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                            && x.SoTienMienGiam != null
                                            && x.SoTienMienGiam > 0

                                            //&& x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi >= tuNgay && a.NgayChi <= denNgay)
                                            //&& !x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi > denNgay))
                                            && lstYCGiuongId.Contains(x.Id)
                                            && !lstYCGiuongNgoaiTimKiemId.Contains(x.Id))

                                .Select(item => new BaoCaoChiTietMienPhiTronVienGridVo()
                                {
                                    YeuCauTiepNhanId = item.YeuCauTiepNhanId,
                                    TenKhoaPhong = item.KhoaPhong.Ten,
                                    MaTN = item.YeuCauTiepNhan.MaYeuCauTiepNhan
                                })
                            )
                        .Union(
                            _donThuocThanhToanChiTietRepository.TableNoTracking
                                .Where(x => x.DonThuocThanhToan.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                            && x.SoTienMienGiam != null
                                            && x.SoTienMienGiam > 0

                                            //&& x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi >= tuNgay && a.NgayChi <= denNgay)
                                            //&& !x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi > denNgay))
                                            && lstDonThuocId.Contains(x.Id)
                                            && !lstDonThuocNgoaiTimKiemId.Contains(x.Id))

                                .Select(item => new BaoCaoChiTietMienPhiTronVienGridVo()
                                {
                                    YeuCauTiepNhanId = item.DonThuocThanhToan.YeuCauTiepNhanId ?? 0,
                                    TenKhoaPhong = item.YeuCauKhamBenhDonThuocChiTiet.YeuCauKhamBenhDonThuoc.NoiKeDon.KhoaPhong.Ten ?? item.NoiTruDonThuocChiTiet.NoiTruDonThuoc.NoiKeDon.KhoaPhong.Ten ?? khoaDuoc,
                                    MaTN = item.DonThuocThanhToan.YeuCauTiepNhan.MaYeuCauTiepNhan
                                })
                            )
                        .Union(
                            _donVTYTThanhToanChiTietRepository.TableNoTracking
                                .Where(x => x.DonVTYTThanhToan.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                            && x.SoTienMienGiam != null
                                            && x.SoTienMienGiam > 0

                                            //&& x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi >= tuNgay && a.NgayChi <= denNgay)
                                            //&& !x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi > denNgay))
                                            && lstDonVTId.Contains(x.Id)
                                            && !lstDonVTNgoaiTimKiemId.Contains(x.Id))

                                .Select(item => new BaoCaoChiTietMienPhiTronVienGridVo()
                                {
                                    YeuCauTiepNhanId = item.DonVTYTThanhToan.YeuCauTiepNhanId ?? 0,
                                    TenKhoaPhong = item.YeuCauKhamBenhDonVTYTChiTiet.YeuCauKhamBenhDonVTYT.NoiKeDon.KhoaPhong.Ten ?? khoaDuoc,
                                    MaTN = item.DonVTYTThanhToan.YeuCauTiepNhan.MaYeuCauTiepNhan
                                })
                            )
                        .Union(
                            _yeuCauTruyenMauRepository.TableNoTracking
                                .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauTruyenMau.DaHuy
                                            && x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                            && x.SoTienMienGiam != null
                                            && x.SoTienMienGiam > 0

                                            //&& x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi >= tuNgay && a.NgayChi <= denNgay)
                                            //&& !x.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true).Any(a => a.NgayChi > denNgay))
                                            && lstYCTruyenMauId.Contains(x.Id)
                                            && !lstYCTruyenMauNgoaiTimKiemId.Contains(x.Id))

                                .Select(item => new BaoCaoChiTietMienPhiTronVienGridVo()
                                {
                                    YeuCauTiepNhanId = item.YeuCauTiepNhanId,
                                    TenKhoaPhong = item.NoiChiDinh.KhoaPhong.Ten,
                                    MaTN = item.YeuCauTiepNhan.MaYeuCauTiepNhan
                                })
                            )
                        .ToList();

                phieuThus = phieuThus
                    .GroupBy(x => new { x.MaTN, x.TenKhoaPhong })
                    .Select(item => new BaoCaoChiTietMienPhiTronVienGridVo()
                    {
                        YeuCauTiepNhanId = item.First().YeuCauTiepNhanId,
                        TenKhoaPhong = item.Key.TenKhoaPhong,
                        MaTN = item.First().MaTN
                    })
                    .ToList();

                var countTask = phieuThus.Count();
                return new GridDataSource { TotalRowCount = countTask };
            }
            return new GridDataSource { TotalRowCount = 0 };
        }

        public virtual byte[] ExportBaoCaoChiTietMienPhiTronVien(GridDataSource gridDataSource, QueryInfo query)
        {
            var timKiemNangCaoObj = new BaoCaoChiTietMienPhiTronVienQueryInfo();
            if (!string.IsNullOrEmpty(query.AdditionalSearchString) && query.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoChiTietMienPhiTronVienQueryInfo>(query.AdditionalSearchString);
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

            var datas = (ICollection<BaoCaoChiTietMienPhiTronVienGridVo>)gridDataSource.Data;
            int ind = 1;

            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoChiTietMienPhiTronVienGridVo>("STT", p => ind++)
            };
            using(var stream  = new MemoryStream())
            {
                using(var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO CHI TIẾT MIỄN PHÍ TRỐN VIỆN");
                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 25;
                    worksheet.Column(3).Width = 15;
                    worksheet.Column(4).Width = 15;
                    worksheet.Column(5).Width = 25;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 15;
                    worksheet.Column(8).Width = 35;
                    worksheet.Column(9).Width = 15;
                    worksheet.Column(10).Width = 15;
                    worksheet.Column(11).Width = 15;
                    worksheet.Column(12).Width = 15;
                    worksheet.Column(13).Width = 25;
                    worksheet.DefaultColWidth = 7;
                    worksheet.Row(3).Height = 21;

                    using(var range = worksheet.Cells["A1:E1"])
                    {
                        range.Worksheet.Cells["A1:E1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:E1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A1:E1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:E1"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A1:E1"].Merge = true;
                        range.Worksheet.Cells["A1:E1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";

                    }

                    using (var range = worksheet.Cells["A3:M3"])
                    {
                        range.Worksheet.Cells["A3:M3"].Style.Font.SetFromFont(new Font("Times New Roman", 15));
                        range.Worksheet.Cells["A3:M3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:M3"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A3:M3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:M3"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A3:M3"].Merge = true;
                        range.Worksheet.Cells["A3:M3"].Value = "BÁO CÁO CHI TIẾT MIỄN PHÍ, GIẢM PHÍ";

                    }

                    using (var range = worksheet.Cells["A4:M4"])
                    {
                        range.Worksheet.Cells["A4:M4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A4:M4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:M4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:M4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:M4"].Style.Font.Italic = true;
                        range.Worksheet.Cells["A4:M4"].Merge = true;
                        range.Worksheet.Cells["A4:M4"].Value = "Thời gian từ ngày " + tuNgay?.FormatNgayGioTimKiemTrenBaoCao() + " - " + denNgay?.FormatNgayGioTimKiemTrenBaoCao();
                    }


                    using (var range = worksheet.Cells["A6:M6"])
                    {
                        range.Worksheet.Cells["A6:M6"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A6:M6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A6:M6"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A6:M6"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A6:M6"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A6:M6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A6:M6"].Style.WrapText = true;

                        range.Worksheet.Cells["A6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A6"].Value = "STT";

                        range.Worksheet.Cells["B6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B6"].Value = "Khoa/Phòng";

                        range.Worksheet.Cells["C6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C6"].Value = "Mã NB";

                        range.Worksheet.Cells["D6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D6"].Value = "Mã TN";

                        range.Worksheet.Cells["E6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E6"].Value = "Tên BN";

                        range.Worksheet.Cells["F6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F6"].Value = "Năm sinh";

                        range.Worksheet.Cells["G6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G6"].Value = "Số điện thoại";

                        range.Worksheet.Cells["H6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H6"].Value = "Địa chỉ";

                        range.Worksheet.Cells["I6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I6"].Value = "Số tiền phải thanh toán";

                        range.Worksheet.Cells["J6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J6"].Value = "Giảm phí";

                        range.Worksheet.Cells["K6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K6"].Value = "Miễn phí";

                        range.Worksheet.Cells["L6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["L6"].Value = "Tổng giảm phí, miễn phí";

                        range.Worksheet.Cells["M6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["M6"].Value = "Lý do";
                    }

                    var manager = new PropertyManager<BaoCaoChiTietMienPhiTronVienGridVo>(requestProperties);
                    int index = 7; // bắt đầu đổ data từ dòng 7

                    ///Đổ data vào
                    ///
                    var stt = 1;
                    if (datas.Any())
                    {
                        foreach(var item in datas)
                        {
                            manager.CurrentObject = item;
                            using (var range = worksheet.Cells["A" + index + ":M" + index])
                            {
                                range.Worksheet.Cells["A" + index + ":M" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                range.Worksheet.Cells["A" + index + ":M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                range.Worksheet.Cells["A" + index + ":M" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                range.Worksheet.Cells["A" + index + ":M" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["A" + index + ":M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                manager.WriteToXlsx(worksheet, index);

                                range.Worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["A" + index].Value = stt;

                                range.Worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["B" + index].Value = item.TenKhoaPhong;
                                range.Worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                range.Worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["C" + index].Value = item.MaNB;
                                range.Worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                range.Worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["D" + index].Value = item.MaTN;
                                range.Worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                range.Worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["E" + index].Value = item.TenBN;
                                range.Worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                range.Worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["F" + index].Value = item.NgaySinhDisplay;
                                range.Worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                range.Worksheet.Cells["G" + index].Value = item.SoDienThoai;
                                range.Worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                range.Worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["H" + index].Value = item.DiaChi;
                                range.Worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                range.Worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["I" + index].Value = item.ThanhTien;
                                range.Worksheet.Cells["I" + index].Style.Numberformat.Format = "#,##0.00";

                                range.Worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["J" + index].Value = item.GiamPhi;
                                range.Worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";

                                range.Worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["K" + index].Value = item.MienPhi;
                                range.Worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";

                                range.Worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["L" + index].Value = item.TongGiamPhiMienPhi;
                                range.Worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";

                                range.Worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["M" + index].Value = item.LyDo;
                                range.Worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                index++;
                                stt++;
                            }
                        }
                    }
                    worksheet.Cells["A" + index + ":M" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                    worksheet.Cells["A" + index + ":M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["A" + index + ":M" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    worksheet.Cells["A" + index + ":M" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["A" + index + ":M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["A" + index + ":M" + index].Style.Font.Bold = true;

                    worksheet.Cells["A" + index + ":I" + index].Merge = true;
                    worksheet.Cells["A" + index + ":I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells["A" + index + ":I" + index].Value = "Tổng cộng";
                    worksheet.Cells["A" + index + ":I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                    worksheet.Cells["J" + index].Value = datas.Sum(s => s.GiamPhi);
                    worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";

                    worksheet.Cells["K" + index].Value = datas.Sum(s => s.MienPhi);
                    worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";

                    worksheet.Cells["L" + index].Value = datas.Sum(s => s.TongGiamPhiMienPhi);
                    worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }
    }
}
