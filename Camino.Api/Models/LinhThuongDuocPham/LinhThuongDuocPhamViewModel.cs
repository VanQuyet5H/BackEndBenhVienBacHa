
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using System;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.LinhThuongDuocPham
{
    public class LinhThuongDuocPhamViewModel : BaseViewModel
    {
        public LinhThuongDuocPhamViewModel()
        {
            YeuCauLinhDuocPhamChiTiets = new List<LinhThuongDuocPhamChiTietViewModel>();
        }
        public long? KhoXuatId { get; set; }
        public string TenKhoXuat { get; set; }
        public long? KhoNhapId { get; set; }
        public string TenKhoNhap { get; set; }
        public EnumLoaiPhieuLinh? LoaiPhieuLinh { get; set; }
        public long? NhanVienYeuCauId { get; set; }
        public long? NhanVienDuyetId { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public bool? DuocDuyet { get; set; }
        public string HoTenNguoiYeuCau { get; set; }
        public string HoTenNguoiDuyet { get; set; }
        public DateTime? NgayYeuCau { get; set; }
        public string GhiChu { get; set; }
        public string LyDoKhongDuyet { get; set; }
        public bool IsLuu { get; set; }
        public byte[] LastModified { get; set; }
        public bool? LaNguoiTaoPhieu { get; set; }
        public bool? DaGui { get; set; }
        public List<LinhThuongDuocPhamChiTietViewModel> YeuCauLinhDuocPhamChiTiets { get; set; }
    }

    public class LinhThuongDuocPhamChiTietViewModel : BaseViewModel
    {
        public string Nhom { get; set; }
        public long? YeuCauLinhDuocPhamId { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public string Ten { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public long KhoXuatId { get; set; }
        public bool? DuocDuyet { get; set; }
        public double? SoLuong { get; set; }
        public double? SLYeuCau { get; set; }
        public double? SLTon { get; set; }
        public double? SoLuongCoTheXuat { get; set; }
        public string HamLuong { get; set; }
        public string HoatChat { get; set; }
        public long? DuongDungId { get; set; }
        public string DuongDung { get; set; }
        public long? DVTId { get; set; }
        public string DVT { get; set; }
        public string NhaSX { get; set; }
        public string NuocSX { get; set; }
        public bool? IsValidator { get; set; }

        public string DanhSachMayXetNghiemId { get; set; }
        public string DanhSachTenMayXetNghiem { get; set; }
    }

    public class DuocPhamGridViewModel
    {
        public DuocPhamGridViewModel()
        {
            YeuCauLinhDuocPhamChiTiets = new List<LinhThuongDuocPhamChiTietViewModel>();
            DuocPhamBenhViens = new List<DuocPhamGridViewModelValidator>();
        }
        public long KhoXuatId { get; set; }
        public int? STT { get; set; }
        public string Nhom { get; set; }
        public long? DuocPhamBenhVienId { get; set; }
        public string Ten { get; set; }
        public string HamLuong { get; set; }
        public string HoatChat { get; set; }
        public long? DuongDungId { get; set; }
        public string DuongDung { get; set; }
        public long? DVTId { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public string DVT { get; set; }
        public string NhaSX { get; set; }
        public string NuocSX { get; set; }
        public double? SLYeuCau { get; set; }
        public int LoaiDuocPham { get; set; }
        public List<LinhThuongDuocPhamChiTietViewModel> YeuCauLinhDuocPhamChiTiets { get; set; }
        public List<DuocPhamGridViewModelValidator> DuocPhamBenhViens { get; set; }
    }  
}
