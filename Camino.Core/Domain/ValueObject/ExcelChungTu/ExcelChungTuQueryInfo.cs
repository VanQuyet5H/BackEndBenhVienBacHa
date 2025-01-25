using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.ExcelChungTu
{
    public class ExcelChungTuQueryInfo
    {
        public ExcelChungTuQueryInfo()
        {
            ThongTinYeuCauTiepNhans = new List<ThongTinYeuCauTiepNhan>();

            YeuCauTiepNhanIds = new List<long>();
            MaYeuCauTiepNhans = new List<string>();
        }

        public LoaiChungTuXuatExcel LoaiChungTu { get; set; }
        public List<ThongTinYeuCauTiepNhan> ThongTinYeuCauTiepNhans { get; set; }

        public List<long> YeuCauTiepNhanIds { get; set; }
        public List<string> MaYeuCauTiepNhans { get; set; }
    }


    public class ThongTinYeuCauTiepNhan
    {
        public long? YeuCauTiepNhanNoiTruId { get; set; }
        public long? YeuCauTiepNhanNgoaiTruId { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
    }


    #region giấy ra viện

    public class GiayRaVienVoData
    {
        public string SoLuuTruGiayRaVien { get; set; }
        public string MaCSKCB { get; set; }
        //public string MaKhoa { get; set; }

        //public string MaThe { get; set; }
        public string MaSoBHXH { get; set; }

        public string HoTen { get; set; }
        //public string DiaChi { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        //public string NgaySinh { get; set; }

        public string MaYTe { get; set; }
        public string MaDanToc { get; set; }
        //public int DanToc { get; set; }
        public Enums.LoaiGioiTinh? LoaiGioiTinh { get; set; }

        public string GiayRaVienJson { get; set; }
        public string ThongTinBenhAn { get; set; }
        public string NgheNghiep { get; set; }

        public DateTime NgayVao { get; set; }
        public DateTime? NgayRa { get; set; }

        public double? TuoiThai { get; set; }

        //public string HoTenCha { get; set; }
        //public string HoTenMe { get; set; }
        //public string TEKT { get; set; }
        //public bool LaGiayRaVien { get; set; }
        public List<GiayRaVienTheBHYTData> GiayRaVienTheBHYTDatas { get; set; }
        public List<GiayRaVienKhoaPhongDieuTriData> GiayRaVienKhoaPhongDieuTriDatas { get; set; }
        public string NguoiLienHeHoTen { get; set; }
        public long? NguoiLienHeQuanHeNhanThanId { get; set; }

        public DateTime? NgayCapCT { get; set; }
        public long KhoaPhongNhapVienId { get; set; }
    }

    public class GiayRaVienKhoaPhongDieuTriData
    {
        public long KhoaPhongId { get; set; }
        public DateTime ThoiDiemVaoKhoa { get; set; }
    }

    public class GiayRaVienTheBHYTData
    {
        public string MaSoThe { get; set; }
        public string BHYTDiaChi { get; set; }
        public DateTime NgayHieuLuc { get; set; }
    }

    public class GiayRaVien
    {
        public string PhuongPhapDieuTri { get; set; }
        public string ChanDoan { get; set; }
        public string GhiChu { get; set; }
        public string GhiChuChuanDoanRaVien { get; set; }
        public long? IdGhiChu { get; set; }
        public long? TruongKhoaId { get; set; }
        public long? GiamDocChuyenMonId { get; set; }
    }

    public class GiayRaVienVo
    {
        public string SoLuuTruGiayRaVien { get; set; }
        public string MaCSKCB { get; set; }

        public string SoSeRi { get; set; } //Lấy thông tin mã y tế
        public string MaKhoa { get; set; } //Hiện tại lấy khoa chuyển sau cùng

        public string MaThe { get; set; }
        public string MaSoBHXH { get; set; }

        public string HoTen { get; set; }
        public string DiaChi { get; set; }
        public string NgaySinh { get; set; }
        public int DanToc { get; set; }
        public int GioiTinh { get; set; }
        public string NgheNghiep { get; set; }

        public string NgayVao { get; set; }
        public string NgayRa { get; set; }

        public double? TuoiThai { get; set; }
        public string ChanDoan { get; set; }
        public string PhuongPhapDieuTri { get; set; }
        public string GhiChu { get; set; }

        public string NguoiDaiDien { get; set; }
        public string MaTruongKhoa { get; set; }
        public string NgayCapCT { get; set; }
        public DateTime? NgayTaoChungTuDateTime { get; set; }
        public string TenTruongKhoa { get; set; }

        public string HoTenCha { get; set; }
        public string HoTenMe { get; set; }
        public string TEKT { get; set; }

        //Đối với trẻ em dưới 7 tuổi không có thẻ, điền số 1 
        //Để trống MA_SOBHXH, MA_THE.
        //Nhập HO_TEN_CHA hoặc HO_TEN_ME
    }

    #endregion giấy ra viện

    #region giấy chứng sinh

    public class GiayChungSinhVoData : GridItem
    {
        public string MaCT { get; set; }
        public string MaThe { get; set; }

        public string MaCSKCB { get; set; }
        public string MaSoBHXHMe { get; set; }
        public string HoTenMe { get; set; }
        public string NgaySinh { get; set; }
        public string DiaChi { get; set; }
        public string CMND { get; set; }
        public DateTime NgayCapCMND { get; set; }
        public string NoiCapCMND { get; set; }
        public int DanToc { get; set; }
        public string HoTenCha { get; set; }
        public DateTime NgaySinhCon { get; set; }
        public string NoiSinhCon { get; set; }
        public string TenCon { get; set; }
        public int SoCon { get; set; }
        public int GioiTinhCon { get; set; }

        public double CanNangCon { get; set; }
        public string TinhTrangCon { get; set; }
        public string GhiChu { get; set; }
        public string NguoiDoDe { get; set; }
        public string NguoiGhiPhieu { get; set; }
        public string NguoiDaiDien { get; set; }
        public string NgayTaoChungTu { get; set; }
        public DateTime? NgayTaoChungTuDateTime { get; set; }
        public int SinhConPhauThuat { get; set; }
        public int SinhConDuoi32Tuan { get; set; }

        public string So { get; set; }
        public string QuyenSo { get; set; }
    }

    public class GiayChungSinhVo : GridItem
    {
        public string MaCT { get; set; }
        public string MaCSKCB { get; set; }
        public string MaThe { get; set; }
        public string SoSeRi { get; set; }

        public string MaSoBHXHMe { get; set; }
        public string HoTenMe { get; set; }
        public string NgaySinh { get; set; }
        public string DiaChi { get; set; }
        public string CMND { get; set; }
        public string NgayCapCMND { get; set; }
        public string NoiCapCMND { get; set; }
        public int DanToc { get; set; }
        public string HoTenCha { get; set; }
        public string NgaySinhCon { get; set; }
        public string NoiSinhCon { get; set; }

        public string TenCon { get; set; }
        public int SoCon { get; set; }
        public int GioiTinhCon { get; set; }
        public double? CanNangCon { get; set; }
        public string TinhTrangCon { get; set; }
        public string GhiChu { get; set; }
        public string NguoiDoDe { get; set; }
        public string NguoiGhiPhieu { get; set; }
        public string NguoiDaiDien { get; set; }
        public string NgayTaoChungTu { get; set; }
        public DateTime? NgayTaoChungTuDateTime { get; set; }
        public int SinhConPhauThuat { get; set; }
        public int SinhConDuoi32Tuan { get; set; }
        public string So { get; set; }
        public string QuyenSo { get; set; }
    }

    #endregion  giấy chứng sinh

    #region Giấy tóm tắt bệnh án


    public class ThongTinTreSoSinhVo
    {
        public ThongTinTreSoSinhVo()
        {
            DacDiemTreSoSinhs = new List<DacDiemTreSoSinh>();
        }

        public List<DacDiemTreSoSinh> DacDiemTreSoSinhs { get; set; }
    }

    public class DacDiemTreSoSinh
    {
        public long? YeuCauTiepNhanConId { get; set; }
        public DateTime? DeLuc { get; set; }
        public EnumTrangThaiSong? TinhTrangId { get; set; }
    }

    public class GiayTomTatBenhAn
    {
        public string MaCT { get; set; }
        public string MaCSKCB { get; set; }
        public string SoSeRi { get; set; }
        public string MaSoBHXH { get; set; }
        public string MaThe { get; set; }


        public string HoTen { get; set; }
        public string NgaySinh { get; set; }
        public int GioiTinh { get; set; }
        public int? DanToc { get; set; }
        public string DiaChi { get; set; }
        public string NgheNghiep { get; set; }

        public string HoTenCha { get; set; }
        public string HoTenMe { get; set; }
        public string NguoiGiamHo { get; set; }
        public string TenDonVi { get; set; }
        public string NgayVao { get; set; }
        public string NgayRa { get; set; }

        public string ChanDoanLucVaoVien { get; set; }
        public string ChanDoanLucRaVien { get; set; }
        public string QuaTrinhBenhLyVaDienBienLamSang { get; set; }
        public string TomTatKetQuaXetNghiemCLS { get; set; }
        public string PhuongPhapDieuTri { get; set; }

        public string NgaySinhCon { get; set; }
        public string NgayChetCon { get; set; }
        public string SoConChet { get; set; }

        public int? TinhTrangRaVien { get; set; }
        public string GhiChu { get; set; }
        public string NguoiDaiDien { get; set; }
        public string NgayCapChungTu { get; set; }
        public DateTime? NgayTaoChungTuDateTime { get; set; }
        public string TEKT { get; set; }
    }

    #endregion Giấy tóm tắt bệnh án

    #region giấy Nghỉ dưỡng thai

    public class GiayChungNhanNghiDuongThai
    {

        public string MaCSKCB { get; set; }
        public string SoSeRi { get; set; }
        public string MaCT { get; set; }
        public string SoKCB { get; set; }
        public string MaSoBHXH { get; set; }
        public string MaThe { get; set; }

        public string HoTen { get; set; }
        public string NgaySinh { get; set; }

        public string TenDonVi { get; set; }
        public string ChanDoan { get; set; }
        public string TuNgay { get; set; }
        public string DenNgay { get; set; }
        public string NguoiDaiDien { get; set; }
        public string TenBS { get; set; }
        public string MaBS { get; set; }
        public string NgayCT { get; set; }
        public DateTime? NgayTaoChungTuDateTime { get; set; }
    }

    #endregion giấy Nghỉ dưỡng thai

    #region giấy Nghỉ Huong BHXH

    public class GiayNghiHuongBHXH
    {
        public string MaCT { get; set; }
        public string SOKCB { get; set; }
        public string MaBV { get; set; }
        public string MaBS { get; set; }
        public string MaSoBHXH { get; set; }
        public string MaThe { get; set; }

        public string HoTen { get; set; }
        public string NgaySinh { get; set; }
        public int GioiTinh { get; set; }
        public string PhuongPhapDieuTri { get; set; }
        public string MaDonVi { get; set; }
        public string TenDonVi { get; set; }

        public string TuNgay { get; set; }
        public string DenNgay { get; set; }
        public int SoNgay { get; set; }
        public string HoTenCha { get; set; }
        public string HoTenMe { get; set; }
        public string NgayCT { get; set; }
        public DateTime? NgayTaoChungTuDateTime { get; set; }

        public string NguoiDaiDien { get; set; }
        public string TenBS { get; set; }
        public string SeRi { get; set; }
        public string MauSo { get; set; }
    }

    #endregion giấy Nghỉ Huong BHXH

    public class MaSoTheBHXH
    {
        public int ThuTu { get; set; }
        public string GiaTri { get; set; }
    }
}
