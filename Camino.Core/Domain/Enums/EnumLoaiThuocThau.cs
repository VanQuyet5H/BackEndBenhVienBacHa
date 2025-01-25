using System.ComponentModel;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum EnumLoaiThuocThau
        {
            [Description("Tổng hợp")]
            TongHop = 1,
            [Description("Tân dược")]
            TanDuoc = 2,
            [Description("Chế phẩm")]
            ChePham = 3,
            [Description("Vị thuốc")]
            ViThuoc = 4,
            [Description("Phóng xạ")]
            PhongXa = 5
        }
    }
}