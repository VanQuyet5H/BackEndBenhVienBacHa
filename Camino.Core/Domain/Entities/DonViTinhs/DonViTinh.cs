using System.Collections.Generic;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.Entities.NoiTruDonThuocs;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.Entities.VatTus;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;

namespace Camino.Core.Domain.Entities.DonViTinhs
{
    public class DonViTinh : BaseEntity
    {
        public string Ten { get; set; }
        public string Ma { get; set; }
        public string MoTa { get; set; }

        private ICollection<DuocPham> _duocPhams;
        public virtual ICollection<DuocPham> DuocPhams
        {
            get => _duocPhams ?? (_duocPhams = new List<DuocPham>());
            protected set => _duocPhams = value;
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

        private ICollection<NoiTruChiDinhDuocPham> _noiTruChiDinhDuocPhams { get; set; }
        public virtual ICollection<NoiTruChiDinhDuocPham> NoiTruChiDinhDuocPhams
        {
            get => _noiTruChiDinhDuocPhams ?? (_noiTruChiDinhDuocPhams = new List<NoiTruChiDinhDuocPham>());
            protected set => _noiTruChiDinhDuocPhams = value;
        }

        private ICollection<TuVanThuocKhamSucKhoe> _tuVanThuocKhamSucKhoes;
        public virtual ICollection<TuVanThuocKhamSucKhoe> TuVanThuocKhamSucKhoes
        {
            get => _tuVanThuocKhamSucKhoes ?? (_tuVanThuocKhamSucKhoes = new List<TuVanThuocKhamSucKhoe>());
            protected set => _tuVanThuocKhamSucKhoes = value;
        }

        private ICollection<NoiTruPhieuDieuTriTuVanThuoc> _noiTruPhieuDieuTriTuVanThuocs;
        public virtual ICollection<NoiTruPhieuDieuTriTuVanThuoc> NoiTruPhieuDieuTriTuVanThuocs
        {
            get => _noiTruPhieuDieuTriTuVanThuocs ?? (_noiTruPhieuDieuTriTuVanThuocs = new List<NoiTruPhieuDieuTriTuVanThuoc>());
            protected set => _noiTruPhieuDieuTriTuVanThuocs = value;
        }

        //update 15/06/2021
        private ICollection<NoiTruDonThuocChiTiet> _noiTruDonThuocChiTiets;
        public virtual ICollection<NoiTruDonThuocChiTiet> NoiTruDonThuocChiTiets
        {
            get => _noiTruDonThuocChiTiets ?? (_noiTruDonThuocChiTiets = new List<NoiTruDonThuocChiTiet>());
            protected set => _noiTruDonThuocChiTiets = value;
        }
    }
}
