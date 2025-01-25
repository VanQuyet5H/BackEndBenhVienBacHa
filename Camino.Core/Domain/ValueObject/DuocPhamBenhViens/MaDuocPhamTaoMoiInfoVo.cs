using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.DuocPhamBenhViens
{
    public class MaDuocPhamTaoMoiInfoVo
    {
        public MaDuocPhamTaoMoiInfoVo()
        {
            MaDuocPhamTemps = new List<string>();
        }
        public string TenDuocPham { get; set; }
        public long? PhanNhomId { get; set; }
        public List<string> MaDuocPhamTemps { get; set; }

        // dùng riêng để xử lý job cập nhật mã
        public long? DuocPhamNhatMaCuoiCungId { get; set; }
    }
}
