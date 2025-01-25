using System.ComponentModel;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum EnumTaiBienPTTT
        {
            [Description("Không")]
            Khong = 1,
            [Description("Do PT")]
            DoPT = 2,
            [Description("Do gây mê")]
            DoGayMe = 3,
            [Description("Do nhiễm khuẩn")]
            DoNhiemKhuan = 4,
            [Description("Khác")]
            Khac = 5
        }
    }
}
