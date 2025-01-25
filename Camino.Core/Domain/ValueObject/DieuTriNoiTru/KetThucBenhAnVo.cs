using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.DieuTriNoiTru
{
    public class KetThucBenhAnVo
    {
        public long YeuCauTiepNhanId { get; set; }
        public DateTime ThoiDiemRaVien { get; set; }
        public Enums.EnumKetQuaDieuTri KetQuaDieuTri { get; set; }
        public Enums.EnumHinhThucRaVien HinhThucRaVien { get; set; }
        public DateTime? NgayHenTaiKham { get; set; }
        public string GhiChuTaiKham { get; set; }
    }

    public class DanhSachKhoaKhongRaVien
    {
        public long KhoaId { get; set; }
        public string Ten { get; set; }
    }
}
