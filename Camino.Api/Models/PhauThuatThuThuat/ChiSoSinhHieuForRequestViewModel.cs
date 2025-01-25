using System;
using System.Collections.Generic;

namespace Camino.Api.Models.PhauThuatThuThuat
{
    public class ChiSoSinhHieuForRequestViewModel
    {
        public long YctnId { get; set; }

        public List<ChiSoSinhHieuViewModel> ChiSoSinhHieus { get; set; }
    }

    public class ChiSoSinhHieuViewModel : BaseViewModel
    {
        public int? NhipTim { get; set; }

        public int? NhipTho { get; set; }

        public int? HuyetApTamThu { get; set; }

        public int? HuyetApTamTruong { get; set; }

        public double? ThanNhiet { get; set; }

        public double? ChieuCao { get; set; }

        public double? CanNang { get; set; }

        public double? Bmi { get; set; }

        public double? Glassgow { get; set; }

        public double? SpO2 { get; set; }

        public long? NoiThucHienId { get; set; }

        public long? NhanVienThucHienId { get; set; }

        public DateTime? NgayThucHien { get; set; }
    }
}
