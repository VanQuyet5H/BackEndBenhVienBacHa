using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Camino.Core.Domain.ValueObject.Home
{

    public class ThongKeKhamBenhSearch
    {
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
    }

    public class ThongKeBenhVien
    {
        public ThongKeBenhVien()
        {
            ThongKeTiepNhan = new ThongKeTiepNhan();
            ThongKeKhamBenh = new ThongKeKhamBenh();
            ThongKeGiuongBenh = new ThongKeGiuongBenh();
            ThongKeNoiTru = new ThongKeNoiTru();
            ThongKeSoSinh = new ThongKeSoSinh();

            ChartTinhTrangKhams = new List<ChartTinhTrangKham>();
            ChartTinhTrangSuDungGiuongs = new List<ChartTinhTrangSuDungGiuong>();
            ChartTinhTrangDieuTriNoiTrus = new List<ChartTinhTrangDieuTriNoiTru>();
        }

        public ThongKeTiepNhan ThongKeTiepNhan { get; set; }
        public ThongKeKhamBenh ThongKeKhamBenh { get; set; }
        public ThongKeGiuongBenh ThongKeGiuongBenh { get; set; }
        public ThongKeNoiTru ThongKeNoiTru { get; set; }
        public ThongKeSoSinh ThongKeSoSinh { get; set; }

        public List<ChartTinhTrangKham> ChartTinhTrangKhams { get; set; }
        public List<ChartTinhTrangSuDungGiuong> ChartTinhTrangSuDungGiuongs { get; set; }
        public List<ChartTinhTrangDieuTriNoiTru> ChartTinhTrangDieuTriNoiTrus { get; set; }
    }

    public class DataTiepNhan
    {
        public long Id { get; set; }
        public bool CoBHYT { get; set; }
        public bool CoKhamBenh => TrangThaiYeuCauKhamBenhs?.Any(o => o != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham) ?? false;
        public bool CoKSK { get; set; }
        public List<Enums.EnumTrangThaiYeuCauKhamBenh> TrangThaiYeuCauKhamBenhs { get; set; }
    }

    public class ThongKeTiepNhan
    {
        public int TongSoTiepNhan { get; set; }
        public int TongSoBHYT { get; set; }
        public int TongSoDichVu { get; set; }
        public int TongSoVienPhi { get; set; }
        public int TongSoKSK { get; set; }
    }
    public class DataKhamBenh
    {
        public long Id { get; set; }
        public Enums.EnumTrangThaiYeuCauKhamBenh TrangThai { get; set; }
        public long DichVuKhamBenhBenhVienId { get; set; }
        public string TenDichVu { get; set; }
    }

    public class ThongKeKhamBenh
    {
        public int TongSoNguoiKhamBenh { get; set; }
        public int TongSoChoKham { get; set; }
        public int TongSoDangKham { get; set; }
        public int TongSoHoanThanh { get; set; }
    }
    public class DataGiuongBenh
    {
        public long Id { get; set; }
        public bool CoSuDung { get; set; }
        public long KhoaPhongId { get; set; }
        public string TenKhoaPhong { get; set; }
    }

    public class ThongKeGiuongBenh
    {
        public int TongSoGiuongBenh { get; set; }
        public int TongSoDaDung { get; set; }
        public int TongSoTrong { get; set; }
    }
    public class DataNoiTru
    {
        public long Id { get; set; }
        public DateTime ThoiDiemNhapVien { get; set; }
        public DateTime? ThoiDiemRaVien { get; set; }
        public Enums.EnumHinhThucRaVien? HinhThucRaVien { get; set; }
        public DataNoiTruKhoaPhongDieuTri DataNoiTruKhoaPhongDangDieuTri { get; set; }
        public List<DataNoiTruKhoaPhongDieuTri> KhoaPhongDieuTris { get; set; }
    }

    public class DataSoSinh
    {
        public long Id { get; set; }
        public DateTime ThoiDiemNhapVien { get; set; }
        public DateTime? ThoiDiemRaVien { get; set; }
        public Enums.EnumHinhThucRaVien? HinhThucRaVien { get; set; }
        public DataNoiTruKhoaPhongDieuTri DataNoiTruKhoaPhongDangDieuTri { get; set; }
        public List<DataNoiTruKhoaPhongDieuTri> KhoaPhongDieuTris { get; set; }
        public List<DataThoiGianDieuTriBenhAnSoSinh> ThoiGianDieuTriBenhAnSoSinhs { get; set; }
    }
    public class DataThoiGianDieuTriBenhAnSoSinh
    {
        public DateTime NgayDieuTri { get; set; }
        public int? GioBatDau { get; set; }
        public int? GioKetThuc { get; set; }
    }
    public class DataNoiTruKhoaPhongDieuTri
    {
        public long KhoaPhongId { get; set; }
        public DateTime ThoiDiemVaoKhoa { get; set; }
        public string TenKhoaPhong { get; set; }
    }
    public class ThongKeNoiTru
    {
        public int TongSoNhapVien { get; set; }
        public int TongChuyenVien { get; set; }
        public int TongSoDieuTri { get; set; }
        public int TongSoTuVong { get; set; }
        public int TongSoRaVien { get; set; }
    }

    public class ThongKeSoSinh
    {
        public int TongSoSSThuong  { get; set; }
        public int TongSoSSBenh  { get; set; }
        public int TongSoRaVien { get; set; }
        public int TongSoChuyenVien { get; set; }
        public int TongSoTuVong { get; set; }
    }

    public class ChartTinhTrangKham
    {
        public string KhoaKham { get; set; }
        public int TongSoNguoiChuaKham { get; set; }
        public int TongSoNguoiDangKham { get; set; }
        public int TongSoNguoiHoanThanh { get; set; }
    }

    public class ChartTinhTrangSuDungGiuong
    {
        public string KhoaKham { get; set; }
        public int TongSoGiuongTrong { get; set; }
        public int TongSoGiuongDaSuDung { get; set; }
    }

    public class ChartTinhTrangDieuTriNoiTru
    {
        public string KhoaKham { get; set; }
        public int TongSoNhapVien { get; set; }
        public int TongSoDangDieuTri { get; set; }
        public int TongSoDaRaVien { get; set; }
        public int TongSoChuyenVien { get; set; }
        public int TongSoTuVong { get; set; }
    }
}
