using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.NhanViens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.KhoNhanVienQuanLys
{
    public class KhoNhanVienQuanLy : BaseEntity
    {
        public long KhoId { get; set; }
        public long NhanVienId { get; set; }
        public virtual Kho Kho { get; set; }
        public virtual NhanVien NhanVien { get; set; }
    }
}
