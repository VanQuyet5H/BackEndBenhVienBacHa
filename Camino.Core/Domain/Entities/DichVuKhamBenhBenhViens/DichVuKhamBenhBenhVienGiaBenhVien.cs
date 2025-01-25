using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens
{
    public class DichVuKhamBenhBenhVienGiaBenhVien : BaseEntity
    {
        public long DichVuKhamBenhBenhVienId { get; set; }
        public long NhomGiaDichVuKhamBenhBenhVienId { get; set; }
        public decimal Gia { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public virtual NhomGiaDichVuKhamBenhBenhVien NhomGiaDichVuKhamBenhBenhVien { get; set; }
        public virtual DichVuKhamBenhBenhVien DichVuKhamBenhBenhVien { get; set; }
    }
}
