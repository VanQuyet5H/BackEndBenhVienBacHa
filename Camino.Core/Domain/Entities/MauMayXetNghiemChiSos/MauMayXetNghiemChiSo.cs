using Camino.Core.Domain.Entities.MauMayXetNghiems;

namespace Camino.Core.Domain.Entities.MauMayXetNghiemChiSos
{
    public class MauMayXetNghiemChiSo : BaseEntity
    {
        public long MauMayXetNghiemId { get; set; }

        public string MaChiSo { get; set; }
        
        public string TenChiSo { get; set; }
        
        public string DonVi { get; set; }

        public virtual MauMayXetNghiem MauMayXetNghiem { get; set; }
    }
}
