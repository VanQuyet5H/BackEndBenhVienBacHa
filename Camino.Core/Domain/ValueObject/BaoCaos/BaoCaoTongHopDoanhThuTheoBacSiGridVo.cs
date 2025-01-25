using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.BaoCaos
{
    public class BaoCaoTongHopDoanhThuTheoBacSiGridVo : GridItem
    {
        public long Stt { get; set; }

        public string HoTenBacSi { get; set; }

        public decimal? DoanhThu { get; set; }

        public decimal? MienGiam { get; set; }

        public decimal? KhoanGiamTruKhac { get; set; }

        public decimal? Bhyt { get; set; }

        public decimal ThucThu => DoanhThu.GetValueOrDefault() - MienGiam.GetValueOrDefault() -
                                  KhoanGiamTruKhac.GetValueOrDefault() - Bhyt.GetValueOrDefault();
    }
}
