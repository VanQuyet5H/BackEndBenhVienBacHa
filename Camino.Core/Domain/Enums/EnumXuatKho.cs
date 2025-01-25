using System.ComponentModel;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum XuatKhoDuocPham
        {
            [Description("Xuất qua kho khác")]
            XuatQuaKhoKhac = 1,
            [Description("Xuất trả nhà cung cấp")]
            XuatTraNhaCungCap = 2,
            [Description("Xuất cho người bệnh")]
            XuatChoBenhNhan = 3,
            [Description("Xuất hủy")]
            XuatHuy = 4,

            [Description("Xuất khác")]
            XuatKhac = 5,
        }

        public enum LoaiNguoiGiaoNhan
        {
            [Description("Trong hệ thống")]
            TrongHeThong = 1,
            [Description("Ngoài hệ thống")]
            NgoaiHeThong = 2,
        }
    }
}