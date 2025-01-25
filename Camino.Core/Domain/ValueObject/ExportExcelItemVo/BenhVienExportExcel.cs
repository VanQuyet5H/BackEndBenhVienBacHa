namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class BenhVienExportExcel
    {
        [Width(28)]
        public string Ma { get; set; }

        [Width(70)]
        public string TenVietTat { get; set; }

        [Width(70)]
        public string Ten { get; set; }

        [Width(35)]
        public string TenDonViHanhChinh { get; set; }

        [Width(35)]
        public string TenLoaiBenhVien { get; set; }

        [Width(35)]
        public string HangBenhVienDisplay { get; set; }

        [Width(35)]
        public string TuyenChuyenMonKyThuatDisplay { get; set; }

        [Width(30)]
        public string SoDienThoaiLanhDao { get; set; }

        [Width(25)]
        public string HieuLucDisplay { get; set; }
    }
}
