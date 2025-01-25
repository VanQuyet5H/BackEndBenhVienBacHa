using System;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.BaoCaos
{
    public class BaoCaoChiTietDoanhThuTheoBacSiGridVo : GridItem
    {
        public long Stt { get; set; }

        public string MaTn { get; set; }

        public DateTime NgayThu { get; set; }
        public string Ngay => NgayThu.ApplyFormatDate();

        public string MaBn { get; set; }

        public string HoTenBn { get; set; }

        public string DichVuChiDinh { get; set; }

        public decimal? DoanhThuTheoThang { get; set; }

        public decimal? MienGiamTheoThang { get; set; }

        public decimal? CacKhoanGiamKhacTheoThang { get; set; }

        public decimal? BhytTheoThang { get; set; }

        public decimal? DoanhThuThuanTheoThang => DoanhThuTheoThang.GetValueOrDefault() -
                                                  MienGiamTheoThang.GetValueOrDefault() -
                                                  CacKhoanGiamKhacTheoThang.GetValueOrDefault() -
                                                  BhytTheoThang.GetValueOrDefault();
    }
}
