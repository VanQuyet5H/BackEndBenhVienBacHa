using Camino.Api.Models.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.NhapKhoDuocPhams
{
    public class NhapKhoDuocPhamViewModel : BaseViewModel
    {
        public long? XuatKhoDuocPhamId { get; set; }
        public long? KhoDuocPhamId { get; set; }
        public string TenKhoDuocPham { get; set; }
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
        private ICollection<NhapKhoDuocPhamChiTietViewModel> _nhapKhoDuocPhamChiTiets;
        public virtual ICollection<NhapKhoDuocPhamChiTietViewModel> NhapKhoDuocPhamChiTiets
        {
            get => _nhapKhoDuocPhamChiTiets ?? (_nhapKhoDuocPhamChiTiets = new List<NhapKhoDuocPhamChiTietViewModel>());
            protected set => _nhapKhoDuocPhamChiTiets = value;
        }
    }
}
