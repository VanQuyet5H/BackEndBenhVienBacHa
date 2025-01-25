using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.KhamBenhs
{
    public class PhongKhamChuyenDenInfoVo
    {
        public PhongKhamChuyenDenInfoVo()
        {
            HangDoiIds = new List<long>();
        }
        public List<long> HangDoiIds { get; set; }
        public long PhongHienTaiId { get; set; }
        public long PhongThucHienId { get; set; }
    }
}
