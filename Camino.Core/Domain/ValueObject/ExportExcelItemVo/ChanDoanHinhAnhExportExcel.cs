namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class ChanDoanHinhAnhExportExcel
    {
        public string Ma { get; set; }

        public string Ten { get; set; }

        public string TenTiengAnh { get; set; }

        [Width(30)]
        public string LoaiChuanDoanHinhAnhDisplay { get; set; }

        [Width(100)]
        public string MoTa { get; set; }

        public string HieuLuc { get; set; }
    }
}
