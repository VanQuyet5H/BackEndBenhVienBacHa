using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.BaoCaos
{
    public class BaoCaoChiTietDoanhThuTheoKhoaPhongMasterGridVo : GridItem
    {
        public string KhoaPhong { get; set; }

        public decimal? ToTalDoanhThuTheoThang { get; set; }

        public decimal? ToTalMienGiamTheoThang { get; set; }

        public decimal? ToTalChiPhiKhacTheoThang { get; set; }

        public decimal? ToTalBhytTheoThang { get; set; }

        public decimal? ToTalDoanhThuThuanTheoThang => ToTalDoanhThuTheoThang.GetValueOrDefault() -
                                                  ToTalMienGiamTheoThang.GetValueOrDefault() -
                                                  ToTalChiPhiKhacTheoThang.GetValueOrDefault() -
                                                  ToTalBhytTheoThang.GetValueOrDefault();


        public decimal? ToTalDoanhThuTheoKySoSanh { get; set; }

        public decimal? ToTalMienGiamTheoKySoSanh { get; set; }

        public decimal? ToTalChiPhiKhacTheoKySoSanh { get; set; }

        public decimal? ToTalBhytTheoKySoSanh { get; set; }

        public decimal? ToTalDoanhThuThuanTheoKySoSanh => ToTalDoanhThuTheoKySoSanh.GetValueOrDefault() -
                                                     ToTalMienGiamTheoKySoSanh.GetValueOrDefault() -
                                                     ToTalChiPhiKhacTheoKySoSanh.GetValueOrDefault() -
                                                     ToTalBhytTheoKySoSanh.GetValueOrDefault();
    }
}
