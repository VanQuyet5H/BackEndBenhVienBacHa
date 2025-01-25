using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.CauHinhs
{
    public class CauHinhNguoiDuyetTheoNhomDichVu : BaseEntity
    {
        public long NhomDichVuBenhVienId { get; set; }
        public long NhanVienId { get; set; }

        public virtual NhomDichVuBenhVien.NhomDichVuBenhVien NhomDichVuBenhVien { get; set; }
        public virtual Camino.Core.Domain.Entities.NhanViens.NhanVien NhanVien { get; set; }
    }
}
