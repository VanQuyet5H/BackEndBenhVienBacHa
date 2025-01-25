using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum EnumNhatKyHeThong
        {
            [Description("Tất cả")]
            TatCa = 0,
            [Description("Đăng nhập")]
            DangNhap = 1,
            [Description("Đăng xuất")]
            DangXuat = 2,
            [Description("Thêm")]
            Them = 3,
            [Description("Cập nhật")]
            CapNhat = 4,
            [Description("Xóa")]
            Xoa = 5
        }
    }
}
