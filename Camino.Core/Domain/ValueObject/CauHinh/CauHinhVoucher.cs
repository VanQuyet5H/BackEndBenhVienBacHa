using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.CauHinh
{
    public class CauHinhVoucher : ISettings
    {
        public CauHinhVoucher()
        {
            SoTienToiDaDuocMienGiam = 1000000;
        }
        public decimal SoTienToiDaDuocMienGiam { get; set; }
    }
}
