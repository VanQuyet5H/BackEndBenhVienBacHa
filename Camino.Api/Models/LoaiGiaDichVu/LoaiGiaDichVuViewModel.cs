using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Helpers;

namespace Camino.Api.Models.LoaiGiaDichVu
{
    public class LoaiGiaDichVuViewModel : BaseViewModel
    {
        public string Ten { get; set; }
        public Enums.NhomDichVuLoaiGia? Nhom { get; set; }
        public string TenNhom => Nhom.GetDescription();
    }
}
