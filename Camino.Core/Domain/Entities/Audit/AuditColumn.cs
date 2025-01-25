using Camino.Core.Domain.Entities.Audit;

namespace Camino.Core.Domain.Entities.Audit
{
    public class AuditColumn : BaseEntity
    {
        public string ColumnName { get; set; }
        public long AuditTableId { get; set; }
        public virtual AuditTable AuditTable { get; set; }
    }
}