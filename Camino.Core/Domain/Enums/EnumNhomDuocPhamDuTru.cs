using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum EnumNhomDuocPhamDuTru
        {
            [Description("Thuốc - vacxin")]
            ThuocVacxin = 1,
            [Description("Hóa chất - Hóa chất XN")]
            HoaChatXN = 2,
        }
    }
}
