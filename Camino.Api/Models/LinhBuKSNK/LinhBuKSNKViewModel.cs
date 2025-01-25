using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.LinhBuKSNK
{
    public class LinhBuKSNKViewModel : BaseViewModel
    {
        public LinhBuKSNKViewModel()
        {
            YeuCauLinhVatTuChiTiets = new List<LinhBuKSNKChiTietViewModel>();
            YeuCauVatTuBenhViens = new List<YeuCauKSNKBenhViensViewModel>();
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
        public bool? DaGui { get; set; }
        public string ThoiDiemChiDinhTu { get; set; }
        public string ThoiDiemChiDinhDen { get; set; }
        public string HoTenNguoiYeuCau { get; set; }
        public List<LinhBuKSNKChiTietViewModel> YeuCauLinhVatTuChiTiets { get; set; }
        public List<YeuCauKSNKBenhViensViewModel> YeuCauVatTuBenhViens { get; set; }
    }

    public class LinhBuKSNKChiTietViewModel : BaseViewModel
    {
        public string Nhom { get; set; }
        public long? YeuCauLinhVatTuId { get; set; }
        public long? VatTuBenhVienId { get; set; }
        public bool? LaVatTuBHYT { get; set; }
        public double? SoLuong { get; set; }
        public string Ten { get; set; }
        public bool? DuocDuyet { get; set; }
        public double? SLYeuCau { get; set; }
        public double? SoLuongCoTheXuat { get; set; }
        public string DVT { get; set; }
        public string NhaSX { get; set; }
        public string NuocSX { get; set; }
        public double? SLCanBu { get; set; }
        public double? SLTon { get; set; }
        public double? SLYeuCauLinhThucTe { get; set; }
        public bool LoaiDuocPhamHayVatTu { get; set; }


    }
    public class YeuCauKSNKBenhViensViewModel : GridItem
    {
        public long? VatTuBenhVienId { get; set; }
        public bool? LaVatTuBHYT { get; set; }
        public string YeuCauVatTuBenhVienIds { get; set; }
        public long? KhoLinhTuId { get; set; }
        public long? KhoLinhVeId { get; set; }
        public double SoLuongCanBu { get; set; }
        public double SoLuongTon { get; set; }
        public double? SoLuongYeuCau { get; set; }
        public double? SoLuongDaBu { get; set; }
        public double? SLYeuCauLinhThucTe { get; set; }
        public double? SLYeuCauLinhThucTeMax { get; set; }
        public bool CheckBox { get; set; }
        public bool LoaiDuocPhamHayVatTu { get; set; }
    }


}
