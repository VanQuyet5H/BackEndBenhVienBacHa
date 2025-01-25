using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.DieuTriNoiTrus
{
    public class NoiTruHoSoKhacFileDinhKem : TaiLieuDinhKemEntity
    {
        public long NoiTruHoSoKhacId { get; set; }

        public virtual NoiTruHoSoKhac NoiTruHoSoKhac { get; set; }
    }
}
