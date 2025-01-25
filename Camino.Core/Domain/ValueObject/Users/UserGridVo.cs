using Camino.Core.Domain.ValueObject.Grid;


namespace Camino.Core.Domain.ValueObject.Users
{
    public class UserGridVo : GridItem
    {
       
        public string Role { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string FullName { get; set; }
        public bool? IsActive { get; set; }
        public string UserRole { get; set; }
        public string UserTypes { get; set; }
        public long NhanVienId { get; set; }
        public long CuDanId { get; set; }
        public long KhachVanLaiId { get; set; }
        public long VanPhongId { get; set; }
        public long KhachHangTrungTamId { get; set; }
        public bool? IsDefault { get; set; }
        public Enums.Region Region { get; set; }
        public string UserTypesDes { get; set; }
    }

    public class UserExternalGridVo : GridItem
    {

        public string Role { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string FullName { get; set; }
        public bool? IsActive { get; set; }
        public string UserRole { get; set; }
        public string UserTypes { get; set; }
        public long NhanVienId { get; set; }
        public long CuDanId { get; set; }
        public long KhachVanLaiId { get; set; }
        public long VanPhongId { get; set; }
        public long KhachHangTrungTamId { get; set; }
        public bool? IsDefault { get; set; }
        public Enums.Region Region { get; set; }
        public string UserTypesDes { get; set; }
        public bool IsChu { get; set; }
        public string HoTen { get; set; }
    }
}
