using System;

namespace Camino.Core.Domain.Entities.YeuCauTiepNhans
{
    public class YeuCauTiepNhanLichSuKhamBHYT : BaseEntity
    {
        public string MaTheBHYT { get; set; }
        public string TenBenh { get; set; }
        public DateTime? NgayVao { get; set; }
        public DateTime? NgayRa { get; set; }
        public string MaCSKCB { get; set; }
        public Enums.EnumTinhTrangRaVien? TinhTrangRaVien { get; set; }
        public Enums.EnumLyDoVaoVien? LyDoVaoVien { get; set; }
        public Enums.EnumKetQuaDieuTri? KetQuaDieuTri { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public virtual YeuCauTiepNhan YeuCauTiepNhan { get; set; }
    }
}