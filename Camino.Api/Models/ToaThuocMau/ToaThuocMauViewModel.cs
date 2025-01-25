using Camino.Core.Domain;
using System.Collections.Generic;

namespace Camino.Api.Models.ToaThuocMau
{
    public class ToaThuocMauViewModel : BaseViewModel
    {
        public ToaThuocMauViewModel()
        {
            ToaThuocMauChiTiets = new List<ToaThuocMauChiTietViewModel>();
        }
        public string Ten { get; set; }
        public long? ICDId { get; set; }
        public long? TrieuChungId { get; set; }
        public long? ChuanDoanId { get; set; }
        public string GhiChu { get; set; }
        public long? BacSiKeToaId { get; set; }
        public bool? IsDisabled { get; set; }
        public string TenBacSiKeToa { get; set; }
        public string TenTrieuChung { get; set; }
        public string TenChuanDoan { get; set; }
        public string TenICD { get; set; }
        public List<ToaThuocMauChiTietViewModel> ToaThuocMauChiTiets { get; set; }
    }
}
