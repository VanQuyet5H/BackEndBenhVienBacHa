using System.ComponentModel;

namespace Camino.Core.Domain
{
    //Temp
    public partial class Enums
    {
        #region Ekip thực hiện
        public enum EnumVaiTroBacSi
        {
            [Description("Phẫu thuật viên chính")]
            PhauThuatVienChinh = 1,
            [Description("Gây mê/tê chính")]
            GayMeTeChinh = 2,
            [Description("Gây mê/tê phụ")]
            GayMeTePhu = 3,
            [Description("BS Hội chẩn")]
            BacSiHoiChan = 4,
            [Description("BS Tăng cường")]
            BacSiTangCuong = 5,
            [Description("Phụ 1")]
            Phu1 = 6,
            [Description("Phụ 2")]
            Phu2 = 7,
            [Description("Phụ 3")]
            Phu3 = 8
        }

        public enum EnumVaiTroDieuDuong
        {
            [Description("Phụ phẫu thuật")]
            PhuPhauThuat = 1,
            [Description("Dụng cụ vòng trong")]
            DungCuVongTrong = 2,
            [Description("Dụng cụ vòng ngoài")]
            DungCuVongNgoai = 3,
            [Description("Phụ 1")]
            Phu1 = 4,
            [Description("Phụ 2")]
            Phu2 = 5,
            [Description("Phụ 3")]
            Phu3 = 6,
            [Description("Phụ mê/phụ tê")]
            PhuMePhuTe = 7,
            [Description("Chạy ngoài")]
            ChayNgoai = 8,
            [Description("Phẫu thuật viên chính")]
            PhauThuatVienChinh = 9
        }
        #endregion

        #region Tình trạng
        public enum EnumTrangThaiPhauThuatThuThuat
        {
            [Description("Chờ phẫu thuật")]
            ChoPhauThuat = 1,
            [Description("Đang phẫu thuật")]
            DangPhauThuat = 2,
            [Description("Theo dõi")]
            TheoDoi = 3,
            [Description("Chuyển giao")]
            ChuyenGiao = 4,
            [Description("Tử vong")]
            TuVong = 5,
            [Description("Không có")]
            KhongCo = 6
        }

        public enum EnumTrangThaiTheoDoiSauPhauThuatThuThuat
        {
            [Description("Đang theo dõi")]
            DangTheoDoi = 1,
            [Description("Kết thúc theo dõi")]
            KetThucTheoDoi = 2
        }
        #endregion

        #region Vật tư / thuốc
        public enum EnumGiaiDoanPhauThuat
        {
            [Description("Gây mê")]
            GayMe = 1,
            [Description("Phẫu thuật")]
            PhauThuat = 2,
            [Description("Hồi tỉnh")]
            HoiTinh = 3,
            [Description("Khác")]
            Khac = 4
        }
        #endregion

        #region BVHD-3882
        public enum EnumDichVuThuePhong
        {
            [Description("Dịch vụ thuê phòng id")]
            Id = 5500
        }
        #endregion
    }
}
