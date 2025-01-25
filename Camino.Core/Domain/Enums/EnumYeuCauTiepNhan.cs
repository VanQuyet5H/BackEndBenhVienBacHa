using System.ComponentModel;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum EnumYeuCauTiepNhan
        {
            [Description("Khám sức khỏe")]
            KhamSucKhoe = 1,
            [Description("CC tai nạn giao thông")]
            CapCuuTaiNanGiaoThong = 2,
            [Description("CC tai nạn lao động")]
            CapCuuTaiNanLaoDong = 3,
            [Description("CC tai nạn rủi ro")]
            CapCuuTaiNanRuiRo = 4,
            [Description("CC tai nạn đã thương")]
            CapCuuTaiNanDaThuong = 5,
            [Description("CC tự tử")]
            CapCuuTuTu = 6,
            [Description("CC tai nạn pháo nổ")]
            CapCuuTaiNanPhaoNo = 7,
            [Description("CC tai nạn ngộ độc thức ăn")]
            CapCuuTaiNanNgoDocThucAn = 8,
            [Description("DV vận chuyển cấp cứu")]
            DichVuVanChuyenCapCuu = 9,
            [Description("Khám bệnh")]
            KhamBenh = 10,
            [Description("Cấp cứu")]
            CapCuu = 11,
            [Description("Nhập viện")]
            NhapVien = 12
        }

        public enum EnumLoaiYeuCauTiepNhan
        {
            [Description("Khám chữa bệnh ngoại trú")]
            KhamChuaBenhNgoaiTru = 1,
            [Description("Khám chữa bệnh nội trú")]
            KhamChuaBenhNoiTru = 2,

            //[Description("Thực hiện dịch vụ kỹ thuật")]
            //ThucHienDichVuKyThuat = 2,
            //[Description("Dịch vụ khác")]
            //DichVuKhac = 3,
            [Description("Mua thuốc")]
            MuaThuoc = 4,

            [Description("Đăng ký gói marketing")]
            DangKyGoiMarketing = 5,
            [Description("Khám sức khỏe")]
            KhamSucKhoe = 6,
        }

        public enum EnumYeuCauDuocPhamBenhVien
        {
            //[Description("None")]
            //None = 1,
            [Description("Chưa thực hiện")]
            ChuaThucHien = 1,
            [Description("Đã thực hiện")]
            DaThucHien = 2,
            [Description("Đã hủy")]
            DaHuy = 3
        }

        public enum EnumTrangThaiYeuCauTruyenMau
        {
            //[Description("None")]
            //None = 1,
            [Description("Chưa thực hiện")]
            ChuaThucHien = 1,
            [Description("Đã thực hiện")]
            DaThucHien = 2,
            [Description("Đã hủy")]
            DaHuy = 3
        }

        public enum EnumYeuCauVatTuBenhVien
        {
            //[Description("None")]
            //None = 1,
            [Description("Chưa thực hiện")]
            ChuaThucHien = 1,
            [Description("Đã thực hiện")]
            DaThucHien = 2,
            [Description("Đã hủy")]
            DaHuy = 3
        }

        public enum EnumTrangThaiYeuCauTiepNhan
        {
            [Description("Đang thực hiện")]
            DangThucHien = 1,
            [Description("Đã hoàn tất")]
            DaHoanTat = 2,
            [Description("Đã hủy")]
            DaHuy = 3,
        }
        //public enum LoaiMienGiamThem
        //{
        //    [Description("Miễn giảm theo số tiền")]
        //    MienGiamTheoSoTien = 1,
        //    [Description("Miễn giảm theo tỉ lệ")]
        //    MienGiamTheoTiLe = 2,
        //}

        public enum BoPhan
        {
            [Description("Tiếp nhận ngoại trú")]
            TiepNhanNgoaiTru = 1,
            [Description("Tiếp nhận nội trú")]
            TiepNhanNoiTru = 2,
            [Description("Khám bệnh")]
            KhamBenh = 3,
            [Description("PTTT")]
            PTTT = 4,
            [Description("Tiêm chủng")]
            TiemChung = 5,
        }
    }
}