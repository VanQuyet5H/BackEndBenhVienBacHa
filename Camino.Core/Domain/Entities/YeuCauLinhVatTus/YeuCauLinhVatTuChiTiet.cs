using Camino.Core.Domain.Entities.VatTuBenhViens;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;

namespace Camino.Core.Domain.Entities.YeuCauLinhVatTus
{
    public class YeuCauLinhVatTuChiTiet : BaseEntity
    {
        public long YeuCauLinhVatTuId { get; set; }
        public long VatTuBenhVienId { get; set; }
        public bool LaVatTuBHYT { get; set; }
        public double SoLuong { get; set; }
        public double? SoLuongCoTheXuat { get; set; }
        public long? YeuCauVatTuBenhVienId { get; set; }

        public double? SoLuongDaLinhBu { get; set; }
        public double? SoLuongCanBu { get; set; }

        public virtual YeuCauLinhVatTu YeuCauLinhVatTu { get; set; }
        public virtual VatTuBenhVien VatTuBenhVien { get; set; }
        public virtual YeuCauVatTuBenhVien YeuCauVatTuBenhVien { get; set; }

    }
}
