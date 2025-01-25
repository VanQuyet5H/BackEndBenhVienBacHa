using Camino.Api.Models.DuocPhamBenhVien;
using Camino.Api.Models.HopDongThauDuocPham;
using Camino.Api.Models.KhoDuocPhamViTri;
using System;
using Camino.Core.Domain;


namespace Camino.Api.Models.NhapKhoDuocPhamChiTiets
{
    public class NhapKhoDuocPhamChiTietViewModel : BaseViewModel
    {
        public bool? LaDuocPhamBHYT { get; set; }

        public long? NhapKhoDuocPhamId { get; set; }
        public long? DuocPhamBenhVienId { get; set; }
        public long? HopDongThauDuocPhamId { get; set; }
        public string Solo { get; set; }
        public bool? DatChatLuong { get; set; }
        public DateTime? HanSuDung { get; set; }
        public double? SoLuongNhap { get; set; }
        public double SoLuongNhapTrongGrid { get; set; }
        public double SoLuongHienTaiDuocPhamTheoHopDongThauDaLuu { get; set; }
        public decimal? DonGiaNhap { get; set; }
        public decimal? DonGiaBan { get; set; }
        public int? VAT { get; set; }
        public int? ChietKhau { get; set; }
        public string MaVach { get; set; }
        public long? KhoDuocPhamViTriId { get; set; }
        public double? SoLuongDaXuat { get; set; }
        public DateTime? NgayNhap { get; set; }
        public DateTime? NgayNhapVaoBenhVien { get; set; }

        public int? TiLeTheoThapGia { get; set; }
        public int? TiLeBHYTThanhToan { get; set; }
        public long? DuocPhamBenhVienPhanNhomId { get; set; }

        public virtual HopDongThauDuocPhamViewModel HopDongThauDuocPhams { get; set; }
        public virtual DuocPhamBenhVienViewModel DuocPhamBenhViens { get; set; }
        public virtual KhoDuocPhamViTriViewModel KhoDuocPhamViTri { get; set; }
        //field View string
        public string TenDuocPham { get; internal set; }
        public string TenHopDongThau { get; internal set; }
        public string ViTri { get; internal set; }
        public string TextHanSuDung { get; internal set; }
        public string TenDatChatLuong { get; internal set; }
        public int? IdView { get; internal set; }
        public string MaRef { get; set; }
        public long? KhoNhapSauKhiDuyetId { get; set; }
        public long? NguoiNhapSauKhiDuyetId { get; set; }
        public decimal ThanhTienTruocVat { get; set; }
        public decimal ThanhTienSauVat { get; set; }
        public Enums.PhuongPhapTinhGiaTriTonKho PhuongPhapTinhGiaTriTonKho { get; set; }
        //field View string
    }
}
