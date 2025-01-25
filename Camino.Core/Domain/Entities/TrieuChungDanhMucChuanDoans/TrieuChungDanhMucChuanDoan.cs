using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.ICDs;
using Camino.Core.Domain.Entities.TrieuChungs;

namespace Camino.Core.Domain.Entities.TrieuChungDanhMucChuanDoans
{
    public class TrieuChungDanhMucChuanDoan : BaseEntity
    {
        public long TrieuChungId { get; set; }
        public long DanhMucChuanDoanId { get; set; }
        public virtual TrieuChung TrieuChung { get; set; }
        public virtual DanhMucChuanDoan DanhMucChuanDoan { get; set; }
    }
}
