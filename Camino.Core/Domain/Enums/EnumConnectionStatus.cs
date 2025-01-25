using System.ComponentModel;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum EnumConnectionStatus
        {
            [Description("Mở")]
            Open = 1,
            [Description("Đóng")]
            Close = 2
        }
    }
}
