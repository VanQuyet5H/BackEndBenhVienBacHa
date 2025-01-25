using Camino.Core.Domain.Entities.ChiSoXetNghiems;
using Camino.Core.Domain.Entities.ChuanDoanHinhAnhs;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Camino.Core.Domain.Entities.GoiDichVus;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using System;
using System.Collections.Generic;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.CongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.DuyetBaoHiems;
using Camino.Core.Domain.Entities.FileKetQuaCanLamSangs;
using Camino.Core.Domain.Entities.GiuongBenhs;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.Entities.MayXetNghiems;
using Camino.Core.Domain.Entities.PhauThuatThuThuats;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.Entities.YeuCauTiepNhanTheBHYTs;
using Camino.Core.Domain.Entities.NoiDungGhiChuMiemGiams;

namespace Camino.Core.Domain.Entities.YeuCauKhamBenhs
{
    public class YeuCauDichVuKyThuat : BaseEntity
    {
        public long YeuCauTiepNhanId { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public Domain.Enums.LoaiDichVuKyThuat LoaiDichVuKyThuat { get; set; }
        public long DichVuKyThuatBenhVienId { get; set; }
        public long NhomDichVuBenhVienId { get; set; }
        public long NhomGiaDichVuKyThuatBenhVienId { get; set; }
        public string MaDichVu { get; set; }
        public string MaGiaDichVu { get; set; }
        public string Ma4350DichVu { get; set; }
        public string TenDichVu { get; set; }
        public string TenTiengAnhDichVu { get; set; }
        public string TenGiaDichVu { get; set; }
        public Enums.EnumDanhMucNhomTheoChiPhi NhomChiPhi { get; set; }
        public Domain.Enums.LoaiPhauThuatThuThuat? LoaiPhauThuatThuThuat { get; set; }
        public decimal Gia { get; set; }
        public decimal? DonGiaTruocChietKhau { get; set; }
        public decimal? DonGiaSauChietKhau { get; set; }
        public int? TiLeUuDai { get; set; }
        public int SoLan { get; set; }
        public bool DuocHuongBaoHiem { get; set; }
        public bool? BaoHiemChiTra { get; set; }
        public DateTime? ThoiDiemDuyetBaoHiem { get; set; }
        public long? NhanVienDuyetBaoHiemId { get; set; }
        public string ThongTu { get; set; }
        public string QuyetDinh { get; set; }
        public string NoiBanHanh { get; set; }
        public string MoTa { get; set; }
        public Domain.Enums.EnumTrangThaiYeuCauDichVuKyThuat TrangThai { get; set; }
        public long NhanVienChiDinhId { get; set; }
        public long? NoiChiDinhId { get; set; }
        public DateTime ThoiDiemChiDinh { get; set; }
        public DateTime ThoiDiemDangKy { get; set; }
        public long? NoiThucHienId { get; set; }
        public string MaGiuong { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public DateTime? ThoiDiemHoanThanh { get; set; }
        public long? NhanVienThucHienId { get; set; }
        public string KetLuan { get; set; }
        public long? NhanVienKetLuanId { get; set; }
        public DateTime? ThoiDiemKetLuan { get; set; }
        public string DeNghi { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }
        public decimal? SoTienBaoHiemTuNhanChiTra { get; set; }
        public decimal? SoTienMienGiam { get; set; }
        public string GhiChuMienGiamThem { get; set; }
        public decimal? SoTienBenhNhanDaChi { get; set; }
        public Enums.TrangThaiThanhToan TrangThaiThanhToan { get; set; }

        public decimal? DonGiaBaoHiem { get; set; }

        public int? MucHuongBaoHiem { get; set; }

        public int? TiLeBaoHiemThanhToan { get; set; }

        public string LyDoHuyThanhToan { get; set; }

        public long? NhanVienHuyThanhToanId { get; set; }

        public string BenhPhamXetNghiem { get; set; }

        public virtual NhanVien NhanVienHuyThanhToan { get; set; }
        public string DataKetQuaCanLamSang { get; set; }
        //public long? FileKetQuaCanLamSangId { get; set; }

        public int? LanThucHien { get; set; }

        public bool? DieuTriNgoaiTru { get; set; }
        public DateTime? ThoiDiemBatDauDieuTri { get; set; }
        public long? TheoDoiSauPhauThuatThuThuatId { get; set; }
        public long? NoiTruPhieuDieuTriId { get; set; }
        public Enums.DoiTuongSuDung? DoiTuongSuDung { get; set; }
        public long? MayTraKetQuaId { get; set; }
        public long? YeuCauTiepNhanTheBHYTId { get; set; }
        public string MaSoTheBHYT { get; set; }

        public long? GoiKhamSucKhoeId { get; set; }
        public int? SoThuTuKhamSucKhoe { get; set; }
        public decimal? DonGiaUuDai { get; set; }
        public decimal? DonGiaChuaUuDai { get; set; }
        public bool? KhongTinhPhi { get; set; }

        public string LyDoHuyDichVu { get; set; }
        public long? NhanVienHuyDichVuId { get; set; }

        public string LoaiKitThu { get; set; } //3761

        // dùng cho màn hình kết quả CĐHA-TDCN
        public long? NhanVienThucHien2Id { get; set; }
        public long? NhanVienKetLuan2Id { get; set; }
        public string GhiChuKetQuaCLS { get; set; }
        //============================================

        // dùng cho chức năng hủy trạng thái thực hiện dịch vụ
        public string LyDoHuyTrangThaiDaThucHien { get; set; }
        public long? NhanVienHuyTrangThaiDaThucHienId { get; set; }
        //=====================================================

        // cập nhật 09/07/2021: hỗ trợ data cho bên báo cáo
        public decimal? GiaBenhVienTaiThoiDiemChiDinh { get; set; }
        //=====================================================

        public long? YeuCauDichVuKyThuatKhamSangLocTiemChungId { get; set; }


        //BVHD-3668
        public long? GoiKhamSucKhoeDichVuPhatSinhId { get; set; }
        public DateTime? ThoiGianDienBien { get; set; }

        public virtual YeuCauTiepNhanTheBHYT YeuCauTiepNhanTheBHYT { get; set; }
        public virtual TheoDoiSauPhauThuatThuThuat TheoDoiSauPhauThuatThuThuat { get; set; }
        public virtual NhomGiaDichVuKyThuatBenhVien NhomGiaDichVuKyThuatBenhVien { get; set; }
        public virtual DichVuKyThuatBenhVien DichVuKyThuatBenhVien { get; set; }
        public virtual NhomDichVuBenhVien.NhomDichVuBenhVien NhomDichVuBenhVien { get; set; }
        //public virtual GoiDichVu GoiDichVu { get; set; }
        public virtual NhanVien NhanVienChiDinh { get; set; }
        public virtual NhanVien NhanVienDuyetBaoHiem { get; set; }
        public virtual NhanVien NhanVienKetLuan { get; set; }
        //public virtual NhanVien NhanVienThanhToan { get; set; }
        public virtual NhanVien NhanVienThucHien { get; set; }
        public virtual PhongBenhVien NoiChiDinh { get; set; }
        //public virtual PhongBenhVien NoiThanhToan { get; set; }
        public virtual PhongBenhVien NoiThucHien { get; set; }
        public virtual YeuCauKhamBenh YeuCauKhamBenh { get; set; }
        public virtual YeuCauTiepNhan YeuCauTiepNhan { get; set; }
        public virtual YeuCauGoiDichVu YeuCauGoiDichVu { get; set; }
        //public virtual FileKetQuaCanLamSang FileKetQuaCanLamSang { get; set; }
        public virtual NoiTruPhieuDieuTri NoiTruPhieuDieuTri { get; set; }
        public virtual MayXetNghiem MayTraKetQua { get; set; }
        public virtual GoiKhamSucKhoe GoiKhamSucKhoe { get; set; }
        public virtual NhanVien NhanVienHuyDichVu { get; set; }

        // dùng cho màn hình kết quả CĐHA-TDCN
        public virtual NhanVien NhanVienKetLuan2 { get; set; }
        public virtual NhanVien NhanVienThucHien2 { get; set; }
        //============================================

        // dùng cho chức năng hủy trạng thái thực hiện dịch vụ
        public virtual NhanVien NhanVienHuyTrangThaiDaThucHien { get; set; }
        //=====================================================

        public virtual YeuCauDichVuKyThuatKhamSangLocTiemChung KhamSangLocTiemChung { get; set; }
        public virtual YeuCauDichVuKyThuatTiemChung TiemChung { get; set; }
        public virtual YeuCauDichVuKyThuatKhamSangLocTiemChung YeuCauDichVuKyThuatKhamSangLocTiemChung { get; set; }

        //BVHD-3668
        public virtual GoiKhamSucKhoe GoiKhamSucKhoeDichVuPhatSinh { get; set; }

        //BVHD-3731
        public long? NoiDungGhiChuMiemGiamId { get; set; }
        public virtual NoiDungGhiChuMiemGiam NoiDungGhiChuMiemGiam { get; set; }

        //BVHD-3779
        public Enums.BuaAn? BuaAn { get; set; }

        private ICollection<KetQuaChuanDoanHinhAnh> _ketQuaChuanDoanHinhAnhs;
        public virtual ICollection<KetQuaChuanDoanHinhAnh> KetQuaChuanDoanHinhAnhs
        {
            get => _ketQuaChuanDoanHinhAnhs ?? (_ketQuaChuanDoanHinhAnhs = new List<KetQuaChuanDoanHinhAnh>());
            protected set => _ketQuaChuanDoanHinhAnhs = value;

        }

        private ICollection<KetQuaXetNghiem> _ketQuaXetNghiems;
        public virtual ICollection<KetQuaXetNghiem> KetQuaXetNghiems
        {
            get => _ketQuaXetNghiems ?? (_ketQuaXetNghiems = new List<KetQuaXetNghiem>());
            protected set => _ketQuaXetNghiems = value;

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
       
        private ICollection<PhongBenhVienHangDoi> _phongBenhVienHangDois;
        public virtual ICollection<PhongBenhVienHangDoi> PhongBenhVienHangDois
        {
            get => _phongBenhVienHangDois ?? (_phongBenhVienHangDois = new List<PhongBenhVienHangDoi>());
            protected set => _phongBenhVienHangDois = value;

        }

        private ICollection<YeuCauDichVuGiuongBenhVien> _yeuCauDichVuGiuongBenhViens;
        public virtual ICollection<YeuCauDichVuGiuongBenhVien> YeuCauDichVuGiuongBenhViens
        {
            get => _yeuCauDichVuGiuongBenhViens ?? (_yeuCauDichVuGiuongBenhViens = new List<YeuCauDichVuGiuongBenhVien>());
            protected set => _yeuCauDichVuGiuongBenhViens = value;

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

        private ICollection<HoatDongGiuongBenh> _hoatDongGiuongBenhs;
        public virtual ICollection<HoatDongGiuongBenh> HoatDongGiuongBenhs
        {
            get => _hoatDongGiuongBenhs ?? (_hoatDongGiuongBenhs = new List<HoatDongGiuongBenh>());
            protected set => _hoatDongGiuongBenhs = value;
        }

        public virtual YeuCauDichVuKyThuatTuongTrinhPTTT YeuCauDichVuKyThuatTuongTrinhPTTT { get; set; }

        private ICollection<FileKetQuaCanLamSang> _fileKetQuaCanLamSangs;
        public virtual ICollection<FileKetQuaCanLamSang> FileKetQuaCanLamSangs
        {
            get => _fileKetQuaCanLamSangs ?? (_fileKetQuaCanLamSangs = new List<FileKetQuaCanLamSang>());
            protected set => _fileKetQuaCanLamSangs = value;
        }

        private ICollection<PhienXetNghiemChiTiet> _phienXetNghiemChiTiets;
        public virtual ICollection<PhienXetNghiemChiTiet> PhienXetNghiemChiTiets
        {
            get => _phienXetNghiemChiTiets ?? (_phienXetNghiemChiTiets = new List<PhienXetNghiemChiTiet>());
            protected set => _phienXetNghiemChiTiets = value;
        }

        private ICollection<KetQuaXetNghiemChiTiet> _ketQuaXetNghiemChiTiets;
        public virtual ICollection<KetQuaXetNghiemChiTiet> KetQuaXetNghiemChiTiets
        {
            get => _ketQuaXetNghiemChiTiets ?? (_ketQuaXetNghiemChiTiets = new List<KetQuaXetNghiemChiTiet>());
            protected set => _ketQuaXetNghiemChiTiets = value;
        }

        private ICollection<NoiTruPhieuDieuTriChiTietYLenh> _noiTruPhieuDieuTriChiTietYLenhs;
        public virtual ICollection<NoiTruPhieuDieuTriChiTietYLenh> NoiTruPhieuDieuTriChiTietYLenhs
        {
            get => _noiTruPhieuDieuTriChiTietYLenhs ?? (_noiTruPhieuDieuTriChiTietYLenhs = new List<NoiTruPhieuDieuTriChiTietYLenh>());
            protected set => _noiTruPhieuDieuTriChiTietYLenhs = value;
        }

        private ICollection<ThuePhong> _thuePhongs;
        public virtual ICollection<ThuePhong> ThuePhongs
        {
            get => _thuePhongs ?? (_thuePhongs = new List<ThuePhong>());
            protected set => _thuePhongs = value;
        }

        private ICollection<ThuePhong> _tinhChiPhiThuePhongs;
        public virtual ICollection<ThuePhong> TinhChiPhiThuePhongs
        {
            get => _tinhChiPhiThuePhongs ?? (_tinhChiPhiThuePhongs = new List<ThuePhong>());
            protected set => _tinhChiPhiThuePhongs = value;
        }
    }
}
