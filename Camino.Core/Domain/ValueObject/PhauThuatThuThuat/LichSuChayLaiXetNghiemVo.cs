using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.PhauThuatThuThuat
{
    public class LichSuChayLaiXetNghiemVo
    {
        public LichSuChayLaiXetNghiemVo()
        {
            LichSuPhienXetNghiemIds = new List<long>();
        }

        public long NhomDichVuBenhVienId { get; set; }
        public List<long> LichSuPhienXetNghiemIds { get; set; }
    }
}