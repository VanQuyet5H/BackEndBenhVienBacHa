using Camino.Api.Models.DonViHanhChinh;
using Camino.Api.Models.NgheNghiep;
using Camino.Api.Models.NhanVien;
using Camino.Api.Models.PhongBenhVien;
using Camino.Api.Models.QuanHeThanNhan;
using Camino.Core.Domain;
using System;
using System.Collections.Generic;
using Camino.Api.Models.DanToc;
using Camino.Api.Models.KetQuaSinhHieu;
using Camino.Core.Helpers;
using Camino.Api.Models.BenhVien;
using Camino.Api.Models.LyDoTiepNhan;

namespace Camino.Api.Models.KhamBenh
{
    public class KhamBenhYeuCauTiepNhanViewModel : BaseViewModel
    {
        public long? BenhNhanId { get; set; }
        public string HoTen { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string NgaySinhDisplay => DateHelper.DOBFormat(NgaySinh, ThangSinh, NamSinh);

        public int? Tuoi
        {
            get { return NamSinh == null ? 0 : DateTime.Now.Year - NamSinh.Value; }
        }

        public string SoChungMinhThu { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }

        public string TenGioiTinh
        {
            get { return GioiTinh == null ? null : GioiTinh.GetDescription(); }
        }

        public int? NhomMau { get; set; }
        public long? NgheNghiepId { get; set; }
        public string NoiLamViec { get; set; }
        public long? QuocTichId { get; set; }
        public long? DanTocId { get; set; }
        public string DiaChi { get; set; }

        public string DiaChiDisplay
        {
            get
            {
                return AddressHelper.ApplyFormatAddress(TinhThanh?.Ten, QuanHuyen?.Ten, PhuongXa?.Ten, DiaChi);
            }
        }

        public long? PhuongXaId { get; set; }
        public long? QuanHuyenId { get; set; }
        public long? TinhThanhId { get; set; }
        public string SoDienThoai { get; set; }
        public string SoDienThoaiDisplay
        {
            get { return SoDienThoai.ApplyFormatPhone(); }
        }

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
        public DateTime? BHYTNgayHieuLuc { get; set; }
        public DateTime? BHYTNgayHetHan { get; set; }
        public string BHYTThoiGianHieuLucDisplay => BHYTNgayHieuLuc?.ApplyFormatDate() + (BHYTNgayHieuLuc != null && BHYTNgayHetHan != null ? " - " : "") + BHYTNgayHetHan?.ApplyFormatDate();
        public bool IsBHYTHetHan => CoBHYT == true && BHYTNgayHieuLuc != null && BHYTNgayHetHan != null && (DateTime.Now.Date < BHYTNgayHieuLuc.Value.Date || DateTime.Now.Date > BHYTNgayHetHan.Value.Date);
        public string BHYTdiaChi { get; set; }
        public string BHYTcoQuanBhxh { get; set; }
        public DateTime? BHYTngayDu5Nam { get; set; }
        public DateTime? BHYTngayDuocMienCungChiTra { get; set; }
        public string BHYTmaKhuVuc { get; set; }
        public long? BHYTgiayMienCungChiTraId { get; set; }
        public long? BHTNcongTyBaoHiemId { get; set; }
        public string BHTNmaSoThe { get; set; }
        public string BHTNdiaChi { get; set; }
        public string BHTNsoDienThoai { get; set; }
        public DateTime? BHTNngayHieuLuc { get; set; }
        public DateTime? BHTNngayHetHan { get; set; }
        public Enums.EnumLoaiYeuCauTiepNhan? LoaiYeuCauTiepNhan { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public DateTime? ThoiDiemTiepNhan { get; set; }
        public string ThoiDiemTiepNhanDisplay
        {
            get { return ThoiDiemTiepNhan?.ApplyFormatDateTimeSACH(); }
        }

        public long? NhanVienTiepNhanId { get; set; }
        public long? NoiTiepNhanId { get; set; }
        public Enums.EnumLyDoVaoVien? LyDoVaoVien { get; set; }
        public string TenLyDoVaoVien {
            get { return LyDoVaoVien == null ? null : LyDoVaoVien.GetDescription(); }
        }
        public int? BHYTMucHuong { get; set; }
        public bool? LaTaiKham { get; set; }
        public bool? CoBHYT { get; set; }

        public string TrieuChungTiepNhan { get; set; }
        public int? LoaiTaiNan { get; set; }
        public bool? DuocChuyenVien { get; set; }
        public long? GiayChuyenVienId { get; set; }
        public DateTime? ThoiGianChuyenVien { get; set; }
        public long? NoiChuyenId { get; set; }
        public long? DoiTuongUuTienKhamChuaBenhId { get; set; }
        public int? KetQuaDieuTri { get; set; }
        public int? TinhTrangRaVien { get; set; }
        public Enums.EnumTrangThaiYeuCauKhamBenh TrangThaiYeuCauTiepNhan { get; set; }
        public DateTime? ThoiDiemCapNhatTrangThai { get; set; }
        public int? TinhTrangThe { get; set; }
        public bool? IsCheckedBHYT { get; set; }
        public string BHYTMaDKBD { get; set; }
        public long? NoiGioiThieuId { get; set; }
        public long? HinhThucDenId { get; set; }

        public long? DoiTuongUuDaiId { get; set; }
        public long? CongTyUuDaiId { get; set; }
        public string SoChuyenTuyen { get; set; }
        public int? TuyenChuyen { get; set; }
        public string LyDoChuyen { get; set; }
        public long? LyDoTiepNhanId { get; set; }
        public string TenLyDoTiepNhan
        {
            get { return LyDoTiepNhan?.Ten; }
        }

        public string TenCongTy { get; set; }
        public Enums.PhanLoaiSucKhoe? KSKPhanLoaiTheLuc { get; set; }
        public long? HopDongKhamSucKhoeId { get; set; }
        public long? HopDongKhamSucKhoeNhanVienId { get; set; }

        // khám đoàn tất cả
        public string KSKKetQuaCanLamSang { get; set; }
        public string KSKDanhGiaCanLamSang { get; set; }
        public long? KSKNhanVienDanhGiaCanLamSangId { get; set; }
        public Enums.PhanLoaiSucKhoe? PhanLoaiSucKhoeId { get; set; }
        public string KSKKetLuanPhanLoaiSucKhoe { get; set; }
        public string KSKKetLuanGhiChu { get; set; }
        public string KSKKetLuanCacBenhTat { get; set; }
        public long? KSKNhanVienKetLuanId { get; set; }
        public DateTime? KSKThoiDiemKetLuan { get; set; }
        public string KetQuaKhamSucKhoeData { get; set; }

        // update mới: kiểm tra nếu đủ 5 chuyên khoa mới bắt valdation kết luận khám đoàn
        public bool? IsDuChuyenKhoaKhamSucKhoeChinh { get; set; }

        //BVHD-3574
        public Enums.EnumTrangThaiYeuCauTiepNhan TrangThaiTiepNhan { get; set; }

        //BVHD-3800
        public bool? LaCapCuu { get; set; }

        //BVHD-3941
        public bool? CoBaoHiemTuNhan { get; set; }
        public string TenCongTyBaoHiemTuNhan { get; set; }

        //BVHD-3960
        public string TenHinhThucDen { get; set; }
        public string TenNoiGioiThieu { get; set; }
        public bool? LaHinhThucDenGioiThieu { get; set; }
        public string HinhThucDenDisplay => LaHinhThucDenGioiThieu.GetValueOrDefault() ? $"{TenHinhThucDen} / {TenNoiGioiThieu}": TenHinhThucDen;

        public virtual NgheNghiepViewModel NgheNghiep { get; set; }
        public virtual DonViHanhChinhViewModel NguoiLienHePhuongXa { get; set; }
        public virtual QuanHeThanNhanViewModel NguoiLienHeQuanHeNhanThan { get; set; }
        public virtual DonViHanhChinhViewModel NguoiLienHeQuanHuyen { get; set; }
        public virtual DonViHanhChinhViewModel NguoiLienHeTinhThanh { get; set; }
        public virtual NhanVienViewModel NhanVienThanhToan { get; set; }
        public virtual NhanVienViewModel NhanVienTiepNhan { get; set; }
        public virtual PhongBenhVienViewModel NoiThanhToan { get; set; }
        public virtual PhongBenhVienViewModel NoiTiepNhan { get; set; }
        public virtual DonViHanhChinhViewModel PhuongXa { get; set; }
        public virtual DonViHanhChinhViewModel QuanHuyen { get; set; }
        public virtual DonViHanhChinhViewModel TinhThanh { get; set; }
        public virtual DanTocViewModel DanToc { get; set; }
        public virtual KhamBenhBenhNhanViewModel BenhNhan { get; set; }
        public virtual BienVienViewModel BenhVienHienTai { get; set; }
        public virtual LyDoTiepNhanViewModel LyDoTiepNhan { get; set; }

        public KhamBenhYeuCauTiepNhanViewModel()
        {
            PhongBenhVienHangDois = new List<KhamBenhPhongBenhVienHangDoiViewModel>();
            YeuCauTiepNhanChuanDoans = new List<KhamBenhYeuCauChuanDoanViewModel>();
            //YeuCauTiepNhanDichVuKhamBenhs = new List<KhamBenhYeuCauDichVuKhamBenhViewModel>();
            YeuCauTiepNhanDichVuKyThuats = new List<KhamBenhYeuCauDichVuKyThuatViewModel>();
            YeuCauTiepNhanDonThuocs = new List<KhamBenhYeuCauDonThuocViewModel>();
            YeuCauTiepNhanIcdkhacs = new List<KhamBenhYeuCauIcdKhacViewModel>();
            YeuCauTiepNhanLichSuTrangThais = new List<KhamBenhYeuCauLichSuTrangThaiViewModel>();
            YeuCauTiepNhanTrieuChungs = new List<KhamBenhYeuCauTrieuChungViewModel>();
            KetQuaSinhHieus = new List<KetQuaSinhHieuViewModel>();
            ChuyenKhoaKhamSucKhoeChinhs = new List<Enums.ChuyenKhoaKhamSucKhoe>();
        }
        public List<KhamBenhPhongBenhVienHangDoiViewModel> PhongBenhVienHangDois { get; set; }
        public List<KhamBenhYeuCauChuanDoanViewModel> YeuCauTiepNhanChuanDoans { get; set; }
        //public List<KhamBenhYeuCauDichVuKhamBenhViewModel> YeuCauTiepNhanDichVuKhamBenhs { get; set; }
        public List<KhamBenhYeuCauDichVuKyThuatViewModel> YeuCauTiepNhanDichVuKyThuats { get; set; }
        public List<KhamBenhYeuCauDonThuocViewModel> YeuCauTiepNhanDonThuocs { get; set; }
        public List<KhamBenhYeuCauIcdKhacViewModel> YeuCauTiepNhanIcdkhacs { get; set; }
        public List<KhamBenhYeuCauLichSuTrangThaiViewModel> YeuCauTiepNhanLichSuTrangThais { get; set; }
        public List<KhamBenhYeuCauTrieuChungViewModel> YeuCauTiepNhanTrieuChungs { get; set; }
        public List<KetQuaSinhHieuViewModel> KetQuaSinhHieus { get; set; }
        public List<Enums.ChuyenKhoaKhamSucKhoe> ChuyenKhoaKhamSucKhoeChinhs { get; set; }
    }   
}
