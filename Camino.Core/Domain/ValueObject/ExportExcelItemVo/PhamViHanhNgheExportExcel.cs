namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class PhamViHanhNgheExportExcel
    {
        [Width(36)]
        public string Ten { get; set; }

        public string Ma { get; set; }

        [Width(100)]
        public string MoTa { get; set; }
    }
}
