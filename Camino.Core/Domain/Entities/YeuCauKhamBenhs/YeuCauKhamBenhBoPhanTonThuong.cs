using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.YeuCauKhamBenhs
{
    public class YeuCauKhamBenhBoPhanTonThuong : BaseEntity
    {
        public long YeuCauKhamBenhId { get; set; }
        public string MoTa { get; set; }
        public string HinhAnh { get; set; }

        public virtual YeuCauKhamBenh YeuCauKhamBenh { get; set; }
    }
}
