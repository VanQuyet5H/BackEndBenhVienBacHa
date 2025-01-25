using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;

namespace Camino.Api.Models.MauVaChePham
{
    public class MauVaChePhamViewModel :BaseViewModel
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public Enums.PhanLoaiMau PhanLoaiMau { get; set; }
        public string TenPhanLoaiMau { get; set; }
        public long TheTich { get; set; }
        public long GiaTriToiDa { get; set; }
        public string GhiChu { get; set; }
    }
}
