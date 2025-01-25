using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.NoiGioiThieu
{
    public class NoiGioiThieuGridVo : GridItem
    {
        public string Ten { get; set; }
        public string MoTa { get; set; }
        public string SoDienThoai { get; set; }
        public string DonVi { get; set; }
        public long? NhanVienQuanLyId { get; set; }
        public string HoTenNguoiQuanLy { get; set; }
        public bool? IsDisabled { get; set; }
        public string SoDienThoaiDisplay { get; set; }

    }

    public class DonViMauGridVo : GridItem
    {
        public string Ten { get; set; }
        public string Ma { get; set; }
        public bool IsDefault { get; set; }
    }
}
