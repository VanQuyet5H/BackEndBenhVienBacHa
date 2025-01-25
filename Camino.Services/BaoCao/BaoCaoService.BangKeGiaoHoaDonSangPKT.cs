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

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        private async Task<List<BaoCaoBangKeGiaoHoaDonSangPKTGridVo>> GetAllDataForCamKetTuNguyenSuDungThuocDVNgoaiBHYT(BaoCaoBangKeGiaoHoaDonSangPKTQueryInfo queryInfo)
        {
            var item1 = new BaoCaoBangKeGiaoHoaDonSangPKTGridVo()
            {
                Id = 1,
                SoHD = "1122",
                NgayHD = DateTime.Now,
                SoTienTT = 30000000,
                NCCId = 1,
                TenNCC = "VIMECIMEX"
            };
            var item2 = new BaoCaoBangKeGiaoHoaDonSangPKTGridVo()
            {
                Id = 2,
                SoHD = "1133",
                NgayHD = DateTime.Now,
                SoTienTT = 30000000,
                NCCId = 1,
                TenNCC = "VIMECIMEX"
            };
            var item3 = new BaoCaoBangKeGiaoHoaDonSangPKTGridVo()
            {
                Id = 3,
                SoHD = "2211",
                NgayHD = DateTime.Now,
                SoTienTT = 30000000,
                NCCId = 1,
                TenNCC = "VIMECIMEX"
            };
            var item4 = new BaoCaoBangKeGiaoHoaDonSangPKTGridVo()
            {
                Id = 4,
                SoHD = "4455",
                NgayHD = DateTime.Now,
                SoTienTT = 30000000,
                NCCId = 1,
                TenNCC = "VIMECIMEX"
            };
            var item5 = new BaoCaoBangKeGiaoHoaDonSangPKTGridVo()
            {
                Id = 5,
                SoHD = "4455",
                NgayHD = DateTime.Now,
                SoTienTT = 30000000,
                NCCId = 2,
                TenNCC = "DKSH"
            };
            var item6 = new BaoCaoBangKeGiaoHoaDonSangPKTGridVo()
            {
                Id = 6,
                SoHD = "4455",
                NgayHD = DateTime.Now,
                SoTienTT = 30000000,
                NCCId = 2,
                TenNCC = "DKSH"
            };
            var item7 = new BaoCaoBangKeGiaoHoaDonSangPKTGridVo()
            {
                Id = 7,
                SoHD = "4455",
                NgayHD = DateTime.Now,
                SoTienTT = 30000000,
                NCCId = 2,
                TenNCC = "DKSH"
            };
            var data = new List<BaoCaoBangKeGiaoHoaDonSangPKTGridVo>();
            data.Add(item1);
            data.Add(item2);
            data.Add(item3);
            data.Add(item4);
            data.Add(item5);
            data.Add(item6);
            data.Add(item7);

            return data;
        }
        public async Task<GridDataSource> GetDataBaoCaoBangKeGiaoHoaDonSangPKTForGridAsync(BaoCaoBangKeGiaoHoaDonSangPKTQueryInfo queryInfo)
        {
            var allData = await GetAllDataForCamKetTuNguyenSuDungThuocDVNgoaiBHYT(queryInfo);
            return new GridDataSource { Data = allData.ToArray(), TotalRowCount = allData.Count() };
        }

        public async Task<GridDataSource> GetDataTotalPageBaoCaoBangKeGiaoHoaDonSangPKTForGridAsync(BaoCaoBangKeGiaoHoaDonSangPKTQueryInfo queryInfo)
        {
            var allData = await GetAllDataForCamKetTuNguyenSuDungThuocDVNgoaiBHYT(queryInfo);
            return new GridDataSource { TotalRowCount = allData.Count() };
        }

        public virtual byte[] ExportBaoCaoBangKeGiaoHoaDonSangPKTGridVo(GridDataSource gridDataSource, BaoCaoBangKeGiaoHoaDonSangPKTQueryInfo query)
        {
            var datas = (ICollection<BaoCaoBangKeGiaoHoaDonSangPKTGridVo>)gridDataSource.Data;
            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoBangKeGiaoHoaDonSangPKTGridVo>("STT", p => ind++)
            };
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BẢNG KÊ GIAO HÓA ĐƠN SANG PHÒNG KẾ TOÁN");

                    // set row
                    worksheet.DefaultRowHeight = 16;

                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 15;
                    worksheet.Column(3).Width = 15;
                    worksheet.Column(4).Width = 25;
                    worksheet.Column(5).Width = 20;
                    worksheet.Column(6).Width = 20;
                    worksheet.Column(7).Width = 35;


                    //SET title BV
                    //using (var range = worksheet.Cells["A1:C1"])
                    //{
                    //    range.Worksheet.Cells["A1:C1"].Merge = true;
                    //    range.Worksheet.Cells["A1:C1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                    //    range.Worksheet.Cells["A1:C1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    //    range.Worksheet.Cells["A1:C1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    //    range.Worksheet.Cells["A1:C1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    //    range.Worksheet.Cells["A1:C1"].Style.Font.Color.SetColor(Color.Black);
                    //    range.Worksheet.Cells["A1:C1"].Style.Font.Bold = true;
                    //}



                    // SET title head cho bảng excel
                    using (var range = worksheet.Cells["A3:G3"])
                    {
                        range.Worksheet.Cells["A3:G3"].Merge = true;
                        range.Worksheet.Cells["A3:G3"].Value = "BẢNG KÊ GIAO HÓA ĐƠN SANG PHÒNG KẾ TOÁN";
                        range.Worksheet.Cells["A3:G3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:G3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:G3"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A3:G3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:G3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A4:G4"])
                    {
                        range.Worksheet.Cells["A4:G4"].Merge = true;
                        range.Worksheet.Cells["A4:G4"].Value = "Từ ngày: " + query.FromDate.FormatNgayGioTimKiemTrenBaoCao()
                                                    + " - đến ngày: " + query.ToDate.FormatNgayGioTimKiemTrenBaoCao();
                        range.Worksheet.Cells["A4:G4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:G4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:G4"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A4:G4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:G4"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A6:G6"])
                    {
                        range.Worksheet.Cells["A6:G6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A6:G6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A6:G6"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A6:G6"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A6:G6"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A6:G6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A6"].Value = "STT";

                        range.Worksheet.Cells["B6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black); //del ?
                        range.Worksheet.Cells["B6"].Value = "Số HĐ";

                        range.Worksheet.Cells["C6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C6"].Value = "NGÀY HĐ";

                        range.Worksheet.Cells["D6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D6"].Value = "TÊN NHÀ CUNG CẤP ";

                        range.Worksheet.Cells["E6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E6"].Value = "SỐ TIỀN THANH TOÁN";

                        range.Worksheet.Cells["F6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F6"].Value = "TỔNG";

                        range.Worksheet.Cells["G6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G6"].Value = "GHI CHÚ";
                    }

                    //write data from line 9

                    int index = 7;
                    int stt = 1;
                    int groupIndex = 7;
                    var dataTheoNCC = datas.GroupBy(x => x.NCCId).Select(x => x.Key);
                    if (datas.Any())
                    {
                        foreach (var data in dataTheoNCC)
                        {
                            var listDataTheoNCC = datas.Where(x => x.NCCId == data.Value).ToList();
                            if (listDataTheoNCC.Any())
                            {
                                foreach (var item in listDataTheoNCC)
                                {
                                    // format border, font chữ,....
                                    worksheet.Cells["A" + index + ":G" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                    worksheet.Cells["A" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                    worksheet.Cells["A" + index + ":G" + index].Style.Font.Color.SetColor(Color.Black);
                                    worksheet.Cells["A" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["A" + index + ":G" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                                    worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Row(index).Height = 20.5;

                                    worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["A" + index].Value = stt;

                                    worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["B" + index].Value = item.SoHD;

                                    worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["C" + index].Value = item.NgayHDStr;

                                    worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["E" + index].Value = item.SoTienTT;
                                    worksheet.Cells["E" + index].Style.Numberformat.Format = "#,##0.00";

                                    worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    stt++;
                                    index++;
                                }

                                using (var range = worksheet.Cells["D" + groupIndex + ":D" + (index - 1)])
                                {
                                    range.Worksheet.Cells["D" + groupIndex + ":D" + (index - 1)].Merge = true;
                                    range.Worksheet.Cells["D" + groupIndex + ":D" + (index - 1)].Value = listDataTheoNCC.FirstOrDefault().TenNCC;
                                    range.Worksheet.Cells["D" + groupIndex + ":D" + (index - 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    range.Worksheet.Cells["D" + groupIndex + ":D" + (index - 1)].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                    range.Worksheet.Cells["D" + groupIndex + ":D" + (index - 1)].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                    range.Worksheet.Cells["D" + groupIndex + ":D" + (index - 1)].Style.Font.Color.SetColor(Color.Black);
                                }

                                using (var range = worksheet.Cells["F" + groupIndex + ":F" + (index - 1)])
                                {
                                    range.Worksheet.Cells["F" + groupIndex + ":F" + (index - 1)].Merge = true;
                                    range.Worksheet.Cells["F" + groupIndex + ":F" + (index - 1)].Value = listDataTheoNCC.Sum(x => x.SoTienTT);
                                    range.Worksheet.Cells["F" + groupIndex + ":F" + (index - 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    range.Worksheet.Cells["F" + groupIndex + ":F" + (index - 1)].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                    range.Worksheet.Cells["F" + groupIndex + ":F" + (index - 1)].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                    range.Worksheet.Cells["F" + groupIndex + ":F" + (index - 1)].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["F" + groupIndex + ":F" + (index - 1)].Style.Numberformat.Format = "#,##0.00";
                                }
                                //worksheet.Cells["D" + groupIndex + ":D" + (index - 1)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                //worksheet.Cells["D" + groupIndex + ":D" + (index - 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                //worksheet.Cells["D" + groupIndex + ":D" + (index - 1)].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                //worksheet.Cells["D" + groupIndex + ":D" + (index - 1)].Value = listDataTheoNCC.FirstOrDefault().TenNCC;

                                //worksheet.Cells["F" + groupIndex + ":F" + (index - 1)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                //worksheet.Cells["F" + groupIndex + ":F" + (index - 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                //worksheet.Cells["F" + groupIndex + ":F" + (index - 1)].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                //worksheet.Cells["F" + groupIndex + ":F" + (index - 1)].Value = listDataTheoNCC.Sum(x => x.SoTienTT);
                                groupIndex = index;
                            }
                        }

                    }

                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }
    }
}
