using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.NoiTruBenhAn;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.YeuCauKhamBenh
{
    public class DanhSachChoKhamGridVo : GridItem
    {
        public long? YeuCauTiepNhanId { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string MaBenhNhan { get; set; }
        public long? BenhNhanId { get; set; }
        public string HoTen { get; set; }
        public int? NamSinh { get; set; }
        public string NgayThangNam { get; set; }
        public string DiaChi { get; set; }
        public string BHYTMaSoThe { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public DateTime? ThoiDiemTiepNhan { get; set; }
        public string TrieuChungTiepNhan { get; set; }
        public string DoiTuong { get; set; }
        public string ThoiDiemTiepNhanTu { get; set; }
        public string ThoiDiemTiepNhanDen { get; set; }
        public string ThoiDiemTiepNhanDisplay { get; set; }
        public string CoBaoHiemTuNhan { get; set; }
        public string SearchString { get; set; }
        public EnumTrangThaiYeuCauTiepNhan? TrangThaiYeuCauTiepNhan { get; set; }
        public string TrangThaiYeuCauTiepNhanSearch { get; set; }
        public EnumTrangThaiYeuCauKhamBenh? TrangThaiYeuCauKhamBenh { get; set; }
        public string TrangThaiYeuCauKhamBenhSearch { get; set; }
        public string TenDichVu { get; set; }
        public bool? CoBHYT { get; set; }
        public bool? CoDonThuocBHYT { get; set; }
        public bool? CoDonThuocKhongBHYT { get; set; }
        public bool? CoDonVatTu { get; set; }
        public bool? CoYeuCauKhamBenhNhapVien { get; set; }
        public string TenNhanVienTiepNhan { get; set; }

        public string ChuanDoan { get; set; }
        public string CachGiaiQuyet { get; set; }
        public string BSKham { get; set; }

        public DateTime? ThoiDiemThucHien { get; set; }
        public string ThoiDiemThucHienDisplay => ThoiDiemThucHien?.ApplyFormatDateTime();
        public string TenNhanVienChiDinh { get; set; }
        public bool? CoNhapVien { get; set; }
    }

    public class DanhSachTiepNhanExportExcel
    {
        [Width(30)]
        public string MaYeuCauTiepNhan { get; set; }
        [Width(20)]
        public string MaBenhNhan { get; set; }
        [Width(40)]
        public string HoTen { get; set; }
        [Width(20)]
        public int? NamSinh { get; set; }
        [Width(60)]
        public string DiaChi { get; set; }
        [Width(40)]
        public string TenNhanVienTiepNhan { get; set; }
        [Width(30)]
        public string ThoiDiemTiepNhanDisplay { get; set; }
        [Width(30)]
        public string ThoiDiemThucHienDisplay { get; set; }
        [Width(40)]
        public string TrieuChungTiepNhan { get; set; }
        [Width(40)]
        public string ChuanDoan { get; set; }
        [Width(40)]
        public string CachGiaiQuyet { get; set; }
        [Width(40)]
        public string BSKham { get; set; }
        [Width(40)]
        public string DoiTuong { get; set; }
        [Width(20)]
        public string CoBaoHiemTuNhan { get; set; }
        [Width(30)]
        public string TrangThaiYeuCauTiepNhanSearch { get; set; }
        [Width(30)]
        public string TenDichVu { get; set; }
        [Width(30)]
        public string TrangThaiYeuCauKhamBenhSearch { get; set; }
    }


    public class KhamBenhDSCKGSearchGridVoItem : GridItem
    {
        public long? PhongBenhVienId { get; set; }
        public string searchString { get; set; }
    }
    public class ICDTemplateVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }
        public int? Rank { get; set; }
    }

    public class InputStringStoredTemplateVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public int? Rank { get; set; }
    }

    public class ICDKhacsTemplateVo : GridItem
    {
        public long YeuCauKhamBenhId { get; set; }
        public long? ICDId { get; set; }
        public string GhiChu { get; set; }
        public string TenICD { get; set; }
    }

    public class NhanVienHoTongTemplateVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string TenNhanVien { get; set; }
        public string TenNhomChucDanh { get; set; }
        public string TenVanBang { get; set; }
    }

    public class DoiTuongPhongChoTemplateVo : LookupItemVo
    {
        public DateTime? ThoiDiemTiepNhan { get; set; }
    }

    public class KhamBenhDSCKGGridVoItem : GridItem
    {
        public string FromDate { get; set; }

        public string ToDate { get; set; }
        public long? PhongBenhVienId { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public string HoTen { get; set; }
        public int? NamSinh { get; set; }
        public string DiaChi { get; set; }
        public Enums.EnumTrangThaiHangDoi? TrangThai { get; set; }
        public Enums.EnumLoaiHangDoi? LoaiHangDoi { get; set; }

        public string TrangThaiDisplay { get; set; }
        public DateTime? ThoiDiemTiepNhan { get; set; }
        public string ThoiDiemTiepNhanDisplay { get; set; }
        public string TenBenh { get; set; }
        public string DoiTuong { get; set; }
        public long? DoiTuongBHYT { get; set; }
        public string ThoiDiemTiepNhanTu { get; set; }
        public string ThoiDiemTiepNhanDen { get; set; }
        public long? PhongKhamHienTaiId { get; set; }
        public string TenDiaChi { get; set; }
        public string TenPhuongXa { get; set; }
        public string TenQuanHuyen { get; set; }
        public string TenTinhThanh { get; set; }
        public string DiaChiSearch { get; set; }
        public string DiaChiSearch2 { get; set; }
    }

    public class DonThuocChiTietGridVoItem : GridItem
    {
        public int? STT { get; set; }
        public long? DuocPhamId { get; set; }
        public long? YeuCauKhamBenhDonThuocId { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public string MaHoatChat { get; set; }
        public string Ma { get; set; }
        public bool? LaDuocPhamBenhVien { get; set; }
        public string Ten { get; set; }
        public string HoatChat { get; set; }
        public string HamLuong { get; set; }
        public string DVT { get; set; }
        public string SangDisplay { get; set; }
        public string TruaDisplay { get; set; }
        public string ChieuDisplay { get; set; }
        public string ToiDisplay { get; set; }
        public double? LieuDungTrenNgay { get; set; }
        public double? SoLanTrenVien { get; set; }
        public string LieuDungTrenNgayDisplay => LieuDungTrenNgay.FloatToStringFraction();
        public string SoLanTrenVienDisplay => SoLanTrenVien.FloatToStringFraction();
        public int? SoNgayDung { get; set; }
        public int? ThoiGianDungSang { get; set; }
        public string ThoiGianDungSangDisplay { get; set; }
        public string ThoiGianDungTruaDisplay { get; set; }
        public string ThoiGianDungChieuDisplay { get; set; }
        public string ThoiGianDungToiDisplay { get; set; }
        public int? ThoiGianDungTrua { get; set; }
        public int? ThoiGianDungChieu { get; set; }
        public int? ThoiGianDungToi { get; set; }
        public double? SoLuong { get; set; }
        public string SoLuongDisplay { get; set; }
        public string TenDuongDung { get; set; }
        public decimal DonGia { get; set; } //=> CalculateHelper.TinhDonGiaBan(DonGiaNhap, TiLeTheoThapGia, VAT);
        //BVHD-3959
        public int DuongDungNumber => BenhVienHelper.GetSoThuThuocTheoDuongDung(DuongDungId);
        public long DuongDungId { get; set; }
        //z.DuongDungId != 12
        //                                                                         && z.DuongDungId != 1
        //                                                                         && z.DuongDungId != 26
        //                                                                         && z.DuongDungId != 22


        public decimal ThanhTien => DonGia * (decimal)SoLuong.GetValueOrDefault();
        public decimal DonGiaNhap { get; set; }
        public int TiLeTheoThapGia { get; set; }
        public int VAT { get; set; }
        public string ThuocBHYT { get; set; }
        public string TuongTacThuoc { get; set; }
        public string DiUngThuocDisplay { get; set; }
        public long? BenhNhanId { get; set; }
        public string GhiChu { get; set; }
        public string GhiChuDonThuoc { get; set; }
        public int NhomId { get; set; }
        public string Nhom { get; set; }
        public Enums.EnumLoaiDonThuoc LoaiDonThuoc { get; set; }
        public LoaiThuocTheoQuanLy? LoaiThuocTheoQuanLy { get; set; }

        //BVHD-3905
        public string TiLeThanhToanBHYT { get; set; }
        public string TooltipTiLeBHYT => TiLeThanhToanBHYT?.Replace("\n", "<br>");
    }
    public class MaHoatChatGridVo : GridItem
    {
        public string Ten1 { get; set; }
        public string Ten2 { get; set; }
        public string MaHoatChat1 { get; set; }
        public string MaHoatChat2 { get; set; }
    }


    public class GetDuocPhamTonKhoGridVoItem : GridItem
    {
        public long DVTId { get; set; }
        public string TenDonViTinh { get; set; }
        public long DuongDungId { get; set; }
        public string TenDuongDung { get; set; }
        public string TuongTacThuoc { get; set; }
        public string DiUngThuoc => FlagDiUng == true ? "Có" : "Không";
        public string ThuocDaKe => FlagThuocDaKe == true ? "Có" : "Không";
        public string DichDaKe => FlagDichDaKe == true ? "Có" : "Không";
        public double TonKho { get; set; }
        public string TonKhoFormat => TonKho == 0 ? "0" : TonKho.ApplyNumber();
        public bool? FlagDiUng { get; set; }
        public bool? FlagDichDaKe { get; set; }
        public bool? FlagTuongTac { get; set; }
        public bool? FlagThuocDaKe { get; set; }
        public DateTime? HanSuDung { get; set; }
        public string HSD => HanSuDung?.ApplyFormatDate();
        public EnumMucDoDiUng MucDo { get; set; }
        public string MucDoDisplay => MucDo.GetDescription();
        public int? TheTich { get; set; }
        public long KhoId { get; set; }
        public bool? FlagThuocDaKeTrungKho { get; set; }
        public bool? FlagDichDaKeTrungKho { get; set; }
        public bool CoNhapKhoDuocPhamChiTiet { get; set; }
        public string HamLuong { get; set; }
        public string NhaSanXuat { get; set; }
        public string DuongDung { get; set; }

    }

    public class ThongTinThuocVo
    {
        public long YeuCauKhamBenhId { get; set; }
        public long DuocPhamId { get; set; }
        public long LoaiDuocPham { get; set; }
    }

    public class CoDonThuocKhoLeKhoTong
    {
        public bool CoDonThuocKhoLe { get; set; }
        public bool CoDonThuocKhoTong { get; set; }
    }
    public class ThongTinVatTuVo
    {
        public long YeuCauKhamBenhId { get; set; }
        public long VatTuId { get; set; }
    }
    public class VatTuTrongKhoVo : GridItem
    {
        public LoaiDuocPhamHoacVatTu LoaiDuocPhamHoacVatTu { get; set; }
        public string DonViTinh { get; set; }
        public double TonKho { get; set; }
        public string TonKhoFormat => TonKho.ApplyNumber();
        public bool? FlagVatTuDaKe { get; set; }
        public DateTime? HanSuDung { get; set; }
        public string HSD => HanSuDung?.ApplyFormatDate();

    }

    public class DuocPhamTemplate
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string Ten { get; set; }
        public string HoatChat { get; set; }
        public string DVT { get; set; }
        public double SLTon { get; set; }
        public string HanSuDung { get; set; }
        public string HamLuong { get; set; }
        public string NhaSanXuat { get; set; }
        public string DuongDung { get; set; }
    }
    public class DuocPhamVaVatTuTemplate
    {
        public long KeyId { get; set; }
        public LoaiDuocPhamHoacVatTu LoaiDuocPhamHoacVatTu { get; set; }
        public string DisplayName { get; set; }
        public string Ten { get; set; }
        public string HoatChat { get; set; }
        public string DVT { get; set; }
        public double SLTon { get; set; }
        public string SLTonFormat => SLTon.ApplyNumber();
        public string HanSuDung { get; set; }
        public string TenLoaiDuocPhamHoacVatTu { get; set; }
        public string HamLuong { get; set; }
        public string NhaSanXuat { get; set; }
        public string DuongDung { get; set; }
        public LoaiThuocTheoQuanLy? LoaiThuocTheoQuanLy { get; set; }

    }

    public class DataBenhNhan
    {
        public string MaTN { get; set; }
        public string MaBN { get; set; }
        public string SoDienThoai { get; set; }
        public long Id { get; set; }
        public string NguoiGiamHo { get; set; }
        public string HoTen { get; set; }
        public int? NamSinh { get; set; }
        public string NamSinhDayDu { get; set; }
        public int? Tuoi { get; set; }
        public int? SoThang { get; set; }
        public string CanNang { get; set; }
        public string DiaChi { get; set; }
        public string BHYTMaSoThe { get; set; }
        public string ChuanDoan { get; set; }
        public DateTime? BHYTNgayHieuLuc { get; set; }
        public DateTime? BHYTNgayHetHan { get; set; }
        public string TenBenhVien { get; set; }
        public string BHYTMaDKBD { get; set; }
        public string GioiTinh { get; set; }
        public string CMND { get; set; }
        public string LoiDan { get; set; }
        public string GhiChu { get; set; }
        public string CachDung { get; set; }
        public bool? LaTreEm => SoThang < 72;

    }
    public class ThoiGianDungThuoc
    {
        public string ThoiGianDungSangDisplay { get; set; }
        public string ThoiGianDungTruaDisplay { get; set; }
        public string ThoiGianDungChieuDisplay { get; set; }
        public string ThoiGianDungToiDisplay { get; set; }
    }

    public class KiemTraThanhToan
    {
        public bool? CoThanhToan { get; set; }
        public bool? CoKhamBenh { get; set; }
    }

    public class KiemTraCoBHYTDuocHuongBaoHiem
    {
        public bool? CoBHYT { get; set; }
        public bool? DuocHuongBaoHiem { get; set; }
    }

    public class DataYCKBDonThuoc
    {
        public string KhoaPhong { get; set; }
        public string Header { get; set; }
        public string TemplateDonThuoc { get; set; }
        public string BarCodeImgBase64 { get; set; }
        public string MaTN { get; set; }
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public string HoTenBoMeHoacNguoiGiamHo { get; set; }
        public int? NamSinh { get; set; }
        public string NamSinhDayDu { get; set; }

        public string SoDienThoai { get; set; }
        public string CMND { get; set; }
        public int? Tuoi { get; set; }
        public int? SoThang { get; set; }
        public string CanNang { get; set; }
        public string NguoiGiamHo { get; set; }
        public string GioiTinh { get; set; }
        public string SoTheBHYT { get; set; }
        public string HanSuDung { get; set; }
        public string NoiDangKy { get; set; }
        public string ChuanDoan { get; set; }
        public string LoiDan { get; set; }
        public string Ngay { get; set; }
        public string Thang { get; set; }
        public string Nam { get; set; }
        public string BacSiKham { get; set; }
        public string LogoUrl { get; set; }
        public int STT { get; set; }
        public int CongKhoan { get; set; }
        public string TenDuocPham { get; set; }
        public string TenHoatChat { get; set; }
        public string Sang { get; set; }
        public string Trua { get; set; }
        public string Chieu { get; set; }
        public string Toi { get; set; }
        public string TenDuongDung { get; set; }
        public int? SoNgayDung { get; set; }
        public double? SoLuong { get; set; }
        public string CachDung { get; set; }
        public string DiaChi { get; set; }
        public string NgayHieuLuc { get; set; }
        public string NgayHetHan { get; set; }
        //public string NgayThangHientai => DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + " ngày " +
        //                                  DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year;
        public DateTime? ThoiDiemKeDon { get; set; }
        public string NgayThangHientai => "Ngày " + ThoiDiemKeDon?.Day + " tháng " + ThoiDiemKeDon?.Month + " năm " + ThoiDiemKeDon?.Year;
        public string NgayThangNam => DateTime.Now.ApplyFormatDateTimeSACH();

    }

    public class DataYCKBVatTu
    {
        public string KhoaPhong { get; set; }
        public string Header { get; set; }
        public string TemplateVatTu { get; set; }
        public string BarCodeImgBase64 { get; set; }
        public string MaTN { get; set; }
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public string HoTenBoMeHoacNguoiGiamHo { get; set; }
        public int? NamSinh { get; set; }
        public string CMND { get; set; }
        public int? Tuoi { get; set; }
        public string CanNang { get; set; }
        public string NguoiGiamHo { get; set; }
        public string GioiTinh { get; set; }
        public string SoTheBHYT { get; set; }
        public string NoiDangKy { get; set; }
        public string Ngay { get; set; }
        public string Thang { get; set; }
        public string Nam { get; set; }
        public string BacSiKham { get; set; }
        public string LogoUrl { get; set; }
        public string DiaChi { get; set; }
        public string NgayHieuLuc { get; set; }
        public string NgayHetHan { get; set; }
        public string ChuanDoan { get; set; }
        public string SoDienThoai { get; set; }
        public int? CongKhoan { get; set; }
        //public string ThoiGianKeDon { get; set; }
        //public string NgayThangHientai => ThoiGianKeDon + " ngày " +
        //                                 Ngay + " tháng " + Thang + " năm " + Nam;
        public string NamSinhDayDu { get; set; }

        public DateTime? ThoiDiemKeDon { get; set; }
        public string NgayThangHientai => "Ngày " + ThoiDiemKeDon?.Day + " tháng " + ThoiDiemKeDon?.Month + " năm " + ThoiDiemKeDon?.Year;
    }


    public class ToaThuocMauGridVo : GridItem
    {
        public string TenToaMau { get; set; }
        public long? TrieuChungId { get; set; }
        public long? ICDId { get; set; }
        public long? ChuanDoanId { get; set; }
        public long? BacSiKeToaId { get; set; }
        public string BacSiKeToa { get; set; }
        public string ChuanDoanICD { get; set; }
        public int? SuDung { get; set; }
        public bool? IsDisabled { get; set; }
        public string GhiChu { get; set; }
    }

    public class ToaThuocMauChiTietGridVo : GridItem
    {
        public int? STT { get; set; }
        public long? ToaThuocMauId { get; set; }
        public long? DuocPhamId { get; set; }
        public long? DonViTinhId { get; set; }
        public long? DuongDungId { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }
        public string HoatChat { get; set; }
        public string TenDuongDung { get; set; }
        public string DVT { get; set; }
        public double? SoLuong { get; set; }
        public string SoLuongDisplay { get; set; }
        public int? SoNgayDung { get; set; }
        public string SangDisplay { get; set; }
        public string TruaDisplay { get; set; }
        public string ChieuDisplay { get; set; }
        public string ToiDisplay { get; set; }
        public string ThoiGianDungSangDisplay { get; set; }
        public string ThoiGianDungTruaDisplay { get; set; }
        public string ThoiGianDungChieuDisplay { get; set; }
        public string ThoiGianDungToiDisplay { get; set; }
        public string GhiChu { get; set; }
        public string NhomToaMau { get; set; }

    }
    public class LichSuKeToaGridVo : GridItem
    {
        public string BacSiKham { get; set; }
        public string ChuanDoanICD { get; set; }
        public string ThoiDiemHoanThanh { get; set; }
        public string MaTN { get; set; }
    }

    public class DonThuocBacSiKhacGridVo : GridItem
    {
        public string BacSiKham { get; set; }
        public string ChuanDoanICD { get; set; }
        public string ThoiDiemHoanThanhDisplay { get; set; }
        public DateTime? ThoiDiemHoanThanh { get; set; }

        public string MaTN { get; set; }
    }

    public class LichSuKeToaChildGridVo : GridItem
    {
        public int? STT { get; set; }
        public long? DuocPhamId { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }
        public string HoatChat { get; set; }
        public string TenDuongDung { get; set; }
        public string DVT { get; set; }
        public double? SoLuong { get; set; }
        public string SoLuongDisplay { get; set; }
        public int? SoNgayDung { get; set; }
        public string SangDisplay { get; set; }
        public string TruaDisplay { get; set; }
        public string ChieuDisplay { get; set; }
        public string ToiDisplay { get; set; }
        public string GhiChu { get; set; }
        public string NhomLSKT { get; set; }
        public Enums.EnumLoaiDonThuoc LoaiDonThuoc { get; set; }
    }
    public class ThongTinYeuCauKhamVo
    {
        public long? BenhNhanId { get; set; }
        public long? YeuCauTiepNhanId { get; set; }
        public long? YeuCauKhamBenhTruocId { get; set; }
    }

    public class ThongTinBenhNhanVo
    {
        public long YeuCauTiepNhanId { get; set; }
        public long? YeuCauKhamBenhTruocId { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public string MaBN { get; set; }
        public long? BenhNhanId { get; set; }
        public string HoTen { get; set; }
        public string TenGioiTinh { get; set; }
        public int? Tuoi { get; set; }
        public string SoDienThoai { get; set; }
        public string DanToc { get; set; }
        public string DiaChi { get; set; }
        public string NgheNghiep { get; set; }
        public string TuyenKham { get; set; }
        public int? BHYTMucHuong { get; set; }
        public string TenLyDoTiepNhan { get; set; }
        public string TenLyDoKhamBenh { get; set; }
        public string ThoiDiemTiepNhanDisplay { get; set; }
        public int? NamSinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NgaySinh { get; set; }
        public string TuoiThoiDiemHienTai { get; set; }
        public string SoBHYT { get; set; }
        public bool? CoBHYT { get; set; }
        public DateTime? BHYTNgayHieuLuc { get; set; }
        public DateTime? BHYTNgayHetHan { get; set; }
        public string BHYTThoiGianHieuLucDisplay => BHYTNgayHieuLuc?.ApplyFormatDate() + (BHYTNgayHieuLuc != null && BHYTNgayHetHan != null ? " - " : "") + BHYTNgayHetHan?.ApplyFormatDate();
        public bool IsBHYTHetHan => CoBHYT == true && BHYTNgayHieuLuc != null && BHYTNgayHetHan != null && (DateTime.Now.Date < BHYTNgayHieuLuc.Value.Date || DateTime.Now.Date > BHYTNgayHetHan.Value.Date);
        public EnumLoaiYeuCauTiepNhan LoaiYeuCauTiepNhan { get; set; }
        public string TenCongTy { get; set; }

        //BVHD-3800
        public bool? LaCapCuu { get; set; }

        //BVHD-3941
        public bool? CoBaoHiemTuNhan { get; set; }
        public string TenCongTyBaoHiemTuNhan { get; set; }
    }

    public class MucDoDiUngThuocVo
    {
        public long YeuCauKhamBenhId { get; set; }
        public bool? flagDiUngThuoc { get; set; }
        public long? DuocPhamId { get; set; }
    }
    public class ChanDoanBacSiKhacVo : GridItem
    {
        public long? DichVuKhamBenhBenhVienId { get; set; }
        public long? BacSiKhamBenhId { get; set; }
        public string TenDichVu { get; set; }
        public string TenBacSiKham { get; set; }
        public string KetQuaCLS { get; set; }
        public long? ICDChinhId { get; set; }
        public string TenICDChinh { get; set; }
        public string GhiChuICDChinh { get; set; }
        public string CachGiaiQuyet { get; set; }
        public ICollection<ICDKhacDetail> YeuCauKhamBenhICDKhacs { get; set; }
    }

    public class ICDKhacDetail : GridItem
    {
        public string TenICDKhac { get; set; }
        public string GhiChu { get; set; }

    }
    public class ChanDoanBacSiKhacDetailVo : GridItem
    {
        public string KetQuaCLS { get; set; }
        public long? ICDChinhId { get; set; }
        public string TenICDChinh { get; set; }
        public string GhiChuICDChinh { get; set; }
        public string CachGiaiQuyet { get; set; }
        public ICollection<YeuCauKhamBenhICDKhac> YeuCauKhamBenhICDKhacs { get; set; }
    }

    public class YeuCauKhamBenhXoaDonThuoc
    {
        public long YeuCauKhamBenhId { get; set; }
        public bool IsKhamBenhDangKham { get; set; }
    }

    public class KiemTraXoaDonThuocThanhToan
    {
        public long YeuCauKhamBenhId { get; set; }
        public long? DonThuocChiTietId { get; set; }
        public bool IsKhamBenhDangKham { get; set; }
    }

    public class KiemTraXoaVatTuThanhToan
    {
        public long YeuCauKhamBenhId { get; set; }
        public long? YeuCauKhamBenhDonVTYTChiTietId { get; set; }
        public bool IsKhamBenhDangKham { get; set; }
    }

    public class InToaThuocKhamBenhDanhSach
    {
        public long YeuCauTiepNhanId { get; set; }
        public long YeuCauKhamBenhId { get; set; }
        public bool ThuocBHYT { get; set; }
        public bool ThuocKhongBHYT { get; set; }
        public bool VatTu { get; set; }
        public string HostingName { get; set; }
        public bool IsGreaterThan6Old { get; set; }
    }

    public class InToaThuocReOrder
    {
        public InToaThuocReOrder()
        {
            ListGridThuoc = new List<DonThuocChiTietGridVoItem>();
            YeuCauKhamBenhICDKhacs = new List<InToaThuocICDKemTheoChiTiet>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public long YeuCauKhamBenhId { get; set; }
        public string TenICDChinh { get; set; }
        public string GhiChuICDChinh { get; set; }
        public string ChanDoan => TenICDChinh + (!string.IsNullOrEmpty(GhiChuICDChinh) ? " (" + GhiChuICDChinh + ")" : "");
        public string HostingName { get; set; }
        public bool Header { get; set; }
        public bool? IsChangeChanDoan { get; set; }
        public List<DonThuocChiTietGridVoItem> ListGridThuoc { get; set; }
        public List<InToaThuocICDKemTheoChiTiet> YeuCauKhamBenhICDKhacs { get; set; }
    }

    public class InToaThuocICDKemTheoChiTiet
    {
        public string TenICD { get; set; }
        public string GhiChu { get; set; }
        public string ChanDoanKemTheo => TenICD + (!string.IsNullOrEmpty(GhiChu) ? " (" + GhiChu + ")" : "");
    }
    public class InVatTuReOrder
    {
        public InVatTuReOrder()
        {
            ListGridVatTu = new List<VatTuYTGridVo>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public long YeuCauKhamBenhId { get; set; }
        public string HostingName { get; set; }
        public bool Header { get; set; }
        public List<VatTuYTGridVo> ListGridVatTu { get; set; }
    }

    public class VatTuYTGridVo : GridItem
    {
        public int? STT { get; set; }
        public string Ten { get; set; }
        public string DonViTinh { get; set; }
        public double? SoLuong { get; set; }
        public string GhiChu { get; set; }
        public decimal DonGiaNhap { get; set; }
        public decimal DonGia { get; set; } //=> CalculateHelper.TinhDonGiaBan(DonGiaNhap, TiLeTheoThapGia, VAT);
        public decimal ThanhTienVatTu => DonGia * (decimal)SoLuong.GetValueOrDefault();
        public int TiLeTheoThapGia { get; set; }
        public int VAT { get; set; }
        public long YeuCauKhamBenhId { get; set; }
        public long VatTuBenhVienId { get; set; }
        public string NhaSX { get; set; }
        public string NuocSX { get; set; }
        public string Ma { get; set; }
        public int NhomId = 1;
        public string Nhom = "Vật Tư";

        //BVHD-3905
        public string TiLeThanhToanBHYT { get; set; }
        public string TooltipTiLeBHYT => TiLeThanhToanBHYT?.Replace("\n", "<br>");
    }

    public class InputStringStoredVo
    {
        public InputStringStoredKey Loai { get; set; }
        public string GhiChu { get; set; }
    }

    public class TuVanThuocGridVoItem : GridItem
    {
        public int? STT { get; set; }
        public long? DuocPhamId { get; set; }
        public long? YeuCauTiepNhanId { get; set; }
        public string MaHoatChat { get; set; }
        public string Ma { get; set; }
        public bool? LaDuocPhamBenhVien { get; set; }
        public string Ten { get; set; }
        public string HoatChat { get; set; }
        public string DVT { get; set; }
        public string SangDisplay { get; set; }
        public string TruaDisplay { get; set; }
        public string ChieuDisplay { get; set; }
        public string ToiDisplay { get; set; }
        public int? SoNgayDung { get; set; }
        public int? ThoiGianDungSang { get; set; }
        public string ThoiGianDungSangDisplay { get; set; }
        public string ThoiGianDungTruaDisplay { get; set; }
        public string ThoiGianDungChieuDisplay { get; set; }
        public string ThoiGianDungToiDisplay { get; set; }
        public int? ThoiGianDungTrua { get; set; }
        public int? ThoiGianDungChieu { get; set; }
        public int? ThoiGianDungToi { get; set; }
        public double? SoLuong { get; set; }
        public string SoLuongDisplay { get; set; }
        public string TenDuongDung { get; set; }
        public string TuongTacThuoc { get; set; }
        public string DiUngThuocDisplay { get; set; }
        public string GhiChu { get; set; }
    }

    public class DuocPhamTuVanTemplate
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string Ten { get; set; }
        public double? SLTon { get; set; }
        public string SLTonFormat => SLTon?.ApplyNumber();
        public DateTime? HanSuDung { get; set; }
        public string HSD => HanSuDung?.ApplyFormatDate();
        public bool LaDuocPhamBenhVien { get; set; }
        public bool CoNhapKhoDuocPhamChiTiet { get; set; }
        public int? Rank { get; set; }
    }

    public class ThongTinThuocTuVanVo
    {
        public long YeuCauTiepNhanId { get; set; }
        public long DuocPhamId { get; set; }
    }

    public class InTuVanThuoc
    {
        public long YeuCauTiepNhanId { get; set; }
        public long YeuCauKhamBenhId { get; set; }
        public string HostingName { get; set; }
    }

    public class InNoiTruPhieuDieuTriTuVanThuoc
    {
        public long YeuCauTiepNhanId { get; set; }
        public long NoiTruPhieuDieuTriId { get; set; }
        public string HostingName { get; set; }
    }

    public class InNoiTruPhieuDieuTriThuocRaVienICDKemTheo
    {
        public InNoiTruPhieuDieuTriThuocRaVienICDKemTheo()
        {
            ChuanDoanKemTheos = new List<ThongTinChuanDoanKemTheo>();
        }
        public List<ThongTinChuanDoanKemTheo> ChuanDoanKemTheos { get; set; }

    }

    public class NoiTruPhieuDieuTriTuVanThuocData
    {
        public string KhoaPhong { get; set; }
        public string Header { get; set; }
        public string TemplateDonThuoc { get; set; }
        public string BarCodeImgBase64 { get; set; }
        public string MaTN { get; set; }
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public int? NamSinh { get; set; }
        public string SoDienThoai { get; set; }
        public string CMND { get; set; }
        public int? Tuoi { get; set; }
        public int? SoThang { get; set; }
        public string GioiTinh { get; set; }
        public string LoiDan { get; set; }
        public string BacSiChiDinh { get; set; }
        public string LogoUrl { get; set; }
        public int STT { get; set; }
        public int CongKhoan { get; set; }
        public string TenDuocPham { get; set; }
        public string TenHoatChat { get; set; }
        public string Sang { get; set; }
        public string Trua { get; set; }
        public string Chieu { get; set; }
        public string Toi { get; set; }
        public string TenDuongDung { get; set; }
        public int? SoNgayDung { get; set; }
        public double? SoLuong { get; set; }
        public string CachDung { get; set; }
        public string DiaChi { get; set; }
        public string NgayThangHientai => DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + " ngày " +
                                          DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year;

    }
    public class DichVuKhuyenMaiGridVo : GridItem
    {
        public string Ten { get; set; }
        public long ChuongTrinhGoiDichVuId { get; set; }
        public long? BenhNhanId { get; set; }
    }

    public class PhieuKhamBenhTheoYeuCauTiepNhanVo
    {
        public long YeuCauTiepNhanId { get; set; }
        public bool CoHeader { get; set; }
    }

    public class KiemTraThuocTrungBSKe
    {
        public long YeuCauTiepNhanId { get; set; }
        public long DuocPhamId { get; set; }
    }
    public class InfoYeuCauKhamBenhVo : GridItem
    {
        public DateTime? NghiHuongBHXHTuNgay { get; set; }
        public DateTime? NghiHuongBHXHDenNgay { get; set; }

        public long? ICDChinhNghiHuongBHYT { get; set; }
        public string TenICDChinhNghiHuongBHYT { get; set; }
        public string PhuongPhapDieuTriNghiHuongBHYT { get; set; }

    }
}
