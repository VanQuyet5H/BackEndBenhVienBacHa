using System;
using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.YeuCauHoanTra
{
    public class VatTuHoanTraGridVo : GridItem
    {
        public new string Id { get; set; }
        public int STT { get; set; }
        public string TenVatTu { get; set; }
        public string DVT { get; set; }
        public bool LaVatTuBHYT { get; set; }
        public string Loai { get { return LaVatTuBHYT ? "BHYT" : "Không BHYT"; } }
        public double SoLuongTon { get; set; }
        public string SoLuongTonDisplay => SoLuongTon.ApplyNumber();
        public DateTime HanSuDung { get; set; }
        public string HanSuDungDisplay => HanSuDung.ApplyFormatDate();

        public DateTime NgayNhap { get; set; }
        public string NgayNhapDisplay => NgayNhap.ApplyFormatDateTime();

        public double SoLuongXuat { get; set; }

        public Enums.LoaiSuDung? LoaiSuDung { get; set; }

        public string Nhom => LoaiSuDung.GetValueOrDefault().GetDescription();

        public string MaVatTu { get; set; }
        public string SoLo { get; set; }

        public int? Vat { get; set; }

        public int? TiLeThapGia { get; set; }
    }
    public class ThemVatTuHoanTra
    {
        public long? NhapKhoVatTuChiTietId { get; set; }
        public long? VatTuBenhVienId { get; set; }
        public int? ChatLuong { get; set; }
        public double? SoLuongXuat { get; set; }
        public double? SoLuongTon { get; set; }
        public long? KhoId { get; set; }

        public decimal? DonGia { get; set; }
        public int? VAT { get; set; }
        public int? ChietKhau { get; set; }

        public bool LaVatTuBHYT { get; set; }

        public Enums.LoaiSuDung? NhomVatTu { get; set; }

        public Enums.EnumLoaiKhoDuocPham? loaiKhoVatTuXuat { get; set; }
        public Enums.XuatKhoDuocPham? loaiXuatKho { get; set; }
    }


    /// Update 06/01/2021
    /// 

    public class YeuCauTraVatTuTuTrucVoSearch
    {
        public YeuCauTraVatTuTuTrucVoSearch()
        {
            VatTuBenhVienVos = new List<YeuCauTraVatTuTuTrucChiTietVo>();
        }
        public long? KhoXuatId { get; set; }
        public string SearchString { get; set; }
        public List<YeuCauTraVatTuTuTrucChiTietVo> VatTuBenhVienVos { get; set; }
        public long? YeuCauTraVatTuId { get; set; }
        public bool IsCreate { get; set; }
    }

    public class YeuCauTraVatTuTuTrucChiTietVo
    {
        public long VatTuBenhVienId { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }
        public bool LaVatTuBHYT { get; set; }
        public string SoLo { get; set; }
        public DateTime? HanSuDung { get; set; }
        public DateTime? NgayNhap { get; set; }
        public double? SoLuongTra { get; set; }
        public decimal? DonGiaNhap { get; set; }
        public long? KhoXuatId { get; set; }
        public int? VAT { get; set; }
        public int? TiLeThapGia { get; set; }
    }

    public class YeuCauHoanTraVatTuChiTietTheoKhoXuatVos
    {
        public YeuCauHoanTraVatTuChiTietTheoKhoXuatVos()
        {
            YeuCauHoanTraVatTuChiTiets = new List<YeuCauTraVatTuGridVo>();
        }
        public List<YeuCauTraVatTuGridVo> YeuCauHoanTraVatTuChiTiets { get; set; }
    }

    public class YeuCauTraVatTuGridVo : GridItem
    {
        public long VatTuBenhVienId { get; set; }
        public string Ten { get; set; }
        public string DVT { get; set; }
        public bool LaVatTuBHYT { get; set; }
        public string Loai { get { return LaVatTuBHYT ? "BHYT" : "Không BHYT"; } }
        public double SoLuongTon { get; set; }
        public string SoLuongTonDisplay => SoLuongTon.ApplyNumber();
        public string Ma { get; set; }
        public string SoLo { get; set; }
        public double SoLuongTra { get; set; }
        public decimal DonGiaNhap { get; set; }
        public DateTime? HanSuDung { get; set; }
        public DateTime? NgayNhap { get; set; }
        public string HanSuDungDisplay => HanSuDung?.ApplyFormatDate();
        public string NgayNhapDisplay => NgayNhap?.ApplyFormatDate();
        public long? KhoXuatId { get; set; }
        public long? XuatKhoVatTuId { get; set; }
        public string SearchString { get; set; }
        public Enums.LoaiSuDung? LoaiSuDung { get; set; }
        public string TenNhom => LoaiSuDung.GetDescription();
        public int? VAT { get; set; }
        public int? TiLeThapGia { get; set; }
    }

    public class DanhSachDaDuyetChiTietVTVo
    {
        public long YeuCauTraVatTuId { get; set; }
        public string SearchString { get; set; }
    }

    public class DanhSachYeuCauHoanTraVTChiTietGridVo : GridItem
    {
        public long VatTuBenhVienId { get; set; }
        public string Ten { get; set; }
        public string HopDong { get; set; }
        public string Ma { get; set; }
        public string DVT { get; set; }
        public bool LaVatTuBHYT { get; set; }
        public string Loai => LaVatTuBHYT ? "BHYT" : "Không BHYT";
        public string SoLo { get; set; }
        public DateTime HanSuDung { get; set; }
        public string HsdText => HanSuDung.ApplyFormatDate();
        public DateTime NgayNhapVaoBenhVien { get; set; }
        public string NgayNhapBvText => NgayNhapVaoBenhVien.ApplyFormatDate();
        public decimal DonGiaNhap { get; set; }
        public Enums.LoaiSuDung? LoaiSuDung { get; set; }
        public int TiLeThapGia { get; set; }
        public int Vat { get; set; }
        public string MaVach { get; set; }
        public string TenNhom => LoaiSuDung.GetDescription();
        public double SoLuongTra { get; set; }
    }
}
