using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.YeuCauKhamBenhChanDoanPhanBiet
{
    public class YeuCauKhamBenhChanDoanPhanBietViewModel : BaseViewModel
    {
        public long? ICDId { get; set; }
        public string TenICD { get; set; }
        public string GhiChu { get; set; }
        public long YeuCauKhamBenhId { get; set; }
    }
    public class YeuCauKhamBenhChanDoanPhanBietReturnViewModel
    {
        public long ICDId { get; set; }

        public string GhiChu { get; set; }

        public long YeuCauKhamBenhId { get; set; }

    }
}
