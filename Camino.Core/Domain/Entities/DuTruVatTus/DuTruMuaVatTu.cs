using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.KyDuTruMuaDuocPhamVatTus;
using Camino.Core.Domain.Entities.NhanViens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Camino.Core.Domain.Entities.DuTruVatTus
{
    public class DuTruMuaVatTu : BaseEntity
    {
        public string SoPhieu { get; set; }
        public long KhoId { get; set; }
        public long NhanVienYeuCauId { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public long KyDuTruMuaDuocPhamVatTuId { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public string GhiChu { get; set; }
        public bool? TruongKhoaDuyet { get; set; }
        public long? TruongKhoaId { get; set; }

        public DateTime? NgayTruongKhoaDuyet { get; set; }
        public long? DuTruMuaVatTuTheoKhoaId { get; set; }
        public string LyDoTruongKhoaTuChoi { get; set; }
        public long? DuTruMuaVatTuKhoDuocId { get; set; }

        public virtual NhanVien NhanVienYeuCau { get; set; }
        public virtual NhanVien TruongKhoa { get; set; }

        public virtual Kho Kho { get; set; }

        public virtual KyDuTruMuaDuocPhamVatTu KyDuTruMuaDuocPhamVatTu { get; set; }
        public virtual DuTruMuaVatTuTheoKhoa DuTruMuaVatTuTheoKhoa { get; set; }
        public virtual DuTruMuaVatTuKhoDuoc DuTruMuaVatTuKhoDuoc { get; set; }

        private ICollection<DuTruMuaVatTuChiTiet> _duTruMuaVatTuChiTiets;
        public virtual ICollection<DuTruMuaVatTuChiTiet> DuTruMuaVatTuChiTiets
        {
            get => _duTruMuaVatTuChiTiets ?? (_duTruMuaVatTuChiTiets = new List<DuTruMuaVatTuChiTiet>());
            protected set => _duTruMuaVatTuChiTiets = value;
        }
    }
}
