using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.QuanLyTaiKhoan
{
    public class QuanLyTaiKhoanGridVo : GridItem
    {
        public string HoTen { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public string DiaChi { get; set; }
        public bool IsActive { get; set; }
        public string IsActiveDisplay { get; set; }
    }

    public class TimNhanVienGridVo : GridItem
    {
        public string HoTen { get; set; }
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string GioiTinhDisplay { get; set; }
    }

    public class ThayDoiQuyen
    {
        public ThayDoiQuyen()
        {
            roleCurrent = new List<long>();
            roleNew = new List<long>();
        }
        public long? Id { get; set; }
        public string HoTen { get; set; }
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }

        public List<long> roleCurrent { get; set; }
        public List<long> roleNew { get; set; }
    }

    public class SearchTimNhanVien
    {
        public string HoTen { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public string DiaChi { get; set; }
    }
}