using System;
using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.YeuCauKhamBenh
{
    public class KhoaKhamTemplateVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }
    }

    public class PhongKhamTemplateVo
    {
        public int Tong { get; set; }
        public int DangKham { get; set; }
        public string TenBacSi { get; set; }
        public string DisplayName { get; set; }
        public long BacSiId { get; set; }
        public long PhongKhamId { get; set; }
        public string KeyId { get; set; }
        public string MaPhong { get; set; }
        public int TongSoKhamGioiHan { get; set; }
        public bool IsWarning { get; set; }
        public string TenPhong { get; set; }
    }

    public class PhongKhamDVKTTemplateVo
    {
        public string KeyId { get; set; }
        public string DisplayName { get; set; }
        public int Tong { get; set; }
        public string MaPhong { get; set; }
        public string TenPhong { get; set; }
    }

    public class TinhThanhTemplateVo
    {
        public string DisplayName { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public long KeyId { get; set; }
    }

    public class MaDichVuTemplateVo
    {
        public string DisplayName { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenTt43 { get; set; }
        public string Loai { get; set; }
        public long KeyId { get; set; }
    }

    public class GoiDichVuTemplateVo
    {
        public string DisplayName { get; set; }
        public string Ten { get; set; }
        public string ChietKhau { get; set; }
        public bool IsCoChietKhau { get; set; }
        public long KeyId { get; set; }
    }

    public class ThemTaiLieu
    {
        public long? LoaiId { get; set; }
        public string MoTa { get; set; }
        public TaiLieuModel TaiLieu { get; set; }
    }

    public class TaiLieuModel
    {
        public long? Id { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenGuid { get; set; }
        public long? KichThuoc { get; set; }
        public string DuongDan { get; set; }
        public string MoTa { get; set; }
        public Enums.LoaiTapTin? LoaiTapTin { get; set; }
    }

    public class QuanHuyenTinhThanhModel
    {
        public long? QuanHuyenId { get; set; }
        public long? TinhThanhId { get; set; }
    }

    public class noiChiDinhDVModel
    {
        public long? BacSiThucHienId { get; set; }
        public long? NoiThucHienId { get; set; }
    }

    public class modelUpdateView
    {
        public long YeuCauTiepNhanId { get; set; }
        public long BenhNhanid { get; set; }
        public int? MucHuongBHYT { get; set; }
        public List<ThemDichVuKhamBenhVo> Data { get; set; }
        public List<ListDichVuCheckTruocDo> ListDichVuCheckTruocDos { get; set; }
    }

    public class ThemDichVuKhamBenhVo
    {
        public long? MaDichVuGoiId { get; set; }
        public long? MaDichVuId { get; set; }
        public long? LoaiGiaId { get; set; }
        public string NoiThucHienId { get; set; }
        public int? SoLuong { get; set; }
        public double? DonGia { get; set; }
        public double? ThanhTien { get; set; }
        public int? BHYTMucHuong { get; set; }
        public long? DoiTuongUuDaiId { get; set; }
        public bool DuocHuongBHYT { get; set; }
        public bool LaGoi { get; set; }

        public long? YeuCauTiepNhanId { get; set; }

        public long? BenhNhanId { get; set; }
        public long? ChuongTrinhGoiDichVuId { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }
        public string TenNhomDichVu { get; set; }
        public List<ListDichVuCheckTruocDo> ListDichVuCheckTruocDos { get; set; }
    }

    public class ThemDichVuKyThuatVo
    {
        public long? MaDichVuId { get; set; }
        public string NoiThucHienId { get; set; }
        public int? SoLuong { get; set; }
        public double? DonGia { get; set; }
        public double? ThanhTien { get; set; }
        public long? DoiTuongUuDaiId { get; set; }
        public long? LoaiGiaId { get; set; }
    }

    public class DiaChiBHYT
    {
        public string TinhThanh { get; set; }
        public long? TinhThanhId { get; set; }
        public string QuanHuyen { get; set; }
        public long? QuanHuyenId { get; set; }
        public string PhuongXa { get; set; }
        public long? PhuongXaId { get; set; }
    }
    public class ThemBaoHiemTuNhan
    {
        public long? Id { get; set; }
        public long? CongTyBaoHiemTuNhanId { get; set; }
        public string MaSoThe { get; set; }
        public DateTime? NgayHieuLuc { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }
    }

    public class ThemBaoHiemTuNhanGridVo
    {
        public long Id { get; set; }
        public long? CongTyBaoHiemTuNhanId { get; set; }
        public string CongTyDisplay { get; set; }
        public string MaSoThe { get; set; }
        public DateTime? NgayHieuLuc { get; set; }
        public string NgayHieuLucDisplay { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public string NgayHetHanDisplay { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }
    }

    public class AddGoiForUpdateView
    {
        public long YeuCauTiepNhanId { get; set; }
        public int? MucHuong { get; set; }
        public List<ChiDinhDichVuGridVo> LstGrid { get; set; }
    }

    public class DichVuNeedUpdate
    {
        public long id { get; set; }
        public string nhom { get; set; }
    }

    public class ChiDinhDichVuGridVo
    {
        public string TinhTrangDisplay { get; set; }
        public EnumTrangThaiYeuCauKhamBenh? TrangThaiYeuCauKhamBenh { get; set; }
        public EnumTrangThaiYeuCauDichVuKyThuat? TrangThaiYeuCauDichVuKyThuat { get; set; }
        public EnumTrangThaiGiuongBenh? TrangThaiGiuongBenh { get; set; }

        public bool IsDontHavePermissionChangeNoiThucHien { get; set; }
        public bool CoGiaBHYT { get; set; }
        public long? Id { get; set; }
        public long MaDichVuId { get; set; }
        public string Ma { get; set; }
        public string TenDichVu { get; set; }
        public string LoaiGia { get; set; }
        public long LoaiGiaId { get; set; }
        public int? SoLuong { get; set; }
        public string DonGiaDisplay { get; set; }
        public double DonGia { get; set; }
        public double? DonGiaSauChietKhau { get; set; }
        public string ThanhTienDisplay { get; set; }
        public double ThanhTien { get; set; }
        public double ThanhTienSauChietKhau { get; set; }
        public string BHYTThanhToanDisplay { get; set; }
        public string TLMGDisplay { get; set; }
        public double TLMG { get; set; }
        public string SoTienMGDisplay { get; set; }
        public double SoTienMG { get; set; }
        public string BnThanhToanDisplay { get; set; }
        public double BnThanhToan { get; set; }
        public string NoiThucHienDisplay { get; set; }
        public string NoiThucHienId { get; set; }
        public decimal BHYTThanhToan { get; set; }
        public decimal BHYTThanhToanChuaBaoGomMucHuong { get; set; }
        public string Nhom { get; set; }
        // thêm cái nhóm id để in chỉ định
        public EnumNhomGoiDichVu? NhomId { get; set; }

        public bool IsHaveNoiThucHien { get; set; }

        public bool DuocHuongBHYT { get; set; }

        public bool DuocHuongBHYTPopup { get; set; }

        //Goi co chiet khau
        public bool IsGoiCoChietKhau { get; set; }
        public long? GoiCoChietKhauId { get; set; }
        public string TenGoiChietKhau { get; set; }
        public bool? IsDichVuTrongGoi { get; set; }
        public long? GoiCoChietKhauIdTemp { get; set; }
        public bool? LaDichVuKhuyenMai { get; set; }
        public bool? CoDichVuNayTrongGoiKhuyenMai { get; set; }
        public bool? CoDichVuNayTrongGoi { get; set; }

        public double TyLeChietKhau { get; set; }
        public string TyLeChietKhauDisplay { get; set; }
        public double DuocGiamTrongGoi { get; set; }
        public double ThanhTienTrongGoi { get; set; }
        public double TongChiPhiGoi { get; set; }

        public long? KhoaPhongId { get; set; }

        //update 10/4/2020
        public double? GiaBHYT { get; set; }
        public string GiaBHYTDislay { get; set; }
        public int? TiLeBaoHiemThanhToan { get; set; }

        public string NoiThucHienModelText { get; set; }

        // Thach can them 2 columns cong no va mien giam
        public decimal? CongNo { get; set; }
        public decimal? SoTienMienGiam { get; set; }

        public long? ChuongTrinhGoiDichVuId { get; set; }

        public int? SoLuongConLai { get; set; }

        //Update code thêm nơi chỉ định khách hàng
        public string TenNhanVienChiDinh { get; set; }

        // cập nhật hủy dịch vụ cho những dv đã hủy thanh toán
        public bool IsDichVuHuyThanhToan { get; set; }
        public string LyDoHuyDichVu { get; set; }

        public DateTime? ThoiDiemChiDinh { get; set; }
        public string ThoiGianChiDinhDisplay => ThoiDiemChiDinh?.ApplyFormatDateTime();

        // cập nhật kiểm tra dịch vụ khác 4 nhóm: PTTT, CDHA, TDCN, XN thì cho phép hoàn thành, hủy hoàn thành
        // BVHD-3268: bổ sung them nhóm khàm sàng lọc tiêm chủng
        public LoaiDichVuKyThuat? LoaiDichVuKyThuat { get; set; }

        public bool ShowButtonHoanThanhDichVu => LoaiDichVuKyThuat != null
                                                 && LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat
                                                 && LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh
                                                 && LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ThamDoChucNang
                                                 && LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.XetNghiem
                                                 && LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.SangLocTiemChung
                                                 && TrangThaiYeuCauDichVuKyThuat != null 
                                                 && TrangThaiYeuCauDichVuKyThuat != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                 && TrangThaiYeuCauDichVuKyThuat != EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien;
        public bool ShowButtonHuyHoanThanhDichVu => LoaiDichVuKyThuat != null
                                                    && LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat
                                                    && LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh
                                                    && LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ThamDoChucNang
                                                    && LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.XetNghiem
                                                    && LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.SangLocTiemChung
                                                    && TrangThaiYeuCauDichVuKyThuat != null
                                                    && TrangThaiYeuCauDichVuKyThuat == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien;
        public string LyDoHuyTrangThaiDaThucHien { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public bool isCheckRowItem { get; set; }
        public long? DichVuBenhVienId { get; set; }
        public bool LaDichVuVacxin { get; set; }

        //BVHD-3825
        public long? ChuongTrinhGoiDichVuKhuyenMaiId { get; set; }
        public long? YeuCauGoiDichVuKhuyenMaiId { get; set; }

        //Cập nhật được hưởng BH
        public bool? LaChiDinhTuKhamBenh { get; set; }
    }

    public class MienGiamCongNoVo
    {
        public decimal? CongNo { get; set; }
        public decimal? SoTienMienGiam { get; set; }
    }

    public class ChiDinhDichVuKyThuatGridVo
    {
        public long MaDichVuId { get; set; }
        public string LoaiGia { get; set; }
        public long LoaiGiaId { get; set; }
        public string Ma { get; set; }
        public string TenDichVu { get; set; }
        public int SoLuong { get; set; }
        public string DonGiaDisplay { get; set; }
        public double DonGia { get; set; }
        public string ThanhTienDisplay { get; set; }
        public double ThanhTien { get; set; }
        public string TLMGDisplay { get; set; }
        public double TLMG { get; set; }
        public string SoTienMGDisplay { get; set; }
        public double SoTienMG { get; set; }
        public string BnThanhToanDisplay { get; set; }
        public double BnThanhToan { get; set; }
        public string NoiThucHienDisplay { get; set; }
        public string NoiThucHienId { get; set; }
        public string Nhom { get; set; }

        public long? KhoaPhongId { get; set; }

        //update 10/4/2020
        public double? GiaBHYT { get; set; }
        public int? TiLeBaoHiemThanhToan { get; set; }
        public bool DuocHuongBHYT { get; set; }

        //BVHD-3825
        public long? ChuongTrinhGoiDichVuKhuyenMaiId { get; set; }
        public long? YeuCauGoiDichVuKhuyenMaiId { get; set; }
        public bool? LaDichVuKhuyenMai { get; set; }
        public decimal? SoTienMienGiam { get; set; }
        public EnumNhomGoiDichVu? NhomId { get; set; }
    }


    public class GetDonGiaVo
    {
        public long? DichVuKhamBenhBenhVienId { get; set; }
        public long? NhomGiaDichVuKhamBenhBenhVienId { get; set; }
        public bool? IsCheckValidDonGia { get; set; }
        public decimal? DonGia { get; set; }
    }

    public class ThongTinTaiKhoanBenhNhan
    {
        public string MaBenhNhan { get; set; }
        public decimal SoDuTaiKhoan { get; set; }
    }

    public class DanhSachGoiChon
    {
        public string TenChuongTrinh { get; set; }
        public string TenGoiDichVu { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }
        public long ChuongTrinhGoiDichVuId { get; set; }
        public bool IsFromMarketing { get; set; }
        public long BenhNhanId { get; set; }
        public bool DaThanhToan { get; set; }
        public long? YeuCauTiepNhanId { get; set; }
    }

    public class DichVuTrongGoiKhiThem
    {
        public string TenChuongTrinh { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }
        public long ChuongTrinhGoiDichVuId { get; set; }
        public bool IsFromMarketing { get; set; }
        public long BenhNhanId { get; set; }

        public string TenDichVu { get; set; }

        public ChiDinhDichVuGridVo Data { get; set; }
    }

    public class DanhSachDichVuChonTrongLanPopup
    {
        public int? SoLan { get; set; }
        public string TenDichVu { get; set; }
        public long DichVuId { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }
        public long ChuongTrinhGoiDichVuId { get; set; }
        public bool? IsFromMarketing { get; set; }
        public long? BenhNhanId { get; set; }
        public string TenNhomDichVu { get; set; }
        public int SoLuongDungLanNay { get; set; }

        public int? SoLuongConLai { get; set; }
        public string ThuocGoi { get; set; }

    }

    public class DefaultValueTNBNModel
    {
        public long? DanTocId { get; set; }
        public long? HinhThucDenId { get; set; }
        public long? LyDoTiepNhanId { get; set; }
        public long? QuocTichId { get; set; }
        public long? TinhThanhPhoId { get; set; }
    }

    public class CheckDuSoLuongTonTrongGoi
    {
        public CheckDuSoLuongTonTrongGoi()
        {
            DanhSachDichVuChonTrongLanPopup = new List<DanhSachDichVuChonTrongLanPopup>();
        }
        public List<DanhSachDichVuChonTrongLanPopup> DanhSachDichVuChonTrongLanPopup { get; set; }
        public ThemDichVuKhamBenhVo DichVuThem { get; set; }
    }

    public class CheckDuSoLuongTonTrongGoiListDichVu
    {
        public CheckDuSoLuongTonTrongGoiListDichVu()
        {
            LstDichVuThem = new List<ThemDichVuKhamBenhVo>();
        }
        public List<ThemDichVuKhamBenhVo> LstDichVuThem { get; set; }
    }

    public class DanhSachDichVuGoiChon
    {
        public string TenChuongTrinh { get; set; }
        public long DichVuId { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }
        public long ChuongTrinhGoiDichVuId { get; set; }
        public int SoLanKhamTrongYCTN { get; set; }
        public int SoLanKhamNgoaiYCTN { get; set; }

        public bool DichVuKhamBenh { get; set; }
        public bool DichVuKyThuat { get; set; }
        public bool DichVuGiuongBenh { get; set; }
        public bool IsFromMarketing { get; set; }
    }

    public class TimKiemBenhNhanGridVo
    {
        public long? Id { get; set; }
        public string MaBN { get; set; }
        public string MaBHYT{ get; set; }
        public string HoTen { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string NgaySinhDisplay  { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string GioiTinhDisplay { get; set; }
        public string SoChungMinhThu { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }

        public int? NamSinh { get; set; }
    }

    public class TimKiemBenhNhanSearch
    {
        public string MaBenhNhan { get; set; }
        public string MaBHYT { get; set; }
        public string HoTen { get; set; }
        public string NgaySinh { get; set; }
        public DateTime? NgaySinhFormat { get; set; }
        //public NgaySinhFormat: Date = null,
        public string CMND { get; set; }
        public string DienThoai { get; set; }
        public string DiaChi { get; set; }

        public int? NamSinh { get; set; }
    }

    public class TimKiemBenhNhanPopup
    {
        public TimKiemBenhNhanGridVo searchBenhNhan { get; set; }
        public TimKiemBenhNhanSearch searchPopup { get; set; }
    }

    #region TNBN update

    public class DichVuOld
    {
        public long Id { get; set; }
        public decimal? GiaBaoHiemThanhToan { get; set; }
        public bool DuocHuongBaoHiem { get; set; }
        public string Nhom { get; set; }
    }


    public class ListChiDinhNeedUpdate
    {
        public string Nhom { get; set; }
        public long DichVuId { get; set; }
    }
    #endregion TNBN update

    #region update chỉ định nhóm dịch vụ thường dùng

    public class ChiDinhDichVuTrongNhomThuongDungVo
    {
        public ChiDinhDichVuTrongNhomThuongDungVo()
        {
            DichVuKhamBenhs = new List<ChiDinhDichVuGridVo>();
            DichVuKyThuats = new List<ChiDinhDichVuKyThuatGridVo>();
        }
        public List<ChiDinhDichVuGridVo> DichVuKhamBenhs { get; set; }
        public List<ChiDinhDichVuKyThuatGridVo> DichVuKyThuats { get; set; }
    }
    

    #endregion
    public class ListDichVuCheckTruocDo
    {
        public long Id { get; set; }
        public long NhomId { get; set; }
    }
}