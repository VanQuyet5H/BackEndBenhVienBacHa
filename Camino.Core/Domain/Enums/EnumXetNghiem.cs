using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum EnumLoaiMauXetNghiem
        {
            [Description("Máu")]
            Mau = 1,
            [Description("Nước Tiểu")]
            NuocTieu = 2,
            [Description("Đờm")]
            Dom = 3,
            [Description("Phân")]
            Phan = 4,
            [Description("Dịch")]
            Dich = 5,
            [Description("Khác")]
            Khac = 6,
            [Description("Dịch tỵ hầu")]
            DichTyHau = 7,
        }
        public enum EnumKetQuaXetNghiem
        {
            [Description("Âm tính")]
            AmTinh = 1,
            [Description("Dương tính")]
            DuongTinh = 2
        }

        public enum EnumNumber
        {
            [Description("int")]
            Integer = 1,
            [Description("double")]
            Double = 2,
        }
    }
}
