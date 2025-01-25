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
        //
        public List<LookupItemBacSiTemplateVo> GetTatCaBacSi(DropDownListRequestModel queryInfo)
        {
            var allNhanViens = new List<LookupItemBacSiTemplateVo>()
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
               .ApplyLike(queryInfo.Query, o => o.DisplayName, g => g.MaChucDanh, f => f.MaNV)
               .Take(queryInfo.Take);

            allNhanViens.AddRange(result);

            return allNhanViens;
        }

        public GridDataSource GetDataThongKeThuocTheoBacSiForGrid(ThongKeThuocTheoBacSiQueryInfo queryInfo)
        {
            var yeuCauDuocPhams = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                .Where(o => o.ThoiDiemChiDinh >= queryInfo.TuNgay && o.ThoiDiemChiDinh < queryInfo.DenNgay &&
                            (queryInfo.BacSiId == 0 || o.NhanVienChiDinhId == queryInfo.BacSiId) &&
                            o.SoLuong > 0 && o.XuatKhoDuocPhamChiTietId != null)
                .Select(o => new ThongKeThuocTheoBacSiDataVo
                {
                    LaThuocBHYT = o.LaDuocPhamBHYT,
                    NoiTru = o.YeuCauTiepNhan.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru,
                    DuocPhamBenhVienId = o.DuocPhamBenhVienId,
                    SoLuong = o.SoLuong,
                    //DonGia = o.DonGiaBan,
                    KhongTinhPhi = o.KhongTinhPhi,

                    DonGiaNhap = o.DonGiaNhap,
                    VAT = o.VAT,
                    TiLeTheoThapGia = o.TiLeTheoThapGia,
                    PhuongPhapTinhGiaTriTonKhos = o.XuatKhoDuocPhamChiTietId != null
                                ? o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Select(a => a.NhapKhoDuocPhamChiTiet.PhuongPhapTinhGiaTriTonKho).ToList()
                                : new List<Enums.PhuongPhapTinhGiaTriTonKho>()
                }).ToList();

            foreach(var yeuCauDuocPham in yeuCauDuocPhams)
            {
                var phuongPhapTinhGiaTriTonKho = yeuCauDuocPham.PhuongPhapTinhGiaTriTonKhos.Any() ? yeuCauDuocPham.PhuongPhapTinhGiaTriTonKhos.First() : Enums.PhuongPhapTinhGiaTriTonKho.ApVAT;
                var giaTonKho = yeuCauDuocPham.DonGiaNhap + (yeuCauDuocPham.DonGiaNhap * (phuongPhapTinhGiaTriTonKho == Enums.PhuongPhapTinhGiaTriTonKho.ApVAT ? yeuCauDuocPham.VAT.GetValueOrDefault() : 0) / 100);
                yeuCauDuocPham.DonGia = Math.Round(giaTonKho + (giaTonKho * yeuCauDuocPham.TiLeTheoThapGia.GetValueOrDefault() / 100), 2, MidpointRounding.AwayFromZero);
            }

            var donThuocThanhToanChiTiets = _donThuocThanhToanChiTietRepository.TableNoTracking
                .Where(o => o.SoLuong > 0 && o.DonThuocThanhToan.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan &&
                            o.DonThuocThanhToan.YeuCauKhamBenhDonThuoc != null && o.DonThuocThanhToan.YeuCauKhamBenhDonThuoc.ThoiDiemKeDon >= queryInfo.TuNgay && o.DonThuocThanhToan.YeuCauKhamBenhDonThuoc.ThoiDiemKeDon < queryInfo.DenNgay &&
                            (queryInfo.BacSiId == 0 || o.DonThuocThanhToan.YeuCauKhamBenhDonThuoc.BacSiKeDonId == queryInfo.BacSiId))
                .Select(o => new ThongKeThuocTheoBacSiDataVo
                {
                    LaThuocBHYT = o.DonThuocThanhToan.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT,
                    DuocPhamBenhVienId = o.DuocPhamId,
                    SoLuong = o.SoLuong,
                    DonGia = o.DonGiaBan,
                    SoTienMienGiam = o.SoTienMienGiam
                }).ToList();
            var allDuocPham = yeuCauDuocPhams.Concat(donThuocThanhToanChiTiets).ToList();
            var duocPhamBenhVienIds = allDuocPham.Select(o => o.DuocPhamBenhVienId).Distinct().ToList();
            var thongTinDuocPhams = _duocPhamBenhVienRepository.TableNoTracking
                .Where(o => duocPhamBenhVienIds.Contains(o.Id))
                .Select(o => new
                {
                    o.Id, DuocPham = o.DuocPham.Ten,
                    HamLuong = o.DuocPham.HamLuong,
                    DonViTinh = o.DuocPham.DonViTinh.Ten
                }).ToList();

            var returnData = allDuocPham.GroupBy(o =>new{ o.DuocPhamBenhVienId, o.LaThuocBHYT, o.NoiTru } , o => o, (k, v) =>
                new DanhSachThongKeThuocTheoBacSi
                {
                    DuocPhamBenhVienId = k.DuocPhamBenhVienId,
                    LaThuocBHYT = k.LaThuocBHYT,
                    NoiTru = k.NoiTru,
                    SoLuong = Math.Round(v.Sum(o=>o.SoLuong),2),
                    ThanhTien = v.Select(o=> o.KhongTinhPhi == true ? 0 : Math.Round(((decimal)o.SoLuong * o.DonGia) - o.SoTienMienGiam.GetValueOrDefault(), 2)).Sum(),
                }).ToList();
            foreach (var danhSachThongKeThuocTheoBacSi in returnData)
            {
                var dp = thongTinDuocPhams.First(o => o.Id == danhSachThongKeThuocTheoBacSi.DuocPhamBenhVienId);
                danhSachThongKeThuocTheoBacSi.TenThuocHamLuong = dp.DuocPham + (string.IsNullOrEmpty(dp.HamLuong) ? "" : " - " + dp.HamLuong);
                danhSachThongKeThuocTheoBacSi.DonViTinh = dp.DonViTinh;
            }

            //var returnData = new List<DanhSachThongKeThuocTheoBacSi>()
            //{
            //    new DanhSachThongKeThuocTheoBacSi () {Id = 1 ,TenThuocHamLuong = "Ephedrine Aguettant inj 30mg/ml" , DonViTinh = "Viên" ,LaThuocBHYT = true , SoLuong = 1 , DonGia = 20000},
            //    new DanhSachThongKeThuocTheoBacSi () {Id = 3 ,TenThuocHamLuong = "Ephedrine Aguettant inj 30mg/ml" , DonViTinh = "Viên" ,LaThuocBHYT = false , SoLuong = 1 , DonGia = 23000},
            //    new DanhSachThongKeThuocTheoBacSi () {Id = 2 ,TenThuocHamLuong = "Ephedrine Aguettant inj 30mg/ml" , DonViTinh = "Viên" ,LaThuocBHYT = true , SoLuong = 1 , DonGia = 23000}
            //};

            return new GridDataSource
            {
                Data = returnData.OrderBy(o=>o.TenThuocHamLuong).ToArray(),
                TotalRowCount = returnData.Count()
            };
        }

        public virtual byte[] ExportThongKeThuocTheoBacSi(GridDataSource gridDataSource, ThongKeThuocTheoBacSiQueryInfo query)
        {
            var datas = (ICollection<DanhSachThongKeThuocTheoBacSi>)gridDataSource.Data;

            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<DanhSachThongKeThuocTheoBacSi>("STT", p => ind++)
            };

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("[DƯỢC] THỐNG KÊ THUỐC THEO BÁC SĨ");
                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 40;
                    worksheet.Column(3).Width = 25;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 20;
                    worksheet.Column(6).Width = 30;


                    worksheet.DefaultColWidth = 7;
                    worksheet.Row(8).Height = 24;


                    using (var range = worksheet.Cells["A1:E1"])
                    {
                        range.Worksheet.Cells["A1:F1"].Merge = true;
                        range.Worksheet.Cells["A1:F1"].Value = "THỐNG KÊ THUỐC THEO BÁC SĨ";
                        range.Worksheet.Cells["A1:F1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A1:F1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:F1"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A1:F1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:F1"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A2:E2"])
                    {
                        range.Worksheet.Cells["A2:F2"].Merge = true;
                        range.Worksheet.Cells["A2:F2"].Value = "Từ ngày: " + query.TuNgay.FormatNgayGioTimKiemTrenBaoCao()
                                                          + " - đến ngày: " + query.DenNgay.FormatNgayGioTimKiemTrenBaoCao();

                        range.Worksheet.Cells["A2:F2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A2:F2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A2:F2"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A2:F2"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A2:F2"].Style.Font.Bold = true;
                    }

                    var tenBacSi = !String.IsNullOrEmpty(_nhanVienRepository.TableNoTracking.Where(x => x.Id == query.BacSiId).Select(c => c.User.HoTen).FirstOrDefault()) ?
                        _nhanVienRepository.TableNoTracking.Where(x => x.Id == query.BacSiId).Select(c => c.User.HoTen).FirstOrDefault() : "Toàn viện";

                    using (var range = worksheet.Cells["A3:E3"])
                    {
                        range.Worksheet.Cells["A3:F3"].Merge = true;
                        range.Worksheet.Cells["A3:F3"].Value = "BS: " + tenBacSi;
                        range.Worksheet.Cells["A3:F3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:F3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:F3"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A3:F3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:F3"].Style.Font.Bold = true;

                    }


                    using (var range = worksheet.Cells["A5:F5"])
                    {
                        range.Worksheet.Cells["A5:F5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A5:F5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A5:F5"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A5:F5"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A5:F5"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A5:F5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A5:A5"].Merge = true;
                        range.Worksheet.Cells["A5:A5"].Value = "STT";
                        range.Worksheet.Cells["A5:A5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A5:A5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["B5:B5"].Merge = true;
                        range.Worksheet.Cells["B5:B5"].Value = "Tên thuốc - Hàm lượng";
                        range.Worksheet.Cells["B5:B5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["B5:B5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["C5:C5"].Merge = true;
                        range.Worksheet.Cells["C5:C5"].Value = "Đơn vị";
                        range.Worksheet.Cells["C5:C5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["C5:C5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["D5:D5"].Merge = true;
                        range.Worksheet.Cells["D5:D5"].Value = "Số lượng";
                        range.Worksheet.Cells["D5:D5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["D5:D5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["E5:E5"].Merge = true;
                        range.Worksheet.Cells["E5:E5"].Value = "Thành tiền";
                        range.Worksheet.Cells["E5:E5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["E5:E5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["F5:F5"].Merge = true;
                        range.Worksheet.Cells["F5:F5"].Value = "Loại";
                        range.Worksheet.Cells["F5:F5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["F5:F5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }

                    var manager = new PropertyManager<DanhSachThongKeThuocTheoBacSi>(requestProperties);
                    int index = 6;


                    var stt = 1;
                    var lstLoai = datas.GroupBy(x => new { x.Loai })
                                          .Select(item => new Core.Domain.ValueObject.BaoCao.LoaiGroupVo
                                          { Loai = item.First().Loai }).OrderByDescending(p => p.Loai).ToList();
                    if (lstLoai.Any())
                    {
                        foreach (var loai in lstLoai)
                        {

                            using (var range = worksheet.Cells["A" + index + ":F" + index])
                            {
                                worksheet.Cells["A" + index + ":F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["A" + index + ":F" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                worksheet.Cells["A" + index + ":E" + index].Merge = true;
                                worksheet.Cells["A" + index + ":E" + index].Value = loai.Loai;
                                worksheet.Cells["A" + index + ":F" + index].Style.Font.Bold = true;

                                index++;

                                var lstDataTheoLoais = datas.Where(o => o.Loai == loai.Loai).ToList();
                                stt = 1;

                                if (lstDataTheoLoais.Any())
                                {
                                    foreach (var itemGroup in lstDataTheoLoais.GroupBy(d=>d.NoiTru).ToList())
                                    {
                                        foreach(var item in itemGroup)
                                        {
                                            worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                            worksheet.Cells["A" + index].Value = stt;

                                            worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["B" + index].Value = item.TenThuocHamLuong;

                                            worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["C" + index].Value = item.DonViTinh;

                                            worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["D" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["D" + index].Value = item.SoLuong;

                                            worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["E" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["E" + index].Value = item.ThanhTien;

                                            worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["F" + index].Value = item.NoiTru == true ? "Nội trú" :"Ngoại trú";

                                            index++;
                                            stt++;
                                        }
                                        if(itemGroup.Select(d=>d.NoiTru).FirstOrDefault() != null)
                                        {
                                            if(itemGroup.Select(d=>d.NoiTru).FirstOrDefault() == true)
                                            {
                                                worksheet.Cells["A" + index + ":E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["A" + index + ":E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                                worksheet.Cells["A" + index + ":E" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                                worksheet.Cells["A" + index + ":E" + index].Style.Font.Bold = true;

                                                worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                
                                                worksheet.Cells["B" + index + ":C" + index].Merge = true;

                                                worksheet.Cells["B" + index + ":C" + index].Value = $"Cộng nội trú: {itemGroup.Count()} khoản";
                                                


                                                worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["D" + index].Style.Numberformat.Format = "#,##0.00";
                                                worksheet.Cells["D" + index].Value = itemGroup.Sum(c => c.SoLuong);

                                                worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["E" + index].Style.Numberformat.Format = "#,##0.00";
                                                worksheet.Cells["E" + index].Value = itemGroup.Sum(c => c.ThanhTien);

                                                worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["F" + index].Style.Numberformat.Format = "#,##0.00";
                                                worksheet.Cells["F" + index].Value = "";
                                                index++;
                                            }
                                            if (itemGroup.Select(d => d.NoiTru).FirstOrDefault() == false)
                                            {
                                                worksheet.Cells["A" + index + ":E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["A" + index + ":E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                                worksheet.Cells["A" + index + ":E" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                                worksheet.Cells["A" + index + ":E" + index].Style.Font.Bold = true;

                                                worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                                worksheet.Cells["B" + index + ":C" + index].Merge = true;

                                                worksheet.Cells["B" + index + ":C" + index].Value = $"Cộng ngoại trú: {itemGroup.Count()} khoản";



                                                worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["D" + index].Style.Numberformat.Format = "#,##0.00";
                                                worksheet.Cells["D" + index].Value = itemGroup.Sum(c => c.SoLuong);

                                                worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["E" + index].Style.Numberformat.Format = "#,##0.00";
                                                worksheet.Cells["E" + index].Value = itemGroup.Sum(c => c.ThanhTien);

                                                worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["F" + index].Style.Numberformat.Format = "#,##0.00";
                                                worksheet.Cells["F" + index].Value = "";
                                                index++;
                                            }
                                        }
                                       
                                    }                                   
                                }

                                worksheet.Cells["A" + index + ":E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index + ":E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["A" + index + ":E" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                worksheet.Cells["A" + index + ":E" + index].Style.Font.Bold = true;

                                worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index + ":C" + index].Merge = true;
                                worksheet.Cells["A" + index + ":C" + index].Value = $"Cộng: {lstDataTheoLoais.Count} khoản";


                                worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["D" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["D" + index].Value = lstDataTheoLoais.Sum(c => c.SoLuong);

                                worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["E" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["E" + index].Value = lstDataTheoLoais.Sum(c => c.ThanhTien);

                                worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["F" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["F" + index].Value = "";

                                index++;
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
                    worksheet.Cells["D" + index].Value = datas.Sum(c => c.SoLuong);

                    worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["E" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["E" + index].Value = datas.Sum(c => c.ThanhTien);

                    worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["F" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["F" + index].Value ="";

                    index++;
                    index++;
                    index++;


                    #region ngoại trú
                    var dsSTT = datas.Where(d => d.NoiTru == false).ToList();
                    var tongSTTNgoaiTru = dsSTT.Count();

                    var dsSoLuong = datas.Where(d => d.NoiTru == false).Select(d=>d.SoLuong).ToList();
                    var tongSoLuongNgoaiTru = dsSoLuong.Sum();

                    var dsThanhTien = datas.Where(d => d.NoiTru == false).Select(d => d.ThanhTien).ToList();
                    var tongThanhTienNgoaiTru = dsThanhTien.Sum();


                    worksheet.Cells["B" + index + ":B" + (index + 2)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["B" + index + ":B" + (index + 2)].Merge = true;
                    worksheet.Cells["B" + index + ":B" + (index + 2)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["B" + index + ":B" + (index + 2)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells["B" + index + ":B" +(index + 2)].Value = $"Tổng ngoại trú:";
                    worksheet.Cells["B" + index + ":B" + (index + 2)].Style.Font.Bold = true;

                    worksheet.Cells["C" + index + ":C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["C" + index + ":C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells["C" + index + ":C" + index].Value = tongSTTNgoaiTru + " khoản";
                    

                    worksheet.Cells["C" + (index+1) + ":C" + (index +1)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["C" + (index + 1) + ":C" + (index + 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells["C" + (index + 1) + ":C" + (index + 1)].Value = "Số lượng: " + tongSoLuongNgoaiTru.ToString("#,##0.00");

                    worksheet.Cells["C" + (index + 2) + ":C" + (index + 2)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["C" + (index + 2) + ":C" + (index + 2)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells["C" + (index + 2) + ":C" + (index + 2)].Value = "Thành tiền: " + tongThanhTienNgoaiTru.ToString("#,##0.00");
                    #endregion

                    index++;
                    index++;
                    index++;

                    #region nội trú
                    var dsSTTNoiTru = datas.Where(d => d.NoiTru == true).ToList();
                    var tongSTTNoiTru = dsSTTNoiTru.Count();

                    var dsSoLuongNoiTru = datas.Where(d => d.NoiTru == true).Select(d => d.SoLuong).ToList();
                    var tongSoLuongNoiTru = dsSoLuongNoiTru.Sum();

                    var dsThanhTienNoiTru = datas.Where(d => d.NoiTru == true).Select(d => d.ThanhTien).ToList();
                    var tongThanhTienNoiTru = dsThanhTienNoiTru.Sum();

                    worksheet.Cells["B" + index + ":B" + (index + 2)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["B" + index + ":B" + (index + 2)].Merge = true;
                    worksheet.Cells["B" + index + ":B" + (index + 2)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["B" + index + ":B" + (index + 2)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells["B" + index + ":B" + (index + 2)].Value = $"Tổng nội trú:";
                    worksheet.Cells["B" + index + ":B" + (index + 2)].Style.Font.Bold = true;

                    worksheet.Cells["C" + index + ":C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["C" + index + ":C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells["C" + index + ":C" + index].Value = tongSTTNoiTru + " khoản";


                    worksheet.Cells["C" + (index + 1) + ":C" + (index + 1)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["C" + (index + 1) + ":C" + (index + 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells["C" + (index + 1) + ":C" + (index + 1)].Value = "Số lượng: " + tongSoLuongNoiTru.ToString("#,##0.00");


                    worksheet.Cells["C" + (index + 2) + ":C" + (index + 2)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["C" + (index + 2) + ":C" + (index + 2)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells["C" + (index + 2) + ":C" + (index + 2)].Value = "Thành tiền: " + tongThanhTienNoiTru.ToString("#,##0.00");
                    #endregion
                    xlPackage.Save();
                }
                return stream.ToArray();

            }
        }
        public async Task<string> ThongKeThuocTheoBacSiHTML(List<DanhSachThongKeThuocTheoBacSi> datas)
        {
            var content = string.Empty;
            content +="<!DOCTYPE html>";
            content += "<html>";
            content += "<body>";

            content += "<table width='100%' class='table table-border'>";
            content += "<thead>";
            content += "<tr>";
            content += "<td class=\"stt\"><b>STT</b></td>";
            content += "<td class=\"tenthuoc\" ><b>TÊN THUỐC - HÀM LƯỢNG</b></td>";
            content += "<td class=\"donvi\"><b>ĐƠN VỊ</b></td>";
            content += "<td class=\"soluong\"><b>SỐ LƯỢNG</b></td>";
            content += "<td class=\"thanhtien\"><b>THÀNH TIỀN</b></td>";
            content += "<td class=\"loai\"><b>LOẠI</b></td>";

            content += "</tr>";
            content += "</thead>";
            content += "<tbody>";
            #region bind data
            var lstLoai = datas.GroupBy(x => new { x.Loai })
                                          .Select(item => new Core.Domain.ValueObject.BaoCao.LoaiGroupVo
                                          { Loai = item.First().Loai }).OrderByDescending(p => p.Loai).ToList();

            var stt = 1;
            foreach (var loai in lstLoai)
            {
                content += "<tr>";
                content += "<td colspan='5'><b>" + loai.Loai + "</b></td>";
                content += "<td></td>";
                content += "</tr>";
                var lstDataTheoLoais = datas.Where(o => o.Loai == loai.Loai).ToList();
                if (lstDataTheoLoais.Any())
                {
                    foreach (var itemGroup in lstDataTheoLoais.GroupBy(d => d.NoiTru).ToList())
                    {

                        foreach (var item in itemGroup)
                        {
                            content += "<tr>";
                            content += "<td class=\"center\">" + stt + "</td>";
                            content += "<td>" + item.TenThuocHamLuong + "</td>";
                            content += "<td class=\"center\">" + item.DonViTinh + "</td>";
                            content += "<td class=\"center\">" + item.SoLuong.ToString("#,##0.00") + "</td>";
                            content += "<td class=\"center\">" + item.ThanhTien.ToString("#,##0.00") + "</td>";
                            content += "<td class=\"center\">" + item.LoaiNoiTru + "</td>";
                            content += "</tr>";
                            stt++;
                        }
                        if (itemGroup.Select(d => d.NoiTru).FirstOrDefault() != null)
                        {
                            if (itemGroup.Select(d => d.NoiTru).FirstOrDefault() == true)
                            {
                                content += "<tr>";
                                content += "<td></td>";
                                content += "<td colspan='2'>" + "<b>Cộng nội trú: " + itemGroup.Count() + " khoản</b>" + "</td>";
                                content += "<td><b>" + itemGroup.Select(d => d.SoLuong).ToList().Sum().ToString("#,##0.00") + "</b></td>";
                                content += "<td><b>" + itemGroup.Select(d => d.ThanhTien).ToList().Sum().ToString("#,##0.00") + "</b></td>";
                                content += "<td>" + "" + "</td>";
                                content += "</tr>";
                            }
                            if (itemGroup.Select(d => d.NoiTru).FirstOrDefault() == false)
                            {
                                content += "<tr>";
                                content += "<td></td>";
                                content += "<td colspan='2' >" + "<b>Cộng ngoại trú: " + itemGroup.Count() + " khoản</b>" + "</td>";
                                content += "<td><b>" + itemGroup.Select(d => d.SoLuong).ToList().Sum().ToString("#,##0.00") + "</b></td>";
                                content += "<td><b>" + itemGroup.Select(d => d.ThanhTien).ToList().Sum().ToString("#,##0.00") + "</b></td>";
                                content += "<td>" + "" + "</td>";
                                content += "</tr>";
                            }
                        }
                    }
                }

                content += "<tr>";
                content += "<td colspan='3'>" + "<b>Cộng: " + lstDataTheoLoais.Count() + " khoản</b>" + "</th>";
                content += "<td><b>" + lstDataTheoLoais.Sum(d=>d.SoLuong).ToString("#,##0.00") + "</b></td>";
                content += "<td><b>" + lstDataTheoLoais.Sum(d => d.ThanhTien).ToString("#,##0.00") + "</b></td>";
                content += "<td></td>";
                content += "</tr>";
            }
            content += "<tr>";
            content += "<td colspan='3'>" + "<b>Tổng cộng: " + datas.Count() + " khoản</b>" + "</td>";
            content += "<td><b>" + datas.Select(d=>d.SoLuong).Sum().ToString("#,##0.00") + "</b></td>";
            content += "<td><b>" + datas.Select(d => d.ThanhTien).Sum().ToString("#,##0.00") + "</b></td>";
            content += "<td></td>";
            content += "<td></td>";
            content += "</tr>";
            #endregion bind data
            content += "</tbody>";
            content += "</table>";

            content += "</body>";
            content += "</html>";


            
            return content;
        }
    }
}
