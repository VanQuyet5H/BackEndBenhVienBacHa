using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum TrangThaiNhapKhoMau
        {
            [Description("Chờ nhập giá")]
            ChoNhapGia = 1,
            [Description("Đã nhập giá")]
            DaNhapGia = 2,
        }

        public enum LoaiXetNghiemMauNhapThem
        {
            [Description("KQ phản ứng chéo ống I")]
            KetQuaPhanUngCheoOngI = 1,
            [Description("KQ phản ứng chéo ống II")]
            KetQuaPhanUngCheoOngII = 2,
            [Description("Môi trường muối")]
            KetQuaXetNghiemMoiTruongMuoi = 3,
            [Description("37oC/Kháng glubulin")]
            KetQuaXetNghiem37oCKhangGlubulin = 4,
            [Description("NAT")]
            KetQuaXetNghiemNAT = 5
        }
    }
}
