using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Core.Domain.ValueObject.YeuCauTiepNhans
{
    public class FilterDanhSachCanLamSangGridVo
    {
        public bool? ChuaCoKetQua { get; set; }

        public bool? DaCoKetQua { get; set; }
    }

    public class CanLamSangExportExcel
    {
        [Width(20)]
        public string MaYeuCauTiepNhan { get; set; }

        [Width(20)]
        public string SoBenhAn { get; set; }

        [Width(20)]
        public int? MaBN { get; set; }

        [Width(35)]
        public string HoTen { get; set; }

        [Width(15)]
        public string GioiTinh { get; set; }
        [Width(20)]
        public string NgayThangNam { get; set; }
        [Width(30)]
        public string BacSiCDDisplay { get; set; }
        [Width(20)]
        public string KetLuanDisplay { get; set; }
        [Width(30)]
        public string BacSiKetLuanDisplay { get; set; }
        [Width(20)]
        public string NgayChiDinhDisplay { get; set; }
        [Width(30)]
        public string KyThuatVien1Display { get; set; }
        [Width(20)]
        public string NgayThucHienDisplay { get; set; }
        [Width(40)]
        public string NoiChiDinh { get; set; }

        [Width(60)]
        public string ChanDoan { get; set; }
        [Width(60)]
        public string ChuanDoanDisplay { get; set; }

        [Width(60)]
        public string ChiDinh { get; set; }

        [Width(20)]
        public string DoiTuong { get; set; }

        [Width(20)]
        public string TenTrangThai { get; set; }
    }

    public class LichSuCanLamSangExportExcel
    {

        [Width(20)]
        public string MaYeuCauTiepNhan { get; set; }
        [Width(20)]
        public string SoBenhAn { get; set; }

        [Width(20)]
        public int? MaBN { get; set; }

        [Width(40)]
        public string HoTen { get; set; }

        [Width(20)]
        public string GioiTinh { get; set; }

        [Width(100)]
        public string DiaChi { get; set; }

        [Width(20)]
        public string SoDienThoai { get; set; }
    }
}
