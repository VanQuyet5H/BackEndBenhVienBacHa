using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Core.Domain.ValueObject.KetQuaCLS
{
    public class KetQuaGoiLaiXetNghiem
    {
        public string LyDo { get; set; }       
        public List<DanhSachGoiXetNghiemLai> DanhSachGoiXetNghiemLais { get; set; }
    }

    public class DanhSachGoiXetNghiemLai
    {
        public bool GoiLai { get; set; }
        public long NhomDichVuBenhVienId { get; set; }
        public long PhienXetNghiemId { get; set; }
        public string NhomDichVu { get; set; }
        public int LanThucHien { get; set; }
    }
}
