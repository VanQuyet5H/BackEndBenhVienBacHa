using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Camino.Core.Domain.ValueObject.KhamDoan
{
    public class LichSuTiepNhanKhamSucKhoeDoanGridVo : GridItem
    {
        public string TenHopDong { get; set; }
        public string TenCongTy { get; set; }
        public int SoLuongBenhNhan { get; set; }
        public int SoLuongBenhNhanDaDen { get; set; }
        public DateTime NgayBatDauKham { get; set; }
        public string NgayBatDauKhamDisplay => NgayBatDauKham.ApplyFormatDate();
        public DateTime? NgayKetThucKham { get; set; }
        public string NgayKetThucKhamDisplay => NgayKetThucKham?.ApplyFormatDate();
        public long HopDongId { get; set; }
        public long CongTyId { get; set; }
        public List<int>  ListYeuCauTiepNhans { get; set; }
        public int SoBenhNhanDaDen { get; set; }
    }
    public class SearchVo
    {
        public string SearchString { get; set; }
        public string NgayBatDau { get; set; }
        public string NgayKetThuc { get; set; }
    }
    public class PhieuInNhanVienKhamSucKhoeInfoVo
    {
        public long Id { get; set; }
        public bool HasHeader { get; set; }
        public string HostingName { get; set; }
    }

    public class PhieuInDangKyKSKVo
    {
        public List<long> Ids { get; set; }
        public bool HasHeader { get; set; }
        public string HostingName { get; set; }
    }

    public class PhieuInNhanVienKhamSucKhoeViewModel
    {
        public string Html { get; set; }
        public string TenFile { get; set; }
        public bool? NoFooter { get; set; }
    }
    public class TienSuBenhBenhNhanKhaiBao
    {
        public long LoaiTienSuId { get; set; }
        public string LoaiTienSu { get; set; }
        public bool? BenhNgheNghiep { get; set; }
        public string TenBenh { get; set; }
        public DateTime? PhatHienNam { get; set; }
    }
    public class SoDinhKyKhamSucKhoeVo
    {
        public string So { get; set; }
        public string HoVaTen { get; set; }
        public string Nam { get; set; }
        public string Nu { get; set; }
        public string SoCMNDHoacHoChieu { get; set; }
        public string CapNgay { get; set; }
        public string Tai { get; set; }
        public string HoKhauThuongTru { get; set; }
        public string ChoOHienTai { get; set; }
        public string NgheNghiep { get; set; }
        public string NoiCongTacHocTap { get; set; }
        public string NgayBatDauVaoHocLamViecTaiDonViHienNay { get; set; }
        public string LietKeCongViecCongViecLamTrongVong10NamGanDay { get; set; }
        public string ThoiGianLamViec { get; set; }
        public string ThoiGianLamViecb { get; set; }
        public string NamLamViec { get; set; }
        public string ThangTuNgay { get; set; }
        public string DenThoiGianNghi { get; set; }
        public string TienSuBenhTatCuaGiaDinh { get; set; }
        public string TienSuBanThan { get; set; }
        public int NgayHienTai { get; set; }
        public int ThangHienTai { get; set; }
        public int NamHienTai { get; set; }
        public string HoTenNguoiLaoDongXacNhan { get; set; }
        public string HotenNguoiLapSo { get; set; }
        public string TienSuBenhTatBN { get; set; }
        public string ChieuCao { get; set; }
        public string CanNang { get; set; }
        public string ChiSoBMI { get; set; }
        public string Mach { get; set; }
        public string HuyetAp { get; set; }
        public string PhanLoaiTheLuc { get; set; }
        public string BsKhoaNoi { get; set; }
        public string TuanHoan { get; set; }
        public string PhanLoaiTuanHoan { get; set; }
        public string HoHap { get; set; }
        public string PhanLoaiHoHap { get; set; }
        public string TieuHoa { get; set; }
        public string PhanLoaiTieuHoa { get; set; }
        public string ThanTietNieu { get; set; }
        public string PhanLoaiThanTietNieu { get; set; }
        public string NoiTiet { get; set; }
        public string PhanLoaiNoiTiet { get; set; }
        public string CoXuongKhop { get; set; }
        public string PhanLoaiCoXuongKhop { get; set; }
        public string ThanKinh { get; set; }
        public string PhanLoaiThanKinh { get; set; }
        public string TamThan { get; set; }
        public string PhanLoaiTamThan { get; set; }
        public string BsNgoaKhoa { get; set; }
        public string NgoaiKhoa { get; set; }
        public string PhanLoaiNgoaiKhoa { get; set; }
        public string BsSanPhuKhoa { get; set; }
        public string SanPhuKhoa { get; set; }
        public string PhanLoaiSanPhuKhoa { get; set; }
        public string BsMat { get; set; }
        public string KhongKinhMatPhai { get; set; }
        public string KhongKinhMatTrai { get; set; }
        public string CoKinhMatPhai { get; set; }
        public string CoKinhMatTrai { get; set; }
        public string CacBenhVeMat { get; set; }
        public string PhanLoaiCacBenhVeMat { get; set; }
        public string BsTaiMuiHong { get; set; }
        public string NoiThuongTrai { get; set; }
        public string NoiThamTrai { get; set; }
        public string NoiThuongPhai { get; set; }
        public string NoiThamPhai { get; set; }
        public string CacBenhVeTaiMuiHong { get; set; }
        public string PhanLoaiTaiMuiHong { get; set; }
        public string BsRangHamMat { get; set; }
        public string HamTren { get; set; }
        public string HamDuoi { get; set; }
        public string CacBenhVeRangHamMat { get; set; }
        public string PhanLoaiRangHamMat { get; set; }
        public string BsDaLieu { get; set; }
        public string DaLieu { get; set; }
        public string PhanLoaiDaLieu { get; set; }
        public string PhanLoaiSucKhoe { get; set; }
        public string KetQua { get; set; }
        public string DanhGia { get; set; }
        public string BacSiKetLuanCLS { get; set; }
        public string BsDanhGia { get; set; }
        public string CacBenhTatNeuCo { get; set; }
        public string KhamSucKhoeDinhKyTienSuBenhTatBanThan { get; set; }
        public string KhamSucKhoeDinhKyTienSuBenhTatGiaDinh { get; set; }
        public string NguoiKetLuan { get; set; }
        public string KetLuanPhanLoaiSucKhoe { get; set; }
        public string KetLuanCacBenhTatNeuCo { get; set; }
        public string CongViec { get; set; }
    }
    public class PhieuDangKyKhamSucKhoeVo
    {
        public string DonViKham { get; set; }
        public string HoTen { get; set; }
        public string GioiTinh { get; set; }
        public string NamSinh { get; set; }
        public string MaNhanVien { get; set; }
        public string ChucVu { get; set; }
        public string GhiChu { get; set; }
        public string ViTriCongTac { get; set; }
        public string columnTable { get; set; }
        public string NhanVienTiepDon { get; set; }
        public string Ngay { get; set; }
        public string Thang { get; set; }
        public string Nam { get; set; }
        public string NguoiDiKham { get; set; }
        public string LogoUrl { get; set; }
        public string BarCodeImgBase64 { get; set; }
        public string NgayGioSACH { get; set; }
        public string MaTN { get; set; }

        //BVHD-3929
        public long YeuCauTiepNhanId { get; set; }
        public long? HopDongKhamSucKhoeNhanVienId { get; set; }
        public Enums.LoaiGioiTinh? LoaiGioiTinh { get; set; }
        public bool CoMangThai { get; set; }
        public bool DaLapGiaDinh { get; set; }
        public int? Tuoi { get; set; }
        public long? GoiKhamSucKhoeId { get; set; }
        public string GhiChuDV { get; set; }

    }
    public class DichVuKhamVaDichVuKyThuatKhamSucKhoeDoan : GridItem
    {
        public DichVuKhamVaDichVuKyThuatKhamSucKhoeDoan()
        {
            TenKhoaPhongThucHiens = new List<string>();
        }
        public int Stt { get; set; }
        public string NgayDV { get; set; }
        public string KhoaPhongThucHien { get; set; }
        public long KhoaPhongThucHienId { get; set; }
        public string GhiChu { get; set; }
        public long DichVu { get; set; }
        public string TenDichVu { get; set; }
        public string NoiDung { get; set; }
        public long DichVuKyThuatId { get; set; }

        //BVHD-3929
        public long? HopDongKhamSucKhoeNhanVienId { get; set; }
        public List<string> TenKhoaPhongThucHiens { get; set; }
    }
    public class KetQuaKhamSucKhoeVo
    {
        public string HOTEN { get; set; }
        public string GioiTinh { get; set; }
        public string MaKhachHang { get; set; }
        public string KhachHangDoanhNghiep { get; set; }
        public string NamSinh { get; set; }
        public string DONVI { get; set; }
        public string DHST { get; set; }
        public string DanhSachDichVuKham { get; set; }
        public string DanhSachDichVuKyThuat { get; set; }
        public string KetLuan { get; set; }
        public string DeNghi { get; set; }
        public string PhanLoaiSucKhoe { get; set; }
        public string Ngay { get; set; }
        public string Thang { get; set; }
        public string Nam { get; set; }
        public string BacSiKetLuanHoSo { get; set; }
        public string LogoUrl { get; set; }
        public string BarCodeImgBase64 { get; set; }
        public string MaTN { get; set; }
    }
    public class DanhSachDichVuKhamGrid : GridItem
    {
        public string TenDichVu { get; set; }
        public string KetQuaDichVu { get; set; }
        public string KetQuaDichVuDefault { get; set; }
        public string ThongTinKhamTheoDichVuData { get; set; }
        public string ThongTinKhamTheoDichVuTemplate { get; set; }
        public string TenNhom { get; set; }
        public long? GoiKhamSucKhoeId { get; set; }
        public Enums.EnumNhomGoiDichVu NhomId { get; set; }
        public EnumTypeLoaiDichVuKyThuat NhomDichVuKyThuat { get; set; }
        public EnumTypeLoaiChuyenKhoaEdit Type { get; set; }
        public string JsonStringKetQua { get; set; }
        public int TrangThaiDVKham { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public long? HopDongKhamSucKhoeNhanVienId { get; set; }
        public bool? KetQuaDaDuocLuu { get; set; }

        public long DichVuKhamBenhVienId { get; set; }

    }
    public enum EnumTypeLoaiDichVuKyThuat
    {
        [Description("THĂM DÒ CHỨC NĂNG VÀ CHẨN ĐOÁN HÌNH ẢNH")]
        NhomDichVuKyThuatTDCNCDHA = 1,
        [Description("XÉT NGHIỆM")]
        NhomDichVuKyThuatXN = 2,
    }
    public enum EnumTypeLoaiChuyenKhoaEdit
    {
        [Description("CHUYÊN KHOA NỘI")]
        ChuyenKhoaNoi = 1,
        [Description("CHUYÊN KHOA TAI MŨI HỌNG")]
        ChuyenKhoaTaiMuiHong = 2,
        [Description("CHUYÊN KHOA RĂNG HÀM MẶT")]
        ChuyenKhoaRangHamMat = 3,
        [Description("CHUYÊN KHOA MẮT")]
        ChuyenKhoaMat = 4,
        [Description("CHUYÊN SẢN PHỤ KHOA")]
        ChuyenSanPhuKhoa = 5,
        [Description("CHUYÊN DA LIỄU")]
        ChuyenDaLieu = 6,
        [Description("CHUYÊN NGOẠI KHOA")]
        ChuyenNgoaiKhoa = 7,
        [Description("DỊCH VỤ KỸ THUẬT")]
        Dvkt = 8
    }
    public class DanhSachDichVuKyThuatGrid
    {
        public string TenDichVu { get; set; }
        public string KetQuaDichVu { get; set; }
    }
    public class ComponentDynamics
    {
        public List<ComponentDynamicsOBJ> dss { get; set; }
    }
    public class ComponentDynamicsOBJ
    {
        public string Id { get; set; }
        public string Value { get; set; }
    }
    public class DataKhamTheoTemplate
    {
        public string Id { get; set; }
        public string Value { get; set; }
    }
    public class DataCLS
    {
        public string TenKetQua { get; set; }
        public string KetQua { get; set; }
        public string KetLuan { get; set; }
    }
    public class KetQuaXNGridVo
    {
        public string KetLuan { get; set; }
        public string TenDichVuXetNghiem { get; set; }
    }
    public class TemplateAndDataGrid : GridItem
    {
        public string ThongTinKhamTheoDichVuTemplate { get; set; }
        public string ThongTinKhamTheoDichVuData { get; set; }
    }

    public class TienSuBenhGridVo
    {
        public Enums.EnumLoaiTienSuBenh LoaiTienSuId { get; set; }
        public string LoaiTienSu { get; set; }
        public bool? BenhNgheNghiep { get; set; }
        public string TenBenh { get; set; }
        public DateTime? PhatHienNam { get; set; }
    }
    public class KetQuaKhamChuyenKhoaNoi
    {
        public string TuanHoan { get; set; }
        public string HoHap { get; set; }
        public string NoiTiet { get; set; }
        public string TieuHoa { get; set; }
        public string ThanKinh { get; set; }
        public string TamThan { get; set; }
        public string CoXuongKhop { get; set; }
        public string ThanTietNieu { get; set; }
        public string Khac { get; set; }
    }
    public class KetQuaKhamChuyenKhoaMat
    {
        public string KhongKinhMatPhai { get; set; }
        public string KhongKinhMatTrai { get; set; }
        public string CoKinhMatPhai { get; set; }
        public string CoKinhMatTrai { get; set; }
        public string CacBenhVeMat { get; set; }
    }
    public class KetQuaKhamChuyenKhoaTaiMuiHong
    {
        public string TaiPhaiNoiThuong { get; set; }
        public string TaiPhaiNoiTham { get; set; }
        public string TaiTraiNoiThuong { get; set; }
        public string TaiTraiNoiTham { get; set; }
        public string CacBenhTaiMuiHong { get; set; }
    }
    public class KetQuaKhamChuyenKhoaNoiRangHamMat
    {
        public string HamTren { get; set; }
        public string HamDuoi { get; set; }
        public string CacBenhRangHamMat { get; set; }
    }

    public class KetQuaNgoaiDaLieuSanPhuKhoa
    {
        public string value { get; set; }
    }
    public class KetQuaDVKT
    {
        public string value { get; set; }
    }
    public class KetQuaTatCaKhoa
    {
        public KetQuaNgoaiDaLieuSanPhuKhoa KetQuaNgoaiDaLieuSanPhuKhoaOBJ { get; set; }
        public KetQuaKhamChuyenKhoaNoiRangHamMat KetQuaKhamChuyenKhoaNoiRangHamMatOBJ { get; set; }
        public KetQuaKhamChuyenKhoaNoi KetQuaKhamChuyenKhoaNoiOBJ { get; set; }
        public KetQuaKhamChuyenKhoaMat KetQuaKhamChuyenKhoaMatOBJ { get; set; }
        public KetQuaKhamChuyenKhoaTaiMuiHong KetQuaKhamChuyenKhoaTaiMuiHongOBJ { get; set; }
        public KetQuaDVKT KetQuaDVKTOBJ { get; set; }
    }
    public class KetQua
    {
        public string value { get; set; }
    }
    public class DanhSachPhanLoaiCacBenhTatGrid : GridItem
    {
        public long? HopDongKhamSucKhoeNhanVienId { get; set; }
        public string Ten { get; set; }
        public string KetQuaDefault { get; set; }
        public string KetQua { get; set; }
        public long PhanLoaiId { get; set; }
        public long PhanLoaiIdCapNhat { get; set; }
        public bool DaCoketLuan { get; set; }
        public bool ShowComBoBox { get; set; }
        public EnumTypeLoaiKetLuan LoaiKetLuan { get; set; }
    }
    public enum EnumTypeLoaiKetLuan
    {
        [Description("Phân loại")]
        PhanLoai = 1,
        [Description("Các bệnh tật (nếu có)")]
        CacBenhTatNeuCo = 2,
        [Description("Đề nghị")]
        DeNghi = 3,
    }
    public class KetQuaKhamSucKhoeDaTa
    {
        public string KetQuaKhamSucKhoe { get; set; }
        public long NhanVienKetLuanId { get; set; }
        public DateTime ThoiDiemKetLuan { get; set; }
    }
    public class KSKUpdateAllNhanVienTheoHopDong
    {
        public string KetQuaKhamSucKhoeData { get; set; }
        public string ketLuanData { get; set; }
        public long YeuCauTiepNhanId { get; set; }
    }
    #region  new api  báo cáo tổng hợp kết qua ksk
    public class InfoDichVu
    {
        public InfoDichVu()
        {
            YeuCauDichVuKyThuatPhauThuatThuThuats = new List<KetQuaDichVuKyThuatPTTT>();
            YeuCauKhamBenhIds = new List<long>();
            YeuCauDVKTIds = new List<long>();
        }
        public List<Camino.Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> YeuCauKhamBenhs { get; set; }
        public List<Camino.Core.Domain.Entities.YeuCauKhamBenhs.YeuCauDichVuKyThuat> YeuCauDichVuKyThuats { get; set; }
        public string ThongTinNhanVienKhamKetQuaKhamSucKhoeData { get; set; }
        public bool? ThongTinNhanVienKhamLoaiLuuInKetQuaKSK { get; set; }
        public long? HopDongKhamSucKhoeNhanVienId { get; set; }
        public List<InfoDVKT> YeuCauDichVuKyThuatIds { get; set; }
        public List<KetQuaDichVuKyThuatPTTT> YeuCauDichVuKyThuatPhauThuatThuThuats { get; set; }

        public List<long> YeuCauKhamBenhIds { get; set; }
        public List<long> YeuCauDVKTIds { get; set; }
    }
    public class InfoDVKT :GridItem
    {
        public Domain.Enums.LoaiDichVuKyThuat LoaiDichVuKyThuat { get; set; }
    }
    public class ThongTinInfo
    {
        public ThongTinInfo()
        {
            ThongTinNhanVienKhamTheoYeuCauKhamBenhs = new List<ThongTinNhanVienKhamTheoYeuCauKhamBenh>();
            ThongTinNhanVienKhamYeuCauDichVuKyThuatTDCNCDHAs = new List<ThongTinNhanVienKhamTheoYeuCauDichVuKyThuatTDCNCDHA>();
            ThongTinNhanVienKhamYeuCauDichVuKyThuatXNs = new List<ThongTinNhanVienKhamTheoYeuCauDichVuKyThuatXetNghiem>();
            ThongTinNhanVienKhamTheoYeuCauDichVuKyThuats = new List<ThongTinNhanVienKhamTheoYeuCauDichVuKyThuat>();
            ThongTinNhanVienKhamTheoYeuCauDichVuKyThuatThuThuatPhauThuats = new List<ThongTinNhanVienKhamTheoYeuCauDichVuKyThuatThuThuatPhauThuat>();
        }
        public List<ThongTinNhanVienKhamTheoYeuCauKhamBenh> ThongTinNhanVienKhamTheoYeuCauKhamBenhs { get; set; }
        public List<ThongTinNhanVienKhamTheoYeuCauDichVuKyThuatTDCNCDHA> ThongTinNhanVienKhamYeuCauDichVuKyThuatTDCNCDHAs { get; set; }
        public List<ThongTinNhanVienKhamTheoYeuCauDichVuKyThuatXetNghiem> ThongTinNhanVienKhamYeuCauDichVuKyThuatXNs { get; set; }
        public List<ThongTinNhanVienKhamTheoYeuCauDichVuKyThuat> ThongTinNhanVienKhamTheoYeuCauDichVuKyThuats { get; set; }
        public List<ThongTinNhanVienKhamTheoYeuCauDichVuKyThuatThuThuatPhauThuat> ThongTinNhanVienKhamTheoYeuCauDichVuKyThuatThuThuatPhauThuats { get; set; }
        public string ThongTinNhanVienKhamKetQuaKhamSucKhoeData { get; set; }
        public bool? ThongTinNhanVienKhamLoaiLuuInKetQuaKSK { get; set; }
        public long? HopDongKhamSucKhoeNhanVienId { get; set; }
    }
    public class ThongTinNhanVienKhamTheoYeuCauKhamBenh : GridItem
    {
        public long? TenDichVuId { get; set; }
        public string TenDichVu { get; set; }
        public string KetQuaDichVu { get; set; }
        public string KetQuaDichVuDefault { get; set; }
        public string ThongTinKhamTheoDichVuData { get; set; }
        public string ThongTinKhamTheoDichVuTemplate { get; set; }
        public string TenNhom { get; set; }
        public long? GoiKhamSucKhoeId { get; set; }
        public Enums.EnumNhomGoiDichVu NhomId { get; set; }
        public EnumTypeLoaiDichVuKyThuat NhomDichVuKyThuat { get; set; }
        public EnumTypeLoaiChuyenKhoaEdit Type { get; set; }
        public string JsonStringKetQua { get; set; }
        public int TrangThaiDVKham { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public long? HopDongKhamSucKhoeNhanVienId { get; set; }
    }
    public class ThongTinNhanVienKhamTheoYeuCauDichVuKyThuatTDCNCDHA :GridItem
    {
        public string DataKetQuaCanLamSang { get; set; }
        public string TenDichVuKyThuat { get; set; }
        public long? GoiKhamSucKhoeId { get; set; }
        public int TrangThaiDVKham { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
    }
    public class ThongTinNhanVienKhamTheoYeuCauDichVuKyThuatXetNghiem : GridItem
    {
        public DataKetQuaCanLamSangVo DataKetQuaCanLamSangVo { get; set; }
        public string TenDichVuKyThuat { get; set; }
        public long? GoiKhamSucKhoeId { get; set; }
        public int TrangThaiDVKham { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
    }
    public class ThongTinNhanVienKhamTheoYeuCauDichVuKyThuat : GridItem
    {
        public string TenDichVuKyThuat { get; set; }
        public long? GoiKhamSucKhoeId { get; set; }
        public int TrangThaiDVKham { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
    }
    public class DataKetQuaCanLamSangVo
    {
        public List<KetQuaXetNghiemChiTiet> KetQuaXetNghiemChiTiets { get; set; }
        public int?  LanThucHien { get; set; }
        public string KetLuan { get; set; }
    }
    public class ThongTinNhanVienKhamTheoYeuCauDichVuKyThuatThuThuatPhauThuat : GridItem
    {
        public string TenDichVuKyThuat { get; set; }
        public long? GoiKhamSucKhoeId { get; set; }
        public int TrangThaiDVKham { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public string KetQua { get; set; }
    }
    public class KetQuaDichVuKyThuatPTTT
    {
        public long Id { get; set; }
        public string ketQua { get; set; }
    }
    #endregion


    public class DichVuGridVos
    {
        public long IdDichVu { get; set; }
        public long NhomId { get; set; }
        public long YeuCauTiepNhanId { get; set; }
    }

    public class InfoPTTT : GridItem
    {
        public string TenDichVuKyThuat { get; set; }
        public long? GoiKhamSucKhoeId { get; set; }
        public int TrangThai { get; set; }
        public string KetQuaDichVu { get; set; }
    }

    public class DVKTXetNghiem : GridItem
    {
        public DataKetQuaCanLamSangVo DataKetQuaCanLamSang { get; set; }
        public string TenDichVuKyThuat { get; set; }
        public int TrangThai { get; set; }
        public long? GoiKhamSucKhoeId { get; set; }
    }
}
