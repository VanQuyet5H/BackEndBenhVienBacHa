using Camino.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.YeuCauKhamBenh
{
    public class YeuCauKhamBenhDonThuocChiTietEditViewModel : BaseViewModel 
    {
        public long? YeuCauKhamBenhDonThuocId { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public long? DuocPhamId { get; set; }
        public bool? LaDuocPhamBenhVien { get; set; }
        public string Ten { get; set; }
        public string TenTiengAnh { get; set; }
        public string SoDangKy { get; set; }
        public string MaHoatChat { get; set; }
        public string HoatChat { get; set; }
        public Enums.LoaiThuocHoacHoatChat? LoaiThuocHoacHoatChat { get; set; }
        public long? DuongDungId { get; set; }
        public string TieuChuan { get; set; }
        public long? DonViTinhId { get; set; }
        public string HuongDan { get; set; }
        public string TacDungPhu { get; set; }
        public double? SoLuong { get; set; }
        public int? SoNgayDung { get; set; }
        public double? DungSang { get; set; }
        public double? DungTrua { get; set; }
        public double? DungChieu { get; set; }
        public double? DungToi { get; set; }
        public string SangDisplay { get; set; }
        public string TruaDisplay { get; set; }
        public string ChieuDisplay { get; set; }
        public string ToiDisplay { get; set; }
        public bool? DuocHuongBaoHiem { get; set; }
        public bool? BenhNhanMuaNgoai { get; set; }
        public string GhiChu { get; set; }
        public string TuongTacThuoc { get; set; }
        public Enums.EnumLoaiDonThuoc? LoaiDonThuoc { get; set; }
        public int LoaiKhoThuoc { get; set; }
        public long? NoiKeDonId { get; set; }
        public decimal? Gia { get; set; }
    }
}
