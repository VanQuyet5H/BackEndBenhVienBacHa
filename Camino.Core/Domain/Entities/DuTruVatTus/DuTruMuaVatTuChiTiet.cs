using Camino.Core.Domain.Entities.VatTus;

namespace Camino.Core.Domain.Entities.DuTruVatTus
{
    public class DuTruMuaVatTuChiTiet : BaseEntity
    {
        public long DuTruMuaVatTuId { get; set; }
        public long VatTuId { get; set; }
        public bool LaVatTuBHYT { get; set; }
        public int SoLuongDuTru { get; set; }
        public int SoLuongDuKienSuDung { get; set; }
        public int? SoLuongDuTruTruongKhoaDuyet { get; set; }
        public long? DuTruMuaVatTuTheoKhoaChiTietId { get; set; }
        public long? DuTruMuaVatTuKhoDuocChiTietId { get; set; }
        public string GhiChu { get; set; }

        public virtual VatTu VatTu { get; set; }
        public virtual DuTruMuaVatTu DuTruMuaVatTu { get; set; }
        public virtual DuTruMuaVatTuTheoKhoaChiTiet DuTruMuaVatTuTheoKhoaChiTiet { get; set; }
        public virtual DuTruMuaVatTuKhoDuocChiTiet DuTruMuaVatTuKhoDuocChiTiet { get; set; }
    }
}
