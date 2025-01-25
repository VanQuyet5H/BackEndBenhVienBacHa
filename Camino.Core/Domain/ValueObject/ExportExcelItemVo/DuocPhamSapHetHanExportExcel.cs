namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class DuocPhamSapHetHanExportExcel
    {
        [Width(20)]
        public string TenKho { get; set; }
        [Width(20)]
        public string TenDuocPham { get; set; }
        [Width(20)]
        public string TenHoatChat { get; set; }
        [Width(20)]
        public string DonViTinh { get; set; }
        [Width(20)]
        public string ViTri { get; set; }
        [Width(20)]
        public double? SoLuongTon { get; set; }
        [Width(20)]
        public string NgayHetHanHienThi { get; set; }
        #region update 06072021
        [Width(20)]
        public string MaDuocPham { get; set; }
        [Width(20)]
        public string SoLo { get; set; }
        [Width(20)]
        public decimal DonGiaNhap { get; set; }
        [Width(20)]
        public long NhapKhoDuocPhamId { get; set; }
        [Width(20)]
        public long DuocPhamId { get; set; }
        [Width(20)]
        public double ThanhTien { get; set; }
        [Width(20)]
        public string HamLuong { get; set; }
        #endregion
    }
}
