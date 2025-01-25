using System;

namespace Camino.Core.Domain.Entities.DichVuKhamBenhs
{
    public class DichVuKhamBenhThongTinGia : BaseEntity
    {
        public long DichVuKhamBenhId { get; set; }
        public decimal Gia { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string MoTa { get; set; }
        public bool HieuLuc { get; set; }

        public virtual DichVuKhamBenh DichVuKhamBenh { get; set; }
    }
}
