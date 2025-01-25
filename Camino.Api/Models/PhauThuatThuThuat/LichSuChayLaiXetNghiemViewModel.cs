using System.Collections.Generic;

namespace Camino.Api.Models.PhauThuatThuThuat
{
    public class LichSuChayLaiXetNghiemViewModel
    {
        public LichSuChayLaiXetNghiemViewModel()
        {
            LichSuPhienXetNghiemIds = new List<long>();
        }

        public long NhomDichVuBenhVienId { get; set; }
        public List<long> LichSuPhienXetNghiemIds { get; set; }
    }
}
