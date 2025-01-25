using Camino.Core.Domain.Entities.DuTruMuaDuocPhams;
using Camino.Core.Domain.Entities.DuTruMuaDuocPhamTheoKhoas;
using Camino.Core.Domain.Entities.KyDuTruMuaDuocPhamVatTus;
using Camino.Core.Domain.Entities.NhanViens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Camino.Core.Domain.Entities.DuTruMuaDuocPhamKhoDuocs
{
    public class DuTruMuaDuocPhamKhoDuoc : BaseEntity
    {
        public string SoPhieu { get; set; }
        public long NhanVienYeuCauId { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public long KyDuTruMuaDuocPhamVatTuId { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public string GhiChu { get; set; }
        public bool? GiamDocDuyet { get; set; }
        public long? GiamDocId { get; set; }
        public DateTime? NgayGiamDocDuyet { get; set; }
        public string LyDoGiamDocTuChoi { get; set; }

        public virtual NhanVien NhanVienYeuCau { get; set; }
        public virtual NhanVien GiamDoc { get; set; }
        public virtual KyDuTruMuaDuocPhamVatTu KyDuTruMuaDuocPhamVatTu { get; set; }

        private ICollection<DuTruMuaDuocPham> _duTruMuaDuocPhams;
        public virtual ICollection<DuTruMuaDuocPham> DuTruMuaDuocPhams
        {
            get => _duTruMuaDuocPhams ?? (_duTruMuaDuocPhams = new List<DuTruMuaDuocPham>());
            protected set => _duTruMuaDuocPhams = value;
        }

        private ICollection<DuTruMuaDuocPhamTheoKhoa> _duTruMuaDuocPhamTheoKhoas;
        public virtual ICollection<DuTruMuaDuocPhamTheoKhoa> DuTruMuaDuocPhamTheoKhoas
        {
            get => _duTruMuaDuocPhamTheoKhoas ?? (_duTruMuaDuocPhamTheoKhoas = new List<DuTruMuaDuocPhamTheoKhoa>());
            protected set => _duTruMuaDuocPhamTheoKhoas = value;
        }

        private ICollection<DuTruMuaDuocPhamKhoDuocChiTiet> _duTruMuaDuocPhamKhoDuocChiTiets;
        public virtual ICollection<DuTruMuaDuocPhamKhoDuocChiTiet> DuTruMuaDuocPhamKhoDuocChiTiets
        {
            get => _duTruMuaDuocPhamKhoDuocChiTiets ?? (_duTruMuaDuocPhamKhoDuocChiTiets = new List<DuTruMuaDuocPhamKhoDuocChiTiet>());
            protected set => _duTruMuaDuocPhamKhoDuocChiTiets = value;
        }
    }
}
