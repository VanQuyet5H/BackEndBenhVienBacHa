using System.Collections.Generic;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Camino.Core.Domain.Entities.KhoaPhongChuyenKhoas;
using Camino.Core.Domain.Entities.KhoaPhongNhanViens;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Camino.Core.Domain.Entities.DichVuGiuongBenhViens;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.DuTruMuaDuocPhamTheoKhoas;
using Camino.Core.Domain.Entities.DuTruVatTus;
using Camino.Core.Domain.Entities.YeuCauNhapViens;

namespace Camino.Core.Domain.Entities.KhoaPhongs
{
    public class KhoaPhong : BaseEntity
    {

        public string Ten { get; set; }

        public string Ma { get; set; }

        public Enums.EnumLoaiKhoaPhong LoaiKhoaPhong { get; set; }

        public bool? CoKhamNgoaiTru { get; set; }
        public bool? CoKhamNoiTru { get; set; }

        public bool? IsDisabled { get; set; }

        public bool IsDefault { get; set; }

        public string MoTa { get; set; }
        public long? SoTienThuTamUng { get; set; }
        public int? SoGiuongKeHoach { get; set; }

        private ICollection<KhoaPhongChuyenKhoa> _khoaPhongChuyenKhoas;
        public virtual ICollection<KhoaPhongChuyenKhoa> KhoaPhongChuyenKhoas
        {
            get => _khoaPhongChuyenKhoas ?? (_khoaPhongChuyenKhoas = new List<KhoaPhongChuyenKhoa>());
            protected set => _khoaPhongChuyenKhoas = value;
        }

        //private ICollection<PhongNgoaiTru> _phongNgoaiTrus;

        //public virtual ICollection<PhongNgoaiTru> PhongNgoaiTrus
        //{
        //    get => _phongNgoaiTrus ?? (_phongNgoaiTrus = new List<PhongNgoaiTru>());
        //    protected set => _phongNgoaiTrus = value;
        //}

        private ICollection<KhoaPhongNhanVien> _khoaPhongNhanViens;

        public virtual ICollection<KhoaPhongNhanVien> KhoaPhongNhanViens
        {
            get => _khoaPhongNhanViens ?? (_khoaPhongNhanViens = new List<KhoaPhongNhanVien>());
            protected set => _khoaPhongNhanViens = value;
        }
        
        private ICollection<PhongBenhVien> _phongBenhViens;
        public virtual ICollection<PhongBenhVien> PhongBenhViens
        {
            get => _phongBenhViens ?? (_phongBenhViens = new List<PhongBenhVien>());
            protected set => _phongBenhViens = value;
        }

        private ICollection<Kho> _khoDuocPhams;
        public virtual ICollection<Kho> KhoDuocPhams
        {
            get => _khoDuocPhams ?? (_khoDuocPhams = new List<Kho>());
            protected set => _khoDuocPhams = value;
        }

        private ICollection<DuTruDuocPham> _duTruDuocPhams;
        public virtual ICollection<DuTruDuocPham> DuTruDuocPhams
        {
            get => _duTruDuocPhams ?? (_duTruDuocPhams = new List<DuTruDuocPham>());
            protected set => _duTruDuocPhams = value;
        }

        private ICollection<DichVuKhamBenhBenhVienNoiThucHien> _dichVuKhamBenhBenhVienNoiThucHiens;
        public virtual ICollection<DichVuKhamBenhBenhVienNoiThucHien> DichVuKhamBenhBenhVienNoiThucHiens
        {
            get => _dichVuKhamBenhBenhVienNoiThucHiens ?? (_dichVuKhamBenhBenhVienNoiThucHiens = new List<DichVuKhamBenhBenhVienNoiThucHien>());
            protected set => _dichVuKhamBenhBenhVienNoiThucHiens = value;
        }

