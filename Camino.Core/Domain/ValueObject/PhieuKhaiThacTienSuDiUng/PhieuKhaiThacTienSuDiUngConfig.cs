using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.PhieuKhaiThacTienSuDiUng
{
    public class PhieuKhaiThacTienSuDiUngConfig
    {
        public string Value { get; set; }
    }
    public class PhieuKhaiThacTienSuDiUngGridVo : GridItem
    {
        public long YeuCauTiepNhanId { get; set; }
        public DateTime ThoiDiemThucHien { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
        public string ThongTinHoSo { get; set; }
        public long NhanVienThucHienId { get; set; }
        public long NoiThucHienId { get; set; }
        public List<FileChuKyPhieuKhaiThacTienSuDiUngGridVo> ListFile { get; set; }
    }
    public class FileChuKyPhieuKhaiThacTienSuDiUngGridVo : GridItem
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenGuid { get; set; }
        public long KichThuoc { get; set; }
        public string DuongDan { get; set; }
        public Enums.LoaiTapTin LoaiTapTin { get; set; }
        public string MoTa { get; set; }
    }
    public class XacNhanInTienSu
    {
        public long NoiTruHoSoKhacId { get; set; }
        public long? YeuCauTiepNhanId { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
        public string Hosting { get; set; }
    }
    public class PhieuKhaiThacTienSuDiUngVo : GridItem
    {
        public string BSKhaiThac { get; set; }
        public string HoTenNgBenh { get; set; }
        public string TuoiNgBenh { get; set; }
        public string GTNgBenh { get; set; }
        public string Khoa { get; set; }
        public string Buong { get; set; }
        public string Giuong { get; set; }
        public string ChanDoan { get; set; }
        public string PhieuKhaiThacTienSuDiUngList { get; set; }
    }
    public class PhieuKhaiThacTienSuDiUngLists
    {
        public PhieuKhaiThacTienSuDiUngLists()
        {
            PhieuKhaiThacTienSuDiUngList = new List<ThongTinPhieuKhaiThacTienSuDiUng>();
        }
        public List<ThongTinPhieuKhaiThacTienSuDiUng> PhieuKhaiThacTienSuDiUngList { get; set; }
    }
    public class ThongTinPhieuKhaiThacTienSuDiUng
    {
        public long? STT { get; set; }
        public string NoiDung { get; set; }
        public string TenThuoc { get; set; }
        public string DiNguyenGayDiUng { get; set; }
        public bool? CoKhong { get; set; }
        public string SoLan { get; set; }
        public string BieuHienLamSang { get; set; }
        public string XuTri { get; set; }
        public string Stt { get; set; }
        public string NgayThucHien { get; set; }
        public string TaiKhoanDangNhap { get; set; }
        public string BSKhaiThac { get; set; }
    }
    public class InfoPhieuInPhieuKhaiThacTienSuDiUng
    {
        public string Khoa { get; set; }
        public string MaNB { get; set; }
        public string BarCodeImgBase64 { get; set; }
        public string HoTen { get; set; }
        public string Tuoi { get; set; }
        public string NamNu { get; set; }
        public string KhoaPhongDangIn { get; set; }
        public string Buong { get; set; }
        public string Giuong { get; set; }
        public string ChanDoan { get; set; }
        public string BacSyKhaiThacTienSuDiUng { get; set; }
        public string NguoiBenh { get; set; }
        public string Table { get; set; }
    }
}
