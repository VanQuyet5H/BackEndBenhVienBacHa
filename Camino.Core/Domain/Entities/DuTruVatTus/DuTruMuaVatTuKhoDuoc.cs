using Camino.Core.Domain.Entities.KyDuTruMuaDuocPhamVatTus;
using Camino.Core.Domain.Entities.NhanViens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Camino.Core.Domain.Entities.DuTruVatTus
{
    public class DuTruMuaVatTuKhoDuoc : BaseEntity
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

        public virtual  NhanVien NhanVienYeuCau { get; set; }
        public virtual NhanVien GiamDoc { get; set; }
        public virtual KyDuTruMuaDuocPhamVatTu KyDuTruMuaDuocPhamVatTu { get; set; }

        private ICollection<DuTruMuaVatTu> _duTruMuaVatTus;
        public virtual ICollection<DuTruMuaVatTu> DuTruMuaVatTus
        {
            get => _duTruMuaVatTus ?? (_duTruMuaVatTus = new List<DuTruMuaVatTu>());
            protected set => _duTruMuaVatTus = value;
        }

        private ICollection<DuTruMuaVatTuTheoKhoa> _duTruMuaVatTuTheoKhoas;
        public virtual ICollection<DuTruMuaVatTuTheoKhoa> DuTruMuaVatTuTheoKhoas
        {
            get => _duTruMuaVatTuTheoKhoas ?? (_duTruMuaVatTuTheoKhoas = new List<DuTruMuaVatTuTheoKhoa>());
            protected set => _duTruMuaVatTuTheoKhoas = value;
        }

        private ICollection<DuTruMuaVatTuKhoDuocChiTiet> _duTruMuaVatTuKhoDuocChiTiets;
        public virtual ICollection<DuTruMuaVatTuKhoDuocChiTiet> DuTruMuaVatTuKhoDuocChiTiets
        {
            get => _duTruMuaVatTuKhoDuocChiTiets ?? (_duTruMuaVatTuKhoDuocChiTiets = new List<DuTruMuaVatTuKhoDuocChiTiet>());
            protected set => _duTruMuaVatTuKhoDuocChiTiets = value;
        }
    }
}

