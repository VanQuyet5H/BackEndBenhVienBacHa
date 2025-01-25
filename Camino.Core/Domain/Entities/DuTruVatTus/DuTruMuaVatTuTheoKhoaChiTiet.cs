using Camino.Core.Domain.Entities.VatTus;
using System.Collections.Generic;

namespace Camino.Core.Domain.Entities.DuTruVatTus
{
    public class DuTruMuaVatTuTheoKhoaChiTiet : BaseEntity
    {
        public long DuTruMuaVatTuTheoKhoaId { get; set; }
        public long VatTuId { get; set; }
        public bool LaVatTuBHYT { get; set; }
        public int SoLuongDuTru { get; set; }
        public int SoLuongDuKienSuDung { get; set; }
        public int SoLuongDuTruTruongKhoaDuyet { get; set; }
        public int? SoLuongDuTruKhoDuocDuyet { get; set; }
        public long? DuTruMuaVatTuKhoDuocChiTietId { get; set; }
        public virtual VatTu VatTu { get; set; }
        public virtual DuTruMuaVatTuTheoKhoa DuTruMuaVatTuTheoKhoa { get; set; }
        public virtual DuTruMuaVatTuKhoDuocChiTiet DuTruMuaVatTuKhoDuocChiTiet { get; set; }

        private ICollection<DuTruMuaVatTuChiTiet> _duTruMuaVatTuChiTiets;
        public virtual ICollection<DuTruMuaVatTuChiTiet> DuTruMuaVatTuChiTiets
        {
            get => _duTruMuaVatTuChiTiets ?? (_duTruMuaVatTuChiTiets = new List<DuTruMuaVatTuChiTiet>());
            protected set => _duTruMuaVatTuChiTiets = value;
        }
    }
}
