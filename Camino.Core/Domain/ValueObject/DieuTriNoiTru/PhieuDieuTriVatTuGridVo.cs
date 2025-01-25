using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.DieuTriNoiTru
{
    public class PhieuDieuTriVatTuGridVo : GridItem
    {
        public PhieuDieuTriVatTuGridVo()
        {
            PhieuDieuTriVatTuGiaGroupGridVos = new List<PhieuDieuTriVatTuGiaGroupGridVo>();
        }
        public string Ids { get; set; }
        public long VatTuBenhVienId { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenDVKT { get; set; }
        public long? KhoId { get; set; }
        public string TenKho { get; set; }
        public string DVT { get; set; }
        public decimal DonGiaNhap { get; set; }
        public decimal DonGiaBan { get; set; }
        public decimal? DonGiaBaoHiem { get; set; }
        public int TiLeTheoThapGia { get; set; }
        public int VAT { get; set; }
        public double? SoLuong { get; set; }
        //public string SoLuongDisplay => SoLuong.FloatToStringFraction();
        public bool? KhongTinhPhi { get; set; }
        public decimal DonGia { get; set; }//=> CalculateHelper.TinhDonGiaBan(DonGiaNhap, TiLeTheoThapGia, VAT);
        public decimal ThanhTien => KhongTinhPhi != true ? 0 : DonGia * (decimal)SoLuong.GetValueOrDefault();
        public bool LaVatTuBHYT { get; set; }
        public int NhomId => LaVatTuBHYT ? 1 : 0;
        public string Nhom => LaVatTuBHYT ? "BHYT" : "Không BHYT";
        public bool TinhTrang { get; set; }
        public string TinhTrangDisplay => TinhTrang ? "Đã xuất" : "Chưa xuất";
        public string PhieuLinh { get; set; }
        public string PhieuXuat { get; set; }
        public bool CoYeuCauTraVTTuBenhNhanChiTiet { get; set; }
        public bool LaTuTruc { get; set; }
        public EnumLoaiKhoDuocPham? LoaiKho => LaTuTruc ? EnumLoaiKhoDuocPham.KhoLe : EnumLoaiKhoDuocPham.KhoTongVTYTCap2;
        public DateTime? ThoiDiemChiDinh { get; set; }
        public string TenNhanVienChiDinh { get; set; }
        public string ThoiDiemChiDinhDisplay => ThoiDiemChiDinh?.ApplyFormatDateTime();
        public DateTime? CreatedOn { get; set; }

        //BVHD-3905
        public string TiLeThanhToanBHYT { get; set; }
        public string TooltipTiLeBHYT => TiLeThanhToanBHYT?.Replace("\n", "<br>");

        public List<PhieuDieuTriVatTuGiaGroupGridVo> PhieuDieuTriVatTuGiaGroupGridVos { get; set; }
    }


    public class PhieuDieuTriVatTuGiaGroupGridVo
    {
        public bool? KhongTinhPhi { get; set; }
        public double? SoLuong { get; set; }
        public decimal? DonGia { get; set; }
        public decimal? DonGiaBaoHiem { get; set; }
        public decimal? ThanhTien => KhongTinhPhi != true ? 0 : DonGia * Convert.ToDecimal(SoLuong);
    }

    public class ThongTinChiTietVatTuTonKhoPDT
    {
        public long YeuCauTiepNhanId { get; set; }
        public long VatTuBenhVienId { get; set; }
        public long KhoId { get; set; }
        public int LoaiVatTu { get; set; }
        public long NoiTruPhieuDieuTriId { get; set; }

    }
    public class ThongTinPhieuDieuTriVatTu
    {
        public string DonViTinh { get; set; }
        public double TonKho { get; set; }
        public string TonKhoFormat => TonKho.ApplyNumber();
        public bool? FlagVatTuDaKe { get; set; }
        public string VatTuDaKe => FlagVatTuDaKe == true ? "Có" : "Không";
        public bool? FlagVatTuDaKeTrungKho { get; set; }
        public DateTime? HanSuDung { get; set; }
        public string HSD => HanSuDung?.ApplyFormatDate();
    }

    public class VatTuBenhVienVo : GridItem
    {
        public string Ids { get; set; }
        public long? YeuCauDichVuKyThuatId { get; set; }
        public long KhoId { get; set; }
        public int LaVatTuBHYT { get; set; }
        public long VatTuBenhVienId { get; set; }
        public double SoLuong { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public bool? KhongTinhPhi { get; set; }
        public long PhieuDieuTriHienTaiId { get; set; }

    }

    public class HoanTraVTVo
    {
        public string Ids { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public long YeuCauVatTuBenhVienId { get; set; }
        public long KhoId { get; set; }
        public bool LaVatTuBHYT { get; set; }
    }


    public class ThongTinHoanTraVTVo : GridItem
    {
        public ThongTinHoanTraVTVo()
        {
            YeuCauVatTuBenhViens = new List<ThongTinHoanTraVTChiTietVo>();
        }
        public string Ids { get; set; }
        public bool? KhongTinhPhi { get; set; }
        public long VatTuBenhVienId { get; set; }
        public string Ten { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public string TenKho { get; set; }
        public long NhanVienYeuCauId { get; set; }
        public string TenNhanVienYeuCau { get; set; }
        public DateTime? NgayYeuCau { get; set; }
        public double SoLuong { get; set; }
        public string SoLuongDisplay => SoLuong.MathRoundNumber(2).ToString();
        public decimal DonGiaNhap { get; set; }
        public decimal DonGia { get; set; } //=> CalculateHelper.TinhDonGiaBan(DonGiaNhap, TiLeTheoThapGia, VAT);
        public decimal ThanhTien => KhongTinhPhi != true ? DonGia * (decimal)SoLuong : 0;
        public int TiLeTheoThapGia { get; set; }
        public int VAT { get; set; }
        public double SoLuongDaTra { get; set; }
        public double? SoLuongTra { get; set; }
        public List<ThongTinHoanTraVTChiTietVo> YeuCauVatTuBenhViens { get; set; }
    }

    public class ThongTinHoanTraVTChiTietVo: GridItem
    {
        public bool? KhongTinhPhi { get; set; }
        public long YeuCauVatTuBenhVienId { get; set; }
        public double SoLuong { get; set; }
        public string SoLuongDisplay => SoLuong.MathRoundNumber(2).ToString();
        public decimal DonGiaNhap { get; set; }
        public decimal DonGia { get; set; } //=> CalculateHelper.TinhDonGiaBan(DonGiaNhap, TiLeTheoThapGia, VAT);
        public decimal ThanhTien => KhongTinhPhi != true ? DonGia * (decimal)SoLuong : 0;
        public int TiLeTheoThapGia { get; set; }
        public int VAT { get; set; }
        public double SoLuongDaTra { get; set; }
        public double? SoLuongTra { get; set; }
    }
}
