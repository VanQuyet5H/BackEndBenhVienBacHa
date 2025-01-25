
using Camino.Api.Models.LinhThuongKSNK.Validators;
using Camino.Api.Models.LinhThuongVatTu.Validators;
using Camino.Core.Domain.ValueObject.YeuCauKSNK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.LinhThuongKSNK
{
    public class LinhThuongKSNKViewModel : BaseViewModel
    {
        public LinhThuongKSNKViewModel()
        {
            YeuCauLinhVatTuChiTiets = new List<LinhThuongKNSKChiTietViewModel>();
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

        public List<LinhThuongKNSKChiTietViewModel> YeuCauLinhVatTuChiTiets { get; set; }
    }

    public class KSNKGridViewModel
    {
        public KSNKGridViewModel()
        {
            YeuCauLinhVatTuChiTiets = new List<LinhThuongKNSKChiTietViewModel>(); // hoặc dược phẩm
            VatTuBenhViens = new List<KSNKGridViewModelValidators>(); // hoặc dược phẩm
        }
        public int? STT { get; set; }
        public string Nhom { get; set; }
        public long? VatTuBenhVienId { get; set; } // hoặc dược phẩm
        public string Ten { get; set; }
        public string Ma { get; set; }
        public bool LaVatTuBHYT { get; set; } // hoặc dược phẩm
        public string DVT { get; set; }
        public long KhoXuatId { get; set; }
        public string NhaSX { get; set; }
        public string NuocSX { get; set; }
        public double? SLYeuCau { get; set; }
        public int? LoaiVatTu { get; set; }
        public List<LinhThuongKNSKChiTietViewModel> YeuCauLinhVatTuChiTiets { get; set; } // cả dược phẩm
        public List<KSNKGridViewModelValidators> VatTuBenhViens { get; set; } // cả dược phẩm
        public long KhoLinhId { get; set; } // lĩnh từ kho id
        public bool LoaiDuocPhamHayVatTu { get; set; }
        public string TenKhoLinh { get; set; }
    }

    public class LinhThuongKNSKChiTietViewModel : BaseViewModel
    {
        public string Nhom { get; set; }
        public long? YeuCauLinhVatTuId { get; set; } // hoặc dược phẩm
        public long? KhoXuatId { get; set; } 
        public long? VatTuBenhVienId { get; set; } // hoặc dược phẩm
        public string Ten { get; set; }
        public string Ma { get; set; }
        public bool? LaVatTuBHYT { get; set; } // hoặc dược phẩm
        public bool? DuocDuyet { get; set; }
        public double? SoLuong { get; set; }
        public double? SLYeuCau { get; set; }
        public double? SLTon { get; set; }
        public double? SoLuongCoTheXuat { get; set; }
        public string DVT { get; set; }
        public string NhaSX { get; set; }
        public string NuocSX { get; set; }
        public bool? IsValidator { get; set; }
        public bool LoaiDuocPhamHayVatTu { get; set; }
       
        public string TenKhoLinh { get; set; }

    }
    public class GridVoYeuCauLinhDuocTao 
    {
        public long YeuCauLinhVatTuId { get; set; } // hoặc dược phẩm
        public bool LoaiDuocPhamHayVatTu { get; set; }

    }
    public class GridVoYeuCauLinh
    {
        public long YeuCauLinhId { get; set; } // hoặc dược phẩm
        public bool LoaiDuocPhamHayVatTu { get; set; }

    }
}
