using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum EnumLoaiNhapKho
        {
            [Description("Nhập từ nhà cung cấp")]
            NhapTuNhaCungCap = 1,
            [Description("Nhập từ kho khác")]
            NhapTuKhoKhac = 2,
        }
    }
}
