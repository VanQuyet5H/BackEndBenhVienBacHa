using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.FileKetQuaCanLamSangs;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DanhSachBenhNhanChoThuNgan;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities.Encoders;

namespace Camino.Services.YeuCauTiepNhans
{
    public partial class YeuCauTiepNhanService
    {
        #region

        public async Task<GridDataSource> GetCanLamSangDataForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var timKiemNangCaoObj = new KetQuaCDHATDCNTimKiemVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<KetQuaCDHATDCNTimKiemVo>(queryInfo.AdditionalSearchString);
                timKiemNangCaoObj.SearchString = timKiemNangCaoObj.SearchString == null ? null : timKiemNangCaoObj.SearchString.Trim().TrimStart().TrimEnd();
            }

            var noiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();

            var query1 = _yeuCauDichVuKyThuatRepository.TableNoTracking.Where(x => x.NoiThucHienId != null
                             && x.NoiThucHienId == noiThucHienId
                             && (x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh)
                             && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                             && (x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan))
                 .Select(item => new KetQuaCDHATDCNTimKiemGridVo()
                 {
                     Id = item.YeuCauTiepNhan.Id,
                     YeuCauDichVuKyThuatId = item.Id,
                     MaYeuCauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                     SoBenhAn = item.YeuCauTiepNhan.NoiTruBenhAn != null ? item.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn : null,
                     MaBN = item.YeuCauTiepNhan.BenhNhan.MaBN,

                     NgaySinh = item.YeuCauTiepNhan.NgaySinh,
                     ThangSinh = item.YeuCauTiepNhan.ThangSinh,
                     NamSinh = item.YeuCauTiepNhan.NamSinh,

                     HoTen = item.YeuCauTiepNhan.HoTen,
                     GioiTinh = item.YeuCauTiepNhan.GioiTinh.GetDescription(),
                     DoiTuong = item.YeuCauTiepNhan.CoBHYT == true ? "BHYT" : "Viện phí",
                     TrangThai = item.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien,
                     NgayChiDinh = item.ThoiDiemChiDinh,
                     NoiChiDinh = item.NoiChiDinh != null ? item.NoiChiDinh.Ten : "",
                     ChiDinh = item.TenDichVu,
                     NgayThucHien = item.ThoiDiemThucHien,
                     //ChuanDoan = item.NoiTruPhieuDieuTri != null ? item.NoiTruPhieuDieuTri.ChanDoanChinhICD.TenTiengViet : item.YeuCauKhamBenh.Icdchinh.TenTiengViet,
                     //BacSiKetLuan = item.NhanVienKetLuan.User.HoTen,
                     //KyThuatVien1 = item.NhanVienThucHien.User.HoTen,
                     //BacSiCD = item.NhanVienChiDinh.User.HoTen,
                     //DataKetQuaCanLamSang = item.DataKetQuaCanLamSang
                 }).ApplyLike(timKiemNangCaoObj.SearchString, x => x.MaYeuCauTiepNhan, x => x.MaBN, x => x.SoBenhAn, x => x.HoTen, x => x.DoiTuong);

