namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class VoucherExportExcel
    {
        [Width(20)]
        public string Ma { get; set; }
        [Width(30)]
        public string Ten { get; set; }
        [Width(20)]
        public string SoLuongPhatHanhDisplay { get; set; }
        [Width(20)]
        public string TuNgayDisplay { get; set; }
        [Width(20)]
        public string DenNgayDisplay { get; set; }
    }
}