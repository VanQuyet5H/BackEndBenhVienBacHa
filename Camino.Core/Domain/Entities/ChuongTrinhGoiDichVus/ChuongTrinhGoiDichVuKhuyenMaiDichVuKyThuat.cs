using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DichVuKyThuats;

namespace Camino.Core.Domain.Entities.ChuongTrinhGoiDichVus
{
    public class ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuat : BaseEntity
    {
        public long ChuongTrinhGoiDichVuId { get; set; }
        public long DichVuKyThuatBenhVienId { get; set; }
        public long NhomGiaDichVuKyThuatBenhVienId { get; set; }
        public decimal DonGia { get; set; }
        public decimal DonGiaKhuyenMai { get; set; }
        public int SoLan { get; set; }
        public int SoNgaySuDung { get; set; }
        public string GhiChu { get; set; }
        public virtual ChuongTrinhGoiDichVu ChuongTrinhGoiDichVu { get; set; }
        public virtual DichVuKyThuatBenhVien DichVuKyThuatBenhVien { get; set; }
        public virtual NhomGiaDichVuKyThuatBenhVien NhomGiaDichVuKyThuatBenhVien { get; set; }
    }
}
