using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Internal;

namespace Camino.Core.Domain.Entities.Audit
{
    public class AuditEntry
    {
        public AuditEntry(EntityEntry entry)
        {
            Entry = entry;
        }

        public EntityEntry Entry { get; }
        public string TableName { get; set; }
        public Enums.EnumAudit Action { get; set; }
        public long? ReferenceKey { get; set; }
        public Dictionary<dynamic, object> KeyValues { get; } = new Dictionary<dynamic, object>();
        public Dictionary<dynamic, object> OldValues { get; } = new Dictionary<dynamic, object>();
        public Dictionary<dynamic, object> NewValues { get; } = new Dictionary<dynamic, object>();
        public List<PropertyEntry> TemporaryProperties { get; } = new List<PropertyEntry>();

        public bool HasTemporaryProperties => TemporaryProperties.Any();
    }
}