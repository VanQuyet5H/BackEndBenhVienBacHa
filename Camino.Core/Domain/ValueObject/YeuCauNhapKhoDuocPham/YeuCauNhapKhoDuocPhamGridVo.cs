using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.YeuCauNhapKhoDuocPham
{
    public class YeuCauNhapKhoDuocPhamGridVo
    {
    }

    public class YeuCauNhapKhoDuocPhamChiTietGridVo
    {
        public double IdView { get; set; }
        public long Id { get; set; }
        public long? HopDongThauDuocPhamId { get; set; }
        public long? NhaThauId { get; set; }
        public long? DuocPhamBenhVienId { get; set; }
        public bool? LaDuocPhamBHYT { get; set; }
        public long? DuocPhamBenhVienPhanNhomId { get; set; }
        public string Solo { get; set; }
        public DateTime? HanSuDung { get; set; }
        public string MaVach { get; set; }
        public int? SoLuongNhap { get; set; }
        public double? DonGiaNhap { get; set; }
        public int? VAT { get; set; }
        public long? KhoViTriId { get; set; }

        public int? TiLeBHYTThanhToan { get; set; }
        public int? LoaiNhap { get; set; } // 1 là hdt, 2 là ncc

        //for grid
        public string HopDongThauDisplay { get; set; }
        public string NhaThauDisplay { get; set; }
        public string DuocPhamDisplay { get; set; }
        public string LoaiDisplay { get; set; }
        public string NhomDisplay { get; set; }
        public string HanSuDungDisplay { get; set; }
        public string SoLuongNhapDisplay { get; set; }
        public string ViTriDisplay { get; set; }

        public string DVT { get; set; }
        public string MaRef { get; set; }
        public DateTime? NgayHoaDon { get; set; }
        public long? KhoNhapSauKhiDuyetId { get; set; }
        public long? NguoiNhapSauKhiDuyetId { get; set; }
        public string TenKhoNhapSauKhiDuyet { get; set; }
        public string TenNguoiNhapSauKhiDuyet { get; set; }
        public decimal? ThanhTienTruocVat { get; set; }
        public decimal? ThanhTienSauVat { get; set; }
        public decimal? ThueVatLamTron { get; set; }

        public string HamLuong { get; set; }
        public string KyHieuHoaDon { get; set; }
        public string GhiChu { get; set; }

    }
}
