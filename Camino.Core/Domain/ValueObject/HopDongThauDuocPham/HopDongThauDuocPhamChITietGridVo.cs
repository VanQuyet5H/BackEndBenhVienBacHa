using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.HopDongThauDuocPham
{
    public class HopDongThauDuocPhamChiTietGridVo : GridItem
    {
        public string DuocPham { get; set; }

        public decimal? Gia { get; set; }
        
        public string GiaDisplay { get; set; }

        public string GiaBaoHiemDisplay { get; set; }

        public string SoLuongDisplay { get; set; }

        public string SoLuongCungCapDisplay { get; set; }
    }
}
