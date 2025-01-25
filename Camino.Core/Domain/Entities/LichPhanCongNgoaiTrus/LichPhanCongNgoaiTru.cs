using System;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.PhongBenhViens;

namespace Camino.Core.Domain.Entities.LichPhanCongNgoaiTrus
{
    public class LichPhanCongNgoaiTru : BaseEntity
    {
        public long PhongNgoaiTruId { get; set; }

        public long NhanVienId { get; set; }

        public DateTime NgayPhanCong { get; set; }

        public Enums.EnumLoaiThoiGianPhanCong LoaiThoiGianPhanCong { get; set; }

        public virtual PhongBenhVien PhongBenhVien { get; set; }

        public virtual NhanVien NhanVien { get; set; }

    }
}
