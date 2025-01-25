using System.ComponentModel;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum EnumTrangThaiYeuCauDichVuKyThuat
        {
            //[Description("Chưa đóng tiền")]
            //ChuaDongTien = 0,
            [Description("Chưa thực hiện")]
            ChuaThucHien = 1,
            [Description("Đang thực hiện")]
            DangThucHien = 2,
            [Description("Đã thực hiện")]
            DaThucHien = 3,
            [Description("Đã hủy")]
            DaHuy = 4
        }

        public enum EnumTrangThaiYeuCauGoiDichVu
        {
            //[Description("Chưa đóng tiền")]
            //ChuaDongTien = 0,
            [Description("Chưa thực hiện")]
            ChuaThucHien = 1,
            [Description("Đang sử dụng")]
            DangThucHien = 2,
            [Description("Đã thực hiện")]
            DaThucHien = 3,
            [Description("Đã hủy")]
            DaHuy = 4
        }
        public enum EnumTrangThaiGiuongBenh
        {
            //[Description("None")]
            //None = 1,
            [Description("Chưa thực hiện")]
            ChuaThucHien = 1,
            [Description("Đang thực hiện")]
            DangThucHien = 2,
            [Description("Đã thực hiện")]
            DaThucHien = 3,
            [Description("Đã hủy")]
            DaHuy = 4
        }

        public enum EnumMucDoDiUng
        {
            [Description("Nhẹ")]
            Nhe = 1,
            [Description("Vừa")]
            Vua = 2,
            [Description("Nặng")]
            Nang = 3,
            [Description("Đe dọa tử vong")]
            DeDoaTuVong = 4
        }

        public enum EnumLoaiTienSuBenh
        {
            [Description("Bản thân")]
            BanThan = 1,
            [Description("Gia đình")]
            GiaDinh = 2
        }

        public enum TinhTrangBenhNhan
        {
            [Description("Đang điều trị")]
            DangDieuTri = 1,
            [Description("Hoàn tất điều trị")]
            HoanTatDieuTri = 2
        }
    }
}
