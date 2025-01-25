using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public async Task<List<LookupItemTemplateVo>> GetNguoiBenhTheoNoiGioiThieuAsync(DropDownListRequestModel queryInfo)
        {
            var noiGioiThieuId = CommonHelper.GetIdFromRequestDropDownList(queryInfo);
            var nguoiBenhs = await _benhNhanRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhans.Any(a => a.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                                                       && a.NoiGioiThieuId != null
                                                       && (noiGioiThieuId == 0 || a.NoiGioiThieuId == noiGioiThieuId)))
                .Select(item => new LookupItemTemplateVo()
                {
                    KeyId = item.Id,
                    Ma = item.MaBN,
                    Ten = item.HoTen,
                    DisplayName = item.HoTen
                })
                .ApplyLike(queryInfo.Query?.Trim(), x => x.Ten, x => x.Ma)
                .OrderBy(x => x.Ten)
                .Take(queryInfo.Take)
                .ToListAsync();
            return nguoiBenhs;
        }

        public async Task<GridDataSource> GetDataBaoCaoBangKeChiTietTheoNguoiBenhForGridAsync(QueryInfo queryInfo)
        {
            var lstChiPhi = new List<BaoCaoBangKeChiTietTheoNguoiBenhGridVo>();
            var timKiemNangCaoObj = new BaoCaoBangKeChiTietTheoNguoiBenhTimKiemVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoBangKeChiTietTheoNguoiBenhTimKiemVo>(queryInfo.AdditionalSearchString);
                timKiemNangCaoObj.MaYeuCauTiepNhan = timKiemNangCaoObj.MaYeuCauTiepNhan?.Trim().ToLower();
            }

            //if (timKiemNangCaoObj.NguoiBenhId != null)// && tuNgay != null && denNgay != null)
            if(!string.IsNullOrEmpty(timKiemNangCaoObj.MaYeuCauTiepNhan))
            {
                var tenDichVuCovid = "SARS-CoV";
                var tiepNhanTheoBenhNhan = _yeuCauTiepNhanRepository.TableNoTracking
                    .Where(x => x.BenhNhanId != null
                                && x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru
                                //&& x.ThoiDiemTiepNhan >= tuNgay
                                //&& x.ThoiDiemTiepNhan <= denNgay
                                //&& x.BenhNhanId == timKiemNangCaoObj.NguoiBenhId
                                && x.MaYeuCauTiepNhan.Trim().ToLower() == timKiemNangCaoObj.MaYeuCauTiepNhan
                                && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy)
                    .Select(x => new BaoCaoBangKeChiTietTheoNguoiBenhGridVo()
                    {
                        YeucauTiepNhanId = x.Id,
                        MaYeuCauTiepNhan = x.MaYeuCauTiepNhan,
                        TrangThaiYeuCauTiepNhan = x.TrangThaiYeuCauTiepNhan,
                        BenhNhanId = x.BenhNhanId.Value,
                        HoTen = x.HoTen,
                        MaBN = x.BenhNhan.MaBN,
                        NoiGioiThieuId = x.NoiGioiThieuId,
                        NoiGioiThieuDisplay = x.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.NoiGioiThieu.DonVi) ? $" - {x.NoiGioiThieu.DonVi}" : ""),
                        TenHinhThucDen = x.HinhThucDen.Ten,
                        LaGioiThieu = x.NoiGioiThieuId != null,
                        ThoiDiemTiepNhan = x.ThoiDiemTiepNhan
                    })
                    .FirstOrDefault();

                if (tiepNhanTheoBenhNhan != null)
                {
                    #region // dùng để check người giới thiệu, tạm thời chưa cần dùng đến
                    //if (tiepNhanTheoBenhNhan.NoiGioiThieuId == null)
                    //{
                    //    var lstTiepNhanTheoBenhNhan = _yeuCauTiepNhanRepository.TableNoTracking
                    //        .Where(x => x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru
                    //                    && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                    //                    && x.BenhNhanId == tiepNhanTheoBenhNhan.BenhNhanId
                    //                    && x.Id <= tiepNhanTheoBenhNhan.Id)
                    //        .Select(x => new BaoCaoBangKeChiTietTheoNguoiBenhGridVo()
                    //        {
                    //            YeucauTiepNhanId = x.Id,
                    //            MaYeuCauTiepNhan = x.MaYeuCauTiepNhan,
                    //            TrangThaiYeuCauTiepNhan = x.TrangThaiYeuCauTiepNhan,
                    //            BenhNhanId = x.BenhNhanId.Value,
                    //            HoTen = x.HoTen,
                    //            MaBN = x.BenhNhan.MaBN,
                    //            NoiGioiThieuId = x.NoiGioiThieuId,
                    //            NoiGioiThieuDisplay = x.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.NoiGioiThieu.DonVi) ? $" - {x.NoiGioiThieu.DonVi}" : ""),
                    //            TenHinhThucDen = x.HinhThucDen.Ten,
                    //            LaGioiThieu = x.NoiGioiThieuId != null,
                    //            ThoiDiemTiepNhan = x.ThoiDiemTiepNhan
                    //        })
                    //        .ToList();

                    //    var lanTiepNhanDauTienCoGioiThieu = lstTiepNhanTheoBenhNhan
                    //        .Where(x => x.NoiGioiThieuId != null)
                    //        .OrderBy(x => x.YeucauTiepNhanId).FirstOrDefault();
                    //    if (lanTiepNhanDauTienCoGioiThieu != null && lanTiepNhanDauTienCoGioiThieu.Id < tiepNhanTheoBenhNhan.Id)
                    //    {
                    //        var tiepNhanBenhNhans = lstTiepNhanTheoBenhNhan
                    //            .Where(x => x.YeucauTiepNhanId > lanTiepNhanDauTienCoGioiThieu.YeucauTiepNhanId)
                    //            .ToList();
                    //        var khongThemNguoiGioiThieu = false;
                    //        var nguoiGioiThieuHienTaiId = lanTiepNhanDauTienCoGioiThieu.NoiGioiThieuId;
                    //        var tenNguoiGioiThieuHienTai = lanTiepNhanDauTienCoGioiThieu.NoiGioiThieuDisplay;
                    //        foreach (var lanTiepNhan in tiepNhanBenhNhans)
                    //        {
                    //            if (lanTiepNhan.NoiGioiThieuId != null)
                    //            {
                    //                if ((timKiemNangCaoObj.NoiGioiThieuId != null && timKiemNangCaoObj.NoiGioiThieuId != 0 && lanTiepNhan.NoiGioiThieuId != timKiemNangCaoObj.NoiGioiThieuId)
                    //                    || ((timKiemNangCaoObj.NoiGioiThieuId == null || timKiemNangCaoObj.NoiGioiThieuId == 0) && lanTiepNhan.NoiGioiThieuId != nguoiGioiThieuHienTaiId))
                    //                {
                    //                    // trường hợp tìm kiếm theo người giới thiệu thì chỉ thêm người giới thiệu đang tìm kiếm thôi
                    //                    if ((timKiemNangCaoObj.NoiGioiThieuId != null && timKiemNangCaoObj.NoiGioiThieuId != 0 && lanTiepNhan.NoiGioiThieuId != timKiemNangCaoObj.NoiGioiThieuId))
                    //                    {
                    //                        khongThemNguoiGioiThieu = true;
                    //                    }
                    //                    else
                    //                    {
                    //                        khongThemNguoiGioiThieu = false;
                    //                    }
                    //                    nguoiGioiThieuHienTaiId = lanTiepNhan.NoiGioiThieuId;
                    //                    tenNguoiGioiThieuHienTai = lanTiepNhan.NoiGioiThieuDisplay;
                    //                }
                    //                else
                    //                {
                    //                    khongThemNguoiGioiThieu = false;
                    //                }
                    //            }

                    //            if (!khongThemNguoiGioiThieu && lanTiepNhan.NoiGioiThieuId == null && lanTiepNhan.Id == tiepNhanTheoBenhNhan.Id)
                    //            {
                    //                tiepNhanTheoBenhNhan.NoiGioiThieuId = nguoiGioiThieuHienTaiId;
                    //                tiepNhanTheoBenhNhan.NoiGioiThieuDisplay = tenNguoiGioiThieuHienTai;
                    //                break;
                    //            }
                    //        }
                    //    }
                    //}
                    #endregion

                    var lstMaTiepNhan = new List<string>(); //lstTiepNhanTheoNoiGioiThieu.Select(x => x.MaYeuCauTiepNhan).Distinct().ToList();
                    lstMaTiepNhan.Add(tiepNhanTheoBenhNhan.MaYeuCauTiepNhan);

                    // tham khảo từ GetDataBaoCaoChiTietMienPhiTronVienForGridAsync

                    #region Chi phí dv khám
                    var lstChiPhiDichVuKham =
                       _yeuCauKhamBenhRepository.TableNoTracking
                           .Where(x => x.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham
                                       && lstMaTiepNhan.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan))
                           .Select(item => new BaoCaoBangKeChiTietTheoNguoiBenhGridVo()
                           {
                               NoiDung = item.TenDichVu,
                               DonViTinh = "lần",
                               SoLuong = 1,
                               GiaNiemYet = item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia,
                               GiaUuDai = null,
                               NguoiBenhDaThanhToan = null,
                               CongNoConPhaiThanhToan = null,
                               NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKhamBenh,
                               DaThucHien = item.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham,
                               LaNoiTru = false,
                               SoTienMienGiam = item.SoTienMienGiam,
                               KhongTinhPhi = item.KhongTinhPhi != null && item.KhongTinhPhi == true
                           })
                           .GroupBy(x => new { x.NhomDichVu, x.NoiDung, x.GiaNiemYet, x.LaNoiTru, x.KhongTinhPhi })
                           .Select(item => new BaoCaoBangKeChiTietTheoNguoiBenhGridVo()
                           {
                               NoiDung = item.Key.NoiDung,
                               DonViTinh = "lần",
                               SoLuong = item.Sum(x => x.SoLuong),
                               GiaNiemYet = item.Key.GiaNiemYet,
                               GiaUuDai = null,
                               NguoiBenhDaThanhToan = null,
                               CongNoConPhaiThanhToan = null,
                               NhomDichVu = item.Key.NhomDichVu,
                               DaThucHien = item.First().DaThucHien,
                               LaNoiTru = item.Key.LaNoiTru,
                               SoTienMienGiam = item.Sum(x => x.SoTienMienGiam ?? 0),
                               KhongTinhPhi = item.Key.KhongTinhPhi
                           })
                           .ToList();
                    lstChiPhi.AddRange(lstChiPhiDichVuKham);
                    #endregion

                    #region Chi phí dv kỹ thuật
                    var lstChiPhiDichVuKyThuat =
                       _yeuCauDichVuKyThuatRepository.TableNoTracking
                           .Where(x => x.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien 
                                       && lstMaTiepNhan.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan))
                           .Select(item => new BaoCaoBangKeChiTietTheoNguoiBenhGridVo()
                           {
                               NoiDung = item.TenDichVu,
                               DonViTinh = "lần",
                               SoLuong = item.SoLan,
                               GiaNiemYet = item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia,
                               GiaUuDai = null,
                               NguoiBenhDaThanhToan = null,
                               CongNoConPhaiThanhToan = null,
                               NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKyThuat,
                               LoaiDichVuKyThuat = item.LoaiDichVuKyThuat,
                               DaThucHien = item.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien,
                               TenDichVuCovid = tenDichVuCovid,
                               LaNoiTru = item.NoiTruPhieuDieuTriId != null,
                               SoTienMienGiam = item.SoTienMienGiam,
                               KhongTinhPhi = item.KhongTinhPhi != null && item.KhongTinhPhi == true
                           })
                           .GroupBy(x => new { x.NhomDichVu, x.LoaiDichVuKyThuat, x.NoiDung, x.GiaNiemYet, x.LaNoiTru, x.KhongTinhPhi })
                           .Select(item => new BaoCaoBangKeChiTietTheoNguoiBenhGridVo()
                           {
                               NoiDung = item.Key.NoiDung,
                               DonViTinh = "lần",
                               SoLuong = item.Sum(x => x.SoLuong),
                               GiaNiemYet = item.Key.GiaNiemYet,
                               GiaUuDai = null,
                               NguoiBenhDaThanhToan = null,
                               CongNoConPhaiThanhToan = null,
                               NhomDichVu = item.Key.NhomDichVu,
                               LoaiDichVuKyThuat = item.Key.LoaiDichVuKyThuat,
                               DaThucHien = item.First().DaThucHien,
                               TenDichVuCovid = tenDichVuCovid,
                               LaNoiTru = item.Key.LaNoiTru,
                               SoTienMienGiam = item.Sum(x => x.SoTienMienGiam ?? 0),
                               KhongTinhPhi = item.Key.KhongTinhPhi
                           })
                           .ToList();
                    lstChiPhi.AddRange(lstChiPhiDichVuKyThuat);
                    #endregion

                    #region Chi phí dv giường
                    var lstChiPhiDichVuGiuong =
                        _yeuCauDichVuGiuongBenhVienChiPhiBenhVienRepository.TableNoTracking
                           .Where(x => lstMaTiepNhan.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan))
                           .Select(item => new BaoCaoBangKeChiTietTheoNguoiBenhGridVo()
                           {
                               NoiDung = item.Ten,
                               DonViTinh = "ngày",
                               SoLuong = item.SoLuong,
                               GiaNiemYet = item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia,
                               GiaUuDai = null,
                               NguoiBenhDaThanhToan = null,
                               CongNoConPhaiThanhToan = null,
                               NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuGiuongBenh,
                               DaThucHien = true,
                               SoTienMienGiam = item.SoTienMienGiam
                           })
                           .GroupBy(x => new { x.NhomDichVu, x.NoiDung, x.GiaNiemYet })
                           .Select(item => new BaoCaoBangKeChiTietTheoNguoiBenhGridVo()
                           {
                               NoiDung = item.Key.NoiDung,
                               DonViTinh = "ngày",
                               SoLuong = item.Sum(x => x.SoLuong),
                               GiaNiemYet = item.Key.GiaNiemYet,
                               GiaUuDai = null,
                               NguoiBenhDaThanhToan = null,
                               CongNoConPhaiThanhToan = null,
                               NhomDichVu = item.Key.NhomDichVu,
                               DaThucHien = item.First().DaThucHien,
                               SoTienMienGiam = item.Sum(x => x.SoTienMienGiam ?? 0)
                           })
                           .ToList();
                    lstChiPhi.AddRange(lstChiPhiDichVuGiuong);
                    #endregion

                    #region Chi phí dược phẩm
                    var lstChiPhiDuocPham =
                        _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                            .Where(x => x.TrangThai == Enums.EnumYeuCauDuocPhamBenhVien.DaThucHien 
                                        && lstMaTiepNhan.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan))
                            .Select(item => new BaoCaoBangKeChiTietTheoNguoiBenhGridVo()
                            {
                                NoiDung = item.Ten,
                                DonViTinh = item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                                SoLuong = item.SoLuong,
                                GiaNiemYet = item.DonGiaBan,
                                GiaUuDai = null,
                                NguoiBenhDaThanhToan = null,
                                CongNoConPhaiThanhToan = null,
                                NhomDichVu = Enums.EnumNhomGoiDichVu.DuocPham,
                                DaThucHien = true,
                                ThanhTienThuocVatTu = item.GiaBan,
                                SoTienMienGiam = item.SoTienMienGiam,
                                KhongTinhPhi = item.KhongTinhPhi != null && item.KhongTinhPhi == true,

                                //BVHD-3767
                                XuatKhoChiTietId = item.XuatKhoDuocPhamChiTietId
                            })
                            .GroupBy(x => new { x.NhomDichVu, x.DonViTinh, x.NoiDung, x.GiaNiemYet, x.KhongTinhPhi })
                            .Select(item => new BaoCaoBangKeChiTietTheoNguoiBenhGridVo()
                            {
                                NoiDung = item.Key.NoiDung,
                                DonViTinh = item.Key.DonViTinh,
                                SoLuong = item.Sum(x => x.SoLuong),
                                GiaNiemYet = item.Key.GiaNiemYet,
                                GiaUuDai = null,
                                NguoiBenhDaThanhToan = null,
                                CongNoConPhaiThanhToan = null,
                                NhomDichVu = item.Key.NhomDichVu,
                                DaThucHien = item.First().DaThucHien,
                                ThanhTienThuocVatTu = item.Sum(x => x.ThanhTien),
                                SoTienMienGiam = item.Sum(x => x.SoTienMienGiam ?? 0),
                                KhongTinhPhi = item.Key.KhongTinhPhi,

                                //BVHD-3767
                                ChiTietGiaTonKhos = item
                                    .Where(a => a.XuatKhoChiTietId != null)
                                    .Select(a => new ChiTietGiaTonKhoThuocVatTuVo()
                                    {
                                        XuatKhoChiTietId = a.XuatKhoChiTietId,
                                        SoLuong = a.SoLuong,
                                        LaDuocPham = true
                                    }).ToList()
                            })
                            .ToList();

                    //BVHD-3767
                    #region Xử lý giá tồn kho
                    var lstXuatDuocPhamChiTietId = lstChiPhiDuocPham
                        .SelectMany(x => x.ChiTietGiaTonKhos)
                        .Where(x => x.XuatKhoChiTietId != null)
                        .Select(x => x.XuatKhoChiTietId).Distinct().ToList();
                    if (lstXuatDuocPhamChiTietId.Any())
                    {
                        var lstNhapDuocPhamChiTiet = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking
                            .Where(x => lstXuatDuocPhamChiTietId.Contains(x.XuatKhoDuocPhamChiTietId))
                            .Select(x => new ChiTietGiaTonKhoThuocVatTuVo()
                            {
                                XuatKhoChiTietId = x.XuatKhoDuocPhamChiTietId,
                                NhapKhoChiTietId = x.NhapKhoDuocPhamChiTietId,
                                DonGiaTonKho = x.NhapKhoDuocPhamChiTiet.DonGiaTonKho,
                                LaDuocPham = true
                            }).ToList();

                        foreach (var chiPhi in lstChiPhiDuocPham)
                        {
                            if (chiPhi.ChiTietGiaTonKhos.Any())
                            {
                                foreach (var chiTietTonKho in chiPhi.ChiTietGiaTonKhos)
                                {
                                    var chiTietNhap = lstNhapDuocPhamChiTiet
                                        .FirstOrDefault(x => x.XuatKhoChiTietId == chiTietTonKho.XuatKhoChiTietId);
                                    if (chiTietNhap != null)
                                    {
                                        chiTietTonKho.DonGiaTonKho = chiTietNhap.DonGiaTonKho;
                                    }
                                }
                            }
                        }
                    }


                    #endregion

                    lstChiPhi.AddRange(lstChiPhiDuocPham);
                    #endregion

                    #region Chi phí vật tư
                    var lstChiPhiVatTu =
                        _yeuCauVatTuBenhVienRepository.TableNoTracking
                            .Where(x => x.TrangThai == Enums.EnumYeuCauVatTuBenhVien.DaThucHien
                                        && lstMaTiepNhan.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan))
                            .Select(item => new BaoCaoBangKeChiTietTheoNguoiBenhGridVo()
                            {
                                NoiDung = item.Ten,
                                DonViTinh = item.VatTuBenhVien.VatTus.DonViTinh,
                                SoLuong = item.SoLuong,
                                GiaNiemYet = item.DonGiaBan,
                                GiaUuDai = null,
                                NguoiBenhDaThanhToan = null,
                                CongNoConPhaiThanhToan = null,
                                NhomDichVu = Enums.EnumNhomGoiDichVu.VatTuTieuHao,
                                DaThucHien = true,
                                ThanhTienThuocVatTu = item.GiaBan,
                                SoTienMienGiam = item.SoTienMienGiam,
                                KhongTinhPhi = item.KhongTinhPhi != null && item.KhongTinhPhi == true,

                                //BVHD-3767
                                XuatKhoChiTietId = item.XuatKhoVatTuChiTietId
                            })
                            .GroupBy(x => new { x.NhomDichVu, x.DonViTinh, x.NoiDung, x.GiaNiemYet, x.KhongTinhPhi })
                            .Select(item => new BaoCaoBangKeChiTietTheoNguoiBenhGridVo()
                            {
                                NoiDung = item.Key.NoiDung,
                                DonViTinh = item.Key.DonViTinh,
                                SoLuong = item.Sum(x => x.SoLuong),
                                GiaNiemYet = item.Key.GiaNiemYet,
                                GiaUuDai = null,
                                NguoiBenhDaThanhToan = null,
                                CongNoConPhaiThanhToan = null,
                                NhomDichVu = item.Key.NhomDichVu,
                                DaThucHien = item.First().DaThucHien,
                                ThanhTienThuocVatTu = item.Sum(x => x.ThanhTien),
                                SoTienMienGiam = item.Sum(x => x.SoTienMienGiam ?? 0),
                                KhongTinhPhi = item.Key.KhongTinhPhi,

                                //BVHD-3767
                                ChiTietGiaTonKhos = item
                                    .Where(a => a.XuatKhoChiTietId != null)
                                    .Select(a => new ChiTietGiaTonKhoThuocVatTuVo()
                                    {
                                        XuatKhoChiTietId = a.XuatKhoChiTietId,
                                        SoLuong = a.SoLuong,
                                        LaDuocPham = false
                                    }).ToList()
                            })
                            .ToList();

                    //BVHD-3767
                    #region Xử lý giá tồn kho
                    var lstXuatVatTuChiTietId = lstChiPhiVatTu
                        .SelectMany(x => x.ChiTietGiaTonKhos)
                        .Where(x => x.XuatKhoChiTietId != null)
                        .Select(x => x.XuatKhoChiTietId).Distinct().ToList();
                    if (lstXuatVatTuChiTietId.Any())
                    {
                        var lstNhapVatTuChiTiet = _xuatKhoVatTuChiTietViTriRepository.TableNoTracking
                            .Where(x => lstXuatVatTuChiTietId.Contains(x.XuatKhoVatTuChiTietId))
                            .Select(x => new ChiTietGiaTonKhoThuocVatTuVo()
                            {
                                XuatKhoChiTietId = x.XuatKhoVatTuChiTietId,
                                NhapKhoChiTietId = x.XuatKhoVatTuChiTietId,
                                DonGiaTonKho = x.NhapKhoVatTuChiTiet.DonGiaTonKho,
                                LaDuocPham = false
                            }).ToList();

                        foreach (var chiPhi in lstChiPhiVatTu)
                        {
                            if (chiPhi.ChiTietGiaTonKhos.Any())
                            {
                                foreach (var chiTietTonKho in chiPhi.ChiTietGiaTonKhos)
                                {
                                    var chiTietNhap = lstNhapVatTuChiTiet
                                        .FirstOrDefault(x => x.XuatKhoChiTietId == chiTietTonKho.XuatKhoChiTietId);
                                    if (chiTietNhap != null)
                                    {
                                        chiTietTonKho.DonGiaTonKho = chiTietNhap.DonGiaTonKho;
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    lstChiPhi.AddRange(lstChiPhiVatTu);
                    #endregion

                    var stt = 1;
                    Enums.BangKeChiPhiTheoNhomDichVu nhomHienTai = Enums.BangKeChiPhiTheoNhomDichVu.CanLamSangNgoaiTru;
                    lstChiPhi = lstChiPhi.OrderBy(x => x.Nhom).ThenBy(x => x.LaNoiTru != true).ToList();
                    foreach (var chiPhi in lstChiPhi)
                    {
                        if (nhomHienTai != chiPhi.Nhom)
                        {
                            stt = 1;
                            nhomHienTai = chiPhi.Nhom;
                        }
                        chiPhi.STT = stt.ToString();
                        stt++;
                    }
                    lstChiPhi = lstChiPhi.Skip(queryInfo.Skip).Take(queryInfo.Take).ToList();
                }
            }

            return new GridDataSource
            {
                Data = lstChiPhi.ToArray(),
                TotalRowCount = lstChiPhi.Count()
            };
        }

        public async Task<GridDataSource> GetTotalPageBaoCaoBangKeChiTietTheoNguoiBenhForGridAsync(QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new BaoCaoBangKeChiTietTheoNguoiBenhTimKiemVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoBangKeChiTietTheoNguoiBenhTimKiemVo>(queryInfo.AdditionalSearchString);
                timKiemNangCaoObj.MaYeuCauTiepNhan = timKiemNangCaoObj.MaYeuCauTiepNhan?.Trim().ToLower();
            }

            //if (timKiemNangCaoObj.NguoiBenhId != null)
            if (!string.IsNullOrEmpty(timKiemNangCaoObj.MaYeuCauTiepNhan))
            {
                var tenDichVuCovid = "SARS-CoV";
                var tiepNhanTheoBenhNhan = _yeuCauTiepNhanRepository.TableNoTracking
                    .Where(x => x.BenhNhanId != null
                                && x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru
                                && x.MaYeuCauTiepNhan.Trim().ToLower() == timKiemNangCaoObj.MaYeuCauTiepNhan
                                && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy)
                    .Select(x => new BaoCaoBangKeChiTietTheoNguoiBenhGridVo()
                    {
                        YeucauTiepNhanId = x.Id,
                        MaYeuCauTiepNhan = x.MaYeuCauTiepNhan,
                        TrangThaiYeuCauTiepNhan = x.TrangThaiYeuCauTiepNhan,
                        BenhNhanId = x.BenhNhanId.Value,
                        HoTen = x.HoTen,
                        MaBN = x.BenhNhan.MaBN,
                        NoiGioiThieuId = x.NoiGioiThieuId,
                        NoiGioiThieuDisplay = x.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.NoiGioiThieu.DonVi) ? $" - {x.NoiGioiThieu.DonVi}" : ""),
                        TenHinhThucDen = x.HinhThucDen.Ten,
                        LaGioiThieu = x.NoiGioiThieuId != null,
                        ThoiDiemTiepNhan = x.ThoiDiemTiepNhan
                    })
                    .FirstOrDefault();
                if (tiepNhanTheoBenhNhan != null)
                {
                    #region // dùng để check người giới thiệu, tạm thời chưa cần dùng đến
                    //if (tiepNhanTheoBenhNhan.NoiGioiThieuId == null)
                    //{
                    //    var lstTiepNhanTheoBenhNhan = _yeuCauTiepNhanRepository.TableNoTracking
                    //        .Where(x => x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru
                    //                    && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                    //                    && x.BenhNhanId == tiepNhanTheoBenhNhan.BenhNhanId
                    //                    && x.Id <= tiepNhanTheoBenhNhan.Id)
                    //        .Select(x => new BaoCaoBangKeChiTietTheoNguoiBenhGridVo()
                    //        {
                    //            YeucauTiepNhanId = x.Id,
                    //            MaYeuCauTiepNhan = x.MaYeuCauTiepNhan,
                    //            TrangThaiYeuCauTiepNhan = x.TrangThaiYeuCauTiepNhan,
                    //            BenhNhanId = x.BenhNhanId.Value,
                    //            HoTen = x.HoTen,
                    //            MaBN = x.BenhNhan.MaBN,
                    //            NoiGioiThieuId = x.NoiGioiThieuId,
                    //            NoiGioiThieuDisplay = x.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.NoiGioiThieu.DonVi) ? $" - {x.NoiGioiThieu.DonVi}" : ""),
                    //            TenHinhThucDen = x.HinhThucDen.Ten,
                    //            LaGioiThieu = x.NoiGioiThieuId != null,
                    //            ThoiDiemTiepNhan = x.ThoiDiemTiepNhan
                    //        })
                    //        .ToList();

                    //    var lanTiepNhanDauTienCoGioiThieu = lstTiepNhanTheoBenhNhan
                    //        .Where(x => x.NoiGioiThieuId != null)
                    //        .OrderBy(x => x.YeucauTiepNhanId).FirstOrDefault();
                    //    if (lanTiepNhanDauTienCoGioiThieu != null && lanTiepNhanDauTienCoGioiThieu.Id < tiepNhanTheoBenhNhan.Id)
                    //    {
                    //        var tiepNhanBenhNhans = lstTiepNhanTheoBenhNhan
                    //            .Where(x => x.YeucauTiepNhanId > lanTiepNhanDauTienCoGioiThieu.YeucauTiepNhanId)
                    //            .ToList();
                    //        var khongThemNguoiGioiThieu = false;
                    //        var nguoiGioiThieuHienTaiId = lanTiepNhanDauTienCoGioiThieu.NoiGioiThieuId;
                    //        var tenNguoiGioiThieuHienTai = lanTiepNhanDauTienCoGioiThieu.NoiGioiThieuDisplay;
                    //        foreach (var lanTiepNhan in tiepNhanBenhNhans)
                    //        {
                    //            if (lanTiepNhan.NoiGioiThieuId != null)
                    //            {
                    //                if ((timKiemNangCaoObj.NoiGioiThieuId != null && timKiemNangCaoObj.NoiGioiThieuId != 0 && lanTiepNhan.NoiGioiThieuId != timKiemNangCaoObj.NoiGioiThieuId)
                    //                    || ((timKiemNangCaoObj.NoiGioiThieuId == null || timKiemNangCaoObj.NoiGioiThieuId == 0) && lanTiepNhan.NoiGioiThieuId != nguoiGioiThieuHienTaiId))
                    //                {
                    //                    // trường hợp tìm kiếm theo người giới thiệu thì chỉ thêm người giới thiệu đang tìm kiếm thôi
                    //                    if ((timKiemNangCaoObj.NoiGioiThieuId != null && timKiemNangCaoObj.NoiGioiThieuId != 0 && lanTiepNhan.NoiGioiThieuId != timKiemNangCaoObj.NoiGioiThieuId))
                    //                    {
                    //                        khongThemNguoiGioiThieu = true;
                    //                    }
                    //                    else
                    //                    {
                    //                        khongThemNguoiGioiThieu = false;
                    //                    }
                    //                    nguoiGioiThieuHienTaiId = lanTiepNhan.NoiGioiThieuId;
                    //                    tenNguoiGioiThieuHienTai = lanTiepNhan.NoiGioiThieuDisplay;
                    //                }
                    //                else
                    //                {
                    //                    khongThemNguoiGioiThieu = false;
                    //                }
                    //            }

                    //            if (!khongThemNguoiGioiThieu && lanTiepNhan.NoiGioiThieuId == null && lanTiepNhan.Id == tiepNhanTheoBenhNhan.Id)
                    //            {
                    //                tiepNhanTheoBenhNhan.NoiGioiThieuId = nguoiGioiThieuHienTaiId;
                    //                tiepNhanTheoBenhNhan.NoiGioiThieuDisplay = tenNguoiGioiThieuHienTai;
                    //                break;
                    //            }
                    //        }
                    //    }
                    //}
                    #endregion

                    var lstMaTiepNhan = new List<string>();
                    lstMaTiepNhan.Add(tiepNhanTheoBenhNhan.MaYeuCauTiepNhan);

                    // tham khảo từ GetDataBaoCaoChiTietMienPhiTronVienForGridAsync
                    #region Chi phí dv khám
                    var lstChiPhiDichVuKham =
                       _yeuCauKhamBenhRepository.TableNoTracking
                           .Where(x => x.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham
                                       && lstMaTiepNhan.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan))
                           .Select(item => new BaoCaoBangKeChiTietTheoNguoiBenhGridVo()
                           {
                               NoiDung = item.TenDichVu,
                               DonViTinh = "lần",
                               SoLuong = 1,
                               GiaNiemYet = item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia,
                               GiaUuDai = null,
                               NguoiBenhDaThanhToan = null,
                               CongNoConPhaiThanhToan = null,
                               NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKhamBenh,
                               DaThucHien = item.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham,
                               LaNoiTru = false
                           })
                           .GroupBy(x => new { x.NhomDichVu, x.NoiDung, x.GiaNiemYet, x.LaNoiTru })
                           .Select(item => new BaoCaoBangKeChiTietTheoNguoiBenhGridVo()
                           {
                               NoiDung = item.Key.NoiDung,
                               DonViTinh = "lần",
                               SoLuong = item.Sum(x => x.SoLuong),
                               GiaNiemYet = item.Key.GiaNiemYet,
                               GiaUuDai = null,
                               NguoiBenhDaThanhToan = null,
                               CongNoConPhaiThanhToan = null,
                               NhomDichVu = item.Key.NhomDichVu,
                               DaThucHien = item.First().DaThucHien,
                               LaNoiTru = item.Key.LaNoiTru
                           })
                           .ToList();
                    #endregion

                    #region Chi phí dv kỹ thuật
                    var lstChiPhiDichVuKyThuat =
                       _yeuCauDichVuKyThuatRepository.TableNoTracking
                           .Where(x => x.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                                       && lstMaTiepNhan.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan))
                           .Select(item => new BaoCaoBangKeChiTietTheoNguoiBenhGridVo()
                           {
                               NoiDung = item.TenDichVu,
                               DonViTinh = "lần",
                               SoLuong = item.SoLan,
                               GiaNiemYet = item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia,
                               GiaUuDai = null,
                               NguoiBenhDaThanhToan = null,
                               CongNoConPhaiThanhToan = null,
                               NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKyThuat,
                               LoaiDichVuKyThuat = item.LoaiDichVuKyThuat,
                               DaThucHien = item.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien,
                               TenDichVuCovid = tenDichVuCovid,
                               LaNoiTru = item.NoiTruPhieuDieuTriId != null
                           })
                           .GroupBy(x => new { x.NhomDichVu, x.LoaiDichVuKyThuat, x.NoiDung, x.GiaNiemYet, x.LaNoiTru })
                           .Select(item => new BaoCaoBangKeChiTietTheoNguoiBenhGridVo()
                           {
                               NoiDung = item.Key.NoiDung,
                               DonViTinh = "lần",
                               SoLuong = item.Sum(x => x.SoLuong),
                               GiaNiemYet = item.Key.GiaNiemYet,
                               GiaUuDai = null,
                               NguoiBenhDaThanhToan = null,
                               CongNoConPhaiThanhToan = null,
                               NhomDichVu = item.Key.NhomDichVu,
                               LoaiDichVuKyThuat = item.Key.LoaiDichVuKyThuat,
                               DaThucHien = item.First().DaThucHien,
                               TenDichVuCovid = tenDichVuCovid,
                               LaNoiTru = item.Key.LaNoiTru
                           })
                           .ToList();
                    #endregion

                    #region Chi phí dv giường
                    var lstChiPhiDichVuGiuong =
                        _yeuCauDichVuGiuongBenhVienChiPhiBenhVienRepository.TableNoTracking
                           .Where(x => lstMaTiepNhan.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan))
                           .Select(item => new BaoCaoBangKeChiTietTheoNguoiBenhGridVo()
                           {
                               NoiDung = item.Ten,
                               DonViTinh = "ngày",
                               SoLuong = item.SoLuong,
                               GiaNiemYet = item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia,
                               GiaUuDai = null,
                               NguoiBenhDaThanhToan = null,
                               CongNoConPhaiThanhToan = null,
                               NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuGiuongBenh,
                               DaThucHien = true
                           })
                           .GroupBy(x => new { x.NhomDichVu, x.NoiDung, x.GiaNiemYet })
                           .Select(item => new BaoCaoBangKeChiTietTheoNguoiBenhGridVo()
                           {
                               NoiDung = item.Key.NoiDung,
                               DonViTinh = "ngày",
                               SoLuong = item.Sum(x => x.SoLuong),
                               GiaNiemYet = item.Key.GiaNiemYet,
                               GiaUuDai = null,
                               NguoiBenhDaThanhToan = null,
                               CongNoConPhaiThanhToan = null,
                               NhomDichVu = item.Key.NhomDichVu,
                               DaThucHien = item.First().DaThucHien
                           })
                           .ToList();
                    #endregion

                    #region Chi phí dược phẩm
                    var lstChiPhiDuocPham =
                        _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                            .Where(x => x.TrangThai == Enums.EnumYeuCauDuocPhamBenhVien.DaThucHien
                                        && lstMaTiepNhan.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan))
                            .Select(item => new BaoCaoBangKeChiTietTheoNguoiBenhGridVo()
                            {
                                NoiDung = item.Ten,
                                DonViTinh = item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                                SoLuong = item.SoLuong,
                                GiaNiemYet = item.DonGiaBan,
                                GiaUuDai = null,
                                NguoiBenhDaThanhToan = null,
                                CongNoConPhaiThanhToan = null,
                                NhomDichVu = Enums.EnumNhomGoiDichVu.DuocPham,
                                DaThucHien = true,
                                ThanhTienThuocVatTu = item.GiaBan
                            })
                            .GroupBy(x => new { x.NhomDichVu, x.DonViTinh, x.NoiDung, x.GiaNiemYet })
                            .Select(item => new BaoCaoBangKeChiTietTheoNguoiBenhGridVo()
                            {
                                NoiDung = item.Key.NoiDung,
                                DonViTinh = item.Key.DonViTinh,
                                SoLuong = item.Sum(x => x.SoLuong),
                                GiaNiemYet = item.Key.GiaNiemYet,
                                GiaUuDai = null,
                                NguoiBenhDaThanhToan = null,
                                CongNoConPhaiThanhToan = null,
                                NhomDichVu = item.Key.NhomDichVu,
                                DaThucHien = item.First().DaThucHien,
                                ThanhTienThuocVatTu = item.Sum(x => x.ThanhTien)
                            })
                            .ToList();
                    #endregion

                    #region Chi phí vật tư
                    var lstChiPhiVatTu =
                        _yeuCauVatTuBenhVienRepository.TableNoTracking
                            .Where(x => x.TrangThai == Enums.EnumYeuCauVatTuBenhVien.DaThucHien
                                        && lstMaTiepNhan.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan))
                            .Select(item => new BaoCaoBangKeChiTietTheoNguoiBenhGridVo()
                            {
                                NoiDung = item.Ten,
                                DonViTinh = item.VatTuBenhVien.VatTus.DonViTinh,
                                SoLuong = item.SoLuong,
                                GiaNiemYet = item.DonGiaBan,
                                GiaUuDai = null,
                                NguoiBenhDaThanhToan = null,
                                CongNoConPhaiThanhToan = null,
                                NhomDichVu = Enums.EnumNhomGoiDichVu.VatTuTieuHao,
                                DaThucHien = true,
                                ThanhTienThuocVatTu = item.GiaBan
                            })
                            .GroupBy(x => new { x.NhomDichVu, x.DonViTinh, x.NoiDung, x.GiaNiemYet })
                            .Select(item => new BaoCaoBangKeChiTietTheoNguoiBenhGridVo()
                            {
                                NoiDung = item.Key.NoiDung,
                                DonViTinh = item.Key.DonViTinh,
                                SoLuong = item.Sum(x => x.SoLuong),
                                GiaNiemYet = item.Key.GiaNiemYet,
                                GiaUuDai = null,
                                NguoiBenhDaThanhToan = null,
                                CongNoConPhaiThanhToan = null,
                                NhomDichVu = item.Key.NhomDichVu,
                                DaThucHien = item.First().DaThucHien,
                                ThanhTienThuocVatTu = item.Sum(x => x.ThanhTien)
                            })
                            .ToList();
                    #endregion

                    var countTask = lstChiPhiDichVuKham.Count + lstChiPhiDichVuKyThuat.Count +
                                    lstChiPhiDichVuGiuong.Count + lstChiPhiDuocPham.Count + lstChiPhiVatTu.Count;
                    return new GridDataSource { TotalRowCount = countTask };
                }
                return new GridDataSource { TotalRowCount = 0 };
            }
            return new GridDataSource { TotalRowCount = 0 };
        }

        public virtual byte[] ExportBaoCaoBangKeChiTietTheoNguoiBenh(GridDataSource gridDataSource, QueryInfo query)
        {
            var timKiemNangCaoObj = new BaoCaoBangKeChiTietTheoNguoiBenhTimKiemVo();
            if (!string.IsNullOrEmpty(query.AdditionalSearchString) && query.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoBangKeChiTietTheoNguoiBenhTimKiemVo>(query.AdditionalSearchString);
                timKiemNangCaoObj.MaYeuCauTiepNhan = timKiemNangCaoObj.MaYeuCauTiepNhan?.Trim().ToLower();
            }

            var nguoiBenh = _yeuCauTiepNhanRepository.TableNoTracking.FirstOrDefault(x => x.MaYeuCauTiepNhan.Trim().ToLower() == timKiemNangCaoObj.MaYeuCauTiepNhan);
            var lstNhomChiPhi = EnumHelper.GetListEnum<Enums.BangKeChiPhiTheoNhomDichVu>()
                .Select(item => new LookupItemTemplateVo()
                {
                    DisplayName = item.GetDescription(),
                    Ma = ConvertNumberToStringRoman((int)item),
                    KeyId = Convert.ToInt32(item)
                }).ToList();

            var datas = (ICollection<BaoCaoBangKeChiTietTheoNguoiBenhGridVo>)gridDataSource.Data;
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BẢNG KÊ CHI TIẾT NGƯỜI BỆNH");

                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 40;
                    worksheet.Column(3).Width = 10;
                    worksheet.Column(4).Width = 10;
                    worksheet.Column(5).Width = 20;
                    worksheet.Column(6).Width = 20;
                    worksheet.Column(7).Width = 20;
                    worksheet.Column(8).Width = 20;
                    worksheet.Column(9).Width = 30;
                    worksheet.Column(10).Width = 30;
                    worksheet.DefaultColWidth = 25;

                    //SET title BV
                    using (var range = worksheet.Cells["A1:C1"])
                    {
                        range.Worksheet.Cells["A1:C1"].Merge = true;
                        range.Worksheet.Cells["A1:C1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:C1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:C1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:C1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:C1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:C1"].Style.Font.Bold = true;
                    }

                    // SET title head cho bảng excel
                    using (var range = worksheet.Cells["A3:I3"])
                    {
                        range.Worksheet.Cells["A3:I3"].Merge = true;
                        range.Worksheet.Cells["A3:I3"].Value = "BẢNG KÊ CHI TIẾT";
                        range.Worksheet.Cells["A3:I3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:I3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:I3"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A3:I3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:I3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A4:I4"])
                    {
                        range.Worksheet.Cells["A4:I4"].Merge = true;
                        range.Worksheet.Cells["A4:I4"].Value = "NGƯỜI BỆNH " + $"{nguoiBenh?.HoTen?.ToUpper()} - {nguoiBenh?.MaYeuCauTiepNhan}";
                        range.Worksheet.Cells["A4:I4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:I4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:I4"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A4:I4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:I4"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A7:J7"])
                    {
                        range.Worksheet.Cells["A7:J7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A7:J7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A7:J7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A7:J7"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A7:J7"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A7:J7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A7"].Value = "STT";

                        range.Worksheet.Cells["B7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B7"].Value = "Nội Dung";

                        range.Worksheet.Cells["C7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C7"].Value = "ĐVT";

                        range.Worksheet.Cells["D7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D7"].Value = "Số Lượng";

                        range.Worksheet.Cells["E7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E7"].Value = "Giá Niêm Yết";

                        range.Worksheet.Cells["F7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F7"].Value = "Giá Ưu Đãi";

                        range.Worksheet.Cells["G7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G7"].Value = "Thành Tiền";

                        range.Worksheet.Cells["H7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H7"].Value = "NB Đã Thanh Toán";

                        range.Worksheet.Cells["I7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I7"].Value = "Công Nợ Còn Phải Thanh Toán";

                        range.Worksheet.Cells["J7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J7"].Value = "Giá tồn kho";
                    }

                    //write data from line 8
                    int index = 8;
                    int stt = 1;
                    var formatCurrency = "#,##0.00";
                    if (datas.Any())
                    {
                        foreach (var nhomChiPhi in lstNhomChiPhi)
                        {
                            var lstDataTheoNhom = datas
                                .Where(x => x.Nhom == (Enums.BangKeChiPhiTheoNhomDichVu)nhomChiPhi.KeyId).ToList();

                            worksheet.Cells["A" + index + ":J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                            worksheet.Cells["A" + index + ":J" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            worksheet.Cells["A" + index + ":J" + index].Style.Font.Color.SetColor(Color.Black);
                            worksheet.Cells["A" + index + ":J" + index].Style.Font.Bold = true;
                            worksheet.Cells["A" + index + ":J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + index + ":J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

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
                            worksheet.Row(index).Height = 20.5;

                            worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["A" + index].Value = nhomChiPhi.Ma;

                            worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["B" + index].Value = nhomChiPhi.DisplayName;

                            worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["G" + index].Value = lstDataTheoNhom.Sum(x => x.ThanhTien);
                            worksheet.Cells["G" + index].Style.Numberformat.Format = formatCurrency;
                            worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["J" + index].Value = lstDataTheoNhom.Sum(x => x.TongCongGiaTonKho);
                            worksheet.Cells["J" + index].Style.Numberformat.Format = formatCurrency;
                            worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            index++;

                            foreach (var item in lstDataTheoNhom)
                            {
                                // format border, font chữ,....
                                worksheet.Cells["A" + index + ":J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                worksheet.Cells["A" + index + ":J" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                worksheet.Cells["A" + index + ":J" + index].Style.Font.Color.SetColor(Color.Black);
                                worksheet.Cells["A" + index + ":J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["A" + index + ":J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

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
                                worksheet.Row(index).Height = 20.5;

                                worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index].Value = item.STT;

                                worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["B" + index].Value = item.NoiDung;

                                worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["C" + index].Value = item.DonViTinh;

                                worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["D" + index].Value = item.SoLuong;

                                worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["E" + index].Value = item.GiaNiemYet;
                                worksheet.Cells["E" + index].Style.Numberformat.Format = formatCurrency;
                                worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                                worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["F" + index].Value = item.GiaUuDai;
                                worksheet.Cells["F" + index].Style.Numberformat.Format = formatCurrency;
                                worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                                worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["G" + index].Value = item.ThanhTien;
                                worksheet.Cells["G" + index].Style.Numberformat.Format = formatCurrency;
                                worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                                worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["H" + index].Value = item.NguoiBenhDaThanhToan;
                                worksheet.Cells["H" + index].Style.Numberformat.Format = formatCurrency;
                                worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                                worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["I" + index].Value = item.CongNoConPhaiThanhToan;
                                worksheet.Cells["I" + index].Style.Numberformat.Format = formatCurrency;
                                worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                                worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["J" + index].Value = item.TongCongGiaTonKho;
                                worksheet.Cells["J" + index].Style.Numberformat.Format = formatCurrency;
                                worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                                stt++;
                                index++;
                            }
                        }

                        //total
                        worksheet.Cells["A" + index + ":J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        worksheet.Cells["A" + index + ":J" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        worksheet.Cells["A" + index + ":J" + index].Style.Font.Color.SetColor(Color.Black);
                        worksheet.Cells["A" + index + ":J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["A" + index + ":J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

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
                        worksheet.Row(index).Height = 20.5;

                        using (var range = worksheet.Cells["A" + index + ":J" + index])
                        {
                            range.Worksheet.Cells["A" + index + ":B" + index].Merge = true;
                            range.Worksheet.Cells["A" + index + ":B" + index].Value = "Tổng Cộng (I+II+III+IV+V+VI+VII)";
                            range.Worksheet.Cells["A" + index + ":B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            range.Worksheet.Cells["A" + index + ":B" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                            range.Worksheet.Cells["A" + index + ":B" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells["A" + index + ":B" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["A" + index + ":B" + index].Style.Font.Bold = true;
                        }


                        worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["G" + index].Style.Font.Bold = true;
                        worksheet.Cells["G" + index].Value = datas.Sum(x => x.ThanhTien);
                        worksheet.Cells["G" + index].Style.Numberformat.Format = formatCurrency;

                        worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["J" + index].Style.Font.Bold = true;
                        worksheet.Cells["J" + index].Value = datas.Sum(x => x.TongCongGiaTonKho);
                        worksheet.Cells["J" + index].Style.Numberformat.Format = formatCurrency;
                    }
                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }

        private string ConvertNumberToStringRoman(int number)
        {
            if ((number < 0) || (number > 3999)) throw new ArgumentOutOfRangeException("insert value betwheen 1 and 3999");
            if (number < 1) return string.Empty;
            if (number >= 1000) return "M" + ConvertNumberToStringRoman(number - 1000);
            if (number >= 900) return "CM" + ConvertNumberToStringRoman(number - 900);
            if (number >= 500) return "D" + ConvertNumberToStringRoman(number - 500);
            if (number >= 400) return "CD" + ConvertNumberToStringRoman(number - 400);
            if (number >= 100) return "C" + ConvertNumberToStringRoman(number - 100);
            if (number >= 90) return "XC" + ConvertNumberToStringRoman(number - 90);
            if (number >= 50) return "L" + ConvertNumberToStringRoman(number - 50);
            if (number >= 40) return "XL" + ConvertNumberToStringRoman(number - 40);
            if (number >= 10) return "X" + ConvertNumberToStringRoman(number - 10);
            if (number >= 9) return "IX" + ConvertNumberToStringRoman(number - 9);
            if (number >= 5) return "V" + ConvertNumberToStringRoman(number - 5);
            if (number >= 4) return "IV" + ConvertNumberToStringRoman(number - 4);
            if (number >= 1) return "I" + ConvertNumberToStringRoman(number - 1);
            throw new ArgumentOutOfRangeException("insert value betwheen 1 and 3999");
        }
    }
}
