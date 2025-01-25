using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.Voucher
{
    public class VoucherMarketingGridVo : GridItem
    {
        public int? STT { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public int SoLuongPhatHanh { get; set; }
        public string SoLuongPhatHanhDisplay => SoLuongPhatHanh == 0 ? SoLuongPhatHanh.ToString() : SoLuongPhatHanh.ApplyNumber();
        public DateTime TuNgay { get; set; }
        public string TuNgayDisplay => TuNgay.ApplyFormatDate();
        public DateTime? DenNgay { get; set; }
        public string DenNgayDisplay => DenNgay != null ? DenNgay.Value.ApplyFormatDate() : "";
        //public string MoTa { get; set; }
        //public bool ChietKhauTatCaDichVu { get; set; }
        //public int LoaiChietKhau { get; set; }
        //public int TiLeChietKhau { get; set; }
        //public double SoTienChietKhau { get; set; }
    }

    public class DichVuVoucherMarketingGridVo : GridItem
    {
        //public string NhomDichVuDisplay { get; set; }
        public long VoucherId { get; set; }
        public EnumDichVuTongHop LoaiDichVuBenhVien { get; set; }
        public long DichVuId { get; set; }
        public string MaDichVu { get; set; }
        public string DichVuDisplay { get; set; }
        public long LoaiGiaId { get; set; }
        public string LoaiGiaDisplay { get; set; }
        public decimal DonGia { get; set; }
        //public decimal DonGiaSauChietKhau { get; set; }
        public LoaiChietKhau LoaiChietKhau { get; set; }
        public int? TiLeChietKhau { get; set; }
        public decimal? SoTienChietKhau { get; set; }
        public string GhiChu { get; set; }
        public long NhomDichVuId { get; set; }
        public string NhomDichVuDisplay { get; set; }
    }

    public class ChiTietBenhNhanDaSuDungGridVo : GridItem
    {
        public string MaTiepNhan { get; set; }
        public string MaBenhNhan { get; set; }
        public string TenBenhNhan { get; set; }
        public string DiaChi { get; set; }
        public DateTime NgayDung { get; set; }
        public string NgayDungDisplay => NgayDung.ApplyFormatDate();
        public string MaVoucher { get; set; }
    }

    public class SoLuongPhatHanhVoucher
    {
        public int SoLuong { get; set; }
        public string SoLuongDisplay => SoLuong == 0 ? SoLuong.ToString() : SoLuong.ApplyNumber();
    }

    public class VoucherSearch
    {
        public string SearchString { get; set; }
        public RangeDate RangeNgayApDung { get; set; }
    }

    public class RangeDate
    {
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
    }
}
