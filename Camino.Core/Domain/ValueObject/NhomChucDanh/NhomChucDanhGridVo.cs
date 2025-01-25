using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.NhomChucDanh
{
    public class NhomChucDanhGridVo : GridItem
    {
        public string Ten { get; set; }
        public string Ma { get; set; }
        public string MoTa { get; set; }
    }
}
