using Camino.Core.Domain.Entities.VatTuBenhViens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.NoiGioiThieu
{
    public class NoiGioiThieuHopDongChiTietHoaHongVatTu : BaseEntity
    {
        public long NoiGioiThieuHopDongId { get; set; }
        public long VatTuBenhVienId { get; set; }
        public decimal TiLeHoaHong { get; set; }

        public virtual NoiGioiThieuHopDong NoiGioiThieuHopDong { get; set; }
        public virtual VatTuBenhVien VatTuBenhVien { get; set; }
    }
}
