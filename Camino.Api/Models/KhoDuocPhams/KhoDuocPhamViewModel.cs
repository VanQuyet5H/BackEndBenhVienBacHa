using Camino.Core.Domain;
using Camino.Core.Helpers;
using System.Collections.Generic;

namespace Camino.Api.Models.KhoDuocPhams
{
    public class KhoDuocPhamViewModel : BaseViewModel
    {
        public string Ten { get; set; }
        public Enums.EnumLoaiKhoDuocPham LoaiKho { get; set; }
        public string TextLoaiKho => LoaiKho.GetDescription();
        public long[] NhanVienIds { get; set; }
        public long? KhoaPhongId { get; set; }
        public string KhoaPhong { get; set; }
        //public long NhanVienId { get; set; }
        //public string NhanVien { get; set; }
        //public long? KhoId { get; set; }
        public long? PhongBenhVienId { get; set; }
        public string PhongBenhVien { get; set; }
        public bool IsDefault { get; set; }
        public bool? LoaiDuocPham { get; set; }
        public bool? LoaiVatTu { get; set; }

    }
}
