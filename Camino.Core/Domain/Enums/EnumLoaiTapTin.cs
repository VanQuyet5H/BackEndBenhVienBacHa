using System.ComponentModel;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum LoaiTapTin
        {
            [Description("Hình ảnh")]
            Image = 1,
            [Description("Pdf")]
            Pdf = 2,
            [Description("Khác")]
            Khac = 10
        }
    }
}