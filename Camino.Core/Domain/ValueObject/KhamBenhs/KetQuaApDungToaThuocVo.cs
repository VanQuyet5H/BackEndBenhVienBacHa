using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.KhamBenhs
{
    public class KetQuaApDungToaThuocVo
    {
        public bool ThanhCong { get; set; }
        public string Error { get; set; }
        public List<ApDungToaThuocChiTietVo> ApDungToaThuocChiTietVos { get; set; }
        public string MucTranChiPhi { get; set; }
    }
    public class ApDungToaThuocChiTietVo
    {
        public long DuocPhamId { get; set; }
        public string Ten { get; set; }
        public string HoatChat { get; set; }
        public string DVT { get; set; }
        public string DuongDung { get; set; }
        public string Nhom { get; set; }
        public double SoLuong { get; set; }
        public string SoLuongDisplay { get; set; }
        public double SoLuongTonKho { get; set; }
        public int? SoNgayDung { get; set; }
        public double? DungSang { get; set; }
        public double? DungTrua { get; set; }
        public double? DungChieu { get; set; }
        public double? DungToi { get; set; }
        public int? ThoiGianDungSang { get; set; }
        public int? ThoiGianDungTrua { get; set; }
        public int? ThoiGianDungChieu { get; set; }
        public int? ThoiGianDungToi { get; set; }
        public string ThoiGianDungSangDisplay { get; set; }
        public string ThoiGianDungTruaDisplay { get; set; }
        public string ThoiGianDungChieuDisplay { get; set; }
        public string ThoiGianDungToiDisplay { get; set; }
        public string SangDisplay { get; set; }
        public string TruaDisplay { get; set; }
        public string ChieuDisplay { get; set; }
        public string ToiDisplay { get; set; }
        public LoaiKhoThuoc LoaiKhoThuoc { get; set; }
        public string GhiChu { get; set; }
    }
}
