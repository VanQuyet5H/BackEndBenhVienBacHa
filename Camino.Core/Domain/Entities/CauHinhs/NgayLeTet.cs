using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.CauHinhs
{
    public class NgayLeTet : BaseEntity
    {
        public string Ten { get; set; }
        public int Ngay { get; set; }
        public int Thang { get; set; }
        public int? Nam { get; set; }
        public bool LeHangNam { get; set; }
        public string GhiChu { get; set; }
    }
}
