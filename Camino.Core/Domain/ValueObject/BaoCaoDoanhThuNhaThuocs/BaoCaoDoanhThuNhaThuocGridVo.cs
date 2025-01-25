using System;
using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.BaoCaoDoanhThuNhaThuocs
{
    public class BaoCaoDoanhThuNhaThuocDaTaVo
    {
        public long Id { get; set; }
        public string SoChungTu { get; set; }
        public DateTime? Ngay { get; set; }
        public string MaYTe { get; set; }
        public string BenhNhan { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public Enums.LoaiGioiTinh? EnumGioiTinh { get; set; }
        public decimal? TienMat { get; set; }
        public decimal? ChuyenKhoan { get; set; }
        public decimal? Pos { get; set; }
        public decimal? CongNo { get; set; }
        public string NguoiBan { get; set; }
        public string GhiChu { get; set; }
        public string ChiTietCongNo { get; set; }
        public string SoHoaDon { get; set; }
        public List<long> x { get; set; }

        public string KhoaChiDinh { get; set; }
        public string LyDoHuyBanThuoc { get; set; }

        public List<BaoCaoDoanhThuNhaThuocDaTaChiVo> BaoCaoDoanhThuNhaThuocDaTaChiVos { get; set; }
        public List<BaoCaoDoanhThuNhaThuocDaTaCongNoVo> BaoCaoDoanhThuNhaThuocDaTaCongNoVos { get; set; }
    }
    public class BaoCaoDoanhThuNhaThuocDaTaChiVo
    {
        public long? DonThuocThanhToanChiTietId { get; set; }
        public long? DonVTYTThanhToanChiTietId { get; set; }
    }
    public class BaoCaoDoanhThuNhaThuocDaTaCongNoVo
    {
        public string TenCongTy { get; set; }
        public decimal SoTien { get; set; }        
    }
    public class BaoCaoDoanhThuNhaThuocGridVo : GridItem
    {
        public string SoChungTu { get; set; }
        public DateTime? Ngay { get; set; }
        public string NgayDisplay => Ngay != null ? Ngay.GetValueOrDefault().ApplyFormatDateTime() : string.Empty;
        public string MaYTe { get; set; }
        public string BenhNhan { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string NgayThangnamSinh => (NgaySinh != null && ThangSinh != null && NamSinh != null) ? $"{NgaySinh}/{ThangSinh}/{NamSinh}" : NamSinh?.ToString();
        public string GioiTinh { get; set; }
        public decimal ThanhTien => (TienMat ?? 0) + (ChuyenKhoan ?? 0) + (Pos ?? 0) + (CongNo ?? 0);
        public decimal? TienMat { get; set; }
        public decimal? ChuyenKhoan { get; set; }
        public decimal? Pos { get; set; }
        public decimal? CongNo { get; set; }
        public string NguoiBan { get; set; }
        public string GhiChu { get; set; }
        public string ChiTietCongNo { get; set; }
        public string SoHoaDon { get; set; }
        public string LyDoHuyBanThuoc{ get; set; }
        public string KhoaChiDinh { get; set; }
    }
    public class BaoCaoDoanhThuNhaThuocVo
    {
        public string SearchString { get; set; }
        public string HostingName { get; set; }
        public BaoCaoDoanhThuNhaThuocTuNgayDenNgayVo TuNgayDenNgay { get; set; }

    }

    public class BaoCaoDoanhThuNhaThuocTuNgayDenNgayVo
    {
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public string TuNgay { get; set; }
        public string DenNgay { get; set; }
    }
}
