using System;
using System.Collections.Generic;
using System.Text;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class GiayRaVienExportExcel 
    {
        [Width(30)]
        public string SoLuuTruGiayRaVien { get; set; }
        [Width(30)]
        public string MaCSKCB { get; set; }
        [Width(30)]
        public string SoSeRi { get; set; }
        [Width(30)]
        public string MaKhoa { get; set; }
        [Width(30)]
        public string MaSoBHXH { get; set; }
        [Width(30)]
        public string MaThe { get; set; }
        [Width(30)]
        public string HoTen { get; set; }
        [Width(50)]
        public string DiaChi { get; set; }
        [Width(30)]
        public string NgaySinh { get; set; }
        [Width(30)]
        public int DanToc { get; set; }
        [Width(30)]
        public int GioiTinh { get; set; }
        [Width(30)]
        public string NgheNghiep { get; set; }
        [Width(30)]
        public DateTime NgayVao { get; set; }
        [Width(30)]
        public string NgayRa { get; set; }
        [Width(30)]
        public double? TuoiThai { get; set; }
        [Width(50)]
        public string ChanDoan { get; set; }
        [Width(30)]
        public string PhuongPhapDieuTri { get; set; }
        [Width(30)]
        public string GhiChu { get; set; }
        [Width(30)]
        public string NguoiDaiDien { get; set; }
        [Width(30)]
        public string TruongKhoa { get; set; }
        [Width(30)]
        public string MaTruongKhoa { get; set; }
        [Width(30)]
        public string NgayCapCT { get; set; }
        [Width(30)]
        public string TenTruongKhoa { get; set; }
        [Width(30)]
        public string HoTenCha { get; set; }
        [Width(30)]
        public string HoTenMe { get; set; }
        [Width(30)]
        public string TEKT { get; set; }
        //Đối với trẻ em dưới 7 tuổi không có thẻ, điền số 1 
        //Để trống MA_SOBHXH, MA_THE.
        //Nhập HO_TEN_CHA hoặc HO_TEN_ME
    }

    public class GiayChungSinhExportExcel
    {
        [Width(30)]
        public string MaCT { get; set; }
        [Width(30)]
        public string MaCSKCB { get; set; }
        [Width(30)]
        public string MaThe { get; set; }
        [Width(30)]
        public string SoSeRi { get; set; }
        [Width(30)]
        public string MaSoBHXHMe { get; set; }
        [Width(30)]
        public string HoTenMe { get; set; }
        [Width(30)]
        public string NgaySinh { get; set; }
        [Width(30)]
        public string DiaChi { get; set; }
        [Width(30)]
        public string CMND { get; set; }
        [Width(30)]
        public DateTime NgayCapCMND { get; set; }
        [Width(30)]
        public string NoiCapCMND { get; set; }
        [Width(30)]
        public int DanToc { get; set; }
        [Width(30)]
        public string HoTenCha { get; set; }
        [Width(30)]
        public DateTime NgaySinhCon { get; set; }
        [Width(30)]
        public string NoiSinhCon { get; set; }
        [Width(30)]
        public string TenCon { get; set; }
        [Width(30)]
        public int SoCon { get; set; }
        [Width(30)]
        public int GioiTinhCon { get; set; }
        [Width(30)]
        public double CanNangCon { get; set; }
        [Width(30)]
        public string TinhTrangCon { get; set; }
        [Width(30)]
        public string GhiChu { get; set; }
        [Width(30)]
        public string NguoiDoDe { get; set; }
        [Width(30)]
        public string NguoiGhiPhieu { get; set; }
        [Width(30)]
        public string NguoiDaiDien { get; set; }
        [Width(30)]
        public string NgayTaoChungTu { get; set; }
        [Width(30)]
        public int SinhConPhauThuat { get; set; }
        [Width(30)]
        public int SinhConDuoi32Tuan { get; set; }
        [Width(30)]
        public string So { get; set; }
        [Width(30)]
        public string QuyenSo { get; set; }
    }

    public class GiayTomTatBenhAnExportExcel
    {
        [Width(30)]
        public string MaCT { get; set; }
        [Width(30)]
        public string MaCSKCB { get; set; }
        [Width(30)]
        public string SoSeRi { get; set; }
        [Width(30)]
        public string MaSoBHXH { get; set; }
        [Width(30)]
        public string MaThe { get; set; }

        [Width(30)]
        public string HoTen { get; set; }
        [Width(30)]
        public string NgaySinh { get; set; }
        [Width(30)]
        public int GioiTinh { get; set; }
        [Width(30)]
        public int? DanToc { get; set; }
        [Width(30)]
        public string DiaChi { get; set; }
        [Width(30)]
        public string NgheNghiep { get; set; }

        [Width(30)]
        public string HoTenCha { get; set; }
        [Width(30)]
        public string HoTenMe { get; set; }
        [Width(30)]
        public string NguoiGiamHo { get; set; }
        [Width(30)]
        public string TenDonVi { get; set; }
        [Width(30)]
        public string NgayVao { get; set; }
        [Width(30)]
        public string NgayRa { get; set; }

        [Width(30)]
        public string ChanDoanLucVaoVien { get; set; }
        [Width(30)]
        public string ChanDoanLucRaVien { get; set; }
        [Width(30)]
        public string QuaTrinhBenhLyVaDienBienLamSang { get; set; }
        [Width(30)]
        public string TomTatKetQuaXetNghiemCLS { get; set; }
        [Width(30)]
        public string PhuongPhapDieuTri { get; set; }

        [Width(30)]
        public string NgaySinhCon { get; set; }
        [Width(30)]
        public string NgayChetCon { get; set; }
        [Width(30)]
        public string SoConChet { get; set; }

        [Width(30)]
        public EnumTinhTrangRaVien? TinhTrangRaVien { get; set; }
        [Width(30)]
        public string GhiChu { get; set; }
        [Width(30)]
        public string NguoiDaiDien { get; set; }
        [Width(30)]
        public string NgayCapChungTu { get; set; }
        [Width(30)]
        public string TAKT { get; set; }
    }

    public class GiayNghiHuongBHXHExportExcel
    {
        [Width(30)]
        public string MaCT { get; set; }
        [Width(30)]
        public string SOKCB { get; set; }
        [Width(30)]
        public string MaBV { get; set; }
        [Width(30)]
        public string MaBS { get; set; }
        [Width(30)]
        public string MaSoBHXH { get; set; }
        [Width(30)]
        public string MaThe { get; set; }

        [Width(30)]
        public string HoTen { get; set; }
        [Width(30)]
        public string NgaySinh { get; set; }
        [Width(30)]
        public int GioiTinh { get; set; }
        [Width(30)]
        public string PhuongPhapDieuTri { get; set; }
        [Width(30)]
        public string MaDonVi { get; set; }
        [Width(30)]
        public string TenDonVi { get; set; }

        [Width(30)]
        public string TuNgay { get; set; }
        [Width(30)]
        public string DenNgay { get; set; }
        [Width(30)]
        public int SoNgay { get; set; }
        [Width(30)]
        public string HoTenCha { get; set; }
        [Width(30)]
        public string HoTenMe { get; set; }
        [Width(30)]
        public string NgayCT { get; set; }

        [Width(30)]
        public string NguoiDaiDien { get; set; }
        [Width(30)]
        public string TenBS { get; set; }
        [Width(30)]
        public string SeRi { get; set; }
        [Width(30)]
        public string MauSo { get; set; }
    }

}

