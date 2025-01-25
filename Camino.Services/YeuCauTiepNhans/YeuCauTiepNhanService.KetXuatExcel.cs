using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.CongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.DichVuGiuongBenhViens;
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.Vouchers;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.DanhSachBenhNhanChoThuNgan;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Core.Helpers;
using Camino.Services.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using RestSharp.Extensions;

namespace Camino.Services.YeuCauTiepNhans
{
    public partial class YeuCauTiepNhanService
    {
        #region Xem Trước Bảng Kê Chi Phí

        public byte[] XuatBangKeNgoaiTruCoBHYTExcel(long yeuCauTiepNhanId, bool xemTruoc = false)
        {
            var yeuCauTiepNhan = BaseRepository.TableNoTracking.Where(c => c.Id == yeuCauTiepNhanId)
                .Include(xx => xx.BenhNhan)
                .Include(xx => xx.YeuCauKhamBenhs).ThenInclude(o => o.NoiThucHien)
                .Include(xx => xx.YeuCauKhamBenhs).ThenInclude(o => o.BenhVienChuyenVien)
                .Include(xx => xx.YeuCauKhamBenhs).ThenInclude(o => o.Icdchinh)
                .Include(xx => xx.YeuCauKhamBenhs).ThenInclude(o => o.YeuCauKhamBenhICDKhacs).ThenInclude(o => o.ICD)
                .Include(cc => cc.PhuongXa)
                .Include(cc => cc.QuanHuyen)
                .Include(cc => cc.TinhThanh)
                .Include(cc => cc.NoiChuyen).FirstOrDefault();

            if (yeuCauTiepNhan == null)
                return null;

            var benhVien = new Core.Domain.Entities.BenhVien.BenhVien();

            string maBHYT = string.Empty;
            string bHYTTuNgay = string.Empty;
            string bHYTDenNgay = string.Empty;
            string mucHuong = string.Empty;
            string NoiDKKCBBanDau = string.Empty;
            string MaKCBBanDau = string.Empty;


            string maICDKemTheo = string.Empty;
            string tenICDKemTheo = string.Empty;

            var yeuCauKhamBenh = yeuCauTiepNhan.YeuCauKhamBenhs.Where(cc => cc.IcdchinhId != null).OrderBy(cc => cc.ThoiDiemHoanThanh).LastOrDefault();

            if (yeuCauKhamBenh != null)
            {
                var maKemTheos = yeuCauKhamBenh.YeuCauKhamBenhICDKhacs.Select(c => c.ICD.Ma).ToList();
                var chuGhiKemTheos = yeuCauKhamBenh.YeuCauKhamBenhICDKhacs.Select(c => c.GhiChu).ToList();

                maICDKemTheo = string.Join(", ", maKemTheos);
                tenICDKemTheo = string.Join(", ", chuGhiKemTheos);
            }

            if (yeuCauTiepNhan.CoBHYT == true)
            {
                if (!String.IsNullOrEmpty(yeuCauTiepNhan.BHYTMaDKBD))
                {
                    benhVien = _benhVienRepository.TableNoTracking.FirstOrDefault(p => p.Ma.Equals(yeuCauTiepNhan.BHYTMaDKBD));
                }
                maBHYT = yeuCauTiepNhan.BHYTMaSoThe;
                bHYTTuNgay = yeuCauTiepNhan.BHYTNgayHieuLuc?.ApplyFormatDate();
                bHYTDenNgay = yeuCauTiepNhan.BHYTNgayHetHan?.ApplyFormatDate();
                mucHuong = yeuCauTiepNhan.BHYTMucHuong.ToString() + "%";
                NoiDKKCBBanDau = benhVien != null ? benhVien?.Ten : "Chưa xác định";
                MaKCBBanDau = yeuCauTiepNhan.BHYTMaDKBD;
            }
            else
            {
                maBHYT = "";
                bHYTTuNgay = "";
                bHYTDenNgay = "";
                mucHuong = "0%";
                NoiDKKCBBanDau = "Chưa xác định";
                MaKCBBanDau = "Chưa xác định";
            }

            List<ChiPhiKhamChuaBenhVo> chiPhis;
            if (xemTruoc)
            {
                chiPhis = GetTatCaDichVuKhamChuaBenh(yeuCauTiepNhanId).Result.Where(o => o.KhongTinhPhi != true && o.DuocHuongBHYT && o.MucHuongBaoHiem > 0).ToList();
            }
            else
            {
                chiPhis = GetTatCaDichVuKhamChuaBenh(yeuCauTiepNhanId).Result.Where(o => o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && o.KhongTinhPhi != true && o.DuocHuongBHYT && o.MucHuongBaoHiem > 0).ToList();
            }

            if (chiPhis.Count == 0)
            {
                return null;
            }
            var dsChiPhiBangKe = new List<BangKeKhamBenhCoBHYTVo>();
            var dateItemChiPhis = string.Empty;


            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {

                    var worksheet = xlPackage.Workbook.Worksheets.Add("Bảng kê chi phí");

                    // set row
                    worksheet.Row(9).Height = 24.5;
                    worksheet.DefaultRowHeight = 16;

                    //SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 10;
                    worksheet.Column(2).Width = 25;
                    worksheet.Column(3).Width = 15;
                    worksheet.Column(4).Width = 15;
                    worksheet.Column(5).Width = 15;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 15;
                    worksheet.Column(8).Width = 15;
                    worksheet.Column(9).Width = 15;
                    worksheet.Column(10).Width = 15;
                    worksheet.Column(11).Width = 15;
                    worksheet.Column(12).Width = 15;
                    worksheet.Column(13).Width = 15;
                    worksheet.Column(14).Width = 15;

                    worksheet.DefaultColWidth = 7;

                    using (var range = worksheet.Cells["A1:C1"])
                    {
                        range.Worksheet.Cells["A1:C1"].Merge = true;
                        range.Worksheet.Cells["A1:C1"].Value = "CƠ SỞ KHÁM CHỮA BỆNH : BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:C1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:C1"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A1:C1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:C1"].Style.Font.Color.SetColor(Color.Black);
                    }
                   
                    using (var range = worksheet.Cells["L1:N1"])
                    {
                        range.Worksheet.Cells["L1:N1"].Merge = true;
                        range.Worksheet.Cells["L1:N1"].Value = "Mẫu số : 01/KBCB ";
                        range.Worksheet.Cells["L1:N1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        range.Worksheet.Cells["L1:N1"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["L1:N1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["L1:N1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["L1:N1"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["L2:N2"])
                    {
                        range.Worksheet.Cells["L2:N2"].Merge = true;
                        range.Worksheet.Cells["L2:N2"].Value = "Mã số người bệnh: " + yeuCauTiepNhan.BenhNhan.MaBN;
                        range.Worksheet.Cells["L2:N2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        range.Worksheet.Cells["L2:N2"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["L2:N2"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["L2:N2"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["L2:N2"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["L3:N3"])
                    {
                        range.Worksheet.Cells["L3:N3"].Merge = true;
                        range.Worksheet.Cells["L3:N3"].Value = "Số khám bệnh:" + yeuCauTiepNhan.MaYeuCauTiepNhan;
                        range.Worksheet.Cells["L3:N3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        range.Worksheet.Cells["L3:N3"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["L3:N3"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["L3:N3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["L3:N3"].Style.Font.Bold = true;
                    }



                    using (var range = worksheet.Cells["A4:N5"])
                    {
                        range.Worksheet.Cells["A4:N5"].Merge = true;
                        range.Worksheet.Cells["A4:N5"].Value = "BẢNG KÊ CHI PHÍ KHÁM BỆNH 1";
                        range.Worksheet.Cells["A4:N5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:N5"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:N5"].Style.Font.SetFromFont(new Font("Times New Roman", 18));
                        range.Worksheet.Cells["A4:N5"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:N5"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A6:N6"])
                    {
                        range.Worksheet.Cells["A6:N6"].Merge = true;
                        range.Worksheet.Cells["A6:N6"].Value = "I.Hành chính";
                        range.Worksheet.Cells["A6:N6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A6:N6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A6:N6"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A6:N6"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A6:N6"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A7:F7"])
                    {
                        range.Worksheet.Cells["A7:F7"].Merge = true;

                        range.Worksheet.Cells["A7:F7"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A7:F7"].RichText.Add("(1) Họ tên người bệnh: ");
                        var value = range.Worksheet.Cells["A7:F7"].RichText.Add(yeuCauTiepNhan.HoTen);
                        value.Bold = true;

                        range.Worksheet.Cells["A7:F7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A7:F7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A7:F7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));


                        range.Worksheet.Cells["A7:F7"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G7:J7"])
                    {
                        range.Worksheet.Cells["G7:J7"].Merge = true;
                        range.Worksheet.Cells["G7:J7"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G7:J7"].RichText.Add("Ngày,tháng,năm sinh: ");
                        var value = range.Worksheet.Cells["G7:J7"].RichText.Add(DateHelper.DOBFormat(yeuCauTiepNhan.NgaySinh, yeuCauTiepNhan.ThangSinh, yeuCauTiepNhan.NamSinh));
                        value.Bold = true;

                        range.Worksheet.Cells["G7:J7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G7:J7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G7:J7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G7:J7"].Style.Font.Color.SetColor(Color.Black);
                    }

                    var gioitinh = yeuCauTiepNhan.GioiTinh != null ? yeuCauTiepNhan.GioiTinh.GetDescription() : "";
                    using (var range = worksheet.Cells["K7:N7"])
                    {
                        range.Worksheet.Cells["K7:N7"].Merge = true;

                        range.Worksheet.Cells["K7:N7"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["K7:N7"].RichText.Add("Giới tính: ");
                        var value = range.Worksheet.Cells["K7:N7"].RichText.Add(gioitinh);
                        value.Bold = true;

                        range.Worksheet.Cells["K7:N7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["K7:N7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["K7:N7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["K7:N7"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["A8:F8"])
                    {
                        range.Worksheet.Cells["A8:F8"].Merge = true;

                        range.Worksheet.Cells["A8:F8"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A8:F8"].RichText.Add("(2)Địa chỉ hiện tại: ");
                        var value = range.Worksheet.Cells["A8:F8"].RichText.Add(yeuCauTiepNhan.DiaChiDayDu);
                        value.Bold = true;

                        range.Worksheet.Cells["A8:F8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A8:F8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A8:F8"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A8:F8"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G8:J8"])
                    {
                        range.Worksheet.Cells["G8:J8"].Merge = true;

                        range.Worksheet.Cells["G8:J8"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G8:J8"].RichText.Add("(3)Mã khu vực(K1/K2/K3): ");
                        var value = range.Worksheet.Cells["G8:J8"].RichText.Add(yeuCauTiepNhan.BHYTMaKhuVuc);
                        value.Bold = true;

                        range.Worksheet.Cells["G8:J8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G8:J8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G8:J8"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G8:J8"].Style.Font.Color.SetColor(Color.Black);
                    }


                    using (var range = worksheet.Cells["A9:F9"])
                    {
                        range.Worksheet.Cells["A9:F9"].Merge = true;
                        range.Worksheet.Cells["A9:F9"].Style.WrapText = true;

                        var title = range.Worksheet.Cells["A9:F9"].RichText.Add("(4)Mã thẻ BHYT: ");
                        var value = range.Worksheet.Cells["A9:F9"].RichText.Add(maBHYT);
                        value.Bold = true;

                        range.Worksheet.Cells["A9:F9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A9:F9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A9:F9"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A9:F9"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G9:J9"])
                    {
                        range.Worksheet.Cells["G9:J9"].Merge = true;
                        range.Worksheet.Cells["G9:J9"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G9:J9"].RichText.Add("Giá trị từ: ");
                        var valuebHYTTuNgay = range.Worksheet.Cells["G9:J9"].RichText.Add(bHYTTuNgay);
                        valuebHYTTuNgay.Bold = true;

                        var den = range.Worksheet.Cells["G9:J9"].RichText.Add(" đến: ");
                        den.Bold = false;
                        var valuebHYTDenNgay = range.Worksheet.Cells["G9:J9"].RichText.Add(bHYTDenNgay);
                        valuebHYTDenNgay.Bold = true;


                        range.Worksheet.Cells["G9:J9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G9:J9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G9:J9"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G9:J9"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["A10:F10"])
                    {
                        range.Worksheet.Cells["A10:F10"].Merge = true;

                        range.Worksheet.Cells["A10:F10"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A10:F10"].RichText.Add("(5)Nơi ĐK KCB ban đầu: ");
                        var valueNoiDKKCBBanDau = range.Worksheet.Cells["A10:F10"].RichText.Add(NoiDKKCBBanDau);
                        valueNoiDKKCBBanDau.Bold = true;

                        range.Worksheet.Cells["A10:F10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A10:F10"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A10:F10"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A10:F10"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G10:J10"])
                    {
                        range.Worksheet.Cells["G10:J10"].Merge = true;
                        range.Worksheet.Cells["G10:J10"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G10:J10"].RichText.Add("(6)Mã: ");
                        var valueMaKCBBanDau = range.Worksheet.Cells["G10:J10"].RichText.Add(MaKCBBanDau);
                        valueMaKCBBanDau.Bold = true;

                        range.Worksheet.Cells["G10:J10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G10:J10"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G10:J10"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G10:J10"].Style.Font.Color.SetColor(Color.Black);
                    }


                    using (var range = worksheet.Cells["A11:F11"])
                    {
                        range.Worksheet.Cells["A11:F11"].Merge = true;

                        range.Worksheet.Cells["A11:F11"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A11:F11"].RichText.Add("(7)Đến Khám: ");
                        var value = range.Worksheet.Cells["A11:F11"].RichText.Add(yeuCauTiepNhan.ThoiDiemTiepNhan.ApplyFormatGioPhutNgay());
                        value.Bold = true;

                        range.Worksheet.Cells["A11:F11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A11:F11"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A11:F11"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A11:F11"].Style.Font.Color.SetColor(Color.Black);
                    }


                    using (var range = worksheet.Cells["A12:F12"])
                    {
                        range.Worksheet.Cells["A12:F12"].Merge = true;

                        range.Worksheet.Cells["A12:F12"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A12:F12"].RichText.Add("(8)Điều trị ngoại trú/nội trú từ: ");
                        var value = range.Worksheet.Cells["A12:F12"].RichText.Add(string.Empty);
                        value.Bold = true;

                        range.Worksheet.Cells["A12:F12"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A12:F12"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A12:F12"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A12:F12"].Style.Font.Color.SetColor(Color.Black);
                    }


                    using (var range = worksheet.Cells["A13:F13"])
                    {
                        range.Worksheet.Cells["A13:F13"].Merge = true;
                        range.Worksheet.Cells["A13:F13"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A13:F13"].RichText.Add("(9)Kết thúc khám/điều trị: ");
                        var value = range.Worksheet.Cells["A13:F13"].RichText.Add(yeuCauKhamBenh?.ThoiDiemHoanThanh?.ApplyFormatGioPhutNgay());
                        value.Bold = true;


                        range.Worksheet.Cells["A13:F13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A13:F13"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A13:F13"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A13:F13"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G13:J13"])
                    {
                        range.Worksheet.Cells["G13:J13"].Merge = true;
                        range.Worksheet.Cells["G13:J13"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G13:J13"].RichText.Add("Tổng số ngày điều trị: ");
                        var value = range.Worksheet.Cells["G13:J13"].RichText.Add(string.Empty);
                        value.Bold = true;

                        range.Worksheet.Cells["G13:J13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G13:J13"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G13:J13"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G13:J13"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["K13:N13"])
                    {
                        range.Worksheet.Cells["K13:N13"].Merge = true;

                        range.Worksheet.Cells["K13:N13"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["K13:N13"].RichText.Add("(10)Tình trạng ra viện: ");
                        var value = range.Worksheet.Cells["K13:N13"].RichText.Add("1");
                        value.Bold = true;


                        range.Worksheet.Cells["K13:N13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["K13:N13"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["K13:N13"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["K13:N13"].Style.Font.Color.SetColor(Color.Black);
                    }

                    //=================================14=================================
                    var coCapCuu = yeuCauTiepNhan.LyDoVaoVien == Enums.EnumLyDoVaoVien.CapCuu ? "X" : string.Empty;
                    var coDungTuyen = yeuCauTiepNhan.LyDoVaoVien == Enums.EnumLyDoVaoVien.DungTuyen ? "X" : string.Empty;
                    var coThongTuyen = yeuCauTiepNhan.LyDoVaoVien == Enums.EnumLyDoVaoVien.ThongTuyen ? "X" : string.Empty;
                    var coTraiTuyen = yeuCauTiepNhan.LyDoVaoVien == Enums.EnumLyDoVaoVien.TraiTuyen ? "X" : string.Empty;

                    using (var range = worksheet.Cells["A14:C14"])
                    {
                        range.Worksheet.Cells["A14:C14"].Merge = true;

                        range.Worksheet.Cells["A14:C14"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A14:C14"].RichText.Add("(11) Cấp cứu: ");
                        var value = range.Worksheet.Cells["A14:C14"].RichText.Add(coCapCuu);
                        value.Bold = true;

                        range.Worksheet.Cells["A14:C14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A14:C14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A14:C14"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A14:C14"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["D14:F14"])
                    {
                        range.Worksheet.Cells["D14:F14"].Merge = true;
                        range.Worksheet.Cells["D14:F14"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["D14:F14"].RichText.Add("(12) Đúng tuyến: ");
                        var value = range.Worksheet.Cells["D14:F14"].RichText.Add(coDungTuyen);
                        value.Bold = true;

                        range.Worksheet.Cells["D14:F14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["D14:F14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["D14:F14"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["D14:F14"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G14:J14"])
                    {
                        range.Worksheet.Cells["G14:J14"].Merge = true;
                        range.Worksheet.Cells["G14:J14"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G14:J14"].RichText.Add("Nơi chuyến đến: ");
                        var value = range.Worksheet.Cells["G14:J14"].RichText.Add(yeuCauTiepNhan.NoiChuyen?.Ten);
                        value.Bold = true;
                        range.Worksheet.Cells["G14:J14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G14:J14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G14:J14"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G14:J14"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["K14:N14"])
                    {
                        range.Worksheet.Cells["K14:N14"].Merge = true;
                        range.Worksheet.Cells["K14:N14"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["K14:N14"].RichText.Add("Nơi chuyển đi: ");
                        var value = range.Worksheet.Cells["K14:N14"].RichText.Add(yeuCauKhamBenh?.BenhVienChuyenVien?.Ten);
                        value.Bold = true;
                        range.Worksheet.Cells["K14:N14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["K14:N14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["K14:N14"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["K14:N14"].Style.Font.Color.SetColor(Color.Black);
                    }

                    //=================================15=================================
                    using (var range = worksheet.Cells["A15:C15"])
                    {
                        range.Worksheet.Cells["A15:C15"].Merge = true;
                        range.Worksheet.Cells["A15:C15"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A15:C15"].RichText.Add("(13)Thông tuyến: ");
                        var value = range.Worksheet.Cells["A15:C15"].RichText.Add(coThongTuyen);
                        value.Bold = true;
                        range.Worksheet.Cells["A15:C15"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A15:C15"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A15:C15"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A15:C15"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["D15:F15"])
                    {
                        range.Worksheet.Cells["D15:F15"].Merge = true;
                        range.Worksheet.Cells["D15:F15"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["D15:F15"].RichText.Add("(14)Trái tuyến: ");
                        var value = range.Worksheet.Cells["D15:F15"].RichText.Add(coTraiTuyen);
                        value.Bold = true;

                        range.Worksheet.Cells["D15:F15"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["D15:F15"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["D15:F15"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["D15:F15"].Style.Font.Color.SetColor(Color.Black);
                    }


                    //================================= 16 =================================

                    using (var range = worksheet.Cells["A16:F16"])
                    {
                        range.Worksheet.Cells["A16:F16"].Merge = true;
                        range.Worksheet.Cells["A16:F16"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A16:F16"].RichText.Add("(15)Chẩn đoán xác định: ");
                        var value = range.Worksheet.Cells["A16:F16"].RichText.Add(yeuCauKhamBenh?.GhiChuICDChinh);
                        value.Bold = true;

                        range.Worksheet.Cells["A16:F16"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A16:F16"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A16:F16"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A16:F16"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G16:N16"])
                    {
                        range.Worksheet.Cells["G16:N16"].Merge = true;

                        range.Worksheet.Cells["G16:N16"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G16:N16"].RichText.Add("(16) Mã bệnh: ");
                        var value = range.Worksheet.Cells["G16:N16"].RichText.Add(yeuCauKhamBenh?.Icdchinh?.Ma);
                        value.Bold = true;


                        range.Worksheet.Cells["G16:N16"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G16:N16"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G16:N16"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G16:N16"].Style.Font.Color.SetColor(Color.Black);
                    }

                    //================================= 17 =================================
                    using (var range = worksheet.Cells["A17:F17"])
                    {
                        range.Worksheet.Cells["A17:F17"].Merge = true;
                        range.Worksheet.Cells["A17:F17"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A17:F17"].RichText.Add("(17)Bệnh kèm theo: ");
                        var value = range.Worksheet.Cells["A17:F17"].RichText.Add(tenICDKemTheo);
                        value.Bold = true;


                        range.Worksheet.Cells["A17:F17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A17:F17"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A17:F17"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A17:F17"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G17:N17"])
                    {
                        range.Worksheet.Cells["G17:N17"].Merge = true;
                        range.Worksheet.Cells["G17:N17"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G17:N17"].RichText.Add("(18)Mã bệnh kèm theo: ");
                        var value = range.Worksheet.Cells["G17:N17"].RichText.Add(maICDKemTheo);
                        value.Bold = true;


                        range.Worksheet.Cells["G17:N17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G17:N17"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G17:N17"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G17:N17"].Style.Font.Color.SetColor(Color.Black);
                    }

                    //================================= 18 =================================
                    using (var range = worksheet.Cells["A18:F18"])
                    {
                        range.Worksheet.Cells["A18:F18"].Merge = true;
                        range.Worksheet.Cells["A18:F18"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A18:F18"].RichText.Add("(19)Thời điểm đủ 05 năm liên tục từ ngày: ");
                        var value = range.Worksheet.Cells["A18:F18"].RichText.Add(yeuCauTiepNhan.BHYTNgayDu5Nam?.ApplyFormatDate());
                        value.Bold = true;


                        range.Worksheet.Cells["A18:F18"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A18:F18"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A18:F18"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A18:F18"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G18:N18"])
                    {
                        range.Worksheet.Cells["G18:N18"].Merge = true;
                        range.Worksheet.Cells["G18:N18"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G18:N18"].RichText.Add("(21)Miễn cùng chi trả trong năm từ ngày: ");
                        var value = range.Worksheet.Cells["G18:N18"].RichText.Add(yeuCauTiepNhan.BHYTNgayDuocMienCungChiTra?.ApplyFormatDate());
                        value.Bold = true;

                        range.Worksheet.Cells["G18:N18"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G18:N18"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G18:N18"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G18:N18"].Style.Font.Color.SetColor(Color.Black);
                    }

                    //================================= 19 =================================

                    using (var range = worksheet.Cells["A19:N19"])
                    {
                        range.Worksheet.Cells["A19:N19"].Merge = true;
                        range.Worksheet.Cells["A19:N19"].Value = "II. Phần Chi phí khám bệnh, chữa bệnh";
                        range.Worksheet.Cells["A19:N19"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A19:N19"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A19:N19"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A19:N19"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A19:N19"].Style.Font.Bold = true;
                    }

                    //================================= 20 =========================================
                    using (var range = worksheet.Cells["A20:C20"])
                    {
                        range.Worksheet.Cells["A20:C20"].Merge = true;
                        range.Worksheet.Cells["A20:C20"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A20:C20"].RichText.Add("Mã thẻ BHYT: ");
                        var value = range.Worksheet.Cells["A20:C20"].RichText.Add(maBHYT);
                        value.Bold = true;

                        range.Worksheet.Cells["A20:C20"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A20:C20"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A20:C20"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A20:C20"].Style.Font.Color.SetColor(Color.Black);
                    }
                    using (var range = worksheet.Cells["D20:F20"])
                    {
                        range.Worksheet.Cells["D20:F20"].Merge = true;
                        range.Worksheet.Cells["D20:F20"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["D20:F20"].RichText.Add("Giá trị từ: ");
                        var valuebHYTTuNgay = range.Worksheet.Cells["D20:F20"].RichText.Add(bHYTTuNgay);
                        valuebHYTTuNgay.Bold = true;

                        var den = range.Worksheet.Cells["D20:F20"].RichText.Add(" đến: ");
                        den.Bold = false;
                        var valuebHYTDenNgay = range.Worksheet.Cells["D20:F20"].RichText.Add(bHYTDenNgay);
                        valuebHYTDenNgay.Bold = true;

                        range.Worksheet.Cells["D20:F20"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["D20:F20"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["D20:F20"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["D20:F20"].Style.Font.Color.SetColor(Color.Black);
                    }
                    using (var range = worksheet.Cells["G20:J20"])
                    {
                        range.Worksheet.Cells["G20:J20"].Merge = true;
                        range.Worksheet.Cells["G20:J20"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G20:J20"].RichText.Add("Mức hưởng: ");
                        var value = range.Worksheet.Cells["G20:J20"].RichText.Add(mucHuong);
                        value.Bold = true;


                        range.Worksheet.Cells["G20:J20"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G20:J20"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G20:J20"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G20:J20"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["A21:N22"])
                    {
                        range.Worksheet.Cells["A21:N22"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A21:N22"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A21:N22"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A21:N22"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A21:N22"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A21:N22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A21:A22"].Merge = true;
                        range.Worksheet.Cells["A21:A22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A21:A22"].Value = "STT";

                        range.Worksheet.Cells["B21:B22"].Merge = true;
                        range.Worksheet.Cells["B21:B22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B21:B22"].Value = "Nội dung";

                        range.Worksheet.Cells["C21:C22"].Merge = true;
                        range.Worksheet.Cells["C21:C22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C21:C22"].Value = "Đơn vị tính";

                        range.Worksheet.Cells["D21:D22"].Merge = true;
                        range.Worksheet.Cells["D21:D22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D21:D22"].Value = "Số lượng";

                        range.Worksheet.Cells["E21:E22"].Merge = true;
                        range.Worksheet.Cells["E21:E22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E21:E22"].Value = "Đơn giá BV(đồng)";

                        range.Worksheet.Cells["F21:F22"].Merge = true;
                        range.Worksheet.Cells["F21:F22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F21:F22"].Value = "Đơn giá BH(đồng)";

                        range.Worksheet.Cells["G21:G22"].Merge = true;
                        range.Worksheet.Cells["G21:G22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G21:G22"].Value = "Tỉ lệ thanh toán theo dịch vụ (%)";

                        range.Worksheet.Cells["H21:H22"].Merge = true;
                        range.Worksheet.Cells["H21:H22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H21:H22"].Value = "Thành tiền BV (đồng)";

                        range.Worksheet.Cells["I21:I22"].Merge = true;
                        range.Worksheet.Cells["I21:I22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I21:I22"].Value = "Tỷ lệ thanh toán BHYT (%)";

                        range.Worksheet.Cells["J21:J22"].Merge = true;
                        range.Worksheet.Cells["J21:J22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J21:J22"].Value = "Thành tiền BH(đồng)";

                        range.Worksheet.Cells["K21:N21"].Merge = true;
                        range.Worksheet.Cells["K21:N21"].Value = "Nguồn thanh toán (đồng)";

                        range.Worksheet.Cells["K22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K22"].Value = "Quỹ BHYT";

                        range.Worksheet.Cells["L22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["L22"].Value = "Người bệnh cùng chi trả (đồng)";

                        range.Worksheet.Cells["M22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["M22"].Value = "Khác (đồng)";

                        range.Worksheet.Cells["N22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["N22"].Value = "Người bệnh tự trả(đồng)";
                    }

                    int indexItem = 1;
                    int index = 23;

                    foreach (var chiPhi in chiPhis)
                    {
                        dsChiPhiBangKe.Add(new BangKeKhamBenhCoBHYTVo
                        {
                            Nhom = chiPhi.NhomChiPhiBangKe,
                            Id = chiPhi.Id,
                            NoiDung = chiPhi.TenDichVu,
                            DonViTinh = chiPhi.DonViTinh,
                            SoLuong = (decimal)chiPhi.Soluong,
                            DuocHuongBaoHiem = chiPhi.DuocHuongBHYT,
                            DonGiaBH = chiPhi.DonGiaBHYT,
                            DonGiaTheoBV = chiPhi.DonGia,
                            MucHuongBaoHiem = chiPhi.MucHuongBaoHiem,
                            TiLeThanhToanTheoDV = chiPhi.TiLeBaoHiemThanhToan,
                            BaoHiemChiTra = true
                        });
                    }
                    var groupChiPhiBangKes = dsChiPhiBangKe.GroupBy(x => x.Nhom).OrderBy(o => (int)o.Key * (o.Key == NhomChiPhiBangKe.NgayGiuongDieuTriBanNgay || o.Key == NhomChiPhiBangKe.NgayGiuongDieuTriNoiTru ? 1 : 10));
                    foreach (var groupChiPhiBangKe in groupChiPhiBangKes)
                    {
                        if (groupChiPhiBangKe.Key == NhomChiPhiBangKe.NgayGiuongDieuTriBanNgay)
                        {
                            worksheet.Cells["A" + index + ":H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["A" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + index + ":H" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            worksheet.Cells["A" + index + ":H" + index].Merge = true;
                            worksheet.Cells["A" + index + ":H" + index].Value = "2. Ngày giường:";
                            worksheet.Cells["A" + index + ":H" + index].Style.Font.Bold = true;

                            index++;

                            worksheet.Cells["A" + index + ":G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["A" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            worksheet.Cells["A" + index + ":G" + index].Merge = true;
                            worksheet.Cells["A" + index + ":G" + index].Value = "2.1." + groupChiPhiBangKe.Key.GetDescription();
                            worksheet.Cells["A" + index + ":G" + index].Style.Font.Bold = true;

                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["H" + index].Style.Font.Bold = true;
                            worksheet.Cells["H" + index].Value = groupChiPhiBangKe.Sum(o => o.ThanhTienBV);

                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["J" + index].Style.Font.Bold = true;
                            worksheet.Cells["J" + index].Value = groupChiPhiBangKe.Sum(o => o.ThanhTienBH);

                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["K" + index].Style.Font.Bold = true;
                            worksheet.Cells["K" + index].Value = groupChiPhiBangKe.Sum(o => o.QuyBHYT);

                            worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["L" + index].Style.Font.Bold = true;
                            worksheet.Cells["L" + index].Value = groupChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra);

                            worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["M" + index].Style.Font.Bold = true;
                            worksheet.Cells["M" + index].Value = groupChiPhiBangKe.Sum(o => o.Khac);

                            worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["N" + index].Style.Font.Bold = true;
                            worksheet.Cells["N" + index].Value = groupChiPhiBangKe.Sum(o => o.NguoiBenhTuTra);

                            index++;

                        }
                        else if (groupChiPhiBangKe.Key == NhomChiPhiBangKe.NgayGiuongDieuTriNoiTru)
                        {
                            if (!groupChiPhiBangKes.Any(o => o.Key == NhomChiPhiBangKe.NgayGiuongDieuTriBanNgay))
                            {
                                worksheet.Cells["A" + index + ":H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["A" + index + ":H" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                worksheet.Cells["A" + index + ":H" + index].Merge = true;
                                worksheet.Cells["A" + index + ":H" + index].Value = "2. Ngày giường:";
                                worksheet.Cells["A" + index + ":H" + index].Style.Font.Bold = true;
                            }

                            worksheet.Cells["A" + index + ":G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["A" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            worksheet.Cells["A" + index + ":G" + index].Merge = true;
                            worksheet.Cells["A" + index + ":G" + index].Value = "2.2." + groupChiPhiBangKe.Key.GetDescription();
                            worksheet.Cells["A" + index + ":G" + index].Style.Font.Bold = true;

                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["H" + index].Style.Font.Bold = true;
                            worksheet.Cells["H" + index].Value = groupChiPhiBangKe.Sum(o => o.ThanhTienBV);

                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["J" + index].Style.Font.Bold = true;
                            worksheet.Cells["J" + index].Value = groupChiPhiBangKe.Sum(o => o.ThanhTienBH);

                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["K" + index].Style.Font.Bold = true;
                            worksheet.Cells["K" + index].Value = groupChiPhiBangKe.Sum(o => o.QuyBHYT);

                            worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["L" + index].Style.Font.Bold = true;
                            worksheet.Cells["L" + index].Value = groupChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra);

                            worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["M" + index].Style.Font.Bold = true;
                            worksheet.Cells["M" + index].Value = groupChiPhiBangKe.Sum(o => o.Khac);

                            worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["N" + index].Style.Font.Bold = true;
                            worksheet.Cells["N" + index].Value = groupChiPhiBangKe.Sum(o => o.NguoiBenhTuTra);

                            index++;

                        }
                        else
                        {
                            worksheet.Cells["A" + index + ":G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["A" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            worksheet.Cells["A" + index + ":G" + index].Merge = true;
                            worksheet.Cells["A" + index + ":G" + index].Value = (int)groupChiPhiBangKe.Key + "." + groupChiPhiBangKe.Key.GetDescription();
                            worksheet.Cells["A" + index + ":G" + index].Style.Font.Bold = true;

                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["H" + index].Style.Font.Bold = true;
                            worksheet.Cells["H" + index].Value = groupChiPhiBangKe.Sum(o => o.ThanhTienBV);

                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["J" + index].Style.Font.Bold = true;
                            worksheet.Cells["J" + index].Value = groupChiPhiBangKe.Sum(o => o.ThanhTienBH);

                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["K" + index].Style.Font.Bold = true;
                            worksheet.Cells["K" + index].Value = groupChiPhiBangKe.Sum(o => o.QuyBHYT);

                            worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["L" + index].Style.Font.Bold = true;
                            worksheet.Cells["L" + index].Value = groupChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra);

                            worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["M" + index].Style.Font.Bold = true;
                            worksheet.Cells["M" + index].Value = groupChiPhiBangKe.Sum(o => o.Khac);

                            worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["N" + index].Style.Font.Bold = true;
                            worksheet.Cells["N" + index].Value = groupChiPhiBangKe.Sum(o => o.NguoiBenhTuTra);

                            index++;
                        }
                        if (groupChiPhiBangKe.Key == NhomChiPhiBangKe.GoiVatTu)
                        {
                            var groupGoiVatTus = groupChiPhiBangKe.ToList().GroupBy(o => o.SoGoiVatTu.GetValueOrDefault()).OrderBy(o => o.Key);
                            int sttGoiVatTu = 1;
                            foreach (var groupGoiVatTu in groupGoiVatTus)
                            {
                                worksheet.Cells["A" + index + ":G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["A" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                worksheet.Cells["A" + index + ":G" + index].Value = "10." + sttGoiVatTu + ". " + groupChiPhiBangKe.Key.GetDescription() + " " + sttGoiVatTu;
                                worksheet.Cells["A" + index + ":G" + index].Style.Font.Bold = true;
                                worksheet.Cells["A" + index + ":G" + index].Merge = true;

                                worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["H" + index].Style.Font.Bold = true;
                                worksheet.Cells["H" + index].Value = groupChiPhiBangKe.Sum(o => o.ThanhTienBV);

                                worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["J" + index].Style.Font.Bold = true;
                                worksheet.Cells["J" + index].Value = groupChiPhiBangKe.Sum(o => o.ThanhTienBH);

                                worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["K" + index].Style.Font.Bold = true;
                                worksheet.Cells["K" + index].Value = groupChiPhiBangKe.Sum(o => o.QuyBHYT);

                                worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["L" + index].Style.Font.Bold = true;
                                worksheet.Cells["L" + index].Value = groupChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra);

                                worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["M" + index].Style.Font.Bold = true;
                                worksheet.Cells["M" + index].Value = groupChiPhiBangKe.Sum(o => o.Khac);

                                worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["N" + index].Style.Font.Bold = true;
                                worksheet.Cells["N" + index].Value = groupChiPhiBangKe.Sum(o => o.NguoiBenhTuTra);


                                foreach (var chiPhiBangKe in groupGoiVatTu)
                                {
                                    worksheet.Cells["A" + index + ":N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["A" + index + ":N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["A" + index + ":N" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));

                                    worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["A" + index].Value = indexItem;

                                    worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["B" + index].Value = chiPhiBangKe.NoiDung;

                                    worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["C" + index].Value = chiPhiBangKe.DonViTinh;

                                    worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["D" + index].Value = chiPhiBangKe.SoLuong;

                                    worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["E" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["E" + index].Value = chiPhiBangKe.DonGiaBV;

                                    worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["F" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["F" + index].Value = chiPhiBangKe.DonGiaBH;

                                    worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["G" + index].Value = chiPhiBangKe.TiLeThanhToanTheoDV;

                                    worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["H" + index].Value = chiPhiBangKe.ThanhTienBV;

                                    worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["I" + index].Value = Convert.ToDouble(chiPhiBangKe.MucHuongBaoHiem > 0 ? 100 : 0).ApplyFormatMoneyToDouble();


                                    worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["J" + index].Value = chiPhiBangKe.ThanhTienBH;


                                    worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["K" + index].Value = chiPhiBangKe.QuyBHYT;


                                    worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["L" + index].Value = chiPhiBangKe.NguoiBenhCungChiTra;


                                    worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["M" + index].Value = chiPhiBangKe.Khac;


                                    worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["N" + index].Value = chiPhiBangKe.NguoiBenhTuTra;

                                    indexItem++;
                                    index++;
                                }
                            }
                        }
                        else
                        {
                            foreach (var chiPhiBangKe in groupChiPhiBangKe)
                            {
                                worksheet.Cells["A" + index + ":N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index + ":N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["A" + index + ":N" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));

                                worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["A" + index].Value = indexItem;

                                worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["B" + index].Value = chiPhiBangKe.NoiDung;

                                worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["C" + index].Value = chiPhiBangKe.DonViTinh;

                                worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["D" + index].Value = chiPhiBangKe.SoLuong;

                                worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["E" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["E" + index].Value = chiPhiBangKe.DonGiaBV;

                                worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["F" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["F" + index].Value = chiPhiBangKe.DonGiaBH;

                                worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["G" + index].Value = chiPhiBangKe.TiLeThanhToanTheoDV;

                                worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["H" + index].Value = chiPhiBangKe.ThanhTienBV;

                                worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["I" + index].Value = Convert.ToDouble(chiPhiBangKe.MucHuongBaoHiem > 0 ? 100 : 0).ApplyFormatMoneyToDouble();


                                worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["J" + index].Value = chiPhiBangKe.ThanhTienBH;


                                worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["K" + index].Value = chiPhiBangKe.QuyBHYT;


                                worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["L" + index].Value = chiPhiBangKe.NguoiBenhCungChiTra;


                                worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["M" + index].Value = chiPhiBangKe.Khac;


                                worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["N" + index].Value = chiPhiBangKe.NguoiBenhTuTra;

                                indexItem++;
                                index++;
                            }
                        }
                    }

                    worksheet.Cells["A" + index + ":G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["A" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells["A" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                    worksheet.Cells["A" + index + ":G" + index].Merge = true;
                    worksheet.Cells["A" + index + ":G" + index].Value = "Tổng cộng";
                    worksheet.Cells["A" + index + ":G" + index].Style.Font.Bold = true;

                    worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["H" + index].Value = groupChiPhiBangKes.Select(c => c.Sum(o => o.ThanhTienBV)).Sum();

                    worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["J" + index].Value = groupChiPhiBangKes.Select(c => c.Sum(o => o.ThanhTienBH)).Sum();

                    worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["K" + index].Value = groupChiPhiBangKes.Select(c => c.Sum(o => o.QuyBHYT)).Sum();

                    worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["L" + index].Value = groupChiPhiBangKes.Select(c => c.Sum(o => o.NguoiBenhCungChiTra)).Sum();

                    worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["M" + index].Value = groupChiPhiBangKes.Select(c => c.Sum(o => o.Khac)).Sum();

                    worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["N" + index].Value = groupChiPhiBangKes.Select(c => c.Sum(o => o.NguoiBenhTuTra)).Sum();

                    index++;

                    worksheet.Cells["A" + index + ":N" + index].Merge = true;
                    worksheet.Cells["A" + index + ":N" + index].Value = "Tổng chi phí lần khám bệnh/cả đợt điều trị: " + Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.ThanhTienBH)).ApplyFormatMoneyToDouble() + " đồng";
                    worksheet.Cells["A" + index + ":N" + index].Style.Font.Bold = true;

                    index++;

                    worksheet.Cells["A" + index + ":N" + index].Merge = true;
                    worksheet.Cells["A" + index + ":N" + index].Value = "(Viết bằng chữ: " + NumberHelper.ChuyenSoRaText(Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.ThanhTienBH))) + ")";

                    index++;

                    worksheet.Cells["A" + index + ":N" + index].Merge = true;
                    worksheet.Cells["A" + index + ":N" + index].Value = "Trong đó , số tiền do:";
                    worksheet.Cells["A" + index + ":N" + index].Style.Font.Bold = true;

                    index++;
                    worksheet.Cells["A" + index + ":N" + index].Merge = true;
                    worksheet.Cells["A" + index + ":N" + index].Value = "- Quỹ BHYT thanh toán: " + Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.QuyBHYT)).ApplyFormatMoneyToDouble();

                    index++;
                    worksheet.Cells["A" + index + ":N" + index].Merge = true;
                    worksheet.Cells["A" + index + ":N" + index].Value = "- Người bệnh trả trong đó: " + (Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra)) + Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.NguoiBenhTuTra))).ApplyFormatMoneyToDouble();

                    index++;
                    worksheet.Cells["A" + index + ":N" + index].Merge = true;
                    worksheet.Cells["A" + index + ":N" + index].Value = "+ Cùng chi trả trong phạm vi BHYT: " + Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra)).ApplyFormatMoneyToDouble(true);

                    index++;
                    worksheet.Cells["A" + index + ":N" + index].Merge = true;
                    worksheet.Cells["A" + index + ":N" + index].Value = "+ Các khoản phải trả khác: " + Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.NguoiBenhTuTra)).ApplyFormatMoneyToDouble(true);

                    index++;
                    worksheet.Cells["A" + index + ":N" + index].Merge = true;
                    worksheet.Cells["A" + index + ":N" + index].Value = "+ Nguồn khác: " + Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.Khac)).ApplyFormatMoneyToDouble(true);


                    index = index + 2;

                    worksheet.Cells["D" + index + ":F" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                    worksheet.Cells["D" + index + ":F" + index].Merge = true;
                    worksheet.Cells["D" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["D" + index + ":F" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    worksheet.Cells["D" + index + ":F" + index].Value = $"Ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}";

                    worksheet.Cells["G" + index + ":I" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                    worksheet.Cells["G" + index + ":I" + index].Merge = true;
                    worksheet.Cells["G" + index + ":I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["G" + index + ":I" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    worksheet.Cells["G" + index + ":I" + index].Value = $"Ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}";

                    worksheet.Cells["J" + index + ":N" + index].Style.Font.Bold = true;
                    worksheet.Cells["J" + index + ":N" + index].Merge = true;
                    worksheet.Cells["J" + index + ":N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["J" + index + ":N" + index].Value = "XÁC NHẬN CỦA NGƯỜI BỆNH ";

                    index++;

                    worksheet.Cells["A" + index + ":C" + index].Style.Font.Bold = true;
                    worksheet.Cells["A" + index + ":C" + index].Merge = true;
                    worksheet.Cells["A" + index + ":C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A" + index + ":C" + index].Value = "NGƯỜI LẬP BẢNG KÊ";

                    worksheet.Cells["D" + index + ":F" + index].Style.Font.Bold = true;
                    worksheet.Cells["D" + index + ":F" + index].Merge = true;
                    worksheet.Cells["D" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["D" + index + ":F" + index].Value = "KẾ TOÁN VIỆN PHÍ";

                    worksheet.Cells["G" + index + ":I" + index].Style.Font.Bold = true;
                    worksheet.Cells["G" + index + ":I" + index].Merge = true;
                    worksheet.Cells["G" + index + ":I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["G" + index + ":I" + index].Value = "GIÁM ĐỊNH BHYT";

                    worksheet.Cells["J" + index + ":N" + index].Style.Font.Bold = true;
                    worksheet.Cells["J" + index + ":N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["J" + index + ":N" + index].Merge = true;
                    worksheet.Cells["J" + index + ":N" + index].Value = "(Ký , Ghi rõ họ tên)";

                    index++;

                    worksheet.Cells["A" + index + ":C" + index].Style.Font.Bold = true;
                    worksheet.Cells["A" + index + ":C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A" + index + ":C" + index].Merge = true;
                    worksheet.Cells["A" + index + ":C" + index].Value = "(Ký , Ghi rõ họ tên)";


                    worksheet.Cells["D" + index + ":F" + index].Style.Font.Bold = true;
                    worksheet.Cells["D" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["D" + index + ":F" + index].Merge = true;
                    worksheet.Cells["D" + index + ":F" + index].Value = "(Ký , Ghi rõ họ tên)";

                    worksheet.Cells["G" + index + ":I" + index].Style.Font.Bold = true;
                    worksheet.Cells["G" + index + ":I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["G" + index + ":I" + index].Merge = true;
                    worksheet.Cells["G" + index + ":I" + index].Value = "(Ký , Ghi rõ họ tên)";

                    worksheet.Cells["J" + index + ":N" + index].Style.Font.Bold = true;
                    worksheet.Cells["J" + index + ":N" + index].Merge = true;
                    worksheet.Cells["J" + index + ":N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["J" + index + ":N" + index].Value = "(Tôi đã nhận...phim...Xquang / CT / MRI)";

                    xlPackage.Save();
                }

                return stream.ToArray();
            }


        }

        public byte[] XuatBangKeNgoaiTruExcel(long yeuCauTiepNhanId, bool xemTruoc = false)
        {
            var yeuCauTiepNhan = BaseRepository.TableNoTracking.Where(c => c.Id == yeuCauTiepNhanId)
                .Include(xx => xx.BenhNhan)
                .Include(xx => xx.YeuCauKhamBenhs).ThenInclude(o => o.NoiThucHien)
                .Include(xx => xx.YeuCauKhamBenhs).ThenInclude(o => o.BenhVienChuyenVien)
                .Include(xx => xx.YeuCauKhamBenhs).ThenInclude(o => o.Icdchinh)
                .Include(xx => xx.YeuCauKhamBenhs).ThenInclude(o => o.YeuCauKhamBenhICDKhacs).ThenInclude(o => o.ICD)
                .Include(cc => cc.PhuongXa)
                .Include(cc => cc.QuanHuyen)
                .Include(cc => cc.TinhThanh)
                .Include(cc => cc.NoiChuyen).FirstOrDefault();
                      
            if (yeuCauTiepNhan == null)
                return null;

            var benhVien = new Core.Domain.Entities.BenhVien.BenhVien();

            string maBHYT = string.Empty;
            string bHYTTuNgay = string.Empty;
            string bHYTDenNgay = string.Empty;
            string mucHuong = string.Empty;
            string NoiDKKCBBanDau = string.Empty;
            string MaKCBBanDau = string.Empty;


            string maICDKemTheo = string.Empty;
            string tenICDKemTheo = string.Empty;

            var yeuCauKhamBenh = yeuCauTiepNhan.YeuCauKhamBenhs.Where(cc => cc.IcdchinhId != null).OrderBy(cc => cc.ThoiDiemHoanThanh).LastOrDefault();

            if (yeuCauKhamBenh != null)
            {
                var maKemTheos = yeuCauKhamBenh.YeuCauKhamBenhICDKhacs.Select(c => c.ICD.Ma).ToList();
                var chuGhiKemTheos = yeuCauKhamBenh.YeuCauKhamBenhICDKhacs.Select(c => c.GhiChu).ToList();

                maICDKemTheo = string.Join(", ", maKemTheos);
                tenICDKemTheo = string.Join(", ", chuGhiKemTheos);
            }

            if (yeuCauTiepNhan.CoBHYT == true)
            {
                if (!String.IsNullOrEmpty(yeuCauTiepNhan.BHYTMaDKBD))
                {
                    benhVien = _benhVienRepository.TableNoTracking.FirstOrDefault(p => p.Ma.Equals(yeuCauTiepNhan.BHYTMaDKBD));
                }
                maBHYT = yeuCauTiepNhan.BHYTMaSoThe;
                bHYTTuNgay = yeuCauTiepNhan.BHYTNgayHieuLuc?.ApplyFormatDate();
                bHYTDenNgay = yeuCauTiepNhan.BHYTNgayHetHan?.ApplyFormatDate();
                mucHuong = yeuCauTiepNhan.BHYTMucHuong.ToString() + "%";
                NoiDKKCBBanDau = benhVien != null ? benhVien?.Ten : "Chưa xác định";
                MaKCBBanDau = yeuCauTiepNhan.BHYTMaDKBD;
            }
            else
            {
                maBHYT = "";
                bHYTTuNgay = "";
                bHYTDenNgay = "";
                mucHuong = "0%";
                NoiDKKCBBanDau = "Chưa xác định";
                MaKCBBanDau = "Chưa xác định";
            }
            List<ChiPhiKhamChuaBenhVo> chiPhis;
            if (xemTruoc)
            {
                chiPhis = GetTatCaDichVuKhamChuaBenh(yeuCauTiepNhanId).Result.Where(o => o.YeuCauGoiDichVuId == null).ToList();
            }
            else
            {
                chiPhis = GetTatCaDichVuKhamChuaBenh(yeuCauTiepNhanId).Result.Where(o => o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && o.YeuCauGoiDichVuId == null).ToList();
            }

            if (chiPhis.Count == 0)
            {
                return null;
            }

            var dsChiPhiBangKe = new List<BangKeKhamBenhBenhVienVo>();
            var dateItemChiPhis = string.Empty;         
                                                  
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {

                    var worksheet = xlPackage.Workbook.Worksheets.Add("Bảng kê chi phí");

                    // set row
                    worksheet.Row(9).Height = 24.5;
                    worksheet.DefaultRowHeight = 16;

                    //SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 10;
                    worksheet.Column(2).Width = 25;
                    worksheet.Column(3).Width = 15;
                    worksheet.Column(4).Width = 15;
                    worksheet.Column(5).Width = 15;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 15;
                    worksheet.Column(8).Width = 15;
                    worksheet.Column(9).Width = 15;
                    worksheet.Column(10).Width = 15;
                    worksheet.Column(11).Width = 15;
                    worksheet.Column(12).Width = 15;
                    worksheet.Column(13).Width = 15;
                    worksheet.Column(14).Width = 15;

                    worksheet.DefaultColWidth = 7;

                    using (var range = worksheet.Cells["A1:C1"])
                    {
                        range.Worksheet.Cells["A1:C1"].Merge = true;
                        range.Worksheet.Cells["A1:C1"].Value = "CƠ SỞ KHÁM CHỮA BỆNH : BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:C1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:C1"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A1:C1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:C1"].Style.Font.Color.SetColor(Color.Black);
                    }

                    //using (var range = worksheet.Cells["A2:C2"])
                    //{
                    //    range.Worksheet.Cells["A2:C2"].Merge = true;
                    //    range.Worksheet.Cells["A2:C2"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                    //    range.Worksheet.Cells["A2:C2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    //    range.Worksheet.Cells["A2:C2"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    //    range.Worksheet.Cells["A2:C2"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    //    range.Worksheet.Cells["A2:C2"].Style.Font.Color.SetColor(Color.Black);
                    //    range.Worksheet.Cells["A2:C2"].Style.Font.Bold = true;
                    //}

                    using (var range = worksheet.Cells["L1:N1"])
                    {
                        range.Worksheet.Cells["L1:N1"].Merge = true;
                        range.Worksheet.Cells["L1:N1"].Value = "Mẫu số : 01/KBCB ";
                        range.Worksheet.Cells["L1:N1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        range.Worksheet.Cells["L1:N1"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["L1:N1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["L1:N1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["L1:N1"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["L2:N2"])
                    {
                        range.Worksheet.Cells["L2:N2"].Merge = true;
                        range.Worksheet.Cells["L2:N2"].Value = "Mã số người bệnh: " + yeuCauTiepNhan.BenhNhan.MaBN;
                        range.Worksheet.Cells["L2:N2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        range.Worksheet.Cells["L2:N2"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["L2:N2"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["L2:N2"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["L2:N2"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["L3:N3"])
                    {
                        range.Worksheet.Cells["L3:N3"].Merge = true;
                        range.Worksheet.Cells["L3:N3"].Value = "Số khám bệnh:" + yeuCauTiepNhan.MaYeuCauTiepNhan;
                        range.Worksheet.Cells["L3:N3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        range.Worksheet.Cells["L3:N3"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["L3:N3"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["L3:N3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["L3:N3"].Style.Font.Bold = true;
                    }



                    using (var range = worksheet.Cells["A4:N5"])
                    {
                        range.Worksheet.Cells["A4:N5"].Merge = true;
                        range.Worksheet.Cells["A4:N5"].Value = "BẢNG KÊ CHI PHÍ KHÁM BỆNH 1";
                        range.Worksheet.Cells["A4:N5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:N5"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:N5"].Style.Font.SetFromFont(new Font("Times New Roman", 18));
                        range.Worksheet.Cells["A4:N5"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:N5"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A6:N6"])
                    {
                        range.Worksheet.Cells["A6:N6"].Merge = true;
                        range.Worksheet.Cells["A6:N6"].Value = "I.Hành chính";
                        range.Worksheet.Cells["A6:N6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A6:N6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A6:N6"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A6:N6"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A6:N6"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A7:F7"])
                    {
                        range.Worksheet.Cells["A7:F7"].Merge = true;

                        range.Worksheet.Cells["A7:F7"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A7:F7"].RichText.Add("(1) Họ tên người bệnh: ");
                        var value = range.Worksheet.Cells["A7:F7"].RichText.Add(yeuCauTiepNhan.HoTen);
                        value.Bold = true;

                        range.Worksheet.Cells["A7:F7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A7:F7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A7:F7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));


                        range.Worksheet.Cells["A7:F7"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G7:J7"])
                    {
                        range.Worksheet.Cells["G7:J7"].Merge = true;
                        range.Worksheet.Cells["G7:J7"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G7:J7"].RichText.Add("Ngày,tháng,năm sinh: ");
                        var value = range.Worksheet.Cells["G7:J7"].RichText.Add(DateHelper.DOBFormat(yeuCauTiepNhan.NgaySinh, yeuCauTiepNhan.ThangSinh, yeuCauTiepNhan.NamSinh));
                        value.Bold = true;

                        range.Worksheet.Cells["G7:J7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G7:J7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G7:J7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G7:J7"].Style.Font.Color.SetColor(Color.Black);
                    }

                    var gioitinh = yeuCauTiepNhan.GioiTinh != null ? yeuCauTiepNhan.GioiTinh.GetDescription() : "";
                    using (var range = worksheet.Cells["K7:N7"])
                    {
                        range.Worksheet.Cells["K7:N7"].Merge = true;

                        range.Worksheet.Cells["K7:N7"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["K7:N7"].RichText.Add("Giới tính: ");
                        var value = range.Worksheet.Cells["K7:N7"].RichText.Add(gioitinh);
                        value.Bold = true;

                        range.Worksheet.Cells["K7:N7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["K7:N7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["K7:N7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["K7:N7"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["A8:F8"])
                    {
                        range.Worksheet.Cells["A8:F8"].Merge = true;

                        range.Worksheet.Cells["A8:F8"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A8:F8"].RichText.Add("(2)Địa chỉ hiện tại: ");
                        var value = range.Worksheet.Cells["A8:F8"].RichText.Add(yeuCauTiepNhan.DiaChiDayDu);
                        value.Bold = true;

                        range.Worksheet.Cells["A8:F8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A8:F8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A8:F8"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A8:F8"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G8:J8"])
                    {
                        range.Worksheet.Cells["G8:J8"].Merge = true;

                        range.Worksheet.Cells["G8:J8"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G8:J8"].RichText.Add("(3)Mã khu vực(K1/K2/K3): ");
                        var value = range.Worksheet.Cells["G8:J8"].RichText.Add(yeuCauTiepNhan.BHYTMaKhuVuc);
                        value.Bold = true;

                        range.Worksheet.Cells["G8:J8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G8:J8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G8:J8"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G8:J8"].Style.Font.Color.SetColor(Color.Black);
                    }


                    using (var range = worksheet.Cells["A9:F9"])
                    {
                        range.Worksheet.Cells["A9:F9"].Merge = true;
                        range.Worksheet.Cells["A9:F9"].Style.WrapText = true;

                        var title = range.Worksheet.Cells["A9:F9"].RichText.Add("(4)Mã thẻ BHYT: ");
                        var value = range.Worksheet.Cells["A9:F9"].RichText.Add(maBHYT);
                        value.Bold = true;

                        range.Worksheet.Cells["A9:F9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A9:F9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A9:F9"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A9:F9"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G9:J9"])
                    {
                        range.Worksheet.Cells["G9:J9"].Merge = true;
                        range.Worksheet.Cells["G9:J9"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G9:J9"].RichText.Add("Giá trị từ: ");
                        var valuebHYTTuNgay = range.Worksheet.Cells["G9:J9"].RichText.Add(bHYTTuNgay);
                        valuebHYTTuNgay.Bold = true;

                        var den = range.Worksheet.Cells["G9:J9"].RichText.Add(" đến: ");
                        den.Bold = false;
                        var valuebHYTDenNgay = range.Worksheet.Cells["G9:J9"].RichText.Add(bHYTDenNgay);
                        valuebHYTDenNgay.Bold = true;


                        range.Worksheet.Cells["G9:J9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G9:J9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G9:J9"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G9:J9"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["A10:F10"])
                    {
                        range.Worksheet.Cells["A10:F10"].Merge = true;

                        range.Worksheet.Cells["A10:F10"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A10:F10"].RichText.Add("(5)Nơi ĐK KCB ban đầu: ");
                        var valueNoiDKKCBBanDau = range.Worksheet.Cells["A10:F10"].RichText.Add(NoiDKKCBBanDau);
                        valueNoiDKKCBBanDau.Bold = true;

                        range.Worksheet.Cells["A10:F10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A10:F10"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A10:F10"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A10:F10"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G10:J10"])
                    {
                        range.Worksheet.Cells["G10:J10"].Merge = true;
                        range.Worksheet.Cells["G10:J10"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G10:J10"].RichText.Add("(6)Mã: ");
                        var valueMaKCBBanDau = range.Worksheet.Cells["G10:J10"].RichText.Add(MaKCBBanDau);
                        valueMaKCBBanDau.Bold = true;

                        range.Worksheet.Cells["G10:J10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G10:J10"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G10:J10"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G10:J10"].Style.Font.Color.SetColor(Color.Black);
                    }


                    using (var range = worksheet.Cells["A11:F11"])
                    {
                        range.Worksheet.Cells["A11:F11"].Merge = true;

                        range.Worksheet.Cells["A11:F11"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A11:F11"].RichText.Add("(7)Đến Khám: ");
                        var value = range.Worksheet.Cells["A11:F11"].RichText.Add(yeuCauTiepNhan.ThoiDiemTiepNhan.ApplyFormatGioPhutNgay());
                        value.Bold = true;

                        range.Worksheet.Cells["A11:F11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A11:F11"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A11:F11"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A11:F11"].Style.Font.Color.SetColor(Color.Black);
                    }


                    using (var range = worksheet.Cells["A12:F12"])
                    {
                        range.Worksheet.Cells["A12:F12"].Merge = true;

                        range.Worksheet.Cells["A12:F12"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A12:F12"].RichText.Add("(8)Điều trị ngoại trú/nội trú từ: ");
                        var value = range.Worksheet.Cells["A12:F12"].RichText.Add(string.Empty);
                        value.Bold = true;

                        range.Worksheet.Cells["A12:F12"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A12:F12"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A12:F12"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A12:F12"].Style.Font.Color.SetColor(Color.Black);
                    }


                    using (var range = worksheet.Cells["A13:F13"])
                    {
                        range.Worksheet.Cells["A13:F13"].Merge = true;
                        range.Worksheet.Cells["A13:F13"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A13:F13"].RichText.Add("(9)Kết thúc khám/điều trị: ");
                        var value = range.Worksheet.Cells["A13:F13"].RichText.Add(yeuCauKhamBenh?.ThoiDiemHoanThanh?.ApplyFormatGioPhutNgay());
                        value.Bold = true;


                        range.Worksheet.Cells["A13:F13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A13:F13"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A13:F13"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A13:F13"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G13:J13"])
                    {
                        range.Worksheet.Cells["G13:J13"].Merge = true;
                        range.Worksheet.Cells["G13:J13"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G13:J13"].RichText.Add("Tổng số ngày điều trị: ");
                        var value = range.Worksheet.Cells["G13:J13"].RichText.Add(string.Empty);
                        value.Bold = true;

                        range.Worksheet.Cells["G13:J13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G13:J13"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G13:J13"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G13:J13"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["K13:N13"])
                    {
                        range.Worksheet.Cells["K13:N13"].Merge = true;

                        range.Worksheet.Cells["K13:N13"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["K13:N13"].RichText.Add("(10)Tình trạng ra viện: ");
                        var value = range.Worksheet.Cells["K13:N13"].RichText.Add("1");
                        value.Bold = true;


                        range.Worksheet.Cells["K13:N13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["K13:N13"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["K13:N13"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["K13:N13"].Style.Font.Color.SetColor(Color.Black);
                    }

                    //=================================14=================================
                    var coCapCuu = yeuCauTiepNhan.LyDoVaoVien == Enums.EnumLyDoVaoVien.CapCuu ? "X" : string.Empty;
                    var coDungTuyen = yeuCauTiepNhan.LyDoVaoVien == Enums.EnumLyDoVaoVien.DungTuyen ? "X" : string.Empty;
                    var coThongTuyen = yeuCauTiepNhan.LyDoVaoVien == Enums.EnumLyDoVaoVien.ThongTuyen ? "X" : string.Empty;
                    var coTraiTuyen = yeuCauTiepNhan.LyDoVaoVien == Enums.EnumLyDoVaoVien.TraiTuyen ? "X" : string.Empty;

                    using (var range = worksheet.Cells["A14:C14"])
                    {
                        range.Worksheet.Cells["A14:C14"].Merge = true;

                        range.Worksheet.Cells["A14:C14"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A14:C14"].RichText.Add("(11) Cấp cứu: ");
                        var value = range.Worksheet.Cells["A14:C14"].RichText.Add(coCapCuu);
                        value.Bold = true;

                        range.Worksheet.Cells["A14:C14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A14:C14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A14:C14"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A14:C14"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["D14:F14"])
                    {
                        range.Worksheet.Cells["D14:F14"].Merge = true;
                        range.Worksheet.Cells["D14:F14"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["D14:F14"].RichText.Add("(12) Đúng tuyến: ");
                        var value = range.Worksheet.Cells["D14:F14"].RichText.Add(coDungTuyen);
                        value.Bold = true;

                        range.Worksheet.Cells["D14:F14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["D14:F14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["D14:F14"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["D14:F14"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G14:J14"])
                    {
                        range.Worksheet.Cells["G14:J14"].Merge = true;
                        range.Worksheet.Cells["G14:J14"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G14:J14"].RichText.Add("Nơi chuyến đến: ");
                        var value = range.Worksheet.Cells["G14:J14"].RichText.Add(yeuCauTiepNhan.NoiChuyen?.Ten);
                        value.Bold = true;
                        range.Worksheet.Cells["G14:J14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G14:J14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G14:J14"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G14:J14"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["K14:N14"])
                    {
                        range.Worksheet.Cells["K14:N14"].Merge = true;
                        range.Worksheet.Cells["K14:N14"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["K14:N14"].RichText.Add("Nơi chuyển đi: ");
                        var value = range.Worksheet.Cells["K14:N14"].RichText.Add(yeuCauKhamBenh?.BenhVienChuyenVien?.Ten);
                        value.Bold = true;
                        range.Worksheet.Cells["K14:N14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["K14:N14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["K14:N14"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["K14:N14"].Style.Font.Color.SetColor(Color.Black);
                    }

                    //=================================15=================================
                    using (var range = worksheet.Cells["A15:C15"])
                    {
                        range.Worksheet.Cells["A15:C15"].Merge = true;
                        range.Worksheet.Cells["A15:C15"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A15:C15"].RichText.Add("(13)Thông tuyến: ");
                        var value = range.Worksheet.Cells["A15:C15"].RichText.Add(coThongTuyen);
                        value.Bold = true;
                        range.Worksheet.Cells["A15:C15"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A15:C15"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A15:C15"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A15:C15"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["D15:F15"])
                    {
                        range.Worksheet.Cells["D15:F15"].Merge = true;
                        range.Worksheet.Cells["D15:F15"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["D15:F15"].RichText.Add("(14)Trái tuyến: ");
                        var value = range.Worksheet.Cells["D15:F15"].RichText.Add(coTraiTuyen);
                        value.Bold = true;

                        range.Worksheet.Cells["D15:F15"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["D15:F15"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["D15:F15"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["D15:F15"].Style.Font.Color.SetColor(Color.Black);
                    }


                    //================================= 16 =================================

                    using (var range = worksheet.Cells["A16:F16"])
                    {
                        range.Worksheet.Cells["A16:F16"].Merge = true;
                        range.Worksheet.Cells["A16:F16"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A16:F16"].RichText.Add("(15)Chẩn đoán xác định: ");
                        var value = range.Worksheet.Cells["A16:F16"].RichText.Add(yeuCauKhamBenh?.GhiChuICDChinh);
                        value.Bold = true;

                        range.Worksheet.Cells["A16:F16"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A16:F16"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A16:F16"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A16:F16"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G16:N16"])
                    {
                        range.Worksheet.Cells["G16:N16"].Merge = true;

                        range.Worksheet.Cells["G16:N16"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G16:N16"].RichText.Add("(16) Mã bệnh: ");
                        var value = range.Worksheet.Cells["G16:N16"].RichText.Add(yeuCauKhamBenh?.Icdchinh?.Ma);
                        value.Bold = true;


                        range.Worksheet.Cells["G16:N16"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G16:N16"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G16:N16"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G16:N16"].Style.Font.Color.SetColor(Color.Black);
                    }

                    //================================= 17 =================================
                    using (var range = worksheet.Cells["A17:F17"])
                    {
                        range.Worksheet.Cells["A17:F17"].Merge = true;
                        range.Worksheet.Cells["A17:F17"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A17:F17"].RichText.Add("(17)Bệnh kèm theo: ");
                        var value = range.Worksheet.Cells["A17:F17"].RichText.Add(tenICDKemTheo);
                        value.Bold = true;


                        range.Worksheet.Cells["A17:F17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A17:F17"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A17:F17"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A17:F17"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G17:N17"])
                    {
                        range.Worksheet.Cells["G17:N17"].Merge = true;
                        range.Worksheet.Cells["G17:N17"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G17:N17"].RichText.Add("(18)Mã bệnh kèm theo: ");
                        var value = range.Worksheet.Cells["G17:N17"].RichText.Add(maICDKemTheo);
                        value.Bold = true;


                        range.Worksheet.Cells["G17:N17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G17:N17"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G17:N17"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G17:N17"].Style.Font.Color.SetColor(Color.Black);
                    }

                    //================================= 18 =================================
                    using (var range = worksheet.Cells["A18:F18"])
                    {
                        range.Worksheet.Cells["A18:F18"].Merge = true;
                        range.Worksheet.Cells["A18:F18"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A18:F18"].RichText.Add("(19)Thời điểm đủ 05 năm liên tục từ ngày: ");
                        var value = range.Worksheet.Cells["A18:F18"].RichText.Add(yeuCauTiepNhan.BHYTNgayDu5Nam?.ApplyFormatDate());
                        value.Bold = true;


                        range.Worksheet.Cells["A18:F18"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A18:F18"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A18:F18"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A18:F18"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G18:N18"])
                    {
                        range.Worksheet.Cells["G18:N18"].Merge = true;
                        range.Worksheet.Cells["G18:N18"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G18:N18"].RichText.Add("(21)Miễn cùng chi trả trong năm từ ngày: ");
                        var value = range.Worksheet.Cells["G18:N18"].RichText.Add(yeuCauTiepNhan.BHYTNgayDuocMienCungChiTra?.ApplyFormatDate());
                        value.Bold = true;

                        range.Worksheet.Cells["G18:N18"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G18:N18"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G18:N18"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G18:N18"].Style.Font.Color.SetColor(Color.Black);
                    }

                    //================================= 19 =================================

                    using (var range = worksheet.Cells["A19:N19"])
                    {
                        range.Worksheet.Cells["A19:N19"].Merge = true;
                        range.Worksheet.Cells["A19:N19"].Value = "II. Phần Chi phí khám bệnh, chữa bệnh";
                        range.Worksheet.Cells["A19:N19"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A19:N19"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A19:N19"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A19:N19"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A19:N19"].Style.Font.Bold = true;
                    }

                    //================================= 20 =========================================
                    using (var range = worksheet.Cells["A20:C20"])
                    {
                        range.Worksheet.Cells["A20:C20"].Merge = true;
                        range.Worksheet.Cells["A20:C20"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A20:C20"].RichText.Add("Mã thẻ BHYT: ");
                        var value = range.Worksheet.Cells["A20:C20"].RichText.Add(maBHYT);
                        value.Bold = true;

                        range.Worksheet.Cells["A20:C20"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A20:C20"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A20:C20"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A20:C20"].Style.Font.Color.SetColor(Color.Black);
                    }
                    using (var range = worksheet.Cells["D20:F20"])
                    {
                        range.Worksheet.Cells["D20:F20"].Merge = true;
                        range.Worksheet.Cells["D20:F20"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["D20:F20"].RichText.Add("Giá trị từ: ");
                        var valuebHYTTuNgay = range.Worksheet.Cells["D20:F20"].RichText.Add(bHYTTuNgay);
                        valuebHYTTuNgay.Bold = true;

                        var den = range.Worksheet.Cells["D20:F20"].RichText.Add(" đến: ");
                        den.Bold = false;
                        var valuebHYTDenNgay = range.Worksheet.Cells["D20:F20"].RichText.Add(bHYTDenNgay);
                        valuebHYTDenNgay.Bold = true;

                        range.Worksheet.Cells["D20:F20"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["D20:F20"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["D20:F20"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["D20:F20"].Style.Font.Color.SetColor(Color.Black);
                    }
                    using (var range = worksheet.Cells["G20:J20"])
                    {
                        range.Worksheet.Cells["G20:J20"].Merge = true;
                        range.Worksheet.Cells["G20:J20"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G20:J20"].RichText.Add("Mức hưởng: ");
                        var value = range.Worksheet.Cells["G20:J20"].RichText.Add(mucHuong);
                        value.Bold = true;


                        range.Worksheet.Cells["G20:J20"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G20:J20"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G20:J20"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G20:J20"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["A21:N22"])
                    {
                        range.Worksheet.Cells["A21:N22"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A21:N22"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A21:N22"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A21:N22"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A21:N22"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A21:N22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A21:A22"].Merge = true;
                        range.Worksheet.Cells["A21:A22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A21:A22"].Value = "STT";

                        range.Worksheet.Cells["B21:B22"].Merge = true;
                        range.Worksheet.Cells["B21:B22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B21:B22"].Value = "Nội dung";

                        range.Worksheet.Cells["C21:C22"].Merge = true;
                        range.Worksheet.Cells["C21:C22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C21:C22"].Value = "Đơn vị tính";

                        range.Worksheet.Cells["D21:D22"].Merge = true;
                        range.Worksheet.Cells["D21:D22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D21:D22"].Value = "Số lượng";

                        range.Worksheet.Cells["E21:E22"].Merge = true;
                        range.Worksheet.Cells["E21:E22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E21:E22"].Value = "Đơn giá BV(đồng)";

                        range.Worksheet.Cells["F21:F22"].Merge = true;
                        range.Worksheet.Cells["F21:F22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F21:F22"].Value = "Đơn giá BH(đồng)";

                        range.Worksheet.Cells["G21:G22"].Merge = true;
                        range.Worksheet.Cells["G21:G22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G21:G22"].Value = "Tỉ lệ thanh toán theo dịch vụ (%)";

                        range.Worksheet.Cells["H21:H22"].Merge = true;
                        range.Worksheet.Cells["H21:H22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H21:H22"].Value = "Thành tiền BV (đồng)";

                        range.Worksheet.Cells["I21:I22"].Merge = true;
                        range.Worksheet.Cells["I21:I22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I21:I22"].Value = "Tỷ lệ thanh toán BHYT (%)";

                        range.Worksheet.Cells["J21:J22"].Merge = true;
                        range.Worksheet.Cells["J21:J22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J21:J22"].Value = "Thành tiền BH(đồng)";

                        range.Worksheet.Cells["K21:N21"].Merge = true;
                        range.Worksheet.Cells["K21:N21"].Value = "Nguồn thanh toán (đồng)";

                        range.Worksheet.Cells["K22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K22"].Value = "Quỹ BHYT";

                        range.Worksheet.Cells["L22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["L22"].Value = "Người bệnh cùng chi trả (đồng)";

                        range.Worksheet.Cells["M22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["M22"].Value = "Khác (đồng)";

                        range.Worksheet.Cells["N22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["N22"].Value = "Người bệnh tự trả(đồng)";
                    }

                    int indexItem = 1;
                    int index = 23;

                    foreach (var chiPhi in chiPhis)
                    {
                        var chiphiBangKe = new BangKeKhamBenhBenhVienVo
                        {
                            Nhom = chiPhi.NhomChiPhiBangKe,
                            Id = chiPhi.Id,
                            NoiDung = chiPhi.TenDichVu,
                            DonViTinh = chiPhi.DonViTinh,
                            SoLuong = (decimal)chiPhi.Soluong,
                            DuocHuongBaoHiem = chiPhi.DuocHuongBHYT,
                            DonGiaBH = chiPhi.DuocHuongBHYT ? chiPhi.DonGiaBHYT : 0,
                            MucHuongBaoHiem = chiPhi.MucHuongBaoHiem,
                            TiLeThanhToanTheoDV = chiPhi.TiLeBaoHiemThanhToan,
                            BaoHiemChiTra = true,
                            DonGiaBV = chiPhi.DonGia
                        };
                        if (chiPhi.KhongTinhPhi == true || chiPhi.YeuCauGoiDichVuId != null)
                        {
                            chiphiBangKe.Khac = chiPhi.MucHuongBaoHiem > 0
                                ? (chiphiBangKe.ThanhTienBV.GetValueOrDefault() - chiphiBangKe.ThanhTienBH.GetValueOrDefault())
                                : chiphiBangKe.ThanhTienBV.GetValueOrDefault();
                        }
                        else
                        {
                            chiphiBangKe.Khac = chiPhi.SoTienMG;
                        }
                        dsChiPhiBangKe.Add(chiphiBangKe);
                    }
                    var groupChiPhiBangKes = dsChiPhiBangKe.GroupBy(x => x.Nhom).OrderBy(o => (int)o.Key * (o.Key == NhomChiPhiBangKe.NgayGiuongDieuTriBanNgay || o.Key == NhomChiPhiBangKe.NgayGiuongDieuTriNoiTru ? 1 : 10));
                    foreach (var groupChiPhiBangKe in groupChiPhiBangKes)
                    {
                        if (groupChiPhiBangKe.Key == NhomChiPhiBangKe.NgayGiuongDieuTriBanNgay)
                        {

                            worksheet.Cells["A" + index + ":H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["A" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + index + ":H" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            worksheet.Cells["A" + index + ":H" + index].Merge = true;
                            worksheet.Cells["A" + index + ":H" + index].Value = "2. Ngày giường:";
                            worksheet.Cells["A" + index + ":H" + index].Style.Font.Bold = true;

                            index++;

                            worksheet.Cells["A" + index + ":G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["A" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            worksheet.Cells["A" + index + ":G" + index].Merge = true;
                            worksheet.Cells["A" + index + ":G" + index].Value = "2.1." + groupChiPhiBangKe.Key.GetDescription();
                            worksheet.Cells["A" + index + ":G" + index].Style.Font.Bold = true;

                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["H" + index].Style.Font.Bold = true;
                            worksheet.Cells["H" + index].Value = groupChiPhiBangKe.Sum(o => o.ThanhTienBV);

                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["J" + index].Style.Font.Bold = true;
                            worksheet.Cells["J" + index].Value = groupChiPhiBangKe.Sum(o => o.ThanhTienBH);

                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["K" + index].Style.Font.Bold = true;
                            worksheet.Cells["K" + index].Value = groupChiPhiBangKe.Sum(o => o.QuyBHYT);

                            worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["L" + index].Style.Font.Bold = true;
                            worksheet.Cells["L" + index].Value = groupChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra);

                            worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["M" + index].Style.Font.Bold = true;
                            worksheet.Cells["M" + index].Value = groupChiPhiBangKe.Sum(o => o.Khac);

                            worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["N" + index].Style.Font.Bold = true;
                            worksheet.Cells["N" + index].Value = groupChiPhiBangKe.Sum(o => o.NguoiBenhTuTra);

                            index++;
                        }
                        else if (groupChiPhiBangKe.Key == NhomChiPhiBangKe.NgayGiuongDieuTriNoiTru)
                        {
                            if (!groupChiPhiBangKes.Any(o => o.Key == NhomChiPhiBangKe.NgayGiuongDieuTriBanNgay))
                            {
                                worksheet.Cells["A" + index + ":H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["A" + index + ":H" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                worksheet.Cells["A" + index + ":H" + index].Merge = true;
                                worksheet.Cells["A" + index + ":H" + index].Value = "2. Ngày giường:";
                                worksheet.Cells["A" + index + ":H" + index].Style.Font.Bold = true;
                            }

                            worksheet.Cells["A" + index + ":G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["A" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            worksheet.Cells["A" + index + ":G" + index].Merge = true;
                            worksheet.Cells["A" + index + ":G" + index].Value = "2.2." + groupChiPhiBangKe.Key.GetDescription();
                            worksheet.Cells["A" + index + ":G" + index].Style.Font.Bold = true;

                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["H" + index].Style.Font.Bold = true;
                            worksheet.Cells["H" + index].Value = groupChiPhiBangKe.Sum(o => o.ThanhTienBV);

                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["J" + index].Style.Font.Bold = true;
                            worksheet.Cells["J" + index].Value = groupChiPhiBangKe.Sum(o => o.ThanhTienBH);

                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["K" + index].Style.Font.Bold = true;
                            worksheet.Cells["K" + index].Value = groupChiPhiBangKe.Sum(o => o.QuyBHYT);

                            worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["L" + index].Style.Font.Bold = true;
                            worksheet.Cells["L" + index].Value = groupChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra);

                            worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["M" + index].Style.Font.Bold = true;
                            worksheet.Cells["M" + index].Value = groupChiPhiBangKe.Sum(o => o.Khac);

                            worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["N" + index].Style.Font.Bold = true;
                            worksheet.Cells["N" + index].Value = groupChiPhiBangKe.Sum(o => o.NguoiBenhTuTra);

                            index++;
                        }
                        else
                        {

                            worksheet.Cells["A" + index + ":G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["A" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            worksheet.Cells["A" + index + ":G" + index].Merge = true;
                            worksheet.Cells["A" + index + ":G" + index].Value = (int)groupChiPhiBangKe.Key + "." + groupChiPhiBangKe.Key.GetDescription();
                            worksheet.Cells["A" + index + ":G" + index].Style.Font.Bold = true;

                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["H" + index].Style.Font.Bold = true;
                            worksheet.Cells["H" + index].Value = groupChiPhiBangKe.Sum(o => o.ThanhTienBV);

                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["J" + index].Style.Font.Bold = true;
                            worksheet.Cells["J" + index].Value = groupChiPhiBangKe.Sum(o => o.ThanhTienBH);

                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["K" + index].Style.Font.Bold = true;
                            worksheet.Cells["K" + index].Value = groupChiPhiBangKe.Sum(o => o.QuyBHYT);

                            worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["L" + index].Style.Font.Bold = true;
                            worksheet.Cells["L" + index].Value = groupChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra);

                            worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["M" + index].Style.Font.Bold = true;
                            worksheet.Cells["M" + index].Value = groupChiPhiBangKe.Sum(o => o.Khac);

                            worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["N" + index].Style.Font.Bold = true;
                            worksheet.Cells["N" + index].Value = groupChiPhiBangKe.Sum(o => o.NguoiBenhTuTra);

                            index++;                           
                        }
                        if (groupChiPhiBangKe.Key == NhomChiPhiBangKe.GoiVatTu)
                        {
                            var groupGoiVatTus = groupChiPhiBangKe.ToList().GroupBy(o => o.SoGoiVatTu.GetValueOrDefault()).OrderBy(o => o.Key);
                            int sttGoiVatTu = 1;
                            foreach (var groupGoiVatTu in groupGoiVatTus)
                            {
                                worksheet.Cells["A" + index + ":G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["A" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                worksheet.Cells["A" + index + ":G" + index].Merge = true;
                                worksheet.Cells["A" + index + ":G" + index].Value = "10." + sttGoiVatTu + ". " + groupChiPhiBangKe.Key.GetDescription() + " " + sttGoiVatTu;
                                worksheet.Cells["A" + index + ":G" + index].Style.Font.Bold = true;

                                worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["H" + index].Style.Font.Bold = true;
                                worksheet.Cells["H" + index].Value = groupChiPhiBangKe.Sum(o => o.ThanhTienBV);

                                worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["J" + index].Style.Font.Bold = true;
                                worksheet.Cells["J" + index].Value = groupChiPhiBangKe.Sum(o => o.ThanhTienBH);

                                worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["K" + index].Style.Font.Bold = true;
                                worksheet.Cells["K" + index].Value = groupChiPhiBangKe.Sum(o => o.QuyBHYT);

                                worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["L" + index].Style.Font.Bold = true;
                                worksheet.Cells["L" + index].Value = groupChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra);

                                worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["M" + index].Style.Font.Bold = true;
                                worksheet.Cells["M" + index].Value = groupChiPhiBangKe.Sum(o => o.Khac);

                                worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["N" + index].Style.Font.Bold = true;
                                worksheet.Cells["N" + index].Value = groupChiPhiBangKe.Sum(o => o.NguoiBenhTuTra);

                                index++;

                               
                                foreach (var chiPhiBangKe in groupGoiVatTu)
                                {
                                    worksheet.Cells["A" + index + ":N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["A" + index + ":N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["A" + index + ":N" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));

                                    worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["A" + index].Value = indexItem;

                                    worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["B" + index].Value = chiPhiBangKe.NoiDung;

                                    worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["C" + index].Value = chiPhiBangKe.DonViTinh;

                                    worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["D" + index].Value = chiPhiBangKe.SoLuong;

                                    worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["E" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["E" + index].Value = chiPhiBangKe.DonGiaBV;

                                    worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["F" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["F" + index].Value = chiPhiBangKe.DonGiaBH;

                                    worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["G" + index].Value = chiPhiBangKe.TiLeThanhToanTheoDV;

                                    worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["H" + index].Value = chiPhiBangKe.ThanhTienBV;

                                    worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["I" + index].Value = Convert.ToDouble(chiPhiBangKe.MucHuongBaoHiem > 0 ? 100 : 0).ApplyFormatMoneyToDouble();


                                    worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["J" + index].Value = chiPhiBangKe.ThanhTienBH;


                                    worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["K" + index].Value = chiPhiBangKe.QuyBHYT;


                                    worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["L" + index].Value = chiPhiBangKe.NguoiBenhCungChiTra;


                                    worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["M" + index].Value = chiPhiBangKe.Khac;


                                    worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["N" + index].Value = chiPhiBangKe.NguoiBenhTuTra;

                                    indexItem++;
                                    index++;
                                }
                            }
                        }
                        else
                        {
                            foreach (var chiPhiBangKe in groupChiPhiBangKe)
                            {
                                worksheet.Cells["A" + index + ":N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index + ":N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["A" + index + ":N" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));

                                worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["A" + index].Value = indexItem;

                                worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["B" + index].Value = chiPhiBangKe.NoiDung;

                                worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["C" + index].Value = chiPhiBangKe.DonViTinh;

                                worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["D" + index].Value = chiPhiBangKe.SoLuong;

                                worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["E" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["E" + index].Value = chiPhiBangKe.DonGiaBV;

                                worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["F" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["F" + index].Value = chiPhiBangKe.DonGiaBH;

                                worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["G" + index].Value = chiPhiBangKe.TiLeThanhToanTheoDV;

                                worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["H" + index].Value = chiPhiBangKe.ThanhTienBV;

                                worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["I" + index].Value = Convert.ToDouble(chiPhiBangKe.MucHuongBaoHiem > 0 ? 100 : 0).ApplyFormatMoneyToDouble();


                                worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["J" + index].Value = chiPhiBangKe.ThanhTienBH;


                                worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["K" + index].Value = chiPhiBangKe.QuyBHYT;


                                worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["L" + index].Value = chiPhiBangKe.NguoiBenhCungChiTra;


                                worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["M" + index].Value = chiPhiBangKe.Khac;


                                worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["N" + index].Value = chiPhiBangKe.NguoiBenhTuTra;

                                indexItem++;
                                index++;
                            }
                        }
                    }

                    worksheet.Cells["A" + index + ":G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["A" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells["A" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                    worksheet.Cells["A" + index + ":G" + index].Merge = true;
                    worksheet.Cells["A" + index + ":G" + index].Value = "Tổng cộng";
                    worksheet.Cells["A" + index + ":G" + index].Style.Font.Bold = true;

                    worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["H" + index].Value = groupChiPhiBangKes.Select(c => c.Sum(o => o.ThanhTienBV)).Sum();

                    worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["J" + index].Value = groupChiPhiBangKes.Select(c => c.Sum(o => o.ThanhTienBH)).Sum();

                    worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["K" + index].Value = groupChiPhiBangKes.Select(c => c.Sum(o => o.QuyBHYT)).Sum();

                    worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["L" + index].Value = groupChiPhiBangKes.Select(c => c.Sum(o => o.NguoiBenhCungChiTra)).Sum();

                    worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["M" + index].Value = groupChiPhiBangKes.Select(c => c.Sum(o => o.Khac));

                    worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["N" + index].Value = groupChiPhiBangKes.Select(c => c.Sum(o => o.NguoiBenhTuTra)).Sum();

                    index++;

                    worksheet.Cells["A" + index + ":N" + index].Merge = true;
                    worksheet.Cells["A" + index + ":N" + index].Value = "Tổng chi phí lần khám bệnh/cả đợt điều trị: " + Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.ThanhTienBV)).ApplyFormatMoneyToDouble() + " đồng";
                    worksheet.Cells["A" + index + ":N" + index].Style.Font.Bold = true;

                    index++;

                    worksheet.Cells["A" + index + ":N" + index].Merge = true;
                    worksheet.Cells["A" + index + ":N" + index].Value = "(Viết bằng chữ: " + NumberHelper.ChuyenSoRaText(Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.ThanhTienBV))) + ")";

                    index++;

                    worksheet.Cells["A" + index + ":N" + index].Merge = true;
                    worksheet.Cells["A" + index + ":N" + index].Value = "Trong đó , số tiền do:";
                    worksheet.Cells["A" + index + ":N" + index].Style.Font.Bold = true;

                    index++;
                    worksheet.Cells["A" + index + ":N" + index].Merge = true;
                    worksheet.Cells["A" + index + ":N" + index].Value = "- Quỹ BHYT thanh toán: " + Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.QuyBHYT)).ApplyFormatMoneyToDouble();

                    index++;
                    worksheet.Cells["A" + index + ":N" + index].Merge = true;
                    worksheet.Cells["A" + index + ":N" + index].Value = "- Người bệnh trả trong đó: " + (Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra)) + Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.NguoiBenhTuTra))).ApplyFormatMoneyToDouble();

                    index++;
                    worksheet.Cells["A" + index + ":N" + index].Merge = true;
                    worksheet.Cells["A" + index + ":N" + index].Value = "+ Cùng chi trả trong phạm vi BHYT: " + Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra)).ApplyFormatMoneyToDouble(true);

                    index++;
                    worksheet.Cells["A" + index + ":N" + index].Merge = true;
                    worksheet.Cells["A" + index + ":N" + index].Value = "+ Các khoản phải trả khác: " + Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.NguoiBenhTuTra)).ApplyFormatMoneyToDouble(true);

                    index++;
                    worksheet.Cells["A" + index + ":N" + index].Merge = true;
                    worksheet.Cells["A" + index + ":N" + index].Value = "+ Nguồn khác: " + Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.Khac)).ApplyFormatMoneyToDouble(true);


                    index = index + 2;

                    worksheet.Cells["D" + index + ":F" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                    worksheet.Cells["D" + index + ":F" + index].Merge = true;
                    worksheet.Cells["D" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["D" + index + ":F" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    worksheet.Cells["D" + index + ":F" + index].Value = $"Ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}";

                    worksheet.Cells["G" + index + ":I" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                    worksheet.Cells["G" + index + ":I" + index].Merge = true;
                    worksheet.Cells["G" + index + ":I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["G" + index + ":I" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    worksheet.Cells["G" + index + ":I" + index].Value = $"Ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}";

                    worksheet.Cells["J" + index + ":N" + index].Style.Font.Bold = true;
                    worksheet.Cells["J" + index + ":N" + index].Merge = true;
                    worksheet.Cells["J" + index + ":N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["J" + index + ":N" + index].Value = "XÁC NHẬN CỦA NGƯỜI BỆNH ";

                    index++;

                    worksheet.Cells["A" + index + ":C" + index].Style.Font.Bold = true;
                    worksheet.Cells["A" + index + ":C" + index].Merge = true;
                    worksheet.Cells["A" + index + ":C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A" + index + ":C" + index].Value = "NGƯỜI LẬP BẢNG KÊ";

                    worksheet.Cells["D" + index + ":F" + index].Style.Font.Bold = true;
                    worksheet.Cells["D" + index + ":F" + index].Merge = true;
                    worksheet.Cells["D" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["D" + index + ":F" + index].Value = "KẾ TOÁN VIỆN PHÍ";

                    worksheet.Cells["G" + index + ":I" + index].Style.Font.Bold = true;
                    worksheet.Cells["G" + index + ":I" + index].Merge = true;
                    worksheet.Cells["G" + index + ":I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["G" + index + ":I" + index].Value = "GIÁM ĐỊNH BHYT";

                    worksheet.Cells["J" + index + ":N" + index].Style.Font.Bold = true;
                    worksheet.Cells["J" + index + ":N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["J" + index + ":N" + index].Merge = true;
                    worksheet.Cells["J" + index + ":N" + index].Value = "(Ký , Ghi rõ họ tên)";

                    index++;

                    worksheet.Cells["A" + index + ":C" + index].Style.Font.Bold = true;
                    worksheet.Cells["A" + index + ":C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A" + index + ":C" + index].Merge = true;
                    worksheet.Cells["A" + index + ":C" + index].Value = "(Ký , Ghi rõ họ tên)";


                    worksheet.Cells["D" + index + ":F" + index].Style.Font.Bold = true;
                    worksheet.Cells["D" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["D" + index + ":F" + index].Merge = true;
                    worksheet.Cells["D" + index + ":F" + index].Value = "(Ký , Ghi rõ họ tên)";

                    worksheet.Cells["G" + index + ":I" + index].Style.Font.Bold = true;
                    worksheet.Cells["G" + index + ":I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["G" + index + ":I" + index].Merge = true;
                    worksheet.Cells["G" + index + ":I" + index].Value = "(Ký , Ghi rõ họ tên)";

                    worksheet.Cells["J" + index + ":N" + index].Style.Font.Bold = true;
                    worksheet.Cells["J" + index + ":N" + index].Merge = true;
                    worksheet.Cells["J" + index + ":N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["J" + index + ":N" + index].Value = "(Tôi đã nhận...phim...Xquang / CT / MRI)";

                    xlPackage.Save();
                }

                return stream.ToArray();
            }
        }

        public byte[] XuatBangKeNgoaiTruTrongGoiDv(long yeuCauTiepNhanId, bool xemTruoc = false)
        {
            var yeuCauTiepNhan = BaseRepository.TableNoTracking.Where(c => c.Id == yeuCauTiepNhanId)
                .Include(xx => xx.BenhNhan)
                .Include(xx => xx.YeuCauKhamBenhs).ThenInclude(o => o.NoiThucHien)
                .Include(xx => xx.YeuCauKhamBenhs).ThenInclude(o => o.BenhVienChuyenVien)
                .Include(xx => xx.YeuCauKhamBenhs).ThenInclude(o => o.Icdchinh)
                .Include(xx => xx.YeuCauKhamBenhs).ThenInclude(o => o.YeuCauKhamBenhICDKhacs).ThenInclude(o => o.ICD)
                .Include(cc => cc.PhuongXa)
                .Include(cc => cc.QuanHuyen)
                .Include(cc => cc.TinhThanh)
                .Include(cc => cc.NoiChuyen).FirstOrDefault();
          
            if (yeuCauTiepNhan == null)
                return null;

            var benhVien = new Core.Domain.Entities.BenhVien.BenhVien();

            string maBHYT = string.Empty;
            string bHYTTuNgay = string.Empty;
            string bHYTDenNgay = string.Empty;
            string mucHuong = string.Empty;
            string NoiDKKCBBanDau = string.Empty;
            string MaKCBBanDau = string.Empty;


            string maICDKemTheo = string.Empty;
            string tenICDKemTheo = string.Empty;

            var yeuCauKhamBenh = yeuCauTiepNhan.YeuCauKhamBenhs.Where(cc => cc.IcdchinhId != null).OrderBy(cc => cc.ThoiDiemHoanThanh).LastOrDefault();

            if (yeuCauKhamBenh != null)
            {
                maICDKemTheo = yeuCauKhamBenh.YeuCauKhamBenhICDKhacs.FirstOrDefault()?.ICD.Ma;
                tenICDKemTheo = yeuCauKhamBenh.YeuCauKhamBenhICDKhacs.FirstOrDefault()?.GhiChu;
            }

            if (yeuCauTiepNhan.CoBHYT == true)
            {
                if (!String.IsNullOrEmpty(yeuCauTiepNhan.BHYTMaDKBD))
                {
                    benhVien = _benhVienRepository.TableNoTracking.FirstOrDefault(p => p.Ma.Equals(yeuCauTiepNhan.BHYTMaDKBD));
                }
                maBHYT = yeuCauTiepNhan.BHYTMaSoThe;
                bHYTTuNgay = yeuCauTiepNhan.BHYTNgayHieuLuc?.ApplyFormatDate();
                bHYTDenNgay = yeuCauTiepNhan.BHYTNgayHetHan?.ApplyFormatDate();
                mucHuong = yeuCauTiepNhan.BHYTMucHuong.ToString() + "%";
                NoiDKKCBBanDau = benhVien != null ? benhVien?.Ten : "Chưa xác định";
                MaKCBBanDau = yeuCauTiepNhan.BHYTMaDKBD;
            }
            else
            {
                maBHYT = "";
                bHYTTuNgay = "";
                bHYTDenNgay = "";
                mucHuong = "0%";
                NoiDKKCBBanDau = "Chưa xác định";
                MaKCBBanDau = "Chưa xác định";
            }
            List<ChiPhiKhamChuaBenhVo> chiPhis;
            if (xemTruoc)
            {
                chiPhis = GetTatCaDichVuKhamChuaBenh(yeuCauTiepNhanId).Result.Where(o => o.YeuCauGoiDichVuId != null).ToList();
            }
            else
            {
                chiPhis = GetTatCaDichVuKhamChuaBenh(yeuCauTiepNhanId).Result.Where(o => o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && o.YeuCauGoiDichVuId != null).ToList();
            }

            if (chiPhis.Count == 0)
            {
                return  null;
            }

            var dsChiPhiBangKe = new List<BangKeKhamBenhBenhVienVo>();
            var dateItemChiPhis = string.Empty;
            int indexItem = 1;
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {

                    var worksheet = xlPackage.Workbook.Worksheets.Add("BẢNG KÊ CHI PHÍ KHÁM CHỮA BỆNH");

                    // set row
                    worksheet.Row(9).Height = 24.5;
                    worksheet.DefaultRowHeight = 16;

                    //SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 10;
                    worksheet.Column(2).Width = 25;
                    worksheet.Column(3).Width = 15;
                    worksheet.Column(4).Width = 15;
                    worksheet.Column(5).Width = 15;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 15;
                    worksheet.Column(8).Width = 15;
                    worksheet.Column(9).Width = 15;
                    worksheet.Column(10).Width = 15;
                    worksheet.Column(11).Width = 15;
                    worksheet.Column(12).Width = 15;
                    worksheet.Column(13).Width = 15;
                    worksheet.Column(14).Width = 15;

                    worksheet.DefaultColWidth = 7;

                    using (var range = worksheet.Cells["A1:C1"])
                    {
                        range.Worksheet.Cells["A1:C1"].Merge = true;
                        range.Worksheet.Cells["A1:C1"].Value = "CƠ SỞ KHÁM CHỮA BỆNH : BỆNH VIỆN ĐKQT BẮC HÀ"; 
                        range.Worksheet.Cells["A1:C1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:C1"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A1:C1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:C1"].Style.Font.Color.SetColor(Color.Black);
                    }

                    //using (var range = worksheet.Cells["A2:C2"])
                    //{
                    //    range.Worksheet.Cells["A2:C2"].Merge = true;
                    //    range.Worksheet.Cells["A2:C2"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                    //    range.Worksheet.Cells["A2:C2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    //    range.Worksheet.Cells["A2:C2"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    //    range.Worksheet.Cells["A2:C2"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    //    range.Worksheet.Cells["A2:C2"].Style.Font.Color.SetColor(Color.Black);
                    //    range.Worksheet.Cells["A2:C2"].Style.Font.Bold = true;
                    //}

                    using (var range = worksheet.Cells["L1:N1"])
                    {
                        range.Worksheet.Cells["L1:N1"].Merge = true;
                        range.Worksheet.Cells["L1:N1"].Value = "Mẫu số : 01/KBCB ";
                        range.Worksheet.Cells["L1:N1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        range.Worksheet.Cells["L1:N1"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["L1:N1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["L1:N1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["L1:N1"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["L2:N2"])
                    {
                        range.Worksheet.Cells["L2:N2"].Merge = true;
                        range.Worksheet.Cells["L2:N2"].Value = "Mã số người bệnh: " + yeuCauTiepNhan.BenhNhan.MaBN;
                        range.Worksheet.Cells["L2:N2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        range.Worksheet.Cells["L2:N2"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["L2:N2"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["L2:N2"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["L2:N2"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["L3:N3"])
                    {
                        range.Worksheet.Cells["L3:N3"].Merge = true;
                        range.Worksheet.Cells["L3:N3"].Value = "Số khám bệnh:" + yeuCauTiepNhan.MaYeuCauTiepNhan;
                        range.Worksheet.Cells["L3:N3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        range.Worksheet.Cells["L3:N3"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["L3:N3"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["L3:N3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["L3:N3"].Style.Font.Bold = true;
                    }



                    using (var range = worksheet.Cells["A4:N5"])
                    {
                        range.Worksheet.Cells["A4:N5"].Merge = true;
                        range.Worksheet.Cells["A4:N5"].Value = "BẢNG KÊ CHI PHÍ KHÁM CHỮA BỆNH (Trong gói)";
                        range.Worksheet.Cells["A4:N5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:N5"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:N5"].Style.Font.SetFromFont(new Font("Times New Roman", 18));
                        range.Worksheet.Cells["A4:N5"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:N5"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A6:N6"])
                    {
                        range.Worksheet.Cells["A6:N6"].Merge = true;
                        range.Worksheet.Cells["A6:N6"].Value = "I.Hành chính";
                        range.Worksheet.Cells["A6:N6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A6:N6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A6:N6"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A6:N6"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A6:N6"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A7:F7"])
                    {
                        range.Worksheet.Cells["A7:F7"].Merge = true;

                        range.Worksheet.Cells["A7:F7"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A7:F7"].RichText.Add("(1) Họ tên người bệnh: ");
                        var value = range.Worksheet.Cells["A7:F7"].RichText.Add(yeuCauTiepNhan.HoTen);
                        value.Bold = true;

                        range.Worksheet.Cells["A7:F7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A7:F7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A7:F7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));


                        range.Worksheet.Cells["A7:F7"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G7:J7"])
                    {
                        range.Worksheet.Cells["G7:J7"].Merge = true;
                        range.Worksheet.Cells["G7:J7"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G7:J7"].RichText.Add("Ngày,tháng,năm sinh: ");
                        var value = range.Worksheet.Cells["G7:J7"].RichText.Add(DateHelper.DOBFormat(yeuCauTiepNhan.NgaySinh, yeuCauTiepNhan.ThangSinh, yeuCauTiepNhan.NamSinh));
                        value.Bold = true;

                        range.Worksheet.Cells["G7:J7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G7:J7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G7:J7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G7:J7"].Style.Font.Color.SetColor(Color.Black);
                    }

                    var gioitinh = yeuCauTiepNhan.GioiTinh != null ? yeuCauTiepNhan.GioiTinh.GetDescription() : "";
                    using (var range = worksheet.Cells["K7:N7"])
                    {
                        range.Worksheet.Cells["K7:N7"].Merge = true;

                        range.Worksheet.Cells["K7:N7"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["K7:N7"].RichText.Add("Giới tính: ");
                        var value = range.Worksheet.Cells["K7:N7"].RichText.Add(gioitinh);
                        value.Bold = true;

                        range.Worksheet.Cells["K7:N7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["K7:N7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["K7:N7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["K7:N7"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["A8:F8"])
                    {
                        range.Worksheet.Cells["A8:F8"].Merge = true;

                        range.Worksheet.Cells["A8:F8"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A8:F8"].RichText.Add("(2)Địa chỉ hiện tại: ");
                        var value = range.Worksheet.Cells["A8:F8"].RichText.Add(yeuCauTiepNhan.DiaChiDayDu);
                        value.Bold = true;

                        range.Worksheet.Cells["A8:F8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A8:F8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A8:F8"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A8:F8"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G8:J8"])
                    {
                        range.Worksheet.Cells["G8:J8"].Merge = true;

                        range.Worksheet.Cells["G8:J8"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G8:J8"].RichText.Add("(3)Mã khu vực(K1/K2/K3): ");
                        var value = range.Worksheet.Cells["G8:J8"].RichText.Add(yeuCauTiepNhan.BHYTMaKhuVuc);
                        value.Bold = true;

                        range.Worksheet.Cells["G8:J8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G8:J8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G8:J8"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G8:J8"].Style.Font.Color.SetColor(Color.Black);
                    }


                    using (var range = worksheet.Cells["A9:F9"])
                    {
                        range.Worksheet.Cells["A9:F9"].Merge = true;
                        range.Worksheet.Cells["A9:F9"].Style.WrapText = true;

                        var title = range.Worksheet.Cells["A9:F9"].RichText.Add("(4)Mã thẻ BHYT: ");
                        var value = range.Worksheet.Cells["A9:F9"].RichText.Add(maBHYT);
                        value.Bold = true;

                        range.Worksheet.Cells["A9:F9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A9:F9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A9:F9"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A9:F9"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G9:J9"])
                    {
                        range.Worksheet.Cells["G9:J9"].Merge = true;
                        range.Worksheet.Cells["G9:J9"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G9:J9"].RichText.Add("Giá trị từ: ");
                        var valuebHYTTuNgay = range.Worksheet.Cells["G9:J9"].RichText.Add(bHYTTuNgay);
                        valuebHYTTuNgay.Bold = true;

                        var den = range.Worksheet.Cells["G9:J9"].RichText.Add(" đến: ");
                        den.Bold = false;
                        var valuebHYTDenNgay = range.Worksheet.Cells["G9:J9"].RichText.Add(bHYTDenNgay);
                        valuebHYTDenNgay.Bold = true;


                        range.Worksheet.Cells["G9:J9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G9:J9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G9:J9"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G9:J9"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["A10:F10"])
                    {
                        range.Worksheet.Cells["A10:F10"].Merge = true;

                        range.Worksheet.Cells["A10:F10"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A10:F10"].RichText.Add("(5)Nơi ĐK KCB ban đầu: ");
                        var valueNoiDKKCBBanDau = range.Worksheet.Cells["A10:F10"].RichText.Add(NoiDKKCBBanDau);
                        valueNoiDKKCBBanDau.Bold = true;

                        range.Worksheet.Cells["A10:F10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A10:F10"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A10:F10"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A10:F10"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G10:J10"])
                    {
                        range.Worksheet.Cells["G10:J10"].Merge = true;
                        range.Worksheet.Cells["G10:J10"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G10:J10"].RichText.Add("(6)Mã: ");
                        var valueMaKCBBanDau = range.Worksheet.Cells["G10:J10"].RichText.Add(MaKCBBanDau);
                        valueMaKCBBanDau.Bold = true;

                        range.Worksheet.Cells["G10:J10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G10:J10"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G10:J10"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G10:J10"].Style.Font.Color.SetColor(Color.Black);
                    }


                    using (var range = worksheet.Cells["A11:F11"])
                    {
                        range.Worksheet.Cells["A11:F11"].Merge = true;

                        range.Worksheet.Cells["A11:F11"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A11:F11"].RichText.Add("(7)Đến Khám: ");
                        var value = range.Worksheet.Cells["A11:F11"].RichText.Add(yeuCauTiepNhan.ThoiDiemTiepNhan.ApplyFormatGioPhutNgay());
                        value.Bold = true;

                        range.Worksheet.Cells["A11:F11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A11:F11"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A11:F11"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A11:F11"].Style.Font.Color.SetColor(Color.Black);
                    }


                    using (var range = worksheet.Cells["A12:F12"])
                    {
                        range.Worksheet.Cells["A12:F12"].Merge = true;

                        range.Worksheet.Cells["A12:F12"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A12:F12"].RichText.Add("(8)Điều trị ngoại trú/nội trú từ: ");
                        var value = range.Worksheet.Cells["A12:F12"].RichText.Add(string.Empty);
                        value.Bold = true;

                        range.Worksheet.Cells["A12:F12"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A12:F12"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A12:F12"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A12:F12"].Style.Font.Color.SetColor(Color.Black);
                    }


                    using (var range = worksheet.Cells["A13:F13"])
                    {
                        range.Worksheet.Cells["A13:F13"].Merge = true;
                        range.Worksheet.Cells["A13:F13"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A13:F13"].RichText.Add("(9)Kết thúc khám/điều trị: ");
                        var value = range.Worksheet.Cells["A13:F13"].RichText.Add(yeuCauKhamBenh?.ThoiDiemHoanThanh?.ApplyFormatGioPhutNgay());
                        value.Bold = true;


                        range.Worksheet.Cells["A13:F13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A13:F13"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A13:F13"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A13:F13"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G13:J13"])
                    {
                        range.Worksheet.Cells["G13:J13"].Merge = true;
                        range.Worksheet.Cells["G13:J13"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G13:J13"].RichText.Add("Tổng số ngày điều trị: ");
                        var value = range.Worksheet.Cells["G13:J13"].RichText.Add(string.Empty);
                        value.Bold = true;

                        range.Worksheet.Cells["G13:J13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G13:J13"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G13:J13"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G13:J13"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["K13:N13"])
                    {
                        range.Worksheet.Cells["K13:N13"].Merge = true;

                        range.Worksheet.Cells["K13:N13"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["K13:N13"].RichText.Add("(10)Tình trạng ra viện: ");
                        var value = range.Worksheet.Cells["K13:N13"].RichText.Add("1");
                        value.Bold = true;


                        range.Worksheet.Cells["K13:N13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["K13:N13"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["K13:N13"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["K13:N13"].Style.Font.Color.SetColor(Color.Black);
                    }

                    //=================================14=================================
                    var coCapCuu = yeuCauTiepNhan.LyDoVaoVien == Enums.EnumLyDoVaoVien.CapCuu ? "X" : string.Empty;
                    var coDungTuyen = yeuCauTiepNhan.LyDoVaoVien == Enums.EnumLyDoVaoVien.DungTuyen ? "X" : string.Empty;
                    var coThongTuyen = yeuCauTiepNhan.LyDoVaoVien == Enums.EnumLyDoVaoVien.ThongTuyen ? "X" : string.Empty;
                    var coTraiTuyen = yeuCauTiepNhan.LyDoVaoVien == Enums.EnumLyDoVaoVien.TraiTuyen ? "X" : string.Empty;

                    using (var range = worksheet.Cells["A14:C14"])
                    {
                        range.Worksheet.Cells["A14:C14"].Merge = true;

                        range.Worksheet.Cells["A14:C14"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A14:C14"].RichText.Add("(11) Cấp cứu: ");
                        var value = range.Worksheet.Cells["A14:C14"].RichText.Add(coCapCuu);
                        value.Bold = true;

                        range.Worksheet.Cells["A14:C14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A14:C14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A14:C14"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A14:C14"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["D14:F14"])
                    {
                        range.Worksheet.Cells["D14:F14"].Merge = true;
                        range.Worksheet.Cells["D14:F14"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["D14:F14"].RichText.Add("(12) Đúng tuyến: ");
                        var value = range.Worksheet.Cells["D14:F14"].RichText.Add(coDungTuyen);
                        value.Bold = true;

                        range.Worksheet.Cells["D14:F14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["D14:F14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["D14:F14"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["D14:F14"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G14:J14"])
                    {
                        range.Worksheet.Cells["G14:J14"].Merge = true;
                        range.Worksheet.Cells["G14:J14"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G14:J14"].RichText.Add("Nơi chuyến đến: ");
                        var value = range.Worksheet.Cells["G14:J14"].RichText.Add(yeuCauTiepNhan.NoiChuyen?.Ten);
                        value.Bold = true;
                        range.Worksheet.Cells["G14:J14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G14:J14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G14:J14"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G14:J14"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["K14:N14"])
                    {
                        range.Worksheet.Cells["K14:N14"].Merge = true;
                        range.Worksheet.Cells["K14:N14"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["K14:N14"].RichText.Add("Nơi chuyển đi: ");
                        var value = range.Worksheet.Cells["K14:N14"].RichText.Add(yeuCauKhamBenh?.BenhVienChuyenVien?.Ten);
                        value.Bold = true;
                        range.Worksheet.Cells["K14:N14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["K14:N14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["K14:N14"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["K14:N14"].Style.Font.Color.SetColor(Color.Black);
                    }

                    //=================================15=================================
                    using (var range = worksheet.Cells["A15:C15"])
                    {
                        range.Worksheet.Cells["A15:C15"].Merge = true;
                        range.Worksheet.Cells["A15:C15"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A15:C15"].RichText.Add("(13)Thông tuyến: ");
                        var value = range.Worksheet.Cells["A15:C15"].RichText.Add(coThongTuyen);
                        value.Bold = true;
                        range.Worksheet.Cells["A15:C15"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A15:C15"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A15:C15"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A15:C15"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["D15:F15"])
                    {
                        range.Worksheet.Cells["D15:F15"].Merge = true;
                        range.Worksheet.Cells["D15:F15"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["D15:F15"].RichText.Add("(14)Trái tuyến: ");
                        var value = range.Worksheet.Cells["D15:F15"].RichText.Add(coTraiTuyen);
                        value.Bold = true;

                        range.Worksheet.Cells["D15:F15"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["D15:F15"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["D15:F15"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["D15:F15"].Style.Font.Color.SetColor(Color.Black);
                    }


                    //================================= 16 =================================

                    using (var range = worksheet.Cells["A16:F16"])
                    {
                        range.Worksheet.Cells["A16:F16"].Merge = true;
                        range.Worksheet.Cells["A16:F16"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A16:F16"].RichText.Add("(15)Chẩn đoán xác định: ");
                        var value = range.Worksheet.Cells["A16:F16"].RichText.Add(yeuCauKhamBenh?.GhiChuICDChinh);
                        value.Bold = true;

                        range.Worksheet.Cells["A16:F16"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A16:F16"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A16:F16"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A16:F16"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G16:N16"])
                    {
                        range.Worksheet.Cells["G16:N16"].Merge = true;

                        range.Worksheet.Cells["G16:N16"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G16:N16"].RichText.Add("(16) Mã bệnh: ");
                        var value = range.Worksheet.Cells["G16:N16"].RichText.Add(yeuCauKhamBenh?.Icdchinh?.Ma);
                        value.Bold = true;


                        range.Worksheet.Cells["G16:N16"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G16:N16"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G16:N16"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G16:N16"].Style.Font.Color.SetColor(Color.Black);
                    }

                    //================================= 17 =================================
                    using (var range = worksheet.Cells["A17:F17"])
                    {
                        range.Worksheet.Cells["A17:F17"].Merge = true;
                        range.Worksheet.Cells["A17:F17"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A17:F17"].RichText.Add("(17)Bệnh kèm theo: ");
                        var value = range.Worksheet.Cells["A17:F17"].RichText.Add(tenICDKemTheo);
                        value.Bold = true;


                        range.Worksheet.Cells["A17:F17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A17:F17"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A17:F17"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A17:F17"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G17:N17"])
                    {
                        range.Worksheet.Cells["G17:N17"].Merge = true;
                        range.Worksheet.Cells["G17:N17"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G17:N17"].RichText.Add("(18)Mã bệnh kèm theo: ");
                        var value = range.Worksheet.Cells["G17:N17"].RichText.Add(maICDKemTheo);
                        value.Bold = true;


                        range.Worksheet.Cells["G17:N17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G17:N17"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G17:N17"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G17:N17"].Style.Font.Color.SetColor(Color.Black);
                    }

                    //================================= 18 =================================
                    using (var range = worksheet.Cells["A18:F18"])
                    {
                        range.Worksheet.Cells["A18:F18"].Merge = true;
                        range.Worksheet.Cells["A18:F18"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A18:F18"].RichText.Add("(19)Thời điểm đủ 05 năm liên tục từ ngày: ");
                        var value = range.Worksheet.Cells["A18:F18"].RichText.Add(yeuCauTiepNhan.BHYTNgayDu5Nam?.ApplyFormatDate());
                        value.Bold = true;


                        range.Worksheet.Cells["A18:F18"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A18:F18"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A18:F18"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A18:F18"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G18:N18"])
                    {
                        range.Worksheet.Cells["G18:N18"].Merge = true;
                        range.Worksheet.Cells["G18:N18"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G18:N18"].RichText.Add("(21)Miễn cùng chi trả trong năm từ ngày: ");
                        var value = range.Worksheet.Cells["G18:N18"].RichText.Add(yeuCauTiepNhan.BHYTNgayDuocMienCungChiTra?.ApplyFormatDate());
                        value.Bold = true;

                        range.Worksheet.Cells["G18:N18"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G18:N18"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G18:N18"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G18:N18"].Style.Font.Color.SetColor(Color.Black);
                    }

                    //================================= 19 =================================

                    using (var range = worksheet.Cells["A19:N19"])
                    {
                        range.Worksheet.Cells["A19:N19"].Merge = true;
                        range.Worksheet.Cells["A19:N19"].Value = "II. Phần Chi phí khám bệnh, chữa bệnh";
                        range.Worksheet.Cells["A19:N19"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A19:N19"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A19:N19"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A19:N19"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A19:N19"].Style.Font.Bold = true;
                    }

                    //================================= 20 =========================================
                    using (var range = worksheet.Cells["A20:C20"])
                    {
                        range.Worksheet.Cells["A20:C20"].Merge = true;
                        range.Worksheet.Cells["A20:C20"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A20:C20"].RichText.Add("Mã thẻ BHYT: ");
                        var value = range.Worksheet.Cells["A20:C20"].RichText.Add(maBHYT);
                        value.Bold = true;

                        range.Worksheet.Cells["A20:C20"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A20:C20"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A20:C20"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A20:C20"].Style.Font.Color.SetColor(Color.Black);
                    }
                    using (var range = worksheet.Cells["D20:F20"])
                    {
                        range.Worksheet.Cells["D20:F20"].Merge = true;
                        range.Worksheet.Cells["D20:F20"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["D20:F20"].RichText.Add("Giá trị từ: ");
                        var valuebHYTTuNgay = range.Worksheet.Cells["D20:F20"].RichText.Add(bHYTTuNgay);
                        valuebHYTTuNgay.Bold = true;

                        var den = range.Worksheet.Cells["D20:F20"].RichText.Add(" đến: ");
                        den.Bold = false;
                        var valuebHYTDenNgay = range.Worksheet.Cells["D20:F20"].RichText.Add(bHYTDenNgay);
                        valuebHYTDenNgay.Bold = true;

                        range.Worksheet.Cells["D20:F20"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["D20:F20"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["D20:F20"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["D20:F20"].Style.Font.Color.SetColor(Color.Black);
                    }
                    using (var range = worksheet.Cells["G20:J20"])
                    {
                        range.Worksheet.Cells["G20:J20"].Merge = true;
                        range.Worksheet.Cells["G20:J20"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G20:J20"].RichText.Add("Mức hưởng: ");
                        var value = range.Worksheet.Cells["G20:J20"].RichText.Add(mucHuong);
                        value.Bold = true;


                        range.Worksheet.Cells["G20:J20"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G20:J20"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G20:J20"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G20:J20"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["A21:N22"])
                    {
                        range.Worksheet.Cells["A21:N22"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A21:N22"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A21:N22"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A21:N22"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A21:N22"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A21:N22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A21:A22"].Merge = true;
                        range.Worksheet.Cells["A21:A22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A21:A22"].Value = "STT";

                        range.Worksheet.Cells["B21:B22"].Merge = true;
                        range.Worksheet.Cells["B21:B22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B21:B22"].Value = "Nội dung";

                        range.Worksheet.Cells["C21:C22"].Merge = true;
                        range.Worksheet.Cells["C21:C22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C21:C22"].Value = "Đơn vị tính";

                        range.Worksheet.Cells["D21:D22"].Merge = true;
                        range.Worksheet.Cells["D21:D22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D21:D22"].Value = "Số lượng";

                        range.Worksheet.Cells["E21:E22"].Merge = true;
                        range.Worksheet.Cells["E21:E22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E21:E22"].Value = "Đơn giá BV(đồng)";

                        range.Worksheet.Cells["F21:F22"].Merge = true;
                        range.Worksheet.Cells["F21:F22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F21:F22"].Value = "Đơn giá BH(đồng)";

                        range.Worksheet.Cells["G21:G22"].Merge = true;
                        range.Worksheet.Cells["G21:G22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G21:G22"].Value = "Tỉ lệ thanh toán theo dịch vụ (%)";

                        range.Worksheet.Cells["H21:H22"].Merge = true;
                        range.Worksheet.Cells["H21:H22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H21:H22"].Value = "Thành tiền BV (đồng)";

                        range.Worksheet.Cells["I21:I22"].Merge = true;
                        range.Worksheet.Cells["I21:I22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I21:I22"].Value = "Tỷ lệ thanh toán BHYT (%)";

                        range.Worksheet.Cells["J21:J22"].Merge = true;
                        range.Worksheet.Cells["J21:J22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J21:J22"].Value = "Thành tiền BH(đồng)";

                        range.Worksheet.Cells["K21:N21"].Merge = true;
                        range.Worksheet.Cells["K21:N21"].Value = "Nguồn thanh toán (đồng)";

                        range.Worksheet.Cells["K22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K22"].Value = "Quỹ BHYT";

                        range.Worksheet.Cells["L22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["L22"].Value = "Người bệnh cùng chi trả (đồng)";

                        range.Worksheet.Cells["M22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["M22"].Value = "Khác (đồng)";

                        range.Worksheet.Cells["N22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["N22"].Value = "Người bệnh tự trả(đồng)";
                    }

                   
                    int index = 23;

                    foreach (var chiPhi in chiPhis)
                    {
                        var chiphiBangKe = new BangKeKhamBenhBenhVienVo
                        {
                            Nhom = chiPhi.NhomChiPhiBangKe,
                            Id = chiPhi.Id,
                            NoiDung = chiPhi.TenDichVu,
                            DonViTinh = chiPhi.DonViTinh,
                            SoLuong = (decimal)chiPhi.Soluong,
                            DuocHuongBaoHiem = chiPhi.DuocHuongBHYT,
                            DonGiaBH = chiPhi.DuocHuongBHYT ? chiPhi.DonGiaBHYT : 0,
                            MucHuongBaoHiem = chiPhi.MucHuongBaoHiem,
                            TiLeThanhToanTheoDV = chiPhi.TiLeBaoHiemThanhToan,
                            BaoHiemChiTra = true,
                            DonGiaBV = chiPhi.DonGia
                        };

                        if (chiPhi.KhongTinhPhi == true || chiPhi.YeuCauGoiDichVuId != null)
                        {
                            chiphiBangKe.Khac = chiPhi.MucHuongBaoHiem > 0
                                ? (chiphiBangKe.ThanhTienBV.GetValueOrDefault() - chiphiBangKe.ThanhTienBH.GetValueOrDefault())
                                : chiphiBangKe.ThanhTienBV.GetValueOrDefault();
                        }
                        else
                        {
                            chiphiBangKe.Khac = chiPhi.SoTienMG;
                        }
                        dsChiPhiBangKe.Add(chiphiBangKe);
                    }
                    var groupChiPhiBangKes = dsChiPhiBangKe.GroupBy(x => x.Nhom).OrderBy(o => (int)o.Key * (o.Key == NhomChiPhiBangKe.NgayGiuongDieuTriBanNgay || o.Key == NhomChiPhiBangKe.NgayGiuongDieuTriNoiTru ? 1 : 10));
                    foreach (var groupChiPhiBangKe in groupChiPhiBangKes)
                    {
                        if (groupChiPhiBangKe.Key == NhomChiPhiBangKe.NgayGiuongDieuTriBanNgay)
                        {
                            worksheet.Cells["A" + index + ":H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["A" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + index + ":H" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            worksheet.Cells["A" + index + ":H" + index].Merge = true;
                            worksheet.Cells["A" + index + ":H" + index].Value = "2. Ngày giường:";
                            worksheet.Cells["A" + index + ":H" + index].Style.Font.Bold = true;

                            index++;

                            worksheet.Cells["A" + index + ":G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["A" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            worksheet.Cells["A" + index + ":G" + index].Merge = true;
                            worksheet.Cells["A" + index + ":G" + index].Value = "2.1." + groupChiPhiBangKe.Key.GetDescription();
                            worksheet.Cells["A" + index + ":G" + index].Style.Font.Bold = true;

                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["H" + index].Style.Font.Bold = true;
                            worksheet.Cells["H" + index].Value = groupChiPhiBangKe.Sum(o => o.ThanhTienBV);

                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["J" + index].Style.Font.Bold = true;
                            worksheet.Cells["J" + index].Value = groupChiPhiBangKe.Sum(o => o.ThanhTienBH);

                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["K" + index].Style.Font.Bold = true;
                            worksheet.Cells["K" + index].Value = groupChiPhiBangKe.Sum(o => o.QuyBHYT);

                            worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["L" + index].Style.Font.Bold = true;
                            worksheet.Cells["L" + index].Value = groupChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra);

                            worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["M" + index].Style.Font.Bold = true;
                            worksheet.Cells["M" + index].Value = groupChiPhiBangKe.Sum(o => o.Khac);

                            worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["N" + index].Style.Font.Bold = true;
                            worksheet.Cells["N" + index].Value = groupChiPhiBangKe.Sum(o => o.NguoiBenhTuTra);

                            index++;
                        }
                        else if (groupChiPhiBangKe.Key == NhomChiPhiBangKe.NgayGiuongDieuTriNoiTru)
                        {
                            if (!groupChiPhiBangKes.Any(o => o.Key == NhomChiPhiBangKe.NgayGiuongDieuTriBanNgay))
                            {
                                worksheet.Cells["A" + index + ":H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["A" + index + ":H" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                worksheet.Cells["A" + index + ":H" + index].Merge = true;
                                worksheet.Cells["A" + index + ":H" + index].Value = "2. Ngày giường:";
                                worksheet.Cells["A" + index + ":H" + index].Style.Font.Bold = true;
                            }
                            worksheet.Cells["A" + index + ":G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["A" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            worksheet.Cells["A" + index + ":G" + index].Merge = true;
                            worksheet.Cells["A" + index + ":G" + index].Value = "2.2." + groupChiPhiBangKe.Key.GetDescription();
                            worksheet.Cells["A" + index + ":G" + index].Style.Font.Bold = true;

                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["H" + index].Style.Font.Bold = true;
                            worksheet.Cells["H" + index].Value = groupChiPhiBangKe.Sum(o => o.ThanhTienBV);

                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["J" + index].Style.Font.Bold = true;
                            worksheet.Cells["J" + index].Value = groupChiPhiBangKe.Sum(o => o.ThanhTienBH);

                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["K" + index].Style.Font.Bold = true;
                            worksheet.Cells["K" + index].Value = groupChiPhiBangKe.Sum(o => o.QuyBHYT);

                            worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["L" + index].Style.Font.Bold = true;
                            worksheet.Cells["L" + index].Value = groupChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra);

                            worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["M" + index].Style.Font.Bold = true;
                            worksheet.Cells["M" + index].Value = groupChiPhiBangKe.Sum(o => o.Khac);

                            worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["N" + index].Style.Font.Bold = true;
                            worksheet.Cells["N" + index].Value = groupChiPhiBangKe.Sum(o => o.NguoiBenhTuTra);

                            index++;
                        }
                        else
                        {
                            worksheet.Cells["A" + index + ":G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["A" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            worksheet.Cells["A" + index + ":G" + index].Merge = true;
                            worksheet.Cells["A" + index + ":G" + index].Value = (int)groupChiPhiBangKe.Key + "." + groupChiPhiBangKe.Key.GetDescription();
                            worksheet.Cells["A" + index + ":G" + index].Style.Font.Bold = true;

                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["H" + index].Style.Font.Bold = true;
                            worksheet.Cells["H" + index].Value = groupChiPhiBangKe.Sum(o => o.ThanhTienBV);

                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["J" + index].Style.Font.Bold = true;
                            worksheet.Cells["J" + index].Value = groupChiPhiBangKe.Sum(o => o.ThanhTienBH);

                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["K" + index].Style.Font.Bold = true;
                            worksheet.Cells["K" + index].Value = groupChiPhiBangKe.Sum(o => o.QuyBHYT);

                            worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["L" + index].Style.Font.Bold = true;
                            worksheet.Cells["L" + index].Value = groupChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra);

                            worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["M" + index].Style.Font.Bold = true;
                            worksheet.Cells["M" + index].Value = groupChiPhiBangKe.Sum(o => o.Khac);

                            worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["N" + index].Style.Font.Bold = true;
                            worksheet.Cells["N" + index].Value = groupChiPhiBangKe.Sum(o => o.NguoiBenhTuTra);

                            index++;
                        }
                        if (groupChiPhiBangKe.Key == NhomChiPhiBangKe.GoiVatTu)
                        {
                            var groupGoiVatTus = groupChiPhiBangKe.ToList().GroupBy(o => o.SoGoiVatTu.GetValueOrDefault()).OrderBy(o => o.Key);
                            int sttGoiVatTu = 1;
                            foreach (var groupGoiVatTu in groupGoiVatTus)
                            {
                                worksheet.Cells["A" + index + ":G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["A" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                worksheet.Cells["A" + index + ":G" + index].Merge = true;
                                worksheet.Cells["A" + index + ":G" + index].Value = "10." + sttGoiVatTu + ". " + groupChiPhiBangKe.Key.GetDescription() + " " + sttGoiVatTu;
                                worksheet.Cells["A" + index + ":G" + index].Style.Font.Bold = true;

                                worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["H" + index].Style.Font.Bold = true;
                                worksheet.Cells["H" + index].Value = groupChiPhiBangKe.Sum(o => o.ThanhTienBV);

                                worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["J" + index].Style.Font.Bold = true;
                                worksheet.Cells["J" + index].Value = groupChiPhiBangKe.Sum(o => o.ThanhTienBH);

                                worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["K" + index].Style.Font.Bold = true;
                                worksheet.Cells["K" + index].Value = groupChiPhiBangKe.Sum(o => o.QuyBHYT);

                                worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["L" + index].Style.Font.Bold = true;
                                worksheet.Cells["L" + index].Value = groupChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra);

                                worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["M" + index].Style.Font.Bold = true;
                                worksheet.Cells["M" + index].Value = groupChiPhiBangKe.Sum(o => o.Khac);

                                worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["N" + index].Style.Font.Bold = true;
                                worksheet.Cells["N" + index].Value = groupChiPhiBangKe.Sum(o => o.NguoiBenhTuTra);

                                index++;
                                foreach (var chiPhiBangKe in groupGoiVatTu)
                                {
                                    worksheet.Cells["A" + index + ":N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["A" + index + ":N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["A" + index + ":N" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));

                                    worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["A" + index].Value = indexItem;

                                    worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["B" + index].Value = chiPhiBangKe.NoiDung;

                                    worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["C" + index].Value = chiPhiBangKe.DonViTinh;

                                    worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["D" + index].Value = chiPhiBangKe.SoLuong;

                                    worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["E" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["E" + index].Value = chiPhiBangKe.DonGiaBV;

                                    worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["F" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["F" + index].Value = chiPhiBangKe.DonGiaBH;

                                    worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["G" + index].Value = chiPhiBangKe.TiLeThanhToanTheoDV;

                                    worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["H" + index].Value = chiPhiBangKe.ThanhTienBV;

                                    worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["I" + index].Value = Convert.ToDouble(chiPhiBangKe.MucHuongBaoHiem > 0 ? 100 : 0).ApplyFormatMoneyToDouble();


                                    worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["J" + index].Value = chiPhiBangKe.ThanhTienBH;


                                    worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["K" + index].Value = chiPhiBangKe.QuyBHYT;


                                    worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["L" + index].Value = chiPhiBangKe.NguoiBenhCungChiTra;


                                    worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["M" + index].Value = chiPhiBangKe.Khac;


                                    worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["N" + index].Value = chiPhiBangKe.NguoiBenhTuTra;

                                    indexItem++;
                                    index++;
                                }
                            }
                        }
                        else
                        {
                            foreach (var chiPhiBangKe in groupChiPhiBangKe)
                            {
                                worksheet.Cells["A" + index + ":N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index + ":N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["A" + index + ":N" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));

                                worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["A" + index].Value = indexItem;

                                worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["B" + index].Value = chiPhiBangKe.NoiDung;

                                worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["C" + index].Value = chiPhiBangKe.DonViTinh;

                                worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["D" + index].Value = chiPhiBangKe.SoLuong;

                                worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["E" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["E" + index].Value = chiPhiBangKe.DonGiaBV;

                                worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["F" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["F" + index].Value = chiPhiBangKe.DonGiaBH;

                                worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["G" + index].Value = chiPhiBangKe.TiLeThanhToanTheoDV;

                                worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["H" + index].Value = chiPhiBangKe.ThanhTienBV;

                                worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["I" + index].Value = Convert.ToDouble(chiPhiBangKe.MucHuongBaoHiem > 0 ? 100 : 0).ApplyFormatMoneyToDouble();


                                worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["J" + index].Value = chiPhiBangKe.ThanhTienBH;


                                worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["K" + index].Value = chiPhiBangKe.QuyBHYT;


                                worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["L" + index].Value = chiPhiBangKe.NguoiBenhCungChiTra;


                                worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["M" + index].Value = chiPhiBangKe.Khac;


                                worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["N" + index].Value = chiPhiBangKe.NguoiBenhTuTra;

                                indexItem++;
                                index++;
                            }
                        }

                    }

                    worksheet.Cells["A" + index + ":G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["A" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells["A" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                    worksheet.Cells["A" + index + ":G" + index].Merge = true;
                    worksheet.Cells["A" + index + ":G" + index].Value = "Tổng cộng";
                    worksheet.Cells["A" + index + ":G" + index].Style.Font.Bold = true;

                    worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["H" + index].Value = groupChiPhiBangKes.Select(c => c.Sum(o => o.ThanhTienBV)).Sum();


                    worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["J" + index].Value = groupChiPhiBangKes.Select(c => c.Sum(o => o.ThanhTienBH)).Sum();

                    worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["K" + index].Value = groupChiPhiBangKes.Select(c => c.Sum(o => o.QuyBHYT)).Sum();

                    worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["L" + index].Value = groupChiPhiBangKes.Select(c => c.Sum(o => o.NguoiBenhCungChiTra)).Sum();

                    worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["M" + index].Value = groupChiPhiBangKes.Select(c => c.Sum(o => o.Khac)).Sum();

                    worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["N" + index].Value = groupChiPhiBangKes.Select(c => c.Sum(o => o.NguoiBenhTuTra)).Sum();

                    index++;

                    worksheet.Cells["A" + index + ":N" + index].Merge = true;
                    worksheet.Cells["A" + index + ":N" + index].Value = "Tổng chi phí lần khám bệnh/cả đợt điều trị: " + Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.ThanhTienBV)).ApplyFormatMoneyToDouble() + " đồng";
                    worksheet.Cells["A" + index + ":N" + index].Style.Font.Bold = true;

                    index++;

                    worksheet.Cells["A" + index + ":N" + index].Merge = true;
                    worksheet.Cells["A" + index + ":N" + index].Value = "(Viết bằng chữ: " + NumberHelper.ChuyenSoRaText(Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.ThanhTienBV))) + ")";

                    index++;

                    worksheet.Cells["A" + index + ":N" + index].Merge = true;
                    worksheet.Cells["A" + index + ":N" + index].Value = "Trong đó , số tiền do:";
                    worksheet.Cells["A" + index + ":N" + index].Style.Font.Bold = true;

                    index++;
                    worksheet.Cells["A" + index + ":N" + index].Merge = true;
                    worksheet.Cells["A" + index + ":N" + index].Value = "- Quỹ BHYT thanh toán: " + Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.QuyBHYT)).ApplyFormatMoneyToDouble();

                    index++;
                    worksheet.Cells["A" + index + ":N" + index].Merge = true;
                    worksheet.Cells["A" + index + ":N" + index].Value = "- Người bệnh trả trong đó: " + (Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra)) + Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.NguoiBenhTuTra))).ApplyFormatMoneyToDouble();

                    index++;
                    worksheet.Cells["A" + index + ":N" + index].Merge = true;
                    worksheet.Cells["A" + index + ":N" + index].Value = "+ Cùng chi trả trong phạm vi BHYT: " + Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra)).ApplyFormatMoneyToDouble(true);

                    index++;
                    worksheet.Cells["A" + index + ":N" + index].Merge = true;
                    worksheet.Cells["A" + index + ":N" + index].Value = "+ Các khoản phải trả khác: " + Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.NguoiBenhTuTra)).ApplyFormatMoneyToDouble(true);

                    index++;
                    worksheet.Cells["A" + index + ":N" + index].Merge = true;
                    worksheet.Cells["A" + index + ":N" + index].Value = "+ Nguồn khác: " + Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.Khac)).ApplyFormatMoneyToDouble(true);


                    index = index + 2;

                    worksheet.Cells["D" + index + ":F" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                    worksheet.Cells["D" + index + ":F" + index].Merge = true;
                    worksheet.Cells["D" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["D" + index + ":F" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    worksheet.Cells["D" + index + ":F" + index].Value = $"Ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}";

                    worksheet.Cells["G" + index + ":I" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                    worksheet.Cells["G" + index + ":I" + index].Merge = true;
                    worksheet.Cells["G" + index + ":I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["G" + index + ":I" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    worksheet.Cells["G" + index + ":I" + index].Value = $"Ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}";

                    worksheet.Cells["J" + index + ":N" + index].Style.Font.Bold = true;
                    worksheet.Cells["J" + index + ":N" + index].Merge = true;
                    worksheet.Cells["J" + index + ":N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["J" + index + ":N" + index].Value = "XÁC NHẬN CỦA NGƯỜI BỆNH ";

                    index++;

                    worksheet.Cells["A" + index + ":C" + index].Style.Font.Bold = true;
                    worksheet.Cells["A" + index + ":C" + index].Merge = true;
                    worksheet.Cells["A" + index + ":C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A" + index + ":C" + index].Value = "NGƯỜI LẬP BẢNG KÊ";

                    worksheet.Cells["D" + index + ":F" + index].Style.Font.Bold = true;
                    worksheet.Cells["D" + index + ":F" + index].Merge = true;
                    worksheet.Cells["D" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["D" + index + ":F" + index].Value = "KẾ TOÁN VIỆN PHÍ";

                    worksheet.Cells["G" + index + ":I" + index].Style.Font.Bold = true;
                    worksheet.Cells["G" + index + ":I" + index].Merge = true;
                    worksheet.Cells["G" + index + ":I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["G" + index + ":I" + index].Value = "GIÁM ĐỊNH BHYT";

                    worksheet.Cells["J" + index + ":N" + index].Style.Font.Bold = true;
                    worksheet.Cells["J" + index + ":N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["J" + index + ":N" + index].Merge = true;
                    worksheet.Cells["J" + index + ":N" + index].Value = "(Ký , Ghi rõ họ tên)";

                    index++;

                    worksheet.Cells["A" + index + ":C" + index].Style.Font.Bold = true;
                    worksheet.Cells["A" + index + ":C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A" + index + ":C" + index].Merge = true;
                    worksheet.Cells["A" + index + ":C" + index].Value = "(Ký , Ghi rõ họ tên)";


                    worksheet.Cells["D" + index + ":F" + index].Style.Font.Bold = true;
                    worksheet.Cells["D" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["D" + index + ":F" + index].Merge = true;
                    worksheet.Cells["D" + index + ":F" + index].Value = "(Ký , Ghi rõ họ tên)";

                    worksheet.Cells["G" + index + ":I" + index].Style.Font.Bold = true;
                    worksheet.Cells["G" + index + ":I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["G" + index + ":I" + index].Merge = true;
                    worksheet.Cells["G" + index + ":I" + index].Value = "(Ký , Ghi rõ họ tên)";

                    worksheet.Cells["J" + index + ":N" + index].Style.Font.Bold = true;
                    worksheet.Cells["J" + index + ":N" + index].Merge = true;
                    worksheet.Cells["J" + index + ":N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["J" + index + ":N" + index].Value = "(Tôi đã nhận...phim...Xquang / CT / MRI)";

                    xlPackage.Save();
                }

                return stream.ToArray();
            }
        }

        #endregion

        #region Xem bảng kê chờ thu

        public byte[] XuatBangKeNgoaiTruChoThuExcel(ThuPhiKhamChuaBenhVo thuPhiKhamChuaBenhVo)
        {
            var yeuCauTiepNhan = BaseRepository.TableNoTracking.Where(c => c.Id == thuPhiKhamChuaBenhVo.Id)
                 .Include(xx => xx.BenhNhan)
                 .Include(xx => xx.YeuCauKhamBenhs).ThenInclude(o => o.NoiThucHien)
                 .Include(xx => xx.YeuCauKhamBenhs).ThenInclude(o => o.BenhVienChuyenVien)
                 .Include(xx => xx.YeuCauKhamBenhs).ThenInclude(o => o.Icdchinh)
                 .Include(xx => xx.YeuCauKhamBenhs).ThenInclude(o => o.YeuCauKhamBenhICDKhacs).ThenInclude(o => o.ICD)
                 .Include(cc => cc.PhuongXa)
                 .Include(cc => cc.QuanHuyen)
                 .Include(cc => cc.TinhThanh)
                 .Include(cc => cc.NoiChuyen).FirstOrDefault();
            
            if (yeuCauTiepNhan == null)
                return null;

            var benhVien = new Core.Domain.Entities.BenhVien.BenhVien();

            string maBHYT = string.Empty;
            string bHYTTuNgay = string.Empty;
            string bHYTDenNgay = string.Empty;
            string mucHuong = string.Empty;
            string NoiDKKCBBanDau = string.Empty;
            string MaKCBBanDau = string.Empty;


            string maICDKemTheo = string.Empty;
            string tenICDKemTheo = string.Empty;

            var yeuCauKhamBenh = yeuCauTiepNhan.YeuCauKhamBenhs.Where(cc => cc.IcdchinhId != null).OrderBy(cc => cc.ThoiDiemHoanThanh).LastOrDefault();

            if (yeuCauKhamBenh != null)
            {
                var maKemTheos = yeuCauKhamBenh.YeuCauKhamBenhICDKhacs.Select(c => c.ICD.Ma).ToList();
                var chuGhiKemTheos = yeuCauKhamBenh.YeuCauKhamBenhICDKhacs.Select(c => c.GhiChu).ToList();

                maICDKemTheo = string.Join(", ", maKemTheos);
                tenICDKemTheo = string.Join(", ", chuGhiKemTheos);
            }

            if (yeuCauTiepNhan.CoBHYT == true)
            {
                if (!String.IsNullOrEmpty(yeuCauTiepNhan.BHYTMaDKBD))
                {
                    benhVien = _benhVienRepository.TableNoTracking.FirstOrDefault(p => p.Ma.Equals(yeuCauTiepNhan.BHYTMaDKBD));
                }
                maBHYT = yeuCauTiepNhan.BHYTMaSoThe;
                bHYTTuNgay = yeuCauTiepNhan.BHYTNgayHieuLuc?.ApplyFormatDate();
                bHYTDenNgay = yeuCauTiepNhan.BHYTNgayHetHan?.ApplyFormatDate();
                mucHuong = yeuCauTiepNhan.BHYTMucHuong.ToString() + "%";
                NoiDKKCBBanDau = benhVien != null ? benhVien?.Ten : "Chưa xác định";
                MaKCBBanDau = yeuCauTiepNhan.BHYTMaDKBD;
            }
            else
            {
                maBHYT = "";
                bHYTTuNgay = "";
                bHYTDenNgay = "";
                mucHuong = "0%";
                NoiDKKCBBanDau = "Chưa xác định";
                MaKCBBanDau = "Chưa xác định";
            }
            List<ChiPhiKhamChuaBenhVo> chiPhis = new List<ChiPhiKhamChuaBenhVo>();

            var tatCaChiPhis = GetTatCaDichVuKhamChuaBenh(thuPhiKhamChuaBenhVo.Id).Result.ToList();


            //if (chiPhis.Count == 0)
            //{
            //    return string.Empty;
            //}

            //tinh miễn giảm theo FE
            var dsChiPhiDaChon = thuPhiKhamChuaBenhVo.DanhSachChiPhiKhamChuaBenhDaChons;
            foreach (var chiPhiKhamChuaBenhVo in dsChiPhiDaChon)
            {
                //var chiphi = dsChiPhi.FirstOrDefault(o => o.LoaiNhom == chiPhiKhamChuaBenhVo.LoaiNhom && o.Id == chiPhiKhamChuaBenhVo.Id);
                //if (chiphi == null || !chiphi.ThanhTien.AlmostEqual(chiPhiKhamChuaBenhVo.ThanhTien) || !chiphi.BHYTThanhToan.AlmostEqual(chiPhiKhamChuaBenhVo.BHYTThanhToan) || chiPhiKhamChuaBenhVo.TongCongNo > chiPhiKhamChuaBenhVo.ThanhTien - chiPhiKhamChuaBenhVo.BHYTThanhToan)
                //{
                //    return new KetQuaThuPhiKhamChuaBenhNoiTruVaQuyetToanDichVuTrongGoiVo { Error = "Thông tin dịch vụ thanh toán không hợp lệ, vui lòng tải lại trang" };
                //}
                var soTienTruocMienGiam = chiPhiKhamChuaBenhVo.ThanhTien - chiPhiKhamChuaBenhVo.BHYTThanhToan;

                decimal soTienMienGiamTheoDv = 0;

                foreach (var mienGiamTheoTiLe in chiPhiKhamChuaBenhVo.DanhSachMienGiamVos.Where(o => o.LoaiChietKhau == Enums.LoaiChietKhau.ChietKhauTheoTiLe))
                {
                    mienGiamTheoTiLe.SoTien = Math.Round((soTienTruocMienGiam * mienGiamTheoTiLe.TiLe.GetValueOrDefault() / 100), 2, MidpointRounding.AwayFromZero);
                    soTienMienGiamTheoDv += mienGiamTheoTiLe.SoTien;
                }
                foreach (var mienGiamTheoTiLe in chiPhiKhamChuaBenhVo.DanhSachMienGiamVos.Where(o => o.LoaiChietKhau == Enums.LoaiChietKhau.ChietKhauTheoSoTien))
                {
                    soTienMienGiamTheoDv += mienGiamTheoTiLe.SoTien;
                }
                //if (soTienMienGiamTheoDv > chiPhiKhamChuaBenhVo.ThanhTien)
                //{
                //    return new KetQuaThuPhiKhamChuaBenhNoiTruVaQuyetToanDichVuTrongGoiVo { Error = "Thông tin dịch vụ thanh toán không hợp lệ, vui lòng tải lại trang" };
                //}
                chiPhiKhamChuaBenhVo.SoTienMG = soTienMienGiamTheoDv;
            }
            foreach (var chiPhiKhamChuaBenhVo in tatCaChiPhis)
            {
                var chiPhiDaChon = dsChiPhiDaChon.FirstOrDefault(o => o.LoaiNhom == chiPhiKhamChuaBenhVo.LoaiNhom && o.Id == chiPhiKhamChuaBenhVo.Id);
                if (chiPhiDaChon != null)
                {
                    chiPhiKhamChuaBenhVo.SoTienMG = chiPhiDaChon.SoTienMG;
                    chiPhis.Add(chiPhiKhamChuaBenhVo);
                }
                else
                {

                }
            }

            var dsChiPhiBangKe = new List<BangKeKhamBenhBenhVienVo>();
            var dateItemChiPhis = string.Empty;
            int indexItem = 1;

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {

                    var worksheet = xlPackage.Workbook.Worksheets.Add("BẢNG KÊ CHI PHÍ KHÁM CHỮA BỆNH CHỜ THU");

                    // set row
                    worksheet.Row(9).Height = 24.5;
                    worksheet.DefaultRowHeight = 16;

                    //SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 10;
                    worksheet.Column(2).Width = 25;
                    worksheet.Column(3).Width = 15;
                    worksheet.Column(4).Width = 15;
                    worksheet.Column(5).Width = 15;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 15;
                    worksheet.Column(8).Width = 15;
                    worksheet.Column(9).Width = 15;
                    worksheet.Column(10).Width = 15;
                    worksheet.Column(11).Width = 15;
                    worksheet.Column(12).Width = 15;
                    worksheet.Column(13).Width = 15;
                    worksheet.Column(14).Width = 15;

                    worksheet.DefaultColWidth = 7;

                    using (var range = worksheet.Cells["A1:C1"])
                    {
                        range.Worksheet.Cells["A1:C1"].Merge = true;
                        range.Worksheet.Cells["A1:C1"].Value = "CƠ SỞ KHÁM CHỮA BỆNH : BỆNH VIỆN ĐKQT BẮC HÀ"; 
                        range.Worksheet.Cells["A1:C1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:C1"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A1:C1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:C1"].Style.Font.Color.SetColor(Color.Black);
                    }

                    //using (var range = worksheet.Cells["A2:C2"])
                    //{
                    //    range.Worksheet.Cells["A2:C2"].Merge = true;
                    //    range.Worksheet.Cells["A2:C2"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                    //    range.Worksheet.Cells["A2:C2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    //    range.Worksheet.Cells["A2:C2"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    //    range.Worksheet.Cells["A2:C2"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    //    range.Worksheet.Cells["A2:C2"].Style.Font.Color.SetColor(Color.Black);
                    //    range.Worksheet.Cells["A2:C2"].Style.Font.Bold = true;
                    //}

                    using (var range = worksheet.Cells["L1:N1"])
                    {
                        range.Worksheet.Cells["L1:N1"].Merge = true;
                        range.Worksheet.Cells["L1:N1"].Value = "Mẫu số : 01/KBCB ";
                        range.Worksheet.Cells["L1:N1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        range.Worksheet.Cells["L1:N1"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["L1:N1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["L1:N1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["L1:N1"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["L2:N2"])
                    {
                        range.Worksheet.Cells["L2:N2"].Merge = true;
                        range.Worksheet.Cells["L2:N2"].Value = "Mã số người bệnh: " + yeuCauTiepNhan.BenhNhan.MaBN;
                        range.Worksheet.Cells["L2:N2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        range.Worksheet.Cells["L2:N2"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["L2:N2"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["L2:N2"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["L2:N2"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["L3:N3"])
                    {
                        range.Worksheet.Cells["L3:N3"].Merge = true;
                        range.Worksheet.Cells["L3:N3"].Value = "Số khám bệnh:" + yeuCauTiepNhan.MaYeuCauTiepNhan;
                        range.Worksheet.Cells["L3:N3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        range.Worksheet.Cells["L3:N3"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["L3:N3"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["L3:N3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["L3:N3"].Style.Font.Bold = true;
                    }



                    using (var range = worksheet.Cells["A4:N5"])
                    {
                        range.Worksheet.Cells["A4:N5"].Merge = true;
                        range.Worksheet.Cells["A4:N5"].Value = "BẢNG KÊ CHI PHÍ KHÁM CHỮA BỆNH CHỜ THU";
                        range.Worksheet.Cells["A4:N5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:N5"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:N5"].Style.Font.SetFromFont(new Font("Times New Roman", 18));
                        range.Worksheet.Cells["A4:N5"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:N5"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A6:N6"])
                    {
                        range.Worksheet.Cells["A6:N6"].Merge = true;
                        range.Worksheet.Cells["A6:N6"].Value = "I.Hành chính";
                        range.Worksheet.Cells["A6:N6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A6:N6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A6:N6"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A6:N6"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A6:N6"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A7:F7"])
                    {
                        range.Worksheet.Cells["A7:F7"].Merge = true;

                        range.Worksheet.Cells["A7:F7"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A7:F7"].RichText.Add("(1) Họ tên người bệnh: ");
                        var value = range.Worksheet.Cells["A7:F7"].RichText.Add(yeuCauTiepNhan.HoTen);
                        value.Bold = true;

                        range.Worksheet.Cells["A7:F7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A7:F7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A7:F7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));


                        range.Worksheet.Cells["A7:F7"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G7:J7"])
                    {
                        range.Worksheet.Cells["G7:J7"].Merge = true;
                        range.Worksheet.Cells["G7:J7"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G7:J7"].RichText.Add("Ngày,tháng,năm sinh: ");
                        var value = range.Worksheet.Cells["G7:J7"].RichText.Add(DateHelper.DOBFormat(yeuCauTiepNhan.NgaySinh, yeuCauTiepNhan.ThangSinh, yeuCauTiepNhan.NamSinh));
                        value.Bold = true;

                        range.Worksheet.Cells["G7:J7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G7:J7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G7:J7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G7:J7"].Style.Font.Color.SetColor(Color.Black);
                    }

                    var gioitinh = yeuCauTiepNhan.GioiTinh != null ? yeuCauTiepNhan.GioiTinh.GetDescription() : "";
                    using (var range = worksheet.Cells["K7:N7"])
                    {
                        range.Worksheet.Cells["K7:N7"].Merge = true;

                        range.Worksheet.Cells["K7:N7"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["K7:N7"].RichText.Add("Giới tính: ");
                        var value = range.Worksheet.Cells["K7:N7"].RichText.Add(gioitinh);
                        value.Bold = true;

                        range.Worksheet.Cells["K7:N7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["K7:N7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["K7:N7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["K7:N7"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["A8:F8"])
                    {
                        range.Worksheet.Cells["A8:F8"].Merge = true;

                        range.Worksheet.Cells["A8:F8"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A8:F8"].RichText.Add("(2)Địa chỉ hiện tại: ");
                        var value = range.Worksheet.Cells["A8:F8"].RichText.Add(yeuCauTiepNhan.DiaChiDayDu);
                        value.Bold = true;

                        range.Worksheet.Cells["A8:F8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A8:F8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A8:F8"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A8:F8"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G8:J8"])
                    {
                        range.Worksheet.Cells["G8:J8"].Merge = true;

                        range.Worksheet.Cells["G8:J8"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G8:J8"].RichText.Add("(3)Mã khu vực(K1/K2/K3): ");
                        var value = range.Worksheet.Cells["G8:J8"].RichText.Add(yeuCauTiepNhan.BHYTMaKhuVuc);
                        value.Bold = true;

                        range.Worksheet.Cells["G8:J8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G8:J8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G8:J8"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G8:J8"].Style.Font.Color.SetColor(Color.Black);
                    }


                    using (var range = worksheet.Cells["A9:F9"])
                    {
                        range.Worksheet.Cells["A9:F9"].Merge = true;
                        range.Worksheet.Cells["A9:F9"].Style.WrapText = true;

                        var title = range.Worksheet.Cells["A9:F9"].RichText.Add("(4)Mã thẻ BHYT: ");
                        var value = range.Worksheet.Cells["A9:F9"].RichText.Add(maBHYT);
                        value.Bold = true;

                        range.Worksheet.Cells["A9:F9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A9:F9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A9:F9"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A9:F9"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G9:J9"])
                    {
                        range.Worksheet.Cells["G9:J9"].Merge = true;
                        range.Worksheet.Cells["G9:J9"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G9:J9"].RichText.Add("Giá trị từ: ");
                        var valuebHYTTuNgay = range.Worksheet.Cells["G9:J9"].RichText.Add(bHYTTuNgay);
                        valuebHYTTuNgay.Bold = true;

                        var den = range.Worksheet.Cells["G9:J9"].RichText.Add(" đến: ");
                        den.Bold = false;
                        var valuebHYTDenNgay = range.Worksheet.Cells["G9:J9"].RichText.Add(bHYTDenNgay);
                        valuebHYTDenNgay.Bold = true;


                        range.Worksheet.Cells["G9:J9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G9:J9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G9:J9"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G9:J9"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["A10:F10"])
                    {
                        range.Worksheet.Cells["A10:F10"].Merge = true;

                        range.Worksheet.Cells["A10:F10"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A10:F10"].RichText.Add("(5)Nơi ĐK KCB ban đầu: ");
                        var valueNoiDKKCBBanDau = range.Worksheet.Cells["A10:F10"].RichText.Add(NoiDKKCBBanDau);
                        valueNoiDKKCBBanDau.Bold = true;

                        range.Worksheet.Cells["A10:F10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A10:F10"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A10:F10"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A10:F10"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G10:J10"])
                    {
                        range.Worksheet.Cells["G10:J10"].Merge = true;
                        range.Worksheet.Cells["G10:J10"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G10:J10"].RichText.Add("(6)Mã: ");
                        var valueMaKCBBanDau = range.Worksheet.Cells["G10:J10"].RichText.Add(MaKCBBanDau);
                        valueMaKCBBanDau.Bold = true;

                        range.Worksheet.Cells["G10:J10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G10:J10"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G10:J10"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G10:J10"].Style.Font.Color.SetColor(Color.Black);
                    }


                    using (var range = worksheet.Cells["A11:F11"])
                    {
                        range.Worksheet.Cells["A11:F11"].Merge = true;

                        range.Worksheet.Cells["A11:F11"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A11:F11"].RichText.Add("(7)Đến Khám: ");
                        var value = range.Worksheet.Cells["A11:F11"].RichText.Add(yeuCauTiepNhan.ThoiDiemTiepNhan.ApplyFormatGioPhutNgay());
                        value.Bold = true;

                        range.Worksheet.Cells["A11:F11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A11:F11"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A11:F11"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A11:F11"].Style.Font.Color.SetColor(Color.Black);
                    }


                    using (var range = worksheet.Cells["A12:F12"])
                    {
                        range.Worksheet.Cells["A12:F12"].Merge = true;

                        range.Worksheet.Cells["A12:F12"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A12:F12"].RichText.Add("(8)Điều trị ngoại trú/nội trú từ: ");
                        var value = range.Worksheet.Cells["A12:F12"].RichText.Add(string.Empty);
                        value.Bold = true;

                        range.Worksheet.Cells["A12:F12"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A12:F12"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A12:F12"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A12:F12"].Style.Font.Color.SetColor(Color.Black);
                    }


                    using (var range = worksheet.Cells["A13:F13"])
                    {
                        range.Worksheet.Cells["A13:F13"].Merge = true;
                        range.Worksheet.Cells["A13:F13"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A13:F13"].RichText.Add("(9)Kết thúc khám/điều trị: ");
                        var value = range.Worksheet.Cells["A13:F13"].RichText.Add(yeuCauKhamBenh?.ThoiDiemHoanThanh?.ApplyFormatGioPhutNgay());
                        value.Bold = true;


                        range.Worksheet.Cells["A13:F13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A13:F13"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A13:F13"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A13:F13"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G13:J13"])
                    {
                        range.Worksheet.Cells["G13:J13"].Merge = true;
                        range.Worksheet.Cells["G13:J13"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G13:J13"].RichText.Add("Tổng số ngày điều trị: ");
                        var value = range.Worksheet.Cells["G13:J13"].RichText.Add(string.Empty);
                        value.Bold = true;

                        range.Worksheet.Cells["G13:J13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G13:J13"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G13:J13"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G13:J13"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["K13:N13"])
                    {
                        range.Worksheet.Cells["K13:N13"].Merge = true;

                        range.Worksheet.Cells["K13:N13"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["K13:N13"].RichText.Add("(10)Tình trạng ra viện: ");
                        var value = range.Worksheet.Cells["K13:N13"].RichText.Add("1");
                        value.Bold = true;


                        range.Worksheet.Cells["K13:N13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["K13:N13"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["K13:N13"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["K13:N13"].Style.Font.Color.SetColor(Color.Black);
                    }

                    //=================================14=================================
                    var coCapCuu = yeuCauTiepNhan.LyDoVaoVien == Enums.EnumLyDoVaoVien.CapCuu ? "X" : string.Empty;
                    var coDungTuyen = yeuCauTiepNhan.LyDoVaoVien == Enums.EnumLyDoVaoVien.DungTuyen ? "X" : string.Empty;
                    var coThongTuyen = yeuCauTiepNhan.LyDoVaoVien == Enums.EnumLyDoVaoVien.ThongTuyen ? "X" : string.Empty;
                    var coTraiTuyen = yeuCauTiepNhan.LyDoVaoVien == Enums.EnumLyDoVaoVien.TraiTuyen ? "X" : string.Empty;

                    using (var range = worksheet.Cells["A14:C14"])
                    {
                        range.Worksheet.Cells["A14:C14"].Merge = true;

                        range.Worksheet.Cells["A14:C14"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A14:C14"].RichText.Add("(11) Cấp cứu: ");
                        var value = range.Worksheet.Cells["A14:C14"].RichText.Add(coCapCuu);
                        value.Bold = true;

                        range.Worksheet.Cells["A14:C14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A14:C14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A14:C14"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A14:C14"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["D14:F14"])
                    {
                        range.Worksheet.Cells["D14:F14"].Merge = true;
                        range.Worksheet.Cells["D14:F14"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["D14:F14"].RichText.Add("(12) Đúng tuyến: ");
                        var value = range.Worksheet.Cells["D14:F14"].RichText.Add(coDungTuyen);
                        value.Bold = true;

                        range.Worksheet.Cells["D14:F14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["D14:F14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["D14:F14"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["D14:F14"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G14:J14"])
                    {
                        range.Worksheet.Cells["G14:J14"].Merge = true;
                        range.Worksheet.Cells["G14:J14"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G14:J14"].RichText.Add("Nơi chuyến đến: ");
                        var value = range.Worksheet.Cells["G14:J14"].RichText.Add(yeuCauTiepNhan.NoiChuyen?.Ten);
                        value.Bold = true;
                        range.Worksheet.Cells["G14:J14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G14:J14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G14:J14"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G14:J14"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["K14:N14"])
                    {
                        range.Worksheet.Cells["K14:N14"].Merge = true;
                        range.Worksheet.Cells["K14:N14"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["K14:N14"].RichText.Add("Nơi chuyển đi: ");
                        var value = range.Worksheet.Cells["K14:N14"].RichText.Add(yeuCauKhamBenh?.BenhVienChuyenVien?.Ten);
                        value.Bold = true;
                        range.Worksheet.Cells["K14:N14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["K14:N14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["K14:N14"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["K14:N14"].Style.Font.Color.SetColor(Color.Black);
                    }

                    //=================================15=================================
                    using (var range = worksheet.Cells["A15:C15"])
                    {
                        range.Worksheet.Cells["A15:C15"].Merge = true;
                        range.Worksheet.Cells["A15:C15"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A15:C15"].RichText.Add("(13)Thông tuyến: ");
                        var value = range.Worksheet.Cells["A15:C15"].RichText.Add(coThongTuyen);
                        value.Bold = true;
                        range.Worksheet.Cells["A15:C15"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A15:C15"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A15:C15"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A15:C15"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["D15:F15"])
                    {
                        range.Worksheet.Cells["D15:F15"].Merge = true;
                        range.Worksheet.Cells["D15:F15"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["D15:F15"].RichText.Add("(14)Trái tuyến: ");
                        var value = range.Worksheet.Cells["D15:F15"].RichText.Add(coTraiTuyen);
                        value.Bold = true;

                        range.Worksheet.Cells["D15:F15"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["D15:F15"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["D15:F15"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["D15:F15"].Style.Font.Color.SetColor(Color.Black);
                    }


                    //================================= 16 =================================

                    using (var range = worksheet.Cells["A16:F16"])
                    {
                        range.Worksheet.Cells["A16:F16"].Merge = true;
                        range.Worksheet.Cells["A16:F16"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A16:F16"].RichText.Add("(15)Chẩn đoán xác định: ");
                        var value = range.Worksheet.Cells["A16:F16"].RichText.Add(yeuCauKhamBenh?.GhiChuICDChinh);
                        value.Bold = true;

                        range.Worksheet.Cells["A16:F16"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A16:F16"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A16:F16"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A16:F16"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G16:N16"])
                    {
                        range.Worksheet.Cells["G16:N16"].Merge = true;

                        range.Worksheet.Cells["G16:N16"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G16:N16"].RichText.Add("(16) Mã bệnh: ");
                        var value = range.Worksheet.Cells["G16:N16"].RichText.Add(yeuCauKhamBenh?.Icdchinh?.Ma);
                        value.Bold = true;


                        range.Worksheet.Cells["G16:N16"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G16:N16"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G16:N16"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G16:N16"].Style.Font.Color.SetColor(Color.Black);
                    }

                    //================================= 17 =================================
                    using (var range = worksheet.Cells["A17:F17"])
                    {
                        range.Worksheet.Cells["A17:F17"].Merge = true;
                        range.Worksheet.Cells["A17:F17"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A17:F17"].RichText.Add("(17)Bệnh kèm theo: ");
                        var value = range.Worksheet.Cells["A17:F17"].RichText.Add(tenICDKemTheo);
                        value.Bold = true;


                        range.Worksheet.Cells["A17:F17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A17:F17"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A17:F17"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A17:F17"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G17:N17"])
                    {
                        range.Worksheet.Cells["G17:N17"].Merge = true;
                        range.Worksheet.Cells["G17:N17"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G17:N17"].RichText.Add("(18)Mã bệnh kèm theo: ");
                        var value = range.Worksheet.Cells["G17:N17"].RichText.Add(maICDKemTheo);
                        value.Bold = true;


                        range.Worksheet.Cells["G17:N17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G17:N17"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G17:N17"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G17:N17"].Style.Font.Color.SetColor(Color.Black);
                    }

                    //================================= 18 =================================
                    using (var range = worksheet.Cells["A18:F18"])
                    {
                        range.Worksheet.Cells["A18:F18"].Merge = true;
                        range.Worksheet.Cells["A18:F18"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A18:F18"].RichText.Add("(19)Thời điểm đủ 05 năm liên tục từ ngày: ");
                        var value = range.Worksheet.Cells["A18:F18"].RichText.Add(yeuCauTiepNhan.BHYTNgayDu5Nam?.ApplyFormatDate());
                        value.Bold = true;


                        range.Worksheet.Cells["A18:F18"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A18:F18"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A18:F18"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A18:F18"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G18:N18"])
                    {
                        range.Worksheet.Cells["G18:N18"].Merge = true;
                        range.Worksheet.Cells["G18:N18"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G18:N18"].RichText.Add("(21)Miễn cùng chi trả trong năm từ ngày: ");
                        var value = range.Worksheet.Cells["G18:N18"].RichText.Add(yeuCauTiepNhan.BHYTNgayDuocMienCungChiTra?.ApplyFormatDate());
                        value.Bold = true;

                        range.Worksheet.Cells["G18:N18"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G18:N18"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G18:N18"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G18:N18"].Style.Font.Color.SetColor(Color.Black);
                    }

                    //================================= 19 =================================

                    using (var range = worksheet.Cells["A19:N19"])
                    {
                        range.Worksheet.Cells["A19:N19"].Merge = true;
                        range.Worksheet.Cells["A19:N19"].Value = "II. Phần Chi phí khám bệnh, chữa bệnh";
                        range.Worksheet.Cells["A19:N19"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A19:N19"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A19:N19"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A19:N19"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A19:N19"].Style.Font.Bold = true;
                    }

                    //================================= 20 =========================================
                    using (var range = worksheet.Cells["A20:C20"])
                    {
                        range.Worksheet.Cells["A20:C20"].Merge = true;
                        range.Worksheet.Cells["A20:C20"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A20:C20"].RichText.Add("Mã thẻ BHYT: ");
                        var value = range.Worksheet.Cells["A20:C20"].RichText.Add(maBHYT);
                        value.Bold = true;

                        range.Worksheet.Cells["A20:C20"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A20:C20"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A20:C20"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A20:C20"].Style.Font.Color.SetColor(Color.Black);
                    }
                    using (var range = worksheet.Cells["D20:F20"])
                    {
                        range.Worksheet.Cells["D20:F20"].Merge = true;
                        range.Worksheet.Cells["D20:F20"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["D20:F20"].RichText.Add("Giá trị từ: ");
                        var valuebHYTTuNgay = range.Worksheet.Cells["D20:F20"].RichText.Add(bHYTTuNgay);
                        valuebHYTTuNgay.Bold = true;

                        var den = range.Worksheet.Cells["D20:F20"].RichText.Add(" đến: ");
                        den.Bold = false;
                        var valuebHYTDenNgay = range.Worksheet.Cells["D20:F20"].RichText.Add(bHYTDenNgay);
                        valuebHYTDenNgay.Bold = true;

                        range.Worksheet.Cells["D20:F20"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["D20:F20"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["D20:F20"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["D20:F20"].Style.Font.Color.SetColor(Color.Black);
                    }
                    using (var range = worksheet.Cells["G20:J20"])
                    {
                        range.Worksheet.Cells["G20:J20"].Merge = true;
                        range.Worksheet.Cells["G20:J20"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G20:J20"].RichText.Add("Mức hưởng: ");
                        var value = range.Worksheet.Cells["G20:J20"].RichText.Add(mucHuong);
                        value.Bold = true;


                        range.Worksheet.Cells["G20:J20"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G20:J20"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G20:J20"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G20:J20"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["A21:N22"])
                    {
                        range.Worksheet.Cells["A21:N22"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A21:N22"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A21:N22"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A21:N22"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A21:N22"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A21:N22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A21:A22"].Merge = true;
                        range.Worksheet.Cells["A21:A22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A21:A22"].Value = "STT";

                        range.Worksheet.Cells["B21:B22"].Merge = true;
                        range.Worksheet.Cells["B21:B22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B21:B22"].Value = "Nội dung";

                        range.Worksheet.Cells["C21:C22"].Merge = true;
                        range.Worksheet.Cells["C21:C22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C21:C22"].Value = "Đơn vị tính";

                        range.Worksheet.Cells["D21:D22"].Merge = true;
                        range.Worksheet.Cells["D21:D22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D21:D22"].Value = "Số lượng";

                        range.Worksheet.Cells["E21:E22"].Merge = true;
                        range.Worksheet.Cells["E21:E22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E21:E22"].Value = "Đơn giá BV(đồng)";

                        range.Worksheet.Cells["F21:F22"].Merge = true;
                        range.Worksheet.Cells["F21:F22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F21:F22"].Value = "Đơn giá BH(đồng)";

                        range.Worksheet.Cells["G21:G22"].Merge = true;
                        range.Worksheet.Cells["G21:G22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G21:G22"].Value = "Tỉ lệ thanh toán theo dịch vụ (%)";

                        range.Worksheet.Cells["H21:H22"].Merge = true;
                        range.Worksheet.Cells["H21:H22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H21:H22"].Value = "Thành tiền BV (đồng)";

                        range.Worksheet.Cells["I21:I22"].Merge = true;
                        range.Worksheet.Cells["I21:I22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I21:I22"].Value = "Tỷ lệ thanh toán BHYT (%)";

                        range.Worksheet.Cells["J21:J22"].Merge = true;
                        range.Worksheet.Cells["J21:J22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J21:J22"].Value = "Thành tiền BH(đồng)";

                        range.Worksheet.Cells["K21:N21"].Merge = true;
                        range.Worksheet.Cells["K21:N21"].Value = "Nguồn thanh toán (đồng)";

                        range.Worksheet.Cells["K22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K22"].Value = "Quỹ BHYT";

                        range.Worksheet.Cells["L22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["L22"].Value = "Người bệnh cùng chi trả (đồng)";

                        range.Worksheet.Cells["M22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["M22"].Value = "Khác (đồng)";

                        range.Worksheet.Cells["N22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["N22"].Value = "Người bệnh tự trả(đồng)";
                    }

                   
                    int index = 23;

                    foreach (var chiPhi in chiPhis)
                    {
                        var chiphiBangKe = new BangKeKhamBenhBenhVienVo
                        {
                            Nhom = chiPhi.NhomChiPhiBangKe,
                            Id = chiPhi.Id,
                            NoiDung = chiPhi.TenDichVu,
                            DonViTinh = chiPhi.DonViTinh,
                            SoLuong = (decimal)chiPhi.Soluong,
                            DuocHuongBaoHiem = chiPhi.DuocHuongBHYT,
                            DonGiaBH = chiPhi.DuocHuongBHYT ? chiPhi.DonGiaBHYT : 0,
                            MucHuongBaoHiem = chiPhi.MucHuongBaoHiem,
                            TiLeThanhToanTheoDV = chiPhi.TiLeBaoHiemThanhToan,
                            BaoHiemChiTra = true,
                            DonGiaBV = chiPhi.DonGia
                        };
                        if (chiPhi.KhongTinhPhi == true || chiPhi.YeuCauGoiDichVuId != null)
                        {
                            chiphiBangKe.Khac = chiPhi.MucHuongBaoHiem > 0
                                ? (chiphiBangKe.ThanhTienBV.GetValueOrDefault() - chiphiBangKe.ThanhTienBH.GetValueOrDefault())
                                : chiphiBangKe.ThanhTienBV.GetValueOrDefault();
                        }
                        else
                        {
                            chiphiBangKe.Khac = chiPhi.SoTienMG;
                        }
                        dsChiPhiBangKe.Add(chiphiBangKe);
                    }
                    var groupChiPhiBangKes = dsChiPhiBangKe.GroupBy(x => x.Nhom).OrderBy(o => (int)o.Key * (o.Key == NhomChiPhiBangKe.NgayGiuongDieuTriBanNgay || o.Key == NhomChiPhiBangKe.NgayGiuongDieuTriNoiTru ? 1 : 10));
                    if (!groupChiPhiBangKes.Any())
                    {
                        return null;
                    }
                    foreach (var groupChiPhiBangKe in groupChiPhiBangKes)
                    {
                        if (groupChiPhiBangKe.Key == NhomChiPhiBangKe.NgayGiuongDieuTriBanNgay)
                        {

                            worksheet.Cells["A" + index + ":H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["A" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + index + ":H" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            worksheet.Cells["A" + index + ":H" + index].Merge = true;
                            worksheet.Cells["A" + index + ":H" + index].Value = "2. Ngày giường:";
                            worksheet.Cells["A" + index + ":H" + index].Style.Font.Bold = true;

                            index++;

                            worksheet.Cells["A" + index + ":G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["A" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            worksheet.Cells["A" + index + ":G" + index].Merge = true;
                            worksheet.Cells["A" + index + ":G" + index].Value = "2.1." + groupChiPhiBangKe.Key.GetDescription();
                            worksheet.Cells["A" + index + ":G" + index].Style.Font.Bold = true;

                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["H" + index].Style.Font.Bold = true;
                            worksheet.Cells["H" + index].Value = groupChiPhiBangKe.Sum(o => o.ThanhTienBV);

                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["J" + index].Style.Font.Bold = true;
                            worksheet.Cells["J" + index].Value = groupChiPhiBangKe.Sum(o => o.ThanhTienBH);

                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["K" + index].Style.Font.Bold = true;
                            worksheet.Cells["K" + index].Value = groupChiPhiBangKe.Sum(o => o.QuyBHYT);

                            worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["L" + index].Style.Font.Bold = true;
                            worksheet.Cells["L" + index].Value = groupChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra);

                            worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["M" + index].Style.Font.Bold = true;
                            worksheet.Cells["M" + index].Value = groupChiPhiBangKe.Sum(o => o.Khac);

                            worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["N" + index].Style.Font.Bold = true;
                            worksheet.Cells["N" + index].Value = groupChiPhiBangKe.Sum(o => o.NguoiBenhTuTra);

                            index++;
                        }
                        else if (groupChiPhiBangKe.Key == NhomChiPhiBangKe.NgayGiuongDieuTriNoiTru)
                        {
                            if (!groupChiPhiBangKes.Any(o => o.Key == NhomChiPhiBangKe.NgayGiuongDieuTriBanNgay))
                            {
                                worksheet.Cells["A" + index + ":H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["A" + index + ":H" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                worksheet.Cells["A" + index + ":H" + index].Merge = true;
                                worksheet.Cells["A" + index + ":H" + index].Value = "2. Ngày giường:";
                                worksheet.Cells["A" + index + ":H" + index].Style.Font.Bold = true;
                            }

                            worksheet.Cells["A" + index + ":G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["A" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            worksheet.Cells["A" + index + ":G" + index].Merge = true;
                            worksheet.Cells["A" + index + ":G" + index].Value = "2.2." + groupChiPhiBangKe.Key.GetDescription();
                            worksheet.Cells["A" + index + ":G" + index].Style.Font.Bold = true;

                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["H" + index].Style.Font.Bold = true;
                            worksheet.Cells["H" + index].Value = groupChiPhiBangKe.Sum(o => o.ThanhTienBV);

                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["J" + index].Style.Font.Bold = true;
                            worksheet.Cells["J" + index].Value = groupChiPhiBangKe.Sum(o => o.ThanhTienBH);

                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["K" + index].Style.Font.Bold = true;
                            worksheet.Cells["K" + index].Value = groupChiPhiBangKe.Sum(o => o.QuyBHYT);

                            worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["L" + index].Style.Font.Bold = true;
                            worksheet.Cells["L" + index].Value = groupChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra);

                            worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["M" + index].Style.Font.Bold = true;
                            worksheet.Cells["M" + index].Value = groupChiPhiBangKe.Sum(o => o.Khac);

                            worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["N" + index].Style.Font.Bold = true;
                            worksheet.Cells["N" + index].Value = groupChiPhiBangKe.Sum(o => o.NguoiBenhTuTra);

                            index++; 
                        }
                        else
                        {
                            worksheet.Cells["A" + index + ":G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["A" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            worksheet.Cells["A" + index + ":G" + index].Merge = true;
                            worksheet.Cells["A" + index + ":G" + index].Value = (int)groupChiPhiBangKe.Key + "." + groupChiPhiBangKe.Key.GetDescription();
                            worksheet.Cells["A" + index + ":G" + index].Style.Font.Bold = true;

                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["H" + index].Style.Font.Bold = true;
                            worksheet.Cells["H" + index].Value = groupChiPhiBangKe.Sum(o => o.ThanhTienBV);

                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["J" + index].Style.Font.Bold = true;
                            worksheet.Cells["J" + index].Value = groupChiPhiBangKe.Sum(o => o.ThanhTienBH);

                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["K" + index].Style.Font.Bold = true;
                            worksheet.Cells["K" + index].Value = groupChiPhiBangKe.Sum(o => o.QuyBHYT);

                            worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["L" + index].Style.Font.Bold = true;
                            worksheet.Cells["L" + index].Value = groupChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra);

                            worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["M" + index].Style.Font.Bold = true;
                            worksheet.Cells["M" + index].Value = groupChiPhiBangKe.Sum(o => o.Khac);

                            worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["N" + index].Style.Font.Bold = true;
                            worksheet.Cells["N" + index].Value = groupChiPhiBangKe.Sum(o => o.NguoiBenhTuTra);

                            index++;
                        }
                        if (groupChiPhiBangKe.Key == NhomChiPhiBangKe.GoiVatTu)
                        {
                            var groupGoiVatTus = groupChiPhiBangKe.ToList().GroupBy(o => o.SoGoiVatTu.GetValueOrDefault()).OrderBy(o => o.Key);
                            int sttGoiVatTu = 1;
                            foreach (var groupGoiVatTu in groupGoiVatTus)
                            {
                                worksheet.Cells["A" + index + ":G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["A" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                worksheet.Cells["A" + index + ":G" + index].Value = "10." + sttGoiVatTu + ". " + groupChiPhiBangKe.Key.GetDescription() + " " + sttGoiVatTu;
                                worksheet.Cells["A" + index + ":G" + index].Style.Font.Bold = true;
                                worksheet.Cells["A" + index + ":G" + index].Merge = true;

                                worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["H" + index].Style.Font.Bold = true;
                                worksheet.Cells["H" + index].Value = groupChiPhiBangKe.Sum(o => o.ThanhTienBV);

                                worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["J" + index].Style.Font.Bold = true;
                                worksheet.Cells["J" + index].Value = groupChiPhiBangKe.Sum(o => o.ThanhTienBH);

                                worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["K" + index].Style.Font.Bold = true;
                                worksheet.Cells["K" + index].Value = groupChiPhiBangKe.Sum(o => o.QuyBHYT);

                                worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["L" + index].Style.Font.Bold = true;
                                worksheet.Cells["L" + index].Value = groupChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra);

                                worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["M" + index].Style.Font.Bold = true;
                                worksheet.Cells["M" + index].Value = groupChiPhiBangKe.Sum(o => o.Khac);

                                worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["N" + index].Style.Font.Bold = true;
                                worksheet.Cells["N" + index].Value = groupChiPhiBangKe.Sum(o => o.NguoiBenhTuTra);

                                index++;

                                foreach (var chiPhiBangKe in groupGoiVatTu)
                                {
                                    worksheet.Cells["A" + index + ":N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["A" + index + ":N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["A" + index + ":N" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));

                                    worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["A" + index].Value = indexItem;

                                    worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["B" + index].Value = chiPhiBangKe.NoiDung;

                                    worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["C" + index].Value = chiPhiBangKe.DonViTinh;

                                    worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["D" + index].Value = chiPhiBangKe.SoLuong;

                                    worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["E" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["E" + index].Value = chiPhiBangKe.DonGiaBV;

                                    worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["F" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["F" + index].Value = chiPhiBangKe.DonGiaBH;

                                    worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["G" + index].Value = chiPhiBangKe.TiLeThanhToanTheoDV;

                                    worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["H" + index].Value = chiPhiBangKe.ThanhTienBV;

                                    worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["I" + index].Value = Convert.ToDouble(chiPhiBangKe.MucHuongBaoHiem > 0 ? 100 : 0).ApplyFormatMoneyToDouble();


                                    worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["J" + index].Value = chiPhiBangKe.ThanhTienBH;


                                    worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["K" + index].Value = chiPhiBangKe.QuyBHYT;


                                    worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["L" + index].Value = chiPhiBangKe.NguoiBenhCungChiTra;


                                    worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["M" + index].Value = chiPhiBangKe.Khac;


                                    worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["N" + index].Value = chiPhiBangKe.NguoiBenhTuTra;

                                    indexItem++;
                                    index++;
                                }
                            }
                        }
                        else
                        {
                            foreach (var chiPhiBangKe in groupChiPhiBangKe)
                            {
                                worksheet.Cells["A" + index + ":N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index + ":N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["A" + index + ":N" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));

                                worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["A" + index].Value = indexItem;

                                worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["B" + index].Value = chiPhiBangKe.NoiDung;

                                worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["C" + index].Value = chiPhiBangKe.DonViTinh;

                                worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["D" + index].Value = chiPhiBangKe.SoLuong;

                                worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["E" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["E" + index].Value = chiPhiBangKe.DonGiaBV;

                                worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["F" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["F" + index].Value = chiPhiBangKe.DonGiaBH;

                                worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["G" + index].Value = chiPhiBangKe.TiLeThanhToanTheoDV;

                                worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["H" + index].Value = chiPhiBangKe.ThanhTienBV;

                                worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["I" + index].Value = Convert.ToDouble(chiPhiBangKe.MucHuongBaoHiem > 0 ? 100 : 0).ApplyFormatMoneyToDouble();


                                worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["J" + index].Value = chiPhiBangKe.ThanhTienBH;


                                worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["K" + index].Value = chiPhiBangKe.QuyBHYT;


                                worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["L" + index].Value = chiPhiBangKe.NguoiBenhCungChiTra;


                                worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["M" + index].Value = chiPhiBangKe.Khac;


                                worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["N" + index].Value = chiPhiBangKe.NguoiBenhTuTra;

                                indexItem++;
                                index++;
                            }
                        }

                    }

                    worksheet.Cells["A" + index + ":G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["A" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells["A" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                    worksheet.Cells["A" + index + ":G" + index].Merge = true;
                    worksheet.Cells["A" + index + ":G" + index].Value = "Tổng cộng";
                    worksheet.Cells["A" + index + ":G" + index].Style.Font.Bold = true;

                    worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["H" + index].Value = groupChiPhiBangKes.Select(c => c.Sum(o => o.ThanhTienBV)).Sum();


                    worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["J" + index].Value = groupChiPhiBangKes.Select(c => c.Sum(o => o.ThanhTienBH)).Sum();

                    worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["K" + index].Value = groupChiPhiBangKes.Select(c => c.Sum(o => o.QuyBHYT)).Sum();

                    worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["L" + index].Value = groupChiPhiBangKes.Select(c => c.Sum(o => o.NguoiBenhCungChiTra)).Sum();

                    worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["M" + index].Value = groupChiPhiBangKes.Select(c => c.Sum(o => o.Khac)).Sum();

                    worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["N" + index].Value = groupChiPhiBangKes.Select(c => c.Sum(o => o.NguoiBenhTuTra)).Sum();

                    index++;

                    worksheet.Cells["A" + index + ":N" + index].Merge = true;
                    worksheet.Cells["A" + index + ":N" + index].Value = "Tổng chi phí lần khám bệnh/cả đợt điều trị: " + Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.ThanhTienBV)).ApplyFormatMoneyToDouble() + " đồng";
                    worksheet.Cells["A" + index + ":N" + index].Style.Font.Bold = true;

                    index++;

                    worksheet.Cells["A" + index + ":N" + index].Merge = true;
                    worksheet.Cells["A" + index + ":N" + index].Value = "(Viết bằng chữ: " + NumberHelper.ChuyenSoRaText(Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.ThanhTienBV))) + ")";

                    index++;

                    worksheet.Cells["A" + index + ":N" + index].Merge = true;
                    worksheet.Cells["A" + index + ":N" + index].Value = "Trong đó , số tiền do:";
                    worksheet.Cells["A" + index + ":N" + index].Style.Font.Bold = true;

                    index++;
                    worksheet.Cells["A" + index + ":N" + index].Merge = true;
                    worksheet.Cells["A" + index + ":N" + index].Value = "- Quỹ BHYT thanh toán: " + Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.QuyBHYT)).ApplyFormatMoneyToDouble();

                    var tongBHTN = Convert.ToDouble(tatCaChiPhis.Where(o => o.TrangThaiThanhToan != Enums.TrangThaiThanhToan.DaThanhToan).Sum(o => o.TongCongNo));                 
                    if (tongBHTN != 0)
                    {
                        index++;
                        worksheet.Cells["A" + index + ":N" + index].Merge = true;
                        worksheet.Cells["A" + index + ":N" + index].Value = "- BHTN bảo lãnh: " + Convert.ToDouble(tongBHTN).ApplyFormatMoneyToDouble();
                    }

                    index++;
                    var tongTamUng = GetSoTienDaTamUngAsync(thuPhiKhamChuaBenhVo.Id).Result;
                    worksheet.Cells["A" + index + ":N" + index].Merge = true;
                    worksheet.Cells["A" + index + ":N" + index].Value = "- Tổng tạm ứng: " + Convert.ToDouble(tongTamUng).ApplyFormatMoneyToDouble();

              
                    index++;
                    worksheet.Cells["A" + index + ":N" + index].Merge = true;
                    worksheet.Cells["A" + index + ":N" + index].Value = "+ Nguồn khác: " + Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.Khac)).ApplyFormatMoneyToDouble(true);

                    var tongQuyBHYTTrongGoi = tatCaChiPhis.Where(o => o.TrangThaiThanhToan != Enums.TrangThaiThanhToan.DaThanhToan && o.KiemTraBHYTXacNhan && o.YeuCauGoiDichVuId != null).Sum(o => o.BHYTThanhToan);
                    var tongQuyBHYTThanhToan = Convert.ToDouble(chiPhis.Sum(o => o.BHYTThanhToan) + tongQuyBHYTTrongGoi);
                    var soTienCanThu = Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.ThanhTienBV)) - Convert.ToDouble(tongTamUng) - tongQuyBHYTThanhToan - tongBHTN - Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.Khac));
                    string soTienNBTuTraHoacTraLaiBN = soTienCanThu < 0 ? $"- Phải trả lại NB: {(soTienCanThu * (-1)).ApplyFormatMoneyToDouble()}" : $"- Người bệnh tự trả: {(soTienCanThu).ApplyFormatMoneyToDouble()}";

                    index++;
                    worksheet.Cells["A" + index + ":N" + index].Merge = true;
                    worksheet.Cells["A" + index + ":N" + index].Value = soTienNBTuTraHoacTraLaiBN;


                    index = index + 2;
                   
                    worksheet.Cells["G" + index + ":N" + index].Style.Font.Bold = true;
                    worksheet.Cells["G" + index + ":N" + index].Merge = true;
                    worksheet.Cells["G" + index + ":N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["G" + index + ":N" + index].Value = "XÁC NHẬN CỦA NGƯỜI BỆNH ";

                    index++;

                    worksheet.Cells["A" + index + ":F" + index].Style.Font.Bold = true;
                    worksheet.Cells["A" + index + ":F" + index].Merge = true;
                    worksheet.Cells["A" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A" + index + ":F" + index].Value = "NGƯỜI LẬP BẢNG KÊ";
       

                    worksheet.Cells["G" + index + ":N" + index].Style.Font.Bold = true;
                    worksheet.Cells["G" + index + ":N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["G" + index + ":N" + index].Merge = true;
                    worksheet.Cells["G" + index + ":N" + index].Value = "(Ký , Ghi rõ họ tên)";

                    index++;

                    worksheet.Cells["A" + index + ":F" + index].Style.Font.Bold = true;
                    worksheet.Cells["A" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A" + index + ":F" + index].Merge = true;
                    worksheet.Cells["A" + index + ":F" + index].Value = "(Ký , Ghi rõ họ tên)";


                    worksheet.Cells["G" + index + ":N" + index].Style.Font.Bold = true;
                    worksheet.Cells["G" + index + ":N" + index].Merge = true;
                    worksheet.Cells["G" + index + ":N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["G" + index + ":N" + index].Value = "(Tôi đã nhận...phim...Xquang / CT / MRI)";

                    xlPackage.Save();
                }

                return stream.ToArray();
            }           
        }

        public byte[] XuatBangKeNgoaiTruTrongGoiChoThuExcel(QuyetToanDichVuTrongGoiVo thuPhiKhamChuaBenhVo)
        {
            var yeuCauTiepNhan = BaseRepository.TableNoTracking.Where(c => c.Id == thuPhiKhamChuaBenhVo.Id)
                 .Include(xx => xx.BenhNhan)
                 .Include(xx => xx.YeuCauKhamBenhs).ThenInclude(o => o.NoiThucHien)
                 .Include(xx => xx.YeuCauKhamBenhs).ThenInclude(o => o.BenhVienChuyenVien)
                 .Include(xx => xx.YeuCauKhamBenhs).ThenInclude(o => o.Icdchinh)
                 .Include(xx => xx.YeuCauKhamBenhs).ThenInclude(o => o.YeuCauKhamBenhICDKhacs).ThenInclude(o => o.ICD)
                 .Include(cc => cc.PhuongXa)
                 .Include(cc => cc.QuanHuyen)
                 .Include(cc => cc.TinhThanh)
                 .Include(cc => cc.NoiChuyen).FirstOrDefault();

            if (yeuCauTiepNhan == null)
                return  null;

            var benhVien = new Core.Domain.Entities.BenhVien.BenhVien();

            string maBHYT = string.Empty;
            string bHYTTuNgay = string.Empty;
            string bHYTDenNgay = string.Empty;
            string mucHuong = string.Empty;
            string NoiDKKCBBanDau = string.Empty;
            string MaKCBBanDau = string.Empty;


            string maICDKemTheo = string.Empty;
            string tenICDKemTheo = string.Empty;

            var yeuCauKhamBenh = yeuCauTiepNhan.YeuCauKhamBenhs.Where(cc => cc.IcdchinhId != null).OrderBy(cc => cc.ThoiDiemHoanThanh).LastOrDefault();

            if (yeuCauKhamBenh != null)
            {
                var maKemTheos = yeuCauKhamBenh.YeuCauKhamBenhICDKhacs.Select(c => c.ICD.Ma).ToList();
                var chuGhiKemTheos = yeuCauKhamBenh.YeuCauKhamBenhICDKhacs.Select(c => c.GhiChu).ToList();

                maICDKemTheo = string.Join(", ", maKemTheos);
                tenICDKemTheo = string.Join(", ", chuGhiKemTheos);
            }

            if (yeuCauTiepNhan.CoBHYT == true)
            {
                if (!String.IsNullOrEmpty(yeuCauTiepNhan.BHYTMaDKBD))
                {
                    benhVien = _benhVienRepository.TableNoTracking.FirstOrDefault(p => p.Ma.Equals(yeuCauTiepNhan.BHYTMaDKBD));
                }
                maBHYT = yeuCauTiepNhan.BHYTMaSoThe;
                bHYTTuNgay = yeuCauTiepNhan.BHYTNgayHieuLuc?.ApplyFormatDate();
                bHYTDenNgay = yeuCauTiepNhan.BHYTNgayHetHan?.ApplyFormatDate();
                mucHuong = yeuCauTiepNhan.BHYTMucHuong.ToString() + "%";
                NoiDKKCBBanDau = benhVien != null ? benhVien?.Ten : "Chưa xác định";
                MaKCBBanDau = yeuCauTiepNhan.BHYTMaDKBD;
            }
            else
            {
                maBHYT = "";
                bHYTTuNgay = "";
                bHYTDenNgay = "";
                mucHuong = "0%";
                NoiDKKCBBanDau = "Chưa xác định";
                MaKCBBanDau = "Chưa xác định";
            }
            List<ChiPhiKhamChuaBenhVo> chiPhis;
            //chỉ lấy những dịch vụ chưa thu tiền
            chiPhis = GetTatCaDichVuKhamChuaBenh(thuPhiKhamChuaBenhVo.Id).Result.Where(o => o.YeuCauGoiDichVuId != null && o.TrangThaiThanhToan != Enums.TrangThaiThanhToan.DaThanhToan).ToList();

            var dsChiPhiBangKe = new List<BangKeKhamBenhBenhVienVo>();
            var dateItemChiPhis = string.Empty;
            int indexItem = 1;
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {

                    var worksheet = xlPackage.Workbook.Worksheets.Add("BẢNG KÊ CHI PHÍ KHÁM CHỮA BỆNH CHỜ THU");
             

                    // set row
                    worksheet.Row(9).Height = 24.5;
                    worksheet.DefaultRowHeight = 16;

                    //SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 10;
                    worksheet.Column(2).Width = 25;
                    worksheet.Column(3).Width = 15;
                    worksheet.Column(4).Width = 15;
                    worksheet.Column(5).Width = 15;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 15;
                    worksheet.Column(8).Width = 15;
                    worksheet.Column(9).Width = 15;
                    worksheet.Column(10).Width = 15;
                    worksheet.Column(11).Width = 15;
                    worksheet.Column(12).Width = 15;
                    worksheet.Column(13).Width = 15;
                    worksheet.Column(14).Width = 15;

                    worksheet.DefaultColWidth = 7;

                    using (var range = worksheet.Cells["A1:C1"])
                    {
                        range.Worksheet.Cells["A1:C1"].Merge = true;
                        range.Worksheet.Cells["A1:C1"].Value = "CƠ SỞ KHÁM CHỮA BỆNH : BỆNH VIỆN ĐKQT BẮC HÀ"; 
                        range.Worksheet.Cells["A1:C1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:C1"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A1:C1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:C1"].Style.Font.Color.SetColor(Color.Black);
                    }

                    //using (var range = worksheet.Cells["A2:C2"])
                    //{
                    //    range.Worksheet.Cells["A2:C2"].Merge = true;
                    //    range.Worksheet.Cells["A2:C2"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                    //    range.Worksheet.Cells["A2:C2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    //    range.Worksheet.Cells["A2:C2"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    //    range.Worksheet.Cells["A2:C2"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    //    range.Worksheet.Cells["A2:C2"].Style.Font.Color.SetColor(Color.Black);
                    //    range.Worksheet.Cells["A2:C2"].Style.Font.Bold = true;
                    //}

                    using (var range = worksheet.Cells["L1:N1"])
                    {
                        range.Worksheet.Cells["L1:N1"].Merge = true;
                        range.Worksheet.Cells["L1:N1"].Value = "Mẫu số : 01/KBCB ";
                        range.Worksheet.Cells["L1:N1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        range.Worksheet.Cells["L1:N1"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["L1:N1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["L1:N1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["L1:N1"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["L2:N2"])
                    {
                        range.Worksheet.Cells["L2:N2"].Merge = true;
                        range.Worksheet.Cells["L2:N2"].Value = "Mã số người bệnh: " + yeuCauTiepNhan.BenhNhan.MaBN;
                        range.Worksheet.Cells["L2:N2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        range.Worksheet.Cells["L2:N2"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["L2:N2"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["L2:N2"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["L2:N2"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["L3:N3"])
                    {
                        range.Worksheet.Cells["L3:N3"].Merge = true;
                        range.Worksheet.Cells["L3:N3"].Value = "Số khám bệnh:" + yeuCauTiepNhan.MaYeuCauTiepNhan;
                        range.Worksheet.Cells["L3:N3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        range.Worksheet.Cells["L3:N3"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["L3:N3"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["L3:N3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["L3:N3"].Style.Font.Bold = true;
                    }



                    using (var range = worksheet.Cells["A4:N5"])
                    {
                        range.Worksheet.Cells["A4:N5"].Merge = true;
                        range.Worksheet.Cells["A4:N5"].Value = "BẢNG KÊ CHI PHÍ KHÁM CHỮA BỆNH CHỜ THU";
                        range.Worksheet.Cells["A4:N5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:N5"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:N5"].Style.Font.SetFromFont(new Font("Times New Roman", 18));
                        range.Worksheet.Cells["A4:N5"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:N5"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A6:N6"])
                    {
                        range.Worksheet.Cells["A6:N6"].Merge = true;
                        range.Worksheet.Cells["A6:N6"].Value = "I.Hành chính";
                        range.Worksheet.Cells["A6:N6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A6:N6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A6:N6"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A6:N6"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A6:N6"].Style.Font.Bold = true;
                    }
                    using (var range = worksheet.Cells["A7:F7"])
                    {
                        range.Worksheet.Cells["A7:F7"].Merge = true;

                        range.Worksheet.Cells["A7:F7"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A7:F7"].RichText.Add("(1) Họ tên người bệnh: ");
                        var value = range.Worksheet.Cells["A7:F7"].RichText.Add(yeuCauTiepNhan.HoTen);
                        value.Bold = true;

                        range.Worksheet.Cells["A7:F7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A7:F7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A7:F7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));


                        range.Worksheet.Cells["A7:F7"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G7:J7"])
                    {
                        range.Worksheet.Cells["G7:J7"].Merge = true;
                        range.Worksheet.Cells["G7:J7"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G7:J7"].RichText.Add("Ngày,tháng,năm sinh: ");
                        var value = range.Worksheet.Cells["G7:J7"].RichText.Add(DateHelper.DOBFormat(yeuCauTiepNhan.NgaySinh, yeuCauTiepNhan.ThangSinh, yeuCauTiepNhan.NamSinh));
                        value.Bold = true;

                        range.Worksheet.Cells["G7:J7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G7:J7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G7:J7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G7:J7"].Style.Font.Color.SetColor(Color.Black);
                    }

                    var gioitinh = yeuCauTiepNhan.GioiTinh != null ? yeuCauTiepNhan.GioiTinh.GetDescription() : "";
                    using (var range = worksheet.Cells["K7:N7"])
                    {
                        range.Worksheet.Cells["K7:N7"].Merge = true;

                        range.Worksheet.Cells["K7:N7"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["K7:N7"].RichText.Add("Giới tính: ");
                        var value = range.Worksheet.Cells["K7:N7"].RichText.Add(gioitinh);
                        value.Bold = true;

                        range.Worksheet.Cells["K7:N7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["K7:N7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["K7:N7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["K7:N7"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["A8:F8"])
                    {
                        range.Worksheet.Cells["A8:F8"].Merge = true;

                        range.Worksheet.Cells["A8:F8"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A8:F8"].RichText.Add("(2)Địa chỉ hiện tại: ");
                        var value = range.Worksheet.Cells["A8:F8"].RichText.Add(yeuCauTiepNhan.DiaChiDayDu);
                        value.Bold = true;

                        range.Worksheet.Cells["A8:F8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A8:F8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A8:F8"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A8:F8"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G8:J8"])
                    {
                        range.Worksheet.Cells["G8:J8"].Merge = true;

                        range.Worksheet.Cells["G8:J8"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G8:J8"].RichText.Add("(3)Mã khu vực(K1/K2/K3): ");
                        var value = range.Worksheet.Cells["G8:J8"].RichText.Add(yeuCauTiepNhan.BHYTMaKhuVuc);
                        value.Bold = true;

                        range.Worksheet.Cells["G8:J8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G8:J8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G8:J8"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G8:J8"].Style.Font.Color.SetColor(Color.Black);
                    }


                    using (var range = worksheet.Cells["A9:F9"])
                    {
                        range.Worksheet.Cells["A9:F9"].Merge = true;
                        range.Worksheet.Cells["A9:F9"].Style.WrapText = true;

                        var title = range.Worksheet.Cells["A9:F9"].RichText.Add("(4)Mã thẻ BHYT: ");
                        var value = range.Worksheet.Cells["A9:F9"].RichText.Add(maBHYT);
                        value.Bold = true;

                        range.Worksheet.Cells["A9:F9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A9:F9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A9:F9"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A9:F9"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G9:J9"])
                    {
                        range.Worksheet.Cells["G9:J9"].Merge = true;
                        range.Worksheet.Cells["G9:J9"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G9:J9"].RichText.Add("Giá trị từ: ");
                        var valuebHYTTuNgay = range.Worksheet.Cells["G9:J9"].RichText.Add(bHYTTuNgay);
                        valuebHYTTuNgay.Bold = true;

                        var den = range.Worksheet.Cells["G9:J9"].RichText.Add(" đến: ");
                        den.Bold = false;
                        var valuebHYTDenNgay = range.Worksheet.Cells["G9:J9"].RichText.Add(bHYTDenNgay);
                        valuebHYTDenNgay.Bold = true;


                        range.Worksheet.Cells["G9:J9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G9:J9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G9:J9"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G9:J9"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["A10:F10"])
                    {
                        range.Worksheet.Cells["A10:F10"].Merge = true;

                        range.Worksheet.Cells["A10:F10"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A10:F10"].RichText.Add("(5)Nơi ĐK KCB ban đầu: ");
                        var valueNoiDKKCBBanDau = range.Worksheet.Cells["A10:F10"].RichText.Add(NoiDKKCBBanDau);
                        valueNoiDKKCBBanDau.Bold = true;

                        range.Worksheet.Cells["A10:F10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A10:F10"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A10:F10"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A10:F10"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G10:J10"])
                    {
                        range.Worksheet.Cells["G10:J10"].Merge = true;
                        range.Worksheet.Cells["G10:J10"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G10:J10"].RichText.Add("(6)Mã: ");
                        var valueMaKCBBanDau = range.Worksheet.Cells["G10:J10"].RichText.Add(MaKCBBanDau);
                        valueMaKCBBanDau.Bold = true;

                        range.Worksheet.Cells["G10:J10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G10:J10"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G10:J10"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G10:J10"].Style.Font.Color.SetColor(Color.Black);
                    }


                    using (var range = worksheet.Cells["A11:F11"])
                    {
                        range.Worksheet.Cells["A11:F11"].Merge = true;

                        range.Worksheet.Cells["A11:F11"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A11:F11"].RichText.Add("(7)Đến Khám: ");
                        var value = range.Worksheet.Cells["A11:F11"].RichText.Add(yeuCauTiepNhan.ThoiDiemTiepNhan.ApplyFormatGioPhutNgay());
                        value.Bold = true;

                        range.Worksheet.Cells["A11:F11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A11:F11"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A11:F11"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A11:F11"].Style.Font.Color.SetColor(Color.Black);
                    }


                    using (var range = worksheet.Cells["A12:F12"])
                    {
                        range.Worksheet.Cells["A12:F12"].Merge = true;

                        range.Worksheet.Cells["A12:F12"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A12:F12"].RichText.Add("(8)Điều trị ngoại trú/nội trú từ: ");
                        var value = range.Worksheet.Cells["A12:F12"].RichText.Add(string.Empty);
                        value.Bold = true;

                        range.Worksheet.Cells["A12:F12"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A12:F12"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A12:F12"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A12:F12"].Style.Font.Color.SetColor(Color.Black);
                    }


                    using (var range = worksheet.Cells["A13:F13"])
                    {
                        range.Worksheet.Cells["A13:F13"].Merge = true;
                        range.Worksheet.Cells["A13:F13"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A13:F13"].RichText.Add("(9)Kết thúc khám/điều trị: ");
                        var value = range.Worksheet.Cells["A13:F13"].RichText.Add(yeuCauKhamBenh?.ThoiDiemHoanThanh?.ApplyFormatGioPhutNgay());
                        value.Bold = true;


                        range.Worksheet.Cells["A13:F13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A13:F13"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A13:F13"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A13:F13"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G13:J13"])
                    {
                        range.Worksheet.Cells["G13:J13"].Merge = true;
                        range.Worksheet.Cells["G13:J13"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G13:J13"].RichText.Add("Tổng số ngày điều trị: ");
                        var value = range.Worksheet.Cells["G13:J13"].RichText.Add(string.Empty);
                        value.Bold = true;

                        range.Worksheet.Cells["G13:J13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G13:J13"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G13:J13"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G13:J13"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["K13:N13"])
                    {
                        range.Worksheet.Cells["K13:N13"].Merge = true;

                        range.Worksheet.Cells["K13:N13"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["K13:N13"].RichText.Add("(10)Tình trạng ra viện: ");
                        var value = range.Worksheet.Cells["K13:N13"].RichText.Add("1");
                        value.Bold = true;


                        range.Worksheet.Cells["K13:N13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["K13:N13"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["K13:N13"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["K13:N13"].Style.Font.Color.SetColor(Color.Black);
                    }

                    //=================================14=================================
                    var coCapCuu = yeuCauTiepNhan.LyDoVaoVien == Enums.EnumLyDoVaoVien.CapCuu ? "X" : string.Empty;
                    var coDungTuyen = yeuCauTiepNhan.LyDoVaoVien == Enums.EnumLyDoVaoVien.DungTuyen ? "X" : string.Empty;
                    var coThongTuyen = yeuCauTiepNhan.LyDoVaoVien == Enums.EnumLyDoVaoVien.ThongTuyen ? "X" : string.Empty;
                    var coTraiTuyen = yeuCauTiepNhan.LyDoVaoVien == Enums.EnumLyDoVaoVien.TraiTuyen ? "X" : string.Empty;

                    using (var range = worksheet.Cells["A14:C14"])
                    {
                        range.Worksheet.Cells["A14:C14"].Merge = true;

                        range.Worksheet.Cells["A14:C14"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A14:C14"].RichText.Add("(11) Cấp cứu: ");
                        var value = range.Worksheet.Cells["A14:C14"].RichText.Add(coCapCuu);
                        value.Bold = true;

                        range.Worksheet.Cells["A14:C14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A14:C14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A14:C14"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A14:C14"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["D14:F14"])
                    {
                        range.Worksheet.Cells["D14:F14"].Merge = true;
                        range.Worksheet.Cells["D14:F14"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["D14:F14"].RichText.Add("(12) Đúng tuyến: ");
                        var value = range.Worksheet.Cells["D14:F14"].RichText.Add(coDungTuyen);
                        value.Bold = true;

                        range.Worksheet.Cells["D14:F14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["D14:F14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["D14:F14"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["D14:F14"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G14:J14"])
                    {
                        range.Worksheet.Cells["G14:J14"].Merge = true;
                        range.Worksheet.Cells["G14:J14"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G14:J14"].RichText.Add("Nơi chuyến đến: ");
                        var value = range.Worksheet.Cells["G14:J14"].RichText.Add(yeuCauTiepNhan.NoiChuyen?.Ten);
                        value.Bold = true;
                        range.Worksheet.Cells["G14:J14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G14:J14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G14:J14"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G14:J14"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["K14:N14"])
                    {
                        range.Worksheet.Cells["K14:N14"].Merge = true;
                        range.Worksheet.Cells["K14:N14"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["K14:N14"].RichText.Add("Nơi chuyển đi: ");
                        var value = range.Worksheet.Cells["K14:N14"].RichText.Add(yeuCauKhamBenh?.BenhVienChuyenVien?.Ten);
                        value.Bold = true;
                        range.Worksheet.Cells["K14:N14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["K14:N14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["K14:N14"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["K14:N14"].Style.Font.Color.SetColor(Color.Black);
                    }

                    //=================================15=================================
                    using (var range = worksheet.Cells["A15:C15"])
                    {
                        range.Worksheet.Cells["A15:C15"].Merge = true;
                        range.Worksheet.Cells["A15:C15"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A15:C15"].RichText.Add("(13)Thông tuyến: ");
                        var value = range.Worksheet.Cells["A15:C15"].RichText.Add(coThongTuyen);
                        value.Bold = true;
                        range.Worksheet.Cells["A15:C15"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A15:C15"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A15:C15"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A15:C15"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["D15:F15"])
                    {
                        range.Worksheet.Cells["D15:F15"].Merge = true;
                        range.Worksheet.Cells["D15:F15"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["D15:F15"].RichText.Add("(14)Trái tuyến: ");
                        var value = range.Worksheet.Cells["D15:F15"].RichText.Add(coTraiTuyen);
                        value.Bold = true;

                        range.Worksheet.Cells["D15:F15"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["D15:F15"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["D15:F15"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["D15:F15"].Style.Font.Color.SetColor(Color.Black);
                    }


                    //================================= 16 =================================

                    using (var range = worksheet.Cells["A16:F16"])
                    {
                        range.Worksheet.Cells["A16:F16"].Merge = true;
                        range.Worksheet.Cells["A16:F16"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A16:F16"].RichText.Add("(15)Chẩn đoán xác định: ");
                        var value = range.Worksheet.Cells["A16:F16"].RichText.Add(yeuCauKhamBenh?.GhiChuICDChinh);
                        value.Bold = true;

                        range.Worksheet.Cells["A16:F16"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A16:F16"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A16:F16"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A16:F16"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G16:N16"])
                    {
                        range.Worksheet.Cells["G16:N16"].Merge = true;

                        range.Worksheet.Cells["G16:N16"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G16:N16"].RichText.Add("(16) Mã bệnh: ");
                        var value = range.Worksheet.Cells["G16:N16"].RichText.Add(yeuCauKhamBenh?.Icdchinh?.Ma);
                        value.Bold = true;


                        range.Worksheet.Cells["G16:N16"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G16:N16"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G16:N16"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G16:N16"].Style.Font.Color.SetColor(Color.Black);
                    }

                    //================================= 17 =================================
                    using (var range = worksheet.Cells["A17:F17"])
                    {
                        range.Worksheet.Cells["A17:F17"].Merge = true;
                        range.Worksheet.Cells["A17:F17"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A17:F17"].RichText.Add("(17)Bệnh kèm theo: ");
                        var value = range.Worksheet.Cells["A17:F17"].RichText.Add(tenICDKemTheo);
                        value.Bold = true;


                        range.Worksheet.Cells["A17:F17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A17:F17"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A17:F17"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A17:F17"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G17:N17"])
                    {
                        range.Worksheet.Cells["G17:N17"].Merge = true;
                        range.Worksheet.Cells["G17:N17"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G17:N17"].RichText.Add("(18)Mã bệnh kèm theo: ");
                        var value = range.Worksheet.Cells["G17:N17"].RichText.Add(maICDKemTheo);
                        value.Bold = true;


                        range.Worksheet.Cells["G17:N17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G17:N17"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G17:N17"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G17:N17"].Style.Font.Color.SetColor(Color.Black);
                    }

                    //================================= 18 =================================
                    using (var range = worksheet.Cells["A18:F18"])
                    {
                        range.Worksheet.Cells["A18:F18"].Merge = true;
                        range.Worksheet.Cells["A18:F18"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A18:F18"].RichText.Add("(19)Thời điểm đủ 05 năm liên tục từ ngày: ");
                        var value = range.Worksheet.Cells["A18:F18"].RichText.Add(yeuCauTiepNhan.BHYTNgayDu5Nam?.ApplyFormatDate());
                        value.Bold = true;


                        range.Worksheet.Cells["A18:F18"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A18:F18"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A18:F18"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A18:F18"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G18:N18"])
                    {
                        range.Worksheet.Cells["G18:N18"].Merge = true;
                        range.Worksheet.Cells["G18:N18"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G18:N18"].RichText.Add("(21)Miễn cùng chi trả trong năm từ ngày: ");
                        var value = range.Worksheet.Cells["G18:N18"].RichText.Add(yeuCauTiepNhan.BHYTNgayDuocMienCungChiTra?.ApplyFormatDate());
                        value.Bold = true;

                        range.Worksheet.Cells["G18:N18"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G18:N18"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G18:N18"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G18:N18"].Style.Font.Color.SetColor(Color.Black);
                    }

                    //================================= 19 =================================

                    using (var range = worksheet.Cells["A19:N19"])
                    {
                        range.Worksheet.Cells["A19:N19"].Merge = true;
                        range.Worksheet.Cells["A19:N19"].Value = "II. Phần Chi phí khám bệnh, chữa bệnh";
                        range.Worksheet.Cells["A19:N19"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A19:N19"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A19:N19"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A19:N19"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A19:N19"].Style.Font.Bold = true;
                    }

                    //================================= 20 =========================================
                    using (var range = worksheet.Cells["A20:C20"])
                    {
                        range.Worksheet.Cells["A20:C20"].Merge = true;
                        range.Worksheet.Cells["A20:C20"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["A20:C20"].RichText.Add("Mã thẻ BHYT: ");
                        var value = range.Worksheet.Cells["A20:C20"].RichText.Add(maBHYT);
                        value.Bold = true;

                        range.Worksheet.Cells["A20:C20"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A20:C20"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A20:C20"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A20:C20"].Style.Font.Color.SetColor(Color.Black);
                    }
                    using (var range = worksheet.Cells["D20:F20"])
                    {
                        range.Worksheet.Cells["D20:F20"].Merge = true;
                        range.Worksheet.Cells["D20:F20"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["D20:F20"].RichText.Add("Giá trị từ: ");
                        var valuebHYTTuNgay = range.Worksheet.Cells["D20:F20"].RichText.Add(bHYTTuNgay);
                        valuebHYTTuNgay.Bold = true;

                        var den = range.Worksheet.Cells["D20:F20"].RichText.Add(" đến: ");
                        den.Bold = false;
                        var valuebHYTDenNgay = range.Worksheet.Cells["D20:F20"].RichText.Add(bHYTDenNgay);
                        valuebHYTDenNgay.Bold = true;

                        range.Worksheet.Cells["D20:F20"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["D20:F20"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["D20:F20"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["D20:F20"].Style.Font.Color.SetColor(Color.Black);
                    }
                    using (var range = worksheet.Cells["G20:J20"])
                    {
                        range.Worksheet.Cells["G20:J20"].Merge = true;
                        range.Worksheet.Cells["G20:J20"].Style.WrapText = true;
                        var title = range.Worksheet.Cells["G20:J20"].RichText.Add("Mức hưởng: ");
                        var value = range.Worksheet.Cells["G20:J20"].RichText.Add(mucHuong);
                        value.Bold = true;


                        range.Worksheet.Cells["G20:J20"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["G20:J20"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G20:J20"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G20:J20"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["A21:N22"])
                    {
                        range.Worksheet.Cells["A21:N22"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A21:N22"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A21:N22"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A21:N22"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A21:N22"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A21:N22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A21:A22"].Merge = true;
                        range.Worksheet.Cells["A21:A22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A21:A22"].Value = "STT";

                        range.Worksheet.Cells["B21:B22"].Merge = true;
                        range.Worksheet.Cells["B21:B22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B21:B22"].Value = "Nội dung";

                        range.Worksheet.Cells["C21:C22"].Merge = true;
                        range.Worksheet.Cells["C21:C22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C21:C22"].Value = "Đơn vị tính";

                        range.Worksheet.Cells["D21:D22"].Merge = true;
                        range.Worksheet.Cells["D21:D22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D21:D22"].Value = "Số lượng";

                        range.Worksheet.Cells["E21:E22"].Merge = true;
                        range.Worksheet.Cells["E21:E22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E21:E22"].Value = "Đơn giá BV(đồng)";

                        range.Worksheet.Cells["F21:F22"].Merge = true;
                        range.Worksheet.Cells["F21:F22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F21:F22"].Value = "Đơn giá BH(đồng)";

                        range.Worksheet.Cells["G21:G22"].Merge = true;
                        range.Worksheet.Cells["G21:G22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G21:G22"].Value = "Tỉ lệ thanh toán theo dịch vụ (%)";

                        range.Worksheet.Cells["H21:H22"].Merge = true;
                        range.Worksheet.Cells["H21:H22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H21:H22"].Value = "Thành tiền BV (đồng)";

                        range.Worksheet.Cells["I21:I22"].Merge = true;
                        range.Worksheet.Cells["I21:I22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I21:I22"].Value = "Tỷ lệ thanh toán BHYT (%)";

                        range.Worksheet.Cells["J21:J22"].Merge = true;
                        range.Worksheet.Cells["J21:J22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J21:J22"].Value = "Thành tiền BH(đồng)";

                        range.Worksheet.Cells["K21:N21"].Merge = true;
                        range.Worksheet.Cells["K21:N21"].Value = "Nguồn thanh toán (đồng)";

                        range.Worksheet.Cells["K22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K22"].Value = "Quỹ BHYT";

                        range.Worksheet.Cells["L22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["L22"].Value = "Người bệnh cùng chi trả (đồng)";

                        range.Worksheet.Cells["M22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["M22"].Value = "Khác (đồng)";

                        range.Worksheet.Cells["N22"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["N22"].Value = "Người bệnh tự trả(đồng)";
                    }

              
                    int index = 23;
                    foreach (var chiPhi in chiPhis)
                    {
                        var chiphiBangKe = new BangKeKhamBenhBenhVienVo
                        {
                            Nhom = chiPhi.NhomChiPhiBangKe,
                            Id = chiPhi.Id,
                            NoiDung = chiPhi.TenDichVu,
                            DonViTinh = chiPhi.DonViTinh,
                            SoLuong = (decimal)chiPhi.Soluong,
                            DuocHuongBaoHiem = chiPhi.DuocHuongBHYT,
                            DonGiaBH = chiPhi.DuocHuongBHYT ? chiPhi.DonGiaBHYT : 0,
                            MucHuongBaoHiem = chiPhi.MucHuongBaoHiem,
                            TiLeThanhToanTheoDV = chiPhi.TiLeBaoHiemThanhToan,
                            BaoHiemChiTra = true,
                            DonGiaBV = chiPhi.DonGia
                        };
                        if (chiPhi.KhongTinhPhi == true || chiPhi.YeuCauGoiDichVuId != null)
                        {
                            chiphiBangKe.Khac = chiPhi.MucHuongBaoHiem > 0
                                ? (chiphiBangKe.ThanhTienBV.GetValueOrDefault() - chiphiBangKe.ThanhTienBH.GetValueOrDefault())
                                : chiphiBangKe.ThanhTienBV.GetValueOrDefault();
                        }
                        else
                        {
                            chiphiBangKe.Khac = chiPhi.SoTienMG;
                        }
                        dsChiPhiBangKe.Add(chiphiBangKe);
                    }
                    var groupChiPhiBangKes = dsChiPhiBangKe.GroupBy(x => x.Nhom).OrderBy(o => (int)o.Key * (o.Key == NhomChiPhiBangKe.NgayGiuongDieuTriBanNgay || o.Key == NhomChiPhiBangKe.NgayGiuongDieuTriNoiTru ? 1 : 10));
                    if (!groupChiPhiBangKes.Any())
                    {
                        return null;
                    }

                    foreach (var groupChiPhiBangKe in groupChiPhiBangKes)
                    {
                        if (groupChiPhiBangKe.Key == NhomChiPhiBangKe.NgayGiuongDieuTriBanNgay)
                        {

                            worksheet.Cells["A" + index + ":H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["A" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + index + ":H" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            worksheet.Cells["A" + index + ":H" + index].Merge = true;
                            worksheet.Cells["A" + index + ":H" + index].Value = "2. Ngày giường:";
                            worksheet.Cells["A" + index + ":H" + index].Style.Font.Bold = true;

                            index++;

                            worksheet.Cells["A" + index + ":G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["A" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            worksheet.Cells["A" + index + ":G" + index].Merge = true;
                            worksheet.Cells["A" + index + ":G" + index].Value = "2.1." + groupChiPhiBangKe.Key.GetDescription();
                            worksheet.Cells["A" + index + ":G" + index].Style.Font.Bold = true;

                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["H" + index].Style.Font.Bold = true;
                            worksheet.Cells["H" + index].Value = groupChiPhiBangKe.Sum(o => o.ThanhTienBV);

                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["J" + index].Style.Font.Bold = true;
                            worksheet.Cells["J" + index].Value = groupChiPhiBangKe.Sum(o => o.ThanhTienBH);

                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["K" + index].Style.Font.Bold = true;
                            worksheet.Cells["K" + index].Value = groupChiPhiBangKe.Sum(o => o.QuyBHYT);

                            worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["L" + index].Style.Font.Bold = true;
                            worksheet.Cells["L" + index].Value = groupChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra);

                            worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["M" + index].Style.Font.Bold = true;
                            worksheet.Cells["M" + index].Value = groupChiPhiBangKe.Sum(o => o.Khac);

                            worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["N" + index].Style.Font.Bold = true;
                            worksheet.Cells["N" + index].Value = groupChiPhiBangKe.Sum(o => o.NguoiBenhTuTra);

                            index++;
                        }
                        else if (groupChiPhiBangKe.Key == NhomChiPhiBangKe.NgayGiuongDieuTriNoiTru)
                        {
                            if (!groupChiPhiBangKes.Any(o => o.Key == NhomChiPhiBangKe.NgayGiuongDieuTriBanNgay))
                            {
                                worksheet.Cells["A" + index + ":H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["A" + index + ":H" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                worksheet.Cells["A" + index + ":H" + index].Merge = true;
                                worksheet.Cells["A" + index + ":H" + index].Value = "2. Ngày giường:";
                                worksheet.Cells["A" + index + ":H" + index].Style.Font.Bold = true;
                            }

                            worksheet.Cells["A" + index + ":G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["A" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            worksheet.Cells["A" + index + ":G" + index].Merge = true;
                            worksheet.Cells["A" + index + ":G" + index].Value = "2.2." + groupChiPhiBangKe.Key.GetDescription();
                            worksheet.Cells["A" + index + ":G" + index].Style.Font.Bold = true;

                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["H" + index].Style.Font.Bold = true;
                            worksheet.Cells["H" + index].Value = groupChiPhiBangKe.Sum(o => o.ThanhTienBV);

                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["J" + index].Style.Font.Bold = true;
                            worksheet.Cells["J" + index].Value = groupChiPhiBangKe.Sum(o => o.ThanhTienBH);

                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["K" + index].Style.Font.Bold = true;
                            worksheet.Cells["K" + index].Value = groupChiPhiBangKe.Sum(o => o.QuyBHYT);

                            worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["L" + index].Style.Font.Bold = true;
                            worksheet.Cells["L" + index].Value = groupChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra);

                            worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["M" + index].Style.Font.Bold = true;
                            worksheet.Cells["M" + index].Value = groupChiPhiBangKe.Sum(o => o.Khac);

                            worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["N" + index].Style.Font.Bold = true;
                            worksheet.Cells["N" + index].Value = groupChiPhiBangKe.Sum(o => o.NguoiBenhTuTra);

                            index++;
                        }
                        else
                        {
                            worksheet.Cells["A" + index + ":G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["A" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            worksheet.Cells["A" + index + ":G" + index].Merge = true;
                            worksheet.Cells["A" + index + ":G" + index].Value = (int)groupChiPhiBangKe.Key + "." + groupChiPhiBangKe.Key.GetDescription();
                            worksheet.Cells["A" + index + ":G" + index].Style.Font.Bold = true;

                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["H" + index].Style.Font.Bold = true;
                            worksheet.Cells["H" + index].Value = groupChiPhiBangKe.Sum(o => o.ThanhTienBV);

                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["J" + index].Style.Font.Bold = true;
                            worksheet.Cells["J" + index].Value = groupChiPhiBangKe.Sum(o => o.ThanhTienBH);

                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["K" + index].Style.Font.Bold = true;
                            worksheet.Cells["K" + index].Value = groupChiPhiBangKe.Sum(o => o.QuyBHYT);

                            worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["L" + index].Style.Font.Bold = true;
                            worksheet.Cells["L" + index].Value = groupChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra);

                            worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["M" + index].Style.Font.Bold = true;
                            worksheet.Cells["M" + index].Value = groupChiPhiBangKe.Sum(o => o.Khac);

                            worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["N" + index].Style.Font.Bold = true;
                            worksheet.Cells["N" + index].Value = groupChiPhiBangKe.Sum(o => o.NguoiBenhTuTra);

                            index++;
                        }
                        if (groupChiPhiBangKe.Key == NhomChiPhiBangKe.GoiVatTu)
                        {
                            var groupGoiVatTus = groupChiPhiBangKe.ToList().GroupBy(o => o.SoGoiVatTu.GetValueOrDefault()).OrderBy(o => o.Key);
                            int sttGoiVatTu = 1;
                            foreach (var groupGoiVatTu in groupGoiVatTus)
                            {
                                worksheet.Cells["A" + index + ":G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["A" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                worksheet.Cells["A" + index + ":G" + index].Value = "10." + sttGoiVatTu + ". " + groupChiPhiBangKe.Key.GetDescription() + " " + sttGoiVatTu;
                                worksheet.Cells["A" + index + ":G" + index].Style.Font.Bold = true;
                                worksheet.Cells["A" + index + ":G" + index].Merge = true;

                                worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["H" + index].Style.Font.Bold = true;
                                worksheet.Cells["H" + index].Value = groupChiPhiBangKe.Sum(o => o.ThanhTienBV);

                                worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["J" + index].Style.Font.Bold = true;
                                worksheet.Cells["J" + index].Value = groupChiPhiBangKe.Sum(o => o.ThanhTienBH);

                                worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["K" + index].Style.Font.Bold = true;
                                worksheet.Cells["K" + index].Value = groupChiPhiBangKe.Sum(o => o.QuyBHYT);

                                worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["L" + index].Style.Font.Bold = true;
                                worksheet.Cells["L" + index].Value = groupChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra);

                                worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["M" + index].Style.Font.Bold = true;
                                worksheet.Cells["M" + index].Value = groupChiPhiBangKe.Sum(o => o.Khac);

                                worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["N" + index].Style.Font.Bold = true;
                                worksheet.Cells["N" + index].Value = groupChiPhiBangKe.Sum(o => o.NguoiBenhTuTra);

                                index++;

                                foreach (var chiPhiBangKe in groupGoiVatTu)
                                {
                                    worksheet.Cells["A" + index + ":N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["A" + index + ":N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["A" + index + ":N" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));

                                    worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["A" + index].Value = indexItem;

                                    worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["B" + index].Value = chiPhiBangKe.NoiDung;

                                    worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["C" + index].Value = chiPhiBangKe.DonViTinh;

                                    worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["D" + index].Value = chiPhiBangKe.SoLuong;

                                    worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["E" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["E" + index].Value = chiPhiBangKe.DonGiaBV;

                                    worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["F" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["F" + index].Value = chiPhiBangKe.DonGiaBH;

                                    worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["G" + index].Value = chiPhiBangKe.TiLeThanhToanTheoDV;

                                    worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["H" + index].Value = chiPhiBangKe.ThanhTienBV;

                                    worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["I" + index].Value = Convert.ToDouble(chiPhiBangKe.MucHuongBaoHiem > 0 ? 100 : 0).ApplyFormatMoneyToDouble();


                                    worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["J" + index].Value = chiPhiBangKe.ThanhTienBH;


                                    worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["K" + index].Value = chiPhiBangKe.QuyBHYT;


                                    worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["L" + index].Value = chiPhiBangKe.NguoiBenhCungChiTra;


                                    worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["M" + index].Value = chiPhiBangKe.Khac;


                                    worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["N" + index].Value = chiPhiBangKe.NguoiBenhTuTra;

                                    indexItem++;
                                    index++;
                                }
                            }
                        }
                        else
                        {
                            foreach (var chiPhiBangKe in groupChiPhiBangKe)
                            {
                                worksheet.Cells["A" + index + ":N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index + ":N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["A" + index + ":N" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));

                                worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["A" + index].Value = indexItem;

                                worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["B" + index].Value = chiPhiBangKe.NoiDung;

                                worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["C" + index].Value = chiPhiBangKe.DonViTinh;

                                worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["D" + index].Value = chiPhiBangKe.SoLuong;

                                worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["E" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["E" + index].Value = chiPhiBangKe.DonGiaBV;

                                worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["F" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["F" + index].Value = chiPhiBangKe.DonGiaBH;

                                worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["G" + index].Value = chiPhiBangKe.TiLeThanhToanTheoDV;

                                worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["H" + index].Value = chiPhiBangKe.ThanhTienBV;

                                worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["I" + index].Value = Convert.ToDouble(chiPhiBangKe.MucHuongBaoHiem > 0 ? 100 : 0).ApplyFormatMoneyToDouble();


                                worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["J" + index].Value = chiPhiBangKe.ThanhTienBH;


                                worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["K" + index].Value = chiPhiBangKe.QuyBHYT;


                                worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["L" + index].Value = chiPhiBangKe.NguoiBenhCungChiTra;


                                worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["M" + index].Value = chiPhiBangKe.Khac;


                                worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["N" + index].Value = chiPhiBangKe.NguoiBenhTuTra;

                                indexItem++;
                                index++;
                            }
                        }

                    }

                    worksheet.Cells["A" + index + ":G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["A" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells["A" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                    worksheet.Cells["A" + index + ":G" + index].Merge = true;
                    worksheet.Cells["A" + index + ":G" + index].Value = "Tổng cộng";
                    worksheet.Cells["A" + index + ":G" + index].Style.Font.Bold = true;

                    worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["H" + index].Value = groupChiPhiBangKes.Select(c => c.Sum(o => o.ThanhTienBV)).Sum();


                    worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["J" + index].Value = groupChiPhiBangKes.Select(c => c.Sum(o => o.ThanhTienBH)).Sum();

                    worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["K" + index].Value = groupChiPhiBangKes.Select(c => c.Sum(o => o.QuyBHYT)).Sum();

                    worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["L" + index].Value = groupChiPhiBangKes.Select(c => c.Sum(o => o.NguoiBenhCungChiTra)).Sum();

                    worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["M" + index].Value = groupChiPhiBangKes.Select(c => c.Sum(o => o.Khac)).Sum();

                    worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["N" + index].Value = groupChiPhiBangKes.Select(c => c.Sum(o => o.NguoiBenhTuTra)).Sum();

                    index++;

                    worksheet.Cells["A" + index + ":N" + index].Merge = true;
                    worksheet.Cells["A" + index + ":N" + index].Value = "Tổng chi phí lần khám bệnh/cả đợt điều trị: " + Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.ThanhTienBV)).ApplyFormatMoneyToDouble() + " đồng";
                    worksheet.Cells["A" + index + ":N" + index].Style.Font.Bold = true;

                    index++;

                    worksheet.Cells["A" + index + ":N" + index].Merge = true;
                    worksheet.Cells["A" + index + ":N" + index].Value = "(Viết bằng chữ: " + NumberHelper.ChuyenSoRaText(Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.ThanhTienBV))) + ")";

                    index++;

                    worksheet.Cells["A" + index + ":N" + index].Merge = true;
                    worksheet.Cells["A" + index + ":N" + index].Value = "Trong đó , số tiền do:";
                    worksheet.Cells["A" + index + ":N" + index].Style.Font.Bold = true;

                    index++;
                    worksheet.Cells["A" + index + ":N" + index].Merge = true;
                    worksheet.Cells["A" + index + ":N" + index].Value = "- Quỹ BHYT thanh toán: " + Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.QuyBHYT)).ApplyFormatMoneyToDouble();
                                       

                    index++;
                    var tongTamUng = GetSoTienDaTamUngAsync(thuPhiKhamChuaBenhVo.Id).Result;
                    worksheet.Cells["A" + index + ":N" + index].Merge = true;
                    worksheet.Cells["A" + index + ":N" + index].Value = "- Tổng tạm ứng: " + Convert.ToDouble(tongTamUng).ApplyFormatMoneyToDouble();


                    index++;
                    worksheet.Cells["A" + index + ":N" + index].Merge = true;
                    worksheet.Cells["A" + index + ":N" + index].Value = "+ Nguồn khác: " + Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.Khac)).ApplyFormatMoneyToDouble(true);

                    var tongQuyBHYTThanhToan = Convert.ToDouble(chiPhis.Sum(o => o.BHYTThanhToan));
                    var soTienCanThu = Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.ThanhTienBV)) - Convert.ToDouble(tongTamUng) - tongQuyBHYTThanhToan - Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.Khac));
                    string soTienNBTuTraHoacTraLaiBN = soTienCanThu < 0 ? $"- Phải trả lại NB: {(soTienCanThu * (-1)).ApplyFormatMoneyToDouble()}" : $"- Người bệnh tự trả: {(soTienCanThu).ApplyFormatMoneyToDouble()}";

                    index++;
                    worksheet.Cells["A" + index + ":N" + index].Merge = true;
                    worksheet.Cells["A" + index + ":N" + index].Value = soTienNBTuTraHoacTraLaiBN;


                    index = index + 2;

                    worksheet.Cells["G" + index + ":N" + index].Style.Font.Bold = true;
                    worksheet.Cells["G" + index + ":N" + index].Merge = true;
                    worksheet.Cells["G" + index + ":N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["G" + index + ":N" + index].Value = "XÁC NHẬN CỦA NGƯỜI BỆNH ";

                    index++;

                    worksheet.Cells["A" + index + ":F" + index].Style.Font.Bold = true;
                    worksheet.Cells["A" + index + ":F" + index].Merge = true;
                    worksheet.Cells["A" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A" + index + ":F" + index].Value = "NGƯỜI LẬP BẢNG KÊ";


                    worksheet.Cells["G" + index + ":N" + index].Style.Font.Bold = true;
                    worksheet.Cells["G" + index + ":N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["G" + index + ":N" + index].Merge = true;
                    worksheet.Cells["G" + index + ":N" + index].Value = "(Ký , Ghi rõ họ tên)";

                    index++;

                    worksheet.Cells["A" + index + ":F" + index].Style.Font.Bold = true;
                    worksheet.Cells["A" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A" + index + ":F" + index].Merge = true;
                    worksheet.Cells["A" + index + ":F" + index].Value = "(Ký , Ghi rõ họ tên)";


                    worksheet.Cells["G" + index + ":N" + index].Style.Font.Bold = true;
                    worksheet.Cells["G" + index + ":N" + index].Merge = true;
                    worksheet.Cells["G" + index + ":N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["G" + index + ":N" + index].Value = "(Tôi đã nhận...phim...Xquang / CT / MRI)";

                    xlPackage.Save();
                }

                return stream.ToArray();
            }
        }

        #endregion
    }
}