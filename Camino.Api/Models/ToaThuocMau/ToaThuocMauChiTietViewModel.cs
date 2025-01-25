using Camino.Api.Models.Thuoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.ToaThuocMau
{
    public class ToaThuocMauChiTietViewModel : BaseViewModel
    {
        public long? ToaThuocMauId { get; set; }
        public long? DuocPhamId { get; set; }
        public string TenDuocPham { get; set; }
        public double? SoLuong { get; set; }
        public int? SoNgayDung { get; set; }
        public double? DungSang { get; set; }
        public double? DungTrua { get; set; }
        public double? DungChieu { get; set; }
        public double? DungToi { get; set; }
        public string DungSangDisplay { get; set; }
        public string DungTruaDisplay { get; set; }
        public string DungChieuDisplay { get; set; }
        public string DungToiDisplay { get; set; }
        public int? ThoiGianDungSang { get; set; }
        public int? ThoiGianDungTrua { get; set; }
        public int? ThoiGianDungChieu { get; set; }
        public int? ThoiGianDungToi { get; set; }
        public string GhiChu { get; set; }
        public virtual DuocPhamViewModel DuocPham { get; set; }

    }
}
