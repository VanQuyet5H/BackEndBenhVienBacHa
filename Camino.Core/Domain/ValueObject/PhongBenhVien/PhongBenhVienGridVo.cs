using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.PhongBenhVien
{
    public class PhongBenhVienGridVo : GridItem
    {
        public string Ten { get; set; }

        public string Ma { get; set; }

        public bool? IsDisabled { get; set; }

        public string TenKhoaPhong { get; set; }

        public bool? KieuKham { get; set; }
    }
}
