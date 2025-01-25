using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.ICDs;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo.Attributes;
using Camino.Core.Domain.ValueObject.GachNos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XetNghiem;
using Camino.Core.Domain.ValueObject.YeuCauTraThuocTuBenhNhan;
using Camino.Core.Helpers;
using Camino.Data;
using Camino.Services.ExportImport.Help;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;



namespace Camino.Services.ExportImport
{
    [ScopedDependency(ServiceType = typeof(IExcelService))]
    public class ExcelService : MasterFileService<ICD>, IExcelService
    {
        public ExcelService(IRepository<ICD> repository) : base(repository)
        {
        }

        public virtual byte[] ExportDetailedRevenueReportByDepartment(IList<BaoCaoChiTietDoanhThuTheoKhoaPhongGridVo> baoCaoChiTietDoanhThuTheoKhoaPhongs,
            DateTimeFilterVo dateTimeFilter)
        {
            int ind = 1;

            string messageRangeDateFilter = null;
            string messageRangeDateFilterKySoSanh = "Kỳ so sánh: ";

            if (dateTimeFilter != null && dateTimeFilter.RangeDateTimeFilter != null)
            {
                messageRangeDateFilter = "Từ " + dateTimeFilter.RangeDateTimeFilter.DateStart.Value.ApplyFormatTimeDate()
                                               + " đến " + dateTimeFilter.RangeDateTimeFilter.DateEnd.Value.ApplyFormatTimeDate();
            }

            if (dateTimeFilter != null && dateTimeFilter.RangeDateTimeSoSanh != null)
            {
                messageRangeDateFilterKySoSanh += "Từ " + dateTimeFilter.RangeDateTimeSoSanh.DateStart.Value.ApplyFormatTimeDate()
                                               + " đến " + dateTimeFilter.RangeDateTimeSoSanh.DateEnd.Value.ApplyFormatTimeDate();
            }

            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoChiTietDoanhThuTheoKhoaPhongGridVo>("STT", p => ind++),
                new PropertyByName<BaoCaoChiTietDoanhThuTheoKhoaPhongGridVo>("Khoa/Phòng", p => p.KhoaPhong)
            };

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BC05");

                    // set row
                    worksheet.Row(9).Height = 24.5;
                    worksheet.DefaultRowHeight = 16;

                    // set column
                    worksheet.Column(2).Width = 20;
                    worksheet.Column(3).Width = 20;
                    worksheet.Column(4).Width = 15;
                    worksheet.Column(5).Width = 15;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 15;
                    worksheet.Column(8).Width = 20;
                    worksheet.Column(9).Width = 15;
                    worksheet.Column(10).Width = 15;
                    worksheet.Column(11).Width = 15;
                    worksheet.Column(12).Width = 15;
                    worksheet.Column(13).Width = 20;
                    worksheet.DefaultColWidth = 7;

