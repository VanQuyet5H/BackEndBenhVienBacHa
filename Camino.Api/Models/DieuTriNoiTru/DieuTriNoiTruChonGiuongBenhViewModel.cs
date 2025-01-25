using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.DieuTriNoiTru
{
    public class DieuTriNoiTruChonGiuongBenhViewModel
    {
        public long? GiuongId { get; set; }
        public bool? BaoPhong { get; set; }
        public DateTime ThoiGianNhan { get; set; }
        public DateTime? ThoiGianTra { get; set; }
        public long? YeuCauDichVuGiuongBenhVienId { get; set; }
        public long? YeuCauTiepNhanNoiTruId { get; set; }
    }
}
