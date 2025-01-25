using Camino.Api.Models.NhapKhoVatTuChiTiets;
using Camino.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.NhapKhoVatTus
{
    public class NhapKhoVatTuViewModel : BaseViewModel
    {
        public long? XuatKhoVatTuId { get; set; }
        public long? KhoId { get; set; }
        public string TenKhoVatTu { get; set; }
        public string SoChungTu { get; set; }
        public Enums.EnumLoaiNhapKho? LoaiNhapKho { get; set; }
        public string TenLoaiNhapKho { get; set; }
        public string TenNguoiGiao { get; set; }
        public string TenNguoiGiaoNgoai { get; set; }
        public string TenNguoiNhap { get; set; }
        public long? NguoiGiaoId { get; set; }
        public long? NguoiNhapId { get; set; }
        public Enums.LoaiNguoiGiaoNhan LoaiNguoiGiao { get; set; }
        public bool? DaHet { get; set; }
        public DateTime? NgayNhap { get; set; }
        public bool? DaXuatKho { get; set; }
        public DateTime? NgayHoaDon { get; set; }
        private ICollection<NhapKhoVatTuChiTietViewModel> _nhapKhoVatTuChiTiets;
        public virtual ICollection<NhapKhoVatTuChiTietViewModel> NhapKhoVatTuChiTiets
        {
            get => _nhapKhoVatTuChiTiets ?? (_nhapKhoVatTuChiTiets = new List<NhapKhoVatTuChiTietViewModel>());
            protected set => _nhapKhoVatTuChiTiets = value;
        }
    }
}
