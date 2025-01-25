using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.DanToc
{
   public  class DanTocGridVo : GridItem
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public long QuocGiaId { get; set; }
        public string TenQuocGia { get; set; }
        public bool? IsDisabled { get; set; }
    }
}
