using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DichVuGiuongBenhViens;

namespace Camino.Core.Domain.Entities.ChuongTrinhGoiDichVus
{
    public class ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuong : BaseEntity
    {
        public long ChuongTrinhGoiDichVuId { get; set; }
        public long DichVuGiuongBenhVienId { get; set; }
        public long NhomGiaDichVuGiuongBenhVienId { get; set; }
        public decimal DonGia { get; set; }
        public decimal DonGiaKhuyenMai { get; set; }
        public int SoLan { get; set; }
        public int SoNgaySuDung { get; set; }
        public string GhiChu { get; set; }
        public virtual ChuongTrinhGoiDichVu ChuongTrinhGoiDichVu { get; set; }
        public virtual DichVuGiuongBenhVien DichVuGiuongBenhVien { get; set; }
        public virtual NhomGiaDichVuGiuongBenhVien NhomGiaDichVuGiuongBenhVien { get; set; }
    }
}
