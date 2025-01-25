using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.PhauThuatThuThuat
{
    public class ThongTinHoanTraVatTuThuocPTTTVo : GridItem
    {
        public ThongTinHoanTraVatTuThuocPTTTVo()
        {
            YeuCauDuocPhamVatTuBenhViens = new List<ThongTinHoanTraVatTuThuocChiTietPTTTVo>();
        }
        
        public string Ten { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public string TenKho { get; set; }
        public long NhanVienYeuCauId { get; set; }
        public string TenNhanVienYeuCau { get; set; }
        public DateTime? NgayYeuCau { get; set; }
        public bool? KhongTinhPhi { get; set; }
        public long VatTuThuocBenhVienId { get; set; }
        public double SoLuong { get; set; }
        public string SoLuongDisplay { get; set; }
        public decimal DonGiaNhap { get; set; }
        public decimal DonGia { get; set; }//=> CalculateHelper.TinhDonGiaBan(DonGiaNhap, TiLeTheoThapGia, VAT);
        public decimal ThanhTien => KhongTinhPhi != true ? DonGia * (decimal)SoLuong : 0;
        public int TiLeTheoThapGia { get; set; }
        public int VAT { get; set; }
        public double SoLuongDaTra { get; set; }
        public double? SoLuongTra { get; set; }
        public EnumNhomGoiDichVu NhomYeuCauId { get; set; }

        public List<ThongTinHoanTraVatTuThuocChiTietPTTTVo> YeuCauDuocPhamVatTuBenhViens { get; set; }
    }

    public class ThongTinHoanTraVatTuThuocChiTietPTTTVo : GridItem
    {
        public bool? KhongTinhPhi { get; set; }
        public long VatTuThuocBenhVienId { get; set; }
        public double SoLuong { get; set; }
        public string SoLuongDisplay => SoLuong.MathRoundNumber(2).ToString();
        public decimal DonGiaNhap { get; set; }
        public decimal DonGia { get; set; } //=> CalculateHelper.TinhDonGiaBan(DonGiaNhap, TiLeTheoThapGia, VAT);
        public decimal ThanhTien => KhongTinhPhi != true ? DonGia * (decimal)SoLuong : 0;
        public int TiLeTheoThapGia { get; set; }
        public int VAT { get; set; }
        public double SoLuongDaTra { get; set; }
        public double? SoLuongTra { get; set; }
        public EnumNhomGoiDichVu NhomYeuCauId { get; set; }
    }
}