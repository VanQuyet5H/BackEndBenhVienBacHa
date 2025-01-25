using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.ICDs
{
    public class ICDDoiTuongBenhNhanChiTiet : BaseEntity
    {
        public long ICDId { get; set; }
        public long ICDDoiTuongBenhNhanId { get; set; }

        public virtual ICD ICD { get; set; }
        public virtual ICDDoiTuongBenhNhan ICDDoiTuongBenhNhan { get; set; }
    }
}
