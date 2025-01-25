using System;
using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.TonKhos
{
    public class NhapXuatTonKhoGridVo : GridItem
    {
        public string Ma { get; set; }
        public string DuocPham { get; set; }
        public string HoatChat { get; set; }
        public string PhanLoai { get; set; }
        public string HamLuong { get; set; }
        public string DonViTinhDisplay { get; set; }
        public double TonDauKy { get; set; }
        public double NhapTrongKy { get; set; }
        public double XuatTrongKy { get; set; }
        public double TonCuoiKy { get; set; }
        public string TenDuocPhamBenhVienPhanNhom { get; set; }
        public long? DuocPhamBenhVienPhanNhomId { get; set; }
    }

    public class NhapXuatTonKhoGridVoItem : GridItem
    {
        public NhapXuatTonKhoGridVoItem()
        {
            Sort = new List<Sort>();
        }
        public long KhoId { get; set; }
        public string Description { get; set; }
        public RangeDate RangeDate { get; set; }
        public List<Sort> Sort { get; set; }
        public string SortStringFormat { get; set; }
        public string SortString
        {
            get
            {
                if (!string.IsNullOrEmpty(SortStringFormat))
                {
                    return SortStringFormat;
                }
                // order the results
                if (Sort != null && Sort.Count > 0)
                {
                    var sorts = new List<string>();
                    Sort.ForEach(x => sorts.Add(string.Format("{0} {1}", x.Field, x.Dir)));
                    return string.Join(",", sorts.ToArray());
                }
                return string.Empty;
            }
        }
    }

    public class RangeDate
    {
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
    }

    public class NhatXuatTonKhoChiTietGridVoItem : GridItem
    {
        public long KhoId { get; set; }
        public long DuocPhamId { get; set; }
        public RangeDate RangeDate { get; set; }
    }

    public class NhapXuatTonKhoDetailGridVo : GridItem
    {
        public int? STT { get; set; }
        public DateTime NgayNhapXuat { get; set; }
        public string NgayDisplay { get; set; }
        public string MaChungTu { get; set; }
        public double Nhap { get; set; }
        public double Xuat { get; set; }
        public double Ton { get; set; }
    }

    public class ChiTietItem
    {
        public long? KhoId { get; set; }
        public long? DuocPhamId { get; set; }
        public string KhoDisplay { get; set; }
        public string DuocPhamDisplay { get; set; }
    }

    public class NhatXuatTonKhoVatTuChiTietGridVoItem : GridItem
    {
        public long KhoId { get; set; }
        public long VatTuId { get; set; }
        public RangeDate RangeDate { get; set; }
    }
    public class NhapXuatTonKhoCapNhatDetailGridVo : GridItem
    {
        public int? STT { get; set; }
        public DateTime NgayNhapXuat { get; set; }
        public string NgayDisplay { get; set; }
        public string MaChungTu { get; set; }
        public double SoLuong { get; set; }
        public double SoLuongDaXuat { get; set; }
        public double Nhap { get; set; }
        public double Xuat { get; set; }
        public double Ton { get; set; }
        public string SoLo { get; set; }
        public DateTime? HanSuDung { get; set; }
        public string HanSuDungDisplay => HanSuDung?.ApplyFormatDateTime();
        public int Loai { get; set; }
        public string LoaiDisplay => Loai == 1 ? "Nhập" : "Xuất";
        public string MaRef { get; set; }
        public string MaVach { get; set; }
        public decimal? DonGiaNhap { get; set; }
        public int? VAT { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public bool LaVatTuBHYT { get; set; }
        public int? TiLeBHYTThanhToan { get; set; }
        public string LoaiSuDung { get; set; }
        public long? DuocPhamBenhVienPhanNhomId { get; set; }
        public string DuocPhamBenhVienPhanNhomTen { get; set; }
        public string HighLightClass { get; set; }
        public string ThongTinBooking { get; set; }


    }

    public class CapNhatTonKhoItem
    {
        public long DuocPhamId { get; set; }
        public string SoDangKy { get; set; }
        public List<NhapXuatTonKhoCapNhatDetailGridVo> CapNhatTonKhoDuocPhamChiTiets { get; set; }

    }
    public class CapNhatTonKhoVatTuItem
    {
        public long VatTuId { get; set; }
        public List<NhapXuatTonKhoCapNhatDetailGridVo> CapNhatTonKhoItemDetails { get; set; }

    }

    public class CapNhatTonKhoVatTuVo : GridItem
    {
        public CapNhatTonKhoVatTuVo()
        {
            NhapKhoVatTuChiTiets = new List<CapNhatTonKhoVatTuChiTietVo>();
        }
        public long? VatTuId { get; set; }
        public List<CapNhatTonKhoVatTuChiTietVo> NhapKhoVatTuChiTiets { get; set; }
    }

    public class CapNhatTonKhoVatTuChiTietVo : GridItem
    {
        public string SoLo { get; set; }
        public DateTime? HanSuDung { get; set; }
        public double? SoLuong { get; set; }
        public double? SoLuongXuat { get; set; }
        public string MaRef { get; set; }
        public string MaVach { get; set; }
        public decimal? DonGiaNhap { get; set; }
        public int? VAT { get; set; }
        public int? TiLeBHYTThanhToan { get; set; }
        public bool LaVatTuBHYT { get; set; }
        public int Loai { get; set; }
    }
}