namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class PhuongPhapVoCamExportExcel
    {
        [Width(20.5)]
        public string Ma { get; set; }

        [Width(30)]
        public string Ten { get; set; }

        [Width(300)]
        public string MoTa { get; set; }
    }
}
