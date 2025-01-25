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
using Microsoft.EntityFrameworkCore.Internal;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public async Task<GridDataSource> GetDataThongKeCacDVChuaLayLenBienLaiThuTienForGrid(QueryInfo queryInfo)
        {
            var lstTiepNhanTheoNoiGioiThieu = new List<ThongTinTiepNhanChuaThuTienCoHinhThucDenVo>();
            decimal tongTien = 0;
            var total = 0;

            var timKiemNangCaoObj = new ThongKeCacDichVuChuaLayLenBienLaiThuTienQueryInfoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<ThongKeCacDichVuChuaLayLenBienLaiThuTienQueryInfoVo>(queryInfo.AdditionalSearchString);
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;

            //kiểm tra tìm kiếm nâng 
            if (timKiemNangCaoObj.FromDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.FromDate))
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
                lstTiepNhanTheoNoiGioiThieu = _yeuCauTiepNhanRepository.TableNoTracking
                    .ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.MaYeuCauTiepNhan, x => x.HoTen, x => x.BenhNhan.MaBN)
                    .Where(x => x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien
                                && x.BenhNhanId != null

                                // Lấy gốc là theo Mã YCTN, nếu YCTN là ngoại trú hoặc KSK thì kiểm tra thêm xem có YCTN nội trú ko
                                && (
                                    x.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru
                                    || x.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamSucKhoe
                                    || (x.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && x.NoiTruBenhAn.LoaiBenhAn == LoaiBenhAn.TreSoSinh)
                                    )
                                && x.ThoiDiemTiepNhan >= tuNgay
                                && x.ThoiDiemTiepNhan <= denNgay)
                    .Select(x => new ThongTinTiepNhanChuaThuTienCoHinhThucDenVo()
                    {
                        YeucauTiepNhanId = x.Id,
                        MaYeucauTiepNhan = x.MaYeuCauTiepNhan,
                        MaBenhAn = x.NoiTruBenhAn.SoBenhAn,
                        MaBN = x.BenhNhan.MaBN,
                        BenhNhanId = x.BenhNhanId.Value,
                        HoVaTen = x.HoTen,
                        NamSinh = x.NamSinh,
                        DiaChi = x.DiaChiDayDu,

                        NoiGioiThieuId = x.NoiGioiThieuId,
                        ThoiDiemTiepNhan = x.ThoiDiemTiepNhan,
                        NoiGioiThieuDisplay = x.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.NoiGioiThieu.DonVi) ? $" - {x.NoiGioiThieu.DonVi}" : ""),
                        TenHinhThucDen = x.HinhThucDen.Ten,
                        LaGioiThieu = x.NoiGioiThieuId != null,
                        LaTiepNhanNoiTru = x.NoiTruBenhAn != null,
                        LaBenhAnSoSinh = x.NoiTruBenhAn.LoaiBenhAn == LoaiBenhAn.TreSoSinh
                    })
                    .OrderBy(x => x.ThoiDiemTiepNhan)
                    .ToList();
                if (lstTiepNhanTheoNoiGioiThieu.Any())
                {
                    var lstBenhNhanId = lstTiepNhanTheoNoiGioiThieu.Select(x => x.BenhNhanId).Distinct().ToList();
                    var lstMaTiepNhanChinh = lstTiepNhanTheoNoiGioiThieu.Select(x => x.MaYeucauTiepNhan).Distinct().ToList();

                    var lstTiepNhanTheoBenhNhan = _yeuCauTiepNhanRepository.TableNoTracking
                        .Where(x => x.BenhNhanId != null
                                    && lstBenhNhanId.Contains(x.BenhNhanId.Value)
                                    // Lấy gốc là theo Mã YCTN, nếu YCTN là ngoại trú hoặc KSK thì kiểm tra thêm xem có YCTN nội trú ko
                                    && (
                                        x.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru
                                        || x.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamSucKhoe
                                        || (x.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru) // && (x.NoiTruBenhAn.LoaiBenhAn == LoaiBenhAn.TreSoSinh || lstMaTiepNhanChinh.Contains(x.MaYeuCauTiepNhan)))
                                    )
                                    && x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien)
                        .Select(x => new ThongTinTiepNhanChuaThuTienCoHinhThucDenVo()
                        {
                            YeucauTiepNhanId = x.Id,
                            MaYeucauTiepNhan = x.MaYeuCauTiepNhan,
                            MaBenhAn = x.NoiTruBenhAn.SoBenhAn,
                            MaBN = x.BenhNhan.MaBN,
                            BenhNhanId = x.BenhNhanId.Value,
                            HoVaTen = x.HoTen,
                            NamSinh = x.NamSinh,
                            DiaChi = x.DiaChiDayDu,

                            NoiGioiThieuId = x.NoiGioiThieuId,
                            ThoiDiemTiepNhan = x.ThoiDiemTiepNhan,
                            NoiGioiThieuDisplay = x.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.NoiGioiThieu.DonVi) ? $" - {x.NoiGioiThieu.DonVi}" : ""),
                            TenHinhThucDen = x.HinhThucDen.Ten,
                            LaGioiThieu = x.NoiGioiThieuId != null,

                            // dùng để xác định người giới thiệu trước đó nếu data hiện tại ko có người giới thiệu
                            LaDataTheoDieuKienTimKiem = lstMaTiepNhanChinh.Contains(x.MaYeuCauTiepNhan),

                            LaTiepNhanNoiTru = x.NoiTruBenhAn != null,
                            LaBenhAnSoSinh = x.NoiTruBenhAn.LoaiBenhAn == LoaiBenhAn.TreSoSinh
                        })
                        .OrderBy(x => x.ThoiDiemTiepNhan)
                        .ToList();

                    foreach (var benhNhanId in lstBenhNhanId)
                    {
                        var lanTiepNhanDauTienCoGioiThieu = lstTiepNhanTheoNoiGioiThieu
                            .Where(x => x.BenhNhanId == benhNhanId && x.NoiGioiThieuId != null)
                            .OrderBy(x => x.YeucauTiepNhanId).FirstOrDefault();
                        if (lanTiepNhanDauTienCoGioiThieu == null)
                        {
                            lanTiepNhanDauTienCoGioiThieu = lstTiepNhanTheoBenhNhan
                                .Where(x => x.BenhNhanId == benhNhanId && x.NoiGioiThieuId != null)
                                .OrderBy(x => x.YeucauTiepNhanId).FirstOrDefault();
                        }

                        var tiepNhanBenhNhans = lstTiepNhanTheoBenhNhan
                            .Where(x => (lanTiepNhanDauTienCoGioiThieu == null || x.YeucauTiepNhanId > lanTiepNhanDauTienCoGioiThieu.YeucauTiepNhanId)
                                        && x.BenhNhanId == benhNhanId)
                            .ToList();

                        var nguoiGioiThieuHienTaiId = lanTiepNhanDauTienCoGioiThieu?.NoiGioiThieuId;
                        var tenNguoiGioiThieuHienTai = lanTiepNhanDauTienCoGioiThieu?.NoiGioiThieuDisplay;
                        foreach (var lanTiepNhan in tiepNhanBenhNhans)
                        {
                            if (lanTiepNhan.NoiGioiThieuId != null && lanTiepNhan.NoiGioiThieuId != nguoiGioiThieuHienTaiId)
                            {
                                nguoiGioiThieuHienTaiId = lanTiepNhan.NoiGioiThieuId;
                                tenNguoiGioiThieuHienTai = lanTiepNhan.NoiGioiThieuDisplay;
                            }

                            if (lstTiepNhanTheoNoiGioiThieu.All(x => x.YeucauTiepNhanId != lanTiepNhan.YeucauTiepNhanId) && lanTiepNhan.LaDataTheoDieuKienTimKiem == true)
                            {
                                lanTiepNhan.NoiGioiThieuId = nguoiGioiThieuHienTaiId;
                                lanTiepNhan.NoiGioiThieuDisplay = tenNguoiGioiThieuHienTai;
                                lanTiepNhan.LaGioiThieu = nguoiGioiThieuHienTaiId != null;
                                lstTiepNhanTheoNoiGioiThieu.Add(lanTiepNhan);
                            }
                            else
                            {
                                if (nguoiGioiThieuHienTaiId != null)
                                {
                                    var lanTiepNhanDaThem = lstTiepNhanTheoNoiGioiThieu.FirstOrDefault(x => x.YeucauTiepNhanId == lanTiepNhan.YeucauTiepNhanId);
                                    if (lanTiepNhanDaThem != null)
                                    {
                                        lanTiepNhanDaThem.NoiGioiThieuId = nguoiGioiThieuHienTaiId;
                                        lanTiepNhanDaThem.NoiGioiThieuDisplay = tenNguoiGioiThieuHienTai;
                                        lanTiepNhanDaThem.LaGioiThieu = nguoiGioiThieuHienTaiId != null;
                                    }
                                }
                            }
                        }
                    }

                    var lstMaTiepTiepNhanChuaThuTien = lstTiepNhanTheoNoiGioiThieu.Select(x => x.MaYeucauTiepNhan).Distinct().ToList();

                    #region Code cũ
                    //#region Chi phí khám bệnh chưa thu
                    //var chiPhiKhams = await _yeuCauKhamBenhRepository.TableNoTracking
                    //.Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham
                    //            && x.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan
                    //            && x.KhongTinhPhi != true
                    //            && lstMaTiepTiepNhanChuaThuTien.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan)
                    //            && x.GoiKhamSucKhoeId == null)
                    //.Select(item => new ThongTinDichVuChuaThuTienVo()
                    //{
                    //    YeucauTiepNhanId = item.YeuCauTiepNhanId,
                    //    MaYeucauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    //    TenDichVu = item.TenDichVu,
                    //    SoLuong = 1,
                    //    DonGia = item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau.Value : item.Gia,
                    //    SoTienMienGiamTheoDichVu = item.SoTienMienGiam,
                    //    DuocHuongBHYT = item.BaoHiemChiTra != null && item.BaoHiemChiTra == true,
                    //    MucHuongBaoHiem = item.MucHuongBaoHiem.GetValueOrDefault(),
                    //    DonGiaBHYT = item.DonGiaBaoHiem,
                    //    TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    //    LaTrongGoi = item.YeuCauGoiDichVuId != null
                    //})
                    //.ToListAsync();
                    //#endregion

                    //#region Chi phí kỹ thuật chưa thu
                    //var chiPhiKyThuats = await _yeuCauDichVuKyThuatRepository.TableNoTracking
                    //    .Where(x => x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                    //                && x.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan
                    //                && x.KhongTinhPhi != true
                    //                && lstMaTiepTiepNhanChuaThuTien.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan)
                    //                && x.GoiKhamSucKhoeId == null)
                    //    .Select(item => new ThongTinDichVuChuaThuTienVo()
                    //    {
                    //        YeucauTiepNhanId = item.YeuCauTiepNhanId,
                    //        MaYeucauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    //        TenDichVu = item.TenDichVu,
                    //        SoLuong = item.SoLan,
                    //        DonGia = item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau.Value : item.Gia,
                    //        SoTienMienGiamTheoDichVu = item.SoTienMienGiam,
                    //        DuocHuongBHYT = item.BaoHiemChiTra != null && item.BaoHiemChiTra == true,
                    //        MucHuongBaoHiem = item.MucHuongBaoHiem.GetValueOrDefault(),
                    //        DonGiaBHYT = item.DonGiaBaoHiem,
                    //        TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    //        LaTrongGoi = item.YeuCauGoiDichVuId != null
                    //    })
                    //    .ToListAsync();
                    //#endregion

                    //#region Chi phí truyền máu chưa thu
                    //var chiPhiTruyenMaus = await _yeuCauTruyenMauRepository.TableNoTracking
                    //    .Where(x => x.TrangThai != EnumTrangThaiYeuCauTruyenMau.DaHuy
                    //                && x.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan
                    //                && lstMaTiepTiepNhanChuaThuTien.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan))
                    //    .Select(item => new ThongTinDichVuChuaThuTienVo()
                    //    {
                    //        YeucauTiepNhanId = item.YeuCauTiepNhanId,
                    //        MaYeucauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    //        TenDichVu = item.TenDichVu,
                    //        SoLuong = 1,
                    //        DonGia = item.DonGiaBan.Value,
                    //        SoTienMienGiamTheoDichVu = item.SoTienMienGiam,
                    //        DuocHuongBHYT = item.BaoHiemChiTra != null && item.BaoHiemChiTra == true,
                    //        MucHuongBaoHiem = item.MucHuongBaoHiem.GetValueOrDefault(),
                    //        DonGiaBHYT = item.DonGiaBaoHiem,
                    //        TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    //        LaTrongGoi = false
                    //    })
                    //    .ToListAsync();
                    //#endregion

                    //#region Chi phí giường chưa thu
                    //var chiPhiGiuongs = await _yeuCauDichVuGiuongBenhVienChiPhiBenhVienRepository.TableNoTracking
                    //    .Where(x => lstMaTiepTiepNhanChuaThuTien.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan))
                    //    .Select(item => new ThongTinDichVuChuaThuTienVo()
                    //    {
                    //        YeuCauDichVuGiuongChiPhiBenhVienId = item.Id,
                    //        YeucauTiepNhanId = item.YeuCauTiepNhanId,
                    //        MaYeucauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    //        TenDichVu = item.Ten,
                    //        SoLuong = item.SoLuong,
                    //        DonGia = item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau.Value : item.Gia,
                    //        SoTienMienGiamTheoDichVu = item.SoTienMienGiam,
                    //            //DuocHuongBHYT = item.BaoHiemChiTra != null && item.BaoHiemChiTra == true,
                    //            //MucHuongBaoHiem = item.MucHuongBaoHiem.GetValueOrDefault(),
                    //            //DonGiaBHYT = item.DonGiaBaoHiem,
                    //            //TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    //            LaTrongGoi = item.YeuCauGoiDichVuId != null
                    //    })
                    //    .ToListAsync();

                    //if (chiPhiGiuongs.Any())
                    //{
                    //    var lstChiPhiGiuongBenhVienId = chiPhiGiuongs
                    //        .Where(x => x.YeuCauDichVuGiuongChiPhiBenhVienId != null)
                    //        .Select(x => x.YeuCauDichVuGiuongChiPhiBenhVienId.Value).Distinct().ToList();

                    //    var chiPhiGiuongBHYTs = await _yeuCauDichVuGiuongBenhVienChiPhiBhytRepository.TableNoTracking
                    //        .Where(x => lstChiPhiGiuongBenhVienId.Contains(x.Id))
                    //        .Select(item => new ThongTinDichVuChuaThuTienVo()
                    //        {
                    //            YeuCauDichVuGiuongChiPhiBenhVienId = item.Id,
                    //            DuocHuongBHYT = item.BaoHiemChiTra != null && item.BaoHiemChiTra == true,
                    //            MucHuongBaoHiem = item.MucHuongBaoHiem.GetValueOrDefault(),
                    //            DonGiaBHYT = item.DonGiaBaoHiem,
                    //            TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan.GetValueOrDefault()
                    //        })
                    //        .ToListAsync();

                    //    foreach (var chiPhiGiuongBenhVien in chiPhiGiuongs)
                    //    {
                    //        var chiPhiBHYT = chiPhiGiuongBHYTs.FirstOrDefault(x => x.YeuCauDichVuGiuongChiPhiBenhVienId == chiPhiGiuongBenhVien.YeuCauDichVuGiuongChiPhiBenhVienId);
                    //        if (chiPhiBHYT != null)
                    //        {
                    //            chiPhiGiuongBenhVien.DuocHuongBHYT = chiPhiBHYT.DuocHuongBHYT;
                    //            chiPhiGiuongBenhVien.MucHuongBaoHiem = chiPhiBHYT.MucHuongBaoHiem;
                    //            chiPhiGiuongBenhVien.DonGiaBHYT = chiPhiBHYT.DonGiaBHYT;
                    //            chiPhiGiuongBenhVien.TiLeBaoHiemThanhToan = chiPhiBHYT.TiLeBaoHiemThanhToan;
                    //        }
                    //    }
                    //}
                    //#endregion

                    //#region Chi phí dược phẩm chưa thu
                    //var chiPhiDuocPhams = await _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                    //    .Where(x => x.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                    //                && x.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan
                    //                && x.KhongTinhPhi != true
                    //                && lstMaTiepTiepNhanChuaThuTien.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan))
                    //    .Select(item => new ThongTinDichVuChuaThuTienVo()
                    //    {
                    //        YeucauTiepNhanId = item.YeuCauTiepNhanId,
                    //        MaYeucauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    //        TenDichVu = item.Ten,
                    //        SoLuong = item.SoLuong,
                    //        GiaBan = item.DonGiaBan,
                    //        SoTienMienGiamTheoDichVu = item.SoTienMienGiam,
                    //        DuocHuongBHYT = item.BaoHiemChiTra != null && item.BaoHiemChiTra == true,
                    //        MucHuongBaoHiem = item.MucHuongBaoHiem.GetValueOrDefault(),
                    //        DonGiaBHYT = item.DonGiaBaoHiem,
                    //        TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    //        LaTrongGoi = item.YeuCauGoiDichVuId != null
                    //    })
                    //    .ToListAsync();
                    //#endregion

                    //#region Chi phí vật tư chưa thu
                    //var chiPhiVatTus = await _yeuCauVatTuBenhVienRepository.TableNoTracking
                    //    .Where(x => x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy
                    //                && x.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan
                    //                && x.KhongTinhPhi != true
                    //                && lstMaTiepTiepNhanChuaThuTien.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan))
                    //    .Select(item => new ThongTinDichVuChuaThuTienVo()
                    //    {
                    //        YeucauTiepNhanId = item.YeuCauTiepNhanId,
                    //        MaYeucauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    //        TenDichVu = item.Ten,
                    //        SoLuong = item.SoLuong,
                    //        GiaBan = item.DonGiaBan,
                    //        SoTienMienGiamTheoDichVu = item.SoTienMienGiam,
                    //        DuocHuongBHYT = item.BaoHiemChiTra != null && item.BaoHiemChiTra == true,
                    //        MucHuongBaoHiem = item.MucHuongBaoHiem.GetValueOrDefault(),
                    //        DonGiaBHYT = item.DonGiaBaoHiem,
                    //        TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    //        LaTrongGoi = item.YeuCauGoiDichVuId != null
                    //    })
                    //    .ToListAsync();
                    //#endregion

                    //#region Chi phí đơn thuốc BHYTs chưa thu
                    //var chiPhiDonThuocBHYts = await _donThuocThanhToanChiTietRepository.TableNoTracking
                    //    .Where(x => x.DonThuocThanhToan.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT
                    //                && (lstMaTiepTiepNhanChuaThuTien.Contains(x.YeuCauKhamBenhDonThuocChiTiet.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.YeuCauTiepNhan.MaYeuCauTiepNhan)
                    //                || lstMaTiepTiepNhanChuaThuTien.Contains(x.NoiTruDonThuocChiTiet.NoiTruDonThuoc.YeuCauTiepNhan.MaYeuCauTiepNhan)))
                    //    .Select(item => new ThongTinDichVuChuaThuTienVo()
                    //    {
                    //        YeucauTiepNhanId = item.YeuCauKhamBenhDonThuocChiTiet != null
                    //            ? item.YeuCauKhamBenhDonThuocChiTiet.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.YeuCauTiepNhanId
                    //            : item.NoiTruDonThuocChiTiet.NoiTruDonThuoc.YeuCauTiepNhanId,
                    //        MaYeucauTiepNhan = item.YeuCauKhamBenhDonThuocChiTiet.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.YeuCauTiepNhan.MaYeuCauTiepNhan ?? item.NoiTruDonThuocChiTiet.NoiTruDonThuoc.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    //        TenDichVu = item.Ten,
                    //        SoLuong = item.SoLuong,
                    //        GiaBan = item.DonGiaBan,
                    //        SoTienMienGiamTheoDichVu = item.SoTienMienGiam,
                    //        DuocHuongBHYT = item.BaoHiemChiTra != null && item.BaoHiemChiTra == true,
                    //        MucHuongBaoHiem = item.MucHuongBaoHiem.GetValueOrDefault(),
                    //        DonGiaBHYT = item.DonGiaBaoHiem,
                    //        TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    //        LaTrongGoi = false
                    //    })
                    //    .ToListAsync();
                    //#endregion

                    #endregion

                    #region Get chi phí theo dịch vụ
                    //Cập nhật 25/07/2022: bỏ union, tách query
                    var lstTemp = new List<ThongTinDichVuChuaThuTienVo>();
                    var dvKhams = _yeuCauKhamBenhRepository.TableNoTracking
                        .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham
                                    && x.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan
                                    && x.KhongTinhPhi != true
                                    && lstMaTiepTiepNhanChuaThuTien.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan)
                                    && x.GoiKhamSucKhoeId == null)
                        .Select(item => new ThongTinDichVuChuaThuTienVo()
                        {
                            YeucauTiepNhanId = item.YeuCauTiepNhanId,
                            MaYeucauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                            TenDichVu = item.TenDichVu,
                            SoLuong = 1,
                            DonGia = item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau.Value : item.Gia,
                            SoTienMienGiamTheoDichVu = item.SoTienMienGiam,
                            DuocHuongBHYT = item.BaoHiemChiTra != null && item.BaoHiemChiTra == true,
                            MucHuongBaoHiem = item.MucHuongBaoHiem.GetValueOrDefault(),
                            DonGiaBHYT = item.DonGiaBaoHiem,
                            TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                            LaTrongGoi = item.YeuCauGoiDichVuId != null
                        }).ToList();

                    if (dvKhams.Any())
                    {
                        lstTemp.AddRange(dvKhams);
                    }

                    var dvKyThuats = _yeuCauDichVuKyThuatRepository.TableNoTracking
                                .Where(x => x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                            && x.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan
                                            && x.KhongTinhPhi != true
                                            && lstMaTiepTiepNhanChuaThuTien.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan)
                                            && x.GoiKhamSucKhoeId == null)
                                .Select(item => new ThongTinDichVuChuaThuTienVo()
                                {
                                    YeucauTiepNhanId = item.YeuCauTiepNhanId,
                                    MaYeucauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                    TenDichVu = item.TenDichVu,
                                    SoLuong = item.SoLan,
                                    DonGia = item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau.Value : item.Gia,
                                    SoTienMienGiamTheoDichVu = item.SoTienMienGiam,
                                    DuocHuongBHYT = item.BaoHiemChiTra != null && item.BaoHiemChiTra == true,
                                    MucHuongBaoHiem = item.MucHuongBaoHiem.GetValueOrDefault(),
                                    DonGiaBHYT = item.DonGiaBaoHiem,
                                    TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                    LaTrongGoi = item.YeuCauGoiDichVuId != null
                                }).ToList();
                    if (dvKyThuats.Any())
                    {
                        lstTemp.AddRange(dvKyThuats);
                    }


                    var truyenMaus = _yeuCauTruyenMauRepository.TableNoTracking
                                .Where(x => x.TrangThai != EnumTrangThaiYeuCauTruyenMau.DaHuy
                                            && x.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan
                                            && lstMaTiepTiepNhanChuaThuTien.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan))
                                .Select(item => new ThongTinDichVuChuaThuTienVo()
                                {
                                    YeucauTiepNhanId = item.YeuCauTiepNhanId,
                                    MaYeucauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                    TenDichVu = item.TenDichVu,
                                    SoLuong = 1,
                                    DonGia = item.DonGiaBan.Value,
                                    SoTienMienGiamTheoDichVu = item.SoTienMienGiam,
                                    DuocHuongBHYT = item.BaoHiemChiTra != null && item.BaoHiemChiTra == true,
                                    MucHuongBaoHiem = item.MucHuongBaoHiem.GetValueOrDefault(),
                                    DonGiaBHYT = item.DonGiaBaoHiem,
                                    TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                    LaTrongGoi = false
                                }).ToList();
                    if (truyenMaus.Any())
                    {
                        lstTemp.AddRange(truyenMaus);
                    }


                    var giuongs = _yeuCauDichVuGiuongBenhVienChiPhiBenhVienRepository.TableNoTracking
                                .Where(x => lstMaTiepTiepNhanChuaThuTien.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan))
                                .Select(item => new ThongTinDichVuChuaThuTienVo()
                                {
                                    YeuCauDichVuGiuongChiPhiBenhVienId = item.Id,
                                    YeucauTiepNhanId = item.YeuCauTiepNhanId,
                                    MaYeucauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                    TenDichVu = item.Ten,
                                    SoLuong = item.SoLuong,
                                    DonGia = item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau.Value : item.Gia,
                                    SoTienMienGiamTheoDichVu = item.SoTienMienGiam,
                                    //DuocHuongBHYT = item.BaoHiemChiTra != null && item.BaoHiemChiTra == true,
                                    //MucHuongBaoHiem = item.MucHuongBaoHiem.GetValueOrDefault(),
                                    //DonGiaBHYT = item.DonGiaBaoHiem,
                                    //TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                    LaTrongGoi = item.YeuCauGoiDichVuId != null
                                }).ToList();
                    if (giuongs.Any())
                    {
                        lstTemp.AddRange(giuongs);
                    }

                    var duocPhams = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                                .Where(x => x.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                                            && x.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan
                                            && x.KhongTinhPhi != true
                                            && lstMaTiepTiepNhanChuaThuTien.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan))
                                .Select(item => new ThongTinDichVuChuaThuTienVo()
                                {
                                    YeucauTiepNhanId = item.YeuCauTiepNhanId,
                                    MaYeucauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                    TenDichVu = item.Ten,
                                    SoLuong = item.SoLuong,
                                    //GiaBan = item.GiaBan,
                                    SoTienMienGiamTheoDichVu = item.SoTienMienGiam,
                                    DuocHuongBHYT = item.BaoHiemChiTra != null && item.BaoHiemChiTra == true,
                                    MucHuongBaoHiem = item.MucHuongBaoHiem.GetValueOrDefault(),
                                    DonGiaBHYT = item.DonGiaBaoHiem,
                                    TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                    LaTrongGoi = item.YeuCauGoiDichVuId != null,

                                    // xử lý tự tính giá bán
                                    XuatKhoChiTietId = item.XuatKhoDuocPhamChiTietId,
                                    DonGiaNhap = item.DonGiaNhap,
                                    VAT = item.VAT,
                                    TiLeTheoThapGia = item.TiLeTheoThapGia,
                                    PhuongPhapTinhGiaTriTonKhos = item.XuatKhoDuocPhamChiTietId != null
                                        ? item.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Select(a => a.NhapKhoDuocPhamChiTiet.PhuongPhapTinhGiaTriTonKho).ToList()
                                        : new List<Enums.PhuongPhapTinhGiaTriTonKho>()
                                }).ToList();
                    if (duocPhams.Any())
                    {
                        foreach (var duocPham in duocPhams)
                        {
                            duocPham.GiaBan = duocPham.GiaBanDuocPhamVatTu;
                        }
                        lstTemp.AddRange(duocPhams);
                    }

                    var vatTus = _yeuCauVatTuBenhVienRepository.TableNoTracking
                                .Where(x => x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy
                                            && x.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan
                                            && x.KhongTinhPhi != true
                                            && lstMaTiepTiepNhanChuaThuTien.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan))
                                .Select(item => new ThongTinDichVuChuaThuTienVo()
                                {
                                    YeucauTiepNhanId = item.YeuCauTiepNhanId,
                                    MaYeucauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                    TenDichVu = item.Ten,
                                    SoLuong = item.SoLuong,
                                    //GiaBan = item.GiaBan,
                                    SoTienMienGiamTheoDichVu = item.SoTienMienGiam,
                                    DuocHuongBHYT = item.BaoHiemChiTra != null && item.BaoHiemChiTra == true,
                                    MucHuongBaoHiem = item.MucHuongBaoHiem.GetValueOrDefault(),
                                    DonGiaBHYT = item.DonGiaBaoHiem,
                                    TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                    LaTrongGoi = item.YeuCauGoiDichVuId != null,

                                    // xử lý tự tính giá bán
                                    XuatKhoChiTietId = item.XuatKhoVatTuChiTietId,
                                    DonGiaNhap = item.DonGiaNhap,
                                    VAT = item.VAT,
                                    TiLeTheoThapGia = item.TiLeTheoThapGia,
                                    PhuongPhapTinhGiaTriTonKhos = item.XuatKhoVatTuChiTietId != null
                                        ? item.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.Select(a => a.NhapKhoVatTuChiTiet.PhuongPhapTinhGiaTriTonKho).ToList()
                                        : new List<Enums.PhuongPhapTinhGiaTriTonKho>()
                                }).ToList();
                    if (vatTus.Any())
                    {
                        foreach (var vatTu in vatTus)
                        {
                            vatTu.GiaBan = vatTu.GiaBanDuocPhamVatTu;
                        }
                        lstTemp.AddRange(vatTus);
                    }

                    var donThuocs = _donThuocThanhToanChiTietRepository.TableNoTracking
                                .Where(x => x.DonThuocThanhToan.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT
                                            && x.DonThuocThanhToan.TrangThai != TrangThaiDonThuocThanhToan.DaHuy
                                            && x.DonThuocThanhToan.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan
                                            //&& (lstMaTiepTiepNhanChuaThuTien.Contains(x.YeuCauKhamBenhDonThuocChiTiet.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.YeuCauTiepNhan.MaYeuCauTiepNhan)
                                            //    || lstMaTiepTiepNhanChuaThuTien.Contains(x.NoiTruDonThuocChiTiet.NoiTruDonThuoc.YeuCauTiepNhan.MaYeuCauTiepNhan)))
                                            && lstMaTiepTiepNhanChuaThuTien.Contains(x.DonThuocThanhToan.YeuCauTiepNhan.MaYeuCauTiepNhan))
                                .Select(item => new ThongTinDichVuChuaThuTienVo()
                                {
                                    YeucauTiepNhanId = item.DonThuocThanhToan.YeuCauTiepNhanId.GetValueOrDefault(),
                                    MaYeucauTiepNhan = item.DonThuocThanhToan.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                    TenDichVu = item.Ten,
                                    SoLuong = item.SoLuong,
                                    //GiaBan = item.GiaBan,
                                    SoTienMienGiamTheoDichVu = item.SoTienMienGiam,
                                    DuocHuongBHYT = item.BaoHiemChiTra != null && item.BaoHiemChiTra == true,
                                    MucHuongBaoHiem = item.MucHuongBaoHiem.GetValueOrDefault(),
                                    DonGiaBHYT = item.DonGiaBaoHiem,
                                    TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                    LaTrongGoi = false,

                                    // xử lý tự tính giá bán
                                    XuatKhoChiTietId = item.XuatKhoDuocPhamChiTietViTriId,
                                    DonGiaNhap = item.DonGiaNhap,
                                    VAT = item.VAT,
                                    TiLeTheoThapGia = item.TiLeTheoThapGia,
                                    PhuongPhapTinhGiaTriTonKhoDonThuoc = item.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.PhuongPhapTinhGiaTriTonKho
                                }).ToList();
                    if (donThuocs.Any())
                    {
                        foreach (var donThuoc in donThuocs)
                        {
                            if (donThuoc.PhuongPhapTinhGiaTriTonKhoDonThuoc != null)
                            {
                                donThuoc.PhuongPhapTinhGiaTriTonKhos = new List<PhuongPhapTinhGiaTriTonKho>();
                                donThuoc.PhuongPhapTinhGiaTriTonKhos.Add(donThuoc.PhuongPhapTinhGiaTriTonKhoDonThuoc.Value);
                            }
                            donThuoc.GiaBan = donThuoc.GiaBanDuocPhamVatTu;
                        }
                        lstTemp.AddRange(donThuocs);
                    }

                    #endregion

                    #region // xử lý check chi phí BHYT dịch vụ giường
                    var chiPhiGiuongs = lstTemp.Where(x => x.YeuCauDichVuGiuongChiPhiBenhVienId != null).ToList();
                    if (chiPhiGiuongs.Any())
                    {
                        var lstChiPhiGiuongBenhVienId = chiPhiGiuongs
                            .Where(x => x.YeuCauDichVuGiuongChiPhiBenhVienId != null)
                            .Select(x => x.YeuCauDichVuGiuongChiPhiBenhVienId.Value).Distinct().ToList();

                        var chiPhiGiuongBHYTs = await _yeuCauDichVuGiuongBenhVienChiPhiBhytRepository.TableNoTracking
                            .Where(x => lstChiPhiGiuongBenhVienId.Contains(x.Id))
                            .Select(item => new ThongTinDichVuChuaThuTienVo()
                            {
                                YeuCauDichVuGiuongChiPhiBenhVienId = item.Id,
                                DuocHuongBHYT = item.BaoHiemChiTra != null && item.BaoHiemChiTra == true,
                                MucHuongBaoHiem = item.MucHuongBaoHiem.GetValueOrDefault(),
                                DonGiaBHYT = item.DonGiaBaoHiem,
                                TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan.GetValueOrDefault()
                            })
                            .ToListAsync();

                        foreach (var chiPhiGiuongBenhVien in chiPhiGiuongs)
                        {
                            var chiPhiBHYT = chiPhiGiuongBHYTs.FirstOrDefault(x => x.YeuCauDichVuGiuongChiPhiBenhVienId == chiPhiGiuongBenhVien.YeuCauDichVuGiuongChiPhiBenhVienId);
                            if (chiPhiBHYT != null)
                            {
                                chiPhiGiuongBenhVien.DuocHuongBHYT = chiPhiBHYT.DuocHuongBHYT;
                                chiPhiGiuongBenhVien.MucHuongBaoHiem = chiPhiBHYT.MucHuongBaoHiem;
                                chiPhiGiuongBenhVien.DonGiaBHYT = chiPhiBHYT.DonGiaBHYT;
                                chiPhiGiuongBenhVien.TiLeBaoHiemThanhToan = chiPhiBHYT.TiLeBaoHiemThanhToan;
                            }
                        }
                    }

                    #endregion

                    var lstTiepNhanBoSungTheoPhanLoai = new List<ThongTinTiepNhanChuaThuTienCoHinhThucDenVo>();
                    foreach (var lanTiepNhan in lstTiepNhanTheoNoiGioiThieu)
                    {
                        //var chiPhiTheoLanTiepNhan = chiPhiKhams.Where(x => x.YeucauTiepNhanId == lanTiepNhan.YeucauTiepNhanId)
                        //    .Union(chiPhiKyThuats.Where(x => x.YeucauTiepNhanId == lanTiepNhan.YeucauTiepNhanId))
                        //    .Union(chiPhiTruyenMaus.Where(x => x.YeucauTiepNhanId == lanTiepNhan.YeucauTiepNhanId))
                        //    .Union(chiPhiGiuongs.Where(x => x.YeucauTiepNhanId == lanTiepNhan.YeucauTiepNhanId))
                        //    .Union(chiPhiDuocPhams.Where(x => x.YeucauTiepNhanId == lanTiepNhan.YeucauTiepNhanId))
                        //    .Union(chiPhiVatTus.Where(x => x.YeucauTiepNhanId == lanTiepNhan.YeucauTiepNhanId))
                        //    .Union(chiPhiDonThuocBHYts.Where(x => x.YeucauTiepNhanId == lanTiepNhan.YeucauTiepNhanId))
                        //    .ToList();

                        var chiPhiTheoLanTiepNhan = lstTemp.Where(x => x.YeucauTiepNhanId == lanTiepNhan.YeucauTiepNhanId)
                            .ToList();

                        if (lanTiepNhan.LaTiepNhanNoiTru)
                        {
                            var chiPhiNoiTruNgoaiGoi = chiPhiTheoLanTiepNhan.Where(x => !x.LaTrongGoi).Sum(x => x.ThanhToan);
                            var chiPhiNoiTruTrongGoi = chiPhiTheoLanTiepNhan.Where(x => x.LaTrongGoi).Sum(x => x.ThanhToan);

                            if (chiPhiNoiTruNgoaiGoi > 0)
                            {
                                lanTiepNhan.TongTienChuaThanhToan = chiPhiNoiTruNgoaiGoi;
                            }

                            if (chiPhiNoiTruTrongGoi > 0)
                            {
                                var lanTiepNhanTrongGoi = lanTiepNhan.Clone();
                                lanTiepNhanTrongGoi.TrongGoi = true;
                                lanTiepNhanTrongGoi.TongTienChuaThanhToan = chiPhiNoiTruTrongGoi;
                                lstTiepNhanBoSungTheoPhanLoai.Add(lanTiepNhanTrongGoi);
                            }
                        }
                        else
                        {
                            var chiPhiNgoaiTruNgoaiGoi = chiPhiTheoLanTiepNhan.Where(x => !x.LaTrongGoi).Sum(x => x.ThanhToan);
                            var chiPhiNgoaiTruTrongGoi = chiPhiTheoLanTiepNhan.Where(x => x.LaTrongGoi).Sum(x => x.ThanhToan);

                            if (chiPhiNgoaiTruNgoaiGoi > 0)
                            {
                                lanTiepNhan.TongTienChuaThanhToan = chiPhiNgoaiTruNgoaiGoi;
                            }

                            if (chiPhiNgoaiTruTrongGoi > 0)
                            {
                                var lanTiepNhanTrongGoi = lanTiepNhan.Clone();
                                lanTiepNhanTrongGoi.TrongGoi = true;
                                lanTiepNhanTrongGoi.TongTienChuaThanhToan = chiPhiNgoaiTruTrongGoi;
                                lstTiepNhanBoSungTheoPhanLoai.Add(lanTiepNhanTrongGoi);
                            }
                        }
                    }
                    lstTiepNhanTheoNoiGioiThieu.AddRange(lstTiepNhanBoSungTheoPhanLoai);

                    #region Cập nhật 25/07/2022: thay thế tính tổng
                    if (lstTiepNhanTheoNoiGioiThieu.Any())
                    {
                        tongTien = lstTiepNhanTheoNoiGioiThieu.Sum(a => a.TongTienChuaThanhToan);
                        total = lstTiepNhanTheoNoiGioiThieu.Count();
                    }
                    #endregion

                    lstTiepNhanTheoNoiGioiThieu = lstTiepNhanTheoNoiGioiThieu
                        .Where(x => x.TongTienChuaThanhToan > 0)
                        .OrderBy(x => x.MaYeucauTiepNhan).ThenBy(x => x.ThoiDiemTiepNhan)
                        .Skip(queryInfo.Skip).Take(queryInfo.Take).ToList();

                    #region Cập nhật 25/07/2022: thay thế tính tổng
                    if (lstTiepNhanTheoNoiGioiThieu.Any())
                    {
                        var item = lstTiepNhanTheoNoiGioiThieu.First();
                        item.TongTienChuaThanhToanHienThiGrid = tongTien;
                    }
                    #endregion
                }
            }

            return new GridDataSource
            {
                Data = lstTiepNhanTheoNoiGioiThieu.ToArray(),
                TotalRowCount = total
            };
        }

        public async Task<GridDataSource> GetTotalPageThongKeCacDVChuaLayLenBienLaiThuTienForGrid(QueryInfo queryInfo)
        {
            var lstTiepNhanTheoNoiGioiThieu = new List<ThongTinTiepNhanChuaThuTienCoHinhThucDenVo>();
            var timKiemNangCaoObj = new ThongKeCacDichVuChuaLayLenBienLaiThuTienQueryInfoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<ThongKeCacDichVuChuaLayLenBienLaiThuTienQueryInfoVo>(queryInfo.AdditionalSearchString);
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;

            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.FromDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.FromDate))
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
                lstTiepNhanTheoNoiGioiThieu = _yeuCauTiepNhanRepository.TableNoTracking
                    .ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.MaYeuCauTiepNhan, x => x.HoTen, x => x.BenhNhan.MaBN)
                    .Where(x => x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien
                                && x.BenhNhanId != null

                                // Lấy gốc là theo Mã YCTN, nếu YCTN là ngoại trú hoặc KSK thì kiểm tra thêm xem có YCTN nội trú ko
                                && (
                                    x.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru
                                    || x.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamSucKhoe
                                    || (x.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && x.NoiTruBenhAn.LoaiBenhAn == LoaiBenhAn.TreSoSinh)
                                    )
                                && x.ThoiDiemTiepNhan >= tuNgay
                                && x.ThoiDiemTiepNhan <= denNgay)
                    .Select(x => new ThongTinTiepNhanChuaThuTienCoHinhThucDenVo()
                    {
                        YeucauTiepNhanId = x.Id,
                        MaYeucauTiepNhan = x.MaYeuCauTiepNhan,
                        MaBenhAn = x.NoiTruBenhAn.SoBenhAn,
                        MaBN = x.BenhNhan.MaBN,
                        BenhNhanId = x.BenhNhanId.Value,
                        HoVaTen = x.HoTen,
                        NamSinh = x.NamSinh,
                        DiaChi = x.DiaChiDayDu,

                        NoiGioiThieuId = x.NoiGioiThieuId,
                        ThoiDiemTiepNhan = x.ThoiDiemTiepNhan,
                        NoiGioiThieuDisplay = x.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.NoiGioiThieu.DonVi) ? $" - {x.NoiGioiThieu.DonVi}" : ""),
                        TenHinhThucDen = x.HinhThucDen.Ten,
                        LaGioiThieu = x.NoiGioiThieuId != null,
                        LaTiepNhanNoiTru = x.NoiTruBenhAn != null,
                        LaBenhAnSoSinh = x.NoiTruBenhAn.LoaiBenhAn == LoaiBenhAn.TreSoSinh
                    })
                    .OrderBy(x => x.ThoiDiemTiepNhan)
                    .ToList();
                if (lstTiepNhanTheoNoiGioiThieu.Any())
                {
                    var lstBenhNhanId = lstTiepNhanTheoNoiGioiThieu.Select(x => x.BenhNhanId).Distinct().ToList();
                    var lstMaTiepNhanChinh = lstTiepNhanTheoNoiGioiThieu.Select(x => x.MaYeucauTiepNhan).Distinct().ToList();

                    var lstTiepNhanTheoBenhNhan = _yeuCauTiepNhanRepository.TableNoTracking
                        .Where(x => x.BenhNhanId != null
                                    && lstBenhNhanId.Contains(x.BenhNhanId.Value)
                                    // Lấy gốc là theo Mã YCTN, nếu YCTN là ngoại trú hoặc KSK thì kiểm tra thêm xem có YCTN nội trú ko
                                    && (
                                        x.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru
                                        || x.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamSucKhoe
                                        || (x.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru) // && (x.NoiTruBenhAn.LoaiBenhAn == LoaiBenhAn.TreSoSinh || lstMaTiepNhanChinh.Contains(x.MaYeuCauTiepNhan)))
                                    )
                                    && x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien)
                        .Select(x => new ThongTinTiepNhanChuaThuTienCoHinhThucDenVo()
                        {
                            YeucauTiepNhanId = x.Id,
                            MaYeucauTiepNhan = x.MaYeuCauTiepNhan,
                            MaBenhAn = x.NoiTruBenhAn.SoBenhAn,
                            MaBN = x.BenhNhan.MaBN,
                            BenhNhanId = x.BenhNhanId.Value,
                            HoVaTen = x.HoTen,
                            NamSinh = x.NamSinh,
                            DiaChi = x.DiaChiDayDu,

                            NoiGioiThieuId = x.NoiGioiThieuId,
                            ThoiDiemTiepNhan = x.ThoiDiemTiepNhan,
                            NoiGioiThieuDisplay = x.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.NoiGioiThieu.DonVi) ? $" - {x.NoiGioiThieu.DonVi}" : ""),
                            TenHinhThucDen = x.HinhThucDen.Ten,
                            LaGioiThieu = x.NoiGioiThieuId != null,

                            // dùng để xác định người giới thiệu trước đó nếu data hiện tại ko có người giới thiệu
                            LaDataTheoDieuKienTimKiem = lstMaTiepNhanChinh.Contains(x.MaYeuCauTiepNhan),

                            LaTiepNhanNoiTru = x.NoiTruBenhAn != null,
                            LaBenhAnSoSinh = x.NoiTruBenhAn.LoaiBenhAn == LoaiBenhAn.TreSoSinh
                        })
                        .OrderBy(x => x.ThoiDiemTiepNhan)
                        .ToList();

                    foreach (var benhNhanId in lstBenhNhanId)
                    {
                        var lanTiepNhanDauTienCoGioiThieu = lstTiepNhanTheoNoiGioiThieu
                            .Where(x => x.BenhNhanId == benhNhanId && x.NoiGioiThieuId != null)
                            .OrderBy(x => x.YeucauTiepNhanId).FirstOrDefault();
                        if (lanTiepNhanDauTienCoGioiThieu == null)
                        {
                            lanTiepNhanDauTienCoGioiThieu = lstTiepNhanTheoBenhNhan
                                .Where(x => x.BenhNhanId == benhNhanId && x.NoiGioiThieuId != null)
                                .OrderBy(x => x.YeucauTiepNhanId).FirstOrDefault();
                        }

                        var tiepNhanBenhNhans = lstTiepNhanTheoBenhNhan
                            .Where(x => (lanTiepNhanDauTienCoGioiThieu == null || x.YeucauTiepNhanId > lanTiepNhanDauTienCoGioiThieu.YeucauTiepNhanId)
                                        && x.BenhNhanId == benhNhanId)
                            .ToList();

                        var nguoiGioiThieuHienTaiId = lanTiepNhanDauTienCoGioiThieu?.NoiGioiThieuId;
                        var tenNguoiGioiThieuHienTai = lanTiepNhanDauTienCoGioiThieu?.NoiGioiThieuDisplay;
                        foreach (var lanTiepNhan in tiepNhanBenhNhans)
                        {
                            if (lanTiepNhan.NoiGioiThieuId != null && lanTiepNhan.NoiGioiThieuId != nguoiGioiThieuHienTaiId)
                            {
                                nguoiGioiThieuHienTaiId = lanTiepNhan.NoiGioiThieuId;
                                tenNguoiGioiThieuHienTai = lanTiepNhan.NoiGioiThieuDisplay;
                            }

                            if (lstTiepNhanTheoNoiGioiThieu.All(x => x.YeucauTiepNhanId != lanTiepNhan.YeucauTiepNhanId) && lanTiepNhan.LaDataTheoDieuKienTimKiem == true)
                            {
                                lanTiepNhan.NoiGioiThieuId = nguoiGioiThieuHienTaiId;
                                lanTiepNhan.NoiGioiThieuDisplay = tenNguoiGioiThieuHienTai;
                                lanTiepNhan.LaGioiThieu = nguoiGioiThieuHienTaiId != null;
                                lstTiepNhanTheoNoiGioiThieu.Add(lanTiepNhan);
                            }
                            else
                            {
                                if (nguoiGioiThieuHienTaiId != null)
                                {
                                    var lanTiepNhanDaThem = lstTiepNhanTheoNoiGioiThieu.FirstOrDefault(x => x.YeucauTiepNhanId == lanTiepNhan.YeucauTiepNhanId);
                                    if (lanTiepNhanDaThem != null)
                                    {
                                        lanTiepNhanDaThem.NoiGioiThieuId = nguoiGioiThieuHienTaiId;
                                        lanTiepNhanDaThem.NoiGioiThieuDisplay = tenNguoiGioiThieuHienTai;
                                        lanTiepNhanDaThem.LaGioiThieu = nguoiGioiThieuHienTaiId != null;
                                    }
                                }
                            }
                        }
                    }

                    var lstMaTiepTiepNhanChuaThuTien = lstTiepNhanTheoNoiGioiThieu.Select(x => x.MaYeucauTiepNhan).Distinct().ToList();

                    #region code cũ
                    //#region Chi phí khám bệnh chưa thu
                    //var chiPhiKhams = await _yeuCauKhamBenhRepository.TableNoTracking
                    //.Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham
                    //            && x.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan
                    //            && x.KhongTinhPhi != true
                    //            && lstMaTiepTiepNhanChuaThuTien.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan)
                    //            && x.GoiKhamSucKhoeId == null)
                    //.Select(item => new ThongTinDichVuChuaThuTienVo()
                    //{
                    //    YeucauTiepNhanId = item.YeuCauTiepNhanId,
                    //    MaYeucauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    //    TenDichVu = item.TenDichVu,
                    //    SoLuong = 1,
                    //    DonGia = item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau.Value : item.Gia,
                    //    SoTienMienGiamTheoDichVu = item.SoTienMienGiam,
                    //    DuocHuongBHYT = item.BaoHiemChiTra != null && item.BaoHiemChiTra == true,
                    //    MucHuongBaoHiem = item.MucHuongBaoHiem.GetValueOrDefault(),
                    //    DonGiaBHYT = item.DonGiaBaoHiem,
                    //    TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    //    LaTrongGoi = item.YeuCauGoiDichVuId != null
                    //})
                    //.ToListAsync();
                    //#endregion

                    //#region Chi phí kỹ thuật chưa thu
                    //var chiPhiKyThuats = await _yeuCauDichVuKyThuatRepository.TableNoTracking
                    //    .Where(x => x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                    //                && x.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan
                    //                && x.KhongTinhPhi != true
                    //                && lstMaTiepTiepNhanChuaThuTien.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan)
                    //                && x.GoiKhamSucKhoeId == null)
                    //    .Select(item => new ThongTinDichVuChuaThuTienVo()
                    //    {
                    //        YeucauTiepNhanId = item.YeuCauTiepNhanId,
                    //        MaYeucauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    //        TenDichVu = item.TenDichVu,
                    //        SoLuong = item.SoLan,
                    //        DonGia = item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau.Value : item.Gia,
                    //        SoTienMienGiamTheoDichVu = item.SoTienMienGiam,
                    //        DuocHuongBHYT = item.BaoHiemChiTra != null && item.BaoHiemChiTra == true,
                    //        MucHuongBaoHiem = item.MucHuongBaoHiem.GetValueOrDefault(),
                    //        DonGiaBHYT = item.DonGiaBaoHiem,
                    //        TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    //        LaTrongGoi = item.YeuCauGoiDichVuId != null
                    //    })
                    //    .ToListAsync();
                    //#endregion

                    //#region Chi phí truyền máu chưa thu
                    //var chiPhiTruyenMaus = await _yeuCauTruyenMauRepository.TableNoTracking
                    //    .Where(x => x.TrangThai != EnumTrangThaiYeuCauTruyenMau.DaHuy
                    //                && x.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan
                    //                && lstMaTiepTiepNhanChuaThuTien.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan))
                    //    .Select(item => new ThongTinDichVuChuaThuTienVo()
                    //    {
                    //        YeucauTiepNhanId = item.YeuCauTiepNhanId,
                    //        MaYeucauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    //        TenDichVu = item.TenDichVu,
                    //        SoLuong = 1,
                    //        DonGia = item.DonGiaBan.Value,
                    //        SoTienMienGiamTheoDichVu = item.SoTienMienGiam,
                    //        DuocHuongBHYT = item.BaoHiemChiTra != null && item.BaoHiemChiTra == true,
                    //        MucHuongBaoHiem = item.MucHuongBaoHiem.GetValueOrDefault(),
                    //        DonGiaBHYT = item.DonGiaBaoHiem,
                    //        TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    //        LaTrongGoi = false
                    //    })
                    //    .ToListAsync();
                    //#endregion

                    //#region Chi phí giường chưa thu
                    //var chiPhiGiuongs = await _yeuCauDichVuGiuongBenhVienChiPhiBenhVienRepository.TableNoTracking
                    //    .Where(x => lstMaTiepTiepNhanChuaThuTien.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan))
                    //    .Select(item => new ThongTinDichVuChuaThuTienVo()
                    //    {
                    //        YeuCauDichVuGiuongChiPhiBenhVienId = item.Id,
                    //        YeucauTiepNhanId = item.YeuCauTiepNhanId,
                    //        MaYeucauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    //        TenDichVu = item.Ten,
                    //        SoLuong = item.SoLuong,
                    //        DonGia = item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau.Value : item.Gia,
                    //        SoTienMienGiamTheoDichVu = item.SoTienMienGiam,
                    //        //DuocHuongBHYT = item.BaoHiemChiTra != null && item.BaoHiemChiTra == true,
                    //        //MucHuongBaoHiem = item.MucHuongBaoHiem.GetValueOrDefault(),
                    //        //DonGiaBHYT = item.DonGiaBaoHiem,
                    //        //TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    //        LaTrongGoi = item.YeuCauGoiDichVuId != null
                    //    })
                    //    .ToListAsync();

                    //if (chiPhiGiuongs.Any())
                    //{
                    //    var lstChiPhiGiuongBenhVienId = chiPhiGiuongs
                    //        .Where(x => x.YeuCauDichVuGiuongChiPhiBenhVienId != null)
                    //        .Select(x => x.YeuCauDichVuGiuongChiPhiBenhVienId.Value).Distinct().ToList();

                    //    var chiPhiGiuongBHYTs = await _yeuCauDichVuGiuongBenhVienChiPhiBhytRepository.TableNoTracking
                    //        .Where(x => lstChiPhiGiuongBenhVienId.Contains(x.Id))
                    //        .Select(item => new ThongTinDichVuChuaThuTienVo()
                    //        {
                    //            YeuCauDichVuGiuongChiPhiBenhVienId = item.Id,
                    //            DuocHuongBHYT = item.BaoHiemChiTra != null && item.BaoHiemChiTra == true,
                    //            MucHuongBaoHiem = item.MucHuongBaoHiem.GetValueOrDefault(),
                    //            DonGiaBHYT = item.DonGiaBaoHiem,
                    //            TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan.GetValueOrDefault()
                    //        })
                    //        .ToListAsync();

                    //    foreach (var chiPhiGiuongBenhVien in chiPhiGiuongs)
                    //    {
                    //        var chiPhiBHYT = chiPhiGiuongBHYTs.FirstOrDefault(x => x.YeuCauDichVuGiuongChiPhiBenhVienId == chiPhiGiuongBenhVien.YeuCauDichVuGiuongChiPhiBenhVienId);
                    //        if (chiPhiBHYT != null)
                    //        {
                    //            chiPhiGiuongBenhVien.DuocHuongBHYT = chiPhiBHYT.DuocHuongBHYT;
                    //            chiPhiGiuongBenhVien.MucHuongBaoHiem = chiPhiBHYT.MucHuongBaoHiem;
                    //            chiPhiGiuongBenhVien.DonGiaBHYT = chiPhiBHYT.DonGiaBHYT;
                    //            chiPhiGiuongBenhVien.TiLeBaoHiemThanhToan = chiPhiBHYT.TiLeBaoHiemThanhToan;
                    //        }
                    //    }
                    //}
                    //#endregion

                    //#region Chi phí dược phẩm chưa thu
                    //var chiPhiDuocPhams = await _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                    //    .Where(x => x.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                    //                && x.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan
                    //                && x.KhongTinhPhi != true
                    //                && lstMaTiepTiepNhanChuaThuTien.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan))
                    //    .Select(item => new ThongTinDichVuChuaThuTienVo()
                    //    {
                    //        YeucauTiepNhanId = item.YeuCauTiepNhanId,
                    //        MaYeucauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    //        TenDichVu = item.Ten,
                    //        SoLuong = item.SoLuong,
                    //        GiaBan = item.DonGiaBan,
                    //        SoTienMienGiamTheoDichVu = item.SoTienMienGiam,
                    //        DuocHuongBHYT = item.BaoHiemChiTra != null && item.BaoHiemChiTra == true,
                    //        MucHuongBaoHiem = item.MucHuongBaoHiem.GetValueOrDefault(),
                    //        DonGiaBHYT = item.DonGiaBaoHiem,
                    //        TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    //        LaTrongGoi = item.YeuCauGoiDichVuId != null
                    //    })
                    //    .ToListAsync();
                    //#endregion

                    //#region Chi phí vật tư chưa thu
                    //var chiPhiVatTus = await _yeuCauVatTuBenhVienRepository.TableNoTracking
                    //    .Where(x => x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy
                    //                && x.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan
                    //                && x.KhongTinhPhi != true
                    //                && lstMaTiepTiepNhanChuaThuTien.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan))
                    //    .Select(item => new ThongTinDichVuChuaThuTienVo()
                    //    {
                    //        YeucauTiepNhanId = item.YeuCauTiepNhanId,
                    //        MaYeucauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    //        TenDichVu = item.Ten,
                    //        SoLuong = item.SoLuong,
                    //        GiaBan = item.DonGiaBan,
                    //        SoTienMienGiamTheoDichVu = item.SoTienMienGiam,
                    //        DuocHuongBHYT = item.BaoHiemChiTra != null && item.BaoHiemChiTra == true,
                    //        MucHuongBaoHiem = item.MucHuongBaoHiem.GetValueOrDefault(),
                    //        DonGiaBHYT = item.DonGiaBaoHiem,
                    //        TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    //        LaTrongGoi = item.YeuCauGoiDichVuId != null
                    //    })
                    //    .ToListAsync();
                    //#endregion

                    //#region Chi phí đơn thuốc BHYTs chưa thu
                    //var chiPhiDonThuocBHYts = await _donThuocThanhToanChiTietRepository.TableNoTracking
                    //    .Where(x => x.DonThuocThanhToan.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT
                    //                && (lstMaTiepTiepNhanChuaThuTien.Contains(x.YeuCauKhamBenhDonThuocChiTiet.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.YeuCauTiepNhan.MaYeuCauTiepNhan)
                    //                || lstMaTiepTiepNhanChuaThuTien.Contains(x.NoiTruDonThuocChiTiet.NoiTruDonThuoc.YeuCauTiepNhan.MaYeuCauTiepNhan)))
                    //    .Select(item => new ThongTinDichVuChuaThuTienVo()
                    //    {
                    //        YeucauTiepNhanId = item.YeuCauKhamBenhDonThuocChiTiet != null
                    //            ? item.YeuCauKhamBenhDonThuocChiTiet.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.YeuCauTiepNhanId
                    //            : item.NoiTruDonThuocChiTiet.NoiTruDonThuoc.YeuCauTiepNhanId,
                    //        MaYeucauTiepNhan = item.YeuCauKhamBenhDonThuocChiTiet.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.YeuCauTiepNhan.MaYeuCauTiepNhan ?? item.NoiTruDonThuocChiTiet.NoiTruDonThuoc.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    //        TenDichVu = item.Ten,
                    //        SoLuong = item.SoLuong,
                    //        GiaBan = item.DonGiaBan,
                    //        SoTienMienGiamTheoDichVu = item.SoTienMienGiam,
                    //        DuocHuongBHYT = item.BaoHiemChiTra != null && item.BaoHiemChiTra == true,
                    //        MucHuongBaoHiem = item.MucHuongBaoHiem.GetValueOrDefault(),
                    //        DonGiaBHYT = item.DonGiaBaoHiem,
                    //        TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    //        LaTrongGoi = false
                    //    })
                    //    .ToListAsync();
                    //#endregion

                    #endregion

                    #region Get chi phí theo dịch vụ
                    //Cập nhật 25/07/2022: bỏ union, tách query
                    var lstTemp = new List<ThongTinDichVuChuaThuTienVo>();
                    var dvKhams = _yeuCauKhamBenhRepository.TableNoTracking
                        .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham
                                    && x.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan
                                    && x.KhongTinhPhi != true
                                    && lstMaTiepTiepNhanChuaThuTien.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan)
                                    && x.GoiKhamSucKhoeId == null)
                        .Select(item => new ThongTinDichVuChuaThuTienVo()
                        {
                            YeucauTiepNhanId = item.YeuCauTiepNhanId,
                            MaYeucauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                            TenDichVu = item.TenDichVu,
                            SoLuong = 1,
                            DonGia = item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau.Value : item.Gia,
                            SoTienMienGiamTheoDichVu = item.SoTienMienGiam,
                            DuocHuongBHYT = item.BaoHiemChiTra != null && item.BaoHiemChiTra == true,
                            MucHuongBaoHiem = item.MucHuongBaoHiem.GetValueOrDefault(),
                            DonGiaBHYT = item.DonGiaBaoHiem,
                            TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                            LaTrongGoi = item.YeuCauGoiDichVuId != null
                        }).ToList();
                    if (dvKhams.Any())
                    {
                        lstTemp.AddRange(dvKhams);
                    }
                    var dvKyThuats = _yeuCauDichVuKyThuatRepository.TableNoTracking
                                .Where(x => x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                            && x.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan
                                            && x.KhongTinhPhi != true
                                            && lstMaTiepTiepNhanChuaThuTien.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan)
                                            && x.GoiKhamSucKhoeId == null)
                                .Select(item => new ThongTinDichVuChuaThuTienVo()
                                {
                                    YeucauTiepNhanId = item.YeuCauTiepNhanId,
                                    MaYeucauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                    TenDichVu = item.TenDichVu,
                                    SoLuong = item.SoLan,
                                    DonGia = item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau.Value : item.Gia,
                                    SoTienMienGiamTheoDichVu = item.SoTienMienGiam,
                                    DuocHuongBHYT = item.BaoHiemChiTra != null && item.BaoHiemChiTra == true,
                                    MucHuongBaoHiem = item.MucHuongBaoHiem.GetValueOrDefault(),
                                    DonGiaBHYT = item.DonGiaBaoHiem,
                                    TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                    LaTrongGoi = item.YeuCauGoiDichVuId != null
                                }).ToList();
                    if (dvKyThuats.Any())
                    {
                        lstTemp.AddRange(dvKyThuats);
                    }
                    var truyenMaus = _yeuCauTruyenMauRepository.TableNoTracking
                                .Where(x => x.TrangThai != EnumTrangThaiYeuCauTruyenMau.DaHuy
                                            && x.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan
                                            && lstMaTiepTiepNhanChuaThuTien.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan))
                                .Select(item => new ThongTinDichVuChuaThuTienVo()
                                {
                                    YeucauTiepNhanId = item.YeuCauTiepNhanId,
                                    MaYeucauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                    TenDichVu = item.TenDichVu,
                                    SoLuong = 1,
                                    DonGia = item.DonGiaBan.Value,
                                    SoTienMienGiamTheoDichVu = item.SoTienMienGiam,
                                    DuocHuongBHYT = item.BaoHiemChiTra != null && item.BaoHiemChiTra == true,
                                    MucHuongBaoHiem = item.MucHuongBaoHiem.GetValueOrDefault(),
                                    DonGiaBHYT = item.DonGiaBaoHiem,
                                    TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                    LaTrongGoi = false
                                }).ToList();
                    if (truyenMaus.Any())
                    {
                        lstTemp.AddRange(truyenMaus);
                    }
                    var giuongs = _yeuCauDichVuGiuongBenhVienChiPhiBenhVienRepository.TableNoTracking
                                .Where(x => lstMaTiepTiepNhanChuaThuTien.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan))
                                .Select(item => new ThongTinDichVuChuaThuTienVo()
                                {
                                    YeuCauDichVuGiuongChiPhiBenhVienId = item.Id,
                                    YeucauTiepNhanId = item.YeuCauTiepNhanId,
                                    MaYeucauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                    TenDichVu = item.Ten,
                                    SoLuong = item.SoLuong,
                                    DonGia = item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau.Value : item.Gia,
                                    SoTienMienGiamTheoDichVu = item.SoTienMienGiam,
                                    //DuocHuongBHYT = item.BaoHiemChiTra != null && item.BaoHiemChiTra == true,
                                    //MucHuongBaoHiem = item.MucHuongBaoHiem.GetValueOrDefault(),
                                    //DonGiaBHYT = item.DonGiaBaoHiem,
                                    //TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                    LaTrongGoi = item.YeuCauGoiDichVuId != null
                                }).ToList();
                    if (giuongs.Any())
                    {
                        lstTemp.AddRange(giuongs);
                    }
                    var duocPhams = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                                .Where(x => x.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                                            && x.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan
                                            && x.KhongTinhPhi != true
                                            && lstMaTiepTiepNhanChuaThuTien.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan))
                                .Select(item => new ThongTinDichVuChuaThuTienVo()
                                {
                                    YeucauTiepNhanId = item.YeuCauTiepNhanId,
                                    MaYeucauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                    TenDichVu = item.Ten,
                                    SoLuong = item.SoLuong,
                                    GiaBan = item.DonGiaBan,
                                    SoTienMienGiamTheoDichVu = item.SoTienMienGiam,
                                    DuocHuongBHYT = item.BaoHiemChiTra != null && item.BaoHiemChiTra == true,
                                    MucHuongBaoHiem = item.MucHuongBaoHiem.GetValueOrDefault(),
                                    DonGiaBHYT = item.DonGiaBaoHiem,
                                    TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                    LaTrongGoi = item.YeuCauGoiDichVuId != null
                                }).ToList();
                    if (duocPhams.Any())
                    {
                        lstTemp.AddRange(duocPhams);
                    }
                    var vatTus = _yeuCauVatTuBenhVienRepository.TableNoTracking
                                .Where(x => x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy
                                            && x.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan
                                            && x.KhongTinhPhi != true
                                            && lstMaTiepTiepNhanChuaThuTien.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan))
                                .Select(item => new ThongTinDichVuChuaThuTienVo()
                                {
                                    YeucauTiepNhanId = item.YeuCauTiepNhanId,
                                    MaYeucauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                    TenDichVu = item.Ten,
                                    SoLuong = item.SoLuong,
                                    GiaBan = item.DonGiaBan,
                                    SoTienMienGiamTheoDichVu = item.SoTienMienGiam,
                                    DuocHuongBHYT = item.BaoHiemChiTra != null && item.BaoHiemChiTra == true,
                                    MucHuongBaoHiem = item.MucHuongBaoHiem.GetValueOrDefault(),
                                    DonGiaBHYT = item.DonGiaBaoHiem,
                                    TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                    LaTrongGoi = item.YeuCauGoiDichVuId != null
                                }).ToList();
                    if (vatTus.Any())
                    {
                        lstTemp.AddRange(vatTus);
                    }
                    var donThuocs = _donThuocThanhToanChiTietRepository.TableNoTracking
                                .Where(x => x.DonThuocThanhToan.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT
                                            //&& (lstMaTiepTiepNhanChuaThuTien.Contains(x.YeuCauKhamBenhDonThuocChiTiet.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.YeuCauTiepNhan.MaYeuCauTiepNhan)
                                            //    || lstMaTiepTiepNhanChuaThuTien.Contains(x.NoiTruDonThuocChiTiet.NoiTruDonThuoc.YeuCauTiepNhan.MaYeuCauTiepNhan)))
                                            && lstMaTiepTiepNhanChuaThuTien.Contains(x.DonThuocThanhToan.YeuCauTiepNhan.MaYeuCauTiepNhan))
                                .Select(item => new ThongTinDichVuChuaThuTienVo()
                                {
                                    YeucauTiepNhanId = item.DonThuocThanhToan.YeuCauTiepNhanId.GetValueOrDefault(),
                                    MaYeucauTiepNhan = item.DonThuocThanhToan.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                    TenDichVu = item.Ten,
                                    SoLuong = item.SoLuong,
                                    GiaBan = item.DonGiaBan,
                                    SoTienMienGiamTheoDichVu = item.SoTienMienGiam,
                                    DuocHuongBHYT = item.BaoHiemChiTra != null && item.BaoHiemChiTra == true,
                                    MucHuongBaoHiem = item.MucHuongBaoHiem.GetValueOrDefault(),
                                    DonGiaBHYT = item.DonGiaBaoHiem,
                                    TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                    LaTrongGoi = false
                                }).ToList();
                    if (donThuocs.Any())
                    {
                        lstTemp.AddRange(donThuocs);
                    }
                    #endregion

                    #region // xử lý check chi phí BHYT dịch vụ giường
                    var chiPhiGiuongs = lstTemp.Where(x => x.YeuCauDichVuGiuongChiPhiBenhVienId != null).ToList();
                    if (chiPhiGiuongs.Any())
                    {
                        var lstChiPhiGiuongBenhVienId = chiPhiGiuongs
                            .Where(x => x.YeuCauDichVuGiuongChiPhiBenhVienId != null)
                            .Select(x => x.YeuCauDichVuGiuongChiPhiBenhVienId.Value).Distinct().ToList();

                        var chiPhiGiuongBHYTs = await _yeuCauDichVuGiuongBenhVienChiPhiBhytRepository.TableNoTracking
                            .Where(x => lstChiPhiGiuongBenhVienId.Contains(x.Id))
                            .Select(item => new ThongTinDichVuChuaThuTienVo()
                            {
                                YeuCauDichVuGiuongChiPhiBenhVienId = item.Id,
                                DuocHuongBHYT = item.BaoHiemChiTra != null && item.BaoHiemChiTra == true,
                                MucHuongBaoHiem = item.MucHuongBaoHiem.GetValueOrDefault(),
                                DonGiaBHYT = item.DonGiaBaoHiem,
                                TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan.GetValueOrDefault()
                            })
                            .ToListAsync();

                        foreach (var chiPhiGiuongBenhVien in chiPhiGiuongs)
                        {
                            var chiPhiBHYT = chiPhiGiuongBHYTs.FirstOrDefault(x => x.YeuCauDichVuGiuongChiPhiBenhVienId == chiPhiGiuongBenhVien.YeuCauDichVuGiuongChiPhiBenhVienId);
                            if (chiPhiBHYT != null)
                            {
                                chiPhiGiuongBenhVien.DuocHuongBHYT = chiPhiBHYT.DuocHuongBHYT;
                                chiPhiGiuongBenhVien.MucHuongBaoHiem = chiPhiBHYT.MucHuongBaoHiem;
                                chiPhiGiuongBenhVien.DonGiaBHYT = chiPhiBHYT.DonGiaBHYT;
                                chiPhiGiuongBenhVien.TiLeBaoHiemThanhToan = chiPhiBHYT.TiLeBaoHiemThanhToan;
                            }
                        }
                    }

                    #endregion

                    var lstTiepNhanBoSungTheoPhanLoai = new List<ThongTinTiepNhanChuaThuTienCoHinhThucDenVo>();
                    foreach (var lanTiepNhan in lstTiepNhanTheoNoiGioiThieu)
                    {
                        //var chiPhiTheoLanTiepNhan = chiPhiKhams.Where(x => x.YeucauTiepNhanId == lanTiepNhan.YeucauTiepNhanId)
                        //    .Union(chiPhiKyThuats.Where(x => x.YeucauTiepNhanId == lanTiepNhan.YeucauTiepNhanId))
                        //    .Union(chiPhiTruyenMaus.Where(x => x.YeucauTiepNhanId == lanTiepNhan.YeucauTiepNhanId))
                        //    .Union(chiPhiGiuongs.Where(x => x.YeucauTiepNhanId == lanTiepNhan.YeucauTiepNhanId))
                        //    .Union(chiPhiDuocPhams.Where(x => x.YeucauTiepNhanId == lanTiepNhan.YeucauTiepNhanId))
                        //    .Union(chiPhiVatTus.Where(x => x.YeucauTiepNhanId == lanTiepNhan.YeucauTiepNhanId))
                        //    .Union(chiPhiDonThuocBHYts.Where(x => x.YeucauTiepNhanId == lanTiepNhan.YeucauTiepNhanId))
                        //    .ToList();

                        var chiPhiTheoLanTiepNhan = lstTemp.Where(x => x.YeucauTiepNhanId == lanTiepNhan.YeucauTiepNhanId)
                            .ToList();

                        if (lanTiepNhan.LaTiepNhanNoiTru)
                        {
                            var chiPhiNoiTruNgoaiGoi = chiPhiTheoLanTiepNhan.Where(x => !x.LaTrongGoi).Sum(x => x.ThanhToan);
                            var chiPhiNoiTruTrongGoi = chiPhiTheoLanTiepNhan.Where(x => x.LaTrongGoi).Sum(x => x.ThanhToan);

                            if (chiPhiNoiTruNgoaiGoi > 0)
                            {
                                lanTiepNhan.TongTienChuaThanhToan = chiPhiNoiTruNgoaiGoi;
                            }

                            if (chiPhiNoiTruTrongGoi > 0)
                            {
                                var lanTiepNhanTrongGoi = lanTiepNhan.Clone();
                                lanTiepNhanTrongGoi.TrongGoi = true;
                                lanTiepNhanTrongGoi.TongTienChuaThanhToan = chiPhiNoiTruTrongGoi;
                                lstTiepNhanBoSungTheoPhanLoai.Add(lanTiepNhanTrongGoi);
                            }
                        }
                        else
                        {
                            var chiPhiNgoaiTruNgoaiGoi = chiPhiTheoLanTiepNhan.Where(x => !x.LaTrongGoi).Sum(x => x.ThanhToan);
                            var chiPhiNgoaiTruTrongGoi = chiPhiTheoLanTiepNhan.Where(x => x.LaTrongGoi).Sum(x => x.ThanhToan);

                            if (chiPhiNgoaiTruNgoaiGoi > 0)
                            {
                                lanTiepNhan.TongTienChuaThanhToan = chiPhiNgoaiTruNgoaiGoi;
                            }

                            if (chiPhiNgoaiTruTrongGoi > 0)
                            {
                                var lanTiepNhanTrongGoi = lanTiepNhan.Clone();
                                lanTiepNhanTrongGoi.TrongGoi = true;
                                lanTiepNhanTrongGoi.TongTienChuaThanhToan = chiPhiNgoaiTruTrongGoi;
                                lstTiepNhanBoSungTheoPhanLoai.Add(lanTiepNhanTrongGoi);
                            }
                        }
                    }
                    lstTiepNhanTheoNoiGioiThieu.AddRange(lstTiepNhanBoSungTheoPhanLoai);

                    lstTiepNhanTheoNoiGioiThieu = lstTiepNhanTheoNoiGioiThieu
                        .Where(x => x.TongTienChuaThanhToan > 0).ToList();

                    return new GridDataSource { TotalRowCount = lstTiepNhanTheoNoiGioiThieu.Count };
                }

                return new GridDataSource { TotalRowCount = 0 };
            }
            return new GridDataSource { TotalRowCount = 0 };
        }

        public virtual byte[] ExportThongKeCacDichVuChuaLayLenBienLaiThuTien(GridDataSource gridDataSource, QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new ThongKeCacDichVuChuaLayLenBienLaiThuTienQueryInfoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<ThongKeCacDichVuChuaLayLenBienLaiThuTienQueryInfoVo>(queryInfo.AdditionalSearchString);
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.FromDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.FromDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.FromDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (timKiemNangCaoObj.ToDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.ToDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.ToDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }

            var datas = (ICollection<ThongTinTiepNhanChuaThuTienCoHinhThucDenVo>)gridDataSource.Data;

            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<ThongTinTiepNhanChuaThuTienCoHinhThucDenVo>("STT", p => ind++)
            };

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("[KT] THỐNG KÊ CÁC DV CHƯA LẤY LÊN BIÊN LAI THU TIỀN");
                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 15;
                    worksheet.Column(3).Width = 15;
                    worksheet.Column(4).Width = 15;
                    worksheet.Column(5).Width = 25;
                    worksheet.Column(6).Width = 10;
                    worksheet.Column(7).Width = 25;
                    worksheet.Column(8).Width = 30;
                    worksheet.Column(9).Width = 15;
                    worksheet.Column(10).Width = 10;
                    worksheet.Column(11).Width = 30;


                    worksheet.DefaultColWidth = 7;
                    worksheet.Row(8).Height = 24;

                    using (var range = worksheet.Cells["A1:K1"])
                    {
                        range.Worksheet.Cells["A1:K1"].Merge = true;
                        range.Worksheet.Cells["A1:K1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:K1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:K1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:K1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:K1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:K1"].Style.Font.Bold = true;

                    }

                    using (var range = worksheet.Cells["A2:K2"])
                    {
                        range.Worksheet.Cells["A2:K2"].Merge = true;
                        range.Worksheet.Cells["A2:K2"].Value = "BẢNG THỐNG KÊ CÁC DỊCH VỤ CHƯA LẤY LÊN BIÊN LAI THU TIỀN";
                        range.Worksheet.Cells["A2:K2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A2:K2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A2:K2"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A2:K2"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A2:K2"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A3:K3"])
                    {
                        range.Worksheet.Cells["A3:K3"].Merge = true;
                        range.Worksheet.Cells["A3:K3"].Value = "Từ ngày: " + tuNgay?.FormatNgayGioTimKiemTrenBaoCao()
                                                          + " - đến ngày: " + denNgay?.FormatNgayGioTimKiemTrenBaoCao();

                        range.Worksheet.Cells["A3:K3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:K3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:K3"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A3:K3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:K3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A5:K5"])
                    {
                        range.Worksheet.Cells["A5:K5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A5:K5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A5:K5"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A5:K5"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A5:K5"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A5:K5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A5:A5"].Merge = true;
                        range.Worksheet.Cells["A5:A5"].Value = "STT";
                        range.Worksheet.Cells["A5:A5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A5:A5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["B5:B5"].Merge = true;
                        range.Worksheet.Cells["B5:B5"].Value = "Mã bệnh án";
                        range.Worksheet.Cells["B5:B5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["B5:B5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["C5:C5"].Merge = true;
                        range.Worksheet.Cells["C5:C5"].Value = "Mã NB";
                        range.Worksheet.Cells["C5:C5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["C5:C5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["D5:D5"].Merge = true;
                        range.Worksheet.Cells["D5:D5"].Value = "Mã tiếp nhận";
                        range.Worksheet.Cells["D5:D5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["D5:D5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["E5:E5"].Merge = true;
                        range.Worksheet.Cells["E5:E5"].Value = "Họ và tên";
                        range.Worksheet.Cells["E5:E5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["E5:E5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["F5:F5"].Merge = true;
                        range.Worksheet.Cells["F5:F5"].Value = "Năm sinh";
                        range.Worksheet.Cells["F5:F5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["F5:F5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["G5:G5"].Merge = true;
                        range.Worksheet.Cells["G5:G5"].Value = "Hình thức đến";
                        range.Worksheet.Cells["G5:G5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["G5:G5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["H5:H5"].Merge = true;
                        range.Worksheet.Cells["H5:H5"].Value = "Địa chỉ";
                        range.Worksheet.Cells["H5:H5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["H5:H5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["I5:I5"].Merge = true;
                        range.Worksheet.Cells["I5:I5"].Value = "Loại";
                        range.Worksheet.Cells["I5:I5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["I5:I5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["J5:J5"].Merge = true;
                        range.Worksheet.Cells["J5:J5"].Value = "Trong gói";
                        range.Worksheet.Cells["J5:J5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["J5:J5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["K5:K5"].Merge = true;
                        range.Worksheet.Cells["K5:K5"].Value = "Tổng số tiền đã thực hiện";
                        range.Worksheet.Cells["K5:K5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["K5:K5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                    }

                    var manager = new PropertyManager<ThongTinTiepNhanChuaThuTienCoHinhThucDenVo>(requestProperties);
                    int index = 6;

                    var stt = 1;
                    if (datas.Any())
                    {
                        foreach (var item in datas)
                        {
                            using (var range = worksheet.Cells["A" + index + ":K" + index])
                            {
                                range.Worksheet.Cells["A" + index + ":K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                range.Worksheet.Cells["A" + index + ":K" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                range.Worksheet.Cells["A" + index + ":K" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["A" + index + ":K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["A" + index].Value = stt;

                                worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["B" + index].Value = item.MaBenhAn;

                                worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["C" + index].Value = item.MaBN;

                                worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["D" + index].Value = item.MaYeucauTiepNhan;

                                worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["E" + index].Value = item.HoVaTen;

                                worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["F" + index].Value = item.NamSinh;

                                worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["G" + index].Value = item.HinhThucDenDisplay;

                                worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["H" + index].Value = item.DiaChi;

                                worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["I" + index].Value = item.Loai;

                                worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["J" + index].Value = item.TrongGoi ? "X" : string.Empty;

                                worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["K" + index].Value = item.TongTienChuaThanhToan;

                                index++;
                                stt++;
                            }
                        }


                        using (var range = worksheet.Cells["A" + index + ":K" + index])
                        {
                            range.Worksheet.Cells["A" + index + ":J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                            range.Worksheet.Cells["A" + index + ":J" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells["A" + index + ":J" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["A" + index + ":J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells["A" + index + ":J" + index].Merge = true;
                            range.Worksheet.Cells["A" + index + ":J" + index].Style.Font.Bold = true;
                            range.Worksheet.Cells["A" + index + ":J" + index].Value = "Tổng cộng";

                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["K" + index].Style.Font.Bold = true;
                            worksheet.Cells["K" + index].Value = datas.Sum(c => c.TongTienChuaThanhToan);
                        }
                    }


                    xlPackage.Save();
                }
                return stream.ToArray();

            }
        }
    }
}
