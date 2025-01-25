using Camino.Core.Domain.ValueObject.Grid;
using System;

namespace Camino.Core.Domain.ValueObject.DichVuKhamBenh
{
    public class DichVuKhamBenhGridVo : GridItem
    {
        public string Ma { get; set; }
        public string MaTT37 { get; set; }
        public string Ten { get; set; }
        public long KhoaId { get; set; }
        public string TenKhoa { get; set; }
        public Enums.HangBenhVien HangBenhVien { get; set; }
        public string TenHangBenhVien { get; set; }
        public string MoTa { get; set; }
        public string Khoa { get; set; }
    }
    public class DichVuKhamBenhThongTinGiaGridVo : GridItem
    {
        public decimal Gia { get; set; }
        public string GiaFormat { get; set; }
        public DateTime? TuNgay { get; set; }
        public string TuNgayFormat { get; set; }
        public DateTime? DenNgay { get; set; }
        public string DenNgayFormat { get; set; }
        public string MoTa { get; set; }
        public bool IsActive { get; set; }
        public string ActiveName { get; set; }
    }
}
