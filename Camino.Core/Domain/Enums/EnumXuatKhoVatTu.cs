using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum EnumLoaiXuatKho
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
    }
}
