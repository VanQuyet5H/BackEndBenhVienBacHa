using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.DichVuGiuongBenhVien
{
    public class DichVuGiuongBenhVienTemplateVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string DichVu { get; set; }
        public string Ma { get; set; }
        public string TenKhoa { get; set; }
        public string MaTT37 { get; set; }
        public decimal? Gia { get; set; }
        public long? NhomGiaDichVuGiuongBenhVienId { get; set; }
        public bool CoGiaBaoHiem { get; set; }
        public Enums.EnumLoaiGiuong? LoaiGiuong { get; set; }
        public bool BaoPhong { get; set; }
        public int LoaiGiaCoHieuLuc { get; set; }
    }
}
