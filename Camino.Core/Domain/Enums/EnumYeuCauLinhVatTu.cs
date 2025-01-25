using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum EnumLoaiPhieuLinh
        {
            [Description("Lĩnh dự trù")]
            LinhDuTru = 1,
            [Description("Lĩnh bù")]
            LinhBu = 2,
            [Description("Lĩnh cho người bệnh")]
            LinhChoBenhNhan = 3,
        }

        public enum EnumTrangThaiPhieuLinh
        {
            [Description("Đang chờ gửi")]
            DangChoGui = 1,
            [Description("Đang chờ duyệt")]
            DangChoDuyet = 2,
            [Description("Đã duyệt")]
            DaDuyet = 3,
            [Description("Từ chối duyệt")]
            TuChoiDuyet = 4,
        }
    }
}
