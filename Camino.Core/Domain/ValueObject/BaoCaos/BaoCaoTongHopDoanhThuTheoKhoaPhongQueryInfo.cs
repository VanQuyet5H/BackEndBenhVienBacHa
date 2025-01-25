using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Helpers;
using System.Linq;
using Camino.Core.Domain.ValueObject.NoiTruBenhAn;
using Newtonsoft.Json;

namespace Camino.Core.Domain.ValueObject.BaoCaos
{
    public class BaoCaoTongHopDoanhThuTheoKhoaPhongQueryInfo : QueryInfo
    {
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public DateTime? KySoSanhTuNgay { get; set; }
        public DateTime? KySoSanhDenNgay { get; set; }
        public bool LayTatCa { get; set; }
    }

    #region Báo cáo Người Bệnh Làm Xét Nghiệm
    public class BaoCaoBenhNhanLamXetNghiemQueryInfo : QueryInfo
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
    #endregion Báo cáo Người Bệnh Làm Xét Nghiệm
    #region Báo cáo Số Xét Nghiệm Sàng Lọc Hiv
    public class BaoCaoSoXetNghiemSangLocHivQueryInfo : QueryInfo
    {
        public long DichVuKyThuatBenhVienId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
    #endregion Báo cáo Số Xét Nghiệm Sàng Lọc Hiv
    #region Báo cáo Tổng Hợp Số Lượng Xét Nghiệm Theo Thời Gian
    public class BaoCaoTongHopSoLuongXetNghiemTheoThoiGianQueryInfo : QueryInfo
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
    #endregion Báo cáo Tổng Hợp Số Lượng Xét Nghiệm Theo Thời Gian
    #region BaoCaoXuatNhapTon
    public class BaoCaoXuatNhapTonQueryInfo : QueryInfo
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public long KhoId { get; set; }
        public long? DuocPhamBenhVienId { get; set; }
    }

    public class BaoCaoTheKhoQueryInfo : QueryInfo
    {
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public long KhoId { get; set; }
        public long DuocPhamHoacVatTuBenhVienId { get; set; }
        public Enums.LoaiDuocPhamHoacVatTu LoaiDuocPhamHoacVatTu { get; set; }
        public string HostingName { get; set; }

    }

    public class BaoCaoTheKhoAdditionalSearchString
    {
        public long KhoId { get; set; }
        public long DuocPhamHoacVatTuBenhVienId { get; set; }
        public Enums.LoaiDuocPhamHoacVatTu LoaiDuocPhamHoacVatTu { get; set; }
        public DateTime NgayThang { get; set; }

    }

    public class BaoCaoXuatNhapTonGridVo : GridItem
    {
        public string Ten { get; set; }
        public string HamLuong { get; set; }
        public string NuocSanXuat { get; set; }
        public string SoLo { get; set; }
        public string DVT { get; set; }
        public double? SLTonDauKy { get; set; }
        public decimal? DonGiaTonDauKy { get; set; }
        public decimal? ThanhTienTonDauKy => (decimal)(SLTonDauKy ?? 0) * (DonGiaTonDauKy ?? 0);
        public double? SLNhapTrongKy { get; set; }
        public decimal? DonGiaNhapTrongKy { get; set; }
        public decimal? ThanhTienNhapTrongKy => (decimal)(SLNhapTrongKy ?? 0) * (DonGiaNhapTrongKy ?? 0);
        public double? SLXuatTrongKy { get; set; }
        public decimal? DonGiaXuatTrongKy { get; set; }
        public decimal? ThanhTienXuatTrongKy => (decimal)(SLXuatTrongKy ?? 0) * (DonGiaXuatTrongKy ?? 0);
        public double? SLTonCuoiKy { get; set; }
        public decimal? DonGiaTonCuoiKy { get; set; }
        public decimal? ThanhTienTonCuoiKy => (decimal)(SLTonCuoiKy ?? 0) * (DonGiaTonCuoiKy ?? 0);
        public string Nhom { get; set; }
        public string Loai { get; set; }
        public long? DuocPhamBenhVienPhanNhomId { get; set; }
    }
    public class BaoCaoChiTietXuatNhapTonGridVo : GridItem
    {
        public long DuocPhamBenhVienId { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string HamLuong { get; set; }
        public string HoatChat { get; set; }
        public string SoLo { get; set; }
        public string DVT { get; set; }
        public decimal DonGiaNhap { get; set; }
        public int VAT { get; set; }
        public decimal DonGiaNhapSauVAT => DonGiaNhap + (DonGiaNhap * VAT / 100);
        public double? SLNhap { get; set; }
        public double? SLXuat { get; set; }
        public DateTime NgayNhapXuat { get; set; }
        public string Nhom { get; set; }
        public long? NhomId { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public long? DuocPhamBenhVienPhanNhomId { get; set; }
    }

    public class BaoCaoXuatNhapTonGroupGridVo : GridItem
    {
        public string NhomBHYT { get; set; }
        public string TenNhom { get; set; }
        public long? NhomDuocPhamVatTuId { get; set; }
        public decimal? ThanhTienTonDauKy { get; set; }
        public decimal? ThanhTienNhapTrongKy { get; set; }
        public decimal? ThanhTienXuatTrongKy { get; set; }
        public decimal? ThanhTienTonCuoiKy { get; set; }
        public decimal? DonGia { get; set; }
        public long KhoId { get; set; }
        public DateTime? TuNgay { get; set; }
        public string TuNgayFormat => TuNgay?.ApplyFormatDate();
        public DateTime? DenNgay { get; set; }
        public string DenNgayFormat => DenNgay?.ApplyFormatDate();
    }

    public class LoaiGroupVo
    {
        public string Loai { get; set; }
    }
    public class NhomGroupVo
    {
        public string Loai { get; set; }
        public string Nhom { get; set; }
    }
    public class InBaoCaoXuatNhapTonVo
    {
        public long? KhoId { get; set; }
        public string HostingName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }

    public class InBaoCaoXuatNhapTonData
    {
        public string LogoUrl { get; set; }
        public string ThoiGian { get; set; }
        public string TenKho { get; set; }
        public string BaoCaoXuatNhapTon { get; set; }
    }
    #endregion
    #region BaoCaoXuatNhapTon vật tư
    public class BaoCaoXuatNhapTonVTQueryInfo : QueryInfo
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public long KhoId { get; set; }
        public long? DuocPhamBenhVienId { get; set; }
    }

    public class BaoCaoTheKhoVTQueryInfo : QueryInfo
    {
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public long KhoId { get; set; }
        public long DuocPhamHoacVatTuBenhVienId { get; set; }
        public Enums.LoaiDuocPhamHoacVatTu LoaiDuocPhamHoacVatTu { get; set; }
        public string HostingName { get; set; }

    }

