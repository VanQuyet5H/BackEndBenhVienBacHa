using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.TiemChungs
{
    public class GridChiDinhVuKyThuatTiemChungQueryInfoVo
    {
        public long YeuCauTiepNhanId { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public int? NhomDichVuId { get; set; }
        public bool? IsKhamDoanTatCa { get; set; }
        public long? YeuCauKhamSangLocId { get; set; }
    }
}
