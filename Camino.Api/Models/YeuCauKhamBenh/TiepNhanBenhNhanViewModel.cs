using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Camino.Api.Models.DichVuKhamBenh;
using Camino.Api.Models.DichVuKyThuatBenhVien;
using Camino.Api.Models.DoiTuongUuDais;
using Camino.Api.Models.NguoiGioiThieu;
using Camino.Api.Models.NhanVien;
using Camino.Api.Models.NoiGioiThieu;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using iText.Layout.Element;

namespace Camino.Api.Models.YeuCauKhamBenh
{
    public class LoaiGiaOrSoLuongChange
    {
        public LoaiGiaOrSoLuongChange()
        {
            lstChiDinhDichVu = new List<ListChiDinhNeedUpdate>();
        }
        public List<ListChiDinhNeedUpdate> lstChiDinhDichVu { get; set; }
        public long? yeuCauTiepNhanId { get; set; }
        public long? maDichVuId { get; set; }
        public long? yeuCauId { get; set; }
        public string nhom { get; set; }
        public int? soLuong { get; set; }
        public long? loaiGiaId { get; set; }
        public bool? LaDichVuTrongGoi { get; set; }
        
        //BVHD-3825
        public bool? LaDichVuKhuyenMai { get; set; }
    }

    public class GridUpdate
    {
        public GridUpdate()
        {
            lstChiDinhDichVu = new List<ListChiDinhNeedUpdate>();
        }
        public bool? TuNhap { get; set; }
        public TiepNhanBenhNhanViewModel model { get; set; }
        public bool? isChecked { get; set; }
        public long? maDichVuId { get; set; }
        public string nhom { get; set; }
        public long? yeuCauTiepNhanId { get; set; }
        public int? mucHuong { get; set; }

        public string NoiThucHienId { get; set; }

        public string HoTen { get; set; }
        public DateTime? NgaySinh { get; set; }

        public List<ListChiDinhNeedUpdate> lstChiDinhDichVu { get; set; }
        public string LyDoHuyDichVu { get; set; }
        public List<ListDichVuCheckTruocDo> ListDichVuCheckTruocDos { get; set; }
    }

    public class TiepNhanBenhNhanViewModel : BaseViewModel
    {
        public TiepNhanBenhNhanViewModel()
        {
            YeuCauKhamBenhs = new List<YeuCauKhamBenhTiepNhanViewModel>();
            YeuCauKhamBenhGrid = new List<ChiDinhDichVuGridVo>();
            YeuCauKhacGrid = new List<ChiDinhDichVuGridVo>();
            YeuCauKyThuatGrid = new List<ChiDinhDichVuGridVo>();
            YeuCauDichVuKyThuats = new List<YeuCauDichVuKyThuatTiepNhanViewModel>();
            //YeuCauVatTuBenhViens = new List<YeuCauVatTuBenhVienTiepNhanViewModel>();
            //YeuCauDuocPhamBenhViens = new List<YeuCauDuocPhamBenhVienTiepNhanViewModel>();
            BaoHiemTuNhans = new List<BaoHiemTuNhanViewModel>();
            HoSoYeuCauTiepNhans = new List<HoSoYeuCauTiepNhanViewModel>();
            LstVoucherId = new List<long>();
            LstMaVoucher = new List<string>();
            GridLichSuKCB = new List<GridLichSuKCB>();
            GridLichSuKiemTraTheBHYT = new List<GridLichSuKiemTraTheBHYT>();
        }

        public bool DisableCoMienGiam { get; set; }

        public bool DisableDoiTuongUuDai { get; set; }

        public decimal? SoTienConLai { get; set; }

        public long? DanTocId { get; set; }

        #region feedback of client on 22/05/2020

        public List<GridLichSuKCB> GridLichSuKCB { get; set; }
        public List<GridLichSuKiemTraTheBHYT> GridLichSuKiemTraTheBHYT { get; set; }

        public bool? TuNhap { get; set; }

        #endregion feedback of client on 22/05/2020

        public Enums.LoaiVoucher? LoaiVoucher { get; set; }
        public long? BenhNhanId { get; set; }

