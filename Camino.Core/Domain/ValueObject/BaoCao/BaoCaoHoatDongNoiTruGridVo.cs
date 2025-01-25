using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.BaoCao
{
    public class BaoCaoHoatDongNoiTruGridVo : GridItem
    {
        public string Muc { get; set; }
        public decimal Tong { get; set; }
        public bool IsCenter { get; set; }
        public bool IsPerCent { get; set; }
    }

    public class DataHoatDongNoiTru
    {
        public long Id { get; set; }
        public DateTime ThoiDiemNhapVien { get; set; }
        public DateTime? ThoiDiemRaVien { get; set; }
        public Enums.LoaiBenhAn LoaiBenhAn { get; set; }
        public bool? CoBHYT { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public Enums.EnumHinhThucRaVien? HinhThucRaVien { get; set; }
        public List<DataHoatDongNoiTruKhoaPhongDieuTri> KhoaPhongDieuTris { get; set; }
        public bool CoChuyenKhoa { get; set; }
        public List<DataHoatDongNoiTruSuDungGiuong> SuDungGiuongs { get; set; }
        public decimal SoNgayDieuTri { get; set; }
        public decimal SoNgayDieuTriRaVien { get; set; }
        public decimal SoNgayDieuTriRaVienBHYT { get; set; }
        public bool LaBenhAnCon { get; set; }
        public List<DataHoatDongNoiTruDieuTriBenhAnSoSinh> DieuTriBenhAnSoSinhs { get; set; }
        public bool TreEmDuoi6Tuoi
        {
            get
            {
                if (NamSinh != null)
                {
                    var birthdate = new DateTime(NamSinh.Value, ThangSinh == null || ThangSinh == 0 ? 1 : ThangSinh.Value, NgaySinh == null || NgaySinh == 0 ? 1 : NgaySinh.Value);
                    var age = ThoiDiemNhapVien.Year - birthdate.Year;
                    if (birthdate.Date > ThoiDiemNhapVien.AddYears(-age)) age--;
                    return age<6;
                }
                return false;
            }
        }
    }

    public class DataHoatDongNoiTruKhoaPhongDieuTri
    {
        public long Id { get; set; }
        public long? KhoaPhongChuyenDiId { get; set; }
        public long KhoaPhongChuyenDenId { get; set; }
        public DateTime ThoiDiemVaoKhoa { get; set; }
    }
    public class DataHoatDongNoiTruSuDungGiuong 
    {
        public Enums.DoiTuongSuDung? DoiTuongSuDung { get; set; }
        public long? GiuongBenhId { get; set; }
        public DateTime? ThoiDiemBatDauSuDung { get; set; }
        public DateTime? ThoiDiemKetThucSuDung { get; set; }
        public Enums.EnumTrangThaiGiuongBenh TrangThai { get; set; }
    }
    public class DataHoatDongNoiTruDieuTriBenhAnSoSinh
    {
        public DateTime NgayDieuTri { get; set; }
        public int? GioBatDau { get; set; }
        public int? GioKetThuc { get; set; }

        public int SoGiayDieuTri
        {
            get
            {
                //if(GioBatDau != null && GioKetThuc !=null && GioKetThuc - GioBatDau > 0)
                //{
                //    return Math.Round((decimal)(GioKetThuc.Value - GioBatDau.Value) / (24 * 60 * 60), 1);
                //}
                if (GioBatDau != null && GioKetThuc != null && GioKetThuc - GioBatDau > 0)
                {
                    return GioKetThuc.Value - GioBatDau.Value;
                }
                return 0;
            }
        }
    }
}