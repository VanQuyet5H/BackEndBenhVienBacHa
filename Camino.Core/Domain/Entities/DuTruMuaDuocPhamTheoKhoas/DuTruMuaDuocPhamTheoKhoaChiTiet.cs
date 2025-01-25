using Camino.Core.Domain.Entities.DuTruMuaDuocPhamKhoDuocs;
using Camino.Core.Domain.Entities.DuTruMuaDuocPhams;
using Camino.Core.Domain.Entities.Thuocs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.DuTruMuaDuocPhamTheoKhoas
{
    public  class DuTruMuaDuocPhamTheoKhoaChiTiet : BaseEntity
    {
        public long DuTruMuaDuocPhamTheoKhoaId { get; set; }
        public long DuocPhamId { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public int SoLuongDuTru { get; set; }
        public int SoLuongDuKienSuDung { get; set; }
        public int SoLuongDuTruTruongKhoaDuyet { get; set; }
        public int? SoLuongDuTruKhoDuocDuyet { get; set; }
        public long? DuTruMuaDuocPhamKhoDuocChiTietId { get; set; }
        public virtual DuocPham DuocPham { get; set; }

        public virtual DuTruMuaDuocPhamTheoKhoa DuTruMuaDuocPhamTheoKhoa { get; set; }
        public virtual DuTruMuaDuocPhamKhoDuocChiTiet DuTruMuaDuocPhamKhoDuocChiTiet { get; set; }

        private ICollection<DuTruMuaDuocPhamChiTiet> _duTruMuaDuocPhamChiTiets;
        public virtual ICollection<DuTruMuaDuocPhamChiTiet> DuTruMuaDuocPhamChiTiets
        {
            get => _duTruMuaDuocPhamChiTiets ?? (_duTruMuaDuocPhamChiTiets = new List<DuTruMuaDuocPhamChiTiet>());
            protected set => _duTruMuaDuocPhamChiTiets = value;
        }
    }
}
