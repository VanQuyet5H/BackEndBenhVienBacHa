using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
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
        public async Task<GridDataSource> GetDataBaoCaoDuocChiTietXuatNoiBoForGridAsync(BaoCaoDuocChiTietXuatNoiBoQueryInfo queryInfo)
        {
            var xuatKhoDuocPhamQuery = _xuatKhoDuocPhamRepository.TableNoTracking
                .Where(o => o.KhoXuatId == queryInfo.KhoId && o.KhoNhapId != null && o.NgayXuat >= queryInfo.FromDate && o.NgayXuat < queryInfo.ToDate);

            var xuatKhoDuocPhamData = xuatKhoDuocPhamQuery.Select(o => new
            {
                KhoNhap = o.KhoDuocPhamNhap.Ten,
                ChiTietDuocPhams = o.XuatKhoDuocPhamChiTiets
                .SelectMany(x => x.XuatKhoDuocPhamChiTietViTris)
                    .Select(y => new BaoCaoDuocChiTietXuatNoiBoGridVo
                    {
                        Id = y.Id,
                        DuocPhamBenhVienId = y.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienId,
                        Ma = y.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.Ma,
                        Ten = y.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.Ten,
                        DVT = y.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                        HamLuong = y.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.HamLuong,
                        SoLuong = y.SoLuongXuat,
                        DonGia = y.NhapKhoDuocPhamChiTiet.DonGiaBan
                    }).ToList()
            }).ToList();

           
            var baoCaoDuocChiTietXuatNoiBoGridVos = new List<BaoCaoDuocChiTietXuatNoiBoGridVo>();
            foreach (var xuatKhoDuocPham in xuatKhoDuocPhamData)
            {
                var chiTiets = xuatKhoDuocPham.ChiTietDuocPhams.ToList();
                for (int i = 0; i < chiTiets.Count; i++)
                {
                    chiTiets[i].Kho = xuatKhoDuocPham.KhoNhap;
                }
                baoCaoDuocChiTietXuatNoiBoGridVos.AddRange(chiTiets);
            }
           
            var dataReturn = baoCaoDuocChiTietXuatNoiBoGridVos
                .GroupBy(o => new {o.Kho, o.DuocPhamBenhVienId, o.DonGia }, o => o,
                (k, v) => new BaoCaoDuocChiTietXuatNoiBoGridVo
                {
                    Kho = k.Kho,
                    DuocPhamBenhVienId = k.DuocPhamBenhVienId,
                    DonGia = k.DonGia,
                    Ma = v.First().Ma,
                    Ten = v.First().Ten,
                    HamLuong = v.First().HamLuong,
                    DVT = v.First().DVT,
                    SoLuong = v.Sum(x => x.SoLuong)
                }).ToArray();       
            return new GridDataSource { Data = dataReturn, TotalRowCount = dataReturn.Length };
        }

        public virtual byte[] ExportBaoCaoDuocChiTietXuatNoiBo(GridDataSource gridDataSource, BaoCaoDuocChiTietXuatNoiBoQueryInfo query)
        {
            var datas = (ICollection<BaoCaoDuocChiTietXuatNoiBoGridVo>)gridDataSource.Data;
            var listKho = datas.GroupBy(s => s.Kho).Select(s => s.First().Kho).ToList();
            var countKho = listKho.Count();
            var listThuoc = datas.GroupBy(s => s.ThongTinThuoc).Select(s => s.First().ThongTinThuoc).ToList();

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO CHI TIẾT XUẤT NỘI BỘ");

                    //set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 40;
                    worksheet.Column(3).Width = 7;
                    worksheet.Row(3).Height = 21;
                    worksheet.DefaultColWidth = 7;

                    using (var range = worksheet.Cells["A1:D1"])
                    {
                        range.Worksheet.Cells["A1:D1"].Merge = true;
                        range.Worksheet.Cells["A1:D1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:D1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:D1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A1:D1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:D1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:D1"].Style.Font.Bold = true;
                    }

                    string[] SetColumnItems = { "D", "E", "F", "G", "H",  "I", "J","K", "L", "M", "N",
                        "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z","AA", "AB", "AC",
                        "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM","AN", "AO",
                        "AP", "AQ", "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY", "AZ"};

                    var worksheetMSLast = "D";
                    var worksheetMSFirst = "C";
                    if (countKho > 3)
                    {
                        worksheetMSLast = SetColumnItems[countKho];
                        worksheetMSFirst = SetColumnItems[countKho - 3];
                    }

                    using (var range = worksheet.Cells["A3:" + worksheetMSLast + "3"])
                    {
                        range.Worksheet.Cells["A3:" + worksheetMSLast + "3"].Style.Font.SetFromFont(new Font("Times New Roman", 18));
                        range.Worksheet.Cells["A3:" + worksheetMSLast + "3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:" + worksheetMSLast + "3"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A3:" + worksheetMSLast + "3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:" + worksheetMSLast + "3"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A3:" + worksheetMSLast + "3"].Merge = true;
                        range.Worksheet.Cells["A3:" + worksheetMSLast + "3"].Value = "BÁO CÁO CHI TIẾT XUẤT NỘI BỘ";
                    }

                    var tenKho = _khoRepository.TableNoTracking.Where(p => p.Id == query.KhoId).Select(p => p.Ten).FirstOrDefault();
                    using (var range = worksheet.Cells["A4:" + worksheetMSLast + "4"])
                    {
                        range.Worksheet.Cells["A4:" + worksheetMSLast + "4"].Merge = true;
                        range.Worksheet.Cells["A4:" + worksheetMSLast + "4"].Value = "Thời gian từ: " + query.FromDate.ApplyFormatDate() + " - " + query.ToDate.ApplyFormatDate();
                        range.Worksheet.Cells["A4:" + worksheetMSLast + "4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:" + worksheetMSLast + "4"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A4:" + worksheetMSLast + "4"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A4:" + worksheetMSLast + "4"].Style.Font.Color.SetColor(Color.Black);

                        range.Worksheet.Cells["A4:" + worksheetMSLast + "4"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A4:" + worksheetMSLast + "4"].Style.Font.Italic = true;
                    }

                    using (var range = worksheet.Cells["A5:" + worksheetMSLast + "5"])
                    {
                        range.Worksheet.Cells["A5:" + worksheetMSLast + "5"].Merge = true;
                        range.Worksheet.Cells["A5:" + worksheetMSLast + "5"].Value = tenKho;
                        range.Worksheet.Cells["A5:" + worksheetMSLast + "5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A5:" + worksheetMSLast + "5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A5:" + worksheetMSLast + "5"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A5:" + worksheetMSLast + "5"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A5:" + worksheetMSLast + "5"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A5:" + worksheetMSLast + "5"].Style.Font.Italic = true;
                    }

                    using (var range = worksheet.Cells["A8:C8"])
                    {
                        range.Worksheet.Cells["A8:C8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A8:C8"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A8:C8"].Style.Font.SetFromFont(new Font("Times New Roman", 8));
                        range.Worksheet.Cells["A8:C8"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A8:C8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A8"].Value = "STT";

                        range.Worksheet.Cells["B8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B8"].Value = "Mã Dược: Tên thuốc, hoá chất, vtyt + Hàm lượng";

                        range.Worksheet.Cells["C8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C8"].Value = "ĐVT";
                    }
               
                    var column = 4;
                    for (int n = 0; n < countKho; n++)
                    {
                        worksheet.Cells[8, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells[8, column].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        worksheet.Cells[8, column].Style.Font.SetFromFont(new Font("Times New Roman", 8));
                        worksheet.Cells[8, column].Style.Font.Color.SetColor(Color.Black);
                        worksheet.Column(column).Width = 15;
                        worksheet.Cells[8, column].Value = listKho[n];
                        worksheet.Cells[8, column].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        column++;
                    }
                    worksheet.Cells[8, countKho + 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[8, countKho + 4].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    worksheet.Cells[8, countKho + 4].Style.Font.SetFromFont(new Font("Times New Roman", 8));
                    worksheet.Cells[8, countKho + 4].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells[8, countKho + 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells[8, countKho + 4].Value = "Tổng cộng";
                    worksheet.Cells[8, countKho + 4].Style.Font.Bold = true;

                    using (var range = worksheet.Cells["A9:C9"])
                    {
                        range.Worksheet.Cells["A9:C9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A9:C9"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A9:C9"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A9:C9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["B9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B9"].Value = "Tổng cộng";
                        range.Worksheet.Cells["B9"].Style.Font.Bold = true;

                        range.Worksheet.Cells["C9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }
                    column = 4;
                    for (int n = 0; n < countKho; n++)
                    {
                        var kho = listKho[n];
                        worksheet.Cells[9, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells[9, column].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        worksheet.Cells[9, column].Style.Font.SetFromFont(new Font("Times New Roman", 8));
                        worksheet.Cells[9, column].Style.Font.Color.SetColor(Color.Black);
                        worksheet.Cells[9, column].Value = datas.Where(s => s.Kho == kho).Sum(s => s.ThanhTien);
                        worksheet.Cells[9, column].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[9, column].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        column++;

                    }
                    worksheet.Cells[9, countKho + 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[9, countKho + 4].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    worksheet.Cells[9, countKho + 4].Style.Font.SetFromFont(new Font("Times New Roman", 8));
                    worksheet.Cells[9, countKho + 4].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells[9, countKho + 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells[9, countKho + 4].Value = datas.Sum(s => s.ThanhTien);
                    worksheet.Cells[9, countKho + 4].Style.Font.Bold = true;
                    worksheet.Cells[9, countKho + 4].Style.Numberformat.Format = "#,##0.00";

                    int index = 10;
                    var stt = 1;
                    if (listThuoc.Any())
                    {
                        foreach (var item in listThuoc)
                        {
                            var duoc = datas.FirstOrDefault(s => s.ThongTinThuoc == item);
                            using (var range = worksheet.Cells["A" + index + ":C" + index])
                            {
                                range.Worksheet.Cells["A" + index + ":C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                range.Worksheet.Cells["A" + index + ":C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                range.Worksheet.Cells["A" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                range.Worksheet.Cells["A" + index + ":C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                range.Worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["A" + index].Value = stt;

                                range.Worksheet.Cells["B" + index].Value = duoc.ThongTinThuoc;
                                range.Worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                range.Worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["C" + index].Value = datas.FirstOrDefault(s => s.ThongTinThuoc == item)?.DVT ?? "";

                                range.Worksheet.Cells["A" + (index + 1) + ":C" + (index + 1)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["A" + (index + 1)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["B" + (index + 1)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["C" + (index + 1)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                            }
                            column = 4;
                            for (int n = 0; n < countKho; n++)
                            {
                                var kho = listKho[n];
                                var soLuong = datas.Where(s => s.ThongTinThuoc == item && s.Kho == kho).Select(o=>o.SoLuong).DefaultIfEmpty(null).Sum();
                                var thanhTien = datas.Where(s => s.ThongTinThuoc == item && s.Kho == kho).Select(o => o.ThanhTien).DefaultIfEmpty(null).Sum();
                                worksheet.Cells[index, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells[index, column].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                worksheet.Cells[index, column].Style.Font.SetFromFont(new Font("Times New Roman", 8));
                                worksheet.Cells[index, column].Style.Font.Color.SetColor(Color.Black);
                                worksheet.Cells[index, column].Value = soLuong != 0 ? soLuong : (double?)null;
                                worksheet.Cells[index, column].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                worksheet.Cells[index + 1, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells[index + 1, column].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                worksheet.Cells[index + 1, column].Style.Font.SetFromFont(new Font("Times New Roman", 8));
                                worksheet.Cells[index + 1, column].Style.Font.Color.SetColor(Color.Black);
                                worksheet.Cells[index + 1, column].Value = thanhTien != 0 ? thanhTien : (decimal?)null;
                                worksheet.Cells[index + 1, column].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells[index + 1, column].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                column++;
                            }
                            worksheet.Cells[index, countKho + 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[index, countKho + 4].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                            worksheet.Cells[index, countKho + 4].Style.Font.SetFromFont(new Font("Times New Roman", 8));
                            worksheet.Cells[index, countKho + 4].Style.Font.Color.SetColor(Color.Black);
                            worksheet.Cells[index, countKho + 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells[index, countKho + 4].Value = datas.Where(s => s.ThongTinThuoc == item).Sum(s => s.SoLuong);
                            worksheet.Cells[index, countKho + 4].Style.Font.Bold = true;

                            worksheet.Cells[index + 1, countKho + 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[index + 1, countKho + 4].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                            worksheet.Cells[index + 1, countKho + 4].Style.Font.SetFromFont(new Font("Times New Roman", 8));
                            worksheet.Cells[index + 1, countKho + 4].Style.Font.Color.SetColor(Color.Black);
                            worksheet.Cells[index + 1, countKho + 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells[index + 1, countKho + 4].Value = datas.Where(s => s.ThongTinThuoc == item).Sum(s => s.ThanhTien);
                            worksheet.Cells[index + 1, countKho + 4].Style.Font.Bold = true;
                            worksheet.Cells[index + 1, countKho + 4].Style.Numberformat.Format = "#,##0.00";
                            stt++;
                            index = index + 2;
                        }
                    }
                    xlPackage.Save();

                }
                return stream.ToArray();

            }
        }
    }
}
