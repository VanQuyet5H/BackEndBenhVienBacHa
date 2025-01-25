using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens
{
    public class DichVuKhamBenhBenhVienGiaBaoHiem : BaseEntity
    {
        public long DichVuKhamBenhBenhVienId { get; set; }
        public decimal Gia { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public int TiLeBaoHiemThanhToan { get; set; }
        public virtual DichVuKhamBenhBenhVien DichVuKhamBenhBenhVien { get; set; }
    }
}
