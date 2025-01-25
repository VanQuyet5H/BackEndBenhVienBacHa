using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.KhoDuocPhamGridVo
{
    public class KhoDuocPhamGridVo : GridItem
    {
        public string Ten { get; set; }
        public long KhoId { get; set; }
        public Enums.EnumLoaiKhoDuocPham LoaiKho { get; set; }
        public string TextLoaiKho => LoaiKho.GetDescription();
        //public string NhanVien { get; set; }
        public string KhoaPhong { get; set; }
        public string PhongBenhVien { get; set; }
        public bool IsDefault { get; set; }
        public bool? LoaiDuocPham { get; set; }
        public bool? LoaiVatTu { get; set; }
        public string LoaiDuocPhamVatTu => LoaiDuocPham == true && LoaiVatTu != true? "Dược phẩm" : (LoaiDuocPham != true && LoaiVatTu == true ? "Vật tư" : 
                        (LoaiDuocPham == true && LoaiVatTu == true ? "Dược phẩm - Vật tư" : ""));
    }
}
