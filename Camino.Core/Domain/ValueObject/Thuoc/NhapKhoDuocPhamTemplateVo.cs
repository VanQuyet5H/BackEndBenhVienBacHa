using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.Thuoc
{
    public class NhapKhoDuocPhamTemplateVo : LookupItemVo
    {
        public string Ten { get; set; }
        public string Ma { get; set; }
        public string HoatChat { get; set; }
        public string NhaSanXuat { get; set; }
        public decimal? Gia { get; set; }
        public double? SoLuongChuaNhap { get; set; }
        public string DVT { get; set; }
        public int? HeSoDinhMucDonViTinh { get; set; }
        public string HamLuong { get; set; }
        public string DuongDung { get; set; }
        public double SLTon { get; set; }
    }

    public class NhapKhoDuocPhamVatTuTheoHopDongThau
    {
        public long? HopDongThauDuocPhamId { get; set; }
        public long? HopDongThauVatTuId { get; set; }
        public long? KhoId { get; set; }
        public bool? LaDuocPhamBHYT { get; set; }
        public bool? LaVatTuBHYT { get; set; }

    }
}
