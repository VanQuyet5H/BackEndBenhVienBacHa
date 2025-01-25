using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Core.Domain.ValueObject.GoiBaoHiemYTe
{   

    public class GoiBaoHiemYTeExportExcel
    {
        [Width(20)]
        public int STT { get; set; }

        [Width(20)]
        public string MaTN { get; set; }

        [Width(20)]
        public string MaBN { get; set; }

        [Width(40)]
        public string HoTen { get; set; }

        [Width(20)]
        public string NamSinh { get; set; }

        [Width(20)]
        public string GioiTinh { get; set; }

        [Width(100)]
        public string DiaChi { get; set; }

        [Width(25)]
        public string ThoiGianTiepNhanStr { get; set; }

        [Width(25)]
        public string MucHuong { get; set; }
    }

    public class LichSuGoiBaoHiemYTeExportExcel
    {
        [Width(20)]
        public int STT { get; set; }

        [Width(20)]
        public string MaTN { get; set; }

        [Width(20)]
        public string MaBN { get; set; }

        [Width(40)]
        public string HoTen { get; set; }

        [Width(20)]
        public string NamSinh { get; set; }

        [Width(20)]
        public string GioiTinh { get; set; }

        [Width(100)]
        public string DiaChi { get; set; }

        [Width(25)]
        public string ThoiGianTiepNhanStr { get; set; }

        [Width(25)]
        public string MucHuong { get; set; }
    }
}
