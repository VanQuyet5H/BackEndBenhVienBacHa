using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.HoSoNhanVienDinhKems
{
    public class HoSoNhanVienFileDinhKem : TaiLieuDinhKemEntity
    {
        public long NhanVienId { get; set; }

        public virtual NhanViens.NhanVien NhanVien { get; set; }
    }
}
