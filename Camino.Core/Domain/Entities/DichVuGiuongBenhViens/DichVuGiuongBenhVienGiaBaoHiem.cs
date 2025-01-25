using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.DichVuGiuongBenhViens
{
   public class DichVuGiuongBenhVienGiaBaoHiem: BaseEntity
    {
        public long DichVuGiuongBenhVienId { get; set; }
        public decimal Gia { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public int TiLeBaoHiemThanhToan { get; set; }
        public virtual DichVuGiuongBenhVien DichVuGiuongBenhVien { get; set; }
    }
}
