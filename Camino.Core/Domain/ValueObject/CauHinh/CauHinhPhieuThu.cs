using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.CauHinh
{
    public class CauHinhPhieuThu : ISettings
    {
        public string TaiKhoanNo { get; set; }
        public string TaiKhoanCo { get; set; }
    }
}
