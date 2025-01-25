using System.ComponentModel;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum EnumLoaiGoiDichVu
        {
            [Description("Marketing")]
            Marketing = 1,

            [Description("Sử dụng trong phòng bác sỹ")]
            TrongPhongBacSy = 2
        }

        public enum EnumNhomGoiDichVu
        {
            [Description("DỊCH VỤ KHÁM BỆNH")]
            DichVuKhamBenh = 1,
            [Description("DỊCH VỤ KỸ THUẬT")]
            DichVuKyThuat = 2,
            [Description("VẬT TƯ TIÊU HAO")]
            VatTuTieuHao = 3,
            [Description("DƯỢC PHẨM")]
            DuocPham = 4,
            [Description("DỊCH VỤ GIƯỜNG BỆNH")]
            DichVuGiuongBenh = 5,
            [Description("ĐƠN THUỐC THANH TOÁN")]
            DonThuocThanhToan = 6,
            [Description("TRUYỀN MÁU")]
            TruyenMau = 7,
        }

        public enum EnumLoaiGoiDichVuMarketing
        {
            [Description("Gói dành cho trẻ sơ sinh")]
            GoiSoSinh = 1,
            [Description("Gói dành cho sản phụ")]
            GoiSanPhu = 2
        }

        public enum PhieuHoSoBenhAn
        {
            [Description("Phiếu hồ sơ bệnh án")]
            PhieuHoSoBenhAnDienTu = 1,
            [Description("Nhóm CLS")]
            NhomCLS = 2,
            [Description("Hồ sơ khác")]
            HoSoKhac = 3
        }
    }
}