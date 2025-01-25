using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using System;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.Entities.BHYT
{
    public class YeuCauTiepNhanDuLieuGuiCongBHYT : BaseEntity
    {
        public long YeuCauTiepNhanId { get; set; }
        public long? DuLieuGuiCongBHYTId { get; set; }
        public string DuLieu { get; set; }
        public int Version { get; set; }
        public bool? CoGuiCong { get; set; }
        public EnumMaHoaHinhThucKCB? HinhThucKCB { get; set; }
        public DateTime? NgayVao { get; set; }
        public DateTime? NgayRa { get; set; }

        public virtual DuLieuGuiCongBHYT DuLieuGuiCongBHYT { get; set; }
        public virtual YeuCauTiepNhan YeuCauTiepNhan { get; set; }
    }
}