using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;

namespace Camino.Core.Domain.ValueObject
{
    public class DichVuKhamBenhBenhVienVO : GridItem
    {
        public string Ma { get; set; }
        public string MaTT37 { get; set; }
        public string Ten { get; set; }
        public long KhoaId { get; set; }
        public string TenKhoa { get; set; }
        public string TenNoiThucHien { get; set; }
        public string HangBenhVien { get; set; }
        public string MoTa { get; set; }
        public long DichVuKhamBenhId { get; set; }
        public long Gia { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public bool HieuLuc { get; set; }
        public string HieuLucHienThi { get; set; }
    }
    public class DichVuKhamBenhBenhVienGiaBaoHiemVO : GridItem
    {
       
        public decimal Gia { get; set; }
        public int TiLeBaoHiemThanhToan { get; set; }
        public DateTime TuNgay { get; set; }
        public String GiaHienThi { get; set; }
        public DateTime? DenNgay { get; set; }
        public string TuNgayHienThi { get; set; }
        public string DenNgayHienThi { get; set; }
    }
    public class DichVuKhamBenhBenhVienGiaBenhVienVO : GridItem
    {

        public string LoaiGia { get; set; }
        public decimal Gia { get; set; }
        public String GiaHienThi { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string TuNgayHienThi { get; set; }
        public string DenNgayHienThi { get; set; }
    }

    public class GiaDichVuBenhVienFileImportVo
    {
        public string TenGuid { get; set; }
        public string DuongDan { get; set; }
        public Stream Path { get; set; }
    }

    public class GiaDichVuBenhVieDataImportVo
    {
        public GiaDichVuBenhVieDataImportVo()
        {
            DuLieuDungs = new List<ThongTinGiaDichVuTuFileExcelVo>();
            DuLieuSais = new List<ThongTinGiaDichVuTuFileExcelVo>();
        }
        public List<ThongTinGiaDichVuTuFileExcelVo> DuLieuDungs { get; set; }
        public List<ThongTinGiaDichVuTuFileExcelVo> DuLieuSais { get; set; }
    }

    public class ThongTinGiaDichVuTuFileExcelVo
    {
        public ThongTinGiaDichVuTuFileExcelVo()
        {
            ValidationErrors = new List<ValidationErrorGiaDichVuVo>();
        }
        public string MaDichVuBenhVien { get; set; }
        public string TenDichVuBenhVien { get; set; }
        public long? DichVuBenhVienId { get; set; }
        public string LoaiGia { get; set; }
        public long? LoaiGiaId { get; set; }
        public string GiaBaoHiem { get; set; }
        public decimal? GiaBaoHiemValue { get; set; }
        public string TiLeBaoHiemThanhToan { get; set; }
        public int? TiLeBaoHiemThanhToanValue { get; set; }
        public string GiaBenhVien { get; set; }
        public decimal? GiaBenhVienValue { get; set; }
        public string TuNgay { get; set; }
        public DateTime? TuNgayValue { get; set; }
        public string DenNgay { get; set; }
        public DateTime? DenNgayValue { get; set; }
        public bool LaCapNhatDenNgayTruocDo { get; set; }
        public List<ValidationErrorGiaDichVuVo> ValidationErrors { get; set; }
    }

    public class ValidationErrorGiaDichVuVo
    {
        public string Field { get; set; }
        public string Message { get; set; }
    }

    public class LookupDichVuBenhVienVo
    {
        public string MaDichVu { get; set; }
        public string TenDichVu { get; set; }
    }

    public class ThongTinDichVuBenhVienVo
    {
        public ThongTinDichVuBenhVienVo()
        {
            ThongTinGiaBenhViens = new List<ThongTinGiaBenhVienVo>();
            ThongTinGiaBaoHiems = new List<ThongTinGiaBaoHiemVo>();
        }
        public Enums.EnumNhomGoiDichVu NhomDichVu { get; set; }
        public long DichVuBenhVienId { get; set; }
        public string MaDichVu { get; set; }
        public string TenDichVu { get; set; }
        public List<ThongTinGiaBenhVienVo> ThongTinGiaBenhViens { get; set; }
        public List<ThongTinGiaBaoHiemVo> ThongTinGiaBaoHiems { get; set; }
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

    public class ThongTinGiaBaoHiemVo
    {
        public long DichVuBenhVienId { get; set; }
        public decimal Gia { get; set; }
        public int TiLeBaoHiemThanhToan { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
    }

    public class GiaDichVuCanKiemTraVo
    {
        public GiaDichVuCanKiemTraVo()
        {
            datas = new List<ThongTinGiaDichVuTuFileExcelVo>();
        }
        public List<ThongTinGiaDichVuTuFileExcelVo> datas { get; set; }
    }

    public class DichVuKhamBenhBenhVienJSON
    {
        public long DichVuKhamBenhBenhVienId { get; set; }
    }
}
