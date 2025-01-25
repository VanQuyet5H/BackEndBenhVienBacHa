using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum TrangThaiLayMauXetNghiem
        {
            [Description("Chờ lấy mẫu")]
            ChoLayMau = 1,
            [Description("Đã cấp barcode")]
            ChoGuiMau = 2,
            [Description("Chờ KQ")]
            ChoKetQua = 3,
            [Description("Đã có KQ")]
            DaCoKetQua = 4
        }

        public enum TrangThaiLayMauXetNghiemNew
        {
            [Description("Chờ lấy mẫu")]
            ChoCapCode = 1,
            [Description("Chờ nhận mẫu")]
            ChoNhanMau = 2,
            [Description("Chờ KQ")]
            ChoKetQua = 3,
            [Description("Đã có KQ")]
            DaCoKetQua = 4,
            [Description("Đã duyệt")]
            DaDuyet = 5,
        }
    }
}
