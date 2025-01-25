using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.KhamDoan
{
    public class YeuCauDuyetPhongNhanSu : GridItem
    {
        public List<NhanSuVo> NhanSus { get; set; }
    }

    public class NhanSuVo : GridItem
    {
        public string HoTen { get; set; }

        public string DonVi { get; set; }

        public long? NguoiGioiThieuId { get; set; }

        public string SoDienThoai { get; set; }

        public string GhiChu { get; set; }
    }
}
