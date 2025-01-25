using System.ComponentModel;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum EnumTinhHinhPhauThuatThuThuat
        {
            [Description("Chủ động")]
            ChuDong = 1,
            [Description("Cấp cứu")]
            CapCuu = 2
        }
    }
}
