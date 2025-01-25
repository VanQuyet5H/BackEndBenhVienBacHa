using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.BHYT
{
   public class ThongTinBenhNhanXemVO
    {
        public string MaThe { get; set; }
        public string TenBenhNhan { get; set; }
        public DateTime? NgaySinh { get; set; }
        public int? NamSinh { get; set; }
    }
}
