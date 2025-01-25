using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Services.ExportImport.Help;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public List<LookupItemVo> GetListDoan(DropDownListRequestModel model)
        {
            var lookupData = _hopDongKhamSucKhoeRepository.TableNoTracking
                .Select(o => o.CongTyKhamSucKhoe)
                .Distinct()
                .OrderBy(o => o.Ten)
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.Ten,
                    KeyId = item.Id,
                })
                .ApplyLike(model.Query, o => o.DisplayName)
                .ToList();
            lookupData.Insert(0, new LookupItemVo
            {
                DisplayName = "Không theo đoàn",
                KeyId = 0,
            });
            lookupData.Insert(0, new LookupItemVo
            {
                DisplayName = "Tất cả",
                KeyId = -1,
            });
            return lookupData;
        }
        private async Task<List<BaoCaoTiepNhanBenhPhamGridVo>> GetAllDataForBaoCaoTiepNhanBenhPham(BaoCaoTiepNhanBenhPhamQueryInfo queryInfo)
        {
            var item1 = new BaoCaoTiepNhanBenhPhamGridVo()
            {
                Id = 1,
                GioNhan = DateTime.Now,
                Barcode = "BP-001",
                Tuoi = 26,
                GioiTinh = "Nữ",
                MauBenhPham = "XN - máu",
                NguoiGiaoMau = "YTA - Hương",
                NguoiNhanMau = "BS - Cường",
                NguoiGiaoKQ = "YTA - Hương",
                NguoiNhanKQ = "YTA - Hương",
                GhiChu = "Xét nghiệm nhóm bệnh nhân CT ABC"
            };
            var item2 = new BaoCaoTiepNhanBenhPhamGridVo()
            {
                Id = 1,
                GioNhan = DateTime.Now,
                Barcode = "BP-002",
                Tuoi = 29,
                GioiTinh = "Nam",
                MauBenhPham = "XN - máu",
                NguoiGiaoMau = "YTA - Hương",
                NguoiNhanMau = "BS - Cường",
                NguoiGiaoKQ = "YTA - Hương",
                NguoiNhanKQ = "YTA - Hương",
                GhiChu = "Xét nghiệm nhóm bệnh nhân CT ABC"
            };
            var data = new List<BaoCaoTiepNhanBenhPhamGridVo>();
            data.Add(item1);
            data.Add(item2);

            return data;
        }
        public async Task<GridDataSource> GetDataBaoCaoTiepNhanBenhPhamForGridAsync(BaoCaoTiepNhanBenhPhamQueryInfo queryInfo)
        {
            var nhomDichVuBenhViens = _nhomDichVuBenhVienRepository.TableNoTracking.ToList();
            var phienXetNghiems = _phienXetNghiemRepository.TableNoTracking
                .Where(o => queryInfo.DoanId == -1 || (queryInfo.DoanId == 0 && o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId == null)
                || (queryInfo.DoanId > 0 && o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId != null && queryInfo.DoanId == o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoeId))
                .Where(o => o.PhienXetNghiemChiTiets.Any(ct => ct.ThoiDiemNhanMau != null && ct.ThoiDiemNhanMau >= queryInfo.FromDate && ct.ThoiDiemNhanMau < queryInfo.ToDate))
                .Include(o => o.YeuCauTiepNhan).Include(o => o.PhienXetNghiemChiTiets);

            var allData = new List<BaoCaoTiepNhanBenhPhamGridVo>();
            foreach (var phienXetNghiem in phienXetNghiems)
            {
                var phienXetNghiemChiTiets = phienXetNghiem.PhienXetNghiemChiTiets
                    .Where(ct => ct.ThoiDiemNhanMau != null && ct.ThoiDiemNhanMau >= queryInfo.FromDate && ct.ThoiDiemNhanMau < queryInfo.ToDate).ToList();

                var thoiDiemNhanMau = phienXetNghiemChiTiets.Select(ct => ct.ThoiDiemNhanMau.GetValueOrDefault()).OrderBy(o => o).Last();
                
                var nhomXn = nhomDichVuBenhViens.Where(o => phienXetNghiemChiTiets.Select(ct => ct.NhomDichVuBenhVienId).Contains(o.Id)).ToList();

                allData.Add(new BaoCaoTiepNhanBenhPhamGridVo()
                {
                    Id = phienXetNghiem.Id,
                    GioNhan = thoiDiemNhanMau,
                    Barcode = phienXetNghiem.BarCodeId,
                    HoTen = phienXetNghiem.YeuCauTiepNhan.HoTen,
                    NamSinh = phienXetNghiem.YeuCauTiepNhan.NamSinh,
                    //Tuoi = CalculateHelper.TinhTuoi(phienXetNghiem.YeuCauTiepNhan.NgaySinh, phienXetNghiem.YeuCauTiepNhan.ThangSinh, phienXetNghiem.YeuCauTiepNhan.NamSinh),
                    GioiTinh = phienXetNghiem.YeuCauTiepNhan.GioiTinh?.GetDescription(),
                    MauBenhPham = string.Join(Environment.NewLine, nhomXn.Select(o => o.Ten)),
                });
                
            }

            return new GridDataSource { Data = allData.OrderBy(o => o.Barcode).ThenBy(o => o.GioNhan).ToArray(), TotalRowCount = allData.Count() };
        }

        public async Task<GridDataSource> GetTotalBaoCaoTiepNhanBenhPhamForGridAsync(BaoCaoTiepNhanBenhPhamQueryInfo queryInfo)
        {
            var phienXetNghiems = _phienXetNghiemRepository.TableNoTracking
                .Where(o => queryInfo.DoanId == -1 || (queryInfo.DoanId == 0 && o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId == null)
                || (queryInfo.DoanId > 0 && o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId != null && queryInfo.DoanId == o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoeId))
                .Where(o => o.PhienXetNghiemChiTiets.Any(ct => ct.ThoiDiemNhanMau != null && ct.ThoiDiemNhanMau >= queryInfo.FromDate && ct.ThoiDiemNhanMau < queryInfo.ToDate));

            return new GridDataSource { TotalRowCount = phienXetNghiems.Count() };
        }

        public virtual byte[] ExportBaoCaoTiepNhanBenhPhamGridVo(GridDataSource gridDataSource, BaoCaoTiepNhanBenhPhamQueryInfo query)
        {
            var datas = (ICollection<BaoCaoTiepNhanBenhPhamGridVo>)gridDataSource.Data;
            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoTiepNhanBenhPhamGridVo>("STT", p => ind++)
            };
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO TIẾP NHẬN BỆNH PHẨM");

                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 15;
                    worksheet.Column(2).Width = 5;
                    worksheet.Column(3).Width = 15;
                    worksheet.Column(4).Width = 30;
                    worksheet.Column(5).Width = 10;
                    worksheet.Column(6).Width = 7;
                    worksheet.Column(7).Width = 30;
                    worksheet.Column(8).Width = 25;
                    worksheet.Column(9).Width = 25;
                    worksheet.Column(10).Width = 25;
                    worksheet.Column(11).Width = 25;
                    worksheet.Column(12).Width = 40;
                    worksheet.DefaultColWidth = 7;

                    //SET title BV
                    using (var range = worksheet.Cells["A1:E1"])
                    {
                        range.Worksheet.Cells["A1:E1"].Merge = true;
                        range.Worksheet.Cells["A1:E1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A1:E1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:E1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:E1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:E1"].Style.Font.Bold = true;
                    }

                    // SET title head cho bảng excel
                    using (var range = worksheet.Cells["A3:L3"])
                    {
                        range.Worksheet.Cells["A3:L3"].Merge = true;
                        range.Worksheet.Cells["A3:L3"].Value = "PHIẾU TIẾP NHẬN BỆNH PHẨM";
                        range.Worksheet.Cells["A3:L3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:L3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:L3"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A3:L3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:L3"].Style.Font.Bold = true;
                    }

                    var index = 4;
                    var tenDoan = "";
                    if (query.DoanId > 0)
                    {
                        tenDoan = _hopDongKhamSucKhoeRepository.TableNoTracking.Where(x => x.CongTyKhamSucKhoe.Id == query.DoanId).Select(o => o.CongTyKhamSucKhoe.Ten).FirstOrDefault();

                        using (var range = worksheet.Cells["A" + index + ":L" + index])
                        {
                            range.Worksheet.Cells["A" + index + ":L" + index].Merge = true;
                            range.Worksheet.Cells["A" + index + ":L" + index].Value = "Đoàn: " + tenDoan;
                            range.Worksheet.Cells["A" + index + ":L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells["A" + index + ":L" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells["A" + index + ":L" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells["A" + index + ":L" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["A" + index + ":L" + index].Style.Font.Bold = true;
                        }
                        index++;
                    }

                    using (var range = worksheet.Cells["A" + index + ":L" + index])
                    {
                        range.Worksheet.Cells["A" + index + ":L" + index].Merge = true;
                        //range.Worksheet.Cells["A" + index + ":L" + index].Value = "Từ ngày: " + query.FromDate.FormatNgayGioTimKiemTrenBaoCao()
                        //                             + " - đến ngày: " + query.ToDate.FormatNgayGioTimKiemTrenBaoCao();
                        range.Worksheet.Cells["A" + index + ":L" + index].Value = "Từ " + query.FromDate.Hour + ":" + query.FromDate.ToString("mm") + ", Ngày " + query.FromDate.ToString("dd/MM/yyy")
                                                     + " đến " + query.ToDate.Hour + ":" + query.ToDate.ToString("mm") + ", Ngày " + query.ToDate.ToString("dd/MM/yyy");
                        range.Worksheet.Cells["A" + index + ":L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":L" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A" + index + ":L" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A" + index + ":L" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A" + index + ":L" + index].Style.Font.Italic = true;
                        index += 2;
                    }

                    using (var range = worksheet.Cells["A" + index + ":L" + index])
                    {
                        range.Worksheet.Cells["A" + index + ":L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":L" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":L" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A" + index + ":L" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A" + index + ":L" + index].Style.Font.Bold = true;
                        range.Worksheet.Cells["A" + index + ":L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A" + index].Value = "Ngày giờ";

                        range.Worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B" + index].Value = "STT";

                        range.Worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C" + index].Value = "Mã Barcode";

                        range.Worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D" + index].Value = "Tên BN";

                        range.Worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E" + index].Value = "NS";

                        range.Worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F" + index].Value = "Giới";

                        range.Worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G" + index].Value = "Mẫu bệnh phẩm";

                        range.Worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H" + index].Value = "Ng. Giao mẫu";

                        range.Worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I" + index].Value = "Ng. Nhận mẫu";

                        range.Worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J" + index].Value = "Ng. Giao KQ";

                        range.Worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K" + index].Value = "Ng. Nhận KQ";

                        range.Worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["L" + index].Value = "Ghi chú";
                        index++;
                    }

                    var stt = 1;
                    if (datas.Any())
                    {
                        foreach (var item in datas)
                        {
                            using (var range = worksheet.Cells["A" + index + ":K" + index])
                            {
                                range.Worksheet.Cells["A" + index + ":K" + index].Style.HorizontalAlignment =
                                    ExcelHorizontalAlignment.Center;
                                range.Worksheet.Cells["A" + index + ":K" + index].Style.VerticalAlignment =
                                    ExcelVerticalAlignment.Bottom;
                                range.Worksheet.Cells["A" + index + ":K" + index].Style.Font
                                    .SetFromFont(new Font("Times New Roman", 10));
                                range.Worksheet.Cells["A" + index + ":K" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["A" + index + ":K" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["A" + index + ":K" + index].Style.Font.Color
                                    .SetColor(Color.Black);

                                worksheet.Cells["A" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index].Value = item.GioNhanString;

                                worksheet.Cells["B" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["B" + index].Value = stt;

                                worksheet.Cells["C" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["C" + index].Value = item.Barcode;

                                worksheet.Cells["D" + index].Style.Border
                                   .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["D" + index].Value = item.HoTen;

                                worksheet.Cells["E" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["E" + index].Value = item.NamSinh;

                                worksheet.Cells["F" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["F" + index].Value = item.GioiTinh;

                                worksheet.Cells["G" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["G" + index].Value = item.MauBenhPham;

                                worksheet.Cells["H" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["H" + index].Value = item.NguoiGiaoMau;

                                worksheet.Cells["I" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["I" + index].Value = item.NguoiNhanMau;

                                worksheet.Cells["J" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["J" + index].Value = item.NguoiGiaoKQ;

                                worksheet.Cells["K" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["K" + index].Value = item.NguoiNhanKQ;

                                worksheet.Cells["L" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["L" + index].Value = item.GhiChu;
                                index++;
                            }
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
