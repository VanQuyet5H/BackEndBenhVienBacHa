using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject
{
    public class ChucDanhGridVo : GridItem
    {
        public string Ten { get; set; }
        public string Ma { get; set; }
        //public long NhomChucDanhId { get; set; }
        public string  MoTa { get; set; }
        public bool? IsDisabled { get; set; }
        public bool? IsDefault { get; set; }
        public string TenNhomChucDanh { get; set; }
        public bool? HideCheckbox { get; set; }
    }
}
