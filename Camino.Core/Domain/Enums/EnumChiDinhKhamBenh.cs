using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum LoaiLoiGoiDichVu
        {
            [Description("Trùng")]
            Trung = 1,
            [Description("Chưa nhập giá")]
            ChuaNhapGia = 2
        }
    }
}
