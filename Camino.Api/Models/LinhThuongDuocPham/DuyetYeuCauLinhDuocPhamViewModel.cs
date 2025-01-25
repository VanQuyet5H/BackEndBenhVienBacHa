using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Camino.Api.Models.LinhThuongDuocPham
{
    public class DuyetYeuCauLinhDuocPhamViewModel : BaseViewModel
    {
        public DuyetYeuCauLinhDuocPhamViewModel()
        {
            DuyetYeuCauLinhDuocPhamChiTiets = new List<DuyetYeuCauLinhDuocPhamChiTietViewModel>();
        }
        public long KhoXuatId { get; set; }
        public string TenKhoXuat { get; set; }
        public long KhoNhapId { get; set; }
        public string TenKhoNhap { get; set; }
        public Enums.EnumLoaiPhieuLinh LoaiPhieuLinh { get; set; }
        public string TenLoaiPhieuLinh
        {
            get { return LoaiPhieuLinh.GetDescription(); }
        }

        public long NhanVienYeuCauId { get; set; }
        public string TenNhanVienYeuCau { get; set; }
        public long NhanVienDuyetId { get; set; }
        public string TenNhanVienDuyet { get; set; }
        public DateTime? NgayYeuCau { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public string GhiChu { get; set; }

        public long? NguoiXuatKhoId { get; set; }
        public string TenNguoiXuatKho { get; set; }
        public long? NguoiNhapKhoId { get; set; }
        public string TenNguoiNhapKho { get; set; }
        public bool? DuocDuyet { get; set; }
        public bool? DaGui { get; set; }

        public string TenTrangThai
        {
            get
            {
                if (DuocDuyet == null)
                {
                    return "Đang chờ duyệt";
                }
                else if (DuocDuyet == false)
                {
                    return "Từ chối duyệt";
                }
                else
                {
                    return "Đã duyệt";
                }
            }
        }

        public string LyDoKhongDuyet { get; set; }

        public List<DuyetYeuCauLinhDuocPhamChiTietViewModel> DuyetYeuCauLinhDuocPhamChiTiets { get; set; }
        public byte[] LastModified { get; set; }
        public bool? LaNguoiTaoPhieu { get; set; }
    }

    public class DuyetYeuCauLinhDuocPhamChiTietViewModel : BaseViewModel
    {
        public int Index { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public string TenDuocPham { get; set; }
        public string NongDoHamLuong { get; set; }
        public string HoatChat { get; set; }
        public string DuongDung { get; set; }
        public string DVT { get; set; }
        public string HangSanXuat { get; set; }
        public string NuocSanXuat { get; set; }
        public double SLTon { get; set; }
        public double SoLuong { get; set; }
        public double? SoLuongCoTheXuat { get; set; }
        public string Nhom { get; set; }
        public bool isTuChoi { get; set; }
    }
}
