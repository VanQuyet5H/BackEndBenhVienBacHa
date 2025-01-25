using System.Collections.Generic;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.DonViTinhs;
using Camino.Core.Domain.Entities.DuocPhamBenhViens;
using Camino.Core.Domain.Entities.DuTruMuaDuocPhamKhoDuocs;
using Camino.Core.Domain.Entities.DuTruMuaDuocPhams;
using Camino.Core.Domain.Entities.DuTruMuaDuocPhamTheoKhoas;
using Camino.Core.Domain.Entities.HopDongThauDuocPhams;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.NoiTruDonThuocs;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;

namespace Camino.Core.Domain.Entities.Thuocs
{
    public class DuocPham : BaseEntity
    {
        public string Ten { get; set; }
        public string TenTiengAnh { get; set; }
        public string SoDangKy { get; set; }
        public int? STTHoatChat { get; set; }
        public string MaHoatChat { get; set; }
        public string HoatChat { get; set; }
        public Enums.LoaiThuocHoacHoatChat LoaiThuocHoacHoatChat { get; set; }
        public string NhaSanXuat { get; set; }
        public string NuocSanXuat { get; set; }
        public long DuongDungId { get; set; }
      
        public string HamLuong { get; set; }
        public string QuyCach { get; set; }
        public string TieuChuan { get; set; }
        public string DangBaoChe { get; set; }
        public long DonViTinhId { get; set; }
        
        public string HuongDan { get; set; }
        public string MoTa { get; set; }
        public string ChiDinh { get; set; }
        public string ChongChiDinh { get; set; }
        public string LieuLuongCachDung { get; set; }
        public string TacDungPhu { get; set; }
        public string ChuYDePhong { get; set; }
        public bool? IsDisabled { get; set; }
        public bool? LaThucPhamChucNang { get; set; }
        //public string DonViTinhThamKhao { get; set; }
        public int? TheTich { get; set; }
        public int? HeSoDinhMucDonViTinh { get; set; }
        public bool? LaThuocHuongThanGayNghien { get; set; }

        public string MaATC { get; set; }

        public bool? DuocPhamCoDau { get; set; }

        //public virtual DuocPhamBenhViens.DuocPhamBenhVien DuocPhamBenhVien { get; set; }
        public virtual DonViTinh DonViTinh { get; set; }
        public virtual DuongDung DuongDung { get; set; }

        public virtual DuocPhamBenhVien DuocPhamBenhVien { get; set; }

        private ICollection<HopDongThauDuocPhamChiTiet> _hopDongThauDuocPhamChiTiet;

        public virtual ICollection<HopDongThauDuocPhamChiTiet> HopDongThauDuocPhamChiTiets
        {
            get => _hopDongThauDuocPhamChiTiet ?? (_hopDongThauDuocPhamChiTiet = new List<HopDongThauDuocPhamChiTiet>());
            protected set => _hopDongThauDuocPhamChiTiet = value;
        }

        private ICollection<YeuCauKhamBenhDonThuocChiTiet> _yeuCauKhamBenhDonThuocChiTiets;
        public virtual ICollection<YeuCauKhamBenhDonThuocChiTiet> YeuCauKhamBenhDonThuocChiTiets
        {
            get => _yeuCauKhamBenhDonThuocChiTiets ?? (_yeuCauKhamBenhDonThuocChiTiets = new List<YeuCauKhamBenhDonThuocChiTiet>());
            protected set => _yeuCauKhamBenhDonThuocChiTiets = value;
        }
        private ICollection<DuTruDuocPham> _duTruDuocPhams;
        public virtual ICollection<DuTruDuocPham> DuTruDuocPhams
        {
            get => _duTruDuocPhams ?? (_duTruDuocPhams = new List<DuTruDuocPham>());
            protected set => _duTruDuocPhams = value;
        }
        private ICollection<DonThuocThanhToanChiTiet> _donThuocThanhToanChiTiets;
        public virtual ICollection<DonThuocThanhToanChiTiet> DonThuocThanhToanChiTiets
        {
            get => _donThuocThanhToanChiTiets ?? (_donThuocThanhToanChiTiets = new List<DonThuocThanhToanChiTiet>());
            protected set => _donThuocThanhToanChiTiets = value;
        }
        private ICollection<ToaThuocMauChiTiet> _toaThuocMauChiTiets;
        public virtual ICollection<ToaThuocMauChiTiet> ToaThuocMauChiTiets
        {
            get => _toaThuocMauChiTiets ?? (_toaThuocMauChiTiets = new List<ToaThuocMauChiTiet>());
            protected set => _toaThuocMauChiTiets = value;
        }
        private ICollection<DuTruMuaDuocPhamChiTiet> _duTruMuaDuocPhamChiTiets;
        public virtual ICollection<DuTruMuaDuocPhamChiTiet> DuTruMuaDuocPhamChiTiets
        {
            get => _duTruMuaDuocPhamChiTiets ?? (_duTruMuaDuocPhamChiTiets = new List<DuTruMuaDuocPhamChiTiet>());
            protected set => _duTruMuaDuocPhamChiTiets = value;
        }
        private ICollection<DuTruMuaDuocPhamTheoKhoaChiTiet> _duTruMuaDuocPhamTheoKhoaChiTiets;
        public virtual ICollection<DuTruMuaDuocPhamTheoKhoaChiTiet> DuTruMuaDuocPhamTheoKhoaChiTiets
        {
            get => _duTruMuaDuocPhamTheoKhoaChiTiets ?? (_duTruMuaDuocPhamTheoKhoaChiTiets = new List<DuTruMuaDuocPhamTheoKhoaChiTiet>());
            protected set => _duTruMuaDuocPhamTheoKhoaChiTiets = value;
        }
        private ICollection<DuTruMuaDuocPhamKhoDuocChiTiet> _duTruMuaDuocPhamKhoDuocChiTiets;
        public virtual ICollection<DuTruMuaDuocPhamKhoDuocChiTiet> DuTruMuaDuocPhamKhoDuocChiTiets
        {
            get => _duTruMuaDuocPhamKhoDuocChiTiets ?? (_duTruMuaDuocPhamKhoDuocChiTiets = new List<DuTruMuaDuocPhamKhoDuocChiTiet>());
            protected set => _duTruMuaDuocPhamKhoDuocChiTiets = value;
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
