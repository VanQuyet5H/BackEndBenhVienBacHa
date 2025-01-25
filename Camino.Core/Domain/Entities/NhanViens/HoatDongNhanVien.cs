using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.Users;
using System;

namespace Camino.Core.Domain.Entities.NhanViens
{
    public class HoatDongNhanVien : BaseEntity
    {
        public long PhongBenhVienId { get; set; }
        public long NhanVienId { get; set; }
        public DateTime ThoiDiemBatDau { get; set; }

        public virtual NhanVien NhanVien { get; set; }
        public virtual PhongBenhVien PhongBenhVien { get; set; }
    }
}
