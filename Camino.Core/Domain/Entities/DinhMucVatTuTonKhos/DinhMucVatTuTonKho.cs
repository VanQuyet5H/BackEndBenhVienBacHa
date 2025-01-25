using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.VatTuBenhViens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.DinhMucVatTuTonKhos
{
    public class DinhMucVatTuTonKho : BaseEntity
    {
        public long VatTuBenhVienId { get; set; }
        public long KhoId { get; set; }
        public int? TonToiThieu { get; set; }
        public int? TonToiDa { get; set; }
        public int? SoNgayTruocKhiHetHan { get; set; }
        public string MoTa { get; set; }
        public virtual VatTuBenhVien VatTuBenhVien { get; set; }
        public virtual Kho Kho { get; set; }
    }
}
