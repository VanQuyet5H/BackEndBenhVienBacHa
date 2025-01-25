using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum EnumNhomDichVu
        {
            [Description("Dịch vụ khám bệnh")]
            KhamBenh = 1,
            [Description("Dịch vụ kỹ thuật")]
            KyThuat = 2,
            [Description("Dịch vụ giường bệnh")]
            GiuongBenh = 3,
            [Description("Thuốc")]
            DuocPham = 4,
            [Description("Vật tư")]
            VatTu = 5
        }
    }
}
