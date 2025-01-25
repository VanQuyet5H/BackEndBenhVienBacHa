namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class VatTuSapHetHanExportExcel
    {
        [Width(45)]
        public string TenKho { get; set; }
        [Width(55)]
        public string TenVatTu { get; set; }
        [Width(20)]
        public string DonViTinh { get; set; }
        [Width(20)]
        public string ViTri { get; set; }
        [Width(20)]
        public double? SoLuongTon { get; set; }
        [Width(20)]
        public string NgayHetHanHienThi { get; set; }
        [Width(20)]
        public string MaVatTu { get; set; }
        [Width(20)]
        public string SoLo { get; set; }
        [Width(20)]
        public string DonGiaNhap { get; set; }
        [Width(20)]
        public string ThanhTien { get; set; }
    }
}
