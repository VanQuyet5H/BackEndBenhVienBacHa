
using System.Collections.Generic;
using Camino.Core.Domain.Entities.DonViTinhs;
using Camino.Core.Domain.Entities.DuTruVatTus;
using Camino.Core.Domain.Entities.HopDongThauVatTus;
using Camino.Core.Domain.Entities.NhomVatTus;
using Camino.Core.Domain.Entities.VatTuBenhViens;

namespace Camino.Core.Domain.Entities.VatTus
{
    public class VatTu : BaseEntity
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public long NhomVatTuId { get; set; }
        public string DonViTinh { get; set; }
        public int TyLeBaoHiemThanhToan { get; set; }
        public string QuyCach { get; set; }
        public string NhaSanXuat { get; set; }
        public string NuocSanXuat { get; set; }
        public string MoTa { get; set; }
        public bool? IsDisabled { get; set; }
        public int? HeSoDinhMucDonViTinh { get; set; }

        public virtual VatTuBenhVien VatTuBenhVien { get; set; }
        public virtual NhomVatTu NhomVatTu { get; set; }

        private ICollection<HopDongThauVatTuChiTiet> _hopDongThauVatTuChiTiets;
        public virtual ICollection<HopDongThauVatTuChiTiet> HopDongThauVatTuChiTiets
        {
            get => _hopDongThauVatTuChiTiets ?? (_hopDongThauVatTuChiTiets = new List<HopDongThauVatTuChiTiet>());
            protected set => _hopDongThauVatTuChiTiets = value;
        }


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

        private ICollection<DuTruMuaVatTuKhoDuocChiTiet> _duTruMuaVatTuKhoDuocChiTiets;
        public virtual ICollection<DuTruMuaVatTuKhoDuocChiTiet> DuTruMuaVatTuKhoDuocChiTiets
        {
            get => _duTruMuaVatTuKhoDuocChiTiets ?? (_duTruMuaVatTuKhoDuocChiTiets = new List<DuTruMuaVatTuKhoDuocChiTiet>());
            protected set => _duTruMuaVatTuKhoDuocChiTiets = value;
        }
    }
}
