using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Core.Helpers;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.DanhSachBenhNhanChoThuNgan
{
    public class DanhSachChuaQuyetToanMarketingGridVo : GridItem
    {
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public int? NamSinh { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string GioiTinhStr => GioiTinh?.GetDescription();
        public string SoDienThoai { get; set; }
        public string SoDienThoaiDisplay { get; set; }
        public string CMND { get; set; }
        public string DiaChi { get; set; }
        public bool ChuaThanhToan { get; set; }
        public bool Highlight { get; set; }
        public TrangThaiThanhToanMarketing TrangThai => ChuaThanhToan
            ? TrangThaiThanhToanMarketing.ChuaThanhToan
            : TrangThaiThanhToanMarketing.DangThanhToan;
        public string SearchString { get; set; }
        public string ToDate { get; set; }
        public string FromDate { get; set; }
    }



    public class ThongTinThuTienGoi
    {
        public long BenhNhanId { get; set; }
        public decimal TienMat { get; set; }
        public decimal ChuyenKhoan { get; set; }
        public decimal POS { get; set; }
        public DateTime NgayThu { get; set; }
        public string NoiDungThu { get; set; }
        public List<GoiChuaQuyetToanMarketingGridVo> GoiChuaQuyetToanMarketing { get; set; }
    }

    public class GoiChuaQuyetToanMarketingGridVo : GridItem
    {
        public string MaGoi { get; set; }
        public string TenGoi { get; set; }
        public DateTime ThoiDiemChiDinh { get; set; }
        public string NgayDang => ThoiDiemChiDinh.ApplyFormatDate();
        public decimal GiaGoi { get; set; }
        public decimal DaThu { get; set; }
        public decimal ChuaThu => GiaGoi - DaThu;
        public decimal TongDVDaDung { get; set; }
        public decimal SoTienThuLanNay { get; set; }
        public bool Highlight { get; set; }
        public bool? NgungSuDung { get; set; }
        public TrangThaiQuyetToan TrangThai { get; set; }
    }


    public class GoiDaQuyetToanMarketingGridVo : GridItem
    {
        public string MaGoi { get; set; }
        public string TenGoi { get; set; }
        public string NgayDang => ThoiDiemChiDinh.ApplyFormatDate();
        public DateTime ThoiDiemChiDinh { get; set; }
        public decimal GiaGoi { get; set; }
        public decimal DaThu { get; set; }
        public decimal ChuaThu => GiaGoi - DaThu;
        public decimal TongDVDaDung { get; set; }
        public decimal TraLaiBN { get; set; }
        public EnumTrangThaiYeuCauGoiDichVu TrangThai { get; set; }
        public string TrangThaiDisplayName => TrangThai.GetDescription();
    }

    public class ThongTinYeuCauGoiDichVu
    {
        public long BenhNhanId { get; set; }
        public long YeuCauGoiDichVuId { get; set; }
        public string TenChuongTrinhGoiDichVu { get; set; }
        public TrangThaiQuyetToanDichVu TrangThaiQuyetToanDichVu { get; set; }

        public decimal TongTienGoi { get; set; }
        public decimal TongTienDaThu { get; set; }
        public decimal TongTienDaDung { get; set; }
        public decimal SoTienTraLai { get; set; }
        public bool? DaQuyetToan { get; set; }
        public DateTime? ThoiDiemQuyetToan { get; set; }
        public bool HuyTrongNgay => DaQuyetToan == true && ThoiDiemQuyetToan != null && ThoiDiemQuyetToan.Value.Date == DateTime.Now.Date;
        public List<DanhSachDichVuTrongGoi> DanhSachDichVuTrongGois { get; set; }

        public bool? HuyGoi { get; set; }
        public string LyDoHuyGoi { get; set; }
    }

    public class HuyQuyetToanGoi
    {
        public long YeuCauGoiDichVuId { get; set; }
        public string LyDoHuyQuyetToan { get; set; }
    }

    public class DanhSachDichVuTrongGoi : GridItem
    {
        public string MaDichVu { get; set; }
        public EnumDichVuTongHop LoaiNhom { get; set; }
        public long DichVuBenhVienId { get; set; }
        public long NhomGiaDichVuId { get; set; }
        public string Nhom { get; set; }
        public long? NhomDichVuBenhVienId { get; set; }
        public string TenDichVu { get; set; }
        public string LoaiGia { get; set; }
        public double SoLuong { get; set; }
        public double SoLuongDaDung { get; set; }
        public double SoLuongChuaDung => SoLuong - SoLuongDaDung;
        public decimal DGBV { get; set; }
        public decimal DGTruocCK { get; set; }
        public decimal DGSauCK { get; set; }

        public decimal TTBV => DGBV * (decimal)SoLuong;
        public decimal TTTruocCK => DGTruocCK * (decimal)SoLuong;
        public decimal TTSauCK => DGSauCK * (decimal)SoLuong;
        public decimal TTBVChuaDung => DGBV * (decimal)SoLuongChuaDung;
        public decimal TTTruocCKChuaDung => DGTruocCK * (decimal)SoLuongChuaDung;
        public decimal TTSauCKChuaDung => DGSauCK * (decimal)SoLuongChuaDung;
        public decimal TTBVDaDung => DGBV * (decimal)SoLuongDaDung;
        public decimal TTTruocCKDaDung => DGTruocCK * (decimal)SoLuongDaDung;
        public decimal TTSauCKDaDung => DGSauCK * (decimal)SoLuongDaDung;
    }


    public class ThongTinPhieuThuGoiDichVuGridVo : GridItem
    {
        public string SoPhieuThu { get; set; }
        public LoaiPhieuThuChiThuNgan? LoaiPhieuThuChiThuNgan { get; set; }
        public IEnumerable<string> ThuCuaGoi { get; set; }
        public string NguoiThu { get; set; }
        public DateTime? NgayThu { get; set; }
        public string NgayThuDisplay => NgayThu?.ApplyFormatDateTimeSACH();

        public bool? ThuHoiPhieu { get; set; }
        public long? NguoiThuHoiId { get; set; }
        public string TenNguoiThuHoi { get; set; }

        public DateTime? ThoiGianThuHoi { get; set; }
        public string LyDo { get; set; }
        public bool DaHuy { get; set; }
        public bool HuyPhieuThu => NgayThu?.Date == DateTime.Now.Date && DaHuy == false;
        public decimal SoTienThu { get; set; }
    }

    public enum TrangThaiThanhToanMarketing
    {
        [Description("Chưa thanh toán")]
        ChuaThanhToan = 1,
        [Description("Đang thanh toán")]
        DangThanhToan = 2
    }
    public enum TrangThaiQuyetToanDichVu
    {
        [Description("Chưa Quyết toán")]
        ChuaQuyetToan = 1,
        [Description("Đã quyết toán")]
        DaQuyetToan = 2
    }
    public enum TrangThaiQuyetToan
    {
        [Description("Quyết toán")]
        QuyetToan = 1,
        [Description("Đã quyết toán")]
        DaQuyetToan = 2
    }
}