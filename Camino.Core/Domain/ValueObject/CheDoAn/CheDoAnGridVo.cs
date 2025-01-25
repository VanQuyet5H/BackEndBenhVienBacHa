using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.CheDoAn
{
    public class CheDoAnGridVo : GridItem
    {
        public string Ten { get; set; }
        public string KyHieu { get; set; }
        public string  MoTa { get; set; }
        public bool? IsDisabled { get; set; }
    }
}
