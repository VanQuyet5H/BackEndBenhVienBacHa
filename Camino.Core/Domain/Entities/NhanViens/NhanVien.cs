using Camino.Core.Domain.Entities.ChucDanhs;
using Camino.Core.Domain.Entities.HocViHocHams;
using Camino.Core.Domain.Entities.PhamViHanhNghes;
using Camino.Core.Domain.Entities.VanBangChuyenMons;
using System;
using System.Collections.Generic;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.DonVatTus;
using Camino.Core.Domain.Entities.DuyetBaoHiems;
using Camino.Core.Domain.Entities.KetQuaSinhHieus;
using Camino.Core.Domain.Entities.KhoaPhongNhanViens;
using Camino.Core.Domain.Entities.LichPhanCongNgoaiTrus;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.NhapKhoDuocPhams;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.Entities.GoiDichVus;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.Entities.NguoiGioiThieus;
using Camino.Core.Domain.Entities.KetQuaNhomXetNghiems;
using Camino.Core.Domain.Entities.PhauThuatThuThuats;
using Camino.Core.Domain.Entities.KhoNhanVienQuanLys;
using Camino.Core.Domain.Entities.YeuCauNhapKhoVatTus;
using Camino.Core.Domain.Entities.YeuCauLinhVatTus;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.YeuCauLinhDuocPhams;
using Camino.Core.Domain.Entities.YeuCauNhapKhoDuocPhams;
using Camino.Core.Domain.Entities.YeuCauTraDuocPhams;
using Camino.Core.Domain.Entities.YeuCauTraVatTus;
using Camino.Core.Domain.Entities.NhapKhoQuaTangs;
using Camino.Core.Domain.Entities.XuatKhoQuaTangs;
using Camino.Core.Domain.Entities.KyDuTruMuaDuocPhamVatTus;
using Camino.Core.Domain.Entities.DuTruMuaDuocPhams;
using Camino.Core.Domain.Entities.DuTruMuaDuocPhamTheoKhoas;
using Camino.Core.Domain.Entities.DuTruMuaDuocPhamKhoDuocs;
using Camino.Core.Domain.Entities.DuTruVatTus;
using Camino.Core.Domain.Entities.GachNos;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.Entities.MauVaChePhams;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.Entities.YeuCauNhapViens;
using Camino.Core.Domain.Entities.YeuCauDieuChuyenDuocPhams;
using Camino.Core.Domain.Entities.NoiTruDonThuocs;
using Camino.Core.Domain.Entities.ChucVus;
using Camino.Core.Domain.Entities.NhanVienChucVus;
using Camino.Core.Domain.Entities.HoSoNhanVienDinhKems;
using Camino.Core.Domain.Entities.CauHinhs;

namespace Camino.Core.Domain.Entities.NhanViens
{
    public class NhanVien : BaseEntity
    {
        public long? HocHamHocViId { get; set; }
        public long? PhamViHanhNgheId { get; set; }
        public long? ChucDanhId { get; set; }
        public long? VanBangChuyenMonId { get; set; }

        public string MaChungChiHanhNghe { get; set; }
        public DateTime? NgayCapChungChiHanhNghe { get; set; }
        public string NoiCapChungChiHanhNghe { get; set; }
        public DateTime? NgayKyHopDong { get; set; }
        public DateTime? NgayHetHopDong { get; set; }


        public string QuyenHan { get; set; }
        public string GhiChu { get; set; }

        //BVHD-3925
        public string MaNhanVien { get; set; }
        public DateTime? NgayDangKyHanhNghe { get; set; }

        public virtual Users.User User { get; set; }

        private ICollection<NhanVienRole> _nhanVienRoles;
        public virtual ICollection<NhanVienRole> NhanVienRoles
        {
            get => _nhanVienRoles ?? (_nhanVienRoles = new List<NhanVienRole>());
            protected set => _nhanVienRoles = value;
        }

        public virtual HocViHocHam HocHamHocVi { get; set; }
        public virtual PhamViHanhNghe PhamViHanhNghe { get; set; }
        public virtual ChucDanh ChucDanh { get; set; }
        public virtual VanBangChuyenMon VanBangChuyenMon { get; set; }

        private ICollection<KhoaPhongNhanVien> _khoaPhongNhanViens;
        public virtual ICollection<KhoaPhongNhanVien> KhoaPhongNhanViens
        {
            get => _khoaPhongNhanViens ?? (_khoaPhongNhanViens = new List<KhoaPhongNhanVien>());
            protected set => _khoaPhongNhanViens = value;
        }

        //private ICollection<YeuCauKhamBenh> _yeuCauKhamBenhs;
        //public virtual ICollection<YeuCauKhamBenh> YeuCauKhamBenhs
        //{
        //    get => _yeuCauKhamBenhs ?? (_yeuCauKhamBenhs = new List<YeuCauKhamBenh>());
        //    protected set => _yeuCauKhamBenhs = value;
        //}

        private ICollection<LichPhanCongNgoaiTru> _lichPhanCongNgoaiTrus;

        public virtual ICollection<LichPhanCongNgoaiTru> LichPhanCongNgoaiTrus
        {
            get => _lichPhanCongNgoaiTrus ?? (_lichPhanCongNgoaiTrus = new List<LichPhanCongNgoaiTru>());
            protected set => _lichPhanCongNgoaiTrus = value;
        }

        private ICollection<NhapKhoDuocPham> _nhapKhoDuocPhams;
        public virtual ICollection<NhapKhoDuocPham> NhapKhoDuocPhams
        {
            get => _nhapKhoDuocPhams ?? (_nhapKhoDuocPhams = new List<NhapKhoDuocPham>());
            protected set => _nhapKhoDuocPhams = value;
        }

        private ICollection<XuatKhoDuocPham> _xuatKhoDuocPhamNguoiNhans;
        public virtual ICollection<XuatKhoDuocPham> XuatKhoDuocPhamNguoiNhans
        {
            get => _xuatKhoDuocPhamNguoiNhans ?? (_xuatKhoDuocPhamNguoiNhans = new List<XuatKhoDuocPham>());
            protected set => _xuatKhoDuocPhamNguoiNhans = value;
        }

        private ICollection<XuatKhoDuocPham> _xuatKhoDuocPhamNguoiXuats;
        public virtual ICollection<XuatKhoDuocPham> XuatKhoDuocPhamNguoiXuats
        {
            get => _xuatKhoDuocPhamNguoiXuats ?? (_xuatKhoDuocPhamNguoiXuats = new List<XuatKhoDuocPham>());
            protected set => _xuatKhoDuocPhamNguoiXuats = value;
        }

        #region Update 12/2/2020

        //private ICollection<YeuCauTiepNhan> _yeuCauTiepNhanBacSiKetLuans;
        //public virtual ICollection<YeuCauTiepNhan> YeuCauTiepNhanBacSiKetLuans
        //{
        //    get => _yeuCauTiepNhanBacSiKetLuans ?? (_yeuCauTiepNhanBacSiKetLuans = new List<YeuCauTiepNhan>());
        //    protected set => _yeuCauTiepNhanBacSiKetLuans = value;
        //}

        //private ICollection<YeuCauTiepNhan> _yeuCauTiepNhanBacSiKhams;
        //public virtual ICollection<YeuCauTiepNhan> YeuCauTiepNhanBacSiKhams
        //{
        //    get => _yeuCauTiepNhanBacSiKhams ?? (_yeuCauTiepNhanBacSiKhams = new List<YeuCauTiepNhan>());
        //    protected set => _yeuCauTiepNhanBacSiKhams = value;
        //}

        //private ICollection<YeuCauTiepNhan> _yeuCauTiepNhanNhanVienThanhToans;
        //public virtual ICollection<YeuCauTiepNhan> YeuCauTiepNhanNhanVienThanhToans
        //{
        //    get => _yeuCauTiepNhanNhanVienThanhToans ?? (_yeuCauTiepNhanNhanVienThanhToans = new List<YeuCauTiepNhan>());
        //    protected set => _yeuCauTiepNhanNhanVienThanhToans = value;
        //}

        private ICollection<YeuCauTiepNhan> _yeuCauTiepNhanNhanVienTiepNhans;
        public virtual ICollection<YeuCauTiepNhan> YeuCauTiepNhanVienTiepNhans
        {
            get => _yeuCauTiepNhanNhanVienTiepNhans ?? (_yeuCauTiepNhanNhanVienTiepNhans = new List<YeuCauTiepNhan>());
            protected set => _yeuCauTiepNhanNhanVienTiepNhans = value;
        }

        #endregion Update 12/2/2020

        #region Update 15/2/2020
        //private ICollection<YeuCauTiepNhanDichVuKhamBenh> _yeuCauTiepNhanDichVuKhamBenhBacSiDangKys { get; set; }
        //public virtual ICollection<YeuCauTiepNhanDichVuKhamBenh> YeuCauTiepNhanDichVuKhamBenhBacSiDangKys
        //{
        //    get => _yeuCauTiepNhanDichVuKhamBenhBacSiDangKys ?? (_yeuCauTiepNhanDichVuKhamBenhBacSiDangKys = new List<YeuCauTiepNhanDichVuKhamBenh>());
        //    protected set => _yeuCauTiepNhanDichVuKhamBenhBacSiDangKys = value;
        //}

        //private ICollection<YeuCauTiepNhanDichVuKhamBenh> _yeuCauTiepNhanDichVuKhamBenhBacSiThucHiens { get; set; }
        //public virtual ICollection<YeuCauTiepNhanDichVuKhamBenh> YeuCauTiepNhanDichVuKhamBenhBacSiThucHiens
        //{
        //    get => _yeuCauTiepNhanDichVuKhamBenhBacSiThucHiens ?? (_yeuCauTiepNhanDichVuKhamBenhBacSiThucHiens = new List<YeuCauTiepNhanDichVuKhamBenh>());
        //    protected set => _yeuCauTiepNhanDichVuKhamBenhBacSiThucHiens = value;
        //}

        //private ICollection<YeuCauTiepNhanDichVuKhamBenh> _yeuCauTiepNhanDichVuKhamBenhNhanVienThanhToans { get; set; }
        //public virtual ICollection<YeuCauTiepNhanDichVuKhamBenh> YeuCauTiepNhanDichVuKhamBenhNhanVienThanhToans
        //{
        //    get => _yeuCauTiepNhanDichVuKhamBenhNhanVienThanhToans ?? (_yeuCauTiepNhanDichVuKhamBenhNhanVienThanhToans = new List<YeuCauTiepNhanDichVuKhamBenh>());
        //    protected set => _yeuCauTiepNhanDichVuKhamBenhNhanVienThanhToans = value;
        //}


        //private ICollection<YeuCauTiepNhanDichVuKyThuat> _yeuCauTiepNhanDichVuKyThuatNhanVienKetLuans { get; set; }
        //public virtual ICollection<YeuCauTiepNhanDichVuKyThuat> YeuCauTiepNhanDichVuKyThuatNhanVienKetLuans
        //{
        //    get => _yeuCauTiepNhanDichVuKyThuatNhanVienKetLuans ?? (_yeuCauTiepNhanDichVuKyThuatNhanVienKetLuans = new List<YeuCauTiepNhanDichVuKyThuat>());
        //    protected set => _yeuCauTiepNhanDichVuKyThuatNhanVienKetLuans = value;
        //}

        //private ICollection<YeuCauTiepNhanDichVuKyThuat> _yeuCauTiepNhanDichVuKyThuatNhanVienThanhToans { get; set; }
        //public virtual ICollection<YeuCauTiepNhanDichVuKyThuat> YeuCauTiepNhanDichVuKyThuatNhanVienThanhToans
        //{
        //    get => _yeuCauTiepNhanDichVuKyThuatNhanVienThanhToans ?? (_yeuCauTiepNhanDichVuKyThuatNhanVienThanhToans = new List<YeuCauTiepNhanDichVuKyThuat>());
        //    protected set => _yeuCauTiepNhanDichVuKyThuatNhanVienThanhToans = value;
        //}

        //private ICollection<YeuCauTiepNhanDichVuKyThuat> _yeuCauTiepNhanDichVuKyThuatNhanVienThucHiens { get; set; }
        //public virtual ICollection<YeuCauTiepNhanDichVuKyThuat> YeuCauTiepNhanDichVuKyThuatNhanVienThucHiens
        //{
        //    get => _yeuCauTiepNhanDichVuKyThuatNhanVienThucHiens ?? (_yeuCauTiepNhanDichVuKyThuatNhanVienThucHiens = new List<YeuCauTiepNhanDichVuKyThuat>());
        //    protected set => _yeuCauTiepNhanDichVuKyThuatNhanVienThucHiens = value;
        //}
        #endregion Update 15/2/2020

        #region Update 17/2/2020
        private ICollection<KetQuaSinhHieu> _ketQuaSinhHieus;
        public virtual ICollection<KetQuaSinhHieu> KetQuaSinhHieus
        {
            get => _ketQuaSinhHieus ?? (_ketQuaSinhHieus = new List<KetQuaSinhHieu>());
            protected set => _ketQuaSinhHieus = value;
        }

