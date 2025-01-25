using System.ComponentModel;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum TrangThaiLishSu
        {
            [Description("Tất cả")]
            TatCa = 0,
            [Description("Thành công")]
            ThanhCong = 1,
            [Description("Thất bại")]
            ThatBai = 2,
        }
    }

}
