using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.BaoCao
{
    public class BaoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo: GridItem
    {
        public int? STT { get; set; }
        public DateTime NgayKham { get; set; }
        public string NgayKhamDisplay => NgayKham.ApplyFormatDate();
        public long BenhNhanId { get; set; }
        public string MaTN { get; set; }
        public long? YeuCauTiepNhanId { get; set; }
        public string TenBN { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string NgaySinhDisplay => DateHelper.DOBFormat(NgaySinh, ThangSinh, NamSinh);
        public string DiaChi { get; set; }
        public string BSKham { get; set; }
        public string TenDV { get; set; }
        public string NhomDV { get; set; }
        public decimal SoTienDV { get; set; }
        public long? NhomDichVuConId { get; set; }
        public long YeuCauDichVuBenhVienId { get; set; }
        public Enums.EnumNhomGoiDichVu NhomDichVu { get; set; }

        public Enums.EnumNhomGoiDichVu NhomDichVuTruocGroup
        {
            get
            {
                if (YeuCauKhamBenhId != null)
                {
                    return Enums.EnumNhomGoiDichVu.DichVuKhamBenh;
                }
                else if (YeuCauDichVuKyThuatId != null)
                {
                    return Enums.EnumNhomGoiDichVu.DichVuKyThuat;
                }
                else if (YeuCauTruyenMauId != null)
                {
                    return Enums.EnumNhomGoiDichVu.TruyenMau;
                }
                else if (YeuCauDuocPhamBenhVienId != null)
                {
                    return Enums.EnumNhomGoiDichVu.DuocPham;
                }
                else if (YeuCauDichVuGiuongBenhVienId != null || YeuCauDichVuGiuongBenhVienChiPhiBenhVienId != null)
                {
                    return Enums.EnumNhomGoiDichVu.DichVuGiuongBenh;
                }
                else if (YeuCauVatTuBenhVienId != null)
                {
                    return Enums.EnumNhomGoiDichVu.VatTuTieuHao;
                }
                return Enums.EnumNhomGoiDichVu.DichVuKhamBenh;
            }
        }
        public long? YeuCauKhamBenhId { get; set; }
        public long? YeuCauDichVuKyThuatId { get; set; }
        public long? YeuCauDuocPhamBenhVienId { get; set; }
        public long? YeuCauVatTuBenhVienId { get; set; }
        public long? YeuCauDichVuGiuongBenhVienId { get; set; }
        public long? YeuCauDichVuGiuongBenhVienChiPhiBenhVienId { get; set; }
        public long? YeuCauTruyenMauId { get; set; }

        public string HinhThucDenDisplay { get; set; }
        public string MaNguoiBenh { get; set; }
        public string SoBienLaiThuTien { get; set; }
        public bool? TinhTrangThanhToan { get; set; }
        //public long DichVuBenhVienId { get; set; }

        //30/11/2021: cập nhật theo yêu cầu: lấy tất cả dv trừ đơn thuốc ra về
        public long DichVuBenhVienId => DichVuKhamBenhId ?? DichVuKyThuatId ?? DuocPhamBenhVienId ?? VatTuBenhVienId ?? DichVuGiuongBenhVienId ?? TruyenMauId ?? 0;
        public long? DichVuKhamBenhId { get; set; }
        public long? DichVuKyThuatId { get; set; }
        public long? DuocPhamBenhVienId { get; set; }
        public long? VatTuBenhVienId { get; set; }
        public long? DichVuGiuongBenhVienId { get; set; }
        public long? TruyenMauId { get; set; }

        public decimal DonGiaBenhVien { get; set; }
        public double? SoLuong { get; set; }
        public string HoaHong { get; set; }
        public decimal? ThanhTienHoaHong { get; set; }
    }

    public class ThongTinTiepNhanCoGioiThieuVo
    {
        public long YeucauTiepNhanId { get; set; }
        public long BenhNhanId { get; set; }
        public long? NoiGioiThieuId { get; set; }
        public DateTime? ThoiDiemTiepNhan { get; set; }

        public string NoiGioiThieuDisplay { get; set; }
        public string TenHinhThucDen { get; set; }
        public bool? LaGioiThieu { get; set; }
        public string HinhThucDenDisplay => LaGioiThieu == true
            ? (TenHinhThucDen + ((!string.IsNullOrEmpty(TenHinhThucDen) && !string.IsNullOrEmpty(NoiGioiThieuDisplay)) ? "/ " : "") + NoiGioiThieuDisplay)
            : TenHinhThucDen;

        public bool? LaDataTheoDieuKienTimKiem { get; set; }
    }

    public class ThongTinDichVuKhamBenhTinhHoaHong
    {
        public long NoiGioiThieuId { get; set; }
        public long DichVuKhamBenhBenhVienId { get; set; }
        public long NhomGiaDichVuKhamBenhBenhVienId { get; set; }
        public int Thang { get; set; }
        public double SoLuong { get; set; }
    }
    public class ThongTinDichVuKyThuatTinhHoaHong
    {
        public long NoiGioiThieuId { get; set; }
        public long DichVuKyThuatBenhVienId { get; set; }
        public long NhomGiaDichVuKyThuatBenhVienId { get; set; }
        public int Thang { get; set; }
        public double SoLuong { get; set; }
    }
    public class ThongTinDichVuGiuongTinhHoaHong
    {
        public long NoiGioiThieuId { get; set; }
        public long DichVuGiuongBenhVienId { get; set; }
        public long NhomGiaDichVuGiuongBenhVienId { get; set; }
        public int Thang { get; set; }
        public double SoLuong { get; set; }
    }
}
