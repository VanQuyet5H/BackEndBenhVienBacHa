using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject
{
    public class NoiDungMauLoiDanBacSiGridVo : GridItem
    {
        public string Ma { get; set; }
        public int? LoaiBenhAn { get; set; }
        public string Ten { get; set; }
        public string NoiDung { get; set; }
    }
}
