using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.YeuCauTiepNhans
{
    public class KetQuaKiemTraTaoMoiYeuCauTiepNhanVo
    {
        public string ErrorMessage { get; set; }
        public long? YeuCauTiepNhanId { get; set; }
    }

    public class ThongTinThucHienVaThanhToanDichVuVo
    {
        public long YeuCauTiepNhanId { get; set; }
        public bool ChuaThucHien { get; set; }
        public bool ChuaThanhToan { get; set; }
    }
}
