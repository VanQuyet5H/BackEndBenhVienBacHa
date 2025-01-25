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
using Camino.Core.Domain;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public async Task<GridDataSource> GetDataBaoCaoBangKeChiTietTTCNForGridAsync(BaoCaoBangKeChiTietTTCNQueryInfo queryInfo)
        {
            var phieuThuCongNos = _taiKhoanBenhNhanThuRepository.TableNoTracking
                .Where(o=>o.DaHuy != true && o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi && 
                          ((o.CongNo != null && o.CongNo > 0) || o.CongTyBaoHiemTuNhanCongNos.Any()) &&
                          o.NgayThu >= queryInfo.FromDate && o.NgayThu < queryInfo.ToDate)
                .Select(o=>new BaoCaoBangKeChiTietTTCNData
                {
                    Id = o.Id,
                    NgayThu = o.NgayThu,
                    MaTN = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    TenBN = o.YeuCauTiepNhan.HoTen,
                    SoPhieuThu = o.SoPhieuHienThi,
                    SoTienCongNo = o.CongNo,
                    ChiTietTTCNCongtys = o.CongTyBaoHiemTuNhanCongNos.Select(cty=>new BaoCaoBangKeChiTietTTCNCongty
                    {
                        TenCongTy = cty.CongTyBaoHiemTuNhan.Ten,
                        SoTien = cty.SoTien,
                        DaHuy = cty.DaHuy
                    }).ToList(),
                    ChiTietTTCNPhieuTraNoCaNhans = o.TaiKhoanBenhNhanThus.Select(t => new BaoCaoBangKeChiTietTTCNPhieuTraNoCaNhan
                    {
                        TienMat = t.TienMat,
                        ChuyenKhoan = t.ChuyenKhoan,
                        POS = t.POS,
                        NgayThu = t.NgayThu,
                        DaHuy = t.DaHuy
                    }).ToList()
                }).OrderBy(o=>o.NgayThu).ToList();

            var dataReturn = new List<BaoCaoBangKeChiTietTTCNGridVo>();
            foreach (var baoCaoBangKeChiTietTtcnData in phieuThuCongNos)
            {
                if (baoCaoBangKeChiTietTtcnData.SoTienCongNo.GetValueOrDefault() > 0)
                {
                    var congNoCaNhan = new BaoCaoBangKeChiTietTTCNGridVo
                    {
                        NgayThang = baoCaoBangKeChiTietTtcnData.NgayThu,
                        DoiTuongBaoLanhCongNo = baoCaoBangKeChiTietTtcnData.TenBN,
                        MaTN = baoCaoBangKeChiTietTtcnData.MaTN,
                        TenBN = baoCaoBangKeChiTietTtcnData.TenBN,
                        SoPhieuThu = baoCaoBangKeChiTietTtcnData.SoPhieuThu,
                        NgayPhatSinhPhieuThu = baoCaoBangKeChiTietTtcnData.NgayThu,
                        SoTienCongNo = baoCaoBangKeChiTietTtcnData.SoTienCongNo,
                        SoTienDaThanhToan = baoCaoBangKeChiTietTtcnData.ChiTietTTCNPhieuTraNoCaNhans.Where(tra=>tra.DaHuy!=true).Select(tra=>tra.TienMat.GetValueOrDefault()+ tra.ChuyenKhoan.GetValueOrDefault()+ tra.POS.GetValueOrDefault()).DefaultIfEmpty().Sum()
                    };
                    dataReturn.Add(congNoCaNhan);
                }

                if (baoCaoBangKeChiTietTtcnData.ChiTietTTCNCongtys.Any(o => o.DaHuy != true))
                {
                    var groupCty = baoCaoBangKeChiTietTtcnData.ChiTietTTCNCongtys.Where(o => o.DaHuy != true).GroupBy(o => o.TenCongTy);
                    foreach (var cty in groupCty)
                    {
                        var congNoCty = new BaoCaoBangKeChiTietTTCNGridVo
                        {
                            NgayThang = baoCaoBangKeChiTietTtcnData.NgayThu,
                            DoiTuongBaoLanhCongNo = cty.Key,
                            MaTN = baoCaoBangKeChiTietTtcnData.MaTN,
                            TenBN = baoCaoBangKeChiTietTtcnData.TenBN,
                            SoPhieuThu = baoCaoBangKeChiTietTtcnData.SoPhieuThu,
                            NgayPhatSinhPhieuThu = baoCaoBangKeChiTietTtcnData.NgayThu,
                            SoTienCongNo = cty.Sum(o=>o.SoTien),
                            SoTienDaThanhToan = 0
                        };
                        dataReturn.Add(congNoCty);
                    }
                }
            }
            return new GridDataSource { Data = dataReturn.ToArray(), TotalRowCount = dataReturn.Count() };
            
        }
        public virtual byte[] ExportBaoCaoBangKeChiTietTTCN(GridDataSource gridDataSource, BaoCaoBangKeChiTietTTCNQueryInfo query)
        {
            var datas = (ICollection<BaoCaoBangKeChiTietTTCNGridVo>)gridDataSource.Data;

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BẢNG KÊ CHI TIẾT TTCN");
                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 15;
                    worksheet.Column(3).Width = 23;
                    worksheet.Column(4).Width = 15;
                    worksheet.Column(5).Width = 20;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 15;
                    worksheet.Column(8).Width = 15;
                    worksheet.Column(9).Width = 20;
                    worksheet.Column(10).Width = 23;
                    worksheet.Column(11).Width = 15;
                    worksheet.DefaultColWidth = 7;
                    using (var range = worksheet.Cells["A1:F1"])
                    {
                        range.Worksheet.Cells["A1:F1"].Merge = true;
                        range.Worksheet.Cells["A1:F1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:F1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:F1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A1:F1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:F1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:F1"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A3:K3"])
                    {
                        range.Worksheet.Cells["A3:K3"].Merge = true;
                        range.Worksheet.Cells["A3:K3"].Value = "BẢNG KÊ CHI TIẾT THANH TOÁN CÔNG NỢ KHÁCH HÀNG/ ĐỐI TƯỢNG BẢO LÃNH CÔNG NỢ";
                        range.Worksheet.Cells["A3:K3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:K3"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A3:K3"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A3:K3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:K3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["D4:I4"])
                    {
                        range.Worksheet.Cells["D4:I4"].Merge = true;
                        range.Worksheet.Cells["D4:I4"].Value = "Từ ngày " + query.FromDate.FormatNgayGioTimKiemTrenBaoCao()
                                                          + " đến ngày " + query.ToDate.FormatNgayGioTimKiemTrenBaoCao();
                        range.Worksheet.Cells["D4:I4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["D4:I4"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["D4:I4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["D4:I4"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["A6:K6"])
                    {
                        range.Worksheet.Cells["A6:K6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A6:K6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A6:K6"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A6:K6"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A6:K6"].Style.Font.Color.SetColor(Color.Black);

                        range.Worksheet.Cells["A6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A6"].Value = "STT";

                        range.Worksheet.Cells["B6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B6"].Value = "NGÀY THÁNG";

                        range.Worksheet.Cells["C6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C6"].Value = "Đối tượng bảo lãnh công nợ";

                        range.Worksheet.Cells["D6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D6"].Value = "Mã TN";

                        range.Worksheet.Cells["E6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E6"].Value = "Tên BN";

                        range.Worksheet.Cells["F6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F6"].Value = "Số BL/HĐ";

                        range.Worksheet.Cells["G6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G6"].Value = "Ngày BL/HD";

                        range.Worksheet.Cells["H6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H6"].Value = "Số tiền phải thu";

                        range.Worksheet.Cells["I6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I6"].Value = "Số tiền đã thanh toán";

                        range.Worksheet.Cells["J6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J6"].Value = "Số tiền còn phải thanh toán";

                        range.Worksheet.Cells["K6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K6"].Value = "Số tiền hoàn trả";
                    }

                    int stt = 1;
                    int index = 7;

                    if (datas.Any())
                    {
                        foreach (var item in datas)
                        {
                            worksheet.Cells["A" + index + ":K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                            worksheet.Cells["A" + index + ":K" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            worksheet.Cells["A" + index + ":K" + index].Style.Font.Color.SetColor(Color.Black);

                            worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + index].Value = stt;

                            worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["B" + index].Value = item.NgayThangStr;

                            worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["C" + index].Value = item.DoiTuongBaoLanhCongNo;

                            worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["D" + index].Value = item.MaTN;

                            worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["E" + index].Value = item.TenBN;

                            worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["F" + index].Value = item.SoPhieuThu;

                            worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["G" + index].Value = item.NgayPhatSinhPhieuThuStr;

                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["H" + index].Value = item.SoTienCongNo;

                            worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["I" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["I" + index].Value = item.SoTienDaThanhToan;

                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["J" + index].Value = item.SoTienConNo;

                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["K" + index].Value = item.SoTienHoanTra;
                            index++;
                            stt++;
                        }
                    }

                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }

    }
}
