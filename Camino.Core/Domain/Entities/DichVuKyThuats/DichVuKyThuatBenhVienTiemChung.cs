using Camino.Core.Domain.Entities.DuocPhamBenhViens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.DichVuKyThuats
{
    public class DichVuKyThuatBenhVienTiemChung : BaseEntity
    {
        public long DichVuKyThuatBenhVienId { get; set; }
        public long DuocPhamBenhVienId { get; set; }

        public virtual DichVuKyThuatBenhVien DichVuKyThuatBenhVien { get; set; }
        public virtual DuocPhamBenhVien DuocPhamBenhVien { get; set; }
    }
}
