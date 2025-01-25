using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.DonViHanhChinhs;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.QuocGias;
using Camino.Core.Domain.Entities.CongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.CongTyUuDais;
using Camino.Core.Domain.Entities.GiayMienCungChiTras;
using Camino.Core.Domain.Entities.DanTocs;
using Camino.Core.Domain.Entities.DoiTuongUuDais;
using Camino.Core.Domain.Entities.DoiTuongUuTienKhamChuaBenhs;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.DuyetBaoHiems;
using Camino.Core.Domain.Entities.GiuongBenhs;
using Camino.Core.Domain.Entities.HinhThucDens;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.ICDs;
using Camino.Core.Domain.Entities.KetQuaSinhHieus;
using Camino.Core.Domain.Entities.LyDoTiepNhans;
using Camino.Core.Domain.Entities.TrieuChungs;
using Camino.Core.Domain.Entities.NgheNghieps;
using Camino.Core.Domain.Entities.QuanHeThanNhans;
using Camino.Core.Domain.Entities.Vouchers;
using Camino.Core.Domain.Entities.YeuCauTiepNhanCongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.NguoiGioiThieus;
using Camino.Core.Domain.Entities.BHYT;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.DonVatTus;
using Camino.Core.Domain.Entities.KetQuaNhomXetNghiems;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.Entities.MauVaChePhams;
using Camino.Core.Domain.Entities.PhauThuatThuThuats;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.Entities.YeuCauNhapViens;
using Camino.Core.Domain.Entities.YeuCauTiepNhanLichSuChuyenDoiTuongs;
using Camino.Core.Domain.Entities.YeuCauTiepNhanTheBHYTs;
using Camino.Core.Domain.Entities.NoiTruDonThuocs;

