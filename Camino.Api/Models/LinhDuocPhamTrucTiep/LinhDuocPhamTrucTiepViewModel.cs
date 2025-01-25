using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.LinhDuocPhamTrucTiep
{
    public class LinhDuocPhamTrucTiepViewModel : BaseViewModel
    {
        public LinhDuocPhamTrucTiepViewModel()
        {
            YeuCauLinhDuocPhamChiTiets = new List<LinhTrucTiepDuocPhamChiTietViewModel>();
            YeuCauDuocPhamBenhViensTT = new List<YeuCauDuocPhamBenhViensTT>();
        }

        public long? KhoXuatId { get; set; }
        public string TenKhoXuat { get; set; }
        public long? KhoNhapId { get; set; }
        public EnumLoaiPhieuLinh? LoaiPhieuLinh { get; set; }
        public long? NhanVienYeuCauId { get; set; }
        public DateTime? NgayYeuCau { get; set; }
        public long? NguoiDuyet { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public string GhiChu { get; set; }
        public string LyDoKhongDuyet { get; set; }
        public bool? DuocDuyet { get; set; }
        public long? NoiYeuCauId { get; set; }
        public List<LinhTrucTiepDuocPhamChiTietViewModel> YeuCauLinhDuocPhamChiTiets { get; set; }
        public List<YeuCauDuocPhamBenhViensTT> YeuCauDuocPhamBenhViensTT { get; set; }
        public List<long> YeuCauDuocPhamBenhIds { get; set; }
        public List<long> CheckKhiTao { get; set; }

        public DateTime? ThoiDiemLinhTongHopTuNgay { get; set; }
        public DateTime? ThoiDiemLinhTongHopDenNgay { get; set; }
        public long? YeuCauLinhDuocPhamId { get; set; }
        public bool Goi { get; set; }
        public bool? SaveOrUpDate { get; set; }

    }
    public class LinhTrucTiepDuocPhamChiTietViewModel : BaseViewModel
    {
        public string Nhom { get; set; }
        public long? YeuCauLinhDuocPhamId { get; set; }
        public long? DuocPhamBenhVienId { get; set; }
        public bool? LaDuocPhamBHYT { get; set; }
        public double? SoLuong { get; set; }
        public double? SoLuongCoTheXuat { get; set; }
        public long? YeuCauDuocPhamBenhVienId { get; set; }
    }
    public class YeuCauDuocPhamBenhViensTT
    {
        public long Id { get; set; }
        public string TenThuoc { get; set; }
        public long YeuCauTiepNhanId { get; set; }
    }
    public class LinhDuocPhamTrucTiepQueryInfo
    {
        public long idKhoLinh { get; set; }
        public long phongDangNhapId { get; set; }
        public string dateSearchStart { get; set; }
        public string dateSearchEnd { get; set; }
    }
}
