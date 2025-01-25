using Camino.Core.Domain.Entities.DuTruMuaDuocPhams;
using Camino.Core.Domain.Entities.DuTruMuaDuocPhamTheoKhoas;
using Camino.Core.Domain.Entities.Thuocs;
using System.Collections.Generic;

namespace Camino.Core.Domain.Entities.DuTruMuaDuocPhamKhoDuocs
{
    public class DuTruMuaDuocPhamKhoDuocChiTiet : BaseEntity
    {
        public long DuTruMuaDuocPhamKhoDuocId { get; set; }
        public long DuocPhamId { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public int SoLuongDuTru { get; set; }
        public int SoLuongDuKienSuDung { get; set; }
        public int SoLuongDuTruTruongKhoaDuyet { get; set; }
        public int SoLuongDuTruKhoDuocDuyet { get; set; }
        public int? SoLuongDuTruGiamDocDuyet { get; set; }

        public virtual DuocPham DuocPham { get; set; }
        public virtual DuTruMuaDuocPhamKhoDuoc DuTruMuaDuocPhamKhoDuoc { get; set; }

        private ICollection<DuTruMuaDuocPhamChiTiet> _duTruMuaDuocPhamChiTiets;
        public virtual ICollection<DuTruMuaDuocPhamChiTiet> DuTruMuaDuocPhamChiTiets
        {
            get => _duTruMuaDuocPhamChiTiets ?? (_duTruMuaDuocPhamChiTiets = new List<DuTruMuaDuocPhamChiTiet>());
            protected set => _duTruMuaDuocPhamChiTiets = value;
        }

        private ICollection<DuTruMuaDuocPhamTheoKhoaChiTiet> _duTruMuaDuocPhamTheoKhoaChiTiets;
        public virtual ICollection<DuTruMuaDuocPhamTheoKhoaChiTiet> DuTruMuaDuocPhamTheoKhoaChiTiets
        {
            get => _duTruMuaDuocPhamTheoKhoaChiTiets ?? (_duTruMuaDuocPhamTheoKhoaChiTiets = new List<DuTruMuaDuocPhamTheoKhoaChiTiet>());
            protected set => _duTruMuaDuocPhamTheoKhoaChiTiets = value;
        }
    }
}
