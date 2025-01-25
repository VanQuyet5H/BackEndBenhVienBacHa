using Camino.Core.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.BaoCaoLuuKetQuaXetNghiemHangNgay
{
    public class BaoCaoLuuKetQuaXetNghiemHangNgayViewModel : QueryInfo
    {
        public long? NoiChiDinhId { get; set; }
        public bool? BHYT { get; set; }
        public bool? KhamSucKhoe { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
    }
}