        private ICollection<Camino.Core.Domain.Entities.DichVukyThuatBenhVienMauKetQua.DichVukyThuatBenhVienMauKetQua> _dichVukyThuatBenhVienMauKetQuas;
        public virtual ICollection<Camino.Core.Domain.Entities.DichVukyThuatBenhVienMauKetQua.DichVukyThuatBenhVienMauKetQua> DichVukyThuatBenhVienMauKetQuas
        {
            get => _dichVukyThuatBenhVienMauKetQuas ?? (_dichVukyThuatBenhVienMauKetQuas = new List<Camino.Core.Domain.Entities.DichVukyThuatBenhVienMauKetQua.DichVukyThuatBenhVienMauKetQua>());
            protected set => _dichVukyThuatBenhVienMauKetQuas = value;
        }

        //update goi dv 10/21
        //private ICollection<GoiDichVu> _goiDichVus;
        //public virtual ICollection<GoiDichVu> GoiDichVus
        //{
        //    get => _goiDichVus ?? (_goiDichVus = new List<GoiDichVu>());
        //    protected set => _goiDichVus = value;
        //}

        private ICollection<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuatNhanVienChiDinhs;
        public virtual ICollection<YeuCauDichVuKyThuat> YeuCauDichVuKyThuatNhanVienChiDinhs
        {
            get => _yeuCauDichVuKyThuatNhanVienChiDinhs ?? (_yeuCauDichVuKyThuatNhanVienChiDinhs = new List<YeuCauDichVuKyThuat>());
            protected set => _yeuCauDichVuKyThuatNhanVienChiDinhs = value;
        }

        private ICollection<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuatNhanVienDuyetBaoHiems;
        public virtual ICollection<YeuCauDichVuKyThuat> YeuCauDichVuKyThuatNhanVienDuyetBaoHiems
        {
            get => _yeuCauDichVuKyThuatNhanVienDuyetBaoHiems ?? (_yeuCauDichVuKyThuatNhanVienDuyetBaoHiems = new List<YeuCauDichVuKyThuat>());
            protected set => _yeuCauDichVuKyThuatNhanVienDuyetBaoHiems = value;
        }

        private ICollection<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuatNhanVienKetLuans;
        public virtual ICollection<YeuCauDichVuKyThuat> YeuCauDichVuKyThuatNhanVienKetLuans
        {
            get => _yeuCauDichVuKyThuatNhanVienKetLuans ?? (_yeuCauDichVuKyThuatNhanVienKetLuans = new List<YeuCauDichVuKyThuat>());
            protected set => _yeuCauDichVuKyThuatNhanVienKetLuans = value;
        }

        //private ICollection<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuatNhanVienThanhToans;
        //public virtual ICollection<YeuCauDichVuKyThuat> YeuCauDichVuKyThuatNhanVienThanhToans
        //{
        //    get => _yeuCauDichVuKyThuatNhanVienThanhToans ?? (_yeuCauDichVuKyThuatNhanVienThanhToans = new List<YeuCauDichVuKyThuat>());
        //    protected set => _yeuCauDichVuKyThuatNhanVienThanhToans = value;
        //}

        private ICollection<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuatNhanVienThucHiens;
        public virtual ICollection<YeuCauDichVuKyThuat> YeuCauDichVuKyThuatNhanVienThucHiens
        {
            get => _yeuCauDichVuKyThuatNhanVienThucHiens ?? (_yeuCauDichVuKyThuatNhanVienThucHiens = new List<YeuCauDichVuKyThuat>());
            protected set => _yeuCauDichVuKyThuatNhanVienThucHiens = value;
        }



        private ICollection<YeuCauDuocPhamBenhVien> _yeuCauDuocPhamBenhVienNhanVienChiDinhs;
        public virtual ICollection<YeuCauDuocPhamBenhVien> YeuCauDuocPhamBenhVienNhanVienChiDinhs
        {
            get => _yeuCauDuocPhamBenhVienNhanVienChiDinhs ?? (_yeuCauDuocPhamBenhVienNhanVienChiDinhs = new List<YeuCauDuocPhamBenhVien>());
            protected set => _yeuCauDuocPhamBenhVienNhanVienChiDinhs = value;
        }

        private ICollection<YeuCauDuocPhamBenhVien> _yeuCauDuocPhamBenhVienNhanVienCapThuocs;
        public virtual ICollection<YeuCauDuocPhamBenhVien> YeuCauDuocPhamBenhVienNhanVienCapThuocs
        {
            get => _yeuCauDuocPhamBenhVienNhanVienCapThuocs ?? (_yeuCauDuocPhamBenhVienNhanVienCapThuocs = new List<YeuCauDuocPhamBenhVien>());
            protected set => _yeuCauDuocPhamBenhVienNhanVienCapThuocs = value;
        }

        private ICollection<YeuCauDuocPhamBenhVien> _yeuCauDuocPhamBenhVienNhanVienDuyetBaoHiems;
        public virtual ICollection<YeuCauDuocPhamBenhVien> YeuCauDuocPhamBenhVienNhanVienDuyetBaoHiems
        {
            get => _yeuCauDuocPhamBenhVienNhanVienDuyetBaoHiems ?? (_yeuCauDuocPhamBenhVienNhanVienDuyetBaoHiems = new List<YeuCauDuocPhamBenhVien>());
            protected set => _yeuCauDuocPhamBenhVienNhanVienDuyetBaoHiems = value;
        }


        //private ICollection<YeuCauDuocPhamBenhVien> _yeuCauDuocPhamBenhVienNhanVienThanhToans;
        //public virtual ICollection<YeuCauDuocPhamBenhVien> YeuCauDuocPhamBenhVienNhanVienThanhToans
        //{
        //    get => _yeuCauDuocPhamBenhVienNhanVienThanhToans ?? (_yeuCauDuocPhamBenhVienNhanVienThanhToans = new List<YeuCauDuocPhamBenhVien>());
        //    protected set => _yeuCauDuocPhamBenhVienNhanVienThanhToans = value;
        //}



        private ICollection<YeuCauKhamBenh> _yeuCauKhamBenhBacSiDangKys;
        public virtual ICollection<YeuCauKhamBenh> YeuCauKhamBenhBacSiDangKys
        {
            get => _yeuCauKhamBenhBacSiDangKys ?? (_yeuCauKhamBenhBacSiDangKys = new List<YeuCauKhamBenh>());
            protected set => _yeuCauKhamBenhBacSiDangKys = value;
        }

        private ICollection<YeuCauKhamBenh> _yeuCauKhamBenhBacSiKetLuans;
        public virtual ICollection<YeuCauKhamBenh> YeuCauKhamBenhBacSiKetLuans
        {
            get => _yeuCauKhamBenhBacSiKetLuans ?? (_yeuCauKhamBenhBacSiKetLuans = new List<YeuCauKhamBenh>());
            protected set => _yeuCauKhamBenhBacSiKetLuans = value;
        }

        private ICollection<YeuCauKhamBenh> _yeuCauKhamBenhBacSiThucHiens;
        public virtual ICollection<YeuCauKhamBenh> YeuCauKhamBenhBacSiThucHiens
        {
            get => _yeuCauKhamBenhBacSiThucHiens ?? (_yeuCauKhamBenhBacSiThucHiens = new List<YeuCauKhamBenh>());
            protected set => _yeuCauKhamBenhBacSiThucHiens = value;
        }

        private ICollection<YeuCauKhamBenh> _yeuCauKhamBenhNhanVienHoTongs;
        public virtual ICollection<YeuCauKhamBenh> YeuCauKhamBenhNhanVienHoTongs
        {
            get => _yeuCauKhamBenhNhanVienHoTongs ?? (_yeuCauKhamBenhNhanVienHoTongs = new List<YeuCauKhamBenh>());
            protected set => _yeuCauKhamBenhNhanVienHoTongs = value;
        }

        private ICollection<YeuCauKhamBenhDonThuoc> _yeuCauKhamBenhDonThuocBacSiKeDons;
        public virtual ICollection<YeuCauKhamBenhDonThuoc> YeuCauKhamBenhDonThuocBacSiKeDons
        {
            get => _yeuCauKhamBenhDonThuocBacSiKeDons ?? (_yeuCauKhamBenhDonThuocBacSiKeDons = new List<YeuCauKhamBenhDonThuoc>());
            protected set => _yeuCauKhamBenhDonThuocBacSiKeDons = value;
        }


        private ICollection<YeuCauKhamBenh> _yeuCauKhamBenhNhanVienChiDinhs;
        public virtual ICollection<YeuCauKhamBenh> YeuCauKhamBenhNhanVienChiDinhs
        {
            get => _yeuCauKhamBenhNhanVienChiDinhs ?? (_yeuCauKhamBenhNhanVienChiDinhs = new List<YeuCauKhamBenh>());
            protected set => _yeuCauKhamBenhNhanVienChiDinhs = value;
        }

        private ICollection<YeuCauKhamBenh> _yeuCauKhamBenhNhanVienDuyetBaoHiems;
        public virtual ICollection<YeuCauKhamBenh> YeuCauKhamBenhNhanVienDuyetBaoHiems
        {
            get => _yeuCauKhamBenhNhanVienDuyetBaoHiems ?? (_yeuCauKhamBenhNhanVienDuyetBaoHiems = new List<YeuCauKhamBenh>());
            protected set => _yeuCauKhamBenhNhanVienDuyetBaoHiems = value;
        }

        private ICollection<YeuCauKhamBenh> _yeuCauKhamBenhNhanVienHuyThanhToans;
        public virtual ICollection<YeuCauKhamBenh> YeuCauKhamBenhNhanVienHuyThanhToans
        {
            get => _yeuCauKhamBenhNhanVienHuyThanhToans ?? (_yeuCauKhamBenhNhanVienHuyThanhToans = new List<YeuCauKhamBenh>());
            protected set => _yeuCauKhamBenhNhanVienHuyThanhToans = value;
        }

        private ICollection<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuatNhanVienHuyThanhToans;
        public virtual ICollection<YeuCauDichVuKyThuat> YeuCauDichVuKyThuatNhanVienHuyThanhToans
        {
            get => _yeuCauDichVuKyThuatNhanVienHuyThanhToans ?? (_yeuCauDichVuKyThuatNhanVienHuyThanhToans = new List<YeuCauDichVuKyThuat>());
            protected set => _yeuCauDichVuKyThuatNhanVienHuyThanhToans = value;
        }

        private ICollection<YeuCauDuocPhamBenhVien> _yeuCauDuocPhamBenhVienNhanVienHuyThanhToans;
        public virtual ICollection<YeuCauDuocPhamBenhVien> YeuCauDuocPhamBenhVienNhanVienHuyThanhToans
        {
            get => _yeuCauDuocPhamBenhVienNhanVienHuyThanhToans ?? (_yeuCauDuocPhamBenhVienNhanVienHuyThanhToans = new List<YeuCauDuocPhamBenhVien>());
            protected set => _yeuCauDuocPhamBenhVienNhanVienHuyThanhToans = value;
        }

        private ICollection<YeuCauDichVuGiuongBenhVien> _yeuCauDichVuGiuongBenhVienNhanVienHuyThanhToans;
        public virtual ICollection<YeuCauDichVuGiuongBenhVien> YeuCauDichVuGiuongBenhVienNhanVienHuyThanhToans
        {
            get => _yeuCauDichVuGiuongBenhVienNhanVienHuyThanhToans ?? (_yeuCauDichVuGiuongBenhVienNhanVienHuyThanhToans = new List<YeuCauDichVuGiuongBenhVien>());
            protected set => _yeuCauDichVuGiuongBenhVienNhanVienHuyThanhToans = value;
        }


        //private ICollection<YeuCauKhamBenh> _yeuCauKhamBenhNhanVienThanhToans;
        //public virtual ICollection<YeuCauKhamBenh> YeuCauKhamBenhNhanVienThanhToans
        //{
        //    get => _yeuCauKhamBenhNhanVienThanhToans ?? (_yeuCauKhamBenhNhanVienThanhToans = new List<YeuCauKhamBenh>());
        //    protected set => _yeuCauKhamBenhNhanVienThanhToans = value;
        //}


        private ICollection<YeuCauVatTuBenhVien> _yeuCauVatTuBenhVienNoiCapVatTus;
        public virtual ICollection<YeuCauVatTuBenhVien> YeuCauVatTuBenhVienNoiCapVatTus
        {
            get => _yeuCauVatTuBenhVienNoiCapVatTus ?? (_yeuCauVatTuBenhVienNoiCapVatTus = new List<YeuCauVatTuBenhVien>());
            protected set => _yeuCauVatTuBenhVienNoiCapVatTus = value;
        }

        private ICollection<YeuCauVatTuBenhVien> _yeuCauVatTuBenhVienNoiChiDinhs;
        public virtual ICollection<YeuCauVatTuBenhVien> YeuCauVatTuBenhVienNoiChiDinhs
        {
            get => _yeuCauVatTuBenhVienNoiChiDinhs ?? (_yeuCauVatTuBenhVienNoiChiDinhs = new List<YeuCauVatTuBenhVien>());
            protected set => _yeuCauVatTuBenhVienNoiChiDinhs = value;
        }

