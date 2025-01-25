namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class QuanHeThanNhanExportExcel
    {
        [Width(20)]
        public string TenVietTat { get; set; }

        [Width(40)]
        public string Ten { get; set; }

        [Width(100)]
        public string MoTa { get; set; }

        [Width(20)]
        public string HieuLuc { get; set; }
    }
}
