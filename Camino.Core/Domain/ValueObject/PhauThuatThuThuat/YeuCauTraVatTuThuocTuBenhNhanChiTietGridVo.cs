using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.PhauThuatThuThuat
{
    public class YeuCauTraVatTuThuocTuBenhNhanChiTietGridVo : GridItem
    {
        public DateTime NgayTra { get; set; }
        public string NgayTraDisplay => NgayTra.ApplyFormatDate();
        public string SoLuongTra { get; set; }
        public string NhanVienTra { get; set; }
        public string SoPhieu { get; set; }
        public bool? DuocDuyet { get; set; }
        public string TinhTrang => DuocDuyet == null ? "Chờ duyệt" : (DuocDuyet == true ? "Đã duyệt" : "Từ chối");
        public DateTime? NgayTao { get; set; }
        public string NgayTaoDisplay => NgayTao?.ApplyFormatDateTimeSACH();
    }

    public class YeuCauTraVatThuThuocTuBenhNhanSearchVo
    {
        public string YeuCauDuocPhamVatTuBenhVienId { get; set; }
        public bool DuocHuongBaoHiem { get; set; }
        public EnumNhomGoiDichVu NhomYeuCauId { get; set; }
    }
}