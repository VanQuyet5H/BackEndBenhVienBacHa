using Camino.Core.Domain.Entities.VatTus;
using System.Collections.Generic;

namespace Camino.Core.Domain.Entities.DuTruVatTus
{
    public class DuTruMuaVatTuKhoDuocChiTiet : BaseEntity
    {       
        public long DuTruMuaVatTuKhoDuocId { get; set; }
        public long VatTuId { get; set; }
        public bool LaVatTuBHYT { get; set; }
        public int SoLuongDuTru { get; set; }
        public int SoLuongDuKienSuDung { get; set; }
        public int SoLuongDuTruTruongKhoaDuyet { get; set; }
        public int SoLuongDuTruKhoDuocDuyet { get; set; }
        public int? SoLuongDuTruGiamDocDuyet { get; set; }
        public virtual VatTu VatTu { get; set; }
        public virtual DuTruMuaVatTuKhoDuoc DuTruMuaVatTuKhoDuoc { get; set; }

        private ICollection<DuTruMuaVatTuChiTiet> _duTruMuaVatTuChiTiets;
        public virtual ICollection<DuTruMuaVatTuChiTiet> DuTruMuaVatTuChiTiets
        {
            get => _duTruMuaVatTuChiTiets ?? (_duTruMuaVatTuChiTiets = new List<DuTruMuaVatTuChiTiet>());
            protected set => _duTruMuaVatTuChiTiets = value;
        }

        private ICollection<DuTruMuaVatTuTheoKhoaChiTiet> _duTruMuaVatTuTheoKhoaChiTiets;
        public virtual ICollection<DuTruMuaVatTuTheoKhoaChiTiet> DuTruMuaVatTuTheoKhoaChiTiets
        {
            get => _duTruMuaVatTuTheoKhoaChiTiets ?? (_duTruMuaVatTuTheoKhoaChiTiets = new List<DuTruMuaVatTuTheoKhoaChiTiet>());
            protected set => _duTruMuaVatTuTheoKhoaChiTiets = value;
        }
    }
}
