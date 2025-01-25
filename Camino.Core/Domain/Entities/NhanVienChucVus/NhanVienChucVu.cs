using Camino.Core.Domain.Entities.ChucVus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.NhanVienChucVus
{
    public class NhanVienChucVu : BaseEntity
    {
        public long NhanVienId { get; set; }
        public long ChucVuId { get; set; }
        public virtual Entities.NhanViens.NhanVien NhanVien { get; set; }
        public virtual ChucVu ChucVu { get; set; }
    }
}
