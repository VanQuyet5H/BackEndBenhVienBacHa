using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public async Task<GridDataSource> GetDataBaoCaoHieuQuaCongViecForGridAsync(QueryInfo queryInfo)
        {
            var data = new List<BaoCaoHieuQuaCongViecGridVo>()
            {
                new BaoCaoHieuQuaCongViecGridVo
                {
                    Id=1,
                    Khoa="Dược",
                    YeuCau=12,
                    DaHoanThanh=7,
                    DangThucHien=5
                },
                new BaoCaoHieuQuaCongViecGridVo
                {
                    Id=2,
                    Khoa="CDHA",
                    YeuCau=38,
                    DaHoanThanh=21,
                    DangThucHien=17
                },
                new BaoCaoHieuQuaCongViecGridVo
                {
                    Id=3,
                    Khoa="BÁC SĨ",
                    YeuCau=66,
                    DaHoanThanh=43,
                    DangThucHien=23
                },
                new BaoCaoHieuQuaCongViecGridVo
                {
                    Id=4,
                    Khoa="Điều Dưỡng",
                    YeuCau=28,
                    DaHoanThanh=10,
                    DangThucHien=18
                },
                new BaoCaoHieuQuaCongViecGridVo
                {
                    Id=5,
                    Khoa="Thu ngân",
                    YeuCau=14,
                    DaHoanThanh=6,
                    DangThucHien=8
                },
                new BaoCaoHieuQuaCongViecGridVo
                {
                    Id=6,
                    Khoa="Khoa Nội",
                    YeuCau=15,
                    DaHoanThanh=5,
                    DangThucHien=10
                },
                new BaoCaoHieuQuaCongViecGridVo
                {
                    Id=7,
                    Khoa="Khoa Sản",
                    YeuCau=32,
                    DaHoanThanh=16,
                    DangThucHien=16
                },
                new BaoCaoHieuQuaCongViecGridVo
                {
                    Id=8,
                    Khoa="Khám Đoàn",
                    YeuCau=28,
                    DaHoanThanh=3,
                    DangThucHien=25
                },
                new BaoCaoHieuQuaCongViecGridVo
                {
                    Id=9,
                    Khoa="Marketing",
                    YeuCau=4,
                    DaHoanThanh=0,
                    DangThucHien=4

                },
                new BaoCaoHieuQuaCongViecGridVo
                {
                    Id=10,
                    Khoa="IT",
                    YeuCau=8,
                    DaHoanThanh=4,
                    DangThucHien=4
                },
                new BaoCaoHieuQuaCongViecGridVo
                {
                    Id=11,
                    Khoa="XN",
                    YeuCau=116,
                    DaHoanThanh=56,
                    DangThucHien=60
                },
                new BaoCaoHieuQuaCongViecGridVo
                {
                    Id=12,
                    Khoa="Nội Trú",
                    YeuCau=39,
                    DaHoanThanh=22,
                    DangThucHien=17
                },
                new BaoCaoHieuQuaCongViecGridVo
                {
                    Id=13,
                    Khoa="Triển khai",
                    YeuCau=57,
                    DaHoanThanh = 23,
                    DangThucHien  = 34
                },
            };
            return new GridDataSource { Data = data.ToArray(), TotalRowCount = data.Count() };
        }

        public virtual byte[] ExportBaoCaoHieuQuaCongViec(GridDataSource gridDataSource, QueryInfo query)
        {
            var datas = (ICollection<BaoCaoHieuQuaCongViecGridVo>)gridDataSource.Data;
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO HIỆU QUẢ CÔNG VIỆC");
                    //set row
                    worksheet.DefaultRowHeight = 16;

                    //set chiều rộng cho từng cột
                    worksheet.Column(1).Width = 15;
                    worksheet.Column(2).Width = 25;
                    worksheet.Column(3).Width = 15;
                    worksheet.Column(4).Width = 25;
                    worksheet.Column(5).Width = 25;
                    worksheet.DefaultColWidth = 7;

                    worksheet.Row(2).Height = 20;

                    using (var range = worksheet.Cells["A2:E2"])
                    {
                        range.Worksheet.Cells["A2:E2"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A2:E2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A2:E2"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A2:E2"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A2:E2"].Merge = true;
                        range.Worksheet.Cells["A2:E2"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A2:E2"].Value = "BÁO CÁO HIỆU QUẢ CÔNG VIỆC";

                    }

                    using (var range = worksheet.Cells["A4:E4"])
                    {
                        range.Worksheet.Cells["A4:E4"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A4:E4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:E4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:E4"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A4:E4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:E4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A4"].Value = "STT";

                        range.Worksheet.Cells["B4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B4"].Value = "KHOA";

                        range.Worksheet.Cells["C4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C4"].Value = "YÊU CẦU";

                        range.Worksheet.Cells["D4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D4"].Value = "ĐÃ HOÀN THÀNH";

                        range.Worksheet.Cells["E4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E4"].Value = "ĐANG THỰC HIỆN";

                    }

                    int index = 5; // bắt đầu đổ data từ dòng 5

                    ///////Đổ data vào bảng excel
                    ///
                    var stt = 1;
                    if (datas.Any())
                    {
                        foreach(var item in datas)
                        {
                            using(var range = worksheet.Cells["A" + index + ":E" + index])
                            {
                                range.Worksheet.Cells["A" + index + ":E" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                range.Worksheet.Cells["A" + index + ":E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                range.Worksheet.Cells["A" + index + ":E" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                range.Worksheet.Cells["A" + index + ":E" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["A" + index + ":E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                range.Worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["A" + index].Value = stt;

                                range.Worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["B" + index].Value = item.Khoa;

                                range.Worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["C" + index].Value = item.YeuCau;

                                range.Worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["D" + index].Value = item.DaHoanThanh;

                                range.Worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["E" + index].Value = item.DangThucHien;

                                index++;
                                stt++;
                            }
                        }

                    }

                    var yeuCauSum= datas.Sum(s => s.YeuCau);
                    var daHoanThanhSum = datas.Sum(s => s.DaHoanThanh);
                    var dangThucHienSum = datas.Sum(s => s.DangThucHien);
                    decimal tiLeDaHoanThanh = (decimal)daHoanThanhSum / (decimal)yeuCauSum;
                    decimal tiLeDangThucHien = (decimal)dangThucHienSum / (decimal)yeuCauSum;

                    worksheet.Cells["A" + index + ":E" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                    worksheet.Cells["A" + index + ":E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A" + index + ":E" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    worksheet.Cells["A" + index + ":E" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["A" + index + ":E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["A" + index + ":E" + index].Style.Font.Bold = true;

                    worksheet.Cells["A" + index + ":B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["A" + index + ":B" + index].Merge = true;
                    worksheet.Cells["A" + index + ":B" + index].Value = "Tổng";

                    worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["C" + index].Value = yeuCauSum;

                    worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["D" + index].Value = daHoanThanhSum;

                    worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["E" + index].Value = dangThucHienSum;
                    index++;

                    worksheet.Cells["A" + index + ":E" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                    worksheet.Cells["A" + index + ":E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A" + index + ":E" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    worksheet.Cells["A" + index + ":E" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["A" + index + ":E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["A" + index + ":E" + index].Style.Font.Bold = true;

                    worksheet.Cells["A" + index + ":B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["A" + index + ":B" + index].Merge = true;
                    worksheet.Cells["A" + index + ":B" + index].Value = "Tỷ lệ";

                    worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["D" + index].Value = String.Format("{0:P2}", tiLeDaHoanThanh);

                    worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["E" + index].Value = String.Format("{0:P2}", tiLeDangThucHien);
                    xlPackage.Save();

                }
                return stream.ToArray();
            }
        }
    }
}
