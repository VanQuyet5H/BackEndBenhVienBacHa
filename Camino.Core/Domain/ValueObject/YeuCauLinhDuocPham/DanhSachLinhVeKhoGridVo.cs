using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham
{
   public class DanhSachLinhVeKhoGridVo
    {
        public string DisplayName { get; set; }
        public long KeyId { get; set; }
        public string TenKhoLinh { get; set; }
        public string LoaiLinh { get; set; }
    }
}
