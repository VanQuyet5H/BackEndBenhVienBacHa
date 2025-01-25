using Camino.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.Thuoc
{
    public class NhomThuocViewModel : BaseViewModel
    {
        public string Ten { get; set; }
        public long? NhomChaId { get; set; }
        public int CapNhom { get; set; }
        public  string TenLoaiHoaChat { get; set; }
        public Enums.LoaiThuocHoacHoatChat LoaiThuocHoacHoatChat { get; set; }
    }
}
