using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.MauVaChePhams;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.NoiDungGhiChuMiemGiams
{
    public class NoiDungGhiChuMiemGiam : BaseEntity
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string NoiDungMiemGiam { get; set; }

        private ICollection<YeuCauKhamBenh> _yeuCauKhamBenhs;
        public virtual ICollection<YeuCauKhamBenh> YeuCauKhamBenhs
        {
            get => _yeuCauKhamBenhs ?? (_yeuCauKhamBenhs = new List<YeuCauKhamBenh>());
            protected set => _yeuCauKhamBenhs = value;
        }

        private ICollection<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuats;
        public virtual ICollection<YeuCauDichVuKyThuat> YeuCauDichVuKyThuats
        {
            get => _yeuCauDichVuKyThuats ?? (_yeuCauDichVuKyThuats = new List<YeuCauDichVuKyThuat>());
            protected set => _yeuCauDichVuKyThuats = value;
        }

        private ICollection<YeuCauDuocPhamBenhVien> _yeuCauDuocPhamBenhViens;
        public virtual ICollection<YeuCauDuocPhamBenhVien> YeuCauDuocPhamBenhViens
        {
            get => _yeuCauDuocPhamBenhViens ?? (_yeuCauDuocPhamBenhViens = new List<YeuCauDuocPhamBenhVien>());
            protected set => _yeuCauDuocPhamBenhViens = value;
        }

        private ICollection<YeuCauVatTuBenhVien> _yeuCauVatTuBenhViens;
        public virtual ICollection<YeuCauVatTuBenhVien> YeuCauVatTuBenhViens
        {
            get => _yeuCauVatTuBenhViens ?? (_yeuCauVatTuBenhViens = new List<YeuCauVatTuBenhVien>());
            protected set => _yeuCauVatTuBenhViens = value;
        }
        
        private ICollection<DonThuocThanhToanChiTiet> _donThuocThanhToanChiTiets;
        public virtual ICollection<DonThuocThanhToanChiTiet> DonThuocThanhToanChiTiets
        {
            get => _donThuocThanhToanChiTiets ?? (_donThuocThanhToanChiTiets = new List<DonThuocThanhToanChiTiet>());
            protected set => _donThuocThanhToanChiTiets = value;
        }

        private ICollection<YeuCauDichVuGiuongBenhVien> _yeuCauDichVuGiuongBenhViens;
        public virtual ICollection<YeuCauDichVuGiuongBenhVien> YeuCauDichVuGiuongBenhViens
        {
            get => _yeuCauDichVuGiuongBenhViens ?? (_yeuCauDichVuGiuongBenhViens = new List<YeuCauDichVuGiuongBenhVien>());
            protected set => _yeuCauDichVuGiuongBenhViens = value;
        }

        private ICollection<YeuCauGoiDichVu> _yeuCauGoiDichVus;
        public virtual ICollection<YeuCauGoiDichVu> YeuCauGoiDichVus
        {
            get => _yeuCauGoiDichVus ?? (_yeuCauGoiDichVus = new List<YeuCauGoiDichVu>());
            protected set => _yeuCauGoiDichVus = value;
        }

        private ICollection<YeuCauTruyenMau> _yeuCauTruyenMaus;
        public virtual ICollection<YeuCauTruyenMau> YeuCauTruyenMaus
        {
            get => _yeuCauTruyenMaus ?? (_yeuCauTruyenMaus = new List<YeuCauTruyenMau>());
            protected set => _yeuCauTruyenMaus = value;
        }

        private ICollection<YeuCauDichVuGiuongBenhVienChiPhiBenhVien> _yeuCauDichVuGiuongBenhVienChiPhiBenhViens;
        public virtual ICollection<YeuCauDichVuGiuongBenhVienChiPhiBenhVien> YeuCauDichVuGiuongBenhVienChiPhiBenhViens
        {
            get => _yeuCauDichVuGiuongBenhVienChiPhiBenhViens ?? (_yeuCauDichVuGiuongBenhVienChiPhiBenhViens = new List<YeuCauDichVuGiuongBenhVienChiPhiBenhVien>());
            protected set => _yeuCauDichVuGiuongBenhVienChiPhiBenhViens = value;
        }
    }
}
