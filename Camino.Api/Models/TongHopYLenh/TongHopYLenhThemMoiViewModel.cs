using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.TongHopYLenh
{
    public class TongHopYLenhThemMoiViewModel
    {
        //public long PhieuDieuTriId { get; set; }
        public long NoiTruBenhAnId { get; set; }
        public string NgayYLenh { get; set; }
        public int? GioYLenh { get; set; }
        public string DienBien { get; set; }
        public string MoTaYLenh { get; set; }
        public long? NhanVienChiDinhId { get; set; }
        public string NhanVienChiDinhDisplay { get; set; }
        public long? NoiChiDinhId { get; set; }
        public long? NhanVienXacNhanThucHienId { get; set; }
        public string NhanVienXacNhanThucHienDisplay { get; set; }
        public DateTime? ThoiDiemXacNhanThucHien { get; set; }
        public int? GioThucHien { get; set; }
        public bool? XacNhanThucHien { get; set; }
    }
}
