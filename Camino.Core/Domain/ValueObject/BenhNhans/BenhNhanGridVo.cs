using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.BenhNhans
{
    public class BenhNhanGridVo : GridItem
    {
        public string HoTen { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string SoChungMinhThu { get; set; }
        public string GioiTinh { get; set; }
        public string DiaChi { get; set; }
        public string Email { get; set; }
    }

    public class ThemBHTN
    {
        public long BenhNhanId { get; set; }
        public long? CongTyBaoHiemTuNhanId { get; set; }
        public string MaSoThe { get; set; }
        public DateTime? NgayHieuLuc { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }
    }

    public class ThemBHTNGridVo
    {
        public long BenhNhanId { get; set; }
        public long? CongTyBaoHiemTuNhanId { get; set; }
        public string CongTyDisplay { get; set; }
        public string MaSoThe { get; set; }
        public DateTime? NgayHieuLuc { get; set; }
        public string NgayHieuLucDisplay { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public string NgayHetHanDisplay { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }
    }


    public class ThemTienSuBenh
    {
        public long? BenhNhanTienSuBenhId { get; set; }
        public long BenhNhanId { get; set; }
        public string TenBenh { get; set; }
        //public string TinhTrangBenh { get; set; }
        //public DateTime? NgayPhatHien { get; set; }
        public Enums.EnumLoaiTienSuBenh? LoaiTienSuBenh { get; set; }
    }

    public class TienSuBenhGridVo
    {
        public long? BenhNhanTienSuBenhId { get; set; }
        public int STT { get; set; }
        public long BenhNhanId { get; set; }
        public string TenBenh { get; set; }
        //public string TinhTrangBenh { get; set; }
        //public DateTime? NgayPhatHien { get; set; }
        //public string NgayPhatHienDisplay { get; set; }
        public Enums.EnumLoaiTienSuBenh? LoaiTienSuBenh { get; set; }
        public string TenLoaiTienSuBenh { get; set; }

    }


    public class ThemDiUngThuoc
    {
        public long? BenhNhanDiUngId { get; set; }
        public long BenhNhanId { get; set; }
        public string TenDiUng { get; set; }
        public Enums.LoaiDiUng? LoaiDiUng { get; set; }
        public string BieuHienDiUng { get; set; }
        public long? ThuocId { get; set; }
        public string TenThuoc { get; set; }
        public Enums.EnumMucDoDiUng? MucDo { get; set; }

    }

    public class DiUngThuocGridVo
    {
        public long? BenhNhanDiUngId { get; set; }
        public int STT { get; set; }
        public long BenhNhanId { get; set; }
        public string TenDiUng { get; set; }
        public Enums.LoaiDiUng? LoaiDiUng { get; set; }
        public string LoaiDiUngDisplay { get; set; }

        public string BieuHienDiUng { get; set; }
        public long? ThuocId { get; set; }
        public string TenThuoc { get; set; }
        public Enums.EnumMucDoDiUng? MucDo { get; set; }
        public string MucDoDisplay { get; set; }

    }
    public class BenhNhanTienSuBenhChiTiet
    {
        public string TenBenh { get; set; }
        public Enums.EnumLoaiTienSuBenh? LoaiTienSuBenh { get; set; }
    }

    public class BenhNhanDiUngThuocChiTiet
    {
        public string TenDiUng { get; set; }
        public long? ThuocId { get; set; }
        public Enums.LoaiDiUng LoaiDiUng { get; set; }
    }
}
