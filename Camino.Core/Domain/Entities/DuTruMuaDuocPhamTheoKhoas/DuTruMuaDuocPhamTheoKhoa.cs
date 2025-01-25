using Camino.Core.Domain.Entities.DuTruMuaDuocPhamKhoDuocs;
using Camino.Core.Domain.Entities.DuTruMuaDuocPhams;
using Camino.Core.Domain.Entities.KhoaPhongs;
using Camino.Core.Domain.Entities.KyDuTruMuaDuocPhamVatTus;
using Camino.Core.Domain.Entities.NhanViens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Camino.Core.Domain.Entities.DuTruMuaDuocPhamTheoKhoas
{
    public class DuTruMuaDuocPhamTheoKhoa : BaseEntity
    {
        public string SoPhieu { get; set; }
        public long KhoaPhongId { get; set; }
        public long NhanVienYeuCauId { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public long KyDuTruMuaDuocPhamVatTuId { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public string GhiChu { get; set; }
        public bool? KhoDuocDuyet { get; set; }
        public long? NhanVienKhoDuocId { get; set; }
        public DateTime? NgayKhoDuocDuyet { get; set; }
        public long? DuTruMuaDuocPhamKhoDuocId { get; set; }
        public string LyDoKhoDuocTuChoi { get; set; }

        public virtual KhoaPhong KhoaPhong { get; set; }
        public virtual NhanVien NhanVienYeuCau { get; set; }
        public virtual NhanVien NhanVienKhoDuoc { get; set; }

        public virtual KyDuTruMuaDuocPhamVatTu KyDuTruMuaDuocPhamVatTu { get; set; }
        public virtual DuTruMuaDuocPhamKhoDuoc DuTruMuaDuocPhamKhoDuoc { get; set; }

        private ICollection<DuTruMuaDuocPham> _duTruMuaDuocPhams;
        public virtual ICollection<DuTruMuaDuocPham> DuTruMuaDuocPhams
        {
            get => _duTruMuaDuocPhams ?? (_duTruMuaDuocPhams = new List<DuTruMuaDuocPham>());
            protected set => _duTruMuaDuocPhams = value;
        }

        private ICollection<DuTruMuaDuocPhamTheoKhoaChiTiet> _duTruMuaDuocPhamTheoKhoaChiTiets;
        public virtual ICollection<DuTruMuaDuocPhamTheoKhoaChiTiet> DuTruMuaDuocPhamTheoKhoaChiTiets
        {
            get => _duTruMuaDuocPhamTheoKhoaChiTiets ?? (_duTruMuaDuocPhamTheoKhoaChiTiets = new List<DuTruMuaDuocPhamTheoKhoaChiTiet>());
            protected set => _duTruMuaDuocPhamTheoKhoaChiTiets = value;
        }
    }
}
