using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.GoiDichVu
{
    public class GoiDichVuChiTietDichVuKyThuatGridVo : GridItem
    {
        public string DichVuKyThuatBenhVien { get; set; }

        public string NhomGiaDichVuKyThuatBenhVien { get; set; }

        public int SoLan { get; set; }
    }
}
