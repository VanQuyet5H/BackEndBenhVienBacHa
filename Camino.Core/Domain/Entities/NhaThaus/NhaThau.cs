using Camino.Core.Domain.Entities.HopDongThauDuocPhams;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.DonVatTus;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.Entities.HopDongThauVatTus;
using Camino.Core.Domain.Entities.MauVaChePhams;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.XuatKhoVatTus;

namespace Camino.Core.Domain.Entities.NhaThaus
{
    public class NhaThau : BaseEntity
    {
        public string Ma { get; set; }
        public string Ten { get; set; }

        public string DiaChi { get; set; }

        public string MaSoThue { get; set; }

        public string TaiKhoanNganHang { get; set; }

        public string NguoiDaiDien { get; set; }

        public string NguoiLienHe { get; set; }

        public string SoDienThoaiLienHe { get; set; }


        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string SoDienThoaiDisplay { get; set; }

        public string EmailLienHe { get; set; }

        private ICollection<HopDongThauDuocPham> _hopDongThauDuocPham;

        public virtual ICollection<HopDongThauDuocPham> HopDongThauDuocPhams
        {
            get => _hopDongThauDuocPham ?? (_hopDongThauDuocPham = new List<HopDongThauDuocPham>());
            protected set => _hopDongThauDuocPham = value;
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

        private ICollection<HopDongThauVatTu> _hopDongThauVatTus;
        public virtual ICollection<HopDongThauVatTu> HopDongThauVatTus
        {
            get => _hopDongThauVatTus ?? (_hopDongThauVatTus = new List<HopDongThauVatTu>());
            protected set => _hopDongThauVatTus = value;
        }

        private ICollection<DonVTYTThanhToanChiTiet> _donVTYTThanhToanChiTiets;
        public virtual ICollection<DonVTYTThanhToanChiTiet> DonVTYTThanhToanChiTiets
        {
            get => _donVTYTThanhToanChiTiets ?? (_donVTYTThanhToanChiTiets = new List<DonVTYTThanhToanChiTiet>());
            protected set => _donVTYTThanhToanChiTiets = value;
        }

        private ICollection<YeuCauVatTuBenhVien> _yeuCauVatTuBenhViens;
        public virtual ICollection<YeuCauVatTuBenhVien> YeuCauVatTuBenhViens
        {
            get => _yeuCauVatTuBenhViens ?? (_yeuCauVatTuBenhViens = new List<YeuCauVatTuBenhVien>());
            protected set => _yeuCauVatTuBenhViens = value;
        }

        private ICollection<NhapKhoMau> _nhapKhoMaus;
        public virtual ICollection<NhapKhoMau> NhapKhoMaus
        {
            get => _nhapKhoMaus ?? (_nhapKhoMaus = new List<NhapKhoMau>());
            protected set => _nhapKhoMaus = value;
        }

        private ICollection<XuatKhoDuocPham> _xuatKhoDuocPhams;
        public virtual ICollection<XuatKhoDuocPham> XuatKhoDuocPhams
        {
            get => _xuatKhoDuocPhams ?? (_xuatKhoDuocPhams = new List<XuatKhoDuocPham>());
            protected set => _xuatKhoDuocPhams = value;
        }

        private ICollection<YeuCauXuatKhoDuocPham> _yeuCauXuatKhoDuocPhams;
        public virtual ICollection<YeuCauXuatKhoDuocPham> YeuCauXuatKhoDuocPhams
        {
            get => _yeuCauXuatKhoDuocPhams ?? (_yeuCauXuatKhoDuocPhams = new List<YeuCauXuatKhoDuocPham>());
            protected set => _yeuCauXuatKhoDuocPhams = value;
        }


        private ICollection<XuatKhoVatTu> _xuatKhoVatTus;
        public virtual ICollection<XuatKhoVatTu> XuatKhoVatTus
        {
            get => _xuatKhoVatTus ?? (_xuatKhoVatTus = new List<XuatKhoVatTu>());
            protected set => _xuatKhoVatTus = value;
        }

        private ICollection<YeuCauXuatKhoVatTu> _yeuCauXuatKhoVatTus;
        public virtual ICollection<YeuCauXuatKhoVatTu> YeuCauXuatKhoVatTus
        {
            get => _yeuCauXuatKhoVatTus ?? (_yeuCauXuatKhoVatTus = new List<YeuCauXuatKhoVatTu>());
            protected set => _yeuCauXuatKhoVatTus = value;
        }
    }
}
