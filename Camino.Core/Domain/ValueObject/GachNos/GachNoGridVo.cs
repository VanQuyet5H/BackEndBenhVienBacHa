using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.GachNos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Newtonsoft.Json;

namespace Camino.Core.Domain.ValueObject.GachNos
{
    public class GachNoGridVo : GridItem
    {
        public string SoChungTu { get; set; }
        public Enums.LoaiDoiTuong LoaiDoiTuong { get; set; }
        public string TenLoaiDoiTuong => LoaiDoiTuong.GetDescription();
        public string LoaiThuChi { get; set; }
        public DateTime NgayChungTu { get; set; }
        public string NgayChungTuDisplay { get; set; }
        public string TaiKhoan { get; set; }
        public string DienGiai { get; set; }
        public int? VAT { get; set; }
        public string KhoanMucPhi { get; set; }
        public decimal TienHachToan { get; set; }
        public decimal TienThueHachToan { get; set; }
        public decimal TongTienHachToan { get; set; }
        public string SoHoaDon { get; set; }
        public DateTime? NgayHoaDon { get; set; }
        public string NgayHoaDonDisplay { get; set; }
        public string MaKhachHang { get; set; }
        public string TenKhachHang { get; set; }
        public string MaSoThue { get; set; }
        public Enums.TrangThaiGachNo TrangThai { get; set; }
        public string TenTrangThai => TrangThai.GetDescription();
        public Enums.LoaiTienTe LoaiTienTe { get; set; }
        public string NguoiXacNhanNhapLieu { get; set; }
        public DateTime? NgayXacNhanNhapLieu { get; set; }
        public string NgayXacNhanNhapLieuDisplay => NgayXacNhanNhapLieu?.ApplyFormatDateTimeSACH();
    }

    public class GachNoTimKiemNangCapVo
    {
        public string LoaiThuChi { get; set; }
        public GachNoTuNgayDenNgayVo TuNgayDenNgayCT { get; set; }
        public GachNoTuNgayDenNgayVo TuNgayDenNgayHD { get; set; }

    }

    public class GachNoTuNgayDenNgayVo
    {
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public string TuNgay { get; set; }
        public string DenNgay { get; set; }
    }

    public class LookupLoaiTienTeItemVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public decimal TyGia { get; set; }
    }

    public class LookupCongTyBHTNItemVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string Ten { get; set; }
        public string MaSoThue { get; set; }
        public string DonVi { get; set; }
        public string DiaChi { get; set; }
    }
    public class LookupBenhNhanItemVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string HoTen { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string NgayThangNamSinh
        {
            get
            {
                if (NgaySinh != null || ThangSinh != null || NamSinh != null)
                {
                    if (NgaySinh == null || ThangSinh == null)
                    {
                        return $"{NamSinh ?? 0}";
                    }
                    return $"{NgaySinh ?? 0}/{ThangSinh ?? 0}/{NamSinh ?? 0}";
                }
                else
                {
                    return string.Empty;
                }
            }
        }//=> string.Format("{0}/{1}/{2}", NgaySinh ?? 0, ThangSinh ?? 0, NamSinh ?? 0);
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string TenGioiTinh => GioiTinh?.GetDescription();
        public string SoChungMinhThu { get; set; }
        public string SoDienThoaiDisplay { get; set; }
        public string DiaChiDayDu { get; set; }
    }

    public class GachNoCongTyBHTNGridVo : GridItem
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string DiaChi { get; set; }
        public string DienThoai { get; set; }
        public string MaSoThue { get; set; }
        public string DonVi { get; set; }
    }

    public class GachNoBenhNhanGridVo: GridItem
    {
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string NgayThangNamSinh
        {
            get
            {
                if (NgaySinh != null || ThangSinh != null || NamSinh != null)
                {
                    if (NgaySinh == null || ThangSinh == null)
                    {
                        return $"{NamSinh ?? 0}";
                    }
                    return $"{NgaySinh ?? 0}/{ThangSinh ?? 0}/{NamSinh ?? 0}";
                }
                else
                {
                    return string.Empty;
                }
            }
        }//=> string.Format("{0}/{1}/{2}", NgaySinh ?? 0, ThangSinh ?? 0, NamSinh ?? 0);
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string TenGioiTinh => GioiTinh?.GetDescription();
        public string SoChungMinhThu { get; set; }
        public string SoDienThoai { get; set; }
        public string SoDienThoaiDisplay { get; set; }
        public string DiaChiDayDu { get; set; }
    }

    public class GachNoHistoryVo : GridItem
    {
        public DateTime ThoiGianThucHien { get; set; }
        public string ThoiGianThucHienDisplay => ThoiGianThucHien.ApplyFormatDateTimeSACH();
        public long NhanVienThucHienId { get; set; }
        public string TenNhanVienThucHien { get; set; }
        public Enums.EnumAudit Action { get; set; }
        public string strOldValue { get; set; }
        public string strNewValue { get; set; }
        public GachNoHistoryItemVo OldValue { get; set; }// => !string.IsNullOrEmpty(strOldValue) ? JsonConvert.DeserializeObject<GachNoHistoryItemVo>(strOldValue) : null;
        public GachNoHistoryItemVo NewValue { get; set; }//=> !string.IsNullOrEmpty(strNewValue) ? JsonConvert.DeserializeObject<GachNoHistoryItemVo>(strNewValue) : null;
    }

    public class GachNoHistoryItemVo : GridItem
    {
        public string SoChungTu { get; set; }
        public DateTime? NgayChungTu { get; set; }
        public Enums.LoaiChungTu? LoaiChungTu { get; set; }
        public string TenLoaiChungTu => LoaiChungTu.GetDescription();
        public string NgayChungTuDisplay => NgayChungTu != null ? NgayChungTu.Value.ApplyFormatDate() : null;
        public string KyKeToan { get; set; }
        public Enums.TrangThaiGachNo? TrangThai { get; set; }
        public string TenTrangThai => TrangThai.GetDescription();
        public Enums.LoaiTienTe? LoaiTienTe { get; set; }
        public string TenLoaiTienTe => LoaiTienTe.GetDescription();
        public Enums.LoaiTienTe? LoaiTienTeDisplay { get; set; }
        public string TienTeDisplay => LoaiTienTeDisplay.GetDescription();
        public decimal? TyGia { get; set; }
        public DateTime? NgayThucThu { get; set; }
        public string NgayThucThuDisplay => NgayThucThu != null ? NgayThucThu.Value.ApplyFormatDate() : null;
        public Enums.LoaiDoiTuong? LoaiDoiTuong { get; set; }
        public string TenLoaiDoiTuong => LoaiDoiTuong.GetDescription();

        public long? CongTyBaoHiemTuNhanId { get; set; }
        public string MaDoiTuong { get; set; }
        public long? BenhNhanId { get; set; }

        public string TaiKhoan { get; set; }
        //public string TaiKhoanLoaiTien { get; set; }
        public string NguoiNop { get; set; }
        public string ChungTuGoc { get; set; }
        public string DienGiaiChung { get; set; }
        public string SoTaiKhoanNganHang { get; set; }
        public string NguyenTe { get; set; }
        public string ThueNguyenTe { get; set; }
        public string TongNguyenTe { get; set; }
        public string HachToan { get; set; }
        public string ThueHachToan { get; set; }
        public string TongHachToan { get; set; }
        public string LoaiThuChi { get; set; }
        public int? VAT { get; set; }
        public decimal? TienHachToan { get; set; }
        public string KhoanMucPhi { get; set; }
        public string SoHoaDon { get; set; }
        public DateTime? NgayHoaDon { get; set; }
        public string NgayHoaDonDisplay => NgayHoaDon != null ? NgayHoaDon.Value.ApplyFormatDate() : null;
        public decimal? TienThueHachToan { get; set; }
        public decimal? TongTienHachToan { get; set; }
    }

    public class BaoCaoGachNoCongTyBhtnQueryInfo
    {
        public long? CongTyId { get; set; }
        public GachNoTuNgayDenNgayVo TuNgayDenNgayCT { get; set; }
        public GachNoTuNgayDenNgayVo TuNgayDenNgayHD { get; set; }
        public GachNoTuNgayDenNgayVo TuNgayDenNgay { get; set; }
        public string SearchString { get; set; }
        public GachNoTimKiemTrangThai TrangThai { get; set; }
    }

    public class BaoCaoGachNoCongTyBhtnGridVo : GridItem
    {
        public long CongTyId { get; set; }
        public string TenCongTy { get; set; }
        public string TaiKhoan { get; set; }
        public string MaTiepNhan { get; set; }
        public string SoChungTu { get; set; }
        public DateTime? NgayChungTu { get; set; }
        public string NgayChungTuDisplay => NgayChungTu?.ApplyFormatDate();
        public string SoHoaDon { get; set; }
        public DateTime? NgayHoaDon { get; set; }
        public string NgayHoaDonDisplay => NgayHoaDon?.ApplyFormatDate();
        public string DienGiai { get; set; }
        public string MaTienTe { get; set; }
        public decimal PhatSinhNo { get; set; }
        public decimal PhatSinhCo { get; set; }
        public decimal DauKyNo { get; set; }
        public decimal DauKyCo { get; set; }
        public decimal CuoiKyNo { get; set; }
        public decimal CuoiKyCo { get; set; }

        public DateTime NgayThuTien { get; set; }
        public long? YeuCauTiepNhanId { get; set; }
        public bool? SuDungGoi { get; set; }
    }

    public class BaoCaoGachNoCongTyBhtnGridDatasourceVo
    {
        public BaoCaoChiTietGachNoCongTyBhtnGrid DataSource { get; set; }
        public decimal TotalPhatSinhNo { get; set; }
        public decimal TotalPhatSinhCo { get; set; }
        public decimal TotalCuoiKyNo { get; set; }
    }

    public class BaoCaoChiTietGachNoCongTyBhtnGrid
    {
        public BaoCaoChiTietGachNoCongTyBhtnGrid()
        {
            Data = new List<BaoCaoGachNoCongTyBhtnGridVo>();
        }
        public ICollection<BaoCaoGachNoCongTyBhtnGridVo> Data { get; set; }

        public int TotalRowCount { get; set; }
    }

    public class GachNoTimKiemTrangThai
    {
        public bool DungGoi { get; set; }
        public bool KhongDungGoi { get; set; }
    }
}