        private ICollection<YeuCauVatTuBenhVien> _yeuCauVatTuBenhVienNoiThanhToans;
        public virtual ICollection<YeuCauVatTuBenhVien> YeuCauVatTuBenhVienNoiThanhToans
        {
            get => _yeuCauVatTuBenhVienNoiThanhToans ?? (_yeuCauVatTuBenhVienNoiThanhToans = new List<YeuCauVatTuBenhVien>());
            protected set => _yeuCauVatTuBenhVienNoiThanhToans = value;
        }


        private ICollection<YeuCauVatTuBenhVien> _yeuCauVatTuBenhVienNhanVienDuyetBaoHiems;
        public virtual ICollection<YeuCauVatTuBenhVien> YeuCauVatTuBenhVienNhanVienDuyetBaoHiems
        {
            get => _yeuCauVatTuBenhVienNhanVienDuyetBaoHiems ?? (_yeuCauVatTuBenhVienNhanVienDuyetBaoHiems = new List<YeuCauVatTuBenhVien>());
            protected set => _yeuCauVatTuBenhVienNhanVienDuyetBaoHiems = value;
        }

        #endregion


        private ICollection<HoatDongNhanVien> _hoatDongNhanViens { get; set; }
        public virtual ICollection<HoatDongNhanVien> HoatDongNhanViens
        {
            get => _hoatDongNhanViens ?? (_hoatDongNhanViens = new List<HoatDongNhanVien>());
            protected set => _hoatDongNhanViens = value;
        }

        private ICollection<LichSuHoatDongNhanVien> _lichSuHoatDongNhanViens { get; set; }
        public virtual ICollection<LichSuHoatDongNhanVien> LichSuHoatDongNhanViens
        {
            get => _lichSuHoatDongNhanViens ?? (_lichSuHoatDongNhanViens = new List<LichSuHoatDongNhanVien>());
            protected set => _lichSuHoatDongNhanViens = value;
        }

        private ICollection<YeuCauDichVuGiuongBenhVien> _yeuCauDichVuGiuongBenhVienNhanVienChiDinhs;
        public virtual ICollection<YeuCauDichVuGiuongBenhVien> YeuCauDichVuGiuongBenhVienNhanVienChiDinhs
        {
            get => _yeuCauDichVuGiuongBenhVienNhanVienChiDinhs ?? (_yeuCauDichVuGiuongBenhVienNhanVienChiDinhs = new List<YeuCauDichVuGiuongBenhVien>());
            protected set => _yeuCauDichVuGiuongBenhVienNhanVienChiDinhs = value;

        }
        //private ICollection<YeuCauDichVuGiuongBenhVien> _yeuCauDichVuGiuongBenhVienNhanVienThanhToans;
        //public virtual ICollection<YeuCauDichVuGiuongBenhVien> YeuCauDichVuGiuongBenhVienNhanVienThanhToans
        //{
        //    get => _yeuCauDichVuGiuongBenhVienNhanVienThanhToans ?? (_yeuCauDichVuGiuongBenhVienNhanVienThanhToans = new List<YeuCauDichVuGiuongBenhVien>());
        //    protected set => _yeuCauDichVuGiuongBenhVienNhanVienThanhToans = value;

        //}
       
        private ICollection<YeuCauDichVuGiuongBenhVien> _yeuCauDichVuGiuongBenhVienNhanVienDuyetBaoHiems;
        public virtual ICollection<YeuCauDichVuGiuongBenhVien> YeuCauDichVuGiuongBenhVienNhanVienDuyetBaoHiems
        {
            get => _yeuCauDichVuGiuongBenhVienNhanVienDuyetBaoHiems ?? (_yeuCauDichVuGiuongBenhVienNhanVienDuyetBaoHiems = new List<YeuCauDichVuGiuongBenhVien>());
            protected set => _yeuCauDichVuGiuongBenhVienNhanVienDuyetBaoHiems = value;

        }

        private ICollection<YeuCauGoiDichVu> _yeuCauGoiDichVuNhanVienChiDinhs;
        public virtual ICollection<YeuCauGoiDichVu> YeuCauGoiDichVuNhanVienChiDinhs
        {
            get => _yeuCauGoiDichVuNhanVienChiDinhs ?? (_yeuCauGoiDichVuNhanVienChiDinhs = new List<YeuCauGoiDichVu>());
            protected set => _yeuCauGoiDichVuNhanVienChiDinhs = value;

        }
        private ICollection<YeuCauGoiDichVu> _yeuCauGoiDichVuNhanVienTuVans;
        public virtual ICollection<YeuCauGoiDichVu> YeuCauGoiDichVuNhanVienTuVans
        {
            get => _yeuCauGoiDichVuNhanVienTuVans ?? (_yeuCauGoiDichVuNhanVienTuVans = new List<YeuCauGoiDichVu>());
            protected set => _yeuCauGoiDichVuNhanVienTuVans = value;

        }
        private ICollection<YeuCauGoiDichVu> _yeuCauGoiDichVuNhanVienQuyetToans;
        public virtual ICollection<YeuCauGoiDichVu> YeuCauGoiDichVuNhanVienQuyetToans
        {
            get => _yeuCauGoiDichVuNhanVienQuyetToans ?? (_yeuCauGoiDichVuNhanVienQuyetToans = new List<YeuCauGoiDichVu>());
            protected set => _yeuCauGoiDichVuNhanVienQuyetToans = value;

        }
        private ICollection<DuTruDuocPham> _duTruDuocPhams;
        public virtual ICollection<DuTruDuocPham> DuTruDuocPhams
        {
            get => _duTruDuocPhams ?? (_duTruDuocPhams = new List<DuTruDuocPham>());
            protected set => _duTruDuocPhams = value;
        }

        private ICollection<DonThuocThanhToan> _donThuocThanhToans;
        public virtual ICollection<DonThuocThanhToan> DonThuocThanhToans
        {
            get => _donThuocThanhToans ?? (_donThuocThanhToans = new List<DonThuocThanhToan>());
            protected set => _donThuocThanhToans = value;
        }

        private ICollection<DonThuocThanhToan> _donThuocThanhToanBiHuys;
        public virtual ICollection<DonThuocThanhToan> DonThuocThanhToanBiHuys
        {
            get => _donThuocThanhToanBiHuys ?? (_donThuocThanhToanBiHuys = new List<DonThuocThanhToan>());
            protected set => _donThuocThanhToanBiHuys = value;
        }
        private ICollection<DonThuocThanhToanChiTiet> _donThuocThanhToanChiTiets;
        public virtual ICollection<DonThuocThanhToanChiTiet> DonThuocThanhToanChiTiets
        {
            get => _donThuocThanhToanChiTiets ?? (_donThuocThanhToanChiTiets = new List<DonThuocThanhToanChiTiet>());
            protected set => _donThuocThanhToanChiTiets = value;
        }
        private ICollection<DuyetBaoHiem> _duyetBaoHiems;
        public virtual ICollection<DuyetBaoHiem> DuyetBaoHiems
        {
            get => _duyetBaoHiems ?? (_duyetBaoHiems = new List<DuyetBaoHiem>());
            protected set => _duyetBaoHiems = value;
        }
        private ICollection<TaiKhoanBenhNhanThu> _taiKhoanBenhNhanThus;
        public virtual ICollection<TaiKhoanBenhNhanThu> TaiKhoanBenhNhanThus
        {
            get => _taiKhoanBenhNhanThus ?? (_taiKhoanBenhNhanThus = new List<TaiKhoanBenhNhanThu>());
            protected set => _taiKhoanBenhNhanThus = value;
        }
        private ICollection<TaiKhoanBenhNhanThu> _taiKhoanBenhNhanThuNhanVienHuys;
        public virtual ICollection<TaiKhoanBenhNhanThu> TaiKhoanBenhNhanThuNhanVienHuys
        {
            get => _taiKhoanBenhNhanThuNhanVienHuys ?? (_taiKhoanBenhNhanThuNhanVienHuys = new List<TaiKhoanBenhNhanThu>());
            protected set => _taiKhoanBenhNhanThuNhanVienHuys = value;
        }
        private ICollection<TaiKhoanBenhNhanThu> _taiKhoanBenhNhanThuNhanVienThuHois;
        public virtual ICollection<TaiKhoanBenhNhanThu> TaiKhoanBenhNhanThuNhanVienThuHois
        {
            get => _taiKhoanBenhNhanThuNhanVienThuHois ?? (_taiKhoanBenhNhanThuNhanVienThuHois = new List<TaiKhoanBenhNhanThu>());
            protected set => _taiKhoanBenhNhanThuNhanVienThuHois = value;
        }
        private ICollection<TaiKhoanBenhNhanChi> _taiKhoanBenhNhanChis;
        public virtual ICollection<TaiKhoanBenhNhanChi> TaiKhoanBenhNhanChis
        {
            get => _taiKhoanBenhNhanChis ?? (_taiKhoanBenhNhanChis = new List<TaiKhoanBenhNhanChi>());
            protected set => _taiKhoanBenhNhanChis = value;
        }

        private ICollection<TaiKhoanBenhNhanChi> _taiKhoanBenhNhanChiHuys;
        public virtual ICollection<TaiKhoanBenhNhanChi> TaiKhoanBenhNhanChiHuys
        {
            get => _taiKhoanBenhNhanChiHuys ?? (_taiKhoanBenhNhanChiHuys = new List<TaiKhoanBenhNhanChi>());
            protected set => _taiKhoanBenhNhanChiHuys = value;
        }

        private ICollection<TaiKhoanBenhNhanChi> _taiKhoanBenhNhanChiThuHois;
        public virtual ICollection<TaiKhoanBenhNhanChi> TaiKhoanBenhNhanChiThuHois
        {
            get => _taiKhoanBenhNhanChiThuHois ?? (_taiKhoanBenhNhanChiThuHois = new List<TaiKhoanBenhNhanChi>());
            protected set => _taiKhoanBenhNhanChiThuHois = value;
        }

        private ICollection<ToaThuocMau> _toaThuocMaus;
        public virtual ICollection<ToaThuocMau> ToaThuocMaus
        {
            get => _toaThuocMaus ?? (_toaThuocMaus = new List<ToaThuocMau>());
            protected set => _toaThuocMaus = value;
        }

        #region Update 19/05/2020
        private ICollection<NguoiGioiThieu> _nguoiGioiThieus;

        public virtual ICollection<NguoiGioiThieu> NguoiGioiThieus
        {
            get => _nguoiGioiThieus ?? (_nguoiGioiThieus = new List<NguoiGioiThieu>());
            protected set => _nguoiGioiThieus = value;
        }
        #endregion

        private ICollection<YeuCauTiepNhan> _yeuCauTiepNhanNhanVienDuyetMienGiamThems;
        public virtual ICollection<YeuCauTiepNhan> YeuCauTiepNhanNhanVienDuyetMienGiamThems
        {
            get => _yeuCauTiepNhanNhanVienDuyetMienGiamThems ?? (_yeuCauTiepNhanNhanVienDuyetMienGiamThems = new List<YeuCauTiepNhan>());
            protected set => _yeuCauTiepNhanNhanVienDuyetMienGiamThems = value;
        }

        #region Update 26/05/2020
        private ICollection<NoiGioiThieu.NoiGioiThieu> _noiGioiThieus;
        public virtual ICollection<NoiGioiThieu.NoiGioiThieu> NoiGioiThieus
        {
            get => _noiGioiThieus ?? (_noiGioiThieus = new List<NoiGioiThieu.NoiGioiThieu>());
            protected set => _noiGioiThieus = value;
        }
        #endregion

        private ICollection<TaiKhoanBenhNhanHuyDichVu> _taiKhoanBenhNhanHuyDichVus;
        public virtual ICollection<TaiKhoanBenhNhanHuyDichVu> TaiKhoanBenhNhanHuyDichVus
        {
            get => _taiKhoanBenhNhanHuyDichVus ?? (_taiKhoanBenhNhanHuyDichVus = new List<TaiKhoanBenhNhanHuyDichVu>());
            protected set => _taiKhoanBenhNhanHuyDichVus = value;
        }

        // update 21/8
        private ICollection<KetQuaNhomXetNghiem> _KetQuaNhomXetNghiems;
        public virtual ICollection<KetQuaNhomXetNghiem> KetQuaNhomXetNghiems
        {
            get => _KetQuaNhomXetNghiems ?? (_KetQuaNhomXetNghiems = new List<KetQuaNhomXetNghiem>());
            protected set => _KetQuaNhomXetNghiems = value;
        }

