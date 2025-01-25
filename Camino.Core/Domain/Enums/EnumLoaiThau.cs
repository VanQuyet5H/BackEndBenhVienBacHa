using System.ComponentModel;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum EnumLoaiThau
        {
            [Description("Thầu tập trung")]
            ThauTapTrung = 1,
            [Description("Thầu riêng")]
            ThauRieng = 2,
            [Description("Tự bào chế")]
            TuBaoChe = 3
        }
    }
}