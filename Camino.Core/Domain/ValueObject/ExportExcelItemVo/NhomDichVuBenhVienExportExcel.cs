namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class NhomDichVuBenhVienExportExcel
    {
        [Width(20)]
        public string Ma { get; set; }
        [Width(35)]
        public string NhomDichVuBenhVienCha { get; set; }
        [Width(50)]
        public string Ten { get; set; }
        [Width(20)]
        public string MoTa { get; set; }
    }
}
