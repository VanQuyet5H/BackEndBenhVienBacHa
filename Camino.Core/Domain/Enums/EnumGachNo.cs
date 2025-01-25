using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum LoaiChungTu
        {
            [Description("Báo có ngân hàng")]
            BaoCoNganHang = 1,
            [Description("Phiếu thu")]
            PhieuThu = 2
        }

        public enum LoaiTienTe
        {
            [Description("VNĐ")]
            VND = 1,
            [Description("USD")]
            USD = 2
        }

        public enum LoaiDoiTuong
        {
            [Description("BHTN")]
            BHTN = 1,
            [Description("Người bệnh")]
            BenhNhan = 2
        }

        public enum TrangThaiGachNo
        {
            [Description("Nhập liệu")]
            NhapLieu = 1,
            [Description("Xác nhận nhập liệu")]
            XacNhanNhapLieu = 2
        }

        public enum DienGiaiGridGachNo
        {
            [Description("Đầu kỳ")]
            DauKy = 1
        }
    }
}
