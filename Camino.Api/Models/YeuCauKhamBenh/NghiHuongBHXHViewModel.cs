using System;

namespace Camino.Api.Models.YeuCauKhamBenh
{
    public class NghiHuongBHXHViewModel
    {
        public long? YeuCauKhamBenhId { get; set; }
        //public long? BacSiKetLuanId { get; set; }
        //public bool LaKhamBenh { get; set; }
        public DateTime? ThoiDiemTiepNhan { get; set; }
        public DateTime? DenNgay { get; set; }

        public long? ICDChinhNghiHuongBHYT { get; set; }
        public string TenICDChinhNghiHuongBHYT { get; set; }
        public string PhuongPhapDieuTriNghiHuongBHYT { get; set; }

    }
}
