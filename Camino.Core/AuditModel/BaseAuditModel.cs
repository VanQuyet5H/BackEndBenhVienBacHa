using Camino.Core.Domain;

namespace Camino.Core.AuditModel
{
    public class BaseAuditModel
    {
        public long Id { get; set; }
        public Enums.EnumAudit Action { get; set; }
    }
}