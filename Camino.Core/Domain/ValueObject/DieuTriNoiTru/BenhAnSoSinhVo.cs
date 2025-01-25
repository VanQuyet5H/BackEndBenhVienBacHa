using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using static Camino.Core.Domain.Enums;
namespace Camino.Core.Domain.ValueObject.DieuTriNoiTru
{
    public class ThongTinBenhAnMe : GridItem
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }

        public long YeuCauTiepNhanMeId { get; set; }
        public string MaBA { get; set; }
        public string MaTN { get; set; }
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public string NamSinh { get; set; }        
    }
}