        private ICollection<DichVuGiuongBenhVienNoiThucHien> _dichVuGiuongBenhVienNoiThucHiens { get; set; }
        public virtual ICollection<DichVuGiuongBenhVienNoiThucHien> DichVuGiuongBenhVienNoiThucHiens
        {
            get => _dichVuGiuongBenhVienNoiThucHiens ?? (_dichVuGiuongBenhVienNoiThucHiens = new List<DichVuGiuongBenhVienNoiThucHien>());
            protected set => _dichVuGiuongBenhVienNoiThucHiens = value;
        }

        private ICollection<DichVuKyThuatBenhVienNoiThucHien> _dichVuKyThuatBenhVienNoiThucHiens { get; set; }
        public virtual ICollection<DichVuKyThuatBenhVienNoiThucHien> DichVuKyThuatBenhVienNoiThucHiens
        {
            get => _dichVuKyThuatBenhVienNoiThucHiens ?? (_dichVuKyThuatBenhVienNoiThucHiens = new List<DichVuKyThuatBenhVienNoiThucHien>());
            protected set => _dichVuKyThuatBenhVienNoiThucHiens = value;
        }

        private ICollection<DuTruMuaVatTuTheoKhoa> _duTruMuaVatTuTheoKhoas { get; set; }
        public virtual ICollection<DuTruMuaVatTuTheoKhoa> DuTruMuaVatTuTheoKhoas
        {
            get => _duTruMuaVatTuTheoKhoas ?? (_duTruMuaVatTuTheoKhoas = new List<DuTruMuaVatTuTheoKhoa>());
            protected set => _duTruMuaVatTuTheoKhoas = value;
        }

        /// <summary>
        /// Update 02/06/2020
        /// </summary>
        private ICollection<YeuCauKhamBenh> _yeuCauKhamBenhs;
        public virtual ICollection<YeuCauKhamBenh> YeuCauKhamBenhs
        {
            get => _yeuCauKhamBenhs ?? (_yeuCauKhamBenhs = new List<YeuCauKhamBenh>());
            protected set => _yeuCauKhamBenhs = value;
        }

        private ICollection<DuTruMuaDuocPhamTheoKhoa> _duTruMuaDuocPhamTheoKhoas;
        public virtual ICollection<DuTruMuaDuocPhamTheoKhoa> DuTruMuaDuocPhamTheoKhoas
        {
            get => _duTruMuaDuocPhamTheoKhoas ?? (_duTruMuaDuocPhamTheoKhoas = new List<DuTruMuaDuocPhamTheoKhoa>());
            protected set => _duTruMuaDuocPhamTheoKhoas = value;
        }

        private ICollection<NoiTruBenhAn> _noiTruBenhAns;
        public virtual ICollection<NoiTruBenhAn> NoiTruBenhAns
        {
            get => _noiTruBenhAns ?? (_noiTruBenhAns = new List<NoiTruBenhAn>());
            protected set => _noiTruBenhAns = value;
        }

        private ICollection<NoiTruEkipDieuTri> _noiTruEkipDieuTris;
        public virtual ICollection<NoiTruEkipDieuTri> NoiTruEkipDieuTris
        {
            get => _noiTruEkipDieuTris ?? (_noiTruEkipDieuTris = new List<NoiTruEkipDieuTri>());
            protected set => _noiTruEkipDieuTris = value;
        }

        private ICollection<NoiTruKhoaPhongDieuTri> _khoaPhongChuyenDiNoiTruKhoaPhongDieuTris;
        public virtual ICollection<NoiTruKhoaPhongDieuTri> KhoaPhongChuyenDiNoiTruKhoaPhongDieuTris
        {
            get => _khoaPhongChuyenDiNoiTruKhoaPhongDieuTris ?? (_khoaPhongChuyenDiNoiTruKhoaPhongDieuTris = new List<NoiTruKhoaPhongDieuTri>());
            protected set => _khoaPhongChuyenDiNoiTruKhoaPhongDieuTris = value;
        }

