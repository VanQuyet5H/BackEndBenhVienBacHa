using Camino.Api.Models.HopDongThauVatTu;
using Camino.Api.Models.VatTuBenhViens;
using System;
using Camino.Core.Domain;

namespace Camino.Api.Models.NhapKhoVatTuChiTiets
{
    public class NhapKhoVatTuChiTietViewModel : BaseViewModel
    {
        public bool? LaVatTuBHYT { get; set; }

        public long? NhapKhoVatTuId { get; set; }
        public long? VatTuBenhVienId { get; set; }
        public long? HopDongThauVatTuId { get; set; }
        public string Solo { get; set; }
        public bool? DatChatLuong { get; set; }
        public DateTime? HanSuDung { get; set; }
        public double? SoLuongNhap { get; set; }
        public double SoLuongNhapTrongGrid { get; set; }
        public double SoLuongHienTaiVatTuTheoHopDongThauDaLuu { get; set; }
        public decimal? DonGiaNhap { get; set; }
        public decimal? DonGiaBan { get; set; }
        public int? VAT { get; set; }
        public int? ChietKhau { get; set; }
        public string MaVach { get; set; }
        public long? KhoViTriId { get; set; }
        public double? SoLuongDaXuat { get; set; }
        public DateTime? NgayNhap { get; set; }
        public DateTime? NgayNhapVaoBenhVien { get; set; }

        public int? TiLeTheoThapGia { get; set; }
        public int? TiLeBHYTThanhToan { get; set; }
        //public long? DuocPhamBenhVienPhanNhomId { get; set; }

        public virtual HopDongThauVatTuViewModel HopDongThauVatTus { get; set; }
        public virtual VatTuBenhVienViewModel VatTuBenhViens { get; set; }
        //public virtual KhoVatTuViTriViewModel KhoDuocPhamViTri { get; set; }
        //field View string
        public string TenVatTu { get; internal set; }
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
