using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.NoiGioiThieu
{
    public class NoiGioiThieuFileImportVo
    {
        public string TenGuid { get; set; }
        public string DuongDan { get; set; }
        public Stream Path { get; set; }
        public long? NoiGioiThieuId { get; set; }
    }

    public class NoiGioiThieuDataImportVo
    {
        public NoiGioiThieuDataImportVo()
        {
            DuLieuDungs = new List<ThongTinDichVuMienGiamTuFileExcelVo>();
            DuLieuSais = new List<ThongTinDichVuMienGiamTuFileExcelVo>();
        }
        public List<ThongTinDichVuMienGiamTuFileExcelVo> DuLieuDungs { get; set; }
        public List<ThongTinDichVuMienGiamTuFileExcelVo> DuLieuSais { get; set; }
    }

    public class ThongTinDichVuMienGiamTuFileExcelVo
    {
        public ThongTinDichVuMienGiamTuFileExcelVo()
        {
            ValidationErrors = new List<ValidationErrorDichVuKhuyenMaiVo>();
        }
        public Enums.EnumNhomGoiDichVu NhomDichVu { get; set; }
        public string TenNhom => NhomDichVu.GetDescription();
        public string MaDichVuBenhVien { get; set; }
        public string TenDichVuBenhVien { get; set; }
        public long? DichVuBenhVienId { get; set; }
        public string LoaiGia { get; set; }
        public long? LoaiGiaId { get; set; }
        public decimal? GiaBenhVien { get; set; }
        public string TiLeChietKhau { get; set; }
        public int? TiLeChietKhauValue { get; set; }
        public string SoTienChietKhau { get; set; }
        public decimal? SoTienChietKhauValue { get; set; }
        public Enums.LoaiChietKhau? LoaiChietKhau { get; set; }
        public string GhiChu { get; set; }
        public List<ValidationErrorDichVuKhuyenMaiVo> ValidationErrors { get; set; }
    }

    public class ValidationErrorDichVuKhuyenMaiVo
    {
        public string Field { get; set; }
        public string Message { get; set; }
    }

    public class ThongTinDichVuBenhVienVo
    {
        public ThongTinDichVuBenhVienVo()
        {
            ThongTinGias = new List<ThongTinGiaBenhVienVo>();
        }
        public Enums.EnumNhomGoiDichVu NhomDichVu { get; set; }
        public long DichVuBenhVienId { get; set; }
        public string MaDichVu { get; set; }
        public string TenDichVu { get; set; }
        public List<ThongTinGiaBenhVienVo> ThongTinGias { get; set; }

    }

    public class ThongTinGiaBenhVienVo
    {
        public long DichVuBenhVienId { get; set; }
        public long NhomGiaId { get; set; }
        public string TenLoaiGia { get; set; }
        public decimal Gia { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
    }

    public class LookupDichVuBenhVienVo
    {
        public Enums.EnumNhomGoiDichVu NhomDichVu { get; set; }
        public string MaDichVu { get; set; }
        public string TenDichVu { get; set; }
    }

    public class NoiGioiThieuChiTietMienGiamVo
    {
        public long? DichVuKhamBenhBenhVienId { get; set; }
        public long? DichVuKyThuatBenhVienId { get; set; }
        public long? DichVuGiuongBenhVienId { get; set; }
        public long? NhomGiaDichVuKhamBenhBenhVienId { get; set; }
        public long? NhomGiaDichVuKyThuatBenhVienId { get; set; }
        public long? NhomGiaDichVuGiuongBenhVienId { get; set; }
        public long? DuocPhamBenhVienId { get; set; }
        public long? VatTuBenhVienId { get; set; }

        public Enums.EnumNhomGoiDichVu? NhomDichVu
        {
            get
            {
                if (DichVuKhamBenhBenhVienId != null)
                {
                    return Enums.EnumNhomGoiDichVu.DichVuKhamBenh;
                }
                else if (DichVuKyThuatBenhVienId != null)
                {
                    return Enums.EnumNhomGoiDichVu.DichVuKyThuat;
                }
                else if (DichVuGiuongBenhVienId != null)
                {
                    return Enums.EnumNhomGoiDichVu.DichVuGiuongBenh;
                }
                else if(DuocPhamBenhVienId != null)
                {
                    return Enums.EnumNhomGoiDichVu.DuocPham;
                }
                else if (VatTuBenhVienId != null)
                {
                    return Enums.EnumNhomGoiDichVu.VatTuTieuHao;
                }
                else
                {
                    return null;
                }
            }
        }
        public long? DichVuBenhVienId => DichVuKhamBenhBenhVienId ?? DichVuKyThuatBenhVienId ?? DichVuGiuongBenhVienId ?? DuocPhamBenhVienId ?? VatTuBenhVienId ?? (long?)null;
    }

    public class DichVuCanKiemTraVo
    {
        public DichVuCanKiemTraVo()
        {
            datas = new List<ThongTinDichVuMienGiamTuFileExcelVo>();
        }
        public long? NoiGioiThieuId { get; set; }
        public List<ThongTinDichVuMienGiamTuFileExcelVo> datas { get; set; }
    }
}
