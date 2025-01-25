using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.DieuTriNoiTru
{
    public class NoiTruDonThuocChiTietViewModel : BaseViewModel
    {
        public long? NoiTruDonThuocId { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public long? DuocPhamId { get; set; }
        public bool? LaDuocPhamBenhVien { get; set; }
        public double? SoLuong { get; set; }
        public int? SoNgayDung { get; set; }
        public string GhiChu { get; set; }
        public int LoaiKhoThuoc { get; set; }
        public string SangDisplay { get; set; }
        public string TruaDisplay { get; set; }
        public string ChieuDisplay { get; set; }
        public string ToiDisplay { get; set; }
        public int? ThoiGianDungSang { get; set; }
        public int? ThoiGianDungTrua { get; set; }
        public int? ThoiGianDungChieu { get; set; }
        public int? ThoiGianDungToi { get; set; }
       
    }
}
