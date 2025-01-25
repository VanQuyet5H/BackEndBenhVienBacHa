using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.BenhAnDienTus
{
    public class GayBenhAnPhieuHoSo : BaseEntity
    {
        public long GayBenhAnId { get; set; }
        public Enums.LoaiPhieuHoSoBenhAnDienTu LoaiPhieuHoSoBenhAnDienTu { get; set; }
        public long? Value { get; set; }

        public virtual GayBenhAn GayBenhAn { get; set; }
    }
}
