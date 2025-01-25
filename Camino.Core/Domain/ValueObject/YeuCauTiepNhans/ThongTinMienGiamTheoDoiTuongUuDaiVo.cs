using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.YeuCauTiepNhans
{
    public class ThongTinMienGiamTheoDoiTuongUuDaiVo
    {
        public string CongTyUudai { get; set; }
        public string DoiTuongUuDai { get; set; }
        public List<DichVuMiemGiamTheoTiLe> DichVuMiemGiamTheoTiLes { get; set; }
    }
}
