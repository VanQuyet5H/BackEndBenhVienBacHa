using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.BaoCaos
{
    public class BaoCaoTongHopDoanhThuTheoKhoaPhongGridVo : GridItem
    {
        public long Stt { get; set; }

        public string KhoaPhong { get; set; }


        public decimal? DoanhThuTheoThang { get; set; }

        public decimal? MienGiamTheoThang { get; set; }

        public decimal? ChiPhiKhacTheoThang { get; set; }

        public decimal? BhytTheoThang { get; set; }

        public decimal? DoanhThuThuanTheoThang => DoanhThuTheoThang.GetValueOrDefault() -
                                                  MienGiamTheoThang.GetValueOrDefault() -
                                                  ChiPhiKhacTheoThang.GetValueOrDefault() -
                                                  BhytTheoThang.GetValueOrDefault();


        public decimal? DoanhThuTheoKySoSanh { get; set; }
                                    
        public decimal? MienGiamTheoKySoSanh { get; set; }

        public decimal? ChiPhiKhacTheoKySoSanh { get; set; }

        public decimal? BhytTheoKySoSanh { get; set; }

        public decimal? DoanhThuThuanTheoKySoSanh => DoanhThuTheoKySoSanh.GetValueOrDefault() -
                                                     MienGiamTheoKySoSanh.GetValueOrDefault() -
                                                     ChiPhiKhacTheoKySoSanh.GetValueOrDefault() -
                                                     BhytTheoKySoSanh.GetValueOrDefault();
    }
}
