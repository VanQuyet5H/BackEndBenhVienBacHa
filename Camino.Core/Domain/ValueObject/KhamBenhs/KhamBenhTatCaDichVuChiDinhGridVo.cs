using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.KhamBenhs
{
    public class KhamBenhTatCaDichVuChiDinhGridVo : GridItem
    {
        public string TenDichVu { get; set; }
        public string Nhom { get; set; }
        public Enums.EnumNhomGoiDichVu NhomId { get; set; }
    }
}
