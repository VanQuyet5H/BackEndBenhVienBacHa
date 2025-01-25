using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.YeuCauTiepNhans
{
    public class ThongTinMienGiamThemVo
    {
        //public Enums.LoaiMienGiamThem LoaiMienGiamThem { get; set; }
        public decimal SoTienMG { get; set; }
        public int TiLeMienGiam { get; set; }
        public decimal SoTienMGConLai { get; set; }
        public string LyDoMiemGiam { get; set; }
        public long? NhanVienDuyetMienGiamThemId { get; set; }
        public string TenNhanVienDuyet { get; set; }
        public TaiLieuDinhKemGiayMiemGiam TaiLieuDinhKemGiayMiemGiam { get; set; }
    }
    public class TaiLieuDinhKemGiayMiemGiam
    {
        public long Id { get; set; }

        public string Ma { get; set; }

        public string Ten { get; set; }

        public string TenGuid { get; set; }

        public string DuongDan { get; set; }

        public long KichThuoc { get; set; }

        public int LoaiTapTin { get; set; }

        public string MoTa { get; set; }
    }
}
