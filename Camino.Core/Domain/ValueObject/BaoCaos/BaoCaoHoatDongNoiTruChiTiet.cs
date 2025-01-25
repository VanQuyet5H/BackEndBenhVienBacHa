using System;
using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.BaoCaos
{
    public class BaoCaoHoatDongNoiTruChiTietQueryInfoVo 
    {
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
    }

    public class BaoCaoHoatDongNoiTruChiTietVo 
    {
        public BaoCaoHoatDongNoiTruChiTietVo()
        {
            BaoCaoHoatDongNoiTruChiTietColumnHeader = new List<BaoCaoHoatDongNoiTruChiTietColumnHeaderVo>();
            BaoCaoHoatDongNoiTruChiTiets = new List<BaoCaoHoatDongNoiTruChiTiet>();
        }     
        public List<BaoCaoHoatDongNoiTruChiTietColumnHeaderVo> BaoCaoHoatDongNoiTruChiTietColumnHeader { get; set; }
        public List<BaoCaoHoatDongNoiTruChiTiet> BaoCaoHoatDongNoiTruChiTiets { get; set; }
    }
   
    public class BaoCaoHoatDongNoiTruChiTiet
    {
        public BaoCaoHoatDongNoiTruChiTiet()
        {           
            BaoCaoHoatDongNoiTruChiTietColumns = new List<BaoCaoHoatDongNoiTruChiTietColumn>();
        }

        public string STT { get; set; }
        public string ChiTieu { get; set; }
        public decimal? Tong { get; set; }
        public bool? CanhGiuaToDam { get; set; }
        public bool IsPerCent { get; set; }
        public List<BaoCaoHoatDongNoiTruChiTietColumn> BaoCaoHoatDongNoiTruChiTietColumns { get; set; }
    }


    public class BaoCaoHoatDongNoiTruChiTietColumnHeaderVo
    {
        public int Index { get; set; }
        public string CellText { get; set; }
    }

    public class BaoCaoHoatDongNoiTruChiTietColumn
    {
        public int Index { get; set; }
        public decimal? CellValue { get; set; }
    }

    public class DataHoatDongNoiTruChiTiet
    {
        public long Id { get; set; }
        public long KhoaPhongNhapVienId { get; set; }
        public long KhoaCuoiCungDieuTriId { get; set; }
        public DateTime ThoiDiemNhapVien { get; set; }
        public DateTime? ThoiDiemRaVien { get; set; }
        public Enums.LoaiBenhAn LoaiBenhAn { get; set; }
        public bool? CoBHYT { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public long? TinhThanhId { get; set; }
        public Enums.EnumHinhThucRaVien? HinhThucRaVien { get; set; }
        public List<DataHoatDongNoiTruChiTietKhoaPhongDieuTri> KhoaPhongDieuTris { get; set; }
        public bool CoChuyenKhoa { get; set; }
        public bool LaCapCuu { get; set; }
        public List<DataHoatDongNoiTruChiTietSuDungGiuong> SuDungGiuongs { get; set; }
        public decimal SoNgayDieuTri { get; set; }
        public decimal SoNgayDieuTriRaVien { get; set; }
        public decimal SoNgayDieuTriRaVienBHYT { get; set; }
        public bool LaBenhAnCon { get; set; }
        public bool TreEm6Den15Tuoi
        {
            get
            {
                if (NamSinh != null)
                {
                    var birthdate = new DateTime(NamSinh.Value, ThangSinh == null || ThangSinh == 0 ? 1 : ThangSinh.Value, NgaySinh == null || NgaySinh == 0 ? 1 : NgaySinh.Value);
                    var age = ThoiDiemNhapVien.Year - birthdate.Year;
                    if (birthdate.Date > ThoiDiemNhapVien.AddYears(-age)) age--;
                    return age >= 6 && age <= 15;
                }
                return false;
            }
        }
        public bool TreEmDuoi6Tuoi
        {
            get
            {
                if (NamSinh != null)
                {
                    var birthdate = new DateTime(NamSinh.Value, ThangSinh == null || ThangSinh == 0 ? 1 : ThangSinh.Value, NgaySinh == null || NgaySinh == 0 ? 1 : NgaySinh.Value);
                    var age = ThoiDiemNhapVien.Year - birthdate.Year;
                    if (birthdate.Date > ThoiDiemNhapVien.AddYears(-age)) age--;
                    return age < 6;
                }
                return false;
            }
        }
    }

    public class DataHoatDongNoiTruChiTietKhoaPhongDieuTri
    {
        public long Id { get; set; }
        public long? KhoaPhongChuyenDiId { get; set; }
        public long KhoaPhongChuyenDenId { get; set; }
        public DateTime ThoiDiemVaoKhoa { get; set; }
        public DateTime? ThoiDiemRaKhoa { get; set; }
        public decimal SoNgayDieuTri { get; set; }
        public decimal SoNgayDieuTriRaVien { get; set; }
    }
    public class DataHoatDongNoiTruChiTietSuDungGiuong
    {
        public Enums.DoiTuongSuDung? DoiTuongSuDung { get; set; }
        public long? GiuongBenhId { get; set; }
        public DateTime? ThoiDiemBatDauSuDung { get; set; }
        public DateTime? ThoiDiemKetThucSuDung { get; set; }
        public Enums.EnumTrangThaiGiuongBenh TrangThai { get; set; }
    }
    public class DataHoatDongNoiTruChiTietSuDungGiuongTheoKhoa
    {
        public string KhoaPhongId { get; set; }
        public List<long> GiuongBenhIds { get; set; }
    }
}
