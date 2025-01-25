using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.NhapKhoDuocPhams;

namespace Camino.Core.Domain.Entities.YeuCauLinhDuocPhams
{
    public class YeuCauLinhDuocPham : BaseEntity
    {
        public long KhoXuatId { get; set; }
        public long KhoNhapId { get; set; }
        public Enums.EnumLoaiPhieuLinh LoaiPhieuLinh { get; set; }
        public long NhanVienYeuCauId { get; set; }
        public long? NoiYeuCauId { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public string GhiChu { get; set; }
        public bool? DuocDuyet { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public long? NhanVienDuyetId { get; set; }
        public string LyDoKhongDuyet { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string SoPhieu { get; set; }

        public DateTime? ThoiDiemLinhTongHopTuNgay { get; set; }
        public DateTime? ThoiDiemLinhTongHopDenNgay { get; set; }

        public bool? DaGui { get; set; }
        public int? LoaiThuocTao { get; set; }

        public virtual Kho KhoXuat { get; set; }
        public virtual Kho KhoNhap { get; set; }
        public virtual NhanVien NhanVienYeuCau { get; set; }
        public virtual NhanVien NhanVienDuyet { get; set; }
        public virtual PhongBenhVien NoiYeuCau { get; set; }

        private ICollection<YeuCauLinhDuocPhamChiTiet> _yeuCauLinhDuocPhamChiTiets { get; set; }
        public virtual ICollection<YeuCauLinhDuocPhamChiTiet> YeuCauLinhDuocPhamChiTiets
        {
            get => _yeuCauLinhDuocPhamChiTiets ?? (_yeuCauLinhDuocPhamChiTiets = new List<YeuCauLinhDuocPhamChiTiet>());
            protected set => _yeuCauLinhDuocPhamChiTiets = value;
        }

        private ICollection<XuatKhoDuocPham> _xuatKhoDuocPhams { get; set; }
        public virtual ICollection<XuatKhoDuocPham> XuatKhoDuocPhams
        {
            get => _xuatKhoDuocPhams ?? (_xuatKhoDuocPhams = new List<XuatKhoDuocPham>());
            protected set => _xuatKhoDuocPhams = value;
        }

        private ICollection<YeuCauDuocPhamBenhVien> _yeuCauDuocPhamBenhViens;
        public virtual ICollection<YeuCauDuocPhamBenhVien> YeuCauDuocPhamBenhViens
        {
            get => _yeuCauDuocPhamBenhViens ?? (_yeuCauDuocPhamBenhViens = new List<YeuCauDuocPhamBenhVien>());
            protected set => _yeuCauDuocPhamBenhViens = value;
        }

        private ICollection<NhapKhoDuocPham> _nhapKhoDuocPhams;
        public virtual ICollection<NhapKhoDuocPham> NhapKhoDuocPhams
        {
            get => _nhapKhoDuocPhams ?? (_nhapKhoDuocPhams = new List<NhapKhoDuocPham>());
            protected set => _nhapKhoDuocPhams = value;
        }
    }
}
