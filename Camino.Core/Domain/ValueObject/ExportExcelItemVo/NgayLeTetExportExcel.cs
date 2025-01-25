namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class NgayLeTetExportExcel
    {
        [Width(20)]
        public string Ten { get; set; }
        [Width(20)]
        public int? Ngay { get; set; }
        [Width(20)]
        public int? Thang { get; set; }
        [Width(20)]
        public int? Nam { get; set; }
        [Width(20)]
        public string LeHangNam { get; set; }
        [Width(20)]
        public string GhiChu { get; set; }
    }
}
