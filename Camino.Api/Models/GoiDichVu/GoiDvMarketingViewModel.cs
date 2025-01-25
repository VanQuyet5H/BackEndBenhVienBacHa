using System.Collections.Generic;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Helpers;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.GoiDichVu
{
    public class GoiDvMarketingViewModel : BaseViewModel
    {
        public string TenGoiDv { get; set; }

        public bool? IsDisabled { get; set; }

        public string MoTa { get; set; }

        public BoPhan? BoPhanId { get; set; }

        public string TenBoPhan { get; set; }
    
        public Enums.EnumLoaiGoiDichVu LoaiGoiDichVu { get; set; }

        public List<DvTrongGoiViewModel> DvTrongGois { get; set; }
    }

    public class DvTrongGoiViewModel
    {
        public long DvId { get; set; }

        public long IdDatabase { get; set; }

        public long GoiDichVuId { get; set; }

        public string MaDv { get; set; }

        public string TenDv { get; set; }

        public long LoaiGia { get; set; }

        public int SoLuong { get; set; }

        public string GhiChu { get; set; }

        public string LoaiGiaDisplay { get; set; }

        public Enums.EnumDichVuTongHop Nhom { get; set; }

        public string NhomDisplay => Nhom.GetDescription();

        public decimal DonGia { get; set; }

        public decimal ThanhTien => DonGia * SoLuong;

    }
}
