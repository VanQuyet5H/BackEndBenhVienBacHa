using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.DichVuCanLamSang
{
    public class DichVuCanLamSangGridVo :GridItem
    {
        public string Ten { get; set; }
        public string Ma { get; set; }
        public string MoTa { get; set; }
        public bool? IsDisabled { get; set; }
    }
}
