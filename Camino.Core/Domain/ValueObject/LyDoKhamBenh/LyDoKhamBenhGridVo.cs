using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.LyDoKhamBenh
{
   public class LyDoKhamBenhGridVo : GridItem
    {
        public string Ten { get; set; }
        public string TenVietTat { get; set; }
        public string MoTa { get; set; }
        public bool? IsDisabled { get; set; }
    }
}
