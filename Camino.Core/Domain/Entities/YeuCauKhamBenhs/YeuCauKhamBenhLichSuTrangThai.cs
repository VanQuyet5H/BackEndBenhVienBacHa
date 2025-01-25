using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.YeuCauKhamBenhs
{
    public class YeuCauKhamBenhLichSuTrangThai: BaseEntity
    {
        public long YeuCauKhamBenhId { get; set; }
        public Enums.EnumTrangThaiYeuCauKhamBenh TrangThaiYeuCauKhamBenh { get; set; }
        public string MoTa { get; set; }
        public virtual YeuCauKhamBenh YeuCauKhamBenh { get; set; }
    }
}
