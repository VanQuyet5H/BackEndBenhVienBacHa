using System;

namespace Camino.Core.Domain.ValueObject.BaoCao
{
    public class BaoCaoKetQuaXetNghiemQueryInfo : QueryInfo
    {
        public long? NoiChiDinhId { get; set; }
        public bool? BHYT { get; set; }
        public bool? KhamSucKhoe { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string Hosting { get; set; }
        public string TuNgayUTC { get; set; }
        public string DenNgayUTC { get; set; }
    }
    public class BaoCaoKetQuaXetNghiemIn 
    {
        public string NguoiLap { get; set; }
        public string TuNgay { get; set; }
        public string DenNgay { get; set; }
        public string Data { get; set; }
        public string GioDenNgay { get; set; }
        public string GioTuNgay { get; set; }
    }
}