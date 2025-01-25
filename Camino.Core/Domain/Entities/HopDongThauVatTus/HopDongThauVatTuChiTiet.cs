using Camino.Core.Domain.Entities.VatTus;

namespace Camino.Core.Domain.Entities.HopDongThauVatTus
{
    public class HopDongThauVatTuChiTiet : BaseEntity
    {
        public long HopDongThauVatTuId { get; set; }

        public long VatTuId { get; set; }

        public decimal Gia { get; set; }

        public double SoLuong { get; set; }

        public double SoLuongDaCap { get; set; }

        public virtual HopDongThauVatTu HopDongThauVatTu { get; set; }

        public virtual VatTu VatTu { get; set; }
    }
}
