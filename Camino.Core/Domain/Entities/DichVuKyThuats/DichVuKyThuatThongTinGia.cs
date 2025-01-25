using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.DichVuKyThuats
{
    public class DichVuKyThuatThongTinGia : BaseEntity
    {
        public long DichVuKyThuatId { get; set; }
        public Enums.HangBenhVien? HangBenhVien { get; set; }
        public decimal Gia { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string ThongTu { get; set; }
        public string QuyetDinh { get; set; }
        public string MoTa { get; set; }
        public bool HieuLuc { get; set; }

        public virtual DichVuKyThuat DichVuKyThuat { get; set; }
    }
}
