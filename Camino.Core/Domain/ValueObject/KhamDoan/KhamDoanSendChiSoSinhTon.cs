using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KetQuaSinhHieus;

namespace Camino.Core.Domain.ValueObject.KhamDoan
{
    public class KhamDoanSendChiSoSinhTon : GridItem
    {
        public KhamDoanSendChiSoSinhTon()
        {
            data = new List<KetQuaSinhHieuGridVo>();
        }

        public List<KetQuaSinhHieuGridVo> data { get; set; }
    }
}
