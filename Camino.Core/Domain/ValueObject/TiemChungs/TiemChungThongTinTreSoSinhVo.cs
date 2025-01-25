using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.TiemChungs
{
    public class TiemChungThongTinTreSoSinhVo
    {
        public TiemChungThongTinTreSoSinhVo()
        {
            DacDiemTreSoSinhs = new List<DacDiemTreSoSinh>();
        }

        public List<DacDiemTreSoSinh> DacDiemTreSoSinhs { get; set; }
    }

    public class DacDiemTreSoSinh : GridItem
    {
        public long? YeuCauTiepNhanConId { get; set; }
        public DateTime? DeLuc { get; set; }
    }
}
