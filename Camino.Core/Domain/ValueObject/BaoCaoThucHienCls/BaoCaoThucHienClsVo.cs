using System;
using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauMuaDuocPham;
using Camino.Core.Helpers;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.BaoCaoThucHienCls
{
    public class BaoCaoThucHienClsVo : GridItem
    {
        public string NhomDv { get; set; }

        public DateTime? ThoiGianChiDinh { get; set; }

        public string ThoiGianChiDinhDisplay => ThoiGianChiDinh != null ? ThoiGianChiDinh.GetValueOrDefault().ApplyFormatDateTime() : string.Empty;

        public string MaTn { get; set; }

        public string HoTenBn { get; set; }

        public long BenhNhanId { get; set; }

        public DateTime? NgaySinh { get; set; }

        public string NgaySinhDisplay => NgaySinh != null ? NgaySinh.GetValueOrDefault().ApplyFormatDate() : string.Empty;

        public Enums.LoaiGioiTinh GioiTinh { get; set; }

        public string KhoaChiDinh { get; set; }

        public string MaDv { get; set; }

        public string DichVu { get; set; }

        public string PhongThucHien { get; set; }

        public string BsKetLuan { get; set; }

        public DateTime? ThoiGianCoKq { get; set; }

        public string ThoiGianCoKqDisplay =>
            ThoiGianCoKq != null ? ThoiGianCoKq.GetValueOrDefault().ApplyFormatDateTime() : string.Empty;
    }

    public class BaoCaoThucHienCLSVo : GridItem
    {
        public BaoCaoThucHienCLSVo()
        {
            BaoCaoThucHienCLSChiTietVos = new List<BaoCaoThucHienCLSChiTietVo>();
        }
        public long? KhoaId { get; set; }

        public long? BacSiKetLuanId { get; set; }
        public string TenBacSiKetLuan { get; set; }
        public int SoBacSiKetLuan { get; set; }
        //public RangeDates RangeFromDate { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public List<BaoCaoThucHienCLSChiTietVo> BaoCaoThucHienCLSChiTietVos { get; set; }
    }

    public class BaoCaoThucHienCLSChiTietVo : GridItem
    {
        public BaoCaoThucHienCLSChiTietVo()
        {
            //KetLuans = new List<string>();
            ChiTietKetQuaObj = new ChiTietKetLuanCDHATDCNJSON();
        }
        public string TenDichVu { get; set; }
        public int? SoLan { get; set; }
        public DateTime? NgayThucHien { get; set; }
        public string NgayThucHienDisplay => NgayThucHien?.ApplyFormatDateTime();
        public string MaTN { get; set; }
        public string SoBA { get; set; }
        public string HoTen { get; set; }
        public int? NamSinh { get; set; }
        public string DiaChi { get; set; }
        public string TenBacSiChiDinh { get; set; }
        public string KTV { get; set; }

        //public string KetLuan
        //{
        //    get
        //    {
        //        var result = string.Empty;
        //        if (KetLuans.Count > 0)
        //        {
        //            foreach (var ketluan in KetLuans)
        //            {
        //                result += MaskHelper.RemoveHtmlFromString(ketluan) + "; ";
        //            }
        //        }
        //        return result;
        //    }
        //}

        //public List<string> KetLuans { get; set; }
        //public string KetLuan { get; set; }

        public string KetLuan => ChiTietKetQuaObj != null && !string.IsNullOrEmpty(ChiTietKetQuaObj.KetLuan) ? MaskHelper.RemoveHtmlFromString(ChiTietKetQuaObj.KetLuan) : "";
        public ChiTietKetLuanCDHATDCNJSON ChiTietKetQuaObj { get; set; }

        public RangeDates RangeFromDate { get; set; }
        public string Nhom { get; set; }
        public long? NhomDichVuBenhVienId { get; set; }

    }

    public class BaoCaoHoatDongCLSVo : GridItem
    {
        public BaoCaoHoatDongCLSVo()
        {
            BaoCaoHoatDongCLSChiTietVos = new List<BaoCaoHoatDongCLSChiTietVo>();
        }
        public long? KhoaId { get; set; }
        public long? BacSiKetLuanId { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Nhom { get; set; }
        public long? NhomDichVuBenhVienId { get; set; }
        public int? SoLan { get; set; }
        public RangeDates RangeFromDate { get; set; }
        public List<BaoCaoHoatDongCLSChiTietVo> BaoCaoHoatDongCLSChiTietVos { get; set; }
    }

    public class BaoCaoHoatDongCLSChiTietVo : GridItem
    {
        public string Nhom { get; set; }
        public long? KhoaId { get; set; }
        public string Ten { get; set; }
        public int? SoLan { get; set; }
        public decimal DonGiaNiemYet { get; set; }
        public decimal ThanhTienNiemYet => DonGiaNiemYet * Convert.ToDecimal(SoLan);
        public decimal? TongThanhTienNiemYet { get; set; }

        public decimal? SoTienMienGiam { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }
        public decimal? DonGiaSauChietKhau { get; set; }
        public decimal? ThanhTienThucThu => YeuCauGoiDichVuId == null ? ((DonGiaNiemYet * SoLan.GetValueOrDefault()) - SoTienMienGiam.GetValueOrDefault()) : ((DonGiaSauChietKhau.GetValueOrDefault() * SoLan.GetValueOrDefault()) - SoTienMienGiam.GetValueOrDefault());
        public decimal? TongThanhTienThucThu { get; set; }

        public DateTime? NgayThucHien { get; set; }
    }

    public class BaoCaoSoThongKeCLSVo : GridItem
    {
        public BaoCaoSoThongKeCLSVo()
        {
            BaoCaoSoThongKeCLSChiTietVos = new List<BaoCaoSoThongKeCLSChiTietVo>();
        }
        public long? KhoaId { get; set; }
        public string Nhom { get; set; }
        public long? NhomDichVuBenhVienId { get; set; }
        public RangeDates RangeFromDate { get; set; }
        public List<BaoCaoSoThongKeCLSChiTietVo> BaoCaoSoThongKeCLSChiTietVos { get; set; }
    }

    public class BaoCaoSoThongKeCLSChiTietVo : GridItem
    {

        public BaoCaoSoThongKeCLSChiTietVo()
        {
            Take = 50;
            Sort = new List<Sort>();
            //KetLuans = new List<string>();
            ChiTietKetQuaObj = new ChiTietKetLuanCDHATDCNJSON();
        }
        public EnumTrangThaiYeuCauDichVuKyThuat? TrangThai { get; set; }
        public int? TrangThaiNumber => TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien ? 1 : 2;
        public bool? ChuaThucHien { get; set; }
        public bool? DaThucHien { get; set; }
        public RangeDates RangeFromDate { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public long? KhoaId { get; set; }
        public string TenKhoa { get; set; }
        public string Nhom { get; set; }
        public long? NhomDichVuBenhVienId { get; set; }
        public string MaTN { get; set; }
        public string SoBA { get; set; }
        public string HoTen { get; set; }
        public int? NamSinh { get; set; }
        public LoaiGioiTinh? GioiTinh { get; set; }
        public string GioiTinhDisplay => GioiTinh?.GetDescription();
        public string BHYTMaSoThe { get; set; }
        public string TenDichVu { get; set; }
        public string ChanDoan { get; set; }
        
        //public List<string> KetLuans { get; set; }

        //public string KetLuan
        //{
        //    get
        //    {
        //        var result = string.Empty;
        //        if (KetLuans != null && KetLuans.Count > 0)
        //        {
        //            foreach (var ketluan in KetLuans)
        //            {
        //                result += MaskHelper.RemoveHtmlFromString(ketluan) + "; ";
        //            }
        //        }
        //        return result;
        //    }
        //}
        public string KetLuan => ChiTietKetQuaObj != null && !string.IsNullOrEmpty(ChiTietKetQuaObj.KetLuan) ? MaskHelper.RemoveHtmlFromString(ChiTietKetQuaObj.KetLuan) : "";
        public ChiTietKetLuanCDHATDCNJSON ChiTietKetQuaObj { get; set; }
        public string DataKetQuaCanLamSang { get; set; }
        public long? NoiTruPhieuDieuTriId { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public Enums.EnumLoaiYeuCauTiepNhan LoaiYeuCauTiepNhan { get; set; }
        public int? SoLan { get; set; }
        public decimal? Gia { get; set; }
        public string TenNoiChiDinh { get; set; }
        public string TenNguoiChiDinh { get; set; }
        public string TenNguoiKetLuan { get; set; }
        public string KTV { get; set; }
        public long? NhanVienChiDinhId { get; set; }
        public long? NhanVienThucHienId { get; set; }
        public long? NhanVienKetLuanId { get; set; }
        public DateTime? ThoiDiemChiDinh { get; set; }
        public string ThoiDiemChiDinhDisplay => ThoiDiemChiDinh?.ApplyFormatDateTime();
        public DateTime? ThoiDiemDangKy { get; set; }
        public string ThoiDiemDangKyDisplay => ThoiDiemDangKy?.ApplyFormatDateTime();
        public DateTime? ThoiDiemThucHien { get; set; }
        public string ThoiDiemThucHienDisplay => ThoiDiemThucHien?.ApplyFormatDateTime();
        public DateTime? ThoiDiemNhapVien { get; set; }
        public string ThoiDiemNhapVienDisplay => ThoiDiemNhapVien?.ApplyFormatDateTime();
        public DateTime? ThoiDiemRaVien { get; set; }
        public string ThoiDiemRaVienDisplay => ThoiDiemRaVien?.ApplyFormatDateTime();
        public string GhiChu { get; set; }


        public string AdditionalSearchString { get; set; }

        public int Take { get; set; }
        public int Skip { get; set; }
        public int QueryId { get; set; }
        public List<Sort> Sort { get; set; }
        public bool? LazyLoadPage { get; set; }
        public string SortStringFormat { get; set; }
        public string SortString
        {
            get
            {
                if (!string.IsNullOrEmpty(SortStringFormat))
                {
                    return SortStringFormat;
                }
                // order the results
                if (Sort != null && Sort.Count > 0)
                {
                    var sorts = new List<string>();
                    Sort.ForEach(x => sorts.Add(string.Format("{0} {1}", x.Field, x.Dir)));
                    return string.Join(",", sorts.ToArray());
                }
                return string.Empty;
            }
        }
    }

    public class ChiTietKetLuanCDHATDCNJSON
    {
        public string KetLuan { get; set; }
    }

}
