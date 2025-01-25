using System;

namespace Camino.Api.Models.KyDuTru
{
    public class KyDuTruViewModel : BaseViewModel
    {
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public long NhanVienTaoId { get; set; }
        public bool? MuaDuocPham { get; set; }
        public bool? MuaVatTu { get; set; }
        public bool HieuLuc { get; set; }
        public string MoTa { get; set; }
        public DateTime? NgayBatDauLap { get; set; }
        public DateTime? NgayKetThucLap { get; set; }
    }
}