    public class BaoCaoXuatNhapTonVTGridVo : GridItem
    {
        public string Ten { get; set; }
        public string HamLuong { get; set; }
        public string SoLo { get; set; }
        public string DVT { get; set; }
        public double? SLTonDauKy { get; set; }
        public decimal? DonGiaTonDauKy { get; set; }
        public decimal? ThanhTienTonDauKy => (decimal)(SLTonDauKy ?? 0) * (DonGiaTonDauKy ?? 0);
        public double? SLNhapTrongKy { get; set; }
        public decimal? DonGiaNhapTrongKy { get; set; }
        public decimal? ThanhTienNhapTrongKy => (decimal)(SLNhapTrongKy ?? 0) * (DonGiaNhapTrongKy ?? 0);
        public double? SLXuatTrongKy { get; set; }
        public decimal? DonGiaXuatTrongKy { get; set; }
        public decimal? ThanhTienXuatTrongKy => (decimal)(SLXuatTrongKy ?? 0) * (DonGiaXuatTrongKy ?? 0);
        public double? SLTonCuoiKy { get; set; }
        public decimal? DonGiaTonCuoiKy { get; set; }
        public decimal? ThanhTienTonCuoiKy => (decimal)(SLTonCuoiKy ?? 0) * (DonGiaTonCuoiKy ?? 0);
        public string Nhom { get; set; }
        public string Loai { get; set; }
    }
    public class BaoCaoChiTietXuatNhapTonVTGridVo : GridItem
    {
        public long VatTuBenhVienId { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string HamLuong { get; set; }
        public string NuocSanXuat { get; set; }
        public string SoLo { get; set; }
        public string DVT { get; set; }
        public decimal DonGiaNhap { get; set; }
        public int VAT { get; set; }
        public decimal DonGiaNhapSauVAT => DonGiaNhap + (DonGiaNhap * VAT / 100);
        public double? SLNhap { get; set; }
        public double? SLXuat { get; set; }
        public DateTime NgayNhapXuat { get; set; }
        public string Nhom { get; set; }
        public long NhomId { get; set; }
        public bool LaVatTuBHYT { get; set; }
    }

    public class BaoCaoXuatNhapTonVTGroupGridVo : GridItem
    {
        public string NhomBHYT { get; set; }
        public string TenNhom { get; set; }
        public long? NhomDuocPhamVatTuId { get; set; }
        public decimal? ThanhTienTonDauKy { get; set; }
        public decimal? ThanhTienNhapTrongKy { get; set; }
        public decimal? ThanhTienXuatTrongKy { get; set; }
        public decimal? ThanhTienTonCuoiKy { get; set; }
        public decimal? DonGia { get; set; }
        public long KhoId { get; set; }
        public DateTime? TuNgay { get; set; }
        public string TuNgayFormat => TuNgay?.ApplyFormatDate();
        public DateTime? DenNgay { get; set; }
        public string DenNgayFormat => DenNgay?.ApplyFormatDate();
    }

    public class LoaiGroupVoVT
    {
        public string Loai { get; set; }
    }
    public class NhomGroupVoVT
    {
        public string Loai { get; set; }
        public string Nhom { get; set; }
    }
    public class InBaoCaoXuatNhapTonVTVo
    {
        public long? KhoId { get; set; }
        public string HostingName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }

    public class InBaoCaoXuatNhapTonVTData
    {
        public string LogoUrl { get; set; }
        public string ThoiGian { get; set; }
        public string TenKho { get; set; }
        public string BaoCaoXuatNhapTon { get; set; }
    }
    #endregion

    #region BaoCaoTiepNhanBenhNhanKham
    public class BaoCaoTNBenhNhanKhamQueryInfo : QueryInfo
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }

    public class BaoCaoTNBenhNhanKhamGridVo : GridItem
    {
        public DateTime ThoiGianTiepNhan { get; set; }
        public string ThoiGianTiepNhanDisplay => ThoiGianTiepNhan.ApplyFormatDateTime();
        public string MaTN { get; set; }
        public string HoTenBN { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string NgaySinhDisplay => DateHelper.DOBFormat(NgaySinh, ThangSinh, NamSinh);
        public string GioiTinhDisplay { get; set; }
        public string BHTYMaSoThe { get; set; }
        public string DichVu { get; set; }
        public long? PhongKhamId { get; set; }
        public string TenPhongKham { get; set; }
        public string TrangThaiDisplay { get; set; }
        public string HinhThucDen { get; set; }
        public string NoiGioiThieu { get; set; }

        public Enums.EnumLoaiTiepNhanNguoiBenh Nhom => BaoCaoTNBenhNhanKhamThongTinYeuCauKhamBenhVos != null &&
                                                       BaoCaoTNBenhNhanKhamThongTinYeuCauKhamBenhVos.Any(a =>
                                                           a.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
            ? Enums.EnumLoaiTiepNhanNguoiBenh.NguoiBenhCoPhieuKham
            : Enums.EnumLoaiTiepNhanNguoiBenh.NguoiBenhChiLamDichVu;
        public string TenNhom => Nhom.GetDescription();

        public string ThoiGianBatDauTrongNgay => "07:00";
        public string ThoiGianKetThucTrongNgay => "16:45";
        public bool NgoaiGioHanhChinh => ThoiGianTiepNhan != null
                                         && (ThoiGianTiepNhan.TimeOfDay < System.TimeSpan.Parse(ThoiGianBatDauTrongNgay)
                                             || ThoiGianTiepNhan.TimeOfDay > System.TimeSpan.Parse(ThoiGianKetThucTrongNgay));
        public List<BaoCaoTNBenhNhanKhamThongTinYeuCauKhamBenhVo> BaoCaoTNBenhNhanKhamThongTinYeuCauKhamBenhVos { get; set; }
        public List<BaoCaoTNBenhNhanKhamThongTinYeuCauDichVuKyThuatVo> BaoCaoTNBenhNhanKhamThongTinYeuCauDichVuKyThuatVos { get; set; }
    }

    public class BaoCaoTNBenhNhanKhamThongTinYeuCauKhamBenhVo
    {
        public long Id { get; set; }
        public Enums.EnumTrangThaiYeuCauKhamBenh TrangThai { get; set; }
    }
    public class BaoCaoTNBenhNhanKhamThongTinYeuCauDichVuKyThuatVo
    {
        public long Id { get; set; }
        public Enums.EnumTrangThaiYeuCauDichVuKyThuat TrangThai { get; set; }
    }
    public class BaoCaoTNBenhNhanKhamThongTinDichVuVo
    {
        public long YeuCauTiepNhanId { get; set; }
        public string TenDichVu { get; set; }
    }

    public class InBaoCaoTNBenhNhanKhamVo
    {
        public string HostingName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }

    public class InBaoCaoTNBenhNhanKhamData
    {
        public string LogoUrl { get; set; }
        public string ThoiGianBaoCao { get; set; }
        public string DataBenhNhan { get; set; }
    }
    #endregion

    #region BaoCaoTheKho
    public class DuocPhamTheoKhoBaoCaoLookup : LookupItemVo
    {
        public long? DuocPhamHoacVatTuBenhVienId { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string DVT { get; set; }
        public Enums.LoaiDuocPhamHoacVatTu LoaiDuocPhamHoacVatTu { get; set; }
        public string LoaiDuocPhamHoacVatTuDisplay => LoaiDuocPhamHoacVatTu.GetDescription();

    }

    public class DuocPhamBaoCaoJsonVo
    {
        public long? KhoId { get; set; }
    }
    #endregion

    public class BaoCaoTonKhoQueryInfo : QueryInfo
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public long KhoId { get; set; }
    }

    public class BaoCaoTonKhoGridVo : GridItem
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string HamLuong { get; set; }
        public string SoLo { get; set; }
        public string DVT { get; set; }
        public string HanSuDung { get; set; }
        public double? TonDau { get; set; }
        public double? Nhap { get; set; }
        public double? TongSo => ((TonDau ?? 0) + (Nhap ?? 0)).MathRoundNumber(2);
        public double? Xuat { get; set; }
        public double? TonCuoi => ((TongSo ?? 0) - (Xuat ?? 0)).MathRoundNumber(2);
        public string Nhom { get; set; }
        public string Loai { get; set; }//Viện Phí / Thuốc BHYT
    }
    public class BaoCaoChiTietTonKhoGridVo : GridItem
    {
        public long DuocPhamBenhVienId { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string HamLuong { get; set; }
        public string SoLo { get; set; }
        public string DVT { get; set; }
        public DateTime HanSuDungDateTime { get; set; }
        public string HanSuDung { get; set; }
        public double? SLNhap { get; set; }
        public double? SLXuat { get; set; }
        public DateTime NgayNhapXuat { get; set; }
        public string Nhom { get; set; }
        public string Loai { get; set; }//Viện Phí / Thuốc BHYT
        public bool LaDuocPhamBHYT { get; set; }
        public long? DuocPhamBenhVienPhanNhomId { get; set; }
    }

