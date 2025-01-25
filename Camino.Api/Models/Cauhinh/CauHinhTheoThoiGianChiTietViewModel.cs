using System;

namespace Camino.Api.Models.Cauhinh
{
    public class CauHinhTheoThoiGianChiTietViewModel : BaseViewModel
    {
        public long CauHinhTheoThoiGianId { get; set; }

        public string Value { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public CauhinhViewModel CauHinhTheoThoiGian { get; set; }
    }
}
