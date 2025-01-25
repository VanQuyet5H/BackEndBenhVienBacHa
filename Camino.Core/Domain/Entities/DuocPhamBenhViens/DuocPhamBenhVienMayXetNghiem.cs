using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.MayXetNghiems;

namespace Camino.Core.Domain.Entities.DuocPhamBenhViens
{
    public class DuocPhamBenhVienMayXetNghiem : BaseEntity
    {
        public long DuocPhamBenhVienId { get; set; }
        public long MayXetNghiemId { get; set; }

        public virtual DuocPhamBenhVien DuocPhamBenhVien { get; set; }
        public virtual MayXetNghiem MayXetNghiem { get; set; }
    }
}
