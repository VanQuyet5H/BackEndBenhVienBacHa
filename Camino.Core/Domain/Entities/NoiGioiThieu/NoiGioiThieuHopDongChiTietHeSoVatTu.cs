using Camino.Core.Domain.Entities.VatTuBenhViens;
using System;
using System.Collections.Generic;
using System.Text;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.Entities.NoiGioiThieu
{
    public class NoiGioiThieuHopDongChiTietHeSoVatTu : BaseEntity
    {
        public long NoiGioiThieuHopDongId { get; set; }
        public long VatTuBenhVienId { get; set; }
        public LoaiGiaNoiGioiThieuHopDong LoaiGia { get; set; }
        public decimal HeSo { get; set; }

        public virtual NoiGioiThieuHopDong NoiGioiThieuHopDong { get; set; }
        public virtual VatTuBenhVien VatTuBenhVien { get; set; }
    }
}
