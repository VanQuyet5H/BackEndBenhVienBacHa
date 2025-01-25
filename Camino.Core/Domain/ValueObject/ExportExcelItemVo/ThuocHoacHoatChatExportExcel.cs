namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class ThuocHoacHoatChatExportExcel
    {
        public string Ma { get; set; }

        [Width(120)]
        public string Ten { get; set; }

        [Width(24)]
        public string SttHoatChat { get; set; }

        public string SttThuoc { get; set; }

        public string MaATC { get; set; }

        [Width(39)]
        public string DuongDung { get; set; }

        public string HoiChan { get; set; } //

        [Width(32)]
        public string TyLeBaoHiemThanhToan { get; set; }

        [Width(22)]
        public string CoDieuKienThanhToan { get; set; }

        [Width(255)]
        public string MoTa { get; set; }

        [Width(50)]
        public string NhomThuoc { get; set; }

        [Width(22)]
        public string BenhVienHang1 { get; set; }

        [Width(22)]
        public string BenhVienHang2 { get; set; }

        [Width(22)]
        public string BenhVienHang3 { get; set; }

        [Width(22)]
        public string BenhVienHang4 { get; set; }
    }
}
