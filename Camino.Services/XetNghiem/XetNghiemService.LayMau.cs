using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DichVuXetNghiems;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XetNghiem;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;

namespace Camino.Services.XetNghiem
{
    public partial class XetNghiemService
    {
        #region Grid
        public async Task<GridDataSource> GetDataForGridYeuCauTiepNhanCanLayMauXetNghiemAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var timKiemNangCaoObj = new LayMauXetNghiemTimKiemNangCaoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<LayMauXetNghiemTimKiemNangCaoVo>(queryInfo.AdditionalSearchString);
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

            var query = _yeuCauTiepNhanRepository.TableNoTracking
                .ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.MaYeuCauTiepNhan, x => x.BenhNhan.MaBN, x => x.HoTen)
                .Where(x => x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                            && x.YeuCauDichVuKyThuats.Any(a =>
                                a.NhomDichVuBenhVien.NhomDichVuBenhVienChaId != null
                            //a.NhomDichVuBenhVien.NhomDichVuBenhVienCha.Ma == "XN"
                                && a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                && a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && (a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                                    a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                                    a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                                && (tuNgay == null || (tuNgay != null && ((a.NoiTruPhieuDieuTri == null && a.ThoiDiemChiDinh >= tuNgay.Value.Date) || (a.NoiTruPhieuDieuTri != null && a.NoiTruPhieuDieuTri.NgayDieuTri >= tuNgay.Value.Date))))
                                && (denNgay == null || (denNgay != null && ((a.NoiTruPhieuDieuTri == null && a.ThoiDiemChiDinh <= denNgay.Value.Date) || (a.NoiTruPhieuDieuTri != null && a.NoiTruPhieuDieuTri.NgayDieuTri <= denNgay.Value.Date))))
                            ))
                .Select(item => new LayMauXetNghiemYeuCauTiepNhanGridVo()
                {
                    Id = item.Id,
                    MaTiepNhan = item.MaYeuCauTiepNhan,
                    MaBenhNhan = item.BenhNhan.MaBN,
                    HoTen = item.HoTen,
                    NamSinh = item.NamSinh,
                    GioiTinh = item.GioiTinh.GetDescription(),
                    DiaChi = item.DiaChiDayDu,
                    SoDienThoai = item.SoDienThoai,
                    SoDienThoaiDisplay = item.SoDienThoaiDisplay,

                    // todo: update thêm số lượng nhóm chờ lấy
                    SoLuongChoLay = item.YeuCauDichVuKyThuats
                        .Where(a =>
                                    a.NhomDichVuBenhVien.NhomDichVuBenhVienChaId != null
                                    //a.NhomDichVuBenhVien.NhomDichVuBenhVienCha.Ma == "XN"
                                    && a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                    && a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                    && (a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                                        a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                                        a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                                    && !a.PhienXetNghiemChiTiets.Any()
                            && (tuNgay == null || (tuNgay != null && ((a.NoiTruPhieuDieuTri == null && a.ThoiDiemChiDinh >= tuNgay.Value.Date) || (a.NoiTruPhieuDieuTri != null && a.NoiTruPhieuDieuTri.NgayDieuTri >= tuNgay.Value.Date))))
                            && (denNgay == null || (denNgay != null && ((a.NoiTruPhieuDieuTri == null && a.ThoiDiemChiDinh <= denNgay.Value.Date) || (a.NoiTruPhieuDieuTri != null && a.NoiTruPhieuDieuTri.NgayDieuTri <= denNgay.Value.Date)))))
                        .Select(a => a.NhomDichVuBenhVienId)
                        .Union(
                            item.PhienXetNghiems
                                .SelectMany(a => a.PhienXetNghiemChiTiets)
                                .Where(a => a.DichVuKyThuatBenhVien.LoaiMauXetNghiem != null)
                                .GroupBy(a => new { a.NhomDichVuBenhVienId, a.PhienXetNghiemId })
                                .Where(a => (
                                                // nhóm bất kỳ có 1 mẫu chưa được tạo
                                                (item.PhienXetNghiems
                                                  .Where(b => b.Id == a.Key.PhienXetNghiemId)
                                                  .SelectMany(b => b.PhienXetNghiemChiTiets.Where(c =>
                                                      c.NhomDichVuBenhVienId == a.Key.NhomDichVuBenhVienId &&
                                                      c.DichVuKyThuatBenhVien.LoaiMauXetNghiem != null))
                                                  .Select(b => b.DichVuKyThuatBenhVien.LoaiMauXetNghiem)
                                                  .GroupBy(b => new { b }).Count() - item.PhienXetNghiems
                                                  .Where(b => b.Id == a.Key.PhienXetNghiemId)
                                                  .SelectMany(b => b.MauXetNghiems.Where(c =>
                                                      c.NhomDichVuBenhVienId == a.Key.NhomDichVuBenhVienId
                                                          //                                                          && (tuNgay == null || (c.ThoiDiemLayMau != null && c.ThoiDiemLayMau.Value.Date >= tuNgay.Value.Date)) 
                                                          //                                                          && (denNgay == null || (c.ThoiDiemLayMau != null && c.ThoiDiemLayMau.Value.Date <= denNgay.Value.Date))
                                                          ))
                                                  .GroupBy(b => new { b.LoaiMauXetNghiem })
                                                  .Count()) != 0

                                             // nhóm bất kỳ có 1 mẫu bị từ chối (lấy mẫu cuối cùng theo từng nhóm dịch vụ)
                                             || item.PhienXetNghiems
                                                 .Where(b => b.Id == a.Key.PhienXetNghiemId)
                                                 .SelectMany(b => b.MauXetNghiems)
                                                 .Where(b => b.NhomDichVuBenhVienId == a.Key.NhomDichVuBenhVienId
                                                                 //                                                             && (tuNgay == null || (b.ThoiDiemLayMau != null && b.ThoiDiemLayMau.Value.Date >= tuNgay.Value.Date)) 
                                                                 //                                                                 && (denNgay == null || (b.ThoiDiemLayMau != null && b.ThoiDiemLayMau.Value.Date <= denNgay.Value.Date))
                                                                 )
                                                 .OrderByDescending(b => b.CreatedOn)
                                                 .GroupBy(b => new { b.LoaiMauXetNghiem })
                                                 .Select(loaiMau => new LoaiMauVo()
                                                 {
                                                     LoaiMau = loaiMau.First().LoaiMauXetNghiem,
                                                     DatChatLuong = loaiMau.First().DatChatLuong
                                                 }).Any(b => b.DatChatLuong == false))
                                            // nhóm bất kỳ có 1 mẫu chưa được gửi mẫu
                                            && item.PhienXetNghiems.Where(b => b.Id == a.Key.PhienXetNghiemId)
                                                .SelectMany(b => b.MauXetNghiems.Where(c =>
                                                    c.NhomDichVuBenhVienId == a.Key.NhomDichVuBenhVienId))
                                                .Any(c => c.PhieuGoiMauXetNghiemId == null))
                                .Select(a => a.Key.NhomDichVuBenhVienId)
                        )
                        .Distinct().Count(),
                    // todo: update thêm số lượng nhóm chờ gửi
                    //                    SoLuongChoGui = item.PhienXetNghiems
                    //                        .SelectMany(a => a.PhienXetNghiemChiTiets)
                    //                        .Where(a => a.DichVuKyThuatBenhVien.LoaiMauXetNghiem != null)
                    //                        .GroupBy(a => new {a.NhomDichVuBenhVienId, a.PhienXetNghiemId})
                    //                        .Count(a =>
                    //                            // tất cả các mẫu đều đã được tạo
                    //                                (item.PhienXetNghiems
                    //                                         .Where(b => b.Id == a.Key.PhienXetNghiemId)
                    //                                         .SelectMany(b => b.PhienXetNghiemChiTiets.Where(c =>
                    //                                             c.NhomDichVuBenhVienId == a.Key.NhomDichVuBenhVienId &&
                    //                                             c.DichVuKyThuatBenhVien.LoaiMauXetNghiem != null))
                    //                                         .Select(b => b.DichVuKyThuatBenhVien.LoaiMauXetNghiem)
                    //                                         .GroupBy(b => new { b }).Count() - item.PhienXetNghiems
                    //                                         .Where(b => b.Id == a.Key.PhienXetNghiemId)
                    //                                         .SelectMany(b => b.MauXetNghiems
                    //                                                              .Where(c => c.NhomDichVuBenhVienId == a.Key.NhomDichVuBenhVienId))
                    //                                         .GroupBy(b => new { b.LoaiMauXetNghiem })
                    //                                         .Count()) == 0
                    //                                    // tất cả loại mẫu đều đạt chất lượngs
                    //                                    && item.PhienXetNghiems
                    //                                        .Where(b => b.Id == a.Key.PhienXetNghiemId)
                    //                                        .SelectMany(b => b.MauXetNghiems)
                    //                                        .Where(b => b.NhomDichVuBenhVienId == a.Key.NhomDichVuBenhVienId
                    //                                                    && (tuNgay == null || (b.ThoiDiemLayMau != null && b.ThoiDiemLayMau.Value.Date >= tuNgay.Value.Date))
                    //                                                        && (denNgay == null || (b.ThoiDiemLayMau != null && b.ThoiDiemLayMau.Value.Date <= denNgay.Value.Date))
                    //                                                    && b.PhieuGoiMauXetNghiem == null)
                    //                                        .OrderByDescending(b => b.CreatedOn)
                    //                                        .GroupBy(b => new { b.LoaiMauXetNghiem })
                    //                                        .Select(loaiMau => new LoaiMauVo()
                    //                                        {
                    //                                            LoaiMau = loaiMau.First().LoaiMauXetNghiem,
                    //                                            DatChatLuong = loaiMau.First().DatChatLuong
                    //                                        }).All(b => b.DatChatLuong == true)
                    //                                    // có ít nhất 1 mẫu chưa được gửi mẫu
                    //                                    && item.PhienXetNghiems
                    //                                            .Where(b => b.Id == a.Key.PhienXetNghiemId)
                    //                                            .Any(b => b.MauXetNghiems
                    //                                                .Any(c =>
                    //                                                      c.NhomDichVuBenhVienId == a.Key.NhomDichVuBenhVienId
                    //                                                      && (tuNgay == null || (c.ThoiDiemLayMau != null && c.ThoiDiemLayMau.Value.Date >= tuNgay.Value.Date))
                    //                                                          && (denNgay == null || (c.ThoiDiemLayMau != null && c.ThoiDiemLayMau.Value.Date <= denNgay.Value.Date))
                    //                                                          && c.PhieuGoiMauXetNghiemId == null))),

                    CoDuKetQua = item.YeuCauDichVuKyThuats.Where(x => x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                                                      && x.NhomDichVuBenhVien.NhomDichVuBenhVienChaId != null
                                                                      && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                                      && (x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan
                                                                          || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan
                                                                          || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
                                                         .All(x => x.PhienXetNghiemChiTiets.Any())
                                 && item.PhienXetNghiems.Any()
                                 && item.PhienXetNghiems.All(x => x.ThoiDiemKetLuan != null),
                    ThoiDiemTiepNhan = item.ThoiDiemTiepNhan,
                    BenhNhanDaNhanKetQua = item.PhienXetNghiems.Any() &&
                                           item.PhienXetNghiems.All(a => a.DaTraKetQua == true),
                    FlagChoKetQua = item.PhienXetNghiems
                                        .Where(a => a.ThoiDiemKetLuan == null)
                                        .SelectMany(a => a.MauXetNghiems.Where(b =>
                                                                                     //                                        (tuNgay == null || (b.ThoiDiemLayMau != null && b.ThoiDiemLayMau.Value.Date >= tuNgay.Value.Date))
                                                                                     //                                                                                     && (denNgay == null || (b.ThoiDiemLayMau != null && b.ThoiDiemLayMau.Value.Date <= denNgay.Value.Date))&& 
                                                                                     b.DatChatLuong == true
                                                                                    && b.PhieuGoiMauXetNghiem != null
                                                                                    && b.PhieuGoiMauXetNghiem.ThoiDiemNhanMau != null)).Any(),
                    FlagDaCoKetQua = item.PhienXetNghiems
                                        .Where(a => a.ThoiDiemKetLuan != null)
                                        .SelectMany(a => a.MauXetNghiems.Where(b =>
                                                                                     //                                        (tuNgay == null || (b.ThoiDiemLayMau != null && b.ThoiDiemLayMau.Value.Date >= tuNgay.Value.Date))
                                                                                     //                                                                                     && (denNgay == null || (b.ThoiDiemLayMau != null && b.ThoiDiemLayMau.Value.Date <= denNgay.Value.Date))&& 
                                                                                     b.DatChatLuong == true
                                                                                     && b.PhieuGoiMauXetNghiem != null
                                                                                     && b.PhieuGoiMauXetNghiem.ThoiDiemNhanMau != null)).Any()

                })
                .Where(x => timKiemNangCaoObj.TrangThai == null
                            || (timKiemNangCaoObj.TrangThai.ChoLayMau && x.FlagChoLay)
                            || (timKiemNangCaoObj.TrangThai.ChoGuiMau && x.FlagChoGui)
                            || (timKiemNangCaoObj.TrangThai.ChoKetQua && x.FlagChoKetQua)
                            || (timKiemNangCaoObj.TrangThai.DaCoKetQua && x.FlagDaCoKetQua)
                            || (!timKiemNangCaoObj.TrangThai.ChoLayMau && !timKiemNangCaoObj.TrangThai.ChoGuiMau &&
                                !timKiemNangCaoObj.TrangThai.ChoKetQua && !timKiemNangCaoObj.TrangThai.DaCoKetQua));

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).ThenBy(x => x.ThoiDiemTiepNhan).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource
            {
                Data = queryTask.Result,
                TotalRowCount = countTask.Result
            };
        }
        public async Task<GridDataSource> GetDataForGridYeuCauTiepNhanCanLayMauXetNghiemAsyncVer2(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var timKiemNangCaoObj = new LayMauXetNghiemTimKiemNangCaoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<LayMauXetNghiemTimKiemNangCaoVo>(queryInfo.AdditionalSearchString);
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

            var query = await _yeuCauTiepNhanRepository.TableNoTracking
                .ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.MaYeuCauTiepNhan, x => x.BenhNhan.MaBN, x => x.HoTen)
                .Where(x => x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                            && x.YeuCauDichVuKyThuats.Any(a =>
                                a.NhomDichVuBenhVien.NhomDichVuBenhVienChaId != null
                                && a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                && a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && (a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                                    a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                                    a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                                && (tuNgay == null || (tuNgay != null && ((a.NoiTruPhieuDieuTri == null && a.ThoiDiemChiDinh >= tuNgay.Value.Date) || (a.NoiTruPhieuDieuTri != null && a.NoiTruPhieuDieuTri.NgayDieuTri >= tuNgay.Value.Date))))
                                && (denNgay == null || (denNgay != null && ((a.NoiTruPhieuDieuTri == null && a.ThoiDiemChiDinh <= denNgay.Value.Date) || (a.NoiTruPhieuDieuTri != null && a.NoiTruPhieuDieuTri.NgayDieuTri <= denNgay.Value.Date)))))
                            && (timKiemNangCaoObj.TrangThai == null
                                || (timKiemNangCaoObj.TrangThai.ChoLayMau
                                    && (x.YeuCauDichVuKyThuats.Any(a => a.NhomDichVuBenhVien.NhomDichVuBenhVienChaId != null
                                                                      && a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                                                      && a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                                      && (a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                                                                          a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                                                                          a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                                                                      && !a.PhienXetNghiemChiTiets.Any()
                                                                      && (tuNgay == null || (tuNgay != null && ((a.NoiTruPhieuDieuTri == null && a.ThoiDiemChiDinh >= tuNgay.Value.Date) || (a.NoiTruPhieuDieuTri != null && a.NoiTruPhieuDieuTri.NgayDieuTri >= tuNgay.Value.Date))))
                                                                      && (denNgay == null || (denNgay != null && ((a.NoiTruPhieuDieuTri == null && a.ThoiDiemChiDinh <= denNgay.Value.Date) || (a.NoiTruPhieuDieuTri != null && a.NoiTruPhieuDieuTri.NgayDieuTri <= denNgay.Value.Date)))))



                                         // nhóm bất kỳ có 1 mẫu chưa được tạo
                                         || x.PhienXetNghiems.Any(a => a.PhienXetNghiemChiTiets
                                                                        .Any(b => a.MauXetNghiems.All(c => c.LoaiMauXetNghiem != b.DichVuKyThuatBenhVien.LoaiMauXetNghiem)
                                                                                && a.MauXetNghiems.Any(c => c.LoaiMauXetNghiem == b.DichVuKyThuatBenhVien.LoaiMauXetNghiem
                                                                                                            && c.PhieuGoiMauXetNghiemId == null)))

                                        // nhóm bất kỳ có 1 mẫu bị từ chối (lấy mẫu cuối cùng theo từng nhóm dịch vụ)
                                        || x.PhienXetNghiems.Any(a => a.MauXetNghiems
                                                                       .Any(b => a.MauXetNghiems.Where(aa => aa.LoaiMauXetNghiem == b.LoaiMauXetNghiem).OrderByDescending(aa => aa.Id).Select(aa => aa.DatChatLuong).FirstOrDefault() == false
                                                                                && a.MauXetNghiems.Any(c => c.LoaiMauXetNghiem == b.LoaiMauXetNghiem
                                                                                                            && c.PhieuGoiMauXetNghiemId == null)))

                                    //&& x.PhienXetNghiems.Any(a => a.MauXetNghiems.Any(c => c.PhieuGoiMauXetNghiemId == null))

                                    ))
                                || (timKiemNangCaoObj.TrangThai.ChoKetQua && x.PhienXetNghiems
                                                                                .Where(a => a.ThoiDiemKetLuan == null)
                                                                                .SelectMany(a => a.MauXetNghiems.Where(b =>
                                                                                    b.DatChatLuong == true
                                                                                    && b.PhieuGoiMauXetNghiem != null
                                                                                    && b.PhieuGoiMauXetNghiem.ThoiDiemNhanMau != null)).Any())
                                || (timKiemNangCaoObj.TrangThai.DaCoKetQua && x.PhienXetNghiems
                                                                                .Where(a => a.ThoiDiemKetLuan != null)
                                                                                .SelectMany(a => a.MauXetNghiems.Where(b =>
                                                                                    b.DatChatLuong == true
                                                                                    && b.PhieuGoiMauXetNghiem != null
                                                                                    && b.PhieuGoiMauXetNghiem.ThoiDiemNhanMau != null)).Any())
                                || (!timKiemNangCaoObj.TrangThai.ChoLayMau && !timKiemNangCaoObj.TrangThai.ChoGuiMau &&
                                    !timKiemNangCaoObj.TrangThai.ChoKetQua && !timKiemNangCaoObj.TrangThai.DaCoKetQua)))
                .Include(x => x.BenhNhan)
                .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.NoiTruPhieuDieuTri)
                .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.DichVuKyThuatBenhVien)
                .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.NhomDichVuBenhVien)
                .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.PhienXetNghiemChiTiets)
                .Include(x => x.PhienXetNghiems).ThenInclude(y => y.PhienXetNghiemChiTiets).ThenInclude(z => z.DichVuKyThuatBenhVien)
                .Include(x => x.PhienXetNghiems).ThenInclude(y => y.MauXetNghiems).ThenInclude(z => z.PhieuGoiMauXetNghiem)
                .OrderBy(queryInfo.SortString).ThenBy(x => x.ThoiDiemTiepNhan).Skip(queryInfo.Skip).Take(queryInfo.Take).ToListAsync();

