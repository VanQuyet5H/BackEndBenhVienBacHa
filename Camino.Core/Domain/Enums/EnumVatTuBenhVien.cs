using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum LoaiSuDung
        {
            [Description("Vật tư tiêu hao")]
            VatTuTieuHao = 1,
            [Description("Vật tư thay thế")]
            VatTuThayThe = 2

        }
        public enum LoaiDuocPhamVatTu
        {
            [Description("Loại vật tư")]
            LoaiVatTu = 1,
            [Description("Loại dược phẩm")]
            LoaiDuocPham = 0

        }
        public enum LoaiGiaThuocVatTuEnum
        {
            [Description("Giá nhập")]
            GiaNhap = 1,
            [Description("Giá bán")]
            Giaban = 2,
        }
    }
}
