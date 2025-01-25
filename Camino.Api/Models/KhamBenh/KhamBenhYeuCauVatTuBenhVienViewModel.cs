using Camino.Core.Domain;
using System;

namespace Camino.Api.Models.KhamBenh
{
    public class KhamBenhYeuCauVatTuBenhVienViewModel : BaseViewModel
    {
        public long? YeuCauTiepNhanId { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public long? YeuCauDichVuKyThuatId { get; set; }
        public long? VatTuBenhVienId { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }
        public long? NhomVatTuId { get; set; }
        public long? DonViTinhId { get; set; }
        public string NhaSanXuat { get; set; }
        public string NuocSanXuat { get; set; }
        public string QuyCach { get; set; }
        public string TieuChuan { get; set; }
        public string MoTa { get; set; }
        public bool? KhongTinhPhi { get; set; }
        public decimal? Gia { get; set; }
        public long? GoiDichVuId { get; set; }
        public int? TiLeChietKhau { get; set; }
        public double? SoLuong { get; set; }
        public long? NhanVienChiDinhId { get; set; }
        public long? NoiChiDinhId { get; set; }
        public DateTime? ThoiDiemChiDinh { get; set; }
        public long? NoiCapVatTuId { get; set; }
        public long? NhanVienCapVatTuId { get; set; }
        public DateTime? ThoiDiemCapVatTu { get; set; }
        public bool? DuocHuongBaoHiem { get; set; }
        public bool? BaoHiemChiTra { get; set; }
        public DateTime? ThoiDiemDuyetBaoHiem { get; set; }
        public long? NhanVienDuyetBaoHiemId { get; set; }
        public decimal? GiaBaoHiemThanhToan { get; set; }
        public bool? DaThanhToan { get; set; }
        public long? NoiThanhToanId { get; set; }
        public long? NhanVienThanhToanId { get; set; }
        public DateTime? ThoiDiemThanhToan { get; set; }
        public bool? DaCapVatTu { get; set; }
        public Enums.EnumYeuCauVatTuBenhVien? TrangThai { get; set; }
        public string GhiChu { get; set; }
    }
}
