using System.Collections.Generic;

namespace Camino.Core.Domain.Entities.Audit
{
    public class AuditTable : BaseEntity
    {
        public string TableName { get; set; }
        public string ParentName { get; set; }
        public bool? IsReference1n { get; set; }
        public bool IsRoot { get; set; }
        public string ReferenceKey { get; set; }
        public string ChildReferenceKey { get; set; }
        public string ChildReferenceName { get; set; }
        private ICollection<Audit.AuditColumn> _auditColumns;
        public virtual ICollection<Audit.AuditColumn> AuditColumns
        {
            get => _auditColumns ?? (_auditColumns = new List<Audit.AuditColumn>());
            protected set => _auditColumns = value;
        }
    }
}