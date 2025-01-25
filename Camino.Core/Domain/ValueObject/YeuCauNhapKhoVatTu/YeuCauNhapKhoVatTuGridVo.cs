using Camino.Core.Helpers;
using System;

namespace Camino.Core.Domain.ValueObject.YeuCauNhapKhoVatTu
{
    public class YeuCauNhapKhoVatTuGridVo
    {
    }

    public class YeuCauNhapKhoVatTuChiTietGridVo
    {
        public int? TiLeBHYTThanhToan { get; set; }

        public double IdView { get; set; }
        public long Id { get; set; }
        public long? HopDongThauVatTuId { get; set; }
        public long? NhaThauId { get; set; }
        public long? VatTuBenhVienId { get; set; }
        public bool? LaVatTuBHYT { get; set; }
        //public long? VatTuBenhVienPhanNhomId { get; set; }
        public Enums.LoaiSuDung? LoaiSuDung { get; set; }
        public string LoaiSuDungDisplay { get; set; }
        public string Solo { get; set; }
        public DateTime? HanSuDung { get; set; }
        public string MaVach { get; set; }
        public int? SoLuongNhap { get; set; }
        public double? DonGiaNhap { get; set; }
        public int? VAT { get; set; }
        public long? KhoViTriId { get; set; }
        public int? LoaiNhap { get; set; } // 1 là hdt, 2 là ncc
        public string NhaThauDisplay { get; set; }

        //for grid
        public string HopDongThauDisplay { get; set; }
        public string VatTuDisplay { get; set; }
        public string LoaiDisplay { get; set; }
        public string NhomDisplay { get; set; }
        public string HanSuDungDisplay { get; set; }
        public string SoLuongNhapDisplay { get; set; }
        public string ViTriDisplay { get; set; }
        public string DVT { get; set; }
        public string MaRef { get; set; }
        public long? KhoNhapSauKhiDuyetId { get; set; }
        public long? NguoiNhapSauKhiDuyetId { get; set; }
        public string TenKhoNhapSauKhiDuyet { get; set; }
        public string TenNguoiNhapSauKhiDuyet { get; set; }
        public decimal? ThanhTienTruocVat { get; set; }
        public decimal? ThanhTienSauVat { get; set; }
        public decimal? ThueVatLamTron { get; set; }
        public string GhiChu { get; set; }

    }
    public class InPhieuNhapKhoVatTu
    {
        public long YeuCauNhapKhoVatTuId { get; set; }
        public bool CoTheoBenhVien { get; set; }
        public bool CoTheoThongTu { get; set; }
        public string HostingName { get; set; }

    }
    public class YeuCauNhapKhoVatTuChiTietData
    {
        public string Ten { get; set; }
        public string TenNguoiNhap { get; set; }
        public bool LaVatTuBHYT { get; set; }
        public string DVT { get; set; }
        public string SoLo { get; set; }
        public DateTime HanSuDung { get; set; }
        public string HanSuDungDisplay => HanSuDung.ApplyFormatDate();
        public double SLTheoChungTu { get; set; }
        public double SLThucNhap => SLTheoChungTu;
        public decimal DonGia { get; set; }
        public decimal ThanhTienTruocVAT { get; set; }
        public decimal ThanhTienSauVAT { get; set; }
        public int VAT { get; set; }
        public DateTime NgayNhap { get; set; }
        public string NgayNhapDisplay => NgayNhap.ApplyFormatNgayThangNam();
        public string NCC { get; set; }
        public string TheoSoHoaDon { get; set; }
        public DateTime? NgayHoaDon { get; set; }
        public string NgayHoaDonDisplay => NgayHoaDon?.ApplyFormatDate();
        public string KhoNhap { get; set; }
        public long? KhoNhapSauDuyetId { get; set; }
        public string NguoiNhan { get; set; }
        public string SoPhieu { get; set; }
        public decimal? ThueVatLamTron { get; set; }
        public string KyHieuHoaDon { get; set; }
        public string KyHieuHoaDonHienThi => TheoSoHoaDon + "/" + KyHieuHoaDon;
    }

