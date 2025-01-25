using Camino.Core.Domain.ValueObject.YeuCauLinhVatTu;
using System;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;
namespace Camino.Api.Models.LinhThuongVatTu
{
    public class LinhThuongVatTuViewModel : BaseViewModel
    {
        public LinhThuongVatTuViewModel()
        {
            YeuCauLinhVatTuChiTiets = new List<LinhThuongVatTuChiTietViewModel>();
        }
        public long? KhoXuatId { get; set; }
        public string TenKhoXuat { get; set; }
        public long? KhoNhapId { get; set; }
        public string TenKhoNhap { get; set; }
        public EnumLoaiPhieuLinh? LoaiPhieuLinh { get; set; }
        public long? NhanVienYeuCauId { get; set; }
        public bool? DuocDuyet { get; set; }
        public string HoTenNguoiYeuCau { get; set; }
        public DateTime? NgayYeuCau { get; set; }
        public string GhiChu { get; set; }
        public string LyDoKhongDuyet { get; set; }
        public long? NhanVienDuyetId { get; set; }
        public string HoTenNguoiDuyet { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public bool IsLuu { get; set; }
        public byte[] LastModified { get; set; }
        public bool? LaNguoiTaoPhieu { get; set; }
        public bool? DaGui { get; set; }

        public List<LinhThuongVatTuChiTietViewModel> YeuCauLinhVatTuChiTiets { get; set; }
    }

    public class VatTuGridViewModel
    {
        public VatTuGridViewModel()
        {
            YeuCauLinhVatTuChiTiets = new List<LinhThuongVatTuChiTietViewModel>();
            VatTuBenhViens = new List<VatTuGridViewModelValidator>();
        }
        public int? STT { get; set; }
        public string Nhom { get; set; }
        public long? VatTuBenhVienId { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }
        public bool LaVatTuBHYT { get; set; }
        public string DVT { get; set; }
        public long KhoXuatId { get; set; }
        public string NhaSX { get; set; }
        public string NuocSX { get; set; }
        public double? SLYeuCau { get; set; }
        public int? LoaiVatTu { get; set; }
        public List<LinhThuongVatTuChiTietViewModel> YeuCauLinhVatTuChiTiets { get; set; }
        public List<VatTuGridViewModelValidator> VatTuBenhViens { get; set; }
    }

    public class LinhThuongVatTuChiTietViewModel : BaseViewModel
    {
        public string Nhom { get; set; }
        public long? YeuCauLinhVatTuId { get; set; }
        public long? KhoXuatId { get; set; }
        public long? VatTuBenhVienId { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }
        public bool? LaVatTuBHYT { get; set; }
        public bool? DuocDuyet { get; set; }
        public double? SoLuong { get; set; }
        public double? SLYeuCau { get; set; }
        public double? SLTon { get; set; }
        public double? SoLuongCoTheXuat { get; set; }
        public string DVT { get; set; }
        public string NhaSX { get; set; }
        public string NuocSX { get; set; }
        public bool? IsValidator { get; set; }

    }
}
