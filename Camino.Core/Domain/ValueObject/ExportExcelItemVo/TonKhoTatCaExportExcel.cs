namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class TonKhoTatCaExportExcel
    {
        [Width(65)]
        public string DuocPham { get; set; }
        [Width(65)]
        public string HoatChat { get; set; }
        [Width(20)]
        public string HamLuong { get; set; }
        [Width(20)]
        public string PhanLoai { get; set; }
        [Width(20)]
        public string DonViTinhName { get; set; }
        [Width(20)]
        public double SoLuongTon { get; set; }
        [Width(20)]
        public string GiaTriSoLuongTonFormat { get; set; }

        //BVHD-3912
        [Width(20)]
        public string MaDuocPham { get; set; }
    }
}
