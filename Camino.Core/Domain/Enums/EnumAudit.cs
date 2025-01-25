using System.ComponentModel;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum EnumAudit
        {
            [Description("Thêm")]
            Added = 4,
            [Description("Xóa")]
            Deleted = 2,
            [Description("Cập nhật")]
            Modified = 3,
        }
    }
}