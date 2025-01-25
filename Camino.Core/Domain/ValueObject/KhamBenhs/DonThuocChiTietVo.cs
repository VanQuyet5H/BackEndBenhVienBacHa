using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.KhamBenhs
{
    public enum LoaiKhoThuoc
    {
        ThuocBHYT = 1,
        ThuocBenhVien = 2,
        ThuocNgoaiBenhVien = 3
    }
    public class DonThuocChiTietVo
    {
        public long DonThuocChiTietId { get; set; }
        public long YeuCauKhamBenhId { get; set; }
        public long? YeuCauTiepNhanId { get; set; }
        public long DuocPhamId { get; set; }
        public double SoLuong { get; set; }
        public int? SoNgayDung { get; set; }
        public int? ThoiGianDungSang { get; set; }
        public int? ThoiGianDungTrua { get; set; }
        public int? ThoiGianDungChieu { get; set; }
        public int? ThoiGianDungToi { get; set; }
        public double? DungSang { get; set; }
        public double? DungTrua { get; set; }
        public double? DungChieu { get; set; }
        public double? DungToi { get; set; }
        public double? SoLanTrenVien { get; set; }
        public double? LieuDungTrenNgay { get; set; }
        public int? SoThuTu { get; set; }

        public Enums.EnumLoaiDonThuoc LoaiDonThuoc => LoaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT
            ? Enums.EnumLoaiDonThuoc.ThuocBHYT
            : Enums.EnumLoaiDonThuoc.ThuocKhongBHYT;
        public LoaiKhoThuoc LoaiKhoThuoc { get; set; }
        public string GhiChu { get; set; }
        public bool? LaTangSTT { get; set; }
    }
    public class XoaDonThuocTheoYeuCauKhamBenhVo
    {
        public long YeuCauKhamBenhId { get; set; }
    }
    public class VatTuChiTietVo
    {
        public long DonVTYTChiTietId { get; set; }
        public long YeuCauKhamBenhId { get; set; }
        public long VatTuId { get; set; }
        public double SoLuong { get; set; }
        public string GhiChu { get; set; }
    }

    public class DonThuocChiTietTangGiamSTTVo
    {
        public long DonThuocChiTietId { get; set; }
        public long YeuCauKhamBenhId { get; set; }

        public Enums.EnumLoaiDonThuoc LoaiDonThuoc { get; set; }
        public bool? LaTangSTT { get; set; }
    }
}
