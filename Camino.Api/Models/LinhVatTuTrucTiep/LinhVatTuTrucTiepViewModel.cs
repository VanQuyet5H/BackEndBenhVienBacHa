using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.LinhVatTuTrucTiep
{
    public class LinhVatTuTrucTiepViewModel : BaseViewModel
    {
        public LinhVatTuTrucTiepViewModel()
        {
            YeuCauLinhVatTuChiTiets = new List<LinhTrucTiepVatTuChiTietViewModel>();
            YeuCauVatTuBenhViensTT = new List<YeuCauVatTuBenhViensTT>();
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
        public List<LinhTrucTiepVatTuChiTietViewModel> YeuCauLinhVatTuChiTiets { get; set; }
        public List<YeuCauVatTuBenhViensTT> YeuCauVatTuBenhViensTT { get; set; }
        public List<long> YeuCauVatTuBenhVienIds { get; set; }
        public List<long> CheckKhiTao { get; set; }

        public DateTime? ThoiDiemLinhTongHopTuNgay { get; set; }
        public DateTime? ThoiDiemLinhTongHopDenNgay { get; set; }
        public long? YeuCauLinhVatTuId { get; set; }
        public bool Goi { get; set; }
        public bool? SaveOrUpDate { get; set; }
    }

    public class LinhTrucTiepVatTuChiTietViewModel : BaseViewModel
    {
        public string Nhom { get; set; }
        public long? YeuCauLinhVatTuId { get; set; }
        public long? VatTuBenhVienId { get; set; }
        public bool? LaVatTuBHYT { get; set; }
        public double? SoLuong { get; set; }
        public double? SoLuongCoTheXuat { get; set; }
        public long? YeuCauVatTuBenhVienId { get; set; }
    }
    public class YeuCauVatTuBenhViensTT
    {
        public long Id { get; set; }
        public string TenThuoc { get; set; }
    }
    public class IdTest
    {
        public long Id { get; set; }
        public string TenThuoc { get; set; }
    }
}

