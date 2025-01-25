using Camino.Core.Domain.ValueObject.YeuCauDieuChuyenDuocPhams;
using System;
using System.Collections.Generic;

namespace Camino.Api.Models.YeuCauDieuChuyenKhoThuoc
{
    public class XuatKhoDieuChuyenKhoNoiBoDuocPhamViewModel : BaseViewModel
    {
        public XuatKhoDieuChuyenKhoNoiBoDuocPhamViewModel()
        {
            YeuCauDieuChuyenDuocPhamChiTiets = new List<XuatKhoDieuChuyenKhoNoiBoDuocPhamChiTietVo>();
            YeuCauDieuChuyenDuocPhamChiTietHienThis = new List<YeuCauDieuChuyenDuocPhamChiTietTheoKhoXuatVo>();

        }
        public long? KhoXuatId { get; set; }
        public string TenKhoXuat { get; set; }
        public long? KhoNhapId { get; set; }
        public string TenKhoNhap { get; set; }
        public long? NguoiXuatId { get; set; }
        public string TenNguoiXuat { get; set; }
        public long? NguoiNhapId { get; set; }
        public string TenNguoiNhap { get; set; }
        public bool? DuocKeToanDuyet { get; set; }
        public long? NhanVienDuyetId { get; set; }
        public string TenNhanVienDuyet { get; set; }
        public string LyDoKhongDuyet { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public bool? HienThiCaThuocHetHan { get; set; }
        public List<XuatKhoDieuChuyenKhoNoiBoDuocPhamChiTietVo> YeuCauDieuChuyenDuocPhamChiTiets { get; set; }
        public List<YeuCauDieuChuyenDuocPhamChiTietTheoKhoXuatVo> YeuCauDieuChuyenDuocPhamChiTietHienThis { get; set; }
    }

    public class XuatKhoDieuChuyenKhoNoiBoDuocPhamChiTietViewModel
    {
        public double? SoLuongDieuChuyen { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public bool? LaDuocPhamBHYT { get; set; }
        
    }
}
