namespace Camino.Core.Helpers
{
    public static class Constants
    {
        public const string PasswordDefaultUserHopDong = null;
        public const string IdSeparator = ",";
        public const string JwtRoleSeparator = ",";
        public const string HeaderUserTypeSeparator = ",";
        public const string ICDSeparator = "|";
        public const string UserNameBHYT = "01249_BV";
        public const string PassBHYT = "75f8a78ae45621c2f35eb566c03c1a5c";
        public const string UserTopicPrefix = "hapu_user_";
        public static class JwtClaimTypes
        {
            public const string Role = "_role", Id = "_id";
        }

        public static class NhomDichVu
        {
            public const string DichVuKhamBenh = "DỊCH VỤ KHÁM BỆNH";
            public const string DichVuKyThuat = "DỊCH VỤ KỸ THUẬT";
            public const string DichVuGiuong = "DỊCH VỤ GIƯỜNG";
            public const string VatTuTieuHao = "VẬT TƯ TIÊU HAO";
            public const string GoiCoChietKhau = "GÓI CÓ CHIẾT KHẤU";
            public const string DuocPham = "DƯỢC PHẨM";

        }

        public static class ExportManagerment
        {
            public const string ClassChildName = "Child";
        }

        public static class TextAlignAttribute
        {
            public const string Left = "left";
            public const string Right = "right";
            public const string Center = "center";
        }

        public static class KetQuaXetNghiemTrangThai
        {
            //update FE and BE
            public const string ChoKQChayLai = "Chờ KQ (chạy lại)";
            public const string ChoKQ = "Chờ KQ";
            public const string ChoDuyetKQ = "Chờ duyệt KQ";
            public const string DaCoKQ = "Đã có KQ";
        }

        public static class StringXuatNhapKho
        {
            public const string LyDoXuatVeKhoSauKhiDuyet = "Xuất trực tiếp khi nhập kho";
        }

        //public static class DuongDungIdSapXep
        //{
        //    public const long Tiem = 12; // Tiêm
        //    public const long Uong = 1; // Uống
        //    public const long Dat = 26; // Đặt
        //    public const long DungNgoai = 22; //Dùng ngoài
        //}

        public static class DuocPhamBenhVienPhanNhom
        {
            public const long TanDuoc = 1; // 
            public const long ThuocTuDuocLieu = 41; // 
            public const long HoaChat = 43; // 
            public const long SinhPham = 50; //
            public const long MyPham = 56; //
            public const long ThucPhamChucNang = 73; //
            public const long Vacxin = 79; //
            public const long ThietBiYTe = 85; 
            public const long VatTuYTe = 96; //
            public const long ChuaPhanNhom = 114; //
            public const long SinhPhamChanDoan = 115; 

        }
    }
}
