using Camino.Core.Domain.ValueObject.Grid;
using static Camino.Core.Domain.Enums;
using Camino.Core.Helpers;
using System;
using Camino.Core.Domain.ValueObject.YeuCauMuaDuocPham;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using System.Collections.Generic;
using System.Linq;

namespace Camino.Core.Domain.ValueObject.BaoCaoKhamDoanHopDong
{
    public class BaoCaoKhamDoanHopDongTheoHopDongVo : GridItem
    {
        public string TenCongTyKhamSucKhoe { get; set; }
        public string TenHopDongKhamSucKhoe { get; set; }
        public int? SoLuongNhanVienTheoHopDong { get; set; }
        public int? SoLuongNhanVienThucTe { get; set; }
        public int? SoLuongNhanVienKhamThucTe { get; set; }
        public int? SoLuongNhanVienChuaKhamMotDichVu { get; set; }
        //public bool? CoNhanVienChuaKham { get; set; }
        //public bool? CoNhanVienDaKham { get; set; }
        //public DateTime ThoiDiemTiepNhan { get; set; }
        public DateTime NgayHopDong { get; set; }
        public bool? ChuaKham { get; set; }
        public bool? DaKham { get; set; }
        //public int? TinhTrang => CoNhanVienChuaKham == true ? 0 : (CoNhanVienDaKham == true ? 1 : 2);
        //public int? TinhTrang => SoLuongNhanVienChuaKhamDichVu > 0 ? 0 : 1;
        public string FromDate1 { get; set; }
        public string ToDate1 { get; set; }
        public string SearchString { get; set; }
        public RangeDates RangeFromDate { get; set; }
        public List<long> HopDongKhamSucKhoeNhanVienIds { get; set; }

    }
    public class HopDongKhamSucKhoeNhanVienQueryDataVo
    {
        public long HopDongKhamSucKhoeId { get; set; }
        public long HopDongKhamSucKhoeNhanVienId { get; set; }
        public List<HopDongKhamSucKhoeNhanVienYeuCauTiepNhanQueryDataVo> YeuCauTiepNhans { get; set; }

    }
    public class HopDongKhamSucKhoeNhanVienYeuCauTiepNhanQueryDataVo
    {
        public HopDongKhamSucKhoeNhanVienYeuCauTiepNhanQueryDataVo()
        {
            YeuCauKhams = new List<HopDongKhamSucKhoeNhanVienYeuCauKhamQueryDataVo>();
            YeuCauDichVuKyThuatVos = new List<HopDongKhamSucKhoeNhanVienYeuCauDichVuKyThuatQueryDataVo>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public List<HopDongKhamSucKhoeNhanVienYeuCauKhamQueryDataVo> YeuCauKhams { get; set; }
        public List<HopDongKhamSucKhoeNhanVienYeuCauDichVuKyThuatQueryDataVo> YeuCauDichVuKyThuatVos { get; set; }

    }
    public class HopDongKhamSucKhoeNhanVienYeuCauKhamQueryDataVo
    {
        public long YeuCauKhamBenhId { get; set; }
        public EnumTrangThaiYeuCauKhamBenh TrangThai { get; set; }
        public ChuyenKhoaKhamSucKhoe? ChuyenKhoaKhamSucKhoe { get; set; }
        public string TenDichVu { get; set; }
        public DateTime? ThoiDiemHoanThanh { get; set; }

        // Cập nhật 09/12/2022: cập nhật load chậm grid báo cáo
        public long? YeuCauTiepNhanId { get; set; }
        public string MaTN { get; set; }
        public DateTime? ThoiDiemTiepNhan { get; set; }
        public long? HopDongKhamSucKhoeId { get; set; }
        public long? HopDongKhamSucKhoeNhanVienId { get; set; }
    }

    public class HopDongKhamSucKhoeNhanVienYeuCauDichVuKyThuatQueryDataVo
    {
        public long YeuCauDichVuKyThuatId { get; set; }
        public LoaiDichVuKyThuat LoaiDichVuKyThuat { get; set; }
        public EnumTrangThaiYeuCauDichVuKyThuat TrangThai { get; set; }
        public bool CoKetQuaCDHA { get; set; }
        public bool DaDuyetXetNghiem { get; set; }
        public string TenDichVu { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public DateTime? ThoiDiemDuyetKetQua { get; set; }

        // Cập nhật 09/12/2022: cập nhật load chậm grid báo cáo
        public long? YeuCauTiepNhanId { get; set; }
        public string MaTN { get; set; }
        public DateTime? ThoiDiemTiepNhan { get; set; }
        public long? HopDongKhamSucKhoeId { get; set; }
        public long? HopDongKhamSucKhoeNhanVienId { get; set; }

    }
    public class HopDongKhamSucKhoeNhanVienYeuCauKhamVo : GridItem
    {
        public long HopDongKhamSucKhoeId { get; set; }
        public long YeuCauKhamBenhId { get; set; }
        public EnumTrangThaiYeuCauKhamBenh TrangThai { get; set; }
        public ChuyenKhoaKhamSucKhoe? ChuyenKhoaKhamSucKhoe { get; set; }
        public string TenDichVu { get; set; }
    }

