using System.ComponentModel;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum EnumKieuKham
        {
            [Description("Nội Trú")]
            NoiTru = 0,

            [Description("Ngoại Trú")]
            NgoaiTru = 1
        }
    }
}