using Camino.Core.Domain.Entities.KhoaPhongs;
using Camino.Core.Domain.Entities.LichPhanCongNgoaiTrus;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using System.Collections.Generic;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.DichVuGiuongBenhViens;
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.DonVatTus;
using Camino.Core.Domain.Entities.DuyetBaoHiems;
using Camino.Core.Domain.Entities.GiuongBenhs;
using Camino.Core.Domain.Entities.KetQuaSinhHieus;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.KhoaPhongNhanViens;
using Camino.Core.Domain.Entities.PhauThuatThuThuats;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.MauVaChePhams;
using Camino.Core.Domain.Entities.YeuCauLinhDuocPhams;
using Camino.Core.Domain.Entities.YeuCauLinhVatTus;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.Entities.YeuCauNhapViens;
using Camino.Core.Domain.Entities.NoiTruDonThuocs;

namespace Camino.Core.Domain.Entities.PhongBenhViens
{
    public class PhongBenhVien : BaseEntity
    {
        public long KhoaPhongId { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }
        public bool? IsDisabled { get; set; }
        public string Tang { get; set; }
        public long? HopDongKhamSucKhoeId { get; set; }

        public virtual KhoaPhong KhoaPhong { get; set; }
        public virtual HopDongKhamSucKhoe HopDongKhamSucKhoe { get; set; }

        private ICollection<LichPhanCongNgoaiTru> _lichPhanCongNgoaiTrus { get; set; }
        public virtual ICollection<LichPhanCongNgoaiTru> LichPhanCongNgoaiTrus
        {
            get => _lichPhanCongNgoaiTrus ?? (_lichPhanCongNgoaiTrus = new List<LichPhanCongNgoaiTru>());
            protected set => _lichPhanCongNgoaiTrus = value;
        }

        private ICollection<PhongBenhVienHangDoi> _phongBenhVienHangDois { get; set; }
        public virtual ICollection<PhongBenhVienHangDoi> PhongBenhVienHangDois
        {
            get => _phongBenhVienHangDois ?? (_phongBenhVienHangDois = new List<PhongBenhVienHangDoi>());
            protected set => _phongBenhVienHangDois = value;
        }

        private ICollection<PhongBenhVienNhomGiaDichVuKhamBenh> _phongBenhVienNhomGiaDichVuKhamBenhs { get; set; }
        public virtual ICollection<PhongBenhVienNhomGiaDichVuKhamBenh> PhongBenhVienNhomGiaDichVuKhamBenhs
        {
            get => _phongBenhVienNhomGiaDichVuKhamBenhs ?? (_phongBenhVienNhomGiaDichVuKhamBenhs = new List<PhongBenhVienNhomGiaDichVuKhamBenh>());
            protected set => _phongBenhVienNhomGiaDichVuKhamBenhs = value;
        }

        private ICollection<PhongBenhVienNhomGiaDichVuKyThuat> _phongBenhVienNhomGiaDichVuKyThuats { get; set; }
        public virtual ICollection<PhongBenhVienNhomGiaDichVuKyThuat> PhongBenhVienNhomGiaDichVuKyThuats
        {
            get => _phongBenhVienNhomGiaDichVuKyThuats ?? (_phongBenhVienNhomGiaDichVuKyThuats = new List<PhongBenhVienNhomGiaDichVuKyThuat>());
            protected set => _phongBenhVienNhomGiaDichVuKyThuats = value;
        }

        private ICollection<YeuCauTiepNhan> _yeuCauTiepNhans { get; set; }
        public virtual ICollection<YeuCauTiepNhan> YeuCauTiepNhan
        {
            get => _yeuCauTiepNhans ?? (_yeuCauTiepNhans = new List<YeuCauTiepNhan>());
            protected set => _yeuCauTiepNhans = value;
        }

        private ICollection<KetQuaSinhHieu> _ketQuaSinhHieus;
        public virtual ICollection<KetQuaSinhHieu> KetQuaSinhHieus
        {
            get => _ketQuaSinhHieus ?? (_ketQuaSinhHieus = new List<KetQuaSinhHieu>());
            protected set => _ketQuaSinhHieus = value;
        }


        private ICollection<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuatNoiChiDinhs;
        public virtual ICollection<YeuCauDichVuKyThuat> YeuCauDichVuKyThuatNoiChiDinhs
        {
            get => _yeuCauDichVuKyThuatNoiChiDinhs ?? (_yeuCauDichVuKyThuatNoiChiDinhs = new List<YeuCauDichVuKyThuat>());
            protected set => _yeuCauDichVuKyThuatNoiChiDinhs = value;
        }

