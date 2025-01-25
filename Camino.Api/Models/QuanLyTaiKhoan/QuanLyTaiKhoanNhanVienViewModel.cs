using System.Collections.Generic;

namespace Camino.Api.Models.QuanLyTaiKhoan
{
    public class QuanLyTaiKhoanNhanVienViewModel : BaseViewModel
    {
        public QuanLyTaiKhoanNhanVienViewModel()
        {
            roleCurrent = new List<long>();
            roleNew = new List<long>();
        }
        public long? NhanVienId { get; set; }
        public string HoTen { get; set; }
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }

        public bool IsChangePassword { get; set; }

        public List<long> roleCurrent { get; set; }
        public List<long> roleNew { get; set; }
    }
}