using Camino.Core.Domain;
using System;
using System.Collections.Generic;
using Camino.Api.Models.KhoaPhongNhanVien;
namespace Camino.Api.Models.RaVien
{
    public class RaVienVM : BaseViewModel
    {
        public bool? HenTaiKham { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public DateTime? NgayHienTaiKham { get; set; }
        public Enums.EnumKetQuaDieuTri? KetQuaDieuTriId { get; set; }
        public Enums.LoaiGiaPhauThuat? GiaPhauThuatId { get; set; }
        public DateTime? ThoiGianRaVien { get; set; }
        public string GhiChuTaiKham { get; set; }
        public Enums.EnumHinhThucRaVien? HinhThucRaVienId { get; set; }
    }
}
