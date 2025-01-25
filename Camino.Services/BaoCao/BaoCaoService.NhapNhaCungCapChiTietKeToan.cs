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
    public partial class BaoCaoService : IBaoCaoService
    {
        public async Task<List<LookupItemVo>> GetTatCaKhoNhapChiTietKeToans(DropDownListRequestModel queryInfo)
        {
            var result = _khoRepository.TableNoTracking
               .Select(s => new LookupItemVo
               {
                   KeyId = s.Id,
                   DisplayName = s.Ten,
               }).ApplyLike(queryInfo.Query, o => o.DisplayName)
                                     .Take(queryInfo.Take)
                                     .ToList();
            result.Insert(0, new LookupItemVo { KeyId = 0, DisplayName = "Tất cả" });
            return result;
        }

        public async Task<GridDataSource> GetDataNhapNhaCungCapChiTietKeToanForGrid(NhapNhaCungCapChiTietKeToanDuocQueryInfoVo queryInfo)
        {
            var nhaThaus = _nhaThauRepository.TableNoTracking.Select(o => new { o.Id, o.Ten }).ToList();
            var khos = _khoRepository.TableNoTracking.Select(o => new { o.Id, o.Ten }).ToList();
            var thongTinDuocPhams = _duocPhamBenhVienRepository.TableNoTracking
                .Select(o => new { o.Id, o.Ma, o.DuocPham.Ten, o.DuocPham.HoatChat, o.DuocPham.QuyCach, DonViTinh = o.DuocPham.DonViTinh.Ten })
                .ToList();
            var thongTinVatTus = _vatTuBenhVienRepository.TableNoTracking
                .Select(o => new { o.Id, o.Ma, o.VatTus.Ten, o.VatTus.QuyCach, o.VatTus.DonViTinh })
                .ToList();


            var dsDuocPhamNhapKhoQuery = _yeuCauNhapKhoDuocPhamRepository.TableNoTracking
                .Where(o => o.DuocKeToanDuyet == true);
            if (!string.IsNullOrEmpty(queryInfo.TimKiem))
            {
                dsDuocPhamNhapKhoQuery = dsDuocPhamNhapKhoQuery.Where(o => o.SoChungTu == queryInfo.TimKiem);
            }
            if (queryInfo.LoaiNgayTimKiem == LoaiNgayTimKiem.NgayDuyetNhap)
            {
                dsDuocPhamNhapKhoQuery = dsDuocPhamNhapKhoQuery.Where(o => o.NgayDuyet >= queryInfo.FromDate && o.NgayDuyet < queryInfo.ToDate);
            }
            else if (queryInfo.LoaiNgayTimKiem == LoaiNgayTimKiem.NgayHoaDon)
            {
                dsDuocPhamNhapKhoQuery = dsDuocPhamNhapKhoQuery.Where(o => o.NgayHoaDon >= queryInfo.FromDate && o.NgayHoaDon < queryInfo.ToDate);
            }
            else
            {
                dsDuocPhamNhapKhoQuery = dsDuocPhamNhapKhoQuery.Where(o => o.NgayNhap >= queryInfo.FromDate && o.NgayNhap < queryInfo.ToDate);
            }

            var dsDuocPhamNhapKho = dsDuocPhamNhapKhoQuery
                .Select(o => new
                {
                    NgayNhap = o.NgayNhap,
                    SoPhieu = o.SoPhieu,
                    NgayHoaDon = o.NgayHoaDon,
                    SoHoaDon = o.SoChungTu,
                    KyHieuHoaDon = o.KyHieuHoaDon,
                    NhapKhoChiTiets = o.YeuCauNhapKhoDuocPhamChiTiets.Select(ct => new
                    {
                        KhoNhapSauKhiDuyetId = ct.KhoNhapSauKhiDuyetId,
                        NhaThauId = ct.HopDongThauDuocPham.NhaThauId,
                        DuocPhamBenhVienId = ct.DuocPhamBenhVienId,
                        DonGiaNhap = ct.DonGiaNhap,
                        SoLuongNhap = ct.SoLuongNhap,
                        Solo = ct.Solo,
                        HanSuDung = ct.HanSuDung,
                        VAT = ct.VAT,
                        DonGiaBan = ct.DonGiaBan,
                        ThueVatLamTron = ct.ThueVatLamTron.GetValueOrDefault(),
                        GhiChu = ct.GhiChu
                    }).ToList()
                }).ToList();

            var returnData = new List<NhapNhaCungCapChiTietKeToanDuoc>();

            foreach (var nhapKho in dsDuocPhamNhapKho)
            {
                foreach (var nhapKhoChiTiet in nhapKho.NhapKhoChiTiets)
                {
                    if (queryInfo.KhoNhapId == 0 || queryInfo.KhoNhapId == nhapKhoChiTiet.KhoNhapSauKhiDuyetId)
                    {
                        var nhomNhaCungCap = $"{nhapKho.KyHieuHoaDon} - {nhapKho.SoHoaDon} - {nhaThaus.FirstOrDefault(o => o.Id == nhapKhoChiTiet.NhaThauId)?.Ten}";
                        var duocPham = thongTinDuocPhams.First(o => o.Id == nhapKhoChiTiet.DuocPhamBenhVienId);
                        var nhapKhoChiTietGridItem = new NhapNhaCungCapChiTietKeToanDuoc
                        {
                            NhomNhaCungCap = nhomNhaCungCap,
                            KhoNhap = nhapKhoChiTiet.KhoNhapSauKhiDuyetId != null ? khos.FirstOrDefault(o => o.Id == nhapKhoChiTiet.KhoNhapSauKhiDuyetId)?.Ten : "",
                            NgayChungTu = nhapKho.NgayNhap,
                            SoChungTu = nhapKho.SoPhieu,
                            NgayHoaDon = nhapKho.NgayHoaDon,
                            MaHangHoa = duocPham.Ma,
                            TenHangHoa = duocPham.Ten,
                            HoatChat = duocPham.HoatChat,
                            QuyCachDongGoi = duocPham.QuyCach,
                            SoLo = nhapKhoChiTiet.Solo,
                            HanDung = nhapKhoChiTiet.HanSuDung,
                            DVT = duocPham.DonViTinh,
                            SoLuongNhap = nhapKhoChiTiet.SoLuongNhap,
                            DonGiaNhap = nhapKhoChiTiet.DonGiaNhap,
                            VAT = nhapKhoChiTiet.ThueVatLamTron,
                            DonGiaCoVat = nhapKhoChiTiet.DonGiaBan,
                            GhiChu = nhapKhoChiTiet.GhiChu
                        };
                        returnData.Add(nhapKhoChiTietGridItem);
                    }
                }
            }

            var dsVatTuNhapKhoQuery = _yeuCauNhapKhoVatTuRepository.TableNoTracking
                .Where(o => o.DuocKeToanDuyet == true);
            if (!string.IsNullOrEmpty(queryInfo.TimKiem))
            {
                dsVatTuNhapKhoQuery = dsVatTuNhapKhoQuery.Where(o => o.SoChungTu == queryInfo.TimKiem);
            }
            if (queryInfo.LoaiNgayTimKiem == LoaiNgayTimKiem.NgayDuyetNhap)
            {
                dsVatTuNhapKhoQuery = dsVatTuNhapKhoQuery.Where(o => o.NgayDuyet >= queryInfo.FromDate && o.NgayDuyet < queryInfo.ToDate);
            }
            else if (queryInfo.LoaiNgayTimKiem == LoaiNgayTimKiem.NgayHoaDon)
            {
                dsVatTuNhapKhoQuery = dsVatTuNhapKhoQuery.Where(o => o.NgayHoaDon >= queryInfo.FromDate && o.NgayHoaDon < queryInfo.ToDate);
            }
            else
            {
                dsVatTuNhapKhoQuery = dsVatTuNhapKhoQuery.Where(o => o.NgayNhap >= queryInfo.FromDate && o.NgayNhap < queryInfo.ToDate);
            }

            var dsVatTuNhapKho = dsVatTuNhapKhoQuery
                .Select(o => new
                {
                    NgayNhap = o.NgayNhap,
                    SoPhieu = o.SoPhieu,
                    NgayHoaDon = o.NgayHoaDon,
                    SoHoaDon = o.SoChungTu,
                    KyHieuHoaDon = o.KyHieuHoaDon,
                    NhapKhoChiTiets = o.YeuCauNhapKhoVatTuChiTiets.Select(ct => new
                    {
                        KhoNhapSauKhiDuyetId = ct.KhoNhapSauKhiDuyetId,
                        NhaThauId = ct.HopDongThauVatTu.NhaThauId,
                        VatTuBenhVienId = ct.VatTuBenhVienId,
                        DonGiaNhap = ct.DonGiaNhap,
                        SoLuongNhap = ct.SoLuongNhap,
                        Solo = ct.Solo,
                        HanSuDung = ct.HanSuDung,
                        VAT = ct.VAT,
                        DonGiaBan = ct.DonGiaBan,
                        ThueVatLamTron = ct.ThueVatLamTron.GetValueOrDefault(),
                        GhiChu = ct.GhiChu
                    }).ToList()
                }).ToList();

            foreach (var nhapKho in dsVatTuNhapKho)
            {
                foreach (var nhapKhoChiTiet in nhapKho.NhapKhoChiTiets)
                {
                    if (queryInfo.KhoNhapId == 0 || queryInfo.KhoNhapId == nhapKhoChiTiet.KhoNhapSauKhiDuyetId)
                    {
                        var nhomNhaCungCap = $"{nhapKho.KyHieuHoaDon} - {nhapKho.SoHoaDon} - {nhaThaus.FirstOrDefault(o => o.Id == nhapKhoChiTiet.NhaThauId)?.Ten}";
                        var vatTu = thongTinVatTus.First(o => o.Id == nhapKhoChiTiet.VatTuBenhVienId);
                        var nhapKhoChiTietGridItem = new NhapNhaCungCapChiTietKeToanDuoc
                        {
                            NhomNhaCungCap = nhomNhaCungCap,
                            KhoNhap = nhapKhoChiTiet.KhoNhapSauKhiDuyetId != null ? khos.FirstOrDefault(o => o.Id == nhapKhoChiTiet.KhoNhapSauKhiDuyetId)?.Ten : "",
                            NgayChungTu = nhapKho.NgayNhap,
                            SoChungTu = nhapKho.SoPhieu,
                            NgayHoaDon = nhapKho.NgayHoaDon,
                            MaHangHoa = vatTu.Ma,
                            TenHangHoa = vatTu.Ten,
                            HoatChat = String.Empty,
                            QuyCachDongGoi = vatTu.QuyCach,
                            SoLo = nhapKhoChiTiet.Solo,
                            HanDung = nhapKhoChiTiet.HanSuDung,
                            DVT = vatTu.DonViTinh,
                            SoLuongNhap = nhapKhoChiTiet.SoLuongNhap,
                            DonGiaNhap = nhapKhoChiTiet.DonGiaNhap,
                            VAT = nhapKhoChiTiet.ThueVatLamTron,
                            DonGiaCoVat = nhapKhoChiTiet.DonGiaBan,
                            GhiChu = nhapKhoChiTiet.GhiChu
                        };
                        returnData.Add(nhapKhoChiTietGridItem);
                    }
                }
            }

            return new GridDataSource
            {
                Data = returnData.OrderBy(o => o.NhomNhaCungCap).ThenBy(o => o.TenHangHoa).ToArray(),
                TotalRowCount = returnData.Count()
            };
        }

        public virtual byte[] ExportBaoCaoNhapCungCapChiTietKeToan(GridDataSource gridDataSource, NhapNhaCungCapChiTietKeToanDuocQueryInfoVo query)
        {
            var datas = (ICollection<NhapNhaCungCapChiTietKeToanDuoc>)gridDataSource.Data;
            var listNhom = datas.GroupBy(s => s.NhomNhaCungCap).Select(s => s.First().NhomNhaCungCap).ToList();

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("[KT+Dược]BC tình hình nhập nhà cung cấp chi tiết");
                    //set row
                    worksheet.DefaultRowHeight = 16;

                    //set chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 7;
                    worksheet.Column(2).Width = 20;
                    worksheet.Column(3).Width = 20;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 20;
                    worksheet.Column(6).Width = 20;
                    worksheet.Column(7).Width = 30;
                    worksheet.Column(8).Width = 30;
                    worksheet.Column(9).Width = 20;
                    worksheet.Column(10).Width = 20;
                    worksheet.Column(11).Width = 20;
                    worksheet.Column(12).Width = 20;
                    worksheet.Column(13).Width = 20;
                    worksheet.Column(14).Width = 40;
                    worksheet.Column(15).Width = 20;
                    worksheet.Column(16).Width = 20;
                    worksheet.Column(17).Width = 20;
                    worksheet.Column(18).Width = 20;
                    worksheet.Column(19).Width = 40;
                    worksheet.DefaultColWidth = 7;
                    worksheet.Row(2).Height = 20.5;

                    using (var range = worksheet.Cells["A1:S1"])
                    {
                        range.Worksheet.Cells["A1:S1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:S1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:S1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A1:S1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:S1"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A1:S1"].Merge = true;
                        range.Worksheet.Cells["A1:S1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                    }

                    using (var range = worksheet.Cells["A2:S2"])
                    {
                        range.Worksheet.Cells["A2:S2"].Style.Font.SetFromFont(new Font("Times New Roman", 17));
                        range.Worksheet.Cells["A2:S2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A2:S2"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A2:S2"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A2:S2"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A2:S2"].Merge = true;
                        range.Worksheet.Cells["A2:S2"].Value = "BÁO CÁO TÌNH HÌNH NHẬP NHÀ CUNG CẤP CHI TIẾT";

                    }
                    using (var range = worksheet.Cells["A3:S3"])
                    {
                        range.Worksheet.Cells["A3:S3"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A3:S3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:S3"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A3:S3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:S3"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A3:S3"].Style.Font.Italic = true;
                        range.Worksheet.Cells["A3:S3"].Merge = true;
                        range.Worksheet.Cells["A3:S3"].Value = "Thời gian từ: " + query.FromDate.ApplyFormatDate()
                                                          + " - " + query.ToDate.ApplyFormatDate();

                    }

                    int indexHeader = 4;
                    using (var range = worksheet.Cells["A" + indexHeader + ":S" + indexHeader])
                    {
                        range.Worksheet.Cells["A" + indexHeader + ":S" + indexHeader].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A" + indexHeader + ":S" + indexHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A" + indexHeader + ":S" + indexHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A" + indexHeader + ":S" + indexHeader].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A" + indexHeader + ":S" + indexHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A" + indexHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A" + indexHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A" + indexHeader].Value = "STT";

                        range.Worksheet.Cells["B" + indexHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B" + indexHeader].Value = "Kho nhập";

                        range.Worksheet.Cells["C" + indexHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C" + indexHeader].Value = "Ngày chứng từ";

                        range.Worksheet.Cells["D" + indexHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D" + indexHeader].Value = "Số chứng từ";

                        range.Worksheet.Cells["E" + indexHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E" + indexHeader].Value = "Ngày hóa đơn";

                        range.Worksheet.Cells["F" + indexHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F" + indexHeader].Value = "Mã hàng hóa";

                        range.Worksheet.Cells["G" + indexHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G" + indexHeader].Value = "Tên hàng hóa";

                        range.Worksheet.Cells["H" + indexHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H" + indexHeader].Value = "Hoạt chất";

                        range.Worksheet.Cells["I" + indexHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I" + indexHeader].Value = "Quy cách đóng gói";

                        range.Worksheet.Cells["J" + indexHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J" + indexHeader].Value = "Số lô";

                        range.Worksheet.Cells["K" + indexHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K" + indexHeader].Value = "Hạn dùng";

                        range.Worksheet.Cells["L" + indexHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["L" + indexHeader].Value = "ĐVT";

                        range.Worksheet.Cells["M" + indexHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["M" + indexHeader].Value = "Số lượng nhập ";

                        range.Worksheet.Cells["N" + indexHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["N" + indexHeader].Value = "Đơn giá nhập ";

                        range.Worksheet.Cells["O" + indexHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["O" + indexHeader].Value = "Thành tiền";

                        range.Worksheet.Cells["P" + indexHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["P" + indexHeader].Value = "VAT";

                        range.Worksheet.Cells["Q" + indexHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["Q" + indexHeader].Value = "Tổng";

                        range.Worksheet.Cells["R" + indexHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["R" + indexHeader].Value = "Đơn giá bán có VAT";

                        range.Worksheet.Cells["S" + indexHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["S" + indexHeader].Value = "Ghi chú";
                    }


                    int index = indexHeader + 1;

                    ///Đổ data
                    ///
                    var stt = 1;

                    if (listNhom.Any())
                    {
                        foreach (var nhom in listNhom)
                        {
                            var listTheoNhom = datas.Where(s => s.NhomNhaCungCap == nhom).ToList();
                            if (listTheoNhom.Any())
                            {
                                using (var range = worksheet.Cells["A" + index + ":S" + index])
                                {
                                    range.Worksheet.Cells["A" + index + ":S" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                    range.Worksheet.Cells["A" + index + ":S" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["A" + index + ":S" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                    range.Worksheet.Cells["A" + index + ":S" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                    range.Worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                    range.Worksheet.Cells["B" + index + ":F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["B" + index + ":F" + index].Merge = true;
                                    range.Worksheet.Cells["B" + index + ":F" + index].Style.Font.Bold = true;
                                    range.Worksheet.Cells["B" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    range.Worksheet.Cells["B" + index + ":F" + index].Value = nhom;

                                    range.Worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                    range.Worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                    range.Worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                    range.Worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                    range.Worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                    range.Worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                    range.Worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["Q" + index].Value = listTheoNhom.Sum(s => s.Tong);
                                    range.Worksheet.Cells["Q" + index].Style.Font.Bold = true;
                                    range.Worksheet.Cells["Q" + index].Style.Numberformat.Format = "#,##0.00";
                                    range.Worksheet.Cells["Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    range.Worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                    index++;

                                }

                                foreach (var item in listTheoNhom)
                                {
                                    using (var range = worksheet.Cells["A" + index + ":S" + index])
                                    {
                                        range.Worksheet.Cells["A" + index + ":S" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                        range.Worksheet.Cells["A" + index + ":S" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                        range.Worksheet.Cells["A" + index + ":S" + index].Style.Font.Color.SetColor(Color.Black);
                                        range.Worksheet.Cells["A" + index + ":S" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                        range.Worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        range.Worksheet.Cells["A" + index].Value = stt;

                                        range.Worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        range.Worksheet.Cells["B" + index].Value = item.KhoNhap;

                                        range.Worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        range.Worksheet.Cells["C" + index].Value = item.NgayChungTuDisplay;

                                        range.Worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        range.Worksheet.Cells["D" + index].Value = item.SoChungTu;

                                        range.Worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        range.Worksheet.Cells["E" + index].Value = item.NgayHoaDonDisplay;

                                        range.Worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        range.Worksheet.Cells["F" + index].Value = item.MaHangHoa;

                                        range.Worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        range.Worksheet.Cells["G" + index].Value = item.TenHangHoa;

                                        range.Worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        range.Worksheet.Cells["H" + index].Value = item.HoatChat;

                                        range.Worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        range.Worksheet.Cells["I" + index].Value = item.QuyCachDongGoi;

                                        range.Worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        range.Worksheet.Cells["J" + index].Value = item.SoLo;

                                        range.Worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        range.Worksheet.Cells["K" + index].Value = item.HanDungDisplay;

                                        range.Worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        range.Worksheet.Cells["L" + index].Value = item.DVT;

                                        range.Worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                        range.Worksheet.Cells["M" + index].Value = item.SoLuongNhap;

                                        range.Worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                        range.Worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                                        range.Worksheet.Cells["N" + index].Value = item.DonGiaNhap;

                                        range.Worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["O" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                        range.Worksheet.Cells["O" + index].Value = item.ThanhTien;
                                        range.Worksheet.Cells["O" + index].Style.Numberformat.Format = "#,##0.00";
                                        range.Worksheet.Cells["O" + index].Style.WrapText = true;

                                        range.Worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["P" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                        range.Worksheet.Cells["P" + index].Style.Numberformat.Format = "#,##0.00";
                                        range.Worksheet.Cells["P" + index].Value = item.VAT;

                                        range.Worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                        range.Worksheet.Cells["Q" + index].Style.Numberformat.Format = "#,##0.00";
                                        range.Worksheet.Cells["Q" + index].Value = item.Tong;

                                        range.Worksheet.Cells["R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["R" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                        range.Worksheet.Cells["R" + index].Style.Numberformat.Format = "#,##0.00";
                                        range.Worksheet.Cells["R" + index].Value = item.DonGiaCoVat;

                                        range.Worksheet.Cells["S" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["S" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                        range.Worksheet.Cells["S" + index].Value = item.GhiChu;

                                        stt++;
                                        index++;

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

        public List<NhapNhaCungCapChiTietKeToanDuoc> DummyData()
        {
            return new List<NhapNhaCungCapChiTietKeToanDuoc> {
                new NhapNhaCungCapChiTietKeToanDuoc
                {
                    NhomNhaCungCap = "1C22TYY-00000447-CHI NHÁNH SỐ 15 CÔNG TY CỔ PHẦN DƯỢC PHẨM THIẾT BỊ Y TẾ HÀ NAM",
                    KhoNhap ="Nhâp từ kho A",
                    NgayChungTu = DateTime.Now,
                    SoChungTu = "1C22TYY-00000447",
                    NgayHoaDon = DateTime.Now,
                    MaHangHoa = "00000447",
                    TenHangHoa="Ciprofloxacxin Kabi 200mg/100ml ",
                    HoatChat="Natri hydrocarbonat (natri bicarbonat)",
                    QuyCachDongGoi = "Hộp 30 gói thuốc cốm",
                    SoLo = "ALB02333",
                    HanDung = DateTime.Now,
                    DVT = "Chai",
                    SoLuongNhap = 30,
                    DonGiaNhap = 40000,
                    VAT = 372777,
                    DonGiaCoVat = 16000,
                    GhiChu = "1C22TYY-00000447-CHI NHÁNH SỐ 15 CÔNG TY CỔ PHẦN DƯỢC PHẨM THIẾT BỊ Y TẾ HÀ NAM"
                },
                new NhapNhaCungCapChiTietKeToanDuoc
                {
                     NhomNhaCungCap = "1C22TYY-00000447-CHI NHÁNH SỐ 15 CÔNG TY CỔ PHẦN DƯỢC PHẨM THIẾT BỊ Y TẾ HÀ NAM",
                    KhoNhap ="Nhâp từ kho A",
                    NgayChungTu = DateTime.Now,
                    SoChungTu = "1C22TYY-00000447",
                    NgayHoaDon = DateTime.Now,
                    MaHangHoa = "00000447",
                    TenHangHoa="Ciprofloxacxin Kabi 200mg/100ml ",
                    HoatChat="Natri hydrocarbonat (natri bicarbonat)",
                    QuyCachDongGoi = "Hộp 30 gói thuốc cốm",
                    SoLo = "ALB02333",
                    HanDung = DateTime.Now,
                    DVT = "Chai",
                    SoLuongNhap = 30,
                    DonGiaNhap = 40000,
                    VAT = 372777,
                    DonGiaCoVat = 16000,
                    GhiChu = "1C22TYY-00000447-CHI NHÁNH SỐ 15 CÔNG TY CỔ PHẦN DƯỢC PHẨM THIẾT BỊ Y TẾ HÀ NAM (Số seri - Số hóa đơn - Tên NCC)"
                },
                new NhapNhaCungCapChiTietKeToanDuoc
                {
                    NhomNhaCungCap = "1C22TYY-00000447-CHI NHÁNH SỐ 15",
                    KhoNhap ="Nhâp từ kho A",
                    NgayChungTu = DateTime.Now,
                    SoChungTu = "1C22TYY-00000447",
                    NgayHoaDon = DateTime.Now,
                    MaHangHoa = "00000447",
                    TenHangHoa="Ciprofloxacxin Kabi 200mg/100ml ",
                    HoatChat="Natri hydrocarbonat (natri bicarbonat)",
                    QuyCachDongGoi = "Hộp 30 gói thuốc cốm",
                    SoLo = "ALB02333",
                    HanDung = DateTime.Now,
                    DVT = "Chai",
                    SoLuongNhap = 30,
                    DonGiaNhap = 40000,
                    VAT = 372777,
                    DonGiaCoVat = 16000,
                    GhiChu = "1C22TYY-00000447-CHI NHÁNH SỐ 15 CÔNG TY CỔ PHẦN DƯỢC PHẨM THIẾT BỊ Y TẾ HÀ NAM (Số seri - Số hóa đơn - Tên NCC)"
                }
            };
        }
    }
}
