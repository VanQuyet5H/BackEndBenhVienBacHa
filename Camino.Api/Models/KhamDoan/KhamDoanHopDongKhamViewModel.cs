using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.KhamDoan;
using System;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.KhamDoan
{
    public class KhamDoanHopDongKhamViewModel : BaseViewModel
    {
        KhamDoanHopDongKhamViewModel()
        {
            HopDongKhamSucKhoeDiaDiems = new List<HopDongSucKhoeDiaDiemViewModel>();
        }
        public long? CongTyKhamSucKhoeId { get; set; }
        public string TenCongTy { get; set; }
        public string SoHopDong { get; set; }
        public DateTime? NgayHopDong { get; set; }
        public LoaiHopDong? LoaiHopDong { get; set; }
        public string TenLoaiHopDong { get; set; }
        public TrangThaiHopDongKham? TrangThaiHopDongKham { get; set; }
        public string TenTrangThaiHopDongKham { get; set; }
        public int? SoNguoiKham { get; set; }
        public double? GiaTriHopDong { get; set; }
        public decimal? ThanhToanPhatSinh { get; set; }
        public int? TiLeChietKhau { get; set; }
        public decimal? SoTienTamUng { get; set; }
        public decimal? SoTienChietKhau { get; set; }
        public decimal? GiaTriThucTe { get; set; }
        public decimal? SoTienPhatSinhThucTe { get; set; }
        public decimal? SoTienChiChoNhanVien { get; set; }
        public DateTime? NgayHieuLuc { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public string NguoiKyHopDong { get; set; }
        public string NguoiGioiThieu { get; set; }
        public string ChucDanhNguoiKy { get; set; }
        public string DienGiai { get; set; }
        public List<HopDongSucKhoeDiaDiemViewModel> HopDongKhamSucKhoeDiaDiems { get; set; }
        public List<GoiKhamSucKhoeDoanVo> GoiKhamSucKhoeDoanVos { get; set; }
    }  

    public class HopDongSucKhoeDiaDiemViewModel
    {
        public string DiaDiem { get; set; }
        public int? CongViecId { get; set; }
        public string CongViec { get; set; }
        public DateTime? Ngay { get; set; }
        public string GhiChu { get; set; }
    }

    public class DanhSachPhongKhamTaiCongTyViewModel : BaseViewModel
    {       
        public long HopDongKhamSucKhoeId { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }
        public string Tang { get; set; }
        public List<long> DanhSachNhanSu { get; set; }
    }   

    public class ImportDanhSachGoiKham
    {
        public List<DanhSachGoiKhamViewModel> DanhSachGoiKhamViewModels { get; set; }
    }

    public class DanhSachGoiKhamViewModel
    {
        public int MaGoiKham { get; set; }
        public string TenGoiKham { get; set; }
        public string TenCongTy { get; set; }
        public string SoHopDong { get; set; }
        public DateTime NgayHieuLuc { get; set; }
        public DateTime NgayKetThuc { get; set; }
    }

    public class ImportDanhSachNhanVien
    {
        public List<HopDongKhamSucKhoeNhanVienViewModel> DanhSachNhanVienViewModels { get; set; }
    }

    public class HopDongKhamExportExcel
    {

        [Width(30)]
        public string SoHopDong { get; set; }
        [Width(30)]
        public string TenCongTy { get; set; }
        [Width(20)]
        public string NgayHopDongDisplay { get; set; }
        [Width(30)]
        public string DiaChiKham { get; set; }
        [Width(20)]
        public string LoaiHopDongDisplay { get; set; }
        [Width(30)]
        public string NgayKham { get; set; }
        [Width(30)]
        public string TenTrangThai { get; set; }
    }

    public class CongTyKhamExportExcel
    {

        [Width(30)]
        public string MaCongTy { get; set; }
        [Width(30)]
        public string TenCongTy { get; set; }
        [Width(30)]
        public string LoaiCongTy { get; set; }
        [Width(30)]
        public string DiaChi { get; set; }
        [Width(30)]
        public string DienThoai { get; set; }
        [Width(30)]
        public string MaSoThue { get; set; }
        [Width(30)]
        public string TaiKhoanNganHang { get; set; }
        [Width(30)]
        public string DaiDien { get; set; }
        [Width(30)]
        public string NguoiLienHe { get; set; }
        [Width(30)]
        public bool CoHoatDong { get; set; }
        [Width(30)]
        public string TrangThai => CoHoatDong ? "Hoạt động" : "Tạm ngưng";
    }

    public class KhamDoanYeuCauNhanSuKhamSucKhoeExportExcel
    {
        [Width(30)]
        public string HopDong { get; set; }
        [Width(30)]
        public string TenCongTy { get; set; }
        [Width(30)]
        public int SoLuongBn { get; set; }
        [Width(30)]
        public int SoLuongBs { get; set; }
        [Width(30)]
        public int SoLuongDd { get; set; }
        [Width(30)]
        public int NhanVienKhac { get; set; }
        [Width(30)]
        public string NgayBatDauKhamDisplay { get; set; }
        [Width(30)]
        public DateTime NgayKetThucKham { get; set; }
        [Width(30)]
        public string NgayKetThucKhamDisplay { get; set; }
        [Width(30)]
        public DateTime NgayGui { get; set; }
        [Width(30)]
        public string NgayGuiDisplay { get; set; }
        [Width(30)]
        public string NguoiGui { get; set; }
        [Width(30)]
        public string KhthDuyet { get; set; }
        [Width(30)]
        public DateTime? NgayKhthDuyet { get; set; }
        [Width(30)]
        public string NgayKhthDuyetDisplay { get; set; }
        [Width(30)]
        public string NsDuyet { get; set; }
        [Width(30)]
        public DateTime? NgayNsDuyet { get; set; }
        [Width(30)]
        public string NgayNsDuyetDisplay { get; set; }
        [Width(30)]
        public string GdDuyet { get; set; }
        [Width(30)]
        public DateTime? NgayGdDuyet { get; set; }
        [Width(30)]
        public string NgayGdDuyetDisplay { get; set; }
        [Width(30)]
        public EnumTrangThaiKhamDoan TrangThai { get; set; }
        [Width(30)]
        public string TenTinhTrang { get; set; }
    }   
}
