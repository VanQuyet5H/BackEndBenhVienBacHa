namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class NhapXuatTonKhoExportExcel
    {
        [Group]
        [Width(40)]
        public string TenDuocPhamBenhVienPhanNhom { get; set; }
        [Width(40)]
        public string Ma { get; set; }
        [Width(65)]
        public string DuocPham { get; set; }
        [Width(65)]
        public string HoatChat { get; set; }
        [Width(20)]
        public string HamLuong { get; set; }
        [Width(20)]
        public string PhanLoai { get; set; }
        [Width(20)]
        public string DonViTinhDisplay { get; set; }
        [Width(20)]
        public double TonDauKy { get; set; }
        [Width(20)]
        public double NhapTrongKy { get; set; }
        [Width(20)]
        public double XuatTrongKy { get; set; }
        [Width(20)]
        public double TonCuoiKy { get; set; }
    }
}