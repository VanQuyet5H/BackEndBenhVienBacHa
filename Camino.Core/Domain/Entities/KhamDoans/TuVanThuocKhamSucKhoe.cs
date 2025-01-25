using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DonViTinhs;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.Entities.KhamDoans
{
    public class TuVanThuocKhamSucKhoe : BaseEntity
    {
        public long YeuCauTiepNhanId { get; set; }
        public long DuocPhamId { get; set; }
        public bool LaDuocPhamBenhVien { get; set; }
        public string Ten { get; set; }
        public string TenTiengAnh { get; set; }
        public string SoDangKy { get; set; }
        public int? STTHoatChat { get; set; }
        public string MaHoatChat { get; set; }
        public string HoatChat { get; set; }
        public LoaiThuocHoacHoatChat LoaiThuocHoacHoatChat { get; set; }
        public string NhaSanXuat { get; set; }
        public string NuocSanXuat { get; set; }
        public long DuongDungId { get; set; }
        public string HamLuong { get; set; }
        public string QuyCach { get; set; }
        public string TieuChuan { get; set; }
        public string DangBaoChe { get; set; }
        public long DonViTinhId { get; set; }
        public string HuongDan { get; set; }
        public string MoTa { get; set; }
        public string ChiDinh { get; set; }
        public string ChongChiDinh { get; set; }
        public string LieuLuongCachDung { get; set; }
        public string TacDungPhu { get; set; }
        public string ChuYDePhong { get; set; }
        public double SoLuong { get; set; }
        public int? SoNgayDung { get; set; }
        public double? DungSang { get; set; }
        public double? DungTrua { get; set; }
        public double? DungChieu { get; set; }
        public double? DungToi { get; set; }
        public int? ThoiGianDungSang { get; set; }
        public int? ThoiGianDungTrua { get; set; }
        public int? ThoiGianDungChieu { get; set; }
        public int? ThoiGianDungToi { get; set; }
        public string GhiChu { get; set; }


        public virtual YeuCauTiepNhan YeuCauTiepNhan { get; set; }
        public virtual DuocPham DuocPham { get; set; }
        public virtual DuongDung DuongDung { get; set; }
        public virtual DonViTinh DonViTinh { get; set; }
    }
}
