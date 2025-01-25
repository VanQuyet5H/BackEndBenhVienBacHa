using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.BHYT;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.ExcelChungTu;
using Camino.Core.Domain.ValueObject.GiayChungNhanNghiDuongThai;
using Camino.Core.Domain.ValueObject.GiayChungNhanNghiViecHuongBHXH;
using Camino.Core.Domain.ValueObject.GiayChungSinhMangThaiHo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.NoiTruBenhAn;
using Camino.Core.Helpers;
using Camino.Services.ExportImport.Help;
using Camino.Services.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.GoiBaoHiemYTe
{
    [ScopedDependency(ServiceType = typeof(IGoiBaoHiemYTeService))]
    public partial class GoiBaoHiemYTeService : MasterFileService<YeuCauTiepNhan>, IGoiBaoHiemYTeService
    {
        #region ĐẨY FILE EXCEL 79a 80a

        public List<ExcelFile7980A> GetThongExcelFile7980A(ExcelFile7980AQueryInfo queryInfo)
        {
            var hinhThucKCB = queryInfo.MauBaoBaoBHYT == MauBaoBaoBHYT.MauBaoCao80a ? EnumMaHoaHinhThucKCB.DieuTriNoiTru : EnumMaHoaHinhThucKCB.KhamBenh;
            var allYeuCauTiepNhanIds = _yeuCauTiepNhanDuLieuGuiCongBHYT.TableNoTracking
                .Where(o => (o.HinhThucKCB == null || o.HinhThucKCB == hinhThucKCB)
                            && (o.NgayRa == null || (queryInfo.TuNgay <= o.NgayRa && o.NgayRa < queryInfo.DenNgay)))
                .Select(o => o.YeuCauTiepNhanId)
                .Distinct().ToList();

            var allDuLieuGuiCongBHYTs = _yeuCauTiepNhanDuLieuGuiCongBHYT.TableNoTracking
                .Where(o => allYeuCauTiepNhanIds.Contains(o.YeuCauTiepNhanId))
                .Select(o => new { o.Id, o.YeuCauTiepNhanId, o.CoGuiCong, o.Version, o.HinhThucKCB, o.NgayRa })
                .ToList();

            var groupDuLieuGuiCongBHYTs = allDuLieuGuiCongBHYTs.GroupBy(o => o.YeuCauTiepNhanId);

            List<long> duLieuGuiCongBHYTIds = new List<long>();
            foreach (var g in groupDuLieuGuiCongBHYTs)
            {
                var last = g.Where(o => o.CoGuiCong == true).OrderBy(o => o.Version).ThenBy(o => o.Id).LastOrDefault();
                if (last != null && (last.HinhThucKCB == null || last.HinhThucKCB == hinhThucKCB)
                            && (last.NgayRa == null || (queryInfo.TuNgay <= last.NgayRa && last.NgayRa < queryInfo.DenNgay)))
                {
                    duLieuGuiCongBHYTIds.Add(last.Id);
                }
            }

            var duLieuGuiCongBHYTChiTiets = _yeuCauTiepNhanDuLieuGuiCongBHYT.TableNoTracking
                .Where(o => duLieuGuiCongBHYTIds.Contains(o.Id))
                .Select(o => new { o.Id, o.YeuCauTiepNhanId, o.DuLieu })
                .OrderBy(o => o.Id).ToList();

            var returnData = new List<ExcelFile7980A>();
            foreach (var duLieuGuiCongBHYTChiTiet in duLieuGuiCongBHYTChiTiets)
            {
                if (!string.IsNullOrEmpty(duLieuGuiCongBHYTChiTiet.DuLieu))
                {
                    var thongTinBenhNhanGoiBHYT = JsonConvert.DeserializeObject<ThongTinBenhNhan>(duLieuGuiCongBHYTChiTiet.DuLieu);
                    if (thongTinBenhNhanGoiBHYT != null && thongTinBenhNhanGoiBHYT.MaLoaiKCB == hinhThucKCB
                            && queryInfo.TuNgay <= thongTinBenhNhanGoiBHYT.NgayRa && thongTinBenhNhanGoiBHYT.NgayRa < queryInfo.DenNgay)
                    {
                        returnData.Add(BHYTHelper.MapThongTinBenhNhanToExcelFile7980A(thongTinBenhNhanGoiBHYT));
                    }
                }
            }

            return returnData;
        }

        //private ExcelFile7980A MapThongTinBenhNhanToExcelFile7980A(ThongTinBenhNhan thongTinBenhNhan)
        //{
        //    var excelFile7980A = new ExcelFile7980A();
        //    excelFile7980A.MaBN = thongTinBenhNhan.MaBenhNhan;
        //    excelFile7980A.HoTen = thongTinBenhNhan.HoTen;
        //    excelFile7980A.NgaySinhDisplay = ConvertNgayToXML(thongTinBenhNhan.NgaySinh);
        //    excelFile7980A.GioiTinh = thongTinBenhNhan.GioiTinh;
        //    excelFile7980A.DiaChi = thongTinBenhNhan.DiaChi;
        //    excelFile7980A.MaThe = thongTinBenhNhan.MaThe;
        //    excelFile7980A.MaDKBD = thongTinBenhNhan.MaCoSoKCBBanDau;
        //    excelFile7980A.GiaTriTheTu = ConvertNgayToXML(thongTinBenhNhan.GiaTriTheTu);
        //    excelFile7980A.GiaTriTheDen = ConvertNgayToXML(thongTinBenhNhan.GiaTriTheDen);
        //    excelFile7980A.MaBenh = thongTinBenhNhan.MaBenh;
        //    excelFile7980A.MaBenhKhac = thongTinBenhNhan.MaBenhKhac;
        //    excelFile7980A.MaLyDoVaoVien = (int)thongTinBenhNhan.LyDoVaoVien;
        //    excelFile7980A.MaNoiChuyen = thongTinBenhNhan.MaNoiChuyen;
        //    excelFile7980A.NgayVaoDisplay = ConvertNgayGioXMl(thongTinBenhNhan.NgayVao);
        //    excelFile7980A.NgayRaDisplay = ConvertNgayGioXMl(thongTinBenhNhan.NgayRa);
        //    excelFile7980A.SoNgayDieuTri = thongTinBenhNhan.SoNgayDieuTri;
        //    excelFile7980A.KetQuaDieuTri = (int)thongTinBenhNhan.KetQuaDieuTri;
        //    excelFile7980A.TinhTrangRaVien = (int)thongTinBenhNhan.TinhTrangRaVien;
        //    excelFile7980A.TienTongChi = thongTinBenhNhan.TienTongChi;

        //    excelFile7980A.TienXetNghiem = thongTinBenhNhan.HoSoChiTietDVKT?.Where(o => o.MaNhom == EnumDanhMucNhomTheoChiPhi.XetNghiem).Select(o => o.ThanhTien).DefaultIfEmpty().Sum() ?? 0;
        //    excelFile7980A.TienCDHA = thongTinBenhNhan.HoSoChiTietDVKT?.Where(o => o.MaNhom == EnumDanhMucNhomTheoChiPhi.ChuanDoanHinhAnh || o.MaNhom == EnumDanhMucNhomTheoChiPhi.ThamDoChucNang).Select(o => o.ThanhTien).DefaultIfEmpty().Sum() ?? 0;
        //    excelFile7980A.TienThuoc = thongTinBenhNhan.HoSoChiTietThuoc?.Select(o => o.ThanhTien).DefaultIfEmpty().Sum() ?? 0;
        //    excelFile7980A.TienMau = thongTinBenhNhan.HoSoChiTietDVKT?.Where(o => o.MaNhom == EnumDanhMucNhomTheoChiPhi.MauVaChePhamMau).Select(o => o.ThanhTien).DefaultIfEmpty().Sum() ?? 0;
        //    excelFile7980A.TienPTTT = thongTinBenhNhan.HoSoChiTietDVKT?.Where(o => o.MaNhom == EnumDanhMucNhomTheoChiPhi.ThuThuatPhauThuat || o.MaNhom == EnumDanhMucNhomTheoChiPhi.ThuThuat).Select(o => o.ThanhTien).DefaultIfEmpty().Sum() ?? 0;
        //    excelFile7980A.TienVTYT = thongTinBenhNhan.HoSoChiTietDVKT?.Where(o => o.MaNhom == EnumDanhMucNhomTheoChiPhi.VatTuYTeTrongDanhMucBHYT).Select(o => o.ThanhTien).DefaultIfEmpty().Sum() ?? 0;
        //    excelFile7980A.TienDVKTTyLe = 0;
        //    excelFile7980A.TienThuocTyLe = 0;
        //    excelFile7980A.TienVTYTTyLe = 0;
        //    excelFile7980A.TienKham = thongTinBenhNhan.HoSoChiTietDVKT?.Where(o => o.MaNhom == EnumDanhMucNhomTheoChiPhi.KhamBenh).Select(o => o.ThanhTien).DefaultIfEmpty().Sum() ?? 0;
        //    excelFile7980A.TienGiuong = thongTinBenhNhan.HoSoChiTietDVKT?.Where(o => o.MaNhom == EnumDanhMucNhomTheoChiPhi.GiuongDieuTriNoiTru || o.MaNhom == EnumDanhMucNhomTheoChiPhi.GiuongDieuTriNgoaiTru).Select(o => o.ThanhTien).DefaultIfEmpty().Sum() ?? 0;
        //    excelFile7980A.TienVanChuyen = thongTinBenhNhan.HoSoChiTietDVKT?.Where(o => o.MaNhom == EnumDanhMucNhomTheoChiPhi.VanChuyen).Select(o => o.ThanhTien).DefaultIfEmpty().Sum() ?? 0;
        //    excelFile7980A.TienBNTuTra = thongTinBenhNhan.TienTongChi - thongTinBenhNhan.TienBaoHiemThanhToan;
        //    excelFile7980A.TienBHTuTra = thongTinBenhNhan.TienBaoHiemThanhToan;
        //    excelFile7980A.TienNgoaiDanhSach = thongTinBenhNhan.TienNgoaiDinhSuat.GetValueOrDefault();
        //    excelFile7980A.MaKhoa = thongTinBenhNhan.MaKhoa;
        //    excelFile7980A.NamQuyetToan = thongTinBenhNhan.NamQuyetToan;
        //    excelFile7980A.ThangQuyetToan = thongTinBenhNhan.ThangQuyetToan;
        //    excelFile7980A.MaKhuVuc = thongTinBenhNhan.MaKhuVuc;
        //    excelFile7980A.MaLoaiKCB = (int)thongTinBenhNhan.MaLoaiKCB;
        //    excelFile7980A.MaCSKCB = thongTinBenhNhan.MaCSKCB;
        //    excelFile7980A.TienNguonKhac = thongTinBenhNhan.TienNguonKhac.GetValueOrDefault();

        //    return excelFile7980A;
        //}

        public virtual byte[] ExportDanhSachDayFile7980(ExcelFile7980AQueryInfo queryInfo)
        {
            var datas = GetThongExcelFile7980A(queryInfo);

            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<ExcelFile7980A>("STT", p => ind++)
            };
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("[Đẩy cổng BHYT] báo cáo chi tiết 79a,80a (đẩy cổng");
                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 30;
                    worksheet.Column(3).Width = 20;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 30;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 30;
                    worksheet.Column(8).Width = 30;
                    worksheet.Column(9).Width = 30;
                    worksheet.Column(10).Width = 35;
                    worksheet.Column(11).Width = 15;
                    worksheet.Column(12).Width = 40;
                    worksheet.Column(13).Width = 30;
                    worksheet.Column(14).Width = 40;
                    worksheet.Column(15).Width = 50;
                    worksheet.Column(16).Width = 50;
                    worksheet.Column(17).Width = 25;
                    worksheet.Column(18).Width = 20;
                    worksheet.Column(19).Width = 20;
                    worksheet.Column(20).Width = 20;
                    worksheet.Column(21).Width = 20;
                    worksheet.Column(22).Width = 20;
                    worksheet.Column(23).Width = 20;
                    worksheet.Column(24).Width = 20;
                    worksheet.Column(25).Width = 20;
                    worksheet.Column(26).Width = 20;
                    worksheet.Column(27).Width = 50;
                    worksheet.Column(28).Width = 25;
                    worksheet.Column(29).Width = 20;
                    worksheet.Column(30).Width = 20;
                    worksheet.Column(31).Width = 20;
                    worksheet.Column(32).Width = 20;
                    worksheet.Column(33).Width = 20;
                    worksheet.Column(34).Width = 20;
                    worksheet.Column(35).Width = 20;
                    worksheet.Column(36).Width = 20;
                    worksheet.Column(37).Width = 20;
                    worksheet.Column(38).Width = 20;
                    worksheet.Column(39).Width = 20;
                    worksheet.Column(40).Width = 20;
                    worksheet.Column(41).Width = 20;
                    worksheet.Column(42).Width = 20;
                    worksheet.DefaultColWidth = 7;

                    using (var range = worksheet.Cells["A1:AP1"])
                    {
                        range.Worksheet.Cells["A1:AP1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A1:AP1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:AP1"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A1:AP1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:AP1"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A1:AP1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A1:A1"].Merge = true;
                        range.Worksheet.Cells["A1:A1"].Value = "STT";
                        range.Worksheet.Cells["A1:A1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A1:A1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["B1:B1"].Merge = true;
                        range.Worksheet.Cells["B1:B1"].Value = "MA_BN";
                        range.Worksheet.Cells["B1:B1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["B1:B1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["C1:C1"].Merge = true;
                        range.Worksheet.Cells["C1:C1"].Value = "HO_TEN";
                        range.Worksheet.Cells["C1:C1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["C1:C1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["D1:D1"].Merge = true;
                        range.Worksheet.Cells["D1:D1"].Value = "NGAY_SINH";
                        range.Worksheet.Cells["D1:D1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["D1:D1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["E1:E1"].Merge = true;
                        range.Worksheet.Cells["E1:E1"].Value = "GIOI_TINH";
                        range.Worksheet.Cells["E1:E1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["E1:E1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["F1:F1"].Merge = true;
                        range.Worksheet.Cells["F1:F1"].Value = "DIA_CHI";
                        range.Worksheet.Cells["F1:F1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["F1:F1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["G1:G1"].Merge = true;
                        range.Worksheet.Cells["G1:G1"].Value = "MA_THE";
                        range.Worksheet.Cells["G1:G1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["G1:G1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["H1:H1"].Merge = true;
                        range.Worksheet.Cells["H1:H1"].Value = "MA_DKBD";
                        range.Worksheet.Cells["H1:H1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["H1:H1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["I1:I1"].Merge = true;
                        range.Worksheet.Cells["I1:I1"].Value = "GT_THE_TU";
                        range.Worksheet.Cells["I1:I1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["I1:I1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["J1:J1"].Merge = true;
                        range.Worksheet.Cells["J1:J1"].Value = "GT_THE_DEN";
                        range.Worksheet.Cells["J1:J1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["J1:J1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["K1:K1"].Merge = true;
                        range.Worksheet.Cells["K1:K1"].Value = "MA_BENH";
                        range.Worksheet.Cells["K1:K1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["K1:K1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["L1:L1"].Merge = true;
                        range.Worksheet.Cells["L1:L1"].Value = "MA_BENHKHAC";
                        range.Worksheet.Cells["L1:L1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["L1:L1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["M1:M1"].Merge = true;
                        range.Worksheet.Cells["M1:M1"].Value = "MA_LYDO_VVIEN";
                        range.Worksheet.Cells["M1:M1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["M1:M1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["N1:N1"].Merge = true;
                        range.Worksheet.Cells["N1:N1"].Value = "MA_NOI_CHUYEN";
                        range.Worksheet.Cells["N1:N1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["N1:N1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["O1:O1"].Merge = true;
                        range.Worksheet.Cells["O1:O1"].Value = "NGAY_VAO";
                        range.Worksheet.Cells["O1:O1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["O1:O1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["P1:P1"].Merge = true;
                        range.Worksheet.Cells["P1:P1"].Value = "NGAY_RA";
                        range.Worksheet.Cells["P1:P1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["P1:P1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["Q1:Q1"].Merge = true;
                        range.Worksheet.Cells["Q1:Q1"].Value = "SO_NGAY_DTRI";
                        range.Worksheet.Cells["Q1:Q1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["Q1:Q1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["R1:R1"].Merge = true;
                        range.Worksheet.Cells["R1:R1"].Value = "KET_QUA_DTRI";
                        range.Worksheet.Cells["R1:R1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["R1:R1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["S1:S1"].Merge = true;
                        range.Worksheet.Cells["S1:S1"].Value = "TINH_TRANG_RV";
                        range.Worksheet.Cells["S1:S1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["S1:S1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["T1:T1"].Merge = true;
                        range.Worksheet.Cells["T1:T1"].Value = "T_TONGCHI";
                        range.Worksheet.Cells["T1:T1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["T1:T1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["U1:U1"].Merge = true;
                        range.Worksheet.Cells["U1:U1"].Value = "T_XN";
                        range.Worksheet.Cells["U1:U1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["U1:U1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["V1:V1"].Merge = true;
                        range.Worksheet.Cells["V1:V1"].Value = "T_CDHA";
                        range.Worksheet.Cells["V1:V1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["V1:V1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["W1:W1"].Merge = true;
                        range.Worksheet.Cells["W1:W1"].Value = "T_THUOC";
                        range.Worksheet.Cells["W1:W1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["W1:W1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["X1:X1"].Merge = true;
                        range.Worksheet.Cells["X1:X1"].Value = "T_MAU";
                        range.Worksheet.Cells["X1:X1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["X1:X1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["Y1:Y1"].Merge = true;
                        range.Worksheet.Cells["Y1:Y1"].Value = "T_PTTT";
                        range.Worksheet.Cells["Y1:Y1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["Y1:Y1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["Z1:Z1"].Merge = true;
                        range.Worksheet.Cells["Z1:Z1"].Value = "T_VTYT";
                        range.Worksheet.Cells["Z1:Z1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["Z1:Z1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["AA1:AA1"].Merge = true;
                        range.Worksheet.Cells["AA1:AA1"].Value = "T_DVKT_TYLE";
                        range.Worksheet.Cells["AA1:AA1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["AA1:AA1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["AB1:AB1"].Merge = true;
                        range.Worksheet.Cells["AB1:AB1"].Value = "T_THUOC_TYLE";
                        range.Worksheet.Cells["AB1:AB1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["AB1:AB1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["AC1:AC1"].Merge = true;
                        range.Worksheet.Cells["AC1:AC1"].Value = "T_VTYT_TYLE";
                        range.Worksheet.Cells["AC1:AC1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["AC1:AC1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["AD1:AD1"].Merge = true;
                        range.Worksheet.Cells["AD1:AD1"].Value = "T_KHAM";
                        range.Worksheet.Cells["AD1:AD1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["AD1:AD1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["AE1:AE1"].Merge = true;
                        range.Worksheet.Cells["AE1:AE1"].Value = "T_GIUONG";
                        range.Worksheet.Cells["AE1:AE1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["AE1:AE1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["AF1:AF1"].Merge = true;
                        range.Worksheet.Cells["AF1:AF1"].Value = "T_VCHUYEN";
                        range.Worksheet.Cells["AF1:AF1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["AF1:AF1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["AG1:AG1"].Merge = true;
                        range.Worksheet.Cells["AG1:AG1"].Value = "T_BNTT";
                        range.Worksheet.Cells["AG1:AG1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["AG1:AG1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["AH1:AH1"].Merge = true;
                        range.Worksheet.Cells["AH1:AH1"].Value = "T_BHTT";
                        range.Worksheet.Cells["AH1:AH1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["AH1:AH1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["AI1:AI1"].Merge = true;
                        range.Worksheet.Cells["AI1:AI1"].Value = "T_NGOAIDS";
                        range.Worksheet.Cells["AI1:AI1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["AI1:AI1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["AJ1:AJ1"].Merge = true;
                        range.Worksheet.Cells["AJ1:AJ1"].Value = "MA_KHOA";
                        range.Worksheet.Cells["AJ1:AJ1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["AJ1:AJ1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["AK1:AK1"].Value = "NAM_QT";
                        range.Worksheet.Cells["AK1:AK1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["AK1:AK1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["AL1:AL1"].Value = "THANG_QT";
                        range.Worksheet.Cells["AL1:AL1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["AL1:AL1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["AM1:AM1"].Value = "MA_KHUVUC";
                        range.Worksheet.Cells["AM1:AM1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["AM1:AM1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["AN1:AN1"].Value = "MA_LOAIKCB";
                        range.Worksheet.Cells["AN1:AN1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["AN1:AN1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["AO1:AO1"].Value = "MA_CSKCB";
                        range.Worksheet.Cells["AO1:AO1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["AO1:AO1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["AP1:AP1"].Value = "T_NGUONKHAC";
                        range.Worksheet.Cells["AP1:AP1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["AP1:AP1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }

                    var manager = new PropertyManager<ExcelFile7980A>(requestProperties);
                    int index = 2;

                    var stt = 1;
                    if (datas.Any())
                    {
                        var fromIndex = index;
                        
                        foreach (var item in datas)
                        {
                            using (var range = worksheet.Cells["A" + index + ":AP1" + index])
                            {
                                //range.Worksheet.Cells["A" + index + ":AP1" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                //range.Worksheet.Cells["A" + index + ":AP1" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                //range.Worksheet.Cells["A" + index + ":AP1" + index].Style.Font.Color.SetColor(Color.Black);

                                worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["A" + index].Value = stt;

                                worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["B" + index].Value = item.MaBN;

                                worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["C" + index].Value = item.HoTen;

                                worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["D" + index].Value = item.NgaySinhDisplay;

                                worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["E" + index].Value = (int)item.GioiTinh;

                                worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["F" + index].Value = item.DiaChi;

                                worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["G" + index].Value = item.MaThe;

                                worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["H" + index].Value = item.MaDKBD;

                                worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["I" + index].Value = item.GiaTriTheTu;

                                worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["J" + index].Value = item.GiaTriTheDen;

                                worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["K" + index].Value = item.MaBenh;

                                worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["L" + index].Value = item.MaBenhKhac;

                                worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["M" + index].Value = item.MaLyDoVaoVien;

                                worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["N" + index].Value = item.MaNoiChuyen;

                                worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["O" + index].Value = item.NgayVaoDisplay;

                                worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["P" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["P" + index].Value = item.NgayRaDisplay;

                                worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["Q" + index].Value = item.SoNgayDieuTri;

                                worksheet.Cells["R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["R" + index].Value = item.KetQuaDieuTri;

                                worksheet.Cells["S" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["S" + index].Value = item.TinhTrangRaVien;

                                worksheet.Cells["T" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["T" + index].Value = item.TienTongChi;

                                worksheet.Cells["U" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["U" + index].Value = item.TienXetNghiem;

                                worksheet.Cells["V" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["V" + index].Value = item.TienCDHA;

                                worksheet.Cells["W" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["W" + index].Value = item.TienThuoc;

                                worksheet.Cells["X" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["X" + index].Value = item.TienMau;

                                worksheet.Cells["Y" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["Y" + index].Value = item.TienPTTT;

                                worksheet.Cells["Z" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["Z" + index].Value = item.TienVTYT;

                                worksheet.Cells["AA" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["AA" + index].Value = item.TienDVKTTyLe;

                                worksheet.Cells["AB" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["AB" + index].Value = item.TienThuocTyLe;

                                worksheet.Cells["AC" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["AC" + index].Value = item.TienVTYTTyLe;


                                worksheet.Cells["AD" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["AD" + index].Value = item.TienKham;

                                worksheet.Cells["AE" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["AE" + index].Value = item.TienGiuong;

                                worksheet.Cells["AF" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["AF" + index].Value = item.TienVanChuyen;

                                worksheet.Cells["AG" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["AG" + index].Value = item.TienBNTuTra;

                                worksheet.Cells["AH" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["AH" + index].Value = item.TienBHTuTra;

                                worksheet.Cells["AI" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["AI" + index].Value = item.TienNgoaiDanhSach;


                                worksheet.Cells["AJ" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["AJ" + index].Value = item.MaKhoa;

                                worksheet.Cells["AK" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["AK" + index].Value = item.NamQuyetToan;

                                worksheet.Cells["AL" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["AL" + index].Value = item.ThangQuyetToan;

                                worksheet.Cells["AM" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["AM" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["AM" + index].Value = item.MaKhuVuc;

                                worksheet.Cells["AN" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["AN" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["AN" + index].Value = item.MaLoaiKCB;

                                worksheet.Cells["AO" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["AO" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["AO" + index].Value = item.MaCSKCB;

                                worksheet.Cells["AP" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["AP" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["AP" + index].Value = item.TienNguonKhac;

                                index++;
                                stt++;
                            }

                        }

                        var toIndex = index;
                        using (var range = worksheet.Cells[$"A{fromIndex}:AP{toIndex}"])
                        {
                            range.Worksheet.Cells["A" + fromIndex + ":AP1" + toIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                            range.Worksheet.Cells["A" + fromIndex + ":AP1" + toIndex].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                            range.Worksheet.Cells["A" + fromIndex + ":AP1" + toIndex].Style.Font.Color.SetColor(Color.Black);
                        }
                    }

                    xlPackage.Save();
                }
                return stream.ToArray();

            }
        }

        public async Task<GridDataSource> GetDataDanhSach79AForGrid(ExcelFile7980AQueryInfo queryInfo)
        {
            var getDatas = GetThongExcelFile7980A(queryInfo);
            var datas = getDatas.Skip(queryInfo.Skip).Take(queryInfo.Take);
            return new GridDataSource { Data = datas.ToArray(), TotalRowCount = getDatas.Count() };
        }

        #endregion
    }
}