        //private ICollection<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuatNoiThanhToans;
        //public virtual ICollection<YeuCauDichVuKyThuat> YeuCauDichVuKyThuatNoiThanhToans
        //{
        //    get => _yeuCauDichVuKyThuatNoiThanhToans ?? (_yeuCauDichVuKyThuatNoiThanhToans = new List<YeuCauDichVuKyThuat>());
        //    protected set => _yeuCauDichVuKyThuatNoiThanhToans = value;
        //}

        private ICollection<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuatNoiThucHiens;
        public virtual ICollection<YeuCauDichVuKyThuat> YeuCauDichVuKyThuatNoiThucHiens
        {
            get => _yeuCauDichVuKyThuatNoiThucHiens ?? (_yeuCauDichVuKyThuatNoiThucHiens = new List<YeuCauDichVuKyThuat>());
            protected set => _yeuCauDichVuKyThuatNoiThucHiens = value;
        }




        private ICollection<YeuCauDuocPhamBenhVien> _yeuCauDuocPhamBenhVienNoiCapThuocs;
        public virtual ICollection<YeuCauDuocPhamBenhVien> YeuCauDuocPhamBenhVienNoiCapThuocs
        {
            get => _yeuCauDuocPhamBenhVienNoiCapThuocs ?? (_yeuCauDuocPhamBenhVienNoiCapThuocs = new List<YeuCauDuocPhamBenhVien>());
            protected set => _yeuCauDuocPhamBenhVienNoiCapThuocs = value;
        }


        private ICollection<YeuCauDuocPhamBenhVien> _yeuCauDuocPhamBenhVienNoiChiDinhs;
        public virtual ICollection<YeuCauDuocPhamBenhVien> YeuCauDuocPhamBenhVienNoiChiDinhs
        {
            get => _yeuCauDuocPhamBenhVienNoiChiDinhs ?? (_yeuCauDuocPhamBenhVienNoiChiDinhs = new List<YeuCauDuocPhamBenhVien>());
            protected set => _yeuCauDuocPhamBenhVienNoiChiDinhs = value;
        }


        //private ICollection<YeuCauDuocPhamBenhVien> _yeuCauDuocPhamBenhVienNoiThanhToan;
        //public virtual ICollection<YeuCauDuocPhamBenhVien> YeuCauDuocPhamBenhVienNoiThanhToans
        //{
        //    get => _yeuCauDuocPhamBenhVienNoiThanhToan ?? (_yeuCauDuocPhamBenhVienNoiThanhToan = new List<YeuCauDuocPhamBenhVien>());
        //    protected set => _yeuCauDuocPhamBenhVienNoiThanhToan = value;
        //}


        private ICollection<YeuCauKhamBenhDonThuoc> _yeuCauKhamBenhDonThuocNoiKeDons;
        public virtual ICollection<YeuCauKhamBenhDonThuoc> YeuCauKhamBenhDonThuocNoiKeDons
        {
            get => _yeuCauKhamBenhDonThuocNoiKeDons ?? (_yeuCauKhamBenhDonThuocNoiKeDons = new List<YeuCauKhamBenhDonThuoc>());
            protected set => _yeuCauKhamBenhDonThuocNoiKeDons = value;
        }


        private ICollection<YeuCauKhamBenh> _yeuCauKhamBenhNoiChiDinh;
        public virtual ICollection<YeuCauKhamBenh> YeuCauKhamBenhNoiChiDinhs
        {
            get => _yeuCauKhamBenhNoiChiDinh ?? (_yeuCauKhamBenhNoiChiDinh = new List<YeuCauKhamBenh>());
            protected set => _yeuCauKhamBenhNoiChiDinh = value;
        }


        private ICollection<YeuCauKhamBenh> _yeuCauKhamBenhNoiDangKys;
        public virtual ICollection<YeuCauKhamBenh> YeuCauKhamBenhNoiDangKys
        {
            get => _yeuCauKhamBenhNoiDangKys ?? (_yeuCauKhamBenhNoiDangKys = new List<YeuCauKhamBenh>());
            protected set => _yeuCauKhamBenhNoiDangKys = value;
        }


