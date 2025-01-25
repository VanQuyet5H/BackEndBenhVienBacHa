using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;

namespace Camino.Api.Models.KhamBenh
{
    public class GoiDichVuChiTietNoiThucHienViewModel: BaseViewModel
    {
        public int NhomId { get; set; }
        public string Nhom { get; set; }
        public string Ma { get; set; }
        public string TenDichVu { get; set; }
        public string NoiThucHien { get; set; }
        public long? NoiThucHienId { get; set; }
        public string NoiThucHienIdStr { get; set; }
        public string LoaiGia { get; set; }
        public long LoaiYeuCauDichVuId { get; set; }
    }
}
