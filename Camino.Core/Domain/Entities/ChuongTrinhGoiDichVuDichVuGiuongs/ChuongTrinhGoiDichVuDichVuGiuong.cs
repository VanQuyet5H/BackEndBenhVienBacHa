using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVus;
using Camino.Core.Domain.Entities.DichVuGiuongBenhViens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.ChuongTrinhGoiDichVuDichVuGiuongs
{
    public class ChuongTrinhGoiDichVuDichVuGiuong : BaseEntity
    {
        public long ChuongTrinhGoiDichVuId { get; set; }
        public long DichVuGiuongBenhVienId { get; set; }
        public long NhomGiaDichVuGiuongBenhVienId { get; set; }
        public decimal DonGia { get; set; }
        public decimal DonGiaTruocChietKhau { get; set; }
        public decimal DonGiaSauChietKhau { get; set; }
        public int SoLan { get; set; }
        public virtual ChuongTrinhGoiDichVu ChuongTrinhGoiDichVu { get; set; }
        public virtual DichVuGiuongBenhVien DichVuGiuongBenhVien { get; set; }
        public virtual NhomGiaDichVuGiuongBenhVien NhomGiaDichVuGiuongBenhVien { get; set; }
    }
}
