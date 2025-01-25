using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.HopDongThauVatTu
{
    public class HdtVatTuChiTietGridVo : GridItem
    {
        public string VatTu { get; set; }

        public decimal? Gia { get; set; }

        public double? SoLuong { get; set; }

        public string SoLuongDisplay => SoLuong.GetValueOrDefault().ApplyVietnameseFloatNumber();

        public double? SoLuongDaCap { get; set; }

        public string SoLuongCungCapDisplay => SoLuongDaCap.GetValueOrDefault().ApplyVietnameseFloatNumber();
    }
}
