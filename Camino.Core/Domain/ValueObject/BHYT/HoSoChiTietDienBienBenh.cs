using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.BHYT
{
    public class HoSoChiTietDienBienBenh
    {
        public string MaLienKet { get; set; }
        public int? STT { get; set; }
        public string DienBien { get; set; }
        public string HoiChuan { get; set; }
        public string PhauThuat { get; set; }
        public DateTime NgayYLenh { get; set; }//old:public string NgayYLenh { get; set; }
                                               //old:public DateTime? NgayYLenhTime { get; set; }
    }
}
