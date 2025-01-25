using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.CauHinh
{
    public class CauHinhMarketing : ISettings
    {
        public CauHinhMarketing()
        {
            SoTienCanhBaoThuThemGoiMarketing = 500000;
        }
        public decimal SoTienCanhBaoThuThemGoiMarketing { get; set; }
    }
}
