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
        public List<LookupItemVo> GetTatKhoaChoDanhSachBARaVienChuaXacNhanHoanTatChiPhi(DropDownListRequestModel queryInfo)
        {
            var allKhoas = new List<LookupItemVo>()
            {
                new LookupItemVo {KeyId = 0 , DisplayName = "Toàn viện" }
            };
            var result = _KhoaPhongRepository.TableNoTracking
               .Select(s => new LookupItemVo
               {
                   KeyId = s.Id,
                   DisplayName = s.Ten,
               })
               .ApplyLike(queryInfo.Query, o => o.DisplayName)
               .Take(queryInfo.Take);

            allKhoas.AddRange(result);

            return allKhoas;
        }

        public async Task<GridDataSource> GetDataDanhSachBARaVienChuaXacNhanHoanTatChiPhiForGrid(DanhSachBARaVienChuaXacNhanHoanTatChiPhiQueryInfoVo queryInfo)
        {
            var queryBenhAn = _noiTruBenhAnRepository.TableNoTracking
                .Where(o => o.ThoiDiemNhapVien >= queryInfo.TuNgay && o.ThoiDiemNhapVien < queryInfo.DenNgay
                            && (queryInfo.KhoaId == 0 || o.KhoaPhongNhapVienId == queryInfo.KhoaId));
            if (!string.IsNullOrEmpty(queryInfo.TimKiem))
            {
                queryBenhAn = queryBenhAn.ApplyLike(queryInfo.TimKiem, o => o.YeuCauTiepNhan.MaYeuCauTiepNhan, o => o.YeuCauTiepNhan.HoTen, o => o.SoBenhAn, o => o.YeuCauTiepNhan.BenhNhan.MaBN);
            }
            var dataBenhAn = queryBenhAn.Select(o => new             
            { 
                o.YeuCauTiepNhan.Id,
                o.YeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId,
                o.KhoaPhongNhapVienId,
                o.SoBenhAn,
                o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                o.YeuCauTiepNhan.BenhNhanId,
                o.YeuCauTiepNhan.BenhNhan.MaBN,
                o.YeuCauTiepNhan.HoTen,
                o.YeuCauTiepNhan.BHYTMaSoThe,
                o.ThoiDiemNhapVien,
                o.ThoiDiemRaVien,
                KhoaPhongDieuTris = o.NoiTruKhoaPhongDieuTris.Select(k=>new {k.ThoiDiemVaoKhoa, k.KhoaPhongChuyenDenId}).ToList(),
            });
            var yeuCauTiepNhanIds = dataBenhAn.Select(o=>o.Id).Concat(dataBenhAn.Select(o => o.YeuCauTiepNhanNgoaiTruCanQuyetToanId.GetValueOrDefault())).Distinct().ToList();
            
            var dataPhieuThu = _taiKhoanBenhNhanThuRepository.TableNoTracking
                .Where(o => o.LoaiNoiThu == Enums.LoaiNoiThu.ThuNgan && (o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi || o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTamUng) &&
                            o.DaHuy != true && yeuCauTiepNhanIds.Contains(o.YeuCauTiepNhanId))
                .Select(o=>new
                {
                    o.YeuCauTiepNhanId,
                    o.LoaiThuTienBenhNhan,
                    o.ThuTienGoiDichVu,
                    o.TienMat,
                    o.ChuyenKhoan,
                    o.POS,
                    o.PhieuHoanUngId,
                    PhieuChis = o.TaiKhoanBenhNhanChis.Select(chi => new PhieuChiDataVo { LoaiChiTienBenhNhan = chi.LoaiChiTienBenhNhan, TienChiPhi = chi.TienChiPhi }).ToList(),
                }).ToList();

            var benhNhanIds = dataBenhAn.Select(o => o.BenhNhanId.GetValueOrDefault()).Distinct().ToList();

            var dataYeuCauGoiDv = _yeuCauGoiDichVuRepository.TableNoTracking
                .Where(o => o.DaQuyetToan != true && benhNhanIds.Contains(o.BenhNhanId))
                .Select(o => new
                {
                    o.BenhNhanId,
                    o.SoTienBenhNhanDaChi,
                    PhieuChis = o.TaiKhoanBenhNhanChis.Where(chi => chi.LoaiChiTienBenhNhan == LoaiChiTienBenhNhan.HoanUng && chi.DaHuy != true).Select(chi => chi.TienMat.GetValueOrDefault()).ToList(),
                });

            var khoas = _KhoaPhongRepository.TableNoTracking.Select(o => new { o.Id, o.Ten }).ToList();

            var returnData = new List<DanhSachBARaVienChuaXacNhanHoanTatChiPhi>();
            foreach (var benhAn in dataBenhAn.OrderBy(o=>o.ThoiDiemNhapVien))
            {
                var khoaRaVien = "";
                if(benhAn.ThoiDiemRaVien != null && benhAn.KhoaPhongDieuTris.Any())
                {
                    khoaRaVien = khoas.First(o=>o.Id == benhAn.KhoaPhongDieuTris.OrderBy(d => d.ThoiDiemVaoKhoa).Last().KhoaPhongChuyenDenId).Ten;
                }

                var phieuThuTrongGois = dataPhieuThu
                    .Where(o => o.ThuTienGoiDichVu == true && o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi && 
                                (o.YeuCauTiepNhanId == benhAn.Id || o.YeuCauTiepNhanId == benhAn.YeuCauTiepNhanNgoaiTruCanQuyetToanId))
                    .ToList();
                var yeuCauGoiDvs = dataYeuCauGoiDv.Where(o => o.BenhNhanId == benhAn.BenhNhanId).ToList();
                if(phieuThuTrongGois.Any() || yeuCauGoiDvs.Any())
                {
                    returnData.Add(new DanhSachBARaVienChuaXacNhanHoanTatChiPhi
                    {
                        Id = benhAn.Id,
                        MaNB = benhAn.MaBN,
                        MaTN = benhAn.MaYeuCauTiepNhan,
                        SoBenhAn = benhAn.SoBenhAn,
                        TenBenhNhan = benhAn.HoTen,
                        SoTheBHYT = benhAn.BHYTMaSoThe,
                        NgayVaoVien = benhAn.ThoiDiemNhapVien,
                        NgayRaVien = benhAn.ThoiDiemRaVien,
                        KhoaRaVien = khoaRaVien,
                        NhapVien = true,
                        TamUng = yeuCauGoiDvs.Select(o=>o.SoTienBenhNhanDaChi.GetValueOrDefault()).DefaultIfEmpty().Sum(),
                        HoanUng = yeuCauGoiDvs.Select(o => o.PhieuChis.DefaultIfEmpty().Sum()).DefaultIfEmpty().Sum(),
                        DaThanhToan = phieuThuTrongGois.Select(o=>o.PhieuChis.Where(chi => chi.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi).Select(chi => chi.TienChiPhi.GetValueOrDefault()).DefaultIfEmpty().Sum()).DefaultIfEmpty().Sum()
                    });
                }

                var phieuTamUngs = dataPhieuThu
                    .Where(o => o.ThuTienGoiDichVu != true && o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTamUng &&
                                (o.YeuCauTiepNhanId == benhAn.Id || o.YeuCauTiepNhanId == benhAn.YeuCauTiepNhanNgoaiTruCanQuyetToanId))
                    .ToList();

                var phieuThuNgoaiGois = dataPhieuThu
                    .Where(o => o.ThuTienGoiDichVu != true && o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi &&
                                (o.YeuCauTiepNhanId == benhAn.Id || o.YeuCauTiepNhanId == benhAn.YeuCauTiepNhanNgoaiTruCanQuyetToanId))
                    .ToList();

                returnData.Add(new DanhSachBARaVienChuaXacNhanHoanTatChiPhi
                {
                    Id = benhAn.Id,
                    MaNB = benhAn.MaBN,
                    MaTN = benhAn.MaYeuCauTiepNhan,
                    SoBenhAn = benhAn.SoBenhAn,
                    TenBenhNhan = benhAn.HoTen,
                    SoTheBHYT = benhAn.BHYTMaSoThe,
                    NgayVaoVien = benhAn.ThoiDiemNhapVien,
                    NgayRaVien = benhAn.ThoiDiemRaVien,
                    KhoaRaVien = khoaRaVien,
                    TamUng = phieuTamUngs.Select(o => o.TienMat.GetValueOrDefault() + o.ChuyenKhoan.GetValueOrDefault() + o.POS.GetValueOrDefault()).DefaultIfEmpty().Sum(),
                    HoanUng = phieuTamUngs.Where(o=>o.PhieuHoanUngId != null).Select(o => o.TienMat.GetValueOrDefault() + o.ChuyenKhoan.GetValueOrDefault() + o.POS.GetValueOrDefault()).DefaultIfEmpty().Sum(),
                    DaThanhToan = phieuThuNgoaiGois.Select(o => o.PhieuChis.Where(chi => chi.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi).Select(chi => chi.TienChiPhi.GetValueOrDefault()).DefaultIfEmpty().Sum()).DefaultIfEmpty().Sum()
                });
            }


            //var returnData = new List<DanhSachBARaVienChuaXacNhanHoanTatChiPhi>() {
            //    new DanhSachBARaVienChuaXacNhanHoanTatChiPhi() {Id = 1 , MaNB ="201212512" , MaTN = "228565656", SoBenhAn ="SBA228565656" , TenBenhNhan ="Nguyễn văn a" , SoTheBHYT="DN126544321" ,NhapVien = true ,NgayVaoVien = DateTime.Now ,NgayRaVien = DateTime.Now ,KhoaRaVien="Khoa Ngoại", TamUng = 100000 ,HoanUng=70000,DaThanhToan=10000 },
            //    new DanhSachBARaVienChuaXacNhanHoanTatChiPhi() {Id = 2 , MaNB ="201212513" , MaTN = "2285656546", SoBenhAn ="SBA2285656356" , TenBenhNhan ="Nguyễn văn b" , SoTheBHYT="DN126544321" ,NhapVien = true ,NgayVaoVien = DateTime.Now ,NgayRaVien = DateTime.Now ,KhoaRaVien="Khoa Ngoại", TamUng = 100000 ,HoanUng=70000,DaThanhToan=10000 },
            //    new DanhSachBARaVienChuaXacNhanHoanTatChiPhi() {Id = 3 , MaNB ="201212513" , MaTN = "2285656546", SoBenhAn ="SBA2285656156" , TenBenhNhan ="Nguyễn văn b" , SoTheBHYT="DN126544321" ,NhapVien = true ,NgayVaoVien = DateTime.Now ,NgayRaVien = DateTime.Now ,KhoaRaVien="Khoa Ngoại", TamUng = 100000 ,HoanUng=70000,DaThanhToan=10000 }
            //};
            return new GridDataSource
            {
                Data = returnData.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray(),
                TotalRowCount = returnData.Count()
            };
        }

        public virtual byte[] ExportDanhSachBARaVienChuaXacNhanHoanTatChiPhi(GridDataSource gridDataSource, DanhSachBARaVienChuaXacNhanHoanTatChiPhiQueryInfoVo query)
        {
            var datas = (ICollection<DanhSachBARaVienChuaXacNhanHoanTatChiPhi>)gridDataSource.Data;

            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<DanhSachBARaVienChuaXacNhanHoanTatChiPhi>("STT", p => ind++)
            };

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("[KT] DS BA ra viện chưa xác nhận hoàn tất chi phí ");
                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 15;
                    worksheet.Column(3).Width = 20;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 20;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 30;
                    worksheet.Column(8).Width = 15;
                    worksheet.Column(9).Width = 35;
                    worksheet.Column(10).Width = 15;
                    worksheet.Column(11).Width = 20;
                    worksheet.Column(12).Width = 20;
                    worksheet.Column(13).Width = 20;
                    worksheet.Column(14).Width = 20;
                    worksheet.Column(15).Width = 20;
                    worksheet.Column(16).Width = 20;
                    worksheet.Column(17).Width = 20;
                    worksheet.Column(18).Width = 20;
                    worksheet.Column(19).Width = 20;
                    worksheet.Column(20).Width = 20;
                    worksheet.Column(21).Width = 20;
                    worksheet.Column(22).Width = 20;
                    worksheet.Column(23).Width = 20;

                    worksheet.DefaultColWidth = 7;
                    worksheet.Row(1).Height = 32;

                    using (var range = worksheet.Cells["A1:M1"])
                    {
                        range.Worksheet.Cells["A1:M1"].Merge = true;
                        range.Worksheet.Cells["A1:M1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ" + Environment.NewLine + "Phòng Tài Chính Kế Toán";
                        range.Worksheet.Cells["A1:M1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:M1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:M1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:M1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:M1"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A1:M1"].Style.WrapText = true;
                    }

                    using (var range = worksheet.Cells["A2:M2"])
                    {
                        range.Worksheet.Cells["A2:M2"].Merge = true;
                        range.Worksheet.Cells["A2:M2"].Value = "CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM";
                        range.Worksheet.Cells["A2:M2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A2:M2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A2:M2"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A2:M2"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A2:M2"].Style.Font.Bold = true;
                    }
                    using (var range = worksheet.Cells["A3:M3"])
                    {
                        range.Worksheet.Cells["A3:M3"].Merge = true;
                        range.Worksheet.Cells["A3:M3"].Value = " Độc lập - Tự do - Hạnh phúc";
                        range.Worksheet.Cells["A3:M3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:M3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:M3"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A3:M3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:M3"].Style.Font.Bold = true;
                       
                    }

                    using (var range = worksheet.Cells["A4:M4"])
                    {
                        range.Worksheet.Cells["A4:M4"].Merge = true;
                        range.Worksheet.Cells["A4:M4"].Value = "DANH SÁCH NGƯỜI BỆNH CHƯA XÁC NHẬN HOÀN TẤT CHI PHÍ";
                        range.Worksheet.Cells["A4:M4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:M4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A4:M4"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A4:M4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:M4"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A5:M5"])
                    {
                        range.Worksheet.Cells["A5:M5"].Merge = true;
                        range.Worksheet.Cells["A5:M5"].Value = "Từ ngày: " + query.TuNgay.FormatNgayGioTimKiemTrenBaoCao()
                                                          + " - đến ngày: " + query.DenNgay.FormatNgayGioTimKiemTrenBaoCao();

                        range.Worksheet.Cells["A5:M5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A5:M5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A5:M5"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A5:M5"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A5:M5"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A6:M7"])
                    {
                        range.Worksheet.Cells["A6:M7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A6:M7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A6:M7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A6:M7"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A6:M7"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A6:M7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A6:A7"].Merge = true;
                        range.Worksheet.Cells["A6:A7"].Value = "STT";
                        range.Worksheet.Cells["A6:A7"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A6:A7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["B6:B7"].Merge = true;
                        range.Worksheet.Cells["B6:B7"].Value = "Mã BN";
                        range.Worksheet.Cells["B6:B7"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["B6:B7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["C6:C7"].Merge = true;
                        range.Worksheet.Cells["C6:C7"].Value = "Mã TN";
                        range.Worksheet.Cells["C6:C7"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["C6:C7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["D6:D7"].Merge = true;
                        range.Worksheet.Cells["D6:D7"].Value = "Số bệnh án";
                        range.Worksheet.Cells["D6:D7"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["D6:D7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["E6:E7"].Merge = true;
                        range.Worksheet.Cells["E6:E7"].Value = "Tên bệnh nhân ";
                        range.Worksheet.Cells["E6:E7"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["E6:E7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["F6:F7"].Merge = true;
                        range.Worksheet.Cells["F6:F7"].Value = "Số BHYT";
                        range.Worksheet.Cells["F6:F7"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["F6:F7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["G6:G7"].Merge = true;
                        range.Worksheet.Cells["G6:G7"].Value = "Ngày vào viện";
                        range.Worksheet.Cells["G6:G7"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["G6:G7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["H6:H7"].Merge = true;
                        range.Worksheet.Cells["H6:H7"].Value = "Ngày ra viện";
                        range.Worksheet.Cells["H6:H7"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["H6:H7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        
                        range.Worksheet.Cells["I6:I7"].Merge = true;
                        range.Worksheet.Cells["I6:I7"].Value = "Khoa RV";
                        range.Worksheet.Cells["I6:I7"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["I6:I7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["J6:J7"].Merge = true;
                        range.Worksheet.Cells["J6:J7"].Value = "Trong gói";
                        range.Worksheet.Cells["J6:J7"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["J6:J7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["K6:M6"].Merge = true;
                        range.Worksheet.Cells["K6:M6"].Value = "Thanh toán";
                        range.Worksheet.Cells["K6:M6"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["K6:M6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["K7:K7"].Merge = true;
                        range.Worksheet.Cells["K7:K7"].Value = "Tạm ứng";
                        range.Worksheet.Cells["K7:K7"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["K7:K7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["L7:L7"].Merge = true;
                        range.Worksheet.Cells["L7:L7"].Value = "Hoàn ứng";
                        range.Worksheet.Cells["L7:L7"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["L7:L7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["M7:M7"].Merge = true;
                        range.Worksheet.Cells["M7:M7"].Value = "Đã thanh toán";
                        range.Worksheet.Cells["M7:M7"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["M7:M7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }

                    var manager = new PropertyManager<DanhSachBARaVienChuaXacNhanHoanTatChiPhi>(requestProperties);
                    int index = 8;

                    var stt = 1;
                    if (datas.Any())
                    {
                        foreach (var item in datas)
                        {
                            using (var range = worksheet.Cells["A" + index + ":M" + index])
                            {
                                range.Worksheet.Cells["A" + index + ":M" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                range.Worksheet.Cells["A" + index + ":M" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                range.Worksheet.Cells["A" + index + ":M" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["A" + index + ":M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["A" + index].Value = stt;

                                worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["B" + index].Value = item.MaNB;

                                worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["C" + index].Value = item.MaTN;

                                worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["D" + index].Value = item.SoBenhAn;

                                worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["E" + index].Value = item.TenBenhNhan;

                                worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["F" + index].Value = item.SoTheBHYT;

                                worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["G" + index].Value = item.NgayVaoVienDisplay;

                                worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["H" + index].Value = item.NgayRaVienDisplay;

                                worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["I" + index].Value = item.KhoaRaVien;

                                worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                worksheet.Cells["J" + index].Value = item.NhapVien ? "X" : string.Empty;                                

                                worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["K" + index].Value = item.TamUng;

                                worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["L" + index].Value = item.HoanUng;

                                worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["M" + index].Value = item.DaThanhToan;

                                index++;
                                stt++;
                            }
                        }
                    }

                    var dateNow = DateTime.Now;

                    using (var range = worksheet.Cells["H" + index + ":J" + index])
                    {
                        range.Worksheet.Cells["H" + index + ":J" + index].Merge = true;
                        range.Worksheet.Cells["H" + index + ":J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["H" + index + ":J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["H" + index + ":J" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["H" + index + ":J" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["H" + index + ":J" + index].Value = "Ngày " + dateNow.Day + " tháng " + dateNow.Month + " năm " + dateNow.Year;                  
                    }
                    index++;
                    using (var range = worksheet.Cells["H" + index + ":J" + index])
                    {
                        range.Worksheet.Cells["H" + index + ":J" + index].Merge = true;
                        range.Worksheet.Cells["H" + index + ":J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["H" + index + ":J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["H" + index + ":J" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["H" + index + ":J" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["H" + index + ":J" + index].Value = "NGƯỜI LẬP";
                        range.Worksheet.Cells["H" + index + ":J" + index].Style.Font.Bold = true;
                    }
                    index++;
                    using (var range = worksheet.Cells["H" + index + ":J" + index])
                    {
                        range.Worksheet.Cells["H" + index + ":J" + index].Merge = true;
                        range.Worksheet.Cells["H" + index + ":J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["H" + index + ":J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["H" + index + ":J" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["H" + index + ":J" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["H" + index + ":J" + index].Style.Font.Italic = true;
                        range.Worksheet.Cells["H" + index + ":J" + index].Value = "(Ký, ghi rõ họ tên)";
                    }

                    xlPackage.Save();
                }
                return stream.ToArray();

            }
        }
    }
}
