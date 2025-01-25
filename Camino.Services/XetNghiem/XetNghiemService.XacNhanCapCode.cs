using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XetNghiem;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;

namespace Camino.Services.XetNghiem
{
    public partial class XetNghiemService
    {
        public static string pathFileSoLuongInThem = @"Resource\\SoLuongInThemBarcodeTheoTaiKhoan.json";

        #region grid
        public async Task<GridDataSource> GetDataForGridChuaCapCodeAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var timKiemNangCaoObj = new XacNhanCapCodeTimKiemNangCaoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<XacNhanCapCodeTimKiemNangCaoVo>(queryInfo.AdditionalSearchString);
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay)) //) || !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }

            var laNhanVienKhoaXetNghiem = await KiemTraNhanVienThuocKhoaXetNghiemAsync();
            var phongHienTai = new Core.Domain.Entities.PhongBenhViens.PhongBenhVien();
            if (!laNhanVienKhoaXetNghiem)
            {
                phongHienTai = _phongBenhVienRepository.TableNoTracking
                    .First(x => x.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            }

            // thach : tach query performance.
            var yeuCauTiepNhanIds = _yeuCauDichVuKyThuatRepository.TableNoTracking.Where(a =>
                                    a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                    && a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                    && (a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                                        a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                                        a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                                    && (tuNgay == null || (tuNgay != null && ((a.NoiTruPhieuDieuTriId == null && a.ThoiDiemChiDinh.Date >= tuNgay.Value.Date) || (a.NoiTruPhieuDieuTriId != null && a.ThoiDiemDangKy.Date >= tuNgay.Value.Date))))
                                    && (denNgay == null || (denNgay != null && ((a.NoiTruPhieuDieuTriId == null && a.ThoiDiemChiDinh.Date <= denNgay.Value.Date) || (a.NoiTruPhieuDieuTriId != null && a.ThoiDiemDangKy.Date <= denNgay.Value.Date))))
                                    // bổ sung load theo tài khoản nhân viên đang login
                                    && (laNhanVienKhoaXetNghiem || (a.NoiChiDinh != null && a.NoiChiDinh.KhoaPhongId == phongHienTai.KhoaPhongId))
                                    && !a.PhienXetNghiemChiTiets.Any()
                                    && (timKiemNangCaoObj.YeuCauTiepNhanId == null || a.YeuCauTiepNhanId != timKiemNangCaoObj.YeuCauTiepNhanId)
                                    && (timKiemNangCaoObj.HopDongKhamSucKhoeId == null
                                    || timKiemNangCaoObj.HopDongKhamSucKhoeId == 0
                                    || (a.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe && a.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien != null && a.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == timKiemNangCaoObj.HopDongKhamSucKhoeId))
                                    ).Select(dv => dv.YeuCauTiepNhanId).Distinct().ToList();

            var query =
                _yeuCauTiepNhanRepository.TableNoTracking
                    .ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.MaYeuCauTiepNhan, x => x.BenhNhan.MaBN, x => x.HoTen)
                    .Include(x => x.BenhNhan)
                    .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.DichVuKyThuatBenhVien)
                    .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.NhomDichVuBenhVien)
                    .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.PhienXetNghiemChiTiets)
                    //.Include(x => x.PhienXetNghiems).ThenInclude(y => y.PhienXetNghiemChiTiets).ThenInclude(z => z.DichVuKyThuatBenhVien)
                    //.Include(x => x.PhienXetNghiems).ThenInclude(y => y.MauXetNghiems).ThenInclude(z => z.PhieuGoiMauXetNghiem)
                    .Where(x => x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                                && (timKiemNangCaoObj.HopDongKhamSucKhoeId == null
                                    || timKiemNangCaoObj.HopDongKhamSucKhoeId == 0
                                    || (x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe && x.HopDongKhamSucKhoeNhanVien != null && x.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == timKiemNangCaoObj.HopDongKhamSucKhoeId))
                                && (timKiemNangCaoObj.YeuCauTiepNhanId == null || x.Id != timKiemNangCaoObj.YeuCauTiepNhanId)
                                                                 //&& x.YeuCauDichVuKyThuats.Any(a =>
                                                                 //    a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                                                 //    && a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                                 //    && (a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                                                                 //        a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                                                                 //        a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                                                                 //    && (tuNgay == null || (tuNgay != null && ((a.NoiTruPhieuDieuTriId == null && a.ThoiDiemChiDinh.Date >= tuNgay.Value.Date) || (a.NoiTruPhieuDieuTriId != null && a.ThoiDiemDangKy.Date >= tuNgay.Value.Date))))
                                                                 //    && (denNgay == null || (denNgay != null && ((a.NoiTruPhieuDieuTriId == null && a.ThoiDiemChiDinh.Date <= denNgay.Value.Date) || (a.NoiTruPhieuDieuTriId != null && a.ThoiDiemDangKy.Date <= denNgay.Value.Date))))
                                                                 //    // bổ sung load theo tài khoản nhân viên đang login
                                                                 //    && (laNhanVienKhoaXetNghiem || (a.NoiChiDinh != null && a.NoiChiDinh.KhoaPhongId == phongHienTai.KhoaPhongId))

                                                                 //    && !a.PhienXetNghiemChiTiets.Any())
                                                                 && yeuCauTiepNhanIds.Contains(x.Id)
                    // kiểm tra trạng thái
                    //&& ((timKiemNangCaoObj.TrangThai.ChoLayMau && !a.PhienXetNghiemChiTiets.Any())
                    //    || (timKiemNangCaoObj.TrangThai.DaLayMau && a.PhienXetNghiemChiTiets.Any(b => b.ThoiDiemLayMau != null))
                    //    || (!timKiemNangCaoObj.TrangThai.ChoLayMau && !timKiemNangCaoObj.TrangThai.DaLayMau)))

                    // chuyển applylike -> search thủ công
                    //&& (x.MaYeuCauTiepNhan.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                    //    || x.HoTen.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                    //    || x.BenhNhan.MaBN.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                    //    || x.PhienXetNghiems.Any(c => c.BarCodeId.Contains(searchStringRemoveVietnameseDiacritics)))
                    )
                    .Select(item => new BenhNhanXetNghiemGridVo()
                    {
                        Id = item.Id,
                        MaTiepNhan = item.MaYeuCauTiepNhan,
                        MaBenhNhan = item.BenhNhan.MaBN,
                        HoTen = item.HoTen,
                        GioiTinh = item.GioiTinh,
                        NamSinh = item.NamSinh,
                        ThoiDiemTiepNhan = item.ThoiDiemTiepNhan,
                        //Dùng export excel
                        NgaySinh = item.NgaySinh,
                        ThangSinh = item.ThangSinh,
                        TenCongTy = item.HopDongKhamSucKhoeNhanVien != null && item.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe != null && item.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoe != null ? item.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten : "",
                        SoHopDong = item.HopDongKhamSucKhoeNhanVien != null && item.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe != null ? item.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.SoHopDong : ""
                        // end  export excel

                    })
                .OrderBy(queryInfo.SortString).ThenBy(x => x.ThoiDiemTiepNhan).Skip(queryInfo.Skip).Take(queryInfo.Take).ToList();

            var result = query.ToArray();
            //.Select(item => new BenhNhanXetNghiemGridVo()
            //{
            //    Id = item.Id,
            //    MaTiepNhan = item.MaYeuCauTiepNhan,
            //    MaBenhNhan = item.BenhNhan.MaBN,
            //    HoTen = item.HoTen,
            //    NamSinh = item.NamSinh
            //}).ToArray();

            return new GridDataSource
            {
                Data = result,
                TotalRowCount = result.Length
            };
        }
        public async Task<GridDataSource> GetTotalPageForGridChuaCapCodeAsync(QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new XacNhanCapCodeTimKiemNangCaoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<XacNhanCapCodeTimKiemNangCaoVo>(queryInfo.AdditionalSearchString);
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay)) //) || !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);

                tuNgay = tuNgayTemp;

            }

            if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }

            var laNhanVienKhoaXetNghiem = await KiemTraNhanVienThuocKhoaXetNghiemAsync();
            var phongHienTai = new Core.Domain.Entities.PhongBenhViens.PhongBenhVien();
            if (!laNhanVienKhoaXetNghiem)
            {
                phongHienTai = _phongBenhVienRepository.TableNoTracking
                    .First(x => x.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            }

            var yeuCauTiepNhanIds = _yeuCauDichVuKyThuatRepository.TableNoTracking.Where(a =>
                        a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                        && a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                        && (a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                            a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                            a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                        && (tuNgay == null || (tuNgay != null && ((a.NoiTruPhieuDieuTriId == null && a.ThoiDiemChiDinh.Date >= tuNgay.Value.Date) || (a.NoiTruPhieuDieuTriId != null && a.ThoiDiemDangKy.Date >= tuNgay.Value.Date))))
                        && (denNgay == null || (denNgay != null && ((a.NoiTruPhieuDieuTriId == null && a.ThoiDiemChiDinh.Date <= denNgay.Value.Date) || (a.NoiTruPhieuDieuTriId != null && a.ThoiDiemDangKy.Date <= denNgay.Value.Date))))
                        // bổ sung load theo tài khoản nhân viên đang login
                        && (laNhanVienKhoaXetNghiem || (a.NoiChiDinh != null && a.NoiChiDinh.KhoaPhongId == phongHienTai.KhoaPhongId))
                        && !a.PhienXetNghiemChiTiets.Any()
                        && (timKiemNangCaoObj.YeuCauTiepNhanId == null || a.YeuCauTiepNhanId != timKiemNangCaoObj.YeuCauTiepNhanId)
                        && (timKiemNangCaoObj.HopDongKhamSucKhoeId == null
                        || timKiemNangCaoObj.HopDongKhamSucKhoeId == 0
                        || (a.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe && a.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien != null && a.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == timKiemNangCaoObj.HopDongKhamSucKhoeId))
                        ).Select(dv => dv.YeuCauTiepNhanId).Distinct().ToList();
            var query = _yeuCauTiepNhanRepository.TableNoTracking
                .ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.MaYeuCauTiepNhan, x => x.BenhNhan.MaBN, x => x.HoTen)
                .Where(x => x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                            && (timKiemNangCaoObj.HopDongKhamSucKhoeId == null
                                || timKiemNangCaoObj.HopDongKhamSucKhoeId == 0
                                || (x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe && x.HopDongKhamSucKhoeNhanVien != null && x.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == timKiemNangCaoObj.HopDongKhamSucKhoeId))
                            && (timKiemNangCaoObj.YeuCauTiepNhanId == null || x.Id != timKiemNangCaoObj.YeuCauTiepNhanId)
                                                             //&& x.YeuCauDichVuKyThuats.Any(a =>
                                                             //    a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                                             //    && a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                             //    && (a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                                                             //        a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                                                             //        a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                                                             //    && (tuNgay == null || (tuNgay != null && ((a.NoiTruPhieuDieuTriId == null && a.ThoiDiemChiDinh.Date >= tuNgay.Value.Date) || (a.NoiTruPhieuDieuTriId != null && a.ThoiDiemDangKy.Date >= tuNgay.Value.Date))))
                                                             //    && (denNgay == null || (denNgay != null && ((a.NoiTruPhieuDieuTriId == null && a.ThoiDiemChiDinh.Date <= denNgay.Value.Date) || (a.NoiTruPhieuDieuTriId != null && a.ThoiDiemDangKy.Date <= denNgay.Value.Date))))
                                                             //    && (laNhanVienKhoaXetNghiem || (a.NoiChiDinh != null && a.NoiChiDinh.KhoaPhongId == phongHienTai.KhoaPhongId))

                                                             //    && !a.PhienXetNghiemChiTiets.Any())
                                                             && yeuCauTiepNhanIds.Contains(x.Id)
                            // kiểm tra trạng thái
                            //&& ((timKiemNangCaoObj.TrangThai.ChoLayMau && !a.PhienXetNghiemChiTiets.Any())
                            //    || (timKiemNangCaoObj.TrangThai.DaLayMau && a.PhienXetNghiemChiTiets.Any(b => b.ThoiDiemLayMau != null))
                            //    || (!timKiemNangCaoObj.TrangThai.ChoLayMau && !timKiemNangCaoObj.TrangThai.DaLayMau)))

                            // chuyển applylike -> search thủ công
                            //&& (x.MaYeuCauTiepNhan.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                            //    || x.HoTen.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                            //    || x.BenhNhan.MaBN.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                            //    || x.PhienXetNghiems.Any(c => c.BarCodeId.Contains(searchStringRemoveVietnameseDiacritics)))
                            )
                .Include(x => x.BenhNhan)
                //.Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.NoiTruPhieuDieuTri)
                .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.DichVuKyThuatBenhVien)
                .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.NhomDichVuBenhVien)
                .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.PhienXetNghiemChiTiets).Select(c => c.Id);
            //.Include(x => x.PhienXetNghiems).ThenInclude(y => y.PhienXetNghiemChiTiets).ThenInclude(z => z.DichVuKyThuatBenhVien)
            //.Include(x => x.PhienXetNghiems).ThenInclude(y => y.MauXetNghiems).ThenInclude(z => z.PhieuGoiMauXetNghiem);

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataForGridChuaCapCodeAsyncVer2(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var timKiemNangCaoObj = new XacNhanCapCodeTimKiemNangCaoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<XacNhanCapCodeTimKiemNangCaoVo>(queryInfo.AdditionalSearchString);
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay)) //) || !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }

            var laNhanVienKhoaXetNghiem = await KiemTraNhanVienThuocKhoaXetNghiemAsync();
            var phongHienTai = new Core.Domain.Entities.PhongBenhViens.PhongBenhVien();
            if (!laNhanVienKhoaXetNghiem)
            {
                phongHienTai = _phongBenhVienRepository.TableNoTracking
                    .First(x => x.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            }

            var query =
                _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.YeuCauTiepNhan.MaYeuCauTiepNhan, x => x.YeuCauTiepNhan.BenhNhan.MaBN, x => x.YeuCauTiepNhan.HoTen)
                    .Where(x => x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                                && (timKiemNangCaoObj.HopDongKhamSucKhoeId == null
                                    || timKiemNangCaoObj.HopDongKhamSucKhoeId == 0
                                    || (x.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe
                                        && x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien != null && x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == timKiemNangCaoObj.HopDongKhamSucKhoeId))

                                //ko hiển thị YCTN đang chọn xem chi tiết
                                && (timKiemNangCaoObj.YeuCauTiepNhanId == null || x.YeuCauTiepNhanId != timKiemNangCaoObj.YeuCauTiepNhanId)

                                && x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && (x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                                    x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                                    x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                                && (tuNgay == null || (tuNgay != null && ((x.NoiTruPhieuDieuTriId == null && x.ThoiDiemChiDinh.Date >= tuNgay.Value.Date) || (x.NoiTruPhieuDieuTriId != null && x.ThoiDiemDangKy.Date >= tuNgay.Value.Date))))
                                && (denNgay == null || (denNgay != null && ((x.NoiTruPhieuDieuTriId == null && x.ThoiDiemChiDinh.Date <= denNgay.Value.Date) || (x.NoiTruPhieuDieuTriId != null && x.ThoiDiemDangKy.Date <= denNgay.Value.Date))))

                                // bổ sung load theo tài khoản nhân viên đang login
                                && (laNhanVienKhoaXetNghiem || (x.NoiChiDinh != null && x.NoiChiDinh.KhoaPhongId == phongHienTai.KhoaPhongId))

                                && !x.PhienXetNghiemChiTiets.Any()
                    )
                    .Select(item => new BenhNhanXetNghiemGridVo()
                    {
                        Id = item.YeuCauTiepNhanId,
                        MaTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                        MaBenhNhan = item.YeuCauTiepNhan.BenhNhan.MaBN,
                        HoTen = item.YeuCauTiepNhan.HoTen,
                        NamSinh = item.YeuCauTiepNhan.NamSinh,
                        ThoiDiemTiepNhan = item.YeuCauTiepNhan.ThoiDiemTiepNhan
                    })
                    .GroupBy(x => new { x.Id })
                    .Select(x => x.First())
                .OrderBy(queryInfo.SortString).ThenBy(x => x.ThoiDiemTiepNhan).Skip(queryInfo.Skip).Take(queryInfo.Take).ToList();

            var result = query.ToArray();

            return new GridDataSource
            {
                Data = result,
                TotalRowCount = result.Length
            };
        }
        public async Task<GridDataSource> GetTotalPageForGridChuaCapCodeAsyncVer2(QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new XacNhanCapCodeTimKiemNangCaoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<XacNhanCapCodeTimKiemNangCaoVo>(queryInfo.AdditionalSearchString);
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay)) //) || !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);

                tuNgay = tuNgayTemp;

            }

            if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }

            var laNhanVienKhoaXetNghiem = await KiemTraNhanVienThuocKhoaXetNghiemAsync();
            var phongHienTai = new Core.Domain.Entities.PhongBenhViens.PhongBenhVien();
            if (!laNhanVienKhoaXetNghiem)
            {
                phongHienTai = _phongBenhVienRepository.TableNoTracking
                    .First(x => x.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            }

            var query =
                _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.YeuCauTiepNhan.MaYeuCauTiepNhan, x => x.YeuCauTiepNhan.BenhNhan.MaBN, x => x.YeuCauTiepNhan.HoTen)
                    .Where(x => x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                                && (timKiemNangCaoObj.HopDongKhamSucKhoeId == null
                                    || timKiemNangCaoObj.HopDongKhamSucKhoeId == 0
                                    || (x.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe
                                        && x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien != null && x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == timKiemNangCaoObj.HopDongKhamSucKhoeId))

                                //ko hiển thị YCTN đang chọn xem chi tiết
                                && (timKiemNangCaoObj.YeuCauTiepNhanId == null || x.YeuCauTiepNhanId != timKiemNangCaoObj.YeuCauTiepNhanId)

                                && x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && (x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                                    x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                                    x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                                && (tuNgay == null || (tuNgay != null && ((x.NoiTruPhieuDieuTriId == null && x.ThoiDiemChiDinh.Date >= tuNgay.Value.Date) || (x.NoiTruPhieuDieuTriId != null && x.ThoiDiemDangKy.Date >= tuNgay.Value.Date))))
                                && (denNgay == null || (denNgay != null && ((x.NoiTruPhieuDieuTriId == null && x.ThoiDiemChiDinh.Date <= denNgay.Value.Date) || (x.NoiTruPhieuDieuTriId != null && x.ThoiDiemDangKy.Date <= denNgay.Value.Date))))

                                // bổ sung load theo tài khoản nhân viên đang login
                                && (laNhanVienKhoaXetNghiem || (x.NoiChiDinh != null && x.NoiChiDinh.KhoaPhongId == phongHienTai.KhoaPhongId))

                                && !x.PhienXetNghiemChiTiets.Any()
                    )
                    .Select(item => new BenhNhanXetNghiemGridVo()
                    {
                        Id = item.YeuCauTiepNhanId,
                        MaTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                        MaBenhNhan = item.YeuCauTiepNhan.BenhNhan.MaBN,
                        HoTen = item.YeuCauTiepNhan.HoTen,
                        NamSinh = item.YeuCauTiepNhan.NamSinh,
                        ThoiDiemTiepNhan = item.YeuCauTiepNhan.ThoiDiemTiepNhan
                    })
                    .GroupBy(x => new { x.Id })
                    .Select(x => x.First());

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataForGridDaCapCodeAsync(QueryInfo queryInfo)
        {
            //BuildDefaultSortExpression(queryInfo);
            var timKiemNangCaoObj = new XacNhanCapCodeTimKiemNangCaoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<XacNhanCapCodeTimKiemNangCaoVo>(queryInfo.AdditionalSearchString);
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay)) //) || !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }

            var laNhanVienKhoaXetNghiem = await KiemTraNhanVienThuocKhoaXetNghiemAsync();
            var phongHienTai = new Core.Domain.Entities.PhongBenhViens.PhongBenhVien();
            if (!laNhanVienKhoaXetNghiem)
            {
                phongHienTai = _phongBenhVienRepository.TableNoTracking
                    .First(x => x.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            }
            var queryPhienXetNghiemChiTiets = _phienXetNghiemChiTietRepository.TableNoTracking.Where(a => (timKiemNangCaoObj.TrangThai == null
                                                                             || (timKiemNangCaoObj.TrangThai.DaCapCode && a.ThoiDiemLayMau != null)
                                                                             || (timKiemNangCaoObj.TrangThai.ChuaNhanMau && a.ThoiDiemNhanMau == null)
                                                                             || (timKiemNangCaoObj.TrangThai.DaNhanMau && a.ThoiDiemNhanMau != null))
                                                                      // kiểm tra nhân viên khoa xét nghiệm
                                                                      && (laNhanVienKhoaXetNghiem
                                                                              || (a.YeuCauDichVuKyThuat.NoiChiDinh != null && a.YeuCauDichVuKyThuat.NoiChiDinh.KhoaPhongId == phongHienTai.KhoaPhongId))

                                                                      // kiểm tra trạng thái YCDVKT
                                                                      && a.YeuCauDichVuKyThuat.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                                      && (a.YeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                                                                          a.YeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                                                                          a.YeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)

                                                                       // kiểm tra thời điểm chỉ định theo ngày filter
                                                                       //&& (tuNgay == null 
                                                                       //    || (tuNgay != null 
                                                                       //        && ((a.YeuCauDichVuKyThuat.NoiTruPhieuDieuTriId == null && a.YeuCauDichVuKyThuat.ThoiDiemChiDinh.Date >= tuNgay.Value.Date) 
                                                                       //            || (a.YeuCauDichVuKyThuat.NoiTruPhieuDieuTriId != null && a.YeuCauDichVuKyThuat.ThoiDiemDangKy.Date >= tuNgay.Value.Date))))
                                                                       //&& (denNgay == null 
                                                                       //    || (denNgay != null 
                                                                       //        && ((a.YeuCauDichVuKyThuat.NoiTruPhieuDieuTriId == null && a.YeuCauDichVuKyThuat.ThoiDiemChiDinh.Date <= denNgay.Value.Date) 
                                                                       //            || (a.YeuCauDichVuKyThuat.NoiTruPhieuDieuTriId != null && a.YeuCauDichVuKyThuat.ThoiDiemDangKy.Date <= denNgay.Value.Date))))
                                                                       && (tuNgay == null
                                    || (tuNgay != null && a.PhienXetNghiem.ThoiDiemBatDau.Date >= tuNgay.Value.Date))
                                && (denNgay == null
                                    || (denNgay != null && a.PhienXetNghiem.ThoiDiemBatDau.Date <= denNgay.Value.Date))
                                                                      ).Select(o => o.PhienXetNghiemId).Distinct().ToList();

            var query =
                _phienXetNghiemRepository.TableNoTracking
                    .ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.YeuCauTiepNhan.MaYeuCauTiepNhan, x => x.BenhNhan.MaBN, x => x.YeuCauTiepNhan.HoTen, x => x.BarCodeId)
                    //.Include(x => x.BenhNhan)
                    //.Include(x => x.YeuCauTiepNhan)
                    //.Include(y => y.PhienXetNghiemChiTiets).ThenInclude(z => z.DichVuKyThuatBenhVien)
                    .Where(x => x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                                && (timKiemNangCaoObj.PhienXetNghiemId == null || x.Id != timKiemNangCaoObj.PhienXetNghiemId)
                                && (timKiemNangCaoObj.HopDongKhamSucKhoeId == null
                                    || timKiemNangCaoObj.HopDongKhamSucKhoeId == 0
                                    || (x.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe
                                            && x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien != null && x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == timKiemNangCaoObj.HopDongKhamSucKhoeId))
                                && (timKiemNangCaoObj.YeuCauTiepNhanId == null || x.Id != timKiemNangCaoObj.YeuCauTiepNhanId)
                                && queryPhienXetNghiemChiTiets.Contains(x.Id)
                                //&& (x.PhienXetNghiemChiTiets.Any(a => (timKiemNangCaoObj.TrangThai == null
                                //                                           || (timKiemNangCaoObj.TrangThai.DaCapCode && a.ThoiDiemLayMau != null)
                                //                                           || (timKiemNangCaoObj.TrangThai.ChuaNhanMau && a.ThoiDiemNhanMau == null)
                                //                                           || (timKiemNangCaoObj.TrangThai.DaNhanMau && a.ThoiDiemNhanMau != null))
                                //                                    // kiểm tra nhân viên khoa xét nghiệm
                                //                                    && (laNhanVienKhoaXetNghiem
                                //                                            || (a.YeuCauDichVuKyThuat.NoiChiDinh != null && a.YeuCauDichVuKyThuat.NoiChiDinh.KhoaPhongId == phongHienTai.KhoaPhongId))

                                //                                    // kiểm tra trạng thái YCDVKT
                                //                                    && a.YeuCauDichVuKyThuat.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                //                                    && (a.YeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                                //                                        a.YeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                                //                                        a.YeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)

                                //                                      // kiểm tra thời điểm chỉ định theo ngày filter
                                //                                      //&& (tuNgay == null 
                                //                                      //    || (tuNgay != null 
                                //                                      //        && ((a.YeuCauDichVuKyThuat.NoiTruPhieuDieuTriId == null && a.YeuCauDichVuKyThuat.ThoiDiemChiDinh.Date >= tuNgay.Value.Date) 
                                //                                      //            || (a.YeuCauDichVuKyThuat.NoiTruPhieuDieuTriId != null && a.YeuCauDichVuKyThuat.ThoiDiemDangKy.Date >= tuNgay.Value.Date))))
                                //                                      //&& (denNgay == null 
                                //                                      //    || (denNgay != null 
                                //                                      //        && ((a.YeuCauDichVuKyThuat.NoiTruPhieuDieuTriId == null && a.YeuCauDichVuKyThuat.ThoiDiemChiDinh.Date <= denNgay.Value.Date) 
                                //                                      //            || (a.YeuCauDichVuKyThuat.NoiTruPhieuDieuTriId != null && a.YeuCauDichVuKyThuat.ThoiDiemDangKy.Date <= denNgay.Value.Date))))

                                //                                      ))
                                // kiểm tra thời điểm bắt đầu theo ngày filter
                                && (tuNgay == null
                                    || (tuNgay != null && x.ThoiDiemBatDau.Date >= tuNgay.Value.Date))
                                && (denNgay == null
                                    || (denNgay != null && x.ThoiDiemBatDau.Date <= denNgay.Value.Date))
                    )
                    .Select(item => new BenhNhanXetNghiemGridVo()
                    {
                        Id = item.YeuCauTiepNhanId,
                        PhienXetNghiemId = item.Id,
                        MaTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                        MaBenhNhan = item.BenhNhan.MaBN,
                        HoTen = item.YeuCauTiepNhan.HoTen,
                        NgaySinh = item.YeuCauTiepNhan.NgaySinh,
                        ThangSinh = item.YeuCauTiepNhan.ThangSinh,
                        NamSinh = item.YeuCauTiepNhan.NamSinh,
                        Barcode = item.BarCodeId,
                        ThoiDiemTiepNhan = item.YeuCauTiepNhan.ThoiDiemTiepNhan,
                        GioiTinh = item.YeuCauTiepNhan.GioiTinh,
                        ThoiDiemBatDau = item.ThoiDiemBatDau,
                    });

            if (string.IsNullOrEmpty(queryInfo.SortString))
            {
                query = query.OrderByDescending(x => x.ThoiDiemBatDau).ThenBy(x => x.ThoiDiemTiepNhan).Skip(queryInfo.Skip).Take(queryInfo.Take);
            }
            else
            {
                query = query.OrderBy(queryInfo.SortString).ThenBy(x => x.ThoiDiemBatDau).Skip(queryInfo.Skip).Take(queryInfo.Take);
            }


            var result = query.ToArray();
            //.Select(item => new BenhNhanXetNghiemGridVo()
            //{
            //    Id = item.YeuCauTiepNhanId,
            //    PhienXetNghiemId = item.Id,
            //    MaTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
            //    MaBenhNhan = item.BenhNhan.MaBN,
            //    HoTen = item.YeuCauTiepNhan.HoTen,
            //    NamSinh = item.YeuCauTiepNhan.NamSinh,
            //    Barcode = item.BarCodeId
            //}).ToArray();

            return new GridDataSource
            {
                Data = result,
                TotalRowCount = result.Length
            };
        }
        public async Task<GridDataSource> GetTotalPageForGridDaCapCodeAsync(QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new XacNhanCapCodeTimKiemNangCaoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<XacNhanCapCodeTimKiemNangCaoVo>(queryInfo.AdditionalSearchString);
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay)) //) || !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);

                tuNgay = tuNgayTemp;

            }

            if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }

            var laNhanVienKhoaXetNghiem = await KiemTraNhanVienThuocKhoaXetNghiemAsync();
            var phongHienTai = new Core.Domain.Entities.PhongBenhViens.PhongBenhVien();
            if (!laNhanVienKhoaXetNghiem)
            {
                phongHienTai = _phongBenhVienRepository.TableNoTracking
                    .First(x => x.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            }
            var queryPhienXetNghiemChiTiets = _phienXetNghiemChiTietRepository.TableNoTracking.Where(a => (timKiemNangCaoObj.TrangThai == null
                                                                             || (timKiemNangCaoObj.TrangThai.DaCapCode && a.ThoiDiemLayMau != null)
                                                                             || (timKiemNangCaoObj.TrangThai.ChuaNhanMau && a.ThoiDiemNhanMau == null)
                                                                             || (timKiemNangCaoObj.TrangThai.DaNhanMau && a.ThoiDiemNhanMau != null))
                                                                      // kiểm tra nhân viên khoa xét nghiệm
                                                                      && (laNhanVienKhoaXetNghiem
                                                                              || (a.YeuCauDichVuKyThuat.NoiChiDinh != null && a.YeuCauDichVuKyThuat.NoiChiDinh.KhoaPhongId == phongHienTai.KhoaPhongId))

                                                                      // kiểm tra trạng thái YCDVKT
                                                                      && a.YeuCauDichVuKyThuat.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                                      && (a.YeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                                                                          a.YeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                                                                          a.YeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)

                                                                       // kiểm tra thời điểm chỉ định theo ngày filter
                                                                       //&& (tuNgay == null 
                                                                       //    || (tuNgay != null 
                                                                       //        && ((a.YeuCauDichVuKyThuat.NoiTruPhieuDieuTriId == null && a.YeuCauDichVuKyThuat.ThoiDiemChiDinh.Date >= tuNgay.Value.Date) 
                                                                       //            || (a.YeuCauDichVuKyThuat.NoiTruPhieuDieuTriId != null && a.YeuCauDichVuKyThuat.ThoiDiemDangKy.Date >= tuNgay.Value.Date))))
                                                                       //&& (denNgay == null 
                                                                       //    || (denNgay != null 
                                                                       //        && ((a.YeuCauDichVuKyThuat.NoiTruPhieuDieuTriId == null && a.YeuCauDichVuKyThuat.ThoiDiemChiDinh.Date <= denNgay.Value.Date) 
                                                                       //            || (a.YeuCauDichVuKyThuat.NoiTruPhieuDieuTriId != null && a.YeuCauDichVuKyThuat.ThoiDiemDangKy.Date <= denNgay.Value.Date))))
                                                                       && (tuNgay == null
                                    || (tuNgay != null && a.PhienXetNghiem.ThoiDiemBatDau.Date >= tuNgay.Value.Date))
                                && (denNgay == null
                                    || (denNgay != null && a.PhienXetNghiem.ThoiDiemBatDau.Date <= denNgay.Value.Date))
                                                                      ).Select(o => o.PhienXetNghiemId).Distinct().ToList();

            var query =
                _phienXetNghiemRepository.TableNoTracking
                    .ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.YeuCauTiepNhan.MaYeuCauTiepNhan, x => x.BenhNhan.MaBN, x => x.YeuCauTiepNhan.HoTen, x => x.BarCodeId)
                    //.Include(x => x.BenhNhan)
                    //.Include(x => x.YeuCauTiepNhan)
                    //.Include(y => y.PhienXetNghiemChiTiets).ThenInclude(z => z.DichVuKyThuatBenhVien)
                    .Where(x => x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                                && (timKiemNangCaoObj.PhienXetNghiemId == null || x.Id != timKiemNangCaoObj.PhienXetNghiemId)
                                && (timKiemNangCaoObj.HopDongKhamSucKhoeId == null
                                    || timKiemNangCaoObj.HopDongKhamSucKhoeId == 0
                                    || (x.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe
                                            && x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien != null && x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == timKiemNangCaoObj.HopDongKhamSucKhoeId))
                                && (timKiemNangCaoObj.YeuCauTiepNhanId == null || x.Id != timKiemNangCaoObj.YeuCauTiepNhanId)
                                && queryPhienXetNghiemChiTiets.Contains(x.Id)
                                //&& (x.PhienXetNghiemChiTiets.Any(a => (timKiemNangCaoObj.TrangThai == null
                                //                                           || (timKiemNangCaoObj.TrangThai.DaCapCode && a.ThoiDiemLayMau != null)
                                //                                           || (timKiemNangCaoObj.TrangThai.ChuaNhanMau && a.ThoiDiemNhanMau == null)
                                //                                           || (timKiemNangCaoObj.TrangThai.DaNhanMau && a.ThoiDiemNhanMau != null))
                                //                                    // kiểm tra nhân viên khoa xét nghiệm
                                //                                    && (laNhanVienKhoaXetNghiem
                                //                                            || (a.YeuCauDichVuKyThuat.NoiChiDinh != null && a.YeuCauDichVuKyThuat.NoiChiDinh.KhoaPhongId == phongHienTai.KhoaPhongId))

                                //                                    // kiểm tra trạng thái YCDVKT
                                //                                    && a.YeuCauDichVuKyThuat.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                //                                    && (a.YeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                                //                                        a.YeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                                //                                        a.YeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)

                                //                                    // kiểm tra thời điểm chỉ định theo ngày filter
                                //                                    //&& (tuNgay == null
                                //                                    //    || (tuNgay != null
                                //                                    //        && ((a.YeuCauDichVuKyThuat.NoiTruPhieuDieuTriId == null && a.YeuCauDichVuKyThuat.ThoiDiemChiDinh.Date >= tuNgay.Value.Date)
                                //                                    //            || (a.YeuCauDichVuKyThuat.NoiTruPhieuDieuTriId != null && a.YeuCauDichVuKyThuat.ThoiDiemDangKy.Date >= tuNgay.Value.Date))))
                                //                                    //&& (denNgay == null
                                //                                    //    || (denNgay != null
                                //                                    //        && ((a.YeuCauDichVuKyThuat.NoiTruPhieuDieuTriId == null && a.YeuCauDichVuKyThuat.ThoiDiemChiDinh.Date <= denNgay.Value.Date)
                                //                                    //            || (a.YeuCauDichVuKyThuat.NoiTruPhieuDieuTriId != null && a.YeuCauDichVuKyThuat.ThoiDiemDangKy.Date <= denNgay.Value.Date))))
                                //                                    ))

                                // kiểm tra thời điểm bắt đầu theo ngày filter
                                && (tuNgay == null
                                    || (tuNgay != null && x.ThoiDiemBatDau.Date >= tuNgay.Value.Date))
                                && (denNgay == null
                                    || (denNgay != null && x.ThoiDiemBatDau.Date <= denNgay.Value.Date))
                    );

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataForGridDaCapCodeAsyncVer2(QueryInfo queryInfo)
        {
            //BuildDefaultSortExpression(queryInfo);
            var timKiemNangCaoObj = new XacNhanCapCodeTimKiemNangCaoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<XacNhanCapCodeTimKiemNangCaoVo>(queryInfo.AdditionalSearchString);
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }

            var laNhanVienKhoaXetNghiem = await KiemTraNhanVienThuocKhoaXetNghiemAsync();
            var phongHienTai = new Core.Domain.Entities.PhongBenhViens.PhongBenhVien();
            if (!laNhanVienKhoaXetNghiem)
            {
                phongHienTai = _phongBenhVienRepository.TableNoTracking
                    .First(x => x.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            }

            var query =
                _phienXetNghiemRepository.TableNoTracking
                    .ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.YeuCauTiepNhan.MaYeuCauTiepNhan, x => x.BenhNhan.MaBN, x => x.YeuCauTiepNhan.HoTen, x => x.BarCodeId)
                    .Where(x => x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                                && (timKiemNangCaoObj.PhienXetNghiemId == null || x.Id != timKiemNangCaoObj.PhienXetNghiemId)
                                && (timKiemNangCaoObj.HopDongKhamSucKhoeId == null
                                    || timKiemNangCaoObj.HopDongKhamSucKhoeId == 0
                                    || (x.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe
                                            && x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien != null && x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == timKiemNangCaoObj.HopDongKhamSucKhoeId))
                                && (timKiemNangCaoObj.YeuCauTiepNhanId == null || x.Id != timKiemNangCaoObj.YeuCauTiepNhanId)

                                //&& (x.PhienXetNghiemChiTiets.Any(a => (timKiemNangCaoObj.TrangThai == null
                                //                                           || (timKiemNangCaoObj.TrangThai.DaCapCode && a.ThoiDiemLayMau != null)
                                //                                           || (timKiemNangCaoObj.TrangThai.ChuaNhanMau && a.ThoiDiemNhanMau == null)
                                //                                           || (timKiemNangCaoObj.TrangThai.DaNhanMau && a.ThoiDiemNhanMau != null))
                                //                                    // kiểm tra nhân viên khoa xét nghiệm
                                //                                    && (laNhanVienKhoaXetNghiem
                                //                                            || (a.YeuCauDichVuKyThuat.NoiChiDinh != null && a.YeuCauDichVuKyThuat.NoiChiDinh.KhoaPhongId == phongHienTai.KhoaPhongId))

                                //                                    // kiểm tra trạng thái YCDVKT
                                //                                    && a.YeuCauDichVuKyThuat.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                //                                    && (a.YeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                                //                                        a.YeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                                //                                        a.YeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                                //                                      ))
                                // kiểm tra thời điểm bắt đầu theo ngày filter
                                && (tuNgay == null
                                    || (tuNgay != null && x.ThoiDiemBatDau.Date >= tuNgay.Value.Date))
                                && (denNgay == null
                                    || (denNgay != null && x.ThoiDiemBatDau.Date <= denNgay.Value.Date))
                    )
                    .Select(item => new BenhNhanXetNghiemGridVo()
                    {
                        Id = item.YeuCauTiepNhanId,
                        PhienXetNghiemId = item.Id,
                        MaTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                        MaBenhNhan = item.BenhNhan.MaBN,
                        HoTen = item.YeuCauTiepNhan.HoTen,
                        NgaySinh = item.YeuCauTiepNhan.NgaySinh,
                        ThangSinh = item.YeuCauTiepNhan.ThangSinh,
                        NamSinh = item.YeuCauTiepNhan.NamSinh,
                        Barcode = item.BarCodeId,
                        ThoiDiemTiepNhan = item.YeuCauTiepNhan.ThoiDiemTiepNhan,
                        GioiTinh = item.YeuCauTiepNhan.GioiTinh,
                        ThoiDiemBatDau = item.ThoiDiemBatDau,

                        BenhNhanXetNghiemPhienChiTiets = item.PhienXetNghiemChiTiets.Select(a => new BenhNhanXetNghiemPhienChiTietGridVo()
                        {
                            ThoiDiemLayMau = a.ThoiDiemLayMau,
                            ThoiDiemNhanMau = a.ThoiDiemNhanMau,
                            KhoaPhongChiDinhId = a.YeuCauDichVuKyThuat.NoiChiDinh.KhoaPhongId,
                            TrangThai = a.YeuCauDichVuKyThuat.TrangThai,
                            TrangThaiThanhToan = a.YeuCauDichVuKyThuat.TrangThaiThanhToan
                        }).ToList()
                    })
                    .OrderByDescending(x => x.ThoiDiemBatDau).ThenBy(x => x.ThoiDiemTiepNhan)
                    .ToList();

            query = query
                .Where(x => x.BenhNhanXetNghiemPhienChiTiets.Any(a => (timKiemNangCaoObj.TrangThai == null
                                                               || (timKiemNangCaoObj.TrangThai.DaCapCode && a.ThoiDiemLayMau != null)
                                                               || (timKiemNangCaoObj.TrangThai.ChuaNhanMau && a.ThoiDiemNhanMau == null)
                                                               || (timKiemNangCaoObj.TrangThai.DaNhanMau && a.ThoiDiemNhanMau != null))
                                                              // kiểm tra nhân viên khoa xét nghiệm
                                                              && (laNhanVienKhoaXetNghiem
                                                                  || (a.KhoaPhongChiDinhId != null && a.KhoaPhongChiDinhId == phongHienTai.KhoaPhongId))

                                                              // kiểm tra trạng thái YCDVKT
                                                              && a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                              && (a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                                                                  a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                                                                  a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                ))
                .ToList();


            var result = query.ToArray();

            return new GridDataSource
            {
                Data = result,
                TotalRowCount = result.Length
            };
        }

        public async Task<GridDataSource> GetDataForGridDichVuXetNghiemQuyTrinhCapCodeVaNhanMauAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var timKiemNangCaoObj = new XacNhanCapCodeTimKiemNangCaoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<XacNhanCapCodeTimKiemNangCaoVo>(queryInfo.AdditionalSearchString);
            }
            long yeuCauTiepNhanId = string.IsNullOrEmpty(queryInfo.SearchTerms) ? 0 : long.Parse(queryInfo.SearchTerms);

            //DateTime? tuNgay = null;
            //DateTime? denNgay = null;
            ////kiểm tra tìm kiếm nâng cao
            //if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay))
            //{
            //    DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
            //    tuNgay = tuNgayTemp;
            //}

            //if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay))
            //{
            //    DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
            //    denNgay = denNgayTemp;
            //}

            var laNhanVienKhoaXetNghiem = await KiemTraNhanVienThuocKhoaXetNghiemAsync();
            var phongHienTai = new Core.Domain.Entities.PhongBenhViens.PhongBenhVien();
            if (!laNhanVienKhoaXetNghiem)
            {
                phongHienTai = _phongBenhVienRepository.TableNoTracking
                    .First(x => x.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            }
            
            var lstDichVu =
                _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(y => y.PhienXetNghiem)
                    .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(y => y.NhanVienLayMau).ThenInclude(z => z.User)
                    .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(y => y.NhanVienNhanMau).ThenInclude(z => z.User)
                    .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(y => y.NhanVienKetLuan).ThenInclude(z => z.User)
                    .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(y => y.KetQuaXetNghiemChiTiets)
                    .Include(x => x.DichVuKyThuatBenhVien).ThenInclude(y => y.DichVuXetNghiem)
                    .Include(x => x.NhanVienChiDinh).ThenInclude(y => y.User)
                    .Include(x => x.NhomDichVuBenhVien)
                    .Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId
                                && (timKiemNangCaoObj.PhienXetNghiemId == null || x.PhienXetNghiemChiTiets.Any(a => a.PhienXetNghiemId == timKiemNangCaoObj.PhienXetNghiemId))
                                && x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && (x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                                    x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                                    x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                                //&& (tuNgay == null || (tuNgay != null && ((x.NoiTruPhieuDieuTri == null && x.ThoiDiemChiDinh.Date >= tuNgay.Value.Date) || (x.NoiTruPhieuDieuTri != null && x.NoiTruPhieuDieuTri.NgayDieuTri >= tuNgay.Value.Date))))
                                //&& (denNgay == null || (denNgay != null && ((x.NoiTruPhieuDieuTri == null && x.ThoiDiemChiDinh.Date <= denNgay.Value.Date) || (x.NoiTruPhieuDieuTri != null && x.NoiTruPhieuDieuTri.NgayDieuTri <= denNgay.Value.Date))))
                                && (laNhanVienKhoaXetNghiem || x.NoiChiDinh.KhoaPhongId == phongHienTai.KhoaPhongId))
                    .ToList();
            var query = lstDichVu
                .Select(item => new DichVuXetNghiemQuyTrinhCapCodeVaNhanMauVo()
                {
                    Id = item.Id,
                    YeuCauDichVuKyThuatId = item.Id,
                    PhienXetNghiemChiTietId = item.PhienXetNghiemChiTiets.Any() ? item.PhienXetNghiemChiTiets.Select(a => a.Id).FirstOrDefault() : (long?)null,
                    MaDichVu = item.MaDichVu,
                    TenDichVu = item.TenDichVu,
                    Barcode = item.PhienXetNghiemChiTiets.Any() ? item.PhienXetNghiemChiTiets.Select(a => a.PhienXetNghiem.BarCodeId).FirstOrDefault() : null,
                    BarcodeNumber = item.PhienXetNghiemChiTiets.Any() ? item.PhienXetNghiemChiTiets.Select(a => a.PhienXetNghiem.BarCodeNumber).FirstOrDefault() : (int?)null,
                    ThoiGianChiDinh = item.ThoiDiemDangKy,
                    NguoiChiDinh = item.NhanVienChiDinh.User.HoTen,
                    BenhPham = item.BenhPhamXetNghiem,
                    LoaiMau = item.DichVuKyThuatBenhVien.LoaiMauXetNghiem,
                    TenNhom = item.NhomDichVuBenhVien.Ten,
                    IsNhanVienKhoaXetNghiem = laNhanVienKhoaXetNghiem,
                    IsChayLaiKetQua = item.PhienXetNghiemChiTiets.Any() && item.PhienXetNghiemChiTiets.OrderByDescending(a => a.Id).Select(a => a.ChayLaiKetQua ?? false).FirstOrDefault(),
                    NgayLayMau = item.PhienXetNghiemChiTiets.Any() ? item.PhienXetNghiemChiTiets.OrderByDescending(a => a.Id).Select(a => a.ThoiDiemLayMau).FirstOrDefault() : null,
                    NguoiLayMau = item.PhienXetNghiemChiTiets.Any() ? item.PhienXetNghiemChiTiets.OrderByDescending(a => a.Id).Select(a => a.NhanVienLayMau?.User.HoTen).FirstOrDefault() : null,
                    NgayNhanMau = item.PhienXetNghiemChiTiets.Any() ? item.PhienXetNghiemChiTiets.OrderByDescending(a => a.Id).Select(a => a.ThoiDiemNhanMau).FirstOrDefault() : null,
                    NguoiNhanMau = item.PhienXetNghiemChiTiets.Any() ? item.PhienXetNghiemChiTiets.OrderByDescending(a => a.Id).Select(a => a.NhanVienNhanMau?.User.HoTen).FirstOrDefault() : null,
                    NgayDuyet = item.PhienXetNghiemChiTiets.Any() ? item.PhienXetNghiemChiTiets.OrderByDescending(a => a.Id).Select(a => a.ThoiDiemKetLuan).FirstOrDefault() : null,
                    TenNguoiDuyet = item.PhienXetNghiemChiTiets.Any() ? item.PhienXetNghiemChiTiets.OrderByDescending(a => a.Id).Select(a => a.NhanVienKetLuan?.User.HoTen).FirstOrDefault() : null,

                    //BVHD-3372
                    PhanLoaiDichVu = timKiemNangCaoObj.TrangThai,

                    TatCaKetQuaChuaCoGiaTri = item.PhienXetNghiemChiTiets.Any()
                                              && item.PhienXetNghiemChiTiets.OrderByDescending(a => a.Id)
                                                  .Select(a => a.KetQuaXetNghiemChiTiets.Any() && a.KetQuaXetNghiemChiTiets.All(b => string.IsNullOrEmpty(b.GiaTriTuMay) && string.IsNullOrEmpty(b.GiaTriNhapTay)))
                                                  .FirstOrDefault(),
                    SoThuTuXetNghiem = item.DichVuKyThuatBenhVien.DichVuXetNghiem.SoThuTu ?? 0,
                    NhomDichVuBenhVienId = item.NhomDichVuBenhVienId,
                    CreatedOn = item.CreatedOn
                }).ToList();
            
            if (timKiemNangCaoObj.TrangThai != null && (timKiemNangCaoObj.TrangThai.ChuaCapCode
                                                        || timKiemNangCaoObj.TrangThai.DaCapCode
                                                        || timKiemNangCaoObj.TrangThai.ChuaNhanMau
                                                        || timKiemNangCaoObj.TrangThai.DaNhanMau))
            {
                //query = query.Where(x =>
                //    (timKiemNangCaoObj.TrangThai.ChuaCapCode && x.TrangThai == Enums.TrangThaiLayMauXetNghiem.ChoLayMau)
                //    || (timKiemNangCaoObj.TrangThai.DaCapCode && x.TrangThai == Enums.TrangThaiLayMauXetNghiem.ChoGuiMau)
                //    || (timKiemNangCaoObj.TrangThai.ChuaNhanMau && x.TrangThai == Enums.TrangThaiLayMauXetNghiem.ChoGuiMau)
                //    || (timKiemNangCaoObj.TrangThai.DaNhanMau && (x.TrangThai == Enums.TrangThaiLayMauXetNghiem.ChoKetQua || x.TrangThai == Enums.TrangThaiLayMauXetNghiem.DaCoKetQua)))
                //    .ToList();

                query = query.Where(x =>
                        (timKiemNangCaoObj.TrangThai.ChuaCapCode && x.TrangThai == Enums.TrangThaiLayMauXetNghiemNew.ChoCapCode)
                        || (timKiemNangCaoObj.TrangThai.DaCapCode && x.TrangThai != Enums.TrangThaiLayMauXetNghiemNew.ChoCapCode) //x.TrangThai == Enums.TrangThaiLayMauXetNghiemNew.ChoNhanMau)
                        || (timKiemNangCaoObj.TrangThai.ChuaNhanMau && x.TrangThai == Enums.TrangThaiLayMauXetNghiemNew.ChoNhanMau)
                        || (timKiemNangCaoObj.TrangThai.DaNhanMau && (x.TrangThai == Enums.TrangThaiLayMauXetNghiemNew.ChoKetQua
                                                                      || x.TrangThai == Enums.TrangThaiLayMauXetNghiemNew.DaCoKetQua
                                                                      || x.TrangThai == Enums.TrangThaiLayMauXetNghiemNew.DaDuyet)))
                    .ToList();
            }

            var lstSortNhomDichVuKyThuat = query.OrderBy(x => x.CreatedOn)
                .Select(x => x.NhomDichVuBenhVienId).Distinct().ToList();

            return new GridDataSource
            {
                Data = query.OrderBy(x => lstSortNhomDichVuKyThuat.IndexOf(x.NhomDichVuBenhVienId))
                            .ThenBy(x => x.SoThuTuXetNghiem)
                            .ThenBy(x => x.CreatedOn).ToArray(), //.Skip(queryInfo.Skip).Take(queryInfo.Take)
                TotalRowCount = query.Count
            };
        }

        public async Task<GridDataSource> GetDataForGridDichVuXetNghiemQuyTrinhCapCodeVaNhanMauAsyncVer2(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var timKiemNangCaoObj = new XacNhanCapCodeTimKiemNangCaoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<XacNhanCapCodeTimKiemNangCaoVo>(queryInfo.AdditionalSearchString);
            }
            long yeuCauTiepNhanId = string.IsNullOrEmpty(queryInfo.SearchTerms) ? 0 : long.Parse(queryInfo.SearchTerms);

            var laNhanVienKhoaXetNghiem = await KiemTraNhanVienThuocKhoaXetNghiemAsync();
            var phongHienTai = new Core.Domain.Entities.PhongBenhViens.PhongBenhVien();
            if (!laNhanVienKhoaXetNghiem)
            {
                phongHienTai = _phongBenhVienRepository.TableNoTracking
                    .First(x => x.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            }
            
            var query = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId
                            && (timKiemNangCaoObj.PhienXetNghiemId == null || x.PhienXetNghiemChiTiets.Any(a => a.PhienXetNghiemId == timKiemNangCaoObj.PhienXetNghiemId))
                            && x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                            && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                            && (x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                                x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                                x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                            && (laNhanVienKhoaXetNghiem || x.NoiChiDinh.KhoaPhongId == phongHienTai.KhoaPhongId))
                .Select(item => new DichVuXetNghiemQuyTrinhCapCodeVaNhanMauVo()
                {
                    Id = item.Id,
                    YeuCauDichVuKyThuatId = item.Id,
                    //PhienXetNghiemChiTietId = item.PhienXetNghiemChiTiets.Any() ? item.PhienXetNghiemChiTiets.Select(a => a.Id).FirstOrDefault() : (long?)null,
                    MaDichVu = item.MaDichVu,
                    TenDichVu = item.TenDichVu,
                    //Barcode = item.PhienXetNghiemChiTiets.Any() ? item.PhienXetNghiemChiTiets.Select(a => a.PhienXetNghiem.BarCodeId).FirstOrDefault() : null,
                    //BarcodeNumber = item.PhienXetNghiemChiTiets.Any() ? item.PhienXetNghiemChiTiets.Select(a => a.PhienXetNghiem.BarCodeNumber).FirstOrDefault() : (int?)null,
                    ThoiGianChiDinh = item.ThoiDiemDangKy,
                    NguoiChiDinh = item.NhanVienChiDinh.User.HoTen,
                    BenhPham = item.BenhPhamXetNghiem,
                    LoaiMau = item.DichVuKyThuatBenhVien.LoaiMauXetNghiem,
                    TenNhom = item.NhomDichVuBenhVien.Ten,
                    IsNhanVienKhoaXetNghiem = laNhanVienKhoaXetNghiem,
                    //IsChayLaiKetQua = item.PhienXetNghiemChiTiets.Any() && item.PhienXetNghiemChiTiets.OrderByDescending(a => a.Id).Select(a => a.ChayLaiKetQua ?? false).FirstOrDefault(),
                    //NgayLayMau = item.PhienXetNghiemChiTiets.Any() ? item.PhienXetNghiemChiTiets.OrderByDescending(a => a.Id).Select(a => a.ThoiDiemLayMau).FirstOrDefault() : null,
                    //NguoiLayMau = item.PhienXetNghiemChiTiets.Any() ? item.PhienXetNghiemChiTiets.OrderByDescending(a => a.Id).Select(a => a.NhanVienLayMau?.User.HoTen).FirstOrDefault() : null,
                    //NgayNhanMau = item.PhienXetNghiemChiTiets.Any() ? item.PhienXetNghiemChiTiets.OrderByDescending(a => a.Id).Select(a => a.ThoiDiemNhanMau).FirstOrDefault() : null,
                    //NguoiNhanMau = item.PhienXetNghiemChiTiets.Any() ? item.PhienXetNghiemChiTiets.OrderByDescending(a => a.Id).Select(a => a.NhanVienNhanMau?.User.HoTen).FirstOrDefault() : null,
                    //NgayDuyet = item.PhienXetNghiemChiTiets.Any() ? item.PhienXetNghiemChiTiets.OrderByDescending(a => a.Id).Select(a => a.ThoiDiemKetLuan).FirstOrDefault() : null,
                    //TenNguoiDuyet = item.PhienXetNghiemChiTiets.Any() ? item.PhienXetNghiemChiTiets.OrderByDescending(a => a.Id).Select(a => a.NhanVienKetLuan?.User.HoTen).FirstOrDefault() : null,

                    //BVHD-3372
                    PhanLoaiDichVu = timKiemNangCaoObj.TrangThai,

                    //TatCaKetQuaChuaCoGiaTri = item.PhienXetNghiemChiTiets.Any()
                    //                          && item.PhienXetNghiemChiTiets.OrderByDescending(a => a.Id)
                    //                              .Select(a => a.KetQuaXetNghiemChiTiets.Any() && a.KetQuaXetNghiemChiTiets.All(b => string.IsNullOrEmpty(b.GiaTriTuMay) && string.IsNullOrEmpty(b.GiaTriNhapTay)))
                    //                              .FirstOrDefault(),
                    SoThuTuXetNghiem = item.DichVuKyThuatBenhVien.DichVuXetNghiem.SoThuTu ?? 0,
                    NhomDichVuBenhVienId = item.NhomDichVuBenhVienId,
                    CreatedOn = item.CreatedOn
                }).ToList();

            #region get thông tin theo phiên
            var lstYeucauDichVuKyThuatId = query.Select(x => x.Id).Distinct().ToList();
            var lstPhienXetNghiemChiTiet = _phienXetNghiemChiTietRepository.TableNoTracking
                .Include(y => y.PhienXetNghiem)
                .Include(y => y.NhanVienLayMau).ThenInclude(z => z.User)
                .Include(y => y.NhanVienNhanMau).ThenInclude(z => z.User)
                .Include(y => y.NhanVienKetLuan).ThenInclude(z => z.User)
                .Where(x => lstYeucauDichVuKyThuatId.Contains(x.YeuCauDichVuKyThuatId))
                .ToList();

            foreach (var yckt in query)
            {
                var lstPhienChiTiet = lstPhienXetNghiemChiTiet.Where(x => x.YeuCauDichVuKyThuatId == yckt.Id).ToList();

                if (lstPhienChiTiet.Any())
                {
                    var phienChiTietHienTai = lstPhienChiTiet.OrderByDescending(a => a.Id).FirstOrDefault();
                    yckt.PhienChiTietCuoiCungHienTaiId = phienChiTietHienTai?.Id;

                    yckt.PhienXetNghiemChiTietId = lstPhienChiTiet.Select(a => a.Id).FirstOrDefault();
                    yckt.Barcode = lstPhienChiTiet.Select(a => a.PhienXetNghiem.BarCodeId).FirstOrDefault();
                    yckt.BarcodeNumber = lstPhienChiTiet.Select(a => a.PhienXetNghiem.BarCodeNumber).FirstOrDefault();
                    yckt.IsChayLaiKetQua = lstPhienChiTiet.OrderByDescending(a => a.Id).Select(a => a.ChayLaiKetQua ?? false).FirstOrDefault();
                    yckt.NgayLayMau = lstPhienChiTiet.OrderByDescending(a => a.Id).Select(a => a.ThoiDiemLayMau).FirstOrDefault();
                    yckt.NguoiLayMau = lstPhienChiTiet.OrderByDescending(a => a.Id).Select(a => a.NhanVienLayMau?.User?.HoTen).FirstOrDefault();
                    yckt.NgayNhanMau = lstPhienChiTiet.OrderByDescending(a => a.Id).Select(a => a.ThoiDiemNhanMau).FirstOrDefault();
                    yckt.NguoiNhanMau = lstPhienChiTiet.OrderByDescending(a => a.Id).Select(a => a.NhanVienNhanMau?.User?.HoTen).FirstOrDefault();
                    yckt.NgayDuyet = lstPhienChiTiet.OrderByDescending(a => a.Id).Select(a => a.ThoiDiemKetLuan).FirstOrDefault();
                    yckt.TenNguoiDuyet = lstPhienChiTiet.OrderByDescending(a => a.Id).Select(a => a.NhanVienLayMau?.User?.HoTen).FirstOrDefault();
                }
            }
            #endregion

            #region Xử lý kiểm tra tất cả kết quả chưa có
            var lstPhienChiTietId = query
                .Where(x => x.PhienChiTietCuoiCungHienTaiId != null)
                .Select(x => x.PhienChiTietCuoiCungHienTaiId.Value)
                .Distinct().ToList();
            var lstKetQuaXetNghiemChiTiet = _ketQuaXetNghiemChiTietRepository.TableNoTracking
                .Where(x => lstPhienChiTietId.Contains(x.PhienXetNghiemChiTietId))
                .ToList();

            foreach (var yckt in query)
            {
                if (yckt.PhienChiTietCuoiCungHienTaiId != null)
                {
                    var lstKetQua = lstKetQuaXetNghiemChiTiet
                        .Where(x => x.PhienXetNghiemChiTietId == yckt.PhienChiTietCuoiCungHienTaiId).ToList();
                    if (lstKetQua.Any())
                    {
                        yckt.TatCaKetQuaChuaCoGiaTri = lstKetQua.Any() && lstKetQua.All(b => string.IsNullOrEmpty(b.GiaTriTuMay) && string.IsNullOrEmpty(b.GiaTriNhapTay));
                    }
                }
            }
            #endregion
            
            if (timKiemNangCaoObj.TrangThai != null && (timKiemNangCaoObj.TrangThai.ChuaCapCode
                                                        || timKiemNangCaoObj.TrangThai.DaCapCode
                                                        || timKiemNangCaoObj.TrangThai.ChuaNhanMau
                                                        || timKiemNangCaoObj.TrangThai.DaNhanMau))
            {
                query = query.Where(x =>
                        (timKiemNangCaoObj.TrangThai.ChuaCapCode && x.TrangThai == Enums.TrangThaiLayMauXetNghiemNew.ChoCapCode)
                        || (timKiemNangCaoObj.TrangThai.DaCapCode && x.TrangThai != Enums.TrangThaiLayMauXetNghiemNew.ChoCapCode)
                        || (timKiemNangCaoObj.TrangThai.ChuaNhanMau && x.TrangThai == Enums.TrangThaiLayMauXetNghiemNew.ChoNhanMau)
                        || (timKiemNangCaoObj.TrangThai.DaNhanMau && (x.TrangThai == Enums.TrangThaiLayMauXetNghiemNew.ChoKetQua
                                                                      || x.TrangThai == Enums.TrangThaiLayMauXetNghiemNew.DaCoKetQua
                                                                      || x.TrangThai == Enums.TrangThaiLayMauXetNghiemNew.DaDuyet)))
                    .ToList();
            }

            var lstSortNhomDichVuKyThuat = query.OrderBy(x => x.CreatedOn)
                .Select(x => x.NhomDichVuBenhVienId).Distinct().ToList();

            return new GridDataSource
            {
                Data = query.OrderBy(x => lstSortNhomDichVuKyThuat.IndexOf(x.NhomDichVuBenhVienId))
                            .ThenBy(x => x.SoThuTuXetNghiem)
                            .ThenBy(x => x.CreatedOn).ToArray(),
                TotalRowCount = query.Count
            };
        }

        public async Task<GridDataSource> GetTotalPageForGridDichVuXetNghiemQuyTrinhCapCodeVaNhanMauAsync(QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new XacNhanCapCodeTimKiemNangCaoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<XacNhanCapCodeTimKiemNangCaoVo>(queryInfo.AdditionalSearchString);
            }
            long yeuCauTiepNhanId = string.IsNullOrEmpty(queryInfo.SearchTerms) ? 0 : long.Parse(queryInfo.SearchTerms);

            //DateTime? tuNgay = null;
            //DateTime? denNgay = null;
            ////kiểm tra tìm kiếm nâng cao
            //if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay))
            //{
            //    DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
            //    tuNgay = tuNgayTemp;
            //}

            //if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay))
            //{
            //    DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
            //    denNgay = denNgayTemp;
            //}

            var laNhanVienKhoaXetNghiem = await KiemTraNhanVienThuocKhoaXetNghiemAsync();
            var phongHienTai = new Core.Domain.Entities.PhongBenhViens.PhongBenhVien();
            if (!laNhanVienKhoaXetNghiem)
            {
                phongHienTai = _phongBenhVienRepository.TableNoTracking
                    .First(x => x.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            }

            var lstDichVu =
                _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(y => y.PhienXetNghiem)
                    .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(y => y.NhanVienLayMau).ThenInclude(z => z.User)
                    .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(y => y.NhanVienNhanMau).ThenInclude(z => z.User)
                    .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(y => y.NhanVienKetLuan).ThenInclude(z => z.User)
                    .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(y => y.KetQuaXetNghiemChiTiets)
                    .Include(x => x.DichVuKyThuatBenhVien).ThenInclude(y => y.DichVuXetNghiem)
                    .Include(x => x.NhanVienChiDinh).ThenInclude(y => y.User)
                    .Include(x => x.NhomDichVuBenhVien)
                    .Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId
                                && (timKiemNangCaoObj.PhienXetNghiemId == null || x.PhienXetNghiemChiTiets.Any(a => a.PhienXetNghiemId == timKiemNangCaoObj.PhienXetNghiemId))
                                && x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && (x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                                    x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                                    x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                                //&& (tuNgay == null || (tuNgay != null && ((x.NoiTruPhieuDieuTri == null && x.ThoiDiemChiDinh.Date >= tuNgay.Value.Date) || (x.NoiTruPhieuDieuTri != null && x.NoiTruPhieuDieuTri.NgayDieuTri >= tuNgay.Value.Date))))
                                //&& (denNgay == null || (denNgay != null && ((x.NoiTruPhieuDieuTri == null && x.ThoiDiemChiDinh.Date <= denNgay.Value.Date) || (x.NoiTruPhieuDieuTri != null && x.NoiTruPhieuDieuTri.NgayDieuTri <= denNgay.Value.Date))))
                                && (laNhanVienKhoaXetNghiem || x.NoiChiDinh.KhoaPhongId == phongHienTai.KhoaPhongId)).ToList();
            var query = lstDichVu
                .Select(item => new DichVuXetNghiemQuyTrinhCapCodeVaNhanMauVo()
                {
                    Id = item.Id,
                    YeuCauDichVuKyThuatId = item.Id,
                    PhienXetNghiemChiTietId = item.PhienXetNghiemChiTiets.Any() ? item.PhienXetNghiemChiTiets.Select(a => a.Id).FirstOrDefault() : (long?)null,
                    MaDichVu = item.MaDichVu,
                    TenDichVu = item.TenDichVu,
                    Barcode = item.PhienXetNghiemChiTiets.Any() ? item.PhienXetNghiemChiTiets.Select(a => a.PhienXetNghiem.BarCodeId).FirstOrDefault() : null,
                    BarcodeNumber = item.PhienXetNghiemChiTiets.Any() ? item.PhienXetNghiemChiTiets.Select(a => a.PhienXetNghiem.BarCodeNumber).FirstOrDefault() : (int?)null,
                    ThoiGianChiDinh = item.ThoiDiemDangKy,
                    NguoiChiDinh = item.NhanVienChiDinh.User.HoTen,
                    BenhPham = item.BenhPhamXetNghiem,
                    LoaiMau = item.DichVuKyThuatBenhVien.LoaiMauXetNghiem,
                    TenNhom = item.NhomDichVuBenhVien.Ten,
                    IsNhanVienKhoaXetNghiem = laNhanVienKhoaXetNghiem,
                    IsChayLaiKetQua = item.PhienXetNghiemChiTiets.Any() && item.PhienXetNghiemChiTiets.OrderByDescending(a => a.Id).Select(a => a.ChayLaiKetQua ?? false).FirstOrDefault(),
                    NgayLayMau = item.PhienXetNghiemChiTiets.Any() ? item.PhienXetNghiemChiTiets.OrderByDescending(a => a.Id).Select(a => a.ThoiDiemLayMau).FirstOrDefault() : null,
                    NguoiLayMau = item.PhienXetNghiemChiTiets.Any() ? item.PhienXetNghiemChiTiets.OrderByDescending(a => a.Id).Select(a => a.NhanVienLayMau?.User.HoTen).FirstOrDefault() : null,
                    NgayNhanMau = item.PhienXetNghiemChiTiets.Any() ? item.PhienXetNghiemChiTiets.OrderByDescending(a => a.Id).Select(a => a.ThoiDiemNhanMau).FirstOrDefault() : null,
                    NguoiNhanMau = item.PhienXetNghiemChiTiets.Any() ? item.PhienXetNghiemChiTiets.OrderByDescending(a => a.Id).Select(a => a.NhanVienNhanMau?.User.HoTen).FirstOrDefault() : null,
                    NgayDuyet = item.PhienXetNghiemChiTiets.Any() ? item.PhienXetNghiemChiTiets.OrderByDescending(a => a.Id).Select(a => a.ThoiDiemKetLuan).FirstOrDefault() : null,
                    TenNguoiDuyet = item.PhienXetNghiemChiTiets.Any() ? item.PhienXetNghiemChiTiets.OrderByDescending(a => a.Id).Select(a => a.NhanVienKetLuan?.User.HoTen).FirstOrDefault() : null,
                    TatCaKetQuaChuaCoGiaTri = item.PhienXetNghiemChiTiets.Any()
                                              && item.PhienXetNghiemChiTiets.OrderByDescending(a => a.Id)
                                                  .Select(a => a.KetQuaXetNghiemChiTiets.Any() && a.KetQuaXetNghiemChiTiets.All(b => string.IsNullOrEmpty(b.GiaTriTuMay) && string.IsNullOrEmpty(b.GiaTriNhapTay)))
                                                  .FirstOrDefault(),
                }).ToList();

            if (timKiemNangCaoObj.TrangThai != null && (timKiemNangCaoObj.TrangThai.ChuaCapCode
                                                        || timKiemNangCaoObj.TrangThai.DaCapCode
                                                        || timKiemNangCaoObj.TrangThai.ChuaNhanMau
                                                        || timKiemNangCaoObj.TrangThai.DaNhanMau))
            {
                //query = query.Where(x =>
                //        (timKiemNangCaoObj.TrangThai.ChuaCapCode && x.TrangThai == Enums.TrangThaiLayMauXetNghiem.ChoLayMau)
                //        || (timKiemNangCaoObj.TrangThai.DaCapCode && x.TrangThai == Enums.TrangThaiLayMauXetNghiem.ChoGuiMau)
                //        || (timKiemNangCaoObj.TrangThai.ChuaNhanMau && x.TrangThai == Enums.TrangThaiLayMauXetNghiem.ChoGuiMau)
                //        || (timKiemNangCaoObj.TrangThai.DaNhanMau && (x.TrangThai == Enums.TrangThaiLayMauXetNghiem.ChoKetQua || x.TrangThai == Enums.TrangThaiLayMauXetNghiem.DaCoKetQua)))
                //    .ToList();

                query = query.Where(x =>
                        (timKiemNangCaoObj.TrangThai.ChuaCapCode && x.TrangThai == Enums.TrangThaiLayMauXetNghiemNew.ChoCapCode)
                        || (timKiemNangCaoObj.TrangThai.DaCapCode && x.TrangThai != Enums.TrangThaiLayMauXetNghiemNew.ChoCapCode) //x.TrangThai == Enums.TrangThaiLayMauXetNghiemNew.ChoNhanMau)
                        || (timKiemNangCaoObj.TrangThai.ChuaNhanMau && x.TrangThai == Enums.TrangThaiLayMauXetNghiemNew.ChoNhanMau)
                        || (timKiemNangCaoObj.TrangThai.DaNhanMau && (x.TrangThai == Enums.TrangThaiLayMauXetNghiemNew.ChoKetQua
                                                                      || x.TrangThai == Enums.TrangThaiLayMauXetNghiemNew.DaCoKetQua
                                                                      || x.TrangThai == Enums.TrangThaiLayMauXetNghiemNew.DaDuyet)))
                    .ToList();
            }

            return new GridDataSource { TotalRowCount = query.Count };
        }
        #endregion

        #region get data
        public async Task<List<LookupItemTemplateVo>> GetListHopDongKhamSucKhoeHieuLucAsync(DropDownListRequestModel model)
        {
            var lstHopDongDefault = new List<LookupItemTemplateVo>();
            lstHopDongDefault.Add(new LookupItemTemplateVo()
            {
                KeyId = 0,
                DisplayName = "Tất cả",
                Ten = "Tất cả"
            });
            var lstHopDong =
                lstHopDongDefault
                    .Union(
                        await _hopDongKhamSucKhoeRepository.TableNoTracking
                            .Where(x => !x.DaKetThuc && x.NgayHieuLuc.Date <= DateTime.Now.Date && (x.NgayKetThuc == null || x.NgayKetThuc.Value.Date >= DateTime.Now.Date))
                            .OrderByDescending(x => x.Id == model.Id).ThenBy(x => x.SoHopDong)
                            .Select(item => new LookupItemTemplateVo()
                            {
                                KeyId = item.Id,
                                DisplayName = item.SoHopDong,
                                //Ma = item.Ma,
                                Ten = item.SoHopDong
                            })
                            .ApplyLike(model.Query, x => x.Ten, x => x.Ma)
                            .Take(model.Take).ToListAsync())
                    .ToList();
            return lstHopDong;
        }

        public async Task<ThongTinBenhNhanXetNghiemVo> GetThongTinBenhNhanXetNghiemAsync(BenhNhanXetNghiemQueryVo query)
        {
            var result = new ThongTinBenhNhanXetNghiemVo();
            var cauHinhTuDongBarcode = _cauHinhService.GetSetting("CauHinhXetNghiem.TaoBarcodeTuDong");

            //03/11: bỏ await
            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.GetById(query.YeuCauTiepNhanId,
                a => a.Include(x => x.BenhNhan)
                .Include(x => x.DanToc)
                .Include(x => x.NgheNghiep)
                .Include(x => x.PhienXetNghiems)
                .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(x => x.HopDongKhamSucKhoe).ThenInclude(x => x.CongTyKhamSucKhoe)
                .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.PhienXetNghiemChiTiets)
                
                //BVHD-3800
                .Include(x => x.YeuCauNhapVien).ThenInclude(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauTiepNhan)
                );

            if (yeuCauTiepNhan != null)
            {
                result.YeuCauTiepNhanId = yeuCauTiepNhan.Id;
                result.BenhNhanId = yeuCauTiepNhan.BenhNhanId;
                result.MaBenhNhan = yeuCauTiepNhan.BenhNhan.MaBN;
                result.MaTiepNhan = yeuCauTiepNhan.MaYeuCauTiepNhan;
                result.HoTen = yeuCauTiepNhan.HoTen;
                result.NgaySinh = yeuCauTiepNhan.NgaySinh;
                result.ThangSinh = yeuCauTiepNhan.ThangSinh;
                result.NamSinh = yeuCauTiepNhan.NamSinh;
                result.GioiTinh = yeuCauTiepNhan.GioiTinh;
                result.Tuyen = yeuCauTiepNhan.LyDoVaoVien.GetDescription();
                result.MucHuong = yeuCauTiepNhan.BHYTMucHuong;
                result.DanToc = yeuCauTiepNhan.DanToc != null ? yeuCauTiepNhan.DanToc.Ten : "";
                result.DiaChi = yeuCauTiepNhan.DiaChiDayDu;
                result.NgheNghiep = yeuCauTiepNhan.NgheNghiep != null ? yeuCauTiepNhan.NgheNghiep.Ten : "";
                result.SoTheBHYT = yeuCauTiepNhan.BHYTMaSoThe;
                result.SoDienThoai = yeuCauTiepNhan.SoDienThoaiDisplay ?? yeuCauTiepNhan.NguoiLienHeSoDienThoai?.ApplyFormatPhone();
                result.IsTraKetQua = yeuCauTiepNhan.PhienXetNghiems.Any() && yeuCauTiepNhan.PhienXetNghiems.All(a => a.DaTraKetQua == true);
                result.IsAutoBarcode = Boolean.Parse(cauHinhTuDongBarcode.Value);
                result.IsCoDuKetQua = yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(x => x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                                                        && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                                        && (x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan
                                                                            || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan
                                                                            || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
                                                            .All(x => x.PhienXetNghiemChiTiets.Any())
                                   && yeuCauTiepNhan.PhienXetNghiems.Any()
                                   && yeuCauTiepNhan.PhienXetNghiems.All(x => x.ThoiDiemKetLuan != null);
                result.IsCoPhienChiTietCoKetQua = yeuCauTiepNhan.YeuCauDichVuKyThuats.Any(x => x.PhienXetNghiemChiTiets.Any())
                                               && yeuCauTiepNhan.YeuCauDichVuKyThuats.Any(x => x.PhienXetNghiemChiTiets.OrderByDescending(a => a.Id).Select(a => a.NhanVienKetLuanId).FirstOrDefault() != null);
                //BVHD-3364
                result.TenCongTy = yeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe ? yeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten : null;
                result.LoaiYeuCauTiepNhan = yeuCauTiepNhan.LoaiYeuCauTiepNhan;
                result.CoBHYT = yeuCauTiepNhan.CoBHYT;

                // bổ sung xử lý gt số lượng thêm theo user login
                var lstSoLuongIn = GetSoLuongInThemTheoUser();
                var soLuongInThemTheoUser = lstSoLuongIn.Where(x => x.UserId == _userAgentHelper.GetCurrentUserId()).FirstOrDefault();
                if (soLuongInThemTheoUser != null)
                {
                    result.SoLuongThem = soLuongInThemTheoUser.SoLuong;
                }
                if (query.PhienXetNghiemId != null)
                {
                    var phienXetNghiemHienTai = yeuCauTiepNhan.PhienXetNghiems.FirstOrDefault(x => x.Id == query.PhienXetNghiemId);
                    if (phienXetNghiemHienTai != null)
                    {
                        result.BarcodeNumber = phienXetNghiemHienTai.BarCodeNumber;
                        result.Barcode = phienXetNghiemHienTai.BarCodeId;
                    }
                }

                //BVHD-3800
                result.LaCapCuu = yeuCauTiepNhan.LaCapCuu ?? yeuCauTiepNhan.YeuCauNhapVien?.YeuCauKhamBenh?.YeuCauTiepNhan?.LaCapCuu;

                //BVHD-3941
                result.CoBaoHiemTuNhan = yeuCauTiepNhan.CoBHTN;
            }

            //.Select(item => new ThongTinBenhNhanXetNghiemVo()
            //{
            //    YeuCauTiepNhanId = item.Id,
            //    BenhNhanId = item.BenhNhanId,
            //    MaBenhNhan = item.BenhNhan.MaBN,
            //    MaTiepNhan = item.MaYeuCauTiepNhan,
            //    HoTen = item.HoTen,
            //    NgaySinh = item.NgaySinh,
            //    ThangSinh = item.ThangSinh,
            //    NamSinh = item.NamSinh,
            //    GioiTinh = item.GioiTinh,
            //    Tuyen = item.LyDoVaoVien.GetDescription(),
            //    MucHuong = item.BHYTMucHuong,
            //    DanToc = item.DanToc != null ? item.DanToc.Ten : "",
            //    DiaChi = item.DiaChiDayDu,
            //    NgheNghiep = item.NgheNghiep != null ? item.NgheNghiep.Ten : "",
            //    SoTheBHYT = item.BHYTMaSoThe,
            //    SoDienThoai = item.SoDienThoaiDisplay,
            //    IsTraKetQua = item.PhienXetNghiems.Any() && item.PhienXetNghiems.All(a => a.DaTraKetQua == true),
            //    IsAutoBarcode = Boolean.Parse(cauHinhTuDongBarcode.Value),
            //    IsCoDuKetQua = item.YeuCauDichVuKyThuats.Where(x => x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
            //                                                        && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
            //                                                        && (x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan
            //                                                            || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan
            //                                                            || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
            //                                            .All(x => x.PhienXetNghiemChiTiets.Any())
            //                   && item.PhienXetNghiems.Any()
            //                   && item.PhienXetNghiems.All(x => x.ThoiDiemKetLuan != null),
            //    IsCoPhienChiTietCoKetQua = item.YeuCauDichVuKyThuats.Any(x => x.PhienXetNghiemChiTiets.Any())
            //                               && item.YeuCauDichVuKyThuats.Any(x => x.PhienXetNghiemChiTiets.OrderByDescending(a => a.Id).Select(a => a.NhanVienKetLuanId).FirstOrDefault() != null),
            //    //BVHD-3364
            //    TenCongTy = item.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe ? item.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten : null,
            //    LoaiYeuCauTiepNhan = item.LoaiYeuCauTiepNhan,
            //    CoBHYT = item.CoBHYT
            //}).FirstOrDefaultAsync();
            return result;
        }

        public async Task<List<PhienXetNghiem>> GetChiTietPhienXetNghiemsAsync(List<long> phienXetNghiemIds)
        {
            var lstPhienXetNghiem = BaseRepository.TableNoTracking
                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(x => x.HopDongKhamSucKhoe).ThenInclude(x => x.CongTyKhamSucKhoe)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(x => x.HopDongKhamSucKhoe).ThenInclude(x => x.HopDongKhamSucKhoeDiaDiems)
                .Include(x => x.MauXetNghiems)
                .Where(x => phienXetNghiemIds.Contains(x.Id))
                .ToList();
            return lstPhienXetNghiem;
        }
        #endregion

        #region Xử lý data

        public async Task KiemTraDichVuCanHuyCapCodeAsync(List<long> yeuCauDichVuKyThuatIds)
        {
            var yeuCauDichVuKyThuats = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Include(x => x.PhienXetNghiemChiTiets)
                .Where(x => yeuCauDichVuKyThuatIds.Contains(x.Id)).ToList();
            if (yeuCauDichVuKyThuats.Any(x => x.PhienXetNghiemChiTiets.OrderByDescending(a => a.Id).Select(a => a.ThoiDiemLayMau).FirstOrDefault() == null))
            {
                throw new Exception(_localizationService.GetResource("GridDaCapCode.DichVuXetNghiem.ChuaCapCode"));
            }

            if (yeuCauDichVuKyThuats.Any(x => x.PhienXetNghiemChiTiets.OrderByDescending(a => a.Id).Select(a => a.ThoiDiemNhanMau).FirstOrDefault() != null))
            {
                throw new Exception(_localizationService.GetResource("GridDaCapCode.DichVuXetNghiem.DaNhanMau"));
            }
        }

        public List<ThongTinSoLuongInThemTheoTaiKhoanVo> GetSoLuongInThemTheoUser()
        {
            var lstSoLuongIn = new List<ThongTinSoLuongInThemTheoTaiKhoanVo>();
            if (!System.IO.File.Exists(pathFileSoLuongInThem))
            {
                FileStream fs1 = new FileStream(pathFileSoLuongInThem, FileMode.Create, FileAccess.ReadWrite);
                fs1.Close();
            }
            var value = System.IO.File.ReadAllText(pathFileSoLuongInThem);
            if (!string.IsNullOrEmpty(value))
            {
                lstSoLuongIn = JsonConvert.DeserializeObject<List<ThongTinSoLuongInThemTheoTaiKhoanVo>>(value);
            }

            return lstSoLuongIn;
        }

        public async Task XuLyCapNhatSoLuongInThemTheoUserAsync(int? soLuongInThem)
        {
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var lstSoLuongIn = new List<ThongTinSoLuongInThemTheoTaiKhoanVo>();

            //var path = @"Resource\\SoLuongInThemBarcodeTheoTaiKhoan.json";
            if (!System.IO.File.Exists(pathFileSoLuongInThem))
            {
                FileStream fs1 = new FileStream(pathFileSoLuongInThem, FileMode.Create, FileAccess.ReadWrite);
                fs1.Close();
            }

            lstSoLuongIn = GetSoLuongInThemTheoUser();
            var soLuongInTheoTaiKhoanHienTai = lstSoLuongIn.Where(x => x.UserId == currentUserId).FirstOrDefault();
            if (soLuongInTheoTaiKhoanHienTai == null)
            {
                if (soLuongInThem != null)
                {
                    var newSoLuong = new ThongTinSoLuongInThemTheoTaiKhoanVo()
                    {
                        UserId = currentUserId,
                        SoLuong = soLuongInThem.Value
                    };
                    lstSoLuongIn.Add(newSoLuong);
                }
            }
            else
            {
                if (soLuongInThem != null)
                {
                    soLuongInTheoTaiKhoanHienTai.SoLuong = soLuongInThem.Value;
                }
                else
                {
                    lstSoLuongIn = lstSoLuongIn.Where(x => x.UserId != currentUserId).ToList();
                }
            }

            System.IO.File.WriteAllText(pathFileSoLuongInThem, JsonConvert.SerializeObject(lstSoLuongIn));
        }

        public async Task KiemTraDichVuCanHuyNhanMauAsync(List<long> yeuCauDichVuKyThuatIds)
        {
            var yeuCauDichVuKyThuats = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets)
                .Where(x => yeuCauDichVuKyThuatIds.Contains(x.Id)).ToList();

            if (yeuCauDichVuKyThuats.Any(x => x.PhienXetNghiemChiTiets.OrderByDescending(a => a.Id).Select(a => a.ThoiDiemNhanMau).FirstOrDefault() == null))
            {
                throw new Exception(_localizationService.GetResource("GridDaCapCode.DichVuXetNghiem.ChuaNhanMau"));
            }

            if (yeuCauDichVuKyThuats.Any(x => x.PhienXetNghiemChiTiets.OrderByDescending(a => a.Id)
                                                .SelectMany(a => a.KetQuaXetNghiemChiTiets)
                                                .Any(a => a.DaDuyet == true || !string.IsNullOrEmpty(a.GiaTriTuMay))))
            {
                throw new Exception(_localizationService.GetResource("GridDaCapCode.DichVuXetNghiem.DaCoKetQuaHoacDaDuyet"));
            }
        }

        public async Task XuLyXacNhanHuyNhanMauTheoDichVuAsync(XacNhanNhanMauChoDichVuVo xacNhanNhanMauVo)
        {
            var phienXetNghiems = BaseRepository.Table
                .Include(x => x.MauXetNghiems).ThenInclude(x => x.PhieuGoiMauXetNghiem).ThenInclude(t => t.MauXetNghiems)
                .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(y => y.KetQuaXetNghiemChiTiets)
                .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(y => y.YeuCauDichVuKyThuat)
                .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(y => y.DichVuKyThuatBenhVien)
                .Where(x => x.YeuCauTiepNhanId == xacNhanNhanMauVo.YeuCauTiepNhanId
                            && x.PhienXetNghiemChiTiets.Any(a => xacNhanNhanMauVo.YeuCauDichVuKyThuatIds.Contains(a.YeuCauDichVuKyThuatId)))
                .ToList();

            if (phienXetNghiems.Any(x => x.ThoiDiemKetLuan != null))
            {
                throw new Exception(_localizationService.GetResource("LayMauXetNghiem.HuyMau.MauDaCoKetQuaHoacDangChayLai"));
            }

            foreach (var phienXetNghiem in phienXetNghiems)
            {
                var phienXetNghiemChiTiets = phienXetNghiem.PhienXetNghiemChiTiets
                    .Where(x => xacNhanNhanMauVo.YeuCauDichVuKyThuatIds.Any(a => a == x.YeuCauDichVuKyThuatId)).ToList();
                if (phienXetNghiemChiTiets.Any(x => x.ChayLaiKetQua == true))
                {
                    throw new Exception(_localizationService.GetResource("GridDaCapCode.DichVuXetNghiem.DangChayLai"));
                }
                foreach (var phienChiTiet in phienXetNghiemChiTiets)
                {
                    phienChiTiet.ThoiDiemNhanMau = null;
                    phienChiTiet.NhanVienNhanMauId = null;
                    phienChiTiet.PhongNhanMauId = null;

                    foreach (var ketQua in phienChiTiet.KetQuaXetNghiemChiTiets)
                    {
                        ketQua.WillDelete = true;
                    }

                    if (!phienXetNghiem.PhienXetNghiemChiTiets.Any(x => x.ThoiDiemNhanMau != null
                                                                       && x.NhomDichVuBenhVienId == phienChiTiet.NhomDichVuBenhVienId
                                                                       && x.DichVuKyThuatBenhVien.LoaiMauXetNghiem == phienChiTiet.DichVuKyThuatBenhVien.LoaiMauXetNghiem))
                    {
                        var mauXetNghiems = phienXetNghiem.MauXetNghiems.Where(x => x.NhomDichVuBenhVienId == phienChiTiet.NhomDichVuBenhVienId
                                                                                    && x.LoaiMauXetNghiem == phienChiTiet.DichVuKyThuatBenhVien.LoaiMauXetNghiem
                                                                                    && x.BarCodeId == phienXetNghiem.BarCodeId
                                                                                    && x.DatChatLuong != false).ToList();
                        foreach (var mauXetNghiem in mauXetNghiems)
                        {
                            mauXetNghiem.PhieuGoiMauXetNghiem.WillDelete = true;
                            mauXetNghiem.ThoiDiemNhanMau = null;
                            mauXetNghiem.NhanVienNhanMauId = null;
                            mauXetNghiem.PhongNhanMauId = null;
                        }

                        var phieuGuiMaus = phienXetNghiem.MauXetNghiems
                            .Where(x => x.PhieuGoiMauXetNghiem != null)
                            .Select(x => x.PhieuGoiMauXetNghiem)
                            .Where(x => x.MauXetNghiems.Any(y => mauXetNghiems.Any(t => t.Id == y.Id))).ToList();
                        foreach (var phieuGuiMau in phieuGuiMaus)
                        {
                            if (phieuGuiMau.MauXetNghiems.All(x => mauXetNghiems.Any(t => t.Id == x.Id)))
                            {
                                phieuGuiMau.WillDelete = true;
                            }
                        }
                    }

                }
            }

            await BaseRepository.Context.SaveChangesAsync();
        }
        #endregion

        private BenhNhanChuaCapBarcode GanThongTin(BenhNhanChuaCapBarcode model, ExcelWorksheet workSheet, int i)
        {
            model.TenCongTy = workSheet.Cells[i, 2].Text;
            model.SoHopDong = workSheet.Cells[i, 3].Text;
            model.MaBN = workSheet.Cells[i, 4].Text;
            model.MaTN = workSheet.Cells[i, 5].Text;
            model.BarcodeNumberInput = workSheet.Cells[i, 6].Text;
            model.HoTen = workSheet.Cells[i, 7].Text;
            model.GioiTinhDisplay = workSheet.Cells[i, 8].Text;
            model.NamSinhDisplay = workSheet.Cells[i, 9].Text;
            //BVHD-3836
            model.ThoiGianLayMauDisplay = workSheet.Cells[i, 10].Text;
            model.NhanVienLayMauIdDisplay = workSheet.Cells[i, 11].Text;

            if (!string.IsNullOrEmpty(model.BarcodeNumberInput))
            {
                var newPreBarcode = DateTime.Now.ToString("ddMMyy"); //yyMMdd
                var maBarcodeFormat = string.Empty;
                switch (model.BarcodeNumberInput.Length)
                {
                    case 1:
                        maBarcodeFormat = "000" + model.BarcodeNumberInput;
                        break;
                    case 2:
                        maBarcodeFormat = "00" + model.BarcodeNumberInput;
                        break;
                    case 3:
                        maBarcodeFormat = "0" + model.BarcodeNumberInput;
                        break;
                    case 4:
                        maBarcodeFormat = model.BarcodeNumberInput;
                        break;
                }
                model.BarcodeId = newPreBarcode + maBarcodeFormat;
                if (Int32.TryParse(model.BarcodeNumberInput, out int value))
                {
                    model.BarcodeNumber = value;
                }
            }
           
            return model;
        }
        #region ImportNguoiBenhChuaCapBarcodeXetNghiem

        public async Task<List<BenhNhanChuaCapBarcode>> ImportNguoiBenhChuaCapBarcodeXetNghiem(Stream path)
        {
            var lstError = new List<BenhNhanChuaCapBarcode>();
            // get list Nhân viên
           
            using (ExcelPackage package = new ExcelPackage(path))
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets["DS Người Bệnh Chưa Cấp Barcode"];
                if (workSheet == null)
                {
                    throw new Exception("Thông tin file nhập barcode không đúng");
                }
                int totalRows = workSheet.Dimension.Rows;// dòng có data
                if (totalRows >= 3)// dòng 5 bắt đầu có data
                {
                    var benhNhanChuaCapBarcodes = new List<BenhNhanChuaCapBarcode>();
                    for (int i = 5; i <= totalRows + 2; i++)
                    {
                        // ...Cells[i, 4] => i = Row, 4 = Column
                        var benhNhanChuaCapBarcode = new BenhNhanChuaCapBarcode();
                        benhNhanChuaCapBarcode = GanThongTin(benhNhanChuaCapBarcode, workSheet, i);
                        //kiểm tra mã TN đúng format
                        if (string.IsNullOrEmpty(benhNhanChuaCapBarcode.MaTN)
                            || (!string.IsNullOrEmpty(benhNhanChuaCapBarcode.MaTN) && (!NumberHelper.IsNumeric(benhNhanChuaCapBarcode.MaTN, Enums.EnumNumber.Integer) || benhNhanChuaCapBarcode.MaTN.Length != 10))
                          )
                        {
                            var errorMess = "";
                            benhNhanChuaCapBarcode.IsError = true;
                            if (string.IsNullOrEmpty(benhNhanChuaCapBarcode.MaTN))
                            {
                                errorMess = "Mã TN chưa nhập.";
                            }
                            else
                            {
                                errorMess = "Mã TN không đúng định dạng.";
                            }

                            var error = new BenhNhanChuaCapBarcode();
                            error = GanThongTin(error, workSheet, i);
                            error.IsError = true;
                            error.Error = errorMess;
                            lstError.Add(error);
                            benhNhanChuaCapBarcodes.Add(benhNhanChuaCapBarcode);
                        }
                        else
                        {
                            //kiểm tra MaTN trùng nếu có cố tình thêm 1 item mới trùng với item đã có
                            if (benhNhanChuaCapBarcodes.Any(z => z.MaTN == benhNhanChuaCapBarcode.MaTN))
                            {
                                benhNhanChuaCapBarcode.IsError = true;
                                var errorMess = "Mã TN đã tồn tại";
                                var error = new BenhNhanChuaCapBarcode();
                                error = GanThongTin(error, workSheet, i);
                                error.IsError = true;
                                error.Error = errorMess;
                                lstError.Add(error);
                                benhNhanChuaCapBarcodes.Add(benhNhanChuaCapBarcode);
                            }
                            else
                            {
                                //kiểm tra mã BN đúng format

                                if (string.IsNullOrEmpty(benhNhanChuaCapBarcode.MaBN)
                                || (!string.IsNullOrEmpty(benhNhanChuaCapBarcode.MaBN) && (!NumberHelper.IsNumeric(benhNhanChuaCapBarcode.MaBN, Enums.EnumNumber.Integer) || benhNhanChuaCapBarcode.MaBN.Length != 8)))
                                {
                                    var errorMess = "";
                                    benhNhanChuaCapBarcode.IsError = true;

                                    if (string.IsNullOrEmpty(benhNhanChuaCapBarcode.MaBN))
                                    {
                                        errorMess = "Mã BN chưa nhập.";
                                    }
                                    else
                                    {
                                        errorMess = "Mã BN không đúng định dạng.";
                                    }
                                    var error = new BenhNhanChuaCapBarcode();
                                    error = GanThongTin(error, workSheet, i);
                                    error.IsError = true;
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    benhNhanChuaCapBarcodes.Add(benhNhanChuaCapBarcode);
                                }
                                else
                                {
                                    ////kiểm tra MaBN trùng nếu có cố tình thêm 1 item mới trùng với item đã có
                                    //if (benhNhanChuaCapBarcodes.Any(z => z.MaBN == benhNhanChuaCapBarcode.MaBN))
                                    //{
                                    //    benhNhanChuaCapBarcode.IsError = true;
                                    //    var errorMess = "Mã BN đã tồn tại";
                                    //    var error = new BenhNhanChuaCapBarcode();
                                    //    error = GanThongTin(error, workSheet, i);
                                    //    error.IsError = true;
                                    //    error.Error = errorMess;
                                    //    lstError.Add(error);
                                    //    benhNhanChuaCapBarcodes.Add(benhNhanChuaCapBarcode);
                                    //}
                                    //else
                                    //{

                                    //}

                                    //kiểm tra có nhập mã barcode và định dạng
                                    if (string.IsNullOrEmpty(benhNhanChuaCapBarcode.BarcodeNumberInput)
                                        || (!string.IsNullOrEmpty(benhNhanChuaCapBarcode.BarcodeNumberInput) && (!NumberHelper.IsNumeric(benhNhanChuaCapBarcode.BarcodeNumberInput, Enums.EnumNumber.Integer) || benhNhanChuaCapBarcode.BarcodeNumberInput.Length > 4)))
                                    {
                                        var errorMess = "";
                                        benhNhanChuaCapBarcode.IsError = true;

                                        if (string.IsNullOrEmpty(benhNhanChuaCapBarcode.BarcodeNumberInput))
                                        {
                                            errorMess = "Mã barcode chưa nhập.";
                                        }
                                        else
                                        {
                                            errorMess = "Mã barcode không đúng định dạng (tối đa 4 số).";
                                        }
                                        var error = new BenhNhanChuaCapBarcode();
                                        error = GanThongTin(error, workSheet, i);
                                        error.IsError = true;
                                        error.Error = errorMess;
                                        lstError.Add(error);
                                        benhNhanChuaCapBarcodes.Add(benhNhanChuaCapBarcode);
                                    }
                                    else
                                    {
                                        //kiểm tra barcode trùng với bệnh nhân khác
                                        if (benhNhanChuaCapBarcodes.Any(z => z.BarcodeNumberInput == benhNhanChuaCapBarcode.BarcodeNumberInput))
                                        {
                                            benhNhanChuaCapBarcode.IsError = true;
                                            var errorMess = "Mã barcode đã tồn tại.";
                                            var error = new BenhNhanChuaCapBarcode();
                                            error = GanThongTin(error, workSheet, i);
                                            error.IsError = true;
                                            error.Error = errorMess;
                                            lstError.Add(error);
                                            benhNhanChuaCapBarcodes.Add(benhNhanChuaCapBarcode);
                                        }
                                        else
                                        {
                                            benhNhanChuaCapBarcode.IsError = false;
                                            benhNhanChuaCapBarcodes.Add(benhNhanChuaCapBarcode);
                                        }
                                    }
                                }
                            }
                        }
                        #region BVHD-3836 
                        // kiểm tra nhân viên lấy mẫu nhập đúng định dạng
                        if (!string.IsNullOrEmpty(benhNhanChuaCapBarcode.NhanVienLayMauIdDisplay))
                        {
                            var kiemTraStringHopLe = isEmail(benhNhanChuaCapBarcode.NhanVienLayMauIdDisplay);
                            if(kiemTraStringHopLe)
                            {
                                var nhanVienLayMauId = GetNhanVienId(benhNhanChuaCapBarcode.NhanVienLayMauIdDisplay);
                                if (nhanVienLayMauId == 0 || nhanVienLayMauId == null)
                                {
                                    var error = new BenhNhanChuaCapBarcode();
                                    error = GanThongTin(error, workSheet, i);
                                    error.IsError = true;
                                    var errorMess = "Nhân viên lấy mẫu không tồn tại.";
                                    error.Error = errorMess;
                                    lstError.Add(error);
                                    benhNhanChuaCapBarcodes.Add(benhNhanChuaCapBarcode);
                                }
                                else
                                {
                                    benhNhanChuaCapBarcode.NhanVienLayMauId = GetNhanVienId(benhNhanChuaCapBarcode.NhanVienLayMauIdDisplay);
                                }
                                
                            }
                            else
                            {
                                var error = new BenhNhanChuaCapBarcode();
                                error = GanThongTin(error, workSheet, i);
                                error.IsError = true;
                                var errorMess = "Nhân viên lấy mẫu không đúng định dạng(Tên nhân viên  + chữ cái họ + chữ cái tên lót + @benhvienbacha.vn).";
                                error.Error = errorMess;
                                lstError.Add(error);
                                benhNhanChuaCapBarcodes.Add(benhNhanChuaCapBarcode);
                            }
                        }
                        // kiểm tra thời gian lấy mẫu nhập đúng định dạng
                        if (!string.IsNullOrEmpty(benhNhanChuaCapBarcode.ThoiGianLayMauDisplay))
                        {

                            DateTime result;
                            var format = "g";
                            var provider = new CultureInfo("fr-FR");
                            try
                            {
                                benhNhanChuaCapBarcode.ThoiGianLayMau = DateTime.ParseExact(benhNhanChuaCapBarcode.ThoiGianLayMauDisplay, format, provider);
                            }
                            catch (FormatException)
                            {
                                var error = new BenhNhanChuaCapBarcode();
                                error = GanThongTin(error, workSheet, i);
                                error.IsError = true;
                                var errorMess = "Thời gian lấy mẫu không đúng định dạng.";
                                error.Error = errorMess;
                                lstError.Add(error);
                                benhNhanChuaCapBarcodes.Add(benhNhanChuaCapBarcode);
                            }
                        }
                        #endregion

                    }
                    var benhNhanChuaCapBarcodesThoaDieuKien = benhNhanChuaCapBarcodes.Where(z => !z.IsError).ToList();

                    //lst barcode tất cả bệnh nhân
                    var maTNExcels = benhNhanChuaCapBarcodesThoaDieuKien.Select(z => z.MaTN).ToList();

                    var yeuCauTiepNhanIds = _yeuCauDichVuKyThuatRepository.TableNoTracking
                      .Where(ycdvkt => maTNExcels.Contains(ycdvkt.YeuCauTiepNhan.MaYeuCauTiepNhan)
                      && ycdvkt.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                      && ycdvkt.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                      && (ycdvkt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan
                                                                    || ycdvkt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan
                                                                    || ycdvkt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                      ).Select(c => c.YeuCauTiepNhanId).Distinct().ToList();

                    //update 24/08/2022 load YeuCauDichVuKyThuats sau
                    var yeuCauTiepNhans = _yeuCauTiepNhanRepository.Table
                                                              //.Include(x => x.PhienXetNghiems).ThenInclude(y => y.PhienXetNghiemChiTiets)
                                                              .Include(x => x.PhienXetNghiems).ThenInclude(y => y.MauXetNghiems)
                                                              //.Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.DichVuKyThuatBenhVien)
                                                              //.Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.PhienXetNghiemChiTiets)
                                                              .Where(x => yeuCauTiepNhanIds.Contains(x.Id)
                                                              ).OrderBy(z => z.Id).ThenBy(z => z.ThoiDiemTiepNhan).ToList();

                    var yeuCauDichVuKyThuatAll = _yeuCauDichVuKyThuatRepository.Table
                                        .Where(ycdvkt => yeuCauTiepNhanIds.Contains(ycdvkt.YeuCauTiepNhanId) && 
                                                        ycdvkt.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                                        && ycdvkt.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                        && (ycdvkt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                                            || ycdvkt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan
                                                            || ycdvkt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan))
                                        .Include(p => p.DichVuKyThuatBenhVien).Include(p => p.PhienXetNghiemChiTiets).ToList();

                    var barCodeExcels = benhNhanChuaCapBarcodesThoaDieuKien.Select(c => c.BarcodeId).Distinct().ToList();

                    var benhNhanDaCapBarcodeVos = _phienXetNghiemRepository.TableNoTracking
                                 .Where(x => barCodeExcels.Contains(x.BarCodeId) && x.ThoiDiemBatDau.Date == DateTime.Now.Date)
                                .Select(s => new BenhNhanChuaCapBarcodeLookupVo
                                {
                                    MaTN = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                    BarCodeId = s.BarCodeId,
                                    BarCodeNumber = s.BarCodeNumber
                                }).Distinct().ToList();

                    foreach (var benhNhanThoaDieuKien in benhNhanChuaCapBarcodesThoaDieuKien)
                    {
                        //check trùng mã bn với barcode trên cùng 1 dòng
                        //query benhNhanChuaCapBarcode.Mã TN != lst. mã TN và benhNhanChuaCapBarcode.Mã barcode (full) == lst.Mã barcode
                        var kiemTraTrungBarcode = benhNhanDaCapBarcodeVos.Any(z => z.MaTN != benhNhanThoaDieuKien.MaTN && z.BarCodeId == benhNhanThoaDieuKien.BarcodeId);
                        //if trung ma thi add lstError
                        if (kiemTraTrungBarcode)
                        {
                            var error = new BenhNhanChuaCapBarcode
                            {
                                TenCongTy = benhNhanThoaDieuKien.TenCongTy,
                                SoHopDong = benhNhanThoaDieuKien.SoHopDong,
                                MaBN = benhNhanThoaDieuKien.MaBN,
                                MaTN = benhNhanThoaDieuKien.MaTN,
                                BarcodeNumber = benhNhanThoaDieuKien.BarcodeNumber,
                                BarcodeId = benhNhanThoaDieuKien.BarcodeId,
                                HoTen = benhNhanThoaDieuKien.HoTen,
                                GioiTinhDisplay = benhNhanThoaDieuKien.GioiTinhDisplay,
                                NamSinhDisplay = benhNhanThoaDieuKien.NamSinhDisplay,
                                IsError = true,
                                Error = string.Format("Mã barcode đã tồn tại."),
                                //BVHD-3836
                                NhanVienLayMauIdDisplay = benhNhanThoaDieuKien.NhanVienLayMauIdDisplay,
                                ThoiGianLayMauDisplay = benhNhanThoaDieuKien.ThoiGianLayMauDisplay,
                            };
                            lstError.Add(error);
                        }
                        //else cap code
                        else
                        {
                            //var yeuCauTiepNhanKhacCuaBenhNhan = yeuCauTiepNhanKhacCuaBenhNhans.Where(z => z.MaYeuCauTiepNhan == benhNhanThoaDieuKien.MaTN).FirstOrDefault();
                            if (!yeuCauTiepNhans.Any(z => z.MaYeuCauTiepNhan == benhNhanThoaDieuKien.MaTN))
                            {
                                var error = new BenhNhanChuaCapBarcode
                                {
                                    TenCongTy = benhNhanThoaDieuKien.TenCongTy,
                                    SoHopDong = benhNhanThoaDieuKien.SoHopDong,
                                    MaBN = benhNhanThoaDieuKien.MaBN,
                                    MaTN = benhNhanThoaDieuKien.MaTN,
                                    BarcodeNumber = benhNhanThoaDieuKien.BarcodeNumber,
                                    BarcodeId = benhNhanThoaDieuKien.BarcodeId,
                                    HoTen = benhNhanThoaDieuKien.HoTen,
                                    GioiTinhDisplay = benhNhanThoaDieuKien.GioiTinhDisplay,
                                    NamSinhDisplay = benhNhanThoaDieuKien.NamSinhDisplay,
                                    IsError = true,
                                    Error = string.Format("Mã TN không tồn tại."),
                                    //BVHD-3836
                                    NhanVienLayMauIdDisplay = benhNhanThoaDieuKien.NhanVienLayMauIdDisplay,
                                    ThoiGianLayMauDisplay = benhNhanThoaDieuKien.ThoiGianLayMauDisplay
                                };
                                lstError.Add(error);
                            }
                            else
                            {
                                foreach (var yeuCauTiepNhan in yeuCauTiepNhans.Where(z => z.MaYeuCauTiepNhan == benhNhanThoaDieuKien.MaTN
                                                                  && z.YeuCauDichVuKyThuats.Any(ycdvkt => ycdvkt.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                                                  && ycdvkt.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                                  && (ycdvkt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan
                                                                      || ycdvkt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan
                                                                      || ycdvkt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
                                                                  )
                                    )
                                {
                                    var error = new BenhNhanChuaCapBarcode();
                                    if (!string.IsNullOrEmpty(benhNhanThoaDieuKien.BarcodeNumberInput))
                                    {
                                        if (Int32.TryParse(benhNhanThoaDieuKien.BarcodeNumberInput, out int value))
                                        {
                                            benhNhanThoaDieuKien.BarcodeNumber = value;
                                        }
                                    }
                                    if (benhNhanDaCapBarcodeVos.Any(x => x.BarCodeId == benhNhanThoaDieuKien.BarcodeId))
                                    {
                                        error.TenCongTy = benhNhanThoaDieuKien.TenCongTy;
                                        error.SoHopDong = benhNhanThoaDieuKien.SoHopDong;
                                        error.MaBN = benhNhanThoaDieuKien.MaBN;
                                        error.MaTN = benhNhanThoaDieuKien.MaTN;
                                        error.HoTen = benhNhanThoaDieuKien.HoTen;
                                        error.BarcodeId = benhNhanThoaDieuKien.BarcodeId;
                                        error.NamSinhDisplay = benhNhanThoaDieuKien.NamSinhDisplay;
                                        error.GioiTinhDisplay = benhNhanThoaDieuKien.GioiTinhDisplay;
                                        error.BarcodeNumber = benhNhanThoaDieuKien.BarcodeNumber;
                                        error.BarcodeNumberInput = benhNhanThoaDieuKien.BarcodeNumberInput;
                                        error.Error = "Mã barcode đã tồn tại.";
                                        error.IsError = true;
                                        //BVHD-3836
                                        error.NhanVienLayMauIdDisplay = benhNhanThoaDieuKien.NhanVienLayMauIdDisplay;
                                        error.ThoiGianLayMauDisplay = benhNhanThoaDieuKien.ThoiGianLayMauDisplay;
                                        error.NhanVienLayMauId = benhNhanThoaDieuKien.NhanVienLayMauId;
                                        error.ThoiGianLayMau = benhNhanThoaDieuKien.ThoiGianLayMau;
                                    }
                                    else
                                    {
                                        var dataBN = new BenhNhanChuaCapBarcode
                                        {
                                            HoTen = benhNhanThoaDieuKien.HoTen,
                                            TenCongTy = benhNhanThoaDieuKien.TenCongTy,
                                            SoHopDong = benhNhanThoaDieuKien.SoHopDong,
                                            MaBN = benhNhanThoaDieuKien.MaBN,
                                            MaTN = benhNhanThoaDieuKien.MaTN,
                                            BarcodeId = benhNhanThoaDieuKien.BarcodeId,
                                            BarcodeNumber = benhNhanThoaDieuKien.BarcodeNumber,
                                            BarcodeNumberInput = benhNhanThoaDieuKien.BarcodeNumberInput,
                                            NamSinhDisplay = benhNhanThoaDieuKien.NamSinhDisplay,
                                            GioiTinhDisplay = benhNhanThoaDieuKien.GioiTinhDisplay,
                                            //BVHD-3836
                                            NhanVienLayMauIdDisplay = benhNhanThoaDieuKien.NhanVienLayMauIdDisplay,
                                            ThoiGianLayMauDisplay = benhNhanThoaDieuKien.ThoiGianLayMauDisplay,
                                            NhanVienLayMauId = benhNhanThoaDieuKien.NhanVienLayMauId,
                                            ThoiGianLayMau = benhNhanThoaDieuKien.ThoiGianLayMau,
                                        };
                                        var yeuCauDichVuKyThuats = yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(ycdvkt => ycdvkt.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                                                                                          && ycdvkt.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                                                                          && (ycdvkt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                                                                                               || ycdvkt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan
                                                                                                               || ycdvkt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan)
                                                                                                          && !ycdvkt.PhienXetNghiemChiTiets.Any()).ToList();
                                        if (yeuCauDichVuKyThuats.Any())
                                        {
                                            error = XuLyCapBarcodeChoBenhNhan(yeuCauTiepNhan, dataBN, yeuCauDichVuKyThuats);
                                        }
                                    }
                                    lstError.Add(error);
                                }
                            }

                        }
                    }
                    //save change 1 lan
                    BaseRepository.Context.SaveChanges();
                }
            }

            return lstError;
        }
        private BenhNhanChuaCapBarcode XuLyCapBarcodeChoBenhNhan(YeuCauTiepNhan yeuCauTiepNhanKhacCuaBenhNhan, BenhNhanChuaCapBarcode model, List<YeuCauDichVuKyThuat> yeuCauDichVuKyThuats)
        {
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var phongHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();
            // xử lý tạo phiên hoặc cập nhật
            var phienXetNghiem = yeuCauTiepNhanKhacCuaBenhNhan.PhienXetNghiems.LastOrDefault(x => x.BarCodeId == model.BarcodeId);
            var error = new BenhNhanChuaCapBarcode();
            // trường hợp cùng phiên
            if (phienXetNghiem != null && phienXetNghiem.NhanVienKetLuanId == null)
            {
                // xử lý cập nhật phiên: thêm phiên chi tiết, mẫu xét nghiệm
                //if (!yeuCauDichVuKyThuats.Any())
                //{
                //    error.TenCongTy = model.TenCongTy;
                //    error.SoHopDong = model.SoHopDong;
                //    error.MaTN = model.MaTN;
                //    error.MaBN = model.MaBN;
                //    error.BarcodeId = model.BarcodeId;
                //    error.BarcodeNumberInput = model.BarcodeNumberInput;
                //    error.BarcodeNumber = model.BarcodeNumber;
                //    error.HoTen = yeuCauTiepNhanKhacCuaBenhNhan.HoTen;
                //    error.GioiTinhDisplay = yeuCauTiepNhanKhacCuaBenhNhan.GioiTinh.GetDescription();
                //    error.NamSinhDisplay = yeuCauTiepNhanKhacCuaBenhNhan.NamSinh.ToString();
                //    error.Error = "Người bệnh chưa có dịch vụ xét nghiệm nào để cấp barcode.";
                //    error.IsError = true;
                //    return error;
                //}
                foreach (var dichVuKyThuat in yeuCauDichVuKyThuats)
                {
                    // xử lý tạo phiên chi tiết
                    dichVuKyThuat.TrangThai = dichVuKyThuat.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien ? Enums.EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien : dichVuKyThuat.TrangThai;
                    
                    dichVuKyThuat.NhanVienThucHienId = model.NhanVienLayMauId != null ? model.NhanVienLayMauId : currentUserId  ; // BVHD-3836
                    dichVuKyThuat.ThoiDiemThucHien = model.ThoiGianLayMau != null ? model.ThoiGianLayMau :DateTime.Now;// BVHD-3836


                    var newPhienChiTiet = new PhienXetNghiemChiTiet()
                    {
                        NhomDichVuBenhVienId = dichVuKyThuat.NhomDichVuBenhVienId,
                        //YeuCauDichVuKyThuatId = dichVuKyThuat.Id,
                        YeuCauDichVuKyThuat = dichVuKyThuat,
                        DichVuKyThuatBenhVienId = dichVuKyThuat.DichVuKyThuatBenhVienId,
                        LanThucHien = 1,
                        ThoiDiemLayMau = model.ThoiGianLayMau != null ? model.ThoiGianLayMau : DateTime.Now,// BVHD-3836
                        NhanVienLayMauId = model.NhanVienLayMauId != null ? model.NhanVienLayMauId : currentUserId ,// BVHD-3836
                        PhongLayMauId = phongHienTaiId
                    };
                    phienXetNghiem.PhienXetNghiemChiTiets.Add(newPhienChiTiet);

                    // xử lý tạo mẫu xét nghiệm
                    var loaiMau = dichVuKyThuat.DichVuKyThuatBenhVien.LoaiMauXetNghiem;
                    if (loaiMau == null)
                    {
                        //throw new Exception(_localizationService.GetResource("LayMauXetNghiem.LoaiMau.Required"));
                        error.TenCongTy = model.TenCongTy;
                        error.SoHopDong = model.SoHopDong;
                        error.MaTN = model.MaTN;
                        error.MaBN = model.MaBN;
                        error.BarcodeId = model.BarcodeId;
                        error.BarcodeNumberInput = model.BarcodeNumberInput;
                        error.BarcodeNumber = model.BarcodeNumber;
                        error.HoTen = yeuCauTiepNhanKhacCuaBenhNhan.HoTen;
                        error.GioiTinhDisplay = yeuCauTiepNhanKhacCuaBenhNhan.GioiTinh.GetDescription();
                        error.NamSinhDisplay = yeuCauTiepNhanKhacCuaBenhNhan.NamSinh.ToString();
                        error.Error = "Dịch vụ chưa có loại mẫu xét nghiệm";
                        error.IsError = true;
                        return error;
                    }
                    if (!phienXetNghiem.MauXetNghiems.Any(x =>
                        x.NhomDichVuBenhVienId == dichVuKyThuat.NhomDichVuBenhVienId
                        && x.LoaiMauXetNghiem == loaiMau))
                    {
                        var newMauXetNghiem = new MauXetNghiem()
                        {
                            NhomDichVuBenhVienId = dichVuKyThuat.NhomDichVuBenhVienId,
                            LoaiMauXetNghiem = loaiMau.Value,
                            SoLuongMau = 1,
                            BarCodeNumber = model.BarcodeNumber,///
                            BarCodeId = model.BarcodeId,
                            DatChatLuong = true,
                            ThoiDiemLayMau = model.ThoiGianLayMau != null ? model.ThoiGianLayMau : DateTime.Now,// BVHD-3836
                            NhanVienLayMauId = model.NhanVienLayMauId != null ? model.NhanVienLayMauId : currentUserId,// BVHD-3836
                            PhongLayMauId = phongHienTaiId
                        };
                        phienXetNghiem.MauXetNghiems.Add(newMauXetNghiem);
                    }
                }
                //BaseRepository.Update(phienXetNghiem);

            }
            // trường hợp tạo mới phiên
            else
            {
                var phienXetNghiemNew = new PhienXetNghiem()
                {
                    BenhNhanId = yeuCauTiepNhanKhacCuaBenhNhan.BenhNhanId.GetValueOrDefault(),
                    YeuCauTiepNhanId = yeuCauTiepNhanKhacCuaBenhNhan.Id,
                    MaSo = model.BarcodeNumber.GetValueOrDefault(),
                    ThoiDiemBatDau = phienXetNghiem?.ThoiDiemBatDau ?? DateTime.Now
                };

                //if (!yeuCauDichVuKyThuats.Any())
                //{
                //    error.TenCongTy = model.TenCongTy;
                //    error.SoHopDong = model.SoHopDong;
                //    error.MaTN = model.MaTN;
                //    error.MaBN = model.MaBN;
                //    error.BarcodeId = model.BarcodeId;
                //    error.BarcodeNumberInput = model.BarcodeNumberInput;
                //    error.BarcodeNumber = model.BarcodeNumber;
                //    error.HoTen = yeuCauTiepNhanKhacCuaBenhNhan.HoTen;
                //    error.GioiTinhDisplay = yeuCauTiepNhanKhacCuaBenhNhan.GioiTinh.GetDescription();
                //    error.NamSinhDisplay = yeuCauTiepNhanKhacCuaBenhNhan.NamSinh.ToString();
                //    error.Error = "Chưa có dịch vụ xét nghiệm để cấp barcode.";
                //    error.IsError = true;
                //    return error;
                //}

                foreach (var dichVuKyThuat in yeuCauDichVuKyThuats)
                {
                    // xử lý tạo phiên chi tiết
                    dichVuKyThuat.TrangThai = dichVuKyThuat.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien ? Enums.EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien : dichVuKyThuat.TrangThai;
                    dichVuKyThuat.NhanVienThucHienId = model.NhanVienLayMauId != null ? model.NhanVienLayMauId : currentUserId; // BVHD-3836
                    dichVuKyThuat.ThoiDiemThucHien = model.ThoiGianLayMau != null ? model.ThoiGianLayMau : DateTime.Now;// BVHD-3836

                    var newPhienChiTiet = new PhienXetNghiemChiTiet()
                    {
                        NhomDichVuBenhVienId = dichVuKyThuat.NhomDichVuBenhVienId,
                        YeuCauDichVuKyThuat = dichVuKyThuat,
                        DichVuKyThuatBenhVienId = dichVuKyThuat.DichVuKyThuatBenhVienId,
                        LanThucHien = 1,
                        ThoiDiemLayMau = model.ThoiGianLayMau != null ? model.ThoiGianLayMau : DateTime.Now,// BVHD-3836
                        NhanVienLayMauId = model.NhanVienLayMauId != null ? model.NhanVienLayMauId : currentUserId,// BVHD-3836
                        PhongLayMauId = phongHienTaiId
                    };
                    phienXetNghiemNew.PhienXetNghiemChiTiets.Add(newPhienChiTiet);

                    // xử lý tạo mẫu xét nghiệm
                    var loaiMau = dichVuKyThuat.DichVuKyThuatBenhVien.LoaiMauXetNghiem;
                    if (loaiMau == null)
                    {
                        error.TenCongTy = model.TenCongTy;
                        error.SoHopDong = model.SoHopDong;
                        error.MaTN = model.MaTN;
                        error.MaBN = model.MaBN;
                        error.BarcodeId = model.BarcodeId;
                        error.BarcodeNumberInput = model.BarcodeNumberInput;
                        error.BarcodeNumber = model.BarcodeNumber;
                        error.HoTen = yeuCauTiepNhanKhacCuaBenhNhan.HoTen;
                        error.GioiTinhDisplay = yeuCauTiepNhanKhacCuaBenhNhan.GioiTinh.GetDescription();
                        error.NamSinhDisplay = yeuCauTiepNhanKhacCuaBenhNhan.NamSinh.ToString();
                        error.Error = "Dịch vụ chưa có loại mẫu xét nghiệm";
                        error.IsError = true;
                        return error;
                    }
                    if (!phienXetNghiemNew.MauXetNghiems.Any(x =>
                        x.NhomDichVuBenhVienId == dichVuKyThuat.NhomDichVuBenhVienId
                        && x.LoaiMauXetNghiem == loaiMau))
                    {
                        var newMauXetNghiem = new MauXetNghiem()
                        {
                            NhomDichVuBenhVienId = dichVuKyThuat.NhomDichVuBenhVienId,
                            LoaiMauXetNghiem = loaiMau.Value,
                            SoLuongMau = 1,
                            BarCodeNumber = model.BarcodeNumber,
                            BarCodeId = model.BarcodeId,
                            DatChatLuong = true,
                            ThoiDiemLayMau = model.ThoiGianLayMau != null ? model.ThoiGianLayMau : DateTime.Now,// BVHD-3836
                            NhanVienLayMauId = model.NhanVienLayMauId != null ? model.NhanVienLayMauId : currentUserId,// BVHD-3836
                            PhongLayMauId = phongHienTaiId
                        };
                        phienXetNghiemNew.MauXetNghiems.Add(newMauXetNghiem);
                    }
                }

                if (!phienXetNghiemNew.MauXetNghiems.Any())
                {
                    error.TenCongTy = model.TenCongTy;
                    error.SoHopDong = model.SoHopDong;
                    error.MaTN = model.MaTN;
                    error.MaBN = model.MaBN;
                    error.BarcodeId = model.BarcodeId;
                    error.BarcodeNumberInput = model.BarcodeNumberInput;
                    error.BarcodeNumber = model.BarcodeNumber;
                    error.HoTen = yeuCauTiepNhanKhacCuaBenhNhan.HoTen;
                    error.GioiTinhDisplay = yeuCauTiepNhanKhacCuaBenhNhan.GioiTinh.GetDescription();
                    error.NamSinhDisplay = yeuCauTiepNhanKhacCuaBenhNhan.NamSinh.ToString();
                    error.Error = "Không có mẫu xét nghiệm cần thêm";
                    error.IsError = true;
                    return error;
                }
                yeuCauTiepNhanKhacCuaBenhNhan.PhienXetNghiems.Add(phienXetNghiemNew);
            }
            return error;

        }
        #endregion

        #region InBarcodeCuaBenhNhan

        public List<string> InBarcodeCuaBenhNhan(InBarcodeDaCapCodeBenhNhan inBarcodeDaCapCodeBenhNhan)

        {
            var templateBarcode = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("LayMauXetNghiemBarcode")).FirstOrDefault();

            var htmls = new List<string>();
            var phienXetNghiemChiTiets = _phienXetNghiemChiTietRepository.TableNoTracking
                          .ApplyLike(inBarcodeDaCapCodeBenhNhan.SearchString?.Trim(), x => x.PhienXetNghiem.YeuCauTiepNhan.MaYeuCauTiepNhan, x => x.PhienXetNghiem.BenhNhan.MaBN, x => x.PhienXetNghiem.YeuCauTiepNhan.HoTen, x => x.PhienXetNghiem.BarCodeId)
                                            .Where(x => (inBarcodeDaCapCodeBenhNhan.HopDongKhamSucKhoeId == null
                                            || inBarcodeDaCapCodeBenhNhan.HopDongKhamSucKhoeId == 0
                                            || (x.PhienXetNghiem.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe
                                                && x.PhienXetNghiem.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien != null
                                                && x.PhienXetNghiem.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == inBarcodeDaCapCodeBenhNhan.HopDongKhamSucKhoeId))
                                            && x.PhienXetNghiem.ThoiDiemBatDau.Date >= inBarcodeDaCapCodeBenhNhan.TuNgay.Date
                                            && x.PhienXetNghiem.ThoiDiemBatDau.Date <= inBarcodeDaCapCodeBenhNhan.DenNgay.Date
                                            )
                                            .OrderBy(xn => xn.PhienXetNghiem.YeuCauTiepNhanId).ThenBy(xn => xn.PhienXetNghiem.YeuCauTiepNhan.ThoiDiemTiepNhan)
                                            .Select(item => new InBarcodeBenhNhansVo
                                            {
                                                PhienXetNghiemId = item.PhienXetNghiemId,
                                                BarcodeId = item.PhienXetNghiem.BarCodeId.Length == 10 ? item.PhienXetNghiem.BarCodeId.Insert(6, "-") : item.PhienXetNghiem.BarCodeId,
                                                BarcodeIdPrint = item.PhienXetNghiem.BarCodeId,
                                                BarcodeNumber = item.PhienXetNghiem.BarCodeNumber,
                                                TenBenhNhan = item.PhienXetNghiem.YeuCauTiepNhan.HoTen,
                                                GioiTinh = item.PhienXetNghiem.YeuCauTiepNhan.GioiTinh,
                                                NgaySinh = item.PhienXetNghiem.YeuCauTiepNhan.NgaySinh,
                                                ThangSinh = item.PhienXetNghiem.YeuCauTiepNhan.ThangSinh,
                                                NamSinh = item.PhienXetNghiem.YeuCauTiepNhan.NamSinh,
                                                GioCapCode = DateTime.Now.ApplyFormatFullTime(),
                                                NhomDichVuBenhVienId = item.NhomDichVuBenhVienId
                                            })
                                            .ToList();
            var dataAllGroup = phienXetNghiemChiTiets
                                             .GroupBy(x => new
                                             {
                                                 x.PhienXetNghiemId,
                                             })
                                            .Select(item => new InBarcodeBenhNhansVo()
                                            {
                                                PhienXetNghiemId = item.First().PhienXetNghiemId,
                                                BarcodeId = item.First().BarcodeId,
                                                BarcodeNumber = item.First().BarcodeNumber,
                                                TenBenhNhan = item.First().TenBenhNhan,
                                                BarcodeByBarcodeId = BarcodeHelper.GenerateBarCode(item.First().BarcodeIdPrint.Substring(6, 4), 210, 56, false),
                                                GioiTinh = item.First().GioiTinh,
                                                NgaySinh = item.First().NgaySinh,
                                                ThangSinh = item.First().ThangSinh,
                                                NamSinh = item.First().NamSinh,
                                                GioCapCode = item.First().GioCapCode,
                                                SLNhomDichVuBenhVien = item.Select(z => z.NhomDichVuBenhVienId).Distinct().Count() + 1
                                            })
                                            .ToList();
            if (inBarcodeDaCapCodeBenhNhan.SoLuong != null)
            {
                foreach (var data in dataAllGroup)
                {
                    var html = "";
                    for (int i = 0; i < inBarcodeDaCapCodeBenhNhan.SoLuong; i++)
                    {
                        html += TemplateHelpper.FormatTemplateWithContentTemplate(templateBarcode.Body, data);
                        if (html != "")
                        {
                            html = html + "<div class=\"pagebreak\"> </div>";
                        }
                    }
                    htmls.Add(html);
                }
            }
            else
            {
                foreach (var data in dataAllGroup)
                {
                    var html = "";
                    for (int i = 0; i < data.SLNhomDichVuBenhVien; i++)
                    {
                        html += TemplateHelpper.FormatTemplateWithContentTemplate(templateBarcode.Body, data);
                        if (html != "")
                        {
                            html = html + "<div class=\"pagebreak\"> </div>";
                        }
                    }
                    htmls.Add(html);
                }
            }
            return htmls;
        }
        #endregion
        private bool IsLong(string sVal)
        {
            long test;
            return long.TryParse(sVal, out test);
        }
        private long GetNhanVienId(string email)
        {
            var result = _nhanVienRepository.TableNoTracking.Where(d => d.User.Email.Equals(email)).Select(d=>d.Id);
            return result.FirstOrDefault();
        }
        private static bool isEmail(string inputEmail)
        {
            inputEmail = inputEmail ?? string.Empty;
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                  @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                  @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(inputEmail))
                return (true);
            else
                return (false);
        }
    }
}
