using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.BaoCao
{
    public class BaoCaoDanhThuTheoBacSiViewModel
    {
        public int STT { get; set; }
        public string MaTT { get; set; }
        public string NgayFormat { get; set; }
        public string MaBN { get; set; }
        public string HoVaTenBenhNhan { get; set; }
        public string TenDichVuChiDinh { get; set; }
        public decimal? MienGiam { get; set; }
        public decimal? DoanhThu { get; set; }
        public decimal? Khac { get; set; }
        public decimal? BHYT { get; set; }
        public decimal? DoanhThuThuan { get; set; }
    }   
}
