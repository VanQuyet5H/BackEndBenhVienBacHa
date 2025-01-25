using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.DichVuGiuongBenhViens
{
    public class DichVuGiuongBenhVienGiaBenhVien:BaseEntity
    {
        public long DichVuGiuongBenhVienId { get; set; }
        public long NhomGiaDichVuGiuongBenhVienId { get; set; }
        public decimal Gia { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        //public bool? BaoPhong { get; set; }

        public virtual DichVuGiuongBenhVien DichVuGiuongBenhVien { get; set; }
        public virtual NhomGiaDichVuGiuongBenhVien NhomGiaDichVuGiuongBenhVien { get; set; }
    }
}
