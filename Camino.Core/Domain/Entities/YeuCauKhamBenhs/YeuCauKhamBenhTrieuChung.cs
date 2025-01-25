using Camino.Core.Domain.Entities.TrieuChungs;

namespace Camino.Core.Domain.Entities.YeuCauKhamBenhs
{
    public class YeuCauKhamBenhTrieuChung : BaseEntity
    {
        public long YeuCauKhamBenhId { get; set; }

        public long TrieuChungId { get; set; }

        public virtual TrieuChung TrieuChung { get; set; }

        public virtual YeuCauKhamBenh YeuCauKhamBenh { get; set; }
    }
}
