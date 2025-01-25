using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum LoaiHangDoiTiemVacxin
        {
            [Description("Khám sàng lọc")]
            KhamSangLoc = 1,
            [Description("Thực hiện tiêm")]
            ThucHienTiem = 2,
            [Description("Lịch sử tiêm")]
            LichSuTiem = 3
        }

        public enum LoaiTienSuTiemVacxin
        {
            [Description("Bản thân")]
            BanThan = 1,
            [Description("Gia đình")]
            GiaDinh = 2
        }

        public enum LoaiDiUngTiemVacxin
        {
            [Description("Thuốc")]
            Thuoc = 1,
            [Description("Thức ăn")]
            ThucAn = 2,
            [Description("Khác")]
            Khac = 3
        }

        public enum MucDoDiUngTiemVacxin
        {
            //Xác nhận lại DS enum
            [Description("Nặng")]
            Nang = 1
        }

        public enum NoiXuTriTheoDoiTiemVacxin
        {
            [Description("Tại nhà")]
            TaiNha = 1,
            [Description("Tại trạm y tế")]
            TaiTramYTe = 2,
            [Description("Bệnh viện tuyến huyện")]
            BenhVienTuyenHuyen = 3,
            [Description("Bệnh viện tuyến tỉnh, trung ương")]
            BenhVienTuyenTinhTrungUong = 4,
            [Description("Y tế tư nhân")]
            YTeTuNhan = 5,
            [Description("Khác")]
            Khac = 6
        }

        public enum TinhTrangHienTaiTheoDoiTiemVacxin
        {
            [Description("Khỏi")]
            Khoi = 1,
            [Description("Di chứng")]
            DiChung = 2,
            [Description("Tử vong")]
            TuVong = 3,
            [Description("Khác")]
            Khac = 4
        }
    }
}
