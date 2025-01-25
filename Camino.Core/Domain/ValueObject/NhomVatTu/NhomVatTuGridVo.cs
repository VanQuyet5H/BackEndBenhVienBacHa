using Camino.Core.Domain.ValueObject.Grid;
using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.NhomVatTu
{
    public class NhomVatTuGridVo  :GridItem
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public long? NhomVatTuChaId { get; set; }
        public int CapNhom { get; set; }
        public virtual List<NhomVatTuGridVo> ListNhomVatTuChildren { get; set; }
        
    }
}
