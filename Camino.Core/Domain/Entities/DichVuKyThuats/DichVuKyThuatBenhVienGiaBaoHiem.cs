using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.DichVuKyThuats
{
    public class DichVuKyThuatBenhVienGiaBaoHiem : BaseEntity
    {
        public long DichVuKyThuatBenhVienId { get; set; }
        public decimal Gia { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public int TiLeBaoHiemThanhToan { get; set; }
        public virtual DichVuKyThuatBenhVien DichVuKyThuatBenhVien { get; set; }

    }
}