        #region PhauThuatThuThuat
        private ICollection<YeuCauDichVuKyThuatTuongTrinhPTTT> _yeuCauDichVuKyThuatTuongTrinhPTTTs;
        public virtual ICollection<YeuCauDichVuKyThuatTuongTrinhPTTT> YeuCauDichVuKyThuatTuongTrinhPTTTs
        {
            get => _yeuCauDichVuKyThuatTuongTrinhPTTTs ?? (_yeuCauDichVuKyThuatTuongTrinhPTTTs = new List<YeuCauDichVuKyThuatTuongTrinhPTTT>());
            protected set => _yeuCauDichVuKyThuatTuongTrinhPTTTs = value;
        }

        private ICollection<PhauThuatThuThuatEkipBacSi> _phauThuatThuThuatEkipBacSis;
        public virtual ICollection<PhauThuatThuThuatEkipBacSi> PhauThuatThuThuatEkipBacSis
        {
            get => _phauThuatThuThuatEkipBacSis ?? (_phauThuatThuThuatEkipBacSis = new List<PhauThuatThuThuatEkipBacSi>());
            protected set => _phauThuatThuThuatEkipBacSis = value;
        }

        private ICollection<PhauThuatThuThuatEkipDieuDuong> _phauThuatThuThuatEkipDieuDuongs;
        public virtual ICollection<PhauThuatThuThuatEkipDieuDuong> PhauThuatThuThuatEkipDieuDuongs
        {
            get => _phauThuatThuThuatEkipDieuDuongs ?? (_phauThuatThuThuatEkipDieuDuongs = new List<PhauThuatThuThuatEkipDieuDuong>());
            protected set => _phauThuatThuThuatEkipDieuDuongs = value;
        }

        private ICollection<TheoDoiSauPhauThuatThuThuat> _theoDoiSauPhauThuatThuThuatBacSiPhuTrachs;
        public virtual ICollection<TheoDoiSauPhauThuatThuThuat> TheoDoiSauPhauThuatThuThuatBacSiPhuTrachs
        {
            get => _theoDoiSauPhauThuatThuThuatBacSiPhuTrachs ?? (_theoDoiSauPhauThuatThuThuatBacSiPhuTrachs = new List<TheoDoiSauPhauThuatThuThuat>());
            protected set => _theoDoiSauPhauThuatThuThuatBacSiPhuTrachs = value;
        }
        
        private ICollection<TheoDoiSauPhauThuatThuThuat> _theoDoiSauPhauThuatThuThuatDieuDuongPhuTrachs;
        public virtual ICollection<TheoDoiSauPhauThuatThuThuat> TheoDoiSauPhauThuatThuThuatDieuDuongPhuTrachs
        {
            get => _theoDoiSauPhauThuatThuThuatDieuDuongPhuTrachs ?? (_theoDoiSauPhauThuatThuThuatDieuDuongPhuTrachs = new List<TheoDoiSauPhauThuatThuThuat>());
            protected set => _theoDoiSauPhauThuatThuThuatDieuDuongPhuTrachs = value;
        }

        private ICollection<KhamTheoDoi> _khamTheoDois;
        public virtual ICollection<KhamTheoDoi> KhamTheoDois
        {
            get => _khamTheoDois ?? (_khamTheoDois = new List<KhamTheoDoi>());
            protected set => _khamTheoDois = value;
        }
        #endregion
        private ICollection<KhoNhanVienQuanLy> _khoNhanVienQuanLys;
        public virtual ICollection<KhoNhanVienQuanLy> KhoNhanVienQuanLys
        {
            get => _khoNhanVienQuanLys ?? (_khoNhanVienQuanLys = new List<KhoNhanVienQuanLy>());
            protected set => _khoNhanVienQuanLys = value;
        }

        private ICollection<YeuCauNhapKhoVatTu> _yeuCauNhapKhoVatTuNguoiGiaos;
        public virtual ICollection<YeuCauNhapKhoVatTu> YeuCauNhapKhoVatTuNguoiGiaos
        {
            get => _yeuCauNhapKhoVatTuNguoiGiaos ?? (_yeuCauNhapKhoVatTuNguoiGiaos = new List<YeuCauNhapKhoVatTu>());
            protected set => _yeuCauNhapKhoVatTuNguoiGiaos = value;
        }

        private ICollection<YeuCauNhapKhoVatTu> _yeuCauNhapKhoVatTuNguoiNhaps;
        public virtual ICollection<YeuCauNhapKhoVatTu> YeuCauNhapKhoVatTuNguoiNhaps
        {
            get => _yeuCauNhapKhoVatTuNguoiNhaps ?? (_yeuCauNhapKhoVatTuNguoiNhaps = new List<YeuCauNhapKhoVatTu>());
            protected set => _yeuCauNhapKhoVatTuNguoiNhaps = value;
        }

        private ICollection<YeuCauNhapKhoVatTu> _yeuCauNhapKhoVatTuNhanVienDuyets;
        public virtual ICollection<YeuCauNhapKhoVatTu> YeuCauNhapKhoVatTuNhanVienDuyets
        {
            get => _yeuCauNhapKhoVatTuNhanVienDuyets ?? (_yeuCauNhapKhoVatTuNhanVienDuyets = new List<YeuCauNhapKhoVatTu>());
            protected set => _yeuCauNhapKhoVatTuNhanVienDuyets = value;
        }

        private ICollection<YeuCauLinhVatTu> _yeuCauLinhVatTuNhanVienYeuCaus;
        public virtual ICollection<YeuCauLinhVatTu> YeuCauLinhVatTuNhanVienYeuCaus
        {
            get => _yeuCauLinhVatTuNhanVienYeuCaus ?? (_yeuCauLinhVatTuNhanVienYeuCaus = new List<YeuCauLinhVatTu>());
            protected set => _yeuCauLinhVatTuNhanVienYeuCaus = value;
        }

        private ICollection<YeuCauLinhVatTu> _yeuCauLinhVatTuNhanVienDuyets;
        public virtual ICollection<YeuCauLinhVatTu> YeuCauLinhVatTuNhanVienDuyets
        {
            get => _yeuCauLinhVatTuNhanVienDuyets ?? (_yeuCauLinhVatTuNhanVienDuyets = new List<YeuCauLinhVatTu>());
            protected set => _yeuCauLinhVatTuNhanVienDuyets = value;
        }

        private ICollection<XuatKhoVatTu> _xuatKhoVatTusNguoiNhan;
        public virtual ICollection<XuatKhoVatTu> XuatKhoVatTuNguoiNhans
        {
            get => _xuatKhoVatTusNguoiNhan ?? (_xuatKhoVatTusNguoiNhan = new List<XuatKhoVatTu>());
            protected set => _xuatKhoVatTusNguoiNhan = value;
        }

        private ICollection<XuatKhoVatTu> _xuatKhoVatTuNguoiXuats;
        public virtual ICollection<XuatKhoVatTu> XuatKhoVatTuNguoiXuats
        {
            get => _xuatKhoVatTuNguoiXuats ?? (_xuatKhoVatTuNguoiXuats = new List<XuatKhoVatTu>());
            protected set => _xuatKhoVatTuNguoiXuats = value;
        }

        private ICollection<NhapKhoVatTu> _nhapKhoVatTuNguoiGiaos;

        public virtual ICollection<NhapKhoVatTu> NhapKhoVatTuNguoiGiaos
        {
            get => _nhapKhoVatTuNguoiGiaos ?? (_nhapKhoVatTuNguoiGiaos = new List<NhapKhoVatTu>());
            protected set => _nhapKhoVatTuNguoiGiaos = value;
        }

        private ICollection<NhapKhoVatTu> _nhapKhoVatTuNguoiNhaps;

        public virtual ICollection<NhapKhoVatTu> NhapKhoVatTuNguoiNhaps
        {
            get => _nhapKhoVatTuNguoiNhaps ?? (_nhapKhoVatTuNguoiNhaps = new List<NhapKhoVatTu>());
            protected set => _nhapKhoVatTuNguoiNhaps = value;
        }

        private ICollection<YeuCauLinhDuocPham> _yeuCauLinhDuocPhamNhanVienYeuCaus;

        public virtual ICollection<YeuCauLinhDuocPham> YeuCauLinhDuocPhamNhanVienYeuCaus
        {
            get => _yeuCauLinhDuocPhamNhanVienYeuCaus ?? (_yeuCauLinhDuocPhamNhanVienYeuCaus = new List<YeuCauLinhDuocPham>());
            protected set => _yeuCauLinhDuocPhamNhanVienYeuCaus = value;
        }

        private ICollection<YeuCauLinhDuocPham> _yeuCauLinhDuocPhamNhanVienDuyets;

        public virtual ICollection<YeuCauLinhDuocPham> YeuCauLinhDuocPhamNhanVienDuyets
        {
            get => _yeuCauLinhDuocPhamNhanVienDuyets ?? (_yeuCauLinhDuocPhamNhanVienDuyets = new List<YeuCauLinhDuocPham>());
            protected set => _yeuCauLinhDuocPhamNhanVienDuyets = value;
        }

        private ICollection<YeuCauNhapKhoDuocPham> _yeuCauNhapKhoDuocPhamNguoiGiaos;

        public virtual ICollection<YeuCauNhapKhoDuocPham> YeuCauNhapKhoDuocPhamNguoiGiaos
        {
            get => _yeuCauNhapKhoDuocPhamNguoiGiaos ?? (_yeuCauNhapKhoDuocPhamNguoiGiaos = new List<YeuCauNhapKhoDuocPham>());
            protected set => _yeuCauNhapKhoDuocPhamNguoiGiaos = value;
        }

        private ICollection<YeuCauNhapKhoDuocPham> _yeuCauNhapKhoDuocPhamNguoiNhaps;

        public virtual ICollection<YeuCauNhapKhoDuocPham> YeuCauNhapKhoDuocPhamNguoiNhaps
        {
            get => _yeuCauNhapKhoDuocPhamNguoiNhaps ?? (_yeuCauNhapKhoDuocPhamNguoiNhaps = new List<YeuCauNhapKhoDuocPham>());
            protected set => _yeuCauNhapKhoDuocPhamNguoiNhaps = value;
        }

        private ICollection<YeuCauNhapKhoDuocPham> _yeuCauNhapKhoDuocPhamNhanVienDuyets;

        public virtual ICollection<YeuCauNhapKhoDuocPham> YeuCauNhapKhoDuocPhamNhanVienDuyets
        {
            get => _yeuCauNhapKhoDuocPhamNhanVienDuyets ?? (_yeuCauNhapKhoDuocPhamNhanVienDuyets = new List<YeuCauNhapKhoDuocPham>());
            protected set => _yeuCauNhapKhoDuocPhamNhanVienDuyets = value;
        }

        private ICollection<YeuCauTraDuocPham> _yeuCauTraDuocPhamNhanVienYeuCaus;

        public virtual ICollection<YeuCauTraDuocPham> YeuCauTraDuocPhamNhanVienYeuCaus
        {
            get => _yeuCauTraDuocPhamNhanVienYeuCaus ?? (_yeuCauTraDuocPhamNhanVienYeuCaus = new List<YeuCauTraDuocPham>());
            protected set => _yeuCauTraDuocPhamNhanVienYeuCaus = value;
        }

        private ICollection<YeuCauTraDuocPham> _yeuCauTraDuocPhamNhanVienDuyets;

        public virtual ICollection<YeuCauTraDuocPham> YeuCauTraDuocPhamNhanVienDuyets
        {
            get => _yeuCauTraDuocPhamNhanVienDuyets ?? (_yeuCauTraDuocPhamNhanVienDuyets = new List<YeuCauTraDuocPham>());
            protected set => _yeuCauTraDuocPhamNhanVienDuyets = value;
        }

        private ICollection<YeuCauTraVatTu> _yeuCauTraVatTuNhanVienYeuCaus;

        public virtual ICollection<YeuCauTraVatTu> YeuCauTraVatTuNhanVienYeuCaus
        {
            get => _yeuCauTraVatTuNhanVienYeuCaus ?? (_yeuCauTraVatTuNhanVienYeuCaus = new List<YeuCauTraVatTu>());
            protected set => _yeuCauTraVatTuNhanVienYeuCaus = value;
        }

        private ICollection<YeuCauTraVatTu> _yeuCauTraVatTuNhanVienDuyets;

        public virtual ICollection<YeuCauTraVatTu> YeuCauTraVatTuNhanVienDuyets
        {
            get => _yeuCauTraVatTuNhanVienDuyets ?? (_yeuCauTraVatTuNhanVienDuyets = new List<YeuCauTraVatTu>());
            protected set => _yeuCauTraVatTuNhanVienDuyets = value;
        }

        private ICollection<YeuCauKhamBenhDonVTYT> _yeuCauKhamBenhDonVTYTBacSiKeDons;
        public virtual ICollection<YeuCauKhamBenhDonVTYT> YeuCauKhamBenhDonVTYTBacSiKeDons
        {
            get => _yeuCauKhamBenhDonVTYTBacSiKeDons ?? (_yeuCauKhamBenhDonVTYTBacSiKeDons = new List<YeuCauKhamBenhDonVTYT>());
            protected set => _yeuCauKhamBenhDonVTYTBacSiKeDons = value;
        }

