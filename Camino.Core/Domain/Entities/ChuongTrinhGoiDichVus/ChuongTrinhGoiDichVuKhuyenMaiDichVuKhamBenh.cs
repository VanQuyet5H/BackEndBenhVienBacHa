using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Camino.Core.Domain.Entities.DichVuKyThuats;

namespace Camino.Core.Domain.Entities.ChuongTrinhGoiDichVus
{
    public class ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenh : BaseEntity
    {
        public long ChuongTrinhGoiDichVuId { get; set; }
        public long DichVuKhamBenhBenhVienId { get; set; }
        public long NhomGiaDichVuKhamBenhBenhVienId { get; set; }
        public decimal DonGia { get; set; }
        public decimal DonGiaKhuyenMai { get; set; }
        public int SoLan { get; set; }
        public int SoNgaySuDung { get; set; }
        public string GhiChu { get; set; }
        public virtual ChuongTrinhGoiDichVu ChuongTrinhGoiDichVu { get; set; }
        public virtual DichVuKhamBenhBenhVien DichVuKhamBenhBenhVien { get; set; }
        public virtual NhomGiaDichVuKhamBenhBenhVien NhomGiaDichVuKhamBenhBenhVien { get; set; }
    }
}