    public class YeuCauNhapKhoVatTuData
    {
        public string NgayNhapKho { get; set; }
        public string LoaiBHYT { get; set; }
        public decimal TienHangDecimal { get; set; }
        public string TienHang => TienHangDecimal.ApplyFormatMoneyVND().Replace(" ₫", "");
        public string DuocPhamHoacVatTus { get; set; }
        public string VAT { get; set; }
        public decimal ThueVATDecimal { get; set; }
        public string ThueVAT => ThueVATDecimal.ApplyFormatMoneyVND().Replace(" ₫", "");
        public string ChietKhau { get; set; }
        public decimal GiaTriThanhToanDecimal => TienHangDecimal + ThueVATDecimal;
        public string GiaTriThanhToan => GiaTriThanhToanDecimal.ApplyFormatMoneyVND().Replace(" ₫", "");
        public string TongTienChu => ConvertNumberToStringCurrencyHelper.ApplytNumberToCurrencyString(GiaTriThanhToanDecimal);
        public string NguoiLap { get; set; }
        public string NguoiGiaoHang { get; set; }
        public string ThuKho { get; set; }
        public string KeToanKho { get; set; }
        public string KeToanTruong { get; set; }
        public string NCC { get; set; }
        public string SoHoaDon { get; set; }
        public string NgayHoaDon { get; set; }
        public string KhoNhap { get; set; }
        public string NguoiNhan { get; set; }
        public string SoPhieu { get; set; }
    }

    public class YeuCauNhapKhoVatTuChiTietDataThongTu
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenNguoiNhap { get; set; }
        public string DVT { get; set; }
        public string NhaSX { get; set; }
        public string NuocSX { get; set; }
        public double SLTheoChungTu { get; set; }
        public double SLThucNhap => SLTheoChungTu;
        public decimal DonGia { get; set; }
        public decimal ThanhTien => DonGia * (decimal)SLTheoChungTu;
        public DateTime NgayNhap { get; set; }
        public string NgayNhapDisplay => NgayNhap.ApplyFormatNgayThangNam();
        public string NCC { get; set; }
        public string TheoSoHoaDon { get; set; }
        public string KhoNhap { get; set; }
        public long? KhoNhapSauDuyetId { get; set; }
        public string NguoiNhap { get; set; }
        public string SoPhieu { get; set; }
        public string DiaChiBoPhan { get; set; }
        public string KhoaPhong { get; set; }
        public decimal ThanhTienTruocVAT { get; set; }
        public string KyHieuHoaDon { get; set; }
        public string KyHieuHoaDonHienThi => TheoSoHoaDon + "/" + KyHieuHoaDon;

        //BVHD-3857
        public string DonViGiaoHang { get; set; }
        public string SoHoaDonPhieuNhap { get; set; }
        public DateTime? NgayHoaDon { get; set; }
        public string NgayHoaDonPhieuNhap => NgayHoaDon?.ApplyFormatDate();
    }

    public class YeuCauNhapKhoVatTuDataThongTu
    {
        public string LogoUrl { get; set; }
        public string NgayNhapKho { get; set; }
        public string DuocPhamHoacVatTus { get; set; }
        public decimal ThanhTien { get; set; }
        public string TongTienChu => ConvertNumberToStringCurrencyHelper.ApplytNumberToCurrencyString(ThanhTien);
        public string SoHoaDon { get; set; }
        public string KhoNhap { get; set; }
        public string NguoiNhap { get; set; }
        public string SoPhieu { get; set; }
        public string DiaChiBoPhan { get; set; }
        public string KhoaPhong { get; set; }

        //BVHD-3857
        public string DonViGiaoHang { get; set; }
        public string SoHoaDonPhieuNhap { get; set; }
        public DateTime? NgayHoaDon { get; set; }
        public string NgayHoaDonPhieuNhap => NgayHoaDon?.ApplyFormatDate();
    }
}
