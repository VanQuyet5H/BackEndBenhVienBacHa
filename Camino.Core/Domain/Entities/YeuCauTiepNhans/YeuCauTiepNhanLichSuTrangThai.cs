
namespace Camino.Core.Domain.Entities.YeuCauTiepNhans
{
    public class YeuCauTiepNhanLichSuTrangThai: BaseEntity
    {
        public long YeuCauTiepNhanId { get; set; }
        public int TrangThaiYeuCauTiepNhan { get; set; }
        public string MoTa { get; set; }
        public virtual YeuCauTiepNhan YeuCauTiepNhan { get; set; }
    }
}
