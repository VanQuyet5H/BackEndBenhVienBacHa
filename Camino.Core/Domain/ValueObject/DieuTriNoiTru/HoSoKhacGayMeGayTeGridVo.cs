
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.DieuTriNoiTru
{
    public class ThongTinHoSoJSON
    {
        public ThongTinHoSoJSON()
        {
            ThongTinQuanHeThanNhans = new List<ThongTinQuanHeThanNhanVo>();
        }
        public long NhanVienGiaiThichId { get; set; }
        public List<ThongTinQuanHeThanNhanVo> ThongTinQuanHeThanNhans { get; set; }
    }

    public class ThongTinQuanHeThanNhanVo : GridItem
    {
        public string HoTen { get; set; }
        public int? NamSinh { get; set; }
        public string CMND { get; set; }
        public long? QuanHeThanNhanId { get; set; }
        public string TenQuanHeThanNhan { get; set; }
        public string DiaChi { get; set; }
    }

    public class BienBanGayTeGayMe
    {
        public string Khoa { get; set; }
        public string NguoiBenh { get; set; }
        public string SinhNam { get; set; }
        public string CMND { get; set; }
        public string NgayCap { get; set; }
        public string NoiCap { get; set; }
        public string MSBenhVien { get; set; }
        public string DiaChiBV { get; set; }
        public string QuanHeThanNhanKhacs { get; set; }
        public string TenQHTN { get; set; }
        public string TenTN { get; set; }
        public string NhanVienThucHien { get; set; }
        public int? SinhNamTN { get; set; }
        public string BSGiaiThich { get; set; }
        public string Ngay { get; set; }
        public string Thang { get; set; }
        public string Nam { get; set; }
        public string MaTN { get; set; }
        public string BarCodeImgBase64 { get; set; }
        public string TenNB { get; set; }
    }

    public class BienKiemTruocTiemChungTE
    {
        //&#10004;
        public string BarCodeImgBase64 { get; set; }
        public string SoBA { get; set; }
        public string HoTen { get; set; }
        public string GTNam { get; set; }
        public string GTNu { get; set; }
        public string Gio { get; set; }
        public string Phut { get; set; }
        public string NgaySinh { get; set; }
        public string ThangSinh { get; set; }
        public string NamSinh { get; set; }
        public string DiaChi { get; set; }
        public string HoTenBoMe { get; set; }
        public string ThuocVacXin { get; set; }
        public string Khong1 { get; set; }
        public string Co1 { get; set; }
        public string Khong2 { get; set; }
        public string Co2 { get; set; }
        public string Khong3 { get; set; }
        public string Co3 { get; set; }
        public string Khong4 { get; set; }
        public string Co4 { get; set; }
        public string Khong5 { get; set; }
        public string Co5 { get; set; }
        public string Khong6 { get; set; }
        public string Co6 { get; set; }
        public string DuDieuKien { get; set; }
        public string TamHoan { get; set; }
        public string ThoiDiemIn { get; set; }
        public string Ngay { get; set; }
        public string Thang { get; set; }
        public string Nam { get; set; }
        public string BSThucHien { get; set; }
    }

    public class ThongTinHoSoBienKiemTruocTiemChungTE
    {
        //&#10004;
        public List<long> DuocPhamIds { get; set; }
        public bool? SotHaThanNhiet { get; set; }
        public bool? NgheTimBatThuong { get; set; }
        public bool? NghePhoiBatThuong { get; set; }
        public bool? TriGiacBatThuong { get; set; }
        public bool? CanNangDuoi2000g { get; set; }
        public bool? CoCacChongChiDinhKhac { get; set; }
        public bool? DuDieuKienTiemChung { get; set; }
        public bool? TamHoanTiemChung { get; set; }
    }

    public class ThongTinNhanVienDangNhap
    {
        public ThongTinNhanVienDangNhap()
        {
            HoSoKhacTreSoSinhs = new List<HoSoKhacTreSoSinhJSON>();
        }
        public long NhanVienDangNhapId { get; set; }
        public string NhanVienDangNhap { get; set; }
        public DateTime NgayThucHien { get; set; }
        public string NgayThucHienDisplay => NgayThucHien.ApplyFormatDateTimeSACH();
        public string ChanDoan { get; set; }
        public string PhuongPhapDieuTri { get; set; }
        public List<HoSoKhacTreSoSinhJSON> HoSoKhacTreSoSinhs { get; set; }
        public int SoConHienTai { get; set; }
    }
    public class HoSoKhacTreSoSinhJSON
    {
        public DateTime? DeLuc { get; set; }
        public long? TinhTrangId { get; set; }
        public string GioiTinh { get; set; }
        public string HoTenCon { get; set; }
        public string CanNang { get; set; }
        public string GhiChu { get; set; }
    }
    public class TongKetBenhAnJSON
    {
        public TongKetBenhAnJSON()
        {
            HoSoKhacTreSoSinhs = new List<HoSoKhacTreSoSinhJSON>();
            DacDiemTreSoSinhs = new List<HoSoKhacTreSoSinhJSON>();
        }
        public List<HoSoKhacTreSoSinhJSON> DacDiemTreSoSinhs { get; set; }
        public List<HoSoKhacTreSoSinhJSON> HoSoKhacTreSoSinhs { get; set; }
    }

    public class PhuongPhapDieuTriJSON
    {
        public string PhuongPhapDieuTri { get; set; }
        public string ChanDoan { get; set; }
        public string GhiChu { get; set; }
        public string GhiChuChuanDoanRaVien { get; set; }
        public long? IdGhiChu { get; set; }
    }

    public class GiayRaVienVo
    {
        public string Khoa { get; set; }
        public string SoBA { get; set; }
        public string MaBN { get; set; }
        public string HoTenBenhNhan { get; set; }
        public string NamSinh { get; set; }
        public string GioiTinh { get; set; }
        public string DanToc { get; set; }
        public string NgheNghiep { get; set; }
        public string BHYTMaSoThe { get; set; }
        public string DiaChi { get; set; }
        public string NgayThangNamGioVaoVien { get; set; }
        public string NgayThangNamGioRaVien { get; set; }
        public string ChanDoan { get; set; }
        public string PPDieuTri { get; set; }
        public string GhiChu { get; set; }
        public string Ngay { get; set; }
        public string Thang { get; set; }
        public string Nam { get; set; }
    }

    public class GiayChungSinhVo
    {
        public string So { get; set; }
        public string Quyen { get; set; }
        public string HoTenMe { get; set; }
        public string NamSinh { get; set; }
        public string NoiDangKyThuongTru { get; set; }
        public string BHYTMaSoThe { get; set; }
        public string CMND { get; set; }
        public string NgayCap { get; set; }
        public string ThangCap { get; set; }
        public string NamCap { get; set; }
        public string NoiCap { get; set; }
        public string DanToc { get; set; }
        public string HoTenCha { get; set; }
        public string GioSDCon { get; set; }
        public string SoConSinh { get; set; }
        public string GTCon { get; set; }
        public string CanNangCon { get; set; }
        public string HoTenCon { get; set; }
        public string GhiChu { get; set; }
        public string Ngay { get; set; }
        public string Thang { get; set; }
        public string Nam { get; set; }
        public string HoTenChaMe { get; set; }
        public string NhanVienDoDe { get; set; }
        public string NhanVienGhiPhieu { get; set; }
        public string GiamDocChuyenMon { get; set; }
        public string LogoUrl { get; set; }
    }

    public class GiayChungSinhJSON
    {
        public GiayChungSinhJSON()
        {
            HoSoKhacTreSoSinhs = new List<HoSoKhacTreSoSinhJSON>();
        }
        public string So { get; set; }
        public string QuyenSo { get; set; }
        public string HoTenCha { get; set; }
        public string GhiChu { get; set; }
        public long? NhanVienDoDeId { get; set; }
        public long? NhanVienGhiPhieuId { get; set; }
        public long? GiamDocChuyenMonId { get; set; }
        public List<HoSoKhacTreSoSinhJSON> HoSoKhacTreSoSinhs { get; set; }

    }
    #region BVHD-3705
    public class GiayChungSinhNewJSONVo
    {
        public long NoiTruHoSoKhacId { get; set; }
        public string So { get; set; }
        public string QuyenSo { get; set; }
        public string HoVaTenCha { get; set; }
        public string CMND { get; set; }
        public DateTime? NgayCap { get; set; }
        public string NoiCap { get; set; }
        public string DuDinhDatTenCon { get; set; }
        public string GioiTinh { get; set; }
        public double? CanNang { get; set; }
        public string GhiChu { get; set; }
        public DateTime? NgayCapGiayChungSinh { get; set; } // ngày thực hiện
        public long? NhanVienDoDeId { get; set; }
        public long? NhanVienGhiPhieuId { get; set; }
        public long? GiamDocChuyenMonId { get; set; }
        public int TrangThaiLuu { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public DateTime? ThoiGianDe { get; set; }
        
    }
    #endregion
    public class InfoBAConGridVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }

        public string DuDinhDatTenCon { get; set; }
        public string GioiTinh { get; set; }
        public double? CanNang { get; set; }
        public DateTime? ThoiGianDe { get; set; }
    }
    public class InfoDacDiemTreSoSinhGridVo : GridItem
    {
        public string GioiTinh { get; set; }
        public double? CanNang { get; set; }
        public string DuDinhDatTenCon { get; set; }
        public string TinhTrang { get; set; }
        public DateTime? DeLuc { get; set; }
        public string DeLucDisplayName { get; set; }
        public long? YeuCauTiepNhanConId { get; set; }
    }
    public class InfoVo : GridItem
    { 
        public string HoTen { get; set; }
    }
    }