            if (timKiemNangCaoObj.TuNgayDenNgay != null && (!string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay) || !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay)))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);

                query1 = query1.Where(p => (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay) || p.NgayChiDinh.Date >= tuNgay.Date)
                                         && (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay) || p.NgayChiDinh.Date <= denNgay.Date));
            }

            if (timKiemNangCaoObj.ThucHienTuNgayDenNgay != null && (!string.IsNullOrEmpty(timKiemNangCaoObj.ThucHienTuNgayDenNgay.TuNgay) || !string.IsNullOrEmpty(timKiemNangCaoObj.ThucHienTuNgayDenNgay.DenNgay)))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.ThucHienTuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                DateTime.TryParseExact(timKiemNangCaoObj.ThucHienTuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);

                query1 = query1.Where(p => (string.IsNullOrEmpty(timKiemNangCaoObj.ThucHienTuNgayDenNgay.TuNgay) || p.NgayThucHien.Value.Date >= tuNgay.Date)
                                         && (string.IsNullOrEmpty(timKiemNangCaoObj.ThucHienTuNgayDenNgay.DenNgay) || p.NgayThucHien.Value.Date <= denNgay.Date));
            }


            if (timKiemNangCaoObj.TrangThai != null && (timKiemNangCaoObj.TrangThai.ChoKetQua || timKiemNangCaoObj.TrangThai.DaCoKetQua))
            {
                query1 = query1.Where(x => (timKiemNangCaoObj.TrangThai.ChoKetQua && x.TrangThai == false)
                                      || (timKiemNangCaoObj.TrangThai.DaCoKetQua && x.TrangThai));
            }
            var dataYeuCauDichVuKyThuats = query1.ToList();

            var groupByYctns = dataYeuCauDichVuKyThuats.GroupBy(o => new { o.Id })
             .Select(item => new KetQuaCDHATDCNTimKiemGridVo()
             {
                 Id = item.First().Id,
                 MaYeuCauTiepNhan = item.First().MaYeuCauTiepNhan,
                 SoBenhAn = item.First().SoBenhAn,
                 MaBN = item.First().MaBN,
                 HoTen = item.First().HoTen,
                 GioiTinh = item.First().GioiTinh,
                 DoiTuong = item.First().DoiTuong,
                 TrangThai = item.All(o => o.TrangThai),
                 NgaySinh = item.First().NgaySinh,
                 ThangSinh = item.First().ThangSinh,
                 NamSinh = item.First().NamSinh,
                 NgayChiDinh = item.OrderByDescending(o => o.NgayChiDinh).First().NgayChiDinh,
                 NgayChiDinhDisplay = String.Join("; ", item.OrderByDescending(o => o.NgayChiDinh).Select(o => o.NgayChiDinh.ApplyFormatDateTime()).ToArray()),
                 NoiChiDinh = String.Join("; ", item.Select(o => o.NoiChiDinh).ToArray()),
                 ChiDinh = "(" + item.Where(o => o.TrangThai).Count() + "/" + item.Count() + ") " + String.Join("; ", item.Select(o => o.ChiDinh).ToArray()),
                 NgayThucHien = item.First().NgayThucHien,
                 NgayThucHienDisplay = String.Join("; ", item.Where(o => o.NgayThucHien != null).OrderByDescending(o => o.NgayThucHien).Select(o => (o.NgayThucHien != null ? ((DateTime)o.NgayThucHien).ApplyFormatDateTime() : "")).ToArray()),
                 //ChuanDoanDisplay = String.Join("; ", item.Where(o => o.ChuanDoan != null).Select(o => (o.ChuanDoan != null ? o.ChuanDoan : "")).ToArray()),
                 //BacSiKetLuanDisplay = String.Join("; ", item.Where(o => o.BacSiKetLuan != null).Select(o => (o.BacSiKetLuan != null ? o.BacSiKetLuan : "")).Distinct().ToArray()),
                 //KyThuatVien1Display = String.Join("; ", item.Where(o => o.KyThuatVien1 != null).Select(o => (o.KyThuatVien1 != null ? o.KyThuatVien1 : "")).Distinct().ToArray()),
                 //BacSiCDDisplay = String.Join("; ", item.Where(o => o.BacSiCD != null).Select(o => (o.BacSiCD != null ? o.BacSiCD : "")).Distinct().ToArray()),
                 //KetLuanDisplay = "(" + item.Where(o => o.TrangThai).Count() + "/" + item.Count() + ") " + String.Join("; ", item.Where(o => !string.IsNullOrEmpty(o.DataKetQuaCanLamSang))
                 //.Select(oo => JsonConvert.DeserializeObject<ChiTietKetQuaCDHATDCNVo>(oo.DataKetQuaCanLamSang)).Select(c => MaskHelper.RemoveHtmlFromString(c.KetLuan)).ToArray()),
             });

            var totalRow = groupByYctns.Count();
            var dataReturn = groupByYctns.AsQueryable().OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
            var yeuCauDichVuKyThuatIds = dataYeuCauDichVuKyThuats.Where(o => dataReturn.Select(yctn => yctn.Id).Contains(o.Id)).Select(o => o.YeuCauDichVuKyThuatId).ToList();
            var dataYeuCauDichVuKyThuatDetails = _yeuCauDichVuKyThuatRepository.TableNoTracking.Where(x => yeuCauDichVuKyThuatIds.Contains(x.Id))
                 .Select(item => new KetQuaCDHATDCNTimKiemGridVo()
                 {
                     Id = item.YeuCauTiepNhan.Id,
                     YeuCauDichVuKyThuatId = item.Id,
                     TrangThai = item.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien,
                     NgayChiDinh = item.ThoiDiemChiDinh,
                     NgayThucHien = item.ThoiDiemThucHien,
                     ChuanDoan = item.NoiTruPhieuDieuTri != null ? item.NoiTruPhieuDieuTri.ChanDoanChinhICD.TenTiengViet : item.YeuCauKhamBenh.Icdchinh.TenTiengViet,
                     BacSiKetLuan = item.NhanVienKetLuan.User.HoTen,
                     KyThuatVien1 = item.NhanVienThucHien.User.HoTen,
                     BacSiCD = item.NhanVienChiDinh.User.HoTen,
                     DataKetQuaCanLamSang = item.DataKetQuaCanLamSang
                 }).ToList();

            foreach (var data in dataReturn)
            {
                var yeuCauDichVuKyThuatDetails = dataYeuCauDichVuKyThuatDetails.Where(o => o.Id == data.Id).OrderBy(o => o.NgayChiDinh).ToList();

                data.ChuanDoanDisplay = String.Join("; ", yeuCauDichVuKyThuatDetails.Where(o => o.ChuanDoan != null).Select(o => (o.ChuanDoan != null ? o.ChuanDoan : "")).ToArray());
                data.BacSiKetLuanDisplay = String.Join("; ", yeuCauDichVuKyThuatDetails.Where(o => o.BacSiKetLuan != null).Select(o => (o.BacSiKetLuan != null ? o.BacSiKetLuan : "")).Distinct().ToArray());
                data.KyThuatVien1Display = String.Join("; ", yeuCauDichVuKyThuatDetails.Where(o => o.KyThuatVien1 != null).Select(o => (o.KyThuatVien1 != null ? o.KyThuatVien1 : "")).Distinct().ToArray());
                data.BacSiCDDisplay = String.Join("; ", yeuCauDichVuKyThuatDetails.Where(o => o.BacSiCD != null).Select(o => (o.BacSiCD != null ? o.BacSiCD : "")).Distinct().ToArray());
                data.KetLuanDisplay = "(" + yeuCauDichVuKyThuatDetails.Where(o => o.TrangThai).Count() + "/" + yeuCauDichVuKyThuatDetails.Count() + ") " + String.Join("; ", yeuCauDichVuKyThuatDetails.Where(o => !string.IsNullOrEmpty(o.DataKetQuaCanLamSang))
                .Select(oo => JsonConvert.DeserializeObject<ChiTietKetQuaCDHATDCNVo>(oo.DataKetQuaCanLamSang)).Select(c => MaskHelper.RemoveHtmlFromString(c.KetLuan)).ToArray());
            }
            return new GridDataSource { Data = dataReturn, TotalRowCount = totalRow };


            //var query = _yeuCauDichVuKyThuatRepository.TableNoTracking.Where(x => x.NoiThucHienId != null
            //                 && x.NoiThucHienId == noiThucHienId
            //                 && (x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh)
            //                 && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
            //                 && (x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan))
            //     .Select(item => new KetQuaCDHATDCNTimKiemGridVo()
            //     {
            //         Id = item.YeuCauTiepNhan.Id,
            //         MaYeuCauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
            //         SoBenhAn = item.YeuCauTiepNhan.NoiTruBenhAn != null ? item.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn : null,
            //         MaBN = item.YeuCauTiepNhan.BenhNhan.MaBN,

            //         NgaySinh = item.YeuCauTiepNhan.NgaySinh,
            //         ThangSinh = item.YeuCauTiepNhan.ThangSinh,
            //         NamSinh = item.YeuCauTiepNhan.NamSinh,

            //         HoTen = item.YeuCauTiepNhan.HoTen,
            //         GioiTinh = item.YeuCauTiepNhan.GioiTinh.GetDescription(),
            //         DoiTuong = item.YeuCauTiepNhan.CoBHYT == true ? "BHYT" : "Viện phí",
            //         TrangThai = item.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien,
            //         NgayChiDinh = item.ThoiDiemChiDinh,
            //         NoiChiDinh = item.NoiChiDinh != null ? item.NoiChiDinh.Ten : "",
            //         ChiDinh = item.TenDichVu,
            //         NgayThucHien = item.ThoiDiemThucHien,
            //         ChuanDoan = item.NoiTruPhieuDieuTri != null ? item.NoiTruPhieuDieuTri.ChanDoanChinhICD.TenTiengViet : item.YeuCauKhamBenh.Icdchinh.TenTiengViet,
            //         BacSiKetLuan = item.NhanVienKetLuan.User.HoTen,
            //         KyThuatVien1 = item.NhanVienThucHien.User.HoTen,
            //         BacSiCD = item.NhanVienChiDinh.User.HoTen,
            //         DataKetQuaCanLamSang = item.DataKetQuaCanLamSang
            //     }).ApplyLike(timKiemNangCaoObj.SearchString, x => x.MaYeuCauTiepNhan, x => x.MaBN, x => x.SoBenhAn, x => x.HoTen, x => x.DoiTuong);

            //if (timKiemNangCaoObj.TuNgayDenNgay != null && (!string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay) || !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay)))
            //{
            //    DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
            //    DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);

            //    query = query.Where(p => (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay) || p.NgayChiDinh.Date >= tuNgay.Date)
            //                             && (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay) || p.NgayChiDinh.Date <= denNgay.Date));
            //}

            //if (timKiemNangCaoObj.ThucHienTuNgayDenNgay != null && (!string.IsNullOrEmpty(timKiemNangCaoObj.ThucHienTuNgayDenNgay.TuNgay) || !string.IsNullOrEmpty(timKiemNangCaoObj.ThucHienTuNgayDenNgay.DenNgay)))
            //{
            //    DateTime.TryParseExact(timKiemNangCaoObj.ThucHienTuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
            //    DateTime.TryParseExact(timKiemNangCaoObj.ThucHienTuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);

            //    query = query.Where(p => (string.IsNullOrEmpty(timKiemNangCaoObj.ThucHienTuNgayDenNgay.TuNgay) || p.NgayThucHien.Value.Date >= tuNgay.Date)
            //                             && (string.IsNullOrEmpty(timKiemNangCaoObj.ThucHienTuNgayDenNgay.DenNgay) || p.NgayThucHien.Value.Date <= denNgay.Date));
            //}


            //if (timKiemNangCaoObj.TrangThai != null && (timKiemNangCaoObj.TrangThai.ChoKetQua || timKiemNangCaoObj.TrangThai.DaCoKetQua))
            //{
            //    var danhSachTimKiemKQ = query.Where(x => (timKiemNangCaoObj.TrangThai.ChoKetQua && x.TrangThai == false)
            //                          || (timKiemNangCaoObj.TrangThai.DaCoKetQua && x.TrangThai));
            //    query = danhSachTimKiemKQ;
            //}

            //query = query.GroupBy(o => new { o.Id, o.MaYeuCauTiepNhan, o.SoBenhAn, o.MaBN, o.HoTen, o.GioiTinh, o.DoiTuong })
            // .Select(item => new KetQuaCDHATDCNTimKiemGridVo()
            // {
            //     Id = item.First().Id,
            //     MaYeuCauTiepNhan = item.First().MaYeuCauTiepNhan,
            //     SoBenhAn = item.First().SoBenhAn,
            //     MaBN = item.First().MaBN,
            //     HoTen = item.First().HoTen,
            //     GioiTinh = item.First().GioiTinh,
            //     DoiTuong = item.First().DoiTuong,
            //     TrangThai = item.All(o => o.TrangThai),
            //     NgaySinh = item.First().NgaySinh,
            //     ThangSinh = item.First().ThangSinh,
            //     NamSinh = item.First().NamSinh,
            //     NgayChiDinh = item.OrderByDescending(o => o.NgayChiDinh).First().NgayChiDinh,
            //     NgayChiDinhDisplay = String.Join("; ", item.OrderByDescending(o => o.NgayChiDinh).Select(o => o.NgayChiDinh.ApplyFormatDateTime()).ToArray()),
            //     NoiChiDinh = String.Join("; ", item.Select(o => o.NoiChiDinh).ToArray()),
            //     ChiDinh = "(" + item.Where(o => o.TrangThai).Count() + "/" + item.Count() + ") " + String.Join("; ", item.Select(o => o.ChiDinh).ToArray()),
            //     NgayThucHien = item.First().NgayThucHien,
            //     NgayThucHienDisplay = String.Join("; ", item.Where(o => o.NgayThucHien != null).OrderByDescending(o => o.NgayThucHien).Select(o => (o.NgayThucHien != null ? ((DateTime)o.NgayThucHien).ApplyFormatDateTime() : "")).ToArray()),
            //     ChuanDoanDisplay = String.Join("; ", item.Where(o => o.ChuanDoan != null).Select(o => (o.ChuanDoan != null ? o.ChuanDoan : "")).ToArray()),
            //     BacSiKetLuanDisplay = String.Join("; ", item.Where(o => o.BacSiKetLuan != null).Select(o => (o.BacSiKetLuan != null ? o.BacSiKetLuan : "")).Distinct().ToArray()),
            //     KyThuatVien1Display = String.Join("; ", item.Where(o => o.KyThuatVien1 != null).Select(o => (o.KyThuatVien1 != null ? o.KyThuatVien1 : "")).Distinct().ToArray()),
            //     BacSiCDDisplay = String.Join("; ", item.Where(o => o.BacSiCD != null).Select(o => (o.BacSiCD != null ? o.BacSiCD : "")).Distinct().ToArray()),
            //     KetLuanDisplay = "(" + item.Where(o => o.TrangThai).Count() + "/" + item.Count() + ") " + String.Join("; ", item.Where(o => !string.IsNullOrEmpty(o.DataKetQuaCanLamSang))
            //     .Select(oo => JsonConvert.DeserializeObject<ChiTietKetQuaCDHATDCNVo>(oo.DataKetQuaCanLamSang)).Select(c => MaskHelper.RemoveHtmlFromString(c.KetLuan)).ToArray()),
            // });

            //if (queryInfo.Sort.Count == 1 && string.IsNullOrEmpty(queryInfo.Sort[0].Dir))
            //{
            //    queryInfo.Sort[0].Dir = "asc";
            //    queryInfo.Sort[0].Field = "TrangThai";
            //}

            //var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            //var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();

            //await Task.WhenAll(countTask, queryTask);
            //return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetCanLamSangTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new KetQuaCDHATDCNTimKiemVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<KetQuaCDHATDCNTimKiemVo>(queryInfo.AdditionalSearchString);
                timKiemNangCaoObj.SearchString = timKiemNangCaoObj.SearchString == null ? null : timKiemNangCaoObj.SearchString.Trim().TrimStart().TrimEnd();
            }

            var noiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var query = _yeuCauDichVuKyThuatRepository.TableNoTracking.Where(x => x.NoiThucHienId != null
                             && x.NoiThucHienId == noiThucHienId
                             //&& x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien
                             && (x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh)
                             && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                             // && (x.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien || x.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                             && (x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan))
                 .Select(item => new KetQuaCDHATDCNTimKiemGridVo()
                 {
                     Id = item.YeuCauTiepNhan.Id,
                     MaYeuCauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                     SoBenhAn = item.YeuCauTiepNhan.NoiTruBenhAn != null ? item.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn : null,
                     MaBN = item.YeuCauTiepNhan.BenhNhan.MaBN,
                     HoTen = item.YeuCauTiepNhan.HoTen,
                     GioiTinh = item.YeuCauTiepNhan.GioiTinh.GetDescription(),
                     DoiTuong = item.YeuCauTiepNhan.CoBHYT == true ? "BHYT" : "Viện phí",
                     TrangThai = item.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien,
                     NgayChiDinh = item.ThoiDiemChiDinh,
                     NoiChiDinh = item.NoiChiDinh != null ? item.NoiChiDinh.Ten : "",
                     ChiDinh = item.TenDichVu,
                     NgayThucHien = item.ThoiDiemThucHien
                 }).ApplyLike(timKiemNangCaoObj.SearchString, x => x.MaYeuCauTiepNhan, x => x.MaBN, x => x.SoBenhAn, x => x.HoTen, x => x.DoiTuong);

            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.TuNgayDenNgay != null && (!string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay) || !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay)))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);

                query = query.Where(p => (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay) || p.NgayChiDinh.Date >= tuNgay.Date)
                                         && (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay) || p.NgayChiDinh.Date <= denNgay.Date));
            }

            if (timKiemNangCaoObj.ThucHienTuNgayDenNgay != null && (!string.IsNullOrEmpty(timKiemNangCaoObj.ThucHienTuNgayDenNgay.TuNgay) || !string.IsNullOrEmpty(timKiemNangCaoObj.ThucHienTuNgayDenNgay.DenNgay)))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.ThucHienTuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                DateTime.TryParseExact(timKiemNangCaoObj.ThucHienTuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);

                query = query.Where(p => (string.IsNullOrEmpty(timKiemNangCaoObj.ThucHienTuNgayDenNgay.TuNgay) || p.NgayThucHien.Value.Date >= tuNgay.Date)
                                         && (string.IsNullOrEmpty(timKiemNangCaoObj.ThucHienTuNgayDenNgay.DenNgay) || p.NgayThucHien.Value.Date <= denNgay.Date));
            }

            if (timKiemNangCaoObj.TrangThai != null && (timKiemNangCaoObj.TrangThai.ChoKetQua || timKiemNangCaoObj.TrangThai.DaCoKetQua))
            {
                var danhSachTimKiemKQ = query.Where(x => (timKiemNangCaoObj.TrangThai.ChoKetQua && x.TrangThai == false)
                                      || (timKiemNangCaoObj.TrangThai.DaCoKetQua && x.TrangThai));
                query = danhSachTimKiemKQ;
            }

            query = query.GroupBy(o => new { o.Id, o.MaYeuCauTiepNhan, o.SoBenhAn, o.MaBN, o.HoTen, o.GioiTinh, o.DoiTuong })
                 .Select(item => new KetQuaCDHATDCNTimKiemGridVo()
                 {
                     Id = item.First().Id,
                     MaYeuCauTiepNhan = item.First().MaYeuCauTiepNhan,
                     SoBenhAn = item.First().SoBenhAn,
                     MaBN = item.First().MaBN,
                     HoTen = item.First().HoTen,
                     GioiTinh = item.First().GioiTinh,
                     DoiTuong = item.First().DoiTuong,
                     TrangThai = item.All(o => o.TrangThai),
                     NgayChiDinhDisplay = String.Join("; ", item.Select(o => o.NgayChiDinh.ApplyFormatDateTime()).ToArray()),
                     NoiChiDinh = String.Join("; ", item.Select(o => o.NoiChiDinh).ToArray()),
                     ChiDinh = "(" + item.Where(o => o.TrangThai).Count() + "/" + item.Count() + ") " + String.Join("; ", item.Select(o => o.ChiDinh).ToArray()),
                     NgayThucHien = item.First().NgayThucHien,
                     NgayThucHienDisplay = String.Join("; ", item.Where(o => o.NgayThucHien != null).OrderByDescending(o => o.NgayThucHien).Select(o => (o.NgayThucHien != null ? ((DateTime)o.NgayThucHien).ApplyFormatDateTime() : "")).ToArray())
                 });



            var totalRowCount = query.Count();
            return new GridDataSource { TotalRowCount = totalRowCount };
        }

        #endregion        

        #region Danh sách kết quả mẫu

        public async Task<GridDataSource> GetDataForGridNoiDungMauAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var query = _ketQuaVaKetLuanMauRepository.TableNoTracking
                //.Where(x => (isKetQuaMau && x.LoaiKetQuaVaKetLuanMau == Enums.LoaiKetQuaVaKetLuanMau.KetQuaMau) || (!isKetQuaMau && x.LoaiKetQuaVaKetLuanMau == Enums.LoaiKetQuaVaKetLuanMau.KetLuanMau))
                .Select(s => new NoiDungMauGridVo()
                {
                    Id = s.Id,
                    Ma = s.Ma,
                    Ten = s.Ten
                }).ApplyLike(queryInfo.SearchTerms, g => g.Ma, g => g.Ten);

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridNoiDungMauAsync(QueryInfo queryInfo)
        {
            var query = _ketQuaVaKetLuanMauRepository.TableNoTracking
                //.Where(x => (isKetQuaMau && x.LoaiKetQuaVaKetLuanMau == Enums.LoaiKetQuaVaKetLuanMau.KetQuaMau) || (!isKetQuaMau && x.LoaiKetQuaVaKetLuanMau == Enums.LoaiKetQuaVaKetLuanMau.KetLuanMau))
                .Select(s => new NoiDungMauGridVo()
                {
                    Id = s.Id,
                    Ma = s.Ma,
                    Ten = s.Ten
                }).ApplyLike(queryInfo.SearchTerms, g => g.Ma, g => g.Ten);

            var totalRowCount = query.Count();

            return new GridDataSource { TotalRowCount = totalRowCount };
        }


        #endregion

        #region Danh Sách Đã Có Kết Quả Cận Lâm Sàng

        public DanhSachCanLamSangVo GetThongTinCanLamSang(long id)
        {

            var nhanVienThucHienId = _userAgentHelper.GetCurrentUserId();
            var noiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var nhanVienThucHien = _useRepository.TableNoTracking.ToDictionary(cc => cc.Id, cc => cc.HoTen);

            var danhSachCanLamSangVo = new DanhSachCanLamSangVo();
            var DanhSachCanLamSangs = new List<DanhSachCanLamSang>();
            var ketQuaNhomXetNghiems = new List<KetQuaNhomXetNghiemVo>();

            var queryYeuCauDichVuKyThuats = BaseRepository.TableNoTracking.Where(cc => cc.Id == id &&
                                cc.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien &&
                                (cc.YeuCauDichVuKyThuats.Any(o => o.NoiChiDinhId == noiThucHienId &&
                                     ((o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan || o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan || o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan) && o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                                     (o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien || o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien) &&
                                      o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh || o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem ||
                                      o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang)))).SelectMany(c => c.YeuCauDichVuKyThuats).Include(cc => cc.FileKetQuaCanLamSangs)
                                                                                                                                               .Include(cc => cc.NhomDichVuBenhVien);


            //Lấy thông tin thăm dò chức năng và chuẩn đoán hình ảnh
            var ketQuaTDCNvaChuanDoanHACanLamSangs = queryYeuCauDichVuKyThuats.Where(o => o.NoiChiDinhId == noiThucHienId && o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang ||
                                                                                      o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh)
                                                                                      .Include(cc => cc.NhanVienThucHien).ThenInclude(cc => cc.User);

            foreach (var queryYeuCauDichVuKyThuat in ketQuaTDCNvaChuanDoanHACanLamSangs)
            {
                if (queryYeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan || queryYeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                    queryYeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan)
                {
                    var danhSachCanLamSang = new DanhSachCanLamSang();
                    danhSachCanLamSang.DichVuKyhuatId = queryYeuCauDichVuKyThuat.Id;
                    danhSachCanLamSang.GhiChu = queryYeuCauDichVuKyThuat.KetLuan;
                    danhSachCanLamSang.TenDichVu = queryYeuCauDichVuKyThuat.TenDichVu;
                    danhSachCanLamSang.LoaiYeuCauKyThuat = (int)queryYeuCauDichVuKyThuat.LoaiDichVuKyThuat;

                    danhSachCanLamSang.NhanVienThucHienId = queryYeuCauDichVuKyThuat.NhanVienThucHienId ?? nhanVienThucHienId;
                    danhSachCanLamSang.TenNhanVienThucHien = queryYeuCauDichVuKyThuat.NhanVienThucHien == null ? nhanVienThucHien[nhanVienThucHienId] : queryYeuCauDichVuKyThuat.NhanVienThucHien.User.HoTen;

                    //todo need update FileKetQuaCanLamSangs
                    if (queryYeuCauDichVuKyThuat.FileKetQuaCanLamSangs.Any())
                    {
                        foreach (var fileKetQuaCanLamSang in queryYeuCauDichVuKyThuat.FileKetQuaCanLamSangs)
                        {
                            var giayKetQuaLamSang = new GiayKetQuaLamSang();
                            giayKetQuaLamSang.Ten = fileKetQuaCanLamSang.Ten;
                            giayKetQuaLamSang.Id = fileKetQuaCanLamSang.Id;
                            giayKetQuaLamSang.Ma = fileKetQuaCanLamSang.Ma;
                            giayKetQuaLamSang.DuongDan = fileKetQuaCanLamSang.DuongDan;
                            giayKetQuaLamSang.TenGuid = fileKetQuaCanLamSang.TenGuid;
                            giayKetQuaLamSang.KichThuoc = fileKetQuaCanLamSang.KichThuoc;
                            giayKetQuaLamSang.LoaiTapTin = (int)fileKetQuaCanLamSang.LoaiTapTin;

                            danhSachCanLamSang.GiayKetQuaLamSang.Add(giayKetQuaLamSang);
                        }

                    }

                    DanhSachCanLamSangs.Add(danhSachCanLamSang);
                }

            }


            //Lấy thông tin nhóm xét nghiệm
            var ketQuaXetNgiemCanLamSangs = queryYeuCauDichVuKyThuats.Where(o => o.NoiChiDinhId == noiThucHienId && o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem && ((o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan || o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan || o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan) && o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && (o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien || o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)))
                                                          .Include(cc => cc.NhanVienThucHien).ThenInclude(cc => cc.User)
                                                          .GroupBy(i => i.NhomDichVuBenhVienId);

            var ketQuaXetNghiem = _ketQuaNhomXetNghiemRepository.TableNoTracking.Where(cc => cc.YeuCauTiepNhanId == id).Include(cc => cc.FileKetQuaCanLamSangs);
            var nhomDichVuBenhViens = _nhomDichVuBenhVienRepository.TableNoTracking.ToDictionary(c => c.Id, c => c.Ten);


            if (ketQuaXetNgiemCanLamSangs.Any())
            {
                foreach (var ketQuaXetNgiemCanLamSang in ketQuaXetNgiemCanLamSangs)
                {
                    var KetQuaNhomXetNghiem = new KetQuaNhomXetNghiemVo();
                    var NhomDanhSachXetNghiems = new List<NhomDanhSachXetNghiem>();

                    KetQuaNhomXetNghiem.NhomDichVuKyThuatId = ketQuaXetNgiemCanLamSang.Key;
                    KetQuaNhomXetNghiem.TenNhomDichVuKyhuat = nhomDichVuBenhViens[ketQuaXetNgiemCanLamSang.Key];
                    KetQuaNhomXetNghiem.CapNhatChuaThanhToan = ketQuaXetNgiemCanLamSang.Any(cc => cc.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien) &&
                                                               ketQuaXetNgiemCanLamSang.Any(cc => cc.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien);


                    //todo need update FileKetQuaCanLamSangs
                    var fileKetQuas = ketQuaXetNghiem
                        .Where(o => o.NhomDichVuBenhVienId == ketQuaXetNgiemCanLamSang.Key)
                        .SelectMany(o => o.FileKetQuaCanLamSangs).ToList();

                    if (fileKetQuas != null)
                    {
                        foreach (var fileKetQua in fileKetQuas)
                        {
                            var giayKetQuaLamSang = new GiayKetQuaLamSang
                            {
                                Ten = fileKetQua.Ten,
                                Id = fileKetQua.Id,
                                Ma = fileKetQua.Ma,
                                DuongDan = fileKetQua.DuongDan,
                                TenGuid = fileKetQua.TenGuid,
                                KichThuoc = fileKetQua.KichThuoc,
                                LoaiTapTin = (int)fileKetQua.LoaiTapTin
                            };
                            KetQuaNhomXetNghiem.GiayKetQuaNhomCanLamSang.Add(giayKetQuaLamSang);
                        }
                    }

                    foreach (var queryYeuCauDichVuKyThuat in ketQuaXetNgiemCanLamSang)
                    {
                        var groupXetNghiems = new NhomDanhSachXetNghiem();
                        groupXetNghiems.DichVuId = queryYeuCauDichVuKyThuat.Id;
                        groupXetNghiems.TenDichVu = queryYeuCauDichVuKyThuat.TenDichVu;
                        groupXetNghiems.CapNhatChuaThanhToan = queryYeuCauDichVuKyThuat.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien;
                        groupXetNghiems.NhanVienThucHienId = queryYeuCauDichVuKyThuat.NhanVienThucHienId ?? nhanVienThucHienId;
                        groupXetNghiems.TenNhanVienThucHien = queryYeuCauDichVuKyThuat.NhanVienThucHien == null ? nhanVienThucHien[nhanVienThucHienId] : queryYeuCauDichVuKyThuat.NhanVienThucHien.User.HoTen;
                        KetQuaNhomXetNghiem.NhanVienThucHienId = queryYeuCauDichVuKyThuat.NhanVienThucHienId ?? nhanVienThucHienId;
                        KetQuaNhomXetNghiem.TenNhanVienThucHien = queryYeuCauDichVuKyThuat.NhanVienThucHien == null ? nhanVienThucHien[nhanVienThucHienId] : queryYeuCauDichVuKyThuat.NhanVienThucHien.User.HoTen;
                        KetQuaNhomXetNghiem.KetLuan = ketQuaXetNghiem.Any(cc => cc.NhomDichVuBenhVienId == ketQuaXetNgiemCanLamSang.Key) ? ketQuaXetNghiem.Where(cc => cc.NhomDichVuBenhVienId == ketQuaXetNgiemCanLamSang.Key).First().KetLuan : queryYeuCauDichVuKyThuat.KetLuan;

                        NhomDanhSachXetNghiems.Add(groupXetNghiems);
                    }
                    KetQuaNhomXetNghiem.NhomDanhSachXetNghiem = NhomDanhSachXetNghiems;
                    ketQuaNhomXetNghiems.Add(KetQuaNhomXetNghiem);
                }
            }

            danhSachCanLamSangVo.DanhSachCanLamSangs.AddRange(DanhSachCanLamSangs);
            danhSachCanLamSangVo.KetQuaNhomXetNghiems.AddRange(ketQuaNhomXetNghiems);

            return danhSachCanLamSangVo;
        }

        public DanhSachCanLamSangVo GetThongTinKLichSuCanLamSang(long id)
        {
            var nhanVienThucHienId = _userAgentHelper.GetCurrentUserId();
            var noiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var nhanVienThucHien = _useRepository.TableNoTracking.ToDictionary(cc => cc.Id, cc => cc.HoTen);

            var danhSachCanLamSangVo = new DanhSachCanLamSangVo();
            var DanhSachCanLamSangs = new List<DanhSachCanLamSang>();
            var ketQuaNhomXetNghiems = new List<KetQuaNhomXetNghiemVo>();

            var queryYeuCauDichVuKyThuats = BaseRepository.TableNoTracking.Where(cc => cc.Id == id &&
                                cc.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien &&
                                (cc.YeuCauDichVuKyThuats.Any(o => o.NoiChiDinhId == noiThucHienId &&
                                     ((o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan || o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan || o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan) && o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                                     (o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien || o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien) &&
                                      o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh || o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem ||
                                      o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang)))).SelectMany(c => c.YeuCauDichVuKyThuats).Include(cc => cc.FileKetQuaCanLamSangs)
                                                                                                                                               .Include(cc => cc.NhomDichVuBenhVien);


            //Lấy thông tin thăm dò chức năng và chuẩn đoán hình ảnh
            var ketQuaTDCNvaChuanDoanHACanLamSangs = queryYeuCauDichVuKyThuats.Where(o => o.NoiChiDinhId == noiThucHienId && o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang ||
                                                                                      o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh)
                                                                                      .Include(cc => cc.NhanVienThucHien).ThenInclude(cc => cc.User);

            foreach (var queryYeuCauDichVuKyThuat in ketQuaTDCNvaChuanDoanHACanLamSangs)
            {
                if (queryYeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan || queryYeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                    queryYeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan)
                {
                    var danhSachCanLamSang = new DanhSachCanLamSang();
                    danhSachCanLamSang.DichVuKyhuatId = queryYeuCauDichVuKyThuat.Id;
                    danhSachCanLamSang.GhiChu = queryYeuCauDichVuKyThuat.KetLuan;
                    danhSachCanLamSang.TenDichVu = queryYeuCauDichVuKyThuat.TenDichVu;
                    danhSachCanLamSang.LoaiYeuCauKyThuat = (int)queryYeuCauDichVuKyThuat.LoaiDichVuKyThuat;

                    danhSachCanLamSang.NhanVienThucHienId = queryYeuCauDichVuKyThuat.NhanVienThucHienId ?? nhanVienThucHienId;
                    danhSachCanLamSang.TenNhanVienThucHien = queryYeuCauDichVuKyThuat.NhanVienThucHien == null ? nhanVienThucHien[nhanVienThucHienId] : queryYeuCauDichVuKyThuat.NhanVienThucHien.User.HoTen;

                    //todo need update FileKetQuaCanLamSangs
                    if (queryYeuCauDichVuKyThuat.FileKetQuaCanLamSangs.Any())
                    {
                        foreach (var fileKetQuaCanLamSang in queryYeuCauDichVuKyThuat.FileKetQuaCanLamSangs)
                        {
                            var giayKetQuaLamSang = new GiayKetQuaLamSang();
                            giayKetQuaLamSang.Ten = fileKetQuaCanLamSang.Ten;
                            giayKetQuaLamSang.Id = fileKetQuaCanLamSang.Id;
                            giayKetQuaLamSang.Ma = fileKetQuaCanLamSang.Ma;
                            giayKetQuaLamSang.DuongDan = fileKetQuaCanLamSang.DuongDan;
                            giayKetQuaLamSang.TenGuid = fileKetQuaCanLamSang.TenGuid;
                            giayKetQuaLamSang.KichThuoc = fileKetQuaCanLamSang.KichThuoc;
                            giayKetQuaLamSang.LoaiTapTin = (int)fileKetQuaCanLamSang.LoaiTapTin;

                            danhSachCanLamSang.GiayKetQuaLamSang.Add(giayKetQuaLamSang);
                        }

                    }

                    DanhSachCanLamSangs.Add(danhSachCanLamSang);
                }

            }


            //Lấy thông tin nhóm xét nghiệm
            var ketQuaXetNgiemCanLamSangs = queryYeuCauDichVuKyThuats.Where(o => o.NoiChiDinhId == noiThucHienId && o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem && ((o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan || o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan || o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan) && o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && (o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien || o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)))
                                                          .Include(cc => cc.NhanVienThucHien).ThenInclude(cc => cc.User)
                                                          .GroupBy(i => i.NhomDichVuBenhVienId);

            var ketQuaXetNghiem = _ketQuaNhomXetNghiemRepository.TableNoTracking.Where(cc => cc.YeuCauTiepNhanId == id).Include(cc => cc.FileKetQuaCanLamSangs);
            var nhomDichVuBenhViens = _nhomDichVuBenhVienRepository.TableNoTracking.ToDictionary(c => c.Id, c => c.Ten);


            if (ketQuaXetNgiemCanLamSangs.Any())
            {
                foreach (var ketQuaXetNgiemCanLamSang in ketQuaXetNgiemCanLamSangs)
                {
                    var KetQuaNhomXetNghiem = new KetQuaNhomXetNghiemVo();
                    var NhomDanhSachXetNghiems = new List<NhomDanhSachXetNghiem>();

                    KetQuaNhomXetNghiem.NhomDichVuKyThuatId = ketQuaXetNgiemCanLamSang.Key;
                    KetQuaNhomXetNghiem.TenNhomDichVuKyhuat = nhomDichVuBenhViens[ketQuaXetNgiemCanLamSang.Key];
                    KetQuaNhomXetNghiem.CapNhatChuaThanhToan = ketQuaXetNgiemCanLamSang.Any(cc => cc.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien) &&
                                                               ketQuaXetNgiemCanLamSang.Any(cc => cc.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien);


                    //todo need update FileKetQuaCanLamSangs
                    var fileKetQuas = ketQuaXetNghiem
                        .Where(o => o.NhomDichVuBenhVienId == ketQuaXetNgiemCanLamSang.Key)
                        .SelectMany(o => o.FileKetQuaCanLamSangs).ToList();

                    if (fileKetQuas != null)
                    {
                        foreach (var fileKetQua in fileKetQuas)
                        {
                            var giayKetQuaLamSang = new GiayKetQuaLamSang
                            {
                                Ten = fileKetQua.Ten,
                                Id = fileKetQua.Id,
                                Ma = fileKetQua.Ma,
                                DuongDan = fileKetQua.DuongDan,
                                TenGuid = fileKetQua.TenGuid,
                                KichThuoc = fileKetQua.KichThuoc,
                                LoaiTapTin = (int)fileKetQua.LoaiTapTin
                            };
                            KetQuaNhomXetNghiem.GiayKetQuaNhomCanLamSang.Add(giayKetQuaLamSang);
                        }
                    }

                    foreach (var queryYeuCauDichVuKyThuat in ketQuaXetNgiemCanLamSang)
                    {
                        var groupXetNghiems = new NhomDanhSachXetNghiem();
                        groupXetNghiems.DichVuId = queryYeuCauDichVuKyThuat.Id;
                        groupXetNghiems.TenDichVu = queryYeuCauDichVuKyThuat.TenDichVu;
                        groupXetNghiems.CapNhatChuaThanhToan = queryYeuCauDichVuKyThuat.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien;
                        groupXetNghiems.NhanVienThucHienId = queryYeuCauDichVuKyThuat.NhanVienThucHienId ?? nhanVienThucHienId;
                        groupXetNghiems.TenNhanVienThucHien = queryYeuCauDichVuKyThuat.NhanVienThucHien == null ? nhanVienThucHien[nhanVienThucHienId] : queryYeuCauDichVuKyThuat.NhanVienThucHien.User.HoTen;
                        KetQuaNhomXetNghiem.NhanVienThucHienId = queryYeuCauDichVuKyThuat.NhanVienThucHienId ?? nhanVienThucHienId;
                        KetQuaNhomXetNghiem.TenNhanVienThucHien = queryYeuCauDichVuKyThuat.NhanVienThucHien == null ? nhanVienThucHien[nhanVienThucHienId] : queryYeuCauDichVuKyThuat.NhanVienThucHien.User.HoTen;
                        KetQuaNhomXetNghiem.KetLuan = ketQuaXetNghiem.Any(cc => cc.NhomDichVuBenhVienId == ketQuaXetNgiemCanLamSang.Key) ? ketQuaXetNghiem.Where(cc => cc.NhomDichVuBenhVienId == ketQuaXetNgiemCanLamSang.Key).First().KetLuan : queryYeuCauDichVuKyThuat.KetLuan;

                        NhomDanhSachXetNghiems.Add(groupXetNghiems);
                    }
                    KetQuaNhomXetNghiem.NhomDanhSachXetNghiem = NhomDanhSachXetNghiems;
                    ketQuaNhomXetNghiems.Add(KetQuaNhomXetNghiem);
                }
            }

            danhSachCanLamSangVo.DanhSachCanLamSangs.AddRange(DanhSachCanLamSangs);
            danhSachCanLamSangVo.KetQuaNhomXetNghiems.AddRange(ketQuaNhomXetNghiems);

            return danhSachCanLamSangVo;
        }

        #endregion

        #region Danh Sách Đã Có Kết Quả Cận Lâm Sàng

        public async Task<GridDataSource> GetLichSuCanLamSangDaCoKetQuaDataForGridAsync(QueryInfo queryInfo)
        {
            var noiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var timKiemNangCaoObj = new LichSuKetQuaCDHATDCNTimKiemVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<LichSuKetQuaCDHATDCNTimKiemVo>(queryInfo.AdditionalSearchString);
            }

            BuildDefaultSortExpression(queryInfo);
            var query = BaseRepository.TableNoTracking
                .Where(x => x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat
                            && x.YeuCauDichVuKyThuats.Any(a => a.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                                                               && (a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh || a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang)
                                                               && a.NoiThucHienId == noiThucHienId))
                .Select(item => new KetQuaCDHATDCNLichSuGridVo()
                {
                    Id = item.Id,
                    MaYeuCauTiepNhan = item.MaYeuCauTiepNhan,
                    SoBenhAn = item.NoiTruBenhAn != null ? item.NoiTruBenhAn.SoBenhAn : null,
                    HoTen = item.HoTen,
                    MaBN = item.BenhNhan.MaBN,
                    GioiTinh = item.GioiTinh.GetDescription(),
                    DiaChi = item.DiaChiDayDu,
                    SoDienThoai = item.SoDienThoaiDisplay
                }).ApplyLike(timKiemNangCaoObj.SearchString, x => x.MaYeuCauTiepNhan, x => x.MaBN, x => x.HoTen, x => x.SoDienThoai, x => x.DiaChi);

            // kiểm tra tìm kiếm nâng cao
            //if (timKiemNangCaoObj.TuNgayDenNgay != null && (!string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay) || !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay)))
            //{
            //    DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
            //    DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);

            //    query = query.Where(p => (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay) || p.NgayChiDinh.Date >= tuNgay.Date)
            //                             && (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay) || p.NgayChiDinh.Date <= denNgay.Date));
            //}

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetLichSuCanLamSangCanLamSangDaCoKetQuaTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var noiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var timKiemNangCaoObj = new LichSuKetQuaCDHATDCNTimKiemVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<LichSuKetQuaCDHATDCNTimKiemVo>(queryInfo.AdditionalSearchString);
            }

            BuildDefaultSortExpression(queryInfo);
            var query = BaseRepository.TableNoTracking
                .Where(x => x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat
                            && x.YeuCauDichVuKyThuats.Any(a => a.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                                                               && (a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh || a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang)
                                                               && a.NoiThucHienId == noiThucHienId))
                .Select(item => new KetQuaCDHATDCNLichSuGridVo()
                {
                    Id = item.Id,
                    MaYeuCauTiepNhan = item.MaYeuCauTiepNhan,
                    SoBenhAn = item.NoiTruBenhAn != null ? item.NoiTruBenhAn.SoBenhAn : null,
                    HoTen = item.HoTen,
                    MaBN = item.BenhNhan.MaBN,
                    GioiTinh = item.GioiTinh.GetDescription(),
                    DiaChi = item.DiaChiDayDu,
                    SoDienThoai = item.SoDienThoaiDisplay
                }).ApplyLike(timKiemNangCaoObj.SearchString, x => x.MaYeuCauTiepNhan, x => x.MaBN, x => x.HoTen, x => x.SoDienThoai, x => x.DiaChi);

            //IQueryable<DanhSachCanLamSangGridVo> query;
            //BuildDefaultSortExpression(queryInfo);
            ////todo: need improve
            //var queryTheoDanhSach = BaseRepository.TableNoTracking.Where(cc =>
            //                         (cc.YeuCauDichVuKyThuats.Any(o => o.NoiChiDinhId == noiThucHienId &&
            //                         ((o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan || o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan || o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan) && o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && (o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien) &&
            //                         (o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem || o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh || o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang)))));

            ////todo: need improve
            //query = queryTheoDanhSach.Select(s => new DanhSachCanLamSangGridVo
            //{
            //    Id = s.Id,
            //    MaTN = s.MaYeuCauTiepNhan,
            //    HoTen = s.HoTen,
            //    DiaChi = s.DiaChiDayDu,
            //    DienThoaiStr = s.SoDienThoai.ApplyFormatPhone(),
            //    GioiTinhStr = s.GioiTinh.GetDescription(),
            //    MaBN = s.BenhNhan != null ? s.BenhNhan.MaBN : "",
            //    NamSinh = s.NamSinh,
            //    NgayCoKetQuaStr = s.CreatedOn.Value.ApplyFormatDateTime(),
            //    NgayCoKetQuaSACHStr = s.CreatedOn.Value.ApplyFormatDateTimeSACH(),
            //    NgayCoKetQua = s.CreatedOn.Value,
            //    Ngay = s.CreatedOn.Value,
            //});

            //if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            //{
            //    var queryString = JsonConvert.DeserializeObject<DanhSachCanLamSangGridVo>(queryInfo.AdditionalSearchString);

            //    if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
            //    {
            //        DateTime denNgay;
            //        DateTime.TryParseExact(queryString.FromDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None,
            //            out var tuNgay);
            //        if (string.IsNullOrEmpty(queryString.ToDate))
            //        {
            //            denNgay = DateTime.Now;
            //        }
            //        else
            //        {
            //            DateTime.TryParseExact(queryString.ToDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out denNgay);
            //        }

            //        query = query.Where(p => p.Ngay >= tuNgay && p.Ngay <= denNgay.AddSeconds(59).AddMilliseconds(999));
            //    }
            //    if (!string.IsNullOrEmpty(queryString.TimKiem))
            //    {
            //        var searchTerms = queryString.TimKiem.Replace("\t", "").Trim();
            //        query = query.ApplyLike(searchTerms,
            //              g => g.MaBN.ToString(),
            //              g => g.HoTen,
            //              g => g.DiaChi,
            //              g => g.NamSinh.ToString(),
            //              g => g.DienThoaiStr,
            //              g => g.MaTN);
            //    }

            //}
            //if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            //{
            //    query = query.ApplyLike(queryInfo.SearchTerms,
            //       g => g.MaBN.ToString(),
            //       g => g.HoTen,
            //       g => g.HoTenRemoveDiacritics,
            //       g => g.DiaChi,
            //       g => g.DiaChiRemoveDiacritics,
            //       g => g.NamSinh.ToString(),
            //       g => g.DienThoaiStr,
            //       g => g.MaTN);
            //}

            var totalRowCount = query.Count();
            return new GridDataSource { TotalRowCount = totalRowCount };
        }

        public async Task<GridDataSource> GetDataChiTietLichSuCanLamSangForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var yeuCauTiepNhanId = long.Parse(queryInfo.AdditionalSearchString);
            var query = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId
                            && x.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                            && (x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang))
                .Select(item => new ChiTietKetQuaCDHATDCNLichSuGridVo()
                {
                    Id = item.Id,
                    DichVu = item.TenDichVu,
                    NguoiChiDinh = item.NhanVienChiDinh.User.HoTen,
                    NoiChiDinh = item.NoiChiDinh.Ten,
                    NgayChiDinh = item.ThoiDiemChiDinh,
                    NguoiThucHien = item.NhanVienThucHien.User.HoTen,
                    NgayThucHien = item.ThoiDiemThucHien,
                    BacSiKetLuan = item.NhanVienKetLuan.User.HoTen,
                    MayTraKetQua = item.MayTraKetQua.Ten,
                    FileChuKy = item.FileKetQuaCanLamSangs.Any() ? item.FileKetQuaCanLamSangs.Select(x => x.Ten).FirstOrDefault() : null,
                    TenGuid = item.FileKetQuaCanLamSangs.Any() ? item.FileKetQuaCanLamSangs.Select(x => x.TenGuid).FirstOrDefault() : null,
                    DuongDan = item.FileKetQuaCanLamSangs.Any() ? item.FileKetQuaCanLamSangs.Select(x => x.DuongDan).FirstOrDefault() : null
                });

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalChiTietLichSuCanLamSangPageForGridAsync(QueryInfo queryInfo)
        {
            var yeuCauTiepNhanId = long.Parse(queryInfo.AdditionalSearchString);
            var query = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId
                            && x.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                            && (x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang))
                .Select(item => new ChiTietKetQuaCDHATDCNLichSuGridVo()
                {
                    Id = item.Id,
                    DichVu = item.TenDichVu,
                    NguoiChiDinh = item.NhanVienChiDinh.User.HoTen,
                    NoiChiDinh = item.NoiChiDinh.Ten,
                    NgayChiDinh = item.ThoiDiemChiDinh,
                    NguoiThucHien = item.NhanVienThucHien.User.HoTen,
                    NgayThucHien = item.ThoiDiemThucHien,
                    BacSiKetLuan = item.NhanVienKetLuan.User.HoTen,
                    MayTraKetQua = item.MayTraKetQua.Ten,
                    FileChuKy = item.FileKetQuaCanLamSangs.Any() ? item.FileKetQuaCanLamSangs.Select(x => x.TenGuid).FirstOrDefault() : null,
                    DuongDan = item.FileKetQuaCanLamSangs.Any() ? item.FileKetQuaCanLamSangs.Select(x => x.DuongDan).FirstOrDefault() : null,
                });

            var totalRowCount = query.Count();
            return new GridDataSource { TotalRowCount = totalRowCount };
        }


        public void DeleteFileKetQuaCanLamSang(List<FileKetQuaCanLamSang> fileKetQuaCanLamSangs)
        {
            foreach (var fileKetQuaCanLamSang in fileKetQuaCanLamSangs)
            {
                _fileKetQuaCanLamSangRepository.Delete(fileKetQuaCanLamSang);
            }
        }

        #endregion

        public KetQuaCLS GetCanLamSangIdByMaBNVaMaTT(TimKiemThongTinBenhNhan TimKiemThongTinBenhNhan)
        {
            KetQuaCLS ketQuaCLS = new KetQuaCLS();
            var queryTheoDanhSach = BaseRepository.TableNoTracking.Include(cc => cc.YeuCauDichVuKyThuats).Where(cc =>
                                      cc.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien &&
                                      (cc.YeuCauDichVuKyThuats.Any(o =>
                                           ((o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan || o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan || o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan) && o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && (o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien || o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien) &&
                                            (o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem || o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh || o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang)))))
                                         .Include(cc => cc.YeuCauDichVuKyThuats).Include(cc => cc.BenhNhan);


            ketQuaCLS = queryTheoDanhSach.Where(cc => cc.MaYeuCauTiepNhan.Contains(TimKiemThongTinBenhNhan.TimKiemMaBNVaMaTN) || cc.BenhNhan.MaBN.ToString().Contains(TimKiemThongTinBenhNhan.TimKiemMaBNVaMaTN))
               .Select(s => new KetQuaCLS
               {

                   Id = s.Id,
                   Type = s.YeuCauDichVuKyThuats.Any(o => (o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan || o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan || o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan) && (o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem || o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh || o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang) && o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien)
               }).FirstOrDefault();
            return ketQuaCLS;
        }

        #region lookup


        public async Task<List<YeuCauKyThuatCDHALookupItemVo>> GetListKyThuatDichVuKyThuatTheoTiepNhan(DropDownListRequestModel queryInfo, long yeuCauTiepNhanId)
        {
            var noiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            if (!string.IsNullOrEmpty(queryInfo.Query) && !queryInfo.Query.Contains(" ") || string.IsNullOrEmpty(queryInfo.Query))
            {
                var lstValues = _yeuCauDichVuKyThuatRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId && p.NoiThucHienId == noiThucHienId
                      && (p.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang || p.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh)
                       && p.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                       && (p.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan || p.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan || p.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan))
                         .ApplyLike(queryInfo.Query, o => o.TenDichVu)
                                                              .Select(item => new YeuCauKyThuatCDHALookupItemVo
                                                              {
                                                                  DisplayName = item.TenDichVu,
                                                                  KeyId = item.Id,
                                                                  TrangThaiDangThucHien = item.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien,
                                                              }).ToList();

                return lstValues;
            }
            else
            {
                var lstValues = _yeuCauDichVuKyThuatRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId && p.NoiThucHienId == noiThucHienId && (p.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang || p.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh)
                                                && p.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                && (p.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan || p.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan || p.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan))
                                                              .Select(item => new YeuCauKyThuatCDHALookupItemVo
                                                              {
                                                                  DisplayName = item.TenDichVu,
                                                                  KeyId = item.Id,
                                                                  TrangThaiDangThucHien = item.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien,
                                                              }).ToList();

                return lstValues;
            }
        }


        public TrangThaiYeuCauKyThuat TrangThaiYeuCauDichVuKyThuat(long yeuCauDichVuKyThuatId)
        {
            #region Cập nhật 28/12/2022
            //var data = _yeuCauDichVuKyThuatRepository.TableNoTracking.Where(p => p.Id == yeuCauDichVuKyThuatId).Include(c => c.YeuCauTiepNhan).FirstOrDefault();
            var data = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(p => p.Id == yeuCauDichVuKyThuatId)
                .Select(x => new
                {
                    x.DataKetQuaCanLamSang,
                    x.TrangThai,
                    x.NhomDichVuBenhVienId,
                    x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan
                })
                .FirstOrDefault();
            #endregion

            var currentUser = _userAgentHelper.GetCurrentUserId();

            #region Cập nhật 28/12/2022
            //var tenNguoiSuaSauCung = _useRepository.TableNoTracking.ToDictionary(c => c.Id, c => c.HoTen);
            #endregion

            var dataKetQuaCanLamSang = !string.IsNullOrEmpty(data.DataKetQuaCanLamSang) ? JsonConvert.DeserializeObject<ChiTietKetQuaCDHATDCNVo>(data.DataKetQuaCanLamSang) : null;

            var lstValues = new TrangThaiYeuCauKyThuat
            {
                #region Cập nhật 28/12/2022
                //TrangThaiYeuCauTiepNhan = data.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat,
                TrangThaiYeuCauTiepNhan = data.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat,
                #endregion

                TrangThaiDaThucHien = data.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien,
                NhomDichVuBenhVienId = data.NhomDichVuBenhVienId,

                NguoiThucHien = dataKetQuaCanLamSang != null ? dataKetQuaCanLamSang.NguoiLuuId == currentUser : false,
                TenNguoiSuaSauCung = dataKetQuaCanLamSang != null ? dataKetQuaCanLamSang.NguoiLuuTen : string.Empty,
                ThoiGianSuaSauCung = dataKetQuaCanLamSang != null ? dataKetQuaCanLamSang.ThoiDiemLuu?.ApplyFormatDateTimeSACH() : string.Empty,
            };

            return lstValues;
        }

        public async Task<List<string>> GetListKyThuatDichVuKyThuatAsync(DropDownListRequestModel queryInfo)
        {
            var lstColumnNameSearch = new List<string>
            {
                nameof(Core.Domain.Entities.InputStringStoreds.InputStringStored.Value)
            };
            if (!string.IsNullOrEmpty(queryInfo.Query) && !queryInfo.Query.Contains(" ") || string.IsNullOrEmpty(queryInfo.Query))
            {
                var lstValues = _inputStringStoredRepository.TableNoTracking
                    .Where(p => p.Set == Enums.InputStringStoredKey.KyThuat)
                    .Select(p => p.Value)
                    .ApplyLike(queryInfo.Query, o => o)
                    .Take(queryInfo.Take);

                return lstValues.ToList();
            }
            else
            {
                var lstIds = _inputStringStoredRepository
                                .ApplyFulltext(queryInfo.Query, nameof(Core.Domain.Entities.InputStringStoreds.InputStringStored), lstColumnNameSearch)
                                .Select(p => p.Id).ToList();

                var dictionary = lstIds.Select((id, index) => new
                {
                    keys = id,
                    rank = index,
                }).ToDictionary(o => o.keys, o => o.rank);

                var lstValues = _inputStringStoredRepository
                                        .TableNoTracking
                                        .Where(p => p.Set == Enums.InputStringStoredKey.KyThuat)
                                        .Take(queryInfo.Take)
                                        .Select(item => new InputStringStoredTemplateVo
                                        {
                                            Rank = dictionary.Any(a => a.Key == item.Id) ? dictionary[item.Id] : dictionary.Count,
                                            DisplayName = item.Value,
                                            KeyId = item.Id,
                                        }).ToList();
                var listValueStrings = lstValues.Select(p => p.DisplayName).ToList();
                return listValueStrings;
            }
        }

        #endregion

        #region thêm/xóa/sửa kết quả

        public void KiemTraLuuNoiDungKetQuaAsync(YeuCauDichVuKyThuat yeuCauDichVuKyThuat, string kyThuat)
        {
            yeuCauDichVuKyThuat.TrangThai = Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien;
            //yeuCauDichVuKyThuat.ThoiDiemThucHien = DateTime.Now;
            _yeuCauDichVuKyThuatRepository.Context.SaveChanges();

            if (!string.IsNullOrEmpty(kyThuat))
            {
                var isExists = _inputStringStoredRepository
                    .TableNoTracking
                    .Any(p => p.Value.Trim().ToLower() == kyThuat.Trim().ToLower()
                                   && p.Set == Enums.InputStringStoredKey.KyThuat);
                if (!isExists)
                {
                    var inputStringStored = new Core.Domain.Entities.InputStringStoreds.InputStringStored
                    {
                        Set = Enums.InputStringStoredKey.KyThuat,
                        Value = kyThuat
                    };
                    _inputStringStoredRepository.Add(inputStringStored);
                }
            }
        }
        #endregion

        #region In phiếu

        public string XuLyInPhieuKetQuaAsync(PhieuInKetQuaInfoVo phieuInKetQuaInfoVo)
        {
            var content = string.Empty;
            var hearder = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
                      "<th>PHIẾU KẾT QUẢ</th>" +
                      "</p>";

            var template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuKetQuaCDHATDCN"));
            var data = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(x => x.Id == phieuInKetQuaInfoVo.Id)
              .Select(item => new PhieuInKetQuaCDHATDCNVo()
              {
                  Header = phieuInKetQuaInfoVo.HasHeader ? hearder : "",

                  LogoUrl = phieuInKetQuaInfoVo.HostingName + "/assets/img/logo-bacha-full.png",
                  BarCodeImgBase64 = BarcodeHelper.GenerateBarCode(item.YeuCauTiepNhan.MaYeuCauTiepNhan, 150, 30, true),
                  MaBarcode = "Mã TN:" + item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                  MaBenhNhan = "Mã NB:" + item.YeuCauTiepNhan.BenhNhan.MaBN,

                  HoTen = item.YeuCauTiepNhan.HoTen,
                  NamSinh = item.YeuCauTiepNhan.NamSinh,
                  GioiTinh = item.YeuCauTiepNhan.GioiTinh.GetDescription(),
                  DiaChi = item.YeuCauTiepNhan.DiaChiDayDu,
                  BacSiChiDinh = string.Format("{0} {1}", (item.NhanVienChiDinh.HocHamHocVi != null ? !string.IsNullOrEmpty(item.NhanVienChiDinh.HocHamHocVi.Ma) ? item.NhanVienChiDinh.HocHamHocVi.Ma + " " : null : null), item.NhanVienChiDinh.User.HoTen),
                  NgayChiDinh = string.Format("{0} giờ {1} ngày {2}", item.ThoiDiemChiDinh.Hour, item.ThoiDiemChiDinh.Minute, item.ThoiDiemChiDinh.ApplyFormatDate()),
                  NoiChiDinh = item.NoiChiDinh.Ten + (item.NoiChiDinh.Tang ?? ""),
                  NoiThucHien = item.NoiThucHien.Ten,
                  SoBenhAn = item.YeuCauTiepNhan.NoiTruBenhAn != null ? item.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn : null,
                  TenChiDinhDichVu = item.TenDichVu,

                  //todo: có thể sẽ cần update lại
                  ChanDoan = item.NoiTruPhieuDieuTri == null ?
                        (item.YeuCauKhamBenh.Icdchinh != null ? item.YeuCauKhamBenh.Icdchinh.Ma + "-" + item.YeuCauKhamBenh.Icdchinh.TenTiengViet : null) :
                        (item.NoiTruPhieuDieuTri.ChanDoanChinhICD != null ? item.NoiTruPhieuDieuTri.ChanDoanChinhICD.Ma + "-" + item.NoiTruPhieuDieuTri.ChanDoanChinhICD.TenTiengViet : null),

                  DataKetQuaCanLamSang = item.DataKetQuaCanLamSang,
                  //KetQuaChiTiet = string.IsNullOrEmpty(item.DataKetQuaCanLamSang) ? new ChiTietKetQuaCDHATDCNVo() : JsonConvert.DeserializeObject<ChiTietKetQuaCDHATDCNVo>(item.DataKetQuaCanLamSang),
                  KetQuaChiTiet = new ChiTietKetQuaCDHATDCNVo(),

                  BacSiChuyenKhoa = item.NhanVienKetLuan != null ? item.NhanVienKetLuan.User.HoTen : null,
                  HocViBacSi = item.NhanVienKetLuan.HocHamHocVi != null ? !string.IsNullOrEmpty(item.NhanVienKetLuan.HocHamHocVi.Ma) ? item.NhanVienKetLuan.HocHamHocVi.Ma + " " : null : null,

                  Ngay = item.ThoiDiemThucHien.Value.Day.ConvertDateToString(),
                  Thang = item.ThoiDiemThucHien.Value.Month.ConvertMonthToString(),
                  Nam = item.ThoiDiemThucHien.Value.Year.ConvertYearToString(),

                  DienGiai = item.NoiTruPhieuDieuTri == null ?
                        (item.YeuCauKhamBenh.ChanDoanSoBoICD != null ? item.YeuCauKhamBenh.ChanDoanSoBoGhiChu : null) :
                        (item.NoiTruPhieuDieuTri.ChanDoanChinhICD != null ? item.NoiTruPhieuDieuTri.ChanDoanChinhGhiChu : null),

                  STTNhanVien = item.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien != null && item.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.STTNhanVien != null ? $"<p class='round-sttNhanVien' style='margin:0;padding:0'>{item.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.STTNhanVien.ToString()}</p>" : string.Empty,
              }).First();

            if (!string.IsNullOrEmpty(data.DataKetQuaCanLamSang))
            {
                var dataKetQuaCanLamSang = JsonConvert.DeserializeObject<ChiTietKetQuaCDHATDCNVo>(data.DataKetQuaCanLamSang);
                data.KetQuaChiTiet = dataKetQuaCanLamSang;
                if (data.HinhAnhDinhKems.Any() && data.CoInKetQuaKemHinhAnh)
                {
                    data.HinhAnh = "<div style='width:100%'>"; //class='container'
                    foreach (var hinhAnh in data.HinhAnhDinhKems)//.Where(x => x.InKemKetQua))
                    {
                        data.HinhAnh += "<div style='width: 24%;float:left;margin: 0 0.5%;'>";
                        var size = new Size { Height = 200, Width = 200 };
                        var resizeImage = Camino.Core.Helpers.ResizeImageString64.ResizeImage(hinhAnh.HinhAnh, size);
                        data.HinhAnh += "<img style='width:100%;' src='" + resizeImage + "' />"; //<div class='image'></div>
                        data.HinhAnh += "<p style='text-align:center;margin-top:15px;font-weight:bold;'>" + hinhAnh.MoTa + "</p>";
                        data.HinhAnh += "</div>";
                    }
                    data.HinhAnh += "</div>";
                }
            }

            content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);// + "<div class='pagebreak'></div>";
            return content;
        }

        public string XuLyInPhieuKetQuaTheoYeuCauAsync(CauHinhHinhVo cauHinhIn)
        {
            var content = string.Empty;
            var hearder = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
                      "<th>PHIẾU KẾT QUẢ</th>" +
                      "</p>";

            //var cauHinhInKyThuatChonSan = _cauHinhService.GetSetting("CauHinhCDHA.CauHinhInKyThuat");
            //var kyThuatChonIns = JsonConvert.DeserializeObject<List<KyThuatBenhVienChonSan>>(cauHinhInKyThuatChonSan.Value);
            //var NhomDichVuBenhVienIdIds = kyThuatChonIns.Any() ? kyThuatChonIns.Select(c => c.NhomDichVuBenhVienId) : null;

            var template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuKetQuaCDHATDCNTheoYeuCau"));
            var data = _yeuCauDichVuKyThuatRepository.TableNoTracking.Where(x => x.Id == cauHinhIn.Id)
              .Select(item => new PhieuInKetQuaCDHATDCNVo()
              {
                  Header = cauHinhIn.HasHeader ? hearder : "",
                  LogoUrl = cauHinhIn.HostingName + "/assets/img/logo-bacha-full.png",
                  BarCodeImgBase64 = BarcodeHelper.GenerateBarCode(item.YeuCauTiepNhan.MaYeuCauTiepNhan, 150, 30, true),

                  MaBarcode = "Mã TN:" + item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                  MaBenhNhan = "Mã NB:" + item.YeuCauTiepNhan.BenhNhan.MaBN,

                  HoTen = item.YeuCauTiepNhan.HoTen,
                  NamSinh = item.YeuCauTiepNhan.NamSinh,
                  GioiTinh = item.YeuCauTiepNhan.GioiTinh.GetDescription(),
                  DiaChi = item.YeuCauTiepNhan.DiaChiDayDu,
                  BacSiChiDinh = string.Format("{0} {1}", (item.NhanVienChiDinh.HocHamHocVi != null ? !string.IsNullOrEmpty(item.NhanVienChiDinh.HocHamHocVi.Ma) ? item.NhanVienChiDinh.HocHamHocVi.Ma + " " : null : null), item.NhanVienChiDinh.User.HoTen),                  
                  NgayChiDinh = string.Format("{0} giờ {1} ngày {2}", item.ThoiDiemChiDinh.Hour, item.ThoiDiemChiDinh.Minute, item.ThoiDiemChiDinh.ApplyFormatDate()),
                  NoiChiDinh = item.NoiChiDinh.Ten + (item.NoiChiDinh.Tang ?? ""),
                  NoiThucHien = item.NoiThucHien.Ten,
                  SoBenhAn = item.YeuCauTiepNhan.NoiTruBenhAn != null ? item.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn : null,
                  TenChiDinhDichVu = item.TenDichVu,

                  //todo: có thể sẽ cần update lại
                  ChanDoan = item.NoiTruPhieuDieuTri == null ?
                        (item.YeuCauKhamBenh.Icdchinh != null ? item.YeuCauKhamBenh.Icdchinh.Ma + "-" + item.YeuCauKhamBenh.Icdchinh.TenTiengViet : null) :
                        (item.NoiTruPhieuDieuTri.ChanDoanChinhICD != null ? item.NoiTruPhieuDieuTri.ChanDoanChinhICD.Ma + "-" + item.NoiTruPhieuDieuTri.ChanDoanChinhICD.TenTiengViet : null),

                  DataKetQuaCanLamSang = item.DataKetQuaCanLamSang,
                  //KetQuaChiTiet = string.IsNullOrEmpty(item.DataKetQuaCanLamSang) ? new ChiTietKetQuaCDHATDCNVo() : JsonConvert.DeserializeObject<ChiTietKetQuaCDHATDCNVo>(item.DataKetQuaCanLamSang),
                  KetQuaChiTiet = new ChiTietKetQuaCDHATDCNVo(),

                  BacSiChuyenKhoa = item.NhanVienKetLuan != null ? item.NhanVienKetLuan.User.HoTen : null,
                  HocViBacSi = item.NhanVienKetLuan.HocHamHocVi != null ? !string.IsNullOrEmpty(item.NhanVienKetLuan.HocHamHocVi.Ma) ? item.NhanVienKetLuan.HocHamHocVi.Ma + " " : null : null,

                  Ngay = DateTime.Now.Day.ConvertDateToString(),
                  Thang = DateTime.Now.Month.ConvertMonthToString(),
                  Nam = DateTime.Now.Year.ConvertYearToString(),
                  DienGiai = item.NoiTruPhieuDieuTri == null ?
                        (item.YeuCauKhamBenh.ChanDoanSoBoICD != null ? item.YeuCauKhamBenh.ChanDoanSoBoGhiChu : null) :
                        (item.NoiTruPhieuDieuTri.ChanDoanChinhICD != null ? item.NoiTruPhieuDieuTri.ChanDoanChinhGhiChu : null),

                  STTNhanVien = item.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien != null && item.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.STTNhanVien != null ? $"<p class='round-sttNhanVien' style='margin:0;padding:0'>{item.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.STTNhanVien.ToString()}</p>" : string.Empty,
                  //NhomDichVuBenhVienDaChon = NhomDichVuBenhVienIdIds != null ? NhomDichVuBenhVienIdIds.Contains(item.NhomDichVuBenhVienId) : false

              }).First();

            if (!string.IsNullOrEmpty(data.DataKetQuaCanLamSang))
            {
                var dataKetQuaCanLamSang = JsonConvert.DeserializeObject<ChiTietKetQuaCDHATDCNVo>(data.DataKetQuaCanLamSang);
                data.KetQuaChiTiet = dataKetQuaCanLamSang;
                if (data.HinhAnhDinhKems.Any() && data.CoInKetQuaKemHinhAnh)
                {
                    data.HinhAnh = "<div style='width:100%'>"; //class='container'
                    foreach (var hinhAnh in data.HinhAnhDinhKems)//.Where(x => x.InKemKetQua))
                    {
                        data.HinhAnh += "<div style='width: 24%;float:left;margin: 0 0.5%;'>";
                        var size = new Size { Height = 200, Width = 200 };
                        var resizeImage = Camino.Core.Helpers.ResizeImageString64.ResizeImage(hinhAnh.HinhAnh, size);
                        data.HinhAnh += "<img style='width:100%;' src='" + resizeImage + "' />"; //<div class='image'></div>
                        data.HinhAnh += "<p style='text-align:center;margin-top:15px;font-weight:bold;'>" + hinhAnh.MoTa + "</p>";
                        data.HinhAnh += "</div>";
                    }
                    data.HinhAnh += "</div>";
                }
            }


            //In theo yêu cầu khách hàng muốn
            if (cauHinhIn.InLogo)
            {
                data.InLogo = "<td>" +
                              "<img src='" + data.LogoUrl + "' style='height: 85px;' alt='lo-go'/>" + data.STTNhanVien +
                              "</td>";
            }

            if (cauHinhIn.InBarcode)
            {
                data.InBarcode = "<td style='padding-left: 50px;'>" +
                          "<div style='text-align:center;float:right;'><img style='height: 40px;' src='data:image/png;base64," + data.BarCodeImgBase64 + "'> <br>" +
                          "<p style='margin:0;padding:0'>" + data.MaBarcode + "</p>" +
                          "<p style='margin:0;padding:0'>" + data.MaBenhNhan + " </p></div>" +
                           "</td>";
            }

            if (cauHinhIn.InTieuDe)
            {
                data.InTieuDe = "<table style='padding: 5px;width: 100%;' >" +
                                "<th><span style='font-size: 28;'>" + data.TenKetQuaLabel + " " + data.TenKetQua + " </span><br></th></table>";
            }

            if (cauHinhIn.InHoVaTen)
            {
                data.InHoVaTen = "<td colspan='2'>Họ và tên: <b>" + data.HoTen + "</b></td>";
            }

            if (cauHinhIn.InNamSinh)
            {
                data.InNamSinh = "<td>Năm sinh:" + data.NamSinh + "</td>";
            }

            if (cauHinhIn.InGioiTinh)
            {
                data.InGioiTinh = "<td>Giới tính: " + data.GioiTinh + "</td>";
            }

            if (cauHinhIn.InDiaChi)
            {
                data.InDiaChi = "<td colspan='3'>Địa chỉ: " + data.DiaChi + "</td>";
            }

            if (cauHinhIn.InSoBenhAn)
            {
                data.InSoBenhAn = "<td colspan='1'>Số bệnh án: " + data.SoBenhAn + "</td>";
            }

            if (cauHinhIn.InBSChiDinh)
            {
                data.InBSChiDinh = "<td colspan='2'>BS.chỉ định: " + data.BacSiChiDinh + "</td>";
            }

            if (cauHinhIn.InNgayChiDinh)
            {
                data.InNgayChiDinh = "<td colspan='2'>Ngày chỉ định: " + data.NgayChiDinh + "</td>";
            }

            if (cauHinhIn.InNoiChiDinh)
            {
                data.InNoiChiDinh = "<td colspan='2'>Nơi chỉ định: " + data.NoiChiDinh + "</td>";
            }

            if (cauHinhIn.InNoiThucHien)
            {
                data.InNoiThucHien = "<td colspan='2'>Nơi thực hiện: " + data.NoiThucHien + "</td>";
            }

            if (cauHinhIn.InChuanDoan)
            {
                data.InChuanDoan = "<td colspan='2'>Chẩn đoán: " + data.ChanDoan + "</td>";
            }

            if (cauHinhIn.InDienGiai)
            {
                data.InDienGiai = "<td colspan='2'>Diễn giải:" + data.DienGiai + "</td>";
            }

            if (cauHinhIn.InChiDinh)
            {
                data.InChiDinh = "<td colspan='4'> Chỉ định: " + data.TenChiDinhDichVu + "</td >";
            }

            if (cauHinhIn.InThanhNgang)
            {
                data.InThanhNgang = "<div class='line_bottom'></div>";
            }
            //data.NhomDichVuBenhVienDaChon ||
            if (cauHinhIn.InKyThuat)
            {
                data.InKyThuat = "<h3 style=" + data.HienThiKyThuat + "><u>KỸ THUẬT</u></h3>" +
                                 "<div>" + data.KyThuat + "</div>";
            }

            content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);// + "<div class='pagebreak'></div>";
            return content;
        }

        #endregion

        #region Cập nhật 27/12/2022
        public YeuCauTiepNhan GetThongTinChungBenhNhan(long yeuCauTiepNhanId)
        {
            var yctn = BaseRepository.TableNoTracking
                .Include(z => z.NoiTruBenhAn)
                .Include(z => z.BenhNhan)
                .FirstOrDefault(x => x.Id == yeuCauTiepNhanId);
            if(yctn == null)
            {
                throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
            }
            var noiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var lstYckt = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Include(y => y.NhanVienChiDinh).ThenInclude(z => z.User)
                .Include(y => y.NoiChiDinh)
                .Include(y => y.YeuCauKhamBenh).ThenInclude(z => z.Icdchinh)
                .Include(y => y.NoiTruPhieuDieuTri).ThenInclude(z => z.ChanDoanChinhICD)
                .Include(y => y.NhomDichVuBenhVien)
                .Include(y => y.FileKetQuaCanLamSangs)
                .Include(y => y.DichVuKyThuatBenhVien)
                .Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId
                            && x.NoiThucHienId == noiThucHienId
                            && (x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh))
                .ToList();

            lstYckt.ForEach(x => yctn.YeuCauDichVuKyThuats.Add(x));
            return yctn;
        }
        #endregion
    }
}