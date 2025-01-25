using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.TrieuChungs
{
    public class TrieuChungGridVo : GridItem
    {
        public string Ten { get; set; }
        public long? TrieuChungChaId { get; set; }
        public int CapNhom { get; set; }
        public List<TrieuChungGridVo> TrieuChungChildren { get; set; }
    }
}
