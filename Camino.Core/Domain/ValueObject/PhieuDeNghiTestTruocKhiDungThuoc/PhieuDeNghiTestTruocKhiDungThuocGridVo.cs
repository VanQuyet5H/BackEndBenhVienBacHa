using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.PhieuDeNghiTestTruocKhiDungThuoc
{
    public class PhieuDeNghiTestTruocKhiDungThuocGridVo : GridItem
    {
        public long YeuCauTiepNhanId { get; set; }
        public DateTime ThoiDiemThucHien { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
        public string ThongTinHoSo { get; set; }
        public long NhanVienThucHienId { get; set; }
        public long NoiThucHienId { get; set; }
        public List<FileChuKyPhieuDeNghiTestTruocKhiDungThuocGridVo> ListFile { get; set; }
    }
    public class FileChuKyPhieuDeNghiTestTruocKhiDungThuocGridVo : GridItem
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenGuid { get; set; }
        public long KichThuoc { get; set; }
        public string DuongDan { get; set; }
        public Enums.LoaiTapTin LoaiTapTin { get; set; }
        public string MoTa { get; set; }
    }
    public class ValiDatorPhieuDeNghiTestTruocKhiDung : GridItem
    {
        public string HoTen { get; set; }
        public int? NamSinh { get; set; }
        public bool? GioiTinh{ get; set; }
        public string DiaChi { get; set; }
    }
    public class InPhieuDeNghiTestTruocKhiDungThuoc 
    {
        public long NoiTruHoSoKhacId { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
        public string Hosting { get; set; }
        public int LoaiPhieuIn { get; set; }
    }
    public class InPhieuDeNghiTestTruocKhiDungThuocobject
    {
        public string HoTen { get; set; }
        public int? NamSinh { get; set; }
        public bool? GioiTinh { get; set; }
        public bool? SelectBenhNhanHoacNguoiNha { get; set; }
        public bool DongYDeNghiTest { get; set; }
        public string DanToc { get; set; }
        public string NgoaiKieu { get; set; }
        public string NgheNghiep { get; set; }
        public string NoiLamViec { get; set; }
        public string DiaChi { get; set; }
        public string ChanDoan { get; set; }
        public List<data> DanhSachThuocCanTestArr { get; set; }
    }
    public class data {
        public DateTime? NgayThu { get; set; }
        public string NgayThuUTC { get; set; }
        public string Thuoc { get; set; }
        public string PhuongPhapThu{ get; set; }
        public string BacSiChiDinh{ get; set; }
        public string NguoiThu { get; set; }
        public string BSDocVaKiemTra { get; set; }
        public string NgayDocKQ { get; set; }
        public string NgayDocKQUTC { get; set; }
        public string SoLo { get; set; }
    }
    public class GetThongTinDuocPham
    {
        public string Ten { get; set; }
        public string HoatChat { get; set; }
        public string HamLuong { get; set; }
        public double SoLuong { get; set; }
        public long? DuocPhamBenhVienPhanNhomId { get; set; }
        public Enums.LoaiThuocTheoQuanLy? LoaiThuocTheoQuanLy { get; set; }
    }
}