        private ICollection<YeuCauKhamBenh> _yeuCauKhamBenhNoiKetLuans;
        public virtual ICollection<YeuCauKhamBenh> YeuCauKhamBenhNoiKetLuans
        {
            get => _yeuCauKhamBenhNoiKetLuans ?? (_yeuCauKhamBenhNoiKetLuans = new List<YeuCauKhamBenh>());
            protected set => _yeuCauKhamBenhNoiKetLuans = value;
        }

        //private ICollection<YeuCauKhamBenh> _yeuCauKhamBenhNoiThanhToans;
        //public virtual ICollection<YeuCauKhamBenh> YeuCauKhamBenhNoiThanhToans
        //{
        //    get => _yeuCauKhamBenhNoiThanhToans ?? (_yeuCauKhamBenhNoiThanhToans = new List<YeuCauKhamBenh>());
        //    protected set => _yeuCauKhamBenhNoiThanhToans = value;
        //}

        private ICollection<YeuCauKhamBenh> _yeuCauKhamBenhNoiThucHiens;
        public virtual ICollection<YeuCauKhamBenh> YeuCauKhamBenhNoiThucHiens
        {
            get => _yeuCauKhamBenhNoiThucHiens ?? (_yeuCauKhamBenhNoiThucHiens = new List<YeuCauKhamBenh>());
            protected set => _yeuCauKhamBenhNoiThucHiens = value;
        }


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

        private ICollection<YeuCauDichVuGiuongBenhVien> _yeuCauDichVuGiuongBenhVienNoiChiDinhs;
        public virtual ICollection<YeuCauDichVuGiuongBenhVien> YeuCauDichVuGiuongBenhVienNoiChiDinhs
        {
            get => _yeuCauDichVuGiuongBenhVienNoiChiDinhs ?? (_yeuCauDichVuGiuongBenhVienNoiChiDinhs = new List<YeuCauDichVuGiuongBenhVien>());
            protected set => _yeuCauDichVuGiuongBenhVienNoiChiDinhs = value;

        }
        private ICollection<YeuCauDichVuGiuongBenhVien> _yeuCauDichVuGiuongBenhVienNoiThucHiens;
        public virtual ICollection<YeuCauDichVuGiuongBenhVien> YeuCauDichVuGiuongBenhVienNoiThucHiens
        {
            get => _yeuCauDichVuGiuongBenhVienNoiThucHiens ?? (_yeuCauDichVuGiuongBenhVienNoiThucHiens = new List<YeuCauDichVuGiuongBenhVien>());
            protected set => _yeuCauDichVuGiuongBenhVienNoiThucHiens = value;

        }
        //private ICollection<YeuCauDichVuGiuongBenhVien> _yeuCauDichVuGiuongBenhVienNoiThanhToans;
        //public virtual ICollection<YeuCauDichVuGiuongBenhVien> YeuCauDichVuGiuongBenhVienNoiThanhToans
        //{
        //    get => _yeuCauDichVuGiuongBenhVienNoiThanhToans ?? (_yeuCauDichVuGiuongBenhVienNoiThanhToans = new List<YeuCauDichVuGiuongBenhVien>());
        //    protected set => _yeuCauDichVuGiuongBenhVienNoiThanhToans = value;

        //}

        private ICollection<YeuCauGoiDichVu> _yeuCauGoiDichVuNoiChiDinhs;
        public virtual ICollection<YeuCauGoiDichVu> YeuCauGoiDichVuNoiChiDinhs
        {
            get => _yeuCauGoiDichVuNoiChiDinhs ?? (_yeuCauGoiDichVuNoiChiDinhs = new List<YeuCauGoiDichVu>());
            protected set => _yeuCauGoiDichVuNoiChiDinhs = value;

        }

        private ICollection<YeuCauGoiDichVu> _yeuCauGoiDichVuNoiQuyetToans;
        public virtual ICollection<YeuCauGoiDichVu> YeuCauGoiDichVuNoiQuyetToans
        {
            get => _yeuCauGoiDichVuNoiQuyetToans ?? (_yeuCauGoiDichVuNoiQuyetToans = new List<YeuCauGoiDichVu>());
            protected set => _yeuCauGoiDichVuNoiQuyetToans = value;

        }

