using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.BaoCaoLuuketQuaXeNghiemTrongNgay;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Services.ExportImport.Help;
using Camino.Services.Helpers;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;


namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public async Task<GridDataSource> GetDataBaoCaoBenhNhanLamXetNghiemForGridAsync(BaoCaoBenhNhanLamXetNghiemQueryInfo queryInfo, bool exportExcel = false)
        {
            var ketQuaXetNghiemQuery = _phienXetNghiemRepository.TableNoTracking
                .Where(o => o.PhienXetNghiemChiTiets.Any(ct =>
                    ct.ThoiDiemCoKetQua != null && ct.ThoiDiemCoKetQua >= queryInfo.FromDate &&
                    ct.ThoiDiemCoKetQua <= queryInfo.ToDate &&
                    (ct.KetQuaXetNghiemChiTiets.Any(kq => !string.IsNullOrEmpty(kq.GiaTriTuMay) || !string.IsNullOrEmpty(kq.GiaTriNhapTay)))));

            IQueryable<PhienXetNghiem> ketQuaXetNghiemDataQuery;
            if (exportExcel)
            {
                ketQuaXetNghiemDataQuery = ketQuaXetNghiemQuery.OrderBy(o => o.BarCodeId);
            }
            else
            {
                var ketQuaXetNghiemGroup = ketQuaXetNghiemQuery.Select(o => new { o.BarCodeId, o.Id }).GroupBy(o => o.BarCodeId).Skip(queryInfo.Skip).Take(queryInfo.Take).ToList();
                var phienXetNghiemIds = ketQuaXetNghiemGroup.SelectMany(o => o.Select(i => i.Id)).ToList();

                ketQuaXetNghiemDataQuery = _phienXetNghiemRepository.TableNoTracking.Where(o => phienXetNghiemIds.Contains(o.Id)).OrderBy(o => o.BarCodeId);
            }


            var ketQuaXetNghiemData = ketQuaXetNghiemDataQuery
                .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.BenhNhan).ToList();

            var gridData = ketQuaXetNghiemData.GroupBy(o => o.BarCodeId)
                .Select((o, i) => new BaoCaoBenhNhanLamXetNghiemGridVo
                {
                    STT = queryInfo.Skip + i + 1,
                    MaBarcode = o.Key,
                    MaBN = o.First().YeuCauTiepNhan.BenhNhan.MaBN,
                    HoTen = o.First().YeuCauTiepNhan.HoTen,
                    NamSinh = o.First().YeuCauTiepNhan.NamSinh?.ToString(),
                    GioiTinh = o.First().YeuCauTiepNhan.GioiTinh?.GetDescription(),
                    LoaiYeuCauTiepNhan = o.First().YeuCauTiepNhan.LoaiYeuCauTiepNhan,
                    MucHuongBHYT = o.First().YeuCauTiepNhan.CoBHYT == true ? o.First().YeuCauTiepNhan.BHYTMucHuong.GetValueOrDefault() : 0,
                    DiaChi = o.First().YeuCauTiepNhan.DiaChiDayDu
                }).ToArray();

            var totalRowCount = ketQuaXetNghiemQuery.GroupBy(o => o.BarCodeId).Count();
            return new GridDataSource { Data = gridData, TotalRowCount = totalRowCount };
        }

        public virtual byte[] ExportBaoCaoBenhNhanLamXetNghiem(GridDataSource gridDataSource, BaoCaoBenhNhanLamXetNghiemQueryInfo query)
        {
            var datas = (ICollection<BaoCaoBenhNhanLamXetNghiemGridVo>)gridDataSource.Data;
            int ind = 1;
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO NGƯỜI BỆNH LÀM XÉT NGHIỆM");

                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 15;
                    worksheet.Column(3).Width = 15;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 15;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 20;
                    worksheet.Column(8).Width = 80;
                    worksheet.DefaultColWidth = 7;

                    //SET img 
                    using (var range = worksheet.Cells["A1:C1"])
                    {
                        //                        var url = hostingName + "/assets/img/logo-bacha-full.png";
                        //                        WebClient wc = new WebClient();
                        //                        byte[] bytes = wc.DownloadData(url); // download file từ server
                        //                        MemoryStream ms = new MemoryStream(bytes); //
                        //                        Image img = Image.FromStream(ms); // chuyển đổi thành img
                        //                        ExcelPicture pic = range.Worksheet.Drawings.AddPicture("Logo", img);
                        //                        pic.SetPosition(0, 0, 0, 0);
                        //                        var height = 120; // chiều cao từ A1 đến A6
                        //                        var width = 510; // chiều rộng từ A1 đến D1
                        //                        pic.SetSize(width, height);
                        //                        range.Worksheet.Protection.IsProtected = false;
                        //                        range.Worksheet.Protection.AllowSelectLockedCells = false;
                        range.Worksheet.Cells["A1:C1"].Merge = true;
                        range.Worksheet.Cells["A1:C1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:C1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:C1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:C1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:C1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:C1"].Style.Font.Bold = true;
                    }

                    // SET title head cho bảng excel
                    using (var range = worksheet.Cells["A3:H3"])
                    {
                        range.Worksheet.Cells["A3:H3"].Merge = true;
                        range.Worksheet.Cells["A3:H3"].Value = "DANH SÁCH NGƯỜI BỆNH LÀM XÉT NGHIỆM";
                        range.Worksheet.Cells["A3:H3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:H3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:H3"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["A3:H3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:H3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A4:H4"])
                    {
                        range.Worksheet.Cells["A4:H4"].Merge = true;
                        range.Worksheet.Cells["A4:H4"].Value = "Từ ngày: " + query.FromDate.FormatNgayGioTimKiemTrenBaoCao()
                                                               + " - đến ngày: " + query.ToDate.FormatNgayGioTimKiemTrenBaoCao();
                        range.Worksheet.Cells["A4:H4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:H4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:H4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A4:H4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:H4"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A6:H6"])
                    {
                        range.Worksheet.Cells["A6:H6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A6:H6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A6:H6"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A6:H6"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A6:H6"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A6:H6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        worksheet.Cells["A6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["A6"].Value = "STT";

                        worksheet.Cells["B6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["B6"].Value = "MÃ BN";

                        worksheet.Cells["C6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["C6"].Value = "MÃ BARCODE";

                        worksheet.Cells["D6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["D6"].Value = "HỌ VÀ TÊN";

                        worksheet.Cells["E6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["E6"].Value = "GIỚI TÍNH";

                        worksheet.Cells["F6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["F6"].Value = "NĂM SINH";

                        worksheet.Cells["G6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["G6"].Value = "ĐỐI TƯỢNG";

                        worksheet.Cells["H6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["H6"].Value = "ĐỊA CHỈ";
                    }

                    int index = 7; // bắt đầu đổ data từ dòng 7

                    ///////Đổ data vào bảng excel
                    ///
                    var stt = 1;

                    if (datas.Any())
                    {
                        foreach (var item in datas)
                        {
                            using (var range = worksheet.Cells["A" + index + ":H" + index])
                            {
                                range.Worksheet.Cells["A" + index + ":H" + index].Style.VerticalAlignment =
                                    ExcelVerticalAlignment.Bottom;
                                range.Worksheet.Cells["A" + index + ":H" + index].Style.Font
                                    .SetFromFont(new Font("Times New Roman", 11));
                                range.Worksheet.Cells["A" + index + ":H" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["A" + index + ":H" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["A" + index + ":H" + index].Style.Font.Color
                                    .SetColor(Color.Black);


                                worksheet.Cells["A" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index].Value = stt;

                                worksheet.Cells["B" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["B" + index].Value = item.MaBN;

                                worksheet.Cells["C" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["C" + index].Value = item.MaBarcode;

                                worksheet.Cells["D" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["D" + index].Value = item.HoTen;

                                worksheet.Cells["E" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["E" + index].Value = item.GioiTinh;

                                worksheet.Cells["F" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["F" + index].Value = item.NamSinh;

                                worksheet.Cells["G" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["G" + index].Value = item.DoiTuong;

                                worksheet.Cells["H" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["H" + index].Value = item.DiaChi;
                                index++;
                            }
                            stt++;
                        }
                    }
                    index++;
                    worksheet.Cells["A" + index + ":H" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["A" + index + ":H" + index].Style.Font.Bold = true;
                    //value
                    worksheet.Cells["G" + index + ":H" + index].Value = "Người lập";
                    worksheet.Cells["G" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["G" + index + ":H" + index].Merge = true;
                    index++;

                    //value
                    worksheet.Cells["G" + index + ":H" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["G" + index + ":H" + index].Style.Font.Italic = true;
                    worksheet.Cells["G" + index + ":H" + index].Value = "(ký, ghi rõ họ tên)";
                    worksheet.Cells["G" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["G" + index + ":H" + index].Merge = true;
                    index++;
                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }
    }
}
