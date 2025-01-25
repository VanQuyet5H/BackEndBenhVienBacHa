namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class ThietBiXetNghiemExportExcel
    {
        [Width(55)]
        public string NhomThietBiDisplay { get; set; }

        [Width(15)]
        public string Ma { get; set; }
        [Width(30)]
        public string Ten { get; set; }
        [Width(35)]
        public string Ncc { get; set; }
        [Width(13)]
        public string TinhTrang { get; set; }
        [Width(16)]
        public string HieuLuc { get; set; }

        [Group]
        public string NhomXetNghiemDisplay { get; set; }
    }
}
