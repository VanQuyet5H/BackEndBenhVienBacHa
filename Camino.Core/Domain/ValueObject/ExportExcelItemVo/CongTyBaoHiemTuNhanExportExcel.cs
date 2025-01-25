namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class CongTyBaoHiemTuNhanExportExcel
    {
        [Width(8)]
        public string Ma { get; set; }

        [Width(43)]
        public string Ten { get; set; }

        [Width(71)]
        public string DiaChi { get; set; }

        [Width(44)]
        public string SoDienThoai { get; set; }

        [Width(100)]
        public string Email { get; set; }

        [Width(24)]
        public string HinhThucBaoHiem { get; set; }

        [Width(24)]
        public string PhamViBaoHiem { get; set; }

        [Width(100)]
        public string GhiChu { get; set; }
    }
}
