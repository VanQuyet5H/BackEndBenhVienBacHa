using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.XetNghiems
{
    public class KetQuaTuMayXetNghiem
    {
        public KetQuaTuMayXetNghiem()
        {
            KetQuaTuMayXetNghiemChiTiets = new List<KetQuaTuMayXetNghiemChiTiet>();
        }
        public int BarCodeNumber { get; set; }
        public string MaMayXetNghiem { get; set; }
        public List<KetQuaTuMayXetNghiemChiTiet> KetQuaTuMayXetNghiemChiTiets { get ; set; }
    }
    public class KetQuaTuMayXetNghiemChiTiet
    {
        public string MaChiSo { get; set; }
        public string GiaTri { get; set; }
        public string DonVi { get; set; }
        public string ThoiGian { get; set; }
    }
}
