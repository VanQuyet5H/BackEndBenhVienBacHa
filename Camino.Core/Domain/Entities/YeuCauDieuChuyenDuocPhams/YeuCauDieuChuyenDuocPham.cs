using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.NhanViens;
using System;
using System.Collections.Generic;

namespace Camino.Core.Domain.Entities.YeuCauDieuChuyenDuocPhams
{
    public class YeuCauDieuChuyenDuocPham : BaseEntity
    {
        public long KhoXuatId { get; set; }
        public long KhoNhapId { get; set; }
        public long NguoiXuatId { get; set; }
        public long NguoiNhapId { get; set; }
        public bool? DuocKeToanDuyet { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public long? NhanVienDuyetId { get; set; }
        public string LyDoKhongDuyet { get; set; }
        public bool? HienThiCaThuocHetHan { get; set; }
        public virtual Kho KhoXuat { get; set; }
        public virtual Kho KhoNhap { get; set; }
        public virtual NhanVien NguoiNhap { get; set; }
        public virtual NhanVien NguoiXuat { get; set; }
        public virtual NhanVien NhanVienDuyet { get; set; }

        private ICollection<YeuCauDieuChuyenDuocPhamChiTiet> _yeuCauDieuChuyenDuocPhamChiTiets;
        public virtual ICollection<YeuCauDieuChuyenDuocPhamChiTiet> YeuCauDieuChuyenDuocPhamChiTiets
        {
            get => _yeuCauDieuChuyenDuocPhamChiTiets ?? (_yeuCauDieuChuyenDuocPhamChiTiets = new List<YeuCauDieuChuyenDuocPhamChiTiet>());
            protected set => _yeuCauDieuChuyenDuocPhamChiTiets = value;
        }
    }
}