    #region Báo cáo tồn kho vật tư 

    public class BaoCaoTonKhoVatTuQueryInfo : QueryInfo
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public long KhoId { get; set; }
    }

    public class BaoCaoChiTietTonKhoVatTuGridVo : GridItem
    {
        public string Nhom => LoaiSuDung == Enums.LoaiSuDung.VatTuThayThe
            ? Enums.LoaiSuDung.VatTuThayThe.GetDescription()
            : Enums.LoaiSuDung.VatTuTieuHao.GetDescription();
        public string Loai { get; set; }
        public bool LaVatTuBHYT { get; set; }
        public long VatTuBenhVienId { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string NhaSanXuat { get; set; }
        public string NuocSanXuat { get; set; }
        public string DVT { get; set; }
        public double? SLNhap { get; set; }
        public double? SLXuat { get; set; }
        public DateTime NgayNhapXuat { get; set; }
        public Enums.LoaiSuDung? LoaiSuDung { get; set; }

    }

    public class BaoCaoTonKhoVatTuGridVo : GridItem
    {
        public string Nhom { get; set; }
        public string Loai { get; set; }//Viện Phí / Thuốc BHYT

        public string Ma { get; set; }
        public string Ten { get; set; }
        public string DVT { get; set; }
        public double? TonDau { get; set; }
        public double? Nhap { get; set; }
        public double? TongSo => ((TonDau ?? 0) + (Nhap ?? 0)).MathRoundNumber(2);
        public double? Xuat { get; set; }
        public double? TonCuoi => ((TongSo ?? 0) - (Xuat ?? 0)).MathRoundNumber(2);
      
    }
    #endregion

    #region Báo cáo thẻ kho vật tư

    public class VatTuTheoKhoBaoCaoLookup : LookupItemVo
    {
        public long? DuocPhamHoacVatTuBenhVienId { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string DVT { get; set; }
        public Enums.LoaiDuocPhamHoacVatTu LoaiDuocPhamHoacVatTu { get; set; }
        public string LoaiDuocPhamHoacVatTuDisplay => LoaiDuocPhamHoacVatTu.GetDescription();

    }

    public class VatTuBaoCaoJsonVo
    {
        public long? KhoId { get; set; }
    }

    #endregion


    #region BaoCaoHoatDongKhoaKhamBenh
    public class BaoCaoHoaDongKhoaKhamBenhQueryInfo : QueryInfo
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? FromDateHoanThanh { get; set; }
        public DateTime? ToDateHoanThanh { get; set; }
    }

    public class BaoCaoHoatDongKhoaKhamBenhGridVo : GridItem
    {
        public long YeuCauTiepNhanId { get; set; }
        public string PhongBenhVien { get; set; }

        //public int TongSo => Bhyt.GetValueOrDefault() +
        //                     VienPhi.GetValueOrDefault() +
        //                     KskDoan.GetValueOrDefault() +
        //                     Ksk.GetValueOrDefault() +
        //                     Goi.GetValueOrDefault();
        public int TongSo { get; set; }

        public int? Bhyt { get; set; }
        public int? BhytCoGoi { get; set; }
        public int? BhytKhongGoi { get; set; }

        public int? VienPhi { get; set; }
        public int? VienPhiCoGoi { get; set; }
        public int? VienPhiKhongGoi { get; set; }
        //public int? KskDoan { get; set; }

        public int? KskDoanCongTy { get; set; }
        public int? KskBHTN { get; set; }
        public int? GiayKsk { get; set; }

        public int? Goi { get; set; }

        public int? TreEm { get; set; }

        public int? SoLanCapCuu { get; set; }

        public int? SoNguoiBenhVaoVien { get; set; }

        public int? SoNguoiBenhChuyenVien { get; set; }
        public int? SoNguoiBenhTuVong { get; set; }
        public int? SoNguoiBenhDieuTriNgoaiTru { get; set; }

        public int? SoNgayDieuTriNgoaiTru { get; set; }
    }
    #endregion

    #region BaoCaoThongKeSoLuongThuThuat

    public class BaoCaoThongKeSoLuongThuThuatQueryInfo
    {
        public long? KhoaPhongId { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public DateTime TuNgayFormat { get; set; }
        public DateTime DenNgayFormat { get; set; }
        public string HostingName { get; set; }

    }

    public class BaoCaoThongKeSoLuongThuThuatGridVo : GridItem
    {
        public string PhongThucHien { get; set; }
        public int? LoaiI { get; set; }
        public int? LoaiII { get; set; }
        public int? LoaiIII { get; set; }
        public int? DacBiet { get; set; }
        public int? Khac { get; set; }

    }

    #endregion

    #region BaoCaoLuuTruHoSoBenhAn
    public class BaoCaoLuuTruHoSoBenhAnQueryInfo : QueryInfo
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public long? KhoaId { get; set; }
        public bool? BHYT { get; set; }
        public bool? VienPhi { get; set; }
    }

    public class BaoCaoTiepNhanNguoiBenhKhamQueryInfo : QueryInfo
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string HostingName { get; set; }
    }

    public class BaoCaoLuuTruHoSoBenhAnGridVo : GridItem
    {
        public string ThuTuSapXep { get; set; }
        public string SoLuuTru { get; set; }
        public string HoTen { get; set; }
        public string GioiTinh { get; set; }
        public int? Tuoi { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string NgaySinhDisplay => DateHelper.DOBFormat(NgaySinh, ThangSinh, NamSinh);
        public DateTime? ThoiGianVaoVien { get; set; }
        public DateTime? ThoiGianRaVien { get; set; }
        public string ThoiGianVaoVienString => ThoiGianVaoVien != null ? ThoiGianVaoVien.GetValueOrDefault().ApplyFormatDateTimeSACH() : "";
        public string ThoiGianRaVienString => ThoiGianRaVien != null ? ThoiGianRaVien.GetValueOrDefault().ApplyFormatDateTimeSACH() : "";
        public string ChanDoan { get; set; }
        public string ICD { get; set; }
    }
    #endregion