                    using (var range = worksheet.Cells["A3:M3"])
                    {
                        range.Worksheet.Cells["A3:M3"].Merge = true;
                        range.Worksheet.Cells["A3:M3"].Value = "Báo cáo chi tiết doanh thu theo khoa/phòng".ToUpper();
                        range.Worksheet.Cells["A3:M3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:M3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:M3"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A3:M3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:M3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A4:M4"])
                    {
                        range.Worksheet.Cells["A4:M4"].Merge = true;
                        range.Worksheet.Cells["A4:M4"].Value = messageRangeDateFilter;
                        range.Worksheet.Cells["A4:M4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:M4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:M4"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A4:M4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:M4"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A4:M4"].Style.Font.Italic = true;
                    }

                    using (var range = worksheet.Cells["A6:M9"])
                    {
                        range.Worksheet.Cells["A6:M9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A6:M9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A6:M9"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A6:M9"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A6:M9"].Style.Font.Bold = true;

                        range.Worksheet.Cells["A6:A8"].Merge = true;
                        range.Worksheet.Cells["A6:A8"].Value = "STT";
                        range.Worksheet.Cells["B6:B8"].Merge = true;
                        range.Worksheet.Cells["B6:B8"].Value = "Khoa/Phòng";
                        range.Worksheet.Cells["C6:C8"].Merge = true;
                        range.Worksheet.Cells["C6:C8"].Value = "Dịch vụ";
                        range.Worksheet.Cells["D6:H6"].Merge = true;
                        range.Worksheet.Cells["D6:H6"].Value = messageRangeDateFilter;
                        range.Worksheet.Cells["I6:M6"].Merge = true;
                        range.Worksheet.Cells["I6:M6"].Value = messageRangeDateFilterKySoSanh;
                        range.Worksheet.Cells["D7:D8"].Merge = true;
                        range.Worksheet.Cells["D7:D8"].Value = "Doanh thu";
                        range.Worksheet.Cells["E7:F7"].Merge = true;
                        range.Worksheet.Cells["E7:F7"].Value = "Các khoản giảm trừ DT";
                        range.Worksheet.Cells["E8"].Value = "Miễn giảm";
                        range.Worksheet.Cells["F8"].Value = "Khác";
                        range.Worksheet.Cells["G7:G8"].Merge = true;
                        range.Worksheet.Cells["G7:G8"].Value = "BHYT";
                        range.Worksheet.Cells["H7:H8"].Merge = true;
                        range.Worksheet.Cells["H7:H8"].Value = "Doanh thu thuần";
                        range.Worksheet.Cells["I7:I8"].Merge = true;
                        range.Worksheet.Cells["I7:I8"].Value = "Doanh thu";
                        range.Worksheet.Cells["J7:K7"].Merge = true;
                        range.Worksheet.Cells["J7:K7"].Value = "Các khoản giảm trừ DT";
                        range.Worksheet.Cells["J8"].Value = "Miễn giảm";
                        range.Worksheet.Cells["K8"].Value = "Khác";
                        range.Worksheet.Cells["L7:L8"].Merge = true;
                        range.Worksheet.Cells["L7:L8"].Value = "BHYT";
                        range.Worksheet.Cells["M7:M8"].Merge = true;
                        range.Worksheet.Cells["M7:M8"].Value = "Doanh thu thuần";
                        range.Worksheet.Cells["D9"].Value = "(1)";
                        range.Worksheet.Cells["E9"].Value = "(2)";
                        range.Worksheet.Cells["F9"].Value = "(3)";
                        range.Worksheet.Cells["G9"].Value = "(4)";
                        range.Worksheet.Cells["H9"].Value = "(5)=(1)-(2)-(3)-(4)";
                        range.Worksheet.Cells["I9"].Value = "(6)";
                        range.Worksheet.Cells["J9"].Value = "(7)";
                        range.Worksheet.Cells["K9"].Value = "(8)";
                        range.Worksheet.Cells["L9"].Value = "(9)";
                        range.Worksheet.Cells["M9"].Value = "(10)=(6)-(7)-(8)-(9)";

                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    var manager = new PropertyManager<BaoCaoChiTietDoanhThuTheoKhoaPhongGridVo>(requestProperties);

                    int index = 10;
                    decimal tongDoanhThuTheoThang = 0;
                    decimal tongMienGiamTheoThang = 0;
                    decimal tongChiPhiKhacTheoThang = 0;
                    decimal tongBhytTheoThang = 0;
                    decimal tongDoanhThuThuanTheoThang = 0;
                    decimal tongDoanhThuTheoKySoSanh = 0;
                    decimal tongMienGiamTheoKySoSanh = 0;
                    decimal tongChiPhiKhacTheoKySoSanh = 0;
                    decimal tongBhytTheoKySoSanh = 0;
                    decimal tongDoanhThuThuanTheoKySoSanh = 0;
                    string oldKhoaPhong = null;

                    for (int i = 0; i < baoCaoChiTietDoanhThuTheoKhoaPhongs.Count; i++)
                    {
                        manager.CurrentObject = baoCaoChiTietDoanhThuTheoKhoaPhongs[i];

                        if (oldKhoaPhong != null && oldKhoaPhong == manager.CurrentObject.KhoaPhong)
                        {
                            continue;
                        }

                        oldKhoaPhong = manager.CurrentObject.KhoaPhong;

                        manager.WriteToXlsx(worksheet, index);
                        worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["B" + index + ":C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells["D" + index + ":M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["A" + index + ":M" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                        worksheet.Cells["A" + index + ":M" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        worksheet.Cells["A" + index + ":B" + index].Style.Font.Color.SetColor(Color.Black);
                        worksheet.Cells["A" + index + ":B" + index].Style.Font.Bold = true;

                        worksheet.Cells["A" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["A" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["A" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["A" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells["B" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["B" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["B" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["B" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells["C" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["C" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["C" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["C" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells["D" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["D" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["D" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["D" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells["E" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["E" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["E" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["E" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells["F" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["F" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["F" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["F" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells["G" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["G" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["G" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["G" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells["H" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["H" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["H" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["H" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells["I" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["I" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["I" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["I" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells["J" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["J" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["J" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["J" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells["K" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["K" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["K" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["K" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells["L" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["L" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["L" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["L" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells["M" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["M" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["M" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["M" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Row(index).Height = 20.5;

                        var indexMain = index;
                        index++;

                        foreach (var dichVu in baoCaoChiTietDoanhThuTheoKhoaPhongs.Where(p => p.KhoaPhong == manager.CurrentObject.KhoaPhong))
                        {
                            tongDoanhThuTheoThang += dichVu.DoanhThuTheoThang ?? 0;
                            tongMienGiamTheoThang += dichVu.MienGiamTheoThang ?? 0;
                            tongChiPhiKhacTheoThang += dichVu.ChiPhiKhacTheoThang ?? 0;
                            tongBhytTheoThang += dichVu.BhytTheoThang ?? 0;
                            tongDoanhThuThuanTheoThang += (dichVu.DoanhThuTheoThang ?? 0) - (dichVu.MienGiamTheoThang ?? 0) - (dichVu.ChiPhiKhacTheoThang ?? 0)
                                - (dichVu.BhytTheoThang ?? 0);

                            tongDoanhThuTheoKySoSanh += dichVu.DoanhThuTheoKySoSanh ?? 0;
                            tongMienGiamTheoKySoSanh += dichVu.MienGiamTheoKySoSanh ?? 0;
                            tongChiPhiKhacTheoKySoSanh += dichVu.ChiPhiKhacTheoKySoSanh ?? 0;
                            tongBhytTheoKySoSanh += dichVu.BhytTheoKySoSanh ?? 0;
                            tongDoanhThuThuanTheoKySoSanh += (dichVu.DoanhThuTheoKySoSanh ?? 0) - (dichVu.MienGiamTheoKySoSanh ?? 0) - (dichVu.ChiPhiKhacTheoKySoSanh ?? 0)
                                                          - (dichVu.BhytTheoKySoSanh ?? 0);

                            worksheet.Cells["C" + index].Value = dichVu.DichVu;
                            worksheet.Cells["D" + index].Value = Convert.ToDouble(dichVu.DoanhThuTheoThang).ApplyFormatMoneyVNDToDouble();
                            worksheet.Cells["E" + index].Value = Convert.ToDouble(dichVu.MienGiamTheoThang).ApplyFormatMoneyVNDToDouble();
                            worksheet.Cells["F" + index].Value = Convert.ToDouble(dichVu.ChiPhiKhacTheoThang).ApplyFormatMoneyVNDToDouble();
                            worksheet.Cells["G" + index].Value = Convert.ToDouble(dichVu.BhytTheoThang).ApplyFormatMoneyVNDToDouble();
                            worksheet.Cells["H" + index].Value = Convert.ToDouble((dichVu.DoanhThuTheoThang ?? 0) - (dichVu.MienGiamTheoThang ?? 0)
                                 - (dichVu.ChiPhiKhacTheoThang ?? 0) - (dichVu.BhytTheoThang ?? 0)).ApplyFormatMoneyVNDToDouble();

                            worksheet.Cells["I" + index].Value = Convert.ToDouble(dichVu.DoanhThuTheoKySoSanh).ApplyFormatMoneyVNDToDouble();
                            worksheet.Cells["J" + index].Value = Convert.ToDouble(dichVu.MienGiamTheoKySoSanh).ApplyFormatMoneyVNDToDouble();
                            worksheet.Cells["K" + index].Value = Convert.ToDouble(dichVu.ChiPhiKhacTheoKySoSanh).ApplyFormatMoneyVNDToDouble();
                            worksheet.Cells["L" + index].Value = Convert.ToDouble(dichVu.BhytTheoKySoSanh).ApplyFormatMoneyVNDToDouble();
                            worksheet.Cells["M" + index].Value = Convert.ToDouble((dichVu.DoanhThuTheoKySoSanh ?? 0) - (dichVu.MienGiamTheoKySoSanh ?? 0)
                                 - (dichVu.ChiPhiKhacTheoKySoSanh ?? 0) - (dichVu.BhytTheoKySoSanh ?? 0)).ApplyFormatMoneyVNDToDouble();

                            worksheet.Cells["A" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["A" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["A" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["A" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells["B" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["B" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["B" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["B" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells["C" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["C" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["C" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["C" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells["D" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["D" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["D" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["D" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells["E" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["E" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["E" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["E" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells["F" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["F" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["F" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["F" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells["G" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["G" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["G" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["G" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells["H" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["H" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["H" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["H" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells["I" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["I" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["I" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["I" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells["J" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["J" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["J" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["J" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells["K" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["K" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["K" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["K" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells["L" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["L" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["L" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["L" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells["M" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["M" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["M" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["M" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["D" + index + ":M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["A" + index + ":M" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                            worksheet.Cells["A" + index + ":M" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                            worksheet.Cells["A" + index + ":M" + index].Style.Font.Color.SetColor(Color.Black);

                            index++;
                        }

                        for (int inde = indexMain + 1; inde <= index - 1; inde++)
                        {
                            worksheet.Row(inde).OutlineLevel = 1;
                        }

                        index++;
                    }

                    worksheet.Cells["A" + index + ":M" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                    worksheet.Cells["A" + index + ":M" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["A" + index + ":M" + index].Style.Font.Bold = true;
                    worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells["D" + index + ":M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["A" + index + ":M" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                    worksheet.Cells["A" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["A" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["A" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["A" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    worksheet.Cells["B" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["B" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["B" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["B" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["B" + index].Value = "Tổng cộng".ToUpper();

                    worksheet.Cells["C" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["C" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["C" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["C" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    worksheet.Cells["D" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["D" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["D" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["D" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["D" + index].Value = Convert.ToDouble(tongDoanhThuTheoThang).ApplyFormatMoneyVNDToDouble();

                    worksheet.Cells["E" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["E" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["E" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["E" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["E" + index].Value = Convert.ToDouble(tongMienGiamTheoThang).ApplyFormatMoneyVNDToDouble();

                    worksheet.Cells["F" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["F" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["F" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["F" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["F" + index].Value = Convert.ToDouble(tongChiPhiKhacTheoThang).ApplyFormatMoneyVNDToDouble();

                    worksheet.Cells["G" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["G" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["G" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["G" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["G" + index].Value = Convert.ToDouble(tongBhytTheoThang).ApplyFormatMoneyVNDToDouble();

                    worksheet.Cells["H" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["H" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["H" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["H" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["H" + index].Value = Convert.ToDouble(tongDoanhThuThuanTheoThang).ApplyFormatMoneyVNDToDouble();

                    worksheet.Cells["I" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["I" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["I" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["I" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["I" + index].Value = Convert.ToDouble(tongDoanhThuTheoKySoSanh).ApplyFormatMoneyVNDToDouble();

                    worksheet.Cells["J" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["J" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["J" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["J" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["J" + index].Value = Convert.ToDouble(tongMienGiamTheoKySoSanh).ApplyFormatMoneyVNDToDouble();

                    worksheet.Cells["K" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["K" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["K" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["K" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["K" + index].Value = Convert.ToDouble(tongChiPhiKhacTheoKySoSanh).ApplyFormatMoneyVNDToDouble();

                    worksheet.Cells["L" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["L" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["L" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["L" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["L" + index].Value = Convert.ToDouble(tongBhytTheoKySoSanh).ApplyFormatMoneyVNDToDouble();

                    worksheet.Cells["M" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["M" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["M" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["M" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["M" + index].Value = Convert.ToDouble(tongDoanhThuThuanTheoKySoSanh).ApplyFormatMoneyVNDToDouble();

                    xlPackage.Save();
                }

                return stream.ToArray();
            }
        }

        //IList<BaoCaoTongHopDoanhThuTheoKhoaPhongGridVo> baoCaoThuPhiVienPhiGridVos
        public virtual byte[] ExportAggregateRevenueReportByDepartment(
            IList<BaoCaoTongHopDoanhThuTheoKhoaPhongGridVo> baoCaoTongHopDoanhThuTheoKhoaPhongs,
            DateTimeFilterVo dateTimeFilter
        )
        {
            int ind = 1;

            string messageRangeDateFilter = null;
            string messageRangeDateFilterKySoSanh = "Kỳ so sánh: ";

            if (dateTimeFilter != null && dateTimeFilter.RangeDateTimeFilter != null)
            {
                messageRangeDateFilter = "Từ " + dateTimeFilter.RangeDateTimeFilter.DateStart.Value.ApplyFormatTimeDate()
                                               + " đến " + dateTimeFilter.RangeDateTimeFilter.DateEnd.Value.ApplyFormatTimeDate();
            }

            if (dateTimeFilter != null && dateTimeFilter.RangeDateTimeSoSanh != null)
            {
                messageRangeDateFilterKySoSanh += "Từ " + dateTimeFilter.RangeDateTimeSoSanh.DateStart.Value.ApplyFormatTimeDate()
                                                        + " đến " + dateTimeFilter.RangeDateTimeSoSanh.DateEnd.Value.ApplyFormatTimeDate();
            }

            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoTongHopDoanhThuTheoKhoaPhongGridVo>("STT", p => ind++),
                new PropertyByName<BaoCaoTongHopDoanhThuTheoKhoaPhongGridVo>("Khoa/Phòng", p => p.KhoaPhong),
                new PropertyByName<BaoCaoTongHopDoanhThuTheoKhoaPhongGridVo>("Doanh thu tháng",
                    p => Convert.ToDouble(p.DoanhThuTheoThang ?? 0).ApplyFormatMoneyVNDToDouble()),
                new PropertyByName<BaoCaoTongHopDoanhThuTheoKhoaPhongGridVo>("Miễn giảm tháng",
                    p => Convert.ToDouble(p.MienGiamTheoThang ?? 0).ApplyFormatMoneyVNDToDouble()),
                new PropertyByName<BaoCaoTongHopDoanhThuTheoKhoaPhongGridVo>("Chi phí khác theo tháng",
                    p => Convert.ToDouble(p.ChiPhiKhacTheoThang ?? 0).ApplyFormatMoneyVNDToDouble()),
                new PropertyByName<BaoCaoTongHopDoanhThuTheoKhoaPhongGridVo>("BHYT tháng",
                    p => Convert.ToDouble(p.BhytTheoThang ?? 0).ApplyFormatMoneyVNDToDouble()),
                new PropertyByName<BaoCaoTongHopDoanhThuTheoKhoaPhongGridVo>("Doanh thu thuần theo tháng",
                    p => Convert.ToDouble((p.DoanhThuTheoThang ?? 0) -  (p.MienGiamTheoThang ?? 0) - (p.ChiPhiKhacTheoThang ?? 0)
                                          - (p.BhytTheoThang ?? 0)).ApplyFormatMoneyVNDToDouble()),
                new PropertyByName<BaoCaoTongHopDoanhThuTheoKhoaPhongGridVo>("Doanh thu kỳ so sánh",
                    p => Convert.ToDouble(p.DoanhThuTheoKySoSanh ?? 0).ApplyFormatMoneyVNDToDouble()),
                new PropertyByName<BaoCaoTongHopDoanhThuTheoKhoaPhongGridVo>("Miễn giảm kỳ so sánh",
                    p => Convert.ToDouble(p.MienGiamTheoKySoSanh ?? 0).ApplyFormatMoneyVNDToDouble()),
                new PropertyByName<BaoCaoTongHopDoanhThuTheoKhoaPhongGridVo>("Chi phí khác kỳ so sánh",
                    p => Convert.ToDouble(p.ChiPhiKhacTheoKySoSanh ?? 0).ApplyFormatMoneyVNDToDouble()),
                new PropertyByName<BaoCaoTongHopDoanhThuTheoKhoaPhongGridVo>("Bhyt kỳ so sánh",
                    p => Convert.ToDouble(p.BhytTheoKySoSanh).ApplyFormatMoneyVNDToDouble()),
                new PropertyByName<BaoCaoTongHopDoanhThuTheoKhoaPhongGridVo>("Doanh thu thuần kỳ so sánh",
                    p => Convert.ToDouble((p.DoanhThuTheoKySoSanh ?? 0) -  (p.MienGiamTheoKySoSanh ?? 0) - (p.ChiPhiKhacTheoKySoSanh ?? 0)
                                          - (p.BhytTheoKySoSanh ?? 0)).ApplyFormatMoneyVNDToDouble())
            };

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BC06");

                    // set row
                    worksheet.Row(2).Height = 20;
                    worksheet.Row(3).Height = 20;
                    worksheet.Row(5).Height = 25;
                    worksheet.Row(6).Height = 25;
                    worksheet.Row(7).Height = 25;
                    worksheet.Row(8).Height = 25;
                    worksheet.DefaultRowHeight = 16;

                    // set column
                    worksheet.Column(1).Width = 7;
                    worksheet.Column(2).Width = 25;
                    worksheet.Column(3).Width = 16;
                    worksheet.Column(4).Width = 16;
                    worksheet.Column(5).Width = 16;
                    worksheet.Column(6).Width = 16;
                    worksheet.Column(7).Width = 20;
                    worksheet.Column(8).Width = 16;
                    worksheet.Column(9).Width = 16;
                    worksheet.Column(10).Width = 16;
                    worksheet.Column(11).Width = 16;
                    worksheet.Column(12).Width = 20;
                    worksheet.DefaultColWidth = 7;

                    using (worksheet.Cells["A2:L2"])
                    {
                        worksheet.Select("A2:L2");
                        worksheet.SelectedRange.Merge = true;
                        worksheet.SelectedRange.Value = "Báo cáo tổng hợp doanh thu theo khoa/phòng".ToUpper();
                        worksheet.SelectedRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.SelectedRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.SelectedRange.Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        worksheet.SelectedRange.Style.Font.Color.SetColor(Color.Black);
                        worksheet.SelectedRange.Style.Font.Bold = true;
                    }

                    using (worksheet.Cells["A3:L3"])
                    {
                        worksheet.Select("A3:L3");
                        worksheet.SelectedRange.Merge = true;
                        worksheet.SelectedRange.Value = messageRangeDateFilter;
                        worksheet.SelectedRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.SelectedRange.Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        worksheet.SelectedRange.Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        worksheet.SelectedRange.Style.Font.Color.SetColor(Color.Black);
                        worksheet.SelectedRange.Style.Font.Bold = true;
                        worksheet.SelectedRange.Style.Font.Italic = true;
                    }

                    using (var range = worksheet.Cells["A5:L8"])
                    {
                        worksheet.Select("A5:L8");
                        worksheet.SelectedRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.SelectedRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.SelectedRange.Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        worksheet.SelectedRange.Style.Font.Color.SetColor(Color.Black);
                        worksheet.SelectedRange.Style.Font.Bold = true;

                        range.Worksheet.Cells["A5:A7"].Merge = true;
                        range.Worksheet.Cells["A5:A7"].Value = "STT";
                        range.Worksheet.Cells["B5:B7"].Merge = true;
                        range.Worksheet.Cells["B5:B7"].Value = "Khoa/Phòng";
                        range.Worksheet.Cells["C5:G5"].Merge = true;
                        range.Worksheet.Cells["C5:G5"].Value = messageRangeDateFilter;
                        range.Worksheet.Cells["H5:L5"].Merge = true;
                        range.Worksheet.Cells["H5:L5"].Value = messageRangeDateFilterKySoSanh;
                        range.Worksheet.Cells["C6:C7"].Merge = true;
                        range.Worksheet.Cells["C6:C7"].Value = "Doanh thu";
                        range.Worksheet.Cells["D6:E6"].Merge = true;
                        range.Worksheet.Cells["D6:E6"].Value = "Các khoản giảm trừ DT";

                        range.Worksheet.Cells["D7"].Value = "Miễn giảm";
                        range.Worksheet.Cells["E7"].Value = "Khác";


                        range.Worksheet.Cells["F6:F7"].Merge = true;
                        range.Worksheet.Cells["F6:F7"].Value = "BHYT";
                        range.Worksheet.Cells["G6:G7"].Merge = true;
                        range.Worksheet.Cells["G6:G7"].Value = "Doanh thu thuần";
                        range.Worksheet.Cells["H6:H7"].Merge = true;
                        range.Worksheet.Cells["H6:H7"].Value = "Doanh thu";
                        range.Worksheet.Cells["I6:J6"].Merge = true;
                        range.Worksheet.Cells["I6:J6"].Value = "Các khoản giảm trừ DT";

                        range.Worksheet.Cells["I7"].Value = "Miễn giảm";
                        range.Worksheet.Cells["J7"].Value = "Khác";

                        range.Worksheet.Cells["K6:K7"].Merge = true;
                        range.Worksheet.Cells["K6:K7"].Value = "BHYT";
                        range.Worksheet.Cells["L6:L7"].Merge = true;
                        range.Worksheet.Cells["L6:L7"].Value = "Doanh thu thuần";

                        range.Worksheet.Cells["C8"].Value = "(1)";
                        range.Worksheet.Cells["D8"].Value = "(2)";
                        range.Worksheet.Cells["E8"].Value = "(3)";
                        range.Worksheet.Cells["F8"].Value = "(4)";
                        range.Worksheet.Cells["G8"].Value = "(5)=(1)-(2)-(3)-(4)";
                        range.Worksheet.Cells["H8"].Value = "(6)";
                        range.Worksheet.Cells["I8"].Value = "(7)";
                        range.Worksheet.Cells["J8"].Value = "(8)";
                        range.Worksheet.Cells["K8"].Value = "(9)";
                        range.Worksheet.Cells["L8"].Value = "(10)=(6)-(7)-(8)-(9)";

                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    var manager = new PropertyManager<BaoCaoTongHopDoanhThuTheoKhoaPhongGridVo>(requestProperties);

                    int index = 9;
                    decimal tongDoanhThuTheoThang = 0;
                    decimal tongMienGiamTheoThang = 0;
                    decimal tongChiPhiKhacTheoThang = 0;
                    decimal tongBhytTheoThang = 0;
                    decimal tongDoanhThuThuanTheoThang = 0;
                    decimal tongDoanhThuTheoKySoSanh = 0;
                    decimal tongMienGiamTheoKySoSanh = 0;
                    decimal tongChiPhiKhacTheoKySoSanh = 0;
                    decimal tongBhytTheoKySoSanh = 0;
                    decimal tongDoanhThuThuanTheoKySoSanh = 0;

                    for (int i = 0; i < baoCaoTongHopDoanhThuTheoKhoaPhongs.Count; i++)
                    {
                        manager.CurrentObject = baoCaoTongHopDoanhThuTheoKhoaPhongs[i];
                        manager.WriteToXlsx(worksheet, index);

                        tongDoanhThuTheoThang += manager.CurrentObject.DoanhThuTheoThang ?? 0;
                        tongMienGiamTheoThang += manager.CurrentObject.MienGiamTheoThang ?? 0;
                        tongChiPhiKhacTheoThang += manager.CurrentObject.ChiPhiKhacTheoThang ?? 0;
                        tongBhytTheoThang += manager.CurrentObject.BhytTheoThang ?? 0;
                        tongDoanhThuThuanTheoThang +=
                            (manager.CurrentObject.DoanhThuTheoThang ?? 0) - (manager.CurrentObject.MienGiamTheoThang ?? 0)
                                                                       - (manager.CurrentObject.ChiPhiKhacTheoThang ?? 0) -
                                                                       (manager.CurrentObject.BhytTheoThang ?? 0);

                        tongDoanhThuTheoKySoSanh += manager.CurrentObject.DoanhThuTheoKySoSanh ?? 0;
                        tongMienGiamTheoKySoSanh += manager.CurrentObject.MienGiamTheoKySoSanh ?? 0;
                        tongChiPhiKhacTheoKySoSanh += manager.CurrentObject.ChiPhiKhacTheoKySoSanh ?? 0;
                        tongBhytTheoKySoSanh += manager.CurrentObject.BhytTheoKySoSanh ?? 0;
                        tongDoanhThuThuanTheoKySoSanh +=
                            (manager.CurrentObject.DoanhThuTheoKySoSanh ?? 0) - (manager.CurrentObject.MienGiamTheoKySoSanh ?? 0)
                                                                       - (manager.CurrentObject.ChiPhiKhacTheoKySoSanh ?? 0) -
                                                                       (manager.CurrentObject.BhytTheoKySoSanh ?? 0);

                        worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells["C" + index + ":L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["A" + index + ":L" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        worksheet.Cells["A" + index + ":L" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        worksheet.Cells["A" + index + ":L" + index].Style.Font.Color.SetColor(Color.Black);


                        worksheet.Cells["A" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["A" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["A" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["A" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells["B" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["B" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["B" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["B" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells["C" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["C" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["C" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["C" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells["D" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["D" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["D" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["D" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells["E" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["E" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["E" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["E" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells["F" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["F" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["F" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["F" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells["G" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["G" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["G" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["G" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells["H" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["H" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["H" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["H" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells["I" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["I" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["I" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["I" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells["J" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["J" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["J" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["J" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells["K" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["K" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["K" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["K" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells["L" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["L" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["L" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["L" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Row(index).Height = 25;
                        index++;
                    }

                    worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells["C" + index + ":L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["A" + index + ":L" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                    worksheet.Cells["A" + index + ":L" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["A" + index + ":L" + index].Style.Font.Bold = true;
                    worksheet.Row(index).Height = 25;

                    worksheet.Cells["A" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["A" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["A" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["A" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    worksheet.Cells["B" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["B" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["B" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["B" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["B" + index].Value = "Tổng cộng".ToUpper();

                    worksheet.Cells["C" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["C" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["C" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["C" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["C" + index].Value = Convert.ToDouble(tongDoanhThuTheoThang).ApplyFormatMoneyVNDToDouble();

                    worksheet.Cells["D" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["D" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["D" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["D" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["D" + index].Value = Convert.ToDouble(tongMienGiamTheoThang).ApplyFormatMoneyVNDToDouble();


                    worksheet.Cells["E" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["E" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["E" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["E" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["E" + index].Value = Convert.ToDouble(tongChiPhiKhacTheoThang).ApplyFormatMoneyVNDToDouble();

                    worksheet.Cells["F" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["F" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["F" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["F" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["F" + index].Value = Convert.ToDouble(tongBhytTheoThang).ApplyFormatMoneyVNDToDouble();

                    worksheet.Cells["G" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["G" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["G" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["G" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["G" + index].Value = Convert.ToDouble(tongDoanhThuThuanTheoThang).ApplyFormatMoneyVNDToDouble();

                    worksheet.Cells["H" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["H" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["H" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["H" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["H" + index].Value = Convert.ToDouble(tongDoanhThuTheoKySoSanh).ApplyFormatMoneyVNDToDouble();

                    worksheet.Cells["I" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["I" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["I" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["I" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["I" + index].Value = Convert.ToDouble(tongMienGiamTheoKySoSanh).ApplyFormatMoneyVNDToDouble();

                    worksheet.Cells["J" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["J" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["J" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["J" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["J" + index].Value = Convert.ToDouble(tongChiPhiKhacTheoKySoSanh).ApplyFormatMoneyVNDToDouble();

                    worksheet.Cells["K" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["K" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["K" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["K" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["K" + index].Value = Convert.ToDouble(tongBhytTheoKySoSanh).ApplyFormatMoneyVNDToDouble();

                    worksheet.Cells["L" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["L" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["L" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["L" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["L" + index].Value = Convert.ToDouble(tongDoanhThuThuanTheoKySoSanh).ApplyFormatMoneyVNDToDouble();

                    xlPackage.Save();
                }

                return stream.ToArray();
            }
        }

        public virtual byte[] ExportConsolidatedSalesReportToXlsx(IList<BaoCaoTongHopDoanhThuTheoBacSiGridVo> baoCaoTongHopDoanhThuTheoBacSi, DateTimeFilterVo dateTimeFilter)
        {
            int ind = 1;

            string messageRangeDateFilter = null;

            if (dateTimeFilter != null && dateTimeFilter.RangeDateTimeFilter != null)
            {
                messageRangeDateFilter = "Từ " + dateTimeFilter.RangeDateTimeFilter.DateStart.Value.ApplyFormatTimeDate()
                                               + " đến " + dateTimeFilter.RangeDateTimeFilter.DateEnd.Value.ApplyFormatTimeDate();
            }

            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoTongHopDoanhThuTheoBacSiGridVo>("STT", p => ind++),
                new PropertyByName<BaoCaoTongHopDoanhThuTheoBacSiGridVo>("Họ và tên bác sĩ", p => p.HoTenBacSi),
                new PropertyByName<BaoCaoTongHopDoanhThuTheoBacSiGridVo>("Doanh thu",
                    p => Convert.ToDouble(p.DoanhThu).ApplyFormatMoneyVNDToDouble()),
                new PropertyByName<BaoCaoTongHopDoanhThuTheoBacSiGridVo>("Miễn giảm",
                    p => Convert.ToDouble(p.MienGiam).ApplyFormatMoneyVNDToDouble()),
                new PropertyByName<BaoCaoTongHopDoanhThuTheoBacSiGridVo>("Khác",
                    p => Convert.ToDouble(p.KhoanGiamTruKhac).ApplyFormatMoneyVNDToDouble()),
                new PropertyByName<BaoCaoTongHopDoanhThuTheoBacSiGridVo>("BHYT",
                    p => Convert.ToDouble(p.Bhyt).ApplyFormatMoneyVNDToDouble()),
                new PropertyByName<BaoCaoTongHopDoanhThuTheoBacSiGridVo>("Thực thu",
                    p => Convert.ToDouble((p.DoanhThu ?? 0) - (p.MienGiam ?? 0)- (p.KhoanGiamTruKhac ?? 0) - (p.Bhyt ?? 0)).ApplyFormatMoneyVNDToDouble())
            };

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BC04");

                    // set row
                    worksheet.Row(4).Height = 17;
                    worksheet.Row(6).Height = 27;
                    worksheet.Row(7).Height = 27;
                    worksheet.Row(8).Height = 27;
                    worksheet.DefaultRowHeight = 15.75;

                    // set column
                    worksheet.Column(1).Width = 10;
                    worksheet.Column(2).Width = 40;
                    worksheet.Column(3).Width = 20;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 20;
                    worksheet.Column(6).Width = 20;
                    worksheet.Column(7).Width = 20;
                    worksheet.DefaultColWidth = 8;

                    using (var range = worksheet.Cells["A2:G3"])
                    {
                        range.Worksheet.Cells["A2:G3"].Merge = true;
                        range.Worksheet.Cells["A2:G3"].Value = "Báo cáo tổng hợp doanh thu bác sỹ".ToUpper();
                        range.Worksheet.Cells["A2:G3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A2:G3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A2:G3"].Style.Font.SetFromFont(new Font("Times New Roman", 15));
                        range.Worksheet.Cells["A2:G3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A2:G3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A4:G4"])
                    {
                        range.Worksheet.Cells["A4:G4"].Merge = true;
                        range.Worksheet.Cells["A4:G4"].Value = messageRangeDateFilter;
                        range.Worksheet.Cells["A4:G4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:G4"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["A4:G4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:G4"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A4:G4"].Style.Font.Italic = true;
                    }

                    using (var range = worksheet.Cells["A6:G8"])
                    {
                        range.Worksheet.Cells["A6:G8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A6:G8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A6:G8"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["A6:G8"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A6:G8"].Style.Font.Bold = true;
                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        range.Worksheet.Cells["A6:A7"].Merge = true;
                        range.Worksheet.Cells["A6:A7"].Value = "STT";
                        range.Worksheet.Cells["B6:B7"].Merge = true;
                        range.Worksheet.Cells["B6:B7"].Value = "Họ và tên bác sĩ";
                        range.Worksheet.Cells["C6:C7"].Merge = true;
                        range.Worksheet.Cells["C6:C7"].Value = "Doanh thu";
                        range.Worksheet.Cells["D6:E6"].Merge = true;
                        range.Worksheet.Cells["D6:E6"].Value = "Các khoản giảm trừ DT";
                        range.Worksheet.Cells["F6:F7"].Merge = true;
                        range.Worksheet.Cells["F6:F7"].Value = "BHYT";
                        range.Worksheet.Cells["G6:G7"].Merge = true;
                        range.Worksheet.Cells["G6:G7"].Value = "Thực thu";
                    }

                    worksheet.Cells["D7"].Value = "Miễn giảm";
                    worksheet.Cells["E7"].Value = "Khác";
                    worksheet.Cells["C8"].Value = "(1)";
                    worksheet.Cells["D8"].Value = "(2)";
                    worksheet.Cells["E8"].Value = "(3)";
                    worksheet.Cells["F8"].Value = "(4)";
                    worksheet.Cells["G8"].Value = "(5)=(1)-(2)-(3)-(4)";

                    var manager = new PropertyManager<BaoCaoTongHopDoanhThuTheoBacSiGridVo>(requestProperties);
                    int index = 9;

                    decimal finalDoanhThu = 0;
                    decimal finalMienGiam = 0;
                    decimal finalKhac = 0;
                    decimal finalBhyt = 0;
                    decimal finalThucThu = 0;

                    for (int i = 0; i < baoCaoTongHopDoanhThuTheoBacSi.Count; i++)
                    {
                        manager.CurrentObject = baoCaoTongHopDoanhThuTheoBacSi[i];

                        finalDoanhThu += manager.CurrentObject.DoanhThu ?? 0;
                        finalMienGiam += manager.CurrentObject.MienGiam ?? 0;
                        finalKhac += manager.CurrentObject.KhoanGiamTruKhac ?? 0;
                        finalBhyt += manager.CurrentObject.Bhyt ?? 0;
                        finalThucThu += (manager.CurrentObject.DoanhThu ?? 0) - (manager.CurrentObject.MienGiam ?? 0) - (manager.CurrentObject.KhoanGiamTruKhac ?? 0) - (manager.CurrentObject.Bhyt ?? 0);

                        manager.WriteToXlsx(worksheet, index);
                        worksheet.Cells["A" + index + ":G" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells["C" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["A" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        worksheet.Cells["A" + index + ":G" + index].Style.Font.Color.SetColor(Color.Black);

                        worksheet.Cells["A" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["A" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["A" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["A" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells["B" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["B" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["B" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["B" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells["C" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["C" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["C" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["C" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells["D" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["D" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["D" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["D" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells["E" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["E" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["E" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["E" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells["F" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["F" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["F" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["F" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells["G" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["G" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["G" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["G" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Row(index).Height = 27;
                        index++;
                    }

                    // after render cell data
                    worksheet.Cells["A" + index + ":G" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["C" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["A" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                    worksheet.Cells["A" + index + ":G" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["A" + index + ":G" + index].Style.Font.Bold = true;

                    worksheet.Cells["A" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["A" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["A" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["A" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    worksheet.Cells["B" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["B" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["B" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["B" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["B" + index].Value = "Tổng cộng:";

                    worksheet.Cells["C" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["C" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["C" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["C" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["C" + index].Value = Convert.ToDouble(finalDoanhThu).ApplyFormatMoneyVNDToDouble();

                    worksheet.Cells["D" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["D" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["D" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["D" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["D" + index].Value = Convert.ToDouble(finalMienGiam).ApplyFormatMoneyVNDToDouble();

                    worksheet.Cells["E" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["E" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["E" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["E" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["E" + index].Value = Convert.ToDouble(finalKhac).ApplyFormatMoneyVNDToDouble();

                    worksheet.Cells["F" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["F" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["F" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["F" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["F" + index].Value = Convert.ToDouble(finalBhyt).ApplyFormatMoneyVNDToDouble();

                    worksheet.Cells["G" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["G" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["G" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["G" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["G" + index].Value = Convert.ToDouble(finalThucThu).ApplyFormatMoneyVNDToDouble();
                    worksheet.Row(index).Height = 27;

                    xlPackage.Save();
                }

                return stream.ToArray();
            }
        }

        public virtual byte[] DetailedSalesReportByDoctor(
            IList<BaoCaoChiTietDoanhThuTheoBacSiGridVo> baoCaoChiTietDoanhThuTheoBacSis,
            DateTimeFilterVo dateTimeFilter, string tenBacSi)
        {
            int ind = 1;

            string messageRangeDateFilter = null;

            if (dateTimeFilter != null && dateTimeFilter.RangeDateTimeFilter != null)
            {
                messageRangeDateFilter = "Từ " + dateTimeFilter.RangeDateTimeFilter.DateStart.Value.ApplyFormatTimeDate()
                       + " đến " + dateTimeFilter.RangeDateTimeFilter.DateEnd.Value.ApplyFormatTimeDate();
            }

            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoChiTietDoanhThuTheoBacSiGridVo>("STT", p => ind++),
                new PropertyByName<BaoCaoChiTietDoanhThuTheoBacSiGridVo>("Mã TN", p => p.MaTn),
                new PropertyByName<BaoCaoChiTietDoanhThuTheoBacSiGridVo>("Ngày", p => p.Ngay),
                new PropertyByName<BaoCaoChiTietDoanhThuTheoBacSiGridVo>("Mã người bệnh", p => p.MaBn),
                new PropertyByName<BaoCaoChiTietDoanhThuTheoBacSiGridVo>("Họ và tên người bệnh", p => p.HoTenBn.ToUpper())
            };

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BC03");

                    // set row
                    worksheet.Row(2).Height = 20.25;
                    worksheet.Row(3).Height = 21;
                    worksheet.Row(4).Height = 21;
                    worksheet.Row(5).Height = 21;
                    worksheet.Row(6).Height = 16.5;
                    worksheet.Row(7).Height = 16.5;
                    worksheet.DefaultRowHeight = 15;

                    // set column
                    worksheet.Column(1).Width = 10;
                    worksheet.Column(2).Width = 20;
                    worksheet.Column(3).Width = 20.33;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 35;
                    worksheet.Column(6).Width = 33;
                    worksheet.Column(7).Width = 13;
                    worksheet.Column(8).Width = 16.89;
                    worksheet.Column(9).Width = 16.89;
                    worksheet.Column(10).Width = 18;
                    worksheet.Column(11).Width = 20;
                    worksheet.DefaultColWidth = 6.67;

                    // set style and value K1
                    worksheet.Cells["K1"].Value = "TCKT/BM03";
                    worksheet.Cells["K1"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                    worksheet.Cells["K1"].Style.Font.Color.SetColor(Color.Black);

                    // set style and value of range "A2:K2"
                    using (var range = worksheet.Cells["A2:K2"])
                    {
                        range.Worksheet.Cells["A2:K2"].Merge = true;
                        range.Worksheet.Cells["A2:K2"].Value = "Báo cáo chi tiết doanh thu theo bác sĩ".ToUpper();
                        range.Worksheet.Cells["A2:K2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A2:K2"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A2:K2"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A2:K2"].Style.Font.Bold = true;
                    }

                    // set style and value of range "A3:K3"
                    using (var range = worksheet.Cells["A3:K3"])
                    {
                        range.Worksheet.Cells["A3:K3"].Merge = true;
                        range.Worksheet.Cells["A3:K3"].Value = messageRangeDateFilter;
                        range.Worksheet.Cells["A3:K3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:K3"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["A3:K3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:K3"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A3:K3"].Style.Font.Italic = true;
                    }

                    // set style and value of range "I4:K4"
                    using (var range = worksheet.Cells["I4:K4"])
                    {
                        range.Worksheet.Cells["I4:K4"].Merge = true;
                        range.Worksheet.Cells["I4:K4"].Value = "Bác sĩ: " + tenBacSi;
                        range.Worksheet.Cells["I4:K4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["I4:K4"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["I4:K4"].Style.Font.Color.SetColor(Color.Red);
                        range.Worksheet.Cells["I4:K4"].Style.Font.Bold = true;
                        range.Worksheet.Cells["I4:K4"].Style.Font.Italic = true;
                    }

                    using (var range = worksheet.Cells["A5:K8"])
                    {
                        range.Worksheet.Cells["A5:K8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A5:K8"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["A5:K8"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A5:K8"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A5:A7"].Merge = true;
                        range.Worksheet.Cells["A5:A7"].Value = "STT";
                        range.Worksheet.Cells["B5:B7"].Merge = true;
                        range.Worksheet.Cells["B5:B7"].Value = "Mã TN";
                        range.Worksheet.Cells["C5:C7"].Merge = true;
                        range.Worksheet.Cells["C5:C7"].Value = "Ngày";
                        range.Worksheet.Cells["D5:D7"].Merge = true;
                        range.Worksheet.Cells["D5:D7"].Value = "Mã người bệnh";
                        range.Worksheet.Cells["E5:E7"].Merge = true;
                        range.Worksheet.Cells["E5:E7"].Value = "Họ và tên người bệnh";
                        range.Worksheet.Cells["F5:F7"].Merge = true;
                        range.Worksheet.Cells["F5:F7"].Value = "Tên dịch vụ chỉ định/thực hiện";
                        range.Worksheet.Cells["G5:K5"].Merge = true;
                        range.Worksheet.Cells["G5:K5"].Value = messageRangeDateFilter;
                        range.Worksheet.Cells["G6:G7"].Merge = true;
                        range.Worksheet.Cells["G6:G7"].Value = "Doanh thu";
                        range.Worksheet.Cells["H6:I6"].Merge = true;
                        range.Worksheet.Cells["H6:I6"].Value = "Các khoản giảm trừ DT";
                        range.Worksheet.Cells["H7"].Value = "Miễn giảm";
                        range.Worksheet.Cells["I7"].Value = "Khác";
                        range.Worksheet.Cells["J6:J7"].Merge = true;
                        range.Worksheet.Cells["J6:J7"].Value = "BHYT";
                        range.Worksheet.Cells["K6:K7"].Merge = true;
                        range.Worksheet.Cells["K6:K7"].Value = "Doanh thu thuần";
                        range.Worksheet.Cells["G8"].Value = "(1)";
                        range.Worksheet.Cells["H8"].Value = "(2)";
                        range.Worksheet.Cells["I8"].Value = "(3)";
                        range.Worksheet.Cells["J8"].Value = "(4)";
                        range.Worksheet.Cells["K8"].Value = "(5)=(1)-(2)-(3)-(4)";
                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    var manager = new PropertyManager<BaoCaoChiTietDoanhThuTheoBacSiGridVo>(requestProperties);
                    int index = 9;

                    decimal finalTongDoanhThu = 0;
                    decimal finalTongMienGiam = 0;
                    decimal finalTongBhyt = 0;
                    decimal finalTongDoanhThuChot = 0;
                    string oldBenhNhan = null;

                    for (int i = 0; i < baoCaoChiTietDoanhThuTheoBacSis.Count; i++)
                    {
                        manager.CurrentObject = baoCaoChiTietDoanhThuTheoBacSis[i];

                        if (oldBenhNhan != null && oldBenhNhan == manager.CurrentObject.MaBn)
                        {
                            continue;
                        }

                        oldBenhNhan = manager.CurrentObject.MaBn;

                        manager.WriteToXlsx(worksheet, index);
                        worksheet.Cells["A" + index + ":K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["A" + index + ":K" + index].Style.Font.SetFromFont(new Font("Times New Roman", 13));

                        worksheet.Cells["A" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["A" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["A" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["A" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells["B" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["B" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["B" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["B" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells["C" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["C" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["C" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["C" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["C" + index].Style.Font.Color.SetColor(Color.Red);

                        worksheet.Cells["D" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["D" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["D" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["D" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["D" + index].Style.Font.Color.SetColor(Color.Red);

                        worksheet.Cells["E" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["E" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["E" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["E" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["E" + index].Style.Font.Color.SetColor(Color.Red);

                        worksheet.Cells["F" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["F" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["F" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["F" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells["G" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["G" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["G" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["G" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells["H" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["H" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["H" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["H" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells["I" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["I" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["I" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["I" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells["J" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["J" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["J" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["J" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells["K" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["K" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["K" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["K" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells["G" + index + ":K" + index].Style.Font.Bold = true;

                        decimal tongDoanhThu = 0;
                        decimal tongMienGiam = 0;
                        decimal tongBhyt = 0;
                        decimal tongDoanhThuThuan = 0;

                        var indexMain = index;
                        index++;

                        foreach (var dichVu in baoCaoChiTietDoanhThuTheoBacSis.Where(p => p.MaBn == manager.CurrentObject.MaBn))
                        {
                            tongDoanhThu += dichVu.DoanhThuTheoThang ?? 0;
                            tongMienGiam += dichVu.MienGiamTheoThang ?? 0;
                            tongBhyt += dichVu.BhytTheoThang ?? 0;
                            tongDoanhThuThuan += dichVu.DoanhThuTheoThang - dichVu.MienGiamTheoThang - dichVu.BhytTheoThang ?? 0;

                            worksheet.Cells["F" + index].Value = dichVu.DichVuChiDinh;
                            worksheet.Cells["G" + index].Value = Convert.ToDouble(dichVu.DoanhThuTheoThang).ApplyFormatMoneyVNDToDouble();
                            worksheet.Cells["H" + index].Value = Convert.ToDouble(dichVu.MienGiamTheoThang).ApplyFormatMoneyVNDToDouble();
                            worksheet.Cells["J" + index].Value = Convert.ToDouble(dichVu.BhytTheoThang).ApplyFormatMoneyVNDToDouble();
                            worksheet.Cells["K" + index].Value = Convert.ToDouble(dichVu.DoanhThuTheoThang - dichVu.MienGiamTheoThang - dichVu.BhytTheoThang).ApplyFormatMoneyVNDToDouble();

                            worksheet.Cells["A" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["A" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["A" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["A" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells["B" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["B" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["B" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["B" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells["C" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["C" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["C" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["C" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells["D" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["D" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["D" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["D" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells["E" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["E" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["E" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["E" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells["F" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["F" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["F" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["F" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells["G" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["G" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["G" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["G" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells["H" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["H" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["H" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["H" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells["I" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["I" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["I" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["I" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells["J" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["J" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["J" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["J" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells["K" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["K" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["K" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["K" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells["A" + index + ":K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["A" + index + ":K" + index].Style.Font.SetFromFont(new Font("Times New Roman", 13));

                            index++;
                        }

                        worksheet.Cells["G" + indexMain].Value = Convert.ToDouble(tongDoanhThu).ApplyFormatMoneyVNDToDouble();
                        worksheet.Cells["H" + indexMain].Value = Convert.ToDouble(tongMienGiam).ApplyFormatMoneyVNDToDouble();
                        worksheet.Cells["J" + indexMain].Value = Convert.ToDouble(tongBhyt).ApplyFormatMoneyVNDToDouble();
                        worksheet.Cells["K" + indexMain].Value = Convert.ToDouble(tongDoanhThuThuan).ApplyFormatMoneyVNDToDouble();

                        finalTongDoanhThu += tongDoanhThu;
                        finalTongMienGiam += tongMienGiam;
                        finalTongBhyt += tongBhyt;
                        finalTongDoanhThuChot += tongDoanhThuThuan;

                        for (int inde = indexMain + 1; inde <= index - 1; inde++)
                        {
                            worksheet.Row(inde).OutlineLevel = 1;
                        }

                        worksheet.Row(index).Height = 24;
                        index++;
                    }

                    worksheet.Cells["A" + index + ":K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A" + index + ":K" + index].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                    worksheet.Cells["A" + index + ":K" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["A" + index + ":K" + index].Style.Font.Bold = true;
                    worksheet.Cells["C" + index + ":E" + index].Merge = true;
                    worksheet.Cells["C" + index + ":E" + index].Value = "Tổng cộng:";
                    worksheet.Cells["G" + index].Value = Convert.ToDouble(finalTongDoanhThu).ApplyFormatMoneyVNDToDouble();
                    worksheet.Cells["H" + index].Value = Convert.ToDouble(finalTongMienGiam).ApplyFormatMoneyVNDToDouble();
                    worksheet.Cells["J" + index].Value = Convert.ToDouble(finalTongBhyt).ApplyFormatMoneyVNDToDouble();
                    worksheet.Cells["K" + index].Value = Convert.ToDouble(finalTongDoanhThuChot).ApplyFormatMoneyVNDToDouble();

                    worksheet.Cells["A" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["A" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["A" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["A" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    worksheet.Cells["B" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["B" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["B" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["B" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    worksheet.Cells["C" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["C" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["C" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["C" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    worksheet.Cells["D" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["D" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["D" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["D" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    worksheet.Cells["E" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["E" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["E" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["E" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    worksheet.Cells["F" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["F" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["F" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["F" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    worksheet.Cells["G" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["G" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["G" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["G" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    worksheet.Cells["H" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["H" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["H" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["H" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    worksheet.Cells["I" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["I" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["I" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["I" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    worksheet.Cells["J" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["J" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["J" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["J" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    worksheet.Cells["K" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["K" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["K" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["K" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlPackage.Save();
                }

                return stream.ToArray();
            }
        }



        #region Báo cáo thu tiền viện phí  

        public virtual byte[] ExportBaoCaoThuTienVienPhi(IList<BaoCaoThuPhiVienPhiGridVo> baoCaoThuPhiVienPhiGridVos, BaoCaoThuPhiVienPhiQueryInfoQueryInfo queryInfo, string tenNhanVien, string tenPhongBenhVien, string hosting, TotalBaoCaoThuPhiVienPhiGridVo datatotal)
        {
            var tuNgay = queryInfo.TuNgay == DateTime.MinValue ? DateTime.Now : queryInfo.TuNgay;
            var denNgay = queryInfo.DenNgay ?? DateTime.Now;

            var dataBaoCaos = baoCaoThuPhiVienPhiGridVos.OrderBy(s => s.Id).GroupBy(p => p.NguoiThu,
                              (key, g) => new { ThuNgan = key, Data = g.ToList() });

            int ind = 1;

            var requestProperties = new[]
            {
                //new PropertyByName<BaoCaoThuPhiVienPhiGridVo>("STT",0),
                new PropertyByName<BaoCaoThuPhiVienPhiGridVo>("Thu Ngân", p => p.NguoiThu)
            };

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BC05");

                    // set row
                    worksheet.Row(9).Height = 24.5;
                    worksheet.DefaultRowHeight = 16;

                    // set column
                    worksheet.Column(2).Width = 30;
                    worksheet.Column(3).Width = 30;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 20;
                    worksheet.Column(6).Width = 20;
                    worksheet.Column(7).Width = 20;
                    worksheet.Column(8).Width = 20;
                    worksheet.Column(9).Width = 15;
                    worksheet.Column(10).Width = 15;
                    worksheet.Column(11).Width = 15;
                    worksheet.Column(12).Width = 15;
                    worksheet.Column(13).Width = 15;
                    worksheet.Column(14).Width = 15;
                    worksheet.Column(15).Width = 15;
                    worksheet.Column(16).Width = 15;

                    worksheet.Column(17).Width = 15;
                    worksheet.Column(18).Width = 15;
                    worksheet.Column(19).Width = 15;
                    worksheet.Column(20).Width = 15;
                    worksheet.Column(21).Width = 15;
                    worksheet.Column(22).Width = 15;
                    worksheet.Column(23).Width = 15;
                    worksheet.Column(24).Width = 39;
                    worksheet.Column(25).Width = 15;
                    worksheet.Column(26).Width = 15;
                    worksheet.Column(27).Width = 15;

                    worksheet.DefaultColWidth = 7;

                    //set column 
                    string[] SetColumnItems = { "A", "B", "C", "D", "E", "F", "G", "H", "A", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W","X","Y", "Z"};
                    var worksheetTitleBacHa = SetColumnItems[0] + 1 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 1;
                    var worksheetTitleThuPhi = SetColumnItems[0] + 3 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 3;
                    var worksheetTitleNgay = SetColumnItems[0] + 4 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 4;
                    var worksheetTitleQuay = SetColumnItems[0] + 5 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 5;
                    using (var range = worksheet.Cells[worksheetTitleThuPhi])
                    {
                        range.Worksheet.Cells[worksheetTitleBacHa].Merge = true;
                        range.Worksheet.Cells[worksheetTitleBacHa].Value = "BỆNH VIỆN ĐKQT BẮC HÀ".ToUpper();
                        range.Worksheet.Cells[worksheetTitleBacHa].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells[worksheetTitleBacHa].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleBacHa].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells[worksheetTitleBacHa].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleBacHa].Style.Font.Bold = true;
                    }
                    using (var range = worksheet.Cells[worksheetTitleThuPhi])
                    {
                        range.Worksheet.Cells[worksheetTitleThuPhi].Merge = true;
                        range.Worksheet.Cells[worksheetTitleThuPhi].Value = "BÁO CÁO THU TIỀN VIỆN PHÍ".ToUpper();
                        range.Worksheet.Cells[worksheetTitleThuPhi].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleThuPhi].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleThuPhi].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells[worksheetTitleThuPhi].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleThuPhi].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleNgay])
                    {
                        range.Worksheet.Cells[worksheetTitleNgay].Merge = true;
                        range.Worksheet.Cells[worksheetTitleNgay].Value = "Từ ngày: " + queryInfo.TuNgay?.FormatNgayGioTimKiemTrenBaoCao() + " đến ngày: " + queryInfo.DenNgay?.FormatNgayGioTimKiemTrenBaoCao();
                        range.Worksheet.Cells[worksheetTitleNgay].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Bold = true;
                        //range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Italic = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleQuay])
                    {
                        range.Worksheet.Cells[worksheetTitleQuay].Merge = true;
                        range.Worksheet.Cells[worksheetTitleQuay].Value = "Nhân viên: " + tenNhanVien;
                        range.Worksheet.Cells[worksheetTitleQuay].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleQuay].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells[worksheetTitleQuay].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells[worksheetTitleQuay].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleQuay].Style.Font.Bold = true;
                        //range.Worksheet.Cells[worksheetTitleQuay].Style.Font.Italic = true;
                    }

                    using (var range = worksheet.Cells["A7:Z9"])
                    {
                        range.Worksheet.Cells["A7:Z9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A7:Z9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A7:Z9"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A7:Z9"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A7:Z9"].Style.Font.Bold = true;

                        //Set column A to I 
                        string[,] SetColumns = { { "A" , "STT" }, { "B", "NGÀY" } , { "C", "Số BL/HD" },
                           { "D", "TIÊM CHỦNG" }, { "E", "MÃ NB" },  { "F", "MÃ Y TẾ" }, { "G", "TÊN NGƯỜI BỆNH" }, { "H", "SỐ BỆNH ÁN" },
                           { "I", "NĂM SINH" },  { "J", "NGƯỜI GIỚI THIỆU" },
                           { "K", "TẠM ỨNG" } , { "L", "HOÀN ỨNG" }, { "M", "THU TIỀN" },
                           { "N", "HỦY/HOÀN" },  { "O", "GÓI DV " }, { "X", "CHI TIẾT CÔNG NỢ " }, { "Y", "SỐ HÓA ĐƠN" },
                           { "Z", "SƠ SINH" }, };

                        for (int i = 0; i < SetColumns.Length / 2; i++)
                        {
                            var setColumn = ((SetColumns[i, 0]).ToString() + 7 + ":" + (SetColumns[i, 0]).ToString() + 9).ToString();
                            range.Worksheet.Cells[setColumn].Merge = true;
                            range.Worksheet.Cells[setColumn].Value = SetColumns[i, 1];
                        }

                        //Set column K to O 
                        range.Worksheet.Cells["P7:S7"].Merge = true;
                        range.Worksheet.Cells["P7:S7"].Value = "HÌNH THỨC THANH TOÁN";

                        range.Worksheet.Cells["P9:P9"].Merge = true;
                        range.Worksheet.Cells["P8:P9"].Value = "CÔNG NỢ";

                        range.Worksheet.Cells["Q8:Q9"].Merge = true;
                        range.Worksheet.Cells["Q8:Q9"].Value = "POS";

                        range.Worksheet.Cells["R8:R9"].Merge = true;
                        range.Worksheet.Cells["R8:R8"].Value = "CHUYỂN KHOẢN";

                        range.Worksheet.Cells["S8:S9"].Merge = true;
                        range.Worksheet.Cells["S8:S9"].Value = "TIỀN MẶT";


                        //Set column K to RSTU
                        range.Worksheet.Cells["T7:W7"].Merge = true;
                        range.Worksheet.Cells["T7:W7"].Value = "CẬP NHẬT CÔNG NỢ";

                        range.Worksheet.Cells["T8:T9"].Merge = true;
                        range.Worksheet.Cells["T8:T9"].Value = "TIỀN MẶT";

                        range.Worksheet.Cells["U8:U9"].Merge = true;
                        range.Worksheet.Cells["U8:U9"].Value = "CHUYỂN KHOẢN";

                        range.Worksheet.Cells["V8:V9"].Merge = true;
                        range.Worksheet.Cells["V8:V9"].Value = "POS";

                        range.Worksheet.Cells["W8:W9"].Merge = true;
                        range.Worksheet.Cells["W8:W9"].Value = "SỐ PHIẾU THU";

                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        //range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        //range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        //range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    var manager = new PropertyManager<BaoCaoThuPhiVienPhiGridVo>(requestProperties);

                    int index =10;
                    decimal tongSoTienTamUng = 0;
                    decimal tongSoTienHoanUng = 0;
                    decimal tongSoTienHuyThu = 0;
                    decimal tongSoTienThu = 0;
                    decimal tongSoTienCongNo = 0;
                    decimal tongSoTienPos = 0;
                    decimal tongSoTienChuyenKhoan = 0;
                    decimal tongSoTienTienMat = 0;

                    decimal tongSoTienThuNoTienMat = 0;
                    decimal tongSoTienThuNoChuyenKhoan = 0;
                    decimal tongSoTienThuNoPos = 0;

                    var thuNgans = dataBaoCaos.Select(cc => cc.ThuNgan).ToArray();

                    var worksheetFirstLast = SetColumnItems[0] + index + ":" + SetColumnItems[SetColumnItems.Length - 1] + index;
                    for (int i = 0; i < thuNgans.Count(); i++)
                    {
                        manager.CurrentObject = baoCaoThuPhiVienPhiGridVos.Where(cc => cc.NguoiThu == thuNgans[i]).FirstOrDefault();
                        manager.WriteToXlsx(worksheet, index);

                        for (int ii = 0; ii < SetColumnItems.Length; ii++)
                        {
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            //worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            //worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            //worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Bold = true;
                        }

                        worksheet.Row(index).Height = 20.5;

                        var indexMain = index;
                        index++;


                        decimal tongSoTienTamUngRow = 0;
                        decimal tongSoTienHoanUngRow = 0;
                        decimal tongSoTienHuyThuRow = 0;
                        decimal tongSoTienThuRow = 0;
                        decimal tongSoTienCongNoRow = 0;
                        decimal tongSoTienPosRow = 0;
                        decimal tongSoTienChuyenKhoanRow = 0;
                        decimal tongSoTienTienMatRow = 0;

                        decimal tongSoTienThuNoTienMatRow = 0;
                        decimal tongSoTienThuNoChuyenKhoanRow = 0;
                        decimal tongSoTienThuNoPosRow = 0;


                        var dataByThuNgans = dataBaoCaos.Where(cc => cc.ThuNgan == thuNgans[i])
                                                        .SelectMany(cc => cc.Data);
                        int Stt = 1;
                        // item
                        var indexBeginThuNgan = index;
                        foreach (var baoCao in dataByThuNgans)
                        {

                            tongSoTienTamUng += baoCao.TamUng ?? 0;
                            tongSoTienHoanUng += baoCao.HoanUng ?? 0;
                            tongSoTienThu += baoCao.SoTienThu ?? 0;
                            tongSoTienHuyThu += baoCao.HuyThu ?? 0;
                            tongSoTienCongNo += baoCao.CongNo ?? 0;
                            tongSoTienPos += baoCao.Pos ?? 0;
                            tongSoTienChuyenKhoan += baoCao.ChuyenKhoan ?? 0;
                            tongSoTienTienMat += baoCao.TienMat ?? 0;

                            tongSoTienThuNoTienMat += baoCao.ThuNoTienMat ?? 0;
                            tongSoTienThuNoChuyenKhoan += baoCao.ThuNoChuyenKhoan ?? 0;
                            tongSoTienThuNoPos += baoCao.ThuNoPos ?? 0;

                            tongSoTienTamUngRow += baoCao.TamUng ?? 0;
                            tongSoTienHoanUngRow += baoCao.HoanUng ?? 0;
                            tongSoTienThuRow += baoCao.SoTienThu ?? 0;
                            tongSoTienHuyThuRow += baoCao.HuyThu ?? 0;
                            tongSoTienCongNoRow += baoCao.CongNo ?? 0;
                            tongSoTienPosRow += baoCao.Pos ?? 0;
                            tongSoTienChuyenKhoanRow += baoCao.ChuyenKhoan ?? 0;
                            tongSoTienTienMatRow += baoCao.TienMat ?? 0;

                            tongSoTienThuNoTienMatRow += baoCao.ThuNoTienMat ?? 0;
                            tongSoTienThuNoChuyenKhoanRow += baoCao.ThuNoChuyenKhoan ?? 0;
                            tongSoTienThuNoPosRow += baoCao.ThuNoPos ?? 0;


                            worksheet.Cells["A" + index].Value = Stt;
                            worksheet.Cells["B" + index].Value = baoCao.NgayThuStr;
                            worksheet.Cells["C" + index].Value = baoCao.SoBLHD;

                            worksheet.Cells["D" + index].Value = baoCao.CoTiemChung ? "X" : string.Empty;
                            worksheet.Cells["E" + index].Value = baoCao.MaBenhNhan;

                            worksheet.Cells["F" + index].Value = baoCao.MaYTe;
                            worksheet.Cells["G" + index].Value = baoCao.TenBenhNhan;
                            worksheet.Cells["H" + index].Value = baoCao.SoBenhAn;

                            worksheet.Cells["I" + index].Value = baoCao.NamSinh;
                            worksheet.Cells["J" + index].Value = baoCao.NguoiGioiThieu;

                            // FOR MAT 
                            worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";

                            worksheet.Cells["P" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["Q" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["R" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["S" + index].Style.Numberformat.Format = "#,##0.00";

                            worksheet.Cells["T" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["U" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["V" + index].Style.Numberformat.Format = "#,##0.00";

                            worksheet.Cells["W" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["X" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["Y" + index].Style.Numberformat.Format = "#,##0.00";

                            // BIND DATA
                            worksheet.Cells["K" + index].Value = Convert.ToDouble(baoCao.TamUng);
                            worksheet.Cells["L" + index].Value = Convert.ToDouble(baoCao.HoanUng);
                            worksheet.Cells["M" + index].Value = Convert.ToDouble(baoCao.SoTienThu);
                            worksheet.Cells["N" + index].Value = Convert.ToDouble(baoCao.HuyThu);
                            worksheet.Cells["O" + index].Value = baoCao.GoiDichVu ? "X" : string.Empty;


                            worksheet.Cells["P" + index].Value = Convert.ToDouble(baoCao.CongNo);
                            worksheet.Cells["Q" + index].Value = Convert.ToDouble(baoCao.Pos);
                            worksheet.Cells["R" + index].Value = Convert.ToDouble(baoCao.ChuyenKhoan);
                            worksheet.Cells["S" + index].Value = Convert.ToDouble(baoCao.TienMat);


                            worksheet.Cells["T" + index].Value = Convert.ToDouble(baoCao.ThuNoTienMat);
                            worksheet.Cells["U" + index].Value = Convert.ToDouble(baoCao.ThuNoChuyenKhoan);
                            worksheet.Cells["V" + index].Value = Convert.ToDouble(baoCao.ThuNoPos);
                            worksheet.Cells["W" + index].Value = baoCao.SoPhieuThuGhiNo;


                            worksheet.Cells["X" + index].Value = baoCao.ChiTietCongNo;
                            worksheet.Cells["Y" + index].Value = baoCao.SoHoaDonChiTiet;
                            worksheet.Cells["Z" + index].Value = baoCao.BenhAnSoSinh ? "X" : string.Empty ;


                            Stt++;

                            worksheet.Cells[$"{SetColumnItems[0]}{index}:{SetColumnItems[SetColumnItems.Length - 1]}{index}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells[$"{SetColumnItems[0]}{index}:{SetColumnItems[SetColumnItems.Length - 1]}{index}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            //worksheet.Cells[$"{SetColumnItems[0]}{index}:{SetColumnItems[SetColumnItems.Length - 1]}{index}"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                            if (baoCao.HuyThu != null && baoCao.HuyThu > 0)
                            {
                                worksheet.Cells[$"{SetColumnItems[0]}{index}:{SetColumnItems[SetColumnItems.Length - 1]}{index}"].Style.Font.Color.SetColor(Color.Red);
                            }

                            //if (baoCao.HuyThu != null)
                            //{
                            //    if (baoCao.HuyThu > 0)
                            //    {
                            //        for (int ii = 0; ii < SetColumnItems.Length; ii++)
                            //        {
                            //            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //            //worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            //            //worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            //            //worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            //            //worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            //            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                            //            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                            //            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Red);
                            //        }
                            //    }
                            //    else
                            //    {
                            //        for (int ii = 0; ii < SetColumnItems.Length; ii++)
                            //        {
                            //            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //            //worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            //            //worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            //            //worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            //            //worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            //            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            //            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                            //            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    for (int ii = 0; ii < SetColumnItems.Length; ii++)
                            //    {
                            //        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //        //worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            //        //worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            //        //worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            //        //worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            //        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            //        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                            //        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                            //    }
                            //}

                            index++;
                        }
                        
                        worksheet.Cells[$"{SetColumnItems[0]}{indexBeginThuNgan}:{SetColumnItems[SetColumnItems.Length - 1]}{index-1}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells[$"{SetColumnItems[0]}{indexBeginThuNgan}:{SetColumnItems[SetColumnItems.Length - 1]}{index-1}"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        for (int ii = 0; ii < SetColumnItems.Length; ii++)
                        {
                            worksheet.Cells[$"{SetColumnItems[ii]}{indexBeginThuNgan}:{SetColumnItems[ii]}{index-1}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        }

                        //tô màu vàng cho phân group    
                        worksheet.Cells["A" + indexMain + ":Z" + indexMain].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["A" + indexMain + ":Z" + indexMain].Style.Fill.BackgroundColor.SetColor(Color.Yellow);

                        // DINH NGHIA STYLE CHO 
                        worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";

                        worksheet.Cells["P" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["Q" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["R" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["S" + index].Style.Numberformat.Format = "#,##0.00";

                        worksheet.Cells["T" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["U" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["V" + index].Style.Numberformat.Format = "#,##0.00";

                        worksheet.Cells["W" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["X" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["Y" + index].Style.Numberformat.Format = "#,##0.00";
                        //

                        worksheet.Cells["K" + indexMain].Value = Convert.ToDouble(tongSoTienTamUngRow);
                        worksheet.Cells["L" + indexMain].Value = Convert.ToDouble(tongSoTienHoanUngRow);
                        worksheet.Cells["M" + indexMain].Value = Convert.ToDouble(tongSoTienThuRow);
                        worksheet.Cells["N" + indexMain].Value = Convert.ToDouble(tongSoTienHuyThuRow);

                        worksheet.Cells["P" + indexMain].Value = Convert.ToDouble(tongSoTienCongNoRow);
                        worksheet.Cells["Q" + indexMain].Value = Convert.ToDouble(tongSoTienPosRow);
                        worksheet.Cells["R" + indexMain].Value = Convert.ToDouble(tongSoTienChuyenKhoanRow);
                        worksheet.Cells["S" + indexMain].Value = Convert.ToDouble(tongSoTienTienMatRow);

                        worksheet.Cells["T" + indexMain].Value = Convert.ToDouble(tongSoTienThuNoTienMatRow);
                        worksheet.Cells["U" + indexMain].Value = Convert.ToDouble(tongSoTienThuNoChuyenKhoanRow);
                        worksheet.Cells["V" + indexMain].Value = Convert.ToDouble(tongSoTienThuNoPosRow);
                      

                        for (int inde = indexMain + 1; inde <= index - 1; inde++)
                        {
                            worksheet.Row(inde).OutlineLevel = 1;
                        }

                        index++;
                    }

                    worksheet.Cells[worksheetFirstLast].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                    worksheet.Cells[worksheetFirstLast].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells[worksheetFirstLast].Style.Font.Bold = true;
                    worksheet.Cells[worksheetFirstLast].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    worksheet.Cells[worksheetFirstLast].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;



                    for (int ii = 0; ii < SetColumnItems.Length; ii++)
                    {

                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Bold = true;
                    }

                    // total grid
                    if (datatotal != null)
                    {
                        using (var range = worksheet.Cells["B" + index + ":J" + index])
                        {
                            range.Worksheet.Cells["B" + index + ":J" + index].Merge = true;
                            range.Worksheet.Cells["B" + index + ":J" + index].Value = "Tổng cộng".ToUpper();
                        }
                        // STYLE 
                        // FOR MAT 
                        worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";

                        worksheet.Cells["P" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["Q" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["R" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["S" + index].Style.Numberformat.Format = "#,##0.00";

                        worksheet.Cells["T" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["U" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["V" + index].Style.Numberformat.Format = "#,##0.00";

                        worksheet.Cells["W" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["X" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["Y" + index].Style.Numberformat.Format = "#,##0.00";
                        //
                        worksheet.Cells["K" + index].Value = Convert.ToDouble(datatotal.TamUng);
                        worksheet.Cells["L" + index].Value = Convert.ToDouble(datatotal.HoanUng);
                        worksheet.Cells["M" + index].Value = Convert.ToDouble(datatotal.SoTienThu);
                        worksheet.Cells["N" + index].Value = Convert.ToDouble(datatotal.HuyThu);

                        worksheet.Cells["P" + index].Value = Convert.ToDouble(datatotal.CongNo);
                        worksheet.Cells["Q" + index].Value = Convert.ToDouble(datatotal.Pos);
                        worksheet.Cells["R" + index].Value = Convert.ToDouble(datatotal.ChuyenKhoan);
                        worksheet.Cells["S" + index].Value = Convert.ToDouble(datatotal.TienMat);

                        worksheet.Cells["T" + index].Value = Convert.ToDouble(datatotal.ThuNoTienMat);
                        worksheet.Cells["U" + index].Value = Convert.ToDouble(datatotal.ThuNoChuyenKhoan);
                        worksheet.Cells["V" + index].Value = Convert.ToDouble(datatotal.ThuNoPos);
                    }


                    xlPackage.Save();
                }

                return stream.ToArray();
            }
        }

        #endregion


        #region Báo cáo thu tiền người bệnh

        public virtual byte[] ExportBaoCaoThuTienBenhNhan(IList<BaoCaoChiTietThuPhiVienPhiBenhNhanGridVo> baoCaoThuPhiVienPhiGridVos, BaoCaoChiTietThuPhiVienPhiBenhNhanQueryInfo queryInfo, string tenNhanVien, string tenPhongBenhVien)
        {
            var tuNgay = queryInfo.TuNgay == DateTime.MinValue ? DateTime.Now : queryInfo.TuNgay;
            var denNgay = queryInfo.DenNgay ?? DateTime.Now;

            var dataBaoCaos = baoCaoThuPhiVienPhiGridVos.GroupBy(p => p.STT,
                              (key, g) => new { STT = key, Data = g.ToList() });

            int ind = 1;

            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoChiTietThuPhiVienPhiBenhNhanGridVo>("STT", p => ind++),
                new PropertyByName<BaoCaoChiTietThuPhiVienPhiBenhNhanGridVo>("Mã Tiếp Nhận", p => p.MaTiepNhan),
                new PropertyByName<BaoCaoChiTietThuPhiVienPhiBenhNhanGridVo>("Ngày thu", p => p.NgayThuStr),
                new PropertyByName<BaoCaoChiTietThuPhiVienPhiBenhNhanGridVo>("Mã Người Bệnh", p => p.MaBenhNhan),
                new PropertyByName<BaoCaoChiTietThuPhiVienPhiBenhNhanGridVo>("Tên Người Bệnh", p => p.TenBenhNhan)
            };
            const double minWidth = 0.00;
            const double maxWidth = 50.00;
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BC05");

                    // set row
                    worksheet.Row(9).Height = 24.5;
                    worksheet.DefaultRowHeight = 16;

                    // set column worksheet.Columns.AutoFit();
                    worksheet.Column(2).Width = 20;
                    worksheet.Column(3).Width = 25;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 20;
                    worksheet.Column(6).Width = 20;
                    worksheet.Column(7).Width = 20;
                    worksheet.Column(8).Width = 25;
                    worksheet.Column(9).Width = 15;
                    worksheet.Column(10).Width = 15;
                    worksheet.Column(11).Width = 25;
                    worksheet.Column(12).Width = 15;
                    worksheet.Column(13).Width = 15;
                    worksheet.Column(14).Width = 30;
                    worksheet.Column(15).Width = 15;
                    worksheet.Column(16).Width = 15;
                    worksheet.Column(17).Width = 15;
                    worksheet.Column(18).Width = 50;

                    worksheet.DefaultColWidth = 7;

                    //set column 
                    string[] SetColumnItems = { "A", "B", "C", "D", "E", "F", "G", "H", "A", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R" };
                    var worksheetTitleThuPhi = SetColumnItems[0] + 1 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 1;
                    var worksheetTitleNgay = SetColumnItems[0] + 2 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 2;
                    var worksheetTitleQuay = SetColumnItems[0] + 3 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 3;

                    var worksheetTitleHeader = SetColumnItems[0] + 6 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 8;
                    using (var range = worksheet.Cells[worksheetTitleThuPhi])
                    {
                        range.Worksheet.Cells[worksheetTitleThuPhi].Merge = true;
                        range.Worksheet.Cells[worksheetTitleThuPhi].Value = "BÁO CÁO CHI TIẾT THU TIỀN VIỆN PHÍ NGƯỜI BỆNH".ToUpper();
                        range.Worksheet.Cells[worksheetTitleThuPhi].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleThuPhi].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleThuPhi].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitleThuPhi].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleThuPhi].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleNgay])
                    {
                        range.Worksheet.Cells[worksheetTitleNgay].Merge = true;
                        range.Worksheet.Cells[worksheetTitleNgay].Value = "Từ ngày: " + tuNgay + " đến ngày: " + denNgay;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Bold = true;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Italic = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleQuay])
                    {
                        range.Worksheet.Cells[worksheetTitleQuay].Merge = true;
                        range.Worksheet.Cells[worksheetTitleQuay].Value = "Quầy thu: " + tenPhongBenhVien + "-  Nhân viên: " + tenNhanVien;
                        range.Worksheet.Cells[worksheetTitleQuay].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleQuay].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells[worksheetTitleQuay].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitleQuay].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleQuay].Style.Font.Bold = true;
                        range.Worksheet.Cells[worksheetTitleQuay].Style.Font.Italic = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleHeader])
                    {
                        range.Worksheet.Cells[worksheetTitleHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Bold = true;

                        string[,] SetColumns = { { "A" , "STT" }, { "B", "Mã tiếp nhận" } , { "C", "Ngày thu" },
                            { "D", "Mã người bệnh" }, { "E", "Tên người bệnh" }, { "F", "Số bệnh án" },
                            { "G", "Tên dịch vụ" } , { "H", "Bác sĩ chỉ định/thực hiện" }, { "I", "Doanh thu" },
                            { "J", "BHYT chi trả" },  { "K", "Bảo hiểm tư nhân chi trả" } , { "N", "Thu từ người bệnh " } , { "R", "Thu ngân" }};

                        for (int i = 0; i < SetColumns.Length / 2; i++)
                        {
                            var setColumn = ((SetColumns[i, 0]).ToString() + 6 + ":" + (SetColumns[i, 0]).ToString() + 8).ToString();
                            range.Worksheet.Cells[setColumn].Merge = true;
                            range.Worksheet.Cells[setColumn].Value = SetColumns[i, 1];
                        }

                        //Set column L to M
                        range.Worksheet.Cells["L6:M6"].Merge = true;
                        range.Worksheet.Cells["L6:M6"].Value = "Các khoản giảm trừ DT";
                        range.Worksheet.Cells["L7:L8"].Merge = true;
                        range.Worksheet.Cells["L7:L8"].Value = "Miễn giảm";
                        range.Worksheet.Cells["M7:M8"].Merge = true;
                        range.Worksheet.Cells["M7:M8"].Value = "Khác";

                        //Set column L to M
                        range.Worksheet.Cells["O6:Q6"].Merge = true;
                        range.Worksheet.Cells["O6:Q6"].Value = "Hình thức thanh toán";
                        range.Worksheet.Cells["O7:O8"].Merge = true;
                        range.Worksheet.Cells["O7:O8"].Value = "Tiền mặt";
                        range.Worksheet.Cells["P7:P8"].Merge = true;
                        range.Worksheet.Cells["P7:P8"].Value = "Chuyển khoản";
                        range.Worksheet.Cells["Q7:Q8"].Merge = true;
                        range.Worksheet.Cells["Q7:Q8"].Value = "POS";



                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    var manager = new PropertyManager<BaoCaoChiTietThuPhiVienPhiBenhNhanGridVo>(requestProperties);

                    int index = 9;

                    decimal tongSoTienDoanhThu = 0;
                    decimal tongSoTienBHYTChiTra = 0;
                    decimal tongSoTienBHTuNhanChiTra = 0;
                    decimal tongSoTienMienGiam = 0;
                    decimal tongSoTienKhac = 0;
                    decimal tongSoTienThuBenhNhan = 0;
                    decimal tongSoTienMat = 0;
                    decimal tongSoTienChuyenKhoan = 0;
                    decimal tongSoTienPos = 0;


                    var dataBySTT = dataBaoCaos.Select(cc => cc.STT).ToArray();

                    var worksheetFirstLast = SetColumnItems[0] + index + ":" + SetColumnItems[SetColumnItems.Length - 1] + index;
                    for (int i = 0; i < dataBySTT.Count(); i++)
                    {
                        manager.CurrentObject = baoCaoThuPhiVienPhiGridVos.Where(cc => cc.STT == dataBySTT[i]).FirstOrDefault();
                        manager.WriteToXlsx(worksheet, index);

                        for (int ii = 0; ii < SetColumnItems.Length; ii++)
                        {
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Bold = true;
                        }

                        worksheet.Row(index).Height = 20.5;

                        var indexMain = index;
                        index++;

                        decimal tongSoTienDoanhThuRow = 0;
                        decimal tongSoTienBHYTChiTraRow = 0;
                        decimal tongSoTienBHTuNhanChiTraRow = 0;
                        decimal tongSoTienMienGiamRow = 0;
                        decimal tongSoTienKhacRow = 0;
                        decimal tongSoTienThuBenhNhanRow = 0;
                        decimal tongSoTienMatRow = 0;
                        decimal tongSoTienChuyenKhoanRow = 0;
                        decimal tongSoTienPosRow = 0;


                        var dataByThuNgans = dataBaoCaos.Where(cc => cc.STT == dataBySTT[i])
                                                        .SelectMany(cc => cc.Data);
                        foreach (var baoCao in dataByThuNgans)
                        {
                            tongSoTienDoanhThu += baoCao.DoanhThu ?? 0;
                            tongSoTienBHYTChiTra += baoCao.BHYTChiTra ?? 0;
                            tongSoTienBHTuNhanChiTra += baoCao.BHYTTuNhanChiTra ?? 0;
                            tongSoTienMienGiam += baoCao.MiemGiam ?? 0;
                            tongSoTienKhac += baoCao.Voucher ?? 0;
                            tongSoTienThuBenhNhan += baoCao.ThuTuBenhNhan ?? 0;
                            tongSoTienMat += baoCao.TienMat ?? 0;
                            tongSoTienChuyenKhoan += baoCao.ChuyenKhoan ?? 0;
                            tongSoTienPos += baoCao.Pos ?? 0;


                            tongSoTienDoanhThuRow += baoCao.DoanhThu ?? 0;
                            tongSoTienBHYTChiTraRow += baoCao.BHYTChiTra ?? 0;
                            tongSoTienBHTuNhanChiTraRow += baoCao.BHYTTuNhanChiTra ?? 0;
                            tongSoTienMienGiamRow += baoCao.MiemGiam ?? 0;
                            tongSoTienKhacRow += baoCao.Voucher ?? 0;
                            tongSoTienThuBenhNhanRow += baoCao.ThuTuBenhNhan ?? 0;
                            tongSoTienMatRow += baoCao.TienMat ?? 0;
                            tongSoTienChuyenKhoanRow += baoCao.ChuyenKhoan ?? 0;
                            tongSoTienPosRow += baoCao.Pos ?? 0;

                            worksheet.Cells["F" + index].Value = baoCao.SoBenhAn;
                            worksheet.Cells["G" + index].Value = baoCao.TenDichVu;
                            worksheet.Cells["H" + index].Value = baoCao.BacSiChiDinhThucHien;
                            //
                            worksheet.Cells["H" + index + ":Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            worksheet.Cells["I" + index].Value = Convert.ToDouble(baoCao.DoanhThu).ApplyFormatMoneyVNDToDouble();
                            worksheet.Cells["J" + index].Value = Convert.ToDouble(baoCao.BHYTChiTra).ApplyFormatMoneyVNDToDouble();


                            worksheet.Cells["K" + index].Value = Convert.ToDouble(baoCao.BHYTTuNhanChiTra).ApplyFormatMoneyVNDToDouble();
                            worksheet.Cells["L" + index].Value = Convert.ToDouble(baoCao.MiemGiam).ApplyFormatMoneyVNDToDouble();
                            worksheet.Cells["M" + index].Value = Convert.ToDouble(baoCao.Voucher).ApplyFormatMoneyVNDToDouble();
                            worksheet.Cells["N" + index].Value = Convert.ToDouble(baoCao.ThuTuBenhNhan).ApplyFormatMoneyVNDToDouble();

                            worksheet.Cells["O" + index].Value = Convert.ToDouble(baoCao.TienMat).ApplyFormatMoneyVNDToDouble();
                            worksheet.Cells["P" + index].Value = Convert.ToDouble(baoCao.ChuyenKhoan).ApplyFormatMoneyVNDToDouble();
                            worksheet.Cells["Q" + index].Value = Convert.ToDouble(baoCao.Pos).ApplyFormatMoneyVNDToDouble();
                            worksheet.Cells["R" + index].Value = baoCao.NguoiThu;

                            for (int ii = 0; ii < SetColumnItems.Length; ii++)
                            {
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                worksheet.Cells["J" + index + ":J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                            }

                            index++;
                        }


                        using (var range = worksheet.Cells["I" + indexMain + ":Q" + indexMain])
                        {
                            worksheet.Cells["I" + indexMain + ":Q" + indexMain].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        }

                        //tô màu vàng cho phân group    
                        worksheet.Cells["A" + indexMain + ":R" + indexMain].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["A" + indexMain + ":R" + indexMain].Style.Fill.BackgroundColor.SetColor(Color.Yellow);

                        worksheet.Cells["I" + indexMain].Value = Convert.ToDouble(tongSoTienDoanhThuRow).ApplyFormatMoneyVNDToDouble();
                        worksheet.Cells["J" + indexMain].Value = Convert.ToDouble(tongSoTienBHYTChiTraRow).ApplyFormatMoneyVNDToDouble();
                        worksheet.Cells["K" + indexMain].Value = Convert.ToDouble(tongSoTienBHTuNhanChiTraRow).ApplyFormatMoneyVNDToDouble();
                        worksheet.Cells["L" + indexMain].Value = Convert.ToDouble(tongSoTienMienGiamRow).ApplyFormatMoneyVNDToDouble();
                        worksheet.Cells["M" + indexMain].Value = Convert.ToDouble(tongSoTienKhacRow).ApplyFormatMoneyVNDToDouble();
                        worksheet.Cells["N" + indexMain].Value = Convert.ToDouble(tongSoTienThuBenhNhanRow).ApplyFormatMoneyVNDToDouble();
                        worksheet.Cells["O" + indexMain].Value = Convert.ToDouble(tongSoTienMatRow).ApplyFormatMoneyVNDToDouble();
                        worksheet.Cells["P" + indexMain].Value = Convert.ToDouble(tongSoTienChuyenKhoanRow).ApplyFormatMoneyVNDToDouble();
                        worksheet.Cells["Q" + indexMain].Value = Convert.ToDouble(tongSoTienPosRow).ApplyFormatMoneyVNDToDouble();



                        for (int inde = indexMain + 1; inde <= index - 1; inde++)
                        {
                            worksheet.Row(inde).OutlineLevel = 1;
                        }

                        index++;
                    }

                    worksheet.Cells[worksheetFirstLast].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                    worksheet.Cells[worksheetFirstLast].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells[worksheetFirstLast].Style.Font.Bold = true;
                    worksheet.Cells[worksheetFirstLast].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;



                    for (int ii = 0; ii < SetColumnItems.Length; ii++)
                    {
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Bold = true;
                    }


                    using (var range = worksheet.Cells["C" + index + ":Q" + index])
                    {
                        range.Worksheet.Cells["C" + index + ":E" + index].Merge = true;
                        range.Worksheet.Cells["C" + index + ":E" + index].Value = "Tổng cộng".ToUpper();
                        worksheet.Cells["C" + index + ":Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }

                    worksheet.Cells["I" + index].Value = Convert.ToDouble(tongSoTienDoanhThu).ApplyFormatMoneyVNDToDouble();
                    worksheet.Cells["J" + index].Value = Convert.ToDouble(tongSoTienBHYTChiTra).ApplyFormatMoneyVNDToDouble();
                    worksheet.Cells["K" + index].Value = Convert.ToDouble(tongSoTienBHTuNhanChiTra).ApplyFormatMoneyVNDToDouble();

                    worksheet.Cells["L" + index].Value = Convert.ToDouble(tongSoTienMienGiam).ApplyFormatMoneyVNDToDouble();
                    worksheet.Cells["M" + index].Value = Convert.ToDouble(tongSoTienKhac).ApplyFormatMoneyVNDToDouble();
                    worksheet.Cells["N" + index].Value = Convert.ToDouble(tongSoTienThuBenhNhan).ApplyFormatMoneyVNDToDouble();

                    worksheet.Cells["O" + index].Value = Convert.ToDouble(tongSoTienMat).ApplyFormatMoneyVNDToDouble();
                    worksheet.Cells["P" + index].Value = Convert.ToDouble(tongSoTienChuyenKhoan).ApplyFormatMoneyVNDToDouble();
                    worksheet.Cells["Q" + index].Value = Convert.ToDouble(tongSoTienPos).ApplyFormatMoneyVNDToDouble();

                    xlPackage.Save();
                }

                return stream.ToArray();
            }
        }


        #endregion

        #region manager view

        public byte[] ExportManagermentView<T>(List<T> lstModel, List<(string, string)> valueObject, string titleName, int indexStartChildGrid, string labelName = null, bool isAutomaticallyIncreasesSTT = false)
        {
            //
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {

                    var classChildName = typeof(T).Name + Constants.ExportManagerment.ClassChildName;
                    var worksheet = xlPackage.Workbook.Worksheets.Add(titleName);


                    //set column header
                    worksheet.Cells[4, 1].Value = "STT";
                    worksheet = SetStyleForHeader(worksheet, 4, 1);

                    #region get group value
                    //check parent grid have group
                    var keyNamePropertyGroupParent = string.Empty;
                    var lstGroupStringParent = new List<string>();

                    var keyNamePropertyGroupChildrent = string.Empty;
                    var lstGroupStringChildrent = new List<string>();
                    var totalColumnChildrent = 1;

                    var typeParentForCheckGroup = typeof(T);
                    foreach (var prop in typeParentForCheckGroup.GetProperties())
                    {
                        if (prop.CustomAttributes.Any(p => p.AttributeType == typeof(GroupAttribute)))
                        {
                            keyNamePropertyGroupParent = prop.Name;
                        }

                        if (classChildName.Equals(prop.Name))
                        {
                            var typeOfClassChild = prop.PropertyType.GetProperty("Item")?.PropertyType;
                            if (typeOfClassChild != null)
                            {
                                foreach (var propChild in typeOfClassChild.GetProperties())
                                {
                                    if (propChild.CustomAttributes.Any(p => p.AttributeType == typeof(GroupAttribute)))
                                    {
                                        keyNamePropertyGroupChildrent = propChild.Name;
                                    }
                                    else
                                    {
                                        totalColumnChildrent++;
                                    }
                                }
                            }

                        }
                    }

                    //set title for excel
                    var rangeObject = valueObject.Where(p => p.Item1 != keyNamePropertyGroupParent).ToList().Count + 1;
                    var totalColumnParent = valueObject.Where(p => p.Item1 != keyNamePropertyGroupParent).ToList().Count + 1;

                    var labelNameTitle = !string.IsNullOrEmpty(labelName) ? labelName.ToUpper() : ("Danh mục " + titleName).ToUpper();

                    using (var range = worksheet.Cells[1, 2, lstModel.Count + 1, 2])
                    {
                        range.Worksheet.Cells[3, 1, 3, rangeObject].Merge = true;
                        range.Worksheet.Cells[3, 1, 3, rangeObject].Value = labelNameTitle;
                        range.Worksheet.Cells[3, 1, 3, rangeObject].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[3, 1, 3, rangeObject].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells[3, 1, 3, rangeObject].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[3, 1, 3, rangeObject].Style.Font.Bold = true;
                    }

                    if (!string.IsNullOrEmpty(keyNamePropertyGroupParent) || !string.IsNullOrEmpty(keyNamePropertyGroupChildrent))
                    {
                        foreach (var item in lstModel)
                        {
                            foreach (var column in valueObject)
                            {
                                var columnKey = column.Item1;

                                //child
                                if (classChildName.Equals(columnKey))
                                {
                                    var columnValue = GetPropValue(item, columnKey);
                                    var totalChildItemObject = GetPropValue(columnValue, "Count").ToString();
                                    int.TryParse(totalChildItemObject, out var totalChildItem);

                                    //is have item child
                                    if (totalChildItem != 0)
                                    {
                                        var typeOfChildItem = columnValue.GetType().GetProperty("Item")?.PropertyType;
                                        if (typeOfChildItem != null)
                                        {
                                            var enumerable = columnValue as IEnumerable;
                                            var lstItemChild = enumerable?.Cast<object>().ToList();
                                            if (lstItemChild != null)
                                            {
                                                foreach (var itemChild in lstItemChild)
                                                {
                                                    foreach (var propChild in itemChild.GetType().GetProperties())
                                                    {
                                                        if (!propChild.CanRead || !propChild.CanWrite)
                                                            continue;
                                                        if (keyNamePropertyGroupChildrent.Equals(propChild.Name))
                                                        {
                                                            var columnChildValue = GetPropValue(itemChild, propChild.Name);
                                                            if (!lstGroupStringChildrent.Any(p => p.Equals(columnChildValue)))
                                                            {
                                                                lstGroupStringChildrent.Add(columnChildValue + "");
                                                            }
                                                        }

                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                //parent
                                else
                                {
                                    var type = typeof(T);
                                    foreach (var prop in type.GetProperties())
                                    {
                                        if (!prop.CanRead || !prop.CanWrite)
                                            continue;
                                        if (prop.Name.Equals(columnKey) && keyNamePropertyGroupParent.Equals(columnKey))
                                        {
                                            var columnValue = GetPropValue(item, columnKey);
                                            if (!lstGroupStringParent.Any(p => p.Equals(columnValue)))
                                            {
                                                lstGroupStringParent.Add(columnValue + "");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //
                    #endregion get group value

                    if (string.IsNullOrEmpty(keyNamePropertyGroupParent) && !lstGroupStringParent.Any())
                    {
                        #region without group of parent

                        var index = 2;
                        foreach (var column in valueObject)
                        {
                            var columnName = column.Item2;
                            var columnKey = column.Item1;

                            if (!classChildName.Equals(columnKey))
                            {
                                worksheet.Cells[4, index].Value = columnName;
                                worksheet = SetStyleForHeader(worksheet, 4, index);

                                index++;
                            }
                        }
                        //index value start is 5
                        var indexValue = 5;
                        var STT = 1;

                        foreach (var item in lstModel)
                        {
                            //STT
                            worksheet.Cells[indexValue, 1].Value = STT;
                            worksheet = SetStyleForDataTable(worksheet, indexValue, 1);
                            //index value start is A
                            var indexColumn = 2;

                            foreach (var column in valueObject)
                            {
                                //
                                var columnKey = column.Item1;
                                double height = 20;
                                double width = 20;
                                //
                                var type = typeof(T);
                                foreach (var prop in type.GetProperties())
                                {
                                    if (!prop.CanRead || !prop.CanWrite)
                                        continue;
                                    if (prop.Name.Equals(columnKey) && prop.CustomAttributes.Any())
                                    {
                                        double.TryParse(
                                            prop.CustomAttributes.FirstOrDefault(p => p.AttributeType == typeof(WidthAttribute))?
                                            .ConstructorArguments.Select(x => x.Value).ToList()[0] +
                                            "", out height);
                                        double.TryParse(
                                            prop.CustomAttributes.FirstOrDefault(p => p.AttributeType == typeof(WidthAttribute))?
                                                .ConstructorArguments.Select(x => x.Value).ToList()[0] +
                                            "", out width);
                                    }
                                }
                                //
                                if (!classChildName.Equals(columnKey))
                                {
                                    //
                                    var attributeTextAlign = GetTextAlignAttributeValue(item, columnKey);
                                    worksheet = SetStyleForDataTable(worksheet, indexValue, indexColumn, attributeTextAlign);
                                    //
                                    worksheet.Column(indexColumn).Width = width;
                                    var columnValue = GetPropValue(item, columnKey);
                                    worksheet.Cells[indexValue, indexColumn].Value = columnValue;
                                }
                                //set value for grid child
                                else
                                {
                                    var columnValue = GetPropValue(item, columnKey);
                                    var totalChildItemObject = GetPropValue(columnValue, "Count").ToString();
                                    int.TryParse(totalChildItemObject, out var totalChildItem);

                                    //is have item child
                                    if (totalChildItem != 0)
                                    {
                                        var typeOfChildItem = columnValue.GetType().GetProperty("Item")?.PropertyType;
                                        if (typeOfChildItem != null)
                                        {
                                            //Set header for grid child
                                            indexValue++;
                                            //STT for grid child
                                            worksheet.Cells[indexValue, indexStartChildGrid].Value = "STT";
                                            worksheet = SetStyleForHeader(worksheet, indexValue, indexStartChildGrid);


                                            var indexColumnChild = indexStartChildGrid + 1;

                                            foreach (var propChild in typeOfChildItem.GetProperties())
                                            {
                                                //Reject header of group
                                                if (propChild.Name.Equals(keyNamePropertyGroupChildrent)) continue;
                                                //
                                                if (propChild.CustomAttributes.Any())
                                                {
                                                    var titleChild = propChild.CustomAttributes.FirstOrDefault(p => p.AttributeType == typeof(TitleGridChildAttribute))?
                                                                         .ConstructorArguments.Select(x => x.Value).ToList()[0] + "";

                                                    worksheet.Cells[indexValue, indexColumnChild].Value = titleChild;
                                                    worksheet = SetStyleForHeader(worksheet, indexValue, indexColumnChild);

                                                    int.TryParse(propChild.CustomAttributes.FirstOrDefault(p =>
                                                                         p.AttributeType == typeof(WidthAttribute))?
                                                                     .ConstructorArguments.Select(x => x.Value).ToList()[0] + "", out var widthChild);

                                                    if (widthChild != 0)
                                                    {
                                                        worksheet.Column(indexColumnChild).Width = widthChild;
                                                    }

                                                    indexColumnChild++;
                                                }
                                            }

                                            if (string.IsNullOrEmpty(keyNamePropertyGroupChildrent) && !lstGroupStringChildrent.Any())
                                            {
                                                #region without group of childrent

                                                indexValue++;
                                                //Set item for grid child
                                                var enumerable = columnValue as IEnumerable;
                                                var lstItemChild = enumerable?.Cast<object>().ToList();
                                                if (lstItemChild != null)
                                                {
                                                    var STTChild = 1;
                                                    var countItemChild = 0;
                                                    foreach (var itemChild in lstItemChild)
                                                    {
                                                        var indexColumnChildItem = indexStartChildGrid + 1;
                                                        //STT Child
                                                        worksheet.Cells[indexValue, indexStartChildGrid].Value = STTChild;
                                                        worksheet = SetStyleForDataTable(worksheet, indexValue, indexStartChildGrid);

                                                        foreach (var propChild in itemChild.GetType().GetProperties())
                                                        {
                                                            var columnChildValue = GetPropValue(itemChild, propChild.Name);
                                                            //set value item
                                                            worksheet.Cells[indexValue, indexColumnChildItem].Value = columnChildValue;

                                                            //
                                                            var attributeTextAlign = GetTextAlignAttributeValue(itemChild, propChild.Name);
                                                            worksheet = SetStyleForDataTable(worksheet, indexValue, indexColumnChildItem, attributeTextAlign);
                                                            //

                                                            //worksheet = SetStyleForDataTable(worksheet, indexValue, indexColumnChildItem);

                                                            indexColumnChildItem++;
                                                        }

                                                        //set last column for value
                                                        worksheet.Cells[indexValue, indexColumnChildItem].Value = "";

                                                        STTChild++;
                                                        countItemChild++;

                                                        if (countItemChild < totalChildItem)
                                                        {
                                                            indexValue++;
                                                        }
                                                    }
                                                }
                                                #endregion without group of childrent
                                            }
                                            else
                                            {
                                                #region with group of childrent

                                                indexValue++;
                                                //Set item for grid child
                                                var enumerable = columnValue as IEnumerable;
                                                var lstItemChild = enumerable?.Cast<object>().ToList();
                                                if (lstItemChild != null)
                                                {
                                                    //group

                                                    //get group name of child grid
                                                    var lstGroupStringChildrentCurrent = new List<string>();
                                                    foreach (var itemChild in lstItemChild)
                                                    {
                                                        foreach (var propChild in itemChild.GetType().GetProperties())
                                                        {
                                                            if (!propChild.CanRead || !propChild.CanWrite)
                                                                continue;
                                                            if (keyNamePropertyGroupChildrent.Equals(propChild.Name))
                                                            {
                                                                var columnChildValue = GetPropValue(itemChild, propChild.Name);
                                                                if (!lstGroupStringChildrentCurrent.Any(p => p.Equals(columnChildValue)))
                                                                {
                                                                    lstGroupStringChildrentCurrent.Add(columnChildValue + "");
                                                                }
                                                            }

                                                        }
                                                    }
                                                    //

                                                    var countItemChild = 0;
                                                    foreach (var groupChildName in lstGroupStringChildrentCurrent)
                                                    {

                                                        var STTChild = 1;
                                                        //set group name
                                                        worksheet.Cells[indexValue, indexStartChildGrid, indexValue,
                                                            indexStartChildGrid + totalColumnChildrent].Merge = true;
                                                        worksheet.Cells[indexValue, indexStartChildGrid, indexValue,
                                                            indexStartChildGrid + totalColumnChildrent].Value = groupChildName;
                                                        worksheet = SetStyleForGroup(worksheet, indexValue, indexStartChildGrid, indexValue,
                                                            indexStartChildGrid + totalColumnChildrent);

                                                        indexValue++;
                                                        //
                                                        foreach (var itemChild in lstItemChild)
                                                        {
                                                            var groupNameItemChild = GetPropValue(itemChild, keyNamePropertyGroupChildrent);
                                                            if (groupNameItemChild.Equals(groupChildName))
                                                            {
                                                                var indexColumnChildItem = indexStartChildGrid + 1;
                                                                //STT Child
                                                                worksheet.Cells[indexValue, indexStartChildGrid].Value = STTChild;
                                                                worksheet = SetStyleForDataTable(worksheet, indexValue, indexStartChildGrid);

                                                                foreach (var propChild in itemChild.GetType().GetProperties())
                                                                {
                                                                    //Reject column grid
                                                                    if (propChild.Name.Equals(keyNamePropertyGroupChildrent)) continue;
                                                                    //
                                                                    var columnChildValue = GetPropValue(itemChild, propChild.Name);
                                                                    //set value item
                                                                    worksheet.Cells[indexValue, indexColumnChildItem].Value = columnChildValue;
                                                                    //worksheet = SetStyleForDataTable(worksheet, indexValue, indexColumnChildItem);
                                                                    //
                                                                    var attributeTextAlign = GetTextAlignAttributeValue(itemChild, propChild.Name);
                                                                    worksheet = SetStyleForDataTable(worksheet, indexValue, indexColumnChildItem, attributeTextAlign);
                                                                    //

                                                                    indexColumnChildItem++;
                                                                }

                                                                //set last column for value
                                                                worksheet.Cells[indexValue, indexColumnChildItem].Value = "";

                                                                STTChild++;
                                                                countItemChild++;
                                                                if (countItemChild < totalChildItem)
                                                                {
                                                                    indexValue++;
                                                                }
                                                            }

                                                        }
                                                        //indexValue--;
                                                    }
                                                    //

                                                }
                                                #endregion with group of childrent
                                            }


                                            // var xm = JsonConvert.DeserializeObject<List<BenhNhanExportExcelChild>>((string)columnValue);
                                        }
                                    }
                                    //foreach (var itemChild in columnValue.)
                                    //{

                                    //}
                                }


                                indexColumn++;
                            }

                            //đm éo hiểu trước mình code kiểu gì
                            //worksheet.Cells[indexValue, indexColumn].Value = "";

                            indexValue++;
                            STT++;
                        }

                        #endregion without group of parent
                    }
                    else
                    {
                        #region with group of parent

                        var index = 2;
                        foreach (var column in valueObject)
                        {
                            var columnName = column.Item2;
                            var columnKey = column.Item1;
                            //Reject header of group
                            if (columnKey.Equals(keyNamePropertyGroupParent)) continue;
                            //
                            if (!classChildName.Equals(columnKey))
                            {
                                worksheet.Cells[4, index].Value = columnName;
                                worksheet = SetStyleForHeader(worksheet, 4, index);

                                index++;
                            }
                        }
                        //index value start is 5
                        var indexValue = 5;

                        //group for parent grid
                        var STT = 1;
                        foreach (var groupParentName in lstGroupStringParent)
                        {
                            //set group name
                            worksheet.Cells[indexValue, 1, indexValue,
                                totalColumnParent].Merge = true;
                            worksheet.Cells[indexValue, 1, indexValue,
                                totalColumnParent].Value = groupParentName;
                            worksheet = SetStyleForGroup(worksheet, indexValue, 1, indexValue,
                                totalColumnParent);

                            indexValue++;
                            //
                            if (!isAutomaticallyIncreasesSTT)//STT tăng liên tục
                            {
                                STT = 1;
                            }
                            foreach (var item in lstModel)
                            {
                                var groupNameItem = GetPropValue(item, keyNamePropertyGroupParent);
                                if (!groupNameItem.Equals(groupParentName)) continue;

                                //STT
                                worksheet.Cells[indexValue, 1].Value = STT;
                                worksheet = SetStyleForDataTable(worksheet, indexValue, 1);
                                //index value start is A
                                var indexColumn = 2;

                                foreach (var column in valueObject)
                                {
                                    //
                                    var columnKey = column.Item1;
                                    double height = 20;
                                    double width = 20;
                                    //
                                    if (columnKey.Equals(keyNamePropertyGroupParent)) continue;
                                    //
                                    var type = typeof(T);
                                    foreach (var prop in type.GetProperties())
                                    {
                                        if (!prop.CanRead || !prop.CanWrite)
                                            continue;
                                        if (prop.Name.Equals(columnKey) && prop.CustomAttributes.Any())
                                        {
                                            double.TryParse(
                                                prop.CustomAttributes.FirstOrDefault(p => p.AttributeType == typeof(WidthAttribute))?
                                                .ConstructorArguments.Select(x => x.Value).ToList()[0] +
                                                "", out height);
                                            double.TryParse(
                                                prop.CustomAttributes.FirstOrDefault(p => p.AttributeType == typeof(WidthAttribute))?
                                                    .ConstructorArguments.Select(x => x.Value).ToList()[0] +
                                                "", out width);
                                        }
                                    }
                                    //
                                    if (!classChildName.Equals(columnKey))
                                    {
                                        worksheet.Column(indexColumn).Width = width;
                                        //worksheet = SetStyleForDataTable(worksheet, indexValue, indexColumn);
                                        //
                                        var attributeTextAlign = GetTextAlignAttributeValue(item, columnKey);
                                        worksheet = SetStyleForDataTable(worksheet, indexValue, indexColumn, attributeTextAlign);
                                        //
                                        var columnValue = GetPropValue(item, columnKey);
                                        worksheet.Cells[indexValue, indexColumn].Value = columnValue;
                                    }
                                    //set value for grid child
                                    else
                                    {
                                        var columnValue = GetPropValue(item, columnKey);
                                        var totalChildItemObject = GetPropValue(columnValue, "Count").ToString();
                                        int.TryParse(totalChildItemObject, out var totalChildItem);

                                        //is have item child
                                        if (totalChildItem != 0)
                                        {
                                            var typeOfChildItem = columnValue.GetType().GetProperty("Item")?.PropertyType;
                                            if (typeOfChildItem != null)
                                            {
                                                //Set header for grid child
                                                indexValue++;
                                                //STT for grid child
                                                worksheet.Cells[indexValue, indexStartChildGrid].Value = "STT";
                                                worksheet = SetStyleForHeader(worksheet, indexValue, indexStartChildGrid);


                                                var indexColumnChild = indexStartChildGrid + 1;

                                                foreach (var propChild in typeOfChildItem.GetProperties())
                                                {
                                                    if (propChild.Name.Equals(keyNamePropertyGroupChildrent)) continue;
                                                    if (propChild.CustomAttributes.Any())
                                                    {
                                                        var titleChild = propChild.CustomAttributes.FirstOrDefault(p => p.AttributeType == typeof(TitleGridChildAttribute))?
                                                                             .ConstructorArguments.Select(x => x.Value).ToList()[0] + "";

                                                        worksheet.Cells[indexValue, indexColumnChild].Value = titleChild;
                                                        worksheet = SetStyleForHeader(worksheet, indexValue, indexColumnChild);

                                                        int.TryParse(propChild.CustomAttributes.FirstOrDefault(p =>
                                                                             p.AttributeType == typeof(WidthAttribute))?
                                                                         .ConstructorArguments.Select(x => x.Value).ToList()[0] + "", out var widthChild);

                                                        if (widthChild != 0)
                                                        {
                                                            worksheet.Column(indexColumnChild).Width = widthChild;
                                                        }

                                                        indexColumnChild++;
                                                    }
                                                }

                                                if (string.IsNullOrEmpty(keyNamePropertyGroupChildrent) && !lstGroupStringChildrent.Any())
                                                {
                                                    #region without group of childrent

                                                    indexValue++;
                                                    //Set item for grid child
                                                    var enumerable = columnValue as IEnumerable;
                                                    var lstItemChild = enumerable?.Cast<object>().ToList();
                                                    if (lstItemChild != null)
                                                    {
                                                        var STTChild = 1;
                                                        var countItemChild = 0;
                                                        foreach (var itemChild in lstItemChild)
                                                        {
                                                            var indexColumnChildItem = indexStartChildGrid + 1;
                                                            //STT Child
                                                            worksheet.Cells[indexValue, indexStartChildGrid].Value = STTChild;
                                                            worksheet = SetStyleForDataTable(worksheet, indexValue, indexStartChildGrid);

                                                            foreach (var propChild in itemChild.GetType().GetProperties())
                                                            {
                                                                var columnChildValue = GetPropValue(itemChild, propChild.Name);
                                                                //set value item
                                                                worksheet.Cells[indexValue, indexColumnChildItem].Value = columnChildValue;
                                                                //worksheet = SetStyleForDataTable(worksheet, indexValue, indexColumnChildItem);
                                                                //
                                                                var attributeTextAlign = GetTextAlignAttributeValue(itemChild, propChild.Name);
                                                                worksheet = SetStyleForDataTable(worksheet, indexValue, indexColumnChildItem, attributeTextAlign);
                                                                //

                                                                indexColumnChildItem++;
                                                            }

                                                            //set last column for value
                                                            worksheet.Cells[indexValue, indexColumnChildItem].Value = "";

                                                            STTChild++;
                                                            countItemChild++;

                                                            if (countItemChild < totalChildItem)
                                                            {
                                                                indexValue++;
                                                            }
                                                        }
                                                    }
                                                    #endregion without group of childrent
                                                }
                                                else
                                                {
                                                    #region with group of childrent

                                                    indexValue++;
                                                    //Set item for grid child
                                                    var enumerable = columnValue as IEnumerable;
                                                    var lstItemChild = enumerable?.Cast<object>().ToList();
                                                    if (lstItemChild != null)
                                                    {
                                                        //group

                                                        //get group name of child grid
                                                        var lstGroupStringChildrentCurrent = new List<string>();
                                                        foreach (var itemChild in lstItemChild)
                                                        {
                                                            foreach (var propChild in itemChild.GetType().GetProperties())
                                                            {
                                                                //if (propChild.Name.Equals(keyNamePropertyGroupChildrent)) continue;
                                                                if (!propChild.CanRead || !propChild.CanWrite)
                                                                    continue;
                                                                if (keyNamePropertyGroupChildrent.Equals(propChild.Name))
                                                                {
                                                                    var columnChildValue = GetPropValue(itemChild, propChild.Name);
                                                                    if (!lstGroupStringChildrentCurrent.Any(p => p.Equals(columnChildValue)))
                                                                    {
                                                                        lstGroupStringChildrentCurrent.Add(columnChildValue + "");
                                                                    }
                                                                }

                                                            }
                                                        }
                                                        //
                                                        var countItemChild = 0;

                                                        foreach (var groupChildName in lstGroupStringChildrentCurrent)
                                                        {
                                                            var STTChild = 1;
                                                            //set group name
                                                            worksheet.Cells[indexValue, indexStartChildGrid, indexValue,
                                                                indexStartChildGrid + totalColumnChildrent].Merge = true;
                                                            worksheet.Cells[indexValue, indexStartChildGrid, indexValue,
                                                                indexStartChildGrid + totalColumnChildrent].Value = groupChildName;
                                                            worksheet = SetStyleForGroup(worksheet, indexValue, indexStartChildGrid, indexValue,
                                                                indexStartChildGrid + totalColumnChildrent);

                                                            indexValue++;
                                                            //
                                                            foreach (var itemChild in lstItemChild)
                                                            {
                                                                var groupNameItemChild = GetPropValue(itemChild, keyNamePropertyGroupChildrent);
                                                                if (groupNameItemChild.Equals(groupChildName))
                                                                {
                                                                    var indexColumnChildItem = indexStartChildGrid + 1;
                                                                    //STT Child
                                                                    worksheet.Cells[indexValue, indexStartChildGrid].Value = STTChild;
                                                                    worksheet = SetStyleForDataTable(worksheet, indexValue, indexStartChildGrid);

                                                                    foreach (var propChild in itemChild.GetType().GetProperties())
                                                                    {
                                                                        //
                                                                        if (propChild.Name.Equals(keyNamePropertyGroupChildrent)) continue;
                                                                        //
                                                                        var columnChildValue = GetPropValue(itemChild, propChild.Name);
                                                                        //set value item
                                                                        worksheet.Cells[indexValue, indexColumnChildItem].Value = columnChildValue;
                                                                        //worksheet = SetStyleForDataTable(worksheet, indexValue, indexColumnChildItem);
                                                                        //
                                                                        var attributeTextAlign = GetTextAlignAttributeValue(itemChild, propChild.Name);
                                                                        worksheet = SetStyleForDataTable(worksheet, indexValue, indexColumnChildItem, attributeTextAlign);
                                                                        //

                                                                        indexColumnChildItem++;
                                                                    }

                                                                    //set last column for value
                                                                    worksheet.Cells[indexValue, indexColumnChildItem].Value = "";

                                                                    STTChild++;
                                                                    countItemChild++;
                                                                    if (countItemChild < totalChildItem)
                                                                    {
                                                                        indexValue++;
                                                                    }
                                                                }
                                                            }
                                                            //indexValue--;
                                                        }
                                                        //

                                                    }
                                                    #endregion with group of childrent
                                                }


                                                // var xm = JsonConvert.DeserializeObject<List<BenhNhanExportExcelChild>>((string)columnValue);
                                            }
                                        }
                                        //foreach (var itemChild in columnValue.)
                                        //{

                                        //}
                                    }


                                    indexColumn++;
                                }

                                worksheet.Cells[indexValue, indexColumn].Value = "";

                                indexValue++;
                                STT++;
                            }
                        }


                        #endregion with group of parent
                    }

                    xlPackage.Save();
                }

                return stream.ToArray();
            }
        }


        private ExcelWorksheet SetStyleForHeader(ExcelWorksheet worksheet, int startCell = 0, int endCell = 0)
        {
            worksheet.Cells[startCell, endCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[startCell, endCell].Style.Font.SetFromFont(new Font("Times New Roman", 13));
            worksheet.Cells[startCell, endCell].Style.Font.Color.SetColor(Color.Black);
            worksheet.Cells[startCell, endCell].Style.Font.Bold = true;
            worksheet.Cells[startCell, endCell].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[startCell, endCell].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[startCell, endCell].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[startCell, endCell].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            return worksheet;
        }

        private ExcelWorksheet SetStyleForDataTable(ExcelWorksheet worksheet, int startCell = 0, int endCell = 0, string horizontalString = null)
        {
            var horizontal = ExcelHorizontalAlignment.Left;
            if (!string.IsNullOrEmpty(horizontalString))
            {
                switch (horizontalString)
                {
                    case Constants.TextAlignAttribute.Right:
                        horizontal = ExcelHorizontalAlignment.Right;
                        break;
                    case Constants.TextAlignAttribute.Left:
                        horizontal = ExcelHorizontalAlignment.Left;
                        break;
                    case Constants.TextAlignAttribute.Center:
                        horizontal = ExcelHorizontalAlignment.Center;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            worksheet.Cells[startCell, endCell].Style.HorizontalAlignment = horizontal;
            worksheet.Cells[startCell, endCell].Style.Font.SetFromFont(new Font("Times New Roman", 13));
            worksheet.Cells[startCell, endCell].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[startCell, endCell].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[startCell, endCell].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[startCell, endCell].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            return worksheet;
        }

        private ExcelWorksheet SetStyleForGroup(ExcelWorksheet worksheet, int fromStartCell = 0, int fromEndCell = 0, int toStartCell = 0, int toEndCell = 0)
        {
            worksheet.Cells[fromStartCell, fromEndCell, toStartCell, toEndCell].Style.Font
                .Color
                .SetColor(Color.Black);
            worksheet.Cells[fromStartCell, fromEndCell, toStartCell, toEndCell].Style.Font.SetFromFont(new Font("Times New Roman", 13));
            worksheet.Cells[fromStartCell, fromEndCell, toStartCell, toEndCell].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[fromStartCell, fromEndCell, toStartCell, toEndCell].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[fromStartCell, fromEndCell, toStartCell, toEndCell].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[fromStartCell, fromEndCell, toStartCell, toEndCell].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[fromStartCell, fromEndCell, toStartCell, toEndCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.Cells[fromStartCell, fromEndCell, toStartCell, toEndCell].Style.Font.Bold = true;
            return worksheet;
        }

        private object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName)?.GetValue(src, null);
        }

        private string GetTextAlignAttributeValue(object src, string propName)
        {
            var result = string.Empty;

            var attribute = src.GetType().GetProperty(propName).CustomAttributes.FirstOrDefault(p => p.AttributeType == typeof(TextAlignAttribute));

            if (attribute == null) return null;

            result = attribute.ConstructorArguments.First().Value + "";

            return result;
        }
        #endregion manager view

        #region Báo cáo công nợ BHTN

        public virtual byte[] ExportBaoCaoCongNoCongTyBaoHiemTuNhan(ICollection<BaoCaoGachNoCongTyBhtnGridVo> baoCaoCongNoCongTyBhtns, BaoCaoGachNoCongTyBhtnGridVo congNoChung, string strQueryInfo)
        {
            var queryInfo = new BaoCaoGachNoCongTyBhtnQueryInfo();
            if (!string.IsNullOrEmpty(strQueryInfo))
            {
                queryInfo = JsonConvert.DeserializeObject<BaoCaoGachNoCongTyBhtnQueryInfo>(strQueryInfo);
            }

            #region Ngày tìm kiếm
            DateTime.TryParseExact(queryInfo.TuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
            DateTime.TryParseExact(queryInfo.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);

            var tuNgay = queryInfo.TuNgayDenNgay.TuNgay == null ? (DateTime?)null : tuNgayTemp;
            var denNgay = queryInfo.TuNgayDenNgay.DenNgay == null ? (DateTime?)null : denNgayTemp;
            #endregion

            #region Ngày chứng từ
            //DateTime.TryParseExact(queryInfo.TuNgayDenNgayCT.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayCT);
            //DateTime.TryParseExact(queryInfo.TuNgayDenNgayCT.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayCT);

            //var tuNgayChungTu = queryInfo.TuNgayDenNgayCT.TuNgay == null ? (DateTime?) null : tuNgayCT;
            //var denNgayChungTu = queryInfo.TuNgayDenNgayCT.DenNgay == null ? (DateTime?)null : denNgayCT;
            #endregion

            #region Ngày hóa đơn
            //DateTime.TryParseExact(queryInfo.TuNgayDenNgayHD.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayHD);
            //DateTime.TryParseExact(queryInfo.TuNgayDenNgayHD.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayHD);

            //var tuNgayHoaDon = queryInfo.TuNgayDenNgayHD.TuNgay == null ? (DateTime?)null : tuNgayHD;
            //var denNgayHoaDon = queryInfo.TuNgayDenNgayHD.DenNgay == null ? (DateTime?)null : denNgayHD;
            #endregion

            var dataBaoCaos = baoCaoCongNoCongTyBhtns
                .GroupBy(p => p.TenCongTy, (key, g) => new { TenCongTy = g.First().TenCongTy, CongTyId = g.First().CongTyId, Data = g.ToList() })
                .OrderBy(x => x.TenCongTy);

            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoGachNoCongTyBhtnGridVo>("STT", p => " "),
                new PropertyByName<BaoCaoGachNoCongTyBhtnGridVo>("Công ty", p => p.TenCongTy)
            };

            #region Công ty

            var tenCongTy = "Tất cả";
            if (dataBaoCaos.Count() == 1)
            {
                tenCongTy = baoCaoCongNoCongTyBhtns.First().TenCongTy;
            }
            #endregion

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("Báo cáo");

                    // set row
                    worksheet.Row(9).Height = 24.5;
                    worksheet.DefaultRowHeight = 16;

                    worksheet.Column(2).Width = 20;
                    worksheet.Column(3).Width = 25;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 20;
                    worksheet.Column(6).Width = 20;
                    worksheet.Column(7).Width = 20;
                    worksheet.Column(8).Width = 25;
                    worksheet.Column(9).Width = 15;
                    worksheet.Column(10).Width = 25;
                    worksheet.Column(11).Width = 25;
                    worksheet.Column(12).Width = 25;
                    worksheet.Column(13).Width = 25;
                    worksheet.Column(14).Width = 25;
                    worksheet.Column(15).Width = 25;

                    worksheet.DefaultColWidth = 7;

                    //set column 
                    string[] SetColumnItems = { "A", "B", "C", "D", "E", "F", "G", "H", "A", "I", "J", "K", "L", "M", "N", "O" };
                    var worksheetTitleCongTyBhtn = SetColumnItems[0] + 1 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 1;
                    var worksheetTitleCongTy = SetColumnItems[0] + 2 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 2;
                    var worksheetTitleNgay = SetColumnItems[0] + 3 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 3;
                    //var worksheetTitleNgayCT = SetColumnItems[0] + 3 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 3;
                    //var worksheetTitleNgayHD = SetColumnItems[0] + 4 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 4;

                    var worksheetTitleHeader = SetColumnItems[0] + 6 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 8;
                    using (var range = worksheet.Cells[worksheetTitleCongTyBhtn])
                    {
                        range.Worksheet.Cells[worksheetTitleCongTyBhtn].Merge = true;
                        range.Worksheet.Cells[worksheetTitleCongTyBhtn].Value = "BÁO CÁO CÔNG NỢ CÔNG TY BHTN".ToUpper();
                        range.Worksheet.Cells[worksheetTitleCongTyBhtn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleCongTyBhtn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleCongTyBhtn].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitleCongTyBhtn].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleCongTyBhtn].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleCongTy])
                    {
                        range.Worksheet.Cells[worksheetTitleCongTy].Merge = true;
                        range.Worksheet.Cells[worksheetTitleCongTy].Value = "Công ty: " + tenCongTy;
                        range.Worksheet.Cells[worksheetTitleCongTy].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleCongTy].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells[worksheetTitleCongTy].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitleCongTy].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleCongTy].Style.Font.Bold = true;
                        range.Worksheet.Cells[worksheetTitleCongTy].Style.Font.Italic = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleNgay])
                    {
                        range.Worksheet.Cells[worksheetTitleNgay].Merge = true;
                        range.Worksheet.Cells[worksheetTitleNgay].Value = "Từ ngày: " + tuNgay?.ApplyFormatDate() + " - đến ngày: " + denNgay?.ApplyFormatDate();
                        range.Worksheet.Cells[worksheetTitleNgay].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Bold = true;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Italic = true;
                    }

                    //using (var range = worksheet.Cells[worksheetTitleNgayCT])
                    //{
                    //    range.Worksheet.Cells[worksheetTitleNgayCT].Merge = true;
                    //    range.Worksheet.Cells[worksheetTitleNgayCT].Value = "Ngày CT từ: " + tuNgayChungTu?.ApplyFormatDate() + " - đến ngày: " + denNgayChungTu?.ApplyFormatDate();
                    //    range.Worksheet.Cells[worksheetTitleNgayCT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    //    range.Worksheet.Cells[worksheetTitleNgayCT].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    //    range.Worksheet.Cells[worksheetTitleNgayCT].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                    //    range.Worksheet.Cells[worksheetTitleNgayCT].Style.Font.Color.SetColor(Color.Black);
                    //    range.Worksheet.Cells[worksheetTitleNgayCT].Style.Font.Bold = true;
                    //    range.Worksheet.Cells[worksheetTitleNgayCT].Style.Font.Italic = true;
                    //}

                    //using (var range = worksheet.Cells[worksheetTitleNgayHD])
                    //{
                    //    range.Worksheet.Cells[worksheetTitleNgayHD].Merge = true;
                    //    range.Worksheet.Cells[worksheetTitleNgayHD].Value = "Ngày HD từ: " + tuNgayHoaDon?.ApplyFormatDate() + " - đến ngày: " + denNgayHoaDon?.ApplyFormatDate();
                    //    range.Worksheet.Cells[worksheetTitleNgayHD].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    //    range.Worksheet.Cells[worksheetTitleNgayHD].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    //    range.Worksheet.Cells[worksheetTitleNgayHD].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                    //    range.Worksheet.Cells[worksheetTitleNgayHD].Style.Font.Color.SetColor(Color.Black);
                    //    range.Worksheet.Cells[worksheetTitleNgayHD].Style.Font.Bold = true;
                    //    range.Worksheet.Cells[worksheetTitleNgayHD].Style.Font.Italic = true;
                    //}

                    using (var range = worksheet.Cells[worksheetTitleHeader])
                    {
                        range.Worksheet.Cells[worksheetTitleHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Bold = true;

                        string[,] SetColumns = { { "A" , "STT" }, { "B", "Tài khoản" }, { "C", "Mã TN" } , { "D", "Số CT" },
                            { "E", "Ngày CT" }, { "F", "Số HĐ" }, { "G", "Ngày HĐ" }, { "H", "Diễn giải" } , { "I", "Mã tiền tệ" }, { "J", "Phát sinh nợ" },
                            { "K", "Phát sinh có" },  { "L", "Đầu kỳ nợ" },  { "M", "Đầu kỳ có" },  { "N", "Cuối kỳ nợ" },  { "O", "Cuối kỳ có" }};

                        for (int i = 0; i < SetColumns.Length / 2; i++)
                        {
                            var setColumn = ((SetColumns[i, 0]).ToString() + 6 + ":" + (SetColumns[i, 0]).ToString() + 8).ToString();
                            range.Worksheet.Cells[setColumn].Merge = true;
                            range.Worksheet.Cells[setColumn].Value = SetColumns[i, 1];
                        }

                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    using (var range = worksheet.Cells["A6:O7"])
                    {
                        range.Worksheet.Cells["A6:O8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A6:O8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A6:O8"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A6:O8"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A6:O8"].Style.Font.Bold = true;

                        //Set column A to I 
                        string[,] SetColumns = { { "A" , "STT" }, { "B", "Tài khoản" }, { "C", "Mã TN" } , { "D", "Số CT" },
                            { "E", "Ngày CT" }, { "F", "Số HĐ" }, { "G", "Ngày HĐ" }, { "H", "Diễn giải" } , { "I", "Mã tiền tệ" }, { "J", "Phát sinh nợ" },
                            { "K", "Phát sinh có" },  { "L", "Đầu kỳ nợ" },  { "M", "Đầu kỳ có" },  { "N", "Cuối kỳ nợ" },  { "O", "Cuối kỳ có" }};

                        for (int i = 0; i < SetColumns.Length / 2; i++)
                        {
                            var setColumn = ((SetColumns[i, 0]).ToString() + 6 + ":" + (SetColumns[i, 0]).ToString() + 8).ToString();
                            range.Worksheet.Cells[setColumn].Merge = true;
                            range.Worksheet.Cells[setColumn].Value = SetColumns[i, 1];
                        }

                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    var manager = new PropertyManager<BaoCaoGachNoCongTyBhtnGridVo>(requestProperties);
                    int index = 9;
                    decimal tongSoTienPhatSinhNo = 0;
                    decimal tongSoTienPhatSinhCo = 0;
                    decimal tongSoTienCuoiKyNo = 0;

                    var congTyIds = dataBaoCaos.Select(cc => cc.CongTyId).ToArray();
                    var length = congTyIds.Length;

                    var worksheetFirstLast = SetColumnItems[0] + index + ":" + SetColumnItems[SetColumnItems.Length - 1] + index;


                    if (congNoChung != null)
                    {
                        worksheet.Cells["B" + index].Value = congNoChung.TaiKhoan;
                        worksheet.Cells["C" + index].Value = congNoChung.MaTiepNhan;
                        worksheet.Cells["D" + index].Value = congNoChung.SoChungTu;
                        worksheet.Cells["E" + index].Value = congNoChung.NgayChungTuDisplay;
                        worksheet.Cells["F" + index].Value = congNoChung.SoHoaDon;
                        worksheet.Cells["G" + index].Value = congNoChung.NgayHoaDonDisplay;
                        worksheet.Cells["H" + index].Value = congNoChung.DienGiai;
                        worksheet.Cells["I" + index].Value = congNoChung.MaTienTe;
                        worksheet.Cells["J" + index].Value = Convert.ToDouble(congNoChung.PhatSinhNo).ApplyFormatMoneyVNDToDouble();
                        worksheet.Cells["J" + index + ":O" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["K" + index].Value = Convert.ToDouble(congNoChung.PhatSinhCo).ApplyFormatMoneyVNDToDouble();
                        worksheet.Cells["L" + index].Value = Convert.ToDouble(congNoChung.DauKyNo).ApplyFormatMoneyVNDToDouble();
                        worksheet.Cells["M" + index].Value = Convert.ToDouble(congNoChung.DauKyCo).ApplyFormatMoneyVNDToDouble();
                        worksheet.Cells["N" + index].Value = Convert.ToDouble(congNoChung.CuoiKyNo).ApplyFormatMoneyVNDToDouble();
                        worksheet.Cells["O" + index].Value = Convert.ToDouble(congNoChung.CuoiKyCo).ApplyFormatMoneyVNDToDouble();

                        worksheet.Cells["A" + index + ":O" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["A" + index + ":O" + index].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                        index++;
                    }
                    for (int i = 0; i < length; i++)
                    {
                        manager.CurrentObject = baoCaoCongNoCongTyBhtns.Where(x => x.CongTyId == congTyIds[i]).First();
                        manager.WriteToXlsx(worksheet, index);

                        for (int ii = 0; ii < SetColumnItems.Length; ii++)
                        {
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Bold = true;
                        }

                        worksheet.Row(index).Height = 20.5;

                        var indexMain = index;
                        index++;

                        decimal tongSoTienPhatSinhNoRow = 0;
                        decimal tongSoTienPhatSinhCoRow = 0;
                        decimal tongSoTienCuoiKyNoRow = 0;

                        var dataCongTys = dataBaoCaos.Where(cc => cc.CongTyId == congTyIds[i])
                                                        .SelectMany(cc => cc.Data);

                        int sttItems = 1;
                        foreach (var baoCao in dataCongTys)
                        {
                            tongSoTienPhatSinhNoRow += baoCao.PhatSinhNo;
                            tongSoTienPhatSinhCoRow += baoCao.PhatSinhCo;
                            tongSoTienCuoiKyNoRow = baoCao.CuoiKyNo;

                            tongSoTienPhatSinhNo += baoCao.PhatSinhNo;
                            tongSoTienPhatSinhCo += baoCao.PhatSinhCo;


                            worksheet.Cells["A" + index].Value = sttItems++;
                            worksheet.Cells["B" + index].Value = baoCao.TaiKhoan;
                            worksheet.Cells["C" + index].Value = baoCao.MaTiepNhan;
                            worksheet.Cells["D" + index].Value = baoCao.SoChungTu;
                            worksheet.Cells["E" + index].Value = baoCao.NgayChungTuDisplay;
                            worksheet.Cells["F" + index].Value = baoCao.SoHoaDon;
                            worksheet.Cells["G" + index].Value = baoCao.NgayHoaDonDisplay;
                            worksheet.Cells["H" + index].Value = baoCao.DienGiai;
                            worksheet.Cells["I" + index].Value = baoCao.MaTienTe;
                            worksheet.Cells["J" + index].Value = Convert.ToDouble(baoCao.PhatSinhNo).ApplyFormatMoneyVNDToDouble();
                            worksheet.Cells["J" + index + ":O" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["K" + index].Value = Convert.ToDouble(baoCao.PhatSinhCo).ApplyFormatMoneyVNDToDouble();
                            worksheet.Cells["L" + index].Value = Convert.ToDouble(baoCao.DauKyNo).ApplyFormatMoneyVNDToDouble();
                            worksheet.Cells["M" + index].Value = Convert.ToDouble(baoCao.DauKyCo).ApplyFormatMoneyVNDToDouble();
                            worksheet.Cells["N" + index].Value = Convert.ToDouble(baoCao.CuoiKyNo).ApplyFormatMoneyVNDToDouble();
                            worksheet.Cells["O" + index].Value = Convert.ToDouble(baoCao.CuoiKyCo).ApplyFormatMoneyVNDToDouble();

                            for (int ii = 0; ii < SetColumnItems.Length; ii++)
                            {
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            }

                            index++;
                        }
                        tongSoTienCuoiKyNo += tongSoTienCuoiKyNoRow;

                        using (var range = worksheet.Cells["G" + indexMain + ":O" + indexMain])
                        {
                            worksheet.Cells["G" + indexMain + ":O" + indexMain].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        }

                        //tô màu vàng cho phân group    
                        worksheet.Cells["A" + indexMain + ":O" + indexMain].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["A" + indexMain + ":O" + indexMain].Style.Fill.BackgroundColor.SetColor(Color.Yellow);

                        worksheet.Cells["J" + indexMain].Value = Convert.ToDouble(tongSoTienPhatSinhNoRow).ApplyFormatMoneyVNDToDouble();
                        worksheet.Cells["K" + indexMain].Value = Convert.ToDouble(tongSoTienPhatSinhCoRow).ApplyFormatMoneyVNDToDouble();
                        worksheet.Cells["N" + indexMain].Value = Convert.ToDouble(tongSoTienCuoiKyNoRow).ApplyFormatMoneyVNDToDouble();

                        for (int inde = indexMain + 1; inde <= index - 1; inde++)
                        {
                            worksheet.Row(inde).OutlineLevel = 1;
                        }

                        index++;
                    }

                    worksheet.Cells[worksheetFirstLast].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                    worksheet.Cells[worksheetFirstLast].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells[worksheetFirstLast].Style.Font.Bold = true;
                    worksheet.Cells[worksheetFirstLast].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                    for (int ii = 0; ii < SetColumnItems.Length; ii++)
                    {
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Bold = true;
                    }


                    using (var range = worksheet.Cells["B" + index + ":E" + index])
                    {
                        range.Worksheet.Cells["B" + index + ":H" + index].Merge = true;
                        range.Worksheet.Cells["B" + index + ":H" + index].Value = "Tổng cộng".ToUpper();
                        worksheet.Cells["B" + index + ":O" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }

                    worksheet.Cells["J" + index].Value = Convert.ToDouble(tongSoTienPhatSinhNo).ApplyFormatMoneyVNDToDouble();
                    worksheet.Cells["K" + index].Value = Convert.ToDouble(tongSoTienPhatSinhCo).ApplyFormatMoneyVNDToDouble();
                    worksheet.Cells["N" + index].Value = Convert.ToDouble(tongSoTienCuoiKyNo).ApplyFormatMoneyVNDToDouble();

                    xlPackage.Save();
                }

                return stream.ToArray();
            }
        }
        #endregion

        #region Xét nghiệm - Lấy mẫu
        public virtual byte[] ExportDanhSachLayMauXetNghiem(ICollection<LayMauXetNghiemYeuCauTiepNhanGridVo> dataLayMaus, string strQueryInfo)
        {
            var queryInfo = new LayMauXetNghiemTimKiemNangCaoVo();
            if (!string.IsNullOrEmpty(strQueryInfo))
            {
                queryInfo = JsonConvert.DeserializeObject<LayMauXetNghiemTimKiemNangCaoVo>(strQueryInfo);
            }

            #region Ngày tìm kiếm
            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (queryInfo.TuNgayDenNgay != null && !string.IsNullOrEmpty(queryInfo.TuNgayDenNgay.TuNgay))
            {
                DateTime.TryParseExact(queryInfo.TuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (queryInfo.TuNgayDenNgay != null && !string.IsNullOrEmpty(queryInfo.TuNgayDenNgay.DenNgay))
            {
                DateTime.TryParseExact(queryInfo.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }
            #endregion

            #region Trạng thái

            var arrTrangThai = new List<string>();
            if (queryInfo.TrangThai != null)
            {
                if (queryInfo.TrangThai.ChoLayMau)
                {
                    arrTrangThai.Add(Enums.TrangThaiLayMauXetNghiem.ChoLayMau.GetDescription());
                }

                if (queryInfo.TrangThai.ChoGuiMau)
                {
                    arrTrangThai.Add(Enums.TrangThaiLayMauXetNghiem.ChoGuiMau.GetDescription());
                }

                if (queryInfo.TrangThai.ChoKetQua)
                {
                    arrTrangThai.Add(Enums.TrangThaiLayMauXetNghiem.ChoKetQua.GetDescription());
                }

                if (queryInfo.TrangThai.DaCoKetQua)
                {
                    arrTrangThai.Add(Enums.TrangThaiLayMauXetNghiem.DaCoKetQua.GetDescription());
                }
            }
            #endregion

            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<LayMauXetNghiemYeuCauTiepNhanGridVo>("STT", p => ind++)
            };
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("DANH SÁCH LẤY MẪU XÉT NGHIỆM");

                    // set row
                    worksheet.Row(9).Height = 24.5;
                    worksheet.DefaultRowHeight = 16;

                    worksheet.Column(2).Width = 20;
                    worksheet.Column(3).Width = 25;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 20;
                    worksheet.Column(6).Width = 20;
                    worksheet.Column(7).Width = 20;
                    worksheet.Column(8).Width = 25;
                    worksheet.Column(9).Width = 15;
                    worksheet.Column(10).Width = 25;
                    worksheet.Column(11).Width = 25;
                    worksheet.Column(12).Width = 25;
                    worksheet.Column(13).Width = 25;
                    worksheet.Column(14).Width = 25;
                    worksheet.Column(15).Width = 25;

                    worksheet.DefaultColWidth = 7;

                    //set column 
                    string[] SetColumnItems = { "A", "B", "C", "D", "E", "F", "G", "H", "A", "I", "J", "K" };
                    var worksheetTitle = SetColumnItems[0] + 1 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 1;
                    var worksheetTitleStatus = SetColumnItems[0] + 2 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 2;
                    var worksheetTitleNgay = SetColumnItems[0] + 3 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 3;
                    var worksheetTitleHeader = SetColumnItems[0] + 6 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 8;

                    using (var range = worksheet.Cells[worksheetTitle])
                    {
                        range.Worksheet.Cells[worksheetTitle].Merge = true;
                        range.Worksheet.Cells[worksheetTitle].Value = "DANH SÁCH LẤY MẪU XÉT NGHIỆM".ToUpper();
                        range.Worksheet.Cells[worksheetTitle].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitle].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitle].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitle].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitle].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleStatus])
                    {
                        range.Worksheet.Cells[worksheetTitleStatus].Merge = true;
                        range.Worksheet.Cells[worksheetTitleStatus].Value = "Trạng thái: " + string.Join(", ", arrTrangThai);
                        range.Worksheet.Cells[worksheetTitleStatus].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleStatus].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleStatus].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitleStatus].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleStatus].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleNgay])
                    {
                        range.Worksheet.Cells[worksheetTitleNgay].Merge = true;
                        range.Worksheet.Cells[worksheetTitleNgay].Value = "Từ ngày: " + tuNgay?.ApplyFormatDate() + " - đến ngày: " + denNgay?.ApplyFormatDate();
                        range.Worksheet.Cells[worksheetTitleNgay].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Bold = true;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Italic = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleHeader])
                    {
                        range.Worksheet.Cells[worksheetTitleHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Bold = true;

                        string[,] SetColumns = { { "A" , "STT" }, { "B", "Mã TN" }, { "C", "Mã BN" } , { "D", "Họ Tên" },
                            { "E", "Năm Sinh" }, { "F", "Giới Tính" }, { "G", "Địa Chỉ" }, { "H", "Điện Thoại" } , { "I", "SL Chờ Lấy" }, { "J", "SL Chờ Gửi" },
                            { "K", "Có Đủ KQ" }};

                        for (int i = 0; i < SetColumns.Length / 2; i++)
                        {
                            var setColumn = ((SetColumns[i, 0]).ToString() + 6 + ":" + (SetColumns[i, 0]).ToString() + 8).ToString();
                            range.Worksheet.Cells[setColumn].Merge = true;
                            range.Worksheet.Cells[setColumn].Value = SetColumns[i, 1];
                        }

                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    using (var range = worksheet.Cells["A6:K7"])
                    {
                        range.Worksheet.Cells["A6:K8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A6:K8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A6:K8"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A6:K8"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A6:K8"].Style.Font.Bold = true;

                        //Set column A to K 
                        string[,] SetColumns = { { "A" , "STT" }, { "B", "Mã TN" }, { "C", "Mã BN" } , { "D", "Họ Tên" },
                            { "E", "Năm Sinh" }, { "F", "Giới Tính" }, { "G", "Địa Chỉ" }, { "H", "Điện Thoại" } , { "I", "SL Chờ Lấy" }, { "J", "SL Chờ Gửi" },
                            { "K", "Có Đủ KQ" }};

                        for (int i = 0; i < SetColumns.Length / 2; i++)
                        {
                            var setColumn = ((SetColumns[i, 0]).ToString() + 6 + ":" + (SetColumns[i, 0]).ToString() + 8).ToString();
                            range.Worksheet.Cells[setColumn].Merge = true;
                            range.Worksheet.Cells[setColumn].Value = SetColumns[i, 1];
                        }

                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    var manager = new PropertyManager<LayMauXetNghiemYeuCauTiepNhanGridVo>(requestProperties);
                    int index = 9;
                    var worksheetFirstLast = SetColumnItems[0] + index + ":" + SetColumnItems[SetColumnItems.Length - 1] + index;

                    int sttYCTN = 1;
                    foreach (var yeuCauTiepNhan in dataLayMaus)
                    {
                        manager.CurrentObject = yeuCauTiepNhan;
                        manager.WriteToXlsx(worksheet, index);

                        worksheet.Cells["A" + index].Value = sttYCTN++;
                        worksheet.Cells["B" + index].Value = yeuCauTiepNhan.MaTiepNhan;
                        worksheet.Cells["C" + index].Value = yeuCauTiepNhan.MaBenhNhan;
                        worksheet.Cells["D" + index].Value = yeuCauTiepNhan.HoTen;
                        worksheet.Cells["E" + index].Value = yeuCauTiepNhan.NamSinh;
                        worksheet.Cells["F" + index].Value = yeuCauTiepNhan.GioiTinh;
                        worksheet.Cells["G" + index].Value = yeuCauTiepNhan.DiaChi;
                        worksheet.Cells["H" + index].Value = yeuCauTiepNhan.SoDienThoaiDisplay;
                        worksheet.Cells["I" + index].Value = yeuCauTiepNhan.SoLuongChoLay;
                        worksheet.Cells["J" + index].Value = yeuCauTiepNhan.SoLuongChoGui;
                        worksheet.Cells["K" + index].Value = yeuCauTiepNhan.CoDuKetQua ? "Có" : "Không";

                        for (int ii = 0; ii < SetColumnItems.Length; ii++)
                        {
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Bold = true;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Fill.BackgroundColor.SetColor(Color.LightSteelBlue);
                        }

                        worksheet.Row(index).Height = 20.5;

                        var indexMain = index;
                        index++;

                        using (var range = worksheet.Cells["A" + index + ":K" + index])
                        {
                            range.Worksheet.Cells["B" + index + ":K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells["B" + index + ":K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells["B" + index + ":K" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                            range.Worksheet.Cells["B" + index + ":K" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["B" + index + ":K" + index].Style.Font.Bold = true;
                            range.Worksheet.Cells["B" + index + ":K" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            range.Worksheet.Cells["B" + index + ":K" + index].Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);

                            //Set column A to K 
                            string[,] SetColumnNhoms = { { "B" , "STT" }, { "C", "Xét Nghiệm" }, { "D", "Mã Barcode" } , { "E", "Trạng Thái" },
                                { "F", "Loại Mẫu" }, { "G", "Số Phiếu Gửi" }};

                            for (int i = 0; i < SetColumnNhoms.Length / 2; i++)
                            {
                                var setColumn = ((SetColumnNhoms[i, 0]).ToString() + index + ":" + (SetColumnNhoms[i, 0]).ToString() + index).ToString();
                                range.Worksheet.Cells[setColumn].Merge = true;
                                range.Worksheet.Cells[setColumn].Value = SetColumnNhoms[i, 1];
                            }

                            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        }
                        index++;

                        int sttItems = 1;
                        foreach (var nhom in yeuCauTiepNhan.NhomCanLayMauXetNghiems)
                        {
                            worksheet.Cells["B" + index].Value = sttItems++;
                            worksheet.Cells["C" + index].Value = nhom.TenNhom;
                            worksheet.Cells["D" + index].Value = nhom.Barcode;
                            worksheet.Cells["E" + index].Value = nhom.TenTrangThai;
                            worksheet.Cells["F" + index].Value = string.Join(", ", nhom.LoaiMaus.Select(x => x.TenLoaiMau).ToList());
                            worksheet.Cells["G" + index].Value = nhom.SoPhieu;

                            for (int ii = 0; ii < SetColumnItems.Length; ii++)
                            {
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            }

                            index++;

                            using (var range = worksheet.Cells["A" + index + ":K" + index])
                            {
                                range.Worksheet.Cells["C" + index + ":K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                range.Worksheet.Cells["C" + index + ":K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                range.Worksheet.Cells["C" + index + ":K" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                range.Worksheet.Cells["C" + index + ":K" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["C" + index + ":K" + index].Style.Font.Bold = true;
                                range.Worksheet.Cells["C" + index + ":K" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                range.Worksheet.Cells["C" + index + ":K" + index].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

                                //Set column A to K 
                                string[,] SetColumnDichVus = { { "C" , "STT" }, { "D", "Mã DV" }, { "E", "Tên DV" } , { "F", "Thời Gian Chỉ Định" },
                                    { "G", "Người Chỉ Định" }, { "H", "Bệnh Phẩm" }, { "I", "Loại Mẫu" }};

                                for (int i = 0; i < SetColumnDichVus.Length / 2; i++)
                                {
                                    var setColumn = ((SetColumnDichVus[i, 0]).ToString() + index + ":" + (SetColumnDichVus[i, 0]).ToString() + index).ToString();
                                    range.Worksheet.Cells[setColumn].Merge = true;
                                    range.Worksheet.Cells[setColumn].Value = SetColumnDichVus[i, 1];
                                }

                                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            }
                            index++;

                            var sttDichVu = 1;
                            foreach (var dichVu in nhom.DichVuCanLayMauXetNghiems)
                            {
                                worksheet.Cells["C" + index].Value = sttDichVu++;
                                worksheet.Cells["D" + index].Value = dichVu.MaDichVu;
                                worksheet.Cells["E" + index].Value = dichVu.TenDichVu;
                                worksheet.Cells["F" + index].Value = dichVu.ThoiGianChiDinhDisplay;
                                worksheet.Cells["G" + index].Value = dichVu.NguoiChiDinh;
                                worksheet.Cells["H" + index].Value = dichVu.BenhPham;
                                worksheet.Cells["I" + index].Value = dichVu.LoaiMau.GetDescription();

                                for (int ii = 0; ii < SetColumnItems.Length; ii++)
                                {
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                }

                                index++;
                            }
                        }

                        //using (var range = worksheet.Cells["G" + indexMain + ":K" + indexMain])
                        //{
                        //    worksheet.Cells["G" + indexMain + ":K" + indexMain].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        //}

                        for (int inde = indexMain + 1; inde <= index - 1; inde++)
                        {
                            worksheet.Row(inde).OutlineLevel = 1;
                        }

                        index++;
                    }

                    xlPackage.Save();
                }

                return stream.ToArray();
            }
        }

        public virtual byte[] ExportDanhSachGoiMauXetNghiem(ICollection<GoiMauDanhSachXetNghiemGridVo> dataGoiMauXetNghiem, string strQueryInfo)
        {
            var queryInfo = new GoiMauXetNghiemSearch();
            if (!string.IsNullOrEmpty(strQueryInfo))
            {
                queryInfo = JsonConvert.DeserializeObject<GoiMauXetNghiemSearch>(strQueryInfo);
            }

            #region Trạng thái
            var arrTrangThai = new List<string>();

            if (!queryInfo.ChoNhanMau && !queryInfo.DaNhanMau)
            {
                arrTrangThai.Add("Chờ nhận mẫu");
                arrTrangThai.Add("Đã nhận mẫu");
            }
            else
            {
                if (queryInfo.ChoNhanMau)
                {
                    arrTrangThai.Add("Chờ nhận mẫu");
                }
                if (queryInfo.DaNhanMau)
                {
                    arrTrangThai.Add("Đã nhận mẫu");
                }
            }

            #endregion

            int idx = 1;
            var requestProperties = new[]
            {
                new PropertyByName<GoiMauDanhSachXetNghiemGridVo>("STT", p => idx++)
            };
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("DANH SÁCH GỞI MẪU XÉT NGHIỆM");

                    // set row
                    worksheet.Row(9).Height = 24.5;
                    worksheet.DefaultRowHeight = 16;

                    worksheet.Column(1).Width = 20;
                    worksheet.Column(2).Width = 20;
                    worksheet.Column(3).Width = 25;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 50;
                    worksheet.Column(6).Width = 20;
                    worksheet.Column(7).Width = 20;
                    worksheet.Column(8).Width = 25;
                    worksheet.Column(9).Width = 15;
                    worksheet.Column(10).Width = 25;
                    worksheet.Column(11).Width = 25;
                    worksheet.Column(12).Width = 25;
                    worksheet.Column(13).Width = 25;
                    worksheet.Column(14).Width = 25;
                    worksheet.Column(15).Width = 25;

                    worksheet.DefaultColWidth = 7;

                    //set column 
                    string[] SetColumnItems = { "A", "B", "C", "D", "E", "F", "G", "H", "A", "I", "J" };
                    var worksheetTitle = SetColumnItems[0] + 1 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 1;
                    var worksheetTitleStatus = SetColumnItems[0] + 2 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 2;
                    var worksheetTitleNgay = SetColumnItems[0] + 3 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 3;
                    var worksheetTitleHeader = SetColumnItems[0] + 6 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 7;

                    using (var range = worksheet.Cells[worksheetTitle])
                    {
                        range.Worksheet.Cells[worksheetTitle].Merge = true;
                        range.Worksheet.Cells[worksheetTitle].Value = "DANH SÁCH GỞI MẪU XÉT NGHIỆM".ToUpper();
                        range.Worksheet.Cells[worksheetTitle].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitle].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitle].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitle].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitle].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleStatus])
                    {
                        range.Worksheet.Cells[worksheetTitleStatus].Merge = true;
                        range.Worksheet.Cells[worksheetTitleStatus].Value = "Trạng thái: " + string.Join(", ", arrTrangThai);
                        range.Worksheet.Cells[worksheetTitleStatus].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleStatus].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleStatus].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitleStatus].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleStatus].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleNgay])
                    {
                        range.Worksheet.Cells[worksheetTitleNgay].Merge = true;
                        range.Worksheet.Cells[worksheetTitleNgay].Value = "Từ ngày: " + queryInfo.RangeNgayGoiMau.startDate?.ApplyFormatDate() + " - đến ngày: " + queryInfo.RangeNgayGoiMau.endDate?.ApplyFormatDate();
                        range.Worksheet.Cells[worksheetTitleNgay].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Bold = true;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Italic = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleHeader])
                    {
                        range.Worksheet.Cells[worksheetTitleHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Bold = true;

                        string[,] SetColumns = { { "A" , "Số phiếu" }, { "B", "Người gởi mẫu" }, { "C", "Ngày gởi mẫu" } , { "D", "SL Mẫu (KQ/Tổng)" },
                            { "E", "Tình trạng" }, { "F", "Nơi tiếp nhận" }, { "G", "Người nhận mẫu" }, { "H", "Ngày nhận mẫu" } };

                        for (int i = 0; i < SetColumns.Length / 2; i++)
                        {
                            var setColumn = ((SetColumns[i, 0]).ToString() + 6 + ":" + (SetColumns[i, 0]).ToString() + 7).ToString();
                            range.Worksheet.Cells[setColumn].Merge = true;
                            range.Worksheet.Cells[setColumn].Value = SetColumns[i, 1];
                        }

                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    using (var range = worksheet.Cells["A6:H7"])
                    {
                        range.Worksheet.Cells["A6:J7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A6:J7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A6:J7"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A6:J7"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A6:J7"].Style.Font.Bold = true;

                        //Set column A to K 
                        string[,] SetColumns = { { "A" , "Số phiếu" }, { "B", "Người gởi mẫu" }, { "C", "Ngày gởi mẫu" } , { "D", "SL Mẫu (KQ/Tổng)" },
                            { "E", "Tình trạng" }, { "F", "Nơi tiếp nhận" }, { "G", "Người nhận mẫu" }, { "H", "Ngày nhận mẫu" }, { "I", "" }, { "J", "" } };

                        for (int i = 0; i < SetColumns.Length / 2; i++)
                        {
                            var setColumn = ((SetColumns[i, 0]).ToString() + 6 + ":" + (SetColumns[i, 0]).ToString() + 7).ToString();
                            range.Worksheet.Cells[setColumn].Merge = true;
                            range.Worksheet.Cells[setColumn].Value = SetColumns[i, 1];
                        }

                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    var manager = new PropertyManager<GoiMauDanhSachXetNghiemGridVo>(requestProperties);
                    int index = 9;
                    var worksheetFirstLast = SetColumnItems[0] + index + ":" + SetColumnItems[SetColumnItems.Length - 1] + index;

                    foreach (var goiMauXetNghiem in dataGoiMauXetNghiem)
                    {
                        manager.CurrentObject = goiMauXetNghiem;
                        manager.WriteToXlsx(worksheet, index);

                        //worksheet.Cells["A" + index].Value = sttYCTN++;
                        worksheet.Cells["A" + index].Value = goiMauXetNghiem.SoPhieu;
                        worksheet.Cells["B" + index].Value = goiMauXetNghiem.NguoiGoiMauDisplay;
                        worksheet.Cells["C" + index].Value = goiMauXetNghiem.NgayGoiMauDisplay;
                        //worksheet.Cells["D" + index].Value = goiMauXetNghiem.SoLuongMau;
                        worksheet.Cells["E" + index].Value = goiMauXetNghiem.TinhTrangDisplay;
                        worksheet.Cells["F" + index].Value = goiMauXetNghiem.NoiTiepNhan;
                        worksheet.Cells["G" + index].Value = goiMauXetNghiem.NguoiNhanMauDisplay;
                        worksheet.Cells["H" + index].Value = goiMauXetNghiem.NgayNhanMauDisplay;

                        for (int ii = 0; ii < SetColumnItems.Length; ii++)
                        {
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Bold = true;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Fill.BackgroundColor.SetColor(Color.LightSteelBlue);
                        }

                        worksheet.Row(index).Height = 20.5;

                        var indexMain = index;
                        index++;

                        using (var range = worksheet.Cells["A" + index + ":J" + index])
                        {
                            range.Worksheet.Cells["B" + index + ":J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells["B" + index + ":J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells["B" + index + ":J" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                            range.Worksheet.Cells["B" + index + ":J" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["B" + index + ":J" + index].Style.Font.Bold = true;
                            range.Worksheet.Cells["B" + index + ":J" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            range.Worksheet.Cells["B" + index + ":J" + index].Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);

                            //Set column A to K 
                            string[,] SetColumnNhoms = { { "B" , "STT" }, { "C", "Xét Nghiệm" },  { "D", "Mã Barcode" }, { "E", "Loại mẫu" } , { "F", "Mã TN" },
                                { "G", "Mã BN" }, { "H", "Họ tên" }, { "I", "Năm sinh" }, { "J", "Giới tính" } };

                            for (int i = 0; i < SetColumnNhoms.Length / 2; i++)
                            {
                                var setColumn = ((SetColumnNhoms[i, 0]).ToString() + index + ":" + (SetColumnNhoms[i, 0]).ToString() + index).ToString();
                                range.Worksheet.Cells[setColumn].Merge = true;
                                range.Worksheet.Cells[setColumn].Value = SetColumnNhoms[i, 1];
                            }

                            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        }
                        index++;

                        int sttItems = 1;
                        foreach (var nhomXetNghiem in goiMauXetNghiem.GoiMauDanhSachNhomXetNghiems)
                        {
                            worksheet.Cells["B" + index].Value = sttItems++;
                            worksheet.Cells["C" + index].Value = nhomXetNghiem.NhomDichVuBenhVienDisplay;
                            worksheet.Cells["D" + index].Value = nhomXetNghiem.Barcode;
                            worksheet.Cells["E" + index].Value = string.Join(", ", nhomXetNghiem.LoaiMauXetNghiems.Select(x => x.LoaiMauDisplay).ToList());
                            worksheet.Cells["F" + index].Value = nhomXetNghiem.MaTiepNhan;
                            worksheet.Cells["G" + index].Value = nhomXetNghiem.MaBenhNhan;
                            worksheet.Cells["H" + index].Value = nhomXetNghiem.HoTen;
                            worksheet.Cells["I" + index].Value = nhomXetNghiem.NamSinh;
                            worksheet.Cells["J" + index].Value = nhomXetNghiem.GioiTinhDisplay;

                            for (int ii = 0; ii < SetColumnItems.Length; ii++)
                            {
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            }

                            index++;

                            using (var range = worksheet.Cells["A" + index + ":J" + index])
                            {
                                range.Worksheet.Cells["C" + index + ":I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                range.Worksheet.Cells["C" + index + ":I" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                range.Worksheet.Cells["C" + index + ":I" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                range.Worksheet.Cells["C" + index + ":I" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["C" + index + ":I" + index].Style.Font.Bold = true;
                                range.Worksheet.Cells["C" + index + ":I" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                range.Worksheet.Cells["C" + index + ":I" + index].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

                                //Set column A to K 
                                string[,] SetColumnDichVus = { { "C" , "STT" }, { "D", "Mã DV" }, { "E", "Tên DV" } , { "F", "Thời Gian Chỉ Định" },
                                    { "G", "Người Chỉ Định" }, { "H", "Bệnh Phẩm" }, { "I", "Loại Mẫu" }};

                                for (int i = 0; i < SetColumnDichVus.Length / 2; i++)
                                {
                                    var setColumn = ((SetColumnDichVus[i, 0]).ToString() + index + ":" + (SetColumnDichVus[i, 0]).ToString() + index).ToString();
                                    range.Worksheet.Cells[setColumn].Merge = true;
                                    range.Worksheet.Cells[setColumn].Value = SetColumnDichVus[i, 1];
                                }

                                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            }
                            index++;

                            var sttDichVu = 1;
                            foreach (var dichVu in nhomXetNghiem.GoiMauDanhSachDichVuXetNghiems)
                            {
                                worksheet.Cells["C" + index].Value = sttDichVu++;
                                worksheet.Cells["D" + index].Value = dichVu.MaDichVu;
                                worksheet.Cells["E" + index].Value = dichVu.TenDichVu;
                                worksheet.Cells["F" + index].Value = dichVu.ThoiGianChiDinhDisplay;
                                worksheet.Cells["G" + index].Value = dichVu.NguoiChiDinhDisplay;
                                worksheet.Cells["H" + index].Value = dichVu.BenhPham;
                                worksheet.Cells["I" + index].Value = dichVu.LoaiMau.GetDescription();

                                for (int ii = 0; ii < SetColumnItems.Length; ii++)
                                {
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                }

                                index++;
                            }
                        }

                        //using (var range = worksheet.Cells["G" + indexMain + ":K" + indexMain])
                        //{
                        //    worksheet.Cells["G" + indexMain + ":K" + indexMain].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        //}

                        for (int inde = indexMain + 1; inde <= index - 1; inde++)
                        {
                            worksheet.Row(inde).OutlineLevel = 1;
                        }

                        index++;
                    }

                    xlPackage.Save();
                }

                return stream.ToArray();
            }
        }

        public virtual byte[] ExportDanhSachNhanMauXetNghiem(ICollection<NhanMauDanhSachXetNghiemGridVo> dataNhanMauXetNghiem, string strQueryInfo)
        {
            var queryInfo = new NhanMauXetNghiemSearch();
            if (!string.IsNullOrEmpty(strQueryInfo))
            {
                queryInfo = JsonConvert.DeserializeObject<NhanMauXetNghiemSearch>(strQueryInfo);
            }

            #region Trạng thái
            var arrTrangThai = new List<string>();

            if (!queryInfo.ChoNhanMau && !queryInfo.DaNhanMau)
            {
                arrTrangThai.Add("Chờ nhận mẫu");
                arrTrangThai.Add("Đã nhận mẫu");
            }
            else
            {
                if (queryInfo.ChoNhanMau)
                {
                    arrTrangThai.Add("Chờ nhận mẫu");
                }
                if (queryInfo.DaNhanMau)
                {
                    arrTrangThai.Add("Đã nhận mẫu");
                }
            }

            #endregion

            int idx = 1;
            var requestProperties = new[]
            {
                new PropertyByName<NhanMauDanhSachXetNghiemGridVo>("STT", p => idx++)
            };
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("DANH SÁCH NHẬN MẪU XÉT NGHIỆM");

                    // set row
                    worksheet.Row(9).Height = 24.5;
                    worksheet.DefaultRowHeight = 16;

                    worksheet.Column(1).Width = 20;
                    worksheet.Column(2).Width = 20;
                    worksheet.Column(3).Width = 25;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 50;
                    worksheet.Column(6).Width = 20;
                    worksheet.Column(7).Width = 20;
                    worksheet.Column(8).Width = 25;
                    worksheet.Column(9).Width = 15;
                    worksheet.Column(10).Width = 25;
                    worksheet.Column(11).Width = 25;
                    worksheet.Column(12).Width = 25;
                    worksheet.Column(13).Width = 25;
                    worksheet.Column(14).Width = 25;
                    worksheet.Column(15).Width = 25;

                    worksheet.DefaultColWidth = 7;

                    //set column 
                    string[] SetColumnItems = { "A", "B", "C", "D", "E", "F", "G", "H", "A", "I", "J" };
                    var worksheetTitle = SetColumnItems[0] + 1 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 1;
                    var worksheetTitleStatus = SetColumnItems[0] + 2 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 2;
                    var worksheetTitleNgay = SetColumnItems[0] + 3 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 3;
                    var worksheetTitleHeader = SetColumnItems[0] + 6 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 8;

                    using (var range = worksheet.Cells[worksheetTitle])
                    {
                        range.Worksheet.Cells[worksheetTitle].Merge = true;
                        range.Worksheet.Cells[worksheetTitle].Value = "DANH SÁCH NHẬN MẪU XÉT NGHIỆM".ToUpper();
                        range.Worksheet.Cells[worksheetTitle].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitle].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitle].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitle].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitle].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleStatus])
                    {
                        range.Worksheet.Cells[worksheetTitleStatus].Merge = true;
                        range.Worksheet.Cells[worksheetTitleStatus].Value = "Trạng thái: " + string.Join(", ", arrTrangThai);
                        range.Worksheet.Cells[worksheetTitleStatus].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleStatus].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleStatus].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitleStatus].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleStatus].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleNgay])
                    {
                        range.Worksheet.Cells[worksheetTitleNgay].Merge = true;
                        range.Worksheet.Cells[worksheetTitleNgay].Value = "Từ ngày: " + queryInfo.RangeNgayGoiMau.startDate?.ApplyFormatDate() + " - đến ngày: " + queryInfo.RangeNgayGoiMau.endDate?.ApplyFormatDate();
                        range.Worksheet.Cells[worksheetTitleNgay].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Bold = true;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Italic = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleHeader])
                    {
                        range.Worksheet.Cells[worksheetTitleHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Bold = true;

                        string[,] SetColumns = { { "A" , "Số phiếu" }, { "B", "Người gởi mẫu" }, { "C", "Ngày gởi mẫu" } , { "D", "SL Mẫu (KQ/Tổng)" },
                            { "E", "Tình trạng" }, { "F", "Nơi tiếp nhận" }, { "G", "Người nhận mẫu" }, { "H", "Ngày nhận mẫu" } };

                        for (int i = 0; i < SetColumns.Length / 2; i++)
                        {
                            var setColumn = ((SetColumns[i, 0]).ToString() + 6 + ":" + (SetColumns[i, 0]).ToString() + 8).ToString();
                            range.Worksheet.Cells[setColumn].Merge = true;
                            range.Worksheet.Cells[setColumn].Value = SetColumns[i, 1];
                        }

                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    using (var range = worksheet.Cells["A6:H7"])
                    {
                        range.Worksheet.Cells["A6:J8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A6:J8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A6:J8"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A6:J8"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A6:J8"].Style.Font.Bold = true;

                        //Set column A to K 
                        string[,] SetColumns = { { "A" , "Số phiếu" }, { "B", "Người gởi mẫu" }, { "C", "Ngày gởi mẫu" } , { "D", "SL Mẫu (KQ/Tổng)" },
                            { "E", "Tình trạng" }, { "F", "Nơi tiếp nhận" }, { "G", "Người nhận mẫu" }, { "H", "Ngày nhận mẫu" }, { "I", "" }, { "J", "" } };

                        for (int i = 0; i < SetColumns.Length / 2; i++)
                        {
                            var setColumn = ((SetColumns[i, 0]).ToString() + 6 + ":" + (SetColumns[i, 0]).ToString() + 8).ToString();
                            range.Worksheet.Cells[setColumn].Merge = true;
                            range.Worksheet.Cells[setColumn].Value = SetColumns[i, 1];
                        }

                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    var manager = new PropertyManager<NhanMauDanhSachXetNghiemGridVo>(requestProperties);
                    int index = 9;
                    var worksheetFirstLast = SetColumnItems[0] + index + ":" + SetColumnItems[SetColumnItems.Length - 1] + index;

                    foreach (var goiMauXetNghiem in dataNhanMauXetNghiem)
                    {
                        manager.CurrentObject = goiMauXetNghiem;
                        manager.WriteToXlsx(worksheet, index);

                        //worksheet.Cells["A" + index].Value = sttYCTN++;
                        worksheet.Cells["A" + index].Value = goiMauXetNghiem.SoPhieu;
                        worksheet.Cells["B" + index].Value = goiMauXetNghiem.NguoiGoiMauDisplay;
                        worksheet.Cells["C" + index].Value = goiMauXetNghiem.NgayGoiMauDisplay;
                        worksheet.Cells["D" + index].Value = goiMauXetNghiem.SoLuongMau;
                        worksheet.Cells["E" + index].Value = goiMauXetNghiem.TinhTrangDisplay;
                        worksheet.Cells["F" + index].Value = goiMauXetNghiem.NoiTiepNhanDisplay;
                        worksheet.Cells["G" + index].Value = goiMauXetNghiem.NguoiNhanMauDisplay;
                        worksheet.Cells["H" + index].Value = goiMauXetNghiem.NgayNhanMauDisplay;

                        for (int ii = 0; ii < SetColumnItems.Length; ii++)
                        {
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Bold = true;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Fill.BackgroundColor.SetColor(Color.LightSteelBlue);
                        }

                        worksheet.Row(index).Height = 20.5;

                        var indexMain = index;
                        index++;

                        using (var range = worksheet.Cells["A" + index + ":J" + index])
                        {
                            range.Worksheet.Cells["B" + index + ":J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells["B" + index + ":J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells["B" + index + ":J" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                            range.Worksheet.Cells["B" + index + ":J" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["B" + index + ":J" + index].Style.Font.Bold = true;
                            range.Worksheet.Cells["B" + index + ":J" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            range.Worksheet.Cells["B" + index + ":J" + index].Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);

                            //Set column A to K 
                            string[,] SetColumnNhoms = { { "B" , "STT" }, { "C", "Xét Nghiệm" },  { "D", "Mã Barcode" }, { "E", "Loại mẫu" } , { "F", "Mã TN" },
                                { "G", "Mã BN" }, { "H", "Họ tên" }, { "I", "Năm sinh" }, { "J", "Giới tính" } };

                            for (int i = 0; i < SetColumnNhoms.Length / 2; i++)
                            {
                                var setColumn = ((SetColumnNhoms[i, 0]).ToString() + index + ":" + (SetColumnNhoms[i, 0]).ToString() + index).ToString();
                                range.Worksheet.Cells[setColumn].Merge = true;
                                range.Worksheet.Cells[setColumn].Value = SetColumnNhoms[i, 1];
                            }

                            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        }
                        index++;

                        int sttItems = 1;
                        foreach (var nhomXetNghiem in goiMauXetNghiem.NhanMauDanhSachNhomXetNghiems)
                        {
                            worksheet.Cells["B" + index].Value = sttItems++;
                            worksheet.Cells["C" + index].Value = nhomXetNghiem.NhomDichVuBenhVienDisplay;
                            worksheet.Cells["D" + index].Value = nhomXetNghiem.Barcode;
                            worksheet.Cells["E" + index].Value = string.Join(", ", nhomXetNghiem.LoaiMauXetNghiems.Select(x => x.LoaiMauDisplay).ToList());
                            worksheet.Cells["F" + index].Value = nhomXetNghiem.MaTiepNhan;
                            worksheet.Cells["G" + index].Value = nhomXetNghiem.MaBenhNhan;
                            worksheet.Cells["H" + index].Value = nhomXetNghiem.HoTen;
                            worksheet.Cells["I" + index].Value = nhomXetNghiem.NamSinh;
                            worksheet.Cells["J" + index].Value = nhomXetNghiem.GioiTinhDisplay;

                            for (int ii = 0; ii < SetColumnItems.Length; ii++)
                            {
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            }

                            index++;

                            using (var range = worksheet.Cells["A" + index + ":J" + index])
                            {
                                range.Worksheet.Cells["C" + index + ":I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                range.Worksheet.Cells["C" + index + ":I" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                range.Worksheet.Cells["C" + index + ":I" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                range.Worksheet.Cells["C" + index + ":I" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["C" + index + ":I" + index].Style.Font.Bold = true;
                                range.Worksheet.Cells["C" + index + ":I" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                range.Worksheet.Cells["C" + index + ":I" + index].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

                                //Set column A to K 
                                string[,] SetColumnDichVus = { { "C" , "STT" }, { "D", "Mã DV" }, { "E", "Tên DV" } , { "F", "Thời Gian Chỉ Định" },
                                    { "G", "Người Chỉ Định" }, { "H", "Bệnh Phẩm" }, { "I", "Loại Mẫu" }};

                                for (int i = 0; i < SetColumnDichVus.Length / 2; i++)
                                {
                                    var setColumn = ((SetColumnDichVus[i, 0]).ToString() + index + ":" + (SetColumnDichVus[i, 0]).ToString() + index).ToString();
                                    range.Worksheet.Cells[setColumn].Merge = true;
                                    range.Worksheet.Cells[setColumn].Value = SetColumnDichVus[i, 1];
                                }

                                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            }
                            index++;

                            var sttDichVu = 1;
                            foreach (var dichVu in nhomXetNghiem.NhanMauDanhSachDichVuXetNghiems)
                            {
                                worksheet.Cells["C" + index].Value = sttDichVu++;
                                worksheet.Cells["D" + index].Value = dichVu.MaDichVu;
                                worksheet.Cells["E" + index].Value = dichVu.TenDichVu;
                                worksheet.Cells["F" + index].Value = dichVu.ThoiGianChiDinhDisplay;
                                worksheet.Cells["G" + index].Value = dichVu.NguoiChiDinhDisplay;
                                worksheet.Cells["H" + index].Value = dichVu.BenhPham;
                                worksheet.Cells["I" + index].Value = dichVu.LoaiMau.GetDescription();

                                for (int ii = 0; ii < SetColumnItems.Length; ii++)
                                {
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                }

                                index++;
                            }
                        }

                        //using (var range = worksheet.Cells["G" + indexMain + ":K" + indexMain])
                        //{
                        //    worksheet.Cells["G" + indexMain + ":K" + indexMain].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        //}

                        for (int inde = indexMain + 1; inde <= index - 1; inde++)
                        {
                            worksheet.Row(inde).OutlineLevel = 1;
                        }

                        index++;
                    }

                    xlPackage.Save();
                }

                return stream.ToArray();
            }
        }

        public virtual byte[] ExportDanhSachNguoiBenhDaCapCodeXetNghiemAsync(ICollection<BenhNhanXetNghiemGridVo> dataCapCodes, List<PhienXetNghiem> phienXetNghiemDaCaps, string strQueryInfo)
        {
            var queryInfo = new XacNhanCapCodeTimKiemNangCaoVo();
            if (!string.IsNullOrEmpty(strQueryInfo))
            {
                queryInfo = JsonConvert.DeserializeObject<XacNhanCapCodeTimKiemNangCaoVo>(strQueryInfo);
            }

            #region Ngày tìm kiếm
            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (queryInfo.TuNgayDenNgay != null && !string.IsNullOrEmpty(queryInfo.TuNgayDenNgay.TuNgay))
            {
                DateTime.TryParseExact(queryInfo.TuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (queryInfo.TuNgayDenNgay != null && !string.IsNullOrEmpty(queryInfo.TuNgayDenNgay.DenNgay))
            {
                DateTime.TryParseExact(queryInfo.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }
            #endregion

            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<BenhNhanXetNghiemGridVo>("STT", p => ind++)
            };

            var title = "DANH SÁCH LẤY MẪU";
            var ngayLayMau = (DateTime?)null;
            if (!phienXetNghiemDaCaps.Any(x => x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId == null))
            {
                var congTys = phienXetNghiemDaCaps.Select(x => x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoe).Distinct().ToList();
                if (congTys.Count == 1)
                {
                    title += " ĐOÀN " + congTys.First().Ten;
                    var diaDiems = phienXetNghiemDaCaps
                        .SelectMany(x => x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.HopDongKhamSucKhoeDiaDiems)
                        .Where(x => x.CongViec.Trim().ToLower().Contains("lấy mẫu") && x.Ngay != null).ToList();
                    if (diaDiems.Any())
                    {
                        ngayLayMau = diaDiems.Select(x => x.Ngay.Value).First();
                    }
                }
            }
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add(title);

                    // set row
                    worksheet.Row(9).Height = 24.5;
                    worksheet.DefaultRowHeight = 16;

                    worksheet.Column(2).Width = 20;
                    worksheet.Column(3).Width = 25;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 20;
                    worksheet.Column(6).Width = 20;
                    worksheet.Column(7).Width = 20;
                    worksheet.Column(8).Width = 25;
                    worksheet.Column(9).Width = 15;
                    worksheet.Column(10).Width = 25;
                    worksheet.Column(11).Width = 25;
                    worksheet.Column(12).Width = 25;
                    worksheet.Column(13).Width = 25;
                    worksheet.Column(14).Width = 25;
                    worksheet.Column(15).Width = 25;

                    worksheet.DefaultColWidth = 7;

                    //set column 
                    string[] SetColumnItems = { "A", "B", "C", "D", "E", "F", "G", "H"};
                    var worksheetTitle = SetColumnItems[0] + 1 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 1;
                    var worksheetTitleNgayLayMau = SetColumnItems[0] + 2 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 2;
                    var worksheetTitleNguoiPhuTrach = SetColumnItems[0] + 3 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 3;
                    var worksheetTitleHeader = SetColumnItems[0] + 6 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 7;

                    using (var range = worksheet.Cells[worksheetTitle])
                    {
                        range.Worksheet.Cells[worksheetTitle].Merge = true;
                        range.Worksheet.Cells[worksheetTitle].Value = title.ToUpper();
                        range.Worksheet.Cells[worksheetTitle].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitle].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitle].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitle].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitle].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleNgayLayMau])
                    {
                        range.Worksheet.Cells[worksheetTitleNgayLayMau].Merge = true;
                        range.Worksheet.Cells[worksheetTitleNgayLayMau].Value = "Ngày lấy mẫu: " + (ngayLayMau == null ? "" : ngayLayMau?.FormatNgayGioTimKiemTrenBaoCao());
                        range.Worksheet.Cells[worksheetTitleNgayLayMau].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleNgayLayMau].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells[worksheetTitleNgayLayMau].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitleNgayLayMau].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleNgayLayMau].Style.Font.Bold = true;
                        range.Worksheet.Cells[worksheetTitleNgayLayMau].Style.Font.Italic = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleNguoiPhuTrach])
                    {
                        range.Worksheet.Cells[worksheetTitleNguoiPhuTrach].Merge = true;
                        range.Worksheet.Cells[worksheetTitleNguoiPhuTrach].Value = "Người phụ trách: .................................................................";
                        range.Worksheet.Cells[worksheetTitleNguoiPhuTrach].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleNguoiPhuTrach].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells[worksheetTitleNguoiPhuTrach].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitleNguoiPhuTrach].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleNguoiPhuTrach].Style.Font.Bold = true;
                        range.Worksheet.Cells[worksheetTitleNguoiPhuTrach].Style.Font.Italic = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleHeader])
                    {
                        range.Worksheet.Cells[worksheetTitleHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Bold = true;

                        string[,] SetColumns = { { "A" , "STT" }, { "B", "Code" }, { "C", "Họ Tên" } , { "D", "Năm sinh" },
                            { "E", "Giới Tính" }, { "G", "Xác Nhận Lấy Mẫu" }, { "H", "Ghi Chú" }};

                        for (int i = 0; i < SetColumns.Length / 2; i++)
                        {
                            var setColumn = string.Empty;
                            if ((SetColumns[i, 0]).ToString() == "E")
                            {
                                setColumn = ((SetColumns[i, 0]).ToString() + 6 + ":F" + 6).ToString();
                                range.Worksheet.Cells[(SetColumns[i, 0]).ToString() + 7].Value = "Nam";
                                range.Worksheet.Cells["F" + 7].Value = "Nữ";
                            }
                            else
                            {
                                setColumn = ((SetColumns[i, 0]).ToString() + 6 + ":" + (SetColumns[i, 0]).ToString() + 7).ToString();
                            }
                            range.Worksheet.Cells[setColumn].Merge = true;
                            range.Worksheet.Cells[setColumn].Value = SetColumns[i, 1];
                        }

                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    var manager = new PropertyManager<BenhNhanXetNghiemGridVo>(requestProperties);
                    int index = 8;
                    var worksheetFirstLast = SetColumnItems[0] + index + ":" + SetColumnItems[SetColumnItems.Length - 1] + index;
                    
                    foreach (var benhNhan in dataCapCodes)
                    {
                        manager.CurrentObject = benhNhan;
                        manager.WriteToXlsx(worksheet, index);

                        worksheet.Cells["A" + index + ":" + "H" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["A" + index + ":" + "H" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["A" + index + ":" + "H" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["A" + index + ":" + "H" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["A" + index + ":" + "H" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells["B" + index].Value = benhNhan.Barcode;
                        worksheet.Cells["C" + index].Value = benhNhan.HoTen;
                        worksheet.Cells["D" + index].Value = benhNhan.NamSinhDisplay;
                        worksheet.Cells["E" + index].Value = benhNhan.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNam ? "Nam" : "";
                        worksheet.Cells["F" + index].Value = benhNhan.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNu ? "Nữ" : "";

                        worksheet.Row(index).Height = 20.5;

                        index++;
                    }

                    #region // xử lý thông tin tổng hợp
                    index++;

                    var worksheetTongSoMau = "A" + index + ":C" + index;
                    using (var range = worksheet.Cells[worksheetTongSoMau])
                    {
                        range.Worksheet.Cells[worksheetTongSoMau].Merge = true;
                        range.Worksheet.Cells[worksheetTongSoMau].Value = "Tổng số mẫu lấy được: " + phienXetNghiemDaCaps.SelectMany(x => x.MauXetNghiems).Where(x => x.DatChatLuong != false).Count();
                    }

                    var worksheetTongSoMauMau = "D" + index;
                    using (var range = worksheet.Cells[worksheetTongSoMauMau])
                    {
                        range.Worksheet.Cells[worksheetTongSoMauMau].Merge = true;
                        range.Worksheet.Cells[worksheetTongSoMauMau].Value = "Máu: " + phienXetNghiemDaCaps.SelectMany(x => x.MauXetNghiems).Where(x => x.DatChatLuong != false && x.LoaiMauXetNghiem == Enums.EnumLoaiMauXetNghiem.Mau).Count();
                    }

                    var worksheetTongSoMauNuocTieu = "F" + index;
                    using (var range = worksheet.Cells[worksheetTongSoMauNuocTieu])
                    {
                        range.Worksheet.Cells[worksheetTongSoMauNuocTieu].Merge = true;
                        range.Worksheet.Cells[worksheetTongSoMauNuocTieu].Value = "Nước tiểu: " + phienXetNghiemDaCaps.SelectMany(x => x.MauXetNghiems).Where(x => x.DatChatLuong != false && x.LoaiMauXetNghiem == Enums.EnumLoaiMauXetNghiem.NuocTieu).Count();
                    }

                    string[] arrCellMau = {"H", "J", "L", "N"};

                    var slMauDom = phienXetNghiemDaCaps.SelectMany(x => x.MauXetNghiems).Where(x => x.DatChatLuong != false && x.LoaiMauXetNghiem == Enums.EnumLoaiMauXetNghiem.Dom).Count();
                    if (slMauDom > 0)
                    {
                        var worksheetTongSoMauDom = arrCellMau.First() + index;
                        using (var range = worksheet.Cells[worksheetTongSoMauDom])
                        {
                            range.Worksheet.Cells[worksheetTongSoMauDom].Merge = true;
                            range.Worksheet.Cells[worksheetTongSoMauDom].Value = "Đờm: " + slMauDom;
                        }

                        arrCellMau = arrCellMau.Skip(1).ToArray();
                    }

                    var slMauPhan = phienXetNghiemDaCaps.SelectMany(x => x.MauXetNghiems).Where(x => x.DatChatLuong != false && x.LoaiMauXetNghiem == Enums.EnumLoaiMauXetNghiem.Phan).Count();
                    if (slMauPhan > 0)
                    {
                        var worksheetTongSoMauPhan = arrCellMau.First() + index;
                        using (var range = worksheet.Cells[worksheetTongSoMauPhan])
                        {
                            range.Worksheet.Cells[worksheetTongSoMauPhan].Merge = true;
                            range.Worksheet.Cells[worksheetTongSoMauPhan].Value = "Phân: " + slMauPhan;
                        }

                        arrCellMau = arrCellMau.Skip(1).ToArray();
                    }
                    

                    var slMauDich = phienXetNghiemDaCaps.SelectMany(x => x.MauXetNghiems).Where(x => x.DatChatLuong != false && x.LoaiMauXetNghiem == Enums.EnumLoaiMauXetNghiem.Dich).Count();
                    if (slMauDich > 0)
                    {
                        var worksheetTongSoMauDich = arrCellMau.First() + index;
                        using (var range = worksheet.Cells[worksheetTongSoMauDich])
                        {
                            range.Worksheet.Cells[worksheetTongSoMauDich].Merge = true;
                            range.Worksheet.Cells[worksheetTongSoMauDich].Value = "Dịch: " + slMauDich;
                        }

                        arrCellMau = arrCellMau.Skip(1).ToArray();
                    }

                    var slMauKhac = phienXetNghiemDaCaps.SelectMany(x => x.MauXetNghiems).Where(x => x.DatChatLuong != false && x.LoaiMauXetNghiem == Enums.EnumLoaiMauXetNghiem.Khac).Count();
                    if (slMauKhac > 0)
                    {
                        var worksheetTongSoMauKhac = arrCellMau.First() + index;
                        using (var range = worksheet.Cells[worksheetTongSoMauKhac])
                        {
                            range.Worksheet.Cells[worksheetTongSoMauKhac].Merge = true;
                            range.Worksheet.Cells[worksheetTongSoMauKhac].Value = "Khác: " + slMauKhac;
                        }
                    }
                    #endregion


                    xlPackage.Save();
                }

                return stream.ToArray();
            }
        }
        #endregion

        #region Phiếu Trả Dược phẩm
        public virtual byte[] ExportPhieuTraThuocTuBenhNhanNoiTru(ICollection<YeuCauTraThuocTuBenhNhanGridVo> dataTraThuocs, string strQueryInfo)
        {
            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<YeuCauTraThuocTuBenhNhanGridVo>("STT", p => ind++)
            };
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("DANH SÁCH PHIẾU TRẢ THUỐC TỪ NGƯỜI BỆNH NỘI TRÚ");

                    // set row
                    worksheet.Row(9).Height = 24.5;
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 30;
                    worksheet.Column(2).Width = 30;
                    worksheet.Column(3).Width = 30;
                    worksheet.Column(4).Width = 30;
                    worksheet.Column(5).Width = 30;
                    worksheet.Column(6).Width = 30;
                    worksheet.Column(7).Width = 30;
                    worksheet.Column(8).Width = 30;
                    worksheet.Column(9).Width = 30;
                    worksheet.Column(10).Width = 30;
                    worksheet.Column(11).Width = 30;
                    worksheet.Column(12).Width = 30;
                    worksheet.Column(13).Width = 30;
                    worksheet.Column(14).Width = 30;
                    worksheet.Column(15).Width = 30;
                    worksheet.Column(16).Width = 30;
                    worksheet.Column(17).Width = 30;

                    worksheet.DefaultColWidth = 7;

                    //set column 
                    string[] SetColumnItems = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K" };
                    var worksheetTitle = SetColumnItems[0] + 1 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 1;
                    var worksheetTitleStatus = SetColumnItems[0] + 2 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 2;
                    var worksheetTitleNgay = SetColumnItems[0] + 3 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 3;
                    var worksheetTitleHeader = SetColumnItems[0] + 3 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 6;

                    using (var range = worksheet.Cells[worksheetTitle])
                    {
                        range.Worksheet.Cells[worksheetTitle].Merge = true;
                        range.Worksheet.Cells[worksheetTitle].Value = "DANH SÁCH PHIẾU TRẢ THUỐC TỪ NGƯỜI BỆNH NỘI TRÚ".ToUpper();
                        range.Worksheet.Cells[worksheetTitle].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitle].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitle].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitle].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitle].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleHeader])
                    {
                        range.Worksheet.Cells[worksheetTitleHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Bold = true;

                        string[,] SetColumns = { { "A" , "STT" }, { "B", "Số Phiếu" }, { "C", "Khoa Hoàn Trả" } , { "D", "Hoàn Trả Về Kho" },
                            { "E", "Người Yêu Cầu" }, { "F", "Ngày Yêu Cầu" },{ "G", "Tình Trạng" }, { "H", "Người Duyệt" }, { "I", "Ngày Duyệt" }};

                        for (int i = 0; i < SetColumns.Length / 2; i++)
                        {
                            var setColumn = ((SetColumns[i, 0]).ToString() + 4 + ":" + (SetColumns[i, 0]).ToString() + 6).ToString();
                            range.Worksheet.Cells[setColumn].Merge = true;
                            range.Worksheet.Cells[setColumn].Value = SetColumns[i, 1];
                        }

                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    using (var range = worksheet.Cells["A6:K7"])
                    {
                        range.Worksheet.Cells["A6:K8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A6:K8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A6:K8"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A6:K8"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A6:K8"].Style.Font.Bold = true;

                        //Set column A to K 

                        string[,] SetColumns = { { "A" , "STT" }, { "B", "Số Phiếu" }, { "C", "Khoa Hoàn Trả" } , { "D", "Hoàn Trả Về Kho" },
                            { "E", "Người Yêu Cầu" }, { "F", "Ngày Yêu Cầu" },{ "G", "Tình Trạng" }, { "H", "Người Duyệt" }, { "I", "Ngày Duyệt" }};

                        for (int i = 0; i < SetColumns.Length / 2; i++)
                        {
                            var setColumn = ((SetColumns[i, 0]).ToString() + 4 + ":" + (SetColumns[i, 0]).ToString() + 6).ToString();
                            range.Worksheet.Cells[setColumn].Merge = true;
                            range.Worksheet.Cells[setColumn].Value = SetColumns[i, 1];
                        }

                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    var manager = new PropertyManager<YeuCauTraThuocTuBenhNhanGridVo>(requestProperties);
                    int index = 9;
                    var worksheetFirstLast = SetColumnItems[0] + index + ":" + SetColumnItems[SetColumnItems.Length - 1] + index;

                    int sttYCTN = 1;
                    foreach (var yeuCau in dataTraThuocs)
                    {
                        manager.CurrentObject = yeuCau;
                        manager.WriteToXlsx(worksheet, index);

                        worksheet.Cells["A" + index].Value = sttYCTN++;
                        worksheet.Cells["B" + index].Value = yeuCau.SoPhieu;
                        worksheet.Cells["C" + index].Value = yeuCau.TenKhoa;
                        worksheet.Cells["D" + index].Value = yeuCau.TenKho;
                        worksheet.Cells["E" + index].Value = yeuCau.NhanVienYeuCau;
                        worksheet.Cells["F" + index].Value = yeuCau.NgayYeuCauDisplay;
                        worksheet.Cells["G" + index].Value = yeuCau.TinhTrang == 0 ? "Chờ duyệt" : (yeuCau.TinhTrang == 1 ? "Được duyệt" : "Từ chối");
                        worksheet.Cells["H" + index].Value = yeuCau.NhanVienDuyet;
                        worksheet.Cells["I" + index].Value = yeuCau.NgayDuyetDisplay;

                        for (int ii = 0; ii < SetColumnItems.Length; ii++)
                        {
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Bold = true;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Fill.BackgroundColor.SetColor(Color.LightSteelBlue);
                        }

                        worksheet.Row(index).Height = 20.5;

                        var indexMain = index;
                        index++;

                        using (var range = worksheet.Cells["A" + index + ":K" + index])
                        {
                            range.Worksheet.Cells["B" + index + ":K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells["B" + index + ":K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells["B" + index + ":K" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                            range.Worksheet.Cells["B" + index + ":K" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["B" + index + ":K" + index].Style.Font.Bold = true;
                            range.Worksheet.Cells["B" + index + ":K" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            range.Worksheet.Cells["B" + index + ":K" + index].Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);

                            //Set column A to K 


                            string[,] SetColumnThuocs = { { "B" , "STT" }, { "C", "Dược Phẩm" }, { "D", "Hoạt Chất" } , { "E", "ĐVT" },
                                { "F", "Tổng SL Chỉ Định" }, { "G", "Tổng SL Đã Trả" }, { "H", "Tổng SL Trả Lần Này" }};

                            for (int i = 0; i < SetColumnThuocs.Length / 2; i++)
                            {
                                var setColumn = ((SetColumnThuocs[i, 0]).ToString() + index + ":" + (SetColumnThuocs[i, 0]).ToString() + index).ToString();
                                range.Worksheet.Cells[setColumn].Merge = true;
                                range.Worksheet.Cells[setColumn].Value = SetColumnThuocs[i, 1];
                            }

                            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        }
                        index++;

                        int sttItems = 1;
                        foreach (var traThuoc in yeuCau.TraDuocPhamChiTietVos.Where(p => p.LaDuocPhamBHYT == true))
                        {
                            worksheet.Cells["B" + index].Value = sttItems++;
                            worksheet.Cells["C" + index].Value = traThuoc.Ten;
                            worksheet.Cells["D" + index].Value = traThuoc.HoatChat;
                            worksheet.Cells["E" + index].Value = traThuoc.DVT;
                            worksheet.Cells["F" + index].Value = traThuoc.TongSLChiDinh;
                            worksheet.Cells["G" + index].Value = traThuoc.TongSLDaTra;
                            worksheet.Cells["H" + index].Value = traThuoc.TongSLDaTraLanNay;

                            for (int ii = 0; ii < SetColumnItems.Length; ii++)
                            {
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            }

                            index++;

                            using (var range = worksheet.Cells["A" + index + ":K" + index])
                            {
                                range.Worksheet.Cells["C" + index + ":K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                range.Worksheet.Cells["C" + index + ":K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                range.Worksheet.Cells["C" + index + ":K" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                range.Worksheet.Cells["C" + index + ":K" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["C" + index + ":K" + index].Style.Font.Bold = true;
                                range.Worksheet.Cells["C" + index + ":K" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                range.Worksheet.Cells["C" + index + ":K" + index].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

                                //Set column A to K 
                                //string[,] SetColumnBenhNhans = { { "D" , "STT" }, { "E", "Ngày Điều Trị" }, { "F", "Ngày Trả" } , { "G", "Người Trả" },
                                //    { "H", "SL Đã Trả" }, { "I", "SL Trả Lần Này" }, { "J", "Đơn Giá" },  { "K", "Thành Tiền" }};

                                string[,] SetColumnBenhNhans = { { "D" , "STT" }, { "E", "Ngày Điều Trị" }, { "F", "Ngày Trả" } , { "G", "Người Trả" },
                                    { "H", "SL Trả Lần Này" }, { "I", "Đơn Giá" }, { "J", "Thành Tiền" },};

                                for (int i = 0; i < SetColumnBenhNhans.Length / 2; i++)
                                {
                                    var setColumn = ((SetColumnBenhNhans[i, 0]).ToString() + index + ":" + (SetColumnBenhNhans[i, 0]).ToString() + index).ToString();
                                    range.Worksheet.Cells[setColumn].Merge = true;
                                    range.Worksheet.Cells[setColumn].Value = SetColumnBenhNhans[i, 1];
                                }

                                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            }
                            index++;

                            var sttDichVu = 1;
                            if (traThuoc.TraDuocPhamBenhNhanChiTietVos.Any())
                            {
                                using (var range = worksheet.Cells["B" + index + ":B" + index])
                                {
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.Bold = true;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Fill.BackgroundColor.SetColor(Color.RosyBrown);
                                    //Set column A to K
                                    string[,] SetColumnLoaiDuocPham = { { "B", "Không BHYT" } };

                                    for (int i = 0; i < SetColumnLoaiDuocPham.Length / 2; i++)
                                    {
                                        var setColumn = ((SetColumnLoaiDuocPham[i, 0]).ToString() + index + ":" + (SetColumnLoaiDuocPham[i, 0]).ToString() + index).ToString();
                                        range.Worksheet.Cells[setColumn].Merge = true;
                                        range.Worksheet.Cells[setColumn].Value = SetColumnLoaiDuocPham[i, 1];
                                    }
                                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                }
                                index++;

                                using (var range = worksheet.Cells["C" + index + ":C" + index])
                                {
                                    range.Worksheet.Cells["C" + index + ":C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    range.Worksheet.Cells["C" + index + ":C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    range.Worksheet.Cells["C" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                    range.Worksheet.Cells["C" + index + ":C" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["C" + index + ":C" + index].Style.Font.Bold = true;
                                    range.Worksheet.Cells["C" + index + ":C" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    range.Worksheet.Cells["C" + index + ":C" + index].Style.Fill.BackgroundColor.SetColor(Color.RosyBrown);
                                    //Set column A to K
                                    var benhNhan = traThuoc.TraDuocPhamBenhNhanChiTietVos.First().BenhNhan.Replace("<b>", "").Replace("</b>", "").Trim();
                                    string[,] SetColumnLoaiBenhNhan = { { "C", benhNhan } };

                                    for (int i = 0; i < SetColumnLoaiBenhNhan.Length / 2; i++)
                                    {
                                        var setColumn = ((SetColumnLoaiBenhNhan[i, 0]).ToString() + index + ":" + (SetColumnLoaiBenhNhan[i, 0]).ToString() + index).ToString();
                                        range.Worksheet.Cells[setColumn].Merge = true;
                                        range.Worksheet.Cells[setColumn].Value = SetColumnLoaiBenhNhan[i, 1];
                                    }
                                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                }
                                index++;

                                foreach (var benhNhan in traThuoc.TraDuocPhamBenhNhanChiTietVos)
                                {
                                    worksheet.Cells["D" + index].Value = sttDichVu++;
                                    worksheet.Cells["E" + index].Value = benhNhan.NgayDieuTriDisplay;
                                    worksheet.Cells["F" + index].Value = benhNhan.NgayTraDisplay;
                                    worksheet.Cells["G" + index].Value = benhNhan.NhanVienYeuCau;
                                    worksheet.Cells["H" + index].Value = benhNhan.SLTraLanNay;
                                    worksheet.Cells["I" + index].Value = benhNhan.DonGia.ApplyFormatMoneyVND();
                                    worksheet.Cells["J" + index].Value = benhNhan.ThanhTien.ApplyFormatMoneyVND();
                                    //worksheet.Cells["K" + index].Value = benhNhan.ThanhTien.ApplyFormatMoneyVND();
                                    for (int ii = 0; ii < SetColumnItems.Length; ii++)
                                    {
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    }

                                    index++;
                                }
                            }

                        }

                        foreach (var traThuoc in yeuCau.TraDuocPhamChiTietVos.Where(p => p.LaDuocPhamBHYT != true))
                        {
                            worksheet.Cells["B" + index].Value = sttItems++;
                            worksheet.Cells["C" + index].Value = traThuoc.Ten;
                            worksheet.Cells["D" + index].Value = traThuoc.HoatChat;
                            worksheet.Cells["E" + index].Value = traThuoc.DVT;
                            worksheet.Cells["F" + index].Value = traThuoc.TongSLChiDinh;
                            worksheet.Cells["G" + index].Value = traThuoc.TongSLDaTra;
                            worksheet.Cells["H" + index].Value = traThuoc.TongSLDaTraLanNay;

                            for (int ii = 0; ii < SetColumnItems.Length; ii++)
                            {
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            }

                            index++;

                            using (var range = worksheet.Cells["A" + index + ":K" + index])
                            {
                                range.Worksheet.Cells["C" + index + ":K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                range.Worksheet.Cells["C" + index + ":K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                range.Worksheet.Cells["C" + index + ":K" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                range.Worksheet.Cells["C" + index + ":K" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["C" + index + ":K" + index].Style.Font.Bold = true;
                                range.Worksheet.Cells["C" + index + ":K" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                range.Worksheet.Cells["C" + index + ":K" + index].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

                                //Set column A to K 
                                string[,] SetColumnBenhNhans = { { "D" , "STT" }, { "E", "Ngày Điều Trị" }, { "F", "Ngày Trả" } , { "G", "Người Trả" },
                                    { "H", "SL Trả Lần Này" }, { "I", "Đơn Giá" }, { "J", "Thành Tiền" },};


                                for (int i = 0; i < SetColumnBenhNhans.Length / 2; i++)
                                {
                                    var setColumn = ((SetColumnBenhNhans[i, 0]).ToString() + index + ":" + (SetColumnBenhNhans[i, 0]).ToString() + index).ToString();
                                    range.Worksheet.Cells[setColumn].Merge = true;
                                    range.Worksheet.Cells[setColumn].Value = SetColumnBenhNhans[i, 1];
                                }

                                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            }
                            index++;

                            var sttDichVu = 1;
                            if (traThuoc.TraDuocPhamBenhNhanChiTietVos.Any())
                            {
                                using (var range = worksheet.Cells["B" + index + ":B" + index])
                                {
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.Bold = true;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Fill.BackgroundColor.SetColor(Color.RosyBrown);
                                    //Set column A to K
                                    string[,] SetColumnLoaiDuocPham = { { "B", "Không BHYT" } };

                                    for (int i = 0; i < SetColumnLoaiDuocPham.Length / 2; i++)
                                    {
                                        var setColumn = ((SetColumnLoaiDuocPham[i, 0]).ToString() + index + ":" + (SetColumnLoaiDuocPham[i, 0]).ToString() + index).ToString();
                                        range.Worksheet.Cells[setColumn].Merge = true;
                                        range.Worksheet.Cells[setColumn].Value = SetColumnLoaiDuocPham[i, 1];
                                    }
                                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                }
                                index++;

                                using (var range = worksheet.Cells["C" + index + ":C" + index])
                                {
                                    range.Worksheet.Cells["C" + index + ":C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    range.Worksheet.Cells["C" + index + ":C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    range.Worksheet.Cells["C" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                    range.Worksheet.Cells["C" + index + ":C" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["C" + index + ":C" + index].Style.Font.Bold = true;
                                    range.Worksheet.Cells["C" + index + ":C" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    range.Worksheet.Cells["C" + index + ":C" + index].Style.Fill.BackgroundColor.SetColor(Color.RosyBrown);
                                    //Set column A to K
                                    var benhNhan = traThuoc.TraDuocPhamBenhNhanChiTietVos.First().BenhNhan.Replace("<b>", "").Replace("</b>", "").Trim();
                                    string[,] SetColumnLoaiBenhNhan = { { "C", benhNhan } };

                                    for (int i = 0; i < SetColumnLoaiBenhNhan.Length / 2; i++)
                                    {
                                        var setColumn = ((SetColumnLoaiBenhNhan[i, 0]).ToString() + index + ":" + (SetColumnLoaiBenhNhan[i, 0]).ToString() + index).ToString();
                                        range.Worksheet.Cells[setColumn].Merge = true;
                                        range.Worksheet.Cells[setColumn].Value = SetColumnLoaiBenhNhan[i, 1];
                                    }
                                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                }
                                index++;

                                foreach (var benhNhan in traThuoc.TraDuocPhamBenhNhanChiTietVos)
                                {
                                    worksheet.Cells["D" + index].Value = sttDichVu++;
                                    worksheet.Cells["E" + index].Value = benhNhan.NgayDieuTriDisplay;
                                    worksheet.Cells["F" + index].Value = benhNhan.NgayTraDisplay;
                                    worksheet.Cells["G" + index].Value = benhNhan.NhanVienYeuCau;
                                    worksheet.Cells["H" + index].Value = benhNhan.SLTraLanNay;
                                    worksheet.Cells["I" + index].Value = benhNhan.DonGia.ApplyFormatMoneyVND();
                                    worksheet.Cells["J" + index].Value = benhNhan.ThanhTien.ApplyFormatMoneyVND();

                                    for (int ii = 0; ii < SetColumnItems.Length; ii++)
                                    {
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    }

                                    index++;
                                }
                            }

                        }


                        for (int inde = indexMain + 1; inde <= index - 1; inde++)
                        {
                            worksheet.Row(inde).OutlineLevel = 1;
                        }

                        index++;
                    }

                    xlPackage.Save();
                }

                return stream.ToArray();
            }
        }
        #endregion

        #region Phiếu Trả Vật Tư
        public virtual byte[] ExportPhieuTraVatTuTuBenhNhanNoiTru(ICollection<YeuCauTraVatTuTuBenhNhanGridVo> dataTraVatTus, string strQueryInfo)
        {
            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<YeuCauTraVatTuTuBenhNhanGridVo>("STT", p => ind++)
            };
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("DANH SÁCH PHIẾU TRẢ VẬT TƯ TỪ NGƯỜI BỆNH NỘI TRÚ");

                    // set row
                    worksheet.Row(9).Height = 24.5;
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 30;
                    worksheet.Column(2).Width = 30;
                    worksheet.Column(3).Width = 30;
                    worksheet.Column(4).Width = 30;
                    worksheet.Column(5).Width = 30;
                    worksheet.Column(6).Width = 30;
                    worksheet.Column(7).Width = 30;
                    worksheet.Column(8).Width = 30;
                    worksheet.Column(9).Width = 30;
                    worksheet.Column(10).Width = 30;
                    worksheet.Column(11).Width = 30;
                    worksheet.Column(12).Width = 30;
                    worksheet.Column(13).Width = 30;
                    worksheet.Column(14).Width = 30;
                    worksheet.Column(15).Width = 30;
                    worksheet.Column(16).Width = 30;
                    worksheet.Column(17).Width = 30;

                    worksheet.DefaultColWidth = 7;

                    //set column 
                    string[] SetColumnItems = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };
                    var worksheetTitle = SetColumnItems[0] + 1 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 1;
                    var worksheetTitleStatus = SetColumnItems[0] + 2 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 2;
                    var worksheetTitleNgay = SetColumnItems[0] + 3 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 3;
                    var worksheetTitleHeader = SetColumnItems[0] + 3 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 6;

                    using (var range = worksheet.Cells[worksheetTitle])
                    {
                        range.Worksheet.Cells[worksheetTitle].Merge = true;
                        range.Worksheet.Cells[worksheetTitle].Value = "DANH SÁCH PHIẾU TRẢ VẬT TƯ TỪ NGƯỜI BỆNH NỘI TRÚ".ToUpper();
                        range.Worksheet.Cells[worksheetTitle].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitle].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitle].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitle].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitle].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleHeader])
                    {
                        range.Worksheet.Cells[worksheetTitleHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Bold = true;

                        string[,] SetColumns = { { "A" , "STT" }, { "B", "Số Phiếu" }, { "C", "Khoa Hoàn Trả" } , { "D", "Hoàn Trả Về Kho" },
                            { "E", "Người Yêu Cầu" }, { "F", "Ngày Yêu Cầu" },{ "G", "Tình Trạng" }, { "H", "Người Duyệt" }, { "I", "Ngày Duyệt" }};

                        for (int i = 0; i < SetColumns.Length / 2; i++)
                        {
                            var setColumn = ((SetColumns[i, 0]).ToString() + 4 + ":" + (SetColumns[i, 0]).ToString() + 6).ToString();
                            range.Worksheet.Cells[setColumn].Merge = true;
                            range.Worksheet.Cells[setColumn].Value = SetColumns[i, 1];
                        }

                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    using (var range = worksheet.Cells["A6:J7"])
                    {
                        range.Worksheet.Cells["A6:J8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A6:J8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A6:J8"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A6:J8"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A6:J8"].Style.Font.Bold = true;

                        //Set column A to K 

                        string[,] SetColumns = { { "A" , "STT" }, { "B", "Số Phiếu" }, { "C", "Khoa Hoàn Trả" } , { "D", "Hoàn Trả Về Kho" },
                            { "E", "Người Yêu Cầu" }, { "F", "Ngày Yêu Cầu" },{ "G", "Tình Trạng" }, { "H", "Người Duyệt" }, { "I", "Ngày Duyệt" }};

                        for (int i = 0; i < SetColumns.Length / 2; i++)
                        {
                            var setColumn = ((SetColumns[i, 0]).ToString() + 4 + ":" + (SetColumns[i, 0]).ToString() + 6).ToString();
                            range.Worksheet.Cells[setColumn].Merge = true;
                            range.Worksheet.Cells[setColumn].Value = SetColumns[i, 1];
                        }

                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    var manager = new PropertyManager<YeuCauTraVatTuTuBenhNhanGridVo>(requestProperties);
                    int index = 9;
                    var worksheetFirstLast = SetColumnItems[0] + index + ":" + SetColumnItems[SetColumnItems.Length - 1] + index;

                    int sttYCTN = 1;
                    foreach (var yeuCau in dataTraVatTus)
                    {
                        manager.CurrentObject = yeuCau;
                        manager.WriteToXlsx(worksheet, index);

                        worksheet.Cells["A" + index].Value = sttYCTN++;
                        worksheet.Cells["B" + index].Value = yeuCau.SoPhieu;
                        worksheet.Cells["C" + index].Value = yeuCau.TenKhoa;
                        worksheet.Cells["D" + index].Value = yeuCau.TenKho;
                        worksheet.Cells["E" + index].Value = yeuCau.NhanVienYeuCau;
                        worksheet.Cells["F" + index].Value = yeuCau.NgayYeuCauDisplay;
                        worksheet.Cells["G" + index].Value = yeuCau.TinhTrang == 0 ? "Chờ duyệt" : (yeuCau.TinhTrang == 1 ? "Được duyệt" : "Từ chối");
                        worksheet.Cells["H" + index].Value = yeuCau.NhanVienDuyet;
                        worksheet.Cells["I" + index].Value = yeuCau.NgayDuyetDisplay;

                        for (int ii = 0; ii < SetColumnItems.Length; ii++)
                        {
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Bold = true;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Fill.BackgroundColor.SetColor(Color.LightSteelBlue);
                        }

                        worksheet.Row(index).Height = 20.5;

                        var indexMain = index;
                        index++;

                        using (var range = worksheet.Cells["A" + index + ":J" + index])
                        {
                            range.Worksheet.Cells["B" + index + ":J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells["B" + index + ":J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells["B" + index + ":J" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                            range.Worksheet.Cells["B" + index + ":J" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["B" + index + ":J" + index].Style.Font.Bold = true;
                            range.Worksheet.Cells["B" + index + ":J" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            range.Worksheet.Cells["B" + index + ":J" + index].Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);

                            //Set column A to K 


                            string[,] SetColumnThuocs = { { "B" , "STT" }, { "C", "Vật Tư" } , { "D", "ĐVT" },
                                { "E", "Tổng SL Chỉ Định" }, { "F", "Tổng SL Đã Trả" }, { "G", "Tổng SL Trả Lần Này" }};

                            for (int i = 0; i < SetColumnThuocs.Length / 2; i++)
                            {
                                var setColumn = ((SetColumnThuocs[i, 0]).ToString() + index + ":" + (SetColumnThuocs[i, 0]).ToString() + index).ToString();
                                range.Worksheet.Cells[setColumn].Merge = true;
                                range.Worksheet.Cells[setColumn].Value = SetColumnThuocs[i, 1];
                            }

                            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        }
                        index++;

                        int sttItems = 1;
                        foreach (var traVatTu in yeuCau.TraVatTuChiTietVos.Where(p => p.LaVatTuBHYT == true))
                        {
                            worksheet.Cells["B" + index].Value = sttItems++;
                            worksheet.Cells["C" + index].Value = traVatTu.Ten;
                            worksheet.Cells["D" + index].Value = traVatTu.DVT;
                            worksheet.Cells["E" + index].Value = traVatTu.TongSLChiDinh;
                            worksheet.Cells["F" + index].Value = traVatTu.TongSLDaTra;
                            worksheet.Cells["G" + index].Value = traVatTu.TongSLDaTraLanNay;

                            for (int ii = 0; ii < SetColumnItems.Length; ii++)
                            {
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            }

                            index++;

                            using (var range = worksheet.Cells["A" + index + ":J" + index])
                            {
                                range.Worksheet.Cells["C" + index + ":K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                range.Worksheet.Cells["C" + index + ":J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                range.Worksheet.Cells["C" + index + ":J" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                range.Worksheet.Cells["C" + index + ":J" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["C" + index + ":J" + index].Style.Font.Bold = true;
                                range.Worksheet.Cells["C" + index + ":J" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                range.Worksheet.Cells["C" + index + ":J" + index].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

                                //Set column A to K 

                                string[,] SetColumnBenhNhans = { { "D" , "STT" }, { "E", "Ngày Điều Trị" }, { "F", "Ngày Trả" } , { "G", "Người Trả" },
                                   { "H", "SL Trả Lần Này" }, { "I", "Đơn Giá" },  { "J", "Thành Tiền" }};

                                for (int i = 0; i < SetColumnBenhNhans.Length / 2; i++)
                                {
                                    var setColumn = ((SetColumnBenhNhans[i, 0]).ToString() + index + ":" + (SetColumnBenhNhans[i, 0]).ToString() + index).ToString();
                                    range.Worksheet.Cells[setColumn].Merge = true;
                                    range.Worksheet.Cells[setColumn].Value = SetColumnBenhNhans[i, 1];
                                }

                                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            }
                            index++;

                            var sttDichVu = 1;
                            if (traVatTu.TraVatTuBenhNhanChiTietVos.Any())
                            {
                                using (var range = worksheet.Cells["B" + index + ":B" + index])
                                {
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.Bold = true;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Fill.BackgroundColor.SetColor(Color.RosyBrown);
                                    //Set column A to K
                                    string[,] SetColumnLoaiDuocPham = { { "B", "Không BHYT" } };

                                    for (int i = 0; i < SetColumnLoaiDuocPham.Length / 2; i++)
                                    {
                                        var setColumn = ((SetColumnLoaiDuocPham[i, 0]).ToString() + index + ":" + (SetColumnLoaiDuocPham[i, 0]).ToString() + index).ToString();
                                        range.Worksheet.Cells[setColumn].Merge = true;
                                        range.Worksheet.Cells[setColumn].Value = SetColumnLoaiDuocPham[i, 1];
                                    }
                                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                }
                                index++;

                                using (var range = worksheet.Cells["C" + index + ":C" + index])
                                {
                                    range.Worksheet.Cells["C" + index + ":C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    range.Worksheet.Cells["C" + index + ":C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    range.Worksheet.Cells["C" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                    range.Worksheet.Cells["C" + index + ":C" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["C" + index + ":C" + index].Style.Font.Bold = true;
                                    range.Worksheet.Cells["C" + index + ":C" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    range.Worksheet.Cells["C" + index + ":C" + index].Style.Fill.BackgroundColor.SetColor(Color.RosyBrown);
                                    //Set column A to K
                                    var benhNhan = traVatTu.TraVatTuBenhNhanChiTietVos.First().BenhNhan.Replace("<b>", "").Replace("</b>", "").Trim();
                                    string[,] SetColumnLoaiBenhNhan = { { "C", benhNhan } };

                                    for (int i = 0; i < SetColumnLoaiBenhNhan.Length / 2; i++)
                                    {
                                        var setColumn = ((SetColumnLoaiBenhNhan[i, 0]).ToString() + index + ":" + (SetColumnLoaiBenhNhan[i, 0]).ToString() + index).ToString();
                                        range.Worksheet.Cells[setColumn].Merge = true;
                                        range.Worksheet.Cells[setColumn].Value = SetColumnLoaiBenhNhan[i, 1];
                                    }
                                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                }
                                index++;

                                foreach (var benhNhan in traVatTu.TraVatTuBenhNhanChiTietVos)
                                {
                                    worksheet.Cells["D" + index].Value = sttDichVu++;
                                    worksheet.Cells["E" + index].Value = benhNhan.NgayDieuTriDisplay;
                                    worksheet.Cells["F" + index].Value = benhNhan.NgayTraDisplay;
                                    worksheet.Cells["G" + index].Value = benhNhan.NhanVienYeuCau;
                                    worksheet.Cells["H" + index].Value = benhNhan.SLTraLanNay;
                                    worksheet.Cells["I" + index].Value = benhNhan.DonGia.ApplyFormatMoneyVND();
                                    worksheet.Cells["J" + index].Value = benhNhan.ThanhTien.ApplyFormatMoneyVND();
                                    for (int ii = 0; ii < SetColumnItems.Length; ii++)
                                    {
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    }

                                    index++;
                                }
                            }

                        }

                        foreach (var traVatTu in yeuCau.TraVatTuChiTietVos.Where(p => p.LaVatTuBHYT != true))
                        {
                            worksheet.Cells["B" + index].Value = sttItems++;
                            worksheet.Cells["C" + index].Value = traVatTu.Ten;
                            worksheet.Cells["D" + index].Value = traVatTu.DVT;
                            worksheet.Cells["E" + index].Value = traVatTu.TongSLChiDinh;
                            worksheet.Cells["F" + index].Value = traVatTu.TongSLDaTra;
                            worksheet.Cells["G" + index].Value = traVatTu.TongSLDaTraLanNay;

                            for (int ii = 0; ii < SetColumnItems.Length; ii++)
                            {
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            }

                            index++;

                            using (var range = worksheet.Cells["A" + index + ":J" + index])
                            {
                                range.Worksheet.Cells["C" + index + ":J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                range.Worksheet.Cells["C" + index + ":J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                range.Worksheet.Cells["C" + index + ":J" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                range.Worksheet.Cells["C" + index + ":J" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["C" + index + ":J" + index].Style.Font.Bold = true;
                                range.Worksheet.Cells["C" + index + ":J" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                range.Worksheet.Cells["C" + index + ":J" + index].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

                                //Set column A to K 
                                string[,] SetColumnBenhNhans = { { "D" , "STT" }, { "E", "Ngày Điều Trị" }, { "F", "Ngày Trả" } , { "G", "Người Trả" },
                                   { "H", "SL Trả Lần Này" }, { "I", "Đơn Giá" },  { "J", "Thành Tiền" }};

                                for (int i = 0; i < SetColumnBenhNhans.Length / 2; i++)
                                {
                                    var setColumn = ((SetColumnBenhNhans[i, 0]).ToString() + index + ":" + (SetColumnBenhNhans[i, 0]).ToString() + index).ToString();
                                    range.Worksheet.Cells[setColumn].Merge = true;
                                    range.Worksheet.Cells[setColumn].Value = SetColumnBenhNhans[i, 1];
                                }

                                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            }
                            index++;

                            var sttDichVu = 1;
                            if (traVatTu.TraVatTuBenhNhanChiTietVos.Any())
                            {
                                using (var range = worksheet.Cells["B" + index + ":B" + index])
                                {
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.Bold = true;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Fill.BackgroundColor.SetColor(Color.RosyBrown);
                                    //Set column A to K
                                    string[,] SetColumnLoaiDuocPham = { { "B", "Không BHYT" } };

                                    for (int i = 0; i < SetColumnLoaiDuocPham.Length / 2; i++)
                                    {
                                        var setColumn = ((SetColumnLoaiDuocPham[i, 0]).ToString() + index + ":" + (SetColumnLoaiDuocPham[i, 0]).ToString() + index).ToString();
                                        range.Worksheet.Cells[setColumn].Merge = true;
                                        range.Worksheet.Cells[setColumn].Value = SetColumnLoaiDuocPham[i, 1];
                                    }
                                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                }
                                index++;

                                using (var range = worksheet.Cells["C" + index + ":C" + index])
                                {
                                    range.Worksheet.Cells["C" + index + ":C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    range.Worksheet.Cells["C" + index + ":C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    range.Worksheet.Cells["C" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                    range.Worksheet.Cells["C" + index + ":C" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["C" + index + ":C" + index].Style.Font.Bold = true;
                                    range.Worksheet.Cells["C" + index + ":C" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    range.Worksheet.Cells["C" + index + ":C" + index].Style.Fill.BackgroundColor.SetColor(Color.RosyBrown);
                                    //Set column A to K
                                    var benhNhan = traVatTu.TraVatTuBenhNhanChiTietVos.First().BenhNhan.Replace("<b>", "").Replace("</b>", "").Trim();
                                    string[,] SetColumnLoaiBenhNhan = { { "C", benhNhan } };

                                    for (int i = 0; i < SetColumnLoaiBenhNhan.Length / 2; i++)
                                    {
                                        var setColumn = ((SetColumnLoaiBenhNhan[i, 0]).ToString() + index + ":" + (SetColumnLoaiBenhNhan[i, 0]).ToString() + index).ToString();
                                        range.Worksheet.Cells[setColumn].Merge = true;
                                        range.Worksheet.Cells[setColumn].Value = SetColumnLoaiBenhNhan[i, 1];
                                    }
                                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                }
                                index++;

                                foreach (var benhNhan in traVatTu.TraVatTuBenhNhanChiTietVos)
                                {
                                    worksheet.Cells["D" + index].Value = sttDichVu++;
                                    worksheet.Cells["E" + index].Value = benhNhan.NgayDieuTriDisplay;
                                    worksheet.Cells["F" + index].Value = benhNhan.NgayTraDisplay;
                                    worksheet.Cells["G" + index].Value = benhNhan.NhanVienYeuCau;
                                    worksheet.Cells["H" + index].Value = benhNhan.SLTraLanNay;
                                    worksheet.Cells["I" + index].Value = benhNhan.DonGia.ApplyFormatMoneyVND();
                                    worksheet.Cells["J" + index].Value = benhNhan.ThanhTien.ApplyFormatMoneyVND();

                                    for (int ii = 0; ii < SetColumnItems.Length; ii++)
                                    {
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    }

                                    index++;
                                }
                            }

                        }


                        for (int inde = indexMain + 1; inde <= index - 1; inde++)
                        {
                            worksheet.Row(inde).OutlineLevel = 1;
                        }

                        index++;
                    }

                    xlPackage.Save();
                }

                return stream.ToArray();
            }
        }
        #endregion

        #region Báo cáo danh thu theo nhóm dịch vụ

        public virtual byte[] ExportBaoCaoDoanhThuTheoNhomBenhVien(GridDataSource gridDataSource,
                                                                   BaoCaoDoanhThuTheoNhomDichVuSearchQueryInfo queryInfo)
        {
            //var tuNgay = queryInfo.TuNgay == DateTime.MinValue ? DateTime.Now : queryInfo.TuNgay;
            //var denNgay = queryInfo.DenNgay ?? DateTime.Now;

            var baoCaoThuPhiVienPhiGridVos = (ICollection<BaoCaoDoanhThuTheoNhomDichVuGridVo>)gridDataSource.Data;

            var dataBaoCaos = baoCaoThuPhiVienPhiGridVos.GroupBy(p => p.Nhom, (key, g) => new { ThuNgan = key, Data = g.ToList() });

            var requestProperties = new[] { new PropertyByName<BaoCaoDoanhThuTheoNhomDichVuGridVo>("BaoCaoDoanhThuTheoNhomDichVu", p => p.Nhom) };

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BaoCaoDoanhThuTheoNhomDichVu");
                    // set row
                    worksheet.Row(9).Height = 24.5;
                    worksheet.DefaultRowHeight = 16;

                    // set column
                    worksheet.Column(2).Width = 30;
                    worksheet.Column(3).Width = 30;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 20;
                    worksheet.Column(6).Width = 20;
                    worksheet.Column(7).Width = 20;
                    worksheet.Column(8).Width = 20;
                    worksheet.Column(9).Width = 20;

                    worksheet.Column(10).Width = 15;
                    worksheet.Column(11).Width = 15;
                    worksheet.Column(12).Width = 15;
                    worksheet.Column(13).Width = 15;
                    worksheet.Column(14).Width = 15;
                    worksheet.Column(15).Width = 15;
                    worksheet.Column(16).Width = 15;
                    worksheet.Column(17).Width = 15;
                    worksheet.Column(18).Width = 15;
                    worksheet.Column(19).Width = 15;
                    worksheet.Column(20).Width = 15;
                    worksheet.Column(21).Width = 15;
                    worksheet.Column(22).Width = 15;
                    worksheet.Column(23).Width = 15;
                    worksheet.Column(24).Width = 25;
                    worksheet.Column(25).Width = 25;
                    worksheet.Column(26).Width = 15;
                    worksheet.Column(27).Width = 15;
                    worksheet.Column(28).Width = 15;
                    worksheet.Column(29).Width = 15;
                    worksheet.Column(30).Width = 15;
                    worksheet.Column(31).Width = 15;
                    worksheet.Column(32).Width = 15;

                    worksheet.DefaultColWidth = 7;

                    //set column 

                    string[] SetColumnItems = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" , "AA", "AB", "AC", "AD", "AE", "AF" };

                    var worksheetTitle = SetColumnItems[0] + 1 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 1;
                    var worksheetTitleThuPhi = SetColumnItems[0] + 2 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 2;
                    var worksheetTitleNgay = SetColumnItems[0] + 3 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 3;                  


                    using (var range = worksheet.Cells[worksheetTitle])
                    {
                        range.Worksheet.Cells[worksheetTitle].Merge = true;
                        range.Worksheet.Cells[worksheetTitle].Value = "BỆNH VIỆN ĐKQT BẮC HÀ".ToUpper();
                        range.Worksheet.Cells[worksheetTitle].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells[worksheetTitle].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells[worksheetTitle].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitle].Style.Font.Bold = true;
                    }


                    using (var range = worksheet.Cells[worksheetTitleThuPhi])
                    {
                        range.Worksheet.Cells[worksheetTitleThuPhi].Merge = true;
                        range.Worksheet.Cells[worksheetTitleThuPhi].Value = "BÁO CÁO DOANH THU THEO NHÓM DỊCH VỤ".ToUpper();
                        range.Worksheet.Cells[worksheetTitleThuPhi].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleThuPhi].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleThuPhi].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells[worksheetTitleThuPhi].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleThuPhi].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleNgay])
                    {
                        range.Worksheet.Cells[worksheetTitleNgay].Merge = true;                   

                        range.Worksheet.Cells[worksheetTitleNgay].Value = "Từ ngày: " + queryInfo.TuNgay.FormatNgayGioTimKiemTrenBaoCao() +
                                                                          "-đến ngày: " + queryInfo.DenNgay.FormatNgayGioTimKiemTrenBaoCao();

                        range.Worksheet.Cells[worksheetTitleNgay].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Italic = true;
                    }

                    using (var range = worksheet.Cells["A6:AF8"])
                    {
                        range.Worksheet.Cells["A6:AF8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A6:AF8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        range.Worksheet.Cells["A6:AF8"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A6:AF8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A6:AF8"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A6:AF8"].Style.Font.Italic = true;

                        //Set column A to Z 
                        string[,] SetColumns = { { "A" , "STT" }, { "B", "Mã NB" } , { "C", "Mã TN" } , { "D", "Họ và Tên" },
                                                 { "E", "Năm sinh" }, { "F", "Giới tính" }, { "G", "Số bệnh án" },
                                                 { "H", "Nội dung" },  { "I", "Ngày" }, { "J", "Người giới thiệu" },
                                                 { "K", "Giá trị nhập kho" },  { "L", "Giá niêm yết hiện tại của BV" }, { "M", "Số tiền được miễn giảm" },
                                                 { "N", "BHYT chi trả" },  { "O", "Hệ số tính theo HĐ với CTV" }
                        };

                        for (int i = 0; i < SetColumns.Length / 2; i++)
                        {
                            var setColumn = ((SetColumns[i, 0]).ToString() + 6 + ":" + (SetColumns[i, 0]).ToString() + 8).ToString();
                            range.Worksheet.Cells[setColumn].Style.WrapText = true;
                            range.Worksheet.Cells[setColumn].Merge = true;
                            range.Worksheet.Cells[setColumn].Value = SetColumns[i, 1];
                        }

                        for (int i = 0; i < SetColumnItems.Length; i++)
                        {
                            var setColumn = SetColumnItems[i] + 9 + ":" + SetColumnItems[i] + 9;
                            range.Worksheet.Cells[setColumn].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                            range.Worksheet.Cells[setColumn].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells[setColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells[setColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells[setColumn].Value = i + 1;
                        }


                        range.Worksheet.Cells["A10:O10"].Merge = true;
                        range.Worksheet.Cells["A10:O10"].Value = "Tổng cộng";
                        range.Worksheet.Cells["A10:O10"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["A10:O10"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A10:O10"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A10:O10"].Style.Font.Italic = true;


                        range.Worksheet.Cells["P6:AF6"].Merge = true;
                        range.Worksheet.Cells["P6:AF6"].Value = "Chi phí khám chữa bệnh phát sinh tại cơ sở KCB";                   

                        range.Worksheet.Cells["P7:P8"].Merge = true;
                        range.Worksheet.Cells["P7:P8"].Value = "Khám bệnh";

                        range.Worksheet.Cells["Q7:Q8"].Merge = true;
                        range.Worksheet.Cells["Q7:Q8"].Value = "Xét nghiệm";

                        range.Worksheet.Cells["R7:R8"].Merge = true;
                        range.Worksheet.Cells["R7:R8"].Value = "Nội soi";

                        range.Worksheet.Cells["S7:S8"].Merge = true;
                        range.Worksheet.Cells["S7:S8"].Value = "Nội soi TMH";

                        range.Worksheet.Cells["T7:T8"].Merge = true;
                        range.Worksheet.Cells["T7:T8"].Value = "Siêu âm";

                        range.Worksheet.Cells["U7:U8"].Merge = true;
                        range.Worksheet.Cells["U7:U8"].Value = "X-Quang";

                        range.Worksheet.Cells["V7:V8"].Merge = true;
                        range.Worksheet.Cells["V7:V8"].Value = "CT Scan";

                        range.Worksheet.Cells["W7:W8"].Merge = true;
                        range.Worksheet.Cells["W7:W8"].Value = "MRI";

                        range.Worksheet.Cells["X7:X8"].Merge = true;
                        range.Worksheet.Cells["X7:X8"].Value = "ĐiệnTim + Điện Não";

                        range.Worksheet.Cells["Y7:Y8"].Merge = true;
                        range.Worksheet.Cells["Y7:Y8"].Value = "TDCN + Đo loãng xương";


                        range.Worksheet.Cells["Z7:Z8"].Merge = true;
                        range.Worksheet.Cells["Z7:Z8"].Value = "Thủ thuật";

                        range.Worksheet.Cells["AA7:AA8"].Merge = true;
                        range.Worksheet.Cells["AA7:AA8"].Value = "Phẫu Thuật";

                        range.Worksheet.Cells["AB7:AB8"].Merge = true;
                        range.Worksheet.Cells["AB7:AB8"].Value = "Ngày giường";


                        range.Worksheet.Cells["AC7:AC8"].Merge = true;
                        range.Worksheet.Cells["AC7:AC8"].Value = "DV khác";

                        range.Worksheet.Cells["AD7:AD8"].Merge = true;
                        range.Worksheet.Cells["AD7:AD8"].Value = "Thuốc";

                        range.Worksheet.Cells["AE7:AE8"].Merge = true;
                        range.Worksheet.Cells["AE7:AE8"].Value = "VTYT";

                        range.Worksheet.Cells["AF7:AF8"].Merge = true;
                        range.Worksheet.Cells["AF7:AF8"].Value = "Tổng cộng"; 

                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    var manager = new PropertyManager<BaoCaoDoanhThuTheoNhomDichVuGridVo>(requestProperties);

                    var thuNgans = dataBaoCaos.Select(cc => cc.ThuNgan).ToArray();
                    int index = 11; // đổ data cho báo cáo dòng 11 trở đi

                    var worksheetFirstLast = SetColumnItems[0] + index + ":" + SetColumnItems[SetColumnItems.Length - 1] + index;

                    for (int i = 0; i < thuNgans.Count(); i++)
                    {
                        manager.CurrentObject = baoCaoThuPhiVienPhiGridVos.Where(cc => cc.Nhom == thuNgans[i]).FirstOrDefault();
                        manager.WriteToXlsx(worksheet, index);

                        for (int ii = 0; ii < SetColumnItems.Length; ii++)
                        {
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Bold = true;
                        }

                        worksheet.Row(index).Height = 20.5;

                        //merge dòng group
                        var group = "A" + index + ":" + "AF" + index;
                        worksheet.Cells[group].Merge = true;

                        var indexMain = index;

                        index++;
                        int Stt = 1;

                        var dataByThuNgans = dataBaoCaos.Where(cc => cc.ThuNgan == thuNgans[i]).SelectMany(cc => cc.Data);

                        foreach (var baoCao in dataByThuNgans)
                        {
                            //worksheet.Cells["A" + index + ":AF" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["A" + index + ":AF" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));

                            worksheet.Cells["A" + index].Value = Stt;
                            worksheet.Cells["B" + index].Value = baoCao.MaNB;
                            worksheet.Cells["C" + index].Value = baoCao.MaTN;
                            worksheet.Cells["D" + index].Value = baoCao.HoVaTen;
                            worksheet.Cells["E" + index].Value = baoCao.NamSinh;
                            worksheet.Cells["F" + index].Value = baoCao.GioiTinh;
                            worksheet.Cells["G" + index].Value = baoCao.SoBenhAn;
                            worksheet.Cells["H" + index].Value = baoCao.NoiDung;
                            worksheet.Cells["I" + index].Value = baoCao.NgayThuStr;
                            worksheet.Cells["J" + index].Value = baoCao.NguoiGioiThieu;

                            //worksheet.Cells["K" + index + ":AF" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            //worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["K" + index].Value = baoCao.GiaNhapKho;

                            //worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["L" + index].Value = baoCao.GiaNiemYet;

                            //worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["M" + index].Value = baoCao.SoTienMienGiam;

                            //worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["N" + index].Value = baoCao.BHYTChiTra;

                            //worksheet.Cells["O" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["O" + index].Value = baoCao.HeSo;

                            //worksheet.Cells["P" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["P" + index].Value = baoCao.KhamBenh;

                            //worksheet.Cells["Q" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["Q" + index].Value = baoCao.XetNghiem;

                            //worksheet.Cells["R" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["R" + index].Value = baoCao.NoiSoi;

                            //worksheet.Cells["S" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["S" + index].Value = baoCao.NoiSoiTMH;

                            //worksheet.Cells["T" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["T" + index].Value = baoCao.SieuAm;

                            //worksheet.Cells["U" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["U" + index].Value = baoCao.XQuang;

                            //worksheet.Cells["V" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["V" + index].Value = baoCao.CTScan;

                            //worksheet.Cells["W" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["W" + index].Value = baoCao.MRI;

                            //worksheet.Cells["X" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["X" + index].Value = baoCao.DienTimDienNao;

                            //worksheet.Cells["Y" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["Y" + index].Value = baoCao.TDCNDoLoangXuong;

                            //worksheet.Cells["Z" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["Z" + index].Value = baoCao.ThuThuat;

                            //worksheet.Cells["AA" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["AA" + index].Value = baoCao.PhauThuat;

                            //worksheet.Cells["AB" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["AB" + index].Value = baoCao.NgayGiuong;

                            //worksheet.Cells["AC" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["AC" + index].Value = baoCao.DVKhac;

                            //worksheet.Cells["AD" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["AD" + index].Value = baoCao.Thuoc;

                            //worksheet.Cells["AE" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["AE" + index].Value = baoCao.VTYT;

                            //worksheet.Cells["AF" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["AF" + index].Value = baoCao.TongCong;

                            Stt++;
                            index++;
                        }
                        worksheet.Cells["A" + (indexMain + 1).ToString() + ":AF" + (index - 1).ToString()].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        worksheet.Cells["K" + (indexMain + 1).ToString() + ":AF" + (index - 1).ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells["K" + (indexMain + 1).ToString() + ":AF" + (index - 1).ToString()].Style.Numberformat.Format = "#,##0.00";
                        //for (int ii = 0; ii < SetColumnItems.Length; ii++)
                        //{
                        //    worksheet.Cells[$"{SetColumnItems[ii]}{indexMain + 1}:{SetColumnItems[ii]}{index-1}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        //}

                        index++;
                    }

                    worksheet.Cells[worksheetFirstLast].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells[worksheetFirstLast].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells[worksheetFirstLast].Style.Font.Bold = true;


                    worksheet.Cells[worksheetFirstLast].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                    // total grid
                    worksheet.Cells["P" + 10 + ":AF" + 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells["P" + 10 + ":AF" + 10].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    for (int ii = 15; ii < SetColumnItems.Length; ii++)
                    {
                        worksheet.Cells[$"{SetColumnItems[ii]}10"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }
                                        
                    worksheet.Cells["P" + 10].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["Q" + 10].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["R" + 10].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["S" + 10].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["T" + 10].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["U" + 10].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["V" + 10].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["W" + 10].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["X" + 10].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["Y" + 10].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["Z" + 10].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["AA" + 10].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["AB" + 10].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["AC" + 10].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["AD" + 10].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["AE" + 10].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["AF" + 10].Style.Numberformat.Format = "#,##0.00";

                    worksheet.Cells["P" + 10].Value = baoCaoThuPhiVienPhiGridVos.Sum(c => c.KhamBenh);
                    worksheet.Cells["Q" + 10].Value = baoCaoThuPhiVienPhiGridVos.Sum(c => c.XetNghiem);
                    worksheet.Cells["R" + 10].Value = baoCaoThuPhiVienPhiGridVos.Sum(c => c.NoiSoi);
                    worksheet.Cells["S" + 10].Value = baoCaoThuPhiVienPhiGridVos.Sum(c => c.NoiSoiTMH);
                    worksheet.Cells["T" + 10].Value = baoCaoThuPhiVienPhiGridVos.Sum(c => c.SieuAm);
                    worksheet.Cells["U" + 10].Value = baoCaoThuPhiVienPhiGridVos.Sum(c => c.XQuang);
                    worksheet.Cells["V" + 10].Value = baoCaoThuPhiVienPhiGridVos.Sum(c => c.CTScan);
                    worksheet.Cells["W" + 10].Value = baoCaoThuPhiVienPhiGridVos.Sum(c => c.MRI);
                    worksheet.Cells["X" + 10].Value = baoCaoThuPhiVienPhiGridVos.Sum(c => c.DienTimDienNao);
                    worksheet.Cells["Y" + 10].Value = baoCaoThuPhiVienPhiGridVos.Sum(c => c.TDCNDoLoangXuong);
                    worksheet.Cells["Z" + 10].Value = baoCaoThuPhiVienPhiGridVos.Sum(c => c.ThuThuat);
                    worksheet.Cells["AA" + 10].Value = baoCaoThuPhiVienPhiGridVos.Sum(c => c.PhauThuat);
                    worksheet.Cells["AB" + 10].Value = baoCaoThuPhiVienPhiGridVos.Sum(c => c.NgayGiuong);
                    worksheet.Cells["AC" + 10].Value = baoCaoThuPhiVienPhiGridVos.Sum(c => c.DVKhac);
                    worksheet.Cells["AD" + 10].Value = baoCaoThuPhiVienPhiGridVos.Sum(c => c.Thuoc);
                    worksheet.Cells["AE" + 10].Value = baoCaoThuPhiVienPhiGridVos.Sum(c => c.VTYT);
                    worksheet.Cells["AF" + 10].Value = baoCaoThuPhiVienPhiGridVos.Sum(c => c.TongCong);

                    xlPackage.Save();
                }

                return stream.ToArray();
            }
        }

        #endregion
    }
}
