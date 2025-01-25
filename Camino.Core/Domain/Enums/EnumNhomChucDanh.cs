using System.ComponentModel;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum EnumNhomChucDanh
        {
            [Description("Bác sĩ")]
            BacSi = 1,
            [Description("Bác sĩ y học dự phòng")]
            BacSiDuPhong = 2,
            [Description("Y sĩ")]
            YSi = 3,
            [Description("Y tế công cộng")]
            YTeCongCong = 4,
            [Description("Điều dưỡng")]
            DieuDuong = 5,
            [Description("Hộ sinh")]
            HoSinh = 6,
            [Description("Kỹ thuật y tế")]
            KyThuatYTe = 7,
            [Description("Dược sĩ")]
            DuocSi = 8,
            [Description("Khác")]
            Khac = 9,
        }
    }
}