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
        public async Task<List<LookupItemVo>> GetKhoDuocPhamBaoCaoTinhHinhNhapNCCChiTietLookupAsync(LookupQueryInfo queryInfo)
        {
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var lookup = await _khoRepository.TableNoTracking.Where(s => s.KhoNhanVienQuanLys.Any(o => o.NhanVienId == currentUserId) &&  s.LoaiDuocPham == true && s.LoaiKho != EnumLoaiKhoDuocPham.KhoLe)
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
        public async Task<GridDataSource> GetDataBaoCaoTinhHinhNhapNCCChiTietForGridAsync(BaoCaoTinhHinhNhapNCCChiTietQueryInfo queryInfo)
        {
            var data = new List<BaoCaoTinhHinhNhapNCCChiTietGridVo>()
            {
                new BaoCaoTinhHinhNhapNCCChiTietGridVo
                {
                    Id = 1,
                    MaThuoc = "TD0285238",
                    TenThuoc = "POLTRAPA (Polfarmwx S.A, Ba Lan) - 0001515",
                    HoatChat = "Paracetamol + Tramadol",
                    DVT = "Viên",
                    HangSX = "Polfarmwx S.A",
                    SoLo = "120719",
                    HanDung = new DateTime (2022, 07,02),
                    DonGia = (decimal)6375.00,
                    SoLuong = 200,
                    NhaCungCap = "0104478739-Công ty TNHH Dịch vụ y tế Hưng Thành"
                },
                new BaoCaoTinhHinhNhapNCCChiTietGridVo
                {
                    Id = 2,
                    MaThuoc = "TD984579",
                    TenThuoc = "cordarone 200mg 200mg ( France) - 0033124",
                    HoatChat = "Amiodaron (hydroclorid)",
                    DVT = "Viên",
                    SoLo = "AA017",
                    HanDung = new DateTime (2022, 06,01),
                    DonGia = (decimal)6800.00,
                    SoLuong = 30,
                    NhaCungCap = "0108976492-Nguyễn Thị Hải Yến: Cửa hàng bán lẻ"
                },
                new BaoCaoTinhHinhNhapNCCChiTietGridVo
                {
                    Id = 3,
                    MaThuoc = "TD8825",
                    TenThuoc = "Sunmesacol 400mg (Sun Pharma) - 0605330",
                    HoatChat = "Mesalazin",
                    DVT = "Viên",
                    HangSX = "Sun Pharma",
                    SoLo = "SKT0682",
                    HanDung = new DateTime (2022, 05,22),
                    DonGia = (decimal)3280.00,
                    SoLuong = 50,
                    NhaCungCap = "0315275368-Công ty CP Dược phẩm FPT Long Châu"
                },
                new BaoCaoTinhHinhNhapNCCChiTietGridVo
                {
                    Id = 4,
                    MaThuoc = "TD90725",
                    TenThuoc = "Ultracet - 0605330",
                    DVT = "Viên",
                    SoLo = "22784",
                    HanDung = new DateTime (2023, 04,13),
                    DonGia = (decimal)8100.00,
                    SoLuong = 60,
                    NhaCungCap = "0315275368-Công ty CP Dược phẩm FPT Long Châu"

                },
                new BaoCaoTinhHinhNhapNCCChiTietGridVo
                {
                    Id = 5,
                    MaThuoc = "TD2875",
                    TenThuoc = "Lidocain 2% 10ml 2% - 0003344",
                    HoatChat = "Lidocain (hydrochlorid)",
                    DVT = "Ống",
                    SoLo = "T025A0319",
                    HanDung = new DateTime (2022, 03,06),
                    DonGia = (decimal)16120.02,
                    SoLuong = 200,
                    NhaCungCap = "BEPHARCO-Chi nhánh công ty cổ phần dược phẩm Bến Tre tại Hà Nội"

                },
                new BaoCaoTinhHinhNhapNCCChiTietGridVo
                {
                    Id = 6,
                    MaThuoc = "TD02937",
                    TenThuoc = "Lidocain 2% 10ml 2% - 0003694",
                    HoatChat = "Lidocain (hydrochlorid)",
                    DVT = "Ống",
                    SoLo = "T099A1219",
                    HanDung = new DateTime (2022, 12,03),
                    DonGia = (decimal)16120.02,
                    SoLuong = 400,
                    NhaCungCap = "BEPHARCO-Chi nhánh công ty cổ phần dược phẩm Bến Tre tại Hà Nội"

                },
            };
            return new GridDataSource { Data = data.ToArray(), TotalRowCount = data.Count() };
        }
        public virtual byte[] ExportBaoCaoTinhHinhNhapNCCChiTiet(GridDataSource gridDataSource, BaoCaoTinhHinhNhapNCCChiTietQueryInfo query)
        {
            var datas = (ICollection<BaoCaoTinhHinhNhapNCCChiTietGridVo>)gridDataSource.Data;
            var listNCC = datas.GroupBy(s => s.NhaCungCap).Select(s => s.First().NhaCungCap).ToList();
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BẢNG KÊ TÌNH HÌNH NHẬP NHÀ CUNG CẤP CHI TIẾT");
                    //set row
                    worksheet.DefaultRowHeight = 16;

                    //set chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 15;
                    worksheet.Column(2).Width = 20;
                    worksheet.Column(3).Width = 30;
                    worksheet.Column(4).Width = 15;
                    worksheet.Column(5).Width = 15;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 15;
                    worksheet.Column(8).Width = 15;
                    worksheet.Column(9).Width = 15;
                    worksheet.Column(10).Width = 15;
                    worksheet.Column(11).Width = 15;
                    worksheet.Column(12).Width = 15;
                    worksheet.DefaultColWidth = 7;

                    worksheet.Row(4).Height = 31;
                    worksheet.Row(5).Height = 21;


                    using (var range = worksheet.Cells["A1:C2"])
                    {
                        range.Worksheet.Cells["A1:C2"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:C2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A1:C2"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A1:C2"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:C2"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A1:C2"].Merge = true;
                        range.Worksheet.Cells["A1:C2"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                    }

                    using (var range = worksheet.Cells["D3:K4"])
                    {
                        range.Worksheet.Cells["D3:K4"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["D3:K4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["D3:K4"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["D3:K4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["D3:K4"].Style.Font.Bold = true;
                        range.Worksheet.Cells["D3:K4"].Merge = true;
                        range.Worksheet.Cells["D3:K4"].Style.WrapText = true;
                        range.Worksheet.Cells["D3:K4"].Value = $"BÁO CÁO TÌNH HÌNH NHẬP NHÀ CUNG CẤP CHI TIẾT {Environment.NewLine} Thời gian từ:  {query.FromDate.ApplyFormatDate()} - {query.ToDate.ApplyFormatDate()}";
                    }

                    var tenKho = _khoRepository.TableNoTracking.Where(p => p.Id == query.KhoId).Select(p => p.Ten).FirstOrDefault();
                    using (var range = worksheet.Cells["D5:K5"])
                    {
                        range.Worksheet.Cells["D5:K5"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["D5:K5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["D5:K5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["D5:K5"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["D5:K5"].Style.Font.Bold = true;
                        range.Worksheet.Cells["D5:K5"].Merge = true;
                        range.Worksheet.Cells["D5:K5"].Value = $"{tenKho}";
                    }

                    using (var range = worksheet.Cells["A8:K9"])
                    {
                        range.Worksheet.Cells["A8:K9"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A8:K9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A8:K9"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A8:K9"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A8:K9"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A8:K9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A8:A9"].Value = "STT";
                        range.Worksheet.Cells["A8:A9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A8:A9"].Merge = true;

                        range.Worksheet.Cells["B8:B9"].Value = "Mã thuốc";
                        range.Worksheet.Cells["B8:B9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B8:B9"].Merge = true;

                        range.Worksheet.Cells["C8:C9"].Value = "Tên thuốc, Hàm lượng (Hãng, Nước sản xuất)";
                        range.Worksheet.Cells["C8:C9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C8:C9"].Merge = true;
                        range.Worksheet.Cells["C8:C9"].Style.WrapText = true;

                        range.Worksheet.Cells["D8:D9"].Value = "Hoạt chất";
                        range.Worksheet.Cells["D8:D9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D8:D9"].Merge = true;
                        range.Worksheet.Cells["D8:D9"].Style.WrapText = true;

                        range.Worksheet.Cells["E8:E9"].Value = "ĐVT";
                        range.Worksheet.Cells["E8:E9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E8:E9"].Merge = true;

                        range.Worksheet.Cells["F8:F9"].Value = "Hãng SX";
                        range.Worksheet.Cells["F8:F9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F8:F9"].Merge = true;

                        range.Worksheet.Cells["G8:G9"].Value = "Số lô";
                        range.Worksheet.Cells["G8:G9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G8:G9"].Merge = true;

                        range.Worksheet.Cells["H8:H9"].Value = "Hạn dùng";
                        range.Worksheet.Cells["H8:H9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H8:H9"].Merge = true;

                        range.Worksheet.Cells["I8:I9"].Value = "Đơn giá";
                        range.Worksheet.Cells["I8:I9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I8:I9"].Merge = true;

                        range.Worksheet.Cells["J8:J9"].Value = "Số lượng";
                        range.Worksheet.Cells["J8:J9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J8:J9"].Merge = true;

                        range.Worksheet.Cells["K8:K9"].Value = "Thành tiền";
                        range.Worksheet.Cells["K8:K9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K8:K9"].Merge = true;

                    }
                    int index = 10;
                    ///Đổ data vào
                    ///
                    int stt = 1;
                    if (listNCC.Any())
                    {
                        foreach (var ncc in listNCC)
                        {
                            var listTheoNCC = datas.Where(s => s.NhaCungCap == ncc).ToList();
                            if (listTheoNCC.Any())
                            {
                                using (var range = worksheet.Cells["A" + index + ":K" + index])
                                {
                                    range.Worksheet.Cells["A" + index + ":K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                    range.Worksheet.Cells["A" + index + ":K" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                    range.Worksheet.Cells["A" + index + ":K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["A" + index + ":K" + index].Style.Font.Bold = true;

                                    range.Worksheet.Cells["A" + index + ":F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["A" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    range.Worksheet.Cells["A" + index + ":F" + index].Merge = true;
                                    range.Worksheet.Cells["A" + index + ":F" + index].Value = ncc;

                                    range.Worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                    range.Worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                    range.Worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                    range.Worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                    range.Worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    range.Worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                                    range.Worksheet.Cells["K" + index].Value = listTheoNCC.Sum(a => a.ThanhTien);

                                    index++;
                                }

                                foreach (var item in listTheoNCC)
                                {
                                    using (var range = worksheet.Cells["A" + index + ":K" + index])
                                    {
                                        range.Worksheet.Cells["A" + index + ":K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                        range.Worksheet.Cells["A" + index + ":K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        range.Worksheet.Cells["A" + index + ":K" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                        range.Worksheet.Cells["A" + index + ":K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                        range.Worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["A" + index].Value = stt;
                                        range.Worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                        range.Worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["B" + index].Value = item.MaThuoc;

                                        range.Worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["C" + index].Value = item.TenThuoc;
                                        range.Worksheet.Cells["C" + index].Style.WrapText = true;

                                        range.Worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["D" + index].Value = item.HoatChat;
                                        range.Worksheet.Cells["D" + index].Style.WrapText = true;

                                        range.Worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["E" + index].Value = item.DVT;

                                        range.Worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["F" + index].Value = item.HangSX;

                                        range.Worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["G" + index].Value = item.SoLo;

                                        range.Worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["H" + index].Value = item.HanDungStr;

                                        range.Worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["I" + index].Value = item.DonGia;
                                        range.Worksheet.Cells["I" + index].Style.Numberformat.Format = "#,##0.00";
                                        range.Worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                                        range.Worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["J" + index].Value = item.SoLuong;
                                        range.Worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                                        range.Worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["K" + index].Value = item.ThanhTien;
                                        range.Worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                                        range.Worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                                        stt++;
                                        index++;
                                    }
                                }
                            }



                        }

                    }

                    index++;
                    worksheet.Cells["J" + index + ":K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    worksheet.Cells["J" + index + ":K" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));

                    worksheet.Cells["J" + index].Value = "Tổng cộng";
                    worksheet.Cells["J" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                    worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    var tongCong = datas.Sum(s => s.ThanhTien);
                    worksheet.Cells["K" + index].Value = tongCong;
                    worksheet.Cells["K" + index].Style.Font.Bold = true;
                    worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    index++;

                    worksheet.Cells["A" + index + ":C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    worksheet.Cells["A" + index + ":C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells["A" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                    worksheet.Cells["A" + index + ":C" + index].Merge = true;
                    worksheet.Cells["A" + index + ":C" + index].Value = $"Cộng: {datas.Count()} khoản";
                    index++;

                    worksheet.Cells["A" + index + ":K" + (index + 1)].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    worksheet.Cells["A" + index + ":K" + (index + 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells["A" + index + ":K" + (index + 1)].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                    worksheet.Cells["A" + index + ":K" + (index + 1)].Merge = true;
                    worksheet.Cells["A" + index + ":K" + (index + 1)].Value = $"Tổng số tiền (viết bằng chữ): {NumberHelper.ChuyenSoRaText(Convert.ToDouble(tongCong))}";
                    index = index + 2;

                    worksheet.Cells["J" + index + ":K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    worksheet.Cells["J" + index + ":K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["J" + index + ":K" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                    worksheet.Cells["J" + index + ":K" + index].Value = $"Ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}";
                    worksheet.Cells["J" + index + ":K" + index].Style.Font.Italic = true;
                    worksheet.Cells["J" + index + ":K" + index].Style.Font.Bold = true;
                    worksheet.Cells["J" + index + ":K" + index].Merge = true;
                    index++;

                    worksheet.Cells["A" + index + ":K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    worksheet.Cells["A" + index + ":K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A" + index + ":K" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                    worksheet.Cells["A" + index + ":K" + index].Style.Font.Bold = true;

                    worksheet.Cells["A" + index + ":C" + (index + 1)].Value = $"Trưởng khoa dược {Environment.NewLine} (Ký, ghi rõ họ tên)";
                    worksheet.Cells["A" + index + ":C" + (index + 1)].Merge = true;
                    worksheet.Cells["A" + index + ":C" + (index + 1)].Style.WrapText = true;

                    worksheet.Cells["D" + index + ":E" + (index + 1)].Value = $"Kế toán {Environment.NewLine} (Ký, ghi rõ họ tên)";
                    worksheet.Cells["D" + index + ":E" + (index + 1)].Merge = true;
                    worksheet.Cells["D" + index + ":E" + (index + 1)].Style.WrapText = true;

                    worksheet.Cells["G" + index + ":H" + (index + 1)].Value = $"Thủ kho {Environment.NewLine} (Ký, ghi rõ họ tên)";
                    worksheet.Cells["G" + index + ":H" + (index + 1)].Merge = true;
                    worksheet.Cells["G" + index + ":H" + (index + 1)].Style.WrapText = true;

                    worksheet.Cells["J" + index + ":K" + (index + 1)].Value = $"Người lập {Environment.NewLine} (Ký, ghi rõ họ tên)";
                    worksheet.Cells["J" + index + ":K" + (index + 1)].Merge = true;
                    worksheet.Cells["J" + index + ":K" + (index + 1)].Style.WrapText = true;

                    xlPackage.Save();
                }
                return stream.ToArray();

            }
        }
    }
}
