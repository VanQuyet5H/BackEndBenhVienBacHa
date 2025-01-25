using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public async Task<List<LookupItemVo>> GetKhoVTYTBaoCaoChiTietHoanTraNoiTruLookupAsync(LookupQueryInfo queryInfo)
        {
            var lookup = await _khoRepository.TableNoTracking.Where(s => s.LoaiVatTu == true && s.LoaiKho == EnumLoaiKhoDuocPham.KhoLe)
                                                             .Select(item => new LookupItemVo
                                                             {
                                                                 DisplayName = item.Ten,
                                                                 KeyId = Convert.ToInt32(item.Id),
                                                             })
                                                           .ApplyLike(queryInfo.Query, g => g.DisplayName)
                                                           .Take(queryInfo.Take)
                                                           .ToListAsync();
            return lookup;
        }

        public async Task<GridDataSource> GetDataBaoCaoChiTietHoanTraNoiBoForGridAsync(BaoCaoChiTietHoanTraNoiBoQueryInfo queryInfo)
        {
            var xuatKhoVatTuQuery = _xuatKhoVatTuRepository.TableNoTracking
                .Where(o => queryInfo.KhoIds.Contains(o.KhoXuatId) && o.KhoNhapId == (long)EnumKhoDuocPham.KhoVatTuYTe && o.NgayXuat >= queryInfo.FromDate && o.NgayXuat < queryInfo.ToDate);

            var xuatKhoVatTuData = xuatKhoVatTuQuery.Select(o => new
            {
                KhoXuat = o.KhoVatTuXuat.Ten,
                KhoXuatId = o.KhoXuatId,
                ChiTietVatTus = o.XuatKhoVatTuChiTiets
                .SelectMany(x => x.XuatKhoVatTuChiTietViTris)
                    .Select(y => new BaoCaoChiTietHoanTraNoiBoGridVo
                    {
                        Id = y.Id,
                        VatTuBenhVienId = y.NhapKhoVatTuChiTiet.VatTuBenhVienId,
                        Ten = y.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.Ten,
                        NhaSanXuat = y.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.NhaSanXuat,
                        NuocSanXuat = y.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.NuocSanXuat,
                        DVT = y.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.DonViTinh,
                        SoLuong = y.SoLuongXuat,
                        DonGia = y.NhapKhoVatTuChiTiet.DonGiaNhap
                    }).ToList()
            }).ToList();


            var baoCaoChiTietHoanTraNoiBoGridVos = new List<BaoCaoChiTietHoanTraNoiBoGridVo>();
            foreach (var xuatKhoVatTu in xuatKhoVatTuData)
            {
                var chiTiets = xuatKhoVatTu.ChiTietVatTus.ToList();
                for (int i = 0; i < chiTiets.Count; i++)
                {
                    chiTiets[i].Kho = xuatKhoVatTu.KhoXuat;
                }
                baoCaoChiTietHoanTraNoiBoGridVos.AddRange(chiTiets);
            }

            var dataReturn = baoCaoChiTietHoanTraNoiBoGridVos
                .GroupBy(o => new { o.Kho, o.VatTuBenhVienId, o.DonGia }, o => o,
                (k, v) => new BaoCaoChiTietHoanTraNoiBoGridVo
                {
                    Kho = k.Kho,
                    VatTuBenhVienId = k.VatTuBenhVienId,
                    DonGia = k.DonGia,
                    Ten = v.First().Ten,
                    NhaSanXuat = v.First().NhaSanXuat,
                    NuocSanXuat = v.First().NuocSanXuat,
                    DVT = v.First().DVT,
                    SoLuong = v.Sum(x => x.SoLuong)
                }).ToArray();
            return new GridDataSource { Data = dataReturn, TotalRowCount = dataReturn.Length };
        }

        public virtual byte[] ExportBaoCaoChiTietHoanTraNoiBo(GridDataSource gridDataSource, BaoCaoChiTietHoanTraNoiBoQueryInfo query)
        {
            var datas = (ICollection<BaoCaoChiTietHoanTraNoiBoGridVo>)gridDataSource.Data;
            var listKho = datas.GroupBy(s => s.Kho).Select(s => s.First().Kho).ToList();
            var countKho = listKho.Count();

            var listThuoc = datas.GroupBy(s => s.TenVTYT).Select(s => s.First().TenVTYT).ToList();

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("[VTYT] Báo cáo chi tiết hoàn trả nội bộ");

                    //set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 40;
                    worksheet.Column(3).Width = 7;
                    worksheet.Column(countKho + 4).Width = 30;
                    worksheet.Row(3).Height = 21;
                    worksheet.DefaultColWidth = 7;

                    using (var range = worksheet.Cells["A1:B2"])
                    {
                        range.Worksheet.Cells["A1:B2"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:B2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:B2"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A1:B2"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:B2"].Value = $"BỆNH VIỆN ĐKQT BẮC HÀ{Environment.NewLine}Phòng Vật tư";
                        range.Worksheet.Cells["A1:B2"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A1:B2"].Merge = true;
                        range.Worksheet.Cells["A1:B2"].Style.WrapText = true;
                        //var title = range.Worksheet.Cells["A1:C2"].RichText.Add("BỆNH VIỆN ĐKQT BẮC HÀ");
                        //var value = range.Worksheet.Cells["A1:C2"].RichText.Add("\n Phòng Vật tư");
                    }                   

                    string[] SetColumnItems = XuLyDanhSachKyTu(countKho, 26).ToArray();
                    var worksheetMSLast = "E";
                    var worksheetMSFirst = "C";

                    if (countKho >= 2)
                    {
                        worksheetMSLast = SetColumnItems[countKho];
                        worksheetMSFirst = SetColumnItems[countKho - 2];
                    }

                    using (var range = worksheet.Cells[worksheetMSFirst + "1:" + worksheetMSLast + "2"])
                    {
                        range.Worksheet.Cells[worksheetMSFirst + "1:" + worksheetMSLast + "2"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells[worksheetMSFirst + "1:" + worksheetMSLast + "2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetMSFirst + "1:" + worksheetMSLast + "2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetMSFirst + "1:" + worksheetMSLast + "2"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetMSFirst + "1:" + worksheetMSLast + "2"].Value = $"MS: 16D/BV-01{Environment.NewLine}Số:....................";

                        range.Worksheet.Cells[worksheetMSFirst + "1:" + worksheetMSLast + "2"].Merge = true;
                        range.Worksheet.Cells[worksheetMSFirst + "1:" + worksheetMSLast + "2"].Style.WrapText = true;
                        //var title = range.Worksheet.Cells[worksheetMSFirst + "1:" + worksheetMSLast + "2"].RichText.Add("MS: 16D/BV-01 ");
                        //var value = range.Worksheet.Cells[worksheetMSFirst + "1:" + worksheetMSLast + "2"].RichText.Add("\n Số:........................");
                    }

                    using (var range = worksheet.Cells["A3:" + worksheetMSLast + "3"])
                    {
                        range.Worksheet.Cells["A3:" + worksheetMSLast + "3"].Style.Font.SetFromFont(new Font("Times New Roman", 18));
                        range.Worksheet.Cells["A3:" + worksheetMSLast + "3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:" + worksheetMSLast + "3"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A3:" + worksheetMSLast + "3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:" + worksheetMSLast + "3"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A3:" + worksheetMSLast + "3"].Merge = true;
                        range.Worksheet.Cells["A3:" + worksheetMSLast + "3"].Value = "BÁO CÁO CHI TIẾT HOÀN TRẢ NỘI BỘ VTYT";
                    }


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
                        //range.Worksheet.Cells["A5:" + worksheetMSLast + "5"].Value = "Nguồn VTYT: Viện Phí ";
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
                        range.Worksheet.Cells["B8"].Value = "Tên thuốc, hóa chất, VTYT (Hãng SX, Nước SX)";

                        range.Worksheet.Cells["C8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C8"].Value = "ĐVT";
                    }

                    //listKho
                    //for(int i = 1; i)
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
                        worksheet.Cells[9, column].Style.Font.Bold = true;
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

                    ///Đổ data
                    ///
                    int index = 10;
                    var stt = 1;
                    if (listThuoc.Any())
                    {
                        foreach (var item in listThuoc)
                        {
                            var duoc = datas.FirstOrDefault(s => s.TenVTYT == item);
                            using (var range = worksheet.Cells["A" + index + ":C" + index])
                            {
                                range.Worksheet.Cells["A" + index + ":C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                range.Worksheet.Cells["A" + index + ":C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                range.Worksheet.Cells["A" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                range.Worksheet.Cells["A" + index + ":C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                range.Worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["A" + index].Value = stt;

                                range.Worksheet.Cells["B" + index].Value = duoc.TenVTYT;
                                range.Worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                range.Worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["C" + index].Value = datas.FirstOrDefault(s => s.TenVTYT == item)?.DVT ?? "";

                                range.Worksheet.Cells["A" + (index + 1) + ":C" + (index + 1)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["A" + (index + 1)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["B" + (index + 1)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["C" + (index + 1)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                            }
                            column = 4;
                            for (int n = 0; n < countKho; n++)
                            {
                                var kho = listKho[n];
                                var soLuong = datas.Where(s => s.TenVTYT == item && s.Kho == kho).Select(o => o.SoLuong).DefaultIfEmpty(null).Sum();
                                var thanhTien = datas.Where(s => s.TenVTYT == item && s.Kho == kho).Select(o => o.ThanhTien).DefaultIfEmpty(null).Sum();
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
                            worksheet.Cells[index, countKho + 4].Value = datas.Where(s => s.TenVTYT == item).Sum(s => s.SoLuong);
                            //worksheet.Cells[index, countKho + 4].Style.Font.Bold = true;

                            worksheet.Cells[index + 1, countKho + 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[index + 1, countKho + 4].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                            worksheet.Cells[index + 1, countKho + 4].Style.Font.SetFromFont(new Font("Times New Roman", 8));
                            worksheet.Cells[index + 1, countKho + 4].Style.Font.Color.SetColor(Color.Black);
                            worksheet.Cells[index + 1, countKho + 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells[index + 1, countKho + 4].Value = datas.Where(s => s.TenVTYT == item).Sum(s => s.ThanhTien);
                            //worksheet.Cells[index + 1, countKho + 4].Style.Font.Bold = true;
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

        private List<string> XuLyDanhSachKyTu(int soKhoHoanTra, int tongSoKyTu)
        {
            var danhSachKyTuSauKhiXuKy = new List<string>();

            var danhSachKyTuPre = KyTus();
            var danhSachKyTuResult = KyTus();

            var khoBatDauTuKyTus = KhoBatDauTuKyTus();

            if (soKhoHoanTra > khoBatDauTuKyTus.Count)
            {
                soKhoHoanTra = soKhoHoanTra - khoBatDauTuKyTus.Count;
                var soLanLap = (soKhoHoanTra / tongSoKyTu) + 1;

                for (int i = 0; i < soLanLap; i++)
                {
                    foreach (var result in danhSachKyTuPre.Take(soLanLap))
                    {
                        foreach (var item in danhSachKyTuResult)
                        {
                            danhSachKyTuSauKhiXuKy.Add(result + item);
                        }
                    }
                }
            }           

            khoBatDauTuKyTus.AddRange(danhSachKyTuSauKhiXuKy);
            return khoBatDauTuKyTus;
        }

        private List<string> KyTus()
        {
            var kyTus = new List<string>()
                    { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };// 26 ký tự
            return kyTus;
        }

        private List<string> KhoBatDauTuKyTus()
        {
            var kyTus = new List<string>()
                    {  "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            return kyTus;
        }

        //private List<BaoCaoChiTietHoanTraNoiBoGridVo> datas()
        //{
        //    var lstHoanTras = new List<BaoCaoChiTietHoanTraNoiBoGridVo>();
        //    for (int i = 0; i < 11; i++)
        //    {
        //        lstHoanTras.Add(new BaoCaoChiTietHoanTraNoiBoGridVo()
        //        {
        //            Id = i,
        //            VatTuBenhVienId = i,
        //            DonGia = 1000000 * i,
        //            DVT = "Chai",
        //            Kho = "Kho A-" + i,
        //            SoLuong = 10 * i,
        //            TenVTYT = i / 2 == 0 ? "Vật tư " + 1 : "Vật tư " + 2,
        //        });
        //    }

        //    return lstHoanTras;
        //}
    }
}