        private ICollection<DonVTYTThanhToan> _donVTYTThanhToans;
        public virtual ICollection<DonVTYTThanhToan> DonVTYTThanhToans
        {
            get => _donVTYTThanhToans ?? (_donVTYTThanhToans = new List<DonVTYTThanhToan>());
            protected set => _donVTYTThanhToans = value;
        }

        private ICollection<DonVTYTThanhToan> _donVTYTThanhToanBiHuys;
        public virtual ICollection<DonVTYTThanhToan> DonVTYTThanhToanBiHuys
        {
            get => _donVTYTThanhToanBiHuys ?? (_donVTYTThanhToanBiHuys = new List<DonVTYTThanhToan>());
            protected set => _donVTYTThanhToanBiHuys = value;
        }

        private ICollection<NhapKhoQuaTang> _nhapKhoQuaTangNguoiNhaps;
        public virtual ICollection<NhapKhoQuaTang> NhapKhoQuaTangNguoiNhaps
        {
            get => _nhapKhoQuaTangNguoiNhaps ?? (_nhapKhoQuaTangNguoiNhaps = new List<NhapKhoQuaTang>());
            protected set => _nhapKhoQuaTangNguoiNhaps = value;
        }

        private ICollection<NhapKhoQuaTang> _nhapKhoQuaTangNguoiGiaos;
        public virtual ICollection<NhapKhoQuaTang> NhapKhoQuaTangNguoiGiaos
        {
            get => _nhapKhoQuaTangNguoiGiaos ?? (_nhapKhoQuaTangNguoiGiaos = new List<NhapKhoQuaTang>());
            protected set => _nhapKhoQuaTangNguoiGiaos = value;
        }

        private ICollection<XuatKhoQuaTang> _xuatKhoQuaTangs;
        public virtual ICollection<XuatKhoQuaTang> XuatKhoQuaTangs
        {
            get => _xuatKhoQuaTangs ?? (_xuatKhoQuaTangs = new List<XuatKhoQuaTang>());
            protected set => _xuatKhoQuaTangs = value;
        }

        //update 11/11/2020
        private ICollection<KyDuTruMuaDuocPhamVatTu> _kyDuTruMuaDuocPhamVatTus { get; set; }
        public virtual ICollection<KyDuTruMuaDuocPhamVatTu> KyDuTruMuaDuocPhamVatTus
        {
            get => _kyDuTruMuaDuocPhamVatTus ?? (_kyDuTruMuaDuocPhamVatTus = new List<KyDuTruMuaDuocPhamVatTu>());
            protected set => _kyDuTruMuaDuocPhamVatTus = value;
        }

        //Duoc Pham

        private ICollection<DuTruMuaDuocPham> _duTruMuaDuocPhamNhanVienYeuCaus { get; set; }
        public virtual ICollection<DuTruMuaDuocPham> DuTruMuaDuocPhamNhanVienYeuCaus
        {
            get => _duTruMuaDuocPhamNhanVienYeuCaus ?? (_duTruMuaDuocPhamNhanVienYeuCaus = new List<DuTruMuaDuocPham>());
            protected set => _duTruMuaDuocPhamNhanVienYeuCaus = value;
        }

        private ICollection<DuTruMuaDuocPham> _duTruMuaDuocPhamTruongKhoas { get; set; }
        public virtual ICollection<DuTruMuaDuocPham> DuTruMuaDuocPhamTruongKhoas
        {
            get => _duTruMuaDuocPhamTruongKhoas ?? (_duTruMuaDuocPhamTruongKhoas = new List<DuTruMuaDuocPham>());
            protected set => _duTruMuaDuocPhamTruongKhoas = value;
        }

        private ICollection<DuTruMuaDuocPhamTheoKhoa> _duTruMuaDuocPhamTheoKhoaNhanVienYeuCaus { get; set; }
        public virtual ICollection<DuTruMuaDuocPhamTheoKhoa> DuTruMuaDuocPhamTheoKhoaNhanVienYeuCaus
        {
            get => _duTruMuaDuocPhamTheoKhoaNhanVienYeuCaus ?? (_duTruMuaDuocPhamTheoKhoaNhanVienYeuCaus = new List<DuTruMuaDuocPhamTheoKhoa>());
            protected set => _duTruMuaDuocPhamTheoKhoaNhanVienYeuCaus = value;
        }

        private ICollection<DuTruMuaDuocPhamTheoKhoa> _duTruMuaDuocPhamTheoKhoaNhanVienKhoDuocs { get; set; }
        public virtual ICollection<DuTruMuaDuocPhamTheoKhoa> DuTruMuaDuocPhamTheoKhoaNhanVienKhoDuocs
        {
            get => _duTruMuaDuocPhamTheoKhoaNhanVienKhoDuocs ?? (_duTruMuaDuocPhamTheoKhoaNhanVienKhoDuocs = new List<DuTruMuaDuocPhamTheoKhoa>());
            protected set => _duTruMuaDuocPhamTheoKhoaNhanVienKhoDuocs = value;
        }

        private ICollection<DuTruMuaDuocPhamKhoDuoc> _duTruMuaDuocPhamKhoDuocNhanVienYeuCaus { get; set; }
        public virtual ICollection<DuTruMuaDuocPhamKhoDuoc> DuTruMuaDuocPhamKhoDuocNhanVienYeuCaus
        {
            get => _duTruMuaDuocPhamKhoDuocNhanVienYeuCaus ?? (_duTruMuaDuocPhamKhoDuocNhanVienYeuCaus = new List<DuTruMuaDuocPhamKhoDuoc>());
            protected set => _duTruMuaDuocPhamKhoDuocNhanVienYeuCaus = value;
        }

        private ICollection<DuTruMuaDuocPhamKhoDuoc> _duTruMuaDuocPhamKhoDuocGiamDocs { get; set; }
        public virtual ICollection<DuTruMuaDuocPhamKhoDuoc> DuTruMuaDuocPhamKhoDuocGiamDocs
        {
            get => _duTruMuaDuocPhamKhoDuocGiamDocs ?? (_duTruMuaDuocPhamKhoDuocGiamDocs = new List<DuTruMuaDuocPhamKhoDuoc>());
            protected set => _duTruMuaDuocPhamKhoDuocGiamDocs = value;
        }

     
        private ICollection<DuTruMuaVatTu> _duTruMuaVatTuNhanVienYeuCaus { get; set; }
        public virtual ICollection<DuTruMuaVatTu> DuTruMuaVatTuNhanVienYeuCaus
        {
            get => _duTruMuaVatTuNhanVienYeuCaus ?? (_duTruMuaVatTuNhanVienYeuCaus = new List<DuTruMuaVatTu>());
            protected set => _duTruMuaVatTuNhanVienYeuCaus = value;
        }
        private ICollection<DuTruMuaVatTu> _duTruMuaVatTuTruongKhoas { get; set; }
        public virtual ICollection<DuTruMuaVatTu> DuTruMuaVatTuTruongKhoas
        {
            get => _duTruMuaVatTuTruongKhoas ?? (_duTruMuaVatTuTruongKhoas = new List<DuTruMuaVatTu>());
            protected set => _duTruMuaVatTuTruongKhoas = value;
        }

        private ICollection<DuTruMuaVatTuTheoKhoa> _duTruMuaVatTuTheoKhoaNhanVienYeuCaus { get; set; }
        public virtual ICollection<DuTruMuaVatTuTheoKhoa> DuTruMuaVatTuTheoKhoaNhanVienYeuCaus
        {
            get => _duTruMuaVatTuTheoKhoaNhanVienYeuCaus ?? (_duTruMuaVatTuTheoKhoaNhanVienYeuCaus = new List<DuTruMuaVatTuTheoKhoa>());
            protected set => _duTruMuaVatTuTheoKhoaNhanVienYeuCaus = value;
        }

        private ICollection<DuTruMuaVatTuTheoKhoa> _duTruMuaVatTuTheoKhoaNhanVienKhoDuocs { get; set; }
        public virtual ICollection<DuTruMuaVatTuTheoKhoa> DuTruMuaVatTuTheoKhoaNhanVienKhoDuocs
        {
            get => _duTruMuaVatTuTheoKhoaNhanVienKhoDuocs ?? (_duTruMuaVatTuTheoKhoaNhanVienKhoDuocs = new List<DuTruMuaVatTuTheoKhoa>());
            protected set => _duTruMuaVatTuTheoKhoaNhanVienKhoDuocs = value;
        }

        private ICollection<DuTruMuaVatTuKhoDuoc> _duTruMuaVatTuKhoDuocNhanVienYeuCaus { get; set; }
        public virtual ICollection<DuTruMuaVatTuKhoDuoc> DuTruMuaVatTuKhoDuocNhanVienYeuCaus
        {
            get => _duTruMuaVatTuKhoDuocNhanVienYeuCaus ?? (_duTruMuaVatTuKhoDuocNhanVienYeuCaus = new List<DuTruMuaVatTuKhoDuoc>());
            protected set => _duTruMuaVatTuKhoDuocNhanVienYeuCaus = value;
        }

        private ICollection<DuTruMuaVatTuKhoDuoc> _duTruMuaVatTuKhoDuocGiamDocs { get; set; }
        public virtual ICollection<DuTruMuaVatTuKhoDuoc> DuTruMuaVatTuKhoDuocGiamDocs
        {
            get => _duTruMuaVatTuKhoDuocGiamDocs ?? (_duTruMuaVatTuKhoDuocGiamDocs = new List<DuTruMuaVatTuKhoDuoc>());
            protected set => _duTruMuaVatTuKhoDuocGiamDocs = value;
        }

        private ICollection<AuditGachNo> _auditGachNos { get; set; }
        public virtual ICollection<AuditGachNo> AuditGachNos
        {
            get => _auditGachNos ?? (_auditGachNos = new List<AuditGachNo>());
            protected set => _auditGachNos = value;
        }

        private ICollection<GachNo> _gachNos { get; set; }
        public virtual ICollection<GachNo> GachNos
        {
            get => _gachNos ?? (_gachNos = new List<GachNo>());
            protected set => _gachNos = value;
        }

        private ICollection<PhienXetNghiem> _phienXetNghiemNhanVienThucHiens;
        public virtual ICollection<PhienXetNghiem> PhienXetNghiemNhanVienThucHiens
        {
            get => _phienXetNghiemNhanVienThucHiens ?? (_phienXetNghiemNhanVienThucHiens = new List<PhienXetNghiem>());
            protected set => _phienXetNghiemNhanVienThucHiens = value;
        }

        private ICollection<PhienXetNghiem> _phienXetNghiemNhanVienKetLuans;
        public virtual ICollection<PhienXetNghiem> PhienXetNghiemNhanVienKetLuans
        {
            get => _phienXetNghiemNhanVienKetLuans ?? (_phienXetNghiemNhanVienKetLuans = new List<PhienXetNghiem>());
            protected set => _phienXetNghiemNhanVienKetLuans = value;
        }

        private ICollection<MauXetNghiem> _mauXetNghiemNhanVienLayMaus;
        public virtual ICollection<MauXetNghiem> MauXetNghiemNhanVienLayMaus
        {
            get => _mauXetNghiemNhanVienLayMaus ?? (_mauXetNghiemNhanVienLayMaus = new List<MauXetNghiem>());
            protected set => _mauXetNghiemNhanVienLayMaus = value;
        }

        private ICollection<MauXetNghiem> _mauXetNghiemNhanVienXetKhongDats;
        public virtual ICollection<MauXetNghiem> MauXetNghiemNhanVienXetKhongDats
        {
            get => _mauXetNghiemNhanVienXetKhongDats ?? (_mauXetNghiemNhanVienXetKhongDats = new List<MauXetNghiem>());
            protected set => _mauXetNghiemNhanVienXetKhongDats = value;
        }

        private ICollection<PhieuGoiMauXetNghiem> _phieuGoiMauXetNghiemNhanVienGoiMaus;
        public virtual ICollection<PhieuGoiMauXetNghiem> PhieuGoiMauXetNghiemNhanVienGoiMaus
        {
            get => _phieuGoiMauXetNghiemNhanVienGoiMaus ?? (_phieuGoiMauXetNghiemNhanVienGoiMaus = new List<PhieuGoiMauXetNghiem>());
            protected set => _phieuGoiMauXetNghiemNhanVienGoiMaus = value;
        }

        private ICollection<PhieuGoiMauXetNghiem> _phieuGoiMauXetNghiemNhanVienNhanMaus;
        public virtual ICollection<PhieuGoiMauXetNghiem> PhieuGoiMauXetNghiemNhanVienNhanMaus
        {
            get => _phieuGoiMauXetNghiemNhanVienNhanMaus ?? (_phieuGoiMauXetNghiemNhanVienNhanMaus = new List<PhieuGoiMauXetNghiem>());
            protected set => _phieuGoiMauXetNghiemNhanVienNhanMaus = value;
        }

