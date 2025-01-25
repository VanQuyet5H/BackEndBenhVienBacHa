using System.ComponentModel;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        //Khám bệnh
        public enum EnumTuVongTrongPTTT
        {
            [Description("Không")]
            Khong = 1,
            [Description("Trên bàn mổ")]
            TrenBanMo = 2,
            [Description("Trong 24 giờ")]
            Trong24Gio = 3,
            [Description("Sau 24 giờ")]
            Sau24Gio = 4
        }

        //PTTT theo ngày
        public enum EnumThoiGianTuVongPTTTTheoNgay
        {
            [Description("Trước 24 giờ")]
            Truoc24Gio = 1,
            [Description("Sau 24 giờ")]
            Sau24Gio = 2
        }

        //PTTT theo ngày
        public enum EnumTuVongPTTTTheoNgay
        {
            [Description("Trước phẫu thuật")]
            TruocPhauThuat = 1,
            [Description("Khi gây mê")]
            KhiGayMe = 2,
            [Description("Trong phẫu thuật")]
            TrongPhauThuat = 3,
            [Description("Sau phẫu thuật")]
            SauPhauThuat = 4
        }
    }
}
