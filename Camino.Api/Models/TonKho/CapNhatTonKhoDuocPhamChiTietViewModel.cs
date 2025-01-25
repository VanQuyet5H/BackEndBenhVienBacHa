

using System;

namespace Camino.Api.Models.TonKho
{
    public class CapNhatTonKhoDuocPhamChiTietViewModel : BaseViewModel
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
        public long? DuocPhamBenhVienPhanNhomId { get; set; }
        public int Loai { get; set; }
    }
}
