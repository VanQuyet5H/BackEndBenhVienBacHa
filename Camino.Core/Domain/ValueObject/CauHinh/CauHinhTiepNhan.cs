using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.CauHinh
{
    public class CauHinhTiepNhan : ISettings
    {
        public long QuocTich { get; set; }
        public long TinhThanhPho { get; set; }
        public long DanToc { get; set; }
        public long LyDoTiepNhan { get; set; }
        public long HinhThucDen { get; set; }
    }
}
