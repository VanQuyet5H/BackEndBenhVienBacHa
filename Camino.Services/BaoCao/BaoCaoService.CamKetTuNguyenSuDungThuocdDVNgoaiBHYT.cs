using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.Grid;
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
        private async Task<List<BaoCaoCamKetTuNguyenSuDungThuocDVNgoaiBHYTGridVo>> GetAllDataForCamKetTuNguyenSuDungThuocDVNgoaiBHYT(BaoCaoCamKetTuNguyenSuDungThuocDVNgoaiBHYTQueryInfo queryInfo)
        {
            var item1 = new BaoCaoCamKetTuNguyenSuDungThuocDVNgoaiBHYTGridVo()
            {
                Id = 1,
                TenVTDV = "Gây mê và phụ mổ BN Bắc Hà",
                SoLuong = 1,
                DonGia = 2000000,
                LoaiVTDVId = 1,
                TenLoaiVTDV = "THỦ THUẬT NGOẠI KHOA"
            };
            var item2 = new BaoCaoCamKetTuNguyenSuDungThuocDVNgoaiBHYTGridVo()
            {
                Id = 2,
                TenVTDV = "Gây mê",
                SoLuong = 1,
                DonGia = 500000,
                LoaiVTDVId = 1,
                TenLoaiVTDV = "THỦ THUẬT NGOẠI KHOA"
            };
            var item3 = new BaoCaoCamKetTuNguyenSuDungThuocDVNgoaiBHYTGridVo()
            {
                Id = 3,
                TenVTDV = "Alphachoay 21 microkatals ",
                SoLuong = 2,
                DonGia = 100000,
                LoaiVTDVId = 2,
                TenLoaiVTDV = "THUỐC, DỊCH TRUYỀN"
            };
            var item4 = new BaoCaoCamKetTuNguyenSuDungThuocDVNgoaiBHYTGridVo()
            {
                Id = 4,
                TenVTDV = "Bơm tiêm 20ml",
                SoLuong = 50,
                DonGia = 3000,
                LoaiVTDVId = 3,
                TenLoaiVTDV = "VẬT TƯ Y TẾ"
            };
            var item5 = new BaoCaoCamKetTuNguyenSuDungThuocDVNgoaiBHYTGridVo()
            {
                Id = 5,
                TenVTDV = "Băng gạc",
                SoLuong = 150,
                DonGia = 2000,
                LoaiVTDVId = 3,
                TenLoaiVTDV = "VẬT TƯ Y TẾ"
            };
            var data = new List<BaoCaoCamKetTuNguyenSuDungThuocDVNgoaiBHYTGridVo>();
            data.Add(item1);
            data.Add(item2);
            data.Add(item3);
            data.Add(item4);
            data.Add(item5);

            return data;
        }
        public async Task<GridDataSource> GetDataBaoCaoCamKetTuNguyenSuDungThuocDVNgoaiBHYTForGridAsync(BaoCaoCamKetTuNguyenSuDungThuocDVNgoaiBHYTQueryInfo queryInfo)
        {
            var allData = await GetAllDataForCamKetTuNguyenSuDungThuocDVNgoaiBHYT(queryInfo);
            return new GridDataSource { Data = allData.ToArray(), TotalRowCount = allData.Count() };
        }

        public async Task<GridDataSource> GetDataTotalPageBaoCaoCamKetTuNguyenSuDungThuocDVNgoaiBHYTForGridAsync(BaoCaoCamKetTuNguyenSuDungThuocDVNgoaiBHYTQueryInfo queryInfo)
        {
            var allData = await GetAllDataForCamKetTuNguyenSuDungThuocDVNgoaiBHYT(queryInfo);
            return new GridDataSource { TotalRowCount = allData.Count() };
        }

        public virtual byte[] ExportBaoCaoCamKetTuNguyenSuDungThuocDVNgoaiBHYTGridVo(GridDataSource gridDataSource, BaoCaoCamKetTuNguyenSuDungThuocDVNgoaiBHYTQueryInfo query)
        {
            var datas = (ICollection<BaoCaoCamKetTuNguyenSuDungThuocDVNgoaiBHYTGridVo>)gridDataSource.Data;
            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoCamKetTuNguyenSuDungThuocDVNgoaiBHYTGridVo>("STT", p => ind++)
            };
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO CAM KẾT TỰ NGUYỆN SỬ DỤNG THUỐC DV NGOÀI BHYT");

                    // set row
                    worksheet.DefaultRowHeight = 16;

                    worksheet.Column(1).Width = 10;
                    worksheet.Column(2).Width = 45;
                    worksheet.Column(3).Width = 15;
                    worksheet.Column(4).Width = 15;
                    worksheet.Column(5).Width = 15;

                    //SET title BV
                    using (var range = worksheet.Cells["A1:B1"])
                    {
                        range.Worksheet.Cells["A1:B1"].Merge = true;
                        range.Worksheet.Cells["A1:B1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:B1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:B1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:B1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:B1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:B1"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["C1:F1"])
                    {
                        range.Worksheet.Cells["C1:F1"].Merge = true;
                        range.Worksheet.Cells["C1:F1"].Value = "Cộng hòa xã hội chủ nghĩa Việt Nam";
                        range.Worksheet.Cells["C1:F1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["C1:F1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["C1:F1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["C1:F1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["C1:F1"].Style.Font.Bold = true;
                    }
                    using (var range = worksheet.Cells["C2:F2"])
                    {
                        range.Worksheet.Cells["C2:F2"].Merge = true;
                        range.Worksheet.Cells["C2:F2"].Value = "Độc lập - Tự do - Hạnh phúc";
                        range.Worksheet.Cells["C2:F2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["C2:F2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["C2:F2"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["C2:F2"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["C2:F2"].Style.Font.Bold = true;
                    }

                    // SET title head cho bảng excel
                    using (var range = worksheet.Cells["A3:F3"])
                    {
                        range.Worksheet.Cells["A3:F3"].Merge = true;
                        range.Worksheet.Cells["A3:F3"].Value = "GIẤY CAM KẾT TỰ NGUYỆN SỬ DỤNG THUỐC - DỊCH VỤ NGOÀI BHYT";
                        range.Worksheet.Cells["A3:F3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:F3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:F3"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A3:F3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:F3"].Style.Font.Bold = true;
                    }


                    var space = " .........................................................................";
                    using (var range = worksheet.Cells["A4:C4"])
                    {
                        range.Worksheet.Cells["A4:C4"].Merge = true;
                        range.Worksheet.Cells["A4:C4"].Value = "Tôi tên là: " + space + space;
                        range.Worksheet.Cells["A4:C4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A4:C4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:C4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A4:C4"].Style.Font.Color.SetColor(Color.Black);
                    }
                    using (var range = worksheet.Cells["D4:F4"])
                    {
                        range.Worksheet.Cells["D4:F4"].Merge = true;
                        range.Worksheet.Cells["D4:F4"].Value = "Quan hệ với bệnh nhân: .....................";
                        range.Worksheet.Cells["D4:F4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["D4:F4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["D4:F4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["D4:F4"].Style.Font.Color.SetColor(Color.Black);
                    }

                    var benhNhan = new
                    {
                        HoTenBN = "Trần Thu Thảo",
                        GioiTinh = "Nữ",
                        NamSinh = 1999,
                        DiaChi = "Q. Hóc Môn. TP.HCM",
                        BHYT = "DN1234567890",
                        HanSuDungStr = "Từ 01-01-2021 đến 31-12-2021"
                    };
                    using (var range = worksheet.Cells["A5:B5"])
                    {
                        range.Worksheet.Cells["A5:B5"].Merge = true;
                        range.Worksheet.Cells["A5:B5"].Value = "Họ tên người bệnh: " + benhNhan.HoTenBN;
                        range.Worksheet.Cells["A5:B5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A5:B5"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A5:B5"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A5:B5"].Style.Font.Color.SetColor(Color.Black);
                    }

                    worksheet.Cells["C5"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["C5"].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["C5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells["C5"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    worksheet.Cells["C5"].Value = "Giới tính: " + benhNhan.GioiTinh;

                    using (var range = worksheet.Cells["D5:F5"])
                    {
                        range.Worksheet.Cells["D5:F5"].Merge = true;
                        range.Worksheet.Cells["D5:F5"].Value = "Địa chỉ: " + benhNhan.DiaChi;
                        range.Worksheet.Cells["D5:F5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["D5:F5"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["D5:F5"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["D5:F5"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["A6:B6"])
                    {
                        range.Worksheet.Cells["A6:B6"].Merge = true;
                        range.Worksheet.Cells["A6:B6"].Value = "Số thẻ BHYT: " + benhNhan.BHYT;
                        range.Worksheet.Cells["A6:B6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A6:B6"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A6:B6"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A6:B6"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["C6:F6"])
                    {
                        range.Worksheet.Cells["C6:F6"].Merge = true;
                        range.Worksheet.Cells["C6:F6"].Value = "Hạn sử dụng: " + benhNhan.HanSuDungStr;
                        range.Worksheet.Cells["C6:F6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["C6:F6"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["C6:F6"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["C6:F6"].Style.Font.Color.SetColor(Color.Black);
                    }


                    using (var range = worksheet.Cells["A7:F7"])
                    {
                        range.Worksheet.Cells["A7:F7"].Merge = true;
                        range.Worksheet.Cells["A7:F7"].Value = "Tôi xin tự nguyện trả tiền để sử dụng dịch vụ y tế ngoài quy định của BHYT, cụ thể :";
                        range.Worksheet.Cells["A7:F7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A7:F7"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A7:F7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A7:F7"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["A8:E8"])
                    {
                        range.Worksheet.Cells["A8:E8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A8:E8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A8:E8"].Style.Font.SetFromFont(new Font("Times New Roman", 9));
                        range.Worksheet.Cells["A8:E8"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A8:E8"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A8:E8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A8"].Value = "STT";

                        range.Worksheet.Cells["B8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black); //del ?
                        range.Worksheet.Cells["B8"].Value = "TÊN DỊCH VỤ KỸ THUẬT , THUỐC , VẬT TƯ Y TẾ";

                        range.Worksheet.Cells["C8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C8"].Value = "SỐ LƯỢNG";

                        range.Worksheet.Cells["D8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D8"].Value = "ĐƠN GIÁ";

                        range.Worksheet.Cells["E8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E8"].Value = "TỔNG TIỀN";
                    }

                    //write data from line 9
                    int index = 9;
                    var dataTheoLoaiVTDV = datas.GroupBy(x => x.LoaiVTDVId).Select(x => x.Key);
                    if (datas.Any())
                    {
                        foreach (var data in dataTheoLoaiVTDV)
                        {
                            var listDataTheoLoaiVTDV = datas.Where(x => x.LoaiVTDVId == data.Value).ToList();
                            if (listDataTheoLoaiVTDV.Any())
                            {
                                worksheet.Cells["A" + index + ":E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index + ":E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["A" + index + ":E" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                worksheet.Cells["A" + index + ":E" + index].Merge = true;
                                worksheet.Cells["A" + index + ":E" + index].Value = listDataTheoLoaiVTDV.FirstOrDefault().TenLoaiVTDV;
                                worksheet.Cells["A" + index + ":E" + index].Style.Font.Bold = true;
                                index++;
                                int stt = 1;
                                foreach (var item in listDataTheoLoaiVTDV)
                                {
                                    // format border, font chữ,....
                                    worksheet.Cells["A" + index + ":E" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                    worksheet.Cells["A" + index + ":E" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                    worksheet.Cells["A" + index + ":E" + index].Style.Font.Color.SetColor(Color.Black);
                                    worksheet.Cells["A" + index + ":E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["A" + index + ":E" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                                    worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Row(index).Height = 20.5;

                                    worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["A" + index].Value = stt;

                                    worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["B" + index].Value = item.TenVTDV;

                                    worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["C" + index].Value = item.SoLuong;

                                    worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["D" + index].Value = item.DonGia;
                                    worksheet.Cells["D" + index].Style.Numberformat.Format = "#,##0.00";

                                    worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["E" + index].Value = item.TongTien;
                                    worksheet.Cells["E" + index].Style.Numberformat.Format = "#,##0.00";
                                    stt++;
                                    index++;                                   
                                }
                                //total
                                worksheet.Cells["A" + index + ":E" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                worksheet.Cells["A" + index + ":E" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                worksheet.Cells["A" + index + ":E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index + ":E" + index].Style.Font.Color.SetColor(Color.Black);
                                worksheet.Cells["A" + index + ":E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["A" + index + ":E" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                                using (var range = worksheet.Cells["A" + index + ":E" + index])
                                {
                                    range.Worksheet.Cells["A" + index + ":E" + index].Merge = true;
                                    range.Worksheet.Cells["A" + index + ":E" + index].Value = listDataTheoLoaiVTDV.Sum(x => x.TongTien);
                                    range.Worksheet.Cells["A" + index + ":E" + index].Style.Font.Bold = true;
                                    range.Worksheet.Cells["A" + index + ":E" + index].Style.Numberformat.Format = "#,##0.00";
                                }
                                index++;

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