namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class VatTuTonKhoTongHopExportExcel
    {
        [Width(65)]
        public string TenVatTu { get; set; }
        [Width(20)]
        public string DonViTinh { get; set; }
        [Width(20)]
        public double SoLuongTon { get; set; }
        [Width(20)]
        public string GiaTriSoLuongTonFormat { get; set; }
    }
}
