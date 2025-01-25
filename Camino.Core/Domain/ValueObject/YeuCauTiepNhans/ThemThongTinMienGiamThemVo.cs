using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.YeuCauTiepNhans
{
    public class ThemThongTinMienGiamThemVo
    {
        public long YeuCauTiepNhanId { get; set; }       
        public decimal SoTienMG { get; set; }
        public int TiLeMienGiam { get; set; }
        public decimal SoTienMGConLai { get; set; }
        public string LyDoMiemGiam { get; set; }
        //public Enums.LoaiMienGiamThem LoaiMienGiamThem { get; set; }
        public TaiLieuDinhKemGiayMiemGiam TaiLieuDinhKemGiayMiemGiam { get; set; }
    }   
}
