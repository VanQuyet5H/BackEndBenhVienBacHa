using System;
using System.Collections.Generic;
using System.ComponentModel;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.BaoCaos
{
    public class TinhHinhBenhTatTuVongQueryInfoVo 
    {
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
    }

    public class DanhSachTinhHinhBenhTatTuVong : GridItem
    {
        public string STT { get; set; }
        public string TenNhomBenh { get; set; }
        public string MaICD { get; set; }
        public bool? LaChuong { get; set; }
        public long ChuongId { get; set; }

        //HDKB : Hoạt động khám bệnh
        public int? HDKBSoLanKhamChung { get; set; }
        public int? HDKBSoLanKhamTreEm { get; set; }
        public int? HDKBSoTuVong { get; set; }

        //TSBN : Tổng số bệnh nhân
        public int? TSBNSoMacBenh { get; set; }
        public int? TSBNSoTuVong { get; set; }
        public int? TSBNNgayDieuTri { get; set; }

        //TE : Trong đó TE<15 tuổi
        public int? TEMacTS { get; set; }
        public int? TEMacDuoi6T { get; set; }

        public int? TESoTuVongTS { get; set; }
        public int? TESoTuVongDuoi6T { get; set; }

        public int? TENgayDieuTriTS { get; set; }
        public int? TENgayDieuTriDuoi6T { get; set; }

        //Tren60T : trên 60 Tuổi
        public int? Tren60TMacBenhTS { get; set; }
        public int? Tren60TMacBenhLaNu { get; set; }

        public int? Tren60TTuVongTS { get; set; }
        public int? Tren60TTuVongLaNu { get; set; }
    }

    public class TinhHinhBenhTatTuVongKhamBenhData
    {
        public long Id { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public DateTime ThoiDiemTiepNhan { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public LoaiGioiTinh? GioiTinh { get; set; }
        public string MaICD { get; set; }
        public bool? CoTuVong { get; set; }
        public List<string> MaICDKhacs { get; set; }

        public bool TreEmDen15Tuoi
        {
            get
            {
                if (NamSinh != null)
                {
                    var birthdate = new DateTime(NamSinh.Value, ThangSinh == null || ThangSinh == 0 ? 1 : ThangSinh.Value, NgaySinh == null || NgaySinh == 0 ? 1 : NgaySinh.Value);
                    var age = ThoiDiemTiepNhan.Year - birthdate.Year;
                    if (birthdate.Date > ThoiDiemTiepNhan.AddYears(-age)) age--;
                    return age <= 15;
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
                    var age = ThoiDiemTiepNhan.Year - birthdate.Year;
                    if (birthdate.Date > ThoiDiemTiepNhan.AddYears(-age)) age--;
                    return age < 6;
                }
                return false;
            }
        }
        public bool NguoiHon60Tuoi
        {
            get
            {
                if (NamSinh != null)
                {
                    var birthdate = new DateTime(NamSinh.Value, ThangSinh == null || ThangSinh == 0 ? 1 : ThangSinh.Value, NgaySinh == null || NgaySinh == 0 ? 1 : NgaySinh.Value);
                    var age = ThoiDiemTiepNhan.Year - birthdate.Year;
                    if (birthdate.Date > ThoiDiemTiepNhan.AddYears(-age)) age--;
                    return age > 60;
                }
                return false;
            }
        }
    }

    public class TinhHinhBenhTatTuVongBenhAnData
    {
        public long Id { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public DateTime ThoiDiemNhapVien { get; set; }
        public DateTime ThoiDiemRaVien { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public LoaiGioiTinh? GioiTinh { get; set; }
        public string MaICD { get; set; }
        public EnumHinhThucRaVien? HinhThucRaVien { get; set; }

        public bool TreEmDen15Tuoi
        {
            get
            {
                if (NamSinh != null)
                {
                    var birthdate = new DateTime(NamSinh.Value, ThangSinh == null || ThangSinh == 0 ? 1 : ThangSinh.Value, NgaySinh == null || NgaySinh == 0 ? 1 : NgaySinh.Value);
                    var age = ThoiDiemNhapVien.Year - birthdate.Year;
                    if (birthdate.Date > ThoiDiemNhapVien.AddYears(-age)) age--;
                    return age <= 15;
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
        public bool NguoiHon60Tuoi
        {
            get
            {
                if (NamSinh != null)
                {
                    var birthdate = new DateTime(NamSinh.Value, ThangSinh == null || ThangSinh == 0 ? 1 : ThangSinh.Value, NgaySinh == null || NgaySinh == 0 ? 1 : NgaySinh.Value);
                    var age = ThoiDiemNhapVien.Year - birthdate.Year;
                    if (birthdate.Date > ThoiDiemNhapVien.AddYears(-age)) age--;
                    return age > 60;
                }
                return false;
            }
        }
    }
}
