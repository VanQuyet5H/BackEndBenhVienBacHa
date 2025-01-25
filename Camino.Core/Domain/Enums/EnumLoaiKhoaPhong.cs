using System.ComponentModel;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum EnumLoaiKhoaPhong
        {
            [Description("Chức Năng")]
            ChucNang = 1,

            [Description("Lâm Sàng")]
            LamSang = 2,

            [Description("Cận Lâm Sàng")]
            CanLamSang = 3
        }
    }
}
