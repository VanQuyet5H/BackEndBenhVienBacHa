using System.ComponentModel;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum EnumLoaiChuanDoanHinhAnh
        {
            [Description("Chụp X quang")]
            ChupXQuang = 1,

            [Description("Siêu âm")]
            SieuAm = 2,

            [Description("Chụp CT")]
            ChupCT = 3,

            [Description("Cộng hưởng từ")]
            CongHuongTu = 4,

            [Description("Điện não")]
            DienNao = 5,

            [Description("Nội soi")]
            NoiSoi = 6,

            [Description("Khác")]
            Khac = 7,

            [Description("Siêu âm màu")]
            SieuAmMau = 8,

            [Description("Siêu âm đầu dò")]
            SieuAmDauDo = 9,

            [Description("Siêu âm màu 4D")]
            SieuAmMau4D = 10,

            [Description("Lưu huyết não")]
            LuuHuyetNao = 11
        }
    }
}
