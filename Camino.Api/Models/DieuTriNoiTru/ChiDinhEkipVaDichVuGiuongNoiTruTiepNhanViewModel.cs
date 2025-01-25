using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.DieuTriNoiTru
{
    public class ChiDinhEkipVaDichVuGiuongNoiTruTiepNhanViewModel
    {
        public long YeuCauTiepNhanId { get; set; }
        public long? BacSiDieuTriId { get; set; }
        public long? DieuDuongId { get; set; }
        public DateTime? TuNgay { get; set; }
        public long? DichVuGiuongId { get; set; }
        public long? GiuongId { get; set; }
        public string TenGiuong { get; set; }
        public long? LoaiGiuong { get; set; }
        public bool? BaoPhong { get; set; }
        public DateTime? ThoiGianNhan { get; set; }
        public bool? KhongCanChiDinhGiuong { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }
    }
}
