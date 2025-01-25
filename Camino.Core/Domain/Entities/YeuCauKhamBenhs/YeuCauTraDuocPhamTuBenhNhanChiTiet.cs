using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DuocPhamBenhViens;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.XuatKhos;

namespace Camino.Core.Domain.Entities.YeuCauKhamBenhs
{
    public class YeuCauTraDuocPhamTuBenhNhanChiTiet : BaseEntity
    {
        public long? YeuCauTraDuocPhamTuBenhNhanId { get; set; }
        public long YeuCauDuocPhamBenhVienId { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public long KhoTraId { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public bool TraVeTuTruc { get; set; }
        public double SoLuongTra { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public long NhanVienYeuCauId { get; set; }

        public virtual YeuCauTraDuocPhamTuBenhNhan YeuCauTraDuocPhamTuBenhNhan { get; set; }
        public virtual YeuCauDuocPhamBenhVien YeuCauDuocPhamBenhVien { get; set; }
        public virtual DuocPhamBenhVien DuocPhamBenhVien { get; set; }
        public virtual Kho KhoTra { get; set; }
        public virtual NhanVien NhanVienYeuCau { get; set; }

        private ICollection<XuatKhoDuocPhamChiTietViTri> _xuatKhoDuocPhamChiTietViTris;
        public virtual ICollection<XuatKhoDuocPhamChiTietViTri> XuatKhoDuocPhamChiTietViTris
        {
            get => _xuatKhoDuocPhamChiTietViTris ?? (_xuatKhoDuocPhamChiTietViTris = new List<XuatKhoDuocPhamChiTietViTri>());
            protected set => _xuatKhoDuocPhamChiTietViTris = value;
        }
    }
}
