using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.KetQuaSinhHieu
{
    public class KetQuaSinhHieuGridVo : GridItem
    {
        public long YeuCauTiepNhanId { get; set; }

        public int? NhipTim { get; set; }

        public int? NhipTho { get; set; }

        public string HuyetAp { get; set; }
        public double? ThanNhiet { get; set; }

        public double? ChieuCao { get; set; }

        public double? CanNang { get; set; }

        public double? Bmi { get; set; }

        public long? NoiThucHienId { get; set; }

        public long NhanVienThucHienId { get; set; }
        public string NhanVienThucHien { get; set; }
        public string NgayThucHien { get; set; }
        public double? Glassgow { get; set; }
        // update 13/8/2020
        public double? SpO2 { get; set; }
    }
}
