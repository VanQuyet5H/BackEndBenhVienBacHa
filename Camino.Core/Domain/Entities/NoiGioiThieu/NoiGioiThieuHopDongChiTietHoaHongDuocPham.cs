using Camino.Core.Domain.Entities.DuocPhamBenhViens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.NoiGioiThieu
{
    public class NoiGioiThieuHopDongChiTietHoaHongDuocPham : BaseEntity
    {
        public long NoiGioiThieuHopDongId { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public decimal TiLeHoaHong { get; set; }

        public virtual NoiGioiThieuHopDong NoiGioiThieuHopDong { get; set; }
        public virtual DuocPhamBenhVien DuocPhamBenhVien { get; set; }
    }
}
