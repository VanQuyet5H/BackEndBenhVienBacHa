using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum EnumNhomDieuTriDuPhong
        {
            [Description("Điều trị")]
            DieuTri = 1,
            [Description("Dự phòng")]
            DuPhong = 2
        }
    }
}
