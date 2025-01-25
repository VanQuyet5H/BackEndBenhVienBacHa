namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class GiuongBenhExportExcel
    {
        [Width(20)]
        public string Ma { get; set; }
        [Width(100)]
        public string Ten { get; set; }
        [Width(20)]
        public string Khoa { get; set; }
        [Width(20)]
        public string Phong { get; set; }
        [Width(200)]
        public string MoTa { get; set; }
    }
}
