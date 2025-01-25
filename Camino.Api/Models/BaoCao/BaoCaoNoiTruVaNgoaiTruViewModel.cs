using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.BaoCao
{
    public class BaoCaoNoiTruViewModel
    {
        public int STT { get; set; }
        public string HoVaTen { get; set; }
        public decimal? TongVienPhiDieuTri { get; set; }
        public string Cong { get; set; }
        public string ThuocVaVatTu { get; set; }
        public decimal? TyLeMienGiam { get; set; }
        public decimal? SoTienMienGiam { get; set; }
        public string DoiTuongMienGiam { get; set; }
        public string GhiChu { get; set; }
    }
    public class BaoCaoNgoaiTruViewModel
    {
        public int STT { get; set; }
        public string HoVaTen { get; set; }
        public decimal? TongVienPhiDieuTri { get; set; }
        public string Cong { get; set; }
        public string ThuocVaVatTu { get; set; }
        public decimal? TyLeMienGiam { get; set; }
        public decimal? SoTienMienGiam { get; set; }
        public string DoiTuongMienGiam { get; set; }
        public string GhiChu { get; set; }
    }
}
