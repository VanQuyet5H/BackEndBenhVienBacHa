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
        public async Task<GridDataSource> GetDataTinhHinhBenhTatTuVongForGrid(TinhHinhBenhTatTuVongQueryInfoVo queryInfo)
        {
            var dataKhamBenh = _yeuCauKhamBenhRepository.TableNoTracking
                .Where(o => o.ThoiDiemHoanThanh != null && o.ThoiDiemHoanThanh >= queryInfo.TuNgay && o.ThoiDiemHoanThanh < queryInfo.DenNgay &&
                            o.YeuCauTiepNhan.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru && o.YeuCauTiepNhan.QuyetToanTheoNoiTru != true && o.IcdchinhId != null)
                .Select(o => new TinhHinhBenhTatTuVongKhamBenhData
                {
                    Id = o.Id,
                    YeuCauTiepNhanId = o.YeuCauTiepNhanId,
                    ThoiDiemTiepNhan = o.YeuCauTiepNhan.ThoiDiemTiepNhan,
                    NgaySinh = o.YeuCauTiepNhan.NgaySinh,
                    ThangSinh = o.YeuCauTiepNhan.ThangSinh,
                    NamSinh = o.YeuCauTiepNhan.NamSinh,
                    GioiTinh = o.YeuCauTiepNhan.GioiTinh,
                    MaICD = o.Icdchinh.Ma,
                    CoTuVong = o.CoTuVong,
                    MaICDKhacs = o.YeuCauTiepNhan.YeuCauKhamBenhs.Where(k => k.IcdchinhId != null).Select(k => k.Icdchinh.Ma).ToList()
                }).ToList();

            var dataKhamBenhTruKSK = dataKhamBenh.Where(o => o.MaICDKhacs.All(i => i != "Z00.0")).ToList();

            var dataBenhAn = _noiTruBenhAnRepository.TableNoTracking
                .Where(o=>o.ThoiDiemRaVien != null && o.ThoiDiemRaVien >= queryInfo.TuNgay && o.ThoiDiemRaVien < queryInfo.DenNgay && o.ChanDoanChinhRaVienICDId != null &&
                            o.YeuCauTiepNhan.YeuCauNhapVienId != null && o.YeuCauTiepNhan.YeuCauNhapVien.YeuCauTiepNhanMeId == null)
                .Select(o => new TinhHinhBenhTatTuVongBenhAnData
                {
                    Id = o.Id,
                    YeuCauTiepNhanId = o.Id,
                    ThoiDiemNhapVien = o.ThoiDiemNhapVien,
                    ThoiDiemRaVien = o.ThoiDiemRaVien.Value,
                    NgaySinh = o.YeuCauTiepNhan.NgaySinh,
                    ThangSinh = o.YeuCauTiepNhan.ThangSinh,
                    NamSinh = o.YeuCauTiepNhan.NamSinh,
                    GioiTinh = o.YeuCauTiepNhan.GioiTinh,
                    MaICD = o.ChanDoanChinhRaVienICD.Ma,
                    HinhThucRaVien = o.HinhThucRaVien
                }).ToList();

            var chuongICDs = _chuongICDRepository.TableNoTracking.Where(o => o.CoBaoCao == true).OrderBy(o => o.Stt).ToList();
            var nhomICDTheoBenhViens = _nhomICDTheoBenhVienRepository.TableNoTracking.Where(o => o.HieuLuc).ToList();

            var danhSachTinhHinhBenhTatTuVongs = new List<DanhSachTinhHinhBenhTatTuVong>();

            foreach(var chuongICD in chuongICDs)
            {
                var tinhHinhBenhTatTuVongChuong = new DanhSachTinhHinhBenhTatTuVong
                {
                    STT = chuongICD.Stt,
                    TenNhomBenh = string.IsNullOrEmpty(chuongICD.TenTiengAnhTheoBenhVien) ? chuongICD.TenTiengVietTheoBenhVien : $"{chuongICD.TenTiengVietTheoBenhVien} - {chuongICD.TenTiengAnhTheoBenhVien}",
                    MaICD = chuongICD.Ma,
                    LaChuong = true,
                    ChuongId = chuongICD.Id
                };
                danhSachTinhHinhBenhTatTuVongs.Add(tinhHinhBenhTatTuVongChuong);
                var danhSachTinhHinhBenhTatTuVongTheoChuongs = new List<DanhSachTinhHinhBenhTatTuVong>();

                var nhomICDTheoChuong = nhomICDTheoBenhViens.Where(o => o.ChuongICDId == chuongICD.Id).OrderBy(o => o.Stt);
                foreach (var nhomICD in nhomICDTheoChuong)
                {
                    var tinhHinhBenhTatTuVongNhom = new DanhSachTinhHinhBenhTatTuVong
                    {
                        STT = nhomICD.Stt,
                        TenNhomBenh = string.IsNullOrEmpty(nhomICD.TenTiengAnh) ? nhomICD.TenTiengViet : $"{nhomICD.TenTiengViet} - {nhomICD.TenTiengAnh}",
                        MaICD = nhomICD.Ma,
                        LaChuong = false,
                        ChuongId = chuongICD.Id
                    };
                    if (!string.IsNullOrEmpty(nhomICD.MoTa))
                    {
                        var arrICD = nhomICD.MoTa.ToLower().Split(';');
                        var dataKhamBenhThuocNhom = dataKhamBenhTruKSK.Where(o => o.MaICD != null && o.MaICD.Length >= 3 && arrICD.Contains(o.MaICD.ToLower().Substring(0, 3))).ToList();
                        var dataBenhAnThuocNhom = dataBenhAn.Where(o => o.MaICD != null && o.MaICD.Length >= 3 && arrICD.Contains(o.MaICD.ToLower().Substring(0, 3))).ToList();

                        tinhHinhBenhTatTuVongNhom.HDKBSoLanKhamChung = dataKhamBenhThuocNhom.Count();
                        tinhHinhBenhTatTuVongNhom.HDKBSoLanKhamTreEm = dataKhamBenhThuocNhom.Count(o=>o.TreEmDen15Tuoi);
                        tinhHinhBenhTatTuVongNhom.HDKBSoTuVong = dataKhamBenhThuocNhom.Count(o => o.CoTuVong == true);

                        tinhHinhBenhTatTuVongNhom.Tren60TMacBenhTS = dataKhamBenhThuocNhom.Count(o => o.NguoiHon60Tuoi);
                        tinhHinhBenhTatTuVongNhom.Tren60TMacBenhLaNu = dataKhamBenhThuocNhom.Count(o => o.NguoiHon60Tuoi && o.GioiTinh == LoaiGioiTinh.GioiTinhNu);
                        tinhHinhBenhTatTuVongNhom.Tren60TTuVongTS = dataKhamBenhThuocNhom.Count(o => o.NguoiHon60Tuoi && o.CoTuVong == true);
                        tinhHinhBenhTatTuVongNhom.Tren60TTuVongLaNu = dataKhamBenhThuocNhom.Count(o => o.NguoiHon60Tuoi && o.CoTuVong == true && o.GioiTinh == LoaiGioiTinh.GioiTinhNu);

                        tinhHinhBenhTatTuVongNhom.TSBNSoMacBenh = dataBenhAnThuocNhom.Count();
                        tinhHinhBenhTatTuVongNhom.TSBNSoTuVong = dataBenhAnThuocNhom.Count(o => o.HinhThucRaVien == EnumHinhThucRaVien.TuVong || o.HinhThucRaVien == EnumHinhThucRaVien.TuVongTruoc24H);
                        tinhHinhBenhTatTuVongNhom.TSBNNgayDieuTri = dataBenhAnThuocNhom.Select(o => (int)Math.Round((o.ThoiDiemRaVien.Date - o.ThoiDiemNhapVien.Date).TotalDays, 0) + (o.ThoiDiemNhapVien < o.ThoiDiemNhapVien.Date.AddHours(20) ? 1 : 0)).DefaultIfEmpty().Sum();

                        tinhHinhBenhTatTuVongNhom.TEMacTS = dataBenhAnThuocNhom.Count(o => o.TreEmDen15Tuoi);
                        tinhHinhBenhTatTuVongNhom.TEMacDuoi6T = dataBenhAnThuocNhom.Count(o => o.TreEmDuoi6Tuoi);

                        tinhHinhBenhTatTuVongNhom.TESoTuVongTS = dataBenhAnThuocNhom.Count(o => o.TreEmDen15Tuoi && (o.HinhThucRaVien == EnumHinhThucRaVien.TuVong || o.HinhThucRaVien == EnumHinhThucRaVien.TuVongTruoc24H));
                        tinhHinhBenhTatTuVongNhom.TESoTuVongDuoi6T = dataBenhAnThuocNhom.Count(o => o.TreEmDuoi6Tuoi && (o.HinhThucRaVien == EnumHinhThucRaVien.TuVong || o.HinhThucRaVien == EnumHinhThucRaVien.TuVongTruoc24H));

                        tinhHinhBenhTatTuVongNhom.TENgayDieuTriTS = dataBenhAnThuocNhom.Where(o=>o.TreEmDen15Tuoi).Select(o => (int)Math.Round((o.ThoiDiemRaVien.Date - o.ThoiDiemNhapVien.Date).TotalDays, 0) + (o.ThoiDiemNhapVien < o.ThoiDiemNhapVien.Date.AddHours(20) ? 1 : 0)).DefaultIfEmpty().Sum();
                        tinhHinhBenhTatTuVongNhom.TENgayDieuTriDuoi6T = dataBenhAnThuocNhom.Where(o => o.TreEmDuoi6Tuoi).Select(o => (int)Math.Round((o.ThoiDiemRaVien.Date - o.ThoiDiemNhapVien.Date).TotalDays, 0) + (o.ThoiDiemNhapVien < o.ThoiDiemNhapVien.Date.AddHours(20) ? 1 : 0)).DefaultIfEmpty().Sum();                        
                    }
                    danhSachTinhHinhBenhTatTuVongs.Add(tinhHinhBenhTatTuVongNhom);
                    danhSachTinhHinhBenhTatTuVongTheoChuongs.Add(tinhHinhBenhTatTuVongNhom);
                }

                tinhHinhBenhTatTuVongChuong.HDKBSoLanKhamChung = danhSachTinhHinhBenhTatTuVongTheoChuongs.Select(o=>o.HDKBSoLanKhamChung.GetValueOrDefault()).DefaultIfEmpty().Sum();
                tinhHinhBenhTatTuVongChuong.HDKBSoLanKhamTreEm = danhSachTinhHinhBenhTatTuVongTheoChuongs.Select(o => o.HDKBSoLanKhamTreEm.GetValueOrDefault()).DefaultIfEmpty().Sum();
                tinhHinhBenhTatTuVongChuong.HDKBSoTuVong = danhSachTinhHinhBenhTatTuVongTheoChuongs.Select(o => o.HDKBSoTuVong.GetValueOrDefault()).DefaultIfEmpty().Sum();

                tinhHinhBenhTatTuVongChuong.Tren60TMacBenhTS = danhSachTinhHinhBenhTatTuVongTheoChuongs.Select(o => o.Tren60TMacBenhTS.GetValueOrDefault()).DefaultIfEmpty().Sum();
                tinhHinhBenhTatTuVongChuong.Tren60TMacBenhLaNu = danhSachTinhHinhBenhTatTuVongTheoChuongs.Select(o => o.Tren60TMacBenhLaNu.GetValueOrDefault()).DefaultIfEmpty().Sum();
                tinhHinhBenhTatTuVongChuong.Tren60TTuVongTS = danhSachTinhHinhBenhTatTuVongTheoChuongs.Select(o => o.Tren60TTuVongTS.GetValueOrDefault()).DefaultIfEmpty().Sum();
                tinhHinhBenhTatTuVongChuong.Tren60TTuVongLaNu = danhSachTinhHinhBenhTatTuVongTheoChuongs.Select(o => o.Tren60TTuVongLaNu.GetValueOrDefault()).DefaultIfEmpty().Sum();

                tinhHinhBenhTatTuVongChuong.TSBNSoMacBenh = danhSachTinhHinhBenhTatTuVongTheoChuongs.Select(o => o.TSBNSoMacBenh.GetValueOrDefault()).DefaultIfEmpty().Sum();
                tinhHinhBenhTatTuVongChuong.TSBNSoTuVong = danhSachTinhHinhBenhTatTuVongTheoChuongs.Select(o => o.TSBNSoTuVong.GetValueOrDefault()).DefaultIfEmpty().Sum();
                tinhHinhBenhTatTuVongChuong.TSBNNgayDieuTri = danhSachTinhHinhBenhTatTuVongTheoChuongs.Select(o => o.TSBNNgayDieuTri.GetValueOrDefault()).DefaultIfEmpty().Sum();

                tinhHinhBenhTatTuVongChuong.TEMacTS = danhSachTinhHinhBenhTatTuVongTheoChuongs.Select(o => o.TEMacTS.GetValueOrDefault()).DefaultIfEmpty().Sum();
                tinhHinhBenhTatTuVongChuong.TEMacDuoi6T = danhSachTinhHinhBenhTatTuVongTheoChuongs.Select(o => o.TEMacDuoi6T.GetValueOrDefault()).DefaultIfEmpty().Sum();

                tinhHinhBenhTatTuVongChuong.TESoTuVongTS = danhSachTinhHinhBenhTatTuVongTheoChuongs.Select(o => o.TESoTuVongTS.GetValueOrDefault()).DefaultIfEmpty().Sum();
                tinhHinhBenhTatTuVongChuong.TESoTuVongDuoi6T = danhSachTinhHinhBenhTatTuVongTheoChuongs.Select(o => o.TESoTuVongDuoi6T.GetValueOrDefault()).DefaultIfEmpty().Sum();

                tinhHinhBenhTatTuVongChuong.TENgayDieuTriTS = danhSachTinhHinhBenhTatTuVongTheoChuongs.Select(o => o.TENgayDieuTriTS.GetValueOrDefault()).DefaultIfEmpty().Sum();
                tinhHinhBenhTatTuVongChuong.TENgayDieuTriDuoi6T = danhSachTinhHinhBenhTatTuVongTheoChuongs.Select(o => o.TENgayDieuTriDuoi6T.GetValueOrDefault()).DefaultIfEmpty().Sum();
            }

            var returnData = danhSachTinhHinhBenhTatTuVongs
                .Where(o => o.HDKBSoLanKhamChung.GetValueOrDefault() != 0 ||
                            o.HDKBSoLanKhamTreEm.GetValueOrDefault() != 0 ||
                            o.HDKBSoTuVong.GetValueOrDefault() != 0 ||
                            o.Tren60TMacBenhTS.GetValueOrDefault() != 0 ||
                            o.Tren60TMacBenhLaNu.GetValueOrDefault() != 0 ||
                            o.Tren60TTuVongTS.GetValueOrDefault() != 0 ||
                            o.Tren60TTuVongLaNu.GetValueOrDefault() != 0 ||
                            o.TSBNSoMacBenh.GetValueOrDefault() != 0 ||
                            o.TSBNSoTuVong.GetValueOrDefault() != 0 ||
                            o.TSBNNgayDieuTri.GetValueOrDefault() != 0 ||
                            o.TEMacTS.GetValueOrDefault() != 0 ||
                            o.TEMacDuoi6T.GetValueOrDefault() != 0 ||
                            o.TESoTuVongTS.GetValueOrDefault() != 0 ||
                            o.TESoTuVongDuoi6T.GetValueOrDefault() != 0 ||
                            o.TENgayDieuTriTS.GetValueOrDefault() != 0 ||
                            o.TENgayDieuTriDuoi6T.GetValueOrDefault() != 0);

            return new GridDataSource
            {
                Data = returnData.ToArray(),
                TotalRowCount = returnData.Count(),
            };
        }

        public virtual byte[] ExportTinhHinhBenhTatTuVong(GridDataSource gridDataSource, TinhHinhBenhTatTuVongQueryInfoVo query)
        {
            var datas = (ICollection<DanhSachTinhHinhBenhTatTuVong>)gridDataSource.Data;

            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<DanhSachTinhHinhBenhTatTuVong>("TT", p => ind++)
            };

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("[KHTH] TÌNH HÌNH BỆNH TẬT TỬ VONG");
                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 15;
                    worksheet.Column(3).Width = 20;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 20;
                    worksheet.Column(6).Width = 20;
                    worksheet.Column(7).Width = 30;
                    worksheet.Column(8).Width = 20;
                    worksheet.Column(9).Width = 20;
                    worksheet.Column(10).Width = 20;
                    worksheet.Column(11).Width = 20;
                    worksheet.Column(12).Width = 20;
                    worksheet.Column(13).Width = 20;
                    worksheet.Column(14).Width = 20;
                    worksheet.Column(15).Width = 20;
                    worksheet.Column(16).Width = 20;
                    worksheet.Column(17).Width = 20;
                    worksheet.Column(18).Width = 20;
                    worksheet.Column(19).Width = 20;

                    worksheet.DefaultColWidth = 7;
                    worksheet.Row(8).Height = 24;

                    using (var range = worksheet.Cells["A1:T1"])
                    {
                        range.Worksheet.Cells["A1:T1"].Merge = true;
                        range.Worksheet.Cells["A1:T1"].Value = "TÌNH HÌNH BỆNH TẬT TỬ VONG";
                        range.Worksheet.Cells["A1:T1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A1:T1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:T1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:T1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:T1"].Style.Font.Bold = true;

                    }

                    using (var range = worksheet.Cells["A2:T2"])
                    {
                        range.Worksheet.Cells["A2:T2"].Merge = true;
                        range.Worksheet.Cells["A2:T2"].Value = "Từ ngày: " + query.TuNgay.FormatNgayGioTimKiemTrenBaoCao()
                                                          + " - đến ngày: " + query.DenNgay.FormatNgayGioTimKiemTrenBaoCao();
                        range.Worksheet.Cells["A2:T2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A2:T2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A2:T2"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A2:T2"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A2:T2"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A3:T6"])
                    {
                        range.Worksheet.Cells["A3:T6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:T6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        range.Worksheet.Cells["A3:T6"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A3:T6"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:T6"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A3:T6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A3:A6"].Merge = true;
                        range.Worksheet.Cells["A3:A6"].Value = "TT";
                        range.Worksheet.Cells["A3:A6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["B3:B6"].Merge = true;
                        range.Worksheet.Cells["B3:B6"].Value = "Số TT";
                        range.Worksheet.Cells["B3:B6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["C3:C6"].Merge = true;
                        range.Worksheet.Cells["C3:C6"].Value = "Tên bệnh/ nhóm bệnh";
                        range.Worksheet.Cells["C3:C6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["D3:D6"].Merge = true;
                        range.Worksheet.Cells["D3:D6"].Value = "Mã ICD 10";
                        range.Worksheet.Cells["D3:D6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        //Hoạt động khám bệnh
                        range.Worksheet.Cells["E3:G4"].Merge = true;
                        range.Worksheet.Cells["E3:G4"].Value = "Hoạt động khám bệnh";
                        range.Worksheet.Cells["E3:G4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["E5:E6"].Merge = true;
                        range.Worksheet.Cells["E5:E6"].Value = "Số lần khám chung";
                        range.Worksheet.Cells["E5:E6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["F5:F6"].Merge = true;
                        range.Worksheet.Cells["F5:F6"].Value = "Số lần khám trẻ em";
                        range.Worksheet.Cells["F5:F6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["G5:G6"].Merge = true;
                        range.Worksheet.Cells["G5:G6"].Value = "Số tử vong";
                        range.Worksheet.Cells["G5:G6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        //Hoạt động điều trị
                        range.Worksheet.Cells["H3:P3"].Merge = true;
                        range.Worksheet.Cells["H3:P3"].Value = "Hoạt động điều trị";
                        range.Worksheet.Cells["H3:P3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["H4:J4"].Merge = true;
                        range.Worksheet.Cells["H4:J4"].Value = "Tổng số bệnh nhân";
                        range.Worksheet.Cells["H4:J4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["K4:P4"].Merge = true;
                        range.Worksheet.Cells["K4:P4"].Value = "Trong đó TE <= 15 tuổi";
                        range.Worksheet.Cells["K4:P4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["K5:L5"].Merge = true;
                        range.Worksheet.Cells["K5:L5"].Value = "Mắc";
                        range.Worksheet.Cells["K5:L5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["M5:N5"].Merge = true;
                        range.Worksheet.Cells["M5:N5"].Value = "Số tử vong";
                        range.Worksheet.Cells["M5:N5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["O5:P5"].Merge = true;
                        range.Worksheet.Cells["O5:P5"].Value = "TS ngày điều trị";
                        range.Worksheet.Cells["O5:P5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["K6:K6"].Merge = true;
                        range.Worksheet.Cells["K6:K6"].Value = "TS";
                        range.Worksheet.Cells["K6:K6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["L6:L6"].Merge = true;
                        range.Worksheet.Cells["L6:L6"].Value = "<6 tuổi";
                        range.Worksheet.Cells["L6:L6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["M6:M6"].Merge = true;
                        range.Worksheet.Cells["M6:M6"].Value = "TS";
                        range.Worksheet.Cells["M6:M6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["N6:N6"].Merge = true;
                        range.Worksheet.Cells["N6:N6"].Value = "<6 tuổi";
                        range.Worksheet.Cells["N6:N6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["O6:O6"].Merge = true;
                        range.Worksheet.Cells["O6:O6"].Value = "TS";
                        range.Worksheet.Cells["O6:O6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["P6:P6"].Merge = true;
                        range.Worksheet.Cells["P6:P6"].Value = "<6 tuổi";
                        range.Worksheet.Cells["P6:P6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["H5:H6"].Merge = true;
                        range.Worksheet.Cells["H5:H6"].Value = "Số mắc bệnh";
                        range.Worksheet.Cells["H5:H6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["I5:I6"].Merge = true;
                        range.Worksheet.Cells["I5:I6"].Value = "Số tử vong";
                        range.Worksheet.Cells["I5:I6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["J5:J6"].Merge = true;
                        range.Worksheet.Cells["J5:J6"].Value = "Ngày điều trị";
                        range.Worksheet.Cells["J5:J6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        //Hoạt động điều trị
                        range.Worksheet.Cells["Q3:T3"].Merge = true;
                        range.Worksheet.Cells["Q3:T3"].Value = "Trong đó > 60 tuổi";
                        range.Worksheet.Cells["Q3:T3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["Q4:R5"].Merge = true;
                        range.Worksheet.Cells["Q4:R5"].Value = "Số mắc bệnh";
                        range.Worksheet.Cells["Q4:R5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["S4:T5"].Merge = true;
                        range.Worksheet.Cells["S4:T5"].Value = "Số tử vong";
                        range.Worksheet.Cells["S4:T5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["Q6:Q6"].Merge = true;
                        range.Worksheet.Cells["Q6:Q6"].Value = "T.Số";
                        range.Worksheet.Cells["Q6:Q6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["R6:R6"].Merge = true;
                        range.Worksheet.Cells["R6:R6"].Value = "Nữ";
                        range.Worksheet.Cells["R6:R6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["S6:S6"].Merge = true;
                        range.Worksheet.Cells["S6:S6"].Value = "T.Số";
                        range.Worksheet.Cells["S6:S6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["T6:T6"].Merge = true;
                        range.Worksheet.Cells["T6:T6"].Value = "Nữ";
                        range.Worksheet.Cells["T6:T6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }

                    var manager = new PropertyManager<DanhSachTinhHinhBenhTatTuVong>(requestProperties);
                    int index = 7;

                    var stt = 1;
                    if (datas.Any())
                    {
                        foreach (var item in datas)
                        {
                            using (var range = worksheet.Cells["A" + index + ":T" + index])
                            {
                                range.Worksheet.Cells["A" + index + ":T" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                range.Worksheet.Cells["A" + index + ":T" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["A" + index + ":T" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));

                                if (item.LaChuong == true)
                                {
                                    range.Worksheet.Cells["A" + index + ":T" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    Color green = System.Drawing.ColorTranslator.FromHtml("#c4edae");
                                    range.Worksheet.Cells["A" + index + ":T" + index].Style.Fill.BackgroundColor.SetColor(green);
                                    range.Worksheet.Cells["A" + index + ":T" + index].Style.Font.Bold = item.LaChuong ?? false;
                                }


                                worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["A" + index].Value = stt;

                                worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["B" + index].Value = item.STT;

                                worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["C" + index].Value = item.TenNhomBenh;

                                worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["D" + index].Value = item.MaICD;

                                worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["E" + index].Value = item.HDKBSoLanKhamChung;

                                worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["F" + index].Value = item.HDKBSoLanKhamTreEm;

                                worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["G" + index].Value = item.HDKBSoTuVong;

                                worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["H" + index].Value = item.TSBNSoMacBenh;

                                worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["I" + index].Value = item.TSBNSoTuVong;

                                worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["J" + index].Value = item.TSBNNgayDieuTri;

                                worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["K" + index].Value = item.TEMacTS;

                                worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["L" + index].Value = item.TEMacDuoi6T;

                                worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["M" + index].Value = item.TESoTuVongTS;

                                worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["N" + index].Value = item.TESoTuVongDuoi6T;

                                worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["O" + index].Value = item.TENgayDieuTriTS;

                                worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["P" + index].Value = item.TENgayDieuTriDuoi6T;

                                worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["Q" + index].Value = item.Tren60TMacBenhTS;

                                worksheet.Cells["R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["R" + index].Value = item.Tren60TMacBenhLaNu;

                                worksheet.Cells["S" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["S" + index].Value = item.Tren60TTuVongTS;

                                worksheet.Cells["T" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["T" + index].Value = item.Tren60TTuVongLaNu;

                                index++;
                                stt++;
                            }
                        }
                    }

                    using (var range = worksheet.Cells["A" + index + ":D" + index])
                    {
                        range.Worksheet.Cells["A" + index + ":D" + index].Merge = true;
                        range.Worksheet.Cells["A" + index + ":D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A" + index + ":D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":D" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":D" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":D" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A" + index + ":D" + index].Style.Font.Bold = true;
                        range.Worksheet.Cells["A" + index + ":D" + index].Value = "Tổng cộng";
                    }

                    worksheet.Cells["E" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["E" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["E" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["E" + index].Style.Font.Bold = true;
                    worksheet.Cells["E" + index].Value = datas.Where(c => c.LaChuong != true).Sum(x => x.HDKBSoLanKhamChung);

                    worksheet.Cells["F" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["F" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["F" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["F" + index].Style.Font.Bold = true;
                    worksheet.Cells["F" + index].Value = datas.Where(c => c.LaChuong != true).Sum(x => x.HDKBSoLanKhamTreEm);

                    worksheet.Cells["G" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["G" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["G" + index].Style.Font.Bold = true;
                    worksheet.Cells["G" + index].Value = datas.Where(c => c.LaChuong != true).Sum(x => x.HDKBSoTuVong);


                    worksheet.Cells["H" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["H" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["H" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["H" + index].Style.Font.Bold = true;
                    worksheet.Cells["H" + index].Value = datas.Where(c => c.LaChuong != true).Sum(x => x.TSBNSoMacBenh);

                    worksheet.Cells["I" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["I" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["I" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["I" + index].Style.Font.Bold = true;
                    worksheet.Cells["I" + index].Value = datas.Where(c => c.LaChuong != true).Sum(x => x.TSBNSoTuVong);

                    worksheet.Cells["J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["J" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["J" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["J" + index].Style.Font.Bold = true;
                    worksheet.Cells["J" + index].Value = datas.Where(c => c.LaChuong != true).Sum(x => x.TSBNNgayDieuTri);

                    worksheet.Cells["K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["K" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["K" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["K" + index].Style.Font.Bold = true;
                    worksheet.Cells["K" + index].Value = datas.Where(c => c.LaChuong != true).Sum(x => x.TEMacTS);

                    worksheet.Cells["L" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["L" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["L" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["L" + index].Style.Font.Bold = true;
                    worksheet.Cells["L" + index].Value = datas.Where(c => c.LaChuong != true).Sum(x => x.TEMacDuoi6T);



                    worksheet.Cells["M" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["M" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["M" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["M" + index].Style.Font.Bold = true;
                    worksheet.Cells["M" + index].Value = datas.Where(c => c.LaChuong != true).Sum(x => x.TESoTuVongTS);

                    worksheet.Cells["N" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["N" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["N" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["N" + index].Style.Font.Bold = true;
                    worksheet.Cells["N" + index].Value = datas.Where(c => c.LaChuong != true).Sum(x => x.TESoTuVongDuoi6T);


                    worksheet.Cells["O" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells["O" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["O" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["O" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["O" + index].Style.Font.Bold = true;
                    worksheet.Cells["O" + index].Value = datas.Where(c => c.LaChuong != true).Sum(x => x.TENgayDieuTriTS);

                    worksheet.Cells["P" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells["P" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["P" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["P" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["P" + index].Style.Font.Bold = true;
                    worksheet.Cells["P" + index].Value = datas.Where(c => c.LaChuong != true).Sum(x => x.TENgayDieuTriDuoi6T);


                    worksheet.Cells["Q" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells["Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["Q" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["Q" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["Q" + index].Style.Font.Bold = true;
                    worksheet.Cells["Q" + index].Value = datas.Where(c => c.LaChuong != true).Sum(x => x.Tren60TMacBenhTS);

                    worksheet.Cells["R" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells["R" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["R" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["R" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["R" + index].Style.Font.Bold = true;
                    worksheet.Cells["R" + index].Value = datas.Where(c => c.LaChuong != true).Sum(x => x.Tren60TMacBenhLaNu);


                    worksheet.Cells["S" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells["S" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["S" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["S" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["S" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["S" + index].Style.Font.Bold = true;
                    worksheet.Cells["S" + index].Value = datas.Where(c => c.LaChuong != true).Sum(x => x.Tren60TTuVongTS);

                    worksheet.Cells["T" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells["T" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["T" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["T" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["T" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["T" + index].Style.Font.Bold = true;
                    worksheet.Cells["T" + index].Value = datas.Where(c => c.LaChuong != true).Sum(x => x.Tren60TTuVongLaNu);


                    xlPackage.Save();
                }
                return stream.ToArray();

            }
        }

        private List<DanhSachTinhHinhBenhTatTuVong> DataTinhHinhBenhTatTuVongDummy()
        {
            var danhSachTinhHinhBenhTatTuVong = new List<DanhSachTinhHinhBenhTatTuVong>();
            for (int i = 0; i < 100; i++)
            {
                danhSachTinhHinhBenhTatTuVong.Add(new DanhSachTinhHinhBenhTatTuVong
                {
                    Id = 1,
                    STT = "C01",
                    TenNhomBenh = "Chương I: Bệnh nhiễm khuẩn và kí sinh vật - Chapter I: Certain infectious and parasistic diseases.",
                    MaICD = "A00-B99",
                    HDKBSoLanKhamChung = 1,
                    HDKBSoLanKhamTreEm = 10,
                    HDKBSoTuVong = 2,
                    TSBNSoTuVong = 1,
                    TSBNSoMacBenh = 2,
                    TSBNNgayDieuTri = 3,
                    TEMacTS = 2,
                    TEMacDuoi6T = 2,
                    TESoTuVongTS = 3,
                    TESoTuVongDuoi6T = 3,
                    TENgayDieuTriTS = 6,
                    TENgayDieuTriDuoi6T = 6,
                    Tren60TMacBenhTS = 4,
                    Tren60TMacBenhLaNu = 4,
                    Tren60TTuVongTS = 7,
                    Tren60TTuVongLaNu = 7,
                    LaChuong = i % 2 == 0 ? true : false
                });
            }
            return danhSachTinhHinhBenhTatTuVong;
        }
    }
}