        private ICollection<DonThuocThanhToan> _donThuocThanhToans;
        public virtual ICollection<DonThuocThanhToan> DonThuocThanhToans
        {
            get => _donThuocThanhToans ?? (_donThuocThanhToans = new List<DonThuocThanhToan>());
            protected set => _donThuocThanhToans = value;
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
        private ICollection<TaiKhoanBenhNhanThu> _taiKhoanBenhNhanThuNoiHuys;
        public virtual ICollection<TaiKhoanBenhNhanThu> TaiKhoanBenhNhanThuNoiHuys
        {
            get => _taiKhoanBenhNhanThuNoiHuys ?? (_taiKhoanBenhNhanThuNoiHuys = new List<TaiKhoanBenhNhanThu>());
            protected set => _taiKhoanBenhNhanThuNoiHuys = value;
        }
        private ICollection<TaiKhoanBenhNhanChi> _taiKhoanBenhNhanChis;
        public virtual ICollection<TaiKhoanBenhNhanChi> TaiKhoanBenhNhanChis
        {
            get => _taiKhoanBenhNhanChis ?? (_taiKhoanBenhNhanChis = new List<TaiKhoanBenhNhanChi>());
            protected set => _taiKhoanBenhNhanChis = value;
        }
        private ICollection<TaiKhoanBenhNhanChi> _taiKhoanBenhNhanChiNoiHuys;
        public virtual ICollection<TaiKhoanBenhNhanChi> TaiKhoanBenhNhanChiNoiHuys
        {
            get => _taiKhoanBenhNhanChiNoiHuys ?? (_taiKhoanBenhNhanChiNoiHuys = new List<TaiKhoanBenhNhanChi>());
            protected set => _taiKhoanBenhNhanChiNoiHuys = value;
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

        private ICollection<TaiKhoanBenhNhanHuyDichVu> _taiKhoanBenhNhanHuyDichVus;
        public virtual ICollection<TaiKhoanBenhNhanHuyDichVu> TaiKhoanBenhNhanHuyDichVus
        {
            get => _taiKhoanBenhNhanHuyDichVus ?? (_taiKhoanBenhNhanHuyDichVus = new List<TaiKhoanBenhNhanHuyDichVu>());
            protected set => _taiKhoanBenhNhanHuyDichVus = value;
        }

        private ICollection<GiuongBenh> _giuongBenhs { get; set; }
        public virtual ICollection<GiuongBenh> GiuongBenhs
        {
            get => _giuongBenhs ?? (_giuongBenhs = new List<GiuongBenh>());
            protected set => _giuongBenhs = value;
        }

        private ICollection<KhoaPhongNhanVien> _khoaPhongNhanViens;
        public virtual ICollection<KhoaPhongNhanVien> KhoaPhongNhanViens
        {
            get => _khoaPhongNhanViens ?? (_khoaPhongNhanViens = new List<KhoaPhongNhanVien>());
            protected set => _khoaPhongNhanViens = value;
        }

        private ICollection<DichVuKyThuatBenhVienNoiThucHienUuTien> _dichVuKyThuatBenhVienNoiThucHienUuTiens { get; set; }
        public virtual ICollection<DichVuKyThuatBenhVienNoiThucHienUuTien> DichVuKyThuatBenhVienNoiThucHienUuTiens
        {
            get => _dichVuKyThuatBenhVienNoiThucHienUuTiens ?? (_dichVuKyThuatBenhVienNoiThucHienUuTiens = new List<DichVuKyThuatBenhVienNoiThucHienUuTien>());
            protected set => _dichVuKyThuatBenhVienNoiThucHienUuTiens = value;
        }

        private ICollection<KhamTheoDoi> _khamTheoDois;
        public virtual ICollection<KhamTheoDoi> KhamTheoDois
        {
            get => _khamTheoDois ?? (_khamTheoDois = new List<KhamTheoDoi>());
            protected set => _khamTheoDois = value;
        }

        private ICollection<Kho> _khoDuocPhams;
        public virtual ICollection<Kho> KhoDuocPhams
        {
            get => _khoDuocPhams ?? (_khoDuocPhams = new List<Kho>());
            protected set => _khoDuocPhams = value;
        }
        private ICollection<YeuCauKhamBenhDonVTYT> _yeuCauKhamBenhDonVTYTNoiKeDons;
        public virtual ICollection<YeuCauKhamBenhDonVTYT> YeuCauKhamBenhDonVTYTNoiKeDons
        {
            get => _yeuCauKhamBenhDonVTYTNoiKeDons ?? (_yeuCauKhamBenhDonVTYTNoiKeDons = new List<YeuCauKhamBenhDonVTYT>());
            protected set => _yeuCauKhamBenhDonVTYTNoiKeDons = value;
        }

        private ICollection<DonVTYTThanhToan> _donVTYTThanhToans;
        public virtual ICollection<DonVTYTThanhToan> DonVTYTThanhToans
        {
            get => _donVTYTThanhToans ?? (_donVTYTThanhToans = new List<DonVTYTThanhToan>());
            protected set => _donVTYTThanhToans = value;
        }

        private ICollection<YeuCauLinhVatTu> _yeuCauLinhVatTus;
        public virtual ICollection<YeuCauLinhVatTu> YeuCauLinhVatTus
        {
            get => _yeuCauLinhVatTus ?? (_yeuCauLinhVatTus = new List<YeuCauLinhVatTu>());
            protected set => _yeuCauLinhVatTus = value;
        }

        private ICollection<YeuCauLinhDuocPham> _yeuCauLinhDuocPhams;
        public virtual ICollection<YeuCauLinhDuocPham> YeuCauLinhDuocPhams
        {
            get => _yeuCauLinhDuocPhams ?? (_yeuCauLinhDuocPhams = new List<YeuCauLinhDuocPham>());
            protected set => _yeuCauLinhDuocPhams = value;
        }

        private ICollection<PhienXetNghiem> _phienXetNghiems;
        public virtual ICollection<PhienXetNghiem> PhienXetNghiems
        {
            get => _phienXetNghiems ?? (_phienXetNghiems = new List<PhienXetNghiem>());
            protected set => _phienXetNghiems = value;
        }

        private ICollection<MauXetNghiem> _mauXetNghiems;
        public virtual ICollection<MauXetNghiem> MauXetNghiems
        {
            get => _mauXetNghiems ?? (_mauXetNghiems = new List<MauXetNghiem>());
            protected set => _mauXetNghiems = value;
        }

        private ICollection<PhieuGoiMauXetNghiem> _phieuGoiMauXetNghiems;
        public virtual ICollection<PhieuGoiMauXetNghiem> PhieuGoiMauXetNghiems
        {
            get => _phieuGoiMauXetNghiems ?? (_phieuGoiMauXetNghiems = new List<PhieuGoiMauXetNghiem>());
            protected set => _phieuGoiMauXetNghiems = value;
        }

        private ICollection<NoiTruHoSoKhac> _noiTruHoSoKhacs;
        public virtual ICollection<NoiTruHoSoKhac> NoiTruHoSoKhacs
        {
            get => _noiTruHoSoKhacs ?? (_noiTruHoSoKhacs = new List<NoiTruHoSoKhac>());
            protected set => _noiTruHoSoKhacs = value;
        }

        private ICollection<YeuCauNhapVien> _yeuCauNhapViens;
        public virtual ICollection<YeuCauNhapVien> YeuCauNhapViens
        {
            get => _yeuCauNhapViens ?? (_yeuCauNhapViens = new List<YeuCauNhapVien>());
            protected set => _yeuCauNhapViens = value;
        }

        private ICollection<YeuCauTruyenMau> _noiChiDinhYeuCauTruyenMaus;
        public virtual ICollection<YeuCauTruyenMau> NoiChiDinhYeuCauTruyenMaus
        {
            get => _noiChiDinhYeuCauTruyenMaus ?? (_noiChiDinhYeuCauTruyenMaus = new List<YeuCauTruyenMau>());
            protected set => _noiChiDinhYeuCauTruyenMaus = value;
        }

        private ICollection<YeuCauTruyenMau> _noiThucHienYeuCauTruyenMaus;
        public virtual ICollection<YeuCauTruyenMau> NoiThucHienYeuCauTruyenMaus
        {
            get => _noiThucHienYeuCauTruyenMaus ?? (_noiThucHienYeuCauTruyenMaus = new List<YeuCauTruyenMau>());
            protected set => _noiThucHienYeuCauTruyenMaus = value;
        }

        private ICollection<NoiTruPhieuDieuTriChiTietYLenh> _noiTruPhieuDieuTriChiTietYLenhs;
        public virtual ICollection<NoiTruPhieuDieuTriChiTietYLenh> NoiTruPhieuDieuTriChiTietYLenhs
        {
            get => _noiTruPhieuDieuTriChiTietYLenhs ?? (_noiTruPhieuDieuTriChiTietYLenhs = new List<NoiTruPhieuDieuTriChiTietYLenh>());
            protected set => _noiTruPhieuDieuTriChiTietYLenhs = value;
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

        private ICollection<GoiKhamSucKhoeNoiThucHien> _goiKhamSucKhoeNoiThucHiens;
        public virtual ICollection<GoiKhamSucKhoeNoiThucHien> GoiKhamSucKhoeNoiThucHiens
        {
            get => _goiKhamSucKhoeNoiThucHiens ?? (_goiKhamSucKhoeNoiThucHiens = new List<GoiKhamSucKhoeNoiThucHien>());
            protected set => _goiKhamSucKhoeNoiThucHiens = value;
        }

        private ICollection<GoiKhamSucKhoeChungNoiThucHien> _goiKhamSucKhoeChungNoiThucHiens;
        public virtual ICollection<GoiKhamSucKhoeChungNoiThucHien> GoiKhamSucKhoeChungNoiThucHiens
        {
            get => _goiKhamSucKhoeChungNoiThucHiens ?? (_goiKhamSucKhoeChungNoiThucHiens = new List<GoiKhamSucKhoeChungNoiThucHien>());
            protected set => _goiKhamSucKhoeChungNoiThucHiens = value;
        }

        private ICollection<NoiTruPhieuDieuTriTuVanThuoc> _noiTruPhieuDieuTriTuVanThuocs;
        public virtual ICollection<NoiTruPhieuDieuTriTuVanThuoc> NoiTruPhieuDieuTriTuVanThuocs
        {
            get => _noiTruPhieuDieuTriTuVanThuocs ?? (_noiTruPhieuDieuTriTuVanThuocs = new List<NoiTruPhieuDieuTriTuVanThuoc>());
            protected set => _noiTruPhieuDieuTriTuVanThuocs = value;
        }

        private ICollection<MauXetNghiem> _mauXetNghiemPhongNhanMaus;
        public virtual ICollection<MauXetNghiem> MauXetNghiemPhongNhanMaus
        {
            get => _mauXetNghiemPhongNhanMaus ?? (_mauXetNghiemPhongNhanMaus = new List<MauXetNghiem>());
            protected set => _mauXetNghiemPhongNhanMaus = value;
        }

        private ICollection<PhienXetNghiemChiTiet> _phienXetNghiemChiTietPhongLayMaus;
        public virtual ICollection<PhienXetNghiemChiTiet> PhienXetNghiemChiTietPhongLayMaus
        {
            get => _phienXetNghiemChiTietPhongLayMaus ?? (_phienXetNghiemChiTietPhongLayMaus = new List<PhienXetNghiemChiTiet>());
            protected set => _phienXetNghiemChiTietPhongLayMaus = value;
        }

        private ICollection<PhienXetNghiemChiTiet> _phienXetNghiemChiTietPhongNhanMaus;
        public virtual ICollection<PhienXetNghiemChiTiet> PhienXetNghiemChiTietPhongNhanMaus
        {
            get => _phienXetNghiemChiTietPhongNhanMaus ?? (_phienXetNghiemChiTietPhongNhanMaus = new List<PhienXetNghiemChiTiet>());
            protected set => _phienXetNghiemChiTietPhongNhanMaus = value;
        }

        private ICollection<NoiTruDonThuoc> _noiTruDonThuocNoiKeDons;
        public virtual ICollection<NoiTruDonThuoc> NoiTruDonThuocNoiKeDons
        {
            get => _noiTruDonThuocNoiKeDons ?? (_noiTruDonThuocNoiKeDons = new List<NoiTruDonThuoc>());
            protected set => _noiTruDonThuocNoiKeDons = value;
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
        //private ICollection<YeuCauDichVuKyThuatKhamSangLocTiemChung> _yeuCauDichVuKyThuatKhamSangLocTiemChungNoiTheoDoiSauTiems;
        //public virtual ICollection<YeuCauDichVuKyThuatKhamSangLocTiemChung> YeuCauDichVuKyThuatKhamSangLocTiemChungNoiTheoDoiSauTiems
        //{
        //    get => _yeuCauDichVuKyThuatKhamSangLocTiemChungNoiTheoDoiSauTiems ?? (_yeuCauDichVuKyThuatKhamSangLocTiemChungNoiTheoDoiSauTiems = new List<YeuCauDichVuKyThuatKhamSangLocTiemChung>());
        //    protected set => _yeuCauDichVuKyThuatKhamSangLocTiemChungNoiTheoDoiSauTiems = value;
        //}

        private ICollection<ThuePhong> _thuePhongs;
        public virtual ICollection<ThuePhong> ThuePhongs
        {
            get => _thuePhongs ?? (_thuePhongs = new List<ThuePhong>());
            protected set => _thuePhongs = value;
        }
    }
}