    #region BaoCaoHieuQuaCongViec
    public class BaoCaoHieuQuaCongViecGridVo : GridItem
    {
        public string Khoa { get; set; }
        public int YeuCau { get; set; }
        public int DaHoanThanh { get; set; }
        public int DangThucHien { get; set; }


    }
    #endregion

    #region BaoCaoTinhHinhTraNCC
    public class BaoCaoTinhHinhTraNCCQueryInfo : QueryInfo
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public long KhoId { get; set; }
    }

    public class BaoCaoTinhHinhTraNCCGridVo : GridItem
    {
        public DateTime? NgayHoaDon { get; set; }
        public string NgayHoaDonStr => NgayHoaDon.HasValue ? NgayHoaDon.Value.ToString("dd/MM/yy") : "";
        public DateTime? NgayTra { get; set; }
        public string NgayTraStr => NgayTra.HasValue ? NgayTra.Value.ToString("dd/MM/yy") : "";
        public string SoHoaDon { get; set; }
        public string SoPhieuTra { get; set; }
        public string CongTy { get; set; }
        public string MaThuoc { get; set; }
        public string TenThuoc { get; set; }
        public string DVT { get; set; }
        public string SoLo { get; set; }
        public double? SoLuongTra { get; set; }
        public decimal? DonGiaNhap { get; set; }
        public decimal? ThanhTien => (decimal)(SoLuongTra ?? 0) * (DonGiaNhap ?? 0);
        public string DienGiai => !string.IsNullOrEmpty(CongTy) ? $"Trả lại công ty: {CongTy}" : "";
        public string Nhom { get; set; }

    }
    #endregion

    #region BaoCaoTinhHinhNhapNCCChiTiet

    public class BaoCaoTinhHinhNhapNCCChiTietQueryInfo : QueryInfo
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public long KhoId { get; set; }
    }

    public class BaoCaoTinhHinhNhapNCCChiTietGridVo : GridItem
    {
        public string MaThuoc { get; set; }
        public string TenThuoc { get; set; }
        public string HoatChat { get; set; }
        public string DVT { get; set; }
        public string HangSX { get; set; }
        public string SoLo { get; set; }
        public DateTime? HanDung { get; set; }
        public string HanDungStr => HanDung.HasValue ? HanDung.Value.ApplyFormatDate() : "";
        public decimal? DonGia { get; set; }
        public double? SoLuong { get; set; }
        public decimal? ThanhTien => (decimal)(SoLuong ?? 0) * (DonGia ?? 0);
        public string NhaCungCap { get; set; }

    }

    #endregion

    #region BaoCaoDuocChiTietXuatNoiBo
    public class BaoCaoDuocChiTietXuatNoiBoQueryInfo : QueryInfo
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public long KhoId { get; set; }
    }
    public class ThongTinKho
    {
        public string Kho { get; set; }
        

    }
    public class BaoCaoDuocChiTietXuatNoiBoGridVo : GridItem
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string HamLuong { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public string ThongTinThuoc => $"{Ma}: {Ten} {HamLuong}";
        public string Kho { get; set; }

        public string DVT { get; set; }
        public double? SoLuong { get; set; }
        public decimal? DonGia { get; set; }
        public decimal? ThanhTien => (decimal)(SoLuong ?? 0) * (DonGia ?? 0);


    }
    #endregion

