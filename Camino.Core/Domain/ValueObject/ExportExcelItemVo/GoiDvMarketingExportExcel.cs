namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class GoiDvMarketingExportExcel
    {
        [Width(51)]
        public string TenGoiDv { get; set; }

        [Width(52)]
        public string MoTa { get; set; }

        [Width(16)]
        public string SuDung { get; set; }
    }
    public class NhomDichVuExportExcel
    {
        public string TenNhom { get; set; }

        public string MoTa { get; set; }

        public string SuDung { get; set; }
    }
}
