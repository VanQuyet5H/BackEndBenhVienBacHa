using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Services.ExportImport.Help;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using Camino.Services.Helpers;
using Camino.Core.Data;
using Microsoft.EntityFrameworkCore.Internal;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public List<LookupItemBacSiTemplateVo> GetTatCaBacSiKeDonTheoThuoc(DropDownListRequestModel queryInfo)
        {
            var allKhoas = new List<LookupItemBacSiTemplateVo>()
            {
                new LookupItemBacSiTemplateVo {KeyId = 0 , DisplayName = "Toàn viện" }
            };

            //Bổ sung thêm Mã NV, chức danh của NV để chọn BS cho chính xác
            //lấy Id làm Mã NV luôn
            var result = _nhanVienRepository.TableNoTracking.Include(d=>d.ChucDanh)
                //lấy tất cả nhân viên
               //.Where(p => p.ChucDanh.NhomChucDanhId == (long)Enums.EnumNhomChucDanh.BacSi || (long)p.ChucDanh.NhomChucDanhId == (int)Enums.EnumNhomChucDanh.BacSiDuPhong)
               .Select(s => new LookupItemBacSiTemplateVo
               {
                   KeyId = s.Id,
                   DisplayName = s.User.HoTen,
                   MaChucDanh = s.ChucDanhId != null ? s.ChucDanh.Ten : "",
                   MaNV = s.Id.ToString()
               })
               .ApplyLike(queryInfo.Query, o => o.DisplayName, g => g.MaChucDanh,f=>f.MaNV)
               .Take(queryInfo.Take);


            allKhoas.AddRange(result);

            return allKhoas;
        }

        public List<DuocPhamBenhVienLookupItemVo> GetTatCaThuocBenhVien(DropDownListRequestModel queryInfo)
        {
            var getThuocBenhViens = _duocPhamBenhVienRepository.TableNoTracking
                         .Where(x => x.HieuLuc)
                            .Select(item => new DuocPhamBenhVienLookupItemVo
                            {
                                DisplayName = item.DuocPham.Ten,
                                KeyId = item.Id,
                                HamLuong = item.DuocPham.HamLuong
                            }).
                            ApplyLike(queryInfo.Query, o => o.DisplayName)
                           .ToList();

            return getThuocBenhViens;
        }

        public GridDataSource GetDataThongKeBacSiKeDonTheoThuocForGrid(ThongKeBacSiKeDonTheoThuocQueryInfo queryInfo)
        {
            var donThuocs = _yeuCauKhamBenhDonThuocChiTietRepository.TableNoTracking
                .Where(o => o.SoLuong > 0 && o.DuocPhamId == queryInfo.ThuocId && 
                            o.YeuCauKhamBenhDonThuoc.ThoiDiemKeDon >= queryInfo.TuNgay && o.YeuCauKhamBenhDonThuoc.ThoiDiemKeDon < queryInfo.DenNgay &&
                            (queryInfo.BacSiId == 0 || o.YeuCauKhamBenhDonThuoc.BacSiKeDonId == queryInfo.BacSiId))
                .Select(o => new ThongKeBacSiKeDonTheoThuocDataVo
                {
                    BacSiKeDonId = o.YeuCauKhamBenhDonThuoc.BacSiKeDonId,
                    KhoaPhongId = o.YeuCauKhamBenhDonThuoc.NoiKeDon.KhoaPhongId,
                    SoLuongDon = o.SoLuong,
                    SoLuongDaKe = o.DonThuocThanhToanChiTiets.Where(ct=>ct.DonThuocThanhToan.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan).Select(ct=>ct.SoLuong).DefaultIfEmpty().Sum()
                }).ToList();
            var bacSiIds = donThuocs.Select(o => o.BacSiKeDonId).Distinct().ToList();
            var khoaPhongIds = donThuocs.Select(o => o.KhoaPhongId).Distinct().ToList();
            var thongTinBacSi = _nhanVienRepository.TableNoTracking
                .Where(o => bacSiIds.Contains(o.Id))
                .Select(o => new {o.Id, o.User.HoTen})
                .ToList();
            var thongTinKhoaPhong = _KhoaPhongRepository.TableNoTracking
                .Where(o => khoaPhongIds.Contains(o.Id))
                .Select(o => new { o.Id, KhoaPhong = o.Ten })
                .ToList();

            var returnData = donThuocs.GroupBy(o => new { o.BacSiKeDonId, o.KhoaPhongId }, o => o, (k, v) =>
                new DanhSachThongKeBacSiKeDonTheoThuoc
                {
                    TenBacSi = thongTinBacSi.First(o=>o.Id == k.BacSiKeDonId).HoTen,
                    KhoaPhongBacSi = thongTinKhoaPhong.First(o => o.Id == k.KhoaPhongId).KhoaPhong,
                    SoLuongDon = Math.Round(v.Sum(o => o.SoLuongDon), 2),
                    SoLuongDaKe = Math.Round(v.Sum(o => o.SoLuongDaKe), 2),
                }).ToList();

            //var returnData = new List<DanhSachThongKeBacSiKeDonTheoThuoc>()
            //{
            //    new DanhSachThongKeBacSiKeDonTheoThuoc () {Id = 1 ,TenBacSi = "Nguyễn Văn A" , KhoaPhongBacSi = "Khoa sản" , SoLuongDon = 10 , SoLuongDaKe = 15 },
            //    new DanhSachThongKeBacSiKeDonTheoThuoc () {Id = 3 ,TenBacSi = "Nguyễn Văn A1" , KhoaPhongBacSi = "Khoa nội" ,SoLuongDon = 5 , SoLuongDaKe = 15 },
            //    new DanhSachThongKeBacSiKeDonTheoThuoc () {Id = 2 ,TenBacSi = "Nguyễn Văn A2" , KhoaPhongBacSi = "Khoa nhi" ,SoLuongDon = 5 , SoLuongDaKe = 15 }
            //};

            return new GridDataSource
            {
                Data = returnData.OrderBy(o=>o.TenBacSi).ToArray(),
                TotalRowCount = returnData.Count()
            };
        }

        public virtual byte[] ExportThongKeBacSiKeDonTheoThuoc(GridDataSource gridDataSource, ThongKeBacSiKeDonTheoThuocQueryInfo query)
        {
            var datas = (ICollection<DanhSachThongKeBacSiKeDonTheoThuoc>)gridDataSource.Data;

            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<DanhSachThongKeBacSiKeDonTheoThuoc>("STT", p => ind++)
            };

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("[DƯỢC] THỐNG KÊ BS KÊ ĐƠN THEO THUỐC");
                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 40;
                    worksheet.Column(3).Width = 20;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 20;
                    worksheet.Column(6).Width = 30;


                    worksheet.DefaultColWidth = 7;
                    worksheet.Row(8).Height = 24;


                    using (var range = worksheet.Cells["A1:E1"])
                    {
                        range.Worksheet.Cells["A1:E1"].Merge = true;
                        range.Worksheet.Cells["A1:E1"].Value = "THỐNG KÊ BS KÊ ĐƠN THEO THUỐC";
                        range.Worksheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A1:E1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:E1"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A1:E1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:E1"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A2:E2"])
                    {
                        range.Worksheet.Cells["A2:E2"].Merge = true;
                        range.Worksheet.Cells["A2:E2"].Value = "Từ ngày: " + query.TuNgay.FormatNgayGioTimKiemTrenBaoCao()
                                                          + " - đến ngày: " + query.DenNgay.FormatNgayGioTimKiemTrenBaoCao();

                        range.Worksheet.Cells["A2:E2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A2:E2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A2:E2"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A2:E2"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A2:E2"].Style.Font.Bold = true;
                    }

                    var thuoc = _duocPhamBenhVienRepository.TableNoTracking.Where(x => x.HieuLuc && x.Id == query.ThuocId)
                                                        .Include(o => o.DuocPham)
                                                        .ThenInclude(v => v.DonViTinh).FirstOrDefault();

                    using (var range = worksheet.Cells["A3:E3"])
                    {
                        range.Worksheet.Cells["A3:E3"].Merge = true;
                        range.Worksheet.Cells["A3:E3"].Value = "Thuốc: " + thuoc.DuocPham.Ten +" "+ thuoc.DuocPham.HamLuong + " (" + thuoc.DuocPham.DonViTinh.Ten + ")";
                        range.Worksheet.Cells["A3:E3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:E3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:E3"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A3:E3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:E3"].Style.Font.Bold = true;

                    }


                    using (var range = worksheet.Cells["A5:E5"])
                    {
                        range.Worksheet.Cells["A5:E5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A5:E5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A5:E5"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A5:E5"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A5:E5"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A5:E5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A5:A5"].Merge = true;
                        range.Worksheet.Cells["A5:A5"].Value = "STT";
                        range.Worksheet.Cells["A5:A5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A5:A5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["B5:B5"].Merge = true;
                        range.Worksheet.Cells["B5:B5"].Value = "Tên bác sĩ";
                        range.Worksheet.Cells["B5:B5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["B5:B5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["C5:C5"].Merge = true;
                        range.Worksheet.Cells["C5:C5"].Value = "Khoa";
                        range.Worksheet.Cells["C5:C5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["C5:C5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["D5:D5"].Merge = true;
                        range.Worksheet.Cells["D5:D5"].Value = "Số lượng đơn";
                        range.Worksheet.Cells["D5:D5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["D5:D5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["E5:E5"].Merge = true;
                        range.Worksheet.Cells["E5:E5"].Value = "Số lượng đã kê";
                        range.Worksheet.Cells["E5:E5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["E5:E5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }

                    var manager = new PropertyManager<DanhSachThongKeBacSiKeDonTheoThuoc>(requestProperties);
                    int index = 6;
                    var stt = 1;

                    if (datas.Any())
                    {
                        foreach (var item in datas)
                        {

                            using (var range = worksheet.Cells["A" + index + ":E" + index])
                            {
                                worksheet.Cells["A" + index + ":E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index + ":E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["A" + index + ":E" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));                         

                                worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["A" + index].Value = stt;

                                worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["B" + index].Value = item.TenBacSi;

                                worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["C" + index].Value = item.KhoaPhongBacSi;

                                worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["D" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["D" + index].Value = item.SoLuongDon;

                                worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["E" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["E" + index].Value = item.SoLuongDaKe;

                                index++;
                                stt++;
                            }
                        }
                    }

                    worksheet.Cells["A" + index + ":E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["A" + index + ":E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["A" + index + ":E" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                    worksheet.Cells["A" + index + ":E" + index].Style.Font.Bold = true;


                    worksheet.Cells["A" + index + ":B" + index].Merge = true;
                    worksheet.Cells["A" + index + ":B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells["A" + index + ":B" + index].Value = $"Tổng cộng: {datas.Count} khoản";
                    worksheet.Cells["A" + index + ":B" + index].Style.Font.Bold = true;

                    worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["D" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["D" + index].Value = datas.Sum(c => c.SoLuongDon);

                    worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["E" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["E" + index].Value = datas.Sum(c => c.SoLuongDaKe);

                    xlPackage.Save();
                }
                return stream.ToArray();

            }
        }
    }
}
