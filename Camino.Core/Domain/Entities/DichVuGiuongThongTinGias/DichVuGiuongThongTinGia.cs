using Camino.Core.Domain.Entities.DichVuGiuongs;
using System;

namespace Camino.Core.Domain.Entities.DichVuGiuongThongTinGias
{
    public class DichVuGiuongThongTinGia : BaseEntity
    {
        public long DichVuGiuongId { get; set; }
        public decimal Gia { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string MoTa { get; set; }
        public bool HieuLuc { get; set; }
        public virtual DichVuGiuong DichVuGiuong { get; set; }

    }
}
