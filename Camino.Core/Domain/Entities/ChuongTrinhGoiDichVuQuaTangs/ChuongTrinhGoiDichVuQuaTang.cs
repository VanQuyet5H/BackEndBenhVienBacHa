using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVus;
using Camino.Core.Domain.Entities.QuaTangs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.ChuongTrinhGoiDichVuQuaTangs
{
    public class ChuongTrinhGoiDichVuQuaTang : BaseEntity
    {
        public long ChuongTrinhGoiDichVuId { get; set; }
        public long QuaTangId { get; set; }
        public int SoLuong { get; set; }
        public string GhiChu { get; set; }
        public virtual ChuongTrinhGoiDichVu ChuongTrinhGoiDichVu { get; set; }
        public virtual QuaTang QuaTang { get; set; }
    }
}
