using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.BangKiemAnToanNBPT
{
    public class ThuocTienMeVaNhanVienGrid
    {
        public List<string> ListThuocTienMe { get; set; }
        public List<string> ListNhanVien { get; set; }
        public List<ChiSoSinhTonBangKiemAnToan> ListChiSoSinhTonBangKiemAnToan { get; set; }
    }
    public class ChiSoSinhTonBangKiemAnToan : GridItem
    {
        public int? NhipTim { get; set; }
        public int? NhipTho { get; set; }
        public string HuyetAp { get; set; }
        public double? ThanNhiet { get; set; }
        public double? ChieuCao { get; set; }
        public double? CanNang { get; set; }
        public double? BMI { get; set; }
        public string NhanVienThucHien { get; set; }
        public string NgayThucHien { get; set; }
        public double? Glassgow { get; set; }
        public double? SpO2 { get; set; }

        public double? HuyetApTamThu { get; set; }
        public double? HuyetApTamTruong { get; set; }

    }
    public class BangKiemAnToanNBPTGridVo : GridItem
    {
        public long YeuCauTiepNhanId { get; set; }
        public DateTime ThoiDiemThucHien { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
        public string ThongTinHoSo { get; set; }
        public long NhanVienThucHienId { get; set; }
        public long NoiThucHienId { get; set; }
        public List<BangKiemAnToanGridVo> ListFile { get; set; }
    }
    public class DanhSachBangKiemAnToanNBPTGridVo : GridItem
    {
        public DateTime NgayGioDuaBNDiPT { get; set; }
        public string NgayGioDuaBNDiPTUTC { get; set; }
        public string NgayGioDuaBNDiPTString { get; set; }
        public string ThongTinHoSo { get; set; }
    }
    public class BangKiemAnToanGridVo : GridItem
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenGuid { get; set; }
        public long KichThuoc { get; set; }
        public string DuongDan { get; set; }
        public Enums.LoaiTapTin LoaiTapTin { get; set; }
        public string MoTa { get; set; }
    }
    public class InBangKiemAnToanGridVo : GridItem
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenGuid { get; set; }
        public long KichThuoc { get; set; }
        public string DuongDan { get; set; }
        public Enums.LoaiTapTin LoaiTapTin { get; set; }
        public string MoTa { get; set; }
    }
    public class InModelBangKiemAnToanNBPT 
    {
       
        public string TienSuDiUng  { get; set; }
        public string ThuocDangDung { get; set; }
        public bool KhamVaTuVanCuaPTV { get; set; }
        public bool BenhChinh { get; set; }
        public bool BenhKemTheo { get; set; }
        public bool DaiThaoDuong { get; set; }
        public bool TangHuyetAp { get; set; }
        public bool Khac { get; set; }
        public bool VeSinhTamGoi { get; set; }
        public bool VatLieuCayGhep { get; set; }
        public bool CatMongTayMongChan { get; set; }
        public bool DoTrangSuc { get; set; }
        public bool ThaoRangGia { get; set; }
        public bool QuanAoSachMoiThay { get; set; }
      
        public bool VetThuongHo { get; set; }
        public bool VeSinhDaVungMo { get; set; }
        public bool BangVoTrungDanhDauViTriPhauThuat { get; set; }
        public bool KhamGayMe { get; set; }
        public bool PTTTGMHS { get; set; }
        public bool PhieuXetNghiemNhomMauDongMau { get; set; }
        public bool PhimChupPhoiSoLuong { get; set; }
        public bool CacLoaiPhimAnhKhacSoLuong { get; set; }
        public bool DienTim { get; set; }
        public bool XacNhanThanhVienGayMePhauThuat { get; set; }
        public bool KhangSinhDuPhong { get; set; }
        public bool DuyetPhauThuat { get; set; }
        public string ThongSoKhac { get; set; }
        public string NguyCoSuyHoHapMatMau { get; set; }
        public string LamSangCLSCanLuuY { get; set; }
        public string NhungLuuYKhac { get; set; }
        public string TiepXuc { get; set; }
        public string NgayThucHien { get; set; }
        public bool NhacBenhNhanNhinAn { get; set; }
        public DateTime NgayGioDuaBNDiPT { get; set; }
        public DateTime? NgayGioDuDinhGayMe { get; set; }
        public string YKienCuaNguoiNhanNguoiBenhTaiPhongGMHS { get; set; }
        public string DDChuanBiNBTruocPT { get; set; }
        public string DDChuanBiNBDenPhongPT { get; set; }
        public string DDNhanBNTaiPhongPTGMHS { get; set; }
        public string ChanDoanICDPhu { get; set; }
        public string ChanDoanICDChinh { get; set; }
        public string ThuocTienMe { get; set; }
        public string NgayGioDuaBNDiPTUTC { get; set; }
        public string NgayGioDuDinhGayMeUTC { get; set; }

        public bool PhieuCamDoanPTTT { get; set; }
        public bool PhimChupPhoiSL { get; set; }
        public bool CacLoaiPhimAnh { get; set; }
        public bool KyCamKetSuDungKTCao { get; set; }
        public bool TheDinhDanh { get; set; }
        public bool PhieuXN  { get; set; }

        public DateTime ThoiDiemKham { get; set; }
        public string ThoiDiemKhamString { get; set; }
    }
    public class InBangKiemAnToanNBPT
    {
        public string KhoaPhongDangIn { get; set; }
        public string LogoUrl { get; set; }
        public string MaTN { get; set; }
        public string Ngay { get; set; }
        public string Thang { get; set; }
        public string Nam { get; set; }
        public string HoTenNguoiBenh { get; set; }
        public string Tuoi { get; set; }
        public string GioiTinhNam { get; set; }
        public string GioiTinhNu { get; set; }
        public string KhoaPhong { get; set; }
        public string Giuong { get; set; }
        public string ChanDoan { get; set; }
        public string ThuocDangDung { get; set; }
        public string TienSuDiUng { get; set; }
        public string KhamVaTuVanCuaPTV { get; set; }

        public string KhamVaTuVanCuaPTVKhong { get; set; }
        public string TiepXuc { get; set; }
        public string BenhChinhStringCo { get; set; }
        public string BenhChinhStringKhong { get; set; }
        public string BenhKemTheoString { get; set; }
        public string BenhKemTheoStringKhong { get; set; }
        public string CanNang { get; set; }
        public string DaiThaoDuong { get; set; }
        public string DaiThaoDuongKhong { get; set; }
        public string Mach { get; set; }
        public string TangHuyetAp { get; set; }
        public string TangHuyetApKhong { get; set; }
        public string NhietDo { get; set; }
        public string Khac { get; set; }
        public string KhacKhong { get; set; }
        public string HuyetAp { get; set; }
        public string TheDinhDanhCo { get; set; }
        public string TheDinhDanhKhong { get; set; }
        public string NhipTho { get; set; }
        public string DanNguoiBenhCo { get; set; }
        public string DanNguoiBenhKhong { get; set; }
        public string VeSinhTamGoi { get; set; }
        public string VeSinhTamGoiKhong { get; set; }
        public string VatLieuCayGhep { get; set; }
        public string VatLieuCayGhepKhong { get; set; }
        public string NhomMau { get; set; }
        public string CatMongTayMongChan { get; set; }
        public string CatMongTayMongChanKhong { get; set; }
        public string ThongSoKhac { get; set; }
        public string DoTrangSuc { get; set; }

        public string DoTrangSucKhong { get; set; }
        public string ThaoRangGia { get; set; }
        public string ThaoRangGiaKhong { get; set; }
        public string QuanAoSachMoiThay { get; set; }
        public string QuanAoSachMoiThayKhong { get; set; }
        public string VetThuongHo { get; set; }


        public string VetThuongHoKhong { get; set; }
        public string VeSinhDaVungMo { get; set; }
        public string VeSinhDaVungMoKhong { get; set; }
        public string BangVoTrungDanhDauViTriPhauThuat { get; set; }
        public string BangVoTrungDanhDauViTriPhauThuatKhong { get; set; }

        public string KhamGayMe { get; set; }

        public string KhamGayMeKhong { get; set; }


        public string PTTTGMHS { get; set; }
        public string PTTTGMHSKhong { get; set; }
        public string PhieuXetNghiemNhomMauDongMau { get; set; }
        public string PhieuXetNghiemNhomMauDongMauKhong { get; set; }
        public string LamSangCLSCanLuuY { get; set; }
        public string PhimChupPhoiSoLuong { get; set; }
        public string PhimChupPhoiSoLuongKhong { get; set; }
        public string CacLoaiPhimAnhKhacSoLuong { get; set; }
        public string CacLoaiPhimAnhKhacSoLuongKhong { get; set; }
        public string DienTim { get; set; }
        public string DienTimKhong { get; set; }
        public string XacNhanThanhVienGayMePhauThuat { get; set; }
        public string XacNhanThanhVienGayMePhauThuatKhong { get; set; }
        public string KhangSinhDuPhong { get; set; }
         public string KhangSinhDuPhongKhong { get; set; }
        public string NhungLuuYKhac { get; set; }
        public string DuyetPhauThuat { get; set; }
        public string DuyetPhauThuatKhong { get; set; }
        public string GioPhauThuat { get; set; }
        public string NgayPhauThuat { get; set; }
        public string ThangPhauThuat { get; set; }
        public string NamPhauThuat { get; set; }
        public string GioGayMe { get; set; }
        public string NgayGayMe { get; set; }
        public string ThangGayMe { get; set; }
        public string NamGayMe { get; set; }
        public string YKienNguoiNguoiNhanNguoiBenh { get; set; }
        public string DDChuanBiNBTruocPT { get; set; }
        public string DDChuanBiNBDenPhongPT { get; set; }
        public string DDNhanBNTaiPhongPTGMHS { get; set; }
        public string NguyCoSuyHoHapMatMau { get; set; }
        public string BarCodeImgBase64 { get; set; }
        public string ChieuCao { get; set; }
        public string KyCamKetSuDungKTCao { get; set; }
        public string KyCamKetSuDungKTCaoKhong { get; set; }

    }
}
