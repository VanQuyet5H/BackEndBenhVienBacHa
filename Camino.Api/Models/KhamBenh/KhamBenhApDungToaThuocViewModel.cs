using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.KhamBenh
{
    public class KhamBenhApDungToaThuocToaMauViewModel
    {
        public long YeuCauKhamBenhHienTaiId { get; set; }
        public long ToaMauId { get; set; }
    }
    public class KhamBenhApDungToaThuocLichSuKeToaViewModel
    {
        public long YeuCauKhamBenhTruocId { get; set; }
        public long YeuCauKhamBenhHienTaiId { get; set; }
    }
    public class GhiChuDonThuocViewModel
    {
        public long YeuCauKhamBenhId { get; set; }
        public string GhiChu { get; set; }
    }
}