        private ICollection<PhienXetNghiemChiTiet> _phienXetNghiemChiTiets;
        public virtual ICollection<PhienXetNghiemChiTiet> PhienXetNghiemChiTiets
        {
            get => _phienXetNghiemChiTiets ?? (_phienXetNghiemChiTiets = new List<PhienXetNghiemChiTiet>());
            protected set => _phienXetNghiemChiTiets = value;
        }

        private ICollection<YeuCauChayLaiXetNghiem> _yeuCauChayLaiXetNghiemNhanVienYeuCaus;
        public virtual ICollection<YeuCauChayLaiXetNghiem> YeuCauChayLaiXetNghiemNhanVienYeuCaus
        {
            get => _yeuCauChayLaiXetNghiemNhanVienYeuCaus ?? (_yeuCauChayLaiXetNghiemNhanVienYeuCaus = new List<YeuCauChayLaiXetNghiem>());
            protected set => _yeuCauChayLaiXetNghiemNhanVienYeuCaus = value;
        }

        private ICollection<YeuCauChayLaiXetNghiem> _yeuCauChayLaiXetNghiemNhanVienDuyets;
        public virtual ICollection<YeuCauChayLaiXetNghiem> YeuCauChayLaiXetNghiemNhanVienDuyets
        {
            get => _yeuCauChayLaiXetNghiemNhanVienDuyets ?? (_yeuCauChayLaiXetNghiemNhanVienDuyets = new List<YeuCauChayLaiXetNghiem>());
            protected set => _yeuCauChayLaiXetNghiemNhanVienDuyets = value;
        }

        private ICollection<KetQuaXetNghiemChiTiet> _ketQuaXetNghiemChiTietNhanVienNhapTays;
        public virtual ICollection<KetQuaXetNghiemChiTiet> KetQuaXetNghiemChiTietNhanVienNhapTays
        {
            get => _ketQuaXetNghiemChiTietNhanVienNhapTays ?? (_ketQuaXetNghiemChiTietNhanVienNhapTays = new List<KetQuaXetNghiemChiTiet>());
            protected set => _ketQuaXetNghiemChiTietNhanVienNhapTays = value;
        }

        private ICollection<KetQuaXetNghiemChiTiet> _ketQuaXetNghiemChiTietNhanVienDuyets;
        public virtual ICollection<KetQuaXetNghiemChiTiet> KetQuaXetNghiemChiTietNhanVienDuyets
        {
            get => _ketQuaXetNghiemChiTietNhanVienDuyets ?? (_ketQuaXetNghiemChiTietNhanVienDuyets = new List<KetQuaXetNghiemChiTiet>());
            protected set => _ketQuaXetNghiemChiTietNhanVienDuyets = value;
        }

        private ICollection<NoiTruBenhAn> _nhanVienLuuTruNoiTruBenhAns;
        public virtual ICollection<NoiTruBenhAn> NhanVienLuuTruNoiTruBenhAns
        {
            get => _nhanVienLuuTruNoiTruBenhAns ?? (_nhanVienLuuTruNoiTruBenhAns = new List<NoiTruBenhAn>());
            protected set => _nhanVienLuuTruNoiTruBenhAns = value;
        }

        private ICollection<NoiTruBenhAn> _nhanVienTaoBenhAnNoiTruBenhAns;
        public virtual ICollection<NoiTruBenhAn> NhanVienTaoBenhAnNoiTruBenhAns
        {
            get => _nhanVienTaoBenhAnNoiTruBenhAns ?? (_nhanVienTaoBenhAnNoiTruBenhAns = new List<NoiTruBenhAn>());
            protected set => _nhanVienTaoBenhAnNoiTruBenhAns = value;
        }

        private ICollection<NoiTruEkipDieuTri> _bacSiNoiTruEkipDieuTris;
        public virtual ICollection<NoiTruEkipDieuTri> BacSiNoiTruEkipDieuTris
        {
            get => _bacSiNoiTruEkipDieuTris ?? (_bacSiNoiTruEkipDieuTris = new List<NoiTruEkipDieuTri>());
            protected set => _bacSiNoiTruEkipDieuTris = value;
        }

        private ICollection<NoiTruEkipDieuTri> _dieuDuongNoiTruEkipDieuTris;
        public virtual ICollection<NoiTruEkipDieuTri> DieuDuongNoiTruEkipDieuTris
        {
            get => _dieuDuongNoiTruEkipDieuTris ?? (_dieuDuongNoiTruEkipDieuTris = new List<NoiTruEkipDieuTri>());
            protected set => _dieuDuongNoiTruEkipDieuTris = value;
        }

        private ICollection<NoiTruEkipDieuTri> _nhanVienLapNoiTruEkipDieuTris;
        public virtual ICollection<NoiTruEkipDieuTri> NhanVienLapNoiTruEkipDieuTris
        {
            get => _nhanVienLapNoiTruEkipDieuTris ?? (_nhanVienLapNoiTruEkipDieuTris = new List<NoiTruEkipDieuTri>());
            protected set => _nhanVienLapNoiTruEkipDieuTris = value;
        }

        private ICollection<NoiTruHoSoKhac> _noiTruHoSoKhacs;
        public virtual ICollection<NoiTruHoSoKhac> NoiTruHoSoKhacs
        {
            get => _noiTruHoSoKhacs ?? (_noiTruHoSoKhacs = new List<NoiTruHoSoKhac>());
            protected set => _noiTruHoSoKhacs = value;
        }

        private ICollection<NoiTruKhoaPhongDieuTri> _noiTruKhoaPhongDieuTris;
        public virtual ICollection<NoiTruKhoaPhongDieuTri> NoiTruKhoaPhongDieuTris
        {
            get => _noiTruKhoaPhongDieuTris ?? (_noiTruKhoaPhongDieuTris = new List<NoiTruKhoaPhongDieuTri>());
            protected set => _noiTruKhoaPhongDieuTris = value;
        }
        
        private ICollection<NoiTruPhieuDieuTriChiTietYLenh> _nhanVienChiDinhNoiTruPhieuDieuTriChiTietYLenhs;
        public virtual ICollection<NoiTruPhieuDieuTriChiTietYLenh> NhanVienChiDinhNoiTruPhieuDieuTriChiTietYLenhs
        {
            get => _nhanVienChiDinhNoiTruPhieuDieuTriChiTietYLenhs ?? (_nhanVienChiDinhNoiTruPhieuDieuTriChiTietYLenhs = new List<NoiTruPhieuDieuTriChiTietYLenh>());
            protected set => _nhanVienChiDinhNoiTruPhieuDieuTriChiTietYLenhs = value;
        }

        private ICollection<NoiTruPhieuDieuTriChiTietYLenh> _nhanVienCapNhatNoiTruPhieuDieuTriChiTietYLenhs;
        public virtual ICollection<NoiTruPhieuDieuTriChiTietYLenh> NhanVienCapNhatNoiTruPhieuDieuTriChiTietYLenhs
        {
            get => _nhanVienCapNhatNoiTruPhieuDieuTriChiTietYLenhs ?? (_nhanVienCapNhatNoiTruPhieuDieuTriChiTietYLenhs = new List<NoiTruPhieuDieuTriChiTietYLenh>());
            protected set => _nhanVienCapNhatNoiTruPhieuDieuTriChiTietYLenhs = value;
        }

        private ICollection<NoiTruPhieuDieuTriChiTietYLenh> _nhanVienXacNhanThucHienNoiTruPhieuDieuTriChiTietYLenhs;
        public virtual ICollection<NoiTruPhieuDieuTriChiTietYLenh> NhanVienXacNhanThucHienNoiTruPhieuDieuTriChiTietYLenhs
        {
            get => _nhanVienXacNhanThucHienNoiTruPhieuDieuTriChiTietYLenhs ?? (_nhanVienXacNhanThucHienNoiTruPhieuDieuTriChiTietYLenhs = new List<NoiTruPhieuDieuTriChiTietYLenh>());
            protected set => _nhanVienXacNhanThucHienNoiTruPhieuDieuTriChiTietYLenhs = value;
        }

        private ICollection<NoiTruPhieuDieuTri> _noiTruPhieuDieuTris;
        public virtual ICollection<NoiTruPhieuDieuTri> NoiTruPhieuDieuTris
        {
            get => _noiTruPhieuDieuTris ?? (_noiTruPhieuDieuTris = new List<NoiTruPhieuDieuTri>());
            protected set => _noiTruPhieuDieuTris = value;
        }

        private ICollection<YeuCauDichVuGiuongBenhVienChiPhi> _nhanVienDuyetBaoHiemYeuCauDichVuGiuongBenhVienChiPhis;
        public virtual ICollection<YeuCauDichVuGiuongBenhVienChiPhi> NhanVienDuyetBaoHiemYeuCauDichVuGiuongBenhVienChiPhis
        {
            get => _nhanVienDuyetBaoHiemYeuCauDichVuGiuongBenhVienChiPhis ?? (_nhanVienDuyetBaoHiemYeuCauDichVuGiuongBenhVienChiPhis = new List<YeuCauDichVuGiuongBenhVienChiPhi>());
            protected set => _nhanVienDuyetBaoHiemYeuCauDichVuGiuongBenhVienChiPhis = value;
        }

        private ICollection<YeuCauDichVuGiuongBenhVienChiPhi> _nhanVienHuyThanhToanYeuCauDichVuGiuongBenhVienChiPhis;
        public virtual ICollection<YeuCauDichVuGiuongBenhVienChiPhi> NhanVienHuyThanhToanYeuCauDichVuGiuongBenhVienChiPhis
        {
            get => _nhanVienHuyThanhToanYeuCauDichVuGiuongBenhVienChiPhis ?? (_nhanVienHuyThanhToanYeuCauDichVuGiuongBenhVienChiPhis = new List<YeuCauDichVuGiuongBenhVienChiPhi>());
            protected set => _nhanVienHuyThanhToanYeuCauDichVuGiuongBenhVienChiPhis = value;
        }

        private ICollection<YeuCauNhapVien> _yeuCauNhapViens;
        public virtual ICollection<YeuCauNhapVien> YeuCauNhapViens
        {
            get => _yeuCauNhapViens ?? (_yeuCauNhapViens = new List<YeuCauNhapVien>());
            protected set => _yeuCauNhapViens = value;
        }

        private ICollection<YeuCauTraDuocPhamTuBenhNhan> _nhanVienYeuCauYeuCauTraDuocPhamTuBenhNhans;
        public virtual ICollection<YeuCauTraDuocPhamTuBenhNhan> NhanVienYeuCauYeuCauTraDuocPhamTuBenhNhans
        {
            get => _nhanVienYeuCauYeuCauTraDuocPhamTuBenhNhans ?? (_nhanVienYeuCauYeuCauTraDuocPhamTuBenhNhans = new List<YeuCauTraDuocPhamTuBenhNhan>());
            protected set => _nhanVienYeuCauYeuCauTraDuocPhamTuBenhNhans = value;
        }

        private ICollection<YeuCauTraDuocPhamTuBenhNhan> _nhanVienDuyetYeuCauTraDuocPhamTuBenhNhans;
        public virtual ICollection<YeuCauTraDuocPhamTuBenhNhan> NhanVienDuyetYeuCauTraDuocPhamTuBenhNhans
        {
            get => _nhanVienDuyetYeuCauTraDuocPhamTuBenhNhans ?? (_nhanVienDuyetYeuCauTraDuocPhamTuBenhNhans = new List<YeuCauTraDuocPhamTuBenhNhan>());
            protected set => _nhanVienDuyetYeuCauTraDuocPhamTuBenhNhans = value;
        }

        private ICollection<YeuCauTraVatTuTuBenhNhan> _nhanVienYeuCauYeuCauTraVatTuTuBenhNhans;
        public virtual ICollection<YeuCauTraVatTuTuBenhNhan> NhanVienYeuCauYeuCauTraVatTuTuBenhNhans
        {
            get => _nhanVienYeuCauYeuCauTraVatTuTuBenhNhans ?? (_nhanVienYeuCauYeuCauTraVatTuTuBenhNhans = new List<YeuCauTraVatTuTuBenhNhan>());
            protected set => _nhanVienYeuCauYeuCauTraVatTuTuBenhNhans = value;
        }

        private ICollection<YeuCauTraVatTuTuBenhNhan> _nhanVienDuyetYeuCauTraVatTuTuBenhNhans;
        public virtual ICollection<YeuCauTraVatTuTuBenhNhan> NhanVienDuyetYeuCauTraVatTuTuBenhNhans
        {
            get => _nhanVienDuyetYeuCauTraVatTuTuBenhNhans ?? (_nhanVienDuyetYeuCauTraVatTuTuBenhNhans = new List<YeuCauTraVatTuTuBenhNhan>());
            protected set => _nhanVienDuyetYeuCauTraVatTuTuBenhNhans = value;
        }

        private ICollection<NhapKhoMau> _nguoiGiaoNhapKhoMaus;
        public virtual ICollection<NhapKhoMau> NguoiGiaoNhapKhoMaus
        {
            get => _nguoiGiaoNhapKhoMaus ?? (_nguoiGiaoNhapKhoMaus = new List<NhapKhoMau>());
            protected set => _nguoiGiaoNhapKhoMaus = value;
        }

