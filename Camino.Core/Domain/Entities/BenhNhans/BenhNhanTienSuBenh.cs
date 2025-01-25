using Camino.Core.Domain.Entities.ICDs;
using System;

namespace Camino.Core.Domain.Entities.BenhNhans
{
    public class BenhNhanTienSuBenh : BaseEntity
    {
        public long BenhNhanId { get; set; }
        public string TenBenh { get; set; }
        public Enums.EnumLoaiTienSuBenh LoaiTienSuBenh { get; set; }
        public virtual BenhNhan BenhNhan { get; set; }
    }
}
