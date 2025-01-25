using Camino.Core.Domain.Entities.DuTruMuaDuocPhamKhoDuocs;
using Camino.Core.Domain.Entities.DuTruMuaDuocPhamTheoKhoas;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.KyDuTruMuaDuocPhamVatTus;
using Camino.Core.Domain.Entities.NhanViens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.Entities.DuTruMuaDuocPhams
{
    public class DuTruMuaDuocPham : BaseEntity
    {
        public string SoPhieu { get; set; }
        public EnumNhomDuocPhamDuTru NhomDuocPhamDuTru { get; set; }
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
        public long? DuTruMuaDuocPhamTheoKhoaId { get; set; }
        public string LyDoTruongKhoaTuChoi { get; set; }
        public long? DuTruMuaDuocPhamKhoDuocId { get; set; }

        public virtual NhanVien NhanVienYeuCau { get; set; }
        public virtual NhanVien TruongKhoa { get; set; }
        public virtual Kho Kho { get; set; }
        public virtual KyDuTruMuaDuocPhamVatTu KyDuTruMuaDuocPhamVatTu { get; set; }
        public virtual DuTruMuaDuocPhamTheoKhoa DuTruMuaDuocPhamTheoKhoa { get; set; }
        public virtual DuTruMuaDuocPhamKhoDuoc DuTruMuaDuocPhamKhoDuoc { get; set; }

        private ICollection<DuTruMuaDuocPhamChiTiet> _duTruMuaDuocPhamChiTiets;
        public virtual ICollection<DuTruMuaDuocPhamChiTiet> DuTruMuaDuocPhamChiTiets
        {
            get => _duTruMuaDuocPhamChiTiets ?? (_duTruMuaDuocPhamChiTiets = new List<DuTruMuaDuocPhamChiTiet>());
            protected set => _duTruMuaDuocPhamChiTiets = value;
        }
    }
}
