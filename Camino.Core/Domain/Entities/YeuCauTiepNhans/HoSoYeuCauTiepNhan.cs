namespace Camino.Core.Domain.Entities.YeuCauTiepNhans
{
    public class HoSoYeuCauTiepNhan : TaiLieuDinhKemEntity
    {
        public long LoaiHoSoYeuCauTiepNhanId { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public virtual LoaiHoSoYeuCauTiepNhan LoaiHoSoYeuCauTiepNhan { get; set; }
        public virtual YeuCauTiepNhan YeuCauTiepNhan { get; set; }
    }
}