using System;
using System.Collections.Generic;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.CongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.DonVatTus;
using Camino.Core.Domain.Entities.DuyetBaoHiems;
using Camino.Core.Domain.Entities.GiuongBenhs;
using Camino.Core.Domain.Entities.ICDs;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.YeuCauNhapViens;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.Entities.YeuCauTiepNhanTheBHYTs;
using Camino.Core.Domain.Entities.NoiDungGhiChuMiemGiams;

namespace Camino.Core.Domain.Entities.YeuCauKhamBenhs
{
    public class YeuCauKhamBenh : BaseEntity
    {
        public long YeuCauTiepNhanId { get; set; }
        public long DichVuKhamBenhBenhVienId { get; set; }
        public long NhomGiaDichVuKhamBenhBenhVienId { get; set; }
        public long? YeuCauKhamBenhTruocId { get; set; }
        public string LoiDanCuaBacSi { get; set; }
        public string MaDichVu { get; set; }
        public string MaDichVuTT37 { get; set; }
        public string TenDichVu { get; set; }
        public decimal Gia { get; set; }
        public decimal? DonGiaTruocChietKhau { get; set; }
        public decimal? DonGiaSauChietKhau { get; set; }
        public int? TiLeUuDai { get; set; }
        public string MoTa { get; set; }
        public bool DuocHuongBaoHiem { get; set; }
        public bool? BaoHiemChiTra { get; set; }
        public DateTime? ThoiDiemDuyetBaoHiem { get; set; }
        public long? NhanVienDuyetBaoHiemId { get; set; }
        public Enums.EnumTrangThaiYeuCauKhamBenh TrangThai { get; set; }
        public long NhanVienChiDinhId { get; set; }
        public long? NoiChiDinhId { get; set; }
        public DateTime ThoiDiemChiDinh { get; set; }
        public DateTime ThoiDiemDangKy { get; set; }
        public long? NoiDangKyId { get; set; }
        public long? BacSiDangKyId { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public long? NoiThucHienId { get; set; }
        public long? BacSiThucHienId { get; set; }
        public string BenhSu { get; set; }
        public string ThongTinKhamTheoKhoa { get; set; }
        public long? IcdchinhId { get; set; }
        public string TenBenh { get; set; }
        public long? NoiKetLuanId { get; set; }
        public long? BacSiKetLuanId { get; set; }
        public DateTime? ThoiDiemHoanThanh { get; set; }
        public Enums.EnumKetQuaDieuTri? KetQuaDieuTri { get; set; }
        public Enums.EnumTinhTrangRaVien? TinhTrangRaVien { get; set; }
        public string HuongDieuTri { get; set; }
        public bool? CoKhamChuyenKhoaTiepTheo { get; set; }
        public bool? CoKeToa { get; set; }
        public bool? CoTaiKham { get; set; }
        public DateTime? NgayTaiKham { get; set; }
        public string GhiChuTaiKham { get; set; }
        public bool? QuayLaiYeuCauKhamBenhTruoc { get; set; }
        //public long NhomGiaDichVuKhamBenhBenhVienId { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }
        public decimal? SoTienBaoHiemTuNhanChiTra { get; set; }
        public decimal? SoTienMienGiam { get; set; }
        public string GhiChuMienGiamThem { get; set; }
        public decimal? SoTienBenhNhanDaChi { get; set; }
        public decimal? DonGiaBaoHiem { get; set; }
        public int? MucHuongBaoHiem { get; set; }
        public int? TiLeBaoHiemThanhToan { get; set; }
        public Enums.TrangThaiThanhToan TrangThaiThanhToan { get; set; }
        public long? NhanVienHuyThanhToanId { get; set; }
        public string LyDoHuyThanhToan { get; set; }
        public bool? KhongTinhPhi { get; set; }
        //Update 30/03/2020
        public string GhiChuTrieuChungLamSang { get; set; }
        public string KhamToanThan { get; set; }
        public long? ChanDoanSoBoICDId { get; set; }
        public string ChanDoanSoBoGhiChu { get; set; }
        /// <summary>
        /// Update 02/06/2020
        /// </summary>
        public bool? CoNhapVien { get; set; }
        public long? KhoaPhongNhapVienId { get; set; }
        public string LyDoNhapVien { get; set; }
        public bool? CoChuyenVien { get; set; }
        public long? BenhVienChuyenVienId { get; set; }
        public string LyDoChuyenVien { get; set; }
        public bool? CoTuVong { get; set; }
        public string ThongTinKhamTheoDichVuTemplate { get; set; }
        public string ThongTinKhamTheoDichVuData { get; set; }
        //update 11/6
        public string TomTatKetQuaCLS { get; set; }
        public string GhiChuICDChinh { get; set; }
        public string CachGiaiQuyet { get; set; }
        public bool? LaThamVan { get; set; }
        public bool? CoDieuTriNgoaiTru { get; set; }
        public string ChanDoanCuaNoiGioiThieu { get; set; }

        /// <summary>
        /// update 07/07/2020
        /// </summary>
        /// 
        public string TinhTrangBenhNhanChuyenVien { get; set; }
        public Enums.LyDoChuyenTuyen? LoaiLyDoChuyenVien { get; set; }
        public DateTime? ThoiDiemChuyenVien { get; set; }
        public string PhuongTienChuyenVien { get; set; }
        public long? NhanVienHoTongChuyenVienId { get; set; }

        /// <summary>
        /// Update 19/08/2020
        /// </summary>
        /// 
        public bool? KhongKeToa { get; set; }
        public long? YeuCauTiepNhanTheBHYTId { get; set; }
        public string MaSoTheBHYT { get; set; }

        public long? GoiKhamSucKhoeId { get; set; }
        public Enums.ChuyenKhoaKhamSucKhoe? ChuyenKhoaKhamSucKhoe { get; set; }
        public int? SoThuTuKhamSucKhoe { get; set; }
        public decimal? DonGiaUuDai { get; set; }
        public decimal? DonGiaChuaUuDai { get; set; }

        public string GhiChuCanLamSang { get; set; }

        public string LyDoHuyDichVu { get; set; }
        public long? NhanVienHuyDichVuId { get; set; }

        //BVHD-3574
        public string NoiDungKhamBenh { get; set; }
        // BVHD-3698
        public DateTime? DuongThaiTuNgay { get; set; }
        public DateTime? DuongThaiDenNgay { get; set; }

        //BVHD-3668
        public long? GoiKhamSucKhoeDichVuPhatSinhId { get; set; }
        public decimal? GiaBenhVienTaiThoiDiemChiDinh { get; set; }

        //BVHD-3706
        public string TrieuChungTiepNhan { get; set; }

        /// <summary>
        /// Update 15/12/2021
        /// </summary>
        /// 
        public DateTime? NghiHuongBHXHTuNgay { get; set; }
        public DateTime? NghiHuongBHXHDenNgay { get; set; }
        public long? NghiHuongBHXHNguoiInId { get; set; }
        public DateTime? NghiHuongBHXHNgayIn { get; set; }
        public long? ICDChinhNghiHuongBHYT { get; set; }
        public string TenICDChinhNghiHuongBHYT { get; set; }
        public string PhuongPhapDieuTriNghiHuongBHYT { get; set; }

        /// <summary>
        /// Update 27/12/2021
        /// </summary>
        /// 
        public long? DuongThaiNguoiInId { get; set; }
        public DateTime? DuongThaiNgayIn { get; set; }

        /// <summary>
        /// Update 13/01/2021
        /// </summary>

        public string KetQuaXetNghiemCLS { get; set; }
        public string PhuongPhapTrongDieuTri { get; set; }

        //BVHD-3575
        public bool? LaChiDinhTuNoiTru { get; set; }

        //Cập nhật STT chuyển viện
        public long? STTChuyenVien { get; set; }
        public DateTime? ThoiGianDienBien { get; set; }

        public virtual YeuCauTiepNhanTheBHYT YeuCauTiepNhanTheBHYT { get; set; }
        public virtual NhanVien NhanVienHoTongChuyenVien { get; set; }
        public virtual NhanVien NghiHuongBHXHNguoiIn { get; set; }

        public virtual BenhVien.BenhVien BenhVienChuyenVien { get; set; }
        public virtual KhoaPhongs.KhoaPhong KhoaPhongNhapVien { get; set; }

        public virtual ICD ChanDoanSoBoICD { get; set; }
        public virtual NhanVien NhanVienHuyThanhToan { get; set; }

        public virtual NhomGiaDichVuKhamBenhBenhVien NhomGiaDichVuKhamBenhBenhVien { get; set; }

        public virtual NhanVien BacSiDangKy { get; set; }
        public virtual NhanVien BacSiKetLuan { get; set; }
        public virtual NhanVien BacSiThucHien { get; set; }
        public virtual DichVuKhamBenhBenhVien DichVuKhamBenhBenhVien { get; set; }
        //public virtual GoiDichVu GoiDichVu { get; set; }
        public virtual ICD Icdchinh { get; set; }
        public virtual NhanVien NhanVienChiDinh { get; set; }
        public virtual NhanVien NhanVienDuyetBaoHiem { get; set; }
        //public virtual NhanVien NhanVienThanhToan { get; set; }
        public virtual PhongBenhVien NoiChiDinh { get; set; }
        public virtual PhongBenhVien NoiDangKy { get; set; }
        public virtual PhongBenhVien NoiKetLuan { get; set; }
        //public virtual PhongBenhVien NoiThanhToan { get; set; }
        public virtual PhongBenhVien NoiThucHien { get; set; }
        public virtual YeuCauKhamBenh YeuCauKhamBenhTruoc { get; set; }
        public virtual YeuCauTiepNhan YeuCauTiepNhan { get; set; }
        public virtual YeuCauGoiDichVu YeuCauGoiDichVu { get; set; }
        public virtual GoiKhamSucKhoe GoiKhamSucKhoe { get; set; }
        public virtual NhanVien NhanVienHuyDichVu { get; set; }

        //BVHD-3668
        public virtual GoiKhamSucKhoe GoiKhamSucKhoeDichVuPhatSinh { get; set; }

        //BVHD-3731
        public long? NoiDungGhiChuMiemGiamId { get; set; }
        public virtual NoiDungGhiChuMiemGiam NoiDungGhiChuMiemGiam { get; set; }

        private ICollection<YeuCauKhamBenh> _inverseYeuCauKhamBenhTruocs { get; set; }
        public virtual ICollection<YeuCauKhamBenh> InverseYeuCauKhamBenhTruocs
        {
            get => _inverseYeuCauKhamBenhTruocs ?? (_inverseYeuCauKhamBenhTruocs = new List<YeuCauKhamBenh>());
            protected set => _inverseYeuCauKhamBenhTruocs = value;
        }

        private ICollection<YeuCauKhamBenhChuanDoan> _yeuCauKhamBenhChuanDoans;
        public virtual ICollection<YeuCauKhamBenhChuanDoan> YeuCauKhamBenhChuanDoans
        {
            get => _yeuCauKhamBenhChuanDoans ?? (_yeuCauKhamBenhChuanDoans = new List<YeuCauKhamBenhChuanDoan>());
            protected set => _yeuCauKhamBenhChuanDoans = value;
        }

        private ICollection<YeuCauKhamBenhTrieuChung> _yeuCauKhamBenhTrieuChung;
        public virtual ICollection<YeuCauKhamBenhTrieuChung> YeuCauKhamBenhTrieuChungs
        {
            get => _yeuCauKhamBenhTrieuChung ?? (_yeuCauKhamBenhTrieuChung = new List<YeuCauKhamBenhTrieuChung>());
            protected set => _yeuCauKhamBenhTrieuChung = value;
        }

        private ICollection<YeuCauVatTuBenhVien> _yeuCauVatTuBenhViens;
        public virtual ICollection<YeuCauVatTuBenhVien> YeuCauVatTuBenhViens
        {
            get => _yeuCauVatTuBenhViens ?? (_yeuCauVatTuBenhViens = new List<YeuCauVatTuBenhVien>());
            protected set => _yeuCauVatTuBenhViens = value;
        }

        private ICollection<YeuCauKhamBenhICDKhac> _yeuCauKhamBenhICDKhacs;
        public virtual ICollection<YeuCauKhamBenhICDKhac> YeuCauKhamBenhICDKhacs
        {
            get => _yeuCauKhamBenhICDKhacs ?? (_yeuCauKhamBenhICDKhacs = new List<YeuCauKhamBenhICDKhac>());
            protected set => _yeuCauKhamBenhICDKhacs = value;
        }

        private ICollection<YeuCauKhamBenhDonThuoc> _yeuCauKhamBenhDonThuocs;
        public virtual ICollection<YeuCauKhamBenhDonThuoc> YeuCauKhamBenhDonThuocs
        {
            get => _yeuCauKhamBenhDonThuocs ?? (_yeuCauKhamBenhDonThuocs = new List<YeuCauKhamBenhDonThuoc>());
            protected set => _yeuCauKhamBenhDonThuocs = value;
        }

        private ICollection<YeuCauKhamBenhDonVTYT> _yeuCauKhamBenhDonVTYTs;
        public virtual ICollection<YeuCauKhamBenhDonVTYT> YeuCauKhamBenhDonVTYTs
        {
            get => _yeuCauKhamBenhDonVTYTs ?? (_yeuCauKhamBenhDonVTYTs = new List<YeuCauKhamBenhDonVTYT>());
            protected set => _yeuCauKhamBenhDonVTYTs = value;
        }

        private ICollection<YeuCauDuocPhamBenhVien> _yeuCauDuocPhamBenhViens;
        public virtual ICollection<YeuCauDuocPhamBenhVien> YeuCauDuocPhamBenhViens
        {
            get => _yeuCauDuocPhamBenhViens ?? (_yeuCauDuocPhamBenhViens = new List<YeuCauDuocPhamBenhVien>());
            protected set => _yeuCauDuocPhamBenhViens = value;
        }

        private ICollection<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuats;
        public virtual ICollection<YeuCauDichVuKyThuat> YeuCauDichVuKyThuats
        {
            get => _yeuCauDichVuKyThuats ?? (_yeuCauDichVuKyThuats = new List<YeuCauDichVuKyThuat>());
            protected set => _yeuCauDichVuKyThuats = value;
        }

        private ICollection<PhongBenhVienHangDoi> _phongBenhVienHangDois { get; set; }
        public virtual ICollection<PhongBenhVienHangDoi> PhongBenhVienHangDois
        {
            get => _phongBenhVienHangDois ?? (_phongBenhVienHangDois = new List<PhongBenhVienHangDoi>());
            protected set => _phongBenhVienHangDois = value;
        }

        private ICollection<YeuCauKhamBenhLichSuTrangThai> _yeuCauKhamBenhLichSuTrangThais { get; set; }
        public virtual ICollection<YeuCauKhamBenhLichSuTrangThai> YeuCauKhamBenhLichSuTrangThais
        {
            get => _yeuCauKhamBenhLichSuTrangThais ?? (_yeuCauKhamBenhLichSuTrangThais = new List<YeuCauKhamBenhLichSuTrangThai>());
            protected set => _yeuCauKhamBenhLichSuTrangThais = value;
        }

        private ICollection<YeuCauDichVuGiuongBenhVien> _yeuCauDichVuGiuongBenhViens;
        public virtual ICollection<YeuCauDichVuGiuongBenhVien> YeuCauDichVuGiuongBenhViens
        {
            get => _yeuCauDichVuGiuongBenhViens ?? (_yeuCauDichVuGiuongBenhViens = new List<YeuCauDichVuGiuongBenhVien>());
            protected set => _yeuCauDichVuGiuongBenhViens = value;

        }

        //private ICollection<YeuCauGoiDichVu> _yeuCauGoiDichVus;
        //public virtual ICollection<YeuCauGoiDichVu> YeuCauGoiDichVus
        //{
        //    get => _yeuCauGoiDichVus ?? (_yeuCauGoiDichVus = new List<YeuCauGoiDichVu>());
        //    protected set => _yeuCauGoiDichVus = value;

        //}

        private ICollection<DonThuocThanhToan> _donThuocThanhToans;
        public virtual ICollection<DonThuocThanhToan> DonThuocThanhToans
        {
            get => _donThuocThanhToans ?? (_donThuocThanhToans = new List<DonThuocThanhToan>());
            protected set => _donThuocThanhToans = value;
        }

        private ICollection<DuyetBaoHiemChiTiet> _duyetBaoHiemChiTiets;
        public virtual ICollection<DuyetBaoHiemChiTiet> DuyetBaoHiemChiTiets
        {
            get => _duyetBaoHiemChiTiets ?? (_duyetBaoHiemChiTiets = new List<DuyetBaoHiemChiTiet>());
            protected set => _duyetBaoHiemChiTiets = value;
        }

        private ICollection<TaiKhoanBenhNhanThu> _taiKhoanBenhNhanThus;
        public virtual ICollection<TaiKhoanBenhNhanThu> TaiKhoanBenhNhanThus
        {
            get => _taiKhoanBenhNhanThus ?? (_taiKhoanBenhNhanThus = new List<TaiKhoanBenhNhanThu>());
            protected set => _taiKhoanBenhNhanThus = value;
        }
        private ICollection<TaiKhoanBenhNhanChi> _taiKhoanBenhNhanChis;
        public virtual ICollection<TaiKhoanBenhNhanChi> TaiKhoanBenhNhanChis
        {
            get => _taiKhoanBenhNhanChis ?? (_taiKhoanBenhNhanChis = new List<TaiKhoanBenhNhanChi>());
            protected set => _taiKhoanBenhNhanChis = value;
        }
        private ICollection<CongTyBaoHiemTuNhanCongNo> _baoHiemTuNhanCongNos;
        public virtual ICollection<CongTyBaoHiemTuNhanCongNo> CongTyBaoHiemTuNhanCongNos
        {
            get => _baoHiemTuNhanCongNos ?? (_baoHiemTuNhanCongNos = new List<CongTyBaoHiemTuNhanCongNo>());
            protected set => _baoHiemTuNhanCongNos = value;
        }

        private ICollection<MienGiamChiPhi> _mienGiamChiPhis;
        public virtual ICollection<MienGiamChiPhi> MienGiamChiPhis
        {
            get => _mienGiamChiPhis ?? (_mienGiamChiPhis = new List<MienGiamChiPhi>());
            protected set => _mienGiamChiPhis = value;
        }

        private ICollection<YeuCauKhamBenhKhamBoPhanKhac> _yeuCauKhamBenhKhamBoPhanKhacs;
        public virtual ICollection<YeuCauKhamBenhKhamBoPhanKhac> YeuCauKhamBenhKhamBoPhanKhacs
        {
            get => _yeuCauKhamBenhKhamBoPhanKhacs ?? (_yeuCauKhamBenhKhamBoPhanKhacs = new List<YeuCauKhamBenhKhamBoPhanKhac>());
            protected set => _yeuCauKhamBenhKhamBoPhanKhacs = value;
        }

        private ICollection<YeuCauKhamBenhChanDoanPhanBiet> _yeuCauKhamBenhChanDoanPhanBiets;
        public virtual ICollection<YeuCauKhamBenhChanDoanPhanBiet> YeuCauKhamBenhChanDoanPhanBiets
        {
            get => _yeuCauKhamBenhChanDoanPhanBiets ?? (_yeuCauKhamBenhChanDoanPhanBiets = new List<YeuCauKhamBenhChanDoanPhanBiet>());
            protected set => _yeuCauKhamBenhChanDoanPhanBiets = value;
        }

        private ICollection<YeuCauKhamBenhBoPhanTonThuong> _yeuCauKhamBenhBoPhanTonThuongs;
        public virtual ICollection<YeuCauKhamBenhBoPhanTonThuong> YeuCauKhamBenhBoPhanTonThuongs
        {
            get => _yeuCauKhamBenhBoPhanTonThuongs ?? (_yeuCauKhamBenhBoPhanTonThuongs = new List<YeuCauKhamBenhBoPhanTonThuong>());
            protected set => _yeuCauKhamBenhBoPhanTonThuongs = value;
        }

        private ICollection<HoatDongGiuongBenh> _hoatDongGiuongBenhs;
        public virtual ICollection<HoatDongGiuongBenh> HoatDongGiuongBenhs
        {
            get => _hoatDongGiuongBenhs ?? (_hoatDongGiuongBenhs = new List<HoatDongGiuongBenh>());
            protected set => _hoatDongGiuongBenhs = value;
        }

        private ICollection<DonVTYTThanhToan> _donVTYTThanhToans;
        public virtual ICollection<DonVTYTThanhToan> DonVTYTThanhToans
        {
            get => _donVTYTThanhToans ?? (_donVTYTThanhToans = new List<DonVTYTThanhToan>());
            protected set => _donVTYTThanhToans = value;
        }

        private ICollection<YeuCauNhapVien> _yeuCauNhapViens;
        public virtual ICollection<YeuCauNhapVien> YeuCauNhapViens
        {
            get => _yeuCauNhapViens ?? (_yeuCauNhapViens = new List<YeuCauNhapVien>());
            protected set => _yeuCauNhapViens = value;
        }

        //BVHD-3575
        private ICollection<NoiTruPhieuDieuTriChiTietYLenh> _noiTruPhieuDieuTriChiTietYLenhs;
        public virtual ICollection<NoiTruPhieuDieuTriChiTietYLenh> NoiTruPhieuDieuTriChiTietYLenhs
        {
            get => _noiTruPhieuDieuTriChiTietYLenhs ?? (_noiTruPhieuDieuTriChiTietYLenhs = new List<NoiTruPhieuDieuTriChiTietYLenh>());
            protected set => _noiTruPhieuDieuTriChiTietYLenhs = value;
        }
    }
}
