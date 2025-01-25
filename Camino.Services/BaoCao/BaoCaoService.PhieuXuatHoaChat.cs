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
using OfficeOpenXml.Drawing;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public List<LookupItemVo> GetTatCaKhoPhieuXuatHoaChat(DropDownListRequestModel queryInfo)
        {
            var result = _khoRepository.TableNoTracking.Where(c => c.LoaiDuocPham == true && c.KhoaPhongId == (long)EnumKhoaPhong.KhoaXetNghiem)
               .Select(s => new LookupItemVo
               {
                   KeyId = s.Id,
                   DisplayName = s.Ten,
               }).ApplyLike(queryInfo.Query, o => o.DisplayName).Take(queryInfo.Take).ToList();

            return result;
        }

        public async Task<List<LookupItemVo>> GetTenMayXetNghiem(DropDownListRequestModel queryInfo)
        {
            var query = _mayXetNghiemRepository.TableNoTracking.Where(o=>o.HieuLuc)
                .ApplyLike(queryInfo.Query, g => g.Ten)
                .Select(s => new LookupItemVo
                {
                    KeyId = s.Id,
                    DisplayName = s.Ten

                }).OrderBy(o => o.DisplayName)
                  .Take(queryInfo.Take)
                  .ToList();

            return query;
        }

        public async Task<List<LookupItemDuocPhamVo>> GetTenDuocPhamTheoKhoXuat(DropDownListRequestModel queryInfo, long? khoId)
        {
            var query = _duocPhamBenhVienRepository.TableNoTracking
                .Where(o => khoId == 0 || o.NhapKhoDuocPhamChiTiets.Any(kho => kho.NhapKhoDuocPhams.KhoId == khoId))
                .ApplyLike(queryInfo.Query, g => g.DuocPham.Ten)
                .Select(s => new LookupItemDuocPhamVo
                {
                    KeyId = s.Id,
                    DisplayName = s.DuocPham.Ten

                }).OrderBy(o => o.DisplayName).Take(queryInfo.Take).ToList();
            var dataDefaut = new LookupItemDuocPhamVo() { KeyId = 0, DisplayName = "Tất cả" };
            query.Insert(0, dataDefaut);

            return query;
        }

        public async Task<GridDataSource> GetDataPhieuXuatHoaChatForGrid(PhieuXuatHoaChatQueryInfo queryInfo)
        {
            var allDataXuat = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking
                .Where(o => o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null && o.SoLuongXuat > 0 &&
                            o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoXuatId == queryInfo.KhoId && o.MayXetNghiemId == queryInfo.MayXetNghiemId &&
                            (queryInfo.DuocPhamId == 0 || o.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienId == queryInfo.DuocPhamId)
                            && ((o.NgayXuat != null && o.NgayXuat >= queryInfo.TuNgay && o.NgayXuat <= queryInfo.DenNgay) ||
                                (o.NgayXuat == null && o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat >= queryInfo.TuNgay && o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat <= queryInfo.DenNgay)))
                .Select(o => new BaoCaoChiTietTonKhoGridVo
                {
                    Id = o.Id,
                    DuocPhamBenhVienId = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienId,
                    Ma = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.Ma,
                    Ten = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.Ten,
                    DVT = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                    SLXuat = o.SoLuongXuat
                }).ToList();

            var allDataGroup = allDataXuat.GroupBy(o => o.DuocPhamBenhVienId);
            var kho = _khoRepository.GetById(queryInfo.KhoId);

            var dataReturn = new List<DanhSachPhieuXuatHoaChat>();
            foreach (var xuatDuocPham in allDataGroup)
            {
                var baoCaoPhieuXuatHoaChat = new DanhSachPhieuXuatHoaChat
                {
                    MayXetNghiemId = queryInfo.MayXetNghiemId,
                    MaDuocPham = xuatDuocPham.First().Ma,
                    KhoId = queryInfo.KhoId,
                    TenKho = kho.Ten,
                    DuocPhamId = xuatDuocPham.Key,
                    TenDuocPham = xuatDuocPham.First().Ten,
                    DonViTinh = xuatDuocPham.First().DVT,
                    TongSoLuongXuat = (xuatDuocPham.Select(o => o.SLXuat.GetValueOrDefault()).DefaultIfEmpty().Sum()).MathRoundNumber(2)
                };
                dataReturn.Add(baoCaoPhieuXuatHoaChat);
            }
            return new GridDataSource { Data = dataReturn.OrderBy(s => s.MaDuocPham).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray(), TotalRowCount = dataReturn.Count };
        }

        public async Task<GridDataSource> GetDataPhieuXuatHoaChatChiTietForGrid(PhieuXuatHoaChatQueryInfo queryInfo)
        {
            var allDataXuat = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking
                .Where(o => o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null && o.SoLuongXuat > 0 &&
                            o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoXuatId == queryInfo.KhoId && o.MayXetNghiemId == queryInfo.MayXetNghiemId &&
                            o.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienId == queryInfo.DuocPhamId
                            && ((o.NgayXuat != null && o.NgayXuat >= queryInfo.TuNgay && o.NgayXuat <= queryInfo.DenNgay) ||
                                (o.NgayXuat == null && o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat >= queryInfo.TuNgay && o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat <= queryInfo.DenNgay)))
                .Select(o => new
                {
                    Id = o.Id,
                    DuocPhamBenhVienId = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienId,
                    Ma = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.Ma,
                    Ten = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.Ten,
                    DVT = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                    SoLo = o.NhapKhoDuocPhamChiTiet.Solo,
                    HanSuDungDateTime = o.NhapKhoDuocPhamChiTiet.HanSuDung,
                    NgayXuat = o.NgayXuat != null
                        ? o.NgayXuat.Value
                        : o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat,
                    SLXuat = o.SoLuongXuat,
                    NguoiXuatId = o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NguoiXuatId,
                    NguoiNhanId = o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NguoiNhanId,
                }).OrderBy(o => o.NgayXuat).ToList();

            var userIds = allDataXuat.Select(o => o.NguoiXuatId).Concat(allDataXuat.Select(o => o.NguoiNhanId.GetValueOrDefault())).ToList();
            var users = _userRepository.TableNoTracking.Where(o => userIds.Contains(o.Id)).Select(o => new { o.Id, o.HoTen }).ToList();

            var dataReturn = new List<DanhSachPhieuXuatHoaChatChiTiet>();
            foreach (var xuatDuocPham in allDataXuat)
            {
                var nguoiNhan = users.FirstOrDefault(o => o.Id == xuatDuocPham.NguoiNhanId.GetValueOrDefault())?.HoTen;
                var nguoiXuat = users.FirstOrDefault(o => o.Id == xuatDuocPham.NguoiXuatId)?.HoTen;
                var baoCaoPhieuXuatHoaChat = new DanhSachPhieuXuatHoaChatChiTiet
                {
                    TenDuocPham = xuatDuocPham.Ten,
                    NgayXuat = xuatDuocPham.NgayXuat,
                    SoLo = xuatDuocPham.SoLo,
                    HanDung = xuatDuocPham.HanSuDungDateTime,
                    LuongXuat = xuatDuocPham.SLXuat,
                    NguoiNhan = nguoiNhan,
                    NguoiXuat = nguoiXuat
                };
                dataReturn.Add(baoCaoPhieuXuatHoaChat);
            }
            return new GridDataSource { Data = dataReturn.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray(), TotalRowCount = dataReturn.Count };
        }

        public virtual byte[] ExportPhieuXuatHoaChat(GridDataSource gridDataSource, PhieuXuatHoaChatQueryInfo query)
        {
            var datas = (ICollection<DanhSachPhieuXuatHoaChatChiTiet>)gridDataSource.Data;

            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<DanhSachPhieuXuatHoaChatChiTiet>("STT", p => ind++)
            };

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("[XN]Phiếu xuất hoá chất");
                    worksheet.DefaultRowHeight = 16;
                    worksheet.Column(1).Width = 13;
                    worksheet.Column(2).Width = 12;
                    worksheet.Column(3).Width = 13;
                    worksheet.Column(4).Width = 13;
                    worksheet.Column(5).Width = 18;
                    worksheet.Column(6).Width = 18;
                    worksheet.DefaultColWidth = 7;
                    worksheet.Row(8).Height = 24;
                    worksheet.Row(10).Height = 32;

                    var tuNgayDataTime = query.TuNgay;
                    var denNgayDataTime = query.DenNgay;
                    var duocPhamBenhVien = _duocPhamBenhVienRepository.GetById(query.DuocPhamId, x => x.Include(s => s.DuocPham).ThenInclude(c => c.DonViTinh));
                    var mayXetNghiem = _mayXetNghiemRepository.GetById(query.MayXetNghiemId);

                    using (var range = worksheet.Cells["A1:C1"])
                    {
                        range.Worksheet.Cells["A1:C1"].Merge = true;
                        range.Worksheet.Cells["A1:C1"].Value = "Phiếu xuất hóa chất, vật tư, chất chuẩn";
                        range.Worksheet.Cells["A1:C1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:C1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:C1"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A1:C1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:C1"].Style.Font.Italic = true;
                    }

                    using (var range = worksheet.Cells["E1:F1"])
                    {
                        range.Worksheet.Cells["E1:F1"].Merge = true;
                        range.Worksheet.Cells["E1:F1"].Value = "BM05.QTQL 5.3.2";
                        range.Worksheet.Cells["E1:F1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        range.Worksheet.Cells["E1:F1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["E1:F1"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["E1:F1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["E1:F1"].Style.Font.Italic = true;
                    }

                    using (var range = worksheet.Cells["A2:F2"])
                    {
                        range.Worksheet.Cells["A2:F2"].Merge = true;

                    }

                    using (var range = worksheet.Cells["A3:F3"])
                    {
                        range.Worksheet.Cells["A3:F3"].Merge = true;
                        range.Worksheet.Cells["A3:F3"].Value = "PHIẾU XUẤT HÓA CHẤT, VẬT TƯ, CHẤT CHUẨN";
                        range.Worksheet.Cells["A3:F3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:F3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:F3"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A3:F3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:F3"].Style.Font.Bold = true;
                    }

                  
                    using (var range = worksheet.Cells["A4:F4"])
                    {
                        range.Worksheet.Cells["A4:F4"].Merge = true;
                        range.Worksheet.Cells["A4:F4"].Value = $"Từ {tuNgayDataTime.Hour} giờ {tuNgayDataTime.Minute} phút ngày {tuNgayDataTime.Day} tháng  {tuNgayDataTime.Month} năm {tuNgayDataTime.Year} đến {denNgayDataTime.Hour} giờ {denNgayDataTime.Minute} phút ngày{denNgayDataTime.Day} tháng {denNgayDataTime.Month} năm {denNgayDataTime.Year}";
                        range.Worksheet.Cells["A4:F4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:F4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A4:F4"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A4:F4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:F4"].Style.Font.Italic = true;
                    }

                    using (var range = worksheet.Cells["A5:F5"])
                    {
                        range.Worksheet.Cells["A5:F5"].Merge = true;
                        range.Worksheet.Cells["A5:F5"].Value = $"Tên hóa chất/chất chuẩn/vật tư: {duocPhamBenhVien.DuocPham?.Ten}          Mã hàng: { duocPhamBenhVien.Ma }";
                        range.Worksheet.Cells["A5:F5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A5:F5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A5:F5"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A5:F5"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["A6:F6"])
                    {
                        range.Worksheet.Cells["A6:F6"].Merge = true;
                        range.Worksheet.Cells["A6:F6"].Value = $"Đơn vị tính: {duocPhamBenhVien.DuocPham?.DonViTinh?.Ten}";
                        range.Worksheet.Cells["A6:F6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A6:F6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A6:F6"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A6:F6"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["A7:F7"])
                    {
                        range.Worksheet.Cells["A7:F7"].Merge = true;
                        range.Worksheet.Cells["A7:F7"].Value = $"Sử dụng cho xét nghiệm/thiết bị: {mayXetNghiem.Ten}";
                        range.Worksheet.Cells["A7:F7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A7:F7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A7:F7"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A7:F7"].Style.Font.Color.SetColor(Color.Black);
                    }

                    var duoi0Do = $"{(duocPhamBenhVien.LoaiDieuKienBaoQuanDuocPham == Enums.LoaiDieuKienBaoQuanDuocPham.Duoi0Do ? "☑" : "☐")} {Enums.LoaiDieuKienBaoQuanDuocPham.Duoi0Do.GetDescription()}";
                    var tu2Den8Do = $"{(duocPhamBenhVien.LoaiDieuKienBaoQuanDuocPham == Enums.LoaiDieuKienBaoQuanDuocPham.Tu2Den8Do ? "☑" : "☐")} {Enums.LoaiDieuKienBaoQuanDuocPham.Tu2Den8Do.GetDescription()}";
                    var tu15Den25Do = $"{(duocPhamBenhVien.LoaiDieuKienBaoQuanDuocPham == Enums.LoaiDieuKienBaoQuanDuocPham.Tu15Den25Do ? "☑" : "☐")} {Enums.LoaiDieuKienBaoQuanDuocPham.Tu15Den25Do.GetDescription()}";
                    var khac = $"{(duocPhamBenhVien.LoaiDieuKienBaoQuanDuocPham == Enums.LoaiDieuKienBaoQuanDuocPham.Khac ? "☑" : "☐")} {Enums.LoaiDieuKienBaoQuanDuocPham.Khac.GetDescription()}{(duocPhamBenhVien.LoaiDieuKienBaoQuanDuocPham == Enums.LoaiDieuKienBaoQuanDuocPham.Khac ? (": " + duocPhamBenhVien.ThongTinDieuKienBaoQuanDuocPham) : "")}";


                    using (var range = worksheet.Cells["A8:F8"])
                    {
                        range.Worksheet.Cells["A8:F8"].Merge = true;
                        range.Worksheet.Cells["A8:F8"].Value = $"Điều kiện bảo quản: {duoi0Do}      {tu2Den8Do}      {tu15Den25Do}      {khac}";
                        range.Worksheet.Cells["A8:F8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A8:F8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A8:F8"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A8:F8"].Style.Font.Color.SetColor(Color.Black);
                    }

                    var indexHeader = 10;
                    using (var range = worksheet.Cells["A" + indexHeader + ":F" + indexHeader])
                    {
                        range.Worksheet.Cells["A" + indexHeader + ":F" + indexHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A" + indexHeader + ":F" + indexHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A" + indexHeader + ":F" + indexHeader].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A" + indexHeader + ":F" + indexHeader].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A" + indexHeader + ":F" + indexHeader].Style.Font.Bold = true;
                        range.Worksheet.Cells["A" + indexHeader + ":F" + indexHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A" + indexHeader + ":A" + indexHeader].Merge = true;
                        range.Worksheet.Cells["A" + indexHeader + ":A" + indexHeader].Value = "Ngày xuất";
                        range.Worksheet.Cells["A" + indexHeader + ":A" + indexHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A" + indexHeader + ":A" + indexHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["B" + indexHeader + ":B" + indexHeader].Merge = true;
                        range.Worksheet.Cells["B" + indexHeader + ":B" + indexHeader].Value = "Lô";
                        range.Worksheet.Cells["B" + indexHeader + ":B" + indexHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["B" + indexHeader + ":B" + indexHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["C" + indexHeader + ":C" + indexHeader].Merge = true;
                        range.Worksheet.Cells["C" + indexHeader + ":C" + indexHeader].Value = "Hạn dùng";
                        range.Worksheet.Cells["C" + indexHeader + ":C" + indexHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["C" + indexHeader + ":C" + indexHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["D" + indexHeader + ":D" + indexHeader].Merge = true;
                        range.Worksheet.Cells["D" + indexHeader + ":D" + indexHeader].Style.WrapText = true;
                        range.Worksheet.Cells["D" + indexHeader + ":D" + indexHeader].Value = "Lượng xuất (lọ/hộp)";
                        range.Worksheet.Cells["D" + indexHeader + ":D" + indexHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["D" + indexHeader + ":D" + indexHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["E" + indexHeader + ":E" + indexHeader].Merge = true;
                        range.Worksheet.Cells["E" + indexHeader + ":E" + indexHeader].Value = "Người xuất";
                        range.Worksheet.Cells["E" + indexHeader + ":E" + indexHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["E" + indexHeader + ":E" + indexHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["F" + indexHeader + ":F" + indexHeader].Merge = true;
                        range.Worksheet.Cells["F" + indexHeader + ":F" + indexHeader].Value = "Người nhận";
                        range.Worksheet.Cells["F" + indexHeader + ":F" + indexHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["F" + indexHeader + ":F" + indexHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }

                    var manager = new PropertyManager<DanhSachPhieuXuatHoaChatChiTiet>(requestProperties);
                    int index = indexHeader + 1;

                    var stt = 1;

                    if (datas.Any())
                    {
                        foreach (var item in datas)
                        {
                            using (var range = worksheet.Cells["A" + index + ":F" + index])
                            {
                                range.Worksheet.Cells["A" + index + ":F" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                range.Worksheet.Cells["A" + index + ":F" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                range.Worksheet.Cells["A" + index + ":F" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["A" + index + ":F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["A" + index].Value = item.NgayXuatDisplay;

                                worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["B" + index].Value = item.SoLo;

                                worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["C" + index].Value = item.HanDungDisplay;

                                worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["D" + index].Value = item.LuongXuat;

                                worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["E" + index].Value = item.NguoiXuat;

                                worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["F" + index].Value = item.NguoiNhan;

                                index++;
                                stt++;
                            }
                        }
                    }
                    index = index + 1;
                    using (var range = worksheet.Cells["A" + index + ":B" + index])
                    {
                        range.Worksheet.Cells["A" + index + ":B" + index].Merge = true;
                        range.Worksheet.Cells["A" + index + ":B" + index].Value = $"Lần BH/SX: 01/00";
                        range.Worksheet.Cells["A" + index + ":B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A" + index + ":B" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":B" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A" + index + ":B" + index].Style.Font.Color.SetColor(Color.Black);
                    }
                    using (var range = worksheet.Cells["C" + index + ":D" + index])
                    {
                        range.Worksheet.Cells["C" + index + ":D" + index].Merge = true;
                        range.Worksheet.Cells["C" + index + ":D" + index].Value = $"Ngày ban hành: 01/01/2022";
                        range.Worksheet.Cells["C" + index + ":D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["C" + index + ":D" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["C" + index + ":D" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["C" + index + ":D" + index].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["F" + index + ":F" + index])
                    {
                        range.Worksheet.Cells["F" + index + ":F" + index].Merge = true;
                        range.Worksheet.Cells["F" + index + ":F" + index].Value = $"Trang 1/1";
                        range.Worksheet.Cells["F" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        range.Worksheet.Cells["F" + index + ":F" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["F" + index + ":F" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["F" + index + ":F" + index].Style.Font.Color.SetColor(Color.Black);
                    }

                    xlPackage.Save();
                }
                return stream.ToArray();

            }
        }
    }
}
