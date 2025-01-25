using Camino.Core.Domain.Entities.ICDs;

namespace Camino.Core.Domain.Entities.YeuCauKhamBenhs
{
    public class YeuCauKhamBenhChuanDoan: BaseEntity
    {
        public long YeuCauKhamBenhId { get; set; }

        public long ChuanDoanId { get; set; }

        public virtual ChuanDoan ChuanDoan { get; set; }

        public virtual YeuCauKhamBenh YeuCauKhamBenh { get; set; }
    }
}
