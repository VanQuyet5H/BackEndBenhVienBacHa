using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVus;
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.ChuongTrinhGoiDichVuDichVuKhamBenhs
{
    public class ChuongTrinhGoiDichVuDichVuKhamBenh : BaseEntity
    {
        public long ChuongTrinhGoiDichVuId { get; set; }
        public long DichVuKhamBenhBenhVienId { get; set; }
        public long NhomGiaDichVuKhamBenhBenhVienId { get; set; }
        public decimal DonGia { get; set; }
        public decimal DonGiaTruocChietKhau { get; set; }
        public decimal DonGiaSauChietKhau { get; set; }
        public int SoLan { get; set; }
        public virtual ChuongTrinhGoiDichVu ChuongTrinhGoiDichVu { get; set; }
        public virtual DichVuKhamBenhBenhVien DichVuKhamBenhBenhVien { get; set; }

        public virtual NhomGiaDichVuKhamBenhBenhVien NhomGiaDichVuKhamBenhBenhVien { get; set; }
    }
}