        private ICollection<NhapKhoMau> _nguoiNhapNhapKhoMaus;
        public virtual ICollection<NhapKhoMau> NguoiNhapNhapKhoMaus
        {
            get => _nguoiNhapNhapKhoMaus ?? (_nguoiNhapNhapKhoMaus = new List<NhapKhoMau>());
            protected set => _nguoiNhapNhapKhoMaus = value;
        }

        private ICollection<NhapKhoMau> _nhanVienDuyetNhapKhoMaus;
        public virtual ICollection<NhapKhoMau> NhanVienDuyetNhapKhoMaus
        {
            get => _nhanVienDuyetNhapKhoMaus ?? (_nhanVienDuyetNhapKhoMaus = new List<NhapKhoMau>());
            protected set => _nhanVienDuyetNhapKhoMaus = value;
        }
        
        private ICollection<XuatKhoMau> _nguoiXuatXuatKhoMaus;
        public virtual ICollection<XuatKhoMau> NguoiXuatXuatKhoMaus
        {
            get => _nguoiXuatXuatKhoMaus ?? (_nguoiXuatXuatKhoMaus = new List<XuatKhoMau>());
            protected set => _nguoiXuatXuatKhoMaus = value;
        }

        private ICollection<XuatKhoMau> _nguoiNhanXuatKhoMaus;
        public virtual ICollection<XuatKhoMau> NguoiNhanXuatKhoMaus
        {
            get => _nguoiNhanXuatKhoMaus ?? (_nguoiNhanXuatKhoMaus = new List<XuatKhoMau>());
            protected set => _nguoiNhanXuatKhoMaus = value;
        }

        private ICollection<YeuCauTruyenMau> _nhanVienDuyetBaoHiemYeuCauTruyenMaus;
        public virtual ICollection<YeuCauTruyenMau> NhanVienDuyetBaoHiemYeuCauTruyenMaus
        {
            get => _nhanVienDuyetBaoHiemYeuCauTruyenMaus ?? (_nhanVienDuyetBaoHiemYeuCauTruyenMaus = new List<YeuCauTruyenMau>());
            protected set => _nhanVienDuyetBaoHiemYeuCauTruyenMaus = value;
        }

        private ICollection<YeuCauTruyenMau> _nhanVienHuyThanhToanYeuCauTruyenMaus;
        public virtual ICollection<YeuCauTruyenMau> NhanVienHuyThanhToanYeuCauTruyenMaus
        {
            get => _nhanVienHuyThanhToanYeuCauTruyenMaus ?? (_nhanVienHuyThanhToanYeuCauTruyenMaus = new List<YeuCauTruyenMau>());
            protected set => _nhanVienHuyThanhToanYeuCauTruyenMaus = value;
        }

        private ICollection<YeuCauTruyenMau> _nhanVienChiDinhYeuCauTruyenMaus;
        public virtual ICollection<YeuCauTruyenMau> NhanVienChiDinhYeuCauTruyenMaus
        {
            get => _nhanVienChiDinhYeuCauTruyenMaus ?? (_nhanVienChiDinhYeuCauTruyenMaus = new List<YeuCauTruyenMau>());
            protected set => _nhanVienChiDinhYeuCauTruyenMaus = value;
        }

        private ICollection<YeuCauTruyenMau> _nhanVienThucHienYeuCauTruyenMaus;
        public virtual ICollection<YeuCauTruyenMau> NhanVienThucHienYeuCauTruyenMaus
        {
            get => _nhanVienThucHienYeuCauTruyenMaus ?? (_nhanVienThucHienYeuCauTruyenMaus = new List<YeuCauTruyenMau>());
            protected set => _nhanVienThucHienYeuCauTruyenMaus = value;
        }        

        private ICollection<NoiTruPhieuDieuTriChiTietDienBien> _noiTruPhieuDieuTriChiTietDienBiens;
        public virtual ICollection<NoiTruPhieuDieuTriChiTietDienBien> NoiTruPhieuDieuTriChiTietDienBiens
        {
            get => _noiTruPhieuDieuTriChiTietDienBiens ?? (_noiTruPhieuDieuTriChiTietDienBiens = new List<NoiTruPhieuDieuTriChiTietDienBien>());
            protected set => _noiTruPhieuDieuTriChiTietDienBiens = value;
        }

        private ICollection<NoiTruChiDinhDuocPham> _noiTruChiDinhDuocPhamCapThuocs { get; set; }
        public virtual ICollection<NoiTruChiDinhDuocPham> NoiTruChiDinhDuocPhamCapThuocs
        {
            get => _noiTruChiDinhDuocPhamCapThuocs ?? (_noiTruChiDinhDuocPhamCapThuocs = new List<NoiTruChiDinhDuocPham>());
            protected set => _noiTruChiDinhDuocPhamCapThuocs = value;
        }

        private ICollection<NoiTruChiDinhDuocPham> _noiTruChiDinhDuocPhamChiDinhs { get; set; }
        public virtual ICollection<NoiTruChiDinhDuocPham> NoiTruChiDinhDuocPhamChiDinhs
        {
            get => _noiTruChiDinhDuocPhamChiDinhs ?? (_noiTruChiDinhDuocPhamChiDinhs = new List<NoiTruChiDinhDuocPham>());
            protected set => _noiTruChiDinhDuocPhamChiDinhs = value;
        }

        private ICollection<YeuCauTraDuocPhamTuBenhNhanChiTiet> _yeuCauTraDuocPhamTuBenhNhanChiTiets { get; set; }
        public virtual ICollection<YeuCauTraDuocPhamTuBenhNhanChiTiet> YeuCauTraDuocPhamTuBenhNhanChiTiets
        {
            get => _yeuCauTraDuocPhamTuBenhNhanChiTiets ?? (_yeuCauTraDuocPhamTuBenhNhanChiTiets = new List<YeuCauTraDuocPhamTuBenhNhanChiTiet>());
            protected set => _yeuCauTraDuocPhamTuBenhNhanChiTiets = value;
        }

