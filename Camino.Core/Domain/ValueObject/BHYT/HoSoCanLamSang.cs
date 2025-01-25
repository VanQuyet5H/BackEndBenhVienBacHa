using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.BHYT
{
    public class HoSoCanLamSang
    {
        public string MaLienKet { get; set; }
        public int? STT { get; set; }
        public string MaDichVu { get; set; }
        public string MaChiSo { get; set; }
        public string TenChiSo { get; set; }
        public string GiaTri { get; set; }
        public string MaMayXetNghiem { get; set; }
        public string MaMay { get; set; }
        public string MoTa { get; set; }
        public string KetLuan { get; set; }
        public DateTime NgayKQ { get; set; }//old:public string NgayKQ { get; set; }
                                            //old:public DateTime? NgayKQTime { get; set; }
        public long YeuCauDichVuKyThuatId { get; set; }//new
    }
}
