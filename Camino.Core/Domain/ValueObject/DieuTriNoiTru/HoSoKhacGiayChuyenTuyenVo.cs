using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.DieuTriNoiTru
{
    public class HoSoKhacGiayChuyenTuyenVo
    {
        public string SoHoSo { get; set; }
        public string SoChuyenTuyenSo { get; set; }
        public string NguoiNhan { get; set; }
        public string DauHieuLamSan { get; set; }
        public string KetQuaXetNghiemCLS { get; set; }
        public string ChanDoan { get; set; }
        public string PhuongPhapSuDungTrongDieuTri { get; set; }
        public string TinhTrangNguoiBenh { get; set; }
        public string HuongDieuTri { get; set; }
        public int? LyDoChuyenTuyen { get; set; }
        public DateTime? ChuyenTuyenHoi { get; set; }
        public string PhuongTienVanChuyen { get; set; }
        public string HoTenNguoiHoTong { get; set; }
        public string ChucDanhNguoiHoTong { get; set; }
        public string TrinhDoChuyenMonNguoiHoTong { get; set; }
        public DateTime? NgayThucHien { get; set; }
        public long? NguoiCoThamQuyenId { get; set; }
        public string NguoiCoThamQuyenDisplay { get; set; }
        public long? YBacSiKhamDieuTriId { get; set; }
        public string YBacSiKhamDieuTriDisplay { get; set; }
    }

    public class HoSoKhacGiayInChuyenTuyen
    {
        public string SoHoSo { get; set; }
        public string SoChuyenTuyenSo { get; set; }
        public string NguoiNhan { get; set; }
        public string HoTenBenhNhan { get; set; }
        public string GioiTinh { get; set; }
        public string Tuoi { get; set; }
        public string DiaChi { get; set; }
        public string DanToc { get; set; }
        public string QuocTich { get; set; }
        public string NgheNghiep { get; set; }
        public string NoiLamViec { get; set; }
        public string NgayHieuLucBHYT { get; set; }
        public string ThangHieuLucBHYT { get; set; }
        public string NamHieuLucBHYT { get; set; }
        public string NgayHetHanBHYT { get; set; }
        public string ThangHetHanBHYT { get; set; }
        public string NamHetHanBHYT { get; set; }
        public string SoThe1 { get; set; }
        public string SoThe2 { get; set; }
        public string SoThe3 { get; set; }
        public string SoThe4 { get; set; }
        public string SoThe5 { get; set; }
        public string DanhSachDuocKhamBenhDieuTri { get; set; }
        public string DauHieuLamSan { get; set; }
        public string KetQuaXetNghiemCLS { get; set; }
        public string ChanDoan { get; set; }
        public string PhuongPhapSuDungTrongDieuTri { get; set; }
        public string TinhTrangNguoiBenh { get; set; }
        public string HuongDieuTri { get; set; }
        public string ChuyenTuyenHoi { get; set; }
        public string PhuongTienVanChuyen { get; set; }
        public string NguoiHoTong { get; set; }
        public string LyDo1 { get; set; } //circle
        public string LyDo2 { get; set; } //circle
        public int Ngay { get; set; }
        public int Thang { get; set; }
        public int Nam { get; set; }
    }
}
