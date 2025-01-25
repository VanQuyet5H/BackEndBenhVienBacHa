using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum EnumLoaiChiSoXetNghiem
        {
            [Description("Nhóm Xét Nghiệm")]
            NhomXetNghiem = 1,
            [Description("Dịch Vụ Kỹ Thuật Bệnh Viện")]
            DVKTBenhVien = 2,
            [Description("Dịch Vụ Xét Nghiệm")]
            DVXetNghiem = 3,
        }
    }
   
}
