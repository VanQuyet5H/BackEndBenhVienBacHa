using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum NhomDichVuLoaiGia
        {
            [Description("DỊCH VỤ KHÁM BỆNH")]
            DichVuKhamBenh = 1,
            [Description("DỊCH VỤ KỸ THUẬT")]
            DichVuKyThuat = 2,
            [Description("DỊCH VỤ GIƯỜNG BỆNH")]
            DichVuGiuongBenh = 3
        }
    }
}
