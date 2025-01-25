namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class KhoaPhongPhongKhamExportExcel
    {
        [Width(38)]
        public string Ten { get; set; }

        public string Ma { get; set; }

        public string IsDisabled { get; set; } //

        [Group]
        public string TenKhoaPhong { get; set; }
    }
}
