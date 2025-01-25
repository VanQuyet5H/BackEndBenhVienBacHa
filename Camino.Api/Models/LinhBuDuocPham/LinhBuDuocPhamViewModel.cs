using System;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.LinhBuDuocPham
{
    public class LinhBuDuocPhamViewModel : BaseViewModel
    {
        public LinhBuDuocPhamViewModel()
        {
            YeuCauLinhDuocPhamChiTiets = new List<LinhBuDuocPhamChiTietViewModel>();
            YeuCauDuocPhamBenhViens = new List<YeuCauDuocPhamBenhVienViewModel>();
        }

        public long? KhoXuatId { get; set; }
        public long? KhoNhapId { get; set; }
        public EnumLoaiPhieuLinh? LoaiPhieuLinh { get; set; }
        public long? NhanVienYeuCauId { get; set; }
        public DateTime? NgayYeuCau { get; set; }
        public string GhiChu { get; set; }
        public string TenKhoXuat { get; set; }
        public string TenKhoNhap { get; set; }
        public bool? DuocDuyet { get; set; }
        public string HoTenNguoiYeuCau { get; set; }
        public bool? DaGui { get; set; }
        public string ThoiDiemChiDinhTu { get; set; }
        public string ThoiDiemChiDinhDen { get; set; }
        public List<LinhBuDuocPhamChiTietViewModel> YeuCauLinhDuocPhamChiTiets { get; set; }
        public List<YeuCauDuocPhamBenhVienViewModel> YeuCauDuocPhamBenhViens { get; set; }
    }

    public class LinhBuDuocPhamChiTietViewModel : BaseViewModel
    {
        public string Nhom { get; set; }
        public long? YeuCauLinhDuocPhamId { get; set; }
        public long? DuocPhamBenhVienId { get; set; }
        public bool? LaDuocPhamBHYT { get; set; }
        public double? SoLuong { get; set; }
        public string Ten { get; set; }
        public bool? DuocDuyet { get; set; }
        public double? SLYeuCau { get; set; }
        public double? SoLuongCoTheXuat { get; set; }
        public string HamLuong { get; set; }
        public string HoatChat { get; set; }
        public long? DuongDungId { get; set; }
        public string DuongDung { get; set; }
        public long? DVTId { get; set; }
        public string DVT { get; set; }
        public string NhaSX { get; set; }
        public string NuocSX { get; set; }
        public double? SLCanBu { get; set; }
        public double? SLTon { get; set; }
        public double? SLYeuCauLinhThucTe { get; set; }

    }
    public class YeuCauDuocPhamBenhVienViewModel : BaseViewModel
    {
        public long? DuocPhamBenhVienId { get; set; }
        public bool? LaDuocPhamBHYT { get; set; }
        public string YeuCauDuocPhamBenhVienIds { get; set; }
        public long? KhoLinhTuId { get; set; }
        public long? KhoLinhVeId { get; set; }
        public double SoLuongCanBu { get; set; }
        public double SoLuongTon { get; set; }
        public double? SoLuongYeuCau { get; set; }
        public double? SoLuongDaBu { get; set; }
        public double? SLYeuCauLinhThucTe { get; set; }
        public double? SLYeuCauLinhThucTeMax { get; set; }
        public bool CheckBox { get; set; }
    }
}
