using System.Collections.Generic;
using Camino.Core.Domain.Entities.Audit;

namespace Camino.Core.AuditModel
{
    public class AuditResultModel
    {
        public AuditResultModel()
        {
            AuditEntry = new List<AuditEntry>();
            LstRoot = new List<Dictionary<dynamic, dynamic>>();
        }
        public List<AuditEntry> AuditEntry { get; set; }
        public List<Dictionary<dynamic, dynamic>> LstRoot { get; set; }
    }
}