using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.TrichBienBanHoiChan
{
    public class TrichBienBanHoiChanGridVo : GridItem
    {
        public long YeuCauTiepNhanId { get; set; }
        public DateTime ThoiDiemThucHien { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
        public string ThongTinHoSo { get; set; }
        public long NhanVienThucHienId { get; set; }
        public long NoiThucHienId { get; set; }
        public List<FileChuKyBangKiemAnToanGridVo> ListFile { get; set; }
    }
    public class NhanVienNgayThucHien
    {
        public string TenNhanVien { get; set; }
        public string NgayThucHien { get; set; }
        public DateTime ThoiDiemKham { get; set; }
        public DateTime? DaDieuTriTuNgay { get; set; }
        public DateTime? DaDieuTriDenNgay { get; set; }
        public string TaiSoGiuong { get; set; }
        public string Phong { get; set; }
        public string Khoa { get; set; }
        public string ChanDoan { get; set; }
        public string ChanDoanICDChinh { get; set; }
        public string ChanDoanICDPhu { get; set; }
        public string TienSuDiUng { get; set; }
    }
    public class XacNhanInTrichBienBanHoiChan
    {
        public long NoiTruHoSoKhacId { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
        public string Hosting { get; set; }
        public string TuNgay { get; set; }
        public string DenNgay { get; set; }

        //BVHD-3876
        public int? LoaiPhieuIn { get; set; } = 1;
    }
    public class InTrichBienBanHoiChan
    {
        public string Khoa { get; set; }
        public string HoTenBenhNhan { get; set; }
        public string TuNgay { get; set; }
        public string TuThang { get; set; }
        public string TuNam { get; set; }
        public string DenNgay { get; set; }
        public string DenThang { get; set; }
        public string DenNam { get; set; }
        public string TaiSoGiuong { get; set; }
        public string Phong { get; set; }
        public string ChanDoan { get; set; }
        public string Gio { get; set; }
        public string Phut { get; set; }
        public string Ngay { get; set; }
        public string Thang { get; set; }
        public string Nam { get; set; }
        public string ChuToa { get; set; }
        public string ThuKy { get; set; }
        public string ThanhVienThamGia { get; set; }
        public string TomTatQuaTrinhDienBienBenh { get; set; }
        public string KetLuan { get; set; }
        public string HuongDieuTriTiep { get; set; }
        public string NgayHienTai { get; set; }
        public string ThangHienTai { get; set; }
        public string NamHienTai { get; set; }
        public string Tuoi { get; set; }
        public string NamNu { get; set; }
    }
    public class InThongTinTrichBienBanHoiChan
    {
        public DateTime? HoiChanLuc { get; set; }
        public string HoiChanLucStringUTC { get; set; }
        public string ChuToa { get; set; }
        public string ThuKy { get; set; }
        public List<string> NguoiThamGia { get; set; }
        public string TomTat { get; set; }
        public string KetLuan { get; set; }
        public string ChanDoan { get; set; }
        public string HuongDieuTriTiep { get; set; }
        public string NgayThucHien { get; set; }
        public string TaiKhoanDangNhap { get; set; }
        public DateTime? DaDieuTriTuNgay { get; set; }
        public DateTime? DaDieuTriDenNgay { get; set; }
        public string TaiSoGiuong { get; set; }
        public string Phong { get; set; }
        public string Khoa { get; set; }
    }
    public class DanhSachTrichBienBanHoiChanyGridVo : GridItem
    {
        public DateTime? NgayHoiChan { get; set; }
        public string HoiChanLucStringUTC { get; set; }
        public DateTime? HoiChanLuc { get; set; }
        public string NgayHoiChanString => NgayHoiChan != null ? NgayHoiChan.GetValueOrDefault().ApplyFormatDate():"";
        public string ChuToa { get; set; }
        public string ThuKy { get; set; }
        public string ThanhVienThamGia { get; set; }
        public List<string> NguoiThamGia { get; set; }
        public string ChanDoan { get; set; }
        public string ThongTinHoSo { get; set; }
    }
    public class FileChuKyBangKiemAnToanGridVo: GridItem
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenGuid { get; set; }
        public long KichThuoc { get; set; }
        public string DuongDan { get; set; }
        public Enums.LoaiTapTin LoaiTapTin { get; set; }
        public string MoTa { get; set; }
    }
}
