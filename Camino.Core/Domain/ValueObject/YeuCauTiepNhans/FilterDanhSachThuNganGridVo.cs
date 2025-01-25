using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo.Attributes;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.YeuCauTiepNhans
{
    public class FilterDanhSachThuNganGridVo
    {
        public bool? ChuaThu { get; set; }

        public bool? DaThu { get; set; }
    }

    public class ThuTienExportExcel
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
        public string DoiTuong { get; set; }

        [Width(20)]
        public string GioiTinhStr { get; set; }

        [Width(25)]
        [TextAlign(Constants.TextAlignAttribute.Right)]
        public string SoTienBNPhaiTT { get; set; }

        [Width(25)]
        [TextAlign(Constants.TextAlignAttribute.Right)]
        public string SoTienTamUng { get; set; }

        [Width(25)]
        [TextAlign(Constants.TextAlignAttribute.Right)]
        public string SoTienDuTaiKhoan { get; set; }

        [Width(25)]
        [TextAlign(Constants.TextAlignAttribute.Right)]
        public string SoTienBNDaTT { get; set; }

        [Width(20)]
        public string Status { get; set; }

        [Width(20)]
        public string ThoiDiemTiepNhanDisplay { get; set; }
    }

    public class LichSuThuTienExportExcel
    {
        [Width(20)]
        public int STT { get; set; }

        [Width(20)]
        public string SoBLHD { get; set; }

        [Width(20)]
        public string MaBN { get; set; }

        [Width(40)]
        public string HoTen { get; set; }

        [Width(20)]
        public string NgayThuStr { get; set; }

        [Width(20)]
        public string NguoiThu { get; set; }

        [Width(20)]
        public string LyDoHuy { get; set; }

        [Width(20)]
        public string ThuChiTienBenhNhanStr { get; set; }

        [Width(25)]
        [TextAlign(Constants.TextAlignAttribute.Right)]
        public string SoTienThu { get; set; }

        [Width(25)]
        [TextAlign(Constants.TextAlignAttribute.Right)]
        public string TienMat { get; set; }

        [Width(25)]
        [TextAlign(Constants.TextAlignAttribute.Right)]
        public string ChuyenKhoan { get; set; }

        [Width(25)]
        [TextAlign(Constants.TextAlignAttribute.Right)]
        public string Pos { get; set; }
    }
}