        public int LoaiMienGiam { get; set; }

        public List<long> LstVoucherId { get; set; }
        public List<string> LstMaVoucher { get; set; }

        #region update v2
        public DateTime? BHYTNgayHieuLuc { get; set; }
        public string BHYTCoQuanBHXH { get; set; }
        public DateTime? BHYTNgayHetHan { get; set; }
        public int? BHYTMucHuong { get; set; }
        public DateTime? BHYTNgayDu5Nam { get; set; }
        public string BHYTDiaChi { get; set; }
        public string NoiDangKyBHYT { get; set; }
        public string BHYTMaSoThe { get; set; }
        public string BHYTMaDKBD { get; set; }
        public bool? CoBHYT { get; set; }
        public long? NoiChuyenId { get; set; }
        public Enums.EnumLyDoVaoVien? LyDoVaoVien { get; set; }
        public long? LyDoTiepNhanId { get; set; }

        public string LyDoTiepNhanText { get; set; }

        public string GioiThieu { get; set; }

        public bool? DuocChuyenVien { get; set; }
        public string SoChuyenTuyen { get; set; }
        public Enums.TuyenChuyenMonKyThuat? TuyenChuyen { get; set; }
        public string LyDoChuyen { get; set; }
        public bool? BHYTDuocMienCungChiTra { get; set; }
        public long? BHYTGiayMienCungChiTraId { get; set; }
        public GiayMienCungChiTraViewModel BHYTGiayMienCungChiTra { get; set; }
        public DateTime? BHYTNgayDuocMienCungChiTra { get; set; }
        public string HoTen { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string SoChungMinhThu { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public long? NgheNghiepId { get; set; }
        public long? QuocTichId { get; set; }
        public long? TinhThanhId { get; set; }
        public long? QuanHuyenId { get; set; }
        public long? PhuongXaId { get; set; }
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public string NoiLamViec { get; set; }
        public bool? DuocUuDai { get; set; }
        public long? DoiTuongUuDaiId { get; set; }
        public DoiTuongUuDaiViewModel DoiTuongUuDai { get; set; }
        public long? CongTyUuDaiId { get; set; }
        public CongTyUuDaiViewModel CongTyUuDai { get; set; }
        public bool? CoBHTN { get; set; }
        //public long? BHTNCongTyBaoHiemId { get; set; }
        //public CongTyBaoHiemTuNhanViewModel BHTNCongTyBaoHiem { get; set; }
        //public string BHTNMaSoThe { get; set; }
        //public DateTime? BHTNNgayHieuLuc { get; set; }
        //public DateTime? BHTNNgayHetHan { get; set; }
        //public string BHTNSoDienThoai { get; set; }
        //public string BHTNDiaChi { get; set; }
        public List<BaoHiemTuNhanViewModel> BaoHiemTuNhans { get; set; }
        public Enums.EnumLoaiYeuCauTiepNhan? LoaiYeuCauTiepNhan { get; set; }
        public DateTime? ThoiDiemTiepNhan { get; set; }
        public long? HinhThucDenId { get; set; }
        public long? NoiGioiThieuId { get; set; }
        public long? NguoiGioiThieuId { get; set; }

        public NguoiGioiThieuViewModel NguoiGioiThieu { get; set; }

        public NoiGioiThieuViewModel NoiGioiThieu { get; set; }
        public string TrieuChungTiepNhan { get; set; }
        public List<YeuCauKhamBenhTiepNhanViewModel> YeuCauKhamBenhs { get; set; }
        public List<ChiDinhDichVuGridVo> YeuCauKhamBenhGrid { get; set; }
        public List<ChiDinhDichVuGridVo> YeuCauKhacGrid { get; set; }
        public List<ChiDinhDichVuGridVo> YeuCauKyThuatGrid { get; set; }
        public List<YeuCauDichVuKyThuatTiepNhanViewModel> YeuCauDichVuKyThuats { get; set; }
        //public List<YeuCauVatTuBenhVienTiepNhanViewModel> YeuCauVatTuBenhViens { get; set; }
        //public List<YeuCauDuocPhamBenhVienTiepNhanViewModel> YeuCauDuocPhamBenhViens { get; set; }

        //người liên hệ
        public string NguoiLienHeHoTen { get; set; }
        public long? NguoiLienHeQuanHeNhanThanId { get; set; }
        public string NguoiLienHeSoDienThoai { get; set; }
        public string NguoiLienHeEmail { get; set; }
        public string NguoiLienHeDiaChi { get; set; }
        public long? NguoiLienHePhuongXaId { get; set; }
        public long? NguoiLienHeQuanHuyenId { get; set; }
        public long? NguoiLienHeTinhThanhId { get; set; }

        public DateTime? NgayThangNamSinh { get; set; }

        #endregion update v2

        #region update from client

        public List<HoSoYeuCauTiepNhanViewModel> HoSoYeuCauTiepNhans { get; set; }

        #endregion update from client

        public DateTime? ThoiGianTiepNhan { get; set; }
        public bool? LaTaiKham { get; set; }
        public long? LyDoKhamBenhId { get; set; }
        public long? PhongKhamId { get; set; }
        public string PhongKhamVaBacSiId { get; set; }
        public long? KhoaKhamId { get; set; }
        public long? GiayChuyenVienId { get; set; }
        public DateTime? ThoiGianChuyenVien { get; set; }
        public long? DoiTuongUuTienKhamChuaBenhId { get; set; }
        public bool? LaKhamBenhTheoYeuCau { get; set; }
        public bool LaKhamTheoYeuCau { get; set; }



        public BenhNhanTiepNhanBenhNhanViewModel BenhNhan { get; set; }
        public LyDoKhamBenhBenhNhanViewModel LyDoKhamBenh { get; set; }
        public PhongNgoaiTruBenhNhanViewModel PhongKham { get; set; }
        public GiayChuyenVienBenhNhanViewModel GiayChuyenVien { get; set; }
        public DoiTuongUuTienKhamChuaBenhBenhNhanViewModel DoiTuongUuTienKhamChuaBenh { get; set; }

        public string LyDoKhamModelText { get; set; }
        public string KhoaKhamModelText { get; set; }
        public string PhongKhamModelText { get; set; }
        public string DoiTuongUuTienModelText { get; set; }
        public bool IsOutOfDate { get; set; } = false;
        public bool? IsCheckedBHYT { get; set; }
        public Enums.EnumTinhTrangThe? TinhTrangThe { get; set; }
        public string MaBN { get; set; }
        public string MaYeuCauTiepNhan { get; set; }

        #region update tạo mới yêu cầu tiếp nhận : 
        // trường hợp BHYT : chưa đóng là không cho tạo => nút tạo mới yctn ẩn đi
        // trường hợp không BHYT (Viện phí) + trong ngày cũng không cho tạo
        //                                  + qua ngày => cho tạo  yctn mới
        public bool CheckYeuCauTiepNhanTaoMoi { get; set; }
        #endregion


        public bool? CoYeuCauGoiDichVu { get; set; }

        public long? ChuongTrinhGoiDichVuId { get; set; }

        // cập nhật điều kiện tạo mới yctn
        public string MessageKhongChoPhepTaoMoiYCTN { get; set; }

        public string BieuHienLamSang { get; set; }

        public string DichTeSarsCoV2 { get; set; }


        //BVHD-3800
        public bool? LaCapCuu { get; set; }
    }

    public class BenhNhanTiepNhanBenhNhanViewModel : BaseViewModel
    {
        #region update v2

        public BenhNhanTiepNhanBenhNhanViewModel()
        {
            BaoHiemTuNhans = new List<BaoHiemTuNhanViewModel>();
        }
        public bool? CoBHYT { get; set; }
        public bool? BHYTDuocMienCungChiTra { get; set; }
        public long? BHYTGiayMienCungChiTraId { get; set; }
        public GiayMienCungChiTraViewModel BHYTGiayMienCungChiTra { get; set; }
        public DateTime? BHYTNgayDuocMienCungChiTra { get; set; }
        public string BHYTMaSoThe { get; set; }
        public string HoTen { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public DateTime? NgayThangNamSinh { get; set; }
        public string NgayThangNamSinhDisplay { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public long? BHYTNoiDangKyId { get; set; }
        public string NoiDangKyBHYT { get; set; }
        public string BHYTDiaChi { get; set; }
        public DateTime? BHYTNgayHieuLuc { get; set; }
        public DateTime? BHYTNgayHetHan { get; set; }
        public bool? CoBHTN { get; set; }
        //public long? BHTNCongTyBaoHiemId { get; set; }
        //public CongTyBaoHiemTuNhanViewModel BHTNCongTyBaoHiem { get; set; }
        //public string BHTNMaSoThe { get; set; }
        //public DateTime? BHTNNgayHieuLuc { get; set; }
        //public DateTime? BHTNNgayHetHan { get; set; }
        //public string BHTNSoDienThoai { get; set; }
        //public string BHTNDiaChi { get; set; }
        public List<BaoHiemTuNhanViewModel> BaoHiemTuNhans { get; set; }
        #endregion update v2

        #region Thong tin bo sung

        public string SoChungMinhThu { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public long? DanTocId { get; set; }
        public long? QuocTichId { get; set; }
        public long? TinhThanhId { get; set; }
        public long? QuanHuyenId { get; set; }
        public long? PhuongXaId { get; set; }
        public string DiaChi { get; set; }
        public long? NgheNghiepId { get; set; }
        public string NoiLamViec { get; set; }

        public string NguoiLienHeHoTen { get; set; }
        public long? NguoiLienHeQuanHeNhanThanId { get; set; }
        public string NguoiLienHeSoDienThoai { get; set; }
        public string NguoiLienHeEmail { get; set; }

        #endregion Thong tin bo sung
        //public DonViHanhChinhBenhNhanViewModel BHYTNoiDangKy { get; set; }

        public string DanTocModelText { get; set; }
        public string QuocTichModelText { get; set; }
        public string TinhThanhModelText { get; set; }
        public string QuanHuyenModelText { get; set; }
        public string PhuongXaModelText { get; set; }
        public string NgheNghiepModelText { get; set; }
        public string NguoiLienHeThanNhanModelText { get; set; }

        public string BHYTCoQuanBHXH { get; set; }
        public DateTime? BHYTNgayDu5Nam { get; set; }
        public string BHYTNgayDu5NamDisplay { get; set; }
        public string BHYTMaDKBD { get; set; }
        public string MaBN { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public bool? CoYeuCauGoiDichVu { get; set; }

        #region Thong tin bổ sung  HinhThucDenId ,NoiGioiThieuId

        public long? HinhThucDenId { get; set; }
        public long? NoiGioiThieuId { get; set; }

        #endregion bổ sung  HinhThucDenId , NoiGioiThieuId
    }

    public class BaoHiemTuNhanViewModel : BaseViewModel
    {
        public long? YeuCauTiepNhanId { get; set; }
        public long? BHTNCongTyBaoHiemId { get; set; }
        public string CongTyDisplay { get; set; }
        public CongTyBaoHiemTuNhanViewModel BHTNCongTyBaoHiem { get; set; }
        public string BHTNMaSoThe { get; set; }
        public DateTime? BHTNNgayHieuLuc { get; set; }
        public DateTime? BHTNNgayHetHan { get; set; }
        public string BHTNSoDienThoai { get; set; }
        public string BHTNDiaChi { get; set; }
        public long? BenhNhanId { get; set; }
    }
    public class DonViHanhChinhBenhNhanViewModel : BaseViewModel
    {
        public string Ten { get; set; }
        public Enums.CapHanhChinh? CapHanhChinh { get; set; }
        public string TenDonViHanhChinh { get; set; }
        public Enums.VungDiaLy? VungDiaLy { get; set; }
        public string TenVietTat { get; set; }
    }

    public class LyDoKhamBenhBenhNhanViewModel : BaseViewModel
    {
        public string Ten { get; set; }
        public string TenVietTat { get; set; }
        public string MoTa { get; set; }
        public bool? IsDisabled { get; set; }
    }

    public class PhongNgoaiTruBenhNhanViewModel : BaseViewModel
    {
        public long? KhoaPhongId { get; set; }

        public string Ten { get; set; }

        public string Ma { get; set; }

        public bool? IsDisabled { get; set; }
    }

    public class GiayChuyenVienBenhNhanViewModel : TaiLieuDinhKemViewModel
    {

    }

    public class GiayMienCungChiTraViewModel : TaiLieuDinhKemViewModel
    {

    }

    public class HoSoYeuCauTiepNhanViewModel : TaiLieuDinhKemViewModel
    {
        public string LoaiDisplay { get; set; }
        public long? LoaiHoSoYeuCauTiepNhanId { get; set; }
        public long? YeuCauTiepNhanId { get; set; }
    }


    public class DoiTuongUuTienKhamChuaBenhBenhNhanViewModel : BaseViewModel
    {
        public string Ten { get; set; }

        public string TenVietTat { get; set; }
        public int? ThuTuUuTien { get; set; }
        public string MoTa { get; set; }
        public bool? IsDisabled { get; set; }
    }

    //public class DoiTuongUuDaiViewModel : BaseViewModel
    //{
    //    public string Ten { get; set; }
    //    public int? TiLeUuDai { get; set; }
    //    public string MoTa { get; set; }
    //    public bool IsDisabled { get; set; }
    //}

    public class CongTyUuDaiViewModel : BaseViewModel
    {
        public string Ten { get; set; }
        public string MoTa { get; set; }
        public bool? IsDisabled { get; set; }
    }

    public class CongTyBaoHiemTuNhanViewModel : BaseViewModel
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public int? HinhThucBaoHiem { get; set; }
        public int? PhamViBaoHiem { get; set; }
        public string GhiChu { get; set; }
    }


    public class YeuCauDichVuKyThuatTiepNhanViewModel : BaseViewModel
    {
        public YeuCauDichVuKyThuatTiepNhanViewModel()
        {
            CongTyBaoHiemTuNhanCongNos = new List<CongTyBaoHiemTuNhanCongNoViewModel>();
        }
        public decimal? DonGiaBaoHiem { get; set; }

        public int? MucHuongBaoHiem { get; set; }
        public NhomGiaDichVuKyThuatBenhVienViewModel NhomGiaDichVuKyThuatBenhVien { get; set; }
        public long? NhomGiaDichVuKyThuatBenhVienId { get; set; }
        public long? YeuCauTiepNhanId { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public Enums.LoaiDichVuKyThuat? LoaiDichVuKyThuat { get; set; }
        public long? DichVuKyThuatBenhVienId { get; set; }
        public string MaDichVu { get; set; }
        public string MaGiaDichVu { get; set; }
        public string Ma4350DichVu { get; set; }
        public string TenDichVu { get; set; }
        public string TenTiengAnhDichVu { get; set; }
        public string TenGiaDichVu { get; set; }
        public Enums.EnumDanhMucNhomTheoChiPhi? NhomChiPhi { get; set; }
        public Enums.LoaiPhauThuatThuThuat? LoaiPhauThuatThuThuat { get; set; }
        public decimal? Gia { get; set; }
        public int? TiLeUuDai { get; set; }
        public int? SoLan { get; set; }
        public bool DuocHuongBaoHiem { get; set; }
        public bool BaoHiemChiTra { get; set; }
        public DateTime? ThoiDiemDuyetBaoHiem { get; set; }
        public long? NhanVienDuyetBaoHiemId { get; set; }
        public decimal? GiaBaoHiemThanhToan { get; set; }
        public string ThongTu { get; set; }
        public string QuyetDinh { get; set; }
        public string NoiBanHanh { get; set; }
        public string MoTa { get; set; }
        public Enums.EnumTrangThaiYeuCauDichVuKyThuat? TrangThai { get; set; }
        public long? NhanVienChiDinhId { get; set; }
        public long? NoiChiDinhId { get; set; }
        public DateTime? ThoiDiemChiDinh { get; set; }
        public string TenNhanVienChiDinh { get; set; }
        public DateTime? ThoiDiemDangKy { get; set; }
        public long? NoiThucHienId { get; set; }
        public bool DaThanhToan { get; set; }
        public long? NoiThanhToanId { get; set; }
        public long? NhanVienThanhToanId { get; set; }
        public DateTime? ThoiDiemThanhToan { get; set; }
        public string MaGiuong { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public DateTime? ThoiDiemHoanThanh { get; set; }
        public long? NhanVienThucHienId { get; set; }
        public string KetLuan { get; set; }
        public long? NhanVienKetLuanId { get; set; }
        public string DeNghi { get; set; }

        public long? YeuCauGoiDichVuId { get; set; }
        public decimal? SoTienBenhNhanDaChi { get; set; }
        public Enums.TrangThaiThanhToan? TrangThaiThanhToan { get; set; }

        public string LyDoHuyThanhToan { get; set; }

        public long? NhanVienHuyThanhToanId { get; set; }

        //Update 14/08/2020
        public decimal? SoTienMienGiam { get; set; }
        public decimal? SoTienBaoHiemTuNhanChiTra { get; set; }
        public List<CongTyBaoHiemTuNhanCongNoViewModel> CongTyBaoHiemTuNhanCongNos { get; set; }

        // cập nhật hủy dịch vụ dã hủy thanh toán
        public bool IsDichVuHuyThanhToan { get; set; }
        public string LyDoHuyDichVu { get; set; }


        // cập nhật kiểm tra dịch vụ khác 4 nhóm: PTTT, CDHA, TDCN, XN thì cho phép hoàn thành, hủy hoàn thành
        public string LyDoHuyTrangThaiDaThucHien { get; set; }

        public long? NhomDichVuBenhVienId { get; set; }
        public bool? LaDichVuVacxin { get; set; }
    }

    //public class NhomGiaDichVuKhamBenhBenhVienViewModel : BaseViewModel
    //{
    //    public string Ten { get; set; }
    //}
    //public class NhomGiaDichVuKyThuatBenhVienViewModel : BaseViewModel
    //{
    //    public string Ten { get; set; }
    //}

    public class YeuCauKhamBenhTiepNhanViewModel : BaseViewModel
    {
        public YeuCauKhamBenhTiepNhanViewModel()
        {
            CongTyBaoHiemTuNhanCongNos = new List<CongTyBaoHiemTuNhanCongNoViewModel>();
        }

        public decimal? DonGiaBaoHiem { get; set; }

        public int? MucHuongBaoHiem { get; set; }

        public NhomGiaDichVuKhamBenhBenhVienViewModel NhomGiaDichVuKhamBenhBenhVien { get; set; }
        public long? NhomGiaDichVuKhamBenhBenhVienId { get; set; }
        public long? YeuCauTiepNhanId { get; set; }
        public long? DichVuKhamBenhBenhVienId { get; set; }
        public long? YeuCauKhamBenhTruocId { get; set; }

        public string LoiDanCuaBacSi { get; set; }

        public string MaDichVu { get; set; }
        public string MaDichVuTt37 { get; set; }
        public string TenDichVu { get; set; }
        public decimal? Gia { get; set; }
        public int? TiLeUuDai { get; set; }
        public long? GoiDichVuId { get; set; }
        public int? TiLeChietKhau { get; set; }
        public string MoTa { get; set; }
        public bool DuocHuongBaoHiem { get; set; }
        public bool BaoHiemChiTra { get; set; }
        public DateTime? ThoiDiemDuyetBaoHiem { get; set; }
        public long? NhanVienDuyetBaoHiemId { get; set; }
        public decimal? GiaBaoHiemThanhToan { get; set; }
        public Enums.EnumTrangThaiYeuCauKhamBenh? TrangThai { get; set; }
        public long? NhanVienChiDinhId { get; set; }
        public long? NoiChiDinhId { get; set; }
        public DateTime? ThoiDiemChiDinh { get; set; }

        public DateTime? ThoiDiemDangKy { get; set; }
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

        public long? YeuCauGoiDichVuId { get; set; }

        public decimal? SoTienBenhNhanDaChi { get; set; }

        public Enums.TrangThaiThanhToan TrangThaiThanhToan { get; set; }
        public long? NhanVienHuyThanhToanId { get; set; }

        public string LyDoHuyThanhToan { get; set; }

        public bool? KhongTinhPhi { get; set; }

        //Update 30/03/2020
        public string GhiChuTrieuChungLamSang { get; set; }

        //Update 14/08/2020
        public decimal? SoTienMienGiam { get; set; }
        public decimal? SoTienBaoHiemTuNhanChiTra { get; set; }
        public List<CongTyBaoHiemTuNhanCongNoViewModel> CongTyBaoHiemTuNhanCongNos { get; set; }

        // cập nhật hủy dịch vụ cho những dv đã hủy thanh toán
        public bool IsDichVuHuyThanhToan { get; set; }
        public string LyDoHuyDichVu { get; set; }
        public string TenNhanVienChiDinh { get; set; }
    }

    public class CongTyBaoHiemTuNhanCongNoViewModel : BaseViewModel
    {
        public long? TaiKhoanBenhNhanThuId { get; set; }
        public long? CongTyBaoHiemTuNhanId { get; set; }
        public decimal? SoTien { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public long? YeuCauDichVuKyThuatId { get; set; }
        public long? YeuCauDuocPhamBenhVienId { get; set; }
        public long? YeuCauVatTuBenhVienId { get; set; }
        public long? YeuCauDichVuGiuongBenhVienId { get; set; }
        public long? DonThuocThanhToanChiTietId { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }
    }

    public class YeuCauVatTuBenhVienTiepNhanViewModel : BaseViewModel
    {
        public YeuCauVatTuBenhVienTiepNhanViewModel()
        {
            CongTyBaoHiemTuNhanCongNos = new List<CongTyBaoHiemTuNhanCongNoViewModel>();
        }

        public decimal? DonGiaBaoHiem { get; set; }

        public int? MucHuongBaoHiem { get; set; }

        public long? YeuCauTiepNhanId { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public long? YeuCauDichVuKyThuatId { get; set; }
        public long? VatTuBenhVienId { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }
        public long? NhomVatTuId { get; set; }
        public string DonViTinh { get; set; }
        public string NhaSanXuat { get; set; }
        public string NuocSanXuat { get; set; }
        public string QuyCach { get; set; }
        public string TieuChuan { get; set; }
        public string MoTa { get; set; }
        public bool? KhongTinhPhi { get; set; }
        public decimal? Gia { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }
        public double? SoLuong { get; set; }
        public long? NhanVienChiDinhId { get; set; }
        public long? NoiChiDinhId { get; set; }
        public DateTime? ThoiDiemChiDinh { get; set; }
        public long? NoiCapVatTuId { get; set; }
        public long? NhanVienCapVatTuId { get; set; }
        public DateTime? ThoiDiemCapVatTu { get; set; }
        public bool? DuocHuongBaoHiem { get; set; }
        public bool? BaoHiemChiTra { get; set; }
        public DateTime? ThoiDiemDuyetBaoHiem { get; set; }
        public long? NhanVienDuyetBaoHiemId { get; set; }
        public decimal? GiaBaoHiemThanhToan { get; set; }
        public decimal? SoTienBenhNhanDaChi { get; set; }
        public Enums.TrangThaiThanhToan? TrangThaiThanhToan { get; set; }
        public long? NhanVienHuyThanhToanId { get; set; }
        public string LyDoHuyThanhToan { get; set; }
        public bool? DaCapVatTu { get; set; }
        public Enums.EnumYeuCauVatTuBenhVien? TrangThai { get; set; }
        public string GhiChu { get; set; }

        //Update 14/08/2020
        public decimal? SoTienMienGiam { get; set; }
        public List<CongTyBaoHiemTuNhanCongNoViewModel> CongTyBaoHiemTuNhanCongNos { get; set; }
    }

    public class YeuCauDuocPhamBenhVienTiepNhanViewModel : BaseViewModel
    {
        public YeuCauDuocPhamBenhVienTiepNhanViewModel()
        {
            CongTyBaoHiemTuNhanCongNos = new List<CongTyBaoHiemTuNhanCongNoViewModel>();
        }

        public decimal? DonGiaBaoHiem { get; set; }

        public int? MucHuongBaoHiem { get; set; }
        public long? YeuCauTiepNhanId { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public long? YeuCauDichVuKyThuatId { get; set; }
        public long? DuocPhamBenhVienId { get; set; }
        public long? XuatKhoDuocPhamChiTietViTriId { get; set; }
        public string Ten { get; set; }
        public string TenTiengAnh { get; set; }
        public string SoDangKy { get; set; }
        public int? STTHoatChat { get; set; }
        //public int NhomChiPhi { get; set; }
        public string MaHoatChat { get; set; }
        public string HoatChat { get; set; }
        public Enums.LoaiThuocHoacHoatChat? LoaiThuocHoacHoatChat { get; set; }
        public string NhaSanXuat { get; set; }
        public string NuocSanXuat { get; set; }
        public long? DuongDungId { get; set; }
        public string HamLuong { get; set; }
        public string QuyCach { get; set; }
        public string TieuChuan { get; set; }
        public string DangBaoChe { get; set; }
        public long? DonViTinhId { get; set; }
        public string HuongDan { get; set; }
        public string MoTa { get; set; }
        public string ChiDinh { get; set; }
        public string ChongChiDinh { get; set; }
        public string LieuLuongCachDung { get; set; }
        public string TacDungPhu { get; set; }
        public string ChuYdePhong { get; set; }
        public long? HopDongThauDuocPhamId { get; set; }
        public long? NhaThauId { get; set; }
        public string SoHopDongThau { get; set; }
        public string SoQuyetDinhThau { get; set; }
        public Enums.EnumLoaiThau? LoaiThau { get; set; }
        public Enums.EnumLoaiThuocThau? LoaiThuocThau { get; set; }
        public string NhomThau { get; set; }
        public string GoiThau { get; set; }
        public int? NamThau { get; set; }
        public bool? KhongTinhPhi { get; set; }
        public decimal? Gia { get; set; }
        //public long? GoiDichVuId { get; set; }
        //public int? TiLeChietKhau { get; set; }
        public double? SoLuong { get; set; }
        public long? NhanVienChiDinhId { get; set; }
        public long? NoiChiDinhId { get; set; }
        public DateTime? ThoiDiemChiDinh { get; set; }
        public long? NoiCapThuocId { get; set; }
        public long? NhanVienCapThuocId { get; set; }
        public DateTime? ThoiDiemCapThuoc { get; set; }
        public bool? DuocHuongBaoHiem { get; set; }
        public bool? BaoHiemChiTra { get; set; }
        public DateTime? ThoiDiemDuyetBaoHiem { get; set; }
        public long? NhanVienDuyetBaoHiemId { get; set; }
        public decimal? GiaBaoHiemThanhToan { get; set; }
        //public bool DaThanhToan { get; set; }
        //public long? NoiThanhToanId { get; set; }
        //public long? NhanVienThanhToanId { get; set; }
        //public DateTime? ThoiDiemThanhToan { get; set; }
        public bool? DaCapThuoc { get; set; }
        public Enums.EnumYeuCauDuocPhamBenhVien? TrangThai { get; set; }
        public string GhiChu { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }

        public decimal? SoTienBenhNhanDaChi { get; set; }
        public Enums.TrangThaiThanhToan? TrangThaiThanhToan { get; set; }

        public long? NhanVienHuyThanhToanId { get; set; }

        public string LyDoHuyThanhToan { get; set; }

        //Update 14/08/2020
        public decimal? SoTienMienGiam { get; set; }
        public List<CongTyBaoHiemTuNhanCongNoViewModel> CongTyBaoHiemTuNhanCongNos { get; set; }
    }
}