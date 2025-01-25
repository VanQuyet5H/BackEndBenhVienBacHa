using Camino.Core.Domain.ValueObject.BHYT;
using Camino.Core.Domain.ValueObject.ExcelChungTu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.Helpers
{
    public static class BHYTHelper
    {
        private const string FormatNgayBHYT = "yyyyMMdd";
        private const string FormatNgayGioBHYT = "yyyyMMddHHmm";
        public static ExcelFile7980A MapThongTinBenhNhanToExcelFile7980A(ThongTinBenhNhan thongTinBenhNhan)
        {
            var excelFile7980A = new ExcelFile7980A();
            excelFile7980A.MaBN = thongTinBenhNhan.MaBenhNhan;
            excelFile7980A.HoTen = thongTinBenhNhan.HoTen;
            excelFile7980A.NgaySinhDisplay = ConvertNgayToXML(thongTinBenhNhan.NgaySinh);
            excelFile7980A.GioiTinh = thongTinBenhNhan.GioiTinh;
            excelFile7980A.DiaChi = thongTinBenhNhan.DiaChi;
            excelFile7980A.MaThe = thongTinBenhNhan.MaThe;
            excelFile7980A.MaDKBD = thongTinBenhNhan.MaCoSoKCBBanDau;
            excelFile7980A.GiaTriTheTu = ConvertNgayToXML(thongTinBenhNhan.GiaTriTheTu);
            excelFile7980A.GiaTriTheDen = ConvertNgayToXML(thongTinBenhNhan.GiaTriTheDen);
            excelFile7980A.MaBenh = thongTinBenhNhan.MaBenh;
            excelFile7980A.MaBenhKhac = thongTinBenhNhan.MaBenhKhac;
            excelFile7980A.MaLyDoVaoVien = (int)thongTinBenhNhan.LyDoVaoVien;
            excelFile7980A.MaNoiChuyen = thongTinBenhNhan.MaNoiChuyen;
            excelFile7980A.NgayVaoDisplay = ConvertNgayGioXMl(thongTinBenhNhan.NgayVao);
            excelFile7980A.NgayRaDisplay = ConvertNgayGioXMl(thongTinBenhNhan.NgayRa);
            excelFile7980A.SoNgayDieuTri = thongTinBenhNhan.SoNgayDieuTri;
            excelFile7980A.KetQuaDieuTri = (int)thongTinBenhNhan.KetQuaDieuTri;
            excelFile7980A.TinhTrangRaVien = (int)thongTinBenhNhan.TinhTrangRaVien;
            excelFile7980A.TienTongChi = thongTinBenhNhan.TienTongChi;

            excelFile7980A.TienXetNghiem = thongTinBenhNhan.HoSoChiTietDVKT?.Where(o => o.MaNhom == EnumDanhMucNhomTheoChiPhi.XetNghiem).Select(o => o.ThanhTien).DefaultIfEmpty().Sum() ?? 0;
            excelFile7980A.TienCDHA = thongTinBenhNhan.HoSoChiTietDVKT?.Where(o => o.MaNhom == EnumDanhMucNhomTheoChiPhi.ChuanDoanHinhAnh || o.MaNhom == EnumDanhMucNhomTheoChiPhi.ThamDoChucNang).Select(o => o.ThanhTien).DefaultIfEmpty().Sum() ?? 0;
            excelFile7980A.TienThuoc = thongTinBenhNhan.HoSoChiTietThuoc?.Select(o => o.ThanhTien).DefaultIfEmpty().Sum() ?? 0;
            excelFile7980A.TienMau = thongTinBenhNhan.HoSoChiTietDVKT?.Where(o => o.MaNhom == EnumDanhMucNhomTheoChiPhi.MauVaChePhamMau).Select(o => o.ThanhTien).DefaultIfEmpty().Sum() ?? 0;
            excelFile7980A.TienPTTT = thongTinBenhNhan.HoSoChiTietDVKT?.Where(o => o.MaNhom == EnumDanhMucNhomTheoChiPhi.ThuThuatPhauThuat || o.MaNhom == EnumDanhMucNhomTheoChiPhi.ThuThuat).Select(o => o.ThanhTien).DefaultIfEmpty().Sum() ?? 0;
            excelFile7980A.TienVTYT = thongTinBenhNhan.HoSoChiTietDVKT?.Where(o => o.MaNhom == EnumDanhMucNhomTheoChiPhi.VatTuYTeTrongDanhMucBHYT).Select(o => o.ThanhTien).DefaultIfEmpty().Sum() ?? 0;
            excelFile7980A.TienDVKTTyLe = 0;
            excelFile7980A.TienThuocTyLe = 0;
            excelFile7980A.TienVTYTTyLe = 0;
            excelFile7980A.TienKham = thongTinBenhNhan.HoSoChiTietDVKT?.Where(o => o.MaNhom == EnumDanhMucNhomTheoChiPhi.KhamBenh).Select(o => o.ThanhTien).DefaultIfEmpty().Sum() ?? 0;
            excelFile7980A.TienGiuong = thongTinBenhNhan.HoSoChiTietDVKT?.Where(o => o.MaNhom == EnumDanhMucNhomTheoChiPhi.GiuongDieuTriNoiTru || o.MaNhom == EnumDanhMucNhomTheoChiPhi.GiuongDieuTriNgoaiTru).Select(o => o.ThanhTien).DefaultIfEmpty().Sum() ?? 0;
            excelFile7980A.TienVanChuyen = thongTinBenhNhan.HoSoChiTietDVKT?.Where(o => o.MaNhom == EnumDanhMucNhomTheoChiPhi.VanChuyen).Select(o => o.ThanhTien).DefaultIfEmpty().Sum() ?? 0;
            excelFile7980A.TienBNTuTra = thongTinBenhNhan.TienTongChi - thongTinBenhNhan.TienBaoHiemThanhToan;
            excelFile7980A.TienBHTuTra = thongTinBenhNhan.TienBaoHiemThanhToan;
            excelFile7980A.TienNgoaiDanhSach = thongTinBenhNhan.TienNgoaiDinhSuat.GetValueOrDefault();
            excelFile7980A.MaKhoa = thongTinBenhNhan.MaKhoa;
            excelFile7980A.NamQuyetToan = thongTinBenhNhan.NamQuyetToan;
            excelFile7980A.ThangQuyetToan = thongTinBenhNhan.ThangQuyetToan;
            excelFile7980A.MaKhuVuc = thongTinBenhNhan.MaKhuVuc;
            excelFile7980A.MaLoaiKCB = (int)thongTinBenhNhan.MaLoaiKCB;
            excelFile7980A.MaCSKCB = thongTinBenhNhan.MaCSKCB;
            excelFile7980A.TienNguonKhac = thongTinBenhNhan.TienNguonKhac.GetValueOrDefault();

            return excelFile7980A;
        }

        public static string ConvertNgayToXML(DateTime? dtTime)
        {
            if (!dtTime.HasValue)
                return "";
            return dtTime.Value.ToString(FormatNgayBHYT);
        }

        public static string ConvertNgayGioXMl(DateTime? dtTime)
        {
            if (!dtTime.HasValue)
                return "";
            return dtTime.Value.ToString(FormatNgayGioBHYT);
        }
    }
}