        private ICollection<NoiTruKhoaPhongDieuTri> _khoaPhongChuyenDenNoiTruKhoaPhongDieuTris;
        public virtual ICollection<NoiTruKhoaPhongDieuTri> KhoaPhongChuyenDenNoiTruKhoaPhongDieuTris
        {
            get => _khoaPhongChuyenDenNoiTruKhoaPhongDieuTris ?? (_khoaPhongChuyenDenNoiTruKhoaPhongDieuTris = new List<NoiTruKhoaPhongDieuTri>());
            protected set => _khoaPhongChuyenDenNoiTruKhoaPhongDieuTris = value;
        }
        private ICollection<NoiTruPhieuDieuTri> _noiTruPhieuDieuTris;
        public virtual ICollection<NoiTruPhieuDieuTri> NoiTruPhieuDieuTris
        {
            get => _noiTruPhieuDieuTris ?? (_noiTruPhieuDieuTris = new List<NoiTruPhieuDieuTri>());
            protected set => _noiTruPhieuDieuTris = value;
        }

        private ICollection<YeuCauNhapVien> _yeuCauNhapViens;
        public virtual ICollection<YeuCauNhapVien> YeuCauNhapViens
        {
            get => _yeuCauNhapViens ?? (_yeuCauNhapViens = new List<YeuCauNhapVien>());
            protected set => _yeuCauNhapViens = value;
        }

        private ICollection<YeuCauTraDuocPhamTuBenhNhan> _yeuCauTraDuocPhamTuBenhNhans;
        public virtual ICollection<YeuCauTraDuocPhamTuBenhNhan> YeuCauTraDuocPhamTuBenhNhans
        {
            get => _yeuCauTraDuocPhamTuBenhNhans ?? (_yeuCauTraDuocPhamTuBenhNhans = new List<YeuCauTraDuocPhamTuBenhNhan>());
            protected set => _yeuCauTraDuocPhamTuBenhNhans = value;
        }

        private ICollection<YeuCauTraVatTuTuBenhNhan> _yeuCauTraVatTuTuBenhNhans;
        public virtual ICollection<YeuCauTraVatTuTuBenhNhan> YeuCauTraVatTuTuBenhNhans
        {
            get => _yeuCauTraVatTuTuBenhNhans ?? (_yeuCauTraVatTuTuBenhNhans = new List<YeuCauTraVatTuTuBenhNhan>());
            protected set => _yeuCauTraVatTuTuBenhNhans = value;
        }
        private ICollection<YeuCauDichVuGiuongBenhVienChiPhiBHYT> _yeuCauDichVuGiuongBenhVienChiPhiBHYTs;
        public virtual ICollection<YeuCauDichVuGiuongBenhVienChiPhiBHYT> YeuCauDichVuGiuongBenhVienChiPhiBHYTs
        {
            get => _yeuCauDichVuGiuongBenhVienChiPhiBHYTs ?? (_yeuCauDichVuGiuongBenhVienChiPhiBHYTs = new List<YeuCauDichVuGiuongBenhVienChiPhiBHYT>());
            protected set => _yeuCauDichVuGiuongBenhVienChiPhiBHYTs = value;
        }

        private ICollection<YeuCauDichVuGiuongBenhVienChiPhiBenhVien> _yeuCauDichVuGiuongBenhVienChiPhiBenhViens;
        public virtual ICollection<YeuCauDichVuGiuongBenhVienChiPhiBenhVien> YeuCauDichVuGiuongBenhVienChiPhiBenhViens
        {
            get => _yeuCauDichVuGiuongBenhVienChiPhiBenhViens ?? (_yeuCauDichVuGiuongBenhVienChiPhiBenhViens = new List<YeuCauDichVuGiuongBenhVienChiPhiBenhVien>());
            protected set => _yeuCauDichVuGiuongBenhVienChiPhiBenhViens = value;
        }

        private ICollection<DichVuKyThuatBenhVien> _dichVuKyThuatBenhViens;
        public virtual ICollection<DichVuKyThuatBenhVien> DichVuKyThuatBenhViens
        {
            get => _dichVuKyThuatBenhViens ?? (_dichVuKyThuatBenhViens = new List<DichVuKyThuatBenhVien>());
            protected set => _dichVuKyThuatBenhViens = value;
        }
    }
}