        private ICollection<YeuCauTraVatTuTuBenhNhanChiTiet> _yeuCauTraVatTuTuBenhNhanChiTiets;
        public virtual ICollection<YeuCauTraVatTuTuBenhNhanChiTiet> YeuCauTraVatTuTuBenhNhanChiTiets
        {
            get => _yeuCauTraVatTuTuBenhNhanChiTiets ?? (_yeuCauTraVatTuTuBenhNhanChiTiets = new List<YeuCauTraVatTuTuBenhNhanChiTiet>());
            protected set => _yeuCauTraVatTuTuBenhNhanChiTiets = value;
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

        private ICollection<YeuCauNhanSuKhamSucKhoe> _yeuCauNhanSuNhanVienGuiYeuCauKhamSucKhoes;
        public virtual ICollection<YeuCauNhanSuKhamSucKhoe> YeuCauNhanSuNhanVienGuiYeuCauKhamSucKhoes
        {
            get => _yeuCauNhanSuNhanVienGuiYeuCauKhamSucKhoes ?? (_yeuCauNhanSuNhanVienGuiYeuCauKhamSucKhoes = new List<YeuCauNhanSuKhamSucKhoe>());
            protected set => _yeuCauNhanSuNhanVienGuiYeuCauKhamSucKhoes = value;
        }

        private ICollection<YeuCauNhanSuKhamSucKhoe> _yeuCauNhanSuNhanVienKHTHDuyetKhamSucKhoes;
        public virtual ICollection<YeuCauNhanSuKhamSucKhoe> YeuCauNhanSuNhanVienKHTHDuyetKhamSucKhoes
        {
            get => _yeuCauNhanSuNhanVienKHTHDuyetKhamSucKhoes ?? (_yeuCauNhanSuNhanVienKHTHDuyetKhamSucKhoes = new List<YeuCauNhanSuKhamSucKhoe>());
            protected set => _yeuCauNhanSuNhanVienKHTHDuyetKhamSucKhoes = value;
        }

        private ICollection<YeuCauNhanSuKhamSucKhoe> _yeuCauNhanSuNhanVienNhanSuDuyetKhamSucKhoes;
        public virtual ICollection<YeuCauNhanSuKhamSucKhoe> YeuCauNhanSuNhanVienNhanSuDuyetKhamSucKhoes
        {
            get => _yeuCauNhanSuNhanVienNhanSuDuyetKhamSucKhoes ?? (_yeuCauNhanSuNhanVienNhanSuDuyetKhamSucKhoes = new List<YeuCauNhanSuKhamSucKhoe>());
            protected set => _yeuCauNhanSuNhanVienNhanSuDuyetKhamSucKhoes = value;
        }

        private ICollection<YeuCauNhanSuKhamSucKhoe> _yeuCauNhanSuGiamDocKhamSucKhoes;
        public virtual ICollection<YeuCauNhanSuKhamSucKhoe> YeuCauNhanSuGiamDocKhamSucKhoes
        {
            get => _yeuCauNhanSuGiamDocKhamSucKhoes ?? (_yeuCauNhanSuGiamDocKhamSucKhoes = new List<YeuCauNhanSuKhamSucKhoe>());
            protected set => _yeuCauNhanSuGiamDocKhamSucKhoes = value;
        }

        private ICollection<YeuCauNhanSuKhamSucKhoeChiTiet> _yeuCauNhanSuNhanVienKhamSucKhoeChiTiets;
        public virtual ICollection<YeuCauNhanSuKhamSucKhoeChiTiet> YeuCauNhanSuNhanVienKhamSucKhoeChiTiets
        {
            get => _yeuCauNhanSuNhanVienKhamSucKhoeChiTiets ?? (_yeuCauNhanSuNhanVienKhamSucKhoeChiTiets = new List<YeuCauNhanSuKhamSucKhoeChiTiet>());
            protected set => _yeuCauNhanSuNhanVienKhamSucKhoeChiTiets = value;
        }

        private ICollection<YeuCauNhanSuKhamSucKhoeChiTiet> _yeuCauNhanSuNguoiGioiThieuKhamSucKhoeChiTiets;
        public virtual ICollection<YeuCauNhanSuKhamSucKhoeChiTiet> YeuCauNhanSuNguoiGioiThieuKhamSucKhoeChiTiets
        {
            get => _yeuCauNhanSuNguoiGioiThieuKhamSucKhoeChiTiets ?? (_yeuCauNhanSuNguoiGioiThieuKhamSucKhoeChiTiets = new List<YeuCauNhanSuKhamSucKhoeChiTiet>());
            protected set => _yeuCauNhanSuNguoiGioiThieuKhamSucKhoeChiTiets = value;
        }


        private ICollection<HopDongKhamSucKhoe> _hopDongKhamSucKhoe;
        public virtual ICollection<HopDongKhamSucKhoe> HopDongKhamSucKhoe
        {
            get => _hopDongKhamSucKhoe ?? (_hopDongKhamSucKhoe = new List<HopDongKhamSucKhoe>());
            protected set => _hopDongKhamSucKhoe = value;
        }

        private ICollection<YeuCauTiepNhan> _yeuCauTiepNhanKSKNhanVienDanhGiaCanLamSangs { get; set; }
        public virtual ICollection<YeuCauTiepNhan> YeuCauTiepNhanKSKNhanVienDanhGiaCanLamSangs
        {
            get => _yeuCauTiepNhanKSKNhanVienDanhGiaCanLamSangs ?? (_yeuCauTiepNhanKSKNhanVienDanhGiaCanLamSangs = new List<YeuCauTiepNhan>());
            protected set => _yeuCauTiepNhanKSKNhanVienDanhGiaCanLamSangs = value;
        }

        private ICollection<YeuCauTiepNhan> _yeuCauTiepNhanKSKNhanVienKetLuans { get; set; }
        public virtual ICollection<YeuCauTiepNhan> YeuCauTiepNhanKSKNhanVienKetLuans
        {
            get => _yeuCauTiepNhanKSKNhanVienKetLuans ?? (_yeuCauTiepNhanKSKNhanVienKetLuans = new List<YeuCauTiepNhan>());
            protected set => _yeuCauTiepNhanKSKNhanVienKetLuans = value;
        }

        private ICollection<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuatNhanVienHuyDichVus;
        public virtual ICollection<YeuCauDichVuKyThuat> YeuCauDichVuKyThuatNhanVienHuyDichVus
        {
            get => _yeuCauDichVuKyThuatNhanVienHuyDichVus ?? (_yeuCauDichVuKyThuatNhanVienHuyDichVus = new List<YeuCauDichVuKyThuat>());
            protected set => _yeuCauDichVuKyThuatNhanVienHuyDichVus = value;
        }

        private ICollection<YeuCauKhamBenh> _yeuCauKhamBenhNhanVienHuyDichVus;
        public virtual ICollection<YeuCauKhamBenh> YeuCauKhamBenhNhanVienHuyDichVus
        {
            get => _yeuCauKhamBenhNhanVienHuyDichVus ?? (_yeuCauKhamBenhNhanVienHuyDichVus = new List<YeuCauKhamBenh>());
            protected set => _yeuCauKhamBenhNhanVienHuyDichVus = value;
        }

        private ICollection<NoiTruPhieuDieuTriTuVanThuoc> _noiTruPhieuDieuTriTuVanThuocs;
        public virtual ICollection<NoiTruPhieuDieuTriTuVanThuoc> NoiTruPhieuDieuTriTuVanThuocs
        {
            get => _noiTruPhieuDieuTriTuVanThuocs ?? (_noiTruPhieuDieuTriTuVanThuocs = new List<NoiTruPhieuDieuTriTuVanThuoc>());
            protected set => _noiTruPhieuDieuTriTuVanThuocs = value;
        }

        private ICollection<MauXetNghiem> _mauXetNghiemNhanVienNhanMaus;
        public virtual ICollection<MauXetNghiem> MauXetNghiemNhanVienNhanMaus
        {
            get => _mauXetNghiemNhanVienNhanMaus ?? (_mauXetNghiemNhanVienNhanMaus = new List<MauXetNghiem>());
            protected set => _mauXetNghiemNhanVienNhanMaus = value;
        }

        private ICollection<PhienXetNghiemChiTiet> _phienXetNghiemChiTietNhanVienLayMaus;
        public virtual ICollection<PhienXetNghiemChiTiet> PhienXetNghiemChiTietNhanVienLayMaus
        {
            get => _phienXetNghiemChiTietNhanVienLayMaus ?? (_phienXetNghiemChiTietNhanVienLayMaus = new List<PhienXetNghiemChiTiet>());
            protected set => _phienXetNghiemChiTietNhanVienLayMaus = value;
        }

        private ICollection<PhienXetNghiemChiTiet> _phienXetNghiemChiTietNhanVienNhanMaus;
        public virtual ICollection<PhienXetNghiemChiTiet> PhienXetNghiemChiTietNhanVienNhanMaus
        {
            get => _phienXetNghiemChiTietNhanVienNhanMaus ?? (_phienXetNghiemChiTietNhanVienNhanMaus = new List<PhienXetNghiemChiTiet>());
            protected set => _phienXetNghiemChiTietNhanVienNhanMaus = value;
        }

        private ICollection<YeuCauXuatKhoDuocPham> _yeuCauXuatKhoDuocPhamNguoiXuats;
        public virtual ICollection<YeuCauXuatKhoDuocPham> YeuCauXuatKhoDuocPhamNguoiXuats
        {
            get => _yeuCauXuatKhoDuocPhamNguoiXuats ?? (_yeuCauXuatKhoDuocPhamNguoiXuats = new List<YeuCauXuatKhoDuocPham>());
            protected set => _yeuCauXuatKhoDuocPhamNguoiXuats = value;
        }

        private ICollection<YeuCauXuatKhoDuocPham> _yeuCauXuatKhoDuocPhamNguoiNhans;
        public virtual ICollection<YeuCauXuatKhoDuocPham> YeuCauXuatKhoDuocPhamNguoiNhans
        {
            get => _yeuCauXuatKhoDuocPhamNguoiNhans ?? (_yeuCauXuatKhoDuocPhamNguoiNhans = new List<YeuCauXuatKhoDuocPham>());
            protected set => _yeuCauXuatKhoDuocPhamNguoiNhans = value;
        }

        private ICollection<YeuCauXuatKhoDuocPham> _yeuCauXuatKhoDuocPhamNhanVienDuyets;
        public virtual ICollection<YeuCauXuatKhoDuocPham> YeuCauXuatKhoDuocPhamNhanVienDuyets
        {
            get => _yeuCauXuatKhoDuocPhamNhanVienDuyets ?? (_yeuCauXuatKhoDuocPhamNhanVienDuyets = new List<YeuCauXuatKhoDuocPham>());
            protected set => _yeuCauXuatKhoDuocPhamNhanVienDuyets = value;
        }
        
        private ICollection<NoiTruDonThuoc> _noiTruDonThuocBacSiKeDons;
        public virtual ICollection<NoiTruDonThuoc> NoiTruDonThuocBacSiKeDons
        {
            get => _noiTruDonThuocBacSiKeDons ?? (_noiTruDonThuocBacSiKeDons = new List<NoiTruDonThuoc>());
            protected set => _noiTruDonThuocBacSiKeDons = value;
        }

        private ICollection<YeuCauXuatKhoVatTu> _yeuCauXuatKhoVatTuNguoiXuats;
        public virtual ICollection<YeuCauXuatKhoVatTu> YeuCauXuatKhoVatTuNguoiXuats
        {
            get => _yeuCauXuatKhoVatTuNguoiXuats ?? (_yeuCauXuatKhoVatTuNguoiXuats = new List<YeuCauXuatKhoVatTu>());
            protected set => _yeuCauXuatKhoVatTuNguoiXuats = value;
        }

        private ICollection<YeuCauXuatKhoVatTu> _yeuCauXuatKhoVatTuNguoiNhans;
        public virtual ICollection<YeuCauXuatKhoVatTu> YeuCauXuatKhoVatTuNguoiNhans
        {
            get => _yeuCauXuatKhoVatTuNguoiNhans ?? (_yeuCauXuatKhoVatTuNguoiNhans = new List<YeuCauXuatKhoVatTu>());
            protected set => _yeuCauXuatKhoVatTuNguoiNhans = value;
        }

        private ICollection<YeuCauXuatKhoVatTu> _yeuCauXuatKhoVatTuNhanVienDuyets;
        public virtual ICollection<YeuCauXuatKhoVatTu> YeuCauXuatKhoVatTuNhanVienDuyets
        {
            get => _yeuCauXuatKhoVatTuNhanVienDuyets ?? (_yeuCauXuatKhoVatTuNhanVienDuyets = new List<YeuCauXuatKhoVatTu>());
            protected set => _yeuCauXuatKhoVatTuNhanVienDuyets = value;
        }

        // dùng cho màn hình kết quả CĐHA-TDCN
        private ICollection<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuatNhanVienKetLuan2s;
        public virtual ICollection<YeuCauDichVuKyThuat> YeuCauDichVuKyThuatNhanVienKetLuan2s
        {
            get => _yeuCauDichVuKyThuatNhanVienKetLuan2s ?? (_yeuCauDichVuKyThuatNhanVienKetLuan2s = new List<YeuCauDichVuKyThuat>());
            protected set => _yeuCauDichVuKyThuatNhanVienKetLuan2s = value;
        }

        private ICollection<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuatNhanVienThucHien2s;
        public virtual ICollection<YeuCauDichVuKyThuat> YeuCauDichVuKyThuatNhanVienThucHien2s
        {
            get => _yeuCauDichVuKyThuatNhanVienThucHien2s ?? (_yeuCauDichVuKyThuatNhanVienThucHien2s = new List<YeuCauDichVuKyThuat>());
            protected set => _yeuCauDichVuKyThuatNhanVienThucHien2s = value;
        }
        //========================================================================================================================================


        // dùng cho chức năng hủy trạng thái thực hiện dịch vụ
        private ICollection<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuatNhanVienHuyTrangThaiDaThucHiens;
        public virtual ICollection<YeuCauDichVuKyThuat> YeuCauDichVuKyThuatNhanVienHuyTrangThaiDaThucHiens
        {
            get => _yeuCauDichVuKyThuatNhanVienHuyTrangThaiDaThucHiens ?? (_yeuCauDichVuKyThuatNhanVienHuyTrangThaiDaThucHiens = new List<YeuCauDichVuKyThuat>());
            protected set => _yeuCauDichVuKyThuatNhanVienHuyTrangThaiDaThucHiens = value;
        }
        //=====================================================


        private ICollection<YeuCauDieuChuyenDuocPham> _yeuCauDieuChuyenDuocPhamNguoiNhaps;
        public virtual ICollection<YeuCauDieuChuyenDuocPham> YeuCauDieuChuyenDuocPhamNguoiNhaps
        {
            get => _yeuCauDieuChuyenDuocPhamNguoiNhaps ?? (_yeuCauDieuChuyenDuocPhamNguoiNhaps = new List<YeuCauDieuChuyenDuocPham>());
            protected set => _yeuCauDieuChuyenDuocPhamNguoiNhaps = value;
        }

        private ICollection<YeuCauDieuChuyenDuocPham> _yeuCauDieuChuyenDuocPhamNguoiXuats;
        public virtual ICollection<YeuCauDieuChuyenDuocPham> YeuCauDieuChuyenDuocPhamNguoiXuats
        {
            get => _yeuCauDieuChuyenDuocPhamNguoiXuats ?? (_yeuCauDieuChuyenDuocPhamNguoiXuats = new List<YeuCauDieuChuyenDuocPham>());
            protected set => _yeuCauDieuChuyenDuocPhamNguoiXuats = value;
        }

        private ICollection<YeuCauDieuChuyenDuocPham> _yeuCauDieuChuyenDuocPhamNguoiNhanVienDuyets;
        public virtual ICollection<YeuCauDieuChuyenDuocPham> YeuCauDieuChuyenDuocPhamNguoiNhanVienDuyets
        {
            get => _yeuCauDieuChuyenDuocPhamNguoiNhanVienDuyets ?? (_yeuCauDieuChuyenDuocPhamNguoiNhanVienDuyets = new List<YeuCauDieuChuyenDuocPham>());
            protected set => _yeuCauDieuChuyenDuocPhamNguoiNhanVienDuyets = value;
        }

        private ICollection<NoiTruChiDinhPhaThuocTiem> _noiTruChiDinhPhaThuocTiems;
        public virtual ICollection<NoiTruChiDinhPhaThuocTiem> NoiTruChiDinhPhaThuocTiems
        {
            get => _noiTruChiDinhPhaThuocTiems ?? (_noiTruChiDinhPhaThuocTiems = new List<NoiTruChiDinhPhaThuocTiem>());
            protected set => _noiTruChiDinhPhaThuocTiems = value;
        }

        private ICollection<NoiTruChiDinhPhaThuocTruyen> _noiTruChiDinhPhaThuocTruyens;
        public virtual ICollection<NoiTruChiDinhPhaThuocTruyen> NoiTruChiDinhPhaThuocTruyens
        {
            get => _noiTruChiDinhPhaThuocTruyens ?? (_noiTruChiDinhPhaThuocTruyens = new List<NoiTruChiDinhPhaThuocTruyen>());
            protected set => _noiTruChiDinhPhaThuocTruyens = value;
        }

        private ICollection<NoiDungMauKhamBenh> _bacSiLuuNoiDungKhamBenhs;
        public virtual ICollection<NoiDungMauKhamBenh> BacSiLuuNoiDungKhamBenhs
        {
            get => _bacSiLuuNoiDungKhamBenhs ?? (_bacSiLuuNoiDungKhamBenhs = new List<NoiDungMauKhamBenh>());
            protected set => _bacSiLuuNoiDungKhamBenhs = value;
        }

        private ICollection<YeuCauKhamBenh> _yeuCauKhamBenhNghiHuongBHXHNguoiIns;
        public virtual ICollection<YeuCauKhamBenh> YeuCauKhamBenhNghiHuongBHXHNguoiIns
        {
            get => _yeuCauKhamBenhNghiHuongBHXHNguoiIns ?? (_yeuCauKhamBenhNghiHuongBHXHNguoiIns = new List<YeuCauKhamBenh>());
            protected set => _yeuCauKhamBenhNghiHuongBHXHNguoiIns = value;
        }
        #region update 28122021
        private ICollection<NhanVienChucVu> _nhanVienChucVus;
        public virtual ICollection<NhanVienChucVu> NhanVienChucVus
        {
            get => _nhanVienChucVus ?? (_nhanVienChucVus = new List<NhanVienChucVu>());
            protected set => _nhanVienChucVus = value;
        }

        private ICollection<HoSoNhanVienFileDinhKem> _hoSoNhanVienFileDinhKems;
        public virtual ICollection<HoSoNhanVienFileDinhKem> HoSoNhanVienFileDinhKems
        {
            get => _hoSoNhanVienFileDinhKems ?? (_hoSoNhanVienFileDinhKems = new List<HoSoNhanVienFileDinhKem>());
            protected set => _hoSoNhanVienFileDinhKems = value;
        }
        #endregion

        private ICollection<ThuePhong> _thuePhongs;
        public virtual ICollection<ThuePhong> ThuePhongs
        {
            get => _thuePhongs ?? (_thuePhongs = new List<ThuePhong>());
            protected set => _thuePhongs = value;
        }

        private ICollection<CauHinhNguoiDuyetTheoNhomDichVu> _cauHinhNguoiDuyetTheoNhomDichVus;
        public virtual ICollection<CauHinhNguoiDuyetTheoNhomDichVu> CauHinhNguoiDuyetTheoNhomDichVus
        {
            get => _cauHinhNguoiDuyetTheoNhomDichVus ?? (_cauHinhNguoiDuyetTheoNhomDichVus = new List<CauHinhNguoiDuyetTheoNhomDichVu>());
            protected set => _cauHinhNguoiDuyetTheoNhomDichVus = value;
        }
    }
}
