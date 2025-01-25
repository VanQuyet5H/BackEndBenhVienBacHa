using System;

namespace Camino.Api.Models.BenhNhanTienSuBenhs
{
    public class BenhNhanTienSuBenhViewModel : BaseViewModel
    {
        public string TenBenh { get; set; }

        public long BenhNhanId { get; set; }

        public DateTime? NgayPhatHien { get; set; }

        public string TenTinhTrang { get; set; }
    }
}
