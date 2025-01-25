using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.Entities.NhaThaus;
using System;
using System.Collections.Generic;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauNhapKhoDuocPhams;
using Camino.Core.Domain.Entities.YeuCauTraDuocPhams;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.YeuCauDieuChuyenDuocPhams;
namespace Camino.Core.Domain.Entities.HopDongThauDuocPhams
{
    public class HopDongThauDuocPham : BaseEntity
    {
        public long NhaThauId { get; set; }

        public string SoHopDong { get; set; }

        public string SoQuyetDinh { get; set; }

        public DateTime CongBo { get; set; }

        public DateTime? NgayKy { get; set; }

        public DateTime NgayHieuLuc { get; set; }

        public DateTime NgayHetHan { get; set; }

        public Enums.EnumLoaiThau LoaiThau { get; set; }

        public Enums.EnumLoaiThuocThau LoaiThuocThau { get; set; }

        public string NhomThau { get; set; }

        public string GoiThau { get; set; }

        public int Nam { get; set; }

        public bool? HeThongTuPhatSinh { get; set; }

        public virtual NhaThau NhaThau { get; set; }

        private ICollection<HopDongThauDuocPhamChiTiet> _hopDongThauDuocPhamChiTiet;

        public virtual ICollection<HopDongThauDuocPhamChiTiet> HopDongThauDuocPhamChiTiets
        {
            get => _hopDongThauDuocPhamChiTiet ?? (_hopDongThauDuocPhamChiTiet = new List<HopDongThauDuocPhamChiTiet>());
            protected set => _hopDongThauDuocPhamChiTiet = value;
        }

        private ICollection<NhapKhoDuocPhamChiTiet> _nhapKhoDuocPhamChiTiet;

        public virtual ICollection<NhapKhoDuocPhamChiTiet> NhapKhoDuocPhamChiTiets
        {
            get => _nhapKhoDuocPhamChiTiet ?? (_nhapKhoDuocPhamChiTiet = new List<NhapKhoDuocPhamChiTiet>());
            protected set => _nhapKhoDuocPhamChiTiet = value;
        }
        private ICollection<YeuCauDuocPhamBenhVien> _yeuCauDuocPhamBenhViens { get; set; }
        public virtual ICollection<YeuCauDuocPhamBenhVien> YeuCauDuocPhamBenhViens
        {
            get => _yeuCauDuocPhamBenhViens ?? (_yeuCauDuocPhamBenhViens = new List<YeuCauDuocPhamBenhVien>());
            protected set => _yeuCauDuocPhamBenhViens = value;
        }
        private ICollection<DonThuocThanhToanChiTiet> _donThuocThanhToanChiTiets;
        public virtual ICollection<DonThuocThanhToanChiTiet> DonThuocThanhToanChiTiets
        {
            get => _donThuocThanhToanChiTiets ?? (_donThuocThanhToanChiTiets = new List<DonThuocThanhToanChiTiet>());
            protected set => _donThuocThanhToanChiTiets = value;
        }

        private ICollection<YeuCauNhapKhoDuocPhamChiTiet> _yeuCauNhapKhoDuocPhamChiTiets;
        public virtual ICollection<YeuCauNhapKhoDuocPhamChiTiet> YeuCauNhapKhoDuocPhamChiTiets
        {
            get => _yeuCauNhapKhoDuocPhamChiTiets ?? (_yeuCauNhapKhoDuocPhamChiTiets = new List<YeuCauNhapKhoDuocPhamChiTiet>());
            protected set => _yeuCauNhapKhoDuocPhamChiTiets = value;

        }

        private ICollection<YeuCauTraDuocPhamChiTiet> _yeuCauTraDuocPhamChiTiets { get; set; }
        public virtual ICollection<YeuCauTraDuocPhamChiTiet> YeuCauTraDuocPhamChiTiets
        {
            get => _yeuCauTraDuocPhamChiTiets ?? (_yeuCauTraDuocPhamChiTiets = new List<YeuCauTraDuocPhamChiTiet>());
            protected set => _yeuCauTraDuocPhamChiTiets = value;
        }

        private ICollection<YeuCauXuatKhoDuocPhamChiTiet> _yeuCauXuatKhoDuocPhamChiTiets;
        public virtual ICollection<YeuCauXuatKhoDuocPhamChiTiet> YeuCauXuatKhoDuocPhamChiTiets
        {
            get => _yeuCauXuatKhoDuocPhamChiTiets ?? (_yeuCauXuatKhoDuocPhamChiTiets = new List<YeuCauXuatKhoDuocPhamChiTiet>());
            protected set => _yeuCauXuatKhoDuocPhamChiTiets = value;
        }
        private ICollection<YeuCauDieuChuyenDuocPhamChiTiet> _yeuCauDieuChuyenDuocPhamChiTiets;
        public virtual ICollection<YeuCauDieuChuyenDuocPhamChiTiet> YeuCauDieuChuyenDuocPhamChiTiets
        {
            get => _yeuCauDieuChuyenDuocPhamChiTiets ?? (_yeuCauDieuChuyenDuocPhamChiTiets = new List<YeuCauDieuChuyenDuocPhamChiTiet>());
            protected set => _yeuCauDieuChuyenDuocPhamChiTiets = value;
        }
    }
}