    #region BaoCaoChiTietMienPhiTronVien
    public class BaoCaoChiTietMienPhiTronVienQueryInfo : QueryInfo
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public DateTime TuNgayFormat { get; set; }
        public DateTime DenNgayFormat { get; set; }
        public string HostingName { get; set; }
    }

    public class BaoCaoChiTietMienPhiTronVienGridVo : GridItem
    {
        public BaoCaoChiTietMienPhiTronVienGridVo()
        {
            ThongTinYeuCaus = new List<ThongTinYeuCauDichVuVo>();
        }

        public long YeuCauTiepNhanId { get; set; }
        public string TenKhoaPhong { get; set; }
        public string MaNB { get; set; }
        public string MaTN { get; set; }
        public string TenBN { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string NgaySinhDisplay => DateHelper.DOBFormat(NgaySinh, ThangSinh, NamSinh);
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }
        public decimal? ThanhTien { get; set; }
        public decimal? GiamPhi { get; set; }
        public decimal? MienPhi { get; set; }
        public decimal? SoTienMienGiam { get; set; }
        public bool KhongTinhPhi { get; set; }
        public decimal? TongGiamPhiMienPhiItem => KhongTinhPhi ? MienPhi : SoTienMienGiam;
        public decimal? TongGiamPhiMienPhi { get; set; }
        public string LyDo { get; set; }

        //BVHD-3918
        public long? YeuCauDichVuId { get; set; }
        public Enums.EnumNhomGoiDichVu? NhomDichVu { get; set; }
        public bool? LaHinhThucDenGioiThieu { get; set; }
        public List<ThongTinYeuCauDichVuVo> ThongTinYeuCaus { get; set; }

        public ICollection<DateTime> NgayChis { get; set; }
    }

    public class ThongTinYeuCauDichVuVo
    {
        public long YeuCauTiepNhanId { get; set; }
        public long? YeuCauDichVuId { get; set; }
        public Enums.EnumNhomGoiDichVu? NhomDichVu { get; set; }
        public string TenGoiKhuyenMai { get; set; }

        //Cập nhật 06/06/2022: hiển thị thêm trường hợp có voucher
        public string MaVoucher { get; set; }
    }
    #endregion

    #region BaoCaoTongHopDoanhThuThaiSanDaSinh
    public class BaoCaoTongHopDoanhThuThaiSanDaSinhQueryInfo : QueryInfo
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string StrQuery { get; set; }
    }
    public class DataGoiDvTongHopDoanhThuThaiSanDaSinh
    {
        public long BenhNhanId { get; set; }
        public long YeuCauGoiDichVuId { get; set; }
        public long? BenhNhanSoSinhId { get; set; }
        public DateTime ThoiDiemChiDinh { get; set; }
        public decimal SoTienSauChietKhau { get; set; }
        public decimal SoTienDaTamUng { get; set; }
        public Enums.EnumTrangThaiYeuCauGoiDichVu TrangThai { get; set; }
    }

    public class ChiPhiChuaThanhToanDoanhThuThaiSanDaSinh
    {
        public long Id { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public double Soluong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => (decimal)(Soluong * (double)DonGia);
        public long? YeuCauGoiDichVuId { get; set; }
        public bool DuocHuongBHYT { get; set; }
        public bool? BaoHiemChiTra { get; set; }
        public decimal DonGiaBHYT { get; set; }
        public int TiLeBaoHiemThanhToan { get; set; }
        public int MucHuongBaoHiem { get; set; }
        public decimal DonGiaBHYTThanhToan => DonGiaBHYT * TiLeBaoHiemThanhToan / 100 * MucHuongBaoHiem / 100;
        public decimal BHYTThanhToan => (DuocHuongBHYT && BaoHiemChiTra == true) ? (decimal)(Soluong * (double)DonGiaBHYTThanhToan) : 0;
        public decimal SoTienMG { get; set; }
        public long? ThanhToanTheoYeuCauDichVuGiuongBenhVienChiPhiBenhVienId { get; set; }
    }

    //{c.LoaiChiTienBenhNhan, c.DaHuy, c.TienChiPhi, c.SoLuong, c.DonGiaBaoHiem, c.TiLeBaoHiemThanhToan, c.MucHuongBaoHiem}
    public class DataPhieuThuCHiPhiTongHopDoanhThuThaiSanDaSinh
    {
        public Enums.LoaiChiTienBenhNhan LoaiChiTienBenhNhan { get; set; }
        public bool? DaHuy { get; set; }
        public decimal? TienChiPhi { get; set; }
        public double? SoLuong { get; set; }
        public decimal? DonGiaBaoHiem { get; set; }
        public int? TiLeBaoHiemThanhToan { get; set; }
        public int? MucHuongBaoHiem { get; set; }
        public decimal? SoTienBaoHiemTuNhanChiTra { get; set; }
    }
    public class DataPhieuThuTongHopDoanhThuThaiSanDaSinh
    {
        public long TaiKhoanBenhNhanThuId { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public long BenhNhanId { get; set; }
        public DateTime NgayThu { get; set; }
        public bool? ThuTienGoiDichVu { get; set; }
        public List<DataPhieuThuCHiPhiTongHopDoanhThuThaiSanDaSinh> ChiPhis { get; set; }
        public decimal TongSoTienChiPhi => ChiPhis?
            .Where(c=>c.LoaiChiTienBenhNhan == Core.Domain.Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi && c.DaHuy != true)
            .Select(c=>c.TienChiPhi.GetValueOrDefault() + c.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault()).DefaultIfEmpty(0).Sum() ?? 0;
        public decimal TongSoTienBHYT => ChiPhis?
            .Where(c => c.LoaiChiTienBenhNhan == Core.Domain.Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi && c.DaHuy != true)
            .Select(c => Convert.ToDecimal(c.SoLuong) * c.DonGiaBaoHiem.GetValueOrDefault() * c.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * c.MucHuongBaoHiem.GetValueOrDefault() / 100).DefaultIfEmpty().Sum() ?? 0;
    }
    //public class ChiPhiPhieuThuTongHopDoanhThuThaiSanDaSinh
    //{
    //    public decimal? BenhNhanId { get; set; }
    //    public DateTime NgayThu { get; set; }
    //    public bool? ThuTienGoiDichVu { get; set; }
    //    public decimal TongSoTienChiPhi { get; set; }
    //    public decimal TongSoTienBHYT { get; set; }
    //}

    public class DataTongHopDoanhThuThaiSanDaSinh
    {
        public long YeuCauTiepNhanId { get; set; }
        public long? YeuCauTiepNhanNgoaiTruId { get; set; }
        public long BenhNhanId { get; set; }
        //public List<long?> DvktGoiDichVuIds { get; set; }
        //public List<long?> GiuongGoiDichVuIds { get; set; }
        public List<long> YeuCauTiepNhanConIds { get; set; }
        public List<long> BenhNhanConIds { get; set; }
        public List<DataPhieuThuTongHopDoanhThuThaiSanDaSinh> DataPhieuThus { get; set; }
        public string MaTN { get; set; }
        public string TenBN { get; set; }
        public string MaBN { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        //public DateTime? NgaySinh { get; set; }
        //public string NgaySinhStr => NgaySinh.HasValue ? NgaySinh.Value.ApplyFormatDate() : "";
        public string DiaChi { get; set; }
        public DateTime? NgayVaoVien { get; set; }
        public string NgayVaoVienStr => NgayVaoVien.HasValue ? NgayVaoVien.Value.ApplyFormatDate() : "";
        public DateTime? NgayRaVien { get; set; }
        public string NgayRaVienStr => NgayRaVien.HasValue ? NgayRaVien.Value.ApplyFormatDate() : "";
        public Enums.LoaiBenhAn LoaiBenhAn { get; set; }
        public bool? DaQuyetToan { get; set; }
        public string CachThucDe { get; set; }
        public decimal? TongTienSauChietKhau { get; set; }
        public decimal? TongTienDichVuNgoaiGoi { get; set; }
        public decimal? TongSoTienChuaTruBHYT => (decimal)(TongTienSauChietKhau ?? 0) + (TongTienDichVuNgoaiGoi ?? 0);
        public decimal? TongTienBHYTChiTra { get; set; }
        public decimal? ThanhTien => (decimal)(TongSoTienChuaTruBHYT ?? 0) - (TongTienBHYTChiTra ?? 0);
        public decimal? SoTienDaThanhToan { get; set; }
        public decimal SoTienConThieu => (decimal)(ThanhTien ?? 0) - (SoTienDaThanhToan ?? 0);

    }

    public class BaoCaoTongHopDoanhThuThaiSanDaSinhGridVo : GridItem
    {
        public string MaTN { get; set; }
        public string MaBN { get; set; }
        public string TenBN { get; set; }
        public string NgaySinhStr { get; set; }
        //public DateTime? NgaySinh { get; set; }
        //public string NgaySinhStr => NgaySinh.HasValue ? NgaySinh.Value.ApplyFormatDate() : "";
        public string DiaChi { get; set; }
        public DateTime? NgayVaoVien { get; set; }
        public string NgayVaoVienStr => NgayVaoVien.HasValue ? NgayVaoVien.Value.ApplyFormatDate() : "";
        public DateTime? NgayRaVien { get; set; }
        public string NgayRaVienStr => NgayRaVien.HasValue ? NgayRaVien.Value.ApplyFormatDate() : "";
        public string CachThucDe { get; set; }
        public decimal? TongTienSauChietKhau { get; set; }
        public decimal? TongTienDichVuNgoaiGoi { get; set; }
        public decimal? TongSoTienChuaTruBHYT => (TongTienSauChietKhau ?? 0) + (TongTienDichVuNgoaiGoi ?? 0);
        public decimal? TongTienBHYTChiTra { get; set; }
        public decimal? ThanhTien => (decimal)(TongSoTienChuaTruBHYT ?? 0) - (TongTienBHYTChiTra ?? 0);
        public decimal? SoTienDaThanhToan { get; set; }
        public decimal SoTienConThieu => (decimal)(ThanhTien ?? 0) - (SoTienDaThanhToan ?? 0);

    }
    #endregion

    #region BaoCaoThuocSapHetHan
    public class BaoCaoThuocSapHetHanQueryInfo : QueryInfo
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public long KhoId { get; set; }
    }

    public class BaoCaoThuocSapHetHanGridVo : GridItem
    {
        public string MaDuoc { get; set; }
        public string TenThuoc { get; set; }
        public string HamLuong { get; set; }
        public string DVT { get; set; }
        public string SoLo { get; set; }
        public DateTime HanDung { get; set; }
        public string HanDungStr => HanDung.ApplyFormatDate();
        public double SLNhap { get; set; }
        public double SLXuat { get; set; }
        public double SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => (decimal)(SoLuong) * (DonGia);
        public string NhomThuoc { get; set; }
    }
    #endregion

    #region BaoCaoTongHopDangKyGoiDichVu
    public class BaoCaoTongHopDangKyGoiDichVuQueryInfo : QueryInfo
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public DateTime TuNgayFormat { get; set; }
        public DateTime DenNgayFormat { get; set; }
    }

    public class BaoCaoTongHopDangKyGoiDichVuGridVo : GridItem
    {
        public string MaNB { get; set; }
        public DateTime? NgayDangKy { get; set; }
        public string NgayDangKyStr => NgayDangKy?.ApplyFormatDateTimeSACH();
        public string TenBN { get; set; }
        public string TenGoi { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string NgaySinhDisplay => DateHelper.DOBFormat(NgaySinh, ThangSinh, NamSinh);
        public string DiaChi { get; set; }
        public decimal? GiaTriGoi { get; set; }
        public decimal? DaThu { get; set; }
        public decimal? ConThieu => (GiaTriGoi ?? 0) - (DaThu ?? 0);
        public decimal? GiaTriDichVuDaThucHien { get; set; }
        public decimal? GiaTriDichVuChuaThucHien => (GiaTriGoi ?? 0) - (GiaTriDichVuDaThucHien ?? 0);
        public decimal? SoTienHoanTra { get; set; }
        public decimal? PhiPhatHuyGoi => (HuyGoi && DaThu.HasValue) ? (DaThu ?? 0) - (GiaTriDichVuDaThucHien ?? 0) - (SoTienHoanTra ?? 0) : (decimal?)null;
        public bool HuyGoi { get; set; }
    }

    public class ChiPhiDichVuDaDungTrongGoiVo
    {
        public long YeuCauGoiDichVuId { get; set; }
        public decimal DonGia { get; set; }
        public double SoLuong { get; set; }
        public bool DuocBHYTChiTra { get; set; }
        public decimal? DonGiaBaoHiem { get; set; }
        public int? MucHuongBaoHiem { get; set; }
        public int? TiLeBaoHiemThanhToan { get; set; }
        public decimal? BHYTChiTra => DonGiaBaoHiem.GetValueOrDefault() * TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * MucHuongBaoHiem.GetValueOrDefault() / 100;
        public decimal? ThanhTien => Convert.ToDecimal((double)(DonGia - (DuocBHYTChiTra ? (BHYTChiTra ?? 0) : 0)) *  SoLuong);
    }
    #endregion

    #region BaoCaoBangKeXuatThuocTheoBenhNhan
    public class NgayGroupVo
    {
        public string NgayXuatStr { get; set; }
    }
    public class ThongTinKhachHangGroupVo
    {
        public string NgayXuatStr { get; set; }
        public string ThongTinKhachHang { get; set; }
        public string ChiTietCongNo { get; set; }
        public DateTime? NgayXuat { get; set; }

    }
    public class BaoCaoBangKeXuatThuocTheoBenhNhanQueryInfo : QueryInfo
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public long KhoId { get; set; }
    }

    public class BaoCaoBangKeXuatThuocTheoBenhNhanQueryData
    {
        public DateTime NgayXuat { get; set; }
        public string SoChungTu { get; set; }
        public List<BaoCaoBangKeXuatThuocTheoBenhNhanChiTietDuocPham> ChiTietDuocPhams { get; set; }
    }

    public class BaoCaoBangKeXuatThuocTheoBenhNhanChiTietDuocPham
    {
        public long Id { get; set; }
        public DateTime NgayXuat { get; set; }
        public string SoChungTu { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public string MaDuoc { get; set; }
        public string Ten { get; set; }
        public string HamLuong { get; set; }
        public string DVT { get; set; }
        public double SoLuong { get; set; }
        public decimal DonGiaNhap { get; set; }
        public decimal DonGiaBan { get; set; }
        public int VAT { get; set; }
        public int TiLeTheoThapGia { get; set; }
        public List<long> YeucauDuocPhamBenhViens { get; set; }
        public List<long> DonThuocThanhToanChiTiets { get; set; }
        public string ThongTinKhachHang { get; set; }
        public List<string> CongNos { get; set; }
    }

    public class BaoCaoBangKeXuatThuocTheoBenhNhanChiTietThuTien
    {
        public long Id { get; set; }
        public string HoTen { get; set; }
        public List<string> CongNos { get; set; }
    }

    public class BaoCaoBangKeXuatThuocTheoBenhNhanGridVo : GridItem
    {
        public string MaDuoc { get; set; }
        public string Ten { get; set; }
        public string HamLuong { get; set; }
        public string DVT { get; set; }
        public DateTime? NgayXuat { get; set; }
        public string NgayXuatStr => NgayXuat.HasValue? NgayXuat.Value.ApplyFormatDate() : "";

        //gồm Thời gian xuất thuốc: Họ tên khách - Số chứng từ 
        public string ThongTinKhachHang { get; set; }
        //public string ThoiGianXuatStr => NgayXuat.HasValue? NgayXuat.Value.ApplyFormatFullTime() : "";
        //public string HoTenKhachHang { get; set; }
        //public string SoChungTu { get; set; }
        public double? SoLuong { get; set; }
        public decimal? DonGiaDaCoVat { get; set; }
        public decimal? ThanhTien => (decimal)(SoLuong ?? 0) * (DonGiaDaCoVat ?? 0);
        public decimal? DonGiaBan { get; set; }
        public decimal? ThanhTienBan => (decimal)(SoLuong ?? 0) * (DonGiaBan ?? 0);
        public decimal? DonGiaHoanTra { get; set; }
        public decimal? ThanhTienHoanTra => (decimal)(SoLuong ?? 0) * (DonGiaHoanTra ?? 0);
        public string ChiTietCongNo { get; set; }
        public string SoHoaDon { get; set; }
        public int? ThueSuat { get; set; }
    }
    #endregion

    #region BaoCaoBangKeChiTietTTCN
    public class BaoCaoBangKeChiTietTTCNQueryInfo : QueryInfo
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }

    public class BaoCaoBangKeChiTietTTCNCongty : GridItem
    {
        public string TenCongTy { get; set; }
        public decimal SoTien { get; set; }
        public bool? DaHuy { get; set; }
    }
    public class BaoCaoBangKeChiTietTTCNPhieuTraNoCaNhan : GridItem
    {
        public decimal? TienMat { get; set; }
        public decimal? ChuyenKhoan { get; set; }
        public decimal? POS { get; set; }
        public DateTime NgayThu { get; set; }
        public bool? DaHuy { get; set; }
    }
    public class BaoCaoBangKeChiTietTTCNData : GridItem
    {
        public DateTime NgayThu { get; set; }
        public string MaTN { get; set; }
        public string TenBN { get; set; }
        public string SoPhieuThu { get; set; }
        public decimal? SoTienCongNo { get; set; }
        public List<BaoCaoBangKeChiTietTTCNCongty> ChiTietTTCNCongtys { get; set; }
        public List<BaoCaoBangKeChiTietTTCNPhieuTraNoCaNhan> ChiTietTTCNPhieuTraNoCaNhans { get; set; }
    }

    public class BaoCaoBangKeChiTietTTCNGridVo : GridItem
    {
        public DateTime? NgayThang { get; set; }
        public string NgayThangStr => NgayThang.HasValue ? NgayThang.Value.ApplyFormatDate() : "";
        public string DoiTuongBaoLanhCongNo { get; set; }
        public string MaTN { get; set; }
        public string TenBN { get; set; }
        public string SoPhieuThu { get; set; }
        public DateTime? NgayPhatSinhPhieuThu { get; set; }
        public string NgayPhatSinhPhieuThuStr => NgayPhatSinhPhieuThu.HasValue ? NgayPhatSinhPhieuThu.Value.ApplyFormatDate() : "";
        public decimal? SoTienCongNo { get; set; }
        public decimal? SoTienDaThanhToan { get; set; }
        public decimal? SoTienConNo => (decimal)(SoTienCongNo ?? 0) - (SoTienDaThanhToan ?? 0);
        public decimal? SoTienHoanTra { get; set; }

    }

    #endregion

    #region BaoCaoHoatDongCls

    public class BaoCaoHoatDongClsQueryInfo : QueryInfo
    {
        public DateTime MonthYear { get; set; }//-> FromDate-ToDate

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int MauId { get; set; }
    }

    public class BaoCaoHoatDongClsGridVo : GridItem
    {
        public string STT { get; set; }
        public string TenDichVu { get; set; }
        public string DonVi { get; set; }
        public int NoiTru { get; set; }
        public int NgoaiTru { get; set; }
        public int SucKhoeKhac { get; set; }

        public int TongSo => NoiTru + NgoaiTru + SucKhoeKhac;
        public string DanhMucCha { get; set; }
        public bool ToDam { get; set; }
    }

    public class DataHoatDongCls
    {
        public long Id { get; set; }
        public string TenDichVu { get; set; }
        public Enums.LoaiDichVuKyThuat? LoaiDichVuKyThuat { get; set; }
        public int SoLanYeuCau { get; set; }
        public int SoLan => SoLanYeuCau * SoLanThucHienXetNghiem.GetValueOrDefault(1);
        public long NhomDichVuBenhVienId { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public Enums.EnumLoaiYeuCauTiepNhan LoaiYeuCauTiepNhan { get; set; }
        public int? SoLanThucHienXetNghiem { get; set; }
        public List<long> PhienXetNghiemChiTietIds { get; set; }
    }

    //public class BaoCaoHoatDongClsGridVo : GridItem
    //{
    //    public string TenDanhMuc { get; set; }
    //    public int NoiTru => DanhMucConBaoCaoHoatDongCls.Sum(p => p.NoiTru);
    //    public int NgoaiTru => DanhMucConBaoCaoHoatDongCls.Sum(p => p.NgoaiTru);
    //    public int SucKhoeKhac => DanhMucConBaoCaoHoatDongCls.Sum(p => p.SucKhoeKhac);

    //    public int TongSo => NoiTru + NgoaiTru + SucKhoeKhac;
    //    public List<DanhMucConBaoCaoHoatDongCls> DanhMucConBaoCaoHoatDongCls { get; set; }

    //}

    #endregion

    #region BaoCaoDuocTinhHinhXuatNoiBo

    public class BaoCaoDuocTinhHinhXuatNoiBoQueryInfo : QueryInfo
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public long KhoId { get; set; }
    }

    public class BaoCaoDuocTinhHinhXuatNoiBoGridVo : GridItem
    {
        public long DuocPhamBenhVienId { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string DVT { get; set; }
        public double SoLuongYeuCau { get; set; }
        public double SoLuongThucXuat { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => (decimal)(SoLuongThucXuat) * (DonGia);
        public string GhiChu { get; set; }
        public string Nhom { get; set; }

    }
    #endregion


    #region BaoCaoKeToanNhapXuatTonChiTiet

    public class KhoaGroupBaoCaoKTNhapXuatTonChiTietVo
    {
        public string Khoa { get; set; }
    }
    public class NhomGroupBaoCaoKTNhapXuatTonChiTietVo
    {
        public string Khoa { get; set; }
        public string Nhom { get; set; }
    }

    public class BaoCaoKTNhapXuatTonChiTietQueryInfo : QueryInfo
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public long KhoaPhongId { get; set; }
        public long KhoId { get; set; }
        public bool CoVAT { get; set; }
    }
    public class BaoCaoKTNhapXuatTonKhoTongCap2XuatKhoChiTietVo
    {
        public long? XuatKhoDuocPhamChiTietId { get; set; }
        public long KhoaPhongId { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
    }

    public class BaoCaoKTNhapXuatTonDonThuocThanhToanChiTietVo
    {
        public long XuatKhoDuocPhamChiTietViTriId { get; set; }
        public long YeuCauKhamBenhNoiKeDonId { get; set; }
        public long NoiTruNoiKeDonId { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
    }

    public class BaoCaoKTNhapXuatTonChiTietQueryData
    {
        public long Id { get; set; }
        public long KhoId { get; set; }
        public long KhoaKhoAoId { get; set; }
        public long VatTuBenhVienId { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public long? NhapTuXuatKhoId { get; set; }
        public bool MuaNCC { get; set; }
        public bool NhapHoanTra { get; set; }
        public bool NhapNoiBo { get; set; }
        public double SLNhap { get; set; }
        public double SLXuat { get; set; }
        public decimal DonGiaNhap { get; set; }
        public int VAT { get; set; }
        public bool DauKy { get; set; }
        public bool? TraNCC { get; set; }
        public bool XuatNoiBo { get; set; }
        public bool BenhNhanTraLai { get; set; }
        public bool XuatChoBenhNhan { get; set; }
        public bool XuatChoKhachHang { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public List<BaoCaoKTNhapXuatTonChiTietXuatChoBenhNhan> BaoCaoKTNhapXuatTonChiTietXuatChoBenhNhans { get; set; }
    }
    public class BaoCaoKTNhapXuatTonChiTietXuatChoBenhNhanTheoKhoa
    {
        public BaoCaoKTNhapXuatTonChiTietXuatChoBenhNhan BaoCaoKTNhapXuatTonChiTietXuatChoBenhNhan { get; set; }
        public long KhoaPhongId { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
    }
    public class BaoCaoKTNhapXuatTonChiTietXuatChoBenhNhan
    {
        public long XuatKhoChiTietViTriId { get; set; }
        public long XuatKhoChiTietId { get; set; }
        public double SoLuongXuat { get; set; }
        public List<long?> DonThuocIds { get; set; }
    }

    public class BaoCaoKTNhapXuatTonChiTietGridVo : GridItem
    {
        public long KhoId { get; set; }
        public long KhoaKhoAoId { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public long VatTuBenhVienId { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string DVT { get; set; }
        public double SLTonDauKy { get; set; }
        public decimal DonGiaNhap { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTienTonDauKy => (decimal)SLTonDauKy * DonGia;
        public double SLNhapMuaNCCTrongKy { get; set; }
        public decimal ThanhTienNhapMuaNCCTrongKy => (decimal)SLNhapMuaNCCTrongKy * DonGia;
        public double SLNhapTangKiemKeTrongKy { get; set; }
        public decimal ThanhTienNhapTangKiemKeTrongKy => (decimal)SLNhapTangKiemKeTrongKy * DonGia;

        public double SLNhapHoanTraTrongKy { get; set; }
        public decimal ThanhTienNhapHoanTraTrongKy => (decimal)SLNhapHoanTraTrongKy * DonGia;

        public double SLNhapNoiBoTrongKy { get; set; }
        public decimal ThanhTienNhapNoiBoTrongKy => (decimal)SLNhapNoiBoTrongKy * DonGia;

        public double SLNhapKhacTrongKy { get; set; }
        public decimal ThanhTienNhapKhacTrongKy => (decimal)SLNhapKhacTrongKy * DonGia;

        public double SLXuatNoiBoTrongKy { get; set; }
        public decimal ThanhTienXuatNoiBoTrongKy =>(decimal)SLXuatNoiBoTrongKy * DonGia;

        public double SLXuatGiamKiemKeTrongKy { get; set; }
        public decimal ThanhTienXuatGiamKiemKeTrongKy =>(decimal)SLXuatGiamKiemKeTrongKy * DonGia;

        public double SLXuatTraNCCTrongKy { get; set; }
        public decimal ThanhTienXuatTraNCCTrongKy =>(decimal)SLXuatTraNCCTrongKy * DonGia;

        public double SLXuatBNTrongKy { get; set; }
        public decimal ThanhTienXuatBNTrongKy =>(decimal)SLXuatBNTrongKy * DonGia;

        public double SLXuatKHTrongKy { get; set; }
        public decimal ThanhTienXuatKHTrongKy =>(decimal)SLXuatKHTrongKy * DonGia;

        public double SLXuatKhacTrongKy { get; set; }
        public decimal ThanhTienXuatKhacTrongKy =>(decimal)SLXuatKhacTrongKy * DonGia;

        public double SLTonCuoiKy => SLTonDauKy + SLNhapMuaNCCTrongKy + SLNhapTangKiemKeTrongKy + SLNhapHoanTraTrongKy + SLNhapNoiBoTrongKy + SLNhapKhacTrongKy - (SLXuatNoiBoTrongKy + SLXuatGiamKiemKeTrongKy + SLXuatTraNCCTrongKy + SLXuatBNTrongKy + SLXuatKHTrongKy + SLXuatKhacTrongKy);
        public decimal ThanhTienTonCuoiKy => (decimal)SLTonCuoiKy * DonGia;
        public string Nhom { get; set; }
        public string Kho { get; set; }


    }
    #endregion

    #region BaoCaoKeToanNhapXuatTon

   
    public class BaoCaoKTNhapXuatTonQueryInfo : QueryInfo
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public long KhoaPhongId { get; set; }  
        public bool CoVAT { get; set; }
        public bool CoThuoc { get; set; }
        public bool CoVTYT { get; set; }
    }

    public class NhomGroupBaoCaoKTNhapXuatTonVo
    {
        public string Nhom { get; set; }
    }

    public class BaoCaoKTNhapXuatTonGridVo : GridItem
    {
        public long VatTuBenhVienId { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string DVT { get; set; }
        public double SLTonDauKy { get; set; }        
        public decimal ThanhTienTonDauKy { get; set; }
        public double SLNhapMuaNCCTrongKy { get; set; }
        public decimal ThanhTienNhapMuaNCCTrongKy { get; set; }
        public double SLNhapTangKiemKeTrongKy { get; set; }
        public decimal ThanhTienNhapTangKiemKeTrongKy { get; set; }

        public double SLNhapHoanTraTrongKy { get; set; }
        public decimal ThanhTienNhapHoanTraTrongKy { get; set; }

        public double SLNhapNoiBoTrongKy { get; set; }
        public decimal ThanhTienNhapNoiBoTrongKy { get; set; }

        public double SLNhapKhacTrongKy { get; set; }
        public decimal ThanhTienNhapKhacTrongKy { get; set; }

        public double SLXuatNoiBoTrongKy { get; set; }
        public decimal ThanhTienXuatNoiBoTrongKy { get; set; }

        public double SLXuatGiamKiemKeTrongKy { get; set; }
        public decimal ThanhTienXuatGiamKiemKeTrongKy { get; set; }

        public double SLXuatTraNCCTrongKy { get; set; }
        public decimal ThanhTienXuatTraNCCTrongKy { get; set; }

        public double SLXuatBNTrongKy { get; set; }
        public decimal ThanhTienXuatBNTrongKy { get; set; }

        public double SLXuatKHTrongKy { get; set; }
        public decimal ThanhTienXuatKHTrongKy { get; set; }

        public double SLXuatKhacTrongKy { get; set; }
        public decimal ThanhTienXuatKhacTrongKy { get; set; }

        public double SLTonCuoiKy => SLTonDauKy + SLNhapMuaNCCTrongKy + SLNhapTangKiemKeTrongKy + SLNhapHoanTraTrongKy + SLNhapNoiBoTrongKy + SLNhapKhacTrongKy - (SLXuatNoiBoTrongKy + SLXuatGiamKiemKeTrongKy + SLXuatTraNCCTrongKy + SLXuatBNTrongKy + SLXuatKHTrongKy + SLXuatKhacTrongKy);
        public decimal ThanhTienTonCuoiKy => ThanhTienTonDauKy + ThanhTienNhapMuaNCCTrongKy + ThanhTienNhapTangKiemKeTrongKy + ThanhTienNhapHoanTraTrongKy + ThanhTienNhapNoiBoTrongKy + ThanhTienNhapKhacTrongKy - (ThanhTienXuatNoiBoTrongKy + ThanhTienXuatGiamKiemKeTrongKy + ThanhTienXuatTraNCCTrongKy + ThanhTienXuatBNTrongKy + ThanhTienXuatKHTrongKy + ThanhTienXuatKhacTrongKy);
        public string Nhom { get; set; }


    }

    public class BaoCaoKTNhapXuatTonVo
    {
        public BaoCaoKTNhapXuatTonVo()
        {
            Data = new List<BaoCaoKTNhapXuatTonGridVo>();
            ListGroupTheoFileExecls = new List<NhomGroupBaoCaoKTNhapXuatTonVo>();
            DataSumPageTotal = new List<BaoCaoKTNhapXuatTonGridVo>();
        }
        public int TotalRowCount { get; set; }
        public List<BaoCaoKTNhapXuatTonGridVo> Data { get; set; }
        public List<NhomGroupBaoCaoKTNhapXuatTonVo> ListGroupTheoFileExecls { get; set; }
        public List<BaoCaoKTNhapXuatTonGridVo> DataSumPageTotal { get; set; }
    }
    public class BaoCaoKTNhapXuatTonChiTietVo
    {
        public BaoCaoKTNhapXuatTonChiTietVo()
        {
            Data = new List<BaoCaoKTNhapXuatTonChiTietGridVo>();
            ListGroupTheoFileExecls = new List<NhomGroupBaoCaoKTNhapXuatTonChiTietVo>();
            DataSumPageTotal = new List<BaoCaoKTNhapXuatTonChiTietGridVo>(); 
        }
        public int TotalRowCount { get; set; }
        public List<BaoCaoKTNhapXuatTonChiTietGridVo> Data { get; set; }
        public List<NhomGroupBaoCaoKTNhapXuatTonChiTietVo> ListGroupTheoFileExecls { get; set; }
        public List<BaoCaoKTNhapXuatTonChiTietGridVo> DataSumPageTotal { get; set; }
    }
    #endregion
}
