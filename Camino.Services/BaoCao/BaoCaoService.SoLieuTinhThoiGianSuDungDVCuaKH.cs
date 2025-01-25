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
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public async Task<GridDataSource> GetDataBaoCaoSoLieuTinhThoiGianSuDungDVCuaKHForGridAsync(BaoCaoSoLieuTinhThoiGianSuDungDVCuaKHQueryInfo queryInfo)
        {
            var query = _yeuCauTiepNhanRepository.TableNoTracking
                .Include(x => x.BenhNhan)
                .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauKhamBenhDonThuocs)
                .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.PhienXetNghiemChiTiets)
                .Where(x => x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                            && x.LoaiYeuCauTiepNhan != Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru
                            && x.ThoiDiemTiepNhan >= queryInfo.FromDate
                            && x.ThoiDiemTiepNhan <= queryInfo.ToDate)
                .OrderBy(x => x.ThoiDiemTiepNhan).ToList();

            var result = query
            .Select(item => new BaoCaoSoLieuTinhThoiGianSuDungDVCuaKHGridVo()
            {
                Id = item.Id,
                MaYeuCauTiepNhan = item.MaYeuCauTiepNhan,
                MaBN = item.BenhNhan.MaBN,
                HoTenKH = item.HoTen,
                TuNgay = queryInfo.FromDate,
                DenNgay = queryInfo.ToDate,


                ThoiDiemTN = item.ThoiDiemTiepNhan,
                ThoiDiemBSKham = item.YeuCauKhamBenhs
                    .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham 
                                && x.ThoiDiemThucHien != null)
                    .OrderBy(x => x.ThoiDiemThucHien).Select(x => x.ThoiDiemThucHien)
                    .FirstOrDefault(),
                ThoiDiemRaChiDinh = item.YeuCauDichVuKyThuats
                    .Any(x => x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && (x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh
                                    || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang
                                    || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem))
                        ? item.YeuCauDichVuKyThuats
                            .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                        && (x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh
                                            || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang
                                            || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem))
                            .OrderBy(x => x.ThoiDiemChiDinh)
                            .Select(x => x.ThoiDiemChiDinh)
                            .FirstOrDefault()
                        : (DateTime?)null,
                ThoiDiemLayMauXN = item.YeuCauDichVuKyThuats
                    .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                && x.PhienXetNghiemChiTiets.Any(a => a.ThoiDiemNhanMau != null))
                    .SelectMany(x => x.PhienXetNghiemChiTiets)
                    .OrderBy(x => x.ThoiDiemNhanMau)
                    .Select(x => x.ThoiDiemNhanMau)
                    .FirstOrDefault(),
                ThoiDiemTraKetQuaXN = item.YeuCauDichVuKyThuats
                    .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                && x.PhienXetNghiemChiTiets.Any(a => a.ThoiDiemKetLuan != null))
                    .SelectMany(x => x.PhienXetNghiemChiTiets)
                    .OrderBy(x => x.ThoiDiemKetLuan)
                    .Select(x => x.ThoiDiemKetLuan)
                    .FirstOrDefault(),
                ThoiDiemThucHienCLS = item.YeuCauDichVuKyThuats
                    .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && (x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh
                                    || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang))
                    .OrderBy(x => x.ThoiDiemHoanThanh)
                    .Select(x => x.ThoiDiemHoanThanh)
                    .FirstOrDefault(),
                ThoiDiemBacSiKetLuan = item.YeuCauKhamBenhs
                    .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham
                                && x.ThoiDiemHoanThanh != null)
                    .OrderBy(x => x.ThoiDiemHoanThanh).Select(x => x.ThoiDiemHoanThanh)
                    .FirstOrDefault(),
                ThoiDiemBacSiKeDonThuoc = item.YeuCauKhamBenhs
                    .Any(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham
                                && x.YeuCauKhamBenhDonThuocs.Any()) 
                        ? item.YeuCauKhamBenhs
                            .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham
                                        && x.YeuCauKhamBenhDonThuocs.Any())
                            .SelectMany(x => x.YeuCauKhamBenhDonThuocs)
                            .OrderBy(x => x.ThoiDiemKeDon).Select(x => x.ThoiDiemKeDon)
                            .FirstOrDefault() 
                        : (DateTime?)null
            }).ToList();

            return new GridDataSource { Data = result.ToArray(), TotalRowCount = result.Count() };
        }

        public async Task<GridDataSource> GetDataTotalPageBaoCaoSoLieuTinhThoiGianSuDungDVCuaKHForGridAsync(BaoCaoSoLieuTinhThoiGianSuDungDVCuaKHQueryInfo queryInfo)
        {
            var query = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(x => x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                            && x.LoaiYeuCauTiepNhan != Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru
                            && x.ThoiDiemTiepNhan >= queryInfo.FromDate
                            && x.ThoiDiemTiepNhan <= queryInfo.ToDate);
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public virtual byte[] ExportBaoCaoSoLieuTinhThoiGianSuDungDVCuaKHGridVo(GridDataSource gridDataSource, BaoCaoSoLieuTinhThoiGianSuDungDVCuaKHQueryInfo query)
        {
            var datas = (ICollection<BaoCaoSoLieuTinhThoiGianSuDungDVCuaKHGridVo>)gridDataSource.Data;
            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoSoLieuTinhThoiGianSuDungDVCuaKHGridVo>("STT", p => ind++)
            };
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BẢNG SỐ LIỆU TÍNH THỜI GIAN SỬ DỤNG DV CỦA KHÁCH HÀNG");

                    // set row
                    worksheet.DefaultRowHeight = 16;

                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 30;
                    worksheet.Column(3).Width = 15;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 20;
                    worksheet.Column(6).Width = 20;
                    worksheet.Column(7).Width = 20;
                    worksheet.Column(8).Width = 20;
                    worksheet.Column(9).Width = 20;
                    worksheet.Column(10).Width = 20;
                    worksheet.Column(11).Width = 20;

                    worksheet.Row(6).Height = 50;

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
                    using (var range = worksheet.Cells["A3:K3"])
                    {
                        range.Worksheet.Cells["A3:K3"].Merge = true;
                        range.Worksheet.Cells["A3:K3"].Value = "BẢNG SỐ LIỆU TÍNH THỜI GIAN SỬ DỤNG DV CỦA KHÁCH HÀNG";
                        range.Worksheet.Cells["A3:K3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:K3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:K3"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
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
                        range.Worksheet.Cells["A4:K4"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A4:K4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:K4"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A6:K6"])
                    {
                        range.Worksheet.Cells["A6:K6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A6:K6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A6:K6"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A6:K6"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A6:K6"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A6:K6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A6"].Value = "STT";

                        range.Worksheet.Cells["B6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black); //del ?
                        range.Worksheet.Cells["B6"].Value = "Họ tên khách hàng";

                        range.Worksheet.Cells["C6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C6"].Value = "Mã người bệnh";

                        range.Worksheet.Cells["D6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D6"].Style.WrapText = true;
                        range.Worksheet.Cells["D6"].Value = "Thời điểm \r\n tiếp nhận tại \r\n quầy LT";

                        range.Worksheet.Cells["E6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E6"].Style.WrapText = true;
                        range.Worksheet.Cells["E6"].Value = "Thời điểm \r\n BS mở hồ sơ khám";

                        range.Worksheet.Cells["F6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F6"].Style.WrapText = true;
                        range.Worksheet.Cells["F6"].Value = "Thời điểm \r\n BS ra chỉ định CLS";

                        range.Worksheet.Cells["G6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G6"].Style.WrapText = true;
                        range.Worksheet.Cells["G6"].Value = "Thời điểm \r\n KH lấy mẫu XN";

                        range.Worksheet.Cells["H6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H6"].Style.WrapText = true;
                        range.Worksheet.Cells["H6"].Value = "Thời điểm \r\n  trả KQXN";

                        range.Worksheet.Cells["I6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I6"].Style.WrapText = true;
                        range.Worksheet.Cells["I6"].Value = "Thời điểm \r\n KH thực hiện CĐHA";

                        range.Worksheet.Cells["J6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J6"].Style.WrapText = true;
                        range.Worksheet.Cells["J6"].Value = "Thời điểm \r\n BS kết luận";

                        range.Worksheet.Cells["K6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K6"].Style.WrapText = true;
                        range.Worksheet.Cells["K6"].Value = "Thời điểm \r\n BS kê đơn thuốc ";
                    }

                    //write data from line 9
                    bool singleDay = false;
                    if (query.ToDate.Year - query.FromDate.Year <= 0 && query.ToDate.Month - query.FromDate.Month <= 0 && query.ToDate.Day - query.FromDate.Day <= 0)
                    {
                        singleDay = true;
                    }
                    int index = 7;
                    int stt = 1;
                    if (datas.Any())
                    {
                        foreach (var item in datas)
                        {

                            // format border, font chữ,....
                            worksheet.Cells["A" + index + ":K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                            worksheet.Cells["A" + index + ":K" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            worksheet.Cells["A" + index + ":K" + index].Style.Font.Color.SetColor(Color.Black);
                            worksheet.Cells["A" + index + ":K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + index + ":K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

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
                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Row(index).Height = 20.5;

                            worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + index].Value = stt;

                            worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["B" + index].Value = item.HoTenKH;

                            worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["C" + index].Value = item.MaYeuCauTiepNhan;

                            worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["D" + index].Value = item.ThoiDiemTNStr; //singleDay ? item.ThoiDiemTN.ToString("HH:mm") : item.ThoiDiemTNStr;

                            worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["E" + index].Value = item.ThoiDiemBSKhamStr; // singleDay ? item.ThoiDiemBSKham?.ToString("HH:mm") : item.ThoiDiemBSKhamStr;

                            worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["F" + index].Value = item.ThoiDiemRaChiDinhStr; // singleDay ? item.ThoiDiemRaChiDinh?.ToString("HH:mm") : item.ThoiDiemRaChiDinhStr;

                            worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["G" + index].Value = item.ThoiDiemLayMauXNStr; // singleDay ? item.ThoiDiemLayMauXN?.ToString("HH:mm") : item.ThoiDiemLayMauXNStr;

                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["H" + index].Value = item.ThoiDiemTraKQXNStr; // singleDay ? item.ThoiDiemTraKetQuaXN?.ToString("HH:mm") : item.ThoiDiemTraKQXNStr;

                            worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["I" + index].Value = item.ThoiDiemCDHAStr; //singleDay ? item.ThoiDiemThucHienCLS?.ToString("HH:mm") : item.ThoiDiemCDHAStr;

                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["J" + index].Value = item.ThoiDiemKLStr; //singleDay ? item.ThoiDiemBacSiKetLuan?.ToString("HH:mm") : item.ThoiDiemKLStr;

                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["K" + index].Value = item.ThoiDiemKeDonStr; //singleDay ? item.ThoiDiemBacSiKeDonThuoc?.ToString("HH:mm") : item.ThoiDiemKeDonStr;

                            stt++;
                            index++;
                        }

                    }

                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }
    }
}
