using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.NhanViens;

namespace Camino.Core.Domain.Entities.GachNos
{
    public class AuditGachNo : BaseEntity
    {
        public string Username { get; set; }
        public string TableName { get; set; }
        public Enums.EnumAudit Action { get; set; }
        public long KeyValues { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
        public virtual NhanVien NhanVienThucHien { get; set; }
    }
}
