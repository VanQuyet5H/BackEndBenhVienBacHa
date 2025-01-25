namespace Camino.Api.Models.Users
{
    public class UserAndDepartmentViewModel
    {
        public string User { get; set; }

        public string Ten { get; set; }

        public string ChucDanh { get; set; }

        public long PhongBenhVienId { get; set; }

        public string TenKhoaPhong { get; set; }

        public string MaKhoaPhong { get; set; }

        public bool IsChucDanhEmpty { get; set; }

        public bool IsKhoaPhongEmpty { get; set; }

        public string TenPhong { get; set; }
        public string MaPhong { get; set; }

        public long KhoaId { get; set; }

        public bool IsPhongEmpty { get; set; }
    }
}
