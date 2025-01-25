using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum TrangThaiDonThuocThanhToan
        {
            [Description("Chưa xuất thuốc")]
            ChuaXuatThuoc = 1,
            [Description("Đã xuất thuốc")]
            DaXuatThuoc = 2,
            [Description("Đã hủy")]
            DaHuy = 3
        }
        public enum TrangThaiDonVTYTThanhToan
        {
            [Description("Chưa xuất vật tư")]
            ChuaXuatVTYT = 1,
            [Description("Đã xuất vật tư")]
            DaXuatVTYT = 2,
            [Description("Đã hủy")]
            DaHuy = 3
        }

    }
}
