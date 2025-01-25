using Camino.Core.Domain.ValueObject.Grid;
using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject
{
    public class NhomICDTheoBenhVienGridVo : GridItem
    {
        public string Stt { get; set; }

        public string Ma { get; set; }

        public string MaICD { get; set; }

        public string TenTiengViet { get; set; }

        public string TenChuongTiengViet { get; set; }

        public bool HieuLuc { get; set; }
    }
    public class JsonMaICD
    {
        public string MaICD { get; set; }
    }
}
