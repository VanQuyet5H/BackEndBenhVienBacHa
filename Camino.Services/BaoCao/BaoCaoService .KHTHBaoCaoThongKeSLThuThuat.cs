using Camino.Core.Data;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Services.ExportImport.Help;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public async Task<List<LookupItemVo>> GetTatCaKhoaThongKeSoLuongThuThuat
            (DropDownListRequestModel queryInfo)
        {
            
            var result = _KhoaPhongRepository.TableNoTracking
               .Select(s => new LookupItemVo
               {
                   KeyId = s.Id,
                   DisplayName = s.Ten,
               })
               .ApplyLike(queryInfo.Query, o => o.DisplayName)
               .Take(queryInfo.Take)
               .ToList();

            result.Insert(0, new LookupItemVo { KeyId = 0, DisplayName = "Tất cả" });

            return result.ToList();
        }

        public async Task<List<LookupItemVo>> GetTatCaPhongTheoKhoaThongKeSoLuongThuThuat(DropDownListRequestModel queryInfo, long khoaId)
        {
            var allKhos = new List<LookupItemVo>()
            {
                new LookupItemVo {KeyId = 0 , DisplayName = "Tất cả" }
            };


            var result = _phongBenhVienRepository.TableNoTracking.Where(c => khoaId == 0 || c.KhoaPhongId == khoaId)
               .Select(s => new LookupItemVo
               {
                   KeyId = s.Id,
                   DisplayName = s.Ten,
               }).ApplyLike(queryInfo.Query, o => o.DisplayName)
               .Take(queryInfo.Take);

            allKhos.AddRange(result);

            return allKhos;
        }


        public async Task<GridDataSource> GetDataKHTHBaoCaoThongKeSLThuThuatForGridAsync(KHTHBaoCaoThongKeSLThuThuatQueryInfo queryInfo, bool exportExcel = false)
        {
            var thongTinPhongBenhViens = _phongBenhVienRepository.TableNoTracking.Select(o => new { o.Id, TenPhong = o.Ten, o.KhoaPhongId, TenKhoa = o.KhoaPhong.Ten }).ToList();
            var loaiThuThuats = new List<string> { LoaiPhauThuatThuThuat.ThuThuatLoai1.GetDescription(), LoaiPhauThuatThuThuat.ThuThuatLoai2.GetDescription(), LoaiPhauThuatThuThuat.ThuThuatLoai3.GetDescription(), LoaiPhauThuatThuThuat.ThuThuatLoaiDacBiet.GetDescription() };
            var dvktbvs = _dichVuKyThuatBenhVienRepository.TableNoTracking
                .Where(o => loaiThuThuats.Contains(o.LoaiPhauThuatThuThuat))
                .Select(o => new { o.Id, o.Ten, o.LoaiPhauThuatThuThuat, ChuyenKhoaChuyenNganh = o.ChuyenKhoaChuyenNganhId != null ? o.ChuyenKhoaChuyenNganh.Ten : "" , o.ThongTu })
                .ToList();

            var dvktbvIds = dvktbvs.Select(o => o.Id).ToList();

            var slThuThuatQuery = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(o => o.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien && o.NoiThucHienId != null && dvktbvIds.Contains(o.DichVuKyThuatBenhVienId)
                            && ((o.ThoiDiemHoanThanh != null && queryInfo.FromDate <= o.ThoiDiemHoanThanh && o.ThoiDiemHoanThanh < queryInfo.ToDate)
                                || (o.ThoiDiemHoanThanh == null && o.ThoiDiemThucHien != null && queryInfo.FromDate <= o.ThoiDiemThucHien && o.ThoiDiemThucHien < queryInfo.ToDate)));

            if(queryInfo.PhongBenhVienId != 0)
            {
                slThuThuatQuery = slThuThuatQuery.Where(o => o.NoiThucHienId == queryInfo.PhongBenhVienId);
            }
            else if(queryInfo.KhoaId != 0)
            {
                var phongBenhVienIds = thongTinPhongBenhViens.Where(o=>o.KhoaPhongId == queryInfo.KhoaId).Select(o=>o.Id).ToList();
                slThuThuatQuery = slThuThuatQuery.Where(o => phongBenhVienIds.Contains(o.NoiThucHienId.GetValueOrDefault()));
            }
            var slThuThuatData = slThuThuatQuery.Select(o => new {o.Id, o.DichVuKyThuatBenhVienId, o.NoiThucHienId, o.SoLan}).ToList();

            var slThuThuatChiTiets = new List<KHTHBaoCaoThongKeSLThuThuatChiTiet>();
            foreach(var thuThuat in slThuThuatData)
            {
                var thongTinPhong = thongTinPhongBenhViens.First(o => o.Id == thuThuat.NoiThucHienId);
                var dvktbv = dvktbvs.First(o => o.Id == thuThuat.DichVuKyThuatBenhVienId);
                slThuThuatChiTiets.Add(new KHTHBaoCaoThongKeSLThuThuatChiTiet
                {
                    Id = thuThuat.Id,
                    SoLan = thuThuat.SoLan,
                    KhoaPhongId = thongTinPhong.KhoaPhongId,
                    TenKhoaPhong = thongTinPhong.TenKhoa,
                    PhongBenhVienId = thongTinPhong.Id,
                    TenPhongBenhVien = thongTinPhong.TenPhong,
                    DichVuKyThuatBenhVienId = thuThuat.DichVuKyThuatBenhVienId,
                    TenDichVuThuThuat = dvktbv.Ten,
                    ThongTuKhoa = dvktbv.ChuyenKhoaChuyenNganh,
                    ThongTuSo = dvktbv.ThongTu,
                    LoaiThuThuat = dvktbv.LoaiPhauThuatThuThuat
                });
            }
            var returnData = new List<KHTHBaoCaoThongKeSLThuThuat>();
            var groupKhoas = slThuThuatChiTiets.GroupBy(o => o.KhoaPhongId).OrderBy(o => o.Key);
            foreach (var groupKhoa in groupKhoas)
            {
                var itemKhoa = new KHTHBaoCaoThongKeSLThuThuat 
                { 
                    TenKhoaPhong = groupKhoa.First().TenKhoaPhong, 
                    DanhMucThuThuat="Tổng cộng chung", 
                    ToDam = true, 
                    Center = true,
                    TongSoLuongThuThuat1 = 0,
                    TongSoLuongThuThuat2 = 0,
                    TongSoLuongThuThuat3 = 0,
                    TongSoLuongThuThuatDacBiet = 0,
                    Phien = 0,
                    CapCuu = 0
                };
                returnData.Add(itemKhoa);

                var groupPhongs = groupKhoa.GroupBy(o => o.PhongBenhVienId).OrderBy(o => o.Key);
                foreach(var groupPhong in groupPhongs)
                {
                    var itemPhong = new KHTHBaoCaoThongKeSLThuThuat 
                    { 
                        DanhMucThuThuat = "Tổng cộng", 
                        ToDam = true, 
                        Center = true, 
                        GachChan = true,
                        TongSoLuongThuThuat1 = 0,
                        TongSoLuongThuThuat2 = 0,
                        TongSoLuongThuThuat3 = 0,
                        TongSoLuongThuThuatDacBiet = 0,
                        Phien = 0,
                        CapCuu = 0
                    };
                    var groupDichVus = groupPhong.GroupBy(o => new { o.DichVuKyThuatBenhVienId, o.TenDichVuThuThuat }).OrderBy(o => o.Key.TenDichVuThuThuat);                    
                    int i = 0;
                    foreach (var groupDichVu in groupDichVus)
                    {
                        var itemDichVu = new KHTHBaoCaoThongKeSLThuThuat 
                        { 
                            TenKhoaPhong = (i == 0 ? groupDichVu.First().TenPhongBenhVien : ""), 
                            DanhMucThuThuat = groupDichVu.Key.TenDichVuThuThuat,
                            ThongTuKhoa = groupDichVu.First().ThongTuKhoa,
                            ThongTuSo = groupDichVu.First().ThongTuSo
                        };
                        if(groupDichVu.First().LoaiThuThuat == LoaiPhauThuatThuThuat.ThuThuatLoai1.GetDescription())
                        {
                            itemDichVu.TongSoLuongThuThuat1 = groupDichVu.Sum(o=>o.SoLan);
                            itemPhong.TongSoLuongThuThuat1 = itemPhong.TongSoLuongThuThuat1.GetValueOrDefault() + itemDichVu.TongSoLuongThuThuat1;
                            itemKhoa.TongSoLuongThuThuat1 = itemKhoa.TongSoLuongThuThuat1.GetValueOrDefault() + itemDichVu.TongSoLuongThuThuat1;
                        }
                        else if (groupDichVu.First().LoaiThuThuat == LoaiPhauThuatThuThuat.ThuThuatLoai2.GetDescription())
                        {
                            itemDichVu.TongSoLuongThuThuat2 = groupDichVu.Sum(o => o.SoLan);
                            itemPhong.TongSoLuongThuThuat2 = itemPhong.TongSoLuongThuThuat2.GetValueOrDefault() + itemDichVu.TongSoLuongThuThuat2;
                            itemKhoa.TongSoLuongThuThuat2 = itemKhoa.TongSoLuongThuThuat2.GetValueOrDefault() + itemDichVu.TongSoLuongThuThuat2;
                        }
                        else if (groupDichVu.First().LoaiThuThuat == LoaiPhauThuatThuThuat.ThuThuatLoai3.GetDescription())
                        {
                            itemDichVu.TongSoLuongThuThuat3 = groupDichVu.Sum(o => o.SoLan);
                            itemPhong.TongSoLuongThuThuat3 = itemPhong.TongSoLuongThuThuat3.GetValueOrDefault() + itemDichVu.TongSoLuongThuThuat3;
                            itemKhoa.TongSoLuongThuThuat3 = itemKhoa.TongSoLuongThuThuat3.GetValueOrDefault() + itemDichVu.TongSoLuongThuThuat3;
                        }
                        else if (groupDichVu.First().LoaiThuThuat == LoaiPhauThuatThuThuat.ThuThuatLoaiDacBiet.GetDescription())
                        {
                            itemDichVu.TongSoLuongThuThuatDacBiet = groupDichVu.Sum(o => o.SoLan);
                            itemPhong.TongSoLuongThuThuatDacBiet = itemPhong.TongSoLuongThuThuatDacBiet.GetValueOrDefault() + itemDichVu.TongSoLuongThuThuatDacBiet;
                            itemKhoa.TongSoLuongThuThuatDacBiet = itemKhoa.TongSoLuongThuThuatDacBiet.GetValueOrDefault() + itemDichVu.TongSoLuongThuThuatDacBiet;
                        }
                        returnData.Add(itemDichVu);
                        i++;
                    }
                    returnData.Add(itemPhong);
                }
            }

            return new GridDataSource { Data = returnData.ToArray(), TotalRowCount = returnData.Count() };
        }

        public virtual byte[] ExportKHTHBaoCaoThongKeSLThuThuatGridVo(GridDataSource gridDataSource, KHTHBaoCaoThongKeSLThuThuatQueryInfo query)
        {
            var datas = (ICollection<KHTHBaoCaoThongKeSLThuThuat>)gridDataSource.Data;
            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<KHTHBaoCaoThongKeSLThuThuat>("STT", p => ind++)
            };
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("[KHTH] Báo cáo thống kê SL thủ thuật");

                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 25;
                    worksheet.Column(2).Width = 46;
                    worksheet.Column(3).Width = 25;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 20;
                    worksheet.Column(6).Width = 20;
                    worksheet.Column(7).Width = 20;
                    worksheet.Column(8).Width = 20;
                    worksheet.Column(9).Width = 20;
                    worksheet.Column(10).Width = 20;
                    worksheet.Column(11).Width = 20;
                    worksheet.Column(12).Width = 20;

                    worksheet.DefaultColWidth = 7;

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

                    using (var range = worksheet.Cells["A3:K3"])
                    {
                        range.Worksheet.Cells["A3:K3"].Merge = true;
                        range.Worksheet.Cells["A3:K3"].Value = "BÁO CÁO THỐNG  KÊ SỐ LƯỢNG THỦ THUẬT";
                        range.Worksheet.Cells["A3:K3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:K3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:K3"].Style.Font.SetFromFont(new Font("Times New Roman", 18));
                        range.Worksheet.Cells["A3:K3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:K3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A4:K4"])
                    {
                        range.Worksheet.Cells["A4:K4"].Merge = true;
                        range.Worksheet.Cells["A4:K4"].Value = "Từ ngày: " + query.FromDate.FormatNgayGioTimKiemTrenBaoCao()
                                                         + " - đến ngày: " + query.ToDate.FormatNgayGioTimKiemTrenBaoCao();
                        range.Worksheet.Cells["A4:K4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:K4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:K4"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A4:K4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:K4"].Style.Font.Bold = true;
                    }

                    var tenKhoa = _KhoaPhongRepository.TableNoTracking.Where(c => c.Id == query.KhoaId).Select(c => c.Ten).FirstOrDefault();
                    using (var range = worksheet.Cells["A5:K5"])
                    {
                        range.Worksheet.Cells["A5:K5"].Merge = true;
                        range.Worksheet.Cells["A5:K5"].Value = "Khoa thực hiện: " + (tenKhoa ?? "Tất cả");
                        range.Worksheet.Cells["A5:K5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A5:K5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A5:K5"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A5:K5"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["A7:K8"])
                    {
                        range.Worksheet.Cells["A7:K8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A7:K8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A7:K8"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A7:K8"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A7:K8"].Style.Font.Bold = true;


                        range.Worksheet.Cells["A7:A8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A7:A8"].Merge = true;
                        range.Worksheet.Cells["A7:A8"].Value = "Tên khoa phòng";

                        range.Worksheet.Cells["B7:B8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B7:B8"].Merge = true;
                        range.Worksheet.Cells["B7:B8"].Value = "Danh mục thủ thuật";

                        range.Worksheet.Cells["C7:D7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C7:D7"].Merge = true;
                        range.Worksheet.Cells["C7:D7"].Value = "Thông tư 50";

                        range.Worksheet.Cells["C8:C8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C8:C8"].Value = "Khoa";

                        range.Worksheet.Cells["D8:D8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D8:D8"].Value = "Số";

                        range.Worksheet.Cells["E7:E8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E7:E8"].Merge = true;
                        range.Worksheet.Cells["E7:E8"].Value = "TS";

                        range.Worksheet.Cells["F7:I7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F7:I7"].Merge = true;
                        range.Worksheet.Cells["F7:I7"].Value = "Loại thủ thuật";

                        range.Worksheet.Cells["F8:F8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F8:F8"].Value = "Đặc biệt";

                        range.Worksheet.Cells["G8:G8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G8:G8"].Value = "1";

                        range.Worksheet.Cells["H8:H8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H8:H8"].Value = "2";

                        range.Worksheet.Cells["I8:I8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I8:I8"].Value = "3";

                        range.Worksheet.Cells["J7:K7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J7:K7"].Merge = true;
                        range.Worksheet.Cells["J7:K7"].Value = "Trong đó";

                        range.Worksheet.Cells["J8:J8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J8:J8"].Value = "Phiên";

                        range.Worksheet.Cells["K8:K8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K8:K8"].Value = "Cấp cứu";
                    }

                    int index = 9;
                    var stt = 1;

                    if (datas.Any())
                    {
                        foreach (var item in datas)
                        {
                            worksheet.Cells["A" + index + ":K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                            worksheet.Cells["A" + index + ":K" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                            worksheet.Cells["A" + index + ":K" + index].Style.Font.Color.SetColor(Color.Black);
                            worksheet.Cells["A" + index + ":K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + index + ":K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                            worksheet.Row(index).Height = 20.5;

                            worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Cells["A" + index].Style.Font.Bold = item.ToDam;
                            worksheet.Cells["A" + index].Style.Font.UnderLine = item.GachChan;
                            worksheet.Cells["A" + index].Value = item.TenKhoaPhong;

                            worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["B" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Cells["B" + index].Style.HorizontalAlignment = item.Center ? ExcelHorizontalAlignment.Center : ExcelHorizontalAlignment.Left;
                            worksheet.Cells["B" + index].Style.Font.Bold = item.ToDam;
                            worksheet.Cells["B" + index].Style.Font.UnderLine = item.GachChan;
                            worksheet.Cells["B" + index].Value = item.DanhMucThuThuat;

                            worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["C" + index].Style.Font.Bold = item.ToDam;
                            worksheet.Cells["C" + index].Style.Font.UnderLine = item.GachChan;
                            worksheet.Cells["C" + index].Value = item.ThongTuKhoa;

                            worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["D" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["D" + index].Value = item.ThongTuSo;

                            worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["E" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Cells["E" + index].Style.Font.Bold = item.ToDam;
                            worksheet.Cells["E" + index].Style.Font.UnderLine = item.GachChan;
                            worksheet.Cells["E" + index].Value = item.TongSo;

                            worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["F" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Cells["F" + index].Style.Font.Bold = item.ToDam;
                            worksheet.Cells["F" + index].Style.Font.UnderLine = item.GachChan;
                            worksheet.Cells["F" + index].Value = item.TongSoLuongThuThuatDacBiet;

                            worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["G" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Cells["G" + index].Style.Font.Bold = item.ToDam;
                            worksheet.Cells["G" + index].Style.Font.UnderLine = item.GachChan;
                            worksheet.Cells["G" + index].Value = item.TongSoLuongThuThuat1;

                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["H" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Cells["H" + index].Style.Font.Bold = item.ToDam;
                            worksheet.Cells["H" + index].Style.Font.UnderLine = item.GachChan;
                            worksheet.Cells["H" + index].Value = item.TongSoLuongThuThuat2;

                            worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["I" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Cells["I" + index].Style.Font.Bold = item.ToDam;
                            worksheet.Cells["I" + index].Style.Font.UnderLine = item.GachChan;
                            worksheet.Cells["I" + index].Value = item.TongSoLuongThuThuat3;

                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Cells["J" + index].Style.Font.Bold = item.ToDam;
                            worksheet.Cells["J" + index].Style.Font.UnderLine = item.GachChan;
                            worksheet.Cells["J" + index].Value = item.Phien;

                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Cells["K" + index].Style.Font.Bold = item.ToDam;
                            worksheet.Cells["K" + index].Style.Font.UnderLine = item.GachChan;
                            worksheet.Cells["K" + index].Value = item.CapCuu;

                            stt++;
                            index++;
                        }

                    }

                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }

        //private List<KHTHBaoCaoThongKeSLThuThuat> KHTHBaoCaoThongKeSLThuThuats()
        //{
        //    var datas = new List<KHTHBaoCaoThongKeSLThuThuat>()
        //    {
        //       new  KHTHBaoCaoThongKeSLThuThuat {ToDam=true, TenKhoaPhong = "Khoa ngoại", DanhMucThuThuat="Không" , ThongTuKhoa = "" , ThongTuSo = null , TongSoLuongThuThuatDacBiet = 0, TongSoLuongThuThuat1 = 0, TongSoLuongThuThuat2 = 0, TongSoLuongThuThuat3 = 0, Phien = 0 ,CapCuu= 0},
        //       new  KHTHBaoCaoThongKeSLThuThuat {ToDam=true,Center = true, TenKhoaPhong = "Khoa khám bệnh", DanhMucThuThuat="Tổng cộng chung" , ThongTuKhoa = "" , ThongTuSo = null , TongSoLuongThuThuatDacBiet = 1, TongSoLuongThuThuat1 = 17, TongSoLuongThuThuat2 = 3, TongSoLuongThuThuat3 = 17, Phien = 0 ,CapCuu= 0},
        //       new  KHTHBaoCaoThongKeSLThuThuat {TenKhoaPhong = "Phòng cấp cứu 113", DanhMucThuThuat="Cắt chỉ vết thương" , ThongTuKhoa = "Nhi Khoa" , ThongTuSo = 3911 , TongSoLuongThuThuatDacBiet = 1,  TongSoLuongThuThuat3 = 3, Phien = 3 ,CapCuu= 0},
        //       new  KHTHBaoCaoThongKeSLThuThuat {TenKhoaPhong = "", DanhMucThuThuat="Chích rạch áp xe nhỏ" , ThongTuKhoa = "Nhi Khoa" , ThongTuSo = 3031 , TongSoLuongThuThuatDacBiet = 1,  TongSoLuongThuThuat3 = 3, Phien = 3 ,CapCuu= 0},
        //       new  KHTHBaoCaoThongKeSLThuThuat {TenKhoaPhong = "", DanhMucThuThuat="Đặt kim luồn" , ThongTuKhoa = "Nhi Khoa" , ThongTuSo = 1407 , TongSoLuongThuThuatDacBiet = 1,  TongSoLuongThuThuat3 = 3, Phien = 3 ,CapCuu= 0},
        //       new  KHTHBaoCaoThongKeSLThuThuat {TenKhoaPhong = "", DanhMucThuThuat="Ghi điện tim cấp cứu tại giường" , ThongTuKhoa = "HSCC-CĐ" , ThongTuSo = 2 , TongSoLuongThuThuatDacBiet = 1,  TongSoLuongThuThuat3 = 3, Phien = 3 ,CapCuu= 0},
        //       new  KHTHBaoCaoThongKeSLThuThuat {ToDam=true, Center = true ,GachChan= true, TenKhoaPhong = "", DanhMucThuThuat="Tổng cộng" ,  TongSoLuongThuThuatDacBiet = 1,TongSoLuongThuThuat1 = 10,TongSoLuongThuThuat2 = 3,  TongSoLuongThuThuat3 = 17, Phien = 3 ,CapCuu= 0},
        //    };

        //    return datas;
        //}
    }
}