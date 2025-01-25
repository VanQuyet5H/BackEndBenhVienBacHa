using System.ComponentModel;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum EnumLoaiThoiGianPhanCong
        {
            [Description("Sáng")]
            Sang = 1,

            [Description("Chiều")]
            Chieu = 2
        }
    }
}