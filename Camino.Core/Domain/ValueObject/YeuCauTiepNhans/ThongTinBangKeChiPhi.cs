using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Camino.Core.Domain.ValueObject.YeuCauTiepNhans
{
    public enum LoaiBangKeChiPhi
    {
        [Description("Xem Trước Bảng Kê")]
        XemTruocBangKe = 1,
        [Description("Bảng Kê Chờ Thu")]
        BangKeChoThu = 2,
        [Description("Bảng Kê Đã Thu")]
        BangKeDaThu = 3
    }
}
