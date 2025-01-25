using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.PhauThuatThuThuat
{
    public class PhauThuatThuThuatThuePhongViewModel
    {
        public long YeuCauTiepNhanId { get; set; }
        public long YeuCauDichVuKyThuatId { get; set; }
        public long? ThuePhongId { get; set; }
        public long? CauHinhThuePhongId { get; set; }
        public bool? CoThuePhong { get; set; }
        public DateTime? ThoiDiemBatDau { get; set; }
        public DateTime? ThoiDiemKetThuc { get; set; }
    }

    public class PhauThuatThuThuatLichSuThuePhongViewModel
    {
        public long YeuCauTiepNhanId { get; set; }
        public long? YeuCauDichVuKyThuatId { get; set; }
        public long? ThuePhongId { get; set; }
        public long? CauHinhThuePhongId { get; set; }
        public bool? CoThuePhong { get; set; }
        public DateTime? ThoiDiemBatDau { get; set; }
        public DateTime? ThoiDiemKetThuc { get; set; }
    }
}
