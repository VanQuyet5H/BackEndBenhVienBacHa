using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Services.ExportImport.Help;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Newtonsoft.Json;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public async Task<GridDataSource> GetDataBaoCaoDoanhThuKhamDoanTheoNhomDVForGridAsync(QueryInfo queryInfo, bool laTinhTong = false)
        {
            //BaoCaoDoanhThuKhamDoanTheoNhomDVGridVo
            var thongTinKhams = new List<BaoCaoDoanhThuKhamDoanTheoKhoaPhongGridVo>();
            var timKiemNangCaoObj = new BaoCaoDoanhThuKhamDoanTheoKhoaPhongQueryInfo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoDoanhThuKhamDoanTheoKhoaPhongQueryInfo>(queryInfo.AdditionalSearchString);
            }

            if (laTinhTong)
            {
                timKiemNangCaoObj.Skip = 0;
                timKiemNangCaoObj.Take = Int32.MaxValue;
            }
            else
            {
                timKiemNangCaoObj.Skip = queryInfo.Skip;
                timKiemNangCaoObj.Take = queryInfo.Take;
            }
            thongTinKhams = await GetDataDoanhThuKhamDoanAsync(timKiemNangCaoObj);

            return new GridDataSource
            {
                Data = thongTinKhams.ToArray(),
                TotalRowCount = thongTinKhams.Count()
            };
        }

        public async Task<GridDataSource> GetDataTotalPageBaoCaoDoanhThuKhamDoanTheoNhomDVForGridAsync(QueryInfo queryInfo)
        {
            //BaoCaoDoanhThuKhamDoanTheoNhomDVGridVo
            var timKiemNangCaoObj = new BaoCaoDoanhThuKhamDoanTheoKhoaPhongQueryInfo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoDoanhThuKhamDoanTheoKhoaPhongQueryInfo>(queryInfo.AdditionalSearchString);
            }


            var thongTinKhams = await GetDataDoanhThuKhamDoanAsync(timKiemNangCaoObj, true);

            return new GridDataSource { TotalRowCount = thongTinKhams.Count };
        }

        public virtual byte[] ExportBaoCaoDoanhThuKhamDoanTheoNhomDVGridVo(GridDataSource gridDataSource, QueryInfo query)
        {

            var timKiemNangCaoObj = new BaoCaoDoanhThuKhamDoanTheoKhoaPhongQueryInfo();
            if (!string.IsNullOrEmpty(query.AdditionalSearchString) && query.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoDoanhThuKhamDoanTheoKhoaPhongQueryInfo>(query.AdditionalSearchString);
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.FromDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.FromDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.FromDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (timKiemNangCaoObj.ToDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.ToDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.ToDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }
            var datas = (ICollection<BaoCaoDoanhThuKhamDoanTheoKhoaPhongGridVo>)gridDataSource.Data;
            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoDoanhThuKhamDoanTheoKhoaPhongGridVo>("STT", p => ind++)
            };
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO DOANH THU KHÁM ĐOÀN THEO NHÓM DỊCH VỤ  ");

                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 25;
                    worksheet.Column(3).Width = 15;
                    worksheet.Column(4).Width = 30;
                    worksheet.Column(5).Width = 10;
                    worksheet.Column(6).Width = 10;
                    worksheet.Column(7).Width = 15;
                    worksheet.Column(8).Width = 15;
                    worksheet.Column(9).Width = 15;
                    worksheet.Column(10).Width = 15;
                    worksheet.Column(11).Width = 15;
                    worksheet.Column(12).Width = 15;
                    worksheet.Column(13).Width = 15;
                    worksheet.Column(14).Width = 15;
                    worksheet.Column(15).Width = 15;
                    worksheet.Column(16).Width = 20;
                    worksheet.Column(17).Width = 15;
                    worksheet.Column(18).Width = 15;

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
                    using (var range = worksheet.Cells["A3:R3"])
                    {
                        range.Worksheet.Cells["A3:R3"].Merge = true;
                        range.Worksheet.Cells["A3:R3"].Value = "BÁO CÁO DOANH THU KHÁM ĐOÀN THEO NHÓM DỊCH VỤ";
                        range.Worksheet.Cells["A3:R3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:R3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:R3"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A3:R3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:R3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A4:R4"])
                    {
                        range.Worksheet.Cells["A4:R4"].Merge = true;
                        range.Worksheet.Cells["A4:R4"].Value = "Từ ngày: " + tuNgay?.FormatNgayGioTimKiemTrenBaoCao()
                                                      + " - đến ngày: " + denNgay?.FormatNgayGioTimKiemTrenBaoCao();
                        range.Worksheet.Cells["A4:R4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:R4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:R4"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A4:R4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:R4"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A6:R6"])
                    {
                        range.Worksheet.Cells["A6:R6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A6:R6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A6:R6"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A6:R6"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A6:R6"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A6:R6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["A6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A6"].Value = "STT";

                        range.Worksheet.Cells["B6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B6"].Value = "Tên Công Ty";

                        range.Worksheet.Cells["C6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C6"].Value = "Mã TN";

                        range.Worksheet.Cells["D6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D6"].Value = "Họ Và Tên";

                        range.Worksheet.Cells["E6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E6"].Value = "Năm Sinh";

                        range.Worksheet.Cells["F6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F6"].Value = "Giới Tính";

                        range.Worksheet.Cells["G6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G6"].Value = "Khám Bệnh";

                        range.Worksheet.Cells["H6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H6"].Value = "Xét Nghiệm";

                        range.Worksheet.Cells["I6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I6"].Value = "Nội Soi";

                        range.Worksheet.Cells["J6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J6"].Value = "Nội Soi TMH";

                        range.Worksheet.Cells["K6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K6"].Value = "Siêu Âm";

                        range.Worksheet.Cells["L6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["L6"].Value = "X-Quang";

                        range.Worksheet.Cells["M6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["M6"].Value = "CT Scan";

                        range.Worksheet.Cells["N6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["N6"].Value = "MRI";

                        range.Worksheet.Cells["O6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["O6"].Value = "Điện Tim + Điện Não";

                        range.Worksheet.Cells["P6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["P6"].Value = "Đo Loãng Xương, Đo Hô Hấp";

                        range.Worksheet.Cells["Q6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["Q6"].Value = "DV Khác";

                        range.Worksheet.Cells["R6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["R6"].Value = "Tổng Cộng";
                    }



                    //write data from line 7
                    int index = 7;
                    var dataTheoCT = datas.GroupBy(x => x.CongTyId).Select(x => x.Key);
                    if (datas.Any())
                    {
                        foreach (var data in dataTheoCT)
                        {
                            var listDataTheoCT = datas.Where(x => x.CongTyId == data).ToList();
                            if (listDataTheoCT.Any())
                            {
                                //loai HC
                                worksheet.Cells["B" + index + ":F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["B" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["B" + index + ":F" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                worksheet.Cells["B" + index + ":F" + index].Merge = true;
                                worksheet.Cells["B" + index + ":F" + index].Style.Font.Bold = true;
                                worksheet.Cells["B" + index + ":F" + index].Value = listDataTheoCT.FirstOrDefault().TenCongTy;

                                worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                index++;

                                int stt = 1;
                                foreach (var item in listDataTheoCT)
                                {
                                    // format border, font chữ,....
                                    worksheet.Cells["A" + index + ":R" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                    worksheet.Cells["A" + index + ":R" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                    worksheet.Cells["A" + index + ":R" + index].Style.Font.Color.SetColor(Color.Black);
                                    worksheet.Cells["A" + index + ":R" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["A" + index + ":R" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

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
                                    worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Row(index).Height = 20.5;

                                    worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["A" + index].Value = stt;

                                    worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["B" + index].Value = "";

                                    worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["C" + index].Value = item.MaTN;

                                    worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["D" + index].Value = item.HoTen;

                                    worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["E" + index].Value = item.NamSinh;

                                    worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["F" + index].Value = item.GioiTinh;

                                    worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["G" + index].Value = item.KhamBenh;
                                    worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";

                                    worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["H" + index].Value = item.XetNghiem;
                                    worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";

                                    worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["I" + index].Value = item.NoiSoi;
                                    worksheet.Cells["I" + index].Style.Numberformat.Format = "#,##0.00";

                                    worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["J" + index].Value = item.NoiSoiTMH;
                                    worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";

                                    worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["K" + index].Value = item.SieuAm;
                                    worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";

                                    worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["L" + index].Value = item.XQuang;
                                    worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";

                                    worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["M" + index].Value = item.CTScan;
                                    worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";

                                    worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["N" + index].Value = item.MRI;
                                    worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";

                                    worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["O" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["O" + index].Value = item.DienTimDienNao;
                                    worksheet.Cells["O" + index].Style.Numberformat.Format = "#,##0.00";

                                    worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["P" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["P" + index].Value = item.TDCNDoLoangXuong;
                                    worksheet.Cells["P" + index].Style.Numberformat.Format = "#,##0.00";

                                    worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["Q" + index].Value = item.DVKhac;
                                    worksheet.Cells["Q" + index].Style.Numberformat.Format = "#,##0.00";

                                    worksheet.Cells["R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["R" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["R" + index].Value = item.TongDoanhThuTheoNhomDichVu;
                                    worksheet.Cells["R" + index].Style.Numberformat.Format = "#,##0.00";
                                    stt++;
                                    index++;
                                }
                            }
                        }

                        //total
                        worksheet.Cells["A" + index + ":R" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        worksheet.Cells["A" + index + ":R" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        worksheet.Cells["A" + index + ":R" + index].Style.Font.Color.SetColor(Color.Black);
                        worksheet.Cells["A" + index + ":R" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["A" + index + ":R" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

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
                        worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Row(index).Height = 20.5;


                        worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells["D" + index].Style.Font.Bold = true;
                        worksheet.Cells["D" + index].Value = "Tổng cộng";

                        worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["G" + index].Style.Font.Bold = true;
                        worksheet.Cells["G" + index].Value = datas.Sum(x => x.KhamBenh);
                        worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";

                        worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["H" + index].Style.Font.Bold = true;
                        worksheet.Cells["H" + index].Value = datas.Sum(x => x.XetNghiem);
                        worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";

                        worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["I" + index].Style.Font.Bold = true;
                        worksheet.Cells["I" + index].Value = datas.Sum(x => x.NoiSoi);
                        worksheet.Cells["I" + index].Style.Numberformat.Format = "#,##0.00";

                        worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["J" + index].Style.Font.Bold = true;
                        worksheet.Cells["J" + index].Value = datas.Sum(x => x.NoiSoiTMH);
                        worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";

                        worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["K" + index].Style.Font.Bold = true;
                        worksheet.Cells["K" + index].Value = datas.Sum(x => x.SieuAm);
                        worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";

                        worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["L" + index].Style.Font.Bold = true;
                        worksheet.Cells["L" + index].Value = datas.Sum(x => x.XQuang);
                        worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";

                        worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["M" + index].Style.Font.Bold = true;
                        worksheet.Cells["M" + index].Value = datas.Sum(x => x.CTScan);
                        worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";

                        worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["N" + index].Style.Font.Bold = true;
                        worksheet.Cells["N" + index].Value = datas.Sum(x => x.MRI);
                        worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";

                        worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["O" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["O" + index].Style.Font.Bold = true;
                        worksheet.Cells["O" + index].Value = datas.Sum(x => x.DienTimDienNao);
                        worksheet.Cells["O" + index].Style.Numberformat.Format = "#,##0.00";

                        worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["P" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["P" + index].Style.Font.Bold = true;
                        worksheet.Cells["P" + index].Value = datas.Sum(x => x.TDCNDoLoangXuong);
                        worksheet.Cells["P" + index].Style.Numberformat.Format = "#,##0.00";

                        worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["Q" + index].Style.Font.Bold = true;
                        worksheet.Cells["Q" + index].Value = datas.Sum(x => x.DVKhac);
                        worksheet.Cells["Q" + index].Style.Numberformat.Format = "#,##0.00";

                        worksheet.Cells["R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["R" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["R" + index].Style.Font.Bold = true;
                        worksheet.Cells["R" + index].Value = datas.Sum(x => x.TongDoanhThuTheoNhomDichVu);
                        worksheet.Cells["R" + index].Style.Numberformat.Format = "#,##0.00";
                    }
                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }
    }
}
