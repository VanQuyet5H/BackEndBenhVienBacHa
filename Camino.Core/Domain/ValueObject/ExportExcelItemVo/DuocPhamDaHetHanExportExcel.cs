namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class DuocPhamDaHetHanExportExcel
    {
        [Width(20)]
        public string Kho { get; set; }

        [Width(42)]
        public string DuocPham { get; set; }

        [Width(42)]
        public string HoatChat { get; set; }
        [Width(20)]
        public string DonViTinh { get; set; }

        [Width(77)]
        public string ViTri { get; set; }
        [Width(20)]
        public string SoLuongTon { get; set; } //
        [Width(20)]
        public string NgayHetHanDisplay { get; set; }
        [Width(20)]
        public string MaDuocPham { get; set; }
        [Width(20)]
        public string SoLo { get; set; }
        [Width(20)]
        public string DonGiaNhap { get; set; }
        [Width(20)]
        public string ThanhTien { get; set; }
        [Width(20)]
        public string HamLuong { get; set; }
    }
}
