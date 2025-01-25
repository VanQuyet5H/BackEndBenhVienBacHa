using System.ComponentModel;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum EnumLoaiCongTy
        {
            [Description("Tư nhân")]
            TuNhan = 1,

            [Description("Nhà nước")]
            NhaNuoc = 2
        }
    }
}
