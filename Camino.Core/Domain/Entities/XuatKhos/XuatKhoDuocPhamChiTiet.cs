using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using System;
using System.Collections.Generic;

namespace Camino.Core.Domain.Entities.XuatKhos
{
    public class XuatKhoDuocPhamChiTiet : BaseEntity
    {
        public long? XuatKhoDuocPhamId { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        //public bool DatChatLuong { get; set; }
        public DateTime? NgayXuat { get; set; }
        public virtual XuatKhoDuocPham XuatKhoDuocPham { get; set; }
        public virtual DuocPhamBenhViens.DuocPhamBenhVien DuocPhamBenhVien { get; set; }

        private ICollection<XuatKhoDuocPhamChiTietViTri> _xuatKhoDuocPhamChiTietViTris;
        public virtual ICollection<XuatKhoDuocPhamChiTietViTri> XuatKhoDuocPhamChiTietViTris
        {
            get => _xuatKhoDuocPhamChiTietViTris ?? (_xuatKhoDuocPhamChiTietViTris = new List<XuatKhoDuocPhamChiTietViTri>());
            protected set => _xuatKhoDuocPhamChiTietViTris = value;
        }

        private ICollection<YeuCauDuocPhamBenhVien> _yeuCauDuocPhamBenhViens;
        public virtual ICollection<YeuCauDuocPhamBenhVien> YeuCauDuocPhamBenhViens
        {
            get => _yeuCauDuocPhamBenhViens ?? (_yeuCauDuocPhamBenhViens = new List<YeuCauDuocPhamBenhVien>());
            protected set => _yeuCauDuocPhamBenhViens = value;
        }

        private ICollection<YeuCauDichVuKyThuatTiemChung> _yeuCauDichVuKyThuatTiemChungs;
        public virtual ICollection<YeuCauDichVuKyThuatTiemChung> YeuCauDichVuKyThuatTiemChungs
        {
            get => _yeuCauDichVuKyThuatTiemChungs ?? (_yeuCauDichVuKyThuatTiemChungs = new List<YeuCauDichVuKyThuatTiemChung>());
            protected set => _yeuCauDichVuKyThuatTiemChungs = value;
        }
    }
}