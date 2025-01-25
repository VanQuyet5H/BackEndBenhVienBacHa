namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class ChuyenKhoaExportExcel
    {
        public string Ma { get; set; }

        [Width(45)]
        public string Ten { get; set; }

        [Width(100)]
        public string MoTa { get; set; }

        public string HieuLuc { get; set; }
    }
}