            var result = query
                .Select(item => new LayMauXetNghiemYeuCauTiepNhanGridVo()
                {
                    Id = item.Id,
                    MaTiepNhan = item.MaYeuCauTiepNhan,
                    MaBenhNhan = item.BenhNhan.MaBN,
                    HoTen = item.HoTen,
                    NamSinh = item.NamSinh,
                    GioiTinh = item.GioiTinh.GetDescription(),
                    DiaChi = item.DiaChiDayDu,
                    SoDienThoai = item.SoDienThoai,
                    SoDienThoaiDisplay = item.SoDienThoaiDisplay,

                    // todo: update thêm số lượng nhóm chờ lấy
                    SoLuongChoLay = item.YeuCauDichVuKyThuats
                        .Where(a =>
                                    a.NhomDichVuBenhVien.NhomDichVuBenhVienChaId != null
                                    //a.NhomDichVuBenhVien.NhomDichVuBenhVienCha.Ma == "XN"
                                    && a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                    && a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                    && (a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                                        a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                                        a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                                    && !a.PhienXetNghiemChiTiets.Any()
                            && (tuNgay == null || (tuNgay != null && ((a.NoiTruPhieuDieuTri == null && a.ThoiDiemChiDinh >= tuNgay.Value.Date) || (a.NoiTruPhieuDieuTri != null && a.NoiTruPhieuDieuTri.NgayDieuTri >= tuNgay.Value.Date))))
                            && (denNgay == null || (denNgay != null && ((a.NoiTruPhieuDieuTri == null && a.ThoiDiemChiDinh <= denNgay.Value.Date) || (a.NoiTruPhieuDieuTri != null && a.NoiTruPhieuDieuTri.NgayDieuTri <= denNgay.Value.Date)))))
                        .Select(a => a.NhomDichVuBenhVienId)
                        .Union(
                            item.PhienXetNghiems
                                .SelectMany(a => a.PhienXetNghiemChiTiets)
                                .Where(a => a.DichVuKyThuatBenhVien.LoaiMauXetNghiem != null)
                                .GroupBy(a => new { a.NhomDichVuBenhVienId, a.PhienXetNghiemId })
                                .Where(a => (
                                                // nhóm bất kỳ có 1 mẫu chưa được tạo
                                                (item.PhienXetNghiems
                                                  .Where(b => b.Id == a.Key.PhienXetNghiemId)
                                                  .SelectMany(b => b.PhienXetNghiemChiTiets.Where(c =>
                                                      c.NhomDichVuBenhVienId == a.Key.NhomDichVuBenhVienId &&
                                                      c.DichVuKyThuatBenhVien.LoaiMauXetNghiem != null))
                                                  .Select(b => b.DichVuKyThuatBenhVien.LoaiMauXetNghiem)
                                                  .GroupBy(b => new { b }).Count() - item.PhienXetNghiems
                                                  .Where(b => b.Id == a.Key.PhienXetNghiemId)
                                                  .SelectMany(b => b.MauXetNghiems.Where(c =>
                                                      c.NhomDichVuBenhVienId == a.Key.NhomDichVuBenhVienId
                                                          //                                                          && (tuNgay == null || (c.ThoiDiemLayMau != null && c.ThoiDiemLayMau.Value.Date >= tuNgay.Value.Date)) 
                                                          //                                                          && (denNgay == null || (c.ThoiDiemLayMau != null && c.ThoiDiemLayMau.Value.Date <= denNgay.Value.Date))
                                                          ))
                                                  .GroupBy(b => new { b.LoaiMauXetNghiem })
                                                  .Count()) != 0

                                             // nhóm bất kỳ có 1 mẫu bị từ chối (lấy mẫu cuối cùng theo từng nhóm dịch vụ)
                                             || item.PhienXetNghiems
                                                 .Where(b => b.Id == a.Key.PhienXetNghiemId)
                                                 .SelectMany(b => b.MauXetNghiems)
                                                 .Where(b => b.NhomDichVuBenhVienId == a.Key.NhomDichVuBenhVienId
                                                                 //                                                             && (tuNgay == null || (b.ThoiDiemLayMau != null && b.ThoiDiemLayMau.Value.Date >= tuNgay.Value.Date)) 
                                                                 //                                                                 && (denNgay == null || (b.ThoiDiemLayMau != null && b.ThoiDiemLayMau.Value.Date <= denNgay.Value.Date))
                                                                 )
                                                 .OrderByDescending(b => b.CreatedOn)
                                                 .GroupBy(b => new { b.LoaiMauXetNghiem })
                                                 .Select(loaiMaub => new LoaiMauVo()
                                                 {
                                                     LoaiMau = loaiMaub.First().LoaiMauXetNghiem,
                                                     DatChatLuong = loaiMaub.First().DatChatLuong
                                                 }).Any(b => b.DatChatLuong == false))
                                            // nhóm bất kỳ có 1 mẫu chưa được gửi mẫu
                                            && item.PhienXetNghiems.Where(b => b.Id == a.Key.PhienXetNghiemId)
                                                .SelectMany(b => b.MauXetNghiems.Where(c =>
                                                    c.NhomDichVuBenhVienId == a.Key.NhomDichVuBenhVienId))
                                                .Any(c => c.PhieuGoiMauXetNghiemId == null))
                                .Select(a => a.Key.NhomDichVuBenhVienId)
                        )
                        .Distinct().Count(),
                    CoDuKetQua = item.YeuCauDichVuKyThuats.Where(x => x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                                                      && x.NhomDichVuBenhVien.NhomDichVuBenhVienChaId != null
                                                                      && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                                      && (x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan
                                                                          || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan
                                                                          || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
                                                         .All(x => x.PhienXetNghiemChiTiets.Any())
                                 && item.PhienXetNghiems.Any()
                                 && item.PhienXetNghiems.All(x => x.ThoiDiemKetLuan != null),
                    ThoiDiemTiepNhan = item.ThoiDiemTiepNhan,
                    BenhNhanDaNhanKetQua = item.PhienXetNghiems.Any() &&
                                           item.PhienXetNghiems.All(a => a.DaTraKetQua == true),
                    FlagChoKetQua = item.PhienXetNghiems
                                        .Where(a => a.ThoiDiemKetLuan == null)
                                        .SelectMany(a => a.MauXetNghiems.Where(b =>
                                                                                     //                                        (tuNgay == null || (b.ThoiDiemLayMau != null && b.ThoiDiemLayMau.Value.Date >= tuNgay.Value.Date))
                                                                                     //                                                                                     && (denNgay == null || (b.ThoiDiemLayMau != null && b.ThoiDiemLayMau.Value.Date <= denNgay.Value.Date))&& 
                                                                                     b.DatChatLuong == true && b.PhieuGoiMauXetNghiem?.ThoiDiemNhanMau != null)).Any(),
                    FlagDaCoKetQua = item.PhienXetNghiems
                                        .Where(a => a.ThoiDiemKetLuan != null)
                                        .SelectMany(a => a.MauXetNghiems.Where(b =>
                                                                                     //                                        (tuNgay == null || (b.ThoiDiemLayMau != null && b.ThoiDiemLayMau.Value.Date >= tuNgay.Value.Date))
                                                                                     //                                                                                     && (denNgay == null || (b.ThoiDiemLayMau != null && b.ThoiDiemLayMau.Value.Date <= denNgay.Value.Date))&& 
                                                                                     b.DatChatLuong == true && b.PhieuGoiMauXetNghiem?.ThoiDiemNhanMau != null)).Any()

                }).ToArray();

            return new GridDataSource
            {
                Data = result,
                TotalRowCount = result.Length
            };
        }
        public async Task<GridDataSource> GetDataForGridYeuCauTiepNhanCanLayMauXetNghiemAsyncVer3(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var timKiemNangCaoObj = new LayMauXetNghiemTimKiemNangCaoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<LayMauXetNghiemTimKiemNangCaoVo>(queryInfo.AdditionalSearchString);
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

            //var khoaXetNghiem = _cauHinhService.GetSetting("CauHinhXetNghiem.KhoaXetNghiem");
            //var khoaXetNghiemId = long.Parse(khoaXetNghiem.Value);
            //var laNhanVienKhoaXetNghiem = await _nhanVienRepository.TableNoTracking
            //    .AnyAsync(x => x.Id == _userAgentHelper.GetCurrentUserId() 
            //                   && x.KhoaPhongNhanViens.Any() 
            //                   && x.KhoaPhongNhanViens.Any(y => y.KhoaPhongId == khoaXetNghiemId));
            var laNhanVienKhoaXetNghiem = await KiemTraNhanVienThuocKhoaXetNghiemAsync();
            var phongHienTai = new Core.Domain.Entities.PhongBenhViens.PhongBenhVien();
            if (!laNhanVienKhoaXetNghiem)
            {
                phongHienTai = _phongBenhVienRepository.TableNoTracking
                    .First(x => x.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            }

            //todo: update bỏ await
            var query = _yeuCauTiepNhanRepository.TableNoTracking
                .ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.MaYeuCauTiepNhan, x => x.BenhNhan.MaBN, x => x.HoTen)
                .Where(x =>  x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                            //&& x.YeuCauDichVuKyThuats.Any(a =>
                            //    a.NhomDichVuBenhVien.NhomDichVuBenhVienChaId != null
                            //    && a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                            //    && a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                            //    && (a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                            //        a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                            //        a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                            //    && (tuNgay == null || (tuNgay != null && ((a.NoiTruPhieuDieuTri == null && a.ThoiDiemChiDinh >= tuNgay.Value.Date) || (a.NoiTruPhieuDieuTri != null && a.NoiTruPhieuDieuTri.NgayDieuTri >= tuNgay.Value.Date))))
                            //    && (denNgay == null || (denNgay != null && ((a.NoiTruPhieuDieuTri == null && a.ThoiDiemChiDinh <= denNgay.Value.Date) || (a.NoiTruPhieuDieuTri != null && a.NoiTruPhieuDieuTri.NgayDieuTri <= denNgay.Value.Date))))

                            //    // bổ sung load theo tài khoản nhân viên đang login
                            //    && (laNhanVienKhoaXetNghiem || (a.NoiChiDinh != null && a.NoiChiDinh.KhoaPhongId == phongHienTai.KhoaPhongId)))
                            //&& (timKiemNangCaoObj.TrangThai == null
                            //    || (timKiemNangCaoObj.TrangThai.ChoLayMau
                            //        && (x.YeuCauDichVuKyThuats.Any(a => 
                            //                //a.NhomDichVuBenhVien.NhomDichVuBenhVienChaId != null
                            //                                           a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                            //                                          && a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                            //                                          && (a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                            //                                              a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                            //                                              a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                            //                                          && !a.PhienXetNghiemChiTiets.Any()
                            //                                          && (tuNgay == null || (tuNgay != null && ((a.ThoiDiemDangKy == null && a.ThoiDiemChiDinh >= tuNgay.Value.Date) || (a.ThoiDiemDangKy != null && a.ThoiDiemDangKy >= tuNgay.Value.Date))))
                            //                                          && (denNgay == null || (denNgay != null && ((a.ThoiDiemDangKy == null && a.ThoiDiemChiDinh <= denNgay.Value.Date) || (a.ThoiDiemDangKy != null && a.ThoiDiemDangKy.Date <= denNgay.Value.Date))))
                            //                                        // bổ sung load theo tài khoản nhân viên đang login
                            //                                        && (laNhanVienKhoaXetNghiem || (a.NoiChiDinh != null && a.NoiChiDinh.KhoaPhongId == phongHienTai.KhoaPhongId)))
                            //            //|| x.YeuCauDichVuKyThuats.Any(a => a.NhomDichVuBenhVien.NhomDichVuBenhVienChaId != null
                            //            //                                   && a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                            //            //                                   && a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                            //            //                                   && (a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                            //            //                                       a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                            //            //                                       a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                            //            //                                   && a.PhienXetNghiemChiTiets.Any(b => b.ThoiDiemNhanMau == null && b.ThoiDiemLayMau != null)
                            //            //                                   && (tuNgay == null || (tuNgay != null && ((a.NoiTruPhieuDieuTri == null && a.ThoiDiemChiDinh >= tuNgay.Value.Date) || (a.NoiTruPhieuDieuTri != null && a.NoiTruPhieuDieuTri.NgayDieuTri >= tuNgay.Value.Date))))
                            //            //                                   && (denNgay == null || (denNgay != null && ((a.NoiTruPhieuDieuTri == null && a.ThoiDiemChiDinh <= denNgay.Value.Date) || (a.NoiTruPhieuDieuTri != null && a.NoiTruPhieuDieuTri.NgayDieuTri <= denNgay.Value.Date))))
                            //            //                                   // bổ sung load theo tài khoản nhân viên đang login
                            //            //                                   && (laNhanVienKhoaXetNghiem || (a.NoiChiDinh != null && a.NoiChiDinh.KhoaPhongId == phongHienTai.KhoaPhongId)))
                            //        )
                            //        )
                            //    || (timKiemNangCaoObj.TrangThai.DaLayMau 
                            //        && x.YeuCauDichVuKyThuats.Any(a =>
                            //            //a.NhomDichVuBenhVien.NhomDichVuBenhVienChaId != null
                            //            a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                            //            && a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                            //            && (a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                            //                a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                            //                a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                            //            && a.PhienXetNghiemChiTiets.Any(b => b.ThoiDiemLayMau != null)
                            //            && (tuNgay == null || (tuNgay != null && ((a.ThoiDiemDangKy == null && a.ThoiDiemChiDinh >= tuNgay.Value.Date) || (a.ThoiDiemDangKy != null && a.ThoiDiemDangKy >= tuNgay.Value.Date))))
                            //            && (denNgay == null || (denNgay != null && ((a.ThoiDiemDangKy == null && a.ThoiDiemChiDinh <= denNgay.Value.Date) || (a.ThoiDiemDangKy != null && a.ThoiDiemDangKy.Date <= denNgay.Value.Date))))
                            //            // bổ sung load theo tài khoản nhân viên đang login
                            //            && (laNhanVienKhoaXetNghiem || (a.NoiChiDinh != null && a.NoiChiDinh.KhoaPhongId == phongHienTai.KhoaPhongId))))

                            //    //|| (timKiemNangCaoObj.TrangThai.ChoKetQua && x.PhienXetNghiems
                            //    //                                                .Where(a => a.ThoiDiemKetLuan == null
                            //    //                                                            && a.PhienXetNghiemChiTiets.Any(b => b.YeuCauDichVuKyThuat.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                            //    //                                                                                                 && (b.YeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                            //    //                                                                                                     b.YeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                            //    //                                                                                                     b.YeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                            //    //                                                                                                 && (laNhanVienKhoaXetNghiem || (b.YeuCauDichVuKyThuat.NoiChiDinh != null && b.YeuCauDichVuKyThuat.NoiChiDinh.KhoaPhongId == phongHienTai.KhoaPhongId))))
                            //    //                                                .SelectMany(a => a.MauXetNghiems.Where(b =>
                            //    //                                                    b.DatChatLuong == true
                            //    //                                                    && b.PhieuGoiMauXetNghiem != null
                            //    //                                                    && b.PhieuGoiMauXetNghiem.ThoiDiemNhanMau != null)).Any())
                            //    //|| (timKiemNangCaoObj.TrangThai.DaCoKetQua && x.PhienXetNghiems
                            //    //                                                .Where(a => a.ThoiDiemKetLuan != null
                            //    //                                                            && a.PhienXetNghiemChiTiets.Any(b => b.YeuCauDichVuKyThuat.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                            //    //                                                                                                   && (b.YeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                            //    //                                                                                                       b.YeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                            //    //                                                                                                       b.YeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                            //    //                                                                                                   && (laNhanVienKhoaXetNghiem || (b.YeuCauDichVuKyThuat.NoiChiDinh != null && b.YeuCauDichVuKyThuat.NoiChiDinh.KhoaPhongId == phongHienTai.KhoaPhongId))))
                            //    //                                                .SelectMany(a => a.MauXetNghiems.Where(b =>
                            //    //                                                    b.DatChatLuong == true
                            //    //                                                    && b.PhieuGoiMauXetNghiem != null
                            //    //                                                    && b.PhieuGoiMauXetNghiem.ThoiDiemNhanMau != null)).Any())
                            //    || (!timKiemNangCaoObj.TrangThai.ChoLayMau 
                            //        //&& !timKiemNangCaoObj.TrangThai.ChoGuiMau 
                            //        //&& !timKiemNangCaoObj.TrangThai.ChoKetQua 
                            //        //&& !timKiemNangCaoObj.TrangThai.DaCoKetQua
                            //        && !timKiemNangCaoObj.TrangThai.DaLayMau
                            //        && x.YeuCauDichVuKyThuats.Any(a =>
                            //            a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                            //            && a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                            //            && (a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                            //                a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                            //                a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                            //            && (tuNgay == null || (tuNgay != null && ((a.ThoiDiemDangKy == null && a.ThoiDiemChiDinh >= tuNgay.Value.Date) || (a.ThoiDiemDangKy != null && a.ThoiDiemDangKy >= tuNgay.Value.Date))))
                            //            && (denNgay == null || (denNgay != null && ((a.ThoiDiemDangKy == null && a.ThoiDiemChiDinh <= denNgay.Value.Date) || (a.ThoiDiemDangKy != null && a.ThoiDiemDangKy.Date <= denNgay.Value.Date))))

                            //            // bổ sung load theo tài khoản nhân viên đang login
                            //            && (laNhanVienKhoaXetNghiem || (a.NoiChiDinh != null && a.NoiChiDinh.KhoaPhongId == phongHienTai.KhoaPhongId)))))

                            && x.YeuCauDichVuKyThuats.Any(a =>
                                a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                && a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && (a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                                    a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                                    a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                                && (tuNgay == null || (tuNgay != null && ((a.NoiTruPhieuDieuTriId == null && a.ThoiDiemChiDinh >= tuNgay.Value.Date) || (a.NoiTruPhieuDieuTriId != null && a.ThoiDiemDangKy >= tuNgay.Value.Date))))
                                && (denNgay == null || (denNgay != null && ((a.NoiTruPhieuDieuTriId == null && a.ThoiDiemChiDinh <= denNgay.Value.Date) || (a.NoiTruPhieuDieuTriId != null && a.ThoiDiemDangKy.Date <= denNgay.Value.Date))))
                                // bổ sung load theo tài khoản nhân viên đang login
                                && (laNhanVienKhoaXetNghiem || (a.NoiChiDinh != null && a.NoiChiDinh.KhoaPhongId == phongHienTai.KhoaPhongId))

                                // kiểm tra trạng thái
                                && ((timKiemNangCaoObj.TrangThai.ChoLayMau && !a.PhienXetNghiemChiTiets.Any())
                                     || (timKiemNangCaoObj.TrangThai.DaLayMau && a.PhienXetNghiemChiTiets.Any(b => b.ThoiDiemLayMau != null))
                                     || (!timKiemNangCaoObj.TrangThai.ChoLayMau && !timKiemNangCaoObj.TrangThai.DaLayMau)))
                            )
                .Include(x => x.BenhNhan)
                //.Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.NoiTruPhieuDieuTri)
                .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.DichVuKyThuatBenhVien)
                .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.NhomDichVuBenhVien)
                .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.PhienXetNghiemChiTiets)
                .Include(x => x.PhienXetNghiems).ThenInclude(y => y.PhienXetNghiemChiTiets).ThenInclude(z => z.DichVuKyThuatBenhVien)
                .Include(x => x.PhienXetNghiems).ThenInclude(y => y.MauXetNghiems).ThenInclude(z => z.PhieuGoiMauXetNghiem)
                .OrderBy(queryInfo.SortString).ThenBy(x => x.ThoiDiemTiepNhan).Skip(queryInfo.Skip).Take(queryInfo.Take).ToList();

            var result = query
                .Select(item => new LayMauXetNghiemYeuCauTiepNhanGridVo()
                {
                    Id = item.Id,
                    MaTiepNhan = item.MaYeuCauTiepNhan,
                    MaBenhNhan = item.BenhNhan.MaBN,
                    HoTen = item.HoTen,
                    NamSinh = item.NamSinh,
                    GioiTinh = item.GioiTinh.GetDescription(),
                    DiaChi = item.DiaChiDayDu,
                    SoDienThoai = item.SoDienThoai,
                    SoDienThoaiDisplay = item.SoDienThoaiDisplay,

                    // todo: update thêm số lượng nhóm chờ lấy
                    SoLuongChoLay = item.YeuCauDichVuKyThuats
                        .Count(a =>
                                    //a.NhomDichVuBenhVien.NhomDichVuBenhVienChaId != null
                                    //a.NhomDichVuBenhVien.NhomDichVuBenhVienCha.Ma == "XN"
                                    a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                    && a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                    && (a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                                        a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                                        a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                                    && !a.PhienXetNghiemChiTiets.Any()
                                    && (tuNgay == null || (tuNgay != null && ((a.NoiTruPhieuDieuTriId == null && a.ThoiDiemChiDinh >= tuNgay.Value.Date) || (a.NoiTruPhieuDieuTriId != null && a.ThoiDiemDangKy >= tuNgay.Value.Date))))
                                    && (denNgay == null || (denNgay != null && ((a.NoiTruPhieuDieuTriId == null && a.ThoiDiemChiDinh <= denNgay.Value.Date) || (a.NoiTruPhieuDieuTriId != null && a.ThoiDiemDangKy.Date <= denNgay.Value.Date))))
                            // bổ sung load theo tài khoản nhân viên đang login
                            && (laNhanVienKhoaXetNghiem || (a.NoiChiDinh != null && a.NoiChiDinh.KhoaPhongId == phongHienTai.KhoaPhongId))),
                    CoDuKetQua = 
                        item.YeuCauDichVuKyThuats.Where(x => x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                                                      //&& x.NhomDichVuBenhVien.NhomDichVuBenhVienChaId != null
                                                                      && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                                      && (x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan
                                                                          || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan
                                                                          || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
                                                         .All(x => x.PhienXetNghiemChiTiets.Any())
                                 && item.PhienXetNghiems.Any()
                                 && item.PhienXetNghiems.All(x => x.ThoiDiemKetLuan != null),
                    ThoiDiemTiepNhan = item.ThoiDiemTiepNhan,
                    BenhNhanDaNhanKetQua = item.PhienXetNghiems.Any() &&
                                           item.PhienXetNghiems.All(a => a.DaTraKetQua == true),
                    FlagChoKetQua = item.PhienXetNghiems
                                        .Where(a => a.ThoiDiemKetLuan == null)
                                        .SelectMany(a => a.MauXetNghiems.Where(b => b.DatChatLuong == true && b.PhieuGoiMauXetNghiem?.ThoiDiemNhanMau != null)).Any(),
                    FlagDaCoKetQua = item.PhienXetNghiems
                                        .Where(a => a.ThoiDiemKetLuan != null)
                                        .SelectMany(a => a.MauXetNghiems.Where(b => b.DatChatLuong == true && b.PhieuGoiMauXetNghiem?.ThoiDiemNhanMau != null)).Any(),
                    FlagCoDichVuDaCoKetQua = item.PhienXetNghiems.Any(a => a.NhanVienKetLuanId != null)

                }).ToArray();

            return new GridDataSource
            {
                Data = result,
                TotalRowCount = result.Length
            };
        }
        public async Task<GridDataSource> GetDataForGridYeuCauTiepNhanCanLayMauXetNghiemAsyncVer4(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var timKiemNangCaoObj = new LayMauXetNghiemTimKiemNangCaoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<LayMauXetNghiemTimKiemNangCaoVo>(queryInfo.AdditionalSearchString);
            }
            var searchStringRemoveVietnameseDiacritics = (timKiemNangCaoObj.SearchString ?? "").Trim().ToLower().RemoveVietnameseDiacritics();

            DateTime ? tuNgay = null;
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
                _yeuCauTiepNhanRepository.TableNoTracking
                    //.ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.MaYeuCauTiepNhan, x => x.BenhNhan.MaBN, x => x.HoTen)
                    .Include(x => x.BenhNhan)
                    .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.DichVuKyThuatBenhVien)
                    .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.NhomDichVuBenhVien)
                    .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.PhienXetNghiemChiTiets)
                    .Include(x => x.PhienXetNghiems).ThenInclude(y => y.PhienXetNghiemChiTiets).ThenInclude(z => z.DichVuKyThuatBenhVien)
                    .Include(x => x.PhienXetNghiems).ThenInclude(y => y.MauXetNghiems).ThenInclude(z => z.PhieuGoiMauXetNghiem)
                    .Where(x => x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                                && x.YeuCauDichVuKyThuats.Any(a =>
                                    a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                    && a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                    && (a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                                        a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                                        a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                                    && (tuNgay == null || (tuNgay != null && ((a.NoiTruPhieuDieuTriId == null && a.ThoiDiemChiDinh >= tuNgay.Value.Date) || (a.NoiTruPhieuDieuTriId != null && a.ThoiDiemDangKy >= tuNgay.Value.Date))))
                                    && (denNgay == null || (denNgay != null && ((a.NoiTruPhieuDieuTriId == null && a.ThoiDiemChiDinh <= denNgay.Value.Date) || (a.NoiTruPhieuDieuTriId != null && a.ThoiDiemDangKy.Date <= denNgay.Value.Date))))
                                    // bổ sung load theo tài khoản nhân viên đang login
                                    && (laNhanVienKhoaXetNghiem || (a.NoiChiDinh != null && a.NoiChiDinh.KhoaPhongId == phongHienTai.KhoaPhongId))

                                    // kiểm tra trạng thái
                                    && ((timKiemNangCaoObj.TrangThai.ChoLayMau && !a.PhienXetNghiemChiTiets.Any())
                                        || (timKiemNangCaoObj.TrangThai.DaLayMau && a.PhienXetNghiemChiTiets.Any(b => b.ThoiDiemLayMau != null))
                                        || (!timKiemNangCaoObj.TrangThai.ChoLayMau && !timKiemNangCaoObj.TrangThai.DaLayMau)))

                                    // chuyển applylike -> search thủ công
                                    && (x.MaYeuCauTiepNhan.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                                        || x.HoTen.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                                        || x.BenhNhan.MaBN.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                                        || x.PhienXetNghiems.Any(c => c.BarCodeId.Contains(searchStringRemoveVietnameseDiacritics)))
                    )
                .OrderBy(queryInfo.SortString).ThenBy(x => x.ThoiDiemTiepNhan).Skip(queryInfo.Skip).Take(queryInfo.Take).ToList();

            var result = query
                .Select(item => new LayMauXetNghiemYeuCauTiepNhanGridVo()
                {
                    Id = item.Id,
                    MaTiepNhan = item.MaYeuCauTiepNhan,
                    MaBenhNhan = item.BenhNhan.MaBN,
                    HoTen = item.HoTen,
                    NamSinh = item.NamSinh,
                    GioiTinh = item.GioiTinh.GetDescription(),
                    DiaChi = item.DiaChiDayDu,
                    SoDienThoai = item.SoDienThoai,
                    SoDienThoaiDisplay = item.SoDienThoaiDisplay,

                    // todo: update thêm số lượng nhóm chờ lấy
                    SoLuongChoLay = item.YeuCauDichVuKyThuats
                        .Count(a =>
                                    a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                    && a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                    && (a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                                        a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                                        a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                                    && !a.PhienXetNghiemChiTiets.Any()
                                    && (tuNgay == null || (tuNgay != null && ((a.NoiTruPhieuDieuTriId == null && a.ThoiDiemChiDinh >= tuNgay.Value.Date) || (a.NoiTruPhieuDieuTriId != null && a.ThoiDiemDangKy >= tuNgay.Value.Date))))
                                    && (denNgay == null || (denNgay != null && ((a.NoiTruPhieuDieuTriId == null && a.ThoiDiemChiDinh <= denNgay.Value.Date) || (a.NoiTruPhieuDieuTriId != null && a.ThoiDiemDangKy.Date <= denNgay.Value.Date))))
                            // bổ sung load theo tài khoản nhân viên đang login
                            && (laNhanVienKhoaXetNghiem || (a.NoiChiDinh != null && a.NoiChiDinh.KhoaPhongId == phongHienTai.KhoaPhongId))),
                    CoDuKetQua =
                        item.YeuCauDichVuKyThuats.Where(x => x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                                                      //&& x.NhomDichVuBenhVien.NhomDichVuBenhVienChaId != null
                                                                      && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                                      && (x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan
                                                                          || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan
                                                                          || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
                                                         .All(x => x.PhienXetNghiemChiTiets.Any())
                                 && item.PhienXetNghiems.Any()
                                 && item.PhienXetNghiems.All(x => x.ThoiDiemKetLuan != null),
                    ThoiDiemTiepNhan = item.ThoiDiemTiepNhan,
                    BenhNhanDaNhanKetQua = item.PhienXetNghiems.Any() &&
                                           item.PhienXetNghiems.All(a => a.DaTraKetQua == true),
                    FlagChoKetQua = item.PhienXetNghiems
                                        .Where(a => a.ThoiDiemKetLuan == null)
                                        .SelectMany(a => a.MauXetNghiems.Where(b => b.DatChatLuong == true && b.PhieuGoiMauXetNghiem?.ThoiDiemNhanMau != null)).Any(),
                    FlagDaCoKetQua = item.PhienXetNghiems
                                        .Where(a => a.ThoiDiemKetLuan != null)
                                        .SelectMany(a => a.MauXetNghiems.Where(b => b.DatChatLuong == true && b.PhieuGoiMauXetNghiem?.ThoiDiemNhanMau != null)).Any(),
                    FlagCoDichVuDaCoKetQua = item.PhienXetNghiems.Any(a => a.NhanVienKetLuanId != null),
                    BarcodeDisplay = string.Join(", ", item.PhienXetNghiems.Select(x => x.BarCodeId).Distinct().ToList())
                }).ToArray();

            return new GridDataSource
            {
                Data = result,
                TotalRowCount = result.Length
            };
        }
        public async Task<GridDataSource> GetTotalPageForGridYeuCauTiepNhanCanLayMauXetNghiemAsync(QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new LayMauXetNghiemTimKiemNangCaoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<LayMauXetNghiemTimKiemNangCaoVo>(queryInfo.AdditionalSearchString);
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

            var query = _yeuCauTiepNhanRepository.TableNoTracking
                .ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.MaYeuCauTiepNhan, x => x.BenhNhan.MaBN, x => x.HoTen)
                .Where(x => x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                            && x.YeuCauDichVuKyThuats.Any(a =>
                                a.NhomDichVuBenhVien.NhomDichVuBenhVienChaId != null
                                //a.NhomDichVuBenhVien.NhomDichVuBenhVienCha.Ma == "XN"
                                && a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                && a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && (a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                                    a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                                    a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                                && (tuNgay == null || (tuNgay != null && ((a.NoiTruPhieuDieuTri == null && a.ThoiDiemChiDinh >= tuNgay.Value.Date) || (a.NoiTruPhieuDieuTri != null && a.NoiTruPhieuDieuTri.NgayDieuTri >= tuNgay.Value.Date))))
                                && (denNgay == null || (denNgay != null && ((a.NoiTruPhieuDieuTri == null && a.ThoiDiemChiDinh <= denNgay.Value.Date) || (a.NoiTruPhieuDieuTri != null && a.NoiTruPhieuDieuTri.NgayDieuTri <= denNgay.Value.Date))))
                            ))
                .Select(item => new LayMauXetNghiemYeuCauTiepNhanGridVo()
                {
                    Id = item.Id,
                    MaTiepNhan = item.MaYeuCauTiepNhan,
                    MaBenhNhan = item.BenhNhan.MaBN,
                    HoTen = item.HoTen,
                    NamSinh = item.NamSinh,
                    GioiTinh = item.GioiTinh.GetDescription(),
                    DiaChi = item.DiaChiDayDu,
                    SoDienThoai = item.SoDienThoai,
                    SoDienThoaiDisplay = item.SoDienThoaiDisplay,

                    // todo: update thêm số lượng nhóm chờ lấy
                    SoLuongChoLay = item.YeuCauDichVuKyThuats
                        .Where(a =>
                                    a.NhomDichVuBenhVien.NhomDichVuBenhVienChaId != null
                                    //a.NhomDichVuBenhVien.NhomDichVuBenhVienCha.Ma == "XN"
                                    && a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                    && a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                    && (a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                                        a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                                        a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                                    && !a.PhienXetNghiemChiTiets.Any()
                            && (tuNgay == null || (tuNgay != null && ((a.NoiTruPhieuDieuTri == null && a.ThoiDiemChiDinh >= tuNgay.Value.Date) || (a.NoiTruPhieuDieuTri != null && a.NoiTruPhieuDieuTri.NgayDieuTri >= tuNgay.Value.Date))))
                            && (denNgay == null || (denNgay != null && ((a.NoiTruPhieuDieuTri == null && a.ThoiDiemChiDinh <= denNgay.Value.Date) || (a.NoiTruPhieuDieuTri != null && a.NoiTruPhieuDieuTri.NgayDieuTri <= denNgay.Value.Date))))
                            )
                        .Select(a => a.NhomDichVuBenhVienId)
                        .Union(
                            item.PhienXetNghiems
                                .SelectMany(a => a.PhienXetNghiemChiTiets)
                                .Where(a => a.DichVuKyThuatBenhVien.LoaiMauXetNghiem != null)
                                .GroupBy(a => new { a.NhomDichVuBenhVienId, a.PhienXetNghiemId })
                                .Where(a => (
                                                // nhóm bất kỳ có 1 mẫu chưa được tạo
                                                (item.PhienXetNghiems
                                                  .Where(b => b.Id == a.Key.PhienXetNghiemId)
                                                  .SelectMany(b => b.PhienXetNghiemChiTiets.Where(c =>
                                                      c.NhomDichVuBenhVienId == a.Key.NhomDichVuBenhVienId &&
                                                      c.DichVuKyThuatBenhVien.LoaiMauXetNghiem != null))
                                                  .Select(b => b.DichVuKyThuatBenhVien.LoaiMauXetNghiem)
                                                  .GroupBy(b => new { b }).Count() - item.PhienXetNghiems
                                                  .Where(b => b.Id == a.Key.PhienXetNghiemId)
                                                  .SelectMany(b => b.MauXetNghiems.Where(c =>
                                                      c.NhomDichVuBenhVienId == a.Key.NhomDichVuBenhVienId
                                                          //                                                          && (tuNgay == null || (c.ThoiDiemLayMau != null && c.ThoiDiemLayMau.Value.Date >= tuNgay.Value.Date))
                                                          //                                                          && (denNgay == null || (c.ThoiDiemLayMau != null && c.ThoiDiemLayMau.Value.Date <= denNgay.Value.Date))
                                                          ))
                                                  .GroupBy(b => new { b.LoaiMauXetNghiem })
                                                  .Count()) != 0

                                             // nhóm bất kỳ có 1 mẫu bị từ chối (lấy mẫu cuối cùng theo từng nhóm dịch vụ)
                                             || item.PhienXetNghiems
                                                 .Where(b => b.Id == a.Key.PhienXetNghiemId)
                                                 .SelectMany(b => b.MauXetNghiems)
                                                 .Where(b => b.NhomDichVuBenhVienId == a.Key.NhomDichVuBenhVienId
                                                                 //                                                             && (tuNgay == null || (b.ThoiDiemLayMau != null && b.ThoiDiemLayMau.Value.Date >= tuNgay.Value.Date))
                                                                 //                                                                 && (denNgay == null || (b.ThoiDiemLayMau != null && b.ThoiDiemLayMau.Value.Date <= denNgay.Value.Date))
                                                                 )
                                                 .OrderByDescending(b => b.CreatedOn)
                                                 .GroupBy(b => new { b.LoaiMauXetNghiem })
                                                 .Select(loaiMau => new LoaiMauVo()
                                                 {
                                                     LoaiMau = loaiMau.First().LoaiMauXetNghiem,
                                                     DatChatLuong = loaiMau.First().DatChatLuong
                                                 }).Any(b => b.DatChatLuong == false))
                                            // nhóm bất kỳ có 1 mẫu chưa được gửi mẫu
                                            && item.PhienXetNghiems.Where(b => b.Id == a.Key.PhienXetNghiemId)
                                                .SelectMany(b => b.MauXetNghiems.Where(c =>
                                                    c.NhomDichVuBenhVienId == a.Key.NhomDichVuBenhVienId))
                                                .Any(c => c.PhieuGoiMauXetNghiemId == null))
                                .Select(a => a.Key.NhomDichVuBenhVienId)
                        )
                        .Distinct().Count(),
                    // todo: update thêm số lượng nhóm chờ gửi
                    //                    SoLuongChoGui = item.PhienXetNghiems
                    //                        .SelectMany(a => a.PhienXetNghiemChiTiets)
                    //                        .Where(a => a.DichVuKyThuatBenhVien.LoaiMauXetNghiem != null)
                    //                        .GroupBy(a => new { a.NhomDichVuBenhVienId, a.PhienXetNghiemId })
                    //                        .Count(a =>
                    //                                // tất cả các mẫu đều đã được tạo
                    //                                (item.PhienXetNghiems
                    //                                         .Where(b => b.Id == a.Key.PhienXetNghiemId)
                    //                                         .SelectMany(b => b.PhienXetNghiemChiTiets.Where(c =>
                    //                                             c.NhomDichVuBenhVienId == a.Key.NhomDichVuBenhVienId &&
                    //                                             c.DichVuKyThuatBenhVien.LoaiMauXetNghiem != null))
                    //                                         .Select(b => b.DichVuKyThuatBenhVien.LoaiMauXetNghiem)
                    //                                         .GroupBy(b => new { b }).Count() - item.PhienXetNghiems
                    //                                         .Where(b => b.Id == a.Key.PhienXetNghiemId)
                    //                                         .SelectMany(b => b.MauXetNghiems
                    //                                                              .Where(c => c.NhomDichVuBenhVienId == a.Key.NhomDichVuBenhVienId))
                    //                                         .GroupBy(b => new { b.LoaiMauXetNghiem })
                    //                                         .Count()) == 0
                    //                                    // tất cả loại mẫu đều đạt chất lượngs
                    //                                    && item.PhienXetNghiems
                    //                                        .Where(b => b.Id == a.Key.PhienXetNghiemId)
                    //                                        .SelectMany(b => b.MauXetNghiems)
                    //                                        .Where(b => b.NhomDichVuBenhVienId == a.Key.NhomDichVuBenhVienId
                    //                                                    && (tuNgay == null || (b.ThoiDiemLayMau != null && b.ThoiDiemLayMau.Value.Date >= tuNgay.Value.Date))
                    //                                                        && (denNgay == null || (b.ThoiDiemLayMau != null && b.ThoiDiemLayMau.Value.Date <= denNgay.Value.Date))
                    //                                                    && b.PhieuGoiMauXetNghiem == null)
                    //                                        .OrderByDescending(b => b.CreatedOn)
                    //                                        .GroupBy(b => new { b.LoaiMauXetNghiem })
                    //                                        .Select(loaiMau => new LoaiMauVo()
                    //                                        {
                    //                                            LoaiMau = loaiMau.First().LoaiMauXetNghiem,
                    //                                            DatChatLuong = loaiMau.First().DatChatLuong
                    //                                        }).All(b => b.DatChatLuong == true)
                    //                                    // có ít nhất 1 mẫu chưa được gửi mẫu
                    //                                    && item.PhienXetNghiems
                    //                                            .Where(b => b.Id == a.Key.PhienXetNghiemId)
                    //                                            .Any(b => b.MauXetNghiems
                    //                                                .Any(c =>
                    //                                                      c.NhomDichVuBenhVienId == a.Key.NhomDichVuBenhVienId
                    //                                                      && (tuNgay == null || (c.ThoiDiemLayMau != null && c.ThoiDiemLayMau.Value.Date >= tuNgay.Value.Date))
                    //                                                          && (denNgay == null || (c.ThoiDiemLayMau != null && c.ThoiDiemLayMau.Value.Date <= denNgay.Value.Date))
                    //                                                          && c.PhieuGoiMauXetNghiemId == null))),

                    CoDuKetQua = item.YeuCauDichVuKyThuats.Where(x => x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                                                      && x.NhomDichVuBenhVien.NhomDichVuBenhVienChaId != null
                                                                      && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                                      && (x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan
                                                                          || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan
                                                                          || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
                                                         .All(x => x.PhienXetNghiemChiTiets.Any())
                                 && item.PhienXetNghiems.Any()
                                 && item.PhienXetNghiems.All(x => x.ThoiDiemKetLuan != null),
                    ThoiDiemTiepNhan = item.ThoiDiemTiepNhan,
                    BenhNhanDaNhanKetQua = item.PhienXetNghiems.Any() &&
                                           item.PhienXetNghiems.All(a => a.DaTraKetQua == true),
                    FlagChoKetQua = item.PhienXetNghiems
                                        .Where(a => a.ThoiDiemKetLuan == null)
                                        .SelectMany(a => a.MauXetNghiems.Where(b =>
                                                                                     //                                        (tuNgay == null || (b.ThoiDiemLayMau != null && b.ThoiDiemLayMau.Value.Date >= tuNgay.Value.Date))
                                                                                     //                                                                                     && (denNgay == null || (b.ThoiDiemLayMau != null && b.ThoiDiemLayMau.Value.Date <= denNgay.Value.Date))&&
                                                                                     b.DatChatLuong == true
                                                                                    && b.PhieuGoiMauXetNghiem != null
                                                                                    && b.PhieuGoiMauXetNghiem.ThoiDiemNhanMau != null)).Any(),
                    FlagDaCoKetQua = item.PhienXetNghiems
                                        .Where(a => a.ThoiDiemKetLuan != null)
                                        .SelectMany(a => a.MauXetNghiems.Where(b =>
                                                                                     //                                        (tuNgay == null || (b.ThoiDiemLayMau != null && b.ThoiDiemLayMau.Value.Date >= tuNgay.Value.Date))
                                                                                     //                                                                                     && (denNgay == null || (b.ThoiDiemLayMau != null && b.ThoiDiemLayMau.Value.Date <= denNgay.Value.Date))&&
                                                                                     b.DatChatLuong == true
                                                                                     && b.PhieuGoiMauXetNghiem != null
                                                                                     && b.PhieuGoiMauXetNghiem.ThoiDiemNhanMau != null)).Any()

                })
                .Where(x => timKiemNangCaoObj.TrangThai == null
                            || (timKiemNangCaoObj.TrangThai.ChoLayMau && x.FlagChoLay)
                            || (timKiemNangCaoObj.TrangThai.ChoGuiMau && x.FlagChoGui)
                            || (timKiemNangCaoObj.TrangThai.ChoKetQua && x.FlagChoKetQua)
                            || (timKiemNangCaoObj.TrangThai.DaCoKetQua && x.FlagDaCoKetQua)
                            || (!timKiemNangCaoObj.TrangThai.ChoLayMau && !timKiemNangCaoObj.TrangThai.ChoGuiMau &&
                                !timKiemNangCaoObj.TrangThai.ChoKetQua && !timKiemNangCaoObj.TrangThai.DaCoKetQua));

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetTotalPageForGridYeuCauTiepNhanCanLayMauXetNghiemAsyncVer2(QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new LayMauXetNghiemTimKiemNangCaoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<LayMauXetNghiemTimKiemNangCaoVo>(queryInfo.AdditionalSearchString);
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

            var query = _yeuCauTiepNhanRepository.TableNoTracking
                .ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.MaYeuCauTiepNhan, x => x.BenhNhan.MaBN, x => x.HoTen)
                .Where(x => x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                            && x.YeuCauDichVuKyThuats.Any(a =>
                                a.NhomDichVuBenhVien.NhomDichVuBenhVienChaId != null
                                && a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                && a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && (a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                                    a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                                    a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                                && (tuNgay == null || (tuNgay != null && ((a.NoiTruPhieuDieuTri == null && a.ThoiDiemChiDinh >= tuNgay.Value.Date) || (a.NoiTruPhieuDieuTri != null && a.NoiTruPhieuDieuTri.NgayDieuTri >= tuNgay.Value.Date))))
                                && (denNgay == null || (denNgay != null && ((a.NoiTruPhieuDieuTri == null && a.ThoiDiemChiDinh <= denNgay.Value.Date) || (a.NoiTruPhieuDieuTri != null && a.NoiTruPhieuDieuTri.NgayDieuTri <= denNgay.Value.Date)))))
                            && (timKiemNangCaoObj.TrangThai == null
                                || (timKiemNangCaoObj.TrangThai.ChoLayMau
                                    && (x.YeuCauDichVuKyThuats.Any(a => a.NhomDichVuBenhVien.NhomDichVuBenhVienChaId != null
                                                                      && a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                                                      && a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                                      && (a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                                                                          a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                                                                          a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                                                                      && !a.PhienXetNghiemChiTiets.Any()
                                                                      && (tuNgay == null || (tuNgay != null && ((a.NoiTruPhieuDieuTri == null && a.ThoiDiemChiDinh >= tuNgay.Value.Date) || (a.NoiTruPhieuDieuTri != null && a.NoiTruPhieuDieuTri.NgayDieuTri >= tuNgay.Value.Date))))
                                                                      && (denNgay == null || (denNgay != null && ((a.NoiTruPhieuDieuTri == null && a.ThoiDiemChiDinh <= denNgay.Value.Date) || (a.NoiTruPhieuDieuTri != null && a.NoiTruPhieuDieuTri.NgayDieuTri <= denNgay.Value.Date)))))



                                         // nhóm bất kỳ có 1 mẫu chưa được tạo
                                         || x.PhienXetNghiems.Any(a => a.PhienXetNghiemChiTiets
                                                                        .Any(b => a.MauXetNghiems.All(c => c.LoaiMauXetNghiem != b.DichVuKyThuatBenhVien.LoaiMauXetNghiem)
                                                                                && a.MauXetNghiems.Any(c => c.LoaiMauXetNghiem == b.DichVuKyThuatBenhVien.LoaiMauXetNghiem
                                                                                                            && c.PhieuGoiMauXetNghiemId == null)))

                                        // nhóm bất kỳ có 1 mẫu bị từ chối (lấy mẫu cuối cùng theo từng nhóm dịch vụ)
                                        || x.PhienXetNghiems.Any(a => a.MauXetNghiems
                                                                       .Any(b => a.MauXetNghiems.Where(aa => aa.LoaiMauXetNghiem == b.LoaiMauXetNghiem).OrderByDescending(aa => aa.Id).Select(aa => aa.DatChatLuong).FirstOrDefault() == false
                                                                                && a.MauXetNghiems.Any(c => c.LoaiMauXetNghiem == b.LoaiMauXetNghiem
                                                                                                            && c.PhieuGoiMauXetNghiemId == null)))

                                    //&& x.PhienXetNghiems.Any(a => a.MauXetNghiems.Any(c => c.PhieuGoiMauXetNghiemId == null))

                                    ))
                                || (timKiemNangCaoObj.TrangThai.ChoKetQua && x.PhienXetNghiems
                                                                                .Where(a => a.ThoiDiemKetLuan == null)
                                                                                .SelectMany(a => a.MauXetNghiems.Where(b =>
                                                                                    b.DatChatLuong == true
                                                                                    && b.PhieuGoiMauXetNghiem != null
                                                                                    && b.PhieuGoiMauXetNghiem.ThoiDiemNhanMau != null)).Any())
                                || (timKiemNangCaoObj.TrangThai.DaCoKetQua && x.PhienXetNghiems
                                                                                .Where(a => a.ThoiDiemKetLuan != null)
                                                                                .SelectMany(a => a.MauXetNghiems.Where(b =>
                                                                                    b.DatChatLuong == true
                                                                                    && b.PhieuGoiMauXetNghiem != null
                                                                                    && b.PhieuGoiMauXetNghiem.ThoiDiemNhanMau != null)).Any())
                                || (!timKiemNangCaoObj.TrangThai.ChoLayMau && !timKiemNangCaoObj.TrangThai.ChoGuiMau &&
                                    !timKiemNangCaoObj.TrangThai.ChoKetQua && !timKiemNangCaoObj.TrangThai.DaCoKetQua)))
                .Include(x => x.BenhNhan)
                .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.NoiTruPhieuDieuTri)
                .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.DichVuKyThuatBenhVien)
                .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.NhomDichVuBenhVien)
                .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.PhienXetNghiemChiTiets)
                .Include(x => x.PhienXetNghiems).ThenInclude(y => y.PhienXetNghiemChiTiets).ThenInclude(z => z.DichVuKyThuatBenhVien)
                .Include(x => x.PhienXetNghiems).ThenInclude(y => y.MauXetNghiems).ThenInclude(z => z.PhieuGoiMauXetNghiem);

            //var result = query
            //    .Select(item => new LayMauXetNghiemYeuCauTiepNhanGridVo()
            //    {
            //        Id = item.Id,
            //        MaTiepNhan = item.MaYeuCauTiepNhan,
            //        MaBenhNhan = item.BenhNhan.MaBN,
            //        HoTen = item.HoTen,
            //        NamSinh = item.NamSinh,
            //        GioiTinh = item.GioiTinh.GetDescription(),
            //        DiaChi = item.DiaChiDayDu,
            //        SoDienThoai = item.SoDienThoai,
            //        SoDienThoaiDisplay = item.SoDienThoaiDisplay,

            //        // todo: update thêm số lượng nhóm chờ lấy
            //        SoLuongChoLay = item.YeuCauDichVuKyThuats
            //            .Where(a =>
            //                        a.NhomDichVuBenhVien.NhomDichVuBenhVienChaId != null
            //                        //a.NhomDichVuBenhVien.NhomDichVuBenhVienCha.Ma == "XN"
            //                        && a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
            //                        && a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
            //                        && (a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
            //                            a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
            //                            a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
            //                        && !a.PhienXetNghiemChiTiets.Any()
            //                && (tuNgay == null || (tuNgay != null && ((a.NoiTruPhieuDieuTri == null && a.ThoiDiemChiDinh >= tuNgay.Value.Date) || (a.NoiTruPhieuDieuTri != null && a.NoiTruPhieuDieuTri.NgayDieuTri >= tuNgay.Value.Date))))
            //                && (denNgay == null || (denNgay != null && ((a.NoiTruPhieuDieuTri == null && a.ThoiDiemChiDinh <= denNgay.Value.Date) || (a.NoiTruPhieuDieuTri != null && a.NoiTruPhieuDieuTri.NgayDieuTri <= denNgay.Value.Date)))))
            //            .Select(a => a.NhomDichVuBenhVienId)
            //            .Union(
            //                item.PhienXetNghiems
            //                    .SelectMany(a => a.PhienXetNghiemChiTiets)
            //                    .Where(a => a.DichVuKyThuatBenhVien.LoaiMauXetNghiem != null)
            //                    .GroupBy(a => new { a.NhomDichVuBenhVienId, a.PhienXetNghiemId })
            //                    .Where(a => (
            //                                    // nhóm bất kỳ có 1 mẫu chưa được tạo
            //                                    (item.PhienXetNghiems
            //                                      .Where(b => b.Id == a.Key.PhienXetNghiemId)
            //                                      .SelectMany(b => b.PhienXetNghiemChiTiets.Where(c =>
            //                                          c.NhomDichVuBenhVienId == a.Key.NhomDichVuBenhVienId &&
            //                                          c.DichVuKyThuatBenhVien.LoaiMauXetNghiem != null))
            //                                      .Select(b => b.DichVuKyThuatBenhVien.LoaiMauXetNghiem)
            //                                      .GroupBy(b => new { b }).Count() - item.PhienXetNghiems
            //                                      .Where(b => b.Id == a.Key.PhienXetNghiemId)
            //                                      .SelectMany(b => b.MauXetNghiems.Where(c =>
            //                                          c.NhomDichVuBenhVienId == a.Key.NhomDichVuBenhVienId
            //                                              //                                                          && (tuNgay == null || (c.ThoiDiemLayMau != null && c.ThoiDiemLayMau.Value.Date >= tuNgay.Value.Date)) 
            //                                              //                                                          && (denNgay == null || (c.ThoiDiemLayMau != null && c.ThoiDiemLayMau.Value.Date <= denNgay.Value.Date))
            //                                              ))
            //                                      .GroupBy(b => new { b.LoaiMauXetNghiem })
            //                                      .Count()) != 0

            //                                 // nhóm bất kỳ có 1 mẫu bị từ chối (lấy mẫu cuối cùng theo từng nhóm dịch vụ)
            //                                 || item.PhienXetNghiems
            //                                     .Where(b => b.Id == a.Key.PhienXetNghiemId)
            //                                     .SelectMany(b => b.MauXetNghiems)
            //                                     .Where(b => b.NhomDichVuBenhVienId == a.Key.NhomDichVuBenhVienId
            //                                                     //                                                             && (tuNgay == null || (b.ThoiDiemLayMau != null && b.ThoiDiemLayMau.Value.Date >= tuNgay.Value.Date)) 
            //                                                     //                                                                 && (denNgay == null || (b.ThoiDiemLayMau != null && b.ThoiDiemLayMau.Value.Date <= denNgay.Value.Date))
            //                                                     )
            //                                     .OrderByDescending(b => b.CreatedOn)
            //                                     .GroupBy(b => new { b.LoaiMauXetNghiem })
            //                                     .Select(loaiMaub => new LoaiMauVo()
            //                                     {
            //                                         LoaiMau = loaiMaub.First().LoaiMauXetNghiem,
            //                                         DatChatLuong = loaiMaub.First().DatChatLuong
            //                                     }).Any(b => b.DatChatLuong == false))
            //                                // nhóm bất kỳ có 1 mẫu chưa được gửi mẫu
            //                                && item.PhienXetNghiems.Where(b => b.Id == a.Key.PhienXetNghiemId)
            //                                    .SelectMany(b => b.MauXetNghiems.Where(c =>
            //                                        c.NhomDichVuBenhVienId == a.Key.NhomDichVuBenhVienId))
            //                                    .Any(c => c.PhieuGoiMauXetNghiemId == null))
            //                    .Select(a => a.Key.NhomDichVuBenhVienId)
            //            )
            //            .Distinct().Count(),
            //        CoDuKetQua = item.YeuCauDichVuKyThuats.Where(x => x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
            //                                                          && x.NhomDichVuBenhVien.NhomDichVuBenhVienChaId != null
            //                                                          && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
            //                                                          && (x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan
            //                                                              || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan
            //                                                              || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
            //                                             .All(x => x.PhienXetNghiemChiTiets.Any())
            //                     && item.PhienXetNghiems.Any()
            //                     && item.PhienXetNghiems.All(x => x.ThoiDiemKetLuan != null),
            //        ThoiDiemTiepNhan = item.ThoiDiemTiepNhan,
            //        BenhNhanDaNhanKetQua = item.PhienXetNghiems.Any() &&
            //                               item.PhienXetNghiems.All(a => a.DaTraKetQua == true),
            //        FlagChoKetQua = item.PhienXetNghiems
            //                            .Where(a => a.ThoiDiemKetLuan == null)
            //                            .SelectMany(a => a.MauXetNghiems.Where(b =>
            //                                                                         //                                        (tuNgay == null || (b.ThoiDiemLayMau != null && b.ThoiDiemLayMau.Value.Date >= tuNgay.Value.Date))
            //                                                                         //                                                                                     && (denNgay == null || (b.ThoiDiemLayMau != null && b.ThoiDiemLayMau.Value.Date <= denNgay.Value.Date))&& 
            //                                                                         b.DatChatLuong == true && b.PhieuGoiMauXetNghiem?.ThoiDiemNhanMau != null)).Any(),
            //        FlagDaCoKetQua = item.PhienXetNghiems
            //                            .Where(a => a.ThoiDiemKetLuan != null)
            //                            .SelectMany(a => a.MauXetNghiems.Where(b =>
            //                                                                         //                                        (tuNgay == null || (b.ThoiDiemLayMau != null && b.ThoiDiemLayMau.Value.Date >= tuNgay.Value.Date))
            //                                                                         //                                                                                     && (denNgay == null || (b.ThoiDiemLayMau != null && b.ThoiDiemLayMau.Value.Date <= denNgay.Value.Date))&& 
            //                                                                         b.DatChatLuong == true && b.PhieuGoiMauXetNghiem?.ThoiDiemNhanMau != null)).Any()

            //    }).ToArray();

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetTotalPageForGridYeuCauTiepNhanCanLayMauXetNghiemAsyncVer3(QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new LayMauXetNghiemTimKiemNangCaoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<LayMauXetNghiemTimKiemNangCaoVo>(queryInfo.AdditionalSearchString);
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

            //var khoaXetNghiem = _cauHinhService.GetSetting("CauHinhXetNghiem.KhoaXetNghiem");
            //var khoaXetNghiemId = long.Parse(khoaXetNghiem.Value);
            //var laNhanVienKhoaXetNghiem = await _nhanVienRepository.TableNoTracking
            //    .AnyAsync(x => x.Id == _userAgentHelper.GetCurrentUserId()
            //                   && x.KhoaPhongNhanViens.Any()
            //                   && x.KhoaPhongNhanViens.Any(y => y.KhoaPhongId == khoaXetNghiemId));
            var laNhanVienKhoaXetNghiem = await KiemTraNhanVienThuocKhoaXetNghiemAsync();
            var phongHienTai = new Core.Domain.Entities.PhongBenhViens.PhongBenhVien();
            if (!laNhanVienKhoaXetNghiem)
            {
                phongHienTai = _phongBenhVienRepository.TableNoTracking
                    .First(x => x.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            }

            var query = _yeuCauTiepNhanRepository.TableNoTracking
                .ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.MaYeuCauTiepNhan, x => x.BenhNhan.MaBN, x => x.HoTen)
                .Where(x => x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                            //&& x.YeuCauDichVuKyThuats.Any(a =>
                            //    a.NhomDichVuBenhVien.NhomDichVuBenhVienChaId != null
                            //    && a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                            //    && a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                            //    && (a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                            //        a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                            //        a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                            //    && (tuNgay == null || (tuNgay != null && ((a.NoiTruPhieuDieuTri == null && a.ThoiDiemChiDinh >= tuNgay.Value.Date) || (a.NoiTruPhieuDieuTri != null && a.NoiTruPhieuDieuTri.NgayDieuTri >= tuNgay.Value.Date))))
                            //    && (denNgay == null || (denNgay != null && ((a.NoiTruPhieuDieuTri == null && a.ThoiDiemChiDinh <= denNgay.Value.Date) || (a.NoiTruPhieuDieuTri != null && a.NoiTruPhieuDieuTri.NgayDieuTri <= denNgay.Value.Date))))

                            //    // bổ sung load theo tài khoản nhân viên đang login
                            //    && (laNhanVienKhoaXetNghiem || (a.NoiChiDinh != null && a.NoiChiDinh.KhoaPhongId == phongHienTai.KhoaPhongId)))
                            //&& (timKiemNangCaoObj.TrangThai == null
                            //    || (timKiemNangCaoObj.TrangThai.ChoLayMau
                            //        && (x.YeuCauDichVuKyThuats.Any(a => 
                            //                //a.NhomDichVuBenhVien.NhomDichVuBenhVienChaId != null
                            //                                            a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                            //                                          && a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                            //                                          && (a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                            //                                              a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                            //                                              a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                            //                                          && !a.PhienXetNghiemChiTiets.Any()
                            //                                            && (tuNgay == null || (tuNgay != null && ((a.ThoiDiemDangKy == null && a.ThoiDiemChiDinh >= tuNgay.Value.Date) || (a.ThoiDiemDangKy != null && a.ThoiDiemDangKy >= tuNgay.Value.Date))))
                            //                                            && (denNgay == null || (denNgay != null && ((a.ThoiDiemDangKy == null && a.ThoiDiemChiDinh <= denNgay.Value.Date) || (a.ThoiDiemDangKy != null && a.ThoiDiemDangKy.Date <= denNgay.Value.Date))))
                            //                                        // bổ sung load theo tài khoản nhân viên đang login
                            //                                        && (laNhanVienKhoaXetNghiem || (a.NoiChiDinh != null && a.NoiChiDinh.KhoaPhongId == phongHienTai.KhoaPhongId)))
                            //            //|| x.YeuCauDichVuKyThuats.Any(a => a.NhomDichVuBenhVien.NhomDichVuBenhVienChaId != null
                            //            //                                   && a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                            //            //                                   && a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                            //            //                                   && (a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                            //            //                                       a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                            //            //                                       a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                            //            //                                   && a.PhienXetNghiemChiTiets.Any(b => b.ThoiDiemNhanMau == null && b.ThoiDiemLayMau != null)
                            //            //                                   && (tuNgay == null || (tuNgay != null && ((a.NoiTruPhieuDieuTri == null && a.ThoiDiemChiDinh >= tuNgay.Value.Date) || (a.NoiTruPhieuDieuTri != null && a.NoiTruPhieuDieuTri.NgayDieuTri >= tuNgay.Value.Date))))
                            //            //                                   && (denNgay == null || (denNgay != null && ((a.NoiTruPhieuDieuTri == null && a.ThoiDiemChiDinh <= denNgay.Value.Date) || (a.NoiTruPhieuDieuTri != null && a.NoiTruPhieuDieuTri.NgayDieuTri <= denNgay.Value.Date))))
                            //            //                                   // bổ sung load theo tài khoản nhân viên đang login
                            //            //                                   && (laNhanVienKhoaXetNghiem || (a.NoiChiDinh != null && a.NoiChiDinh.KhoaPhongId == phongHienTai.KhoaPhongId)))
                            //        )
                            //        )
                            //    || (timKiemNangCaoObj.TrangThai.DaLayMau
                            //        && x.YeuCauDichVuKyThuats.Any(a =>
                            //            //a.NhomDichVuBenhVien.NhomDichVuBenhVienChaId != null
                            //            a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                            //            && a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                            //            && (a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                            //                a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                            //                a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                            //            && a.PhienXetNghiemChiTiets.Any(b => b.ThoiDiemLayMau != null)
                            //            && (tuNgay == null || (tuNgay != null && ((a.ThoiDiemDangKy == null && a.ThoiDiemChiDinh >= tuNgay.Value.Date) || (a.ThoiDiemDangKy != null && a.ThoiDiemDangKy >= tuNgay.Value.Date))))
                            //            && (denNgay == null || (denNgay != null && ((a.ThoiDiemDangKy == null && a.ThoiDiemChiDinh <= denNgay.Value.Date) || (a.ThoiDiemDangKy != null && a.ThoiDiemDangKy.Date <= denNgay.Value.Date))))
                            //            // bổ sung load theo tài khoản nhân viên đang login
                            //            && (laNhanVienKhoaXetNghiem || (a.NoiChiDinh != null && a.NoiChiDinh.KhoaPhongId == phongHienTai.KhoaPhongId))))
                            //    //|| (timKiemNangCaoObj.TrangThai.ChoKetQua && x.PhienXetNghiems
                            //    //                                                .Where(a => a.ThoiDiemKetLuan == null
                            //    //                                                            && a.PhienXetNghiemChiTiets.Any(b => b.YeuCauDichVuKyThuat.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                            //    //                                                                                                 && (b.YeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                            //    //                                                                                                     b.YeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                            //    //                                                                                                     b.YeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                            //    //                                                                                                 && (laNhanVienKhoaXetNghiem || (b.YeuCauDichVuKyThuat.NoiChiDinh != null && b.YeuCauDichVuKyThuat.NoiChiDinh.KhoaPhongId == phongHienTai.KhoaPhongId))))
                            //    //                                                .SelectMany(a => a.MauXetNghiems.Where(b =>
                            //    //                                                    b.DatChatLuong == true
                            //    //                                                    && b.PhieuGoiMauXetNghiem != null
                            //    //                                                    && b.PhieuGoiMauXetNghiem.ThoiDiemNhanMau != null)).Any())
                            //    //|| (timKiemNangCaoObj.TrangThai.DaCoKetQua && x.PhienXetNghiems
                            //    //                                                .Where(a => a.ThoiDiemKetLuan != null
                            //    //                                                            && a.PhienXetNghiemChiTiets.Any(b => b.YeuCauDichVuKyThuat.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                            //    //                                                                                                 && (b.YeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                            //    //                                                                                                     b.YeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                            //    //                                                                                                     b.YeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                            //    //                                                                                                 && (laNhanVienKhoaXetNghiem || (b.YeuCauDichVuKyThuat.NoiChiDinh != null && b.YeuCauDichVuKyThuat.NoiChiDinh.KhoaPhongId == phongHienTai.KhoaPhongId))))
                            //    //                                                .SelectMany(a => a.MauXetNghiems.Where(b =>
                            //    //                                                    b.DatChatLuong == true
                            //    //                                                    && b.PhieuGoiMauXetNghiem != null
                            //    //                                                    && b.PhieuGoiMauXetNghiem.ThoiDiemNhanMau != null)).Any())
                            //    || (!timKiemNangCaoObj.TrangThai.ChoLayMau 
                            //        //&& !timKiemNangCaoObj.TrangThai.ChoGuiMau 
                            //        //&& !timKiemNangCaoObj.TrangThai.ChoKetQua 
                            //        //&& !timKiemNangCaoObj.TrangThai.DaCoKetQua
                            //        && !timKiemNangCaoObj.TrangThai.DaLayMau
                            //        && x.YeuCauDichVuKyThuats.Any(a =>
                            //            a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                            //            && a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                            //            && (a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                            //                a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                            //                a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                            //            && (tuNgay == null || (tuNgay != null && ((a.ThoiDiemDangKy == null && a.ThoiDiemChiDinh >= tuNgay.Value.Date) || (a.ThoiDiemDangKy != null && a.ThoiDiemDangKy >= tuNgay.Value.Date))))
                            //            && (denNgay == null || (denNgay != null && ((a.ThoiDiemDangKy == null && a.ThoiDiemChiDinh <= denNgay.Value.Date) || (a.ThoiDiemDangKy != null && a.ThoiDiemDangKy.Date <= denNgay.Value.Date))))

                            //            // bổ sung load theo tài khoản nhân viên đang login
                            //            && (laNhanVienKhoaXetNghiem || (a.NoiChiDinh != null && a.NoiChiDinh.KhoaPhongId == phongHienTai.KhoaPhongId)))))

                            && x.YeuCauDichVuKyThuats.Any(a =>
                                a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                && a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && (a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                                    a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                                    a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                                && (tuNgay == null || (tuNgay != null && ((a.NoiTruPhieuDieuTriId == null && a.ThoiDiemChiDinh >= tuNgay.Value.Date) || (a.NoiTruPhieuDieuTriId != null && a.ThoiDiemDangKy >= tuNgay.Value.Date))))
                                && (denNgay == null || (denNgay != null && ((a.NoiTruPhieuDieuTriId == null && a.ThoiDiemChiDinh <= denNgay.Value.Date) || (a.NoiTruPhieuDieuTriId != null && a.ThoiDiemDangKy.Date <= denNgay.Value.Date))))
                                && (laNhanVienKhoaXetNghiem || (a.NoiChiDinh != null && a.NoiChiDinh.KhoaPhongId == phongHienTai.KhoaPhongId))

                                // kiểm tra trạng thái
                                && ((timKiemNangCaoObj.TrangThai.ChoLayMau && !a.PhienXetNghiemChiTiets.Any())
                                    || (timKiemNangCaoObj.TrangThai.DaLayMau && a.PhienXetNghiemChiTiets.Any(b => b.ThoiDiemLayMau != null))
                                    || (!timKiemNangCaoObj.TrangThai.ChoLayMau && !timKiemNangCaoObj.TrangThai.DaLayMau)))
                            )
                .Include(x => x.BenhNhan)
                //.Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.NoiTruPhieuDieuTri)
                .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.DichVuKyThuatBenhVien)
                .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.NhomDichVuBenhVien)
                .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.PhienXetNghiemChiTiets)
                .Include(x => x.PhienXetNghiems).ThenInclude(y => y.PhienXetNghiemChiTiets).ThenInclude(z => z.DichVuKyThuatBenhVien)
                .Include(x => x.PhienXetNghiems).ThenInclude(y => y.MauXetNghiems).ThenInclude(z => z.PhieuGoiMauXetNghiem);

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetTotalPageForGridYeuCauTiepNhanCanLayMauXetNghiemAsyncVer4(QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new LayMauXetNghiemTimKiemNangCaoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<LayMauXetNghiemTimKiemNangCaoVo>(queryInfo.AdditionalSearchString);
            }
            var searchStringRemoveVietnameseDiacritics = (timKiemNangCaoObj.SearchString ?? "").Trim().ToLower().RemoveVietnameseDiacritics();

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

            var query = _yeuCauTiepNhanRepository.TableNoTracking
                //.ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.MaYeuCauTiepNhan, x => x.BenhNhan.MaBN, x => x.HoTen)
                .Where(x => x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy

                            && x.YeuCauDichVuKyThuats.Any(a =>
                                a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                && a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && (a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                                    a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                                    a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                                && (tuNgay == null || (tuNgay != null && ((a.NoiTruPhieuDieuTriId == null && a.ThoiDiemChiDinh >= tuNgay.Value.Date) || (a.NoiTruPhieuDieuTriId != null && a.ThoiDiemDangKy >= tuNgay.Value.Date))))
                                && (denNgay == null || (denNgay != null && ((a.NoiTruPhieuDieuTriId == null && a.ThoiDiemChiDinh <= denNgay.Value.Date) || (a.NoiTruPhieuDieuTriId != null && a.ThoiDiemDangKy.Date <= denNgay.Value.Date))))
                                && (laNhanVienKhoaXetNghiem || (a.NoiChiDinh != null && a.NoiChiDinh.KhoaPhongId == phongHienTai.KhoaPhongId))

                                // kiểm tra trạng thái
                                && ((timKiemNangCaoObj.TrangThai.ChoLayMau && !a.PhienXetNghiemChiTiets.Any())
                                    || (timKiemNangCaoObj.TrangThai.DaLayMau && a.PhienXetNghiemChiTiets.Any(b => b.ThoiDiemLayMau != null))
                                    || (!timKiemNangCaoObj.TrangThai.ChoLayMau && !timKiemNangCaoObj.TrangThai.DaLayMau)))

                                // chuyển applylike -> search thủ công
                                && (x.MaYeuCauTiepNhan.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                                    || x.HoTen.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                                    || x.BenhNhan.MaBN.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                                    || x.PhienXetNghiems.Any(c => c.BarCodeId.Contains(searchStringRemoveVietnameseDiacritics)))
                            )
                .Include(x => x.BenhNhan)
                //.Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.NoiTruPhieuDieuTri)
                .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.DichVuKyThuatBenhVien)
                .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.NhomDichVuBenhVien)
                .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.PhienXetNghiemChiTiets)
                .Include(x => x.PhienXetNghiems).ThenInclude(y => y.PhienXetNghiemChiTiets).ThenInclude(z => z.DichVuKyThuatBenhVien)
                .Include(x => x.PhienXetNghiems).ThenInclude(y => y.MauXetNghiems).ThenInclude(z => z.PhieuGoiMauXetNghiem);

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataForGridNhomCanLayMauXetNghiemAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var timKiemNangCaoObj = new LayMauXetNghiemTimKiemNangCaoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<LayMauXetNghiemTimKiemNangCaoVo>(queryInfo.AdditionalSearchString);
            }

            long yeuCauTiepNhanId = string.IsNullOrEmpty(queryInfo.SearchTerms) ? 0 : long.Parse(queryInfo.SearchTerms);
            var query =
                // chờ lấy mẫu
                // todo: cần kiểm tra lại -> còn thiếu case
                _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(x => (yeuCauTiepNhanId == 0 || x.YeuCauTiepNhanId == yeuCauTiepNhanId)
                            && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                            && (x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan
                                || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan
                                || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                            && x.NhomDichVuBenhVien.NhomDichVuBenhVienChaId != null
                            //&& x.NhomDichVuBenhVien.NhomDichVuBenhVienCha.Ma == "XN"
                            && x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                            && x.PhienXetNghiemChiTiets.All(a => a.YeuCauDichVuKyThuatId != x.Id))
                .GroupBy(x => new { x.NhomDichVuBenhVienId })
                .Select(item => new NhomCanLayMauXetNghiemGridVo()
                {
                    Id = item.First().YeuCauTiepNhanId,
                    YeuCauTiepNhanId = item.First().YeuCauTiepNhanId, //yeuCauTiepNhanId,
                    BenhNhanId = item.First().YeuCauTiepNhan.BenhNhanId ?? 0,
                    PhienXetNghiemId = 0,
                    NhomDichVuBenhVienId = item.First().NhomDichVuBenhVienId,
                    TenNhom = item.First().NhomDichVuBenhVien.Ten,
                    Barcode = null,
                    //TrangThai = Enums.TrangThaiLayMauXetNghiem.ChoLayMau,
                    //TenTrangThai = "Chờ lấy mẫu",
                    LoaiMaus = item
                                .Select(a => a.DichVuKyThuatBenhVien.LoaiMauXetNghiem)
                                .Where(a => a != null)
                                .GroupBy(a => new { a })
                                .Select(a => new LoaiMauVo()
                                {
                                    LoaiMau = a.First(),
                                    DatChatLuong = null
                                })
                                .Distinct().ToList(),
                    SoPhieu = null,
                    NgayLayMau = null,
                    ThoiDiemChiDinhHoacNgayDieuTri = item.First().ThoiDiemDangKy//Tram: nếu là dv nội trú thì lấy ngày điều trị, nếu là ngoại trú thì lấy thời điể chỉ định
                })
                // chờ gửi mẫu, chờ KQ, đã có KQ
                // todo: cần kiểm tra lại điều kiện
                .Union(
                    _phienXetNghiemChiTietRepository.TableNoTracking
                        .Where(x => (yeuCauTiepNhanId == 0 || x.PhienXetNghiem.YeuCauTiepNhanId == yeuCauTiepNhanId)
                                    && x.DichVuKyThuatBenhVien.LoaiMauXetNghiem != null)
                        .Select(item => new NhomCanLayMauXetNghiemGridVo()
                        {
                            Id = item.PhienXetNghiem.YeuCauTiepNhanId,
                            YeuCauTiepNhanId = item.PhienXetNghiem.YeuCauTiepNhanId,//yeuCauTiepNhanId,
                            MaTiepNhan = item.PhienXetNghiem.YeuCauTiepNhan.MaYeuCauTiepNhan,
                            BenhNhanId = item.PhienXetNghiem.YeuCauTiepNhan.BenhNhanId ?? 0,
                            MaBenhNhan = item.PhienXetNghiem.YeuCauTiepNhan.BenhNhan.MaBN,
                            HoTen = item.PhienXetNghiem.YeuCauTiepNhan.HoTen,
                            NamSinh = item.PhienXetNghiem.YeuCauTiepNhan.NamSinh,
                            GioiTinh = item.PhienXetNghiem.YeuCauTiepNhan.GioiTinh.GetDescription(),
                            PhienXetNghiemId = item.PhienXetNghiemId,
                            NhomDichVuBenhVienId = item.NhomDichVuBenhVienId,
                            TenNhom = item.NhomDichVuBenhVien.Ten,
                            Barcode = item.PhienXetNghiem.MauXetNghiems.Any() ?
                                item.PhienXetNghiem.MauXetNghiems
                                .Where(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId)
                                .OrderByDescending(a => a.CreatedOn)
                                .Select(a => a.BarCodeId)
                                .FirstOrDefault() : null,
                            SoLuongMauDaTao = item.PhienXetNghiem.MauXetNghiems.Any() ? item.PhienXetNghiem.MauXetNghiems
                                .Where(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId)
                                .GroupBy(a => new { a.LoaiMauXetNghiem })
                                .Count() : 0,
                            LoaiMau = new LoaiMauVo()
                            {
                                LoaiMau = item.DichVuKyThuatBenhVien.LoaiMauXetNghiem,
                                DatChatLuong = item.PhienXetNghiem.MauXetNghiems.Any(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId) ?
                                        item.PhienXetNghiem.MauXetNghiems
                                        .Where(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId
                                                    && a.LoaiMauXetNghiem == item.DichVuKyThuatBenhVien.LoaiMauXetNghiem)
                                        .OrderByDescending(a => a.CreatedOn)
                                        .Select(a => a.DatChatLuong)
                                        .FirstOrDefault() : null,
                                TenNhanVienXetKhongDat = item.PhienXetNghiem.MauXetNghiems.Any(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId) ?
                                    item.PhienXetNghiem.MauXetNghiems
                                        .Where(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId
                                                    && a.LoaiMauXetNghiem == item.DichVuKyThuatBenhVien.LoaiMauXetNghiem)
                                        .OrderByDescending(a => a.CreatedOn)
                                        .Select(a => a.NhanVienXetKhongDatId == null ? null : a.NhanVienXetKhongDat.User.HoTen)
                                        .FirstOrDefault() : null,
                                ThoiDiemXetKhongDat = item.PhienXetNghiem.MauXetNghiems.Any(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId) ?
                                    item.PhienXetNghiem.MauXetNghiems
                                        .Where(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId
                                                    && a.LoaiMauXetNghiem == item.DichVuKyThuatBenhVien.LoaiMauXetNghiem)
                                        .OrderByDescending(a => a.CreatedOn)
                                        .Select(a => a.ThoiDiemXetKhongDat)
                                        .FirstOrDefault() : null,
                                LyDoKhongDat = item.PhienXetNghiem.MauXetNghiems.Any(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId) ?
                                    item.PhienXetNghiem.MauXetNghiems
                                        .Where(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId
                                                    && a.LoaiMauXetNghiem == item.DichVuKyThuatBenhVien.LoaiMauXetNghiem)
                                        .OrderByDescending(a => a.CreatedOn)
                                        .Select(a => a.LyDoKhongDat)
                                        .FirstOrDefault() : null,
                            },
                            SoPhieu = item.PhienXetNghiem.MauXetNghiems.Any(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId) ?
                                item.PhienXetNghiem.MauXetNghiems
                                .Where(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId)
                                .OrderByDescending(a => a.CreatedOn)
                                .Select(a => a.PhieuGoiMauXetNghiem == null ? null : a.PhieuGoiMauXetNghiem.SoPhieu)
                                .FirstOrDefault() : null,
                            PhieuGuiMauXetNghiemId = item.PhienXetNghiem.MauXetNghiems.Any(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId) ?
                                item.PhienXetNghiem.MauXetNghiems
                                    .Where(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId)
                                    .OrderByDescending(a => a.CreatedOn)
                                    .Select(a => a.PhieuGoiMauXetNghiem == null ? (long?)null : a.PhieuGoiMauXetNghiem.Id)
                                    .FirstOrDefault() : (long?)null,
                            NgayLayMau = item.PhienXetNghiem.MauXetNghiems.Any(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId) ?
                                    item.PhienXetNghiem.MauXetNghiems
                                    .Where(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId)
                                    .OrderByDescending(a => a.CreatedOn)
                                    .Select(a => a.ThoiDiemLayMau)
                                    .FirstOrDefault() : null,
                            TenNguoiLayMau = item.PhienXetNghiem.MauXetNghiems.Any(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId) ?
                                item.PhienXetNghiem.MauXetNghiems
                                    .Where(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId)
                                    .OrderByDescending(a => a.CreatedOn)
                                    .Select(a => a.NhanVienLayMauId == null ? null : a.NhanVienLayMau.User.HoTen)
                                    .FirstOrDefault() : null,
                            NgayGui = item.PhienXetNghiem.MauXetNghiems.Any(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId) ?
                                item.PhienXetNghiem.MauXetNghiems
                                    .Where(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId)
                                    .OrderByDescending(a => a.CreatedOn)
                                    .Select(a => a.PhieuGoiMauXetNghiemId == null ? (DateTime?)null : a.PhieuGoiMauXetNghiem.ThoiDiemGoiMau)
                                    .FirstOrDefault() : null,
                            TenNGuoiGui = item.PhienXetNghiem.MauXetNghiems.Any(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId) ?
                                item.PhienXetNghiem.MauXetNghiems
                                    .Where(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId)
                                    .OrderByDescending(a => a.CreatedOn)
                                    .Select(a => a.PhieuGoiMauXetNghiemId == null ? null : a.PhieuGoiMauXetNghiem.NhanVienGoiMau.User.HoTen)
                                    .FirstOrDefault() : null,
                            NgayNhan = item.PhienXetNghiem.MauXetNghiems.Any(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId) ?
                                item.PhienXetNghiem.MauXetNghiems
                                    .Where(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId)
                                    .OrderByDescending(a => a.CreatedOn)
                                    .Select(a => a.PhieuGoiMauXetNghiemId == null ? (DateTime?)null : a.PhieuGoiMauXetNghiem.ThoiDiemNhanMau)
                                    .FirstOrDefault() : null,
                            TenNguoiNhan = item.PhienXetNghiem.MauXetNghiems.Any(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId) ?
                                item.PhienXetNghiem.MauXetNghiems
                                    .Where(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId)
                                    .OrderByDescending(a => a.CreatedOn)
                                    .Select(a => a.PhieuGoiMauXetNghiemId == null ? null : (a.PhieuGoiMauXetNghiem.NhanVienNhanMauId == null ? null : a.PhieuGoiMauXetNghiem.NhanVienNhanMau.User.HoTen))
                                    .FirstOrDefault() : null,
                            NgayThucHien = item.PhienXetNghiem.ThoiDiemBatDau,//item.YeuCauDichVuKyThuat.ThoiDiemThucHien,
                            TenNguoiThucHien = item.PhienXetNghiem.NhanVienThucHienId == null ? null : item.PhienXetNghiem.NhanVienThucHien.User.HoTen,//item.YeuCauDichVuKyThuat.NhanVienThucHienId == null ? null : item.YeuCauDichVuKyThuat.NhanVienThucHien.User.HoTen,
                            NgayDuyet = item.PhienXetNghiem.ThoiDiemKetLuan,
                            TenNguoiDuyet = item.PhienXetNghiem.NhanVienKetLuanId == null ? null : item.PhienXetNghiem.NhanVienKetLuan.User.HoTen,
                            ThoiDiemChiDinhHoacNgayDieuTri = item != null && item.YeuCauDichVuKyThuat != null ? item.YeuCauDichVuKyThuat.ThoiDiemDangKy : (DateTime?)null
                        })
                    .GroupBy(x => new { x.PhienXetNghiemId, x.NhomDichVuBenhVienId })
                        .Select(item => new NhomCanLayMauXetNghiemGridVo()
                        {
                            Id = item.First().YeuCauTiepNhanId,
                            YeuCauTiepNhanId = item.First().YeuCauTiepNhanId,
                            MaTiepNhan = item.First().MaTiepNhan,
                            BenhNhanId = item.First().BenhNhanId,
                            MaBenhNhan = item.First().MaBenhNhan,
                            HoTen = item.First().HoTen,
                            NamSinh = item.First().NamSinh,
                            GioiTinh = item.First().GioiTinh,
                            PhienXetNghiemId = item.Key.PhienXetNghiemId,
                            NhomDichVuBenhVienId = item.Key.NhomDichVuBenhVienId,
                            TenNhom = item.First().TenNhom,
                            Barcode = item.First().Barcode,
                            SoLuongMauDaTao = item.First().SoLuongMauDaTao,//item.Sum(a => a.SoLuongMauDaTao),
                            LoaiMaus = item.Select(a => a.LoaiMau)
                                .Where(a => a.LoaiMau != null)
                                .GroupBy(a => new { a.LoaiMau })
                                .Select(itemLoaiMau => new LoaiMauVo()
                                {
                                    LoaiMau = itemLoaiMau.First().LoaiMau,
                                    DatChatLuong = itemLoaiMau.First().DatChatLuong,
                                    TenNhanVienXetKhongDat = itemLoaiMau.First().TenNhanVienXetKhongDat,
                                    ThoiDiemXetKhongDat = itemLoaiMau.First().ThoiDiemXetKhongDat,
                                    LyDoKhongDat = itemLoaiMau.First().LyDoKhongDat
                                })
                                .ToList(),
                            SoPhieu = item.First().SoPhieu,
                            PhieuGuiMauXetNghiemId = item.First().PhieuGuiMauXetNghiemId,
                            NgayLayMau = item.First().NgayLayMau,
                            TenNguoiLayMau = item.First().TenNguoiLayMau,
                            NgayGui = item.First().NgayGui,
                            TenNGuoiGui = item.First().TenNGuoiGui,
                            NgayNhan = item.First().NgayNhan,
                            TenNguoiNhan = item.First().TenNguoiNhan,
                            NgayThucHien = item.First().NgayThucHien,
                            TenNguoiThucHien = item.First().TenNguoiThucHien,
                            NgayDuyet = item.First().NgayDuyet,
                            TenNguoiDuyet = item.First().TenNguoiDuyet,
                            ThoiDiemChiDinhHoacNgayDieuTri = item.First().ThoiDiemChiDinhHoacNgayDieuTri
                        })
                );


            // kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.TuNgayDenNgay != null && (!string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay) || !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay)))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);

                query = query.Where(p => (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay) || (p.ThoiDiemChiDinhHoacNgayDieuTri != null && p.ThoiDiemChiDinhHoacNgayDieuTri.Value.Date >= tuNgay.Date))
                                         && (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay) || (p.ThoiDiemChiDinhHoacNgayDieuTri != null && p.ThoiDiemChiDinhHoacNgayDieuTri.Value.Date <= denNgay.Date)));
            }

            if (timKiemNangCaoObj.TrangThai != null && (timKiemNangCaoObj.TrangThai.ChoLayMau || timKiemNangCaoObj.TrangThai.ChoGuiMau ||
                timKiemNangCaoObj.TrangThai.ChoKetQua || timKiemNangCaoObj.TrangThai.DaCoKetQua))
            {
                query = query.Where(x =>
                    (timKiemNangCaoObj.TrangThai.ChoLayMau && x.TrangThai == Enums.TrangThaiLayMauXetNghiem.ChoLayMau)
                    || (timKiemNangCaoObj.TrangThai.ChoGuiMau && x.TrangThai == Enums.TrangThaiLayMauXetNghiem.ChoGuiMau)
                    || (timKiemNangCaoObj.TrangThai.ChoKetQua && x.TrangThai == Enums.TrangThaiLayMauXetNghiem.ChoKetQua)
                    || (timKiemNangCaoObj.TrangThai.DaCoKetQua && x.TrangThai == Enums.TrangThaiLayMauXetNghiem.DaCoKetQua));
            }

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).ThenBy(x => x.TrangThai).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource
            {
                Data = queryTask.Result,
                TotalRowCount = countTask.Result
            };
        }
        public async Task<GridDataSource> GetTotalPageForGridNhomCanLayMauXetNghiemAsync(QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new LayMauXetNghiemTimKiemNangCaoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<LayMauXetNghiemTimKiemNangCaoVo>(queryInfo.AdditionalSearchString);
            }

            long yeuCauTiepNhanId = string.IsNullOrEmpty(queryInfo.SearchTerms) ? 0 : long.Parse(queryInfo.SearchTerms);

            var query =
                // chờ lấy mẫu
                // todo: cần kiểm tra lại -> còn thiếu case
                _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId
                            && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                            && (x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                                x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                                x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                            && x.NhomDichVuBenhVien.NhomDichVuBenhVienChaId != null
                            //&& x.NhomDichVuBenhVien.NhomDichVuBenhVienCha.Ma == "XN"
                            && x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                            && x.PhienXetNghiemChiTiets.All(a => a.YeuCauDichVuKyThuatId != x.Id))
                .GroupBy(x => new { x.NhomDichVuBenhVienId })
                .Select(item => new NhomCanLayMauXetNghiemGridVo()
                {
                    YeuCauTiepNhanId = yeuCauTiepNhanId,
                    BenhNhanId = item.First().YeuCauTiepNhan.BenhNhanId ?? 0,
                    PhienXetNghiemId = 0,
                    NhomDichVuBenhVienId = item.First().NhomDichVuBenhVienId,
                    TenNhom = item.First().NhomDichVuBenhVien.Ten,
                    Barcode = null,
                    //TrangThai = Enums.TrangThaiLayMauXetNghiem.ChoLayMau,
                    //TenTrangThai = "Chờ lấy mẫu",
                    LoaiMaus = item
                                .Select(a => a.DichVuKyThuatBenhVien.LoaiMauXetNghiem)
                                .Where(a => a != null)
                                .GroupBy(a => new { a })
                                .Select(a => new LoaiMauVo()
                                {
                                    LoaiMau = a.First(),
                                    DatChatLuong = null
                                })
                                .Distinct().ToList(),
                    SoPhieu = null,
                    NgayLayMau = null,
                    ThoiDiemChiDinhHoacNgayDieuTri = item.First().ThoiDiemDangKy//Tram: nếu là dv nội trú thì lấy ngày điều trị, nếu là ngoại trú thì lấy thời điểm chỉ định
                })
                // chờ gửi mẫu, chờ KQ, đã có KQ
                // todo: cần kiểm tra lại điều kiện
                .Union(
                    _phienXetNghiemChiTietRepository.TableNoTracking
                        .Where(x => x.PhienXetNghiem.YeuCauTiepNhanId == yeuCauTiepNhanId
                                    && x.DichVuKyThuatBenhVien.LoaiMauXetNghiem != null)
                        .Select(item => new NhomCanLayMauXetNghiemGridVo()
                        {
                            YeuCauTiepNhanId = yeuCauTiepNhanId,
                            BenhNhanId = item.PhienXetNghiem.YeuCauTiepNhan.BenhNhanId ?? 0,
                            PhienXetNghiemId = item.PhienXetNghiemId,
                            NhomDichVuBenhVienId = item.NhomDichVuBenhVienId,
                            TenNhom = item.NhomDichVuBenhVien.Ten,
                            Barcode = item.PhienXetNghiem.MauXetNghiems.Any() ?
                                item.PhienXetNghiem.MauXetNghiems
                                .Where(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId)
                                .OrderByDescending(a => a.CreatedOn)
                                .Select(a => a.BarCodeId)
                                .FirstOrDefault() : null,
                            SoLuongMauDaTao = item.PhienXetNghiem.MauXetNghiems.Any() ? item.PhienXetNghiem.MauXetNghiems
                                .Where(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId)
                                .GroupBy(a => new { a.LoaiMauXetNghiem })
                                .Count() : 0,
                            LoaiMau = new LoaiMauVo()
                            {
                                LoaiMau = item.DichVuKyThuatBenhVien.LoaiMauXetNghiem,
                                DatChatLuong = item.PhienXetNghiem.MauXetNghiems.Any(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId) ?
                                        item.PhienXetNghiem.MauXetNghiems
                                        .Where(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId
                                                    && a.LoaiMauXetNghiem == item.DichVuKyThuatBenhVien.LoaiMauXetNghiem)
                                        .OrderByDescending(a => a.CreatedOn)
                                        .Select(a => a.DatChatLuong)
                                        .FirstOrDefault() : null,
                                TenNhanVienXetKhongDat = item.PhienXetNghiem.MauXetNghiems.Any(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId) ?
                                    item.PhienXetNghiem.MauXetNghiems
                                        .Where(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId
                                                    && a.LoaiMauXetNghiem == item.DichVuKyThuatBenhVien.LoaiMauXetNghiem)
                                        .OrderByDescending(a => a.CreatedOn)
                                        .Select(a => a.NhanVienXetKhongDatId == null ? null : a.NhanVienXetKhongDat.User.HoTen)
                                        .FirstOrDefault() : null,
                                ThoiDiemXetKhongDat = item.PhienXetNghiem.MauXetNghiems.Any(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId) ?
                                    item.PhienXetNghiem.MauXetNghiems
                                        .Where(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId
                                                    && a.LoaiMauXetNghiem == item.DichVuKyThuatBenhVien.LoaiMauXetNghiem)
                                        .OrderByDescending(a => a.CreatedOn)
                                        .Select(a => a.ThoiDiemXetKhongDat)
                                        .FirstOrDefault() : null,
                                LyDoKhongDat = item.PhienXetNghiem.MauXetNghiems.Any(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId) ?
                                    item.PhienXetNghiem.MauXetNghiems
                                        .Where(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId
                                                    && a.LoaiMauXetNghiem == item.DichVuKyThuatBenhVien.LoaiMauXetNghiem)
                                        .OrderByDescending(a => a.CreatedOn)
                                        .Select(a => a.LyDoKhongDat)
                                        .FirstOrDefault() : null,
                            },
                            SoPhieu = item.PhienXetNghiem.MauXetNghiems.Any(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId) ?
                                item.PhienXetNghiem.MauXetNghiems
                                    .Where(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId)
                                    .OrderByDescending(a => a.CreatedOn)
                                    .Select(a => a.PhieuGoiMauXetNghiem == null ? null : a.PhieuGoiMauXetNghiem.SoPhieu)
                                    .FirstOrDefault() : null,
                            PhieuGuiMauXetNghiemId = item.PhienXetNghiem.MauXetNghiems.Any(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId) ?
                                item.PhienXetNghiem.MauXetNghiems
                                    .Where(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId)
                                    .OrderByDescending(a => a.CreatedOn)
                                    .Select(a => a.PhieuGoiMauXetNghiem == null ? (long?)null : a.PhieuGoiMauXetNghiem.Id)
                                    .FirstOrDefault() : (long?)null,
                            NgayLayMau = item.PhienXetNghiem.MauXetNghiems.Any(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId) ?
                                    item.PhienXetNghiem.MauXetNghiems
                                    .Where(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId)
                                    .OrderByDescending(a => a.CreatedOn)
                                    .Select(a => a.ThoiDiemLayMau)
                                    .FirstOrDefault() : null,
                            TenNguoiLayMau = item.PhienXetNghiem.MauXetNghiems.Any(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId) ?
                                item.PhienXetNghiem.MauXetNghiems
                                    .Where(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId)
                                    .OrderByDescending(a => a.CreatedOn)
                                    .Select(a => a.NhanVienLayMauId == null ? null : a.NhanVienLayMau.User.HoTen)
                                    .FirstOrDefault() : null,
                            NgayGui = item.PhienXetNghiem.MauXetNghiems.Any(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId) ?
                                item.PhienXetNghiem.MauXetNghiems
                                    .Where(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId)
                                    .OrderByDescending(a => a.CreatedOn)
                                    .Select(a => a.PhieuGoiMauXetNghiemId == null ? (DateTime?)null : a.PhieuGoiMauXetNghiem.ThoiDiemGoiMau)
                                    .FirstOrDefault() : null,
                            TenNGuoiGui = item.PhienXetNghiem.MauXetNghiems.Any(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId) ?
                                item.PhienXetNghiem.MauXetNghiems
                                    .Where(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId)
                                    .OrderByDescending(a => a.CreatedOn)
                                    .Select(a => a.PhieuGoiMauXetNghiemId == null ? null : a.PhieuGoiMauXetNghiem.NhanVienGoiMau.User.HoTen)
                                    .FirstOrDefault() : null,
                            NgayNhan = item.PhienXetNghiem.MauXetNghiems.Any(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId) ?
                                item.PhienXetNghiem.MauXetNghiems
                                    .Where(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId)
                                    .OrderByDescending(a => a.CreatedOn)
                                    .Select(a => a.PhieuGoiMauXetNghiemId == null ? (DateTime?)null : a.PhieuGoiMauXetNghiem.ThoiDiemNhanMau)
                                    .FirstOrDefault() : null,
                            TenNguoiNhan = item.PhienXetNghiem.MauXetNghiems.Any(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId) ?
                                item.PhienXetNghiem.MauXetNghiems
                                    .Where(a => a.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId)
                                    .OrderByDescending(a => a.CreatedOn)
                                    .Select(a => a.PhieuGoiMauXetNghiemId == null ? null : (a.PhieuGoiMauXetNghiem.NhanVienNhanMauId == null ? null : a.PhieuGoiMauXetNghiem.NhanVienNhanMau.User.HoTen))
                                    .FirstOrDefault() : null,
                            NgayThucHien = item.PhienXetNghiem.ThoiDiemBatDau,//item.YeuCauDichVuKyThuat.ThoiDiemThucHien,
                            TenNguoiThucHien = item.PhienXetNghiem.NhanVienThucHienId == null ? null : item.PhienXetNghiem.NhanVienThucHien.User.HoTen,//item.YeuCauDichVuKyThuat.NhanVienThucHienId == null ? null : item.YeuCauDichVuKyThuat.NhanVienThucHien.User.HoTen,
                            NgayDuyet = item.PhienXetNghiem.ThoiDiemKetLuan,
                            TenNguoiDuyet = item.PhienXetNghiem.NhanVienKetLuanId == null ? null : item.PhienXetNghiem.NhanVienKetLuan.User.HoTen,
                            ThoiDiemChiDinhHoacNgayDieuTri = item != null && item.YeuCauDichVuKyThuat != null ? item.YeuCauDichVuKyThuat.ThoiDiemDangKy : (DateTime?)null
                        })
                    .GroupBy(x => new { x.PhienXetNghiemId, x.NhomDichVuBenhVienId })
                        .Select(item => new NhomCanLayMauXetNghiemGridVo()
                        {
                            YeuCauTiepNhanId = item.First().YeuCauTiepNhanId,
                            BenhNhanId = item.First().BenhNhanId,
                            PhienXetNghiemId = item.Key.PhienXetNghiemId,
                            NhomDichVuBenhVienId = item.Key.NhomDichVuBenhVienId,
                            TenNhom = item.First().TenNhom,
                            Barcode = item.First().Barcode,
                            SoLuongMauDaTao = item.First().SoLuongMauDaTao,//item.Sum(a => a.SoLuongMauDaTao),
                            LoaiMaus = item.Select(a => a.LoaiMau)
                                .Where(a => a.LoaiMau != null)
                                .GroupBy(a => new { a.LoaiMau })
                                .Select(itemLoaiMau => new LoaiMauVo()
                                {
                                    LoaiMau = itemLoaiMau.First().LoaiMau,
                                    DatChatLuong = itemLoaiMau.First().DatChatLuong,
                                    TenNhanVienXetKhongDat = itemLoaiMau.First().TenNhanVienXetKhongDat,
                                    ThoiDiemXetKhongDat = itemLoaiMau.First().ThoiDiemXetKhongDat,
                                    LyDoKhongDat = itemLoaiMau.First().LyDoKhongDat
                                })
                                .ToList(),
                            SoPhieu = item.First().SoPhieu,
                            NgayLayMau = item.First().NgayLayMau,
                            TenNguoiLayMau = item.First().TenNguoiLayMau,
                            NgayGui = item.First().NgayGui,
                            TenNGuoiGui = item.First().TenNGuoiGui,
                            NgayNhan = item.First().NgayNhan,
                            TenNguoiNhan = item.First().TenNguoiNhan,
                            NgayThucHien = item.First().NgayThucHien,
                            TenNguoiThucHien = item.First().TenNguoiThucHien,
                            NgayDuyet = item.First().NgayDuyet,
                            TenNguoiDuyet = item.First().TenNguoiDuyet,
                            ThoiDiemChiDinhHoacNgayDieuTri = item.First().ThoiDiemChiDinhHoacNgayDieuTri
                        })
                );


            // kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.TuNgayDenNgay != null && (!string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay) || !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay)))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);

                query = query.Where(p => (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay) || (p.ThoiDiemChiDinhHoacNgayDieuTri != null && p.ThoiDiemChiDinhHoacNgayDieuTri.Value.Date >= tuNgay.Date))
                                         && (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay) || (p.ThoiDiemChiDinhHoacNgayDieuTri != null && p.ThoiDiemChiDinhHoacNgayDieuTri.Value.Date <= denNgay.Date)));
            }

            if (timKiemNangCaoObj.TrangThai != null && (timKiemNangCaoObj.TrangThai.ChoLayMau || timKiemNangCaoObj.TrangThai.ChoGuiMau ||
                timKiemNangCaoObj.TrangThai.ChoKetQua || timKiemNangCaoObj.TrangThai.DaCoKetQua))
            {
                query = query.Where(x =>
                    (timKiemNangCaoObj.TrangThai.ChoLayMau && x.TrangThai == Enums.TrangThaiLayMauXetNghiem.ChoLayMau)
                    || (timKiemNangCaoObj.TrangThai.ChoGuiMau && x.TrangThai == Enums.TrangThaiLayMauXetNghiem.ChoGuiMau)
                    || (timKiemNangCaoObj.TrangThai.ChoKetQua && x.TrangThai == Enums.TrangThaiLayMauXetNghiem.ChoKetQua)
                    || (timKiemNangCaoObj.TrangThai.DaCoKetQua && x.TrangThai == Enums.TrangThaiLayMauXetNghiem.DaCoKetQua));
            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataForGridDichVuCanLayMauXetNghiemAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var arrParam = queryInfo.AdditionalSearchString.Split(";");
            long yeuCauTiepNhanId = long.Parse(arrParam[0]);
            long phienXetNghiemId = long.Parse(arrParam[1]);
            long nhomDichVuBenhVienId = long.Parse(arrParam[2]);
            int trangThai = int.Parse(arrParam[3]);

            IQueryable<DichVuCanLayMauXetNghiemGridVo> query = null;
            if (phienXetNghiemId == 0)
            {
                query =
                    _yeuCauDichVuKyThuatRepository.TableNoTracking
                        .Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId
                                    && x.NhomDichVuBenhVienId == nhomDichVuBenhVienId
                                    && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                    && (x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                                        x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                                        x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                                    && !x.PhienXetNghiemChiTiets.Any())
                        .Select(item => new DichVuCanLayMauXetNghiemGridVo()
                        {
                            YeuCauDichVuKyThuatId = item.Id,
                            MaDichVu = item.MaDichVu,
                            TenDichVu = item.TenDichVu,
                            ThoiGianChiDinh = item.ThoiDiemDangKy,//Tram: nếu là dv nội trú thì lấy ngày điều trị, nếu là ngoại trú thì lấy thời điể chỉ định
                            NguoiChiDinh = item.NhanVienChiDinh.User.HoTen,
                            BenhPham = item.BenhPhamXetNghiem,
                            LoaiMau = item.DichVuKyThuatBenhVien.LoaiMauXetNghiem
                        });
            }
            else
            {
                query =
                    _phienXetNghiemChiTietRepository.TableNoTracking
                        .Where(x => x.PhienXetNghiemId == phienXetNghiemId
                                    && x.NhomDichVuBenhVienId == nhomDichVuBenhVienId)
                        .Select(item => new DichVuCanLayMauXetNghiemGridVo()
                        {
                            YeuCauDichVuKyThuatId = item.YeuCauDichVuKyThuatId,
                            MaDichVu = item.YeuCauDichVuKyThuat.MaDichVu,
                            TenDichVu = item.YeuCauDichVuKyThuat.TenDichVu,
                            ThoiGianChiDinh = item.YeuCauDichVuKyThuat.ThoiDiemChiDinh,
                            NguoiChiDinh = item.YeuCauDichVuKyThuat.NhanVienChiDinh.User.HoTen,
                            BenhPham = item.YeuCauDichVuKyThuat.BenhPhamXetNghiem,
                            LoaiMau = item.DichVuKyThuatBenhVien.LoaiMauXetNghiem
                        })
                        .Distinct();
            }

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource
            {
                Data = queryTask.Result,
                TotalRowCount = countTask.Result
            };
        }
        public async Task<GridDataSource> GetTotalPageForGridDichVuCanLayMauXetNghiemAsync(QueryInfo queryInfo)
        {
            var arrParam = queryInfo.AdditionalSearchString.Split(";");
            long yeuCauTiepNhanId = long.Parse(arrParam[0]);
            long phienXetNghiemId = long.Parse(arrParam[1]);
            long nhomDichVuBenhVienId = long.Parse(arrParam[2]);
            int trangThai = int.Parse(arrParam[3]);

            IQueryable<DichVuCanLayMauXetNghiemGridVo> query = null;
            if (phienXetNghiemId == 0)
            {
                query =
                    _yeuCauDichVuKyThuatRepository.TableNoTracking
                        .Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId
                                    && x.NhomDichVuBenhVienId == nhomDichVuBenhVienId
                                    && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                    && (x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                                        x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                                        x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                                    && !x.PhienXetNghiemChiTiets.Any())
                        .Select(item => new DichVuCanLayMauXetNghiemGridVo()
                        {
                            YeuCauDichVuKyThuatId = item.Id,
                            MaDichVu = item.MaDichVu,
                            TenDichVu = item.TenDichVu,
                            ThoiGianChiDinh = item.ThoiDiemDangKy,//Tram: nếu là dv nội trú thì lấy ngày điều trị, nếu là ngoại trú thì lấy thời điể chỉ định
                            NguoiChiDinh = item.NhanVienChiDinh.User.HoTen,
                            BenhPham = item.BenhPhamXetNghiem,
                            LoaiMau = item.DichVuKyThuatBenhVien.LoaiMauXetNghiem
                        });
            }
            else
            {
                query =
                    _phienXetNghiemChiTietRepository.TableNoTracking
                        .Where(x => x.PhienXetNghiemId == phienXetNghiemId
                                    && x.NhomDichVuBenhVienId == nhomDichVuBenhVienId)
                        .Select(item => new DichVuCanLayMauXetNghiemGridVo()
                        {
                            YeuCauDichVuKyThuatId = item.YeuCauDichVuKyThuatId,
                            MaDichVu = item.YeuCauDichVuKyThuat.MaDichVu,
                            TenDichVu = item.YeuCauDichVuKyThuat.TenDichVu,
                            ThoiGianChiDinh = item.YeuCauDichVuKyThuat.ThoiDiemChiDinh,
                            NguoiChiDinh = item.YeuCauDichVuKyThuat.NhanVienChiDinh.User.HoTen,
                            BenhPham = item.YeuCauDichVuKyThuat.BenhPhamXetNghiem,
                            LoaiMau = item.DichVuKyThuatBenhVien.LoaiMauXetNghiem
                        })
                        .Distinct();
            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataForGridDichVuCanLayMauXetNghiemAsyncVer2(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var timKiemNangCaoObj = new LayMauXetNghiemTimKiemNangCaoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<LayMauXetNghiemTimKiemNangCaoVo>(queryInfo.AdditionalSearchString);
            }
            long yeuCauTiepNhanId = string.IsNullOrEmpty(queryInfo.SearchTerms) ? 0 : long.Parse(queryInfo.SearchTerms);

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

            //var laNhanVienKhoaXetNghiem = await _nhanVienRepository.TableNoTracking
            //    .AnyAsync(x => x.Id == _userAgentHelper.GetCurrentUserId()
            //                   && x.KhoaPhongNhanViens.Any()
            //                   && x.KhoaPhongNhanViens.Any(y => y.KhoaPhong.Ma == "XN" || y.PhongBenhVien.KhoaPhong.Ma == "XN"));
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
                    .Include(x => x.DichVuKyThuatBenhVien)
                    .Include(x => x.NhanVienChiDinh).ThenInclude(y => y.User)
                    .Include(x => x.NhomDichVuBenhVien)
                    .Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId
                                && x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && (x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                                    x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                                    x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                                //&& !x.PhienXetNghiemChiTiets.Any()
                                && (tuNgay == null || (tuNgay != null && ((x.NoiTruPhieuDieuTri == null && x.ThoiDiemChiDinh >= tuNgay.Value.Date) || (x.NoiTruPhieuDieuTri != null && x.NoiTruPhieuDieuTri.NgayDieuTri >= tuNgay.Value.Date))))
                                && (denNgay == null || (denNgay != null && ((x.NoiTruPhieuDieuTri == null && x.ThoiDiemChiDinh <= denNgay.Value.Date) || (x.NoiTruPhieuDieuTri != null && x.NoiTruPhieuDieuTri.NgayDieuTri <= denNgay.Value.Date))))
                                && (laNhanVienKhoaXetNghiem || x.NoiChiDinh.KhoaPhongId == phongHienTai.KhoaPhongId))
                    //.Skip(queryInfo.Skip).Take(queryInfo.Take)
                    .ToList();
            var query = lstDichVu
                .Select(item => new DichVuCanLayMauXetNghiemGridVo()
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
                    TenNguoiDuyet = item.PhienXetNghiemChiTiets.Any() ? item.PhienXetNghiemChiTiets.OrderByDescending(a => a.Id).Select(a => a.NhanVienKetLuan?.User.HoTen).FirstOrDefault() : null
                }).ToList();

            if (timKiemNangCaoObj.TrangThai != null && (timKiemNangCaoObj.TrangThai.ChoLayMau 
                                                        //|| timKiemNangCaoObj.TrangThai.ChoGuiMau 
                                                        //|| timKiemNangCaoObj.TrangThai.ChoKetQua 
                                                        //|| timKiemNangCaoObj.TrangThai.DaCoKetQua
                                                        || timKiemNangCaoObj.TrangThai.DaLayMau))
            {
                query = query.Where(x =>
                    //(timKiemNangCaoObj.TrangThai.ChoLayMau 
                    //        && ((timKiemNangCaoObj.IsGridChuaCapCode != true && (x.TrangThai == Enums.TrangThaiLayMauXetNghiem.ChoLayMau || x.TrangThai == Enums.TrangThaiLayMauXetNghiem.ChoGuiMau))
                    //            || (timKiemNangCaoObj.IsGridChuaCapCode == true && x.TrangThai == Enums.TrangThaiLayMauXetNghiem.ChoLayMau)))
                    //|| (timKiemNangCaoObj.TrangThai.ChoGuiMau && x.TrangThai == Enums.TrangThaiLayMauXetNghiem.ChoGuiMau)
                    //|| (timKiemNangCaoObj.TrangThai.ChoKetQua && x.TrangThai == Enums.TrangThaiLayMauXetNghiem.ChoKetQua)
                    //|| (timKiemNangCaoObj.TrangThai.DaCoKetQua && x.TrangThai == Enums.TrangThaiLayMauXetNghiem.DaCoKetQua)
                    (timKiemNangCaoObj.TrangThai.ChoLayMau && x.TrangThai == Enums.TrangThaiLayMauXetNghiem.ChoLayMau)
                    || (timKiemNangCaoObj.TrangThai.DaLayMau && (x.TrangThai == Enums.TrangThaiLayMauXetNghiem.ChoGuiMau
                                                                 || x.TrangThai == Enums.TrangThaiLayMauXetNghiem.ChoKetQua
                                                                 || x.TrangThai == Enums.TrangThaiLayMauXetNghiem.DaCoKetQua))
                    ).ToList();
            }

            //var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            //var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
            //    .Take(queryInfo.Take).ToArrayAsync();

            //await Task.WhenAll(countTask, queryTask);

            return new GridDataSource
            {
                Data = query.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray(), // queryTask.Result, <= cần cân nhắc skip take, tuy dv xét nghiệm ko nhiều
                TotalRowCount = query.Count // countTask.Result
            };
        }
        public async Task<GridDataSource> GetTotalPageForGridDichVuCanLayMauXetNghiemAsyncVer2(QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new LayMauXetNghiemTimKiemNangCaoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<LayMauXetNghiemTimKiemNangCaoVo>(queryInfo.AdditionalSearchString);
            }
            long yeuCauTiepNhanId = string.IsNullOrEmpty(queryInfo.SearchTerms) ? 0 : long.Parse(queryInfo.SearchTerms);

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

            //var laNhanVienKhoaXetNghiem = await _nhanVienRepository.TableNoTracking
            //    .AnyAsync(x => x.Id == _userAgentHelper.GetCurrentUserId()
            //                   && x.KhoaPhongNhanViens.Any()
            //                   && x.KhoaPhongNhanViens.Any(y => y.KhoaPhong.Ma == "XN" || y.PhongBenhVien.KhoaPhong.Ma == "XN"));
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
                    .Include(x => x.DichVuKyThuatBenhVien)
                    .Include(x => x.NhanVienChiDinh).ThenInclude(y => y.User)
                    .Include(x => x.NhomDichVuBenhVien)
                    .Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId
                                && x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && (x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                                    x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                                    x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                                //&& !x.PhienXetNghiemChiTiets.Any()
                                && (tuNgay == null || (tuNgay != null && ((x.NoiTruPhieuDieuTri == null && x.ThoiDiemChiDinh >= tuNgay.Value.Date) || (x.NoiTruPhieuDieuTri != null && x.NoiTruPhieuDieuTri.NgayDieuTri >= tuNgay.Value.Date))))
                                && (denNgay == null || (denNgay != null && ((x.NoiTruPhieuDieuTri == null && x.ThoiDiemChiDinh <= denNgay.Value.Date) || (x.NoiTruPhieuDieuTri != null && x.NoiTruPhieuDieuTri.NgayDieuTri <= denNgay.Value.Date))))
                                && (laNhanVienKhoaXetNghiem || x.NoiChiDinh.KhoaPhongId == phongHienTai.KhoaPhongId)).ToList();
            var query = lstDichVu
                .Select(item => new DichVuCanLayMauXetNghiemGridVo()
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
                    TenNguoiDuyet = item.PhienXetNghiemChiTiets.Any() ? item.PhienXetNghiemChiTiets.OrderByDescending(a => a.Id).Select(a => a.NhanVienKetLuan?.User.HoTen).FirstOrDefault() : null
                }).ToList();

            if (timKiemNangCaoObj.TrangThai != null && (timKiemNangCaoObj.TrangThai.ChoLayMau 
                                                        //|| timKiemNangCaoObj.TrangThai.ChoGuiMau 
                                                        //|| timKiemNangCaoObj.TrangThai.ChoKetQua 
                                                        //|| timKiemNangCaoObj.TrangThai.DaCoKetQua
                                                        || timKiemNangCaoObj.TrangThai.DaLayMau
                                                        ))
            {
                query = query.Where(x =>
                    //(timKiemNangCaoObj.TrangThai.ChoLayMau
                    // && ((timKiemNangCaoObj.IsGridChuaCapCode != true && (x.TrangThai == Enums.TrangThaiLayMauXetNghiem.ChoLayMau || x.TrangThai == Enums.TrangThaiLayMauXetNghiem.ChoGuiMau))
                    //     || (timKiemNangCaoObj.IsGridChuaCapCode == true && x.TrangThai == Enums.TrangThaiLayMauXetNghiem.ChoLayMau)))
                    //|| (timKiemNangCaoObj.TrangThai.ChoGuiMau && x.TrangThai == Enums.TrangThaiLayMauXetNghiem.ChoGuiMau)
                    //|| (timKiemNangCaoObj.TrangThai.ChoKetQua && x.TrangThai == Enums.TrangThaiLayMauXetNghiem.ChoKetQua)
                    //|| (timKiemNangCaoObj.TrangThai.DaCoKetQua && x.TrangThai == Enums.TrangThaiLayMauXetNghiem.DaCoKetQua)
                    (timKiemNangCaoObj.TrangThai.ChoLayMau && x.TrangThai == Enums.TrangThaiLayMauXetNghiem.ChoLayMau)
                    || (timKiemNangCaoObj.TrangThai.DaLayMau && (x.TrangThai == Enums.TrangThaiLayMauXetNghiem.ChoGuiMau
                                                                 || x.TrangThai == Enums.TrangThaiLayMauXetNghiem.ChoKetQua
                                                                 || x.TrangThai == Enums.TrangThaiLayMauXetNghiem.DaCoKetQua))
                    ).ToList();
            }

            //var countTask = query.CountAsync();
            //await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = query.Count };
        }

        #endregion

        #region Get data
        public async Task<ThongTinYeuCauTiepNhanLayMauVo> GetDanhThongTinYeuCauTiepNhanLayMauAsync(long yeuCauTiepNhanId)
        {
            var cauHinhTuDongBarcode = _cauHinhService.GetSetting("CauHinhXetNghiem.TaoBarcodeTuDong");

            var chiTiet = await _yeuCauTiepNhanRepository.TableNoTracking
                .Where(x => x.Id == yeuCauTiepNhanId)
                .Select(item => new ThongTinYeuCauTiepNhanLayMauVo()
                {
                    YeuCauTiepNhanId = item.Id,
                    BenhNhanId = item.BenhNhanId,
                    MaBenhNhan = item.BenhNhan.MaBN,
                    MaTiepNhan = item.MaYeuCauTiepNhan,
                    HoTen = item.HoTen,
                    NgaySinh = item.NgaySinh,
                    ThangSinh = item.ThangSinh,
                    NamSinh = item.NamSinh,
                    GioiTinh = item.GioiTinh,
                    Tuyen = item.LyDoVaoVien.GetDescription(),
                    MucHuong = item.BHYTMucHuong,
                    DanToc = item.DanToc != null ? item.DanToc.Ten : "",
                    DiaChi = item.DiaChiDayDu,
                    NgheNghiep = item.NgheNghiep != null ? item.NgheNghiep.Ten : "",
                    SoTheBHYT = item.BHYTMaSoThe,
                    SoDienThoai = item.SoDienThoaiDisplay,
                    IsTraKetQua = item.PhienXetNghiems.Any() && item.PhienXetNghiems.All(a => a.DaTraKetQua == true),
                    IsAutoBarcode = Boolean.Parse(cauHinhTuDongBarcode.Value),
                    IsCoDuKetQua = item.YeuCauDichVuKyThuats.Where(x => x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                                                        //&& x.NhomDichVuBenhVien.NhomDichVuBenhVienChaId != null
                                                                        && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                                        && (x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan
                                                                            || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan
                                                                            || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
                                                            .All(x => x.PhienXetNghiemChiTiets.Any())
                                   && item.PhienXetNghiems.Any()
                                   && item.PhienXetNghiems.All(x => x.ThoiDiemKetLuan != null),
                    IsCoPhienChiTietCoKetQua = item.YeuCauDichVuKyThuats.Any(x => x.PhienXetNghiemChiTiets.Any()) 
                                               && item.YeuCauDichVuKyThuats.Any(x => x.PhienXetNghiemChiTiets.OrderByDescending(a => a.Id).Select(a => a.NhanVienKetLuanId).FirstOrDefault() != null),
                    //BVHD-3364
                    TenCongTy = item.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe ? item.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten : null
                }).FirstOrDefaultAsync();
            return chiTiet;
        }

        public async Task<List<LookupItemTemplateVo>> GetListBarcodeTheoYeuCauTiepNhanAsync(DropDownListRequestModel model)
        {
            if (string.IsNullOrEmpty(model.ParameterDependencies))
            {
                return new List<LookupItemTemplateVo>();
            }
            var param = JsonConvert.DeserializeObject<LayMauXetNghiemBarcodeVo>(model.ParameterDependencies);

            //BVHD-3200
            // Cho phép chọn lại barcode của bệnh nhân ở ngoại trú
            var yeuCauTiepNhanKhacCuaBenhNhan = _yeuCauTiepNhanRepository.TableNoTracking
                .Include(x => x.YeuCauNhapVien).ThenInclude(y => y.YeuCauKhamBenh)
                .Include(x => x.YeuCauKhamBenhs).ThenInclude(y => y.YeuCauNhapViens).ThenInclude(z => z.YeuCauTiepNhans)
                .Where(x => x.Id == param.YeuCauTiepNhanId)
                .First();
            long? yeuCauTiepNhanKhacCuaBenhNhanId = null;
            if (yeuCauTiepNhanKhacCuaBenhNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru)
            {
                yeuCauTiepNhanKhacCuaBenhNhanId = yeuCauTiepNhanKhacCuaBenhNhan.YeuCauNhapVien?.YeuCauKhamBenh?.YeuCauTiepNhanId;
            }
            else
            {
                yeuCauTiepNhanKhacCuaBenhNhanId = yeuCauTiepNhanKhacCuaBenhNhan.YeuCauKhamBenhs
                    .Where(x => x.YeuCauNhapViens.Any())
                    .SelectMany(x => x.YeuCauNhapViens.Where(y => y.YeuCauTiepNhans.Any()))
                    .SelectMany(x => x.YeuCauTiepNhans)
                    .Select(x => x.Id)
                    .FirstOrDefault();
            }
            var lstBarcode = await BaseRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhanId == param.YeuCauTiepNhanId || x.YeuCauTiepNhanId == yeuCauTiepNhanKhacCuaBenhNhanId)
                .SelectMany(x => x.MauXetNghiems)
                .Where(x => 
                            //x.CreatedOn.Value.Date == DateTime.Now.Date
                            x.BarCodeNumber != null
                            && param.BarcodeNumbers.All(a => a != x.BarCodeNumber))
                .Select(item => new LookupItemTemplateVo()
                {
                    KeyId = item.BarCodeNumber.Value,
                    DisplayName = item.BarCodeId// item.BarCodeNumber.Value.ToString()
                })
                .ApplyLike(model.Query, x => x.DisplayName)
                .GroupBy(x => new { x.DisplayName })
                .Select(item => new LookupItemTemplateVo()
                {
                    KeyId = item.First().KeyId,
                    DisplayName = item.Key.DisplayName.ToString()
                })
                .Union(param.BarcodeNumbers
                    .Where(x => x.ToString().Contains(model.Query ?? ""))
                    .Select(item => new LookupItemTemplateVo()
                    {
                        KeyId = item,
                        DisplayName = item.ToString()
                    }))
                .ToListAsync();
            return lstBarcode;
        }

        public async Task<LookupItemVo> KiemTraBarcodeDangChonAsync(string strQuery)
        {
            var arrParam = strQuery.Split(";");
            var yeuCauTiepNhanId = long.Parse(arrParam[0]);

            //if (string.IsNullOrEmpty(arrParam[1]) || arrParam[1].Contains("null"))
            //{
            //    throw new Exception(_localizationService.GetResource("LayMauXetNghiem.BarcodeNumber.Required"));
            //}
            //if (!string.IsNullOrEmpty(arrParam[1]) && arrParam[1].Length > 4)
            //{
            //    throw new Exception(_localizationService.GetResource("LayMauXetNghiem.BarcodeNumber.Length"));
            //}
            var barcodeNumber = string.IsNullOrEmpty(arrParam[1]) ? (int?)null : int.Parse(arrParam[1]);
            var barcodeString = string.IsNullOrEmpty(arrParam[2]) ? null : arrParam[2];
            var barcodeId = string.Empty;

            if (string.IsNullOrEmpty(barcodeString))
            {
                var newPreBarcode = DateTime.Now.ToString("ddMMyy"); //yyMMdd
                var maBarcodeFormat = string.Empty;
                switch (barcodeNumber.ToString().Length)
                {
                    case 1:
                        maBarcodeFormat = "000" + barcodeNumber;
                        break;
                    case 2:
                        maBarcodeFormat = "00" + barcodeNumber;
                        break;
                    case 3:
                        maBarcodeFormat = "0" + barcodeNumber;
                        break;
                }

                barcodeId = newPreBarcode + maBarcodeFormat;
            }
            else
            {
                barcodeId = barcodeString;
            }


            //BVHD-3200
            // Cho phép chọn lại barcode của bệnh nhân ở ngoại trú, nội trú
            var yeuCauTiepNhanKhacCuaBenhNhan = _yeuCauTiepNhanRepository.TableNoTracking
                .Include(x => x.YeuCauNhapVien).ThenInclude(y => y.YeuCauKhamBenh)
                .Include(x => x.YeuCauKhamBenhs).ThenInclude(y => y.YeuCauNhapViens).ThenInclude(z => z.YeuCauTiepNhans)
                .Where(x => x.Id == yeuCauTiepNhanId)
                .First();
            long? yeuCauTiepNhanKhacCuaBenhNhanId = null;
            if (yeuCauTiepNhanKhacCuaBenhNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru)
            {
                yeuCauTiepNhanKhacCuaBenhNhanId = yeuCauTiepNhanKhacCuaBenhNhan.YeuCauNhapVien?.YeuCauKhamBenh?.YeuCauTiepNhanId;
            }
            else
            {
                yeuCauTiepNhanKhacCuaBenhNhanId = yeuCauTiepNhanKhacCuaBenhNhan.YeuCauKhamBenhs
                    .Where(x => x.YeuCauNhapViens.Any())
                    .SelectMany(x => x.YeuCauNhapViens.Where(y => y.YeuCauTiepNhans.Any()))
                    .SelectMany(x => x.YeuCauTiepNhans)
                    .Select(x => x.Id)
                    .FirstOrDefault();
            }


            //BVHD-3200
            // Cho phép chọn lại barcode của bệnh nhân ở ngoại trú, nội trú
            var isExists = await BaseRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhanId != yeuCauTiepNhanId 
                            && x.YeuCauTiepNhanId != yeuCauTiepNhanKhacCuaBenhNhanId)
                .SelectMany(x => x.MauXetNghiems)
                .Where(x => x.BarCodeNumber != null
                            && barcodeNumber != null
                            && x.BarCodeNumber == barcodeNumber
                            && x.BarCodeId == barcodeId)
                .OrderByDescending(x => x.CreatedOn)
                .Select(item => new LookupItemVo()
                {
                    KeyId = item.BarCodeNumber.Value,
                    DisplayName = item.BarCodeId
                })
                .AnyAsync();

            if (isExists)
            {
                throw new Exception(_localizationService.GetResource("LayMauXetNghiem.BarcodeNumberId.TrungVoiBenhNhanKhac"));
            }

            //BVHD-3200
            // Cho phép chọn lại barcode của bệnh nhân ở ngoại trú, nội trú
            var barcode = await BaseRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId 
                            || x.YeuCauTiepNhanId == yeuCauTiepNhanKhacCuaBenhNhanId)
                .SelectMany(x => x.MauXetNghiems)
                .Where(x => x.BarCodeNumber != null
                            && barcodeNumber != null
                            && x.BarCodeNumber == barcodeNumber
                            && x.BarCodeId == barcodeId)
                .OrderByDescending(x => x.CreatedOn)
                .Select(item => new LookupItemVo()
                {
                    KeyId = item.BarCodeNumber.Value,
                    DisplayName = item.BarCodeId
                })
                .FirstOrDefaultAsync();

            if (barcode == null)
            {
                // xử lý tạo barcode mới
                barcode = TaoBarcodeAsync(barcodeNumber);
            }

            return barcode;
        }

        public LookupItemVo TaoBarcodeAsync(int? barcodeNumber)
        {
            var newPreBarcode = DateTime.Now.ToString("ddMMyy"); //yyMMdd
            var maBarcodeFormat = string.Empty;
            // tạo tự động
            if (barcodeNumber == null)
            {
                var path = @"Resource\\LayMauXetNghiemBarcode.xml";
                XDocument data = XDocument.Load(path);
                XNamespace root = data.Root.GetDefaultNamespace();
                XElement barcodeXML = data.Descendants(root + "Barcode").FirstOrDefault();
                var barcodeNumberOld = (string)barcodeXML.Element(root + "BarcodeNumber");
                //var preBarcodeOld = (string)barcodeXML.Element(root + "PreBarcode");


                //BVHD_3642: chỉ kiểm tra những barcode đc tạo từ 0 -> cấu hình
                var cauHinhMaxBarcode = _cauHinhService.GetSetting("CauHinhXetNghiem.MaxBarcodeTuDong");
                int.TryParse(cauHinhMaxBarcode?.Value, out int cauHinhMaxBarcodeNumber);

                if (!string.IsNullOrEmpty(barcodeNumberOld) && Convert.ToInt32(barcodeNumberOld) >= cauHinhMaxBarcodeNumber) //== 9999)
                {
                    barcodeNumberOld = "0";
                }
                //todo: kiểm tra nếu old number chưa được sử dụng thì sử dụng lại luôn
                barcodeNumber = !string.IsNullOrEmpty(barcodeNumberOld) ? Convert.ToInt32(barcodeNumberOld) + 1 : 1;

                var lstBarcodeNumberDaCapNgayHienTai = BaseRepository.TableNoTracking
                    .Where(x => x.ThoiDiemBatDau.Date >= DateTime.Now.Date
                                
                                //BVHD-3642: chỉ kiểm tra những barcode đc tạo từ 0 -> cấu hình
                                && x.BarCodeNumber <= cauHinhMaxBarcodeNumber
                                )
                    .Select(x => x.MaSo)
                    .ToList();
                if (lstBarcodeNumberDaCapNgayHienTai.Any())
                {
                    var maxHienTai = lstBarcodeNumberDaCapNgayHienTai.Max();

                    // trường hợp barcodenumber lưu từ file chưa được cấp, hoặc > barcodenumber max đã cấp thì ko cần kiểm tra
                    if (lstBarcodeNumberDaCapNgayHienTai.All(x => x != barcodeNumber) || barcodeNumber > maxHienTai)
                    {

                    }
                    // trường hợp barcodenumber lưu từ file = barcodenumber max đã cấp thì +1
                    else if (barcodeNumber == maxHienTai)
                    {
                        barcodeNumber += 1;
                    }
                    // trường hợp barcodenumber lưu từ file < barcodenumber max đã cấp thì kiểm tra lấy số đầu tiên chưa chưa sử dụng
                    else if (barcodeNumber < maxHienTai)
                    {
                        var startNumber = barcodeNumber.Value + 1;
                        for (int i = startNumber; i < maxHienTai; i++)
                        {
                            if (lstBarcodeNumberDaCapNgayHienTai.All(x => x != i))
                            {
                                barcodeNumber = i;
                                break;
                            }
                        }
                    }
                }


                //if (newPreBarcode != preBarcodeOld)
                //{
                //    barcodeNumber = 1;
                //}

                //var barcodeNumberMaxInDay = _mauXetNghiemRepository.TableNoTracking
                //    .Where(x => x.CreatedOn.Value.Date == DateTime.Now.Date
                //                && x.BarCodeNumber != null
                //                && x.BarCodeNumber.Value > 0)
                //    .OrderByDescending(x => x.BarCodeNumber)
                //    .Select(x => x.BarCodeNumber)
                //    .FirstOrDefault();
                //if (barcodeNumberMaxInDay != null && barcodeNumber <= barcodeNumberMaxInDay)
                //{
                //    barcodeNumber = barcodeNumberMaxInDay + 1;
                //}

                maBarcodeFormat = barcodeNumber.ToString();
                switch (barcodeNumber.ToString().Length)
                {
                    case 1:
                        maBarcodeFormat = "000" + barcodeNumber;
                        break;
                    case 2:
                        maBarcodeFormat = "00" + barcodeNumber;
                        break;
                    case 3:
                        maBarcodeFormat = "0" + barcodeNumber;
                        break;
                }
                //Cập nhập vào file
                barcodeXML.Element("BarcodeNumber").Value = barcodeNumber.ToString();
                //barcodeXML.Element("PreBarcode").Value = newPreBarcode;
                data.Save(path);
            }
            // tạo theo người dùng nhập
            else
            {
                maBarcodeFormat = barcodeNumber.ToString();
                switch (barcodeNumber.ToString().Length)
                {
                    case 1:
                        maBarcodeFormat = "000" + barcodeNumber;
                        break;
                    case 2:
                        maBarcodeFormat = "00" + barcodeNumber;
                        break;
                    case 3:
                        maBarcodeFormat = "0" + barcodeNumber;
                        break;
                }
            }

            return new LookupItemVo()
            {
                KeyId = barcodeNumber.Value,
                DisplayName = newPreBarcode + maBarcodeFormat
            };
        }

        public async Task<List<LichSuTuChoiMauVo>> GetLichSuTuChoiMauAsync(long yeuCauTiepNhanId)
        {
            var query = await _mauXetNghiemRepository.TableNoTracking
                .Where(x => x.PhienXetNghiem.YeuCauTiepNhanId == yeuCauTiepNhanId
                            && x.DatChatLuong == false)
                //.OrderByDescending(x => x.ThoiDiemXetKhongDat)
                .Select(item => new LichSuTuChoiMauVo()
                {
                    NguoiTuChoi = item.NhanVienXetKhongDat.User.HoTen,
                    ThoiGianThucHien = item.ThoiDiemXetKhongDat,
                    LyDoTuChoi = item.LyDoKhongDat,
                    LichSuTuChoiMauItem = new LichSuTuChoiMauItemVo()
                    {
                        TenNhom = item.NhomDichVuBenhVien.Ten,
                        TenMau = item.LoaiMauXetNghiem.GetDescription(),
                        Barcode = item.BarCodeId,
                        NgayLayMau = item.ThoiDiemLayMau,
                        NguoiLayMau = item.NhanVienLayMau != null ? item.NhanVienLayMau.User.HoTen : null,
                        NgayGui = item.PhieuGoiMauXetNghiem.ThoiDiemGoiMau,
                        NguoiGui = item.PhieuGoiMauXetNghiem.NhanVienGoiMau.User.HoTen,
                        SoPhieu = item.PhieuGoiMauXetNghiem.SoPhieu,
                        PhieuGuiMauXetNghiemId = item.PhieuGoiMauXetNghiemId
                    }
                })
                .GroupBy(x => new { x.NguoiTuChoi, x.ThoiGianThucHien, x.LyDoTuChoi })
                .Select(x => new LichSuTuChoiMauVo()
                {
                    NguoiTuChoi = x.Key.NguoiTuChoi,
                    ThoiGianThucHien = x.Key.ThoiGianThucHien,
                    LyDoTuChoi = x.Key.LyDoTuChoi,
                    LichSuTuChoiMauGridVo = new LichSuTuChoiMauItemDataSource()
                    {
                        data = x.Select(a => a.LichSuTuChoiMauItem).ToList()
                    }
                })
                .OrderByDescending(x => x.ThoiGianThucHien)
                .ToListAsync();
            return query;
        }

        public async Task<string> InBarcodeLayMauXetNghiemAsync(LayMauXetNghiemInBarcodeVo model)
        {
            var content = string.Empty;
            var templateBarcode = await _templateRepository.TableNoTracking.Where(x => x.Name.Equals("LayMauXetNghiemBarcode")).FirstOrDefaultAsync();
            if (templateBarcode != null)
            {
                var data = await _yeuCauTiepNhanRepository.TableNoTracking
                    .Where(x => x.Id == model.YeuCauTiepNhanId)
                    .Select(item => new LayMauXetNghiemInBarcodeVo()
                    {
                        HostingName = model.HostingName,
                        BarcodeId = model.BarcodeId.Length == 10 ? model.BarcodeId.Insert(6,"-") : model.BarcodeId,
                        BarcodeNumber = model.BarcodeNumber,
                        TenBenhNhan = item.HoTen,
                        BarcodeByBarcodeId = BarcodeHelper.GenerateBarCode(model.BarcodeId.Substring(6,4), 210, 56,false),
                        GioiTinh = item.GioiTinh,
                        NgaySinh = item.NgaySinh,
                        ThangSinh = item.ThangSinh,
                        NamSinh = item.NamSinh,
                        GioCapCode = DateTime.Now.ApplyFormatFullTime()
                    }).FirstOrDefaultAsync();
                content = TemplateHelpper.FormatTemplateWithContentTemplate(templateBarcode.Body, data);
            }
            return content;
        }

        #endregion

        #region Xử lý tương tác data

        public async Task XuLyLayMauXetNghiemAsync(LayMauXetNghiemXacNhanLayMauVo layMauXetNghiemVo)
        {
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var phongHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();
            // kiểm tra trùng barcode
            var isExistBarcode = await _mauXetNghiemRepository.TableNoTracking
                .Where(x => x.BarCodeNumber != null)
                .AnyAsync(x =>
                    x.PhienXetNghiem.YeuCauTiepNhanId != layMauXetNghiemVo.YeuCauTiepNhanId
                    && x.BarCodeId == layMauXetNghiemVo.BarcodeId);
            if (isExistBarcode)
            {
                throw new Exception(_localizationService.GetResource("LayMauXetNghiem.BarcodeId.IsExists"));
            }

            // xử lý tạo phiên hoặc cập nhật
            //var isExistPhienXetNghiem = await BaseRepository.TableNoTracking
            //    .Where(x => x.YeuCauTiepNhanId == layMauXetNghiemVo.YeuCauTiepNhanId)
            //    .SelectMany(x => x.MauXetNghiems)
            //    //.Where(x => x.BarCodeNumber != null)
            //    .AnyAsync(x => x.BarCodeId == layMauXetNghiemVo.BarcodeId);

            var phienXetNghiem = await BaseRepository.Table
                .Include(x => x.PhienXetNghiemChiTiets)
                .Include(x => x.MauXetNghiems)
                .Where(x => x.YeuCauTiepNhanId == layMauXetNghiemVo.YeuCauTiepNhanId)
                .LastOrDefaultAsync(x => x.BarCodeId == layMauXetNghiemVo.BarcodeId);

            // trường hợp cùng phiên
            if (phienXetNghiem != null && phienXetNghiem.NhanVienKetLuanId == null)
            {
                // xử lý cập nhật phiên: thêm phiên chi tiết, mẫu xét nghiệm
                //var phienXetNghiem = await BaseRepository.Table
                //    .Include(x => x.PhienXetNghiemChiTiets)
                //    .Include(x => x.MauXetNghiems)
                //    .Where(x => x.MauXetNghiems.Any(a => a.BarCodeId == layMauXetNghiemVo.BarcodeId))
                //    .FirstOrDefaultAsync();
                //if (phienXetNghiem == null)
                //{
                //    throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
                //}

                //var yeuCauDichVuKyThuats = phienXetNghiem.PhienXetNghiemChiTiets.Select(x => x.YeuCauDichVuKyThuat);
                var yeuCauDichVuKyThuats = await _yeuCauDichVuKyThuatRepository.Table
                    .Include(x => x.DichVuKyThuatBenhVien)
                    .Where(x => x.YeuCauTiepNhanId == layMauXetNghiemVo.YeuCauTiepNhanId
                                && x.NhomDichVuBenhVienId == layMauXetNghiemVo.NhomDichVuBenhVienId
                                && x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                && x.NhomDichVuBenhVien.NhomDichVuBenhVienChaId != null
                                && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && (x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan ||
                                    x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                                    x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan)
                                && !x.PhienXetNghiemChiTiets.Any())
                    .ToListAsync();
                if (!yeuCauDichVuKyThuats.Any())
                {
                    throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
                }

                var flag = false;
                foreach (var dichVuKyThuat in yeuCauDichVuKyThuats)
                {
                    // xử lý tạo phiên chi tiết
                    dichVuKyThuat.TrangThai = dichVuKyThuat.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien ? Enums.EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien : dichVuKyThuat.TrangThai;
                    dichVuKyThuat.NhanVienThucHienId = currentUserId;
                    dichVuKyThuat.ThoiDiemThucHien = DateTime.Now;

                    var newPhienChiTiet = new PhienXetNghiemChiTiet()
                    {
                        NhomDichVuBenhVienId = dichVuKyThuat.NhomDichVuBenhVienId,
                        //YeuCauDichVuKyThuatId = dichVuKyThuat.Id,
                        YeuCauDichVuKyThuat = dichVuKyThuat,
                        DichVuKyThuatBenhVienId = dichVuKyThuat.DichVuKyThuatBenhVienId,
                        LanThucHien = 1
                    };
                    phienXetNghiem.PhienXetNghiemChiTiets.Add(newPhienChiTiet);

                    // xử lý tạo mẫu xét nghiệm
                    var loaiMau = dichVuKyThuat.DichVuKyThuatBenhVien.LoaiMauXetNghiem;
                    if (loaiMau == null)
                    {
                        throw new Exception(_localizationService.GetResource("LayMauXetNghiem.LoaiMau.Required"));
                    }
                    if (!phienXetNghiem.MauXetNghiems.Any(x =>
                        x.NhomDichVuBenhVienId == layMauXetNghiemVo.NhomDichVuBenhVienId
                        && x.LoaiMauXetNghiem == loaiMau))
                    {
                        flag = true;
                        var newMauXetNghiem = new MauXetNghiem()
                        {
                            NhomDichVuBenhVienId = layMauXetNghiemVo.NhomDichVuBenhVienId,
                            LoaiMauXetNghiem = loaiMau.Value,
                            SoLuongMau = 1,
                            BarCodeNumber = layMauXetNghiemVo.BarcodeNumber,
                            BarCodeId = layMauXetNghiemVo.BarcodeId,
                            DatChatLuong = true,
                            ThoiDiemLayMau = DateTime.Now,
                            NhanVienLayMauId = currentUserId,
                            PhongLayMauId = phongHienTaiId
                        };
                        phienXetNghiem.MauXetNghiems.Add(newMauXetNghiem);
                    }
                }

                if (!flag)
                {
                    throw new Exception(_localizationService.GetResource("LayMauXetNghiem.MauXetNghiems.Required"));
                }
                await BaseRepository.UpdateAsync(phienXetNghiem);

            }
            // trường hợp tạo mới phiên
            else
            {
                var newPhienXetNhgiem = new PhienXetNghiem()
                {
                    BenhNhanId = layMauXetNghiemVo.BenhNhanId,
                    YeuCauTiepNhanId = layMauXetNghiemVo.YeuCauTiepNhanId,
                    MaSo = layMauXetNghiemVo.BarcodeNumber,
                    ThoiDiemBatDau = phienXetNghiem?.ThoiDiemBatDau ?? DateTime.Now
                };

                var lstYeuCauDichVuKyThuat = await _yeuCauDichVuKyThuatRepository.Table
                    .Include(y => y.DichVuKyThuatBenhVien)
                    .Where(x => x.YeuCauTiepNhanId == layMauXetNghiemVo.YeuCauTiepNhanId
                                && x.NhomDichVuBenhVienId == layMauXetNghiemVo.NhomDichVuBenhVienId
                                && x.NhomDichVuBenhVien.NhomDichVuBenhVienChaId != null
                                //&& (x.NhomDichVuBenhVien.Ma == "XN" || x.NhomDichVuBenhVien.NhomDichVuBenhVienCha.Ma == "XN")
                                && x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && (x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan
                                    || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan
                                    || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                                && !x.PhienXetNghiemChiTiets.Any())
                    .ToListAsync();
                if (!lstYeuCauDichVuKyThuat.Any())
                {
                    throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
                }

                foreach (var dichVuKyThuat in lstYeuCauDichVuKyThuat)
                {
                    // xử lý tạo phiên chi tiết
                    dichVuKyThuat.TrangThai = dichVuKyThuat.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien ? Enums.EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien : dichVuKyThuat.TrangThai;
                    dichVuKyThuat.NhanVienThucHienId = currentUserId;
                    dichVuKyThuat.ThoiDiemThucHien = DateTime.Now;

                    var newPhienChiTiet = new PhienXetNghiemChiTiet()
                    {
                        NhomDichVuBenhVienId = dichVuKyThuat.NhomDichVuBenhVienId,
                        //YeuCauDichVuKyThuatId = dichVuKyThuat.Id,
                        YeuCauDichVuKyThuat = dichVuKyThuat,
                        DichVuKyThuatBenhVienId = dichVuKyThuat.DichVuKyThuatBenhVienId,
                        LanThucHien = 1
                    };
                    newPhienXetNhgiem.PhienXetNghiemChiTiets.Add(newPhienChiTiet);

                    // xử lý tạo mẫu xét nghiệm
                    var loaiMau = dichVuKyThuat.DichVuKyThuatBenhVien.LoaiMauXetNghiem;
                    if (loaiMau == null)
                    {
                        throw new Exception(_localizationService.GetResource("LayMauXetNghiem.LoaiMau.Required"));
                    }
                    if (!newPhienXetNhgiem.MauXetNghiems.Any(x =>
                        x.NhomDichVuBenhVienId == layMauXetNghiemVo.NhomDichVuBenhVienId
                        && x.LoaiMauXetNghiem == loaiMau))
                    {
                        var newMauXetNghiem = new MauXetNghiem()
                        {
                            NhomDichVuBenhVienId = layMauXetNghiemVo.NhomDichVuBenhVienId,
                            LoaiMauXetNghiem = loaiMau.Value,
                            SoLuongMau = 1,
                            BarCodeNumber = layMauXetNghiemVo.BarcodeNumber,
                            BarCodeId = layMauXetNghiemVo.BarcodeId,
                            DatChatLuong = true,
                            ThoiDiemLayMau = DateTime.Now,
                            NhanVienLayMauId = currentUserId,
                            PhongLayMauId = phongHienTaiId
                        };
                        newPhienXetNhgiem.MauXetNghiems.Add(newMauXetNghiem);
                    }
                }

                if (!newPhienXetNhgiem.MauXetNghiems.Any())
                {
                    throw new Exception(_localizationService.GetResource("LayMauXetNghiem.MauXetNghiems.Required"));
                }

                await BaseRepository.AddAsync(newPhienXetNhgiem);
            }
        }

        public async Task XuLyLayLaiMauXetNghiemAsync(LayMauXetNghiemXacNhanLayMauVo layMauXetNghiemVo)
        {
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var phongHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();

            var phienXetNghiem = await BaseRepository.Table
                .Include(x => x.PhienXetNghiemChiTiets)
                .Include(x => x.MauXetNghiems)
                .Where(x => x.Id == layMauXetNghiemVo.PhienXetNghiemId)
                .FirstOrDefaultAsync();

            if (phienXetNghiem == null)
            {
                throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
            }

            var mauXetNghiems = phienXetNghiem.MauXetNghiems
                .Where(x => x.NhomDichVuBenhVienId == layMauXetNghiemVo.NhomDichVuBenhVienId)
                .GroupBy(x => new { x.LoaiMauXetNghiem })
                .Select(item => new MauXetNghiemCanLayLaiVo
                {
                    LoaiMauXetNghiem = item.Key.LoaiMauXetNghiem,
                    DatChatLuong = item.First().DatChatLuong.Value,
                    BarcodeNumber = item.First().BarCodeNumber.Value,
                    BarcodeId = item.First().BarCodeId
                }).ToList();

            if (!mauXetNghiems.Any() || mauXetNghiems.All(x => x.DatChatLuong))
            {
                throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
            }

            var flag = false;
            var mauXetNghiemCanLayLais = mauXetNghiems.Where(x => !x.DatChatLuong).ToList();
            foreach (var mauChiTiet in mauXetNghiemCanLayLais)
            {
                if (!mauChiTiet.DatChatLuong)
                {
                    flag = true;
                    var newMauXetNghiem = new MauXetNghiem()
                    {
                        NhomDichVuBenhVienId = layMauXetNghiemVo.NhomDichVuBenhVienId,
                        LoaiMauXetNghiem = mauChiTiet.LoaiMauXetNghiem,
                        SoLuongMau = 1,
                        BarCodeNumber = mauChiTiet.BarcodeNumber,
                        BarCodeId = mauChiTiet.BarcodeId,
                        DatChatLuong = true,
                        ThoiDiemLayMau = DateTime.Now,
                        NhanVienLayMauId = currentUserId,
                        PhongLayMauId = phongHienTaiId
                    };
                    phienXetNghiem.MauXetNghiems.Add(newMauXetNghiem);
                }
            }

            if (!flag)
            {
                throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
            }

            await BaseRepository.UpdateAsync(phienXetNghiem);
        }

        public async Task XuLyBenhNhanNhanKetQuaAsync(long yeuCauTiepNhanId)
        {
            var phienXetNghiems = await BaseRepository.Table
                .Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId)
                .ToListAsync();
            if (phienXetNghiems.Any() && phienXetNghiems.All(x => x.NhanVienKetLuanId != null) && phienXetNghiems.Any(x => x.DaTraKetQua != true))
            {
                foreach (var phienXetNghiem in phienXetNghiems)
                {
                    if (phienXetNghiem.DaTraKetQua != true)
                    {
                        phienXetNghiem.DaTraKetQua = true;
                        phienXetNghiem.ThoiDiemTraKetQua = DateTime.Now;
                    }
                }

                await BaseRepository.UpdateAsync(phienXetNghiems);
            }
            else
            {
                throw new Exception(_localizationService.GetResource("LayMauXetNghiem.TraKetQuaChoBenhNhan.IsEmpty"));
            }
        }

        public async Task<long> XuLyGuiMauXetNghiemAsync(GuiMauXetNghiemVo phieuGuiMauXetNghiem)
        {
            var newPhieuGuiMau = new PhieuGoiMauXetNghiem()
            {
                NhanVienGoiMauId = phieuGuiMauXetNghiem.NhanVienGuiMauId.Value,
                ThoiDiemGoiMau = phieuGuiMauXetNghiem.ThoiDiemGuiMau.Value,
                PhongNhanMauId = phieuGuiMauXetNghiem.PhongNhanMauId.Value,
                GhiChu = phieuGuiMauXetNghiem.GhiChu
            };

            // get list mẫu xét nghiệm
            var mauXetNghiemCanGuis = await _mauXetNghiemRepository.Table
                .Where(x => phieuGuiMauXetNghiem.NhomMauGuis.Any(a =>
                    a.PhienXetNghiemId == x.PhienXetNghiemId && a.NhomDichVuBenhVienId == x.NhomDichVuBenhVienId))
                .GroupBy(x => new { x.NhomDichVuBenhVienId, x.PhienXetNghiemId, x.LoaiMauXetNghiem })
                .Select(item => item.OrderByDescending(a => a.CreatedOn).First())
                .ToListAsync();
            if (mauXetNghiemCanGuis.Any(x => x.DatChatLuong == false))
            {
                throw new Exception(_localizationService.GetResource("GuiMau.DatChatLuong.KhongDat"));
            }

            foreach (var loaiMau in mauXetNghiemCanGuis)
            {
                newPhieuGuiMau.MauXetNghiems.Add(loaiMau);
            }

            await _phieuGoiMauXetNghiemRepository.UpdateAsync(newPhieuGuiMau);
            return newPhieuGuiMau.Id;
        }


        public async Task<string> InPhieuGuiMauXetNghiemAsync(InPhieuDGuimauXetNghiemVo inPhieu)
        {
            var content = string.Empty;
            var hearder = string.Empty;

            var phongId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = await _phongBenhVienRepository.GetByIdAsync(phongId, x => x.Include(y => y.KhoaPhong));

            var templateLinhThuongDuocPham = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuGuiMauXetNghiem"));

            var query = await _phieuGoiMauXetNghiemRepository.TableNoTracking
                .Include(x => x.NhanVienGoiMau).ThenInclude(y => y.User)
                .Include(x => x.NhanVienGoiMau).ThenInclude(y => y.KhoaPhongNhanViens).ThenInclude(z => z.KhoaPhong)
                .Include(x => x.PhongNhanMau)
                .Include(x => x.MauXetNghiems).ThenInclude(y => y.NhomDichVuBenhVien)
                .Include(x => x.MauXetNghiems).ThenInclude(y => y.PhienXetNghiem).ThenInclude(z => z.PhienXetNghiemChiTiets).ThenInclude(a => a.YeuCauDichVuKyThuat).ThenInclude(b => b.DichVuKyThuatBenhVien)
                .Include(x => x.MauXetNghiems).ThenInclude(y => y.PhienXetNghiem).ThenInclude(a => a.YeuCauTiepNhan).ThenInclude(b => b.BenhNhan)
                .Where(x => x.Id == inPhieu.PhieuGuiMauId)
                //.Select(item => new ThongTinInPhieuGuiMauXetNghiemVo()
                //{
                //    TenNguoiGuiMau = item.NhanVienGoiMau.User.HoTen,
                //    BoPhan = item.NhanVienGoiMau.KhoaPhongNhanViens.Select(x => x.KhoaPhong.Ten).FirstOrDefault(),
                //    GhiChu = item.GhiChu,
                //    GuiToiBoPhan = item.PhongNhanMau.Ten,
                //    Ngay = DateTime.Now.Day.ConvertDateToString(),
                //    Thang = DateTime.Now.Month.ConvertMonthToString(),
                //    Nam = DateTime.Now.Year.ConvertYearToString()
                //})
                .FirstAsync();

            var data = new ThongTinInPhieuGuiMauXetNghiemVo()
            {
                TenNguoiGuiMau = query.NhanVienGoiMau.User.HoTen,
                BoPhan = phongBenhVien.KhoaPhong?.Ten, //query.NhanVienGoiMau.KhoaPhongNhanViens.Select(x => x.KhoaPhong.Ten).FirstOrDefault(),
                GhiChu = query.GhiChu,
                GuiToiBoPhan = query.PhongNhanMau.Ten,
                Ngay = DateTime.Now.Day.ConvertDateToString(),
                Thang = DateTime.Now.Month.ConvertMonthToString(),
                Nam = DateTime.Now.Year.ConvertYearToString()
            };

            if (inPhieu.HasHeader)
            {
                hearder = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
                              "<th>PHIẾU GỬI MẪU</th>" +
                         "</p>";
            }

            var headerChild = "<table style='width:100%; border:1px solid #020000; border-collapse: collapse;'>"
                              + "<thead>"
                                + "<tr>"
                                    + "<th style = 'border:1px solid #020000; border-collapse:collapse;width:5%;text-align:center;padding: 5px;' > STT </ th >"
                                    + "<th style = 'border:1px solid #020000; border-collapse:collapse;width:20%;text-align:center;padding: 5px;'> Mã DV </ th >"
                                    + "<th style = 'border:1px solid #020000; border-collapse:collapse;width:45%;text-align:center;padding: 5px;'> Tên DV </ th >"
                                    + "<th style = 'border:1px solid #020000; border-collapse:collapse; width:15%;text-align:center;padding: 5px;'>Bệnh Phẩm </ th >"
                                    + "<th style = 'border:1px solid #020000; border-collapse:collapse;width:15%;text-align:center;padding: 5px;'>Loại Mẫu </ th >"
                                + "</tr>"
                              + "</thead>"
                              + "<tbody>{{0}}"
                              + "</tbody>"
                              + "</table>";

            var infoMauChiTiet = string.Empty;
            var nhomMauTheoPhienXetNghiems = query.MauXetNghiems
                .GroupBy(x => new { x.PhienXetNghiemId, x.NhomDichVuBenhVienId })
                .Select(x => x.First()).ToList();
            var STT = 1;
            foreach (var nhom in nhomMauTheoPhienXetNghiems)
            {
                data.TongSoLuongMau += 1;
                var lstLoaiMau = nhom.PhienXetNghiem
                    .PhienXetNghiemChiTiets
                    .Where(x => x.NhomDichVuBenhVienId == nhom.NhomDichVuBenhVienId)
                    .Select(x => x.YeuCauDichVuKyThuat.DichVuKyThuatBenhVien)
                    .Where(x => x.LoaiMauXetNghiem != null)
                    .Select(x => x.LoaiMauXetNghiem.Value.GetDescription())
                    .Distinct()
                    .ToList();
                infoMauChiTiet += "<tr style='border: 1px solid #020000;text-align: center; '>"
                                    + "<td style=''border: 1px solid #020000;text-align: center;'>" + STT++
                                    + "<td style = 'border: 1px solid #020000;text-align: left;'>" + nhom.NhomDichVuBenhVien.Ten
                                    + "<td style = 'border: 1px solid #020000;text-align: left;'>" + nhom.BarCodeId
                                    + "<td style = 'border: 1px solid #020000;text-align: left;'>" + string.Join(", ", lstLoaiMau)
                                    + "<td style = 'border: 1px solid #020000;text-align: left;'>" + nhom.PhienXetNghiem.YeuCauTiepNhan.MaYeuCauTiepNhan
                                    + "<td style = 'border: 1px solid #020000;text-align: left;'>" + nhom.PhienXetNghiem.YeuCauTiepNhan.BenhNhan.MaBN
                                    + "<td style = 'border: 1px solid #020000;text-align: left;'>" + nhom.PhienXetNghiem.YeuCauTiepNhan.HoTen
                                    + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (nhom.PhienXetNghiem.YeuCauTiepNhan.NamSinh == null ? "" : nhom.PhienXetNghiem.YeuCauTiepNhan.NamSinh.Value.ToString())
                                    + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (nhom.PhienXetNghiem.YeuCauTiepNhan.GioiTinh == null ? "" : nhom.PhienXetNghiem.YeuCauTiepNhan.GioiTinh.Value.GetDescription())
                                    + "</tr>";
                var phienChiTiets =
                    nhom.PhienXetNghiem.PhienXetNghiemChiTiets.Where(x =>
                        x.NhomDichVuBenhVienId == nhom.NhomDichVuBenhVienId).ToList();
                if (phienChiTiets.Any())
                {
                    infoMauChiTiet += "<tr><td></td><td colspan='8'>";

                    var sttDichVu = 1;
                    var isFirst = true;

                    foreach (var phienChiTiet in phienChiTiets)
                    {
                        if (isFirst)
                        {
                            isFirst = false;
                            infoMauChiTiet +=
                                "<table style='width:100%; border:1px solid #020000; border-collapse: collapse;'>"
                                + "<thead>"
                                + "<tr>"
                                + "<th style = 'border:1px solid #020000; border-collapse:collapse;width:5%;text-align:center;padding: 5px;' > STT </ th >"
                                + "<th style = 'border:1px solid #020000; border-collapse:collapse;width:20%;text-align:center;padding: 5px;'> Mã DV </ th >"
                                + "<th style = 'border:1px solid #020000; border-collapse:collapse;width:45%;text-align:center;padding: 5px;'> Tên DV </ th >"
                                + "<th style = 'border:1px solid #020000; border-collapse:collapse; width:15%;text-align:center;padding: 5px;'>Bệnh Phẩm </ th >"
                                + "<th style = 'border:1px solid #020000; border-collapse:collapse;width:15%;text-align:center;padding: 5px;'>Loại Mẫu </ th >"
                                + "</tr>"
                                + "</thead>"
                                + "<tbody>";
                        }
                        infoMauChiTiet += "<tr style='border: 1px solid #020000;text-align: center; '>"
                                        + "<td style=''border: 1px solid #020000;text-align: center;'>" + sttDichVu++
                                        + "<td style = 'border: 1px solid #020000;text-align: left;'>" + phienChiTiet.YeuCauDichVuKyThuat.MaDichVu
                                        + "<td style = 'border: 1px solid #020000;text-align: left;'>" + phienChiTiet.YeuCauDichVuKyThuat.TenDichVu
                                        + "<td style = 'border: 1px solid #020000;text-align: left;'>" + phienChiTiet.YeuCauDichVuKyThuat.BenhPhamXetNghiem
                                        + "<td style = 'border: 1px solid #020000;text-align: left;'>" + phienChiTiet.YeuCauDichVuKyThuat.DichVuKyThuatBenhVien.LoaiMauXetNghiem.GetDescription()
                                        + "</tr>";
                    }
                    infoMauChiTiet += "</tbody>"
                                    + "</table>";
                }
            }

            data.Header = hearder;
            data.DanhSachMau = infoMauChiTiet;

            content = TemplateHelpper.FormatTemplateWithContentTemplate(templateLinhThuongDuocPham.Body, data);
            return content;
        }
        #endregion

        #region Xử lý lấy, gửi và nhận mẫu 1 lần

        public async Task<long> XuLyGuiVaNhanMauXetNghiemAsync(LayMauXetNghiemXacNhanLayMauVo layMauXetNghiemVo)
        {
            var nhanVienId = _userAgentHelper.GetCurrentUserId();
            var phongHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();

            #region Xử lý gửi mẫu

            var phongXetnghiem = await _phongBenhVienRepository.TableNoTracking
                .Where(x => x.KhoaPhong.Ma == "XN")
                .FirstOrDefaultAsync();
            var phieuGuiMauVo = new GuiMauXetNghiemVo()
            {
                NhanVienGuiMauId = nhanVienId,
                ThoiDiemGuiMau = DateTime.Now,
                PhongNhanMauId = phongXetnghiem?.Id ?? phongHienTaiId,
                GhiChu = null
            };

            var queryObj = new LayMauXetNghiemTimKiemNangCaoVo()
            {
                TrangThai = new LayMauXetNghiemTrangThaiTimKiemNangCapVo()
                {
                    ChoGuiMau = true
                }
            };
            var queryString = JsonConvert.SerializeObject(queryObj);
            var gridNhomMauCanGui = await GetDataForGridNhomCanLayMauXetNghiemAsync(new QueryInfo()
            {
                SearchString = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("<AdvancedQueryParameters><SearchTerms>" + layMauXetNghiemVo.YeuCauTiepNhanId + "</SearchTerms></AdvancedQueryParameters>")),
                AdditionalSearchString = queryString
            });

            var nhomMauGuis = gridNhomMauCanGui.Data.Select(p => (NhomCanLayMauXetNghiemGridVo)p)
                .Where(x => x.NhomDichVuBenhVienId == layMauXetNghiemVo.NhomDichVuBenhVienId
                            && x.Barcode == layMauXetNghiemVo.BarcodeId).ToList();
            foreach (var nhomMau in nhomMauGuis)
            {
                var newNhomMau = new LayMauXetNghiemVo()
                {
                    YeuCauTiepNhanId = nhomMau.YeuCauTiepNhanId,
                    BenhNhanId = nhomMau.BenhNhanId,
                    PhienXetNghiemId = nhomMau.PhienXetNghiemId,
                    NhomDichVuBenhVienId = nhomMau.NhomDichVuBenhVienId,
                    BarcodeNumber = nhomMau.BarcodeNumber,
                    BarcodeId = nhomMau.Barcode
                };
                phieuGuiMauVo.NhomMauGuis.Add(newNhomMau);
            }
            var phieuGuiMauId = await XuLyGuiMauXetNghiemAsync(phieuGuiMauVo);
            #endregion

            #region Xử lý nhận mẫu
            await DuyetPhieuGuiMauXetNghiem(phieuGuiMauId, nhanVienId);
            #endregion

            return phieuGuiMauId;
        }

        public async Task XuLyHuyMauXetNghiemAsync(LayMauXetNghiemXacNhanLayMauVo layMauXetNghiemVo)
        {
            var phienXetNghiem = await BaseRepository.Table
                .Include(x => x.MauXetNghiems).ThenInclude(x => x.PhieuGoiMauXetNghiem).ThenInclude(t => t.MauXetNghiems)
                .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(y => y.KetQuaXetNghiemChiTiets)
                .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(y => y.YeuCauDichVuKyThuat)
                .Where(x => x.Id == layMauXetNghiemVo.PhienXetNghiemId)
                .FirstOrDefaultAsync();

            if (phienXetNghiem.ThoiDiemKetLuan != null)
            {
                throw new Exception(_localizationService.GetResource("LayMauXetNghiem.HuyMau.MauDaCoKetQuaHoacDangChayLai"));
            }

            var phienXetNghiemChiTiets = phienXetNghiem.PhienXetNghiemChiTiets
                .Where(x => x.NhomDichVuBenhVienId == layMauXetNghiemVo.NhomDichVuBenhVienId).ToList();
            foreach (var phienChiTiet in phienXetNghiemChiTiets)
            {
                phienChiTiet.WillDelete = true;
                phienChiTiet.YeuCauDichVuKyThuat.TrangThai = Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien;
                phienChiTiet.YeuCauDichVuKyThuat.ThoiDiemThucHien = null;
                phienChiTiet.YeuCauDichVuKyThuat.NhanVienThucHienId = null;

                foreach (var ketQua in phienChiTiet.KetQuaXetNghiemChiTiets)
                {
                    ketQua.WillDelete = true;
                }
            }

            var mauXetNghiems = phienXetNghiem.MauXetNghiems.Where(x =>
                x.NhomDichVuBenhVienId == layMauXetNghiemVo.NhomDichVuBenhVienId &&
                x.BarCodeId == layMauXetNghiemVo.BarcodeId).ToList();
            foreach (var mauXetNghiem in mauXetNghiems)
            {
                mauXetNghiem.WillDelete = true;
            }

            var phieuGuiMaus = phienXetNghiem.MauXetNghiems.Select(x => x.PhieuGoiMauXetNghiem).Where(x => x.MauXetNghiems.Any(y => mauXetNghiems.Any(t => t.Id == y.Id))).ToList();
            foreach (var phieuGuiMau in phieuGuiMaus)
            {
                if (phieuGuiMau.MauXetNghiems.All(x => mauXetNghiems.Any(t => t.Id == x.Id)))
                {
                    phieuGuiMau.WillDelete = true;
                }
            }

            if (phienXetNghiem.PhienXetNghiemChiTiets.All(x => x.WillDelete))
            {
                phienXetNghiem.WillDelete = true;
            }

            await BaseRepository.UpdateAsync(phienXetNghiem);
        }
        #endregion

        #region Cập nhật lấy mẫu
        public async Task XuLyCapBarcodeChoDichhVuDangChonAsync(CapBarcodeTheoDichVuVo capBarcodeTheoDichVuVo)
        {
            //03/11: bỏ await
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var phongHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();

            //BVHD-3200
            // Cho phép chọn lại barcode của bệnh nhân ở ngoại trú, nội trú
            var yeuCauTiepNhanKhacCuaBenhNhan = _yeuCauTiepNhanRepository.TableNoTracking
                .Include(x => x.YeuCauNhapVien).ThenInclude(y => y.YeuCauKhamBenh)
                .Include(x => x.YeuCauKhamBenhs).ThenInclude(y => y.YeuCauNhapViens).ThenInclude(z => z.YeuCauTiepNhans)
                .Where(x => x.Id == capBarcodeTheoDichVuVo.YeuCauTiepNhanId)
                .First();
            long? yeuCauTiepNhanKhacCuaBenhNhanId = null;
            if (yeuCauTiepNhanKhacCuaBenhNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru)
            {
                yeuCauTiepNhanKhacCuaBenhNhanId = yeuCauTiepNhanKhacCuaBenhNhan.YeuCauNhapVien?.YeuCauKhamBenh?.YeuCauTiepNhanId;
            }
            else
            {
                yeuCauTiepNhanKhacCuaBenhNhanId = yeuCauTiepNhanKhacCuaBenhNhan.YeuCauKhamBenhs
                    .Where(x => x.YeuCauNhapViens.Any())
                    .SelectMany(x => x.YeuCauNhapViens.Where(y => y.YeuCauTiepNhans.Any()))
                    .SelectMany(x => x.YeuCauTiepNhans)
                    .Select(x => x.Id)
                    .FirstOrDefault();
            }
            // kiểm tra trùng barcode
            var isExistBarcode = _mauXetNghiemRepository.TableNoTracking
                .Where(x => x.BarCodeNumber != null)
                .Any(x =>
                    x.PhienXetNghiem.YeuCauTiepNhanId != capBarcodeTheoDichVuVo.YeuCauTiepNhanId
                    && x.PhienXetNghiem.YeuCauTiepNhanId != yeuCauTiepNhanKhacCuaBenhNhanId
                    && x.BarCodeId == capBarcodeTheoDichVuVo.BarcodeId);
            if (isExistBarcode)
            {
                throw new Exception(_localizationService.GetResource("LayMauXetNghiem.BarcodeId.IsExists"));
            }

            // xử lý tạo phiên hoặc cập nhật
            var phienXetNghiem = BaseRepository.Table
                .Include(x => x.PhienXetNghiemChiTiets)
                .Include(x => x.MauXetNghiems)
                .Where(x => x.YeuCauTiepNhanId == capBarcodeTheoDichVuVo.YeuCauTiepNhanId)
                .LastOrDefault(x => x.BarCodeId == capBarcodeTheoDichVuVo.BarcodeId);

            // trường hợp cùng phiên
            if (phienXetNghiem != null && phienXetNghiem.NhanVienKetLuanId == null)
            {
                // xử lý cập nhật phiên: thêm phiên chi tiết, mẫu xét nghiệm
                var yeuCauDichVuKyThuats = _yeuCauDichVuKyThuatRepository.Table
                    .Include(x => x.DichVuKyThuatBenhVien)
                    .Where(x => x.YeuCauTiepNhanId == capBarcodeTheoDichVuVo.YeuCauTiepNhanId
                                && x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                //&& x.NhomDichVuBenhVien.NhomDichVuBenhVienChaId != null
                                && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && (x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan ||
                                    x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                                    x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan)
                                && !x.PhienXetNghiemChiTiets.Any()
                                && capBarcodeTheoDichVuVo.YeuCauDichVuKyThuatIds.Contains(x.Id))
                    .ToList();
                if (!yeuCauDichVuKyThuats.Any())
                {
                    throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
                }

                var flag = false;
                foreach (var dichVuKyThuat in yeuCauDichVuKyThuats)
                {
                    // xử lý tạo phiên chi tiết
                    dichVuKyThuat.TrangThai = dichVuKyThuat.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien ? Enums.EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien : dichVuKyThuat.TrangThai;
                    dichVuKyThuat.NhanVienThucHienId = capBarcodeTheoDichVuVo.NhanVienLayMauId != null ? capBarcodeTheoDichVuVo.NhanVienLayMauId : currentUserId;// BVHD-3836;
                    dichVuKyThuat.ThoiDiemThucHien = capBarcodeTheoDichVuVo.ThoiGianLayMau != null ? capBarcodeTheoDichVuVo.ThoiGianLayMau : DateTime.Now; // BVHD-3836;

                    var newPhienChiTiet = new PhienXetNghiemChiTiet()
                    {
                        NhomDichVuBenhVienId = dichVuKyThuat.NhomDichVuBenhVienId,
                        //YeuCauDichVuKyThuatId = dichVuKyThuat.Id,
                        YeuCauDichVuKyThuat = dichVuKyThuat,
                        DichVuKyThuatBenhVienId = dichVuKyThuat.DichVuKyThuatBenhVienId,
                        LanThucHien = 1,
                        ThoiDiemLayMau = capBarcodeTheoDichVuVo.ThoiGianLayMau != null ? capBarcodeTheoDichVuVo.ThoiGianLayMau : DateTime.Now, // BVHD-3836
                        NhanVienLayMauId = capBarcodeTheoDichVuVo.NhanVienLayMauId != null ? capBarcodeTheoDichVuVo.NhanVienLayMauId : currentUserId, // BVHD-3836
                        PhongLayMauId = phongHienTaiId
                    };
                    phienXetNghiem.PhienXetNghiemChiTiets.Add(newPhienChiTiet);

                    // xử lý tạo mẫu xét nghiệm
                    var loaiMau = dichVuKyThuat.DichVuKyThuatBenhVien.LoaiMauXetNghiem;
                    if (loaiMau == null)
                    {
                        throw new Exception(_localizationService.GetResource("LayMauXetNghiem.LoaiMau.Required"));
                    }
                    if (!phienXetNghiem.MauXetNghiems.Any(x =>
                        x.NhomDichVuBenhVienId == dichVuKyThuat.NhomDichVuBenhVienId
                        && x.LoaiMauXetNghiem == loaiMau))
                    {
                        //flag = true;
                        var newMauXetNghiem = new MauXetNghiem()
                        {
                            NhomDichVuBenhVienId = dichVuKyThuat.NhomDichVuBenhVienId,
                            LoaiMauXetNghiem = loaiMau.Value,
                            SoLuongMau = 1,
                            BarCodeNumber = capBarcodeTheoDichVuVo.BarcodeNumber,
                            BarCodeId = capBarcodeTheoDichVuVo.BarcodeId,
                            DatChatLuong = true,
                            ThoiDiemLayMau = capBarcodeTheoDichVuVo.ThoiGianLayMau != null ? capBarcodeTheoDichVuVo.ThoiGianLayMau : DateTime.Now, // BVHD-3836
                            NhanVienLayMauId = capBarcodeTheoDichVuVo.NhanVienLayMauId != null ? capBarcodeTheoDichVuVo.NhanVienLayMauId : currentUserId, // BVHD-3836
                            PhongLayMauId = phongHienTaiId
                        };
                        phienXetNghiem.MauXetNghiems.Add(newMauXetNghiem);
                    }
                }

                //if (!flag)
                //{
                //    throw new Exception(_localizationService.GetResource("LayMauXetNghiem.MauXetNghiems.Required"));
                //}
                BaseRepository.Update(phienXetNghiem);

            }
            // trường hợp tạo mới phiên
            else
            {
                var newPhienXetNhgiem = new PhienXetNghiem()
                {
                    BenhNhanId = capBarcodeTheoDichVuVo.BenhNhanId,
                    YeuCauTiepNhanId = capBarcodeTheoDichVuVo.YeuCauTiepNhanId,
                    MaSo = capBarcodeTheoDichVuVo.BarcodeNumber,
                    ThoiDiemBatDau = phienXetNghiem?.ThoiDiemBatDau ?? DateTime.Now
                };

                var lstYeuCauDichVuKyThuat = _yeuCauDichVuKyThuatRepository.Table
                    .Include(y => y.DichVuKyThuatBenhVien)
                    .Where(x => x.YeuCauTiepNhanId == capBarcodeTheoDichVuVo.YeuCauTiepNhanId
                                && x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && (x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan
                                    || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan
                                    || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                                && !x.PhienXetNghiemChiTiets.Any()
                                && capBarcodeTheoDichVuVo.YeuCauDichVuKyThuatIds.Contains(x.Id))
                    .ToList();
                if (!lstYeuCauDichVuKyThuat.Any())
                {
                    throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
                }

                foreach (var dichVuKyThuat in lstYeuCauDichVuKyThuat)
                {
                    // xử lý tạo phiên chi tiết
                    dichVuKyThuat.TrangThai = dichVuKyThuat.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien ? Enums.EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien : dichVuKyThuat.TrangThai;
                    dichVuKyThuat.NhanVienThucHienId = capBarcodeTheoDichVuVo.NhanVienLayMauId != null ? capBarcodeTheoDichVuVo.NhanVienLayMauId : currentUserId;// BVHD-3836;
                    dichVuKyThuat.ThoiDiemThucHien = capBarcodeTheoDichVuVo.ThoiGianLayMau != null ? capBarcodeTheoDichVuVo.ThoiGianLayMau : DateTime.Now; // BVHD-3836;

                    var newPhienChiTiet = new PhienXetNghiemChiTiet()
                    {
                        NhomDichVuBenhVienId = dichVuKyThuat.NhomDichVuBenhVienId,
                        YeuCauDichVuKyThuat = dichVuKyThuat,
                        DichVuKyThuatBenhVienId = dichVuKyThuat.DichVuKyThuatBenhVienId,
                        LanThucHien = 1,
                        ThoiDiemLayMau = capBarcodeTheoDichVuVo.ThoiGianLayMau != null ? capBarcodeTheoDichVuVo.ThoiGianLayMau : DateTime.Now, // BVHD-3836
                        NhanVienLayMauId = capBarcodeTheoDichVuVo.NhanVienLayMauId != null ? capBarcodeTheoDichVuVo.NhanVienLayMauId : currentUserId, // BVHD-3836
                        PhongLayMauId = phongHienTaiId
                    };
                    newPhienXetNhgiem.PhienXetNghiemChiTiets.Add(newPhienChiTiet);

                    // xử lý tạo mẫu xét nghiệm
                    var loaiMau = dichVuKyThuat.DichVuKyThuatBenhVien.LoaiMauXetNghiem;
                    if (loaiMau == null)
                    {
                        throw new Exception(_localizationService.GetResource("LayMauXetNghiem.LoaiMau.Required"));
                    }
                    if (!newPhienXetNhgiem.MauXetNghiems.Any(x =>
                        x.NhomDichVuBenhVienId == dichVuKyThuat.NhomDichVuBenhVienId
                        && x.LoaiMauXetNghiem == loaiMau))
                    {
                        var newMauXetNghiem = new MauXetNghiem()
                        {
                            NhomDichVuBenhVienId = dichVuKyThuat.NhomDichVuBenhVienId,
                            LoaiMauXetNghiem = loaiMau.Value,
                            SoLuongMau = 1,
                            BarCodeNumber = capBarcodeTheoDichVuVo.BarcodeNumber,
                            BarCodeId = capBarcodeTheoDichVuVo.BarcodeId,
                            DatChatLuong = true,
                            ThoiDiemLayMau = capBarcodeTheoDichVuVo.ThoiGianLayMau != null ? capBarcodeTheoDichVuVo.ThoiGianLayMau : DateTime.Now, // BVHD-3836
                            NhanVienLayMauId = capBarcodeTheoDichVuVo.NhanVienLayMauId != null ? capBarcodeTheoDichVuVo.NhanVienLayMauId : currentUserId, // BVHD-3836
                            PhongLayMauId = phongHienTaiId
                        };
                        newPhienXetNhgiem.MauXetNghiems.Add(newMauXetNghiem);
                    }
                }

                if (!newPhienXetNhgiem.MauXetNghiems.Any())
                {
                    throw new Exception(_localizationService.GetResource("LayMauXetNghiem.MauXetNghiems.Required"));
                }

                BaseRepository.Add(newPhienXetNhgiem);
            }
        }

        private async Task<bool> KiemTraNhanVienThuocKhoaXetNghiemAsync()
        {
            var khoaXetNghiem = _cauHinhService.GetSetting("CauHinhXetNghiem.KhoaXetNghiem");
            var khoaXetNghiemId = long.Parse(khoaXetNghiem.Value);

            //todo: update bỏ await
            var laNhanVienKhoaXetNghiem = _nhanVienRepository.TableNoTracking
                .Any(x => x.Id == _userAgentHelper.GetCurrentUserId()
                               && x.KhoaPhongNhanViens.Any()
                               && x.KhoaPhongNhanViens.Any(y => y.KhoaPhongId == khoaXetNghiemId));
            return laNhanVienKhoaXetNghiem;
        }

        public async Task XuLyXacNhanNhanMauXetNghiemAsync(XacNhanNhanMauChoDichVuVo xacNhanNhanMauVo)
        {
            var nhanVienId = _userAgentHelper.GetCurrentUserId();
            var phongHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var thoiDiemHienTai = DateTime.Now;

            //03/11: bỏ await
            var phienXetNghiems = BaseRepository.Table
                    .Include(x => x.BenhNhan)
                    .Include(x => x.MauXetNghiems).ThenInclude(y => y.PhieuGoiMauXetNghiem)
                    .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(y => y.KetQuaXetNghiemChiTiets)
                    .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(y => y.DichVuKyThuatBenhVien)
                .Where(x => x.YeuCauTiepNhanId == xacNhanNhanMauVo.YeuCauTiepNhanId
                            && x.PhienXetNghiemChiTiets.Any(a => xacNhanNhanMauVo.YeuCauDichVuKyThuatIds.Contains(a.YeuCauDichVuKyThuatId)))
                .ToList();
            if (phienXetNghiems.Any(x => x.ThoiDiemKetLuan != null))
            {
                throw new Exception(_localizationService.GetResource("LayMauXetNghiem.HuyMau.MauDaCoKetQuaHoacDangChayLai"));
            }
            var ketQuaXetNghiemTruocs = GetKetQuaXetNghiemTruocCuaBenhNhan(phienXetNghiems.First().BenhNhanId);
            //var phienXetNghiem = await BaseRepository.Table
            //    .Include(x => x.BenhNhan)
            //    .Include(x => x.MauXetNghiems).ThenInclude(y => y.PhieuGoiMauXetNghiem)
            //    .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(y => y.KetQuaXetNghiemChiTiets)
            //    .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(y => y.DichVuKyThuatBenhVien)
            //    .Where(x => x.PhienXetNghiemChiTiets.Any(a => a.Id == phienXetNghiemChiTietId))
            //    .FirstOrDefaultAsync();

            var dichVuXetNghiems = BaseRepository.Context
                .Set<Core.Domain.Entities.DichVuXetNghiems.DichVuXetNghiem>().AsNoTracking().ToList();
            var dichVuXetNghiemKetNoiChiSos = BaseRepository.Context.Set<DichVuXetNghiemKetNoiChiSo>()
                .AsNoTracking().ToList();
            foreach (var phienXetNghiem in phienXetNghiems)
            {
                var phienXetNghiemChiTiets = phienXetNghiem.PhienXetNghiemChiTiets
                    .Where(x => xacNhanNhanMauVo.YeuCauDichVuKyThuatIds.Any(a => a == x.YeuCauDichVuKyThuatId)).ToList();
                if (phienXetNghiemChiTiets.Any(x => x.ChayLaiKetQua == true))
                {
                    throw new Exception(_localizationService.GetResource("GridDaCapCode.DichVuXetNghiem.DangChayLai"));
                }

                foreach (var phienChiTietCanXacNhanNhanMau in phienXetNghiemChiTiets)
                {
                    //var phienChiTietCanXacNhanNhanMau =
                    //    phienXetNghiem.PhienXetNghiemChiTiets.First(x => x.Id == phienXetNghiemChiTietId);
                    if (phienChiTietCanXacNhanNhanMau.KetQuaXetNghiemChiTiets.Any() || phienChiTietCanXacNhanNhanMau.ThoiDiemNhanMau != null)
                    {
                        throw new Exception(
                            _localizationService.GetResource("NhanMau.PhienXetNghiemChiTiet.DaNhanMau"));
                    }

                    if (phienChiTietCanXacNhanNhanMau.DichVuKyThuatBenhVien.DichVuXetNghiemId == null)
                    {
                        throw new Exception(_localizationService.GetResource("NhanMau.DichVuXetNghiem.NotExists"));
                    }

                    var mauXetNghiem = phienXetNghiem.MauXetNghiems.OrderByDescending(x => x.Id)
                        .First(x => x.NhomDichVuBenhVienId == phienChiTietCanXacNhanNhanMau.NhomDichVuBenhVienId
                                    && x.LoaiMauXetNghiem == phienChiTietCanXacNhanNhanMau.DichVuKyThuatBenhVien.LoaiMauXetNghiem);
                    if (mauXetNghiem.DatChatLuong == false)
                    {
                        throw new Exception(_localizationService.GetResource("GuiMau.DatChatLuong.KhongDat"));
                    }

                    var phieuGuiMau = new PhieuGoiMauXetNghiem();
                    if (mauXetNghiem.PhieuGoiMauXetNghiemId == null)
                    {
                        phieuGuiMau = new PhieuGoiMauXetNghiem()
                        {
                            NhanVienGoiMauId = nhanVienId,
                            ThoiDiemGoiMau = thoiDiemHienTai,
                            PhongNhanMauId = phongHienTaiId,
                            GhiChu = "",
                            ThoiDiemNhanMau = thoiDiemHienTai,
                            NhanVienNhanMauId = nhanVienId,
                            DaNhanMau = true
                        };
                        
                        mauXetNghiem.PhieuGoiMauXetNghiem = phieuGuiMau;

                        phienChiTietCanXacNhanNhanMau.ThoiDiemNhanMau = mauXetNghiem.ThoiDiemNhanMau = phieuGuiMau.ThoiDiemNhanMau;
                        phienChiTietCanXacNhanNhanMau.NhanVienNhanMauId = mauXetNghiem.NhanVienNhanMauId = phieuGuiMau.NhanVienNhanMauId;
                        phienChiTietCanXacNhanNhanMau.PhongNhanMauId = mauXetNghiem.PhongNhanMauId = phieuGuiMau.PhongNhanMauId;
                    }
                    else
                    {
                        // cập nhật 26/05/2021: kkhi nhận mẫu, lấy thông tin tại thời điểm hiện tại
                        //phieuGuiMau = mauXetNghiem.PhieuGoiMauXetNghiem;

                        phienChiTietCanXacNhanNhanMau.ThoiDiemNhanMau = thoiDiemHienTai; // phieuGuiMau.ThoiDiemNhanMau;
                        phienChiTietCanXacNhanNhanMau.NhanVienNhanMauId = nhanVienId; // phieuGuiMau.NhanVienNhanMauId;
                        phienChiTietCanXacNhanNhanMau.PhongNhanMauId = phongHienTaiId; // phieuGuiMau.PhongNhanMauId;


                        #region // BVHD-3372
                        // bổ sung dùng chung cho chức năng nhận mẫu
                        // vì khi hủy nhận mẫu thì ok xóa mẫu mà chuyển phiên chi tiết về đã cấp code, và clear thông tin nhận mẫu của mẫu hiện tại
                        if (mauXetNghiem.ThoiDiemNhanMau == null)
                        {
                            mauXetNghiem.ThoiDiemNhanMau = thoiDiemHienTai;
                        }

                        if (mauXetNghiem.NhanVienNhanMauId == null)
                        {
                            mauXetNghiem.NhanVienNhanMauId = nhanVienId;
                        }

                        if (mauXetNghiem.PhongNhanMauId == null)
                        {
                            mauXetNghiem.PhongNhanMauId = phongHienTaiId;
                        }
                        #endregion
                    }

                    //var dichVuXetNghiems = await BaseRepository.Context
                    //    .Set<Core.Domain.Entities.DichVuXetNghiems.DichVuXetNghiem>().AsNoTracking().ToListAsync();
                    //var dichVuXetNghiemKetNoiChiSos = await BaseRepository.Context.Set<DichVuXetNghiemKetNoiChiSo>()
                    //    .AsNoTracking().ToListAsync();
                    if (!phienChiTietCanXacNhanNhanMau.KetQuaXetNghiemChiTiets.Any() &&
                        phienChiTietCanXacNhanNhanMau.DichVuKyThuatBenhVien.DichVuXetNghiemId != null)
                    {
                        AddKetQuaXetNghiemChiTiet(phienChiTietCanXacNhanNhanMau,
                            dichVuXetNghiems.First(o =>
                                o.Id == phienChiTietCanXacNhanNhanMau.DichVuKyThuatBenhVien.DichVuXetNghiemId),
                            phienXetNghiem.BenhNhan, dichVuXetNghiems, dichVuXetNghiemKetNoiChiSos, ketQuaXetNghiemTruocs);
                        //update 02/11 xử lý ds phiên XN
                        if(phienXetNghiem.ChoKetQua == null)
                        {
                            phienXetNghiem.ChoKetQua = true;
                        }
                    }
                }
            }

            BaseRepository.Context.SaveChanges();
        }

        public async Task XuLyXacNhanHuyCapBarcodeTheoDichVuAsync(XacNhanNhanMauChoDichVuVo xacNhanNhanMauVo)
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
                    phienChiTiet.WillDelete = true;
                    phienChiTiet.YeuCauDichVuKyThuat.TrangThai = Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien;
                    phienChiTiet.YeuCauDichVuKyThuat.ThoiDiemThucHien = null;
                    phienChiTiet.YeuCauDichVuKyThuat.NhanVienThucHienId = null;

                    foreach (var ketQua in phienChiTiet.KetQuaXetNghiemChiTiets)
                    {
                        ketQua.WillDelete = true;
                    }

                    if(!phienXetNghiem.PhienXetNghiemChiTiets.Any(x => x.WillDelete != true 
                                                                      && x.NhomDichVuBenhVienId == phienChiTiet.NhomDichVuBenhVienId 
                                                                      && x.DichVuKyThuatBenhVien.LoaiMauXetNghiem == phienChiTiet.DichVuKyThuatBenhVien.LoaiMauXetNghiem))
                    {
                        var mauXetNghiems = phienXetNghiem.MauXetNghiems.Where(x => x.NhomDichVuBenhVienId == phienChiTiet.NhomDichVuBenhVienId 
                                                                                    && x.LoaiMauXetNghiem == phienChiTiet.DichVuKyThuatBenhVien.LoaiMauXetNghiem
                                                                                    && x.BarCodeId == phienXetNghiem.BarCodeId).ToList();
                        foreach (var mauXetNghiem in mauXetNghiems)
                        {
                            mauXetNghiem.WillDelete = true;
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

                if (phienXetNghiem.PhienXetNghiemChiTiets.All(x => x.WillDelete))
                {
                    phienXetNghiem.WillDelete = true;
                }
            }

            await BaseRepository.Context.SaveChangesAsync();
        }

        public async Task CapNhatGridItemThoiGianNhanMauAsync(CapNhatGridItemChoDichVuDaCapCodeVo capNhatNgayNhanMauVo)
        {
            var phienChiTiet = await _phienXetNghiemChiTietRepository.Table
                .Where(x => x.PhienXetNghiem.YeuCauTiepNhanId == capNhatNgayNhanMauVo.YeuCauTiepNhanId
                            && x.YeuCauDichVuKyThuatId == capNhatNgayNhanMauVo.YeuCauDichVuKyThuatId)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();
            if (phienChiTiet == null)
            {
                throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
            }

            phienChiTiet.ThoiDiemNhanMau = capNhatNgayNhanMauVo.NgayNhanMau;
            await BaseRepository.Context.SaveChangesAsync();
        }
        #endregion
    }
}
