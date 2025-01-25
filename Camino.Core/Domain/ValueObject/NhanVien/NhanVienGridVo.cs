using System;
using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.NhanVien
{
    public class NhanVienGridVo : GridItem
    {
        public string HoTen { get; set; }
        public long UserId { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string SoChungMinhThu { get; set; }
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }
        public string SoDienThoaiDisplay { get; set; }
        public string SoDienThoaiGoc { get; set; }
        public string Avatar { get; set; }
        public string QuyenHan { get; set; }
        public string GhiChu { get; set; }
        public Enums.LoaiGioiTinh GioiTinh { get; set; }
        public bool? KichHoat { get; set; }
        public string GioiTinhText { get; set; }
        public string Email { get; set; }

    }

    public class KhoaPhongNhanVienVo : GridItem
    {
        public string NhomKhoa { get; set; }
        public long NhomKhoaId { get; set; }
        public string MaPhong { get; set; }
        public string TenPhong { get; set; }
        public bool PhongChinh { get; set; }
        public bool IsChecked { get; set; }
        public bool IsCheckedParent { get; set; }
    }

    public class KhoNhanVienVo : GridItem
    {
        public string TenKho { get; set; }
        public string TenKhoa { get; set; }
        public string TenPhong { get; set; }
        public bool DaChon { get; set; }
        public bool Checked => DaChon;
    }

    public class SreachKhoaPhong
    {
        public int NhanVienId { get; set; }
        public List<long?> KhoaPhongIds { get; set; }
        public List<long?> PhongBenhVienIds { get; set; }
    }
}
