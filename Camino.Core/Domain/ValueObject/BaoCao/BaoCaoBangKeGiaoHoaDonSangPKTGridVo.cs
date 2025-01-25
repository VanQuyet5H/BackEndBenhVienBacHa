using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.BaoCao
{
    public class BaoCaoBangKeGiaoHoaDonSangPKTGridVo: GridItem
    {
        public string SoHD { get; set; }
        public DateTime NgayHD { get; set; }
        public string NgayHDStr => NgayHD.ApplyFormatDateTimeSACH();
        public decimal SoTienTT { get; set; }

        public long? NCCId { get; set; }
        public string TenNCC { get; set; }
    }
}
