using System.ComponentModel;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum EnumChauLuc
        {
            [Description("Bắc Mỹ")]
            BacMy = 1,

            [Description("Nam Mỹ")]
            NamMy = 2,

            [Description("Châu Nam Cực")]
            ChauNamCuc = 3,

            [Description("Châu Phi")]
            ChauPhi = 4,

            [Description("Châu Âu")]
            ChauAu = 5,

            [Description("Châu Á")]
            ChauA = 6,

            [Description("Châu Đại Dương")]
            ChauDaiDuong = 7
        }
    }
}
