using System;

namespace Camino.Core.Domain.Entities.CauHinhs
{
    public class CauHinhTheoThoiGianChiTiet : BaseEntity
    {
        public long CauHinhTheoThoiGianId { get; set; }

        public string Value { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public virtual CauHinhTheoThoiGian CauHinhTheoThoiGian { get; set; }
    }
}
