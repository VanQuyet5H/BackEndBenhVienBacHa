using System.ComponentModel;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum EnumKhoaPhong
        {
            [Description("Phòng Tài chính kế toán")]
            PTCKT = 23,
            [Description("Khoa GMHS")]
            KhoaGMHS = 2,
            [Description("Khoa Xét Nghiệm")]
            KhoaXetNghiem = 12,
            [Description("Khoa Phụ Sản")]
            KhoaPhuSan = 8,
            [Description("Khoa Thẩm Mỹ")]
            KhoaThamMy = 9
        }
    }
}