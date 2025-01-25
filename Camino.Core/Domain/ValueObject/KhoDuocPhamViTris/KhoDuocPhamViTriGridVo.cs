using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.KhoDuocPhamViTris
{
    public class KhoDuocPhamViTriGridVo : GridItem
    {
        public long KhoDuocPhamId { get; set; }
        public string Ten { get; set; }
        public string MoTa { get; set; }
        public bool? IsDisabled { get; set; }
       
        public string Kho { get; set; }
        
    }

    public class LookupItemViTriVo: LookupItemVo
    {
        public long KhoDuocPhamId { get; set; }
    }
}