namespace Camino.Core.Domain.Entities.YeuCauTiepNhans
{
    public class YeuCauTiepNhan : BaseEntity
    {
        public bool? CoBHYT { get; set; }
        public bool? CoBHTN { get; set; }
        public bool? DuocUuDai { get; set; }
        public long? BenhNhanId { get; set; }
        public string HoTen { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string SoChungMinhThu { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public Enums.EnumNhomMau? NhomMau { get; set; }
        public Enums.EnumYeuToRh? YeuToRh { get; set; }
        public long? NgheNghiepId { get; set; }
        public string NoiLamViec { get; set; }
        public long? QuocTichId { get; set; }
        public long? DanTocId { get; set; }
        public string DiaChi { get; set; }
        public long? PhuongXaId { get; set; }
        public long? QuanHuyenId { get; set; }
        public long? TinhThanhId { get; set; }
        public string SoDienThoai { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string SoDienThoaiDisplay { get; set; }
        public string Email { get; set; }
        public string NguoiLienHeHoTen { get; set; }
        public long? NguoiLienHeQuanHeNhanThanId { get; set; }
        public string NguoiLienHeSoDienThoai { get; set; }
        public string NguoiLienHeEmail { get; set; }
        public string NguoiLienHeDiaChi { get; set; }
        public long? NguoiLienHePhuongXaId { get; set; }
        public long? NguoiLienHeQuanHuyenId { get; set; }
        public long? NguoiLienHeTinhThanhId { get; set; }     
        public string BHYTMaSoThe { get; set; }
        //public long? BHYTnoiDangKyId { get; set; }
        public DateTime? BHYTNgayHieuLuc { get; set; }
        public DateTime? BHYTNgayHetHan { get; set; }
        public string BHYTDiaChi { get; set; }
        public string BHYTCoQuanBHXH { get; set; }
        public DateTime? BHYTNgayDu5Nam { get; set; }
        public bool? BHYTDuocMienCungChiTra { get; set; }
        public DateTime? BHYTNgayDuocMienCungChiTra { get; set; }
        public string BHYTMaKhuVuc { get; set; }


        public string BHYTThe2MaSoThe { get; set; }
        public int? BHYTThe2MucHuong { get; set; }
        public string BHYTThe2MaDKBD { get; set; }
        public DateTime? BHYTThe2NgayHieuLuc { get; set; }
        public DateTime? BHYTThe2NgayHetHan { get; set; }
        public string BHYTThe2DiaChi { get; set; }
        public string BHYTThe2CoQuanBHXH { get; set; }
        public DateTime? BHYTThe2NgayDu5Nam { get; set; }
        public DateTime? BHYTThe2NgayDuocMienCungChiTra { get; set; }
        public string BHYTThe2MaKhuVuc { get; set; }

        public long? BHYTGiayMienCungChiTraId { get; set; }
        public bool? BHYTDuocGiaHanThe { get; set; }
        //public long? BHTNCongTyBaoHiemId { get; set; }
        //public string BHTNMaSoThe { get; set; }
        //public string BHTNDiaChi { get; set; }
        //public string BHTNSoDienThoai { get; set; }
        //public DateTime? BHTNNgayHieuLuc { get; set; }
        //public DateTime? BHTNNgayHetHan { get; set; }
        public Enums.EnumLoaiYeuCauTiepNhan LoaiYeuCauTiepNhan { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public DateTime ThoiDiemTiepNhan { get; set; }
        public long? NhanVienTiepNhanId { get; set; }
        public long? NoiTiepNhanId { get; set; }
        public Enums.EnumLyDoVaoVien? LyDoVaoVien { get; set; }
        //public Enums.EnumYeuCauTiepNhan? LyDoTiepNhan { get; set; }
        public long? LyDoTiepNhanId { get; set; }

        public bool? LaTaiKham { get; set; }
       // public long TrieuChungId { get; set; }
        public string TrieuChungTiepNhan { get; set; }
        public int? LoaiTaiNan { get; set; }
        //public long? PhongKhamChiDinhId { get; set; }
        //public long? BacSiChiDinhId { get; set; }
        public bool? DuocChuyenVien { get; set; }
        public long? GiayChuyenVienId { get; set; }
        public DateTime? ThoiGianChuyenVien { get; set; }
        public long? NoiChuyenId { get; set; }
        public long? DoiTuongUuTienKhamChuaBenhId { get; set; }
        //public bool LaKhamTheoYeuCau { get; set; }
        //public long? PhongKhamId { get; set; }
        //public long? BacSiKhamId { get; set; }
        //public long? IcdchinhId { get; set; }
        //public string TenBenh { get; set; }
        //public long? PhongKetLuanId { get; set; }
        //public long? BacSiKetLuanId { get; set; }
        //public DateTime? ThoiDiemKetLuan { get; set; }
        //public DateTime? ThoiDiemRaVien { get; set; }
        //public DateTime? ThoiDiemThanhToan { get; set; }
        //public long? NhanVienThanhToanId { get; set; }
        //public long? NoiThanhToanId { get; set; }
        public int? KetQuaDieuTri { get; set; }
        public int? TinhTrangRaVien { get; set; }
        //public Enums.EnumTrangThaiYeuCauKhamBenh TrangThaiYeuCauTiepNhan { get; set; }
        public Enums.EnumTrangThaiYeuCauTiepNhan TrangThaiYeuCauTiepNhan { get; set; }
        public DateTime ThoiDiemCapNhatTrangThai { get; set; }
        public Enums.EnumTinhTrangThe? TinhTrangThe { get; set; }
        public bool? IsCheckedBHYT { get; set; }
        public string BHYTMaDKBD { get; set; }
        public long? NoiGioiThieuId { get; set; }
        public long? HinhThucDenId { get; set; }
        //public string TenUuDai { get; set; }

        public long? DoiTuongUuDaiId { get; set; }
        public long? CongTyUuDaiId { get; set; }
        public string SoChuyenTuyen { get; set; }
        public Enums.TuyenChuyenMonKyThuat? TuyenChuyen { get; set; }
        public string LyDoChuyen { get; set; }
        public int? BHYTMucHuong { get; set; }

        public bool? CoMienGiamThem { get; set; }
        //public Enums.LoaiMienGiamThem? LoaiMienGiamThem { get; set; }
        //public decimal? SoTienMienGiamThem { get; set; }
        //public int? TiLeMienGiamThem { get; set; }
        public string LyDoMienGiamThem { get; set; }
        public long? GiayMienGiamThemId { get; set; }

        public bool? TuNhap { get; set; }
        public long? NhanVienDuyetMienGiamThemId { get; set; }
        public long? NguoiGioiThieuId { get; set; }

        public bool? DaGuiCongBHYT { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string DiaChiDayDu { get; private set; }

        public bool? QuyetToanTheoNoiTru { get; set; }
        public long? YeuCauTiepNhanNgoaiTruCanQuyetToanId { get; set; }
        public bool? ChoTamUngThem { get; set; }

        public Enums.EnumTuVongPTTTTheoNgay? TuVongTrongPTTT { get; set; }

        public Enums.EnumThoiGianTuVongPTTTTheoNgay? KhoangThoiGianTuVong { get; set; }

        public DateTime? ThoiDiemTuVong { get; set; }
        public long? YeuCauNhapVienId { get; set; }
        public string TenBanDau { get; set; }
        public string TenKhaiSinh { get; set; }
        public int? GioSinh { get; set; }

        public string HoTenBo { get; set; }
        public string TrinhDoVanHoaCuaBo { get; set; }
        public long? NgheNghiepCuaBoId { get; set; }
        public string HoTenMe { get; set; }
        public string TrinhDoVanHoaCuaMe { get; set; }
        public long? NgheNghiepCuaMeId { get; set; }
        public string LyDoHuyNhapVien { get; set; }

        public long? HopDongKhamSucKhoeNhanVienId { get; set; }
        public Enums.PhanLoaiSucKhoe? KSKPhanLoaiTheLuc { get; set; }
        public string KSKKetQuaCanLamSang { get; set; }
        public string KSKDanhGiaCanLamSang { get; set; }
        public long? KSKNhanVienDanhGiaCanLamSangId { get; set; }
        public string KSKKetLuanPhanLoaiSucKhoe { get; set; }
        public string KSKKetLuanGhiChu { get; set; }
        public string KSKKetLuanCacBenhTat { get; set; }
        public string KetQuaKhamSucKhoeData { get; set; }
        public long? KSKNhanVienKetLuanId { get; set; }
        public DateTime? KSKThoiDiemKetLuan { get; set; }

        public string KSKKetLuanData { get; set; }
        public bool? LoaiLuuInKetQuaKSK { get; set; }

        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        //public string NguoiLienHeDiaChiDayDu { get; private set; }


        //BVHD-3668
        public long? YeuCauTiepNhanKhamSucKhoeId { get; set; }

        //BVHD-3761
        public string BieuHienLamSang { get; set; }
        public string DichTeSarsCoV2 { get; set; }

        //BVHD-3800
        public bool? LaCapCuu { get; set; }

        public virtual YeuCauNhapVien YeuCauNhapVien { get; set; }
        public virtual YeuCauTiepNhan YeuCauTiepNhanNgoaiTruCanQuyetToan { get; set; }

        public virtual LyDoTiepNhan LyDoTiepNhan { get; set; }

        public virtual NhanVien NhanVienDuyetMienGiamThem { get; set; }
        public virtual NguoiGioiThieu NguoiGioiThieu { get; set; }

        public virtual DoiTuongUuDai DoiTuongUuDai { get; set; }
        public virtual CongTyUuDai CongTyUuDai { get; set; }

        public virtual NoiGioiThieu.NoiGioiThieu NoiGioiThieu { get; set; }
        public virtual HinhThucDen HinhThucDen { get; set; }

        public virtual BenhVien.BenhVien NoiChuyen { get; set; }
        //public virtual NhanVien BacSiKetLuan { get; set; }
        //public virtual NhanVien BacSiKham { get; set; }
        public virtual BenhNhan BenhNhan { get; set; }
        //public virtual CongTyBaoHiemTuNhan BHTNCongTyBaoHiem { get; set; }
        public virtual GiayMienCungChiTra BHYTGiayMienCungChiTra { get; set; }
        //public virtual BenhVien.BenhVien BHYTnoiDangKy { get; set; }
        public virtual DanToc DanToc { get; set; }
        public virtual DoiTuongUuTienKhamChuaBenh DoiTuongUuTienKhamChuaBenh { get; set; }
        public virtual GiayChuyenVien GiayChuyenVien { get; set; }
        //public virtual ICD Icdchinh { get; set; }
        //public virtual TrieuChung TrieuChung { get; set; }
        public virtual NgheNghiep NgheNghiep { get; set; }
        public virtual DonViHanhChinh NguoiLienHePhuongXa { get; set; }
        public virtual QuanHeThanNhan NguoiLienHeQuanHeNhanThan { get; set; }
        public virtual DonViHanhChinh NguoiLienHeQuanHuyen { get; set; }
        public virtual DonViHanhChinh NguoiLienHeTinhThanh { get; set; }
        //public virtual NhanVien NhanVienThanhToan { get; set; }
        public virtual NhanVien NhanVienTiepNhan { get; set; }
        //public virtual PhongBenhVien NoiThanhToan { get; set; }
        public virtual PhongBenhVien NoiTiepNhan { get; set; }
        //public virtual PhongBenhVien PhongKetLuan { get; set; }
        //public virtual PhongBenhVien PhongKham { get; set; }
        public virtual DonViHanhChinh PhuongXa { get; set; }
        public virtual DonViHanhChinh QuanHuyen { get; set; }
        public virtual QuocGia QuocTich { get; set; }
        public virtual DonViHanhChinh TinhThanh { get; set; }
        public virtual GiayMienGiamThem GiayMienGiamThem { get; set; }
        public virtual NoiTruBenhAn NoiTruBenhAn { get; set; }
        public virtual NgheNghiep NgheNghiepCuaBo { get; set; }
        public virtual NgheNghiep NgheNghiepCuaMe { get; set; }

        public virtual HopDongKhamSucKhoeNhanVien HopDongKhamSucKhoeNhanVien { get; set; }
        public virtual NhanVien KSKNhanVienDanhGiaCanLamSang { get; set; }
        public virtual NhanVien KSKNhanVienKetLuan { get; set; }


        private ICollection<PhongBenhVienHangDoi> _phongBenhVienHangDois { get; set; }
        public virtual ICollection<PhongBenhVienHangDoi> PhongBenhVienHangDois
        {
            get => _phongBenhVienHangDois ?? (_phongBenhVienHangDois = new List<PhongBenhVienHangDoi>());
            protected set => _phongBenhVienHangDois = value;
        }

        private ICollection<YeuCauKhamBenh> _yeuCauKhamBenhs { get; set; }
        public virtual ICollection<YeuCauKhamBenh> YeuCauKhamBenhs
        {
            get => _yeuCauKhamBenhs ?? (_yeuCauKhamBenhs = new List<YeuCauKhamBenh>());
            protected set => _yeuCauKhamBenhs = value;
        }

        private ICollection<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuats { get; set; }
        public virtual ICollection<YeuCauDichVuKyThuat> YeuCauDichVuKyThuats
        {
            get => _yeuCauDichVuKyThuats ?? (_yeuCauDichVuKyThuats = new List<YeuCauDichVuKyThuat>());
            protected set => _yeuCauDichVuKyThuats = value;
        }

        private ICollection<YeuCauTiepNhanLichSuTrangThai> _yeuCauTiepNhanLichSuTrangThais { get; set; }
        public virtual ICollection<YeuCauTiepNhanLichSuTrangThai> YeuCauTiepNhanLichSuTrangThais
        {
            get => _yeuCauTiepNhanLichSuTrangThais ?? (_yeuCauTiepNhanLichSuTrangThais = new List<YeuCauTiepNhanLichSuTrangThai>());
            protected set => _yeuCauTiepNhanLichSuTrangThais = value;
        }

        private ICollection<KetQuaSinhHieu> _ketQuaSinhHieus;
        public virtual ICollection<KetQuaSinhHieu> KetQuaSinhHieus
        {
            get => _ketQuaSinhHieus ?? (_ketQuaSinhHieus = new List<KetQuaSinhHieu>());
            protected set => _ketQuaSinhHieus = value;
        }

        private ICollection<KetQuaNhomXetNghiem> _ketQuaNhomXetNghiems;
        public virtual ICollection<KetQuaNhomXetNghiem> KetQuaNhomXetNghiems
        {
            get => _ketQuaNhomXetNghiems ?? (_ketQuaNhomXetNghiems = new List<KetQuaNhomXetNghiem>());
            protected set => _ketQuaNhomXetNghiems = value;
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

        //update 22/02/2020 - thach
        private ICollection<YeuCauTiepNhanCongTyBaoHiemTuNhan> _yeuCauTiepNhanCongTyBaoHiemTuNhans;
        public virtual ICollection<YeuCauTiepNhanCongTyBaoHiemTuNhan> YeuCauTiepNhanCongTyBaoHiemTuNhans
        {
            get => _yeuCauTiepNhanCongTyBaoHiemTuNhans ?? (_yeuCauTiepNhanCongTyBaoHiemTuNhans = new List<YeuCauTiepNhanCongTyBaoHiemTuNhan>());
            protected set => _yeuCauTiepNhanCongTyBaoHiemTuNhans = value;
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

        private ICollection<HoSoYeuCauTiepNhan> _hoSoYeuCauTiepNhans;
        public virtual ICollection<HoSoYeuCauTiepNhan> HoSoYeuCauTiepNhans
        {
            get => _hoSoYeuCauTiepNhans ?? (_hoSoYeuCauTiepNhans = new List<HoSoYeuCauTiepNhan>());
            protected set => _hoSoYeuCauTiepNhans = value;
        }

        private ICollection<TheVoucherYeuCauTiepNhan> _theVoucherYeuCauTiepNhans { get; set; }
        public virtual ICollection<TheVoucherYeuCauTiepNhan> TheVoucherYeuCauTiepNhans
        {
            get => _theVoucherYeuCauTiepNhans ?? (_theVoucherYeuCauTiepNhans = new List<TheVoucherYeuCauTiepNhan>());
            protected set => _theVoucherYeuCauTiepNhans = value;
        }

        private ICollection<MienGiamChiPhi> _mienGiamChiPhis;
        public virtual ICollection<MienGiamChiPhi> MienGiamChiPhis
        {
            get => _mienGiamChiPhis ?? (_mienGiamChiPhis = new List<MienGiamChiPhi>());
            protected set => _mienGiamChiPhis = value;
        }

        private ICollection<DuyetBaoHiem> _duyetBaoHiems;
        public virtual ICollection<DuyetBaoHiem> DuyetBaoHiems
        {
            get => _duyetBaoHiems ?? (_duyetBaoHiems = new List<DuyetBaoHiem>());
            protected set => _duyetBaoHiems = value;
        }

        private ICollection<YeuCauTiepNhanLichSuKiemTraTheBHYT> _yeuCauTiepNhanLichSuKiemTraTheBHYT;
        public virtual ICollection<YeuCauTiepNhanLichSuKiemTraTheBHYT> YeuCauTiepNhanLichSuKiemTraTheBHYTs
        {
            get => _yeuCauTiepNhanLichSuKiemTraTheBHYT ?? (_yeuCauTiepNhanLichSuKiemTraTheBHYT = new List<YeuCauTiepNhanLichSuKiemTraTheBHYT>());
            protected set => _yeuCauTiepNhanLichSuKiemTraTheBHYT = value;
        }

        private ICollection<YeuCauTiepNhanLichSuKhamBHYT> _yeuCauTiepNhanLichSuKhamBHYT;
        public virtual ICollection<YeuCauTiepNhanLichSuKhamBHYT> YeuCauTiepNhanLichSuKhamBHYT
        {
            get => _yeuCauTiepNhanLichSuKhamBHYT ?? (_yeuCauTiepNhanLichSuKhamBHYT = new List<YeuCauTiepNhanLichSuKhamBHYT>());
            protected set => _yeuCauTiepNhanLichSuKhamBHYT = value;
        }

        private ICollection<TaiKhoanBenhNhanHuyDichVu> _taiKhoanBenhNhanHuyDichVus;
        public virtual ICollection<TaiKhoanBenhNhanHuyDichVu> TaiKhoanBenhNhanHuyDichVus
        {
            get => _taiKhoanBenhNhanHuyDichVus ?? (_taiKhoanBenhNhanHuyDichVus = new List<TaiKhoanBenhNhanHuyDichVu>());
            protected set => _taiKhoanBenhNhanHuyDichVus = value;
        }

        private ICollection<HoatDongGiuongBenh> _hoatDongGiuongBenhs { get; set; }
        public virtual ICollection<HoatDongGiuongBenh> HoatDongGiuongBenhs
        {
            get => _hoatDongGiuongBenhs ?? (_hoatDongGiuongBenhs = new List<HoatDongGiuongBenh>());
            protected set => _hoatDongGiuongBenhs = value;
        }

        private ICollection<YeuCauTiepNhanDuLieuGuiCongBHYT> _yeuCauTiepNhanDuLieuGuiCongBHYTs { get; set; }
        public virtual ICollection<YeuCauTiepNhanDuLieuGuiCongBHYT> YeuCauTiepNhanDuLieuGuiCongBHYTs
        {
            get => _yeuCauTiepNhanDuLieuGuiCongBHYTs ?? (_yeuCauTiepNhanDuLieuGuiCongBHYTs = new List<YeuCauTiepNhanDuLieuGuiCongBHYT>());
            protected set => _yeuCauTiepNhanDuLieuGuiCongBHYTs = value;
        }

        private ICollection<TheoDoiSauPhauThuatThuThuat> _theoDoiSauPhauThuatThuThuats { get; set; }
        public virtual ICollection<TheoDoiSauPhauThuatThuThuat> TheoDoiSauPhauThuatThuThuats
        {
            get => _theoDoiSauPhauThuatThuThuats ?? (_theoDoiSauPhauThuatThuThuats = new List<TheoDoiSauPhauThuatThuThuat>());
            protected set => _theoDoiSauPhauThuatThuThuats = value;
        }
        private ICollection<DonVTYTThanhToan> _donVTYTThanhToans;
        public virtual ICollection<DonVTYTThanhToan> DonVTYTThanhToans
        {
            get => _donVTYTThanhToans ?? (_donVTYTThanhToans = new List<DonVTYTThanhToan>());
            protected set => _donVTYTThanhToans = value;
        }

        private ICollection<PhienXetNghiem> _phienXetNghiems;
        public virtual ICollection<PhienXetNghiem> PhienXetNghiems
        {
            get => _phienXetNghiems ?? (_phienXetNghiems = new List<PhienXetNghiem>());
            protected set => _phienXetNghiems = value;
        }

        private ICollection<NoiTruHoSoKhac> _noiTruHoSoKhacs;
        public virtual ICollection<NoiTruHoSoKhac> NoiTruHoSoKhacs
        {
            get => _noiTruHoSoKhacs ?? (_noiTruHoSoKhacs = new List<NoiTruHoSoKhac>());
            protected set => _noiTruHoSoKhacs = value;
        }
        
        private ICollection<YeuCauDichVuGiuongBenhVienChiPhi> _yeuCauDichVuGiuongBenhVienChiPhis;
        public virtual ICollection<YeuCauDichVuGiuongBenhVienChiPhi> YeuCauDichVuGiuongBenhVienChiPhis
        {
            get => _yeuCauDichVuGiuongBenhVienChiPhis ?? (_yeuCauDichVuGiuongBenhVienChiPhis = new List<YeuCauDichVuGiuongBenhVienChiPhi>());
            protected set => _yeuCauDichVuGiuongBenhVienChiPhis = value;
        }

        private ICollection<YeuCauTruyenMau> _yeuCauTruyenMaus;
        public virtual ICollection<YeuCauTruyenMau> YeuCauTruyenMaus
        {
            get => _yeuCauTruyenMaus ?? (_yeuCauTruyenMaus = new List<YeuCauTruyenMau>());
            protected set => _yeuCauTruyenMaus = value;
        }

        private ICollection<NoiTruChiDinhDuocPham> _noiTruChiDinhDuocPhams;
        public virtual ICollection<NoiTruChiDinhDuocPham> NoiTruChiDinhDuocPhams
        {
            get => _noiTruChiDinhDuocPhams ?? (_noiTruChiDinhDuocPhams = new List<NoiTruChiDinhDuocPham>());
            protected set => _noiTruChiDinhDuocPhams = value;
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

        private ICollection<YeuCauNhapVien> _yeuCauNhapVienCons;
        public virtual ICollection<YeuCauNhapVien> YeuCauNhapVienCons
        {
            get => _yeuCauNhapVienCons ?? (_yeuCauNhapVienCons = new List<YeuCauNhapVien>());
            protected set => _yeuCauNhapVienCons = value;
        }

        private ICollection<YeuCauTiepNhanLichSuChuyenDoiTuong> _yeuCauTiepNhanLichSuChuyenDoiTuongs;
        public virtual ICollection<YeuCauTiepNhanLichSuChuyenDoiTuong> YeuCauTiepNhanLichSuChuyenDoiTuongs
        {
            get => _yeuCauTiepNhanLichSuChuyenDoiTuongs ?? (_yeuCauTiepNhanLichSuChuyenDoiTuongs = new List<YeuCauTiepNhanLichSuChuyenDoiTuong>());
            protected set => _yeuCauTiepNhanLichSuChuyenDoiTuongs = value;
        }

        private ICollection<YeuCauTiepNhanTheBHYT> _yeuCauTiepNhanTheBHYTs;
        public virtual ICollection<YeuCauTiepNhanTheBHYT> YeuCauTiepNhanTheBHYTs
        {
            get => _yeuCauTiepNhanTheBHYTs ?? (_yeuCauTiepNhanTheBHYTs = new List<YeuCauTiepNhanTheBHYT>());
            protected set => _yeuCauTiepNhanTheBHYTs = value;
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

        private ICollection<NoiTruDonThuoc> _noiTruDonThuoc;
        public virtual ICollection<NoiTruDonThuoc> NoiTruDonThuocs
        {
            get => _noiTruDonThuoc ?? (_noiTruDonThuoc = new List<NoiTruDonThuoc>());
            protected set => _noiTruDonThuoc = value;
        }

        private ICollection<YeuCauTiepNhan> _yeuCauTiepNhanNoiTruQuyetToans;
        public virtual ICollection<YeuCauTiepNhan> YeuCauTiepNhanNoiTruQuyetToans
        {
            get => _yeuCauTiepNhanNoiTruQuyetToans ?? (_yeuCauTiepNhanNoiTruQuyetToans = new List<YeuCauTiepNhan>());
            protected set => _yeuCauTiepNhanNoiTruQuyetToans = value;
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

        #region BVHD-3668
        public virtual YeuCauTiepNhan YeuCauTiepNhanKhamSucKhoe { get; set; }

        private ICollection<YeuCauTiepNhan> _yeuCauTiepNhanNgoaiTruKhamSucKhoes;
        public virtual ICollection<YeuCauTiepNhan> YeuCauTiepNhanNgoaiTruKhamSucKhoes
        {
            get => _yeuCauTiepNhanNgoaiTruKhamSucKhoes ?? (_yeuCauTiepNhanNgoaiTruKhamSucKhoes = new List<YeuCauTiepNhan>());
            protected set => _yeuCauTiepNhanNgoaiTruKhamSucKhoes = value;
        }
        #endregion

        private ICollection<ThuePhong> _thuePhongs;
        public virtual ICollection<ThuePhong> ThuePhongs
        {
            get => _thuePhongs ?? (_thuePhongs = new List<ThuePhong>());
            protected set => _thuePhongs = value;
        }
    }
}
