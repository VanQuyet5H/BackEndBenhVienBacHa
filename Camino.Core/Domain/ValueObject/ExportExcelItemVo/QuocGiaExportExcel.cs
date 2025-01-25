namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class QuocGiaExportExcel
    {
        [Width(20)]
        public string Ma { get; set; }
        [Width(30)]
        public string Ten { get; set; }
        [Width(20)]
        public string TenVietTat { get; set; }
        [Width(20)]
        public string QuocTich { get; set; }
        [Width(30)]
        public string MaDienThoaiQuocTe { get; set; }
        [Width(20)]
        public string ThuDo { get; set; }
        [Width(20)]
        public string ChauLuc { get; set; }
        [Width(20)]
        public string IsDisabled { get; set; }
    }
}
