using Camino.Core.Domain.Entities.KhoaPhongs;
using Camino.Core.Domain.Entities.KyDuTruMuaDuocPhamVatTus;
using Camino.Core.Domain.Entities.NhanViens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Camino.Core.Domain.Entities.DuTruVatTus
{
    public class DuTruMuaVatTuTheoKhoa : BaseEntity
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
        public long? DuTruMuaVatTuKhoDuocId { get; set; }
        public string LyDoKhoDuocTuChoi { get; set; }

        public virtual NhanVien NhanVienYeuCau { get; set; }
        public virtual NhanVien NhanVienKhoDuoc { get; set; }
        public virtual KyDuTruMuaDuocPhamVatTu KyDuTruMuaDuocPhamVatTu { get; set; }

        public virtual KhoaPhong KhoaPhong { get; set; }
        public virtual DuTruMuaVatTuKhoDuoc DuTruMuaVatTuKhoDuoc { get; set; }

        private ICollection<DuTruMuaVatTu> _duTruMuaVatTus;
        public virtual ICollection<DuTruMuaVatTu> DuTruMuaVatTus
        {
            get => _duTruMuaVatTus ?? (_duTruMuaVatTus = new List<DuTruMuaVatTu>());
            protected set => _duTruMuaVatTus = value;
        }

        private ICollection<DuTruMuaVatTuTheoKhoaChiTiet> _duTruMuaVatTuTheoKhoaChiTiets;
        public virtual ICollection<DuTruMuaVatTuTheoKhoaChiTiet> DuTruMuaVatTuTheoKhoaChiTiets
        {
            get => _duTruMuaVatTuTheoKhoaChiTiets ?? (_duTruMuaVatTuTheoKhoaChiTiets = new List<DuTruMuaVatTuTheoKhoaChiTiet>());
            protected set => _duTruMuaVatTuTheoKhoaChiTiets = value;
        }
    }
}
