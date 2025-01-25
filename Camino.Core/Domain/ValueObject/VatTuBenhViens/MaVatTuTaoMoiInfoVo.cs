using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.VatTuBenhViens
{
    public class MaVatTuTaoMoiInfoVo
    {
        public MaVatTuTaoMoiInfoVo()
        {
            MaVatTuTemps = new List<string>();
        }
        public string TenVatTu { get; set; }
        public List<string> MaVatTuTemps { get; set; }

        // dùng riêng để xử lý job cập nhật mã
        public long? VatTuMaCuoiCungId { get; set; }
    }
}
