using System;
using System.Collections.Generic;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.MayXetNghiems;
using Camino.Core.Domain.Entities.YeuCauDieuChuyenDuocPhams;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTraDuocPhams;

namespace Camino.Core.Domain.Entities.XuatKhos
{
    public class XuatKhoDuocPhamChiTietViTri : BaseEntity
    {
        public long XuatKhoDuocPhamChiTietId { get; set; }
        public long NhapKhoDuocPhamChiTietId { get; set; }
        public virtual NhapKhoDuocPhamChiTiets.NhapKhoDuocPhamChiTiet NhapKhoDuocPhamChiTiet { get; set; }
        public double SoLuongXuat { get; set; }
        public DateTime? NgayXuat { get; set; }
        public long? YeuCauTraDuocPhamTuBenhNhanChiTietId { get; set; }
        public string GhiChu { get; set; }
        public long? MayXetNghiemId { get; set; }

        public virtual MayXetNghiem MayXetNghiem { get; set; }
        public virtual XuatKhoDuocPhamChiTiet XuatKhoDuocPhamChiTiet { get; set; }
        public virtual YeuCauTraDuocPhamTuBenhNhanChiTiet YeuCauTraDuocPhamTuBenhNhanChiTiet { get; set; }
        private ICollection<DonThuocThanhToanChiTiet> _donThuocThanhToanChiTiets;
        public virtual ICollection<DonThuocThanhToanChiTiet> DonThuocThanhToanChiTiets
        {
            get => _donThuocThanhToanChiTiets ?? (_donThuocThanhToanChiTiets = new List<DonThuocThanhToanChiTiet>());
            protected set => _donThuocThanhToanChiTiets = value;
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