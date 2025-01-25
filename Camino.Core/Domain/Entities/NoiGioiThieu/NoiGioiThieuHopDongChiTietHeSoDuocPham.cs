using Camino.Core.Domain.Entities.DuocPhamBenhViens;
using System;
using System.Collections.Generic;
using System.Text;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.Entities.NoiGioiThieu
{
    public class NoiGioiThieuHopDongChiTietHeSoDuocPham : BaseEntity
    {
        public long NoiGioiThieuHopDongId { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public LoaiGiaNoiGioiThieuHopDong LoaiGia { get; set; }
        public decimal HeSo { get; set; }

        public virtual NoiGioiThieuHopDong NoiGioiThieuHopDong { get; set; }
        public virtual DuocPhamBenhVien DuocPhamBenhVien { get; set; }
    }
}
