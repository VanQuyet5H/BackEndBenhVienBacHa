namespace Camino.Api.Models.PhauThuatThuThuat
{
    public class GridItemGhiNhanVatTuThuocPTTTViewModel
    {
        public long YeuCauTiepNhanId { get; set; }
        public long YeuCauDichVuKyThuatId { get; set; }
        public string YeuCauGhiNhanVTTHThuocId { get; set; }
        public int? SoLuong { get; set; }
        public bool IsCapNhatTinhPhi { get; set; }
        public bool IsCapNhatSoLuong { get; set; }
        public bool? TinhPhi { get; set; }
    }
}
