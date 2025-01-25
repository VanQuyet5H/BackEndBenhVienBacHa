using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;

namespace Camino.Api.Models.DieuTriNoiTru
{
    public class NoiTruGiayMienCungChiTraViewModel : BaseViewModel
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenGuid { get; set; }
        public long? KichThuoc { get; set; }
        public string DuongDan { get; set; }
        public string MoTa { get; set; }
        public Enums.LoaiTapTin? LoaiTapTin { get; set; }
    }
}
