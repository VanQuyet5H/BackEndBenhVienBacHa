using Camino.Core.Domain.ValueObject.YeuCauMuaVatTu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.DuTruMuaVatTu
{
    public class DuTruMuaVatTuChiTietViewModel : BaseViewModel
    {
        public long? DuTruMuaVatTuId { get; set; }
        public long VatTuId { get; set; }
        public string Ten { get; set; }
        public bool LaVatTuBHYT { get; set; }
        public int? SoLuongDuTru { get; set; }
        public int? SoLuongDuKienSuDung { get; set; }
        public int? SoLuongDuTruTruongKhoaDuyet { get; set; }
        public string DVT { get; set; }
        public string NhaSX { get; set; }
        public string NuocSX { get; set; }
        public string TenLoaiVatTu { get; set; }
        public string GhiChu { get; set; }

    }

    public class VatTuDuTruGridViewModel : BaseViewModel
    {
        public VatTuDuTruGridViewModel()
        {
            YeuCauMuaVatTuChiTietValidators = new List<VatTuDuTruViewModelValidator>();
        }
        public string Ma { get; set; }
        public int LoaiVatTu { get; set; }
        public string TenLoaiVatTu { get; set; }
        public long? VatTuId { get; set; }
        public long? NhomVatTuId { get; set; }
        public int? TyLeBaoHiemThanhToan = 0;
        public string Ten { get; set; }
        public string DVT { get; set; }
        public bool? LaVatTuBHYT { get; set; }
        public string QuyCach { get; set; }
        public string NhaSX { get; set; }
        public string NuocSX { get; set; }
        public string GhiChu { get; set; }
        public double? SoLuongTonDuTru { get; set; }
        public double? SoLuongDuTru { get; set; }
        public double? SoLuongDuKienSuDung { get; set; }
        public List<VatTuDuTruViewModelValidator> YeuCauMuaVatTuChiTietValidators { get; set; }
        public bool IsThemVatTu { get; set; }
    }
}
