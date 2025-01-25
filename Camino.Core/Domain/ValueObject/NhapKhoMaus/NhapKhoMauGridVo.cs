using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.NhapKhoMaus
{
    public class PhieuNhapKhoMauGridVo: GridItem
    {
        public string SoPhieu { get; set; }
        public string SoHoaDon { get; set; }
        public DateTime? NgayHoaDon { get; set; }
        public string NgayHoaDonDisplay => NgayHoaDon?.ApplyFormatDate();
        public string NhaCungCap { get; set; }
        public string GhiChu { get; set; }
        public Enums.TrangThaiNhapKhoMau TinhTrang { get; set; }
        public string TenTinhTrang => TinhTrang.GetDescription();
        public string NguoiNhap { get; set; }
        public DateTime? NgayNhap { get; set; }
        public string NgayNhapDisplay => NgayNhap?.ApplyFormatDateTime();
        public string NguoiDuyet { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public string NgayDuyetDisplay => NgayDuyet?.ApplyFormatDateTime();

    }

    public class PhieuNhapKhoMauGridChiTietVo : GridItem
    {
        public string NhomMauNguoiBenh { get; set; }
        public string MaTuiMau { get; set; }
        public string ChePhamMau { get; set; }
        public DateTime? NgaySanXuat { get; set; }
        public string NgaySanXuatDisplay => NgaySanXuat?.ApplyFormatDateTime();
        public DateTime? HanSuDung { get; set; }
        public string HanSuDungDisplay => HanSuDung?.ApplyFormatDateTime();
        public decimal? DonGiaDichVu { get; set; }
        public decimal? DonGiaBaoHiem { get; set; }

    }
}