    public class HopDongKhamSucKhoeNhanVienTheoDichVuVo
    {
        public long HopDongKhamSucKhoeNhanVienId { get; set; }
        public long HopDongKhamSucKhoeId { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public EnumTrangThaiYeuCauKhamBenh? TrangThaiKhamBenh { get; set; }
        public long YeuCauDichVuKyThuatId { get; set; }
        public LoaiDichVuKyThuat LoaiDichVuKyThuat { get; set; }
        public EnumTrangThaiYeuCauDichVuKyThuat TrangThaiKyThuat { get; set; }
        public ChuyenKhoaKhamSucKhoe? ChuyenKhoaKhamSucKhoe { get; set; }
        public bool CoKetQuaCDHA { get; set; }
        public bool DaDuyetXetNghiem { get; set; }
        public string TenDichVu { get; set; }
        public bool LaDichVuKhamBenh { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public DateTime? ThoiDiemHoanThanh { get; set; }
        public DateTime? ThoiDiemDuyetKetQua { get; set; }
    }

    public class BaoCaoKhamDoanHopDongTheoNhanVienVo : GridItem
    {
        public string MaBN { get; set; }
        public string MaNV { get; set; }
        public string MaTN { get; set; }
        public string HoTen { get; set; }
        public int? NamSinh { get; set; }
        public LoaiGioiTinh? GioiTinh { get; set; }
        public string GioiTinhDisplay => GioiTinh.GetDescription();
        public DateTime? ThoiDiemTiepNhan { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public string ThoiDiemThucHienDisplay => ThoiDiemThucHien?.ApplyFormatDateTime();
        public long? HopDongKhamSucKhoeId { get; set; }
        public DateTime? NgayHopDong { get; set; }
        public DateTime? NgayHoanThanhTatCa { get; set; }


        public int? Loai { get; set; }
        public bool? ChuaKhamNhanVien { get; set; }
        public bool? DangKhamNhanVien { get; set; }
        public bool? DaKhamNhanVien { get; set; }
        public int? TinhTrang => ChuaKhamNhanVien == true ? 0 : (DangKhamNhanVien == true ? 1 : DaKhamNhanVien == true ? 2 : 3);
        public string TenCongTyKhamSucKhoe { get; set; }
        public string DichVuKhamBenhChuaKham { get; set; }
        public string DichVuKyThuatChuaThucHien { get; set; }
        public string DichVuChuaKham => !string.IsNullOrEmpty(DichVuKhamBenhChuaKham) && !string.IsNullOrEmpty(DichVuKyThuatChuaThucHien) ? DichVuKhamBenhChuaKham + ", " + DichVuKyThuatChuaThucHien
                : (!string.IsNullOrEmpty(DichVuKhamBenhChuaKham) && string.IsNullOrEmpty(DichVuKyThuatChuaThucHien) ? DichVuKhamBenhChuaKham : (string.IsNullOrEmpty(DichVuKhamBenhChuaKham) && !string.IsNullOrEmpty(DichVuKyThuatChuaThucHien) ? DichVuKyThuatChuaThucHien : ""));

        //public string DichVuKhamBenhDangKham { get; set; }
        //public string DichVuKyThuatDangThucHien { get; set; }
        //public string DichVuDangKham => !string.IsNullOrEmpty(DichVuKhamBenhDangKham) && !string.IsNullOrEmpty(DichVuKyThuatDangThucHien) ? DichVuKhamBenhDangKham + ", " + DichVuKyThuatDangThucHien
        //        : (!string.IsNullOrEmpty(DichVuKhamBenhDangKham) && string.IsNullOrEmpty(DichVuKyThuatDangThucHien) ? DichVuKhamBenhDangKham : (string.IsNullOrEmpty(DichVuKhamBenhDangKham) && !string.IsNullOrEmpty(DichVuKyThuatDangThucHien) ? DichVuKyThuatDangThucHien : ""));

        public string DichVuKhamBenhDaKham { get; set; }
        public string DichVuKyThuatDaThucHien { get; set; }

        public string DichVuDaKham => !string.IsNullOrEmpty(DichVuKhamBenhDaKham) && !string.IsNullOrEmpty(DichVuKyThuatDaThucHien) ? DichVuKhamBenhDaKham + ", " + DichVuKyThuatDaThucHien
             : (!string.IsNullOrEmpty(DichVuKhamBenhDaKham) && string.IsNullOrEmpty(DichVuKyThuatDaThucHien) ? DichVuKhamBenhDaKham : (string.IsNullOrEmpty(DichVuKhamBenhDaKham) && !string.IsNullOrEmpty(DichVuKyThuatDaThucHien) ? DichVuKyThuatDaThucHien : ""));
        public string SearchString { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string FromDate1 { get; set; }
        public string ToDate1 { get; set; }
    }

    public class BaoCaoKhamDoanHopDongDichVuNgoaiVo : GridItem
    {
        public string MaNV { get; set; }
        public string MaBN { get; set; }
        public string MaTN { get; set; }
        public string HoTen { get; set; }
        public int? NamSinh { get; set; }
        public LoaiGioiTinh? GioiTinh { get; set; }
        public string GioiTinhDisplay => GioiTinh.GetDescription();
        public string TenDichVu { get; set; }
        public long? YeuCauTiepNhanId { get; set; }
        public int? SoLan { get; set; }
        public decimal? DonGiaBV { get; set; }
        public decimal? DonGiaMoi { get; set; }
        public decimal? SoTienMienGiam { get; set; }
        public decimal? SoTienThucThu => DonGiaMoi.GetValueOrDefault() * Convert.ToDecimal(SoLan) - SoTienMienGiam.GetValueOrDefault();
        public decimal? ThanhTienThucThu => SoTienThucThu.GetValueOrDefault();
        public decimal? SoTienCongNo { get; set; }
        public decimal? TongSoTienCongNo { get; set; }
        public DateTime? ThoiDiemTiepNhan { get; set; }
        public long? HopDongKhamSucKhoeId { get; set; }
        public long? CongTyKhamSucKhoeId { get; set; }
        public string TenCongTyKhamSucKhoe { get; set; }
        public string TenHopDongKhamSucKhoe { get; set; }
        public RangeDates RangeFromDate { get; set; }
        public bool? LaDichVuKhamBenh { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public DateTime? CreateOn { get; set; }
        public decimal? TongThanhTienThucThuTheoBenhNhan { get; set; }
    }

    public class BaoCaoKhamDoanHopDongDichVuNgoaiExportExcel
    {
        [Width(15)]

        public string MaNV { get; set; }
        [Width(15)]

        public string MaBN { get; set; }
        [Width(15)]

        public string MaTN { get; set; }
        [Width(15)]

        public string HoTen { get; set; }
        [Width(15)]

        public int? NamSinh { get; set; }
        [Width(15)]

        public string GioiTinhDisplay { get; set; }
        [Width(15)]

        public string TenDichVu { get; set; }
        [Width(15)]

        public int? SoLan { get; set; }
        public decimal? SoTienThucThu { get; set; }
        public decimal? SoTienCongNo { get; set; }
        [Width(15)]
        public decimal? DonGiaBV { get; set; }
        [Width(15)]
        public decimal? DonGiaTaiThoiDiemChiDinh { get; set; }
        [Width(15)]
        public decimal? ThanhTienThucThu => SoTienThucThu.GetValueOrDefault() * Convert.ToDecimal(SoLan);
        [Width(15)]
        public decimal? SoTienMienGiam { get; set; }

    }

    public class BaoCaoKhamDoanHopDongTheoNhanVienExportExcel
    {
        [Width(15)]

        public string MaBN { get; set; }
        [Width(15)]

        public string MaNV { get; set; }
        [Width(15)]

        public string MaTN { get; set; }
        [Width(15)]

        public string HoTen { get; set; }
        [Width(15)]

        public int? NamSinh { get; set; }
        [Width(15)]

        public string GioiTinhDisplay { get; set; }
        [Width(15)]

        public string DichVuDangKham { get; set; }
        [Width(15)]

        public string DichVuChuaKham { get; set; }
        [Width(15)]

        public string DichVuDaKham { get; set; }
        [Width(15)]

        public string TenCongTyKhamSucKhoe { get; set; }
        [Width(15)]

        public string ThoiDiemThucHienDisplay { get; set; }


    }

    public class InDanhSachNhanVien
    {
        public InDanhSachNhanVien()
        {
            Datas = new List<DanhSachNhanVienChiTiet>();
        }
        public int Loai { get; set; }
        public string TrangThai { get; set; }
        public string HostingName { get; set; }
        public List<DanhSachNhanVienChiTiet> Datas { get; set; }
    }

    public class DanhSachNhanVienChiTiet
    {
        public string MaNV { get; set; }
        public string MaBN { get; set; }
        public string MaTN { get; set; }
        public string HoTen { get; set; }
        public int? NamSinh { get; set; }
        public string GioiTinhDisplay { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }

    }
    public class DataNhanVienChiTiet
    {
        public string TrangThai { get; set; }
        public string NhanViens { get; set; }
        public string LogoUrl { get; set; }
    }

    public class NguoiBenhKhamDichVuTheoPhongQueryData
    {
        public long Id { get; set; }
        public long CongTyKhamSucKhoeId { get; set; }
        public long HopDongKhamSucKhoeId { get; set; }
        public string TenCongTyKhamSucKhoe { get; set; }
        public string TenHopDongKhamSucKhoe { get; set; }
        public long NoiThucHienId { get; set; }
        public string TenNoiThucHien { get; set; }
        public ChuyenKhoaKhamSucKhoe? ChuyenKhoaKhamSucKhoe { get; set; }
        public string TenChuyenKhoaKhamSucKhoe { get; set; }

    }

    public class NguoiBenhKhamDichVuTheoPhong : GridItem
    {
        public NguoiBenhKhamDichVuTheoPhong()
        {
            NoiThucHienCuaNguoiBenhs = new List<NoiThucHienCuaNguoiBenh>();
        }
        public long? CongTyKhamSucKhoeId { get; set; }
        public long? HopDongKhamSucKhoeId { get; set; }
        public string TenCongTyKhamSucKhoe { get; set; }
        public string TenHopDongKhamSucKhoe { get; set; }
        public int? TongSo => NoiThucHienCuaNguoiBenhs.Select(o => o.SoLan.GetValueOrDefault()).Sum();
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public List<NoiThucHienCuaNguoiBenh> NoiThucHienCuaNguoiBenhs { get; set; }
    }

    public class NoiThucHienCuaNguoiBenh : GridItem
    {
        public long NoiThucHienId { get; set; }
        public string TenNoiThucHien { get; set; }
        public int? SoLan { get; set; }
    }

    public class NoiThucHienCuaNguoiBenhTheoChuyenKhoa : GridItem
    {
        public ChuyenKhoaKhamSucKhoe? ChuyenKhoaKhamSucKhoe { get; set; }
        public string TenChuyenKhoaKhamSucKhoe { get; set; }
        public int? SoLan { get; set; }
    }

    public class NguoiBenhKhamDichVuTheoChuyenKhoa : GridItem
    {
        public NguoiBenhKhamDichVuTheoChuyenKhoa()
        {
            NoiThucHienCuaNguoiBenhs = new List<NoiThucHienCuaNguoiBenhTheoChuyenKhoa>();
        }
        public long? CongTyKhamSucKhoeId { get; set; }
        public long? HopDongKhamSucKhoeId { get; set; }
        public string TenCongTyKhamSucKhoe { get; set; }
        public string TenHopDongKhamSucKhoe { get; set; }
        public int? TongSo => NoiThucHienCuaNguoiBenhs.Select(o => o.SoLan.GetValueOrDefault()).Sum();
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public List<NoiThucHienCuaNguoiBenhTheoChuyenKhoa> NoiThucHienCuaNguoiBenhs { get; set; }
    }

    public class BaoCaoNguoiBenhKhamDichVuTheoPhongQueryInfo : QueryInfo
    {
        public long? CongTyKhamSucKhoeId { get; set; }
        public long? HopDongKhamSucKhoeId { get; set; }
        public string FromDateString { get; set; }
        public string ToDateString { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class DichVuKhamBenhTheoGoiKham
    {
        public string Ma { get; set; }
        public string TenDichVu { get; set; }
    }

    public class HopDongKhamSucKhoeNhanVienChiTietQueryDataVo
    {
        public HopDongKhamSucKhoeNhanVienChiTietQueryDataVo()
        {
            YeuCauTiepNhans = new List<HopDongKhamSucKhoeNhanVienChiTietYeuCauTiepNhanQueryDataVo>();
        }
        public long HopDongKhamSucKhoeId { get; set; }
        public long HopDongKhamSucKhoeNhanVienId { get; set; }
        public string MaBN { get; set; }
        public string MaNV { get; set; }
        public string HoTen { get; set; }
        public int? NamSinh { get; set; }
        public LoaiGioiTinh? GioiTinh { get; set; }
        public string GioiTinhDisplay => GioiTinh.GetDescription();
        public string TenCongTyKhamSucKhoe { get; set; }
        public List<HopDongKhamSucKhoeNhanVienChiTietYeuCauTiepNhanQueryDataVo> YeuCauTiepNhans { get; set; }

    }
    public class HopDongKhamSucKhoeNhanVienChiTietYeuCauTiepNhanQueryDataVo
    {
        public HopDongKhamSucKhoeNhanVienChiTietYeuCauTiepNhanQueryDataVo()
        {
            YeuCauKhams = new List<HopDongKhamSucKhoeNhanVienChiTietYeuCauKhamQueryDataVo>();
            YeuCauDichVuKyThuatVos = new List<HopDongKhamSucKhoeNhanVienChiTietYeuCauDichVuKyThuatQueryDataVo>();
        }

        public long YeuCauTiepNhanId { get; set; }
        public string DichVuKhamBenhChuaKham { get; set; }
        public string DichVuKhamBenhDaKham { get; set; }
        public string MaTN { get; set; }
        public DateTime? ThoiDiemTiepNhan { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public string ThoiDiemThucHienDisplay => ThoiDiemThucHien?.ApplyFormatDateTime();
        public List<HopDongKhamSucKhoeNhanVienChiTietYeuCauKhamQueryDataVo> YeuCauKhams { get; set; }
        public List<HopDongKhamSucKhoeNhanVienChiTietYeuCauDichVuKyThuatQueryDataVo> YeuCauDichVuKyThuatVos { get; set; }
    }

    public class YeuCauTiepNhanQueryDataVo
    {
        public YeuCauTiepNhanQueryDataVo()
        {
            YeuCauKhams = new List<HopDongKhamSucKhoeNhanVienChiTietYeuCauKhamQueryDataVo>();
            YeuCauDichVuKyThuatVos = new List<HopDongKhamSucKhoeNhanVienChiTietYeuCauDichVuKyThuatQueryDataVo>();
        }

        public long? HopDongKhamSucKhoeNhanVienId { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public long? BenhNhanId { get; set; }
        public string MaTN { get; set; }
        public string MaBN { get; set; }
        public string TenCongTyKhamSucKhoe { get; set; }
        public string MaNV { get; set; }
        public string HoTen { get; set; }
        public int? NamSinh { get; set; }
        public LoaiGioiTinh? GioiTinh { get; set; }
        public string GioiTinhDisplay => GioiTinh.GetDescription();
        public DateTime? ThoiDiemTiepNhan { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public string ThoiDiemThucHienDisplay => ThoiDiemThucHien?.ApplyFormatDateTime();
        public List<HopDongKhamSucKhoeNhanVienChiTietYeuCauKhamQueryDataVo> YeuCauKhams { get; set; }
        public List<HopDongKhamSucKhoeNhanVienChiTietYeuCauDichVuKyThuatQueryDataVo> YeuCauDichVuKyThuatVos { get; set; }
    }

    public class HopDongKhamSucKhoeNhanVienChiTietYeuCauKhamQueryDataVo
    {
        public long YeuCauKhamBenhId { get; set; }
        public EnumTrangThaiYeuCauKhamBenh TrangThai { get; set; }
        public ChuyenKhoaKhamSucKhoe? ChuyenKhoaKhamSucKhoe { get; set; }
        public string TenDichVu { get; set; }
        public DateTime? ThoiDiemHoanThanh { get; set; }

    }

    public class HopDongKhamSucKhoeNhanVienChiTietYeuCauDichVuKyThuatQueryDataVo
    {
        public long YeuCauDichVuKyThuatId { get; set; }
        public LoaiDichVuKyThuat LoaiDichVuKyThuat { get; set; }
        public EnumTrangThaiYeuCauDichVuKyThuat TrangThaiKyThuat { get; set; }
        public bool CoKetQuaCDHA { get; set; }
        public bool DaDuyetXetNghiem { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public string TenDichVu { get; set; }
    }
}
