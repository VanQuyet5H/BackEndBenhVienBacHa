using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.NguoiGioiThieu
{
    public class NguoiGioiThieuGridVo : GridItem
    {
        public string HoTen { get; set; }
        public string SoDienThoai { get; set; }
        public long NhanVienQuanLyId { get; set; }
        public string HoTenNhanVienQuanLy { get; set; }
        public string SoDienThoaiDisplay { get; set; }

    }

    public class NguoiQuanLyTemplateVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string HoTen { get; set; }
        public string SoDienThoai { get; set; }
    }
}
