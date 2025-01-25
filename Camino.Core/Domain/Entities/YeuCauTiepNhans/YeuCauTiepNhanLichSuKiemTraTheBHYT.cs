using System;

namespace Camino.Core.Domain.Entities.YeuCauTiepNhans
{
    public class YeuCauTiepNhanLichSuKiemTraTheBHYT : BaseEntity
    {
        public string MaUserKiemTra { get; set; }
        public DateTime? ThoiGianKiemTra { get; set; }
        public string ThongBao { get; set; }
        public string MaLoi { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public virtual YeuCauTiepNhan YeuCauTiepNhan { get; set; }
    }
}