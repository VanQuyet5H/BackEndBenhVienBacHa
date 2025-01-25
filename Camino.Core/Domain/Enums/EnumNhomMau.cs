using System.ComponentModel;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum EnumNhomMau
        {
            [Description("A")]
            A = 1,

            [Description("B")]
            B = 2,

            [Description("AB")]
            AB = 3,

            [Description("O")]
            O = 4
        }
        public enum EnumYeuToRh
        {
            //[Description("Âm tính")]
            [Description("-")]
            Amtinh = 1,

            //[Description("Dương tính")]
            [Description("+")]
            DuongTinh = 2,
        }
    }
}