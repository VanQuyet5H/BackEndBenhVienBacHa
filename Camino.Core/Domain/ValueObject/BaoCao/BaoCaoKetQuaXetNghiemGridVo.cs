using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject
{
    public class BaoCaoKetQuaXetNghiemGridVo : GridItem
    {
        public int STT { get; set; }
        public string SID { get; set; }
        public string HoTen { get; set; }
        public string NamSinh { get; set; }
        public string GioiTinh { get; set; }
        public string NoiChiDinh { get; set; }
        public bool BHYT { get; set; }
        public bool KSK { get; set; }
        public string BacSi { get; set; }
        public string ChanDoan { get; set; }
        public string KetQua { get; set; }
    }
}