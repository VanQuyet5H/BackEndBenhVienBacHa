namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class VanBangChuyenMonExportExcel
    {
        [Width(30)]
        public string Ma { get; set; }
        [Width(45)]
        public string Ten { get; set; }
        [Width(40)]
        public string TenVietTat { get; set; }
        [Width(20)]
        public string MoTa { get; set; }
        [Width(20)]
        public string IsDisabled { get; set; }
    }
}
