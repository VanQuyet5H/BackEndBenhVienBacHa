using System.ComponentModel;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum EnumTrangThaiLoaiDuTru
        {
            [Description("Chờ duyệt")]
            ChoDuyet = 1,
            [Description("Chờ gửi")]
            ChoGoi = 2,
        }
        public enum EnumTrangThaiLoaiDuTruDaXuLy
        {
            [Description("Đã gửi & Chờ duyệt")]
            DaGoiVaChoDuyet = 1,
            [Description("Đã duyệt")]
            DaDuyet = 2,
            [Description("Từ chối")]
            TuChoi = 3
        }
        public enum EnumTrangThaiDuTruKhoaDuoc
        {
            [Description("Chờ duyệt")]
            ChoDuyet = 1,
            [Description("Chờ gửi")]
            ChoGoi = 2,
            [Description("Đã gửi & Chờ duyệt")]
            DaGoiVaChoDuyet = 3,
            [Description("Đã duyệt")]
            DaDuyet = 4,
            [Description("Từ chối")]
            TuChoi = 5
        }
    }

}